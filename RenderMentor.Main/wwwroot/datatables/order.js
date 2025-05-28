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
            "url": "/Admin/Order/" + url
        },
        "columns": [
            {
                "data": "orderDate",
                "render": function (data, type) {
                    var parsedDate = new Date(Date.parse(data));
                    var date = parsedDate.toLocaleDateString();
                    var time = parsedDate.toLocaleTimeString();
                    time = time.substring(0, 5);
                    if (type === "display") {
                        return `
                            ${date} ${time}
                        `;
                    }
                    return data;
                }, "width": "12%"
            },
            {
                "data": "orderNumber",
                "render": function (data, type, row) {
                    return `
                        <a href="/Admin/Order/Details/${row.id}">${row.orderNumber.substring(0,20)}...</a>
                        `;
                }, "width": "15%"
            },
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "10%" },
            { "data": "orderTotal", "width": "8%" },
            { "data": "orderStatus", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="/Admin/Order/Details/${data}" class="btn btn-info"><i class="fa fa-search"></i> İncele</a>
                        </div>
                        `;
                }, "width": "5%"
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