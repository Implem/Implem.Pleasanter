
$p.openResponsiveMenu = function () {
    var t = document.getElementById('Navigations');
    var t2 = document.getElementById('navtgl');
    if (t.classList.contains('open') == true) {
        t.classList.remove('open');
        t2.classList.remove('on');
    } else {
        t.classList.add('open');
        t2.classList.add('on');
    }
}

$p.switchResponsive = function ($control) {
    var redirect = 1;
    var data = {};
    data.Responsive = $control.data("action");
    $p.ajax(
        $('#ApplicationPath').val() + 'Resources/Responsive',
        'POST',
        data,
        undefined,
        redirect !== 1);
    if (redirect === 1) {
        $p.transition(location.href);
    }
}

var $toggleBtns = $('.sub-menu').children('div');
$.each($toggleBtns, function (i, el) {
    el.addEventListener('touchstart', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var $self = $(e.currentTarget);
        $self.toggleClass('is-open');
        $self.next().not(':animated').slideToggle(300);
    }, {
        passive: false
    });
});

if(screen.width < 981){
    $p.send($('#ReduceViewFilters'));
    $p.send($('#ReduceAggregations'));
}


