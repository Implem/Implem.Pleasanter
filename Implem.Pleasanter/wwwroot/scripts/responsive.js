$p.openResponsiveMenu = function () {
    var t = document.getElementById('Navigations');
    var t2 = document.getElementById('navtgl');
    const body = $('body');
    const header = $('#Header');
    if (t.classList.contains('open') == true) {
        t.classList.remove('open');
        t2.classList.remove('on');
        body.removeClass('is-showMenu');
        if ($('.bg-overlay').length) {
            $('.bg-overlay').remove();
        }
    } else {
        t.classList.add('open');
        t2.classList.add('on');
        body.addClass('is-showMenu');
        if (!$('.bg-overlay').length) {
            header.append(`<div class='bg-overlay'></div>`);
        }
        $('.bg-overlay').css({
            'display': 'block'
        });

        if ($('.bg-overlay').length) {
            $('.bg-overlay').on('click', function () {
                $p.openResponsiveMenu();
            });
        }
    }
}

$(document).ready(function () {
    if ($p.responsive() && screen.width < 768) {
        const heightHeader = $('#Header').length > 0 ? $('#Header').height() : 100;

        $('#Application').css({
            'padding-top': `${heightHeader}px`
        });

        $(window).scroll(function () {
            // Get the current scroll position
            const scrollPosition = $(this).scrollTop();

            if (scrollPosition === 0) {
                $('#Header').css({
                    'box-shadow': 'unset'
                });
            } else if (scrollPosition > heightHeader) {
                const headerDummyIndex = $('body thead .ui-widget-header');

                $('#Header').css({
                    'box-shadow': 'rgba(0, 0, 0, 0.19) 0px 10px 20px, rgba(0, 0, 0, 0.23) 0px 6px 6px'
                });

                if (window.location.pathname.includes("index")) {
                    $('body thead').css({
                        'top': `${heightHeader}px`
                    });

                    $(document).ajaxComplete(function () {
                        if (headerDummyIndex.length > 1) {
                            $(headerDummyIndex[1]).css({
                                'top': '0px',
                                'display': 'none'
                            });
                        } else {
                            $(headerDummyIndex[0]).css({
                                'top': '0px',
                                'display': 'none'
                            });
                        }
                    });

                }
            }
        });
    }
});

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

if (screen.width < 768) {
    $p.send($('#ReduceViewFilters'));
    $p.send($('#ReduceAggregations'));
}

$(document).ready(function () {
    function handleSMobileViewport() {
        let heightMainCommand;
        let heightFooter;

        if ($('#MainCommandsContainer').length > 0) {
            if ($('#Message').length > 0) {
                heightMainCommand = $('#MainCommandsContainer').height();
                heightFooter = $('#MainCommandsContainer').height();
                $(document).ajaxComplete(function () {
                    setTimeout(function () {
                        heightMainCommand = $('#MainCommandsContainer').height();
                        heightFooter = $('#MainCommandsContainer').height();
                        $('#Message').css(
                            'bottom', parseInt(heightMainCommand + heightFooter + 4)
                        );
                    }, 1);
                });

                $('#Message').css(
                    'bottom', parseInt(heightMainCommand + heightFooter + 4)
                );
            }
        }
    }

    // Initial check and event listener
    if ($p.responsive() && screen.width < 768) {
        handleSMobileViewport();
    }

    // Attach a listener for changes in viewport width
    window.addEventListener("resize", function () {
        if ($p.responsive()  && screen.width < 768) {
            handleSMobileViewport();
        }
    });
});

if ($p.responsive() && screen.width < 768) {
    $('#ViewModeContainer').on('scroll', function () {
        let scrollLeft = $(this).scrollLeft();
        if ($(this).scrollLeft() > 0) {
            $('body > thead').css({
                'left': `calc(5vw - ${scrollLeft}px)`
            });
        }
    });
}
