$(document).ready(function () {
    $('.edit-holder').hide();
});
function EditTask() {
    $(event.target).parents('tr').find('.edit').hide();
    $(event.target).parents('tr').find('.save').show();
    $(event.target).parents('tr').find('.value-holder').hide();
    $(event.target).parents('tr').find('.edit-holder').show();
};
function SaveTask() {
    $(event.target).parents('tr').find('.save').hide();
    $(event.target).parents('tr').find('.edit').show();
    $(event.target).parents('tr').find('.value-holder').show();
    $(event.target).parents('tr').find('.edit-holder').hide();
    var a = $.trim($(event.target).parents('tr').find('.description-edit').find('input').val());
    var b = $(event.target).parents('tr').find('.data-edit').find('input').val();
    var c = $(event.target).parents('tr').find('.priority-edit').val(); //this is 'select' element
    var d = $(event.target).parents('tr').find('.number-edit').find('input').val();
    var e = $(event.target).parents('tr').find(':checkbox').attr('checked');
    if (!e) {
        e = " false";
    }
    else {
        e = "true";
    }
    $.ajax({
        url: 'Task/UpdateTask',
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=windows-1251',
        data: {
            id: $(event.target).parents('tr').find(':checkbox').attr('id'),
            description: a,
            data: b,
            priority: c,
            number: d,
            isComplete: e
        },
        success: function () {
            //alert($(event.target).parents('tr').find(':checkbox').attr('id'));
        }
    });
    $(event.target).parents('tr').find('.description-value').text(a);
    $(event.target).parents('tr').find('.data-value').text(b);
    $(event.target).parents('tr').find('.priority-value').text(c);
    $(event.target).parents('tr').find('.number-value').text(d);
};

/*function ClearForm() {
    document.getElementById("AddTaskForm").reset();
}*/

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
    if (!$('#number-input').val()) {
        isValid = false;
        $('#Error-message-holder').text("Введите номер задания");
    }
    if ($('#number-input').val() < 1) {
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
    //alert(value + " " + a);
    $.ajax({
        url: 'Task/ChangeTaskStatus',
        type: 'POST',
        data: { id: a, value: value },
        success: function () {
            //alert("Success");
        }
    });
};