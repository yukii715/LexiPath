<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="LexiPath._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="hero-section text-white text-center position-relative overflow-hidden mb-5" 
         style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 0 0 50% 50% / 20px;">
        <div class="container py-5">
            <div class="row justify-content-center py-5">
                <div class="col-lg-8">
                    <h1 class="display-3 fw-bold mb-3 animate-up">Master Languages in Context <i class="bi bi-globe-americas"></i></h1>
                    <p class="lead mb-4 opacity-75 animate-up delay-1">
                        Stop memorizing lists. Start communicating. <br />
                        Explore our scene-based courses designed for real-world fluency.
                    </p>
                    
                    <div class="d-grid gap-3 d-sm-flex justify-content-sm-center animate-up delay-2">
                        <a href="Courses.aspx" class="btn btn-light btn-lg rounded-pill px-5 fw-bold shadow-sm text-primary">
                            Browse Courses
                        </a>
                        <asp:HyperLink ID="lnkJoinHero" runat="server" NavigateUrl="~/Register.aspx" 
                            CssClass="btn btn-outline-light btn-lg rounded-pill px-5 fw-bold">
                            Join for Free
                        </asp:HyperLink>
                    </div>
                </div>
            </div>
        </div>
        <div class="position-absolute bottom-0 start-0 w-100 overflow-hidden" style="line-height: 0;">
            <svg viewBox="0 0 1200 120" preserveAspectRatio="none" style="width: 100%; height: 60px; fill: #ffffff;">
                <path d="M321.39,56.44c58-10.79,114.16-30.13,172-41.86,82.39-16.72,168.19-17.73,250.45-.39C823.78,31,906.67,72,985.66,92.83c70.05,18.48,146.53,26.09,214.34,3V0H0V27.35A600.21,600.21,0,0,0,321.39,56.44Z"></path>
            </svg>
        </div>
    </div>

    <section class="container mb-5">
        <div class="text-center mb-5">
            <h6 class="text-uppercase text-primary fw-bold tracking-wide">Why LexiPath?</h6>
            <h2 class="fw-bold display-6">Learn the way you live.</h2>
        </div>

        <div class="row g-4">
            <div class="col-md-4">
                <div class="card h-100 border-0 shadow-sm hover-lift text-center p-4 rounded-4">
                    <div class="icon-box bg-success-subtle text-success mb-4 mx-auto rounded-circle d-flex align-items-center justify-content-center" style="width: 80px; height: 80px;">
                        <i class="bi bi-map-fill fs-2"></i>
                    </div>
                    <h4 class="fw-bold">Scenario-Based</h4>
                    <p class="text-muted">Don't just learn words. Learn how to order coffee, hail a taxi, or make a friend in realistic contexts.</p>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card h-100 border-0 shadow-sm hover-lift text-center p-4 rounded-4">
                    <div class="icon-box bg-warning-subtle text-warning mb-4 mx-auto rounded-circle d-flex align-items-center justify-content-center" style="width: 80px; height: 80px;">
                        <i class="bi bi-stopwatch-fill fs-2"></i>
                    </div>
                    <h4 class="fw-bold">Flexible Pace</h4>
                    <p class="text-muted">No streaks to lose. No pressure. Pick up exactly where you left off whenever you have time.</p>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card h-100 border-0 shadow-sm hover-lift text-center p-4 rounded-4">
                    <div class="icon-box bg-info-subtle text-info mb-4 mx-auto rounded-circle d-flex align-items-center justify-content-center" style="width: 80px; height: 80px;">
                        <i class="bi bi-phone-landscape-fill fs-2"></i>
                    </div>
                    <h4 class="fw-bold">Mobile Friendly</h4>
                    <p class="text-muted">Access your lessons from anywhere. Our platform is optimized for learning on the go.</p>
                </div>
            </div>
        </div>
    </section>

    <section class="py-5 bg-light rounded-4 my-5 mx-3 mx-md-5 position-relative overflow-hidden">
        <div class="container position-relative z-1">
            <div class="row align-items-center">
                <div class="col-lg-5 mb-4 mb-lg-0">
                    <h2 class="display-5 fw-bold mb-3">Unlock the Full Experience <span class="text-primary">Free</span></h2>
                    <p class="lead text-muted mb-4">Guest access lets you browse, but registered members get the tools to truly master the language.</p>
                    
                    <asp:HyperLink ID="lnkRegisterBtn" runat="server" NavigateUrl="~/Register.aspx" CssClass="btn btn-primary btn-lg rounded-pill px-5 shadow">
                        Create Free Account
                    </asp:HyperLink>
                </div>

                <div class="col-lg-7">
                    <div class="row g-3">
                        <div class="col-md-6">
                            <div class="p-3 bg-white rounded-3 shadow-sm d-flex align-items-start h-100 border-start border-4 border-primary">
                                <div class="me-3 text-primary"><i class="bi bi-volume-up-fill fs-3"></i></div>
                                <div>
                                    <h5 class="fw-bold mb-1">Native Audio</h5>
                                    <p class="small text-muted mb-0">Hear correct pronunciation for every vocabulary word and phrase.</p>
                                </div>
                            </div>
                        </div>
                        
                        <div class="col-md-6">
                            <div class="p-3 bg-white rounded-3 shadow-sm d-flex align-items-start h-100 border-start border-4 border-success">
                                <div class="me-3 text-success"><i class="bi bi-puzzle-fill fs-3"></i></div>
                                <div>
                                    <h5 class="fw-bold mb-1">Interactive Practice</h5>
                                    <p class="small text-muted mb-0">Test yourself with quizzes to ensure you retain what you learn.</p>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="p-3 bg-white rounded-3 shadow-sm d-flex align-items-start h-100 border-start border-4 border-info">
                                <div class="me-3 text-info"><i class="bi bi-chat-dots-fill fs-3"></i></div>
                                <div>
                                    <h5 class="fw-bold mb-1">Course Discussions</h5>
                                    <p class="small text-muted mb-0">Ask questions, share tips, and interact with other learners.</p>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="p-3 bg-white rounded-3 shadow-sm d-flex align-items-start h-100 border-start border-4 border-warning">
                                <div class="me-3 text-warning"><i class="bi bi-graph-up-arrow fs-3"></i></div>
                                <div>
                                    <h5 class="fw-bold mb-1">Progress Tracking</h5>
                                    <p class="small text-muted mb-0">Visualize your learning hours and track completed courses.</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="container py-5 mb-5">
        <div class="text-center mb-5">
            <h2 class="fw-bold">What Learners Say</h2>
        </div>
        <div class="row justify-content-center">
            <div class="col-md-8">
                <div class="card border-0 shadow-lg rounded-4 text-center p-5" style="background: #fff url('https://www.transparenttextures.com/patterns/cubes.png');">
                    <div class="mb-4 text-warning">
                        <i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i><i class="bi bi-star-fill"></i>
                    </div>
                    <h3 class="fst-italic fw-light mb-4">"The scenario-based approach finally made language learning click for me. I felt confident on my first trip abroad!"</h3>
                    
                    <div class="d-flex align-items-center justify-content-center">
                        <div class="bg-primary text-white rounded-circle d-flex align-items-center justify-content-center fw-bold me-3" style="width: 50px; height: 50px;">JD</div>
                        <div class="text-start">
                            <h6 class="fw-bold mb-0">John Doe</h6>
                            <small class="text-muted">Learner from New York</small>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <style>
        /* Animations */
        .animate-up { animation: fadeInUp 0.8s cubic-bezier(0.2, 0.8, 0.2, 1) forwards; opacity: 0; transform: translateY(20px); }
        .delay-1 { animation-delay: 0.2s; }
        .delay-2 { animation-delay: 0.4s; }
        
        @keyframes fadeInUp {
            to { opacity: 1; transform: translateY(0); }
        }

        /* Hover Effects */
        .hover-lift { transition: transform 0.3s ease, box-shadow 0.3s ease; }
        .hover-lift:hover { transform: translateY(-10px); box-shadow: 0 1rem 3rem rgba(0,0,0,.175)!important; }
        
        .text-primary { color: #667eea !important; }
        .btn-primary { background-color: #667eea; border-color: #667eea; }
        .btn-primary:hover { background-color: #764ba2; border-color: #764ba2; }
        
        .bg-success-subtle { background-color: #d1e7dd; }
        .bg-warning-subtle { background-color: #fff3cd; }
        .bg-info-subtle { background-color: #cff4fc; }
    </style>

</asp:Content>