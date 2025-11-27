<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ManageCourses.aspx.cs" Inherits="LexiPath.Admin.ManageCourses" MaintainScrollPositionOnPostback="true" %>
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
                            <asp:Image ID="imgCourse" runat="server" 
                                ImageUrl='<%# GetImagePath(Eval("ImagePath")) %>' 
                                CssClass="img-fluid rounded" style="width: 100px; height: 60px; object-fit: cover;" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="CourseName" HeaderText="Course Name" SortExpression="CourseName" />
                    <asp:BoundField DataField="CategoryName" HeaderText="Category" SortExpression="CategoryName" />
                    <asp:BoundField DataField="LanguageName" HeaderText="Language" SortExpression="LanguageName" />
                    <asp:BoundField DataField="CourseType" HeaderText="Type" SortExpression="CourseType" />
                    
                    <asp:TemplateField HeaderText="Status" SortExpression="isActivated">
                        <ItemTemplate>
                            <span class="badge <%# (bool)Eval("isActivated") ? "bg-success" : "bg-danger"%>">
                                <%# (bool)Eval("isActivated") ? "Active" : "Deactivated" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <%-- Edit Button: Redirects to EditCourse.aspx --%>
                            <asp:Button ID="btnEdit" runat="server" CommandName="EditCourse" 
                                CommandArgument='<%# Eval("CourseID") %>' Text="Edit" 
                                CssClass="btn btn-secondary btn-sm" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <%-- ADD NEW COURSE MODAL --%>
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
                            <div class="row g-3">
                                <div class="col-12">
                                    <label class="form-label">Course Name</label>
                                    <%-- UPDATED: autocomplete="off" --%>
                                    <asp:TextBox ID="txtNewCourseName" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
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
                                <div class="col-12">
                                    <label class="form-label">Course Image</label>
                                    
                                    <div class="mb-2 text-center">
                                        <asp:Image ID="imgAddPreview" runat="server" CssClass="img-fluid rounded shadow-sm" 
                                            style="max-height: 150px; display: none;" ClientIDMode="Static" />
                                    </div>

                                    <asp:FileUpload ID="fileUploadCourse" runat="server" CssClass="form-control" 
                                        onchange="previewImage(this, 'imgAddPreview');" />
                                </div>
                                <div class="col-12">
                                    <label class="form-label">Description</label>
                                    <%-- UPDATED: autocomplete="off" --%>
                                    <asp:TextBox ID="txtNewDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnAddCourse" runat="server" Text="Add Course" CssClass="btn btn-primary" OnClick="btnAddCourse_Click" />
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <script type="text/javascript">
        function previewImage(input, previewId) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var img = document.getElementById(previewId);
                    if (img) {
                        img.src = e.target.result;
                        img.style.display = 'block';
                    }
                }
                reader.readAsDataURL(input.files[0]);
            }
        }

        function showNotification(type, message) {
            Swal.fire({
                toast: true,
                position: 'top',
                icon: type,
                title: message,
                showConfirmButton: false,
                timer: 4000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.addEventListener('mouseenter', Swal.stopTimer)
                    toast.addEventListener('mouseleave', Swal.resumeTimer)
                }
            });
        }

        var addCourseModalInstance = null;

        function showAddModal() {
            cleanupBackdrops();
            if (addCourseModalInstance) {
                addCourseModalInstance.dispose();
                addCourseModalInstance = null;
            }
            var modalEl = document.getElementById('addCourseModal');
            if (modalEl) {
                addCourseModalInstance = new bootstrap.Modal(modalEl);
                addCourseModalInstance.show();
            }
        }

        function hideAddModal() {
            if (addCourseModalInstance) {
                addCourseModalInstance.hide();
            }
            cleanupBackdrops();
        }

        function cleanupBackdrops() {
            document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
            document.body.classList.remove('modal-open');
            document.body.style = '';
        }

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
        });
    </script>

</asp:Content>