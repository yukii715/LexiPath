<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="LexiPath.Admin.AdminDashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="AdminContent" runat="server">
    
    <h1 class="mb-4">Admin Dashboard</h1>

    <div class="card shadow-sm border-0 mb-5">
        <div class="card-header bg-light">
            <h4 class="mb-0">Key Performance Indicators (KPIs)</h4>
        </div>
        <div class="card-body">
            <div class="row g-4 text-center">
                
                <div class="col-md-3">
                    <div class="p-3 border rounded-3 bg-light">
                        <i class="bi bi-people-fill text-primary" style="font-size: 2rem;"></i>
                        <h2 class="display-5 mt-2 mb-0"><asp:Literal ID="litTotalUsers" runat="server" Text="0"></asp:Literal></h2>
                        <p class="text-muted mb-0">Active Users</p>
                    </div>
                </div>

                <div class="col-md-3">
                    <div class="p-3 border rounded-3 bg-light">
                        <i class="bi bi-book-fill text-success" style="font-size: 2rem;"></i>
                        <h2 class="display-5 mt-2 mb-0"><asp:Literal ID="litTotalCourses" runat="server" Text="0"></asp:Literal></h2>
                        <p class="text-muted mb-0">Active Courses</p>
                    </div>
                </div>

                <div class="col-md-3">
                    <div class="p-3 border rounded-3 bg-light">
                        <i class="bi bi-patch-question-fill text-warning" style="font-size: 2rem;"></i>
                        <h2 class="display-5 mt-2 mb-0"><asp:Literal ID="litTotalQuizzes" runat="server" Text="0"></asp:Literal></h2>
                        <p class="text-muted mb-0">Active Quizzes/Practices</p>
                    </div>
                </div>
                
                <div class="col-md-3">
                    <div class="p-3 border rounded-3 bg-light">
                        <i class="bi bi-tags-fill text-info" style="font-size: 2rem;"></i>
                        <h2 class="display-5 mt-2 mb-0"><asp:Literal ID="litTotalCategories" runat="server" Text="0"></asp:Literal></h2>
                        <p class="text-muted mb-0">Active Categories</p>
                    </div>
                </div>

            </div>
        </div>
    </div>
    
    <h4 class="mb-3">Quick Navigation</h4>
    <div class="row g-4">
        <div class="col-md-6 col-lg-3">
            <a href="ManageUsers.aspx" class="card text-decoration-none shadow-sm h-100 bg-light-hover">
                <div class="card-body text-center py-4">
                    <i class="bi bi-person-lines-fill text-primary" style="font-size: 2rem;"></i>
                    <h5 class="card-title mt-2 mb-0">Manage Users</h5>
                </div>
            </a>
        </div>
        <div class="col-md-6 col-lg-3">
            <a href="ManageCategories.aspx" class="card text-decoration-none shadow-sm h-100 bg-light-hover">
                <div class="card-body text-center py-4">
                    <i class="bi bi-folder-fill text-info" style="font-size: 2rem;"></i>
                    <h5 class="card-title mt-2 mb-0">Manage Categories</h5>
                </div>
            </a>
        </div>
        <div class="col-md-6 col-lg-3">
            <a href="ManageCourses.aspx" class="card text-decoration-none shadow-sm h-100 bg-light-hover">
                <div class="card-body text-center py-4">
                    <i class="bi bi-journal-bookmark-fill text-success" style="font-size: 2rem;"></i>
                    <h5 class="card-title mt-2 mb-0">Manage Courses</h5>
                </div>
            </a>
        </div>
        <div class="col-md-6 col-lg-3">
            <a href="ManageQuizzes.aspx" class="card text-decoration-none shadow-sm h-100 bg-light-hover">
                <div class="card-body text-center py-4">
                    <i class="bi bi-patch-check-fill text-warning" style="font-size: 2rem;"></i>
                    <h5 class="card-title mt-2 mb-0">Manage Quizzes</h5>
                </div>
            </a>
        </div>
    </div>

</asp:Content>