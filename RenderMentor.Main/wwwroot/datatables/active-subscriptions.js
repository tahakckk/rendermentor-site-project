var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "order": [[0, "desc"]],
        "autoWidth": false,
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.20/i18n/Turkish.json",
        },
        "ajax": {
            "url": "/Admin/Subscription/GetAll"
        },
        "columns": [
            {
                "data": "startDate",
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
                "data": "endDate",
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
            { "data": "packageName", "width": "14%" },
            { "data": "daysToExpire", "width": "4%" },
            { "data": "studentName", "width": "15%" },
            { "data": "studentEmail", "width": "18%" },
            {
                "data": "userId",
                "render": function (data) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="/Admin/User/Upsert/${data}" class="btn btn-info"><i class="fa fa-edit"></i> Öğrenci Sayfası</a>
                            <a href="/" class="btn btn-warning"><i class="fa fa-search"></i> Panele Git</a>
                        </div>
                    `;
                },
                "width": "25%"
            },
        ],
        "columnDefs" : [
            {
                "orderable": false,
                "targets" : [5]
            }
        ],
    });
}