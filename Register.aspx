<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="LexiPath.Register" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-5">
        <div class="row justify-content-center">
            <div class="col-md-8 col-lg-7">
                <div class="card shadow-lg">
                    <div class="card-header bg-success text-white text-center">
                        <h2>Create Your LexiPath Account</h2>
                    </div>
                    <div class="card-body">
                        
                        <%-- MESSAGE DISPLAY (Success or Failure) --%>
                        <asp:Label ID="lblMessage" runat="server" EnableViewState="false" CssClass="d-block mb-3"></asp:Label>

                        <div class="mb-3">
                            <label for="<%= txtUsername.ClientID %>" class="form-label">Username (Alphanumeric, min 3 chars)</label>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Choose a username"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="reqUsername" runat="server" ControlToValidate="txtUsername"
                                ErrorMessage="Username is required." Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regexUsername" runat="server" ControlToValidate="txtUsername"
                                ValidationExpression="^[a-zA-Z0-9]{3,}$"
                                ErrorMessage="Username must be 3+ alphanumeric characters." Display="Dynamic" CssClass="text-danger"></asp:RegularExpressionValidator>
                        </div>

                        <div class="mb-3">
                            <label for="<%= txtEmail.ClientID %>" class="form-label">Email Address</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="Enter your email"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="reqEmail" runat="server" ControlToValidate="txtEmail"
                                ErrorMessage="Email is required." Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                        </div>

                        <div class="mb-3">
                            <label for="<%= txtPassword.ClientID %>" class="form-label">Password (8-20 chars, must include symbols/numbers)</label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="reqPassword" runat="server" ControlToValidate="txtPassword"
                                ErrorMessage="Password is required." Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regexPassword" runat="server" ControlToValidate="txtPassword"
                                ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$"
                                ErrorMessage="Password must be 8-20 characters and include uppercase, lowercase, number, and symbol." Display="Dynamic" CssClass="text-danger"></asp:RegularExpressionValidator>
                        </div>

                        <div class="mb-3">
                            <label for="<%= txtConfirmPassword.ClientID %>" class="form-label">Confirm Password</label>
                            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Confirm password"></asp:TextBox>
                            <asp:CompareValidator ID="compPassword" runat="server" ControlToCompare="txtPassword" ControlToValidate="txtConfirmPassword"
                                ErrorMessage="Passwords do not match." Display="Dynamic" CssClass="text-danger"></asp:CompareValidator>
                        </div>

                        <asp:Button ID="btnRegister" runat="server" Text="Register Account" CssClass="btn btn-success w-100" OnClick="btnRegister_Click" />
                    </div>
                    
                    <div class="card-footer text-center">
                        <p class="mb-0">Already have an account? 
                            <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="~/Login.aspx">Sign In</asp:HyperLink>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
