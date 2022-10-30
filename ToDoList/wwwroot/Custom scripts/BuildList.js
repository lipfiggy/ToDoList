
$(document).ready(function () {

    $.ajax({
        url: '/Tasks/BuildListOfToDos',
        success: function (result) {
            $('#listDiv').html(result);
        }
    });
});