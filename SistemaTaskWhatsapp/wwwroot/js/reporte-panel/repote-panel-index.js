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