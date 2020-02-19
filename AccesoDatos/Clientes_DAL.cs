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
            SqlConnection objSqlConnection = new SqlConnection();
            SqlCommand objSqlCommand = new SqlCommand();
            objSqlConnection.ConnectionString = cadenaConexion;
            objSqlCommand.CommandType = CommandType.StoredProcedure;
            objSqlCommand.CommandText = "STPR_CLIENTES_PRUEBA_MANTENIMIENTO";
            try
            {
                objSqlCommand.Parameters.AddWithValue("@P_Accion", tipoAccion);
                objSqlCommand.Parameters.Add("@P_Mensaje", SqlDbType.VarChar, 100);
                objSqlCommand.Parameters["@P_Mensaje"].Direction = ParameterDirection.Output;
                if(cliente == null && tipoAccion == 'B')
                {
                    objSqlCommand.Parameters.AddWithValue("@P_ID", ID);
                    objSqlCommand.Parameters.Add("@P_NombreCompleto", SqlDbType.VarChar).Value = DBNull.Value;
                    objSqlCommand.Parameters.Add("@P_Identificacion", SqlDbType.Float).Value = DBNull.Value;
                    objSqlCommand.Parameters.Add("@P_Telefono", SqlDbType.Float).Value = DBNull.Value;
                }
                else
                {
                    if(cliente != null && tipoAccion == 'A')
                    {
                        objSqlCommand.Parameters.AddWithValue("@P_ID", ID);
                    }
                    else
                    {
                        objSqlCommand.Parameters.Add("@P_ID", SqlDbType.BigInt).Value = DBNull.Value;
                    }
                    objSqlCommand.Parameters.AddWithValue("@P_NombreCompleto", cliente.NOMBRE_COMPLETO);
                    objSqlCommand.Parameters.AddWithValue("@P_Identificacion", cliente.IDENTIFICACION);
                    objSqlCommand.Parameters.AddWithValue("@P_Telefono", cliente.TELEFONO);
                }
                objSqlConnection.Open();
                objSqlCommand.Connection = objSqlConnection;
                objSqlCommand.ExecuteNonQuery();
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
            SqlConnection objSqlConnection = new SqlConnection();
            SqlCommand objSqlCommand = new SqlCommand();
            objSqlConnection.ConnectionString = cadenaConexion;
            objSqlCommand.CommandText = "SELECT * FROM dbo.FTN_CLIENTES_PRUEBA_LISTA_REPORTE(@P_BUSQUEDA, @P_ID, @P_IDENTIFICACION)";
            if (busqueda == null)
            {
                objSqlCommand.Parameters.Add("@P_BUSQUEDA", SqlDbType.NVarChar).Value = DBNull.Value;
            }
            else
            {
                objSqlCommand.Parameters.AddWithValue("@P_BUSQUEDA", busqueda);
            }
            if (ID == 0)
            {
                objSqlCommand.Parameters.Add("@P_ID", SqlDbType.BigInt).Value = DBNull.Value;
            }
            else
            {
                objSqlCommand.Parameters.AddWithValue("@P_ID", ID);
            }
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
