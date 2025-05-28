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
            "url": "/Student/Student/GetQuestions"
        },
        "columns": [
            {
                "data": "dateTime",
                "render": function (data, type) {
                    var parsedDate = new Date(Date.parse(data));
                    var lastDate = parsedDate.toLocaleDateString();
                    var time = parsedDate.toLocaleTimeString();
                    time = time.substring(0, 5);
                    if (type === "display") {
                        return `
                            <small>${lastDate} ${time}</small>
                        `;
                    }
                    return data;
                }, "width": "10%"
            },
            {
                "data": "courseName",
                "render": function (data, type, row) {
                    return `
                        <p>${data} <small style="margin-left: 15px">${row.courseSectionName}</small></p>
                        `;
                }, "width": "40%"
            },
            { "data": "lectureName", "width": "25%" },
            {
                "data": "answered",
                "render": function (data, type, row) {
                    if (data == true) {
                        // blog is currently published
                        return `
                            <div class="text-center">
                                <span class="badge badge-success"><i class="fa fa-check"></i> Cevaplandı</span>
                            </div>
                           `;
                    } else {
                        return `
                            <div class="text-center">
                                <span class="badge badge-danger"><i class="fa fa-times"></i> Cevaplanmadı</span>
                            </div>
                            `;

                    }
                }, "width": "8%"
            },
            {
                "data": "id",
                "render": function (data, type, row) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="/Student/Student/Answer/${data}" class="btn btn-info btn-sm"><i class="fa fa-edit"></i> Cevaplar</a>
                            <a href="/Student/Student/Presentation/${row.courseId}" target="_blank" class="btn btn-secondary btn-sm"><i class="fa fa-search"></i> İncele</a>
                        </div>
                        `;
                }, "width": "12%"
            },
        ],
        "columnDefs": [
            {
                "orderable": false,
                "targets": [4]
            }
        ],
    });
}