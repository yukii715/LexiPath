<%@ Page Title="Admin Profile" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="AdminProfile.aspx.cs" Inherits="LexiPath.Admin.AdminProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="container-fluid">
        <h2 class="mb-4">My Profile</h2>

        <div class="row g-4">
            <div class="col-md-4">
                <div class="card border-0 shadow-sm">
                    <div class="card-body text-center">
                        <h5 class="card-title mb-3">Profile Picture</h5>
                        
                        <asp:Image ID="imgProfilePic" runat="server" CssClass="rounded-circle mb-3 shadow-sm" 
                            ImageUrl="/Image/System/placeholder_profile.png" 
                            style="width: 150px; height: 150px; object-fit: cover;" />
                        
                        <div class="mb-3 text-start">
                            <asp:FileUpload ID="fileUploadPic" runat="server" CssClass="form-control form-control-sm" />
                        </div>
                        
                        <asp:Button ID="btnUploadPic" runat="server" Text="Update Picture" 
                            CssClass="btn btn-primary w-100" OnClick="btnUploadPic_Click" />
                    </div>
                </div>
            </div>

            <div class="col-md-8">
                
                <div class="card border-0 shadow-sm mb-4">
                    <div class="card-header bg-white py-3">
                        <h5 class="mb-0"><i class="bi bi-person-lines-fill me-2"></i>Account Details</h5>
                    </div>
                    <div class="card-body">
                        <div class="mb-3">
                            <label class="form-label">Username</label>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Email</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" autocomplete="off"></asp:TextBox>
                        </div>
                        
                        <div class="text-end">
                            <asp:Button ID="btnUpdateProfile" runat="server" Text="Save Changes" 
                                CssClass="btn btn-success" OnClick="btnUpdateProfile_Click" />
                        </div>
                    </div>
                </div>

                <div class="card border-0 shadow-sm">
                    <div class="card-header bg-white py-3">
                        <h5 class="mb-0"><i class="bi bi-shield-lock me-2"></i>Change Password</h5>
                    </div>
                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-12">
                                <label class="form-label">Current Password</label>
                                <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                            </div>

                            <div class="col-md-6">
                                <label class="form-label">New Password</label>
                                <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label">Confirm Password</label>
                                <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                <asp:CompareValidator ID="compPassword" runat="server" 
                                    ControlToCompare="txtNewPassword" ControlToValidate="txtConfirmPassword"
                                    ErrorMessage="Passwords do not match." Display="Dynamic" CssClass="text-danger small"></asp:CompareValidator>
                            </div>
                        </div>

                        <div class="text-end mt-3">
                            <asp:Button ID="btnUpdatePassword" runat="server" Text="Update Password" 
                                CssClass="btn btn-warning" OnClick="btnUpdatePassword_Click" />
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <script type="text/javascript">
        function showNotification(type, message) {
            Swal.fire({
                toast: true,
                position: 'top',
                icon: type,
                title: message,
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.addEventListener('mouseenter', Swal.stopTimer)
                    toast.addEventListener('mouseleave', Swal.resumeTimer)
                }
            });
        }
    </script>
</asp:Content>