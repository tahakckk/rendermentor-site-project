const items = 12;
var gallery = $("#gallerySection");
var pagination = $('.pagination-box');
var count = 0;
var adjacents = 0;
var page = 1;
var minItem = 1;
var maxItem = items;

function calculatePagination(pageNumber) {
    maxItem = items;
    minItem = (pageNumber * items) - (items);
    maxItem = (pageNumber * maxItem);
    if (maxItem > count) {
        maxItem = count;
    }
}

function LoadGallery(data) {
    gallery.empty();
    pagination.empty();
    $.each(data.data, function (i, item) {
        ++count;
    });
    calculatePagination(page);
    for (var i = minItem; i < maxItem; i++) {
        gallery.append(`
            <div class="col-md-6 col-lg-3 marginTop-30">
                <div class="media-box">
                    <a href="${data.data[i].image}" data-fancybox="gallery" class="card shadow-v1 align-items-center p-0 hover:transformTop">
                        <img class="img-responsive" src="${data.data[i].image}" />
                    </a>
                    <input class="imageId" type="hidden" name="id" value="${data.data[i].id}" />
                </div>
            </div>
        `);
    }

    adjacents = Math.ceil(count / items);
    if (count > items) {
        pagination.append(`
            <div class="col-12">
                <div class="media-pagination">
        `);
        var paginationInner = $('.media-pagination');
        for (var i = 0; i < adjacents; i++) {
            var number = i + 1;
            if (page == number) {
                paginationInner.append(`<a class="active" href="javascript:void(0);"><span>${number}</span></a>`);
            } else {
                paginationInner.append(`<a href="javascript:void(0);"><span>${number}</span></a>`);
            }
        }
        pagination.append(`
                </div>
            </div>
        `);
    }
}
function GetGallery(data) {
    $.ajax({
        url: '/Admin/MediaGallery/GetAllCourseCovers',
        method: 'GET',
        success: function (data) {
            count = 0;
            LoadGallery(data);
        },
        error: function (err) {
            console.error(err);
        }
    });
}

var showCheck = false;
$('#showCheck').click(function () {
    $('#showCheck i').addClass(showCheck ? 'far fa-square' : 'fas fa-check-square');
    $('#showCheck i').removeClass(showCheck ? 'fas fa-check-square' : 'far fa-square');
    $('#showCheck span').text(showCheck ? 'Görsel Seç' : 'Seçim Modunu Kapat');

    showCheck = !showCheck;

    if (!showCheck) {
        $('.select-box').hide();
        $('.delete-box').hide();
        $('.checkAllBox').empty();
        $('.deleteAllBox').empty();
    } else {
        $('.select-box').show();
        $.each($('.media-box'), function (i, item) {
            $(this).append(`
            <span class="select-box"><i class="far fa-square"></i></span>
            <span class="delete-box"><i class="fa fa-trash"></i></span>
            `);
        });
        $('.checkAllBox').append(`
            <button id="checkAll" type="button" class="btn btn-check-all"><i class="far fa-square"></i> <span>Tümünü Seç</span></button>
        `);
        $('.deleteAllBox').append(`
            <button id="deleteAll" type="button" class="mt-2 btn btn-danger btn-sm"> Seçilenleri Sil</button>
        `);
    }
});

var checkAll = false;
$('.gallery-controls').on('click', '#checkAll', function () {
    $('#checkAll i').addClass(checkAll ? 'far fa-square' : 'fas fa-check-square');
    $('#checkAll i').removeClass(checkAll ? 'fas fa-check-square' : 'far fa-square');
    checkAll = !checkAll;
    $('#checkAll span').text(checkAll ? 'Tüm Seçimleri Kaldır' : 'Tümünü Seç');

    $.each($('.select-box'), function (i, item) {
        if (!checkAll) {
            $(this).children().removeClass('fa-check-square');
            $(this).children().addClass('fa-square');
            $(this).removeClass('checked');
        } else {
            $(this).children().removeClass('fa-square');
            $(this).children().addClass('fa-check-square');
            $(this).addClass('checked');
        }
    });
});

$('.media-gallery').on('click', '.select-box', function () {
    if ($(this).hasClass('checked')) {
        $(this).children().removeClass('fa-check-square');
        $(this).children().addClass('fa-square');
        $(this).removeClass('checked');
    } else {
        $(this).children().removeClass('fa-square');
        $(this).children().addClass('fa-check-square');
        $(this).addClass('checked');
    }
});
var toDeleteIds = [];
$('.gallery-controls').on('click', '#deleteAll', function () {
    swal({
        title: "Toplu silme işlemini onaylıyor musunuz?",
        text: "Bu işlemle birlikte seçili tüm görseller sistemden silinecektir.",
        icon: "warning",
        buttons: ["İptal", "Tamam"],
        dangerMode: true
    }).then((willDelete) => {
        toDeleteIds = [];
        $.each($('.checked'), function (i, item) {
            var toDeleteId = $(this).parent().find('.imageId').val();
            toDeleteIds.push(toDeleteId);
        });
        toDeleteIds = JSON.stringify(toDeleteIds);
        $.ajax({
            type: "DELETE",
            url: "/Admin/MediaGallery/DeleteBulkCourseCovers/" + toDeleteIds,
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    GetGallery();
                    showCheck = false;
                    $('#showCheck span').text('Görsel Seç');
                    $('#showCheck i').removeClass('fas fa-check-square');
                    $('#showCheck i').addClass('far fa-square');
                    $('.checkAllBox').empty();
                    $('.deleteAllBox').empty();
                } else {
                    toastr.error(data.message);
                }
            }
        });
    });
});

$('.media-gallery').on('click', '.delete-box', function () {
    var id = $(this).parent().find('.imageId').val();
    swal({
        title: "Silmek istediğinizden emin misiniz?",
        text: "Seçili görsel sistemden silinecektir. Silme işlemini geri alamazsınız!",
        icon: "warning",
        buttons: ["İptal", "Tamam"],
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: "/Admin/MediaGallery/DeleteCourseCover/" + id,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        GetGallery();
                        showCheck = false;
                        $('#showCheck span').text('Görsel Seç');
                        $('#showCheck i').removeClass('fas fa-check-square');
                        $('#showCheck i').addClass('far fa-square');
                        $('.checkAllBox').empty();
                        $('.deleteAllBox').empty();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
});

$('.pagination-box').on('click', '.media-pagination a', function () {
    page = $(this).children().text();
    GetGallery();
    showCheck = false;
    $('#showCheck span').text('Görsel Seç');
    $('#showCheck i').removeClass('fas fa-check-square');
    $('#showCheck i').addClass('far fa-square');
    $('.checkAllBox').empty();
    $('.deleteAllBox').empty();
});

GetGallery();
function TriggerClick() {
    $('#uploadGallery').trigger('click');
}
$(document).ready(function () {

    $('#uploadGallery').change(function (e) {
        e.preventDefault();
        var form = $(this).form();
        var url = form.attr('action');
        form.ajaxSubmit({
            url: url,
            success: function (data) {
                if (data.success) {
                    GetGallery();
                    showCheck = false;
                    $('#showCheck span').text('Görsel Seç');
                    $('#showCheck i').removeClass('fas fa-check-square');
                    $('#showCheck i').addClass('far fa-square');
                    $('.checkAllBox').empty();
                    $('.deleteAllBox').empty();
                    toastr.success(data.message);
                } else {
                    toastr.error(data.message);
                }
            }
        });

        return false;
    });

});