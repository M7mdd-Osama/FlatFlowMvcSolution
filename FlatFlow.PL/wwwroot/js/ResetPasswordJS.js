document.addEventListener('DOMContentLoaded', function () {
    let timeLeft = 30 * 60;
    const countdownEl = document.getElementById('countdown');

    let resendTimeLeft = 2 * 60;
    const resendBtn = document.getElementById('resendBtn');
    const resendCountdownEl = document.getElementById('resendCountdown');

    let countdownInterval = null;
    let resendInterval = null;

    function startCountdowns() {
        if (countdownInterval) clearInterval(countdownInterval);
        if (resendInterval) clearInterval(resendInterval);

        countdownInterval = setInterval(() => {
            timeLeft--;

            const minutes = Math.floor(timeLeft / 60);
            const seconds = timeLeft % 60;

            countdownEl.textContent = `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;

            if (timeLeft <= 0) {
                clearInterval(countdownInterval);
                countdownEl.textContent = "Expired!";
            }
        }, 1000);

        resendInterval = setInterval(() => {
            resendTimeLeft--;

            const minutes = Math.floor(resendTimeLeft / 60);
            const seconds = resendTimeLeft % 60;

            resendCountdownEl.textContent = `You can resend in ${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;

            if (resendTimeLeft <= 0) {
                clearInterval(resendInterval);
                resendCountdownEl.textContent = "";
                resendBtn.disabled = false;
            }
        }, 1000);
    }

    resendBtn.disabled = true;
    startCountdowns();

    resendBtn.addEventListener('click', async function () {
        try {
            resendBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Sending...';
            resendBtn.disabled = true;

            const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
            if (!tokenElement) {
                throw new Error('Anti-forgery token not found');
            }
            const token = tokenElement.value;

            const emailElement = document.querySelector('input[name="Email"]');
            if (!emailElement) {
                throw new Error('Email not found');
            }
            const email = emailElement.value;

            const response = await fetch('/Account/ResendResetCode', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({ email: email })
            });

            if (response.ok) {
                timeLeft = 30 * 60;
                resendTimeLeft = 2 * 60;
                resendBtn.innerHTML = 'Resend Code';

                startCountdowns();

                alert('A new code has been sent to your email.');
            } else {
                const errorText = await response.text();
                throw new Error(`Server error: ${response.status} - ${errorText}`);
            }
        } catch (error) {
            resendBtn.innerHTML = 'Resend Code';
            resendBtn.disabled = false;
            alert('Error resending code. Please try again.');
            console.error('Error:', error);
        }
    });
});
