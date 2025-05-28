var courseId = $("#courseId").val();
var referenceTable;

$(document).ready(function () {
    loadReferenceTable();
});

function loadReferenceTable() {
    referenceTable = $('#references').DataTable({
        rowReorder: {
            dataSrc: 'listOrder'
        },
        paging: false,
        "searching": false,
        "order": [[0, "asc"]],
        "autoWidth": false,
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.20/i18n/Turkish.json",
        },
        "ajax": {
            "url": "/Admin/Home/GetReferences/"
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
                        <a href="${row.logo}" data-fancybox="courseGallery"><img src="${row.logo}" height="60" /></a>
                        `;
                }, "width": "83%"
            },
            {
                "data": "id",
                "render": function (data, type, row) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a onclick=Delete("/Admin/Home/DeleteReference/${row.id}") class="btn btn-danger text-white"><i class="fa fa-trash"></i> Sil</a>
                        </div>
                        `;
                }, "width": "12%"
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

$('#references').on('row-reorder.dt', function (dragEvent, data, nodes) {
    for (var i = 0, ien = data.length; i < ien; i++) {
        var rowData = referenceTable.row(data[i].node).data();
        $.ajax({
            type: "GET",
            cache: false,
            contentType: "application/json; charset=utf-8",
            url: '/Admin/Home/ReferenceListOrder/',
            data: { Id: rowData.listOrder, fromPosition: data[i].oldData, toPosition: data[i].newData },
            dataType: "json"
        });
    }
});

function Delete(url) {
    swal({
        title: "Silmek istediğinizden emin misiniz?",
        text: "Bu işlemle birlikte resmin dosyası da sistemden silinecektir.",
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
                        referenceTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}