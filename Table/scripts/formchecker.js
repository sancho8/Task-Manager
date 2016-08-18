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