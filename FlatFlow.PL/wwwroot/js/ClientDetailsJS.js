// Function to confirm delete
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

            // Submit the delete form
            const form = document.getElementById('deleteForm');
            form.action = `/Client/DeleteConfirmed/${clientId}`;
            form.submit();
        }
    });
}
