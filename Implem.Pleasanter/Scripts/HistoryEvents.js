$(document).on('click', '.grid-row.history', function () {
    var data = $p.getData();
    data.Ver = $(this).attr('data-ver');
    data.Latest = $(this).attr('data-latest');
    data.SwitchTargets = $('#SwitchTargets').val();
    $p.send($(this));
});