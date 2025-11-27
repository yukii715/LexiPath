<%@ Page Title="Edit Quiz" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="EditQuiz.aspx.cs" Inherits="LexiPath.Admin.EditQuiz" MaintainScrollPositionOnPostback="true" EnableEventValidation="false" %>
<%@ Import Namespace="LexiPath.Data" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    
    <asp:HiddenField ID="hdnQuizID" runat="server" Value="0" />

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1 class="mb-0">Edit Quiz & Questions</h1>
        <asp:HyperLink ID="hlBackLink" runat="server" NavigateUrl="~/Admin/ManageQuizzes.aspx" CssClass="btn btn-outline-secondary">
            <i class="bi bi-arrow-left"></i> Back to List
        </asp:HyperLink>
    </div>

    <%-- (Keep Quiz Details Section exactly as it was) --%>
    <asp:UpdatePanel ID="upQuizDetails" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <%-- ... (Quiz Details Content skipped for brevity, it is unchanged) ... --%>
            <%-- COPY YOUR EXISTING QUIZ DETAILS SECTION HERE --%>
             <div class="card shadow-sm border-0 mb-4">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <h5 class="mb-0">Quiz Details</h5>
                    <div class="d-flex gap-2">
                        <asp:Button ID="btnUpdateQuizDetails" runat="server" Text="Save Details" CssClass="btn btn-success" OnClick="btnUpdateQuizDetails_Click" ValidationGroup="QuizDetails" />
                        <asp:Button ID="btnActivateQuiz" runat="server" Text="Activate Quiz" CssClass="btn btn-primary" OnClick="btnActivateQuiz_Click" Visible="false" />
                        <asp:Button ID="btnDeactivateQuiz" runat="server" Text="Deactivate Quiz" CssClass="btn btn-warning" OnClick="btnDeactivateQuiz_Click" Visible="false" OnClientClick="return confirmDeactivate(this);" />
                    </div>
                </div>
                <div class="card-body">
                    <div class="row g-3">
                        <div class="col-12">
                            <label class="form-label">Title</label>
                            <asp:TextBox ID="txtEditTitle" runat="server" CssClass="form-control" ValidationGroup="QuizDetails" autocomplete="off"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ErrorMessage="Title is required." ControlToValidate="txtEditTitle" CssClass="text-danger" Display="Dynamic" ValidationGroup="QuizDetails" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Language</label>
                            <asp:DropDownList ID="ddlEditLanguage" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlEditLanguage_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                             <label class="form-label">Update Image (Optional)</label>
                             <div class="d-flex align-items-center gap-3">
                                <asp:Image ID="imgEditPreview" runat="server" CssClass="img-fluid rounded" style="max-height: 60px; display: block;" ClientIDMode="Static" />
                                <asp:FileUpload ID="fileUploadEditImage" runat="server" CssClass="form-control" onchange="previewImage(this, 'imgEditPreview');" />
                             </div>
                        </div>
                        <div class="col-12">
                             <hr />
                            <asp:CheckBox ID="chkEditIsPractice" runat="server" ClientIDMode="Static" CssClass="d-none" AutoPostBack="true" OnCheckedChanged="chkEditIsPractice_CheckedChanged" />
                            <div class="form-check form-switch fs-5">
                                <input class="form-check-input" type="checkbox" role="switch" id="visibleToggle" onclick="triggerHiddenCheck();" />
                                <label class="form-check-label" for="visibleToggle" id="visibleToggleLabel">Quiz</label>
                            </div>
                        </div>
                        
                        <asp:Panel ID="pnlPracticeCourse" runat="server" CssClass="col-12 row g-3">
                            <div class="col-12">
                                <label class="form-label">Filter by Category</label>
                                <asp:DropDownList ID="ddlPracticeCategory" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlPracticeCategory_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div class="col-12">
                                <label class="form-label">Dedicated Practice Course</label>
                                <asp:DropDownList ID="ddlPracticeCourse" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                        </asp:Panel>
    
                        <asp:Panel ID="pnlRelatedCourses" runat="server" CssClass="col-12">
                            <label class="form-label fw-bold">Related Courses (Select at least 2)</label>
                            <div class="border p-3 rounded bg-light mb-3">
                                <small class="text-muted d-block mb-2">Selected Courses:</small>
                                <asp:Panel ID="pnlNoSelection" runat="server" Visible="false"><span class="text-muted fst-italic">No courses selected yet.</span></asp:Panel>
                                <div class="d-flex flex-wrap gap-2">
                                    <asp:Repeater ID="rptSelectedCourses" runat="server" OnItemCommand="rptSelectedCourses_ItemCommand">
                                        <ItemTemplate>
                                            <span class="badge bg-primary fs-6 p-2 d-flex align-items-center">
                                                <%# Eval("CourseName") %>
                                                <asp:LinkButton ID="btnRemoveCourse" runat="server" CommandName="RemoveCourse" CommandArgument='<%# Eval("CourseID") %>' CssClass="text-white ms-2 text-decoration-none"><i class="bi bi-x-lg"></i></asp:LinkButton>
                                            </span>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                            <div class="card">
                                <div class="card-header bg-white">
                                    <div class="row g-2">
                                        <div class="col-md-6">
                                            <label class="small text-muted">Filter by Category</label>
                                            <asp:DropDownList ID="ddlEditCategory" runat="server" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="ddlEditCategory_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body p-0" style="max-height: 250px; overflow-y: auto;">
                                    <asp:GridView ID="gvAvailableCourses" runat="server" AutoGenerateColumns="false" CssClass="table table-hover table-sm mb-0"
                                        ShowHeader="false" EmptyDataText="No courses found in this category." OnRowCommand="gvAvailableCourses_RowCommand">
                                        <Columns>
                                            <asp:BoundField DataField="CourseName" ItemStyle-CssClass="align-middle ps-3" />
                                            <asp:TemplateField ItemStyle-CssClass="text-end pe-3" ItemStyle-Width="80px">
                                                <ItemTemplate><asp:Button ID="btnAdd" runat="server" Text="Add" CommandName="AddCourse" CommandArgument='<%# Eval("CourseID") %>' CssClass="btn btn-sm btn-outline-success" /></ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                            <asp:CustomValidator ID="cvRelatedCourses" runat="server" ErrorMessage="Select at least 2 courses for a quiz." OnServerValidate="cvRelatedCourses_ServerValidate" CssClass="text-danger mt-2" Display="Dynamic" ValidationGroup="QuizDetails" />
                        </asp:Panel>

                         <div class="col-12">
                            <label class="form-label">Description</label>
                             <asp:TextBox ID="txtEditDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers><asp:PostBackTrigger ControlID="btnUpdateQuizDetails" /></Triggers>
    </asp:UpdatePanel>
    
    <hr />

    <%-- QUESTION MANAGEMENT --%>
    <asp:UpdatePanel ID="upQuestions" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h3 class="mb-0">Manage Questions</h3>
                <asp:Label ID="lblQuestionCountWarning" runat="server" CssClass="text-danger fw-bold" Visible="false" Text="<i class='bi bi-exclamation-triangle-fill'></i> Min 5 questions required." />
            </div>
            <div class="list-group shadow-sm">
                <asp:Repeater ID="rptQuestions" runat="server" OnItemDataBound="rptQuestions_ItemDataBound" OnItemCommand="rptQuestions_ItemCommand">
                    <ItemTemplate>
                        <div class="list-group-item" id='qrow_<%# Eval("QuestionID") %>'>
                            <div class="d-flex w-100 justify-content-between align-items-center">
                                <div>
                                    <strong class="me-2">#<%# Eval("SequenceOrder") %></strong>
                                    <span class="badge bg-secondary me-2"><%# Eval("QuestionTypeName") %></span>
                                    <span><%# Eval("QuestionText") %></span>
                                </div>
                                <div>
                                     <asp:LinkButton ID="btnMoveUp" runat="server" CommandName="MoveUp" CommandArgument='<%# Eval("QuestionID") %>' CssClass="btn btn-sm btn-outline-secondary me-1" ToolTip="Move Up"><i class="bi bi-arrow-up"></i></asp:LinkButton>
                                     <asp:LinkButton ID="btnMoveDown" runat="server" CommandName="MoveDown" CommandArgument='<%# Eval("QuestionID") %>' CssClass="btn btn-sm btn-outline-secondary me-3" ToolTip="Move Down"><i class="bi bi-arrow-down"></i></asp:LinkButton>
                                    <a class="btn btn-sm btn-info" data-bs-toggle="collapse" href='<%# "#" + Container.FindControl("pnlCollapse").ClientID %>' onclick="toggleIcon(this)">Edit <i class='<%# GetIconClass(Eval("SequenceOrder")) %>'></i></a>
                                </div>
                            </div>
                            <asp:Panel ID="pnlCollapse" runat="server" CssClass='<%# GetCollapseClass(Eval("SequenceOrder")) %>'>
                                <hr />
                                <asp:HiddenField ID="hdnQuestionID" runat="server" Value='<%# Eval("QuestionID") %>' />
                                <asp:HiddenField ID="hdnSequence" runat="server" Value='<%# Eval("SequenceOrder") %>' />
                                
                                <div class="row g-3">
                                    <div class="col-md-8">
                                         <label class="form-label">Question Text</label>
                                        <asp:TextBox ID="txtQuestionText" runat="server" Text='<%# Eval("QuestionText") %>' CssClass="form-control" TextMode="MultiLine" Rows="3" autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="col-md-4">
                                        <label class="form-label">Type</label>
                                        <asp:DropDownList ID="ddlQuestionType" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlQuestionType_SelectedIndexChanged"></asp:DropDownList>
                                        <label class="form-label mt-2">Image</label>
                                        <asp:Image ID="imgQuestionPreview" runat="server" ImageUrl='<%# GetImagePath(Eval("ImagePath")) %>' CssClass="img-fluid rounded mb-2 d-block" style="max-height: 60px;" />
                                        <asp:FileUpload ID="fileQuestionImage" runat="server" CssClass="form-control" onchange='<%# "previewImage(this, \"" + Container.FindControl("imgQuestionPreview").ClientID + "\");" %>' />
                                    </div>
                                    
                                    <%-- OPTIONS SECTION --%>
                                    <asp:Panel ID="pnlMcqOptions" runat="server" Visible="false" CssClass="col-12">
                                        <div class="card bg-light border-0 p-3">
                                            <h6 class="card-title">Answer Options</h6>
                                            
                                            <asp:Repeater ID="rptOptions" runat="server">
                                                <ItemTemplate>
                                                    <div class="input-group mb-2">
                                                        <div class="input-group-text">
                                                             <%-- ADDED: Hidden field to track ID --%>
                                                             <asp:HiddenField ID="hdnOptionID" runat="server" Value='<%# Eval("OptionID") %>' />
                                                             
                                                             <asp:RadioButton ID="rbCorrect" runat="server" 
                                                                 GroupName='<%# "Correct_" + DataBinder.Eval(Container.NamingContainer.NamingContainer, "DataItem.QuestionID") %>' 
                                                                 Checked='<%# Eval("IsCorrect") %>' 
                                                                 onclick='<%# "selectSingleRadio(this, " + DataBinder.Eval(Container.NamingContainer.NamingContainer, "DataItem.QuestionID") + ");" %>' />
                                                        </div>
                                                        <asp:TextBox ID="txtOptionText" runat="server" Text='<%# Eval("OptionText") %>' CssClass="form-control" autocomplete="off"></asp:TextBox>
                                                        
                                                        <asp:Button ID="btnRemoveOption" runat="server" Text="X" CssClass="btn btn-outline-danger" 
                                                            OnClick="btnRemoveOption_Click" 
                                                            CommandArgument='<%# Eval("OptionID") %>' 
                                                            CausesValidation="false" />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>

                                            <div class="mt-2">
                                                 <asp:Button ID="btnAddOption" runat="server" Text="+ Add Option" CssClass="btn btn-outline-primary btn-sm" CommandName="AddOption" CausesValidation="false" CommandArgument='<%# Eval("QuestionID") %>' />
                                            </div>
                                       </div>
                                    </asp:Panel>

                                    <asp:Panel ID="pnlTextInputAnswer" runat="server" Visible="false" CssClass="col-12">
                                         <label class="form-label">Correct Answer</label>
                                         <asp:TextBox ID="txtCorrectAnswer" runat="server" Text='<%# Eval("CorrectAnswer") %>' CssClass="form-control" autocomplete="off"></asp:TextBox>
                                     </asp:Panel>
                                </div>
                                <div class="mt-3 text-end">
                                    <asp:Button ID="btnDeleteQuestion" runat="server" Text="Delete" CssClass="btn btn-danger me-2" CommandName="DeleteQuestion" CommandArgument='<%# Eval("QuestionID") %>' CausesValidation="false" OnClientClick="return confirmDelete(this);" />
                                    <asp:Button ID="btnSaveQuestion" runat="server" Text="Save Question" CssClass="btn btn-success" CommandName="SaveQuestion" CommandArgument='<%# Eval("QuestionID") %>' />
                                </div>
                            </asp:Panel>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div class="mt-4 text-center">
                 <asp:LinkButton ID="btnAddNewQuestionSimple" runat="server" CssClass="btn btn-outline-primary btn-lg" OnClick="btnAddNewQuestionSimple_Click" CausesValidation="false"><i class='bi bi-plus-lg'></i> Add New Question</asp:LinkButton>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <script type="text/javascript">
        function selectSingleRadio(rb, questionId) {
            var container = document.getElementById('qrow_' + questionId);
            if (container) {
                var radios = container.getElementsByTagName('input');
                for (var i = 0; i < radios.length; i++) {
                    if (radios[i].type == 'radio' && radios[i] != rb) {
                        radios[i].checked = false;
                    }
                }
            }
        }

        // --- SCROLL FIX (Color Removed) ---
        var targetScrollId = null;
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
            if (targetScrollId) {
                var el = document.getElementById(targetScrollId);
                if (el) {
                    // Simply scroll, no color flashing
                    el.scrollIntoView({ behavior: 'smooth', block: 'center' });
                }
                targetScrollId = null;
            }
            updateUIToggle();
        });
        function setScrollTarget(id) { targetScrollId = 'qrow_' + id; }

        // ... (Other standard helpers) ...
        function previewImage(input, previewId) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) { var img = document.getElementById(previewId); if (img) { img.src = e.target.result; img.style.display = 'block'; } }
                reader.readAsDataURL(input.files[0]);
            }
        }
        function triggerHiddenCheck() { var h = document.getElementById('chkEditIsPractice'); if (h) h.click(); }
        function updateUIToggle() {
            var h = document.getElementById('chkEditIsPractice'); var v = document.getElementById('visibleToggle'); var l = document.getElementById('visibleToggleLabel');
            if (h && v) { v.checked = h.checked; l.innerText = h.checked ? "Practice" : "Quiz"; }
        }
        function showNotification(type, message) {
            Swal.fire({ toast: true, position: 'top', icon: type, title: message, showConfirmButton: false, timer: 4000, timerProgressBar: true, didOpen: (toast) => { toast.addEventListener('mouseenter', Swal.stopTimer); toast.addEventListener('mouseleave', Swal.resumeTimer); } });
        }
        function confirmDelete(b) { if (b.dataset.confirmed) return true; event.preventDefault(); Swal.fire({ title: 'Are you sure?', icon: 'warning', showCancelButton: true, confirmButtonColor: '#dc3545', confirmButtonText: 'Yes, delete it!' }).then((r) => { if (r.isConfirmed) { b.dataset.confirmed = "true"; b.click(); } }); return false; }
        function confirmDeactivate(b) { if (b.dataset.confirmed) return true; event.preventDefault(); Swal.fire({ title: 'Are you sure?', icon: 'warning', showCancelButton: true, confirmButtonColor: '#ffc107', confirmButtonText: 'Yes, deactivate!' }).then((r) => { if (r.isConfirmed) { b.dataset.confirmed = "true"; b.click(); } }); return false; }
        function toggleIcon(b) { var i = b.querySelector('i'); if (i.classList.contains('bi-chevron-down')) { i.classList.remove('bi-chevron-down'); i.classList.add('bi-chevron-up'); } else { i.classList.remove('bi-chevron-up'); i.classList.add('bi-chevron-down'); } }
        Sys.Application.add_load(updateUIToggle);
    </script>
</asp:Content>