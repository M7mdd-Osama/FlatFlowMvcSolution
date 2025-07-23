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

// Auto dismiss alerts after 5 seconds
setTimeout(function () {
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function (alert) {
        const bsAlert = new bootstrap.Alert(alert);
        bsAlert.close();
    });
}, 5000);
