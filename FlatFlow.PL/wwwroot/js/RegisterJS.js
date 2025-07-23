document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form');
    const inputs = document.querySelectorAll('.form-control');

    // Add focus animations
    inputs.forEach(input => {
        input.addEventListener('focus', function () {
            this.parentElement.style.transform = 'scale(1.02)';
        });

        input.addEventListener('blur', function () {
            this.parentElement.style.transform = 'scale(1)';
        });
    });

    // Form submission with loading state
    form.addEventListener('submit', function (e) {
        const submitBtn = this.querySelector('.btn-register');
        if (this.checkValidity()) {
            submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Creating Account...';
            submitBtn.disabled = true;
        }
    });

    // Password confirmation validation
    const password = document.querySelector('[name="Password"]');
    const confirmPassword = document.querySelector('[name="ConfirmPassword"]');

    function validatePasswordMatch() {
        if (password.value !== confirmPassword.value && confirmPassword.value !== '') {
            confirmPassword.setCustomValidity('Passwords do not match');
        } else {
            confirmPassword.setCustomValidity('');
        }
    }

    password.addEventListener('input', validatePasswordMatch);
    confirmPassword.addEventListener('input', validatePasswordMatch);
});
