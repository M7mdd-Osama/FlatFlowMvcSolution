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
});

// Handle confirmation text for account deletion
document.getElementById('confirmationText').addEventListener('input', function () {
    const confirmBtn = document.getElementById('confirmDeleteBtn');
    const confirmText = this.value.trim();

    if (confirmText === 'DELETE MY ACCOUNT') {
        confirmBtn.disabled = false;
        confirmBtn.classList.remove('btn-outline-danger');
        confirmBtn.classList.add('btn-danger');
    } else {
        confirmBtn.disabled = true;
        confirmBtn.classList.remove('btn-danger');
        confirmBtn.classList.add('btn-outline-danger');
    }
});

// Reset modal when closed
document.getElementById('deleteAccountModal').addEventListener('hidden.bs.modal', function () {
    document.getElementById('confirmationText').value = '';
    const confirmBtn = document.getElementById('confirmDeleteBtn');
    confirmBtn.disabled = true;
    confirmBtn.classList.remove('btn-danger');
    confirmBtn.classList.add('btn-outline-danger');
});
