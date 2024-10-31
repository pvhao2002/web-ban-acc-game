const fileUpload = document.getElementById('file-upload');
const uploadPreviewTemplate = document.getElementById('uploadPreviewTemplate');

// listen for file selection
fileUpload.addEventListener('change', function() {
    const files = this.files;
    if (files.length === 0) {
        return;
    }
    uploadPreviewTemplate.innerHTML = '';
    const file = files[0];
    const reader = new FileReader();

    // listen for file load
    reader.addEventListener('load', function() {
        const html = `
                <div class="card mt-1 mb-0 shadow-none border">
                    <div class="p-2">
                        <div class="row align-items-center">
                            <div class="col-auto">
                                <img src="${this.result}" class="avatar-sm rounded bg-light" alt="B Shop">
                            </div>
                            <div class="col ps-0">
                                <a href="javascript:void(0);" class="text-muted fw-bold">${file.name}</a>
                                <p class="mb-0" >${(file.size / 1024).toFixed(2)} KB</p>
                            </div>
                            <div class="col-auto">
                                <!-- Button -->
                                <a href="javascript:void(0);" class="btn btn-link btn-lg text-muted img-remove" onclick="remove(this)">
                                    <i class="dripicons-cross"></i>
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
                `;
        const uploadPreviewTemplate = document.getElementById('uploadPreviewTemplate');
        uploadPreviewTemplate.insertAdjacentHTML('beforeend', html);
    });
    // read file as data url
    reader.readAsDataURL(file);
});

// Add event listener for remove button
function remove(element) {
    // Get the file name to be removed
    const fileName = element.closest('.card').querySelector('a').innerText;
    // Get the files from file input
    const files = fileUpload.files;
    // Create a new DataTransfer object to hold the new list of files
    const dataTransfer = new DataTransfer();
    // Loop through the files and add the ones that are not the one being removed
    for (let i = 0; i < files.length; i++) {
        if (files[i].name !== fileName) {
            dataTransfer.items.add(files[i]);
        }
    }
    // Update the input's file list with the new files list
    fileUpload.files = dataTransfer.files;
    // Remove the preview card
    element.closest('.card').remove();
}