$(function () {
    $(document).on('click', '#DataViewFilters_Reset', function () {
        $('[id^="DataViewFilters_"]').each(function () {
            $p.clear($(this));
        });
        $p.send($(this));
    });
});