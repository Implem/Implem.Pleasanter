$(function () {
    $(document).on('change', '#Search', function () {
        search(0);
    });
    $(document).on('keydown', '#Search', function (e) {
        if (e.which === 13) search(0);
    });
    $(window).on('load scroll resize', function () {
        var $control = $('#SearchResults')
        if ($control.length) {
            if ($(window).scrollTop() + $(window).height() >= $control.offset().top + $control.height()) {
                if ($('#SearchOffset').val() !== '-1') {
                    search($('#SearchOffset').val());
                }
            }
        }
    });
    $(document).on('click', '#SearchResults .result', function () {
        location.href = $(this).attr('data-href');
    });

    function search(offset)
    {
        var searchWord = $('#Search').val();
        if (offset !== '-1' && $p.searchWord !== searchWord + offset) {
            $p.searchWord = searchWord + offset;
            console.log(arguments.callee.caller);
            $p.ajax($('#ApplicationPath').val() +
                'items/ajaxsearch?text=' + escape(searchWord) +
                '&offset=' + offset, 'get');
        }
    }
});