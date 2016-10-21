$(document).ready(function () {
    $('.edit-holder').hide();
    date = new Date().toJSON().slice(0, 10);
    if ($("#datetimepicker").length > 0) {
        LoadCalendar();
    }
});

function validateContactForm() {
    if(!$("#feedback-name").val()){
        $('#contact-form-error-message').text("Введите имя");
        return false;
    }
    if(!$("#feedback-message").val()){
        $('#contact-form-error-message').text("Введите сообщение");
        return false;
    }
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    if (!re.test($("#feedback-email").val())) {
        $('#contact-form-error-message').text("Введите почтовый адресс");
        return false;
    }
    return true;
}

$(document).ajaxComplete(LoadCalendar);

function LoadCalendar() {
    jQuery('#datetimepicker').datetimepicker({
        timepicker: false,
        format: 'd.m.Y H:i',
        inline: true,
        lang: 'ru',
        startDate: date,
        onSelectDate: function (ct, $i) {
            $.ajax({
                type: "POST",
                url: "/Task/ChangeDate",
                data: {
                    date: ct.toString()
                },
                success: function (used) {
                    //alert(ct);
                    date = ct;
                    $('#actionlink').click();
                }
            });
        }
    });
    $.datetimepicker.setLocale('ru');
}

$(".edit-holder").keypress(function (e) {
    if (e.keyCode == 13) {
        $(event.target).parents("tr").find(".save").click();
    }
});

$('#login_modal').keypress(function (e) {
    if (e.keyCode == 13) {
        OnLogin();
    }
});
$('#myModal').keypress(function (e) {
    if (e.keyCode == 13) {
        OnRegistration();
    }
});

function checkSize() {
    var a = 0;
};

$('#RegLogin').focusout(event, function () {
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

function SaveTaskWithReload(){
    SaveTask();
    $("#matrix-button").click();
    //.click();
}

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
            $("#matrix-button").click();
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
};

function OnLogin() {
    $('#login-error').text("");
        $.ajax({
            type: "POST",
            async: false,
            url: '/Auth/LogInUser',
            data:{
                login: $("#login-input").val(),
                password: $("#password-input").val()
            },
            success: function (data) {
                if (data == "True") {
                    $('#login-form').submit();
                }
                else {
                    $('#login-error').text("Неверно веденные данные!");
                }
            }
        });
};

$('#AddTaskForm').focusout((function () {
    $('#Error-message-holder').text("");
}));

$('#myModal').focusout(function () {
    $('#RegError').text("");
});

function OnRegistration() {
    $("#RegError").text("");
    $.ajax({
        type: "POST",
        async: false,
        url: '/Auth/RegisterUser',
        data: {
            login: $('#RegLogin').val(),
            password:  $('#RegPassword').val(),
            confirmPassword: $("#RegConfirmPassword").val(),
            email: $("#RegEmail").val(),
            needDelivery: $("#RegNeedDelivery").val()
        },
        success: function (data) {
            if (data == "True") {
                $.ajax({
                    type: "POST",
                    async: false,
                    url: '/Auth/LogInUser',
                    data: {
                        login: $('#RegLogin').val(),
                        password: $('#RegPassword').val()
                    },
                });
                window.location.reload();
            }
            else {
                $("#RegError").text(data);
            }
        }
    });
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

$('#change-password-button').on('click', function () {
    $('.inform-box p').hide();
    $('.field-redact-holder').css("display", "inline");
    $('#change-password-button').css("display", "none");
    $('#save-changes-button').css("display", "inline");
    $('#reset-changes-button').css("display", "inline");
});


/*function ValidateUpdateProfileDataForm (){
    var changeLogin = $('#change-login-field').find('input').val();
    var changeEmail = $('#change-email-field').find('input').val();
    var oldPassword = $('#old-password-field').find('input').val();
    var newPassword = $('#new-password-field').find('input').val();
    var confirmPassword = $('#confirm-password-field').find('input').val();
    $.ajax({
        url: '/Profile/ValidateUpdateProfileForm',
        method: 'POST',
        async: false,
        data:{
            login: changeLogin,
            email: changeEmail,
            oldPassword: oldPassword,
            newPassword: newPassword,
            confirmPassword: confirmPassword,
            loginChanged: fieldChanges.login,
            emailChanged: fieldChanges.email,
            passwordChanged: fieldChanges.password
            //needDelivery: $('#needDelivety-field').attr('checked')
        },
        success: function (result) {
            if (result == "Success") {
                    $('#loginChanged').prop('checked', true);
                $('#emailChanged').val(fieldChanges.login);
                $('#passwordChanged').val(fieldChanges.login);
                $("#ChangeProfileDataForm").submit();
                alert("success");
            }
            else {
                $('#change-profile-error-message-holder').text(result);
            }
            fieldChanges.login = false;
            fieldChanges.email = false;
            fieldChanges.password = false;
            $('#change-login-field').find('input').val("");
            $('#change-email-field').find('input').val("");
            $('#old-password-field').find('input').val("");
            $('#new-password-field').find('input').val("");
        }
    });
};*/

var fieldChanges = {
    "login": false,
    "email": false,
    "password": false,
}

$('#reset-changes-button').on('click', function () {
    $('.inform-box p').show();
    $('#change-profile-error-message-holder').text("");
    $('.field-redact-holder').css("display", "none");
    $('#save-changes-button').css("display", "none");
    $('#reset-changes-button').css("display", "none");
    $('#change-password-button').css("display", "block");
});