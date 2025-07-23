document.addEventListener('DOMContentLoaded', function () {
    // Auto-dismiss alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            alert.classList.add('fade');
            setTimeout(function () {
                alert.remove();
            }, 500);
        }, 5000);
    });

    // Form validation and submission
    const form = document.getElementById('editForm');
    const saveBtn = document.getElementById('saveBtn');
    const inputs = form.querySelectorAll('input[type="text"], input[type="url"]');

    // Real-time validation
    inputs.forEach(input => {
        input.addEventListener('input', function () {
            validateField(this);
        });

        input.addEventListener('blur', function () {
            validateField(this);
        });
    });

    function validateField(field) {
        const value = field.value.trim();
        const fieldName = field.getAttribute('data-val-required') || field.name;

        // Remove existing validation classes
        field.classList.remove('is-valid', 'is-invalid');

        // Basic validation
        if (field.hasAttribute('data-val-required') && !value) {
            field.classList.add('is-invalid');
            showFieldError(field, `${fieldName} is required.`);
            return false;
        }

        if (field.type === 'url' && value && !isValidUrl(value)) {
            field.classList.add('is-invalid');
            showFieldError(field, 'Please enter a valid URL.');
            return false;
        }

        if (field.name === 'GroupLink' && value && !value.includes('facebook.com')) {
            field.classList.add('is-invalid');
            showFieldError(field, 'Please enter a valid Facebook group URL.');
            return false;
        }

        // Field is valid
        field.classList.add('is-valid');
        hideFieldError(field);
        return true;
    }

    function showFieldError(field, message) {
        let errorElement = field.parentNode.parentNode.querySelector('.invalid-feedback');
        if (errorElement) {
            errorElement.innerHTML = `<i class="fas fa-exclamation-circle"></i> ${message}`;
            errorElement.style.display = 'flex';
        }
    }

    function hideFieldError(field) {
        let errorElement = field.parentNode.parentNode.querySelector('.invalid-feedback');
        if (errorElement) {
            errorElement.style.display = 'none';
        }
    }

    function isValidUrl(string) {
        try {
            new URL(string);
            return true;
        } catch (_) {
            return false;
        }
    }

    // Form submission handling
    form.addEventListener('submit', function (e) {
        let isValid = true;

        // Validate all fields
        inputs.forEach(input => {
            if (!validateField(input)) {
                isValid = false;
            }
        });

        if (!isValid) {
            e.preventDefault();
            showToast('Please fix the validation errors before submitting.', 'error');

            // Focus on first invalid field
            const firstInvalid = form.querySelector('.is-invalid');
            if (firstInvalid) {
                firstInvalid.focus();
                firstInvalid.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
            return;
        }

        // Show loading state
        saveBtn.classList.add('btn-loading');
        saveBtn.disabled = true;

        // Allow form to submit normally
        showToast('Updating Facebook group...', 'info');
    });

    // Toast notification function
    function showToast(message, type = 'info') {
        // Remove existing toasts
        const existingToasts = document.querySelectorAll('.toast-notification');
        existingToasts.forEach(toast => toast.remove());

        // Create toast element
        const toast = document.createElement('div');
        toast.className = `toast-notification toast-${type}`;

        // Set icon based on type
        let icon = '';
        switch (type) {
            case 'success':
                icon = 'fa-check-circle';
                break;
            case 'error':
                icon = 'fa-exclamation-circle';
                break;
            case 'info':
            default:
                icon = 'fa-info-circle';
        }

        toast.innerHTML = `
    <div class="toast-content">
        <i class="fas ${icon}"></i>
        <span>${message}</span>
    </div>
    `;

        // Add toast styles
        toast.style.cssText = `
    position: fixed;
    top: 20px;
    right: 20px;
    z-index: 10000;
    background: ${type === 'success' ? 'rgba(16, 185, 129, 0.9)' :
                type === 'error' ? 'rgba(239, 68, 68, 0.9)' : 'rgba(59, 130, 246, 0.9)'};
    color: white;
    padding: 15px 20px;
    border-radius: 10px;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
    backdrop-filter: blur(10px);
    animation: slideInRight 0.3s ease-out;
    min-width: 300px;
    display: flex;
    align-items: center;
    gap: 10px;
    `;

        document.body.appendChild(toast);

        // Auto remove after 3 seconds
        setTimeout(() => {
            toast.style.animation = 'slideOutRight 0.3s ease-out';
            setTimeout(() => {
                toast.remove();
            }, 300);
        }, 3000);
    }

    // Add CSS animations for toast
    const toastStyles = document.createElement('style');
    toastStyles.textContent = `
    @@keyframes slideInRight {
        from {
        transform: translateX(100%);
    opacity: 0;
                    }
    to {
        transform: translateX(0);
    opacity: 1;
                    }
                }

    @@keyframes slideOutRight {
        from {
        transform: translateX(0);
    opacity: 1;
                    }
    to {
        transform: translateX(100%);
    opacity: 0;
                    }
                }

    .toast-notification {
        opacity: 0;
    transform: translateX(100%);
                }
    `;
    document.head.appendChild(toastStyles);

    // Add ripple effect to buttons
    document.addEventListener('click', function (e) {
        if (e.target.classList.contains('btn') || e.target.closest('.btn')) {
            const button = e.target.classList.contains('btn') ? e.target : e.target.closest('.btn');
            const ripple = document.createElement('span');
            const rect = button.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.cssText = `
    position: absolute;
    width: ${size}px;
    height: ${size}px;
    left: ${x}px;
    top: ${y}px;
    background: rgba(255, 255, 255, 0.3);
    border-radius: 50%;
    transform: scale(0);
    animation: ripple 0.6s linear;
    pointer-events: none;
    `;

            button.style.position = 'relative';
            button.style.overflow = 'hidden';
            button.appendChild(ripple);

            setTimeout(() => {
                ripple.remove();
            }, 600);
        }
    });

    // Add ripple animation
    const rippleStyles = document.createElement('style');
    rippleStyles.textContent = `
    @@keyframes ripple {
        to {
        transform: scale(2);
    opacity: 0;
                    }
                }
    `;
    document.head.appendChild(rippleStyles);

    // Auto-format Facebook URL
    const groupLinkInput = document.querySelector('input[name="GroupLink"]');
    if (groupLinkInput) {
        groupLinkInput.addEventListener('blur', function () {
            let url = this.value.trim();
            if (url && !url.startsWith('http')) {
                if (url.includes('facebook.com') || url.includes('fb.com')) {
                    this.value = 'https://' + url;
                }
            }
        });
    }

    // Confirm before delete
    const deleteBtn = document.querySelector('.btn-danger');
    if (deleteBtn) {
        deleteBtn.addEventListener('click', function (e) {
            if (!confirm('Are you sure you want to delete this group? This action cannot be undone.')) {
                e.preventDefault();
            }
        });
    }
});
