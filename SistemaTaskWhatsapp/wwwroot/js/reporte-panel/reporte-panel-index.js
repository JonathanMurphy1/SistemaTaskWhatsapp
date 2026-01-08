//Modal Revisar
document.getElementById('modalRevisar')
    .addEventListener('show.bs.modal', function () {

        const form = document.getElementById('formRevisar');
        form.reset();
    });

//Mostrar datos y mandar id al modal eliminar
const modalRevisar = document.getElementById("modalRevisar");

modalRevisar.addEventListener('show.bs.modal', (e) => {
    const boton = e.relatedTarget;
    const reporteId = boton.getAttribute('data-reporteId');

    modalRevisar.querySelector('.reporte-id').value = reporteId;
});


//Modal Detalle
const modalDetalle = document.getElementById('modalDetalle');

//Mostrar datos en el modal detalle
modalDetalle.addEventListener('show.bs.modal', (e) => {
    const boton = e.relatedTarget;
    const reporteId = boton.getAttribute('data-reporteId');

    const form = modalDetalle.querySelector('#formEditarRevision');
    form.reset();

    modalDetalle.querySelector('.reporte-id').value = reporteId;

    fetch(`/ReportePanel/ObtenerRevision?reporteId=${reporteId}`)
        .then(respose => {
            if (!respose.ok) throw new Error();
            return respose.json();
        })
        .then(data => {
            modalDetalle.querySelector('.mostrar-fecha').innerText = data.fecha;
            modalDetalle.querySelector('.comentario-input').value = data.comentario;
            modalDetalle.querySelector('.mostrar-fecha').innerText = data.fecha;
            modalDetalle.querySelector('.nombre-supervisor').innerText = data.supervisor;
            modalDetalle.querySelector('.revision-id').value = data.revisionId;


            if (data.estado === 1) {
                modalDetalle.querySelector('#aceptado').checked = true;
            } else if (data.estado === 2) {
                modalDetalle.querySelector('#rechazado').checked = true;
            }
        })
        .catch(() => {
            return;
        });
});

//Ver evidencia

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