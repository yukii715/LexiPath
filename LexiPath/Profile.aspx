<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="LexiPath.Profile" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <div class="container mt-4">
        
        <div class="p-5 mb-4 rounded-3 text-white shadow-lg" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);">
            <div class="container-fluid py-2 px-0">
                <h1 class="display-5 fw-bold"><i class="bi bi-person-badge-fill me-3"></i>My Profile</h1>
                <p class="fs-5 mt-3 text-white-75">Manage your account details, track your progress, and update your password.</p>
            </div>
        </div>

        <h3 class="fw-bold mb-3"><i class="bi bi-graph-up me-2 text-primary"></i>Your Statistics</h3>
        <div class="row g-4 mb-5">
            <div class="col-md-4">
                <div class="card text-center shadow-sm h-100 border-0">
                    <div class="card-body p-4">
                        <i class="bi bi-check-circle-fill text-success display-4 mb-2"></i>
                        <h2 class="display-4 fw-bold text-success"><asp:Literal ID="litCoursesCompleted" runat="server" Text="0"></asp:Literal></h2>
                        <p class="card-text fs-5 mb-0">Courses Completed</p>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card text-center shadow-sm h-100 border-0">
                    <div class="card-body p-4">
                        <i class="bi bi-clock-history text-warning display-4 mb-2"></i>
                        <h2 class="display-4 fw-bold text-warning"><asp:Literal ID="litLearningHours" runat="server" Text="0.00"></asp:Literal></h2>
                        <p class="card-text fs-5 mb-0">Learning Hours</p>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card text-center shadow-sm h-100 border-0">
                    <div class="card-body p-4">
                        <i class="bi bi-journal-check text-info display-4 mb-2"></i>
                        <h2 class="display-4 fw-bold text-info"><asp:Literal ID="litQuizzesTaken" runat="server" Text="0"></asp:Literal></h2>
                        <p class="card-text fs-5 mb-0">Quizzes Taken</p>
                    </div>
                </div>
            </div>
        </div>
        
        <hr class="my-5" />

        <div class="row g-5">
            
            <div class="col-md-4">
                <h3 class="fw-bold mb-3"><i class="bi bi-image me-2 text-secondary"></i>Profile Picture</h3>
                <div class="card shadow-sm rounded-3 p-3">
                    <asp:Image ID="imgProfilePic" runat="server" CssClass="img-fluid rounded-3 shadow-sm mb-3" ImageUrl="/Image/System/placeholder_profile.png" style="max-width: 200px; max-height: 200px; object-fit: cover;" />
                    
                    <asp:FileUpload ID="fileUploadPic" runat="server" CssClass="form-control" onchange="previewImage(this)" />
                    
                    <asp:Button ID="btnUploadPic" runat="server" Text="Upload New Picture" 
                        CssClass="btn btn-secondary mt-3 w-100 rounded-pill" OnClick="btnUploadPic_Click" />
                </div>
            </div>

            <div class="col-md-8">
                
                <h3 class="fw-bold mb-3"><i class="bi bi-pencil-square me-2 text-primary"></i>Account Details</h3>
                <div class="card shadow-sm rounded-3 mb-5">
                    <div class="card-body p-4">
                        
                        <div class="mb-3">
                            <label class="form-label fw-bold text-secondary">Username</label>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control rounded-pill px-4" ValidationGroup="ProfileDetails" autocomplete="off"></asp:TextBox>
                            
                            <asp:RequiredFieldValidator ID="reqUser" runat="server" ControlToValidate="txtUsername" 
                                ErrorMessage="Username is required" Display="Dynamic" CssClass="text-danger small ms-2" ValidationGroup="ProfileDetails" />
                            <asp:RegularExpressionValidator ID="regexUser" runat="server" ControlToValidate="txtUsername"
                                ValidationExpression="^[a-zA-Z0-9]{3,}$"
                                ErrorMessage="Min 3 alphanumeric characters." Display="Dynamic" CssClass="text-danger small ms-2" ValidationGroup="ProfileDetails" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label fw-bold text-secondary">Email</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control rounded-pill px-4" TextMode="Email" ValidationGroup="ProfileDetails" autocomplete="off"></asp:TextBox>
                            
                            <asp:RequiredFieldValidator ID="reqEmail" runat="server" ControlToValidate="txtEmail" 
                                ErrorMessage="Email is required" Display="Dynamic" CssClass="text-danger small ms-2" ValidationGroup="ProfileDetails" />
                            <asp:RegularExpressionValidator ID="regexEmail" runat="server" ControlToValidate="txtEmail"
                                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                ErrorMessage="Invalid email format." Display="Dynamic" CssClass="text-danger small ms-2" ValidationGroup="ProfileDetails" />
                        </div>

                        <asp:Button ID="btnUpdateProfile" runat="server" Text="Save Details" 
                            CssClass="btn btn-primary rounded-pill px-5 mt-3" OnClick="btnUpdateProfile_Click" ValidationGroup="ProfileDetails" />
                    </div>
                </div>

                <h3 class="fw-bold mb-3"><i class="bi bi-shield-lock-fill me-2 text-danger"></i>Change Password</h3>
                <div class="card shadow-sm rounded-3">
                    <div class="card-body p-4">
                        <asp:Label ID="lblPasswordMessage" runat="server" EnableViewState="false" CssClass="d-block mb-3 fw-bold"></asp:Label>
        
                        <div class="mb-3">
                            <label class="form-label fw-bold text-secondary">Current Password</label>
                            <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control rounded-pill px-4" TextMode="Password"></asp:TextBox>
                        </div>

                        <div class="mb-3">
                            <label class="form-label fw-bold text-secondary">New Password</label>
                            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control rounded-pill px-4" TextMode="Password"></asp:TextBox>
                        </div>

                        <div class="mb-3">
                            <label class="form-label fw-bold text-secondary">Confirm New Password</label>
                            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control rounded-pill px-4" TextMode="Password"></asp:TextBox>
                        </div>

                        <asp:Button ID="btnUpdatePassword" runat="server" Text="Change Password" 
                            CssClass="btn btn-danger rounded-pill px-5 mt-3" OnClick="btnUpdatePassword_Click" />
                    </div>
                </div>
            </div>
        </div>
        
        <hr class="my-5" />

        <a id="savedCoursesAnchor"></a>
        <h3 class="fw-bold mb-4"><i class="bi bi-star-fill me-2 text-warning"></i>Saved Courses</h3>

        <div class="card shadow-sm rounded-3 mb-5">
            <div class="card-body p-4">
                <h4 class="mb-3">Filter Saved Courses</h4>
                <asp:Panel ID="pnlFilterControls" runat="server" DefaultButton="btnSearchCourses" CssClass="row g-3 align-items-end">
                    <div class="col-md-5">
                        <label class="form-label">Search by Course Name</label>
                        <asp:TextBox ID="txtCourseSearch" runat="server" CssClass="form-control rounded-pill px-3" placeholder="Search saved courses..."></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">Order By</label>
                        <asp:DropDownList ID="ddlCourseSort" runat="server" CssClass="form-select rounded-pill px-3" OnSelectedIndexChanged="btnSearchCourses_Click" AutoPostBack="True">
                            <asp:ListItem Value="Alphabetical" Text="Name (A-Z)" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="DateAdded" Text="Date Added (Oldest First)"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <asp:Button ID="btnSearchCourses" runat="server" Text="Apply Filter" CssClass="btn btn-primary w-100 rounded-pill" OnClick="btnSearchCourses_Click" />
                    </div>
                </asp:Panel>
            </div>
        </div>
        
        <div class="row g-5">
            <div class="col-md-6">
                <h4 class="fw-bold mb-3"><i class="bi bi-heart-fill text-danger me-2"></i> Liked Courses</h4>
                <asp:Repeater ID="rptLikedCourses" runat="server">
                    <HeaderTemplate><div class="list-group shadow-sm rounded-3"></HeaderTemplate>
                    <ItemTemplate>
                        <a href='CourseDetail.aspx?CourseID=<%# Eval("CourseID") %>' class="list-group-item list-group-item-action border-0 py-3">
                            <%# Eval("CourseName") %> <span class="badge bg-secondary rounded-pill ms-2"><%# Eval("CourseType") %></span>
                        </a>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>
                <asp:Label ID="lblNoLiked" runat="server" CssClass="alert alert-secondary mt-3 d-block rounded-3" Visible="false" Text="You haven't liked any courses yet!"></asp:Label>
            </div>

            <div class="col-md-6">
                <h4 class="fw-bold mb-3"><i class="bi bi-bookmark-fill text-info me-2"></i> Collected Courses</h4>
                <asp:Repeater ID="rptCollectedCourses" runat="server">
                    <HeaderTemplate><div class="list-group shadow-sm rounded-3"></HeaderTemplate>
                    <ItemTemplate>
                        <a href='CourseDetail.aspx?CourseID=<%# Eval("CourseID") %>' class="list-group-item list-group-item-action border-0 py-3">
                            <%# Eval("CourseName") %> <span class="badge bg-secondary rounded-pill ms-2"><%# Eval("CourseType") %></span>
                        </a>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>
                <asp:Label ID="lblNoCollected" runat="server" CssClass="alert alert-secondary mt-3 d-block rounded-3" Visible="false" Text="You haven't collected any courses yet!"></asp:Label>
            </div>
        </div>
    </div>
    
    <script type="text/javascript">
        function previewImage(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var img = document.getElementById('<%= imgProfilePic.ClientID %>');
                    if (img) { img.src = e.target.result; }
                };
                reader.readAsDataURL(input.files[0]);
            }
        }

        function showNotification(type, message) {
            if (typeof Swal === 'undefined') {
                console.error("SweetAlert2 is not loaded!");
                return;
            }
            Swal.fire({
                toast: true,
                position: 'top',
                icon: type,
                title: message,
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.addEventListener('mouseenter', Swal.stopTimer)
                    toast.addEventListener('mouseleave', Swal.resumeTimer)
                }
            });
        }
    </script>

    <style>
        .shadow-xl { box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.175) !important; }
        .text-primary { color: #667eea !important; }
        .btn-primary { background-color: #667eea; border-color: #667eea; }
        .btn-primary:hover { background-color: #764ba2; border-color: #764ba2; }
        .text-success { color: #20c997 !important; }
        .text-info { color: #17a2b8 !important; }
        .text-warning { color: #ffc107 !important; }
        .rounded-pill { border-radius: 50rem !important; }
        .form-control.rounded-pill, .form-select.rounded-pill { padding-right: 1.5rem; padding-left: 1.5rem; }
    </style>
</asp:Content>