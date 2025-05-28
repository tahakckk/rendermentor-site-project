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
            "url": "/Admin/Home/GetAll"
        },
        "columns": [
            {
                "data": "listOrder",
                "className" : 'reorder',
                "width": "5%"
            },
            {
                "data": "image",
                "render": function (data, type, row) {
                    return `
                        <a href="#pickSlideBgs" data-toggle="modal" class="pickImage"><img src="${row.slideBg}" height="80" /></a>
                        `;
                }, "width": "20%"
            },
            {
                "data": "title",
                "render": function (data, type, row) {
                    return `
                        <a href="#editSlide" class="edit" data-toggle="modal">${row.title}</a>
                        `;
                }, "width": "70%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="#editSlide" data-toggle="modal" class="edit btn btn-info"><i class="fa fa-edit"></i> Düzenle</a>
                            <a onclick=DeleteSlide("/Admin/Home/DeleteSlide/${data}") class="btn btn-danger text-white"><i class="fa fa-trash"></i> Sil</a>
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
            url: '/Admin/Home/SlideListOrder',
            data: { Id: rowData.listOrder, fromPosition: data[i].oldData, toPosition: data[i].newData },
            dataType: "json"
        });
    }
});

function DeleteSlide(url) {
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