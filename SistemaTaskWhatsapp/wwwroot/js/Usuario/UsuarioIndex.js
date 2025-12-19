var dataTable;

$(document).ready(function () {
    cargarDatatable();
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
            { data: "rol" }

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
