<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Learn.aspx.cs" Inherits="LexiPath.Learn" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Learn - LexiPath</title>
    
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />

    <script type="text/javascript">
        // This flag stops the "are you sure" prompt when we click a button
        var isLessonComplete = false;

        // This variable is set by C# (see Page_Load)
        var isUser = <%= IsUserRegistered.ToString().ToLower() %>;

        function confirmEndLesson() {
            if (isUser && !isLessonComplete) {
                // Ask registered users for confirmation
                return confirm('Are you sure you want to end the lesson? Your progress will not be marked as complete.');
            }
            return true; // Guests can leave
        }

        // This event fires when the user tries to CLOSE THE TAB
        window.onbeforeunload = function () {
            if (isUser && !isLessonComplete) {
                // This will show a generic "Are you sure?" prompt
                return 'Your progress will not be marked as complete.';
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
            ToolTip="Exit Lesson"
            OnClientClick="return confirmEndLesson();"
            OnClick="lnkBack_Click" /> <div class="container mt-4 d-flex justify-content-center">
            <div class="col-lg-8" style="margin-top: 50px;"> 
                
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <h3 class="mb-0"><asp:Literal ID="litCourseName" runat="server" Text="Course Name..."></asp:Literal></h3>
                    <strong><asp:Literal ID="litCurrentPage" runat="server" Text="0 / 0"></asp:Literal></strong>
                </div>
                
                <div class="card shadow-lg" style="min-height: 450px;">
                    <div class="card-body p-4 p-md-5">

                        <asp:Panel ID="pnlVocabView" runat="server" Visible="false" CssClass="d-flex flex-column h-100">
                            <div class="row align-items-center flex-grow-1">
                                <div class="col-md-5 text-center">
                                    <asp:Image ID="imgVocab" runat="server" CssClass="img-fluid rounded mb-3" style="max-height: 250px;" />
                                </div>
                                <div class="col-md-7">
                                    <h2 class="display-6"><asp:Literal ID="litVocabText" runat="server"></asp:Literal></h2>
                                    <p class="fs-4"><strong>Meaning:</strong> <asp:Literal ID="litVocabMeaning" runat="server"></asp:Literal></p>
                                </div>
                            </div>
                        </asp:Panel>

                        <asp:Panel ID="pnlPhraseView" runat="server" Visible="false" CssClass="d-flex flex-column h-100">
                            <div class="flex-grow-1">
                                <h2 class="display-6"><asp:Literal ID="litPhraseText" runat="server"></asp:Literal></h2>
                                <p class="fs-4"><strong>Meaning:</strong> <asp:Literal ID="litPhraseMeaning" runat="server"></asp:Literal></p>
                                <hr />
                                <h5>Explanations:</h5>
                                <asp:Repeater ID="rptPhraseDetails" runat="server">
                                    <HeaderTemplate><ul class="list-group list-group-flush"></HeaderTemplate>
                                    <ItemTemplate>
                                        <li class="list-group-item">
                                            <strong><%# Eval("DetailType") %>:</strong> <%# Eval("Content") %>
                                        </li>
                                    </ItemTemplate>
                                    <FooterTemplate></ul></FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </asp:Panel>

                    </div>
                    
                    <div class="card-footer d-flex p-3">
                        <asp:Button ID="btnPrev" runat="server" Text="&larr; Previous" CssClass="btn btn-secondary btn-lg" 
                            OnClick="btnPrev_Click" 
                            OnClientClick="isLessonComplete = true;" />
                        <asp:Button ID="btnNext" runat="server" Text="Next &rarr;" CssClass="btn btn-primary btn-lg ms-auto" 
                            OnClick="btnNext_Click" 
                            OnClientClick="isLessonComplete = true;" />
                        
                        <asp:Button ID="btnEndLesson" runat="server" Text="End Lesson" 
                            CssClass="btn btn-success btn-lg ms-auto" Visible="false" 
                            OnClick="btnEndLesson_Click" 
                            OnClientClick="isLessonComplete = true;" /> </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>