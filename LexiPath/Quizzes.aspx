<%@ Page Title="Quizzes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Quizzes.aspx.cs" Inherits="LexiPath.Quizzes" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid px-4 py-4">

        <div class="row mb-4">
            <div class="col-12">
                <div class="p-4 rounded-3" style="background: linear-gradient(135deg, #764ba2 0%, #667eea 100%);">
                    <div class="d-flex justify-content-between align-items-center flex-wrap">
                        <div>
                            <h1 class="text-white mb-2">
                                <i class="bi bi-question-diamond-fill me-2"></i> Available Quizzes
                            </h1>
                            <p class="text-white-50 mb-0">Test your language skills and track your progress!</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row mb-4">
            <div class="col-12">
                <div class="card border-0 shadow-sm">
                    <div class="card-body">
                        <div class="row g-3 align-items-center"> 
                            <div class="col-md-6 col-lg-5"> 
                                <div class="input-group input-group-lg">
                                    <span class="input-group-text bg-white border-end-0">
                                        <i class="bi bi-search"></i>
                                    </span>
                                    <asp:TextBox ID="txtSearch" runat="server"
                                        CssClass="form-control border-start-0 ps-0"
                                        placeholder="Search quizzes by title or tag...">
                                    </asp:TextBox>
                                    <asp:Button ID="btnSearch" runat="server"
                                        Text="Search"
                                        CssClass="btn btn-primary px-4"
                                        OnClick="btnSearch_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <asp:Repeater ID="rptQuizzes" runat="server" OnItemDataBound="rptQuizzes_ItemDataBound">
            <HeaderTemplate>
                <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-xl-4 g-4">
            </HeaderTemplate>
            <ItemTemplate>
                <div class="col">
                    <div class="card h-100 border-0 shadow-sm hover-lift" style="transition: transform 0.3s ease, box-shadow 0.3s ease;">

                        <div class="position-relative overflow-hidden" style="height: 180px;">
                            <img src='<%# GetImagePath(Eval("ImagePath")) %>'
                                class="card-img-top w-100 h-100"
                                alt='<%# Eval("Title") %>'
                                style="object-fit: cover; transition: transform 0.3s ease;">
                            
                            <asp:Literal ID="litCompletedTag" runat="server" Visible="false"
                                Text="<div class='position-absolute top-0 end-0 m-3'><span class='badge bg-success px-3 py-2 shadow'><i class='bi bi-check-circle'></i> Completed</span></div>" />
                        </div>

                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title fw-bold text-center mb-2">
                                <%# Eval("Title") %>
                            </h5>
                            <p class="card-text text-muted flex-grow-1 text-center small"><%# Eval("Description") %></p>

                            <div class="mt-auto text-center">
                                <asp:HyperLink ID="lnkViewQuiz" runat="server"
                                    NavigateUrl='<%# "~/QuizDetail.aspx?QuizID=" + Eval("QuizID") %>'
                                    CssClass="btn btn-primary w-100 rounded-pill">
                                    <i class="bi bi-caret-right-fill"></i> Start Quiz
                                </asp:HyperLink>
                            </div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <asp:Panel ID="pnlNoQuizzes" runat="server" Visible="false" CssClass="text-center py-5">
            <div class="card border-0 shadow-sm mx-auto" style="max-width: 500px;">
                <div class="card-body p-5">
                    <i class="bi bi-emoji-frown display-1 text-muted mb-3"></i>
                    <h4 class="mb-3">No Quizzes Found</h4>
                    <p class="text-muted">We couldn't find any quizzes matching your criteria. Try adjusting your search.</p>
                    <asp:HyperLink runat="server" NavigateUrl="~/Quizzes.aspx"
                        CssClass="btn btn-outline-primary rounded-pill px-4 mt-2">
                        View All Quizzes
                    </asp:HyperLink>
                </div>
            </div>
        </asp:Panel>
    </div>

    <style>
        .hover-lift:hover {
            transform: translateY(-8px);
            box-shadow: 0 12px 24px rgba(0,0,0,0.15) !important;
        }

        .hover-lift:hover img {
            transform: scale(1.05);
        }

        .btn-outline-primary.active,
        .btn-outline-primary:hover,
        .btn-primary.active {
            background-color: #667eea;
            border-color: #667eea;
            color: white;
        }
        
    </style>
</asp:Content>