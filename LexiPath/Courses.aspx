<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Courses.aspx.cs" Inherits="LexiPath.Courses" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid px-4 py-4">
        
        <div class="row mb-4">
            <div class="col-12">
                <div class="p-4 rounded-3" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);">
                    <div class="d-flex justify-content-between align-items-center flex-wrap">
                        <div>
                            <h1 class="text-white mb-2">
                                <asp:Literal ID="litCategoryName" runat="server" Text="All Courses"></asp:Literal>
                            </h1>
                            <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="~/Categories.aspx" 
                                CssClass="text-white text-decoration-none" Visible="false">
                                <i class="bi bi-arrow-left"></i> Back to all categories
                            </asp:HyperLink>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row mb-4">
            <div class="col-12">
                <div class="p-3 bg-white rounded-3 shadow-sm">
                    <div class="row g-3 align-items-center">
                        <div class="col-md-6 col-lg-5">
                            <div class="input-group input-group-lg">
                                <span class="input-group-text bg-white border-end-0 border-primary">
                                    <i class="bi bi-search"></i>
                                </span>
                                <asp:TextBox ID="txtSearch" runat="server" 
                                    CssClass="form-control border-start-0 ps-0 border-primary" 
                                    placeholder="Search courses...">
                                </asp:TextBox>
                                <asp:Button ID="btnSearch" runat="server" 
                                    Text="Search" 
                                    CssClass="btn btn-primary px-4" 
                                    OnClick="btnSearch_Click" />
                            </div>
                        </div>
                        
                        <div class="col-md-6 col-lg-7">
                            <div class="d-flex gap-2 flex-wrap justify-content-md-end">
                                <asp:HyperLink ID="lnkAll" runat="server" 
                                    CssClass="btn btn-primary rounded-pill px-4" 
                                    NavigateUrl="~/Courses.aspx?type=All">
                                    All Courses
                                </asp:HyperLink>
                                <asp:HyperLink ID="lnkVocab" runat="server" 
                                    CssClass="btn btn-outline-primary rounded-pill px-4" 
                                    NavigateUrl="~/Courses.aspx?type=Vocabulary">
                                    <i class="bi bi-book"></i> Vocabulary
                                </asp:HyperLink>
                                <asp:HyperLink ID="lnkPhrase" runat="server" 
                                    CssClass="btn btn-outline-primary rounded-pill px-4" 
                                    NavigateUrl="~/Courses.aspx?type=Phrase">
                                    <i class="bi bi-chat-quote"></i> Phrases
                                </asp:HyperLink>
                                <asp:HyperLink ID="lnkMixed" runat="server" 
                                    CssClass="btn btn-outline-primary rounded-pill px-4" 
                                    NavigateUrl="~/Courses.aspx?type=Mixed">
                                    <i class="bi bi-collection"></i> Mixed
                                </asp:HyperLink>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <asp:Repeater ID="rptCourses" runat="server" OnItemDataBound="rptCourses_ItemDataBound">
            <HeaderTemplate>
                <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-xl-4 g-4">
            </HeaderTemplate>
            <ItemTemplate>
                <div class="col">
                    <div class="card h-100 border-0 shadow-sm hover-lift" style="transition: transform 0.3s ease, box-shadow 0.3s ease;">
                        
                        <div class="position-relative overflow-hidden" style="height: 220px;">
                            <a href='<%# GetDetailUrl(Eval("CourseID")) %>' class="d-block h-100">
                                <img src='<%# GetImagePath(Eval("ImagePath")) %>' 
                                    class="card-img-top w-100 h-100" 
                                    alt="<%# Eval("CourseName") %>" 
                                    style="object-fit: cover; transition: transform 0.3s ease;">
                            </a>
                            
                            <asp:Literal ID="litCompletedTag" runat="server" Visible="false"
                                Text="<div class='position-absolute top-0 end-0 m-3'><span class='badge bg-success px-3 py-2 shadow'><i class='bi bi-check-circle'></i> Completed</span></div>" />
                        </div>
                        
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title mb-3 text-center fw-bold">
                                <%# Eval("CourseName") %>
                            </h5>
                            
                            <div class="mt-auto text-center">
                                <a href='<%# GetDetailUrl(Eval("CourseID")) %>' 
                                    class="btn btn-primary w-100 rounded-pill">
                                    <i class="bi bi-play-circle"></i> Start Learning
                                </a>
                            </div>
                        </div>
                        
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <asp:Panel ID="pnlNoCourses" runat="server" Visible="false" CssClass="text-center py-5">
            <div class="card border-0 shadow-sm mx-auto" style="max-width: 500px;">
                <div class="card-body p-5">
                    <i class="bi bi-inbox display-1 text-muted mb-3"></i>
                    <h4 class="mb-3">No Courses Found</h4>
                    <p class="text-muted">We couldn't find any courses matching your criteria. Try adjusting your search or filter.</p>
                    <asp:HyperLink runat="server" NavigateUrl="~/Courses.aspx" 
                        CssClass="btn btn-outline-primary rounded-pill px-4 mt-2">
                        View All Courses
                    </asp:HyperLink>
                </div>
            </div>
        </asp:Panel>
    </div>

    <style>
        /* Card Hover Effect */
        .hover-lift:hover {
            transform: translateY(-8px);
            box-shadow: 0 12px 24px rgba(0,0,0,0.15) !important;
        }
        .hover-lift:hover img {
            transform: scale(1.05);
        }
        
        /* Filter Button Transitions */
        .btn {
            transition: all 0.3s ease;
        }

        /* SHARED STYLES FOR ALIGNMENT (The Fix) */
        .btn-primary, .btn-outline-primary {
            display: inline-flex !important; /* Forces Flexbox */
            align-items: center;             /* Vertically centers text/icon */
            justify-content: center;         /* Horizontally centers content */
            gap: 8px;                        /* Adds nice space between icon and text */
            min-width: 120px;                /* Optional: Ensures buttons have a uniform minimum width */
        }

        /* ACTIVE STATE (Filled) */
        .btn-primary {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border: none;
            color: white;
        }
        .btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(102, 126, 234, 0.4);
        }

        /* INACTIVE STATE (Outline) */
        .btn-outline-primary {
            color: #667eea;
            border: 2px solid #667eea; /* Made border slightly thicker for better visibility */
            background: transparent;
            padding-top: 6px;    /* Micro-adjustment for border width difference */
            padding-bottom: 6px;
        }
        .btn-outline-primary:hover {
            background-color: #f3f4fd;
            color: #667eea;
            transform: translateY(-2px);
        }
    </style>
</asp:Content>