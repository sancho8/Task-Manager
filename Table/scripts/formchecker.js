function TaskStatusChanged() {
    alert('changed');
    $.ajax({
        url: location.href
    }).done(function () {
        alert('Added');
    });
}
function changeChevron() {
    $(this).removeClass("glyphicon-chevron-up");
    $(this).addClass("glyphicon-chevron-down");
    alert("changed class");
}