var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("GetOrderList?status=inprocess");
    } else if (url.includes("pending")) {
        loadDataTable("GetOrderList?status=pending");
    } else if (url.includes("completed")) {
        loadDataTable("GetOrderList?status=completed");
    } else if (url.includes("rejected")) {
        loadDataTable("GetOrderList?status=rejected");
    } else {
        loadDataTable("GetOrderList?status=all");
    }
});

function loadDataTable(url) {
    dataTable = $('#tblData').DataTable({
        "order": [[0, "desc"]],
        "autoWidth": false,
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.20/i18n/Turkish.json",
        },
        "ajax": {
            "url": "/Student/Student/" + url
        },
        "columns": [
            {
                "data": "orderDate",
                "render": function (data, type, row) {
                    var date = data.substring(0, 10);
                    var time = data.substring(11, 16);
                    return `
                        ${date} ${time}
                        `;
                }, "width": "23%"
            },
            {
                "data": "orderNumber",
                "render": function (data, type, row) {
                    return `
                        <a href="/Student/Student/OrderDetails/${row.id}">${row.orderNumber.substring(0, 10)}...</a>
                        `;
                }, "width": "20%"
            },
            { "data": "orderTotal", "width": "12%" },
            { "data": "orderStatus", "width": "25%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="/Student/Student/OrderDetails/${data}" class="btn btn-info"><i class="fa fa-search"></i> İncele</a>
                        </div>
                        `;
                }, "width": "20%"
            },
        ],
        "columnDefs" : [
            {
                "orderable": false,
                "targets" : [1, 2]
            }
        ],
    });
}