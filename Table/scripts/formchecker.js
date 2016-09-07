$(document).ready(function () {
    $('.edit-holder').hide();
});
function EditTask() {
    $('.edit').hide();
    $('.save').show();
    $(event.target).parents('tr').find('.value-holder').hide();
    $(event.target).parents('tr').find('.edit-holder').show();
};
function SaveTask() {
    $('.save').hide();
    $('.edit').show();
    $(event.target).parents('tr').find('.value-holder').show();
    $(event.target).parents('tr').find('.edit-holder').hide();
};

function ClearForm() {
    document.getElementById("AddTaskForm").reset();
}

function ValidateAddTaskForm() {
    $('#Error-message-holder').css('color', "#FF0000");
    var isValid = true;
    if (!$('#description-input').val()) {
        isValid = false;
        $('#Error-message-holder').text("Введите описание задания");
    }
    if (!$('#data-input').val()) {
        isValid = false;
        $('#Error-message-holder').text("Введите дату");
    }
    if (!$('#nunber-input').val()) {
        isValid = false;
        $('#Error-message-holder').text("Введите номер задания");
    }
    if ($('#nunber-input').val() < 1) {
        isValid = false;
        $('#Error-message-holder').text("Приоритет не может быть меньше 0!");
    }
    if (isValid) { 
        return true;
        $('#Error-message-holder').text("");
    } else {
        return false;
}
}

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