// Auto-hide alert messages
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

    // Form validation
    const form = document.getElementById('changePasswordForm');
    if (form) {
        form.addEventListener('submit', function (e) {
            const currentPassword = document.getElementById('CurrentPassword').value;
            const newPassword = document.getElementById('NewPassword').value;
            const confirmPassword = document.getElementById('ConfirmPassword').value;

            if (!currentPassword || !newPassword || !confirmPassword) {
                e.preventDefault();
                alert('Please fill in all password fields.');
                return;
            }

            if (newPassword !== confirmPassword) {
                e.preventDefault();
                alert('New password and confirmation password do not match.');
                return;
            }

            if (newPassword.length < 6) {
                e.preventDefault();
                alert('New password must be at least 6 characters long.');
                return;
            }
        });
    }

    // Form animations
    const formControls = document.querySelectorAll('.form-control');
    formControls.forEach(function (control) {
        control.addEventListener('focus', function () {
            this.parentElement.style.transform = 'scale(1.02)';
        });

        control.addEventListener('blur', function () {
            this.parentElement.style.transform = 'scale(1)';
        });
    });
});

// Toggle password visibility
function togglePassword(fieldId) {
    const field = document.getElementById(fieldId);
    const toggle = field.nextElementSibling;

    if (field.type === 'password') {
        field.type = 'text';
        toggle.classList.remove('fa-eye');
        toggle.classList.add('fa-eye-slash');
    } else {
        field.type = 'password';
        toggle.classList.remove('fa-eye-slash');
        toggle.classList.add('fa-eye');
    }
}

// Password strength checker
function checkPasswordStrength(password) {
    const strengthIndicator = document.getElementById('passwordStrength');
    const strengthLevel = document.getElementById('strengthLevel');

    if (password.length === 0) {
        strengthIndicator.style.display = 'none';
        return;
    }

    strengthIndicator.style.display = 'block';

    let strength = 0;
    let strengthText = '';
    let strengthClass = '';

    // Length check
    if (password.length >= 8) strength += 1;
    if (password.length >= 12) strength += 1;

    // Character variety checks
    if (/[a-z]/.test(password)) strength += 1;
    if (/[A-Z]/.test(password)) strength += 1;
    if (/[0-9]/.test(password)) strength += 1;
    if (/[^A-Za-z0-9]/.test(password)) strength += 1;

    // Determine strength level
    if (strength <= 2) {
        strengthText = 'Weak';
        strengthClass = 'strength-weak';
    } else if (strength <= 4) {
        strengthText = 'Fair';
        strengthClass = 'strength-fair';
    } else if (strength <= 5) {
        strengthText = 'Good';
        strengthClass = 'strength-good';
    } else {
        strengthText = 'Strong';
        strengthClass = 'strength-strong';
    }

    // Update UI
    strengthIndicator.className = 'password-strength ' + strengthClass;
    strengthLevel.textContent = strengthText;
}

// Real-time password matching
document.getElementById('ConfirmPassword')?.addEventListener('input', function () {
    const newPassword = document.getElementById('NewPassword').value;
    const confirmPassword = this.value;

    if (confirmPassword && newPassword !== confirmPassword) {
        this.classList.add('is-invalid');
        let errorSpan = this.parentElement.querySelector('.invalid-feedback');
        if (!errorSpan) {
            errorSpan = document.createElement('span');
            errorSpan.className = 'invalid-feedback';
            this.parentElement.appendChild(errorSpan);
        }
        errorSpan.textContent = 'Passwords do not match.';
    } else {
        this.classList.remove('is-invalid');
        const errorSpan = this.parentElement.querySelector('.invalid-feedback');
        if (errorSpan) {
            errorSpan.textContent = '';
        }
    }
});
