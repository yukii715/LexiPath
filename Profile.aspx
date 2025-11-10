<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="LexiPath.Profile" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        
        <div class="p-5 mb-4 bg-light rounded-3">
            <div class="container-fluid py-5">
                <h1 class="display-5 fw-bold">My Profile</h1>
                <p class="fs-4">Manage your account details, track your progress, and update your password.</p>
            </div>
        </div>

        <h3>Your Statistics</h3>
        <div class="row g-4 mb-4">
            <div class="col-md-6">
                <div class="card text-center shadow-sm">
                    <div class="card-body">
                        <h2 class="display-4"><asp:Literal ID="litCoursesCompleted" runat="server" Text="0"></asp:Literal></h2>
                        <p class="card-text fs-5">Courses Completed</p>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card text-center shadow-sm">
                    <div class="card-body">
                        <h2 class="display-4"><asp:Literal ID="litQuizzesTaken" runat="server" Text="0"></asp:Literal></h2>
                        <p class="card-text fs-5">Quizzes Taken</p>
                    </div>
                </div>
            </div>
        </div>
        
        <hr class="my-5" />

        <div class="row g-5">
            
            <div class="col-md-4">
                <h3>Profile Picture</h3>
                <asp:Image ID="imgProfilePic" runat="server" CssClass="img-fluid rounded shadow-sm mb-3" ImageUrl="/Image/System/placeholder_profile.png" />
                
                <asp:FileUpload ID="fileUploadPic" runat="server" CssClass="form-control" />
                <asp:Button ID="btnUploadPic" runat="server" Text="Upload New Picture" CssClass="btn btn-secondary mt-2 w-100" OnClick="btnUploadPic_Click" />
                <asp:Label ID="lblPicMessage" runat="server" EnableViewState="false" CssClass="d-block mt-2"></asp:Label>
            </div>

            <div class="col-md-8">
                
                <h3>Account Details</h3>
                <div class="card shadow-sm mb-4">
                    <div class="card-body">
                        <asp:Label ID="lblProfileMessage" runat="server" EnableViewState="false" CssClass="d-block mb-3"></asp:Label>
                        <div class="mb-3">
                            <label class="form-label">Username</label>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Email</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                        </div>
                        <asp:Button ID="btnUpdateProfile" runat="server" Text="Save Details" CssClass="btn btn-primary" OnClick="btnUpdateProfile_Click" />
                    </div>
                </div>

                <h3>Change Password</h3>
                <div class="card shadow-sm">
                    <div class="card-body">
                        <asp:Label ID="lblPasswordMessage" runat="server" EnableViewState="false" CssClass="d-block mb-3"></asp:Label>
                        <div class="mb-3">
                            <label class="form-label">New Password</label>
                            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Confirm New Password</label>
                            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                            <asp:CompareValidator ID="compPassword" runat="server" ControlToCompare="txtNewPassword" ControlToValidate="txtConfirmPassword"
                                ErrorMessage="Passwords do not match." Display="Dynamic" CssClass="text-danger"></asp:CompareValidator>
                        </div>
                        <asp:Button ID="btnUpdatePassword" runat="server" Text="Change Password" CssClass="btn btn-danger" OnClick="btnUpdatePassword_Click" />
                    </div>
                </div>

            </div>
        </div>

    </div>
</asp:Content>

