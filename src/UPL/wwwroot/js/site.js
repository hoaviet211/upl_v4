// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Navbar scroll behavior and offcanvas link close
(function(){
  const navbar = document.getElementById('mainNavbar');
  const offcanvasEl = document.getElementById('mainNavOffcanvas');

  const onScroll = () => {
    if (!navbar) return;
    if (window.scrollY > 24) navbar.classList.add('navbar-scrolled');
    else navbar.classList.remove('navbar-scrolled');
  };

  window.addEventListener('scroll', onScroll, { passive: true });
  document.addEventListener('DOMContentLoaded', onScroll);

  if (offcanvasEl && window.bootstrap) {
    const instance = window.bootstrap.Offcanvas.getOrCreateInstance(offcanvasEl);
    offcanvasEl.addEventListener('click', (e) => {
      const t = e.target;
      if (t && t.closest('.nav-link, .btn')) instance.hide();
    });
  }
})();
