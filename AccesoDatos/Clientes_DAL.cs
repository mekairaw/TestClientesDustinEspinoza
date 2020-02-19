using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Entidades;

namespace AccesoDatos
{
    public class Clientes_DAL
    {
        public string Clientes_Acciones(string cadenaConexion, char tipoAccion, Clientes cliente = null, int ID = 0)
        {
            //metodo para las acciones de Ingresar, Actualizar y Borrar clientes
            SqlConnection objSqlConnection = new SqlConnection();
            SqlCommand objSqlCommand = new SqlCommand();
            objSqlConnection.ConnectionString = cadenaConexion;
            objSqlCommand.CommandType = CommandType.StoredProcedure;
            objSqlCommand.CommandText = "STPR_CLIENTES_PRUEBA_MANTENIMIENTO";
            try
            {
                //asignaciond e parametros genericos para cada accion
                objSqlCommand.Parameters.AddWithValue("@P_Accion", tipoAccion);
                objSqlCommand.Parameters.Add("@P_Mensaje", SqlDbType.VarChar, 100);
                objSqlCommand.Parameters["@P_Mensaje"].Direction = ParameterDirection.Output;
                if(cliente == null && tipoAccion == 'B')
                {
                    //asignacion de parametros en el caso que la accion sea borrar
                    objSqlCommand.Parameters.AddWithValue("@P_ID", ID);
                    objSqlCommand.Parameters.Add("@P_NombreCompleto", SqlDbType.VarChar).Value = DBNull.Value;
                    objSqlCommand.Parameters.Add("@P_Identificacion", SqlDbType.Float).Value = DBNull.Value;
                    objSqlCommand.Parameters.Add("@P_Telefono", SqlDbType.Float).Value = DBNull.Value;
                }
                else
                {
                    if(cliente != null && tipoAccion == 'A')
                    {
                        //asignacion de parametros especiales para actualizar, especificamente el ID del cliente.
                        objSqlCommand.Parameters.AddWithValue("@P_ID", ID);
                    }
                    else
                    {
                        objSqlCommand.Parameters.Add("@P_ID", SqlDbType.BigInt).Value = DBNull.Value;
                    }
                    //asignacion de parametros utilizados tanto en el ingreso como en la actualizacion de datos de un cliente
                    objSqlCommand.Parameters.AddWithValue("@P_NombreCompleto", cliente.NOMBRE_COMPLETO);
                    objSqlCommand.Parameters.AddWithValue("@P_Identificacion", cliente.IDENTIFICACION);
                    objSqlCommand.Parameters.AddWithValue("@P_Telefono", cliente.TELEFONO);
                }
                objSqlConnection.Open();
                objSqlCommand.Connection = objSqlConnection;
                objSqlCommand.ExecuteNonQuery();
                //en dado caso se necesite utiilzar el mensaje devuelto por la base de datos
                string output = objSqlCommand.Parameters["@P_Mensaje"].Value.ToString();
                return output;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {

                if (objSqlConnection.State != ConnectionState.Closed)
                    objSqlConnection.Close();
            }
        }
        public List<Clientes> Clientes_Leer_Busqueda(string cadenaConexion, string busqueda = null)
        {
            //metodo utilizado con la funcion de busqueda realizada, tambien trae la lista completa de clientes en caso que no se defina un termino de busqueda
            SqlConnection objSqlConnection = new SqlConnection();
            SqlCommand objSqlCommand = new SqlCommand();
            objSqlConnection.ConnectionString = cadenaConexion;
            objSqlCommand.CommandText = "SELECT * FROM dbo.FTN_CLIENTES_PRUEBA_LISTA_CLIENTES(@P_BUSQUEDA)";
            if(busqueda == null)
            {
                objSqlCommand.Parameters.Add("@P_BUSQUEDA", SqlDbType.NVarChar).Value = DBNull.Value;
            }
            else
            {
                objSqlCommand.Parameters.AddWithValue("@P_BUSQUEDA", busqueda);
            }
            objSqlCommand.Connection = objSqlConnection;
            SqlDataAdapter objSqlAdapter = new SqlDataAdapter(objSqlCommand);
            DataTable dt = new DataTable();
            objSqlAdapter.Fill(dt);
            List<Clientes> listaClientes = new List<Clientes>();
            //se realiza el parseo de data table a lista para facilidad de uso en el grid view
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                Clientes cliente = new Clientes();
                cliente.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                cliente.NOMBRE_COMPLETO = dt.Rows[i]["NOMBRE_COMPLETO"].ToString();
                cliente.IDENTIFICACION = Convert.ToDouble(dt.Rows[i]["IDENTIFICACION"]);
                cliente.TELEFONO = Convert.ToDouble(dt.Rows[i]["TELEFONO"]);
                listaClientes.Add(cliente);
            }

            return listaClientes;
        }
        public DataTable Clientes_Reporte(string cadenaConexion, string busqueda = null, int ID = 0, double identificacion = 0)
        {
            //metodo para obtener la informacion del reporte
            SqlConnection objSqlConnection = new SqlConnection();
            SqlCommand objSqlCommand = new SqlCommand();
            objSqlConnection.ConnectionString = cadenaConexion;
            objSqlCommand.CommandText = "SELECT * FROM dbo.FTN_CLIENTES_PRUEBA_LISTA_REPORTE(@P_BUSQUEDA, @P_ID, @P_IDENTIFICACION)";
            //en el caso que se este trayendo la lista filtrada
            if (busqueda == null)
            {
                objSqlCommand.Parameters.Add("@P_BUSQUEDA", SqlDbType.NVarChar).Value = DBNull.Value;
            }
            else
            {
                objSqlCommand.Parameters.AddWithValue("@P_BUSQUEDA", busqueda);
            }
            //en el caso que se este trayendo un cliente especifico ya existente en la base de datos
            if (ID == 0)
            {
                objSqlCommand.Parameters.Add("@P_ID", SqlDbType.BigInt).Value = DBNull.Value;
            }
            else
            {
                objSqlCommand.Parameters.AddWithValue("@P_ID", ID);
            }
            //para el caso de traer el reporte de un cliente recien creado en la base de datos
            if(identificacion == 0)
            {
                objSqlCommand.Parameters.Add("@P_IDENTIFICACION", SqlDbType.Float).Value = DBNull.Value;
            }
            else
            {
                objSqlCommand.Parameters.Add("@P_IDENTIFICACION", SqlDbType.Decimal).Value = Convert.ToDecimal(identificacion);
            }
            objSqlCommand.Connection = objSqlConnection;
            SqlDataAdapter objSqlAdapter = new SqlDataAdapter(objSqlCommand);
            DataTable dt = new DataTable();
            objSqlAdapter.Fill(dt);
            return dt;
        }
        public Clientes Cliente_Especifico(string cadenaCOnexion, int ID)
        {
            //metodo para obtener el cliente especifico, para ser mas facil la asignacion de los datos al formulario de mantenimiento de clientes a la hora de actualizar
            Clientes cliente = new Clientes();
            DataTable dt = Clientes_Reporte(cadenaCOnexion, null, ID);
            cliente.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
            cliente.NOMBRE_COMPLETO = dt.Rows[0]["NOMBRE_COMPLETO"].ToString();
            cliente.IDENTIFICACION = Convert.ToDouble(dt.Rows[0]["IDENTIFICACION"]);
            cliente.TELEFONO = Convert.ToDouble(dt.Rows[0]["TELEFONO"]);
            return cliente;
        }
    }
}
