    function viewDetails(apartmentId) {
        // Navigate to apartment details page
        window.location.href = `/Apartment/Details/${apartmentId}`;
        }

    // Video modal functionality
    function playVideo(videoUrl) {
            const modal = document.getElementById('videoModal');
    const modalVideo = document.getElementById('modalVideo');
    const videoSource = modalVideo.querySelector('source');

    videoSource.src = videoUrl;
    modalVideo.load();
    modal.style.display = 'block';

    // Auto play when modal opens
    modalVideo.play();
        }

    // Close modal functionality
    document.querySelector('.video-modal-close').onclick = function() {
            const modal = document.getElementById('videoModal');
    const modalVideo = document.getElementById('modalVideo');

    modalVideo.pause();
    modalVideo.currentTime = 0;
    modal.style.display = 'none';
        }

    // Close modal when clicking outside
    window.onclick = function(event) {
            const modal = document.getElementById('videoModal');
    if (event.target == modal) {
                const modalVideo = document.getElementById('modalVideo');
    modalVideo.pause();
    modalVideo.currentTime = 0;
    modal.style.display = 'none';
            }
        }

    // Preload video thumbnails after page loads
    document.addEventListener('DOMContentLoaded', function() {
            const videos = document.querySelectorAll('.video-thumbnail video');
            videos.forEach(video => {
        // Set currentTime to 1 second to get a better thumbnail
        video.addEventListener('loadedmetadata', function () {
            if (video.duration >= 1) {
                video.currentTime = 1;
            }
        });
            });
        });
    // Auto-hide alert messages after 5 seconds
    document.addEventListener('DOMContentLoaded', function() {
            const alerts = document.querySelectorAll('.alert');

    alerts.forEach(function(alert) {
        setTimeout(function () {
            alert.classList.add('fade-out');

            setTimeout(function () {
                if (alert.parentNode) {
                    alert.parentNode.removeChild(alert);
                }
            }, 500);
        }, 4000);
            });
        });
    function changePageSize(size) {
    const url = new URL(window.location);
    url.searchParams.set('pageSize', size);
    url.searchParams.set('page', '1'); // Reset to first page
    window.location.href = url.toString();
}
