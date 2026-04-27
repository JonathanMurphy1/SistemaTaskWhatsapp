//Mostrar datos y mandar id al modal eliminar
const modalEliminar = document.getElementById("modalEliminar");

modalEliminar.addEventListener('show.bs.modal', (e) => {

    const boton = e.relatedTarget;

    const id = boton.getAttribute('data-id');
    const nombre = boton.getAttribute('data-nombre');

    modalEliminar.querySelector('#mostrarNombre').innerText = nombre;
    modalEliminar.querySelector('#inputId').value = id;

});


// RELACIONES
const modalRelaciones = document.getElementById("modalRelaciones");

modalRelaciones.addEventListener('show.bs.modal', (e) => {

    const boton = e.relatedTarget;
    const empresaId = boton.getAttribute('data-id');
    const nombre = boton.getAttribute('data-nombre');

    document.getElementById("nombreEmpresaRelaciones").innerText = nombre;

    const lista = document.getElementById("listaRelaciones");
    lista.innerHTML = "";

    const relaciones = window.relacionesEmpresa[empresaId];

    if (!relaciones || relaciones.length === 0) {
        lista.innerHTML = `<li class="list-group-item text-muted">Sin relaciones</li>`;
        return;
    }

    relaciones.forEach(r => {
        const li = document.createElement("li");
        li.className = "list-group-item";
        li.innerText = r;
        lista.appendChild(li);
    });

});