document.addEventListener('DOMContentLoaded', function () {
    // Add subtle animation to buttons
    const buttons = document.querySelectorAll('.btn-primary, .btn-secondary');

    buttons.forEach(button => {
        button.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-3px) scale(1.02)';
        });

        button.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    // Add typing effect to error title
    const errorTitle = document.querySelector('.error-title');
    const originalText = errorTitle.textContent;
    errorTitle.textContent = '';

    let i = 0;
    function typeWriter() {
        if (i < originalText.length) {
            errorTitle.textContent += originalText.charAt(i);
            i++;
            setTimeout(typeWriter, 100);
        }
    }

    setTimeout(typeWriter, 500);

    // Add floating animation to error icon
    const errorIcon = document.querySelector('.error-icon');
    setInterval(() => {
        errorIcon.style.animation = 'none';
        setTimeout(() => {
            errorIcon.style.animation = 'pulse 2s ease-in-out infinite';
        }, 50);
    }, 5000);
});
