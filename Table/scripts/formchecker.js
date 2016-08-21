function EditTask() {
    $('.edit').hide();
    $('.save').show();
}
function SaveTask() {
    $('.save').hide();
    $('.edit').show();
}
function SortTasks() {
    $.ajax({
        url: "Task/OrderTasks",
        method: "POST",
        data: {prop: "Number" },
        success: function () {
            alert("Called");  // or any other indication if you want to show
        },
        statusCode: {
            404: function (content) { alert('cannot find resource'); },
            500: function (content) { alert('internal server error'); }
        }
    });
};