function EditTask() {
    $('.edit').hide();
    $('.save').show();
};
function SaveTask() {
    $('.save').hide();
    $('.edit').show();
};
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

function NumberChanged() {
    alert($(this).val());
}

function TaskStatusChanged(elem) {
    var value = elem.checked;
    var a = elem.id;
    alert(value + " " + a);
    $.ajax({
        url: 'Task/UpdateTask',
        type: 'POST',
        data: { id: a, value: value },
        success: function () {
            alert("Success");
        }
    });
};