<%@ Page Title="Quizzes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Quizzes.aspx.cs" Inherits="LexiPath.Quizzes" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Available Quizzes</h2>
        <p>Test your knowledge with our quizzes! You don't need to complete a course first.</p>
        <hr />

        <div class="row mb-3">
            <div class="col-md-6">
                <div class="input-group">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search quizzes by title or tag..."></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                </div>
            </div>
            <%-- You can add a filter for IsPractice here later if needed --%>
        </div>

        <asp:Panel ID="pnlNoQuizzes" runat="server" Visible="false" CssClass="alert alert-info">
            <p>No quizzes found matching your criteria. Try a different search term!</p>
        </asp:Panel>

        <asp:Repeater ID="rptQuizzes" runat="server" OnItemDataBound="rptQuizzes_ItemDataBound">
            <HeaderTemplate>
                <div class="row row-cols-1 row-cols-md-2 row-cols-lg-3 g-4">
            </HeaderTemplate>
            <ItemTemplate>
                <div class="col">
                    <div class="card h-100 shadow-sm">
                        <img src='<%# GetImagePath(Eval("ImagePath")) %>' class="card-img-top" alt='<%# Eval("Title") %>' style="height: 180px; object-fit: cover;">
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title"><%# Eval("Title") %></h5>
                            <asp:Literal ID="litCompletedTag" runat="server" Visible="false"
                                Text="<span class='badge bg-success mb-2'>Completed</span>" />
                            <p class="card-text flex-grow-1 text-muted"><%# Eval("Description") %></p>
                            <div class="mt-auto">
                                <asp:HyperLink ID="lnkViewQuiz" runat="server" 
                                    NavigateUrl='<%# "~/QuizDetail.aspx?QuizID=" + Eval("QuizID") %>' 
                                    CssClass="btn btn-info btn-sm">
                                    View Quiz
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
    </div>
</asp:Content>