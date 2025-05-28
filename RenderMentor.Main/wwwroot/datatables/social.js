var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        rowReorder: {
            dataSrc: 'listOrder'
        },
        "order": [[0, "asc"]],
        "autoWidth": false,
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.20/i18n/Turkish.json",
        },
        "ajax": {
            "url": "/Admin/Contact/GetSocials"
        },
        "columns": [
            {
                "data": "listOrder",
                "className" : 'reorder',
                "width": "5%"
            },            
            {
                "data": "name",
                "render": function (data) {
                    switch (data) {
                        case 1:
                            return `
                                <a href="#editSocial" class="edit" data-toggle="modal">Facebook</a>
                            `;
                        case 2:
                            return `
                                <a href="#editSocial" class="edit" data-toggle="modal">Instagram</a>
                            `;
                        case 3:
                            return `
                                <a href="#editSocial" class="edit" data-toggle="modal">Twitter</a>
                            `;
                        case 4:
                            return `
                                <a href="#editSocial" class="edit" data-toggle="modal">Linkedin</a>
                            `;
                        case 5:
                            return `
                                <a href="#editSocial" class="edit" data-toggle="modal">Youtube</a>
                            `;
                        case 6:
                            return `
                                <a href="#editSocial" class="edit" data-toggle="modal">Vimeo</a>
                            `;
                        default:
                            return `
                                <a href="#editSocial" class="edit" data-toggle="modal">Facebook</a>
                            `;
                    }
                }, "width": "15%"
            },
            { "data": "link", "width": "70%"},
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="#editSocial" data-toggle="modal" class="edit btn btn-info"><i class="fa fa-edit"></i> Düzenle</a>
                            <a onclick=DeleteSocial("/Admin/Contact/DeleteSocial/${data}") class="btn btn-danger text-white"><i class="fa fa-trash"></i> Sil</a>
                            <input type='hidden' class='editId' value='${data}' />
                        </div>
                        `;
                }, "width": "10%"
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

$('#tblData').on('row-reorder.dt', function (dragEvent, data, nodes) {
    for (var i = 0, ien = data.length; i < ien; i++) {
        var rowData = dataTable.row(data[i].node).data();
        $.ajax({
            type: "GET",
            cache: false,
            contentType: "application/json; charset=utf-8",
            url: '/Admin/Contact/SocialListOrder',
            data: { Id: rowData.listOrder, fromPosition: data[i].oldData, toPosition: data[i].newData },
            dataType: "json"
        });
    }
});

function DeleteSocial(url) {
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