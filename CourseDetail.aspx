<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CourseDetail.aspx.cs" Inherits="LexiPath.CourseDetail" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <asp:HiddenField ID="hdnCourseID" runat="server" />

    <div class="container mt-4">
        
        <p>
            <asp:HyperLink ID="lnkBackToCourses" runat="server" NavigateUrl="~/Courses.aspx" CssClass="btn btn-outline-secondary btn-sm">
                &larr; Back to Course List
            </asp:HyperLink>
        </p>

        <div class="p-5 mb-4 bg-light rounded-3">
            <div class="container-fluid">
                <div class="row align-items-center">
                    <div class="col-md-4">
                        <asp:Image ID="imgCourse" runat="server" CssClass="img-fluid rounded shadow-sm" alt="Course Image" />
                    </div>
                    <div class="col-md-8">
                        <h1 class="display-5 fw-bold"><asp:Literal ID="litCourseName" runat="server"></asp:Literal></h1>
                        <h5><span class="badge bg-secondary fs-6"><asp:Literal ID="litCourseType" runat="server"></asp:Literal></span></h5>
                        <p class="fs-4 mt-3"><asp:Literal ID="litDescription" runat="server"></asp:Literal></p>
                    </div>
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlLearn" runat="server">
            <h3>Learn the Content</h3>
            <p>
                <asp:Literal ID="litLearnMessage" runat="server"></asp:Literal>
            </p>
            
            <asp:Button ID="btnStartLesson" runat="server" Text="Start Lesson" 
                CssClass="btn btn-primary btn-lg mt-3" OnClick="btnStartLesson_Click" />
            
            </asp:Panel>

        <asp:Panel ID="pnlPractice" runat="server" Visible="false">
            <hr class="my-5" />
            <h3>Practice Module</h3>
            <p>You've completed the course! Test your knowledge with this practice session.</p>

            <asp:HiddenField ID="hdnPracticeQuizID" runat="server" Value="0" />

            <asp:Button ID="btnStartPractice" runat="server" Text="Start Practice" 
                CssClass="btn btn-success btn-lg" OnClick="btnStartPractice_Click" Visible="false" />

            <asp:Label ID="lblNoPractice" runat="server" CssClass="alert alert-secondary d-block" 
                Text="A practice module for this course is not available yet." Visible="false"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="pnlPracticeLocked" runat="server" Visible="false">
            <hr class="my-5" />
            <h3>Practice Module (Locked)</h3>
            <div class="alert alert-info">
                You must complete the "Learn" module by clicking "End Lesson" before you can access the practice questions.
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlLoginRequired" runat="server" Visible="false">
            <hr class="my-5" />
            <h3>Practice Module</h3>
            <div class="alert alert-warning">
                You must <a href="Login.aspx">sign in</a> to track your progress and access the practice module.
            </div>
        </asp:Panel>

    </div>
</asp:Content>