<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuizAttempt.aspx.cs" Inherits="LexiPath.QuizAttempt" EnableEventValidation="false" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Quiz - LexiPath</title>
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />
    
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script type="text/javascript">
        var isUser = <%= IsUserRegistered.ToString().ToLower() %>;
        var isAdmin = <%= IsAdmin.ToString().ToLower() %>;

        var validNavigation = false;
        function allowNavigation() { validNavigation = true; }

        window.onbeforeunload = function () {
            if (isUser && !isAdmin && !validNavigation) {
                return 'Your progress will not be saved.';
            }
        };

        // --- UPDATED: Close Tab Logic ---
        function confirmEndQuiz(event) {
            event.preventDefault();

            if (isUser && !isAdmin) {
                Swal.fire({
                    title: 'Quit Quiz?',
                    text: "Your progress will not be saved.",
                    icon: 'warning',
                    position: 'top',
                    showCancelButton: true,
                    confirmButtonColor: '#dc3545',
                    cancelButtonColor: '#6c757d',
                    confirmButtonText: 'Yes, Quit'
                }).then((result) => {
                    if (result.isConfirmed) {
                        allowNavigation();
                        closeOrRedirect(); // Call new helper
                    }
                });
            } else {
                allowNavigation();
                closeOrRedirect(); // Call new helper
            }
            return false;
        }

        // NEW HELPER FUNCTION
        function closeOrRedirect() {
            // Try to close the window
            window.close();

            // If the window is still open after 100ms (meaning script blocked close),
            // redirect back to the course list as a fallback.
            setTimeout(function () {
                window.location.href = 'Courses.aspx';
            }, 100);
        }

        function showNotification(type, message) {
            Swal.fire({ toast: true, position: 'top', icon: type, title: message, showConfirmButton: false, timer: 3000, timerProgressBar: true });
        }
    </script>
    <style>
        body { background-color: #f8f9fa; }
        .shadow-xl { box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.175) !important; }
        .text-primary { color: #667eea !important; }
        
        /* Custom Radio List Styling */
        .custom-radio-list { width: 100%; border-collapse: separate; border-spacing: 0 12px; }
        .custom-radio-list td {
            background-color: #ffffff; border: 2px solid #e9ecef; border-radius: 12px;
            padding: 15px 20px; transition: all 0.2s ease-in-out; cursor: pointer;
            display: flex; align-items: center; width: 100%; box-sizing: border-box;
        }
        .custom-radio-list td:hover {
            background-color: #f0f4ff; border-color: #667eea;
            transform: translateY(-2px); box-shadow: 0 4px 10px rgba(102, 126, 234, 0.15);
        }
        .custom-radio-list input[type="radio"] {
            appearance: none; -webkit-appearance: none; width: 20px; height: 20px;
            border: 2px solid #adb5bd; border-radius: 50%; margin-right: 15px;
            position: relative; flex-shrink: 0; cursor: pointer; transition: all 0.2s;
        }
        .custom-radio-list input[type="radio"]:checked {
            border-color: #667eea; background-color: #667eea;
            box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.2);
        }
        .custom-radio-list input[type="radio"]:checked::after {
            content: ''; position: absolute; top: 50%; left: 50%;
            transform: translate(-50%, -50%); width: 8px; height: 8px;
            background-color: white; border-radius: 50%;
        }
        .custom-radio-list label {
            margin-bottom: 0; font-size: 1.1rem; color: #495057;
            font-weight: 500; cursor: pointer; width: 100%;
        }
        .custom-radio-list input[type="radio"]:checked + label { color: #667eea; font-weight: bold; }
        .custom-radio-list td:has(input:checked) { border-color: #667eea; background-color: #f4f6ff; }
        .question-card { transition: transform 0.3s ease; }
        
        /* Fixed Image Size */
        .quiz-image {
            height: 300px;
            width: 100%;
            object-fit: contain;
            background-color: #ffffff;
            border: 1px solid #e9ecef;
        }
    </style>
</head>
<body class="bg-light">
    <form id="form1" runat="server">
        
        <%-- Quit Quiz Button: Calls confirmEndQuiz which closes tab --%>
        <asp:LinkButton ID="lnkBack" runat="server" 
            CssClass="btn btn-outline-secondary rounded-pill px-3 py-2 shadow-sm" 
            style="position: absolute; top: 20px; right: 20px; z-index: 10;"
            ToolTip="Exit Quiz"
            OnClientClick="return confirmEndQuiz(event);">
            <i class="bi bi-x-lg me-1"></i> Quit Quiz
        </asp:LinkButton>

        <asp:Panel ID="pnlQuiz" runat="server">
            <div class="container d-flex justify-content-center">
                <div class="col-lg-8" style="margin-top: 40px; margin-bottom: 40px;"> 
                    
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <h3 class="mb-0 fw-bold text-primary">
                            <i class="bi bi-patch-question-fill me-2"></i> <asp:Literal ID="litQuizTitle" runat="server"></asp:Literal>
                        </h3>
                        <span class="badge bg-white text-secondary border shadow-sm fs-6 px-3 py-2">
                            Question <asp:Literal ID="litCurrentPage" runat="server" Text="0 / 0"></asp:Literal>
                        </span>
                    </div>
                    
                    <div class="card shadow-xl rounded-4 border-0 d-flex flex-column question-card" style="min-height: 450px;">
                        <div class="card-body p-5 flex-grow-1">
                            
                            <h2 class="h4 mb-4 fw-bold text-dark lh-base">
                                <asp:Literal ID="litQuestionText" runat="server"></asp:Literal>
                            </h2>
                            
                            <asp:Image ID="imgQuestion" runat="server" CssClass="img-fluid rounded-3 mb-4 shadow-sm border quiz-image" Visible="false" />

                            <asp:Panel ID="pnlMCQ" runat="server" Visible="false">
                                <asp:RadioButtonList ID="rblOptions" runat="server" CssClass="custom-radio-list" RepeatLayout="Table" />
                            </asp:Panel>

                            <asp:Panel ID="pnlTypeInAnswer" runat="server" Visible="false">
                                <div class="form-group">
                                    <label class="text-muted mb-2 small fw-bold text-uppercase">Your Answer</label>
                                    <asp:TextBox ID="txtAnswer" runat="server" 
                                        CssClass="form-control form-control-lg rounded-pill px-4 py-3 bg-light border-0 shadow-inner" 
                                        placeholder="Type your answer here..." 
                                        autocomplete="off"></asp:TextBox>
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="pnlFeedback" runat="server" Visible="false" CssClass="alert mt-4 rounded-3 p-3 shadow-sm border-0">
                                <asp:Literal ID="litFeedback" runat="server"></asp:Literal>
                            </asp:Panel>
                        </div>
                        
                        <div class="card-footer d-flex p-4 bg-white border-top-0 rounded-bottom-4">
                            <asp:Button ID="btnSubmitAnswer" runat="server" Text="Submit Answer" 
                                CssClass="btn btn-primary btn-lg rounded-pill px-5 ms-auto shadow-sm" 
                                OnClick="btnSubmitAnswer_Click" OnClientClick="allowNavigation()" />
                            
                            <asp:Button ID="btnNextQuestion" runat="server" Text="Next Question &rarr;" 
                                CssClass="btn btn-secondary btn-lg rounded-pill px-5 ms-auto shadow-sm" Visible="false" 
                                OnClick="btnNextQuestion_Click" OnClientClick="allowNavigation()" />
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlReview" runat="server" Visible="false">
            <div class="container mt-5">
                <div class="col-lg-8 mx-auto">
                    <div class="text-center p-5 mb-5 rounded-4 text-white shadow-xl" 
                         style="background: linear-gradient(135deg, #28a745 0%, #007bff 100%);">
                        <h1 class="display-5 fw-bold mb-3">Quiz Complete! <i class="bi bi-trophy-fill ms-2"></i></h1>
                        <p class="fs-4 mb-0">You scored</p>
                        <strong class="display-1 d-block my-2"><asp:Literal ID="litFinalScore" runat="server"></asp:Literal></strong>
                        
                        <%-- UPDATED: Back Button closes tab --%>
                        <asp:Button ID="btnBackToQuiz" runat="server" Text="Close Quiz" 
                            CssClass="btn btn-light btn-lg rounded-pill px-5 mt-4 fw-bold shadow-sm" 
                            OnClientClick="allowNavigation(); window.close(); return false;" />
                    </div>

                    <h3 class="mb-4 fw-bold text-secondary"><i class="bi bi-clipboard-data me-2"></i> Results Breakdown</h3>
                    
                    <div class="vstack gap-4">
                        <asp:Repeater ID="rptReview" runat="server">
                             <ItemTemplate>
                                <div class="card shadow-sm border-0 rounded-3 overflow-hidden">
                                    <div class="card-body p-0">
                                        <div class="d-flex">
                                            <div class="p-3 d-flex align-items-center justify-content-center text-white" 
                                                 style="width: 60px; background-color: <%# (bool)Eval("IsCorrect") ? "#198754" : "#dc3545" %>;">
                                                 <i class="bi <%# (bool)Eval("IsCorrect") ? "bi-check-lg" : "bi-x-lg" %> fs-3"></i>
                                            </div>
                                            <div class="p-3 w-100">
                                                <h6 class="fw-bold text-dark mb-2"><%# Eval("QuestionText") %></h6>
                                                <div class="d-flex flex-column gap-1">
                                                    <small class="text-muted">Your Answer:</small>
                                                    <span class="fw-bold <%# (bool)Eval("IsCorrect") ? "text-success" : "text-danger" %>">
                                                        <%# Eval("UserSelection") %>
                                                    </span>
                                                    <asp:Panel ID="pnlCorrectAnswer" runat="server" Visible='<%# !(bool)Eval("IsCorrect") %>'>
                                                        <small class="text-muted mt-2 d-block">Correct Answer:</small>
                                                        <span class="fw-bold text-success"><%# Eval("CorrectAnswer") %></span>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                 </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="text-center mt-5 mb-5">
                        <small class="text-muted">LexiPath Learning System</small>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>