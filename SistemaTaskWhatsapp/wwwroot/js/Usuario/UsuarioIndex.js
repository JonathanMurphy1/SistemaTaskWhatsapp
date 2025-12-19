var dataTable;

$(document).ready(function () {
    cargarDatatable();
    configurarModalEliminar();
});

function cargarDatatable() {
    dataTable = $("#tblUsuario").DataTable({
        ajax: {
            url: "/Usuario/GetAll",
            type: "GET",
            datatype: "json"
        },
        columns: [
            { data: "id" },
            { data: "nombre" },
            { data: "email" },
            { data: "telefono" },
            { data: "rol" },
            {
                data: "id",
                width: "30%",
                render: function (data, type, row) {
                    return `
                        <div class="d-flex justify-content-center gap-2">
                            <a title="Editar"
                               href="/Usuario/Edit/${data}"
                               class="btn btn-warning rounded-circle align-content-center" style="width: 50px; height: 50px;">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <button type="button" title="Borrar" data-id="${data}" data-nombre="${row.nombre}" data-bs-toggle="modal"
                                    data-bs-target="#modalEliminar" class="btn btn-danger rounded-circle" style="width: 50px; height: 50px;">
                                <i class="bi bi-trash-fill"></i>
                            </button>
                        </div>
                    `;
                }
            }
        ],
        language: {
            decimal: "",
            emptyTable: "No hay registros",
            info: "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
            infoEmpty: "Mostrando 0 a 0 de 0 Entradas",
            infoFiltered: "(Filtrado de _MAX_ total entradas)",
            thousands: ",",
            lengthMenu: "Mostrar _MENU_ Entradas",
            loadingRecords: "Procesando...",
            search: "Buscar:",
            zeroRecords: "Sin resultados encontrados",
            paginate: {
                first: "Primero",
                last: "Ultimo",
                next: "Siguiente",
                previous: "Anterior"
            }
        }
    });
}

// Función para mostrar alertas Bootstrap
function showAlert(message, isError) {
    const existingAlerts = document.querySelectorAll('.alert');
    existingAlerts.forEach(alert => alert.remove());

    const alertDiv = document.createElement('div');
    alertDiv.className = `position-fixed top-0 end-0 mt-3 me-3 alert alert-${isError ? 'danger' : 'success'} alert-dismissible fade show`;
    alertDiv.setAttribute('role', 'alert');
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

    document.body.appendChild(alertDiv);

    setTimeout(() => {
        alertDiv.remove();
    }, 4000);
}

//Mandar info al modal eliminar
function configurarModalEliminar() {
    var modalEliminar = document.getElementById('modalEliminar');

    if (!modalEliminar) return;

    modalEliminar.addEventListener('show.bs.modal', function (event) {
        var boton = event.relatedTarget;

        var id = boton.getAttribute('data-id');
        var nombre = boton.getAttribute('data-nombre');

        document.getElementById('mostrarId').innerText = "Id: " + id;
        document.getElementById('mostrarNombre').innerText = "Nombre: " + nombre;
        document.getElementById('idInput').value = id;
    });
}
