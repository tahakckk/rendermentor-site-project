
// Open Partial View in Modal (No required Data: For File Browser or any Static View)
function GetModal(url, getCodeName, myModalName) {
    $.ajax({
        url: url,
        cache: false,
        context: this,
        success: function (result) {

            $(getCodeName).empty();
            $(getCodeName).html(result);
            jQuery(myModalName).modal('show');
        },
        error: function (result) {
            alert("Error");
        }
    });
}
// Open Partial View in Modal (Data Required: Pass data as ID or more for Edit/Create/Upsert Views in Modal)
function GetModalWithData(parameters, url, getCodeName, myModalName) {
    $.ajax({
        url: url,
        data: parameters,
        cache: false,
        context: this,
        success: function (result) {

            $(getCodeName).empty();
            $(getCodeName).html(result);
            jQuery(myModalName).modal('show');
        },
        error: function (result) {
            alert("Error");
        }
    });
}

function secondsTimeSpanToHMS(s) {
    var h = Math.floor(s / 3600); //Get whole hours
    s -= h * 3600;
    var m = Math.floor(s / 60); //Get remaining minutes
    s -= m * 60;
    return h + ":" + (m < 10 ? '0' + m : m) + ":" + (s < 10 ? '0' + s : s); //zero padding on minutes and seconds
}

// When Upload image changed set preview image for the selected image
$(".image-input input").change(function () {
    if (this.files && this.files[0]) {
        var reader = new FileReader();
        var thisMedia = $(this).parent().parent().parent();
        reader.onload = function (e) {
            thisMedia.find('img').attr('src', e.target.result);
            thisMedia.find('.position-relative a').attr('href', e.target.result);
        }

        reader.readAsDataURL(this.files[0]); // convert to base64 string
    }
});

// If element is visible on screen
$.fn.isInViewport = function () {
    var elementTop = $(this).offset().top;
    var elementBottom = elementTop + $(this).outerHeight();

    var viewportTop = $(window).scrollTop();
    var viewportBottom = viewportTop + $(window).height();

    return elementBottom > viewportTop && elementTop < viewportBottom;
};
  



