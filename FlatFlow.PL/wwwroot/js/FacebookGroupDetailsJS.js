document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.copy-link-btn').forEach(el => {
        el.addEventListener('click', function () {
            const link = this.getAttribute('data-link');
            if (!link) return;

            navigator.clipboard.writeText(link)
                .then(() => {
                    showToast('Group link copied!', 'success');
                })
                .catch(err => {
                    console.error('Copy failed:', err);
                    showToast('Failed to copy link', 'error');
                });
        });
    });
    // Auto-hide alert messages
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            alert.classList.add('fade-out');
            setTimeout(function () {
                if (alert.parentNode) {
                    alert.parentNode.removeChild(alert);
                }
            }, 500);
        }, 5000);
    });

    // Delete confirmation with SweetAlert2
    const deleteButtons = document.querySelectorAll('.btn-delete');
    deleteButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();
            const groupId = @Model.Id; // Get group ID from model
            const groupName = '@Model.GroupName'; // Get group name from model

            Swal.fire({
                title: 'Delete Facebook Group?',
                html: `Are you sure you want to delete <strong>${groupName}</strong>?<br>This action cannot be undone!`,
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
            }).then(async (result) => {
                if (result.isConfirmed) {
                    // Show loading indicator
                    Swal.fire({
                        title: 'Deleting...',
                        text: 'Please wait while we delete the group.',
                        icon: 'info',
                        allowOutsideClick: false,
                        showConfirmButton: false,
                        background: 'rgba(26, 26, 46, 0.95)',
                        color: '#ffffff',
                        didOpen: () => {
                            Swal.showLoading();
                        }
                    });

                    try {
                        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
                        const response = await fetch('/FacebookGroup/DeleteAjax', {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/x-www-form-urlencoded',
                                'X-Requested-With': 'XMLHttpRequest'
                            },
                            body: `id=${groupId}&__RequestVerificationToken=${token}`
                        });

                        const result = await response.json();

                        if (result.success) {
                            Swal.fire({
                                title: 'Deleted!',
                                text: result.message,
                                icon: 'success',
                                background: 'rgba(26, 26, 46, 0.95)',
                                color: '#ffffff',
                                confirmButtonColor: '#10b981',
                                timer: 2000,
                                timerProgressBar: true
                            }).then(() => {
                                // Redirect to groups list
                                window.location.href = '/FacebookGroup/Index';
                            });
                        } else {
                            Swal.fire({
                                title: 'Error!',
                                text: result.message || 'Failed to delete the group.',
                                icon: 'error',
                                background: 'rgba(26, 26, 46, 0.95)',
                                color: '#ffffff',
                                confirmButtonColor: '#ef4444'
                            });
                        }
                    } catch (error) {
                        console.error('Delete error:', error);
                        Swal.fire({
                            title: 'Error!',
                            text: 'An error occurred while deleting the group.',
                            icon: 'error',
                            background: 'rgba(26, 26, 46, 0.95)',
                            color: '#ffffff',
                            confirmButtonColor: '#ef4444'
                        });
                    }
                }
            });
        });
    });
    // Toggle post status confirmation
    const toggleButtons = document.querySelectorAll('.btn-toggle');
    toggleButtons.forEach(button => {
        button.addEventListener('click', function () {
            const apartmentId = this.getAttribute('data-apartment-id');
            const groupId = this.getAttribute('data-group-id');
            const isCurrentlyPosted = this.classList.contains('btn-delete');
            const action = isCurrentlyPosted ? 'unmark as not posted' : 'mark as posted';

            Swal.fire({
                title: 'Confirm Status Change',
                html: `Are you sure you want to <strong>${action}</strong> this apartment in the group?`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#10b981',
                cancelButtonColor: '#6b7280',
                confirmButtonText: `Yes, ${action}`,
                cancelButtonText: 'Cancel',
                background: 'rgba(26, 26, 46, 0.95)',
                color: '#ffffff',
                backdrop: 'rgba(0,0,0,0.8)'
            }).then((result) => {
                if (result.isConfirmed) {
                    togglePostStatus(apartmentId, groupId, this);
                }
            });
        });
    });

    // Toast notification function
    function showToast(message, type = 'info') {
        const toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true,
            background: 'rgba(26, 26, 46, 0.95)',
            color: '#ffffff',
            didOpen: (toast) => {
                toast.addEventListener('mouseenter', Swal.stopTimer)
                toast.addEventListener('mouseleave', Swal.resumeTimer)
            }
        });

        toast.fire({
            icon: type,
            title: message
        });
    }

    // Toggle post status function
    async function togglePostStatus(apartmentId, groupId, button) {
        const originalText = button.innerHTML;
        button.disabled = true;
        button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Updating...';

        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            const response = await fetch('/FacebookGroup/TogglePostStatus', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: `apartmentId=${apartmentId}&groupId=${groupId}&__RequestVerificationToken=${token}`
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const result = await response.json();

            if (result.success) {
                // Update UI
                const isPosted = result.isPosted;
                button.className = `btn btn-toggle ${isPosted ? 'btn-delete' : 'btn-success'}`;
                button.innerHTML = `<i class="fas ${isPosted ? 'fa-times' : 'fa-check'}"></i> ${isPosted ? 'Mark Not Posted' : 'Mark Posted'}`;

                // Update status badge
                const statusBadge = button.closest('.post-card').querySelector('.post-status');
                statusBadge.className = `post-status ${isPosted ? 'status-posted' : 'status-not-posted'}`;
                statusBadge.innerHTML = isPosted ?
                    '<i class="fas fa-check"></i> <span>Posted</span>' :
                    '<i class="fas fa-clock"></i> <span>Pending</span>';

                showToast(result.message, 'success');
                updateStatsCounters();
            } else {
                showToast(result.message || 'Error updating post status', 'error');
                button.innerHTML = originalText;
            }
        } catch (error) {
            console.error('Error:', error);
            showToast('Error updating post status', 'error');
            button.innerHTML = originalText;
        }

        button.disabled = false;
    }
    // Update stats counters
    function updateStatsCounters() {
        const allPosts = document.querySelectorAll('.post-card').length;
        const postedPosts = document.querySelectorAll('.status-posted').length;
        const pendingPosts = document.querySelectorAll('.status-not-posted').length;
        const postingRate = allPosts > 0 ? Math.round((postedPosts / allPosts) * 100 * 10) / 10 : 0;

        // Update stats cards
        const statsCards = document.querySelectorAll('.stats-card .stats-number');
        if (statsCards.length >= 4) {
            statsCards[0].textContent = allPosts;
            statsCards[1].textContent = postedPosts;
            statsCards[2].textContent = pendingPosts;
            statsCards[3].textContent = postingRate + '%';
        }
    }

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

    // Add CSS for animations
    const style = document.createElement('style');
    style.textContent = `
    @@keyframes ripple {
        to {
        transform: scale(2);
    opacity: 0;
                    }
                }
    .fade-out {
        opacity: 0;
    transition: opacity 0.5s ease;
                }
    .btn-loading .btn-text {
        opacity: 0;
                }
    .btn-loading::after {
        content: '';
    position: absolute;
    top: 50%;
    left: 50%;
    width: 20px;
    height: 20px;
    border: 2px solid transparent;
    border-top: 2px solid currentColor;
    border-radius: 50%;
    animation: spin 1s linear infinite;
    transform: translate(-50%, -50%);
                }
    @@keyframes spin {
        0 % { transform: translate(-50 %, -50 %) rotate(0deg); }
                    100% {transform: translate(-50%, -50%) rotate(360deg); }
                }
    `;
    document.head.appendChild(style);
});
