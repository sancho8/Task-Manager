$('.redact').mouseover(function () {
    $(this).find(".changeable").css('visibility', 'visible');
});

$('.redact').mouseleave(function () {
    $(this).find(".changeable").css('visibility', 'hidden');
});

$(".changeable").click(function () {
    if ($(this).hasClass("change-desc")) {
        $(this).parents('td').find("#redacttd").css('display', 'none');
        $(this).parents('td').find(".change-desc-input").css('display', 'inline');
        $(this).parents('td').find(".change-desc-input").val("sss");
        $(this).parents('td').find(".change-desc-input").focus();
    }
    if ($(this).hasClass("change-data")) {
        alert("data");
    }
    if ($(this).hasClass("change-prior")) {
        alert("prior");
    }
});

$('.change-desc-input').focusout(function Asd () {
    $(this).parents('td').find("#redacttd").css('display', 'inline');
    $(this).css('display', 'none');
})