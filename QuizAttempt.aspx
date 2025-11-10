<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuizAttempt.aspx.cs" Inherits="LexiPath.QuizAttempt" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Quiz - LexiPath</title>

    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />

    <script type="text/javascript">
        // This flag stops the "are you sure" prompt
        var isQuizComplete = false;

        // This variable is set by C#
        var isUser = <%= IsUserRegistered.ToString().ToLower() %>;

        function confirmEndQuiz() {
            if (isUser && !isQuizComplete) {
                return confirm('Are you sure you want to quit the quiz? Your progress will not be saved.');
            }
            return true;
        }

        // This event fires when the user tries to CLOSE THE TAB
        window.onbeforeunload = function() {
            if (isUser && !isQuizComplete) {
                return 'Your progress will not be saved.';
            }
        };
    </script>
</head>
<body style="background-color: #f8f9fa;">
    <form id="form1" runat="server">        
        <asp:LinkButton ID="lnkBack" runat="server" 
            CssClass="btn-close" 
            style="position: absolute; top: 20px; right: 20px; font-size: 1.5rem;" 
            aria-label="Close"
            ToolTip="Exit Quiz"
            OnClientClick="return confirmEndQuiz();"
            OnClick="lnkBack_Click" />
        <asp:Panel ID="pnlQuiz" runat="server">
            <div class="container mt-4 d-flex justify-content-center">
                <div class="col-lg-8" style="margin-top: 50px;"> 

                    <div class="d-flex justify-content-between align-items-center mb-2">
                        <h3 class="mb-0"><asp:Literal ID="litQuizTitle" runat="server"></asp:Literal></h3>
                        <strong><asp:Literal ID="litCurrentPage" runat="server" Text="0 / 0"></asp:Literal></strong>
                    </div>
                    
                    <div class="card shadow-lg" style="min-height: 450px;">
                        <div class="card-body p-4 p-md-5">
                            <h2 class="display-6 mb-3"><asp:Literal ID="litQuestionText" runat="server"></asp:Literal></h2>
                            <asp:Image ID="imgQuestion" runat="server" CssClass="img-fluid rounded mb-3" Visible="false" style="max-height: 250px;" />

                            <asp:Panel ID="pnlMCQ" runat="server" Visible="false">
                                <asp:RadioButtonList ID="rblOptions" runat="server" CssClass="list-group" RepeatLayout="Flow" />
                            </asp:Panel>

                            <asp:Panel ID="pnlTypeInAnswer" runat="server" Visible="false">
                                <asp:TextBox ID="txtAnswer" runat="server" CssClass="form-control form-control-lg" placeholder="Type your answer here..."></asp:TextBox>
                            </asp:Panel>

                            <asp:Panel ID="pnlFeedback" runat="server" Visible="false" CssClass="alert mt-4">
                                <asp:Literal ID="litFeedback" runat="server"></asp:Literal>
                            </asp:Panel>

                        </div>
                        
                        <div class="card-footer d-flex p-3">
                            <asp:Button ID="btnSubmitAnswer" runat="server" Text="Submit Answer" CssClass="btn btn-primary btn-lg ms-auto" 
                                OnClick="btnSubmitAnswer_Click" 
                                OnClientClick="isQuizComplete = true;" />
                            
                            <asp:Button ID="btnNextQuestion" runat="server" Text="Next Question" CssClass="btn btn-secondary btn-lg ms-auto" Visible="false" 
                                OnClick="btnNextQuestion_Click" 
                                OnClientClick="isQuizComplete = true;" />
                        </div>
                    </div>

                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlReview" runat="server" Visible="false">
            <div class="container mt-4">
                <div class="col-lg-10 mx-auto">
                    
                    <div class="text-center p-5 mb-4 bg-light rounded-3">
                        <h2>Quiz Complete!</h2>
                        <p class="fs-4">Your final score is: 
                            <strong><asp:Literal ID="litFinalScore" runat="server"></asp:Literal></strong>
                        </p>
                        <asp:Button ID="btnBackToQuiz" runat="server" Text="&larr; Back to Quiz Page" 
                            CssClass="btn btn-primary btn-lg" 
                            OnClick="btnBackToQuiz_Click" 
                            OnClientClick="isQuizComplete = true;" />
                    </div>

                    <h3>Review Your Answers</h3>
                    <asp:Repeater ID="rptReview" runat="server">
                        <ItemTemplate>
                            <div class="card mb-3 <%# (bool)Eval("IsCorrect") ? "border-success" : "border-danger" %>">
                                <div class="card-header">
                                    <strong>Question:</strong> <%# Eval("QuestionText") %>
                                </div>
                                <div class="card-body">
                                    <p class="mb-1"><strong>Your Answer:</strong> <span class="<%# (bool)Eval("IsCorrect") ? "text-success" : "text-danger" %>"><%# Eval("UserSelection") %></span></p>
                                    
                                    <asp:Panel ID="pnlCorrectAnswer" runat="server" Visible='<%# !(bool)Eval("IsCorrect") %>'>
                                        <p class="mb-0"><strong>Correct Answer:</strong> <span class="text-success"><%# Eval("CorrectAnswer") %></span></p>
                                    </asp:Panel>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>