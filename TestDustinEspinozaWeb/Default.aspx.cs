using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AccesoDatos;
using Entidades;

namespace TestDustinEspinozaWeb
{
    public partial class Default : System.Web.UI.Page
    {
        public List<Clientes> lst_clientes { get { return ViewState["lst_clientes"] as List<Clientes>; } set { ViewState["lst_clientes"] = value; } }
        protected void Page_Load(object sender, EventArgs e)
        {
            //Limpira las sesiones para evitar posibles errores a la hora de la generacion del reporte.
            Session["identificacion"] = null;
            Session["ID"] = null;
            if (!Page.IsPostBack)
            {
                CargarLista();
            }
        }
        protected void CargarLista()
        {
            //Carga de lista inicialmente con la cadena de conexion almacenada en el web.config
            string CadenaConexion = "";
            CadenaConexion = ConfigurationManager.ConnectionStrings["ConexionPrincipal"].ConnectionString;
            lst_clientes = new Clientes_DAL().Clientes_Leer_Busqueda(CadenaConexion);
            LlenarGrid();
        }
        protected void LlenarGrid()
        {
            //metodo generico para llenar el grid con los registros en la lista.
            try
            {
                grv_clientes.DataSource = lst_clientes;
                grv_clientes.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void grv_clientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //distintas acciones que puedo ejecutar a partir de mi grid view, como la de seleccionar y eliminar.
            string ID = "";
            if (e.CommandName == "eliminar")
            {
                ID = e.CommandArgument.ToString();
                txtID.Text = ID;
                //comando para ejecutar scripts JS del lado del cliente desde el servidor, usados principalmente para la apertura de los Modal de Bootstrap.
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DeleteModal", "openDeleteModal();", true);
            }
            if (e.CommandName == "seleccionar")
            {
                ID = e.CommandArgument.ToString();
                Session["ID"] = ID;
                asignar_valores(Convert.ToInt32(ID));
                //deshabilito el campo de texto para la identificacion
                txtIdentificacion.Enabled = false;
                Session["Guardado"] = 1;
                //hago visible el boton para imprimir datos cuando selecciono un cliente especifico
                btnImprimirSeleccionar.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "openEditModal();", true);
            }
        }

        protected void grv_clientes_PageIndexChanged(object sender, EventArgs e)
        {
            //para rellenar el grid en el dado caso que se cambie de pagina del grid view
            LlenarGrid();
        }

        protected void grv_clientes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //valido si la lista de clientes no es nula a partir del view state, si no lo es, lleno nuevamente el grid con esta lista.
            if (ViewState["lst_clientes"] != null)
            {
                List<Clientes> lst_cli = (List<Clientes>)ViewState["lst_clientes"];
                grv_clientes.PageIndex = e.NewPageIndex;
                ViewState["lst_clientes"] = lst_cli;
                grv_clientes.DataSource = lst_cli;
                grv_clientes.DataBind();
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string busqueda = txtBusqueda.Text.ToString();
            string CadenaConexion = "";
            CadenaConexion = ConfigurationManager.ConnectionStrings["ConexionPrincipal"].ConnectionString;
            Clientes_DAL clienteDAL = new Clientes_DAL();
            //verifico que mi string no sea nulo, si es nulo mando a traer la lista completa, en caso que no simplemente filtro por resultados parecidos al escrito en la base de datos"
            if (string.IsNullOrEmpty(busqueda) == false)
            {
                lst_clientes = clienteDAL.Clientes_Leer_Busqueda(CadenaConexion, busqueda);
            }
            else
            {
                lst_clientes = clienteDAL.Clientes_Leer_Busqueda(CadenaConexion);
            }
            LlenarGrid();
            Session["busqueda"] = busqueda;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            //realizo la validacion de nombre comppleto y de telefono en ambos escenarios antes de proceder al guardado, si vienen validos, prosigo.
            if(ValidarNombreCompleto() == false && ValidarTelefono() == false)
            {
                string CadenaConexion = "";
                CadenaConexion = ConfigurationManager.ConnectionStrings["ConexionPrincipal"].ConnectionString;
                Clientes_DAL clienteDAL = new Clientes_DAL();
                Clientes cliente = new Clientes();
                cliente.IDENTIFICACION = Convert.ToDouble(txtIdentificacion.Text.ToString());
                cliente.NOMBRE_COMPLETO = txtNombreCompleto.Text.ToString();
                cliente.TELEFONO = Convert.ToDouble(txtTelefono.Text.ToString());

                //sesion de guardado para conocer cuando estoy actualizando o ingresando
                if (Convert.ToInt32(Session["Guardado"]) == 1)
                {
                    cliente.ID = Convert.ToInt32(txtID.Text.ToString());
                    clienteDAL.Clientes_Acciones(CadenaConexion, 'A', cliente, cliente.ID);
                    lblSuccess.Text = "Registro actualizado satisfactoriamente.";
                }
                //valido si la identificacione es correcta a la hora de crear un nuevo registro, dado que no necesito validarla en la accion de actualizar porque no es editable.
                else if (Convert.ToInt32(Session["Guardado"]) == 0 && ValidarIdentificacion() == false)
                {
                    clienteDAL.Clientes_Acciones(CadenaConexion, 'I', cliente);
                    Session["identificacion"] = txtIdentificacion.Text.ToString();
                    lblSuccess.Text = "Registro guardado satisfactoriamente.";
                }
                txtID.Text = "";
                CargarLista();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SuccessModal", "openSuccessModal();", true);
                //a la hora de guardar ejecuto un script en el servidor para abrir una nueva ventana que me lleva al reporte que me mostrara al cliente recien creado.
                string script = "window.open('Report.aspx','_blank');";
                ClientScript.RegisterStartupScript(GetType(), "Navigation", script, true);
            }
            else
            {
                //script para reabrir el modal cuando sea cerrado por el postback del sitio.
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "openEditModal();", true);
            }
        }

        protected void asignar_valores(int ID)
        {
            //a la hora de seleccionar un elemento de mi grid view, asigno los valores antes de abrir el Modal de mantenimiento
            string CadenaConexion = "";
            CadenaConexion = ConfigurationManager.ConnectionStrings["ConexionPrincipal"].ConnectionString;
            Clientes_DAL clienteDAL = new Clientes_DAL();

            try
            {
                Clientes cliente = clienteDAL.Cliente_Especifico(CadenaConexion, ID);
                if (cliente != null)
                {
                    txtIdentificacion.Text = cliente.IDENTIFICACION.ToString();
                    txtNombreCompleto.Text = cliente.NOMBRE_COMPLETO;
                    txtTelefono.Text = cliente.TELEFONO.ToString();
                    txtID.Text = cliente.ID.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            //asigno seccion de guardado a la hora de hacer click previo a abrir el Modal.
            Session["Guardado"] = 0;
            txtIdentificacion.Enabled = true;
            txtIdentificacion.Text = "";
            //oculto el boton de imprimir porque esta es accion de crear y no de actualizar un cliente.
            btnImprimirSeleccionar.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "openEditModal();", true);
        }

        protected void btnConfirmarBorrar_Click(object sender, EventArgs e)
        {
            //accion que me abre el modal para confirmar el borrado de un cliente.
            string CadenaConexion = "";
            CadenaConexion = ConfigurationManager.ConnectionStrings["ConexionPrincipal"].ConnectionString;
            Clientes_DAL clienteDAL = new Clientes_DAL();
            clienteDAL.Clientes_Acciones(CadenaConexion, 'B', null, Convert.ToInt32(txtID.Text.ToString()));
            txtID.Text = "";
            CargarLista();
        }

        protected bool ValidarIdentificacion()
        {
            //metodo para validar la identificacion ingresada previo a guardar
            string CadenaConexion = "";
            CadenaConexion = ConfigurationManager.ConnectionStrings["ConexionPrincipal"].ConnectionString;
            Clientes_DAL clienteDAL = new Clientes_DAL();
            //valido si tiene espacios en blanco o si viene nulo
            if (string.IsNullOrEmpty(txtIdentificacion.Text.ToString()) == true || string.IsNullOrWhiteSpace(txtIdentificacion.Text.ToString()) == true)
            {
                txtIdentificacionError.Text = "Este campo no puede estar vacío.";
                txtIdentificacionError.Visible = true;
                return true;
            }
            //valido si el numero de caracteres ingresados es mayor a 10 a como es su equivalente en la base de datos.
            if (txtIdentificacion.Text.ToString().Length > 10)
            {
                txtIdentificacionError.Text = "Este campo solo puede tener 10 caracteres.";
                txtIdentificacionError.Visible = true;
                return true;
            }
            //verifico si en los registros existentes ya hay un registro con esta identificacion
            List<Clientes> clientes = clienteDAL.Clientes_Leer_Busqueda(CadenaConexion);
            for (int i = 0; i < clientes.Count; i++)
            {
                if (clientes[i].IDENTIFICACION == Convert.ToDouble(txtIdentificacion.Text.ToString()))
                {
                    txtIdentificacionError.Text = "Esta identificación ya existe.";
                    txtIdentificacionError.Visible = true;
                    return true;
                }
            }
            txtIdentificacionError.Text = "";
            txtIdentificacionError.Visible = false;
            return false;
        }
        protected bool ValidarNombreCompleto()
        {
            //verifico si el nombre ingresado contiene espacios en blanco o es nulo
            if (string.IsNullOrEmpty(txtNombreCompleto.Text.ToString()) == true || string.IsNullOrWhiteSpace(txtNombreCompleto.Text.ToString()) == true)
            {
                txtNombreCompletoError.Text = "Este campo no puede estar vacío.";
                txtNombreCompletoError.Visible = true;
                return true;
            }
            //verifica si el nombre completo tiene mas de 200 caracteres
            if (txtNombreCompleto.Text.ToString().Length > 200)
            {
                txtNombreCompletoError.Text = "Este campo solo puede tener 200 caracteres.";
                txtNombreCompletoError.Visible = true;
                return true;
            }
            txtNombreCompletoError.Text = "";
            txtNombreCompletoError.Visible = false;
            return false;
        }
        protected bool ValidarTelefono() 
        {
            //valido si el numero de telefono esta vacio o es nulo
            if (string.IsNullOrEmpty(txtTelefono.Text.ToString()) == true || string.IsNullOrWhiteSpace(txtTelefono.Text.ToString()) == true)
            {
                txtTelefonoError.Text = "Este campo no puede estar vacío.";
                txtTelefonoError.Visible = true;
                return true;
            }
            //valido si el numero de caracteres ingresados es mayor a 10
            if (txtTelefono.Text.ToString().Length > 10)
            {
                txtTelefonoError.Text = "Este campo solo puede tener 10 caracteres.";
                txtTelefonoError.Visible = true;
                return true;
            }
            txtTelefonoError.Text = "";
            txtTelefonoError.Visible = false;
            return false;
        }
    }
}