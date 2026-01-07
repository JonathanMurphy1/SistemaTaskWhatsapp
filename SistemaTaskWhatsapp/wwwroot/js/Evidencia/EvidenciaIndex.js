//Mostrar datos y mandar id al modal eliminar
const modalEliminar = document.getElementById("modalEliminar");

modalEliminar.addEventListener('show.bs.modal', (e) => {
    const boton = e.relatedTarget;
    const id = boton.getAttribute('data-id');

    modalEliminar.querySelector('#inputId').value = id;

});

document.addEventListener("DOMContentLoaded", function () {

    const modal = document.getElementById("modalPreviewEvidencia");
    const previewContainer = document.getElementById("previewContainer");
    const btnDescargar = document.getElementById("btnDescargarArchivo");

    modal.addEventListener("show.bs.modal", function (event) {

        const button = event.relatedTarget;
        const url = button.getAttribute("data-url");

        previewContainer.innerHTML = "";
        btnDescargar.href = url;

        const extension = url.split('.').pop().toLowerCase();

        if (["jpg", "jpeg", "png"].includes(extension)) {
            const img = document.createElement("img");
            img.src = url;
            img.className = "img-fluid rounded shadow";
            img.style.maxHeight = "500px";
            previewContainer.appendChild(img);
        }
        else if (extension === "pdf") {
            const iframe = document.createElement("iframe");
            iframe.src = url;
            iframe.style.width = "100%";
            iframe.style.height = "500px";
            iframe.className = "border rounded";
            previewContainer.appendChild(iframe);
        }
        else {
            previewContainer.innerHTML = `
                <div class="text-center">
                    <i class="fa-solid fa-file-lines fa-3x text-secondary mb-3"></i>
                    <p>No se puede previsualizar este archivo.</p>
                </div>
            `;
        }
    });

});

