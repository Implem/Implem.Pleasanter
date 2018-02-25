$p.setImageLib = function () {
    $p.paging('#ImageLib');
}

$p.deleteImage = function ($control) {
    var data = {};
    data.Guid = $control.attr('data-id');
    $p.ajax(
        $control.attr('data-action'),
        $control.attr('data-method'),
        data,
        $control);
}