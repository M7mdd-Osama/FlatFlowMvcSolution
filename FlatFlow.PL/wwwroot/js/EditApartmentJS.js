// Array to track images marked for deletion
let imagesToDelete = [];

// File upload functionality
const fileInput = document.getElementById('fileInput');
const filePreview = document.getElementById('filePreview');
const uploadArea = document.querySelector('.file-upload-area');

fileInput.addEventListener('change', function (e) {
    handleFiles(e.target.files);
});

// Drag and drop functionality
uploadArea.addEventListener('dragover', function (e) {
    e.preventDefault();
    uploadArea.classList.add('dragover');
});

uploadArea.addEventListener('dragleave', function (e) {
    e.preventDefault();
    uploadArea.classList.remove('dragover');
});

uploadArea.addEventListener('drop', function (e) {
    e.preventDefault();
    uploadArea.classList.remove('dragover');
    handleFiles(e.dataTransfer.files);
});

function handleFiles(files) {
    if (files.length > 0) {
        filePreview.style.display = 'grid';
        filePreview.innerHTML = '';

        Array.from(files).forEach((file, index) => {
            const fileItem = document.createElement('div');
            fileItem.className = 'image-item';

            if (file.type.startsWith('image/')) {
                const img = document.createElement('img');
                img.src = URL.createObjectURL(file);
                img.alt = file.name;
                fileItem.appendChild(img);
            } else if (file.type.startsWith('video/')) {
                const video = document.createElement('video');
                video.src = URL.createObjectURL(file);
                video.style.width = '100%';
                video.style.height = '100%';
                video.style.objectFit = 'cover';
                video.controls = false;
                fileItem.appendChild(video);

                const videoIcon = document.createElement('div');
                videoIcon.innerHTML = '<i class="fas fa-play" style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); font-size: 2rem; color: white;"></i>';
                fileItem.appendChild(videoIcon);
            }

            const fileName = document.createElement('div');
            fileName.style.position = 'absolute';
            fileName.style.bottom = '0';
            fileName.style.left = '0';
            fileName.style.right = '0';
            fileName.style.background = 'rgba(0,0,0,0.7)';
            fileName.style.color = 'white';
            fileName.style.padding = '5px';
            fileName.style.fontSize = '0.7rem';
            fileName.textContent = file.name;
            fileItem.appendChild(fileName);

            filePreview.appendChild(fileItem);
        });
    } else {
        filePreview.style.display = 'none';
    }
}

// Mark image for deletion (visual only, actual deletion happens on form submit)
function markForDeletion(imageUrl, button) {
    const imageItem = button.closest('.image-item');
    const index = imagesToDelete.indexOf(imageUrl);

    if (index === -1) {
        // Mark for deletion
        imagesToDelete.push(imageUrl);
        imageItem.classList.add('marked-for-deletion');
        button.innerHTML = '<i class="fas fa-undo"></i>';
        button.classList.add('undo-btn');
        button.title = 'Click to undo deletion';

        showMessage('info', 'Image marked for deletion. Click Update to apply changes.');
    } else {
        // Undo deletion
        imagesToDelete.splice(index, 1);
        imageItem.classList.remove('marked-for-deletion');
        button.innerHTML = '<i class="fas fa-times"></i>';
        button.classList.remove('undo-btn');
        button.title = 'Click to delete image';

        showMessage('success', 'Image deletion canceled.');
    }

    // Update hidden field
    document.getElementById('imagesToDelete').value = imagesToDelete.join(',');
}

// Helper function to show messages
function showMessage(type, message) {
    const alertClass = type === 'success' ? 'alert-success' :
        type === 'info' ? 'alert-info' : 'alert-danger';
    const icon = type === 'success' ? 'fas fa-check-circle' :
        type === 'info' ? 'fas fa-info-circle' : 'fas fa-exclamation-circle';

    const alert = document.createElement('div');
    alert.className = `alert ${alertClass} alert-dismissible fade show`;
    alert.innerHTML = `
                <i class="${icon}"></i> ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            `;

    const container = document.querySelector('.form-container');
    container.insertBefore(alert, container.firstChild);

    // Auto dismiss after 5 seconds
    setTimeout(() => {
        if (alert.parentNode) {
            alert.remove();
        }
    }, 5000);
}

// Form validation
document.getElementById('editForm').addEventListener('submit', function (e) {
    const title = document.querySelector('input[name="Title"]').value.trim();
    const price = document.querySelector('input[name="Price"]').value;
    const location = document.querySelector('input[name="Location"]').value.trim();

    if (!title || !price || !location) {
        e.preventDefault();
        alert('Please fill in all required fields.');
        return false;
    }

    if (parseFloat(price) <= 0) {
        e.preventDefault();
        alert('Price must be greater than 0.');
        return false;
    }

    // Show confirmation if images are marked for deletion
    if (imagesToDelete.length > 0) {
        const confirmed = confirm(`Are you sure you want to delete ${imagesToDelete.length} image(s)? This action cannot be undone.`);
        if (!confirmed) {
            e.preventDefault();
            return false;
        }
    }
});
