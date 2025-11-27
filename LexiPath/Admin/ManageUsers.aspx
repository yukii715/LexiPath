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
                BorderStyle="None" GridLines="None">
                
                <HeaderStyle CssClass="table-light fw-bold" />
                <Columns>
                    <asp:BoundField DataField="UserID" HeaderText="ID" ReadOnly="True" ItemStyle-Width="50px" />
                    
                    <asp:TemplateField HeaderText="Username">
                        <ItemTemplate>
                            <div class="d-flex align-items-center">
                                <asp:Image ID="imgProfile" runat="server" 
                                    ImageUrl='<%# GetProfileImage(Eval("ProfilePicPath")) %>' 
                                    CssClass="rounded-circle me-3 border" 
                                    Width="40" Height="40" style="object-fit: cover;" />
                                <span class="fw-medium"><%# Eval("Username") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Email" HeaderText="Email" />
                    <asp:BoundField DataField="CreatedAt" HeaderText="Joined" DataFormatString="{0:MMM dd, yyyy}" ItemStyle-CssClass="text-muted small" />
                    
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class="badge rounded-pill <%# Eval("Status").ToString() == "Active" ? "bg-success-subtle text-success" : "bg-danger-subtle text-danger" %>">
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Actions" ItemStyle-Width="300px">
                        <ItemTemplate>
                            <div class="d-flex gap-2">
                                
                                <%-- UPDATED: Added OnClientClick to trigger confirmation --%>
                                <asp:LinkButton ID="btnToggleStatus" runat="server"
                                    CommandName="ToggleStatus"
                                    CommandArgument='<%# Eval("UserID") %>'
                                    CssClass='<%# Eval("Status").ToString() == "Active" ? "btn btn-outline-danger btn-sm text-nowrap" : "btn btn-outline-success btn-sm text-nowrap" %>'
                                    ToolTip='<%# Eval("Status").ToString() == "Active" ? "Block User" : "Activate User" %>'
                                    OnClientClick='<%# "return confirmToggle(this, \"" + (Eval("Status").ToString() == "Active" ? "Block" : "Activate") + "\");" %>'>
                                    
                                    <i class="bi <%# Eval("Status").ToString() == "Active" ? "bi-slash-circle" : "bi-check-circle" %>"></i> 
                                    <%# Eval("Status").ToString() == "Active" ? "Block" : "Activate" %>
                                </asp:LinkButton>

                                <asp:LinkButton ID="btnResetPassword" runat="server"
                                    CommandName="ResetPassword"
                                    CommandArgument='<%# Eval("UserID") %>'
                                    CssClass="btn btn-outline-warning btn-sm text-nowrap"
                                    ToolTip="Reset Password"
                                    OnClientClick="return confirmReset(this);">
                                    <i class="bi bi-key"></i> Reset Password
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

        </div>
    </div>

    <script type="text/javascript">
        // 1. Standard Toast
        function showNotification(type, message) {
            Swal.fire({
                toast: true,
                position: 'top',
                icon: type,
                title: message,
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true
            });
        }

        // 2. Password Modal
        function showPasswordModal(password) {
            Swal.fire({
                title: 'Password Reset Successful',
                html: 'Copy this password immediately:<br/><br/><h2 class="text-danger fw-bold bg-light p-2 rounded border">' + password + '</h2>',
                icon: 'warning',
                confirmButtonText: 'OK, I copied it',
                confirmButtonColor: '#667eea',
                allowOutsideClick: false
            });
        }

        // 3. NEW: Confirm Status Toggle (Block vs Activate)
        function confirmToggle(btn, action) {
            if (btn.dataset.confirmed) return true;
            event.preventDefault();

            var titleText = action === 'Block' ? 'Block User?' : 'Activate User?';
            var bodyText = action === 'Block' ? 'This user will lose access to the system.' : 'This user will regain access to the system.';
            var btnColor = action === 'Block' ? '#dc3545' : '#198754'; // Red for Block, Green for Activate

            Swal.fire({
                title: titleText,
                text: bodyText,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: btnColor,
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Yes, ' + action
            }).then((result) => {
                if (result.isConfirmed) {
                    btn.dataset.confirmed = "true";
                    btn.click();
                }
            });
            return false;
        }

        // 4. Confirm Reset Password
        function confirmReset(btn) {
            if (btn.dataset.confirmed) return true;
            event.preventDefault();

            Swal.fire({
                title: 'Reset Password?',
                text: "This will generate a new random password.",
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#ffc107',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Yes, Generate New'
            }).then((result) => {
                if (result.isConfirmed) {
                    btn.dataset.confirmed = "true";
                    btn.click();
                }
            });
            return false;
        }
    </script>

    <style>
        .bg-success-subtle { background-color: #d1e7dd; }
        .bg-danger-subtle { background-color: #f8d7da; }
        .object-fit-cover { object-fit: cover; }
    </style>

</asp:Content>