<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CourseDetail.aspx.cs" Inherits="LexiPath.CourseDetail" EnableEventValidation="false" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <asp:HiddenField ID="hdnCourseID" runat="server" />
    <asp:HiddenField ID="hdnPracticeQuizID" runat="server" Value="0" />

    <div class="container mt-4">
        
        <p>
            <asp:HyperLink ID="lnkBackToCourses" runat="server" NavigateUrl="~/Courses.aspx" 
                CssClass="btn btn-outline-primary btn-sm rounded-pill mb-3 shadow-sm">
                <i class="bi bi-arrow-left me-1"></i> Back to Course List
            </asp:HyperLink>
        </p>

        <div class="p-5 mb-4 rounded-3 text-white shadow-lg" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);">
            <div class="container-fluid p-0">
                 <div class="row align-items-center">
                    <div class="col-md-4 mb-4 mb-md-0">
                        <asp:Image ID="imgCourse" runat="server" CssClass="img-fluid rounded-lg shadow-xl" alt="Course Image" />
                    </div>
                    <div class="col-md-8">
                        <h1 class="display-5 fw-bold mb-1"><asp:Literal ID="litCourseName" runat="server"></asp:Literal></h1>
                        <h2 class="h4 fw-bold text-white-75 mb-3"><asp:Literal ID="Literal1" runat="server"></asp:Literal></h2>
                        
                         <div class="d-flex align-items-center mb-3 flex-wrap gap-3">
                            <span class="badge bg-white text-primary fs-6 px-3 py-1 rounded-pill"><asp:Literal ID="litCourseType" runat="server"></asp:Literal></span>
                            <span class="badge bg-white text-secondary fs-6 px-3 py-1 rounded-pill"><asp:Literal ID="Literal2" runat="server"></asp:Literal></span>

                            <asp:Panel ID="pnlInteractions" runat="server" Visible="false" CssClass="ms-auto d-flex gap-2">
                                <asp:LinkButton ID="btnToggleCollect" runat="server" OnClick="btnToggleCollect_Click" CssClass="btn btn-outline-light rounded-pill" ToolTip="Bookmark">
                                     <asp:Literal ID="litCollectHtml" runat="server" Mode="PassThrough"></asp:Literal>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnToggleLike" runat="server" OnClick="btnToggleLike_Click" CssClass="btn btn-outline-light rounded-pill" ToolTip="Like">
                                    <asp:Literal ID="litLikeHtml" runat="server" Mode="PassThrough"></asp:Literal>
                                 </asp:LinkButton>
                            </asp:Panel>
                        </div>
                        
                        <p class="fs-5 mt-3 text-white-75"><asp:Literal ID="litDescription" runat="server"></asp:Literal></p>
                        <p class="fs-6 text-white-50"><asp:Literal ID="Literal3" runat="server"></asp:Literal></p>
                    </div>
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlLearn" runat="server" CssClass="card border-0 shadow-sm rounded-3 p-4 mb-5">
            <div class="card-body">
                 <h3 class="card-title text-primary fw-bold mb-3"><i class="bi bi-book-half me-2"></i> Start Learning</h3>
                <p><asp:Literal ID="litLearnMessage" runat="server"></asp:Literal></p>
                <asp:Button ID="btnStartLesson" runat="server" Text="Start Lesson" CssClass="btn btn-primary btn-lg mt-3 rounded-pill px-5" OnClick="btnStartLesson_Click" />
             </div>
        </asp:Panel>

        <hr class="my-5" />

        <h2 class="fw-bold mb-4"><i class="bi bi-person-fill-check me-2 text-success"></i> Practice & Test</h2>

        <asp:Panel ID="pnlPractice" runat="server" Visible="false" CssClass="card border-0 shadow-sm rounded-3 p-4 mb-4">
             <div class="card-body">
                <h3 class="card-title fw-bold">Available Practices</h3>
                <p class="text-muted mb-4">Test your knowledge with these practice sessions.</p>

                <%-- Repeater for Multiple Practices --%>
                <div class="row g-3">
                    <asp:Repeater ID="rptPractices" runat="server" OnItemCommand="rptPractices_ItemCommand">
                        <ItemTemplate>
                            <div class="col-12">
                                <div class="d-flex justify-content-between align-items-center p-3 border rounded bg-light h-100">
                                    <div>
                                        <h5 class="fw-bold mb-1"><%# Eval("Title") %></h5>
                                        <small class="text-muted"><%# Eval("Description") %></small>
                                    </div>
                                    <asp:Button ID="btnStartThisPractice" runat="server" Text="Start" 
                                        CommandName="StartPractice" CommandArgument='<%# Eval("QuizID") %>'
                                        CssClass="btn btn-success rounded-pill px-4 ms-3" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <asp:Label ID="lblNoPractice" runat="server" CssClass="alert alert-warning d-block my-3" 
                   Text="No active practice modules are available for this course yet." Visible="false"></asp:Label>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlPracticeLocked" runat="server" Visible="false" CssClass="card border-0 shadow-sm rounded-3 p-4 mb-4">
            <div class="card-body">
                <h3 class="card-title fw-bold text-info">Practice Module Locked</h3>
                <div class="alert alert-info my-3"><i class="bi bi-lock-fill me-2"></i> You must complete the "Start Learning" module first.</div>
            </div>
        </asp:Panel>
        
        <asp:Panel ID="pnlLoginRequired" runat="server" Visible="false" CssClass="card border-0 shadow-sm rounded-3 p-4 mb-4">
            <div class="card-body">
                <h3 class="card-title fw-bold text-warning">Login Required</h3>
                 <div class="alert alert-warning my-3"><i class="bi bi-exclamation-triangle-fill me-2"></i> Please <a href="Login.aspx" class="alert-link fw-bold">sign in</a> to access practice.</div>
            </div>
        </asp:Panel>

        <hr class="my-5" />

        <h2 class="fw-bold mb-4"><i class="bi bi-chat-dots-fill me-2 text-secondary"></i> Course Discussion</h2>

        <asp:UpdatePanel ID="upForum" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pnlCreatePost" runat="server" CssClass="card border-0 shadow-sm rounded-3 p-4 mb-4 bg-light">
                    <div class="d-flex gap-3">
                        <div class="flex-grow-1">
                            <asp:TextBox ID="txtNewPost" runat="server" TextMode="MultiLine" Rows="2" 
                                CssClass="form-control border-0 shadow-sm" placeholder="Ask a question or share your thoughts..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="align-self-end">
                            <asp:Button ID="btnSubmitPost" runat="server" Text="Post" 
                                CssClass="btn btn-primary px-4 rounded-pill" OnClick="btnSubmitPost_Click" />
                        </div>
                    </div>
                    <asp:Panel ID="pnlLoginToPost" runat="server" Visible="false" CssClass="mt-2">
                         <small class="text-muted">You must <a href="Login.aspx">login</a> to post.</small>
                    </asp:Panel>
                </asp:Panel>

                <div class="vstack gap-3">
                    <asp:Repeater ID="rptForum" runat="server" OnItemDataBound="rptForum_ItemDataBound" OnItemCommand="rptForum_ItemCommand">
                        <ItemTemplate>
                            <div class="card border-0 shadow-sm rounded-3">
                                <div class="card-body p-4">
                                    <div class="d-flex align-items-center mb-3">
                                        <img src='<%# Eval("ProfilePicPath") %>' class="rounded-circle me-3" style="width: 45px; height: 45px; object-fit: cover;" />
                                        <div>
                                            <h6 class="fw-bold mb-0 text-dark"><%# Eval("Username") %></h6>
                                            <small class="text-muted" style="font-size: 0.85rem;"><%# Eval("CreatedAt", "{0:MMM dd, yyyy HH:mm}") %></small>
                                        </div>
                                    </div>
                                    
                                    <p class="card-text fs-6 mb-3"><%# Eval("Content") %></p>

                                    <div class="bg-light p-3 rounded-3 mt-3">
                                        <asp:Repeater ID="rptComments" runat="server">
                                            <ItemTemplate>
                                                <div class="d-flex gap-2 mb-2 pb-2 border-bottom">
                                                    <img src='<%# Eval("ProfilePicPath") %>' class="rounded-circle" style="width: 30px; height: 30px; object-fit: cover;" />
                                                    <div>
                                                        <strong class="small text-dark"><%# Eval("Username") %></strong>
                                                        <span class="text-muted small ms-2"><%# Eval("CreatedAt", "{0:MMM dd, HH:mm}") %></span>
                                                        <p class="mb-0 small text-secondary"><%# Eval("Content") %></p>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                        <div class="d-flex gap-2 mt-3">
                                            <asp:TextBox ID="txtNewComment" runat="server" CssClass="form-control form-control-sm rounded-pill" placeholder="Write a reply..." autocomplete="off"></asp:TextBox>
                                            <asp:Button ID="btnPostComment" runat="server" Text="Reply" 
                                                CommandName="AddComment" CommandArgument='<%# Eval("PostID") %>'
                                                CssClass="btn btn-sm btn-outline-primary rounded-pill px-3" />
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblNoPosts" runat="server" Visible="false" CssClass="text-muted text-center py-4">No posts yet. Be the first to start a discussion!</asp:Label>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

    </div>
    
    <style>
        .text-primary { color: #667eea !important; }
        .shadow-xl { box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.175) !important; }
        .rounded-lg { border-radius: 0.5rem !important; }
    </style>
</asp:Content>