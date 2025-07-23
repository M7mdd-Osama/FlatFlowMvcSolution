// Add this JavaScript to your login form (replace the existing script)
document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form');
    const inputs = document.querySelectorAll('.form-control');
    const validationSummary = document.querySelector('.validation-summary');

    // Show validation summary if there are errors
    if (validationSummary && validationSummary.querySelector('li')) {
        validationSummary.style.display = 'block';
    }

    // Show individual field errors
    const errorMessages = document.querySelectorAll('.validation-error');
    errorMessages.forEach(error => {
        if (error.textContent.trim()) {
            error.style.display = 'block';
        }
    });

    // Add focus animations
    inputs.forEach(input => {
        input.addEventListener('focus', function () {
            this.parentElement.style.transform = 'scale(1.02)';
            // Hide error message on focus
            const errorMsg = this.parentElement.parentElement.querySelector('.validation-error');
            if (errorMsg) {
                errorMsg.style.display = 'none';
            }
        });

        input.addEventListener('blur', function () {
            this.parentElement.style.transform = 'scale(1)';
        });
    });

    // Form submission with loading state
    form.addEventListener('submit', function (e) {
        const submitBtn = this.querySelector('.btn-login');

        // Basic client-side validation
        let hasErrors = false;
        inputs.forEach(input => {
            if (input.required && !input.value.trim()) {
                hasErrors = true;
                const errorMsg = input.parentElement.parentElement.querySelector('.validation-error');
                if (errorMsg) {
                    errorMsg.textContent = 'This field is required.';
                    errorMsg.style.display = 'block';
                }
            }
        });

        if (!hasErrors) {
            submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Signing In...';
            submitBtn.disabled = true;
        } else {
            e.preventDefault();
        }
    });

    // Remember me checkbox handling
    const rememberCheckbox = document.getElementById('rememberMe');
    if (rememberCheckbox) {
        rememberCheckbox.addEventListener('change', function () {
            console.log('Remember me:', this.checked);
        });
    }
});
