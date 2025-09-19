// Styles
import 'bootstrap/dist/css/bootstrap.min.css'
import '@fortawesome/fontawesome-free/css/all.min.css'
import './main.css'

// JS (includes Popper via bootstrap bundle)
import * as bootstrap from 'bootstrap'
// Expose Bootstrap namespace globally for legacy scripts
if (typeof window !== 'undefined' && !window.bootstrap) {
  window.bootstrap = bootstrap
}

// jQuery + Validation (bundled via npm)
import jQuery from 'jquery'
window.$ = window.jQuery = jQuery
import 'jquery-validation'
import 'jquery-validation-unobtrusive'

// Fonts (local via @fontsource to avoid CSP issues)
import '@fontsource/poppins/400.css'
import '@fontsource/poppins/600.css'
import '@fontsource/poppins/700.css'

// Localization for validation messages (Vietnamese)
if (window.jQuery && window.jQuery.validator) {
  window.jQuery.extend(window.jQuery.validator.messages, {
    required: 'Vui lòng nhập trường này',
    email: 'Email không đúng định dạng',
    number: 'Vui lòng nhập số hợp lệ',
    maxlength: window.jQuery.validator.format('Tối đa {0} ký tự'),
    minlength: window.jQuery.validator.format('Tối thiểu {0} ký tự')
  })
}

// Optional: small runtime check
document.addEventListener('DOMContentLoaded', () => {
  // Example: enable all tooltips if present
  const tooltipTriggerList = Array.from(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
  tooltipTriggerList.forEach((el) => new bootstrap.Tooltip(el))
  // Navbar scroll state
  const nav = document.getElementById('mainNavbar')
  const onScroll = () => {
    if (!nav) return
    if (window.scrollY > 8) nav.classList.add('navbar-scrolled')
    else nav.classList.remove('navbar-scrolled')
  }
  onScroll()
  window.addEventListener('scroll', onScroll, { passive: true })

  // Sidebar toggling: on lg+ collapse/expand left pane, on <lg use offcanvas
  const toggler = document.getElementById('sidebarToggle')
  const sideNav = document.getElementById('sideNav')
  if (toggler && sideNav) {
    const updateExpanded = (state) => {
      try { toggler.setAttribute('aria-expanded', state) } catch {}
    }
    updateExpanded('false')
    toggler.addEventListener('click', () => {
      if (!bootstrap.Offcanvas) return
      const oc = bootstrap.Offcanvas.getOrCreateInstance(sideNav, { scroll: true, backdrop: true })
      oc.toggle()
    })
    sideNav.addEventListener('shown.bs.offcanvas', () => updateExpanded('true'))
    sideNav.addEventListener('hidden.bs.offcanvas', () => updateExpanded('false'))
  }
})






