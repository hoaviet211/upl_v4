// Styles
import 'bootstrap/dist/css/bootstrap.min.css'
import '@fortawesome/fontawesome-free/css/all.min.css'
import './main.css'

// JS (includes Popper via bootstrap bundle)
import 'bootstrap'

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
  tooltipTriggerList.forEach((el) => new window.bootstrap.Tooltip(el))
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
  const layoutShell = document.querySelector('.layout-shell')
  const isLgUp = () => window.matchMedia('(min-width: 992px)').matches
  if (toggler && layoutShell) {
    try { toggler.setAttribute('aria-expanded', 'true') } catch {}
    toggler.addEventListener('click', (e) => {
      if (isLgUp()) {
        // Desktop/Tablet: collapse only the left sidebar column (no page overlay)
        const collapsed = layoutShell.classList.toggle('sidebar-collapsed')
        toggler.setAttribute('aria-expanded', (!collapsed).toString())
      } else {
        // Mobile: open Bootstrap Offcanvas programmatically
        const oc = window.bootstrap.Offcanvas.getOrCreateInstance('#sideNav', { scroll: true, backdrop: true })
        oc.toggle()
      }
    })
  }
})
