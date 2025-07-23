// Auto-hide alert messages after 5 seconds
document.addEventListener('DOMContentLoaded', function () {
    const alerts = document.querySelectorAll('.alert');

    alerts.forEach(function (alert) {
        setTimeout(function () {
            const bsAlert = new bootstrap.Alert(alert);
            if (bsAlert) {
                bsAlert.close();
            }
        }, 5000);
    });

    // Add form validation feedback
    const form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            const firstNameInput = document.getElementById('FirstName');

            if (!firstNameInput.value.trim()) {
                e.preventDefault();
                firstNameInput.classList.add('is-invalid');

                // Show error message if it doesn't exist
                let errorSpan = firstNameInput.nextElementSibling;
                if (!errorSpan || !errorSpan.classList.contains('invalid-feedback')) {
                    errorSpan = document.createElement('span');
                    errorSpan.className = 'invalid-feedback';
                    firstNameInput.parentNode.appendChild(errorSpan);
                }
                errorSpan.textContent = 'First name is required.';
            }
        });
    }

    // Real-time validation
    const firstNameInput = document.getElementById('FirstName');
    if (firstNameInput) {
        firstNameInput.addEventListener('input', function () {
            if (this.value.trim()) {
                this.classList.remove('is-invalid');
                const errorSpan = this.nextElementSibling;
                if (errorSpan && errorSpan.classList.contains('invalid-feedback')) {
                    errorSpan.textContent = '';
                }
            }
        });
    }

    // Smooth animations for form elements
    const formControls = document.querySelectorAll('.form-control');
    formControls.forEach(function (control) {
        control.addEventListener('focus', function () {
            this.style.transform = 'scale(1.02)';
        });

        control.addEventListener('blur', function () {
            this.style.transform = 'scale(1)';
        });
    });
});
