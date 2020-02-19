using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AccesoDatos;
using Microsoft.Reporting.WebForms;

namespace TestDustinEspinozaWeb
{
    public partial class Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string CadenaConexion = "";
                CadenaConexion = ConfigurationManager.ConnectionStrings["ConexionPrincipal"].ConnectionString;
                Clientes_DAL clienteDAL = new Clientes_DAL();
                DataTable dt = new DataTable();
                ReportParameter p;
                if (Session["ID"] != null && Session["ID"].ToString() != "")
                {
                    dt = clienteDAL.Clientes_Reporte(CadenaConexion, null, Convert.ToInt32(Session["ID"].ToString()));
                    p = new ReportParameter("Titulo", "Reporte Registro de Cliente Individual");
                }
                else if (Session["busqueda"] != null && Session["busqueda"].ToString() != "")
                {
                    dt = clienteDAL.Clientes_Reporte(CadenaConexion, Session["busqueda"].ToString());
                    p = new ReportParameter("Titulo", "Reporte Registro de Clientes General");
                }
                else if (Session["identificacion"] != null && Session["identificacion"].ToString() != "")
                {
                    dt = clienteDAL.Clientes_Reporte(CadenaConexion, null, 0, Convert.ToDouble(Session["identificacion"]));
                    p = new ReportParameter("Titulo", "Reporte Registro de Cliente Individual");
                }
                else
                {
                    dt = clienteDAL.Clientes_Reporte(CadenaConexion);
                    p = new ReportParameter("Titulo", "Reporte Registro de Clientes General");
                }
                DataView dv = dt.DefaultView;
                dv.Sort = "NOMBRE_COMPLETO";
                dt = dv.ToTable();

                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("ReporteCliente.rdlc");
                ReportViewer1.SizeToReportContent = true;
                ReportViewer1.Width = Unit.Percentage(100);
                ReportViewer1.Height = Unit.Percentage(100);
                ReportViewer1.LocalReport.DataSources.Clear();
                List<ReportParameter> par = new List<ReportParameter>();
                par.Add(p);
                ReportViewer1.LocalReport.SetParameters(par);
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", dt));
                ReportViewer1.LocalReport.Refresh();
            }
        }
    }
}