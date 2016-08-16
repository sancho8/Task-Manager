/*function SendTask() {
    alert("Loaded!");
    $.ajax({
        url: '~/Home/AddTask',
        type: "POST",
        data: {
            Id: document.getElementById('id').value,
            UserId: document.getElementById('userid').value,
            Description: document.getElementById('description').value,
            Data: document.getElementById('data').value,
            Priority: document.getElementById('priority').value,
            IsComlete: document.getElementById('iscompleted').value,
        },
        contentType: 'application/json; charset=utf-8',
        success: function (msg) {
            if (msg == 'True') {
                alert('Данные успешно приняты и обработаны контроллером!');
                window.location.reload(true);
            };
        }
    });
    alert("Sended!");
}*/