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
            "url": "/Admin/Course/GetAll"
        },
        "columns": [
            {
                "data": "listOrder",
                "className" : 'reorder',
                "width": "5%"
            },
            {
                "data": "title",
                "render": function (data, type, row) {
                    return `
                        <a href="/Admin/Course/Upsert/${row.id}">${row.title}</a>
                        `;
                }, "width": "18%"
            },
            { "data": "instructor.applicationUser.name", "width": "12%" },
            { "data": "courseCategory.name", "width": "12%" },
            {
                "data": "published",
                "render": function (data, type, row) {
                    if (data == true) {
                        // course is currently published
                        return `
                            <div class="text-center">
                                <span class="badge badge-success mr-2">Yayında</span>
                                <a onclick=PublishToggle('${row.id}') class="btn btn-danger btn-sm text-white" style="width: 140px;">
                                    <i class="fas fa-lock"></i> Yayından Kaldır
                                </a>
                            </div>
                           `;
                    } else {
                        return `
                            <div class="text-center">
                                <span class="badge badge-danger mr-2">Yayında Değil</span>
                                <a onclick=PublishToggle('${row.id}') class="btn btn-success btn-sm text-white" style="width: 140px;">
                                    <i class="fas fa-lock-open"></i> Yayına Al
                                </a>
                            </div>
                            `;

                    }
                }, "width": "20%"
            },
            {
                "data": "onSale",
                "render": function (data, type, row) {
                    if (data == true) {
                        // course is currently on sale
                        return `
                            <div class="text-center">
                                <span class="badge badge-success mr-2">Satışta</span>
                                <a onclick=SaleToggle('${row.id}') class="btn btn-danger btn-sm text-white" style="width: 140px;">
                                    <i class="fas fa-lock"></i> Satıştan Kaldır
                                </a>
                            </div>
                           `;
                    } else {
                        return `
                            <div class="text-center">
                                <span class="badge badge-danger mr-2">Satışta Değil</span>
                                <a onclick=SaleToggle('${row.id}') class="btn btn-success btn-sm text-white" style="width: 140px;">
                                    <i class="fas fa-lock-open"></i> Satışa Al
                                </a>
                            </div>
                            `;

                    }
                }, "width": "20%"
            },
            {
                "data": "id",
                "render": function (data, type, row) {
                    console.log(row);
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="/Admin/Course/Upsert/${data}" class="btn btn-info"><i class="fa fa-edit"></i> Düzenle</a>
                            <a onclick=Delete("/Admin/Course/Delete/${data}") class="btn btn-danger text-white"><i class="fa fa-trash"></i> Sil</a>
                            <a href="/online-kurslar/${row.courseCategory.seoUrl}/${row.seoUrl}" target="_blank" class="btn btn-secondary"><i class="fa fa-search"></i> İncele</a>
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

$('#tblData').on('row-reorder.dt', function (dragEvent, data, nodes) {
    for (var i = 0, ien = data.length; i < ien; i++) {
        var rowData = dataTable.row(data[i].node).data();
        $.ajax({
            type: "GET",
            cache: false,
            contentType: "application/json; charset=utf-8",
            url: '/Admin/Course/ListOrder',
            data: { Id: rowData.listOrder, fromPosition: data[i].oldData, toPosition: data[i].newData },
            dataType: "json"
        });
    }
});

function PublishToggle(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/Course/PublishToggle',
        data: JSON.stringify(id),
        contentType: 'application/json',
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

function SaleToggle(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/Course/SaleToggle',
        data: JSON.stringify(id),
        contentType: 'application/json',
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

function Delete(url) {
    swal({
        title: "Silmek istediğinizden emin misiniz?",
        text: "Kursun altındaki dersler, tüm bölümler ve varsa içindeki galeri görselleri de silinecektir. Silme işlemini geri alamazsınız!",
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