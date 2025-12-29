//Mostrar datos y mandar id al modal eliminar
const modalEliminar = document.getElementById("modalEliminar");

modalEliminar.addEventListener('show.bs.modal', (e) => {
    const boton = e.relatedTarget;
    const id = boton.getAttribute('data-id');
    const nombre = boton.getAttribute('data-nombre');

    modalEliminar.querySelector('.mostrarNombre').innerText = nombre;
    modalEliminar.querySelector('.inputId').value = id;
});

//Mostrar datos y mandar id al modal eliminar
const modalFinalizar = document.getElementById("modalFinalizar");

modalFinalizar.addEventListener('show.bs.modal', (e) => {
    const boton = e.relatedTarget;
    const id = boton.getAttribute('data-id');
    const nombre = boton.getAttribute('data-nombre');

    modalFinalizar.querySelector('.mostrarNombre').innerText = nombre;
    modalFinalizar.querySelector('.inputId').value = id;
});