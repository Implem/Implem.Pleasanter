$(function () {
    $(document).on('click', '#ViewFilters_Reset', function () {
        $p.send($(this));
    });
});