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
    alert($(this).attr('class'));
    if($(this).class("change-desc")){
        alert("desc");
    }
    if ($(this).attr('class') == "change-data") {
        alert("data");
    }
    if ($(this).class("change-prior")) {
        alert("prior");
    }
    else {
        alert("else");
    }
});