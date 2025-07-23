document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form');
    const emailInput = document.getElementById('Email');
    const submitBtn = form.querySelector('.btn-login');

    form.addEventListener('submit', function (e) {
        if (!emailInput.value.trim()) {
            e.preventDefault();
            const errorMsg = emailInput.parentElement.parentElement.querySelector('.validation-error');
            if (errorMsg) {
                errorMsg.textContent = 'Email is required.';
                errorMsg.style.display = 'block';
            }
        } else {
            submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Sending...';
            submitBtn.disabled = true;
        }
    });
});

