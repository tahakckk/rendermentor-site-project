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
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            {
                "data": "name",                
                "render": function (data, type, row) {
                    return `
                        <a href="/Admin/Company/Upsert/${row.id}">${row.name}</a>
                        `;
                }, "width": "66%"
            },
            { "data": "city", "width": "12%" },
            { "data": "phoneNumber", "width": "12%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="/Admin/Company/Upsert/${data}" class="btn btn-info"><i class="fa fa-edit"></i> Düzenle</a>
                            <a onclick=Delete("/Admin/Company/Delete/${data}") class="btn btn-danger text-white"><i class="fa fa-trash"></i> Sil</a>
                        </div>
                        `;
                }, "width": "10%"
            },
        ]
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