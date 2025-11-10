<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ManageCourses.aspx.cs" Inherits="LexiPath.Admin.ManageCourses" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1 class="mb-0">Manage Courses</h1>
        <button type="button" class="btn btn-primary btn-lg" data-bs-toggle="modal" data-bs-target="#addCourseModal">
            <i class="bi bi-plus-lg"></i> New Course
        </button>
    </div>
    
    <div class="card shadow-sm border-0">
        <div class="card-body">
            <asp:GridView ID="gvCourses" runat="server" 
                AutoGenerateColumns="False" 
                CssClass="table table-hover align-middle"
                DataKeyNames="CourseID"
                EmptyDataText="No courses found."
                AllowSorting="True" 
                OnSorting="gvCourses_Sorting"
                OnRowCommand="gvCourses_RowCommand"
                OnRowDataBound="gvCourses_RowDataBound">
                
                <HeaderStyle CssClass="table-light" />
                <Columns>
                    <asp:BoundField DataField="CourseID" HeaderText="ID" ReadOnly="True" SortExpression="CourseID" />
                    <asp:TemplateField HeaderText="Image">
                        <ItemTemplate>
                            <asp:Image ID="imgCourse" runat="server" ImageUrl='<%# GetImagePath(Eval("ImagePath")) %>' 
                                CssClass="img-fluid rounded" style="width: 100px; height: 60px; object-fit: cover;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CourseName" HeaderText="Course Name" SortExpression="CourseName" />
                    <asp:BoundField DataField="CategoryName" HeaderText="Category" SortExpression="CategoryName" />
                    <asp:BoundField DataField="LanguageName" HeaderText="Language" SortExpression="LanguageName" />
                    <asp:BoundField DataField="CourseType" HeaderText="Type" SortExpression="CourseType" />
                    <asp:TemplateField HeaderText="Status" SortExpression="IsDeleted">
                        <ItemTemplate>
                            <span class="badge <%# (bool)Eval("IsDeleted") ? "bg-danger" : "bg-success" %>">
                                <%# (bool)Eval("IsDeleted") ? "Deleted" : "Active" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:Button ID="btnEdit" runat="server" CommandName="ShowEditModal" 
                                CommandArgument='<%# Eval("CourseID") %>' Text="Edit" 
                                CssClass="btn btn-secondary btn-sm" Enabled='<%# !(bool)Eval("IsDeleted") %>' />
                            <asp:Button ID="btnDelete" runat="server" CommandName="DeleteCourse"
                                CommandArgument='<%# Eval("CourseID") %>' Text="Delete"
                                CssClass="btn btn-danger btn-sm" Enabled='<%# !(bool)Eval("IsDeleted") %>'
                                OnClientClick="return confirm('Are you sure?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <div class="modal fade" id="addCourseModal" tabindex="-1" aria-labelledby="addCourseModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upAddCourse" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="addCourseModalLabel">New Course</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="lblAddMessage" runat="server" EnableViewState="false" CssClass="mb-3"></asp:Label>
                            <div class="row g-3">
                                <div class="col-12">
                                    <label class="form-label">Course Name</label>
                                    <asp:TextBox ID="txtNewCourseName" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Language</label>
                                    <asp:DropDownList ID="ddlNewLanguage" runat="server" CssClass="form-select" 
                                        AutoPostBack="True" OnSelectedIndexChanged="ddlNewLanguage_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Category</label>
                                    <asp:DropDownList ID="ddlNewCategory" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Course Type</label>
                                    <asp:DropDownList ID="ddlNewCourseType" runat="server" CssClass="form-select">
                                        <asp:ListItem Value="Mixed">Mixed</asp:ListItem>
                                        <asp:ListItem Value="Vocabulary">Vocabulary</asp:ListItem>
                                        <asp:ListItem Value="Phrase">Phrase</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Course Image</label>
                                    <asp:FileUpload ID="fileUploadCourse" runat="server" CssClass="form-control" />
                                </div>
                                <div class="col-12">
                                    <label class="form-label">Description</label>
                                    <asp:TextBox ID="txtNewDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnAddCourse" runat="server" Text="Add Course" CssClass="btn btn-primary" OnClick="btnAddCourse_Click" />
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnAddCourse" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    
    <div class="modal fade" id="editCourseModal" tabindex="-1" aria-labelledby="editCourseModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upEditCourse" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="editCourseModalLabel">Edit Course</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="lblEditMessage" runat="server" EnableViewState="false" CssClass="mb-3"></asp:Label>
                            <asp:HiddenField ID="hdnEditCourseID" runat="server" Value="0" />
                            <div class="row g-3">
                                <div class="col-12">
                                    <label class="form-label">Course Name</label>
                                    <asp:TextBox ID="txtEditCourseName" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Language</label>
                                    <asp:DropDownList ID="ddlEditLanguage" runat="server" CssClass="form-select" 
                                        AutoPostBack="True" OnSelectedIndexChanged="ddlEditLanguage_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Category</label>
                                    <asp:DropDownList ID="ddlEditCategory" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Course Type</label>
                                    <asp:DropDownList ID="ddlEditCourseType" runat="server" CssClass="form-select">
                                        <asp:ListItem Value="Mixed">Mixed</asp:ListItem>
                                        <asp:ListItem Value="Vocabulary">Vocabulary</asp:ListItem>
                                        <asp:ListItem Value="Phrase">Phrase</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Update Image (Optional)</label>
                                    <asp:Image ID="imgEditPreview" runat="server" CssClass="img-fluid rounded shadow-sm mb-2" style="max-height: 100px;" />
                                    <asp:FileUpload ID="fileUploadEditCourse" runat="server" CssClass="form-control" />
                                </div>
                                <div class="col-12">
                                    <label class="form-label">Description</label>
                                    <asp:TextBox ID="txtEditDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnUpdateCourse" runat="server" Text="Update Course" CssClass="btn btn-success" OnClick="btnUpdateCourse_Click" />
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnUpdateCourse" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>