<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Categories.aspx.cs" Inherits="LexiPath.Categories" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Categories</h2>
        <p>Explore all our language learning categories.</p>
        <hr />
        
        <div class="row mb-3">
            <div class="col-md-6">
                <div class="input-group">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search categories..."></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                </div>
            </div>
        </div>

        <asp:Repeater ID="rptCategories" runat="server">
            <HeaderTemplate>
                <div class="row row-cols-1 row-cols-md-3 row-cols-lg-4 g-4">
            </HeaderTemplate>

            <ItemTemplate>
                <div class="col">
                    <div class="card h-100 shadow-sm text-decoration-none text-dark">
                        <a href='Courses.aspx?CategoryID=<%# Eval("CategoryID") %>' class="text-decoration-none text-dark">
                            
                            <img src='<%# GetImagePath(Eval("ImagePath")) %>' class="card-img-top" alt="<%# Eval("CategoryName") %>" style="height: 200px; object-fit: cover;">
                            
                            <div class="card-body text-center">
                                <h5 class="card-title">
                                    <%# Eval("CategoryName") %>
                                </h5>
                            </div>
                        </a>
                    </div>
                </div>
            </ItemTemplate>

            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>