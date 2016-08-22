function EditTask() {
    $('.edit').hide();
    $('.save').show();
}
function SaveTask() {
    $('.save').hide();
    $('.edit').show();
}
$(function () {
    $.ajaxSetup({ cache: false });
    $("#login-button").click(function (e) {

        e.preventDefault();
        $.get(this.href, function (data) {
            $('#dialogContent').html(data);
            $('#modDialog').modal('show');
        });
    });
});