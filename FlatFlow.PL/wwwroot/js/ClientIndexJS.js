// Search functionality
document.getElementById('searchInput').addEventListener('input', function () {
    const searchTerm = this.value.toLowerCase();
    const rows = document.querySelectorAll('tbody tr');

    rows.forEach(row => {
        const name = row.cells[0].textContent.toLowerCase();
        const phone = row.cells[1].textContent.toLowerCase();

        if (name.includes(searchTerm) || phone.includes(searchTerm)) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
});

// Status filter
document.getElementById('statusFilter').addEventListener('change', function () {
    const selectedStatus = this.value;
    const rows = document.querySelectorAll('tbody tr');

    rows.forEach(row => {
        const status = row.cells[3].textContent.trim();

        if (selectedStatus === '' || status === selectedStatus) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
});

// Sort functionality
document.getElementById('sortBy').addEventListener('change', function () {
    const sortBy = this.value;
    const tbody = document.querySelector('tbody');
    const rows = Array.from(tbody.querySelectorAll('tr'));

    rows.sort((a, b) => {
        let aValue, bValue;

        switch (sortBy) {
            case 'name':
                aValue = a.cells[0].textContent.trim().toLowerCase();
                bValue = b.cells[0].textContent.trim().toLowerCase();
                return aValue.localeCompare(bValue);

            case 'date':
                // Assuming you want to sort by creation date (you might need to add a hidden date field)
                // For now, sorting by name as fallback
                aValue = a.cells[0].textContent.trim().toLowerCase();
                bValue = b.cells[0].textContent.trim().toLowerCase();
                return aValue.localeCompare(bValue);

            case 'commission':
                // Extract commission values
                aValue = parseFloat(a.cells[4].textContent.replace(/[^0-9.-]+/g, '') || '0');
                bValue = parseFloat(b.cells[4].textContent.replace(/[^0-9.-]+/g, '') || '0');
                return bValue - aValue; // Descending order

            default:
                return 0;
        }
    });

    // Clear tbody and append sorted rows
    tbody.innerHTML = '';
    rows.forEach(row => tbody.appendChild(row));
});

// Alternative: Change only the JavaScript to use DeleteConfirmed
function confirmDelete(clientId, clientName) {
    Swal.fire({
        title: 'Are you sure?',
        text: `You won't be able to revert this! This will permanently delete ${clientName} and all their data.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'Cancel',
        background: 'rgba(26, 26, 46, 0.95)',
        color: '#ffffff',
        backdrop: 'rgba(0,0,0,0.8)',
        customClass: {
            popup: 'swal2-popup',
            title: 'swal2-title',
            htmlContainer: 'swal2-html-container',
            confirmButton: 'swal2-confirm',
            cancelButton: 'swal2-cancel'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            // Show loading
            Swal.fire({
                title: 'Deleting...',
                text: 'Please wait while we delete the client.',
                icon: 'info',
                allowOutsideClick: false,
                showConfirmButton: false,
                background: 'rgba(26, 26, 46, 0.95)',
                color: '#ffffff',
                didOpen: () => {
                    Swal.showLoading();
                }
            });

            // Submit the delete form - Changed to use DeleteConfirmed
            const form = document.getElementById('deleteForm');
            form.action = `/Client/DeleteConfirmed/${clientId}`;
            form.submit();
        }
    });
}
// Combined filter function (search + status)
function applyFilters() {
    const searchTerm = document.getElementById('searchInput').value.toLowerCase();
    const selectedStatus = document.getElementById('statusFilter').value;
    const rows = document.querySelectorAll('tbody tr');

    rows.forEach(row => {
        const name = row.cells[0].textContent.toLowerCase();
        const phone = row.cells[1].textContent.toLowerCase();
        const status = row.cells[3].textContent.trim();

        const matchesSearch = name.includes(searchTerm) || phone.includes(searchTerm);
        const matchesStatus = selectedStatus === '' || status === selectedStatus;

        if (matchesSearch && matchesStatus) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
}

// Update event listeners to use combined filter
document.getElementById('searchInput').addEventListener('input', applyFilters);
document.getElementById('statusFilter').addEventListener('change', applyFilters);

// Auto-hide alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function () {
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.opacity = '0';
            alert.style.transition = 'opacity 0.5s ease';
            setTimeout(() => {
                alert.remove();
            }, 500);
        }, 5000);
    });
});

// Add some visual feedback on button hover
document.addEventListener('DOMContentLoaded', function () {
    const deleteButtons = document.querySelectorAll('.btn-danger');

    deleteButtons.forEach(button => {
        button.addEventListener('mouseenter', function () {
            this.style.transform = 'scale(1.1)';
        });

        button.addEventListener('mouseleave', function () {
            this.style.transform = 'scale(1)';
        });
    });
});

function changePageSize(size) {
    const url = new URL(window.location);
    url.searchParams.set('pageSize', size);
    url.searchParams.set('page', '1');
    window.location.href = url.toString();
}
