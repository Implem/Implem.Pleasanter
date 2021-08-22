$(function () {
    $(document).on('click', '.message .close', function () {
        $(this).parent().remove();
    });

    var $data = $('#MessageData');
    if ($data.length === 1) {
        $.each(JSON.parse($data.val()), function (index, message) {
            $p.setMessage('#Message', JSON.stringify(message));
        });
        $data.remove();
    }
});