<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TestDustinEspinozaWeb.Default" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/css/bootstrap.min.css" integrity="sha384-Vkoo8x4CGsO3+Hhxv8T/Q5PaXtkKtu6ug5TOeNV6gBiFeWPGFN9MuhOf23Q9Ifjh" crossorigin="anonymous">
    <title></title>
        <script type="text/javascript" src="https://code.jquery.com/jquery-3.4.1.min.js"></script>
        <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.0/dist/umd/popper.min.js" integrity="sha384-Q6E9RHvbIyZFJoft+2mJbHaEWldlvI9IOYy5n3zV9zzTtmI3UksdQRVvoxMfooAo" crossorigin="anonymous"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/js/bootstrap.min.js" integrity="sha384-wfSDF2E50Y2D1uUdj0O3uMBJnjuUD4Ih7YwaYd1iqfktj0Uod8GCExl3Og8ifwB6" crossorigin="anonymous"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#txtTelefono').attr('type', 'number');
            $('#txtIdentificacion').attr('type', 'number');

            $('#btnNuevo').click(function (e) {
                $('#txtIdentificacion').val('');
                $('#txtNombreCompleto').val('');
                $('#txtTelefono').val('');
            });
        });
            function openEditModal() {
                $('#exampleModal').modal('show');
        }
        function openDeleteModal() {
            $('#deleteModal').modal('show');
        }

        function openSuccessModal() {
            $('#successModal').modal('show');
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">Mantenimiento de clientes</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <asp:TextBox ID="txtID" runat="server" Visible="false"></asp:TextBox>
                        <div class="form-group">
                            <label for="txtIdentificacion">Identificación:</label>
                            <asp:TextBox ID="txtIdentificacion" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:Label ID="txtIdentificacionError" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                        </div>
                        <div class="form-group">
                            <label for="txtNombreCompleto">Nombre completo:</label>
                            <asp:TextBox ID="txtNombreCompleto" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:Label ID="txtNombreCompletoError" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                        </div>
                        <div class="form-group">
                            <label for="txtTelefono">Teléfono:</label>
                            <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:Label ID="txtTelefonoError" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <a id="btnImprimirSeleccionar" runat="server" class="btn btn-secondary" href="Report.aspx" target="_blank" style="color:white">Imprimir</a>
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                        <asp:Button CssClass="btn btn-primary" runat="server" ID="btnGuardar" Text="Guardar" OnClick="btnGuardar_Click"></asp:Button>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal fade" id="deleteModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="deleteModalLabel">¿Seguro desea eliminar el registro?</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-footer">
                        <asp:Button CssClass="btn btn-primary" runat="server" ID="btnConfirmarBorrar" Text="Sí" OnClick="btnConfirmarBorrar_Click"></asp:Button>
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal fade" id="successModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <asp:Label ID="lblSuccess" runat="server" class="modal-title"></asp:Label>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-footer justify-content-center">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Aceptar</button>
                    </div>
                </div>
            </div>
        </div>
        <div class="container">
            <div style="height:48px"></div>
            <h1 class="display-4 border-bottom">Consulta de clientes</h1>
            <div class="row justify-content-end" style="margin-bottom:16px">
                <div style="margin-right:8px">
                    <asp:Button ID="btnNuevo" runat="server" CssClass="btn btn-success" Text="Nuevo" OnClick="btnNuevo_Click">
                    </asp:Button>
                </div>
                <div style="margin-left:8px">
                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-info" OnClick="btnBuscar_Click"/>
                </div>
                <div style="margin-left:8px">
                    <a id="btnImprimir" runat="server" class="btn btn-secondary" href="Report.aspx" target="_blank" style="color:white">Imprimir</a>
                </div>
            </div>
            <div class="row">
                <div class="col-3">
                    <asp:Label ID="lblBusqueda" runat="server" Text="Buscar por Nombre Completo:"></asp:Label>
                </div>
                <div class="col-9">
                    <asp:TextBox ID="txtBusqueda" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <br />
            </div>
            <div style="height:16px"></div>
            <asp:GridView ID="grv_clientes" runat="server" AutoGenerateColumns="false" RowStyle-Wrap="false"
                PageSize="5" CellSpacing="1" EmptyDataText="No hay clientes ingresados." OnPageIndexChanged="grv_clientes_PageIndexChanged" 
                OnPageIndexChanging="grv_clientes_PageIndexChanging" OnRowCommand="grv_clientes_RowCommand" 
                ValidateRequestMode="Enabled" DataKeyNames="ID" CssClass="table table-striped">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lblSel" runat="server" Text="Sel."></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnSeleccionar" runat="server" Text="Sel." CommandName="seleccionar" CommandArgument='<%#Eval("ID")%>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lbIdentificacion" runat="server" Text="Identificación"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("IDENTIFICACION")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lbNombre" runat="server" Text="Nombre Completo"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("NOMBRE_COMPLETO")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lbTelefono" runat="server" Text="Teléfono"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%#Eval("TELEFONO")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="lbEliminar" runat="server" Text="Eliminar"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEliminar" runat="server" Text="Eliminar" CommandName="eliminar" CommandArgument='<%#Eval("ID")%>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
