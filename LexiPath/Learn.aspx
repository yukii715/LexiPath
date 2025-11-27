<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Learn.aspx.cs" Inherits="LexiPath.Learn" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Learn - LexiPath</title>
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <script type="text/javascript">
        var isUser = <%= IsUserRegistered.ToString().ToLower() %>;
        var isAdmin = <%= IsAdmin.ToString().ToLower() %>;
        var targetLang = "<%= CourseLanguageCode %>";
        var validNavigation = false;

        function allowNavigation() { validNavigation = true; }

        window.onbeforeunload = function () {
            if (isUser && !isAdmin && !validNavigation) {
                return 'Your progress will not be marked as complete.';
            }
        };

        function confirmEndLesson(event) {
            if (isUser && !isAdmin) {
                event.preventDefault();
                Swal.fire({
                    title: 'Exit Lesson?', text: "Your progress will not be saved.", icon: 'warning', position: 'top',
                    showCancelButton: true, confirmButtonColor: '#dc3545', cancelButtonColor: '#6c757d', confirmButtonText: 'Yes, Exit'
                }).then((result) => {
                    if (result.isConfirmed) {
                        allowNavigation();
                        __doPostBack('<%= lnkBack.UniqueID %>', '');
                    }
                });
                return false;
            }
            allowNavigation();
            return true;
        }

        function showNotification(type, message) {
            Swal.fire({ toast: true, position: 'top', icon: type, title: message, showConfirmButton: false, timer: 3000, timerProgressBar: true });
        }

        // --- AUDIO API FUNCTION ---
        function playAudio(elementId) {
            if (!('speechSynthesis' in window)) {
                showNotification('error', 'Browser does not support audio.');
                return;
            }

            var text = document.getElementById(elementId).innerText;
            window.speechSynthesis.cancel();
            var msg = new SpeechSynthesisUtterance(text);
            msg.lang = targetLang;
            msg.rate = 0.9;
            msg.onerror = function (event) { console.error('Audio error:', event); };

            window.speechSynthesis.speak(msg);
        }
    </script>
    <style>
        /* --- 1. PAGE & LAYOUT --- */
        body {
            background-color: #f0f2f5; /* Softer gray background */
            font-family: 'Poppins', sans-serif; /* Ensure you have this font or system default */
        }

        /* --- 2. MAIN CARD DESIGN --- */
        .content-card {
            border: none;
            transition: all 0.4s cubic-bezier(0.165, 0.84, 0.44, 1); /* Smooth physics-based ease */
            background: #ffffff;
        }

        /* Lift effect on hover to keep user engaged */
        .content-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 20px 40px rgba(102, 126, 234, 0.15) !important;
        }

        /* Deep shadow for depth */
        .shadow-xl {
            box-shadow: 0 1rem 3rem rgba(0, 0, 0, 0.1) !important;
        }

        /* --- 3. IMAGE CONTAINER (The Fix) --- */
        .learning-image {
            height: 320px; /* Fixed height for consistency */
            width: 100%; 
            object-fit: contain; /* Ensures the whole image is visible */
            background-color: #f8f9fa; /* Light gray background for transparent images */
            border: 1px solid #e9ecef;
            padding: 10px;
            transition: transform 0.3s ease;
        }
    
        /* Subtle zoom on image hover */
        .learning-image:hover {
            transform: scale(1.02);
        }

        /* --- 4. TYPOGRAPHY & COLORS --- */
        .text-primary {
            color: #667eea !important; /* Brand Purple */
        }
    
        .fw-bolder {
            font-weight: 700 !important;
            letter-spacing: -0.5px;
        }

        /* --- 5. AUDIO BUTTON --- */
        .btn-audio {
            background-color: #eff2f7;
            color: #667eea;
            width: 45px;
            height: 45px;
            border-radius: 50%;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            border: 2px solid transparent;
            transition: all 0.3s cubic-bezier(0.68, -0.55, 0.265, 1.55); /* Bouncy effect */
            cursor: pointer;
        }

        .btn-audio:hover {
            background-color: #667eea;
            color: #ffffff;
            transform: scale(1.15) rotate(15deg); /* Playful interaction */
            box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
        }

        .btn-audio:active {
            transform: scale(0.9);
        }

        /* --- 6. NAVIGATION BUTTONS --- */
        .btn-primary {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border: none;
            font-weight: 600;
            box-shadow: 0 4px 6px rgba(102, 126, 234, 0.3);
            transition: all 0.3s ease;
        }

        .btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 15px rgba(102, 126, 234, 0.4);
            background: linear-gradient(135deg, #764ba2 0%, #667eea 100%);
        }

        .btn-outline-secondary {
            border: 2px solid #e9ecef;
            color: #6c757d;
            font-weight: 600;
            background: transparent;
        }

        .btn-outline-secondary:hover {
            background-color: #e9ecef;
            color: #495057;
            border-color: #dee2e6;
        }

        /* --- 7. PROGRESS TEXT --- */
        .text-secondary {
            font-family: monospace; /* Makes the "1 / 5" look like code/data */
            letter-spacing: 1px;
        }
    </style>
</head>
<body class="bg-light">
    <form id="form1" runat="server">
        
        <asp:LinkButton ID="lnkBack" runat="server" 
            CssClass="btn btn-outline-secondary rounded-pill px-3 py-2 shadow-sm" 
            style="position: absolute; top: 20px; right: 20px; z-index: 10;"
            ToolTip="Exit Lesson" OnClientClick="return confirmEndLesson(event);" OnClick="lnkBack_Click">
            <i class="bi bi-x-lg me-1"></i> Exit Lesson
        </asp:LinkButton>

        <div class="container d-flex justify-content-center" style="min-height: 100vh;">
            <div class="col-lg-8" style="margin-top: 20px; margin-bottom: 20px;"> 
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <h3 class="mb-0 fw-bold text-primary"><asp:Literal ID="litCourseName" runat="server" Text="Course Name..."></asp:Literal></h3>
                    <strong class="text-secondary fs-5"><asp:Literal ID="litCurrentPage" runat="server" Text="0 / 0"></asp:Literal></strong>
                </div>
                
                <div class="card shadow-xl rounded-3 d-flex flex-column content-card" style="min-height: 400px;">
                     <div class="card-body p-4 p-md-4 flex-grow-1">
                        
                        <%-- VOCAB VIEW --%>
                        <asp:Panel ID="pnlVocabView" runat="server" Visible="false" CssClass="d-flex flex-column h-100">
                            <div class="row align-items-center">
                                 <div class="col-md-5 text-center">
                                    <asp:Image ID="imgVocab" runat="server" CssClass="img-fluid rounded-3 mb-3 shadow-sm learning-image" style="max-height: 250px;  width: 100%;" />
                                </div>
                                <div class="col-md-7">
                                     <h2 class="h3 fw-bolder text-primary mb-2 d-flex align-items-center gap-2">
                                         <%-- Wrapped text in span for JS to find --%>
                                         <span id="txtVocab"><asp:Literal ID="litVocabText" runat="server"></asp:Literal></span>
                                         
                                         <%-- AUDIO BUTTON --%>
                                         <button type="button" runat="server" id="btnVocabAudio" class="btn-audio text-secondary fs-4" 
                                             onclick="playAudio('txtVocab');" title="Listen">
                                             <i class="bi bi-volume-up-fill"></i>
                                         </button>
                                     </h2>
                                    <p class="fs-5 text-muted mb-0"><strong>Meaning:</strong> <asp:Literal ID="litVocabMeaning" runat="server"></asp:Literal></p>
                                </div>
                             </div>
                        </asp:Panel>

                        <%-- PHRASE VIEW --%>
                        <asp:Panel ID="pnlPhraseView" runat="server" Visible="false" CssClass="d-flex flex-column h-100">
                            <div class="flex-grow-1">
                                 <h2 class="h3 fw-bolder text-primary mb-2 d-flex align-items-center gap-2">
                                     <%-- Wrapped text in span for JS to find --%>
                                     <span id="txtPhrase"><asp:Literal ID="litPhraseText" runat="server"></asp:Literal></span>

                                     <%-- AUDIO BUTTON --%>
                                     <button type="button" runat="server" id="btnPhraseAudio" class="btn-audio text-secondary fs-4" 
                                         onclick="playAudio('txtPhrase');" title="Listen">
                                         <i class="bi bi-volume-up-fill"></i>
                                     </button>
                                 </h2>
                                <p class="fs-5 text-muted mb-3"><strong>Meaning:</strong> <asp:Literal ID="litPhraseMeaning" runat="server"></asp:Literal></p>
                                <hr class="my-3" />
                                <h5><i class="bi bi-lightbulb me-2 text-info"></i> Explanations & Context:</h5>
                                <asp:Repeater ID="rptPhraseDetails" runat="server">
                                    <HeaderTemplate><ul class="list-group list-group-flush mt-3"></HeaderTemplate>
                                    <ItemTemplate>
                                     <li class="list-group-item px-0 border-top-0 border-bottom-1 py-1">
                                             <strong class="text-secondary"><%# Eval("DetailType") %>:</strong> <%# Eval("Content") %>
                                        </li>
                                     </ItemTemplate>
                                    <FooterTemplate></ul></FooterTemplate>
                                </asp:Repeater>
                           </div>
                        </asp:Panel>
                    </div>
                    
                   <div class="card-footer d-flex p-3 bg-light border-top rounded-bottom-3">
                        <asp:Button ID="btnPrev" runat="server" Text="&larr; Previous" CssClass="btn btn-outline-secondary rounded-pill px-4" OnClick="btnPrev_Click" OnClientClick="allowNavigation()" />
                        <asp:Button ID="btnNext" runat="server" Text="Next &rarr;" CssClass="btn btn-primary rounded-pill px-4 ms-auto" OnClick="btnNext_Click" OnClientClick="allowNavigation()" />
                        <asp:Button ID="btnEndLesson" runat="server" Text="End Lesson" CssClass="btn btn-success rounded-pill px-4 ms-auto" Visible="false" OnClick="btnEndLesson_Click" OnClientClick="allowNavigation()" />
                    </div>
                </div>
            </div>
       </div>
    </form>
</body>
</html>