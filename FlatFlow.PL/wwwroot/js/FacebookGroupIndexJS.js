// Auto-hide alert messages after 5 seconds
document.addEventListener('DOMContentLoaded', function () {
    const alerts = document.querySelectorAll('.alert');

    alerts.forEach(function (alert) {
        setTimeout(function () {
            alert.classList.add('fade-out');

            setTimeout(function () {
                if (alert.parentNode) {
                    alert.parentNode.removeChild(alert);
                }
            }, 500);
        }, 4000);
    });

    // Add staggered animation for tiles
    const tiles = document.querySelectorAll('.group-tile');
    tiles.forEach((tile, index) => {
        tile.style.animationDelay = `${index * 0.1}s`;
        tile.style.animation = 'slideInUp 0.6s ease-out forwards';
    });
});

// Add CSS for slide animation
const style = document.createElement('style');
style.textContent = `
    @@keyframes slideInUp {
        from {
        opacity: 0;
    transform: translateY(30px);
                }
    to {
        opacity: 1;
    transform: translateY(0);
                }
            }

    .group-tile {
        opacity: 0;
            }
    `;
document.head.appendChild(style);
