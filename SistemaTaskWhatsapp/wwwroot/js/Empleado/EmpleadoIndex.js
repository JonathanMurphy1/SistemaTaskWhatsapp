//Mostrar datos y mandar id al modal eliminar
const modalEliminar = document.getElementById("modalEliminar");

modalEliminar.addEventListener('show.bs.modal', (e) => {
    const boton = e.relatedTarget;
    const id = boton.getAttribute('data-id');
    const nombre = boton.getAttribute('data-nombre');

    modalEliminar.querySelector('#mostrarNombre').innerText = nombre;
    modalEliminar.querySelector('#inputId').value = id;

});

document.addEventListener("DOMContentLoaded", function () {

    const botones = document.querySelectorAll(".btnCambiarEstado");

    botones.forEach(boton => {
        boton.addEventListener("click", function () {

            const id = this.dataset.id;
            const nombre = this.dataset.nombre;
            const estado = this.dataset.estado;

            document.getElementById("estadoEmpleadoId").value = id;
            document.getElementById("nombreEmpleado").innerText = nombre;
            document.getElementById("estadoSelect").value = estado;
        });
    });
});
