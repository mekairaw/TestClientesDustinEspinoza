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
            Session["identificacion"] = null;
            Session["ID"] = null;
            if (!Page.IsPostBack)
            {
                CargarLista();
            }
        }
        protected void CargarLista()
        {
            string CadenaConexion = "";
            CadenaConexion = ConfigurationManager.ConnectionStrings["ConexionPrincipal"].ConnectionString;
            lst_clientes = new Clientes_DAL().Clientes_Leer_Busqueda(CadenaConexion);
            LlenarGrid();
        }
        protected void LlenarGrid()
        {
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
            string ID = "";
            if (e.CommandName == "eliminar")
            {
                ID = e.CommandArgument.ToString();
                txtID.Text = ID;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "DeleteModal", "openDeleteModal();", true);
            }
            if (e.CommandName == "seleccionar")
            {
                ID = e.CommandArgument.ToString();
                Session["ID"] = ID;
                asignar_valores(Convert.ToInt32(ID));
                txtIdentificacion.Enabled = false;
                Session["Guardado"] = 1;
                btnImprimirSeleccionar.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "openEditModal();", true);
            }
        }

        protected void grv_clientes_PageIndexChanged(object sender, EventArgs e)
        {
            LlenarGrid();
        }

        protected void grv_clientes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
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
            if(ValidarNombreCompleto() == false && ValidarTelefono() == false)
            {
                string CadenaConexion = "";
                CadenaConexion = ConfigurationManager.ConnectionStrings["ConexionPrincipal"].ConnectionString;
                Clientes_DAL clienteDAL = new Clientes_DAL();
                Clientes cliente = new Clientes();
                cliente.IDENTIFICACION = Convert.ToDouble(txtIdentificacion.Text.ToString());
                cliente.NOMBRE_COMPLETO = txtNombreCompleto.Text.ToString();
                cliente.TELEFONO = Convert.ToDouble(txtTelefono.Text.ToString());

                if (Convert.ToInt32(Session["Guardado"]) == 1)
                {
                    cliente.ID = Convert.ToInt32(txtID.Text.ToString());
                    clienteDAL.Clientes_Acciones(CadenaConexion, 'A', cliente, cliente.ID);
                    lblSuccess.Text = "Registro actualizado satisfactoriamente.";
                }
                else if (Convert.ToInt32(Session["Guardado"]) == 0 && ValidarIdentificacion() == false)
                {
                    clienteDAL.Clientes_Acciones(CadenaConexion, 'I', cliente);
                    Session["identificacion"] = txtIdentificacion.Text.ToString();
                    lblSuccess.Text = "Registro guardado satisfactoriamente.";
                }
                txtID.Text = "";
                CargarLista();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SuccessModal", "openSuccessModal();", true);
                string script = "window.open('Report.aspx','_blank');";
                ClientScript.RegisterStartupScript(GetType(), "Navigation", script, true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "openEditModal();", true);
            }
        }

        protected void asignar_valores(int ID)
        {
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
            Session["Guardado"] = 0;
            txtIdentificacion.Enabled = true;
            txtIdentificacion.Text = "";
            btnImprimirSeleccionar.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "openEditModal();", true);
        }

        protected void btnConfirmarBorrar_Click(object sender, EventArgs e)
        {
            string CadenaConexion = "";
            CadenaConexion = ConfigurationManager.ConnectionStrings["ConexionPrincipal"].ConnectionString;
            Clientes_DAL clienteDAL = new Clientes_DAL();
            clienteDAL.Clientes_Acciones(CadenaConexion, 'B', null, Convert.ToInt32(txtID.Text.ToString()));
            txtID.Text = "";
            CargarLista();
        }

        protected bool ValidarIdentificacion()
        {
            string CadenaConexion = "";
            CadenaConexion = ConfigurationManager.ConnectionStrings["ConexionPrincipal"].ConnectionString;
            Clientes_DAL clienteDAL = new Clientes_DAL();
            if (string.IsNullOrEmpty(txtIdentificacion.Text.ToString()) == true || string.IsNullOrWhiteSpace(txtIdentificacion.Text.ToString()) == true)
            {
                txtIdentificacionError.Text = "Este campo no puede estar vacío.";
                txtIdentificacionError.Visible = true;
                return true;
            }
            if (txtIdentificacion.Text.ToString().Length > 10)
            {
                txtIdentificacionError.Text = "Este campo solo puede tener 10 caracteres.";
                txtIdentificacionError.Visible = true;
                return true;
            }
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
            if (string.IsNullOrEmpty(txtNombreCompleto.Text.ToString()) == true || string.IsNullOrWhiteSpace(txtNombreCompleto.Text.ToString()) == true)
            {
                txtNombreCompletoError.Text = "Este campo no puede estar vacío.";
                txtNombreCompletoError.Visible = true;
                return true;
            }
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
            if (string.IsNullOrEmpty(txtTelefono.Text.ToString()) == true || string.IsNullOrWhiteSpace(txtTelefono.Text.ToString()) == true)
            {
                txtTelefonoError.Text = "Este campo no puede estar vacío.";
                txtTelefonoError.Visible = true;
                return true;
            }
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