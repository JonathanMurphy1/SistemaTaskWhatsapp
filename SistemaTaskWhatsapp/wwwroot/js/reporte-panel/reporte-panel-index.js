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

            if (data.estado === 1) {
                modalDetalle.querySelector('#aceptado').checked = true;
            } else if (data.estado === 2) {
                modalDetalle.querySelector('rechazado').checked = true;
            }
        })
        .catch(() => {
            return;
        });
});