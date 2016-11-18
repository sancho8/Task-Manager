$(document).ready(function () {
    $('.edit-holder').hide();
    date = new Date().toJSON().slice(0, 10);
    // If datetimepicker element exist.
    if ($("#datetimepicker").length > 0) {
        LoadCalendar();
    }
    $("#data-input").datetimepicker();
    $.datetimepicker.setLocale('ru');
});

// Function for validating contact form
function validateContactForm() {
    // Check if sender name value not empty
    if(!$("#feedback-name").val()){
        $('#contact-form-error-message').text("Введите имя");
        return false;
    }
    // Check if message isn't empty
    if(!$("#feedback-message").val()){
        $('#contact-form-error-message').text("Введите сообщение");
        return false;
    }
    // Validating email
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    if (!re.test($("#feedback-email").val())) {
        $('#contact-form-error-message').text("Введите почтовый адресс");
        return false;
    }
    return true;
}

// Load calender after asynchronous loading document.
$(document).ajaxComplete(LoadCalendar);

// Function for loading calendar.
function LoadCalendar() {
    jQuery('#datetimepicker').datetimepicker({
        timepicker: false,
        format: 'd.m.Y H:i',
        inline: true,
        lang: 'ru',
        startDate: date,
        // Calling controller action when data is changed.
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

// Enter click when task is in editing mode.
$(".edit-holder").keypress(function (e) {
    if (e.keyCode == 13) {
        $(event.target).parents("tr").find(".save").click();
    }
});

// Enter click when opened login form.
$('#login_modal').keypress(function (e) {
    if (e.keyCode == 13) {
        OnLogin();
    }
});

// Enter click when registration form opened.
$('#myModal').keypress(function (e) {
    if (e.keyCode == 13) {
        OnRegistration();
    }
});

// Real-time validation for login field, called when focus leave from input.
$('#RegLogin').focusout(event, function () {
    var a = $('#RegLogin').val();
    if (a) {
        // Check if login already exist.
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

// Real-time validation for login field, called when focus leave from input.
$('#RegEmail').focusout(event, function () {
    //alert("focusout");
    var a = $('#RegEmail').val();
    if (a) {
        // Check if login already exist.
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
    }
});

// Trigger for task mode (on read, on edit)
var onEditing = false;
var date;
var dataChanged = false;

// Called when edit task button clicked
function EditTask() {
    // Hide display elements, show elements for editing
    $(event.target).parents('tr').find('.edit').hide();
    $(event.target).parents('tr').find('.save').show();
    $(event.target).parents('tr').find('.value-holder').hide();
    $(event.target).parents('tr').find('.edit-holder').show();
    // Get value of priority for setting it as default value in editing element
    switch ($(event.target).parents('tr').find('.priority-value').text().charAt(0)) {
        case "A": $("#priority-select option[value='A']").attr("selected", "selected"); break;
        case "B": $("#priority-select option[value='B']").attr("selected", "selected"); break;
        case "C": $("#priority-select option[value='C']").attr("selected", "selected"); break;
        case "D": $("#priority-select option[value='D']").attr("selected", "selected"); break;
        default: $("#priority-select option[value=' ']").attr("selected", "selected");
    };
    onEditing = true;
    date = $(event.target).parents('tr').find('.data-value').text();
    alert(date);
};

// Enter click when task is in editing mode
$(document).keypress(function (e) {
    if ((e.which == 13) && (onEditing == true)) {
        $('.edit').filter(function () {
        }
    )
    }
});

// Save task and reload partioal view for matrix mode
function SaveTaskMatrixMode(){
    SaveTask();
    $("#matrix-button").click();
}
// Save task and reload partioal view for task-rows mode
function SaveTaskRowsMode() {
    SaveTask();
}

// Save Task function
function SaveTask() {
    // Change displaying mode
    $(event.target).parents('tr').find('.save').hide();
    $(event.target).parents('tr').find('.edit').show();
    $(event.target).parents('tr').find('.value-holder').show();
    $(event.target).parents('tr').find('.edit-holder').hide();
    // Get values of inputs
    var a = $.trim($(event.target).parents('tr').find('.description-edit').find('input').val());
    var b = $(event.target).parents('tr').find('.data-edit').find('input').val();
    var c = $(event.target).parents('tr').find('.priority-edit').val(); //this is 'select' element
    var d = $(event.target).parents('tr').find('.number-edit').find('input').val();
    var e = $(event.target).parents('tr').find(':checkbox').attr('checked');
    if (b == "") {
        b = date;
    }
    // Check value of task-status(completed/uncompleted)
    if (!e) {
        e = " false";
    }
    else {
        e = "true";
    }
    if (a.length > 50) {
        return;
    }
    // Pass form data to controller
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
    // Set values to display element for displaying them untill asynchronous reload happens
    $(event.target).parents('tr').find('.description-value').text(a);
    $(event.target).parents('tr').find('.data-value').text(b);
    $(event.target).parents('tr').find('.priority-value').text(c);
    $(event.target).parents('tr').find('.number-value').text(d);
};

// Function for reseting 'add-task' form
function ClearForm() {
    document.getElementById("AddTaskForm").reset();
};

// Validation login form
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
                    window.location.reload();
                }
                else {
                    $('#login-error').text("Неверно веденные данные!");
                }
            }
        });
};

// Clearing error message field when focus leave from form
$('#AddTaskForm').focusout((function () {
    $('#Error-message-holder').text("");
}));

// Clearing error message field when focus leave from form
$('#myModal').focusout(function () {
    $('#RegError').text("");
});

// Function for validation registration form
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

// Function for validating add task form
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

// Real-time validation for number input in add task form
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

// Change delivery status
$("#needDelivety-field").click(function () {
    var status = false;
    if ($("#needDelivety-field").is(":checked")) {
        status = true;
    }
    $.ajax({
        url: 'Profile/ChangeDeliveryStatus',
        type: 'POST',
        data: { status: status },
        success: function () {
            if (status == true) {
                alert("Вы подписилсь на рассылку планов задач");
            }
            if (status == false) {
                alert("Вы отписались от рассылки планов задач");
            }
        }
    });
});

// Called when task status checkbox input changed
function TaskStatusChanged(elem) {
    var value = elem.checked;
    var a = elem.id;
    $.ajax({
        url: 'Task/ChangeTaskStatus',
        type: 'POST',
        data: { id: a, value: value }
    });
};

// Reset values of change profile data button
$('#reset-changes-button').on('click', function () {
    $('.inform-box p').show();
    $('#change-profile-error-message-holder').text("");
    $('.field-redact-holder').css("display", "none");
    $('#save-changes-button').css("display", "none");
    $('#reset-changes-button').css("display", "none");
    $('#change-password-button').css("display", "block");
});