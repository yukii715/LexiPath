<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuizDetail.aspx.cs" Inherits="LexiPath.QuizDetail" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <asp:HiddenField ID="hdnQuizID" runat="server" />

    <div class="container mt-4">
        
        <p>
            <asp:HyperLink ID="lnkBackToQuizzes" runat="server" NavigateUrl="~/Quizzes.aspx" CssClass="btn btn-outline-secondary btn-sm">
                &larr; Back to All Quizzes
            </asp:HyperLink>
        </p>

        <div class="p-5 mb-4 bg-light rounded-3">
            <div class="container-fluid">
                <div class="row align-items-center">
                    <div class="col-md-4">
                        <asp:Image ID="imgQuiz" runat="server" CssClass="img-fluid rounded shadow-sm" alt="Quiz Image" />
                    </div>
                    <div class="col-md-8">
                        <h1 class="display-5 fw-bold"><asp:Literal ID="litQuizTitle" runat="server"></asp:Literal></h1>
                        <p class="fs-4"><asp:Literal ID="litDescription" runat="server"></asp:Literal></p>
                        
                        <asp:Button ID="btnStartQuiz" runat="server" Text="Start Quiz" 
                            CssClass="btn btn-primary btn-lg mt-3" OnClick="btnStartQuiz_Click" />
                    </div>
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlRelatedCourses" runat="server" Visible="false">
            <h4>Recommended Courses</h4>
            <p>This quiz covers material from the following courses. You can review them before you start (links open in a new tab).</p>
            
            <asp:Repeater ID="rptRelatedCourses" runat="server">
                <ItemTemplate>
                    <a href='<%# GetDetailUrl(Eval("CourseID")) %>' target="_blank" class="btn btn-outline-info me-2 mb-2">
                        <%# Eval("CourseName") %>
                    </a>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>
    </div>
</asp:Content>
