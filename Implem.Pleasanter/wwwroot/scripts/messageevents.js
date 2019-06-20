$(function () {
    $(document).on('click', '.message .close', function () {
        $(this).parent().remove();
    });

    var $data = $('#MessageData');
    if ($data.length === 1) {
        $p.setMessage('#Message', $data.val());
    }
});