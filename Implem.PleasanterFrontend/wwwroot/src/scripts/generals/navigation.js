$p.currentIndex = function (array) {
    return array.indexOf($('#Id').val());
};

$p.switchTargets = function () {
    var $control = $('#SwitchTargets');
    return $control.length === 1 ? $control.val().split(',') : [];
};

$p.setSwitchTargets = function () {
    var $control = $('#SwitchTargets');
    if ($control.length === 1) {
        $control.appendTo('body');
        $p.setCurrentIndex();
    }
};

$p.setCurrentIndex = function () {
    var array = $p.switchTargets();
    if (array.length > 1) {
        var index = $p.currentIndex(array);
        $('#CurrentIndex').text(index + 1 + '/' + array.length);
    } else {
        $('#Previous').hide();
        $('#CurrentIndex').hide();
        $('#Next').hide();
    }
};

$p.back = function () {
    var $control = $('#BackUrl');
    if ($control.length === 1) {
        $p.transition($control.val());
    }
};

$p.transition = function (url) {
    try {
        location.href = url;
    } catch (e) {
        if (e.number !== -2147467259) {
            throw e;
        }
    }
};

//SSOLoginボタン押下時のアクション。
//Formのdata-actionから取得したreturnUrlパラメータ付きのUrlでusers/challengeへリダイレクトする
$p.ssoLogin = function ($control) {
    $form = $control.closest('form');
    var action = $control.attr('data-action');
    var url = $form.attr('action').replace('_action_', action.toLowerCase());
    $p.transition(url);
};

$(function () {
    const basicConditions = 'a[href="#"][onclick]';
    const targetClasses = ['void-zero', 'menulabel'];
    const targetSelector = targetClasses
        .map(className => `${basicConditions}.${className}`)
        .join(', ');
    $(targetSelector).on('click', function (e) {
        e.preventDefault();
    });
});
