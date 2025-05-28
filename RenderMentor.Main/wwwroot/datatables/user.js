var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "autoWidth": false,
        "pageLength": 50,
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.20/i18n/Turkish.json",
        },
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            {
                "data": "name",
                "render": function (data, type, row) {
                    return `
                        <a href="/Admin/User/Upsert/${row.id}">${row.name}</a>
                    `;
                }, "width": "20%"
            },
            { "data": "email", "width": "16%" },
            {
                "data": "gender",
                "render": function (data) {
                    if (data == "1") {
                        return `Erkek`;
                    } else {
                        return `Kadın`;
                    }
                }, "width": "5%"
            },
            {
                "data": "role",
                "render": function (data) {
                    if (data == "Student") {
                        return `Öğrenci`;
                    } else {
                        return `Firma`;
                    }
                },
                "width": "7%"
            },
            { "data": "company.name", "width": "19%" },
            {
                "data": "createDate",
                "render": function (data, type) {
                    if (data == '0001-01-01T00:00:00') {
                        return ``;
                    }
                    var parsedDate = new Date(Date.parse(data));
                    var date = parsedDate.toLocaleDateString();
                    var time = parsedDate.toLocaleTimeString();
                    time = time.substring(0, 5);
                    if (type === "display") {
                        return `
                            ${date} ${time}
                        `;
                    }
                    return data;
                }, "width": "10%"
            },
            {
                "data": "emailConfirmed",
                "render": function (data) {
                    if (data == true) {
                        return `
                        <div class="text-center">
                            <span class="badge badge-success">Onaylandı</span>
                        </div>
                        `;
                    } else {
                        return `
                        <div class="text-center">
                            <span class="badge badge-danger">Onaylanmadı</span>
                        </div>
                        `;
                    }
                },
                "width": "5%"
            },
            
            {
                "data": { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        // user is currently locked
                        return `
                           <div class="text-center">
                               <a onclick=LockUnlock('${data.id}') class="btn btn-danger btn-sm text-white" style="width: 120px;">
                                   <i class="fas fa-lock-open"></i> Aktive Et
                               </a>
                           </div>
                           `;
                    } else {
                        return `
                           <div class="text-center">
                               <a onclick=LockUnlock('${data.id}') class="btn btn-success btn-sm text-white" style="width: 120px;">
                                   <i class="fas fa-lock"></i> Dondur
                               </a>
                           </div>
                           `;
                    }
                }, "width": "10%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="/Admin/User/Upsert/${data}" class="btn btn-info"><i class="fa fa-edit"></i> Düzenle</a>
                            <a href="/" class="btn btn-warning"><i class="fa fa-search"></i> Panele Git</a>
                        </div>
                    `;
                },
                "width": "8%"
            },
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
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