$(function () {
    var timer;
    var intervalTimer;
    $(document).on('mouseenter', '#ViewFilters label', function () {
        clearTimeout(timer);
        clearInterval(intervalTimer);
        if ($(".menu-negative:visible").length) {
            $(".menu-negative:visible").hide();
        }
        if ($('.ui-multiselect-close:visible').length) {
            $('.ui-multiselect-close:visible').click();
        }
        $("#ViewFiltersLabelMenus p").remove();
        timer = setTimeout(function ($this) {
            var dataName = $this[0].htmlFor;
            var textbox_element = document.getElementById('ViewFilters__');
            var new_element = document.createElement('p');
            new_element.textContent = dataName;
            textbox_element.appendChild(new_element).style.display = "none";
            $menuNegative = $(".menu-negative[id='ViewFilters__']");
            $menuNegative.css('width', '');
            $menuNegative
                .css('position', 'fixed')
                .css('top', $this.offset().top + $this.outerHeight())
                .css('left', $this.offset().left)
                .outerWidth($this.outerWidth() > $menuNegative.outerWidth()
                    ? $this.outerWidth()
                    : $menuNegative.outerWidth())
                .show();
        }, 700, $(this));
    });
    $(document).on('mouseenter', '#ViewFilters', function () {
        clearInterval(intervalTimer);
    });
    $(document).on('mouseleave', '.menu-negative', function () {
        if (!$('.ui-multiselect-menu:visible').length) {
            $('.menu-negative:visible').hide();
        }
    });
    $(document).on('click', '.menu-negative > li.negative', function (e) {
        var data = {};
        data.ViewFilters_Negative = $("#ViewFiltersLabelMenus p").text();
        $p.ajax(
            $('#ApplicationPath').val() + 'items/' + $p.id() + '/index',
            'POST',
            data,
            undefined);
    });
    $(document).on('click', '.menu-negative > li.positive', function (e) {
        var data = {};
        data.ViewFilters_Positive = $("#ViewFiltersLabelMenus p").text();
        $p.ajax(
            $('#ApplicationPath').val() + 'items/' + $p.id() + '/index',
            'POST',
            data,
            undefined);
    });
});