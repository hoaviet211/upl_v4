(() => {
  const navbar = document.getElementById('mainNavbar');
  const offcanvasEl = document.getElementById('mainNavOffcanvas');

  // Sticky style change after scroll > 24px
  const onScroll = () => {
    if (!navbar) return;
    if (window.scrollY > 24) {
      navbar.classList.add('navbar-scrolled');
    } else {
      navbar.classList.remove('navbar-scrolled');
    }
  };

  window.addEventListener('scroll', onScroll, { passive: true });
  document.addEventListener('DOMContentLoaded', onScroll);

  // Close offcanvas after clicking a nav link
  if (offcanvasEl && typeof bootstrap !== 'undefined') {
    const instance = bootstrap.Offcanvas.getOrCreateInstance(offcanvasEl);
    offcanvasEl.addEventListener('click', (e) => {
      const target = e.target;
      if (target && target.closest('.nav-link, .btn')) {
        instance.hide();
      }
    });
  }
})();

