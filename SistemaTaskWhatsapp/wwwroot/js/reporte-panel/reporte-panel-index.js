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

modal.addEventListener("show.bs.modal", function (event) {

    const button = event.relatedTarget;
    const reporteId = button.getAttribute("data-reporte-id");

    previewContainer.innerHTML = `
        <p class="text-muted text-center">Cargando evidencias...</p>
    `;

    fetch(`/ReportePanel/EvidenciasReporte?reporteId=${reporteId}`)
        .then(respose => {
            if (!respose.ok) throw new Error();
            return respose.json();
        })
        .then(data =>{

            previewContainer.innerHTML = "";

            if (data.length === 0) {
                previewContainer.innerHTML = `
                    <p class="text-muted text-center">No hay evidencias</p>
                `;
                return;
            }

            data.forEach(evidencia => {

                const evidenciaCol = document.createElement("div");
                evidenciaCol.className = "col-md-6 mb-4";

                const card = document.createElement("div");
                card.className = "border rounded shadow-sm h-100 p-3";

                const innerRow = document.createElement("div");
                innerRow.className = "row align-items-stretch h-100";

                // Columna descripción
                const colDescripcion = document.createElement("div");
                colDescripcion.className = "col-6 ps-4 d-flex flex-column justify-content-center";

                colDescripcion.innerHTML = `
                    <h6 class="fw-bold mb-2">Descripción</h6>
                    <p class="text-muted small mb-0">
                        ${evidencia.descripcion ?? "Sin descripción"}
                    </p>
                `;

                // Columna archivo
                const colArchivo = document.createElement("div");
                colArchivo.className = "col-6 d-flex flex-column justify-content-center align-items-center text-center";

                const extension = evidencia.url.split('.').pop().toLowerCase();
                let contenidoArchivo = "";

                if (["jpg", "jpeg", "png"].includes(extension)) {
                    contenidoArchivo = `
                        <img src="${evidencia.url}"
                             class="img-fluid rounded shadow mb-2"
                             style="height:250px">
                    `;
                }
                else if (extension === "pdf") {
                    contenidoArchivo = `
                        <iframe src="${evidencia.url}"
                                class="w-100 rounded mb-2"
                                style="height:350px;"></iframe>
                    `;
                }
                else {
                    contenidoArchivo = `
                        <i class="fa-solid fa-file fa-3x text-secondary mb-2"></i>
                    `;
                }

                colArchivo.innerHTML = `
                    ${contenidoArchivo}
                    <div>
                        <a href="${evidencia.url}"
                           class="btn btn-success btn-sm" download target="_blank">
                            <i class="fa-solid fa-download"></i> Descargar
                        </a>
                    </div>
                `;

                innerRow.appendChild(colDescripcion);
                innerRow.appendChild(colArchivo);

                card.appendChild(innerRow);
                evidenciaCol.appendChild(card);
                previewContainer.appendChild(evidenciaCol);
            });
        })
        .catch(() => {
            previewContainer.innerHTML = `
                <p class="text-danger text-center">
                    Error al cargar evidencias
                </p>
            `;
        });
});
