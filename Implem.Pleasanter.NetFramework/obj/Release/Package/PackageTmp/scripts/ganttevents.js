$(function () {
    $(document).on('click', '#Gantt .planned rect,#Gantt .earned rect,#Gantt .title text', function () {
        if ($(this).filter('.summary').length === 0) {
            location.href = $('#BaseUrl').val() + $(this).attr('data-id');
        }
    });
});