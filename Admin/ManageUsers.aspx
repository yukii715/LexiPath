<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="LexiPath.Admin.ManageUsers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    
    <h1 class="mb-4">Manage Users</h1>

    <div class="card shadow-sm border-0">
        <div class="card-body">
            
            <asp:GridView ID="gvUsers" runat="server" 
                AutoGenerateColumns="False" 
                CssClass="table table-hover align-middle" 
                DataKeyNames="UserID"
                OnRowCommand="gvUsers_RowCommand"
                EmptyDataText="No users found."
                BorderStyle="None">
                
                <HeaderStyle CssClass="table-light" />
                <Columns>
                    <asp:BoundField DataField="UserID" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="Username" HeaderText="Username" />
                    <asp:BoundField DataField="Email" HeaderText="Email" />
                    <asp:BoundField DataField="CreatedAt" HeaderText="Joined" DataFormatString="{0:d MMM yyyy}" />
                    
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class="badge <%# Eval("Status").ToString() == "Active" ? "bg-success" : "bg-danger" %>">
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:Button ID="btnToggleStatus" runat="server"
                                CommandName="ToggleStatus"
                                CommandArgument='<%# Eval("UserID") %>'
                                Text='<%# Eval("Status").ToString() == "Active" ? "Block" : "Activate" %>'
                                CssClass='<%# Eval("Status").ToString() == "Active" ? "btn btn-danger btn-sm" : "btn btn-success btn-sm" %>'
                                OnClientClick="return confirm('Are you sure?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>

            </asp:GridView>

        </div>
    </div>

</asp:Content>