var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "autoWidth": false,
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.20/i18n/Turkish.json",
        },
        "ajax": {
            "url": "/Admin/CourseCategory/GetAll"
        },
        "columns": [
            {
                "data": "name",
                "render": function (data, type, row) {
                    return `
                        <a href="/Admin/CourseCategory/Upsert/${row.id}">${row.name}</a>
                        `;
                }, "width": "80%"
            },
            {
                "data": "listOrder",
                "render": function (data) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="" class="btn btn-link"><i class="fa fa-chevron-up fa-lg"></i></a>
                            <a href="" class="btn btn-link"><i class="fa fa-chevron-down fa-lg"></i></a>
                        </div>
                        `;
                }, "width": "8%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="/Admin/CourseCategory/Upsert/${data}" class="btn btn-info"><i class="fa fa-edit"></i> Düzenle</a>
                            <a onclick=Delete("/Admin/CourseCategory/Delete/${data}") class="btn btn-danger text-white"><i class="fa fa-trash"></i> Sil</a>
                            <a href="/" class="btn btn-secondary"><i class="fa fa-search"></i> İncele</a>
                        </div>
                        `;
                }, "width": "12%"
            },
        ]
    });
}

function Delete(url) {
    swal({
        title: "Silmek istediğinizden emin misiniz?",
        text: "Silme işlemini geri alamazsınız!",
        icon: "warning",
        buttons: true,
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