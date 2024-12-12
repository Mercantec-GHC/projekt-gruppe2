export function Load(userId) {
    let input = document.getElementById('input');
    let image = document.getElementById('image');
    let avatarModal = document.getElementById('avatarModal');
    let avatarButton = document.getElementById('avatarButton');
    let modal = new bootstrap.Modal(avatarModal);
    let cropper;

    input.addEventListener('change', function (e) {
        var files = e.target.files;
        var done = function (url) {
            input.value = '';
            image.src = url;
            modal.show(avatarModal);
        };
        var reader;
        var file;

        if (files && files.length > 0) {
            file = files[0];

            if (URL) {
                done(URL.createObjectURL(file));
            } else if (FileReader) {
                reader = new FileReader();
                reader.onload = function (e) {
                    done(reader.result);
                };
                reader.readAsDataURL(file);
            }
        }
    });

    avatarModal.addEventListener('show.bs.modal', function (event) {
        cropper = new Cropper(image, {
            aspectRatio: 1,
            viewMode: 3,
            movable: true,
            zoomable: true,
            autoCropArea: 1,
            cropBoxMovable: false,
            cropBoxResizable: false,
            rotatable: false,
            dragMode: "move",
            toggleDragModeOnDblclick: false,
            center: false,
            guides: false,
            highlight: false
        });
    });

    avatarModal.addEventListener('hidden.bs.modal', function (event) {
        cropper.destroy();
        cropper = null;
    });

    avatarButton.addEventListener('click', function () {
        let canvas = cropper.getCroppedCanvas({
            width: 512,
            height: 512,
        });

        canvas.toBlob(function (blob) {
            let formData = new FormData();
            let destinationPath = "avatars";

            formData.append('file', blob, `${userId}.png`);
            formData.append('destinationPath', destinationPath);

            fetch("/api/file/upload", {
                method: "POST",
                body: formData
            }).then(res => {
                if (res.ok) {
                    const avatarImages = document.querySelectorAll(".avatar-img");
                    let croppedImage = canvas.toDataURL("image/png");
                    avatarImages.forEach(function (img) {
                        //img.src = `images/avatars/${userId}.png`;
                        img.src = croppedImage;
                    });
                    modal.hide();
                }
            });
        });
    });
}