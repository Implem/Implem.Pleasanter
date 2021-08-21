$(function () {
    $(document).on('click', '.message .close', function () {
        $(this).parent().remove();
    });

    var $data = $('#MessageData');
    if ($data.length === 1) {
        for (var message of JSON.parse($data.val())) {
            $p.setMessage('#Message', JSON.stringify(message));
        }
        $data.remove();
    }
});