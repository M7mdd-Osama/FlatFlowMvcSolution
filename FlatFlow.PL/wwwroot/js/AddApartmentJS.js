let selectedFiles = [];
const maxFileSize = 100 * 1024 * 1024; // 100MB
const allowedImageTypes = ['image/jpeg', 'image/jpg', 'image/png'];
const allowedVideoTypes = ['video/mp4', 'video/mov', 'video/avi'];
const allowedTypes = [...allowedImageTypes, ...allowedVideoTypes];

function openFileDialog() {
    document.getElementById('mediaFiles').click();
}

document.getElementById('mediaFiles').addEventListener('change', function (e) {
    const newFiles = Array.from(e.target.files);
    addFiles(newFiles);
});

function addFiles(newFiles) {
    let validFiles = [];
    let errors = [];

    newFiles.forEach(file => {
        if (!allowedTypes.includes(file.type)) {
            errors.push(`${file.name}: File type not supported`);
            return;
        }

        if (file.size > maxFileSize) {
            errors.push(`${file.name}: File size exceeds 100MB`);
            return;
        }

        const exists = selectedFiles.some(existingFile =>
            existingFile.name === file.name && existingFile.size === file.size
        );

        if (!exists) {
            validFiles.push(file);
        }
    });

    selectedFiles = [...selectedFiles, ...validFiles];

    if (errors.length > 0) {
        alert('Errors found:\n' + errors.join('\n'));
    }

    updateFileCounter();
    previewMedia();
    updateFileInput();
}

function previewMedia() {
    const previewContainer = document.getElementById('mediaPreview');
    const clearBtn = document.getElementById('clearBtn');

    previewContainer.innerHTML = '';

    if (selectedFiles.length > 0) {
        clearBtn.style.display = 'block';

        selectedFiles.forEach((file, index) => {
            const previewItem = document.createElement('div');
            previewItem.className = 'preview-item';

            if (allowedImageTypes.includes(file.type)) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    const img = document.createElement('img');
                    img.src = e.target.result;
                    img.className = 'preview-image';
                    img.alt = file.name;
                    previewItem.appendChild(img);
                };
                reader.readAsDataURL(file);

            } else if (allowedVideoTypes.includes(file.type)) {
                const video = document.createElement('video');
                video.className = 'preview-video';
                video.muted = true;

                const videoOverlay = document.createElement('div');
                videoOverlay.className = 'video-overlay';
                videoOverlay.innerHTML = '<i class="fas fa-play"></i>';

                const reader = new FileReader();
                reader.onload = function (e) {
                    video.src = e.target.result;
                };
                reader.readAsDataURL(file);

                previewItem.appendChild(video);
                previewItem.appendChild(videoOverlay);
            }

            const removeBtn = document.createElement('button');
            removeBtn.type = 'button';
            removeBtn.className = 'remove-media';
            removeBtn.innerHTML = '×';
            removeBtn.onclick = () => removeFile(index);

            previewItem.appendChild(removeBtn);
            previewContainer.appendChild(previewItem);
        });
    } else {
        clearBtn.style.display = 'none';
    }
}

function removeFile(index) {
    selectedFiles.splice(index, 1);
    updateFileCounter();
    updateFileInput();
    previewMedia();
}

function clearAllMedia() {
    selectedFiles = [];
    document.getElementById('mediaFiles').value = '';
    updateFileCounter();
    previewMedia();
}

function updateFileInput() {
    const dataTransfer = new DataTransfer();
    selectedFiles.forEach(file => dataTransfer.items.add(file));
    document.getElementById('mediaFiles').files = dataTransfer.files;
}

function updateFileCounter() {
    const counter = document.getElementById('fileCounter');
    const countText = document.getElementById('fileCountText');

    if (selectedFiles.length > 0) {
        counter.style.display = 'block';
        const imageCount = selectedFiles.filter(f => allowedImageTypes.includes(f.type)).length;
        const videoCount = selectedFiles.filter(f => allowedVideoTypes.includes(f.type)).length;
        countText.textContent = `${selectedFiles.length} files selected (${imageCount} images, ${videoCount} videos)`;
    } else {
        counter.style.display = 'none';
    }
}

// Drag and drop functionality
const uploadArea = document.getElementById('uploadArea');

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

    const files = Array.from(e.dataTransfer.files);
    addFiles(files);
});

['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
    uploadArea.addEventListener(eventName, preventDefaults, false);
    document.body.addEventListener(eventName, preventDefaults, false);
});

function preventDefaults(e) {
    e.preventDefault();
    e.stopPropagation();
}

// Form submission
document.getElementById('apartmentForm').addEventListener('submit', function (e) {
    if (selectedFiles.length > 0) {
        document.getElementById('loadingOverlay').style.display = 'flex';
        document.getElementById('progressBar').style.display = 'block';

        let progress = 0;
        const progressInterval = setInterval(() => {
            progress += Math.random() * 15;
            if (progress >= 90) {
                progress = 90;
                clearInterval(progressInterval);
            }
            document.getElementById('progressFill').style.width = progress + '%';
        }, 200);
    }

    document.getElementById('submitBtn').disabled = true;
});

window.addEventListener('load', function () {
    document.getElementById('loadingOverlay').style.display = 'none';
});
