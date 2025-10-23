
var dataTable;


$(document).ready(function () {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "Order/GetAll"
        },
        "columns": [
            { "data": "OrderHeaderId", "width": "5%" },
            { "data": "Email", "width": "25%" },
            { "data": "Name", "width": "20%" },
            { "data": "Phone", "width": "10%" },
            { "data": "Status", "width": "10%" },
            { "data": "OrderTotal", "width": "10%" }
        ]
    });
});

