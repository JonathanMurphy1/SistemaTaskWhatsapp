//Mostrar datos y mandar id al modal eliminar
const modalEliminar = document.getElementById("modalEliminar");

modalEliminar.addEventListener('show.bs.modal', (e) => {
    const boton = e.relatedTarget;

    const id = boton.getAttribute('data-id');
    const nombre = boton.getAttribute('data-nombre');

    modalEliminar.querySelector('#mostrarNombre').innerText = nombre;
    modalEliminar.querySelector('#inputId').value = id;

});

const modalProgramas = document.getElementById("modalProgramas");

let empresaActualId = 0;

if (modalProgramas) {
    modalProgramas.addEventListener('show.bs.modal', (e) => {

        const boton = e.relatedTarget;

        empresaActualId = boton.getAttribute('data-id');
        const nombre = boton.getAttribute('data-nombre');

        modalProgramas.querySelector('#nombreEmpresa').innerText = nombre;

        cargarProgramasRelacionados();
        cargarProgramasSelect();
    });
}

//Cargar relaciones
function cargarProgramasRelacionados() {

    const contenedor = document.getElementById("contenedorProgramas");
    contenedor.innerHTML = "<p class='text-muted'>Cargando...</p>";

    fetch(`/Empresas/GetProgramasByEmpresa?id=${empresaActualId}`)
        .then(res => res.json())
        .then(data => {

            if (!data || data.length === 0) {
                contenedor.innerHTML = "<p class='text-muted'>No hay programas relacionados</p>";
                return;
            }

            let html = "<ul class='list-group'>";

            data.forEach(p => {

                let accion = "";

                if (p.puedeEliminar) {
                    accion = `
                        <button class="btn btn-sm btn-outline-danger btnEliminarRelacion" data-id="${p.id}">
                            <i class="bi bi-trash"></i>
                        </button>
                    `;
                }

                html += `
                    <li class="list-group-item d-flex justify-content-between align-items-center">
                        <span class="fw-medium">${p.nombre}</span>
                        ${accion}
                    </li>
                `;
            });

            html += "</ul>";
            contenedor.innerHTML = html;

        })
        .catch(() => {
            contenedor.innerHTML = "<p class='text-danger'>Error al cargar datos</p>";
        });
}


//Cargar programas
function cargarProgramasSelect() {

    fetch(`/Empresas/GetProgramas`)
        .then(res => res.json())
        .then(data => {

            const select = document.getElementById("selectProgramas");
            select.innerHTML = "";

            data.forEach(p => {
                select.innerHTML += `<option value="${p.id}">${p.nombre}</option>`;
            });
        });
}

//Crear relación
const btnAgregar = document.getElementById("btnAgregarPrograma");

if (btnAgregar) {
    btnAgregar.addEventListener("click", function () {

        const programaId = document.getElementById("selectProgramas").value;

        fetch(`/Empresas/AddRelacion?empresaId=${empresaActualId}&programaId=${programaId}`, {
            method: "POST"
        })
            .then(res => {
                if (!res.ok) throw new Error();
                cargarProgramasRelacionados();
            })
            .catch(() => alert("Error o ya existe relación"));
    });
}

//Eliminar relación
document.addEventListener("click", function (e) {

    const btn = e.target.closest(".btnEliminarRelacion");

    if (btn) {

        const id = btn.getAttribute("data-id");

        fetch(`/Empresas/DeleteRelacionAjax?id=${id}`, {
            method: "POST"
        })
            .then(res => {
                if (!res.ok) throw new Error();
                cargarProgramasRelacionados();
            })
            .catch(() => alert("Error al eliminar"));
    }

});