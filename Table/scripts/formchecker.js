$('table tr td:last-child').click(function () {
    var id = $(this).parent('tr').find('td:first-child').text();
    alert(id);
    $.ajax({
        url: "Home/DeleteTask",
        data: { id: id },
    }).done(function (){
        alert("Called");
        });
});
$('.redact').mouseover(function () {
    $(this).find(".changeable").css('visibility', 'visible');
});
$('.redact').mouseleave(function () {
    $(this).find(".changeable").css('visibility', 'hidden');
});
$(".changeable").click(function () {
    if ($(this).attr('class') == "changeable change-desc") {
        $(this).parents('td').find(".change-desc-input").css('display', 'inline');
        $(this).parents('td').find(".change-desc-input").focus();
    }
    if ($(this).attr('class') == "changeable change-data") {
        alert("data");
    }
    if ($(this).attr('class')=="changeable change-prior") {
        alert("prior");
    }
});
$('.change-desc-input').focusout(function () {
    $(this).css('display', 'none');
})