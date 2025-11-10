<%@ Page Title="Edit Quiz" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="EditQuiz.aspx.cs" Inherits="LexiPath.Admin.EditQuiz" %>
<%@ Import Namespace="LexiPath.Data" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    
    <asp:HiddenField ID="hdnQuizID" runat="server" Value="0" />

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1 class="mb-0">Edit Quiz & Questions</h1>
        <asp:HyperLink ID="hlBackLink" runat="server" NavigateUrl="~/Admin/ManageQuizzes.aspx" CssClass="btn btn-outline-secondary">
            <i class="bi bi-arrow-left"></i> Back to List
        </asp:HyperLink>
    </div>

    <asp:UpdatePanel ID="upQuizDetails" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="card shadow-sm border-0 mb-4">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <h5 class="mb-0">Quiz Details</h5>
                    <asp:Button ID="btnUpdateQuizDetails" runat="server" Text="Save Details" CssClass="btn btn-success" OnClick="btnUpdateQuizDetails_Click" ValidationGroup="QuizDetails" />
                </div>
                <div class="card-body">
                    <asp:Label ID="lblDetailMessage" runat="server" EnableViewState="false" CssClass="mb-3"></asp:Label>
                    <div class="row g-3">
                        <div class="col-12">
                            <label class="form-label">Title</label>
                            <asp:TextBox ID="txtEditTitle" runat="server" CssClass="form-control" ValidationGroup="QuizDetails"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ErrorMessage="Title is required." ControlToValidate="txtEditTitle" CssClass="text-danger" Display="Dynamic" ValidationGroup="QuizDetails" />
                        </div>
                        <div class="col-12">
                            <label class="form-label">Description</label>
                            <asp:TextBox ID="txtEditDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Language</label>
                            <asp:DropDownList ID="ddlEditLanguage" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Update Image (Optional)</label>
                            <asp:Image ID="imgEditPreview" runat="server" CssClass="img-fluid rounded mb-2" style="max-height: 100px; display: block;" />
                            <asp:FileUpload ID="fileUploadEditImage" runat="server" CssClass="form-control" />
                        </div>

                        <div class="col-12">
                            <hr />
                            <%-- REQUIREMENT: Toggle for Practice/Quiz --%>
                            <div class="form-check form-switch fs-5">
                                <asp:CheckBox ID="chkEditIsPractice" runat="server" CssClass="form-check-input" role="switch" AutoPostBack="true" OnCheckedChanged="chkEditIsPractice_CheckedChanged" />
                                <label class="form-check-label" for="<%= chkEditIsPractice.ClientID %>">This is a Practice</label>
                            </div>
                        </div>

                        <asp:Panel ID="pnlPracticeCourse" runat="server" Visible="false" CssClass="row g-3">
                            <div class="col-md-6">
                                <label class="form-label">Dedicated Course</label>
                                <asp:DropDownList ID="ddlPracticeCourse" runat="server" CssClass="form-select">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvPracticeCourse" runat="server"
                                    ControlToValidate="ddlPracticeCourse"
                                    InitialValue="0"
                                    ErrorMessage="A Practice must have one dedicated course."
                                    CssClass="text-danger" Display="Dynamic" ValidationGroup="QuizDetails"
                                    Enabled="false" />
                            </div>
                        </asp:Panel>

                        <%-- REQUIREMENT: Conditional Panel for Related Courses --%>
                        <asp:Panel ID="pnlRelatedCourses" runat="server" CssClass="row g-3" Visible="false">
                            <div class="col-md-6">
                                <%-- REQUIREMENT: CheckBoxList for Courses --%>
                                <label class="form-label">Related Courses (Must select at least 2 for a quiz)</label>
                                <div class="form-control" style="height: 200px; overflow-y: auto;">
                                    <asp:CheckBoxList ID="cblEditCourses" runat="server" CssClass="form-check-list" RepeatLayout="Table" RepeatColumns="1"></asp:CheckBoxList>
                                </div>
                                <%-- REQUIREMENT: Validation for 2 courses --%>
                                <asp:CustomValidator ID="cvRelatedCourses" runat="server" ErrorMessage="A quiz must have at least two related courses."
                                    OnServerValidate="cvRelatedCourses_ServerValidate" CssClass="text-danger" Display="Dynamic" ValidationGroup="QuizDetails" />
                            </div>
                        </asp:Panel>
                        
                        <div class="col-md-6">
                            <%-- REQUIREMENT: CheckBoxList for Tags --%>
                            <label class="form-label">Related Tags</label>
                            <div class="form-control" style="height: 200px; overflow-y: auto;">
                                <asp:CheckBoxList ID="cblEditTags" runat="server" CssClass="form-check-list" RepeatLayout="Table" RepeatColumns="1"></asp:CheckBoxList>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUpdateQuizDetails" />
        </Triggers>
    </asp:UpdatePanel>

    <hr />

    <%-- =================================================================== --%>
    <%--                 QUESTIONS MANAGEMENT SECTION                        --%>
    <%-- =================================================================== --%>

    <asp:UpdatePanel ID="upQuestions" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h3 class="mb-0">Manage Questions</h3>
                 <%-- REQUIREMENT: At least 5 questions warning --%>
                <asp:Label ID="lblQuestionCountWarning" runat="server" CssClass="text-danger fw-bold" Visible="false"
                    Text="<i class='bi bi-exclamation-triangle-fill'></i> This quiz must have at least 5 questions to be valid." />
            </div>

            <asp:Label ID="lblQuestionMessage" runat="server" EnableViewState="false" CssClass="mb-3"></asp:Label>
            
            <div class="list-group shadow-sm" id="questionListAccordion">
                <%-- REQUIREMENT: Question List with re-ordering --%>
                <asp:Repeater ID="rptQuestions" runat="server" OnItemDataBound="rptQuestions_ItemDataBound" OnItemCommand="rptQuestions_ItemCommand">
                    <ItemTemplate>
                        <div class="list-group-item">
                            <div class="d-flex w-100 justify-content-between align-items-center">
                                <div>
                                    <%-- REQUIREMENT: Sequence, Text, Type --%>
                                    <strong class="me-2">#<%# Eval("SequenceOrder") %></strong>
                                    <span class="me-3"><%# Eval("QuestionText", "{0:S30}...") %></span>
                                    <span class="badge bg-secondary"><%# Eval("QuestionTypeName") %></span>
                                </div>
                                <div>
                                    <%-- REQUIREMENT: Up/Down arrows --%>
                                    <asp:LinkButton ID="btnMoveUp" runat="server" CommandName="MoveUp" CommandArgument='<%# Eval("QuestionID") %>' CssClass="btn btn-sm btn-outline-secondary me-1" ToolTip="Move Up">
                                        <i class="bi bi-arrow-up"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnMoveDown" runat="server" CommandName="MoveDown" CommandArgument='<%# Eval("QuestionID") %>' CssClass="btn btn-sm btn-outline-secondary me-3" ToolTip="Move Down">
                                        <i class="bi bi-arrow-down"></i>
                                    </asp:LinkButton>
                                    
                                    <%-- REQUIREMENT: Click to drop down --%>
                                    <a class="btn btn-sm btn-info" data-bs-toggle="collapse" href='<%# "#qCollapse" + Eval("QuestionID") %>' role="button" aria-expanded="false" aria-controls='<%# "qCollapse" + Eval("QuestionID") %>'>
                                        Edit <i class="bi bi-chevron-down"></i>
                                    </a>
                                </div>
                            </div>

                            <%-- REQUIREMENT: Collapsible Edit Panel --%>
                            <div class="collapse" id='<%# "qCollapse" + Eval("QuestionID") %>'>
                                <hr />
                                <asp:HiddenField ID="hdnQuestionID" runat="server" Value='<%# Eval("QuestionID") %>' />
                                <div class="row g-3">
                                    <div class="col-md-8">
                                        <label class="form-label">Question Text</label>
                                        <asp:TextBox ID="txtQuestionText" runat="server" Text='<%# Eval("QuestionText") %>' CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                    </div>
                                    <div class="col-md-4">
                                        <label class="form-label">Question Type</label>
                                        <asp:DropDownList ID="ddlQuestionType" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlQuestionType_SelectedIndexChanged"></asp:DropDownList>
                                        
                                        <label class="form-label mt-2">Update Image (Optional)</label>
                                        <asp:Image ID="imgQuestionPreview" runat="server" ImageUrl='<%# GetImagePath(Eval("ImagePath")) %>' CssClass="img-fluid rounded mb-2" style="max-height: 80px; display: block;" />
                                        <asp:FileUpload ID="fileQuestionImage" runat="server" CssClass="form-control" />
                                    </div>

                                    <%-- REQUIREMENT: MCQ Options Panel --%>
                                    <asp:Panel ID="pnlMcqOptions" runat="server" Visible="false" CssClass="col-12">
                                        <fieldset class="border p-3">
                                            <legend class="float-none w-auto px-2 fs-6">Answer Options</legend>
                                            <asp:Label ID="lblOptionError" runat="server" CssClass="text-danger" EnableViewState="false"></asp:Label>
                                            
                                            <asp:Repeater ID="rptOptions" runat="server" OnItemCommand="rptOptions_ItemCommand">
                                                <ItemTemplate>
                                                    <div class="input-group mb-2">
                                                        <div class="input-group-text">
                                                            <asp:RadioButton ID="rbCorrect" runat="server" GroupName='<%# "CorrectAnswerGroup_" + Eval("QuestionID") %>' Checked='<%# Eval("IsCorrect") %>' />
                                                        </div>
                                                        <asp:TextBox ID="txtOptionText" runat="server" Text='<%# Eval("OptionText") %>' CssClass="form-control"></asp:TextBox>
                                                        <asp:Button ID="btnRemoveOption" runat="server" Text="Remove" CssClass="btn btn-outline-danger" CommandName="RemoveOption" CommandArgument='<%# Eval("OptionID") %>' CausesValidation="false" />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            
                                            <asp:Button ID="btnAddOption" runat="server" Text="Add Option" CssClass="btn btn-outline-primary btn-sm mt-2" CommandName="AddOption" CausesValidation="false" CommandArgument='<%# Eval("QuestionID") %>' />
                                        </fieldset>
                                    </asp:Panel>
                                    
                                    <%-- For other types like 'TypeInAnswer' --%>
                                    <asp:Panel ID="pnlTextInputAnswer" runat="server" Visible="false" CssClass="col-12">
                                         <label class="form-label">Correct Answer</label>
                                         <asp:TextBox ID="txtCorrectAnswer" runat="server" Text='<%# Eval("CorrectAnswer") %>' CssClass="form-control"></asp:TextBox>
                                    </asp:Panel>

                                </div>
                                <hr />
                                <div class="d-flex justify-content-end gap-2">
                                    <asp:Button ID="btnDeleteQuestion" runat="server" Text="Delete Question" CssClass="btn btn-danger" CommandName="DeleteQuestion" CommandArgument='<%# Eval("QuestionID") %>' CausesValidation="false" OnClientClick="return confirm('Are you sure you want to delete this question?');" />
                                    <asp:Button ID="btnSaveQuestion" runat="server" Text="Save Question" CssClass="btn btn-success" CommandName="SaveQuestion" CommandArgument='<%# Eval("QuestionID") %>' />
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <%-- Add New Question Form --%>
            <div class="card shadow-sm border-0 mt-4">
                <div class="card-header">
                    <h5 class="mb-0">Add New Question</h5>
                </div>
                <div class="card-body">
                    <div class="row g-3">
                        <div class="col-md-8">
                            <label class="form-label">Question Text</label>
                            <asp:TextBox ID="txtNewQuestionText" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">Question Type</label>
                            <asp:DropDownList ID="ddlNewQuestionType" runat="server" CssClass="form-select"></asp:DropDownList>
                            
                            <label class="form-label mt-2">Image (Optional)</label>
                            <asp:FileUpload ID="fileNewQuestionImage" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                </div>
                <div class="card-footer text-end">
                    <asp:Button ID="btnAddQuestion" runat="server" Text="Add This Question" CssClass="btn btn-primary" OnClick="btnAddQuestion_Click" />
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>