var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "order": [[0, "asc"]],
        "autoWidth": false,
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.20/i18n/Turkish.json",
        },
        "ajax": {
            "url": "/Admin/Draw/GetAll"
        },
        "columns": [
            {
                "data": "submitDate",
                "render": function (data, type, row) {
                    var parsedDate = new Date(Date.parse(data));
                    var date = parsedDate.toLocaleDateString();
                    var time = parsedDate.toLocaleTimeString();
                    time = time.substring(0, 5);
                    return `
                        ${date} ${time}
                        `;
                }, "width": "15%"
            },
            { "data": "email", "width": "38%" },
            {
                "data": "isWinner",
                "render": function (data, type, row) {
                    if (data == true) {
                        return `
                        <div class="text-center">
                            <span class="badge badge-success"><i class="fa fa-trophy"></i> Kazandı</span>
                        </div>
                        `;
                    } else {
                        return `
                        <div class="text-center"></div>
                        `;
                    }
                },
                "width": "10%"
            },
            {
                "data": "isStudent",
                "render": function (data, type, row) {
                    if (data == true) {
                        return `
                        <div class="text-center">
                            <span class="badge badge-success"><i class="fa fa-check"></i> Öğrenci olarak kayıtlı</span>
                        </div>
                        `;
                    } else {
                        return `
                        <div class="text-center">
                            <span class="badge badge-danger"><i class="fa fa-times"></i> Öğrenci kaydı yok</span>
                        </div>
                        `;
                    }
                },
                "width": "13%"
            },
            {
                "data": "studentId",
                "render": function (data, type, row) {
                    if (row.isStudent == true) {
                        return `
                        <div class="text-center">
                            <div class="btn-group btn-group-sm" role="group">
                                <a href="/Admin/User/Upsert/${row.studentId}" class="btn btn-info mt-1"><i class="fa fa-user"></i> Öğrenci Profili</a>
                            </div>
                        </div>
                        `;
                    } else {
                        return `
                        <div class="text-center">
                            <div class="btn-group btn-group-sm" role="group">
                                <a href="javascript:void(0);" class="btn btn-sm btn-secondary mt-1 disabled"><i class="fa fa-times"></i> Öğrenci Profili</a>
                            </div>
                        </div>
                        `;
                    }
                },
                "width": "12%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a onclick=Delete("/Admin/Draw/Delete/${data}") class="btn btn-danger text-white"><i class="fa fa-trash"></i> Sil</a>
                        </div>
                        `;
                }, "width": "12%"
            },
        ],
        "columnDefs" : [
            {
                "orderable": false,
                "targets" : [4, 5]
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