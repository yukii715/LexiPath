<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ManageQuizzes.aspx.cs" Inherits="LexiPath.Admin.ManageQuizzes" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1 class="mb-0">Manage Quizzes & Practices</h1>
        <button type="button" class="btn btn-primary btn-lg" data-bs-toggle="modal" data-bs-target="#addQuizModal">
            <i class="bi bi-plus-lg"></i> New Quiz/Practice
        </button>
    </div>
    
    <div class="card shadow-sm border-0">
        <div class="card-body">
            <asp:GridView ID="gvQuizzes" runat="server" 
                AutoGenerateColumns="False" 
                CssClass="table table-hover align-middle"
                DataKeyNames="QuizID"
                EmptyDataText="No quizzes or practices found."
                AllowSorting="True" 
                OnSorting="gvQuizzes_Sorting"
                OnRowCommand="gvQuizzes_RowCommand">
                
                <HeaderStyle CssClass="table-light" />
                <Columns>
                    <asp:BoundField DataField="QuizID" HeaderText="ID" ReadOnly="True" SortExpression="QuizID" />
                    <asp:TemplateField HeaderText="Image">
                        <ItemTemplate>
                            <asp:Image ID="imgQuiz" runat="server" 
                                ImageUrl='<%# GetImagePath(Eval("ImagePath")) %>' 
                                CssClass="img-fluid rounded" style="width: 100px; height: 60px; object-fit: cover;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
                    <asp:BoundField DataField="LanguageName" HeaderText="Language" SortExpression="LanguageName" />
                    <asp:TemplateField HeaderText="Type" SortExpression="IsPractice">
                        <ItemTemplate>
                            <span class="badge <%# (bool)Eval("IsPractice") ? "bg-info text-dark" : "bg-primary" %>">
                                <%# (bool)Eval("IsPractice") ? "Practice" : "Quiz" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                     <asp:BoundField DataField="RelatedCourses" HeaderText="Related Courses" />
                    
                    <%-- TAGS COLUMN REMOVED --%>
                    
                    <asp:TemplateField HeaderText="Status" SortExpression="isActivated">
                        <ItemTemplate>
                            <span class="badge <%# (bool)Eval("isActivated") ? "bg-success" : "bg-danger" %>">
                                <%# (bool)Eval("isActivated") ? "Active" : "Deactivated" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEdit" runat="server"
                                CommandName="EditQuiz"
                                CommandArgument='<%# Eval("QuizID") %>'
                                ToolTip="Edit Details & Manage Questions"
                                Text="Edit"
                                CssClass="btn btn-secondary btn-sm"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <%-- ADD NEW MODAL --%>
    <div class="modal fade" id="addQuizModal" tabindex="-1" aria-labelledby="addQuizModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upAddQuiz" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="addQuizModalLabel">New Quiz / Practice</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <div class="row g-3">
                                <div class="col-12">
                                    <label class="form-label">Title</label>
                                    <asp:TextBox ID="txtNewTitle" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="col-12">
                                    <label class="form-label">Description</label>
                                    <asp:TextBox ID="txtNewDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Language</label>
                                    <asp:DropDownList ID="ddlNewLanguage" runat="server" CssClass="form-select" AutoPostBack="True" OnSelectedIndexChanged="ddlNewLanguage_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Image (Optional)</label>
                                    <div class="mb-2 text-center">
                                        <asp:Image ID="imgAddPreview" runat="server" CssClass="img-fluid rounded shadow-sm" 
                                            style="max-height: 150px; display: none;" ClientIDMode="Static" />
                                    </div>
                                    <asp:FileUpload ID="fileUploadNewImage" runat="server" CssClass="form-control" onchange="previewImage(this, 'imgAddPreview');" />
                                </div>
                                <div class="col-12">
                                    <hr />
                                    <asp:CheckBox ID="chkNewIsPractice" runat="server" ClientIDMode="Static" CssClass="d-none" />
                                    <div class="form-check form-switch fs-5">
                                        <input class="form-check-input" type="checkbox" role="switch" id="visibleToggleNew" onclick="syncNewToggle(this);" />
                                        <label class="form-check-label" for="visibleToggleNew" id="visibleToggleLabelNew">Quiz</label>
                                    </div>
                                </div>

                                <%-- PRACTICE PANEL (With Category Filter) --%>
                                <asp:Panel ID="pnlNewPracticeCourse" runat="server" CssClass="row g-3" style="display: none;">
                                    <div class="col-md-6">
                                        <label class="form-label">Filter by Category</label>
                                        <asp:DropDownList ID="ddlNewPracticeCategory" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlNewPracticeCategory_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                    <div class="col-md-6">
                                        <label class="form-label">Dedicated Course</label>
                                        <asp:DropDownList ID="ddlNewPracticeCourse" runat="server" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </asp:Panel>

                                <%-- QUIZ PANEL (With Bubbles + Filter + Grid) --%>
                                <asp:Panel ID="pnlNewRelatedCourses" runat="server" CssClass="col-12">
                                    <label class="form-label fw-bold">Related Courses (Select at least 2)</label>
                                    
                                    <%-- A. Selected Bubbles --%>
                                    <div class="border p-3 rounded bg-light mb-3">
                                        <small class="text-muted d-block mb-2">Selected:</small>
                                        <asp:Panel ID="pnlNewNoSelection" runat="server" Visible="true">
                                            <span class="text-muted fst-italic">None selected.</span>
                                        </asp:Panel>
                                        <div class="d-flex flex-wrap gap-2">
                                            <asp:Repeater ID="rptNewSelectedCourses" runat="server" OnItemCommand="rptNewSelectedCourses_ItemCommand">
                                                <ItemTemplate>
                                                    <span class="badge bg-primary fs-6 p-2 d-flex align-items-center">
                                                        <%# Eval("CourseName") %>
                                                        <asp:LinkButton ID="btnRemove" runat="server" CommandName="RemoveCourse" CommandArgument='<%# Eval("CourseID") %>' CssClass="text-white ms-2 text-decoration-none"><i class="bi bi-x-lg"></i></asp:LinkButton>
                                                    </span>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>

                                    <%-- B. Filter & Available Grid --%>
                                    <div class="card">
                                        <div class="card-header bg-white">
                                            <label class="small text-muted">Filter by Category</label>
                                            <asp:DropDownList ID="ddlNewCategory" runat="server" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="ddlNewCategory_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                        <div class="card-body p-0" style="max-height: 200px; overflow-y: auto;">
                                            <asp:GridView ID="gvNewAvailableCourses" runat="server" AutoGenerateColumns="false" CssClass="table table-hover table-sm mb-0"
                                                ShowHeader="false" EmptyDataText="No courses found." OnRowCommand="gvNewAvailableCourses_RowCommand">
                                                <Columns>
                                                    <asp:BoundField DataField="CourseName" ItemStyle-CssClass="align-middle ps-3" />
                                                    <asp:TemplateField ItemStyle-CssClass="text-end pe-3" ItemStyle-Width="80px">
                                                        <ItemTemplate>
                                                            <asp:Button ID="btnAdd" runat="server" Text="Add" CommandName="AddCourse" CommandArgument='<%# Eval("CourseID") %>' CssClass="btn btn-sm btn-outline-success" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </asp:Panel>
                                
                                <%-- TAG PANEL REMOVED --%>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnAddQuiz" runat="server" Text="Create" CssClass="btn btn-primary" OnClick="btnAddQuiz_Click" />
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnAddQuiz" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    <script type="text/javascript">
        function previewImage(input, previewId) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var img = document.getElementById(previewId);
                    if (img) { img.src = e.target.result; img.style.display = 'block'; }
                }
                reader.readAsDataURL(input.files[0]);
            }
        }
        function showNotification(type, message) {
            Swal.fire({
                toast: true, position: 'top', icon: type, title: message,
                showConfirmButton: false, timer: 4000, timerProgressBar: true,
                didOpen: (toast) => { toast.addEventListener('mouseenter', Swal.stopTimer); toast.addEventListener('mouseleave', Swal.resumeTimer); }
            });
        }
        var addQuizModalInstance = null;
        function showAddModal() {
            cleanupBackdrops();
            if (addQuizModalInstance) { addQuizModalInstance.dispose(); addQuizModalInstance = null; }
            var modalEl = document.getElementById('addQuizModal');
            if (modalEl) { addQuizModalInstance = new bootstrap.Modal(modalEl); addQuizModalInstance.show(); }
        }
        function hideAddModal() {
            if (addQuizModalInstance) { addQuizModalInstance.hide(); }
            cleanupBackdrops();
        }
        function cleanupBackdrops() {
            document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
            document.body.classList.remove('modal-open');
            document.body.style = '';
        }
        function initializeAddModal() {
            var hiddenCheck = document.getElementById('chkNewIsPractice');
            var visibleCheck = document.getElementById('visibleToggleNew');
            if (!hiddenCheck || !visibleCheck) return;
            visibleCheck.checked = hiddenCheck.checked;
            updateNewUI();
        }
        function syncNewToggle(visibleCheckbox) {
            var hiddenCheck = document.getElementById('chkNewIsPractice');
            if (!hiddenCheck) return;
            hiddenCheck.checked = visibleCheckbox.checked;
            updateNewUI();
        }
        function updateNewUI() {
            var hiddenCheck = document.getElementById('chkNewIsPractice');
            var visibleLabel = document.getElementById('visibleToggleLabelNew');
            var panelPractice = $get('<%= pnlNewPracticeCourse.ClientID %>');
            var panelQuiz = $get('<%= pnlNewRelatedCourses.ClientID %>');
            if (!hiddenCheck || !visibleLabel || !panelPractice || !panelQuiz) return;
            if (hiddenCheck.checked) {
                panelPractice.style.display = 'flex'; panelQuiz.style.display = 'none'; visibleLabel.textContent = 'Practice';
            } else {
                panelPractice.style.display = 'none'; panelQuiz.style.display = 'block'; visibleLabel.textContent = 'Quiz';
            }
        }
        Sys.Application.add_load(initializeAddModal);
    </script>
</asp:Content>