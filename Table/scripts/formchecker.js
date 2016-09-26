$(document).ready(function () {
    $('.edit-holder').hide();
});

$(".edit-holder").keypress(function (e) {
    if (e.keyCode == 13) {
        $(event.target).parents("tr").find(".save").click();
    }
});

function checkSize() {
    var a = 0;
};

$('#RegLogin').focusout(event, function () {
    //alert("focusout");
    var a = $('#RegLogin').val();
    if (a) {
        $.ajax({
            url: '/Auth/CheckFormInput',
            type: 'POST',
            data: {
                field: "login",
                value: a
            },
            success: function (used) {
                if (used == "False") {
                    $('#RegLogin').css("border", "1px solid red");
                }
                else {
                    $('#RegLogin').css("border", "1px solid grey");
                }
            }
        });
    }
});
$('#RegEmail').focusout(event, function () {
    //alert("focusout");
    var a = $('#RegEmail').val();
    if (a)
        $.ajax({
            url: '/Auth/CheckFormInput',
            type: 'POST',
            data: {
                field: "email",
                value: a
            },
            success: function (used) {
                if (used == "False") {
                    $('#RegEmail').css("border", "1px solid red");
                }
                else {
                    $('#RegEmail').css("border", "1px solid grey");
                }
            }
        });
});

var onEditing = false;

function EditTask() {
    $(event.target).parents('tr').find('.edit').hide();
    $(event.target).parents('tr').find('.save').show();
    $(event.target).parents('tr').find('.value-holder').hide();
    $(event.target).parents('tr').find('.edit-holder').show();
    switch ($(event.target).parents('tr').find('.priority-value').text().charAt(0)) {
        case "A": $("#priority-select option[value='A']").attr("selected", "selected"); break;
        case "B": $("#priority-select option[value='B']").attr("selected", "selected"); break;
        case "C": $("#priority-select option[value='C']").attr("selected", "selected"); break;
        case "D": $("#priority-select option[value='D']").attr("selected", "selected"); break;
        default: $("#priority-select option[value=' ']").attr("selected", "selected");
    };
    onEditing = true;
};
$(document).keypress(function (e) {
    if ((e.which == 13) && (onEditing == true)) {
        $('.edit').filter(function () {
        }
    )
    }
});
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
    if (a.length > 50) {
        return;
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
            onEditing = false;
        }
    });
    $(event.target).parents('tr').find('.description-value').text(a);
    $(event.target).parents('tr').find('.data-value').text(b);
    $(event.target).parents('tr').find('.priority-value').text(c);
    $(event.target).parents('tr').find('.number-value').text(d);
};

function ClearForm() {
    document.getElementById("AddTaskForm").reset();
}

$('#AddTaskForm').focusout((function () {
    $('#Error-message-holder').text("");
}));

$('#myModal').focusout(function () {
    $('#RegError').text("");
});

function ValidateRegistrationForm() {
    $('#RegError').text("");
    $('#RegError').css('color', "#FF0000");
    if ($('#RegLogin').css("border") == "1px solid red") {
        $('#RegError').text("Логин уже используется");
        return false;
    }
    if ($('#RegEmail').css("border") == "1px solid red") {
        $('#RegEmail').text("Почтовый ящик уже используется");
        return false;
    }
    var a = $('#RegLogin').val();
    if (a.length < 5) {
        $('#RegError').text("Логин должен быть длиннее 5 символов");
        return false;
    }
    var b = $('#RegPassword').val();
    if (!(b.match(/^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z]{8,}$/))) {
        $('#RegError').text("Введите корректный пароль");
        return false;
    }
    var c = $('#RegConfirmPassword').val();
    if (!(c == b)) {
        $('#RegError').text("Пароли не совпадают");
        return false;
    }
    var d = $('#RegEmail').val();
    var mailValidateRegExp = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    if (!d.match(mailValidateRegExp)) {
        $('#RegError').text("Введите корректный почтовый адресс");
        return false;
    }
    return true;
};

function ValidateAddTaskForm() {
    $('#Error-message-holder').css('color', "#FF0000");
    var isValid = true;
    if (!$('#number-input').val()) {

    }
    else {
        if ($('#number-input').val() < 1) {
            isValid = false;
            $('#Error-message-holder').text("Приоритет не может быть меньше нуля");
        }
    }
    if (!$('#description-input').val()) {
        isValid = false;
        $('#Error-message-holder').text("Введите описание задания");
    }
    if ($('#description-input').val().length > 50) {
        isValid = false;
        $('#Error-message-holder').text("Слишком длинное описание");
    }
    if (isValid) {
        return true;
        $('#Error-message-holder').text("");
    } else {
        return false;
    }
}

$('#number-input').change(function () {
    if (!$(event.target).val()) {
        return;
    }
    if ($(event.target).val() <= 0) {
        $(event.target).val(1);
    }
    if ($(event.target).val() > 99) {
        $(event.target).val(99);
    }
});

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