$(document).ready(function () {
    // Initialize form validation
    $('#facebookGroupForm').removeData('validator').removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse('#facebookGroupForm');

    // Custom validation method for Facebook URLs
    $.validator.addMethod("facebookurl", function (value, element) {
        if (!value) return false;
        var facebookPattern = /^https?:\/\/(www\.)?(facebook\.com|fb\.com)\//i;
        return facebookPattern.test(value);
    }, "Please enter a valid Facebook URL");

    // Add custom validation rules
    $("#GroupLink").rules("add", {
        facebookurl: true
    });

    // Form animation
    const formCard = $('.form-card');
    formCard.css({
        'opacity': '0',
        'transform': 'translateY(30px)'
    });

    setTimeout(() => {
        formCard.css({
            'transition': 'all 0.6s ease',
            'opacity': '1',
            'transform': 'translateY(0)'
        });
    }, 200);

    // Enhanced input interactions
    $('.form-control').on('focus', function () {
        $(this).css({
            'border-color': '#8b5cf6',
            'box-shadow': '0 0 0 0.2rem rgba(139, 92, 246, 0.25)'
        });
    });

    $('.form-control').on('blur', function () {
        const $this = $(this);
        const $validationSpan = $this.siblings('.field-validation-error');

        if ($this.val().trim() !== '' && $validationSpan.length === 0) {
            $this.css({
                'border-color': 'rgba(16, 185, 129, 0.5)',
                'box-shadow': '0 0 0 0.2rem rgba(16, 185, 129, 0.15)'
            });
        }
    });

    // Auto-format Facebook URL
    $('#GroupLink').on('blur', function () {
        let url = $(this).val().trim();
        if (url && !url.match(/^https?:\/\//i)) {
            if (url.includes('facebook.com') || url.includes('fb.com')) {
                $(this).val('https://' + url);
            }
        }
    });

    // Form submission with validation
    $('#facebookGroupForm').on('submit', function (e) {
        e.preventDefault();

        // Clear previous validation summary
        $('#validationSummary').hide().find('ul').empty();

        if ($(this).valid()) {
            // If form is valid, you can submit to server
            console.log('Form is valid, submitting...');

            // Simulate server validation errors for demonstration
            setTimeout(() => {
                // Example of showing server-side validation errors
                showServerValidationErrors({
                    'GroupLink': 'This Facebook group already exists in the system.'
                });
            }, 1000);

        } else {
            // Form has client-side validation errors
            console.log('Form has validation errors');

            // Scroll to first error
            const firstError = $('.input-validation-error').first();
            if (firstError.length) {
                $('html, body').animate({
                    scrollTop: firstError.offset().top - 100
                }, 500);
            }
        }
    });

    // Function to show server-side validation errors
    function showServerValidationErrors(errors) {
        const $summary = $('#validationSummary');
        const $summaryList = $summary.find('ul');

        // Clear existing errors
        $('.field-validation-error').removeClass('field-validation-error').addClass('field-validation-valid').empty();
        $('.input-validation-error').removeClass('input-validation-error');

        // Add field-specific errors
        Object.keys(errors).forEach(function (field) {
            const $input = $(`#${field}`);
            const $span = $input.siblings('[data-valmsg-for="' + field + '"]');

            $input.addClass('input-validation-error');
            $span.removeClass('field-validation-valid')
                .addClass('field-validation-error')
                .html('<i class="fas fa-exclamation-circle"></i> ' + errors[field]);

            // Add to summary
            $summaryList.append('<li>' + errors[field] + '</li>');
        });

        // Show validation summary if there are errors
        if ($summaryList.children().length > 0) {
            $summary.show();
        }
    }

    // Add floating particles
    const formContent = $('.form-content');
    for (let i = 0; i < 3; i++) {
        const particle = $('<div>').css({
            'position': 'absolute',
            'width': '4px',
            'height': '4px',
            'background': 'rgba(147, 51, 234, 0.6)',
            'border-radius': '50%',
            'pointer-events': 'none',
            'animation': `floatParticle 8s infinite ${i * 2}s`,
            'top': `${20 + i * 30}%`,
            'left': `${10 + i * 25}%`,
            'z-index': '0'
        });
        formContent.append(particle);
    }

    // Add particle animation CSS
    $('<style>').text(`
            @@keyframes floatParticle {
                0 %, 100 % {
                    transform: translateY(0px) translateX(0px);
                        opacity: 0.3;
                }
                    25% {
                transform: translateY(-15px) translateX(5px);
            opacity: 0.7;
                    }
            50% {
                transform: translateY(-25px) translateX(-5px);
            opacity: 1;
                    }
            75% {
                transform: translateY(-10px) translateX(10px);
            opacity: 0.5;
                    }
                }
            `).appendTo('head');
});
