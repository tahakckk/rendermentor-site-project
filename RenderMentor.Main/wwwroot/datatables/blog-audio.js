var blogId = $("#blogId").val();
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
            "url": "/Admin/Blog/GetBlogAudios/" + blogId
        },
        "columns": [
            {
                "data": "listOrder",
                "className": 'reorder',
                "width": "5%"
            },
            {
                "data": "title",
                "render": function (data) {
                    return `
                        <a href="#editPodcast" data-toggle="modal" class="edit">${data}</a>
                        `;
                }, "width": "33%"
            },
            {
                "data": "audio",
                "render": function (data, type, row) {
                    return `
                        <audio controls 2 class="iru-tiny-player" 3 data-title="${row.title}">
                            4
                            <source src="${row.audio}" type="audio/mpeg">
                            5
                        </audio>
                        `;
                }, "width": "50%"
            },
            {
                "data": "id",
                "render": function (data, type, row) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="#editPodcast" data-toggle="modal" class="edit btn btn-info"><i class="fa fa-edit"></i> Düzenle</a>
                            <a onclick=Delete("/Admin/Blog/DeleteBlogAudio/${row.blogId}/${row.id}") class="btn btn-danger text-white"><i class="fa fa-trash"></i> Sil</a>
                            <input type='hidden' class='editId' value='${row.id}' />
                        </div>
                        `;
                }, "width": "12%"
            },
        ],
        "columnDefs": [
            {
                "orderable": false,
                "targets": [1, 2]
            }
        ]
    });
}

$('#tblData').on('row-reorder.dt', function (dragEvent, data, nodes) {
    for (var i = 0, ien = data.length; i < ien; i++) {
        var rowData = dataTable.row(data[i].node).data();
        $.ajax({
            type: "GET",
            cache: false,
            contentType: "application/json; charset=utf-8",
            url: '/Admin/Blog/BlogAudioListOrder/' + blogId,
            data: { AudioId: rowData.listOrder, fromPosition: data[i].oldData, toPosition: data[i].newData },
            dataType: "json"
        });
    }
});

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