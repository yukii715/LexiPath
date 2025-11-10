<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LexiPath.Login" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-5">
        <div class="row justify-content-center">
            <div class="col-md-6 col-lg-5">
                <div class="card shadow-lg">
                    <div class="card-header bg-primary text-white text-center">
                        <h2>Sign In to LexiPath</h2>
                    </div>
                    <div class="card-body">
                        
                        <%-- ERROR MESSAGE DISPLAY --%>
                        <asp:Label ID="lblMessage" runat="server" ForeColor="Red" EnableViewState="false" CssClass="d-block mb-3"></asp:Label>

                        <div class="mb-3">
                            <label for="<%= txtUsername.ClientID %>" class="form-label">Username</label>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter your username"></asp:TextBox>
                            <%-- Validator 1: Required Field --%>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtUsername"
                                ErrorMessage="Username is required." Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                        </div>

                        <div class="mb-3">
                            <label for="<%= txtPassword.ClientID %>" class="form-label">Password</label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter your password"></asp:TextBox>
                            <%-- Validator 2: Required Field --%>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword"
                                ErrorMessage="Password is required." Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                        </div>

                        <asp:Button ID="btnLogin" runat="server" Text="Sign In" CssClass="btn btn-primary w-100" OnClick="btnLogin_Click" />
                    </div>
                    
                    <div class="card-footer text-center">
                        <p class="mb-0">Don't have an account? 
                            <asp:HyperLink ID="lnkRegister" runat="server" NavigateUrl="~/Register.aspx">Register Now</asp:HyperLink>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
