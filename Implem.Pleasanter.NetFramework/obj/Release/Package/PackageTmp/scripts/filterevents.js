$(function () {
    $(document).on('click', '#ViewFilters_Reset', function () {
        $('[id^="ViewFilters_"]').each(function () {
            $p.clear($(this));
        });
        $p.send($(this));
    });
});