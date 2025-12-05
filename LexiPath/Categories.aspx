<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Categories.aspx.cs" Inherits="LexiPath.Categories" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid px-4 py-4">

        <div class="row mb-4">
            <div class="col-12">
                <div class="p-4 rounded-3" style="background: linear-gradient(135deg, #764ba2 0%, #667eea 100%);">
                    <div class="d-flex justify-content-between align-items-center flex-wrap">
                        <div>
                            <h1 class="text-white mb-2">
                                <i class="bi bi-grid-3x3-gap-fill me-2"></i> Explore Categories
                            </h1>
                            <p class="text-white-50 mb-0">Discover the perfect language path for your learning journey.</p>
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
                                        placeholder="Search categories...">
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

        <asp:Repeater ID="rptCategories" runat="server">
            <HeaderTemplate>
                <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-xl-4 g-4">
            </HeaderTemplate>

            <ItemTemplate>
                <div class="col">
                    <div class="card h-100 border-0 shadow-sm hover-lift" style="transition: transform 0.3s ease, box-shadow 0.3s ease;">
                        <a href='Courses.aspx?CategoryID=<%# Eval("CategoryID") %>' class="text-decoration-none text-dark d-block">

                            <div class="overflow-hidden" style="height: 200px;">
                                <img src='<%# GetImagePath(Eval("ImagePath")) %>'
                                    class="card-img-top w-100 h-100"
                                    alt="<%# Eval("CategoryName") %>"
                                    style="object-fit: cover; transition: transform 0.3s ease;">
                            </div>

                            <div class="card-body text-center d-flex align-items-center justify-content-center" style="min-height: 80px;">
                                <h5 class="card-title fw-bold text-primary mb-0">
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

        <asp:Panel ID="pnlNoCategories" runat="server" Visible="false" CssClass="text-center py-5">
            <div class="card border-0 shadow-sm mx-auto" style="max-width: 500px;">
                <div class="card-body p-5">
                    <i class="bi bi-x-octagon display-1 text-danger mb-3"></i>
                    <h4 class="mb-3">No Categories Found</h4>
                    <p class="text-muted">We couldn't find any categories matching your search. Please try a different term.</p>
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
        

        .text-primary {
            color: #667eea !important; 
        }
    </style>
</asp:Content>