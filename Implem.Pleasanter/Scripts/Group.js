$p.setGroup = function ($control) {
    $('#CurrentMembers').find('.ui-selected').each(function () {
        var $this = $(this);
        var data = $this.attr('data-value').split(',');
        var type = $control.attr('id');
        $this.attr('data-value', data[0] + ',' + data[1] + ',' + (type === 'Manager'));
        $this.text($this.text().replace(/\(.*\)/, ''));
        $this.text($this.text() + (type === 'GeneralUser'
            ? ''
            : '(' + $p.display(type) + ')'));
    });
    $p.setData($('#CurrentMembers'));
}