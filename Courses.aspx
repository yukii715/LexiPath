<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Courses.aspx.cs" Inherits="LexiPath.Courses" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        
        <h2><asp:Literal ID="litCategoryName" runat="server" Text="Courses"></asp:Literal></h2>
        <p>
            <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="~/Categories.aspx">&larr; Back to all categories</asp:HyperLink>
        </p>
        <hr />

        <div class="row mb-3">
            <div class="col-md-6">
                <div class="input-group">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search courses..."></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                </div>
            </div>
        </div>

        <ul class="nav nav-pills mb-3">
            <li class="nav-item">
                <asp:HyperLink ID="lnkAll" runat="server" CssClass="nav-link" NavigateUrl="~/Courses.aspx?type=All">All</asp:HyperLink>
            </li>
            <li class="nav-item">
                <asp:HyperLink ID="lnkVocab" runat="server" CssClass="nav-link" NavigateUrl="~/Courses.aspx?type=Vocabulary">Vocabulary</asp:HyperLink>
            </li>
            <li class="nav-item">
                <asp:HyperLink ID="lnkPhrase" runat="server" CssClass="nav-link" NavigateUrl="~/Courses.aspx?type=Phrase">Phrase</asp:HyperLink>
            </li>
            <li class="nav-item">
                <asp:HyperLink ID="lnkMixed" runat="server" CssClass="nav-link" NavigateUrl="~/Courses.aspx?type=Mixed">Mixed</asp:HyperLink>
            </li>
        </ul>
        
        <asp:Repeater ID="rptCourses" runat="server" OnItemDataBound="rptCourses_ItemDataBound">
            <HeaderTemplate>
                <div class="row row-cols-1 row-cols-md-3 row-cols-lg-4 g-4">
            </HeaderTemplate>

            <ItemTemplate>
                <div class="col">
                    <div class="card h-100 shadow-sm">
            
                        <a href='<%# GetDetailUrl(Eval("CourseID")) %>'>
                            <img src='<%# GetImagePath(Eval("ImagePath")) %>' class="card-img-top" alt="<%# Eval("CourseName") %>" style="height: 200px; object-fit: cover;">
                        </a>
            
                        &nbsp;&nbsp;<div class="card-body text-center">
                            <h5 class="card-title">
                                <%# Eval("CourseName") %>
                            </h5>
                            <asp:Literal ID="litCompletedTag" runat="server" Visible="false"
                                Text="<span class='badge bg-success mb-2'>Completed</span>" />
                        </div>

                        <div class="card-footer d-flex justify-content-around">
                            <a href='<%# GetDetailUrl(Eval("CourseID")) %>' class="btn btn-sm btn-outline-primary">
                                View Course
                            </a>
                        </div>
            
                    </div>
                </div>
            </ItemTemplate>

            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <asp:Panel ID="pnlNoCourses" runat="server" Visible="false" CssClass="alert alert-info">
            No courses were found in this category.
        </asp:Panel>
    </div>
</asp:Content>