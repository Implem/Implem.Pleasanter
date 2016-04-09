func.setUi = function () {
    $('.edit-form-tabs').tabs();
    $('.edit-form-tabs-max').tabs();
    $('.button-goback').button({ icons: { primary: 'ui-icon-circle-arrow-w' } });
    $('.button-create').button({ icons: { primary: 'ui-icon-plus' } });
    $('.button-save').button({ icons: { primary: 'ui-icon-disk' } });
    $('.button-copy').button({ icons: { primary: 'ui-icon-copy' } });
    $('.button-move').button({ icons: { primary: 'ui-icon-transferthick-e-w' } });
    $('.button-send-mail').button({ icons: { primary: 'ui-icon-mail-closed' } });
    $('.button-delete').button({ icons: { primary: 'ui-icon-trash' } });
    $('.button-separate').button({ icons: { primary: 'ui-icon-extlink' } });
    $('.button-synchronize').button({ icons: { primary: 'ui-icon-refresh' } });
    $('.button-history').button({ icons: { primary: 'ui-icon-clock' } });
    $('.button-export').button({ icons: { primary: 'ui-icon-arrowreturnthick-1-w' } });
    $('.button-import').button({ icons: { primary: 'ui-icon-arrowreturnthick-1-e' } });
    $('.button-previous').button({ icons: { primary: 'ui-icon-seek-prev' } });
    $('.button-next').button({ icons: { primary: 'ui-icon-seek-next' } });
    $('.button-reload').button({ icons: { primary: 'ui-icon-refresh' } });
    $('.button-cancel').button({ icons: { primary: 'ui-icon-cancel' } });
    $('.button-list').button({ icons: { primary: 'ui-icon-document' } });
    $('.button-setting').button({ icons: { primary: 'ui-icon-gear' } });
    $('.button-person').button({ icons: { primary: 'ui-icon-person' } });
    $('.button-up').button({ icons: { primary: 'ui-icon-circle-triangle-n' } });
    $('.button-down').button({ icons: { primary: 'ui-icon-circle-triangle-s' } });
    $('.button-authenticate').button({ icons: { primary: 'ui-icon-unlocked' } });
    $('.button-to-left').button({ icons: { primary: 'ui-icon-circle-triangle-w' } });
    $('.button-to-right').button({ icons: { primary: 'ui-icon-circle-triangle-e' } });
    $('.button-visible').button({ icons: { primary: 'ui-icon-image' } });
    $('.button-hide, .button-reset').button({ icons: { primary: 'ui-icon-close' } });
    $('#ui-datepicker-div').remove();
    $('.datepicker').datepicker({
        showButtonPanel: true,
        onSelect: function (date) {
            getFormData($(this))[this.id] = date;
        }
    });
    $('.radio').buttonset();
    $('.control-selectable').selectable({
        stop: function () {
            getFormData($(this))[this.id] = $('.ui-selected', this)
                .map(function () { return $(this).attr('value'); })
                .get()
                .join(';');
        }
    });
    $('.control-slider-ui').each(function () {
        var $control = $('#' + $(this).attr('id').split(',')[0]);
        $(this).slider({
            min: parseFloat($(this).attr('min')),
            max: parseFloat($(this).attr('max')),
            step: parseFloat($(this).attr('step')),
            value: parseFloat($control.text()),
            slide: function (event, ui) {
                $control.text(ui.value);
                setFormData($control);
            }
        });
    });
    $('.control-spinner').spinner({
        spin: function (event, ui) {
            getFormData($(this))[this.id] = ui.value;
        }
    }).css('width', function () {
        return $(this).attr('data-width');
    });
    $('.legend').each(function (e) {
        if ($(this).find('[class^="ui-icon ui-icon-carat-1-"]').length === 0) {
            $(this).prepend($('<span/>').addClass('ui-icon ui-icon-carat-1-s'));
        }
    });
    $('.status').each(function () {
        $(this).addClass($(this).find('option:selected').attr('data-class'));
    });
    $('.control-markdown').each(function () {
        var $viewer = $('[id="' + this.id + '.viewer"]');
        $viewer.html('' + getMarkedUp($(this).val()) + '');
        resizeEditor($(this), $viewer);
    });
    $('.markup').each(function () {
        $(this).html('' + getMarkedUp($(this).html(), true) + '');
        $(this).removeClass('markup');
    });
}