<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ManageQuizzes.aspx.cs" Inherits="LexiPath.Admin.ManageQuizzes" %>
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
                     <asp:BoundField DataField="RelatedTags" HeaderText="Tags" />
                    <asp:TemplateField HeaderText="Status" SortExpression="IsDeleted">
                        <ItemTemplate>
                            <span class="badge <%# (bool)Eval("IsDeleted") ? "bg-danger" : "bg-success" %>">
                                <%# (bool)Eval("IsDeleted") ? "Deleted" : "Active" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
        
                            <asp:HyperLink ID="hlManageQuestions" runat="server"
                                NavigateUrl='<%# "ManageQuizQuestions.aspx?QuizID=" + Eval("QuizID") %>'
                                Text="Questions"
                                CssClass="btn btn-info btn-sm" />
                            
                            <asp:Button ID="btnEdit" runat="server" 
                                CommandName="ShowEditModal" 
                                CommandArgument='<%# Eval("QuizID") %>' 
                                Text="Edit" 
                                CssClass="btn btn-secondary btn-sm" 
                                Enabled='<%# !(bool)Eval("IsDeleted") %>' />
                            
                            <asp:Button ID="btnDelete" runat="server" 
                                CommandName="DeleteQuiz" 
                                CommandArgument='<%# Eval("QuizID") %>' 
                                Text="Delete"
                                CssClass="btn btn-danger btn-sm" 
                                Enabled='<%# !(bool)Eval("IsDeleted") %>'
                                OnClientClick="return confirm('Are you sure? This will hide the quiz/practice.');" />
                                
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

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
                            <asp:Label ID="lblAddMessage" runat="server" EnableViewState="false" CssClass="mb-3"></asp:Label>
                            <div class="row g-3">
                                <div class="col-12">
                                    <label class="form-label">Title</label>
                                    <asp:TextBox ID="txtNewTitle" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-12">
                                    <label class="form-label">Description</label>
                                    <asp:TextBox ID="txtNewDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Language</label>
                                    <asp:DropDownList ID="ddlNewLanguage" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Image</label>
                                    <asp:FileUpload ID="fileUploadNewImage" runat="server" CssClass="form-control" />
                                </div>
                                <div class="col-12">
                                    <asp:CheckBox ID="chkNewIsPractice" runat="server" Text=" This is a Practice (not a Quiz)" CssClass="form-check-label" />
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Related Courses (Hold Ctrl to select multiple)</label>
                                    <asp:ListBox ID="lstNewCourses" runat="server" CssClass="form-control" SelectionMode="Multiple" Rows="6"></asp:ListBox>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Related Tags (Hold Ctrl to select multiple)</label>
                                    <asp:ListBox ID="lstNewTags" runat="server" CssClass="form-control" SelectionMode="Multiple" Rows="6"></asp:ListBox>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnAddQuiz" runat="server" Text="Add Quiz" CssClass="btn btn-primary" OnClick="btnAddQuiz_Click" />
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnAddQuiz" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    <div class="modal fade" id="editQuizModal" tabindex="-1" aria-labelledby="editQuizModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upEditQuiz" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="editQuizModalLabel">Edit Quiz / Practice</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="lblEditMessage" runat="server" EnableViewState="false" CssClass="mb-3"></asp:Label>
                            <asp:HiddenField ID="hdnEditQuizID" runat="server" Value="0" />
                            <div class="row g-3">
                                <div class="col-12">
                                    <label class="form-label">Title</label>
                                    <asp:TextBox ID="txtEditTitle" runat="server" CssClass="form-control"></asp:TextBox>
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
                                    <asp:Image ID="imgEditPreview" runat="server" CssClass="img-fluid rounded mb-2" style="max-height: 100px;" />
                                    <asp:FileUpload ID="fileUploadEditImage" runat="server" CssClass="form-control" />
                                </div>
                                <div class="col-12">
                                    <asp:CheckBox ID="chkEditIsPractice" runat="server" Text=" This is a Practice (not a Quiz)" CssClass="form-check-label" />
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Related Courses (Hold Ctrl to select multiple)</label>
                                    <asp:ListBox ID="lstEditCourses" runat="server" CssClass="form-control" SelectionMode="Multiple" Rows="6"></asp:ListBox>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Related Tags (Hold Ctrl to select multiple)</label>
                                    <asp:ListBox ID="lstEditTags" runat="server" CssClass="form-control" SelectionMode="Multiple" Rows="6"></asp:ListBox>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnUpdateQuiz" runat="server" Text="Update Quiz" CssClass="btn btn-success" OnClick="btnUpdateQuiz_Click" />
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnUpdateQuiz" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
    
</asp:Content>