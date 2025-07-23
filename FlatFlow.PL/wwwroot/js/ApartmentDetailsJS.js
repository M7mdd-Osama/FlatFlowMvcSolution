// Media modal functions
function openMediaModal(mediaUrl, isVideo) {
    const modal = document.getElementById('mediaModal');
    const modalImage = document.getElementById('modalImage');
    const modalVideo = document.getElementById('modalVideo');
    modalImage.style.display = 'none';
    modalVideo.style.display = 'none';
    if (isVideo) {
        modalVideo.src = mediaUrl;
        modalVideo.style.display = 'block';
        modalVideo.load();
    } else {
        modalImage.src = mediaUrl;
        modalImage.style.display = 'block';
    }
    modal.style.display = 'block';
}

function changeMainMedia(mediaUrl, thumbnail, isVideo) {
    const mainImageContainer = document.getElementById('mainImage');
    const mainVideoContainer = document.getElementById('mainVideo');
    const galleryContainer = document.querySelector('.image-gallery');
    if (mainImageContainer) mainImageContainer.remove();
    if (mainVideoContainer && mainVideoContainer.parentElement) {
        mainVideoContainer.parentElement.remove();
    }
    document.querySelectorAll('.thumbnail').forEach(thumb => {
        if (thumb.tagName === 'VIDEO') {
            thumb.parentElement.classList.remove('active');
        } else {
            thumb.classList.remove('active');
        }
    });
    if (thumbnail.tagName === 'VIDEO') {
        thumbnail.parentElement.classList.add('active');
    } else {
        thumbnail.classList.add('active');
    }
    if (isVideo) {
        const videoDiv = document.createElement('div');
        videoDiv.className = 'video-thumbnail';
        videoDiv.innerHTML = `
    <video class="main-video" id="mainVideo" preload="metadata" muted onclick="openMediaModal('${mediaUrl}', true)">
        <source src="${mediaUrl}#t=1" type="video/mp4" />
    </video>
    <div class="video-indicator">
        <i class="fas fa-video"></i>
        <span>Video</span>
    </div>
    <div class="video-play-overlay" onclick="openMediaModal('${mediaUrl}', true)">
        <i class="fas fa-play"></i>
    </div>
    `;
        galleryContainer.insertBefore(videoDiv, galleryContainer.querySelector('.image-thumbnails'));
    } else {
        const img = document.createElement('img');
        img.src = mediaUrl;
        img.alt = 'Apartment Image';
        img.className = 'main-image';
        img.id = 'mainImage';
        img.onclick = () => openMediaModal(mediaUrl, false);
        galleryContainer.insertBefore(img, galleryContainer.querySelector('.image-thumbnails'));
    }
}

function closeMediaModal() {
    const modal = document.getElementById('mediaModal');
    const modalVideo = document.getElementById('modalVideo');
    if (modalVideo && !modalVideo.paused) {
        modalVideo.pause();
    }
    modal.style.display = 'none';
    const modalImage = document.getElementById('modalImage');
    if (modalImage) modalImage.src = '';
    if (modalVideo) {
        modalVideo.src = '';
        modalVideo.load();
    }
}

// Toggle post status function for Facebook groups
function togglePostStatus(apartmentId, groupId, currentStatus) {
    const actionText = currentStatus ? 'mark as not posted' : 'mark as posted';

    Swal.fire({
        title: 'Confirm Action',
        text: `Are you sure you want to ${actionText} for this Facebook group?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#7c3aed',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Yes, update it!',
        background: 'rgba(26, 26, 46, 0.95)',
        color: '#ffffff',
        backdrop: 'rgba(0,0,0,0.8)'
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(`/Apartment/TogglePostStatus`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: JSON.stringify({
                    apartmentId: apartmentId,
                    groupId: groupId,
                    isPosted: !currentStatus
                })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        Swal.fire({
                            title: 'Success!',
                            text: data.message,
                            icon: 'success',
                            background: 'rgba(26, 26, 46, 0.95)',
                            color: '#ffffff',
                            confirmButtonColor: '#7c3aed'
                        }).then(() => {
                            location.reload();
                        });
                    } else {
                        Swal.fire({
                            title: 'Error!',
                            text: data.message,
                            icon: 'error',
                            background: 'rgba(26, 26, 46, 0.95)',
                            color: '#ffffff',
                            confirmButtonColor: '#ef4444'
                        });
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    Swal.fire({
                        title: 'Error!',
                        text: 'Something went wrong. Please try again.',
                        icon: 'error',
                        background: 'rgba(26, 26, 46, 0.95)',
                        color: '#ffffff',
                        confirmButtonColor: '#ef4444'
                    });
                });
        }
    });
}

// Existing functions for apartment actions
function toggleRentStatus(apartmentId, currentStatus) {
    Swal.fire({
        title: 'Confirm Status Change',
        text: `Are you sure you want to mark this apartment as ${currentStatus ? 'Available' : 'Rented'}?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#7c3aed',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Yes, change it!',
        background: 'rgba(26, 26, 46, 0.95)',
        color: '#ffffff',
        backdrop: 'rgba(0,0,0,0.8)'
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(`/Apartment/ToggleRentStatus/${apartmentId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        Swal.fire({
                            title: 'Success!',
                            text: data.message,
                            icon: 'success',
                            background: 'rgba(26, 26, 46, 0.95)',
                            color: '#ffffff',
                            confirmButtonColor: '#7c3aed'
                        }).then(() => {
                            location.reload();
                        });
                    } else {
                        Swal.fire({
                            title: 'Error!',
                            text: data.message,
                            icon: 'error',
                            background: 'rgba(26, 26, 46, 0.95)',
                            color: '#ffffff',
                            confirmButtonColor: '#ef4444'
                        });
                    }
                })
                .catch(error => {
                    Swal.fire({
                        title: 'Error!',
                        text: 'Something went wrong. Please try again.',
                        icon: 'error',
                        background: 'rgba(26, 26, 46, 0.95)',
                        color: '#ffffff',
                        confirmButtonColor: '#ef4444'
                    });
                });
        }
    });
}

function confirmDelete(apartmentId) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this! This will permanently delete the apartment and all its data.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Yes, delete it!',
        background: 'rgba(26, 26, 46, 0.95)',
        color: '#ffffff',
        backdrop: 'rgba(0,0,0,0.8)'
    }).then((result) => {
        if (result.isConfirmed) {
            const form = document.getElementById('deleteForm');
            form.action = `/Apartment/Delete/${apartmentId}`;
            form.submit();
        }
    });
}

// Modal event listeners
document.addEventListener('DOMContentLoaded', function () {
    const modal = document.getElementById('mediaModal');
    const closeBtn = document.querySelector('.media-modal-close');

    closeBtn.onclick = function () {
        closeMediaModal();
    };

    modal.onclick = function (event) {
        if (event.target === modal) {
            closeMediaModal();
        }
    };

    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape' && modal.style.display === 'block') {
            closeMediaModal();
        }
    });
});
