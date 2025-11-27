<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuizDetail.aspx.cs" Inherits="LexiPath.QuizDetail" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <asp:HiddenField ID="hdnQuizID" runat="server" />

    <div class="container mt-4">
        
        <!-- Back Link -->
        <p>
            <asp:HyperLink ID="lnkBackToQuizzes" runat="server" NavigateUrl="~/Quizzes.aspx" 
                CssClass="btn btn-outline-primary btn-sm rounded-pill mb-3 shadow-sm">
                <i class="bi bi-arrow-left me-1"></i> Back to All Quizzes
            </asp:HyperLink>
        </p>

        <!-- Quiz Header/Hero Section (Gradient Style) -->
        <div class="p-5 mb-4 rounded-3 text-white shadow-lg" 
             style="background: linear-gradient(135deg, #764ba2 0%, #667eea 100%);">
            <div class="container-fluid p-0">
                <div class="row align-items-center">
                    <!-- Image Column -->
                    <div class="col-md-4 mb-4 mb-md-0">
                        <asp:Image ID="imgQuiz" runat="server" CssClass="img-fluid rounded-lg shadow-xl" alt="Quiz Image" />
                    </div>
                    <!-- Details Column -->
                    <div class="col-md-8">
                        <h1 class="display-5 fw-bold mb-3">
                            <i class="bi bi-question-diamond-fill me-2"></i> <asp:Literal ID="litQuizTitle" runat="server"></asp:Literal>
                        </h1>
                        <p class="fs-5 mt-3 text-white-75">
                            <asp:Literal ID="litDescription" runat="server"></asp:Literal>
                        </p>
                        
                        <asp:Button ID="btnStartQuiz" runat="server" Text="Start Quiz" 
                            CssClass="btn btn-warning btn-lg mt-4 rounded-pill px-5 fw-bold shadow" OnClick="btnStartQuiz_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Related Courses Panel -->
        <asp:Panel ID="pnlRelatedCourses" runat="server" Visible="false" 
                   CssClass="card border-0 shadow-sm rounded-3 p-4 mb-5">
            <div class="card-body">
                <h4 class="card-title fw-bold mb-3 text-primary">
                    <i class="bi bi-link-45deg me-2"></i> Recommended Courses
                </h4>
                <p class="text-muted">This quiz covers material from the following courses. You can review them before you start (links open in a new tab).</p>
                
                <div class="mt-3">
                    <asp:Repeater ID="rptRelatedCourses" runat="server">
                        <ItemTemplate>
                            <a href='<%# GetDetailUrl(Eval("CourseID")) %>' target="_blank" 
                               class="btn btn-outline-info rounded-pill me-2 mb-2 px-3 py-2">
                                <i class="bi bi-book me-1"></i> <%# Eval("CourseName") %>
                            </a>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </asp:Panel>
    </div>
    
    <!-- Custom Styles -->
    <style>
        /* Define the primary color for text/icons */
        .text-primary {
            color: #667eea !important;
        }

        /* Shadow for image within hero section */
        .shadow-xl {
            box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.175) !important;
        }

        /* Ensure card image has rounded corners */
        .rounded-lg {
            border-radius: 0.5rem !important;
        }

        /* Custom style for the Start Quiz button */
        .btn-warning {
            background-color: #ffc107; /* Standard Bootstrap warning color for quizzes */
            border-color: #ffc107;
            color: #212529; /* Dark text for contrast */
        }
        
        .btn-warning:hover {
            background-color: #e0a800;
            border-color: #e0a800;
        }
    </style>
</asp:Content>