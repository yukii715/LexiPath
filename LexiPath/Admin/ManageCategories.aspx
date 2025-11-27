<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ManageCategories.aspx.cs" Inherits="LexiPath.Admin.ManageCategories" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1 class="mb-0">Manage Categories</h1>
        <button type="button" class="btn btn-primary btn-lg" data-bs-toggle="modal" data-bs-target="#addCategoryModal">
            <i class="bi bi-plus-lg"></i> New Category
        </button>
    </div>
    
    <div class="card shadow-sm border-0">
        <div class="card-body">
            <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:GridView ID="gvCategories" runat="server" 
                        AutoGenerateColumns="False" 
                        CssClass="table table-hover align-middle"
                        DataKeyNames="CategoryID"
                        EmptyDataText="No categories found."
                        AllowSorting="True" 
                        OnSorting="gvCategories_Sorting"
                        OnRowCommand="gvCategories_RowCommand"
                        OnRowDataBound="gvCategories_RowDataBound">
                        
                        <HeaderStyle CssClass="table-light" />
                        <Columns>
                            <asp:BoundField DataField="CategoryID" HeaderText="ID" ReadOnly="True" SortExpression="CategoryID" />
                            
                            <asp:TemplateField HeaderText="Image">
                                <ItemTemplate>
                                    <asp:Image ID="imgCategory" runat="server" 
                                        ImageUrl='<%# GetImagePath(Eval("ImagePath")) %>' 
                                        CssClass="img-fluid rounded" 
                                        style="width: 100px; height: 60px; object-fit: cover;" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="CategoryName" HeaderText="Category Name" SortExpression="CategoryName" />
                            <asp:BoundField DataField="LanguageName" HeaderText="Language" SortExpression="LanguageName" />
                            
                            <asp:TemplateField HeaderText="Status" SortExpression="isActivated">
                                <ItemTemplate>
                                    <span class="badge <%# (bool)Eval("isActivated") ? "bg-success" : "bg-danger"%>">
                                        <%# (bool)Eval("isActivated") ? "Active" : "Deactivated" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:Button ID="btnEdit" runat="server" 
                                        CommandName="ShowEditModal" 
                                        CommandArgument='<%# Eval("CategoryID") %>'
                                        Text="Edit" 
                                        CssClass="btn btn-secondary btn-sm" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <%-- ADD CATEGORY MODAL --%>
    <div class="modal fade" id="addCategoryModal" tabindex="-1" aria-labelledby="addCategoryModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upAddCategory" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="addCategoryModalLabel">New Category</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="lblAddMessage" runat="server" EnableViewState="false" CssClass="mb-3"></asp:Label>
                            <div class="row g-3">
                                <div class="col-md-6">
                                    <label class="form-label">Category Name</label>
                                    <%-- UPDATED: autocomplete="off" --%>
                                    <asp:TextBox ID="txtNewCategoryName" runat="server" CssClass="form-control" placeholder="e.g., Business (Korean)" autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Language</label>
                                    <asp:DropDownList ID="ddlNewLanguage" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>
                                <div class="col-12">
                                    <label class="form-label">Category Image</label>
                                    
                                    <div class="mb-2 text-center">
                                        <asp:Image ID="imgAddPreview" runat="server" CssClass="img-fluid rounded shadow-sm" 
                                            style="max-height: 150px; display: none;" ClientIDMode="Static" />
                                    </div>

                                    <asp:FileUpload ID="fileUploadCategory" runat="server" CssClass="form-control" 
                                        onchange="previewImage(this, 'imgAddPreview');" />
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnAddCategory" runat="server" Text="Add Category" CssClass="btn btn-primary" OnClick="btnAddCategory_Click" />
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <%-- EDIT CATEGORY MODAL --%>
    <div class="modal fade" id="editCategoryModal" tabindex="-1" aria-labelledby="editCategoryModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upEditCategory" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="editCategoryModalLabel">Edit Category</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="lblEditMessage" runat="server" EnableViewState="false" CssClass="mb-3"></asp:Label>
                            <asp:HiddenField ID="hdnEditCategoryID" runat="server" Value="0" />
            
                            <div class="row g-3">
                                <div class="col-md-6">
                                    <label class="form-label">Category Name</label>
                                    <%-- UPDATED: autocomplete="off" --%>
                                    <asp:TextBox ID="txtEditCategoryName" runat="server" CssClass="form-control" autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Language</label>
                                    <asp:DropDownList ID="ddlEditLanguage" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>
                                
                                <div class="col-12">
                                    <label class="form-label">Update Image (Optional)</label>
                                    
                                    <div class="mb-2 text-center">
                                        <asp:Image ID="imgEditPreview" runat="server" CssClass="img-fluid rounded shadow-sm" 
                                            style="max-height: 150px;" ClientIDMode="Static" />
                                    </div>

                                    <asp:FileUpload ID="fileUploadEditCategory" runat="server" CssClass="form-control" 
                                        onchange="previewImage(this, 'imgEditPreview');" />
                                    
                                    <small class="text-muted">Only select a file if you want to replace the current image.</small>
                                </div>
                            </div>
                        </div>
                        
                        <div class="modal-footer d-flex justify-content-between">
                            <%-- LEFT SIDE: ACTIVATE / DEACTIVATE BUTTONS --%>
                            <div>
                                <asp:Button ID="btnActivateCategory" runat="server" Text="Activate Category" 
                                    CssClass="btn btn-success" OnClick="btnActivateCategory_Click" Visible="false" />

                                <asp:Button ID="btnDeactivateCategory" runat="server" Text="Deactivate Category" 
                                    CssClass="btn btn-warning" OnClick="btnDeactivateCategory_Click" Visible="false" 
                                    OnClientClick="return confirmDeactivation(this);" />
                            </div>
                            
                            <%-- RIGHT SIDE: CANCEL / UPDATE --%>
                            <div>
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                <asp:Button ID="btnUpdateCategory" runat="server" Text="Update Category" CssClass="btn btn-primary" OnClick="btnUpdateCategory_Click" />
                            </div>
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

        function hideModal(modalId) {
            var modalEl = document.getElementById(modalId);
            var modal = bootstrap.Modal.getInstance(modalEl);
            if (modal) {
                modal.hide();
            }
            document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
            document.body.classList.remove('modal-open');
            document.body.style = '';
        }

        function confirmDeactivation(button) {
            if (button.dataset.confirmed) {
                return true;
            }
            event.preventDefault();

            Swal.fire({
                title: 'Are you sure?',
                text: "You are about to deactivate this category.",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#ffc107',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Yes, deactivate it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    button.dataset.confirmed = "true";
                    button.click();
                }
            });

            return false;
        }
    </script>

</asp:Content>