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
            "url": "/Admin/TrialMember/GetAll"
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
                }, "width": "25%"
            },
            { "data": "studentName", "width": "25%" },
            { "data": "studentEmail", "width": "25%" },
            { "data": "trialStatus", "width": "25%" },
        ],
        "columnDefs" : [
            {
                "orderable": false,
                "targets" : []
            }
        ],
    });
}