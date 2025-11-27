<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LexiPath.Login" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container d-flex align-items-center justify-content-center" style="min-height: 85vh;">
        <div class="col-12 col-md-7 col-lg-6 col-xl-5">
            
            <div class="card shadow-lg border-0 rounded-4 overflow-hidden">
                
                <div class="card-header border-0 p-0" style="height: 8px; background: linear-gradient(to right, #667eea, #764ba2);"></div>
                
                <div class="card-body p-5">

                    <div class="text-center mb-5">
                        <div class="mb-3 d-inline-flex align-items-center justify-content-center bg-light text-primary rounded-circle" style="width: 64px; height: 64px;">
                            <i class="bi bi-person-circle fs-2"></i>
                        </div>
                        <h2 class="fw-bold text-dark">Welcome Back!</h2>
                        <p class="text-muted">Please sign in to continue your progress.</p>
                    </div>

                    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" CssClass="d-block mb-3 text-center fw-bold text-danger"></asp:Label>

                    <div class="mb-4">
                        <label class="form-label fw-bold text-secondary small ms-3">Username</label>
                        <div class="input-group shadow-sm rounded-pill overflow-hidden bg-white border">
                            <span class="input-group-text bg-white border-0 ps-3 text-muted"><i class="bi bi-person fs-5"></i></span>
                            <asp:TextBox ID="txtUsername" runat="server" 
                                CssClass="form-control border-0 shadow-none" 
                                placeholder="Enter your username"
                                autocomplete="off"></asp:TextBox>
                        </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtUsername"
                            ErrorMessage="Username is required." Display="Dynamic" CssClass="text-danger small ms-3 mt-1"></asp:RequiredFieldValidator>
                    </div>

                    <div class="mb-4">
                        <label class="form-label fw-bold text-secondary small ms-3">Password</label>
                        <div class="input-group shadow-sm rounded-pill overflow-hidden bg-white border">
                            <span class="input-group-text bg-white border-0 ps-3 text-muted"><i class="bi bi-lock fs-5"></i></span>
                            <asp:TextBox ID="txtPassword" runat="server" 
                                CssClass="form-control border-0 shadow-none" 
                                TextMode="Password" placeholder="Enter your password"></asp:TextBox>
                        </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword"
                            ErrorMessage="Password is required." Display="Dynamic" CssClass="text-danger small ms-3 mt-1"></asp:RequiredFieldValidator>
                    </div>

                    <div class="text-center mt-5">
                        <asp:Button ID="btnLogin" runat="server" Text="SIGN IN" 
                            CssClass="btn btn-primary btn-lg px-5 rounded-pill fw-bold shadow-sm" 
                            OnClick="btnLogin_Click"
                            style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border: none; min-width: 200px;" />
                    </div>

                    <div class="text-center mt-4">
                        <span class="text-muted">Don't have an account?</span>
                        <asp:HyperLink ID="lnkRegister" runat="server" NavigateUrl="~/Register.aspx" 
                            CssClass="text-decoration-none fw-bold text-primary ms-1">
                            Register Now
                        </asp:HyperLink>
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