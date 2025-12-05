$(function () {
    // metaタグからCSRFトークンを取得
    var $csrfToken = $('meta[name="csrf-token"]');
    // metaタグが存在する場合のみ、jQueryのAJAXグローバル設定を行う
    if ($csrfToken.length > 0) {
        $.ajaxSetup({
            // すべてのAJAXリクエストが送信される直前に実行される関数に設定
            beforeSend: function (xhr, settings) {
                // クロスドメインのリクエストでなければ、AJAXリクエストにトークンを送る
                if (!settings.crossDomain) {
                    xhr.setRequestHeader('X-CSRF-TOKEN', $csrfToken.attr('content'));
                }
            }
        });
    }
});
$p.ajax = function (url, methodType, data, $control, _async, clearMessage) {
    // CAPTCHA前処理チェック
    var captchaStatus = $p.captcha.prepareData(methodType, data);

    // CAPTCHAトークンが未取得の場合の処理
    if (!captchaStatus.ready) {
        var typeLower = captchaStatus.captchaType.toLowerCase();
        if (typeLower === 'recaptchav2' || typeLower === 'turnstile') {
            // v2/Turnstile: ユーザーの手動操作が必要
            return $p.captcha.handleMissingToken(captchaStatus.captchaType);
        }

        var siteIdVal = $('#SiteId').length ? String($('#SiteId').val()) : '';
        var dataAction = $control && $control.attr ? $control.attr('data-action') : '';
        var action = siteIdVal + '_' + dataAction;
        // v3: プログラムで自動取得してからAJAX実行（再帰呼び出し）
        $p.captcha.getToken(
            captchaStatus.captchaType,
            captchaStatus.siteKey,
            action,
            function (token, fieldName) {
                if (token) {
                    var updatedData = captchaStatus.data || {};
                    updatedData[fieldName] = token;
                    // トークン取得後に再度$p.ajax呼び出し
                    $p.ajax(url, methodType, updatedData, $control, _async, clearMessage);
                }
            }
        );
        return;
    }

    // === AJAX処理実行（CAPTCHA準備完了時） ===
    data = captchaStatus.data;

    if ($p.before_send($p.eventArgs(url, methodType, data, $control, _async)) === false) {
        return false;
    }
    if ($control) {
        var _confirm = $control.attr('data-confirm');
        if (_confirm !== undefined) {
            if (!confirm($p.display(_confirm))) {
                return false;
            }
        }
    }
    $p.loading($control);
    var ret = 0;
    _async = _async !== undefined ? _async : true;
    if (clearMessage !== false) {
        $p.clearMessage();
    }
    $p.execEvents(
        'ajax_before_send',
        $p.eventArgs(url, methodType, data, $control, _async, ret, null)
    );
    if ($('#Token').length) {
        if (!data) {
            data = {};
        }
        data.Token = $('#Token').val();
    }
    $.ajax({
        url: url,
        type: methodType,
        async: _async,
        cache: false,
        data: data,
        dataType: 'json'
    })
        .done(function (json, textStatus, jqXHR) {
            $p.execEvents(
                'ajax_before_done',
                $p.eventArgs(url, methodType, data, $control, _async, ret, json)
            );
            $p.setByJson(url, methodType, data, $control, _async, json);
            ret =
                json.filter(function (i) {
                    return i.Method === 'Message' && JSON.parse(i.Value).Css === 'alert-error';
                }).length !== 0
                    ? -1
                    : 0;
            $p.execEvents(
                'ajax_after_done',
                $p.eventArgs(url, methodType, data, $control, _async, ret, json)
            );
            if (url.indexOf('authenticate') !== -1) {
                $p.showQr();
                $p.authenticatebymail();
            }
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            ret = -1;
            if (!jqXHR.getAllResponseHeaders()) {
                return;
            }
            if (jqXHR.status === 400) {
                alert($p.display('BadRequest'));
            } else if (jqXHR.status === 403) {
                alert($p.display('UnauthorizedRequest'));
            } else {
                $p.execEvents(
                    'ajax_before_fail',
                    $p.eventArgs(url, methodType, data, $control, _async, ret, null)
                );
                if (!$p.setServerErrorMessage(jqXHR.responseJSON)) {
                    alert(
                        (
                            jqXHR.status +
                            '\n' +
                            textStatus +
                            '\n' +
                            JSON.parse(jqXHR.responseJSON[0].Value).Text
                        )
                            .trim()
                            .replace('\n', '')
                    );
                }
                $p.execEvents(
                    'ajax_after_fail',
                    $p.eventArgs(url, methodType, data, $control, _async, ret, null)
                );
            }
        })
        .always(function (jqXHR, textStatus) {
            $p.execEvents(
                'ajax_before_always',
                $p.eventArgs(url, methodType, data, $control, _async, ret, null)
            );
            $p.clearData('ControlId', data);
            $p.loaded();
            $p.execEvents(
                'ajax_after_always',
                $p.eventArgs(url, methodType, data, $control, _async, ret, null)
            );
        });
    $p.execEvents(
        'ajax_after_send',
        $p.eventArgs(url, methodType, data, $control, _async, ret, null)
    );
    $p.after_send($p.eventArgs(url, methodType, data, $control, _async, ret));
    return ret;
};

$p.multiUpload = function (url, data, $control, statusBar, callback) {
    $p.loading($control);
    $p.clearMessage();
    var methodType = 'post';
    if ($('#Token').length === 1) {
        data.append('Token', $('#Token').val());
    }
    var uploader = $.ajax({
        xhr: function () {
            var uploadobj = $.ajaxSettings.xhr();
            if (uploadobj.upload) {
                uploadobj.upload.addEventListener(
                    'progress',
                    function (event) {
                        var percent = 0;
                        var position = event.loaded || event.position;
                        var total = event.total;
                        if (event.lengthComputable) {
                            percent = Math.ceil((position / total) * 100);
                        }
                        if (statusBar !== undefined) {
                            statusBar.setProgress(percent);
                        }
                    },
                    false
                );
            }
            return uploadobj;
        },
        url: url,
        type: methodType,
        contentType: false,
        processData: false,
        cache: false,
        data: data,
        success: function (data) {
            if (statusBar !== undefined) {
                statusBar.setProgress(100);
            }
        }
    })
        .done(function (json, textStatus, jqXHR) {
            if (callback) {
                callback(json);
            } else {
                $p.setByJson(url, methodType, data, $control, true, JSON.parse(json));
            }
            return true;
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status === 400) {
                alert($p.display('BadRequest'));
            } else if (jqXHR.status === 403) {
                alert($p.display('UnauthorizedRequest'));
            } else {
                alert(textStatus + '\n' + $(jqXHR.responseText).text().trim().replace('\n', ''));
            }
            return false;
        })
        .always(function (jqXHR, textStatus) {
            $p.clearData('ControlId', data);
            $p.loaded();
            if (statusBar !== undefined) {
                statusBar.status.hide();
            }
        });
    if (statusBar !== undefined) {
        statusBar.setAbort(uploader);
    }
};

$p.captcha = {
    // CAPTCHA設定の取得
    getConfig: function () {
        return {
            type: $('meta[name="captcha-type"]').attr('content') || '',
            siteKey: $('meta[name="captcha-site-key"]').attr('content') || ''
        };
    },

    // CAPTCHAが必要かチェック
    isRequired: function (methodType) {
        var config = this.getConfig();
        return (
            methodType &&
            methodType.toLowerCase() !== 'get' &&
            config.type &&
            config.type.toLowerCase() !== 'none' &&
            config.siteKey
        );
    },

    // ページ内ウィジェットからトークンを data にマージ
    mergeDomTokens: function (data, captchaType) {
        data = data || {};
        switch ((captchaType || '').toLowerCase()) {
            case 'recaptchav2': {
                var v2 = ($('#g-recaptcha-response').val() || '').trim();
                if (v2) data['g-recaptcha-response'] = v2;
                break;
            }
            case 'turnstile': {
                // Turnstile は hidden input が自動追加される
                var ts = ($('input[name="cf-turnstile-response"]').val() || '').trim();
                if (ts) data['cf-turnstile-response'] = ts;
                break;
            }
        }
        return data;
    },

    // CAPTCHAトークンの有無をチェック（DOM も見る）
    hasToken: function (data, captchaType) {
        var type = (captchaType || '').toLowerCase();
        data = data || {};
        switch (type) {
            case 'recaptchav3':
                return !!data['g-recaptcha-response'];
            case 'recaptchav2':
                if (data['g-recaptcha-response']) return true;
                return !!($('#g-recaptcha-response').val() || '').trim();
            case 'turnstile':
                if (data['cf-turnstile-response']) return true;
                return !!($('input[name="cf-turnstile-response"]').val() || '').trim();
            default:
                return true;
        }
    },

    // トークン未取得時のUI処理（v2/Turnstile）
    handleMissingToken: function (captchaType) {
        var typeLower = captchaType.toLowerCase();
        if (typeLower === 'recaptchav2' || typeLower === 'turnstile') {
            var $widget =
                typeLower === 'recaptchav2'
                    ? $('#RecaptchaV2Widget, .g-recaptcha').first()
                    : $('#TurnstileWidget, .cf-turnstile').first();

            if ($widget.length) {
                try {
                    $('html, body').animate({ scrollTop: $widget.offset().top - 80 }, 200);
                } catch (e) {}
            }

            $p.setErrorMessage('CaptchaTokenMissing');
            return false;
        }
        return true;
    },

    // CAPTCHAトークン取得処理
    getToken: function (captchaType, siteKey, action, callback) {
        switch (captchaType.toLowerCase()) {
            case 'recaptchav3':
                // v3: 見えないCAPTCHA
                if (typeof grecaptcha !== 'undefined') {
                    try {
                        grecaptcha.ready(function () {
                            try {
                                grecaptcha
                                    .execute(siteKey, { action: action })
                                    .then(function (token) {
                                        callback(token, 'g-recaptcha-response');
                                    })
                                    .catch(function (error) {
                                        // 非同期エラーハンドリング
                                        console.error('reCAPTCHA v3 error (async):', error);
                                        $p.setErrorMessage('CaptchaTokenMissing');
                                        $p.loaded();
                                        callback(null, null);
                                    });
                            } catch (error) {
                                // 同期エラーハンドリング（execute内部）
                                console.error('reCAPTCHA v3 error (sync in ready):', error);
                                $p.setErrorMessage('CaptchaTokenMissing');
                                $p.loaded();
                                callback(null, null);
                            }
                        });
                    } catch (error) {
                        // 同期エラーハンドリング（ready呼び出し）
                        console.error('reCAPTCHA v3 error (sync):', error);
                        $p.setErrorMessage('CaptchaTokenMissing');
                        $p.loaded();
                        callback(null, null);
                    }
                } else {
                    console.error('reCAPTCHA v3 library not loaded');
                    $p.setErrorMessage('CaptchaLibraryNotLoaded');
                    $p.loaded();
                    callback(null, null);
                }
                break;

            default:
                console.warn('Unknown CAPTCHA type: ' + captchaType);
                callback(null, null);
        }
    },

    // CAPTCHA前処理：必要に応じてデータにトークンを追加
    prepareData: function (methodType, data) {
        var config = this.getConfig();

        // CAPTCHAが不要な場合はそのまま返す
        if (!this.isRequired(methodType)) {
            return { ready: true, data: data };
        }

        // ページ内ウィジェットのトークンを data に取り込む（v2/Turnstile）
        data = this.mergeDomTokens(data, config.type);

        // トークンが既に取得済みの場合は準備完了
        if (this.hasToken(data, config.type)) {
            return { ready: true, data: data };
        }

        // トークンが未取得の場合
        return {
            ready: false,
            data: data,
            captchaType: config.type,
            siteKey: config.siteKey
        };
    }
};
