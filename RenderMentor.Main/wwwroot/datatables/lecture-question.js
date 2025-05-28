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
            "url": "/Admin/Question/GetAll"
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
                            ${lastDate} ${time}
                        `;
                    }
                    return data;
                }, "width": "10%"
            },
            { "data": "courseName", "width": "16%" },
            { "data": "courseSectionName", "width": "18%" },
            { "data": "lectureName", "width": "16%" },
            {
                "data": "studentName",
                "render": function (data, type, row) {
                    return `
                        <p>${data} <small style="margin-left: 15px">${row.studentEmail}</small></p>
                        `;
                }, "width": "15%"
            },
            { "data": "answerCount", "width": "3%" },
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
                }, "width": "10%"
            },
            {
                "data": "id",
                "render": function (data, type, row) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="/Admin/Question/Answer/${data}" class="btn btn-info"><i class="fa fa-edit"></i> Cevapla</a>
                            <a onclick=Delete("/Admin/Question/Delete/${data}") class="btn btn-danger text-white"><i class="fa fa-trash"></i> Sil</a>
                            <a href="/Student/Student/Presentation/${row.courseId}" target="_blank" class="btn btn-secondary"><i class="fa fa-search"></i> İncele</a>
                        </div>
                        `;
                }, "width": "12%"
            },
        ],
        "columnDefs": [
            {
                "orderable": false,
                "targets": [7]
            }
        ],
    });
}

function Delete(url) {
    swal({
        title: "Silmek istediğinizden emin misiniz?",
        text: "Silme işlemini geri alamazsınız!",
        icon: "warning",
        buttons: ["İptal", "Tamam"],
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}