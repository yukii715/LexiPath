<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="LexiPath.Register" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container d-flex align-items-center justify-content-center" style="min-height: 85vh;">
        <div class="col-12 col-md-8 col-lg-6 col-xl-5">

            <div class="card shadow-lg border-0 rounded-4 overflow-hidden">
                
                <div class="card-header border-0 p-0" style="height: 8px; background: linear-gradient(to right, #20c997, #667eea);"></div>

                <div class="card-body p-5">
                    
                    <div class="text-center mb-5">
                        <h2 class="fw-bold text-dark mb-2">Create Account</h2>
                        <p class="text-muted">Join LexiPath to start your learning journey.</p>
                    </div>

                    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" CssClass="d-block mb-3 text-center fw-bold text-danger"></asp:Label>

                    <div class="mb-3">
                        <label class="form-label fw-bold text-secondary small ms-3">Username</label>
                        <div class="input-group shadow-sm rounded-pill overflow-hidden bg-white border">
                            <span class="input-group-text bg-white border-0 ps-3 text-muted"><i class="bi bi-person fs-5"></i></span>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control border-0 shadow-none" placeholder="Enter your username" autocomplete="off"></asp:TextBox>
                        </div>
                        <asp:RequiredFieldValidator ID="reqUsername" runat="server" ControlToValidate="txtUsername" ErrorMessage="Username is required." Display="Dynamic" CssClass="text-danger small ms-3 mt-1" />
                        <asp:RegularExpressionValidator ID="regexUsername" runat="server" ControlToValidate="txtUsername" ValidationExpression="^[a-zA-Z0-9]{3,}$" ErrorMessage="Min 3 alphanumeric chars." Display="Dynamic" CssClass="text-danger small ms-3 mt-1" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label fw-bold text-secondary small ms-3">Email Address</label>
                        <div class="input-group shadow-sm rounded-pill overflow-hidden bg-white border">
                            <span class="input-group-text bg-white border-0 ps-3 text-muted"><i class="bi bi-envelope fs-5"></i></span>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control border-0 shadow-none" TextMode="Email" placeholder="Enter your email" autocomplete="off"></asp:TextBox>
                        </div>
                        <asp:RequiredFieldValidator ID="reqEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required." Display="Dynamic" CssClass="text-danger small ms-3 mt-1" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label fw-bold text-secondary small ms-3">Password</label>
                        <div class="input-group shadow-sm rounded-pill overflow-hidden bg-white border">
                            <span class="input-group-text bg-white border-0 ps-3 text-muted"><i class="bi bi-key fs-5"></i></span>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control border-0 shadow-none" TextMode="Password" placeholder="Create a password"></asp:TextBox>
                        </div>
                        <asp:RequiredFieldValidator ID="reqPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required." Display="Dynamic" CssClass="text-danger small ms-3 mt-1" />
                        <asp:RegularExpressionValidator ID="regexPassword" runat="server" ControlToValidate="txtPassword" ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$" ErrorMessage="8-20 chars, Uppercase, Lowercase, Number, Symbol." Display="Dynamic" CssClass="text-danger small ms-3 mt-1" />
                    </div>

                    <div class="mb-4">
                        <label class="form-label fw-bold text-secondary small ms-3">Confirm Password</label>
                        <div class="input-group shadow-sm rounded-pill overflow-hidden bg-white border">
                            <span class="input-group-text bg-white border-0 ps-3 text-muted"><i class="bi bi-check2-circle fs-5"></i></span>
                            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control border-0 shadow-none" TextMode="Password" placeholder="Repeat your password"></asp:TextBox>
                        </div>
                        <asp:CompareValidator ID="compPassword" runat="server" ControlToCompare="txtPassword" ControlToValidate="txtConfirmPassword" ErrorMessage="Passwords do not match." Display="Dynamic" CssClass="text-danger small ms-3 mt-1" />
                    </div>

                    <div class="text-center mt-4">
                        <asp:Button ID="btnRegister" runat="server" Text="SIGN UP"
                            CssClass="btn btn-primary btn-lg px-5 rounded-pill fw-bold shadow-sm"
                            OnClick="btnRegister_Click" 
                            style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border: none; min-width: 200px;" />
                    </div>

                    <div class="text-center mt-4">
                        <span class="text-muted">Already have an account?</span>
                        <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="~/Login.aspx" CssClass="text-decoration-none fw-bold text-primary ms-1">Sign In</asp:HyperLink>
                    </div>

                </div>
            </div>
        </div>
    </div>

    <style>
        /* Hover effect for the input groups */
        .input-group {
            transition: box-shadow 0.3s ease, border-color 0.3s ease;
        }
        .input-group:focus-within {
            border-color: #667eea !important;
            box-shadow: 0 0 0 0.25rem rgba(102, 126, 234, 0.15) !important;
        }
        .text-primary { color: #667eea !important; }
    </style>

</asp:Content>