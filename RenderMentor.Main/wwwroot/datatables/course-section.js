var courseId = $("#courseId").val();
var dataTable;
var lecturesTable = [];

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        rowReorder: {
            dataSrc: 'listOrder',
            selector: 'td.reorder'
        },
        "order": [[1, "asc"]],
        "autoWidth": false,
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.20/i18n/Turkish.json",
        },
        "ajax": {
            "url": "/Admin/Course/GetCourseSections/" + courseId
        },
        "columns": [
            {
                "className": 'details-control',
                "orderable": false,
                "data": null,
                "width": "3%",
                "render": function () {
                    return `
                        <i class="ti-plus"></i>
                        `;
                },
                "defaultContent": ''
            },
            {
                "data": "listOrder",
                "className": 'reorder',
                "width": "5%"
            },
            {
                "data": "title",
                "render": function (data, type, row) {
                    return `
                        <a href="#editSection" class="edit" data-toggle="modal">${row.title}</a>
                        `;
                }, "width": "72%"
            },
            {
                "data": "id",
                "render": function (data, type, row) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="#editSection" data-toggle="modal" class="edit btn btn-info text-white"><i class="fa fa-edit"></i> Düzenle</a>
                            <a onclick=DeleteSection("/Admin/Course/DeleteSection/${row.courseId}/${row.id}") class="btn btn-danger text-white"><i class="fa fa-trash"></i> Sil</a>
                            <input type='hidden' class='editId' value='${row.id}' />
                        </div>
                        `;
                }, "width": "12%"
            },
        ],
        "columnDefs": [
            {
                "orderable": false,
                "targets": [2, 3]
            }
        ],
    });
}

function createChild(row) {
    var rowData = row.data();
    var tableNumber = rowData.listOrder - 1;

    var lecTable = '<table id="lectureTable' + tableNumber + '" class="table lecture-table"></table>';
    var populateChild = '<div class="slideTable">' + lecTable + '<a href="#createLecture" data-toggle="modal" data-id="' + rowData.id + '" class="create-lecture icon-btn btn btn-sm m-3 btn-warning"><i class="fa fa-plus"></i> Ders Ekle </a></div>'
    row.child(populateChild).show();

    lecturesTable[tableNumber] = $('#lectureTable' + tableNumber).DataTable({
        rowReorder: {
            dataSrc: 'listOrder',
            selector: 'td.reorder'
        },
        "bRetrieve": true,
        "order": [[0, "asc"]],
        "autoWidth": false,
        "bInfo": false,
        "bPaginate": false,
        "searching": false,
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.20/i18n/Turkish.json",
        },
        "ajax": {
            "url": "/Admin/Course/GetSectionLectures/" + rowData.id
        },
        "columns": [
            {
                "data": "listOrder",
                "className": 'reorder',
                "width": "5%"
            },
            {
                "render": function (data, type, row) {
                    return `Ders ${row.listOrder}:`;
                },
                "className": 'lectureCount',
                "width": "%7"
            },
            {
                "data": "title",
                "render": function (data, type, row) {
                    return `
                        <a href="#editLecture" data-toggle="modal" class="edit-lecture">${row.title}</a>
                        <input type='hidden' class='lectureId' value='${row.id}' />
                    `
                }, "width": "81%"
            },
            {
                "data": "id",
                "render": function (data, type, row) {
                    return `
                        <div class="btn-group btn-group-sm" role="group" aria-label="Basic example">
                            <a href="#editLecture" data-toggle="modal" class="edit-lecture btn btn-info text-white"><i class="fa fa-edit"></i> Düzenle</a>
                            <a onclick="DeleteLecture('/Admin/Course/DeleteLecture/${row.courseSectionId}/${row.id}', ${tableNumber})" class="btn btn-danger text-white"><i class="fa fa-trash"></i> Sil</a>
                            <input type='hidden' class='lectureId' value='${row.id}' />
                        </div>
                        `;
                }, "width": "7%"
            },
        ],
        "columnDefs": [
            {
                "orderable": false,
                "targets": [1, 2]
            }
        ],
        "fnInitComplete": function (oSettings) {
            $('.lecture-table thead').hide();
        }
    });

    $('#lectureTable' + tableNumber).on('row-reorder.dt', function (dragEvent, data, nodes) {
        for (var i = 0, ien = data.length; i < ien; i++) {
            var rowData = lecturesTable[tableNumber].row(data[i].node).data();
            var target = rowData.courseSectionId;
            $.ajax({
                type: "GET",
                cache: false,
                contentType: "application/json; charset=utf-8",
                url: '/Admin/Course/LectureListOrder/' + target,
                data: { LectureId: rowData.listOrder, fromPosition: data[i].oldData, toPosition: data[i].newData },
                dataType: "json"
            });
        }
    });
}

function destroyChild(row) {
    var lecTable = $("#tblData", row.child());
    lecTable.detach();
    lecTable.DataTable().destroy();

    // And then hide the row
    row.child.hide();
}

$('#tblData').on('click', '.details-control', function () {
    var tr = $(this).closest('tr');
    $(this).children().toggleClass('ti-plus ti-minus');
    var row = dataTable.row(tr);
    var getSectionId = row.data().id;
    $(".courseSecId").val(getSectionId);

    if (row.child.isShown()) {
        // This row is already open - close it
        $('.slideTable', row.child()).slideUp(function () {
            destroyChild(row);
            tr.removeClass('shown');
        });
    }
    else {
        // Open this row
        createChild(row, 'child-table');
        tr.addClass('shown');
        $('.slideTable', row.child()).slideDown();
    }
});

$('#tblData').on('row-reorder.dt', function (dragEvent, data, nodes) {
    for (var i = 0, ien = data.length; i < ien; i++) {
        var rowData = dataTable.row(data[i].node).data();
        $.ajax({
            type: "GET",
            cache: false,
            contentType: "application/json; charset=utf-8",
            url: '/Admin/Course/SectionListOrder/' + courseId,
            data: { SectionId: rowData.listOrder, fromPosition: data[i].oldData, toPosition: data[i].newData },
            dataType: "json"
        });
    }
});

function DeleteSection(url) {
    swal({
        title: "Silmek istediğinizden emin misiniz?",
        text: "Bölümü silmeden önce başka bölüme transfer etmek istediğiniz dersler varsa lütfen öncelikle bu işlemi yapınız. Bölümün altındaki tüm dersler de bölümle birlikte silinecektir. Silme işlemini geri alamazsınız!",
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

function DeleteLecture(url, tableNumber) {
    swal({
        title: "Silmek istediğinizden emin misiniz?",
        text: "Silme işlemini geri alamazsınız!",
        icon: "warning",
        buttons: ["İptal", "Tamam"],
        dangerMode: true,
        focusConfirm: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        lecturesTable[tableNumber].ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}