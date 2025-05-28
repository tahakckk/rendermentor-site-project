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
            "url": "/Admin/Instructor/GetAll"
        },
        "columns": [
            {
                "data": "listOrder",
                "className": 'reorder',
                "width": "5%"
            },
            {
                "data": "applicationUser.name",
                "render": function (data, type, row) {
                    return `
                        <a href="/Admin/Instructor/Upsert/${row.id}">${row.applicationUser.name}</a>
                    `;
                }, "width": "43%"
            },
            { "data": "applicationUser.email", "width": "20%" },
            {
                "data": "applicationUser.emailConfirmed",
                "render": function (data) {
                    if (data == true) {
                        return `
                        <div class="text-center">
                            <span class="badge badge-success">Mail Onaylandı</span>
                        </div>
                        `;
                    } else {
                        return `
                        <div class="text-center">
                            <span class="badge badge-danger">Mail Onaylanmadı</span>
                        </div>
                        `;
                    }
                },
                "width": "8%"
            },
            {
                "data": "applicationUser.lockoutEnd",
                "render": function (data, type, row) {
                    var today = new Date().getTime();
                    var lockout = new Date(row.applicationUser.lockoutEnd).getTime();
                    if (lockout > today) {
                        // user is currently locked
                        return `
                           <div class="text-center">
                               <a onclick=LockUnlock('${row.id}') class="btn btn-danger btn-sm text-white" style="width: 140px;">
                                   <i class="fas fa-lock-open"></i> Aktive Et
                               </a>
                           </div>
                           `;
                    } else {
                        if (row.isAdmin == true) {
                            return `
                            <div class="text-center">
                                <a onclick=IsAdmin() class="btn btn-secondary btn-sm text-white" style="width: 140px; cursor: not-allowed !important;">
                                    <i class="fas fa-lock"></i> Dondur
                                </a>
                            </div>
                            `;
                        } else {
                            return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${row.id}') class="btn btn-success btn-sm text-white" style="width: 140px;">
                                    <i class="fas fa-lock"></i> Dondur
                                </a>
                            </div>
                            `;
                        }
                        
                    }
                }, "width": "16%"
            },
            {
                "data": "id",
                "render": function (data, type, row) {
                    if (row.isAdmin == true) {
                        return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="/Admin/Instructor/Upsert/${row.id}" class="btn btn-info"><i class="fa fa-edit"></i> Düzenle</a>
                            <a onclick=IsAdmin() class="btn btn-secondary text-white" style="cursor: not-allowed !important;"><i class="fa fa-trash"></i> Sil</a>
                        </div>
                        `;
                    } else {
                        return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="/Admin/Instructor/Upsert/${row.id}" class="btn btn-info"><i class="fa fa-edit"></i> Düzenle</a>
                            <a onclick=Delete("/Admin/Instructor/Delete/${row.id}") class="btn btn-danger text-white"><i class="fa fa-trash"></i> Sil</a>
                        </div>
                        `;
                    }
                },
                "width": "8%"
            },
        ],
        "columnDefs": [
            {
                "orderable": false,
                "targets": [1, 2]
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
            url: '/Admin/Instructor/ListOrder',
            data: { Id: rowData.listOrder, fromPosition: data[i].oldData, toPosition: data[i].newData },
            dataType: "json"
        });
    }
});

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/Instructor/LockUnlock',
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

function IsAdmin() {
    swal({
        title: "İzinsiz işlem",
        text: "İşlem yapmak istediğiniz eğitmen Admin yetkisi olduğundan dolayı dondurulumaz veya silinemez.",
        icon: "warning",
        buttons: ["Tamam"],
        dangerMode: true
    });
}

function Delete(url) {
    swal({
        title: "Silmek istediğinizden emin misiniz?",
        text: "Silme işlemini geri alamazsınız! Eğitmeni silmeniz durumunda öğretmenin atanmış olduğu ders olmaması için dikkat ediniz.",
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