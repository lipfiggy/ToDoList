$(document).ready(function () {

    $('.ActiveCheck').change(function () {

        var self = $(this);
        var id = self.attr('id');
        var value = self.prop('checked');


        $.ajax({
            url: '/Tasks/AjaxEdit',
            data: {
                id: id,
                value: value
            },
            success: function (result) {
                $('#listDiv').html(result);
            }
        })
    });
})