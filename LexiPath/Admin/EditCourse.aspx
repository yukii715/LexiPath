<%@ Page Title="Edit Course" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="EditCourse.aspx.cs" Inherits="LexiPath.Admin.EditCourse" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <asp:HiddenField ID="hdnCourseID" runat="server" Value="0" />
    
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1 class="mb-0">Edit Course Content</h1>
        <asp:HyperLink ID="hlBack" runat="server" NavigateUrl="~/Admin/ManageCourses.aspx" CssClass="btn btn-outline-secondary">
            <i class="bi bi-arrow-left"></i> Back to Courses
        </asp:HyperLink>
    </div>

    <asp:UpdatePanel ID="upCourseDetails" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="card shadow-sm border-0 mb-4">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <h5 class="mb-0">Course Details</h5>
                    <div class="d-flex gap-2">
                        <asp:Button ID="btnSaveDetails" runat="server" Text="Save Details" CssClass="btn btn-success" OnClick="btnSaveDetails_Click" />
                        <asp:Button ID="btnActivate" runat="server" Text="Activate Course" CssClass="btn btn-primary" OnClick="btnActivate_Click" Visible="false" />
                        <asp:Button ID="btnDeactivate" runat="server" Text="Deactivate Course" CssClass="btn btn-warning" OnClick="btnDeactivate_Click" Visible="false" OnClientClick="return confirmDeactivate(this);" />
                    </div>
                </div>
                <div class="card-body">
                    <div class="row g-3">
                        <div class="col-12">
                            <label class="form-label">Course Name</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Language</label>
                            <asp:DropDownList ID="ddlLanguage" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlLanguage_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Category</label>
                            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Type</label>
                            <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select" Enabled="false">
                                <asp:ListItem Value="Mixed">Mixed</asp:ListItem>
                                <asp:ListItem Value="Vocabulary">Vocabulary</asp:ListItem>
                                <asp:ListItem Value="Phrase">Phrase</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                         <div class="col-md-6">
                            <label class="form-label">Update Image (Optional)</label>
                            <asp:Image ID="imgPreview" runat="server" CssClass="img-fluid rounded mb-2" style="max-height: 80px; display: block;" ClientIDMode="Static" />
                            <asp:FileUpload ID="fileUploadImage" runat="server" CssClass="form-control" accept="image/*" onchange="previewImage(this, 'imgPreview');" />
                        </div>
                        <div class="col-12">
                            <label class="form-label">Description</label>
                            <asp:TextBox ID="txtDesc" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
             <asp:PostBackTrigger ControlID="btnSaveDetails" />
        </Triggers>
    </asp:UpdatePanel>

    <hr />

    <asp:UpdatePanel ID="upItems" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <h3 class="mb-3">Learning Items</h3>

            <div class="list-group shadow-sm">
                <asp:Repeater ID="rptItems" runat="server" OnItemDataBound="rptItems_ItemDataBound" OnItemCommand="rptItems_ItemCommand">
                    <ItemTemplate>
                        <div class="list-group-item">
                             <div class="d-flex w-100 justify-content-between align-items-center">
                                <div>
                                    <strong class="me-2">#<%# Eval("SequenceOrder") %></strong>
                                    <span class="badge bg-secondary me-2"><%# Eval("ItemType") %></span>
                                    <span><%# Eval("ItemType").ToString() == "Vocabulary" ? Eval("VocabText") : Eval("PhraseText") %></span>
                                </div>
                                <div>
                                    <asp:LinkButton ID="btnMoveUp" runat="server" CommandName="MoveUp" CommandArgument='<%# Eval("ItemID") %>' CssClass="btn btn-sm btn-outline-secondary me-1" ToolTip="Move Up">
                                        <i class="bi bi-arrow-up"></i>
                                    </asp:LinkButton>
                                     <asp:LinkButton ID="btnMoveDown" runat="server" CommandName="MoveDown" CommandArgument='<%# Eval("ItemID") %>' CssClass="btn btn-sm btn-outline-secondary me-3" ToolTip="Move Down">
                                        <i class="bi bi-arrow-down"></i>
                                    </asp:LinkButton>

                                     <%-- CHANGE: Use GetIconClass with SequenceOrder --%>
                                     <a class="btn btn-sm btn-info" data-bs-toggle="collapse" 
                                        href='<%# "#" + Container.FindControl("pnlItemCollapse").ClientID %>' 
                                        onclick="toggleIcon(this)">
                                         Edit <i class='<%# GetIconClass(Eval("SequenceOrder")) %>'></i>
                                    </a>
                                </div>
                            </div>

                            <%-- CHANGE: Use GetCollapseClass with SequenceOrder --%>
                            <asp:Panel ID="pnlItemCollapse" runat="server" CssClass='<%# GetCollapseClass(Eval("SequenceOrder")) %>'>
                                <hr />
                                <asp:HiddenField ID="hdnItemID" runat="server" Value='<%# Eval("ItemID") %>' />
                                <asp:HiddenField ID="hdnItemType" runat="server" Value='<%# Eval("ItemType") %>' />
                                <%-- CHANGE: Added HiddenField for Sequence --%>
                                <asp:HiddenField ID="hdnSequence" runat="server" Value='<%# Eval("SequenceOrder") %>' />
                                
                                <div class="row g-3">
                                    <asp:Panel ID="pnlVocabFields" runat="server" Visible="false" CssClass="col-12 row g-3">
                                        <div class="col-md-6">
                                            <label>Word</label>
                                            <asp:TextBox ID="txtVocab" runat="server" Text='<%# Eval("VocabText") %>' CssClass="form-control" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="col-md-6">
                                            <label>Meaning</label>
                                            <asp:TextBox ID="txtVocabMeaning" runat="server" Text='<%# Eval("VocabMeaning") %>' CssClass="form-control" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="col-12">
                                            <label>Image</label>
                                            <asp:Image ID="imgVocabPreview" runat="server" ImageUrl='<%# GetImagePath(Eval("VocabImagePath")) %>' 
                                                CssClass="img-fluid rounded mb-2 d-block" style="max-height: 80px;" />
                                            <asp:FileUpload ID="fileVocabImg" runat="server" CssClass="form-control" accept="image/*"
                                                onchange='<%# "previewImage(this, \"" + Container.FindControl("imgVocabPreview").ClientID + "\");" %>' />
                                        </div>
                                    </asp:Panel>

                                    <asp:Panel ID="pnlPhraseFields" runat="server" Visible="false" CssClass="col-12">
                                        <div class="row g-3 mb-3">
                                            <div class="col-12">
                                                <label class="form-label fw-bold">Phrase Text</label>
                                                <asp:TextBox ID="txtPhrase" runat="server" Text='<%# Eval("PhraseText") %>' CssClass="form-control" TextMode="MultiLine" Rows="2" autocomplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-12">
                                                <label class="form-label fw-bold">Meaning</label>
                                                <asp:TextBox ID="txtPhraseMeaning" runat="server" Text='<%# Eval("PhraseMeaning") %>' CssClass="form-control" autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="card bg-light border-0 p-3">
                                            <h6 class="card-title">Extra Details (Examples, Usage, etc.)</h6>
                                            
                                            <asp:Repeater ID="rptPhraseDetails" runat="server" OnItemCommand="rptPhraseDetails_ItemCommand">
                                                <ItemTemplate>
                                                    <div class="input-group mb-2">
                                                        <asp:HiddenField ID="hdnDetailID" runat="server" Value='<%# Eval("PhraseDetailID") %>' />
                                                        <span class="input-group-text">Type</span>
                                                        <asp:TextBox ID="txtDetailType" runat="server" Text='<%# Eval("DetailType") %>' CssClass="form-control" style="max-width: 120px;"
                                                            autocomplete="off" placeholder="e.g. Grammar"></asp:TextBox>
                                                        
                                                        <span class="input-group-text">Content</span>
                                                        <asp:TextBox ID="txtDetailContent" runat="server" Text='<%# Eval("Content") %>' CssClass="form-control" autocomplete="off"></asp:TextBox>
                                                        
                                                        <asp:Button ID="btnDeleteDetail" runat="server" Text="X" 
                                                            CommandName="DeleteDetail" CommandArgument='<%# Eval("PhraseDetailID") %>' 
                                                            CssClass="btn btn-outline-danger" 
                                                            OnClientClick='<%# "return confirmDetailDelete(this, \"" + Container.FindControl("txtDetailContent").ClientID + "\");" %>' />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>

                                            <div class="mt-2">
                                                <asp:Button ID="btnAddDetail" runat="server" Text="+ Add Detail Row" CommandName="AddDetail" CommandArgument='<%# Eval("ItemID") %>' CssClass="btn btn-sm btn-outline-primary" />
                                            </div>
                                        </div>
                                    </asp:Panel>
                            </div>
                                <div class="mt-3 text-end">
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="DeleteItem" CommandArgument='<%# Eval("ItemID") %>' CssClass="btn btn-danger me-2" OnClientClick="return confirmDelete(this);" />
                                    <asp:Button ID="btnSave" runat="server" Text="Save Item" CommandName="SaveItem" CommandArgument='<%# Eval("ItemID") %>' CssClass="btn btn-success" />
                                </div>
                            </asp:Panel>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <div class="mt-4 text-center">
                <asp:Panel ID="pnlAddButtons" runat="server">
                    <asp:LinkButton ID="btnAddVocab" runat="server" CssClass="btn btn-outline-primary btn-lg me-2" OnClick="btnAddVocab_Click">
                        <i class="bi bi-plus-lg"></i> Add Vocabulary
                    </asp:LinkButton>
                     <asp:LinkButton ID="btnAddPhrase" runat="server" CssClass="btn btn-outline-success btn-lg" OnClick="btnAddPhrase_Click">
                        <i class="bi bi-plus-lg"></i> Add Phrase
                    </asp:LinkButton>
                </asp:Panel>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

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
                toast: true, position: 'top', icon: type, title: message,
                showConfirmButton: false, timer: 4000, timerProgressBar: true,
                didOpen: (toast) => { toast.addEventListener('mouseenter', Swal.stopTimer); toast.addEventListener('mouseleave', Swal.resumeTimer); }
            });
        }
        function confirmDelete(button) {
            if (button.dataset.confirmed) { return true; }
            event.preventDefault();
            Swal.fire({
                title: 'Are you sure?', text: "You won't be able to revert this!",
                icon: 'warning', showCancelButton: true, confirmButtonColor: '#dc3545', cancelButtonColor: '#6c757d', confirmButtonText: 'Yes, delete it!'
            }).then((result) => { if (result.isConfirmed) { button.dataset.confirmed = "true"; button.click(); } });
            return false;
        }
        function confirmDeactivate(button) {
            if (button.dataset.confirmed) { return true; }
            event.preventDefault();
            Swal.fire({
                title: 'Are you sure?', text: "This will hide the course from users.",
                icon: 'warning', showCancelButton: true, confirmButtonColor: '#ffc107', cancelButtonColor: '#6c757d', confirmButtonText: 'Yes, deactivate it!'
            }).then((result) => { if (result.isConfirmed) { button.dataset.confirmed = "true"; button.click(); } });
            return false;
        }
        function confirmDetailDelete(button, inputId) {
            var input = document.getElementById(inputId);
            if (input && input.value.trim() === "") { return true; }
            return confirmDelete(button);
        }
        function toggleIcon(btn) {
            var icon = btn.querySelector('i');
            if (icon.classList.contains('bi-chevron-down')) {
                icon.classList.remove('bi-chevron-down'); icon.classList.add('bi-chevron-up');
            } else {
                icon.classList.remove('bi-chevron-up'); icon.classList.add('bi-chevron-down');
            }
        }
    </script>
</asp:Content>