<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="ManageCategories.aspx.cs" Inherits="LexiPath.Admin.ManageCategories" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1 class="mb-0">Manage Categories</h1>
        <button type="button" class="btn btn-primary btn-lg" data-bs-toggle="modal" data-bs-target="#addCategoryModal">
            <i class="bi bi-plus-lg"></i> New Category
        </button>
    </div>
    
    <div class="card shadow-sm border-0">
        <div class="card-body">
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
                    
                    <asp:TemplateField HeaderText="Status" SortExpression="IsDeleted">
                        <ItemTemplate>
                            <span class="badge <%# (bool)Eval("IsDeleted") ? "bg-danger" : "bg-success" %>">
                                <%# (bool)Eval("IsDeleted") ? "Deleted" : "Active" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:Button ID="btnEdit" runat="server" 
                                CommandName="ShowEditModal" 
                                CommandArgument='<%# Eval("CategoryID") %>'
                                Text="Edit" 
                                CssClass="btn btn-secondary btn-sm" 
                                Enabled='<%# !(bool)Eval("IsDeleted") %>' />
                                
                            <asp:Button ID="btnDelete" runat="server"
                                CommandName="DeleteCategory"
                                CommandArgument='<%# Eval("CategoryID") %>'
                                Text="Delete"
                                CssClass="btn btn-danger btn-sm"
                                Enabled='<%# !(bool)Eval("IsDeleted") %>'
                                OnClientClick="return confirm('Are you sure?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <div class="modal fade" id="addCategoryModal" tabindex="-1" aria-labelledby="addCategoryModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upAddCategory" runat="server">
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
                                    <asp:TextBox ID="txtNewCategoryName" runat="server" CssClass="form-control" placeholder="e.g., Business (Korean)"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Language</label>
                                    <asp:DropDownList ID="ddlNewLanguage" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>
                                <div class="col-12">
                                    <label class="form-label">Category Image</label>
                                    <asp:FileUpload ID="fileUploadCategory" runat="server" CssClass="form-control" />
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnAddCategory" runat="server" Text="Add Category" CssClass="btn btn-primary" OnClick="btnAddCategory_Click" />
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnAddCategory" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    
    <div class="modal fade" id="editCategoryModal" tabindex="-1" aria-labelledby="editCategoryModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upEditCategory" runat="server">
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
                                    <asp:TextBox ID="txtEditCategoryName" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Language</label>
                                    <asp:DropDownList ID="ddlEditLanguage" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>
                                
                                <div class="col-12">
                                    <label class="form-label">Update Image (Optional)</label>
                                    <p class="text-muted small">Current Image:</p>
                                    <asp:Image ID="imgEditPreview" runat="server" CssClass="img-fluid rounded shadow-sm mb-2" style="max-height: 100px;" />
                                    <asp:FileUpload ID="fileUploadEditCategory" runat="server" CssClass="form-control" />
                                    <small class="text-muted">Only select a file if you want to replace the current image.</small>
                                </div>
                                </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnUpdateCategory" runat="server" Text="Update Category" CssClass="btn btn-success" OnClick="btnUpdateCategory_Click" />
                        </div>
                    </div>
                </ContentTemplate>
                
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnUpdateCategory" />
                </Triggers>
                </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>