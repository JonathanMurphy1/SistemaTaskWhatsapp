//Mostrar datos y mandar id al modal eliminar
const modalEliminar = document.getElementById("modalEliminar");

modalEliminar.addEventListener('show.bs.modal', (e) => {
    const boton = e.relatedTarget;
    const id = boton.getAttribute('data-id');
    const nombre = boton.getAttribute('data-nombre');

    modalEliminar.querySelector('.mostrarNombre').innerText = nombre;
    modalEliminar.querySelector('.inputId').value = id;
});
