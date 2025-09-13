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
})
