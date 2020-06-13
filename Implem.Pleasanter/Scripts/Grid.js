$p.setGrid = function () {
    $p.paging('#Grid');
}

$p.openEditorDialog = function (id) {
    $p.data.DialogEditorForm = {};
    $('#EditorDialog').dialog({
        modal: true,
        width: '90%',
        resizable: false,
        open: function () {
            $('#EditorLoading').val(0);
            $p.initRelatingColumn();
        }
    });
}

$p.editOnGrid = function ($control, val) {
    if (val === 0) {
        if (!$p.confirmReload()) return false;
    }
    $('#EditOnGrid').val(val);
    $p.send($control);
}

$p.newOnGrid = function ($control) {
    $p.send($control, 'MainForm');
    $(window).scrollTop(0);
}

$p.copyRow = function ($control) {
    $p.getData($control).OriginalId = $control.closest('.grid-row').attr('data-id');
    $p.send($control);
    $(window).scrollTop(0);
}

$p.cancelNewRow = function ($control) {
    $p.getData($control).CancelRowId = $control.closest('.grid-row').attr('data-id');
    $p.send($control);
}

$(function () {
    if ($("#Grid").length == 0) return;
    var TBLHD = TBLHD || {};
    TBLHD.$clone = null;
    TBLHD.view = 0;
    TBLHD.StyleCopy = function ($copyTo, $copyFrom) {
        $copyTo.css("width",
            $copyFrom.css("width"));
        $copyTo.css("height",
            $copyFrom.css("height"));
        $copyTo.css("padding-top",
            $copyFrom.css("padding-top"));
        $copyTo.css("padding-left",
            $copyFrom.css("padding-left"));
        $copyTo.css("padding-bottom",
            $copyFrom.css("padding-bottom"));
        $copyTo.css("padding-right",
            $copyFrom.css("padding-right"));
        $copyTo.css("background",
            $copyFrom.css("background"));
        $copyTo.css("background-color",
            $copyFrom.css("background-color"));
        $copyTo.css("vertical-align",
            $copyFrom.css("vertical-align"));
        $copyTo.css("border-top-width",
            $copyFrom.css("border-top-width"));
        $copyTo.css("border-top-color",
            $copyFrom.css("border-top-color"));
        $copyTo.css("border-top-style",
            $copyFrom.css("border-top-style"));
        $copyTo.css("border-left-width",
            $copyFrom.css("border-left-width"));
        $copyTo.css("border-left-color",
            $copyFrom.css("border-left-color"));
        $copyTo.css("border-left-style",
            $copyFrom.css("border-left-style"));
        $copyTo.css("border-right-width",
            $copyFrom.css("border-right-width"));
        $copyTo.css("border-right-color",
            $copyFrom.css("border-right-color"));
        $copyTo.css("border-right-style",
            $copyFrom.css("border-right-style"));
        $copyTo.css("border-bottom-width",
            $copyFrom.css("border-bottom-width"));
        $copyTo.css("border-bottom-color",
            $copyFrom.css("border-bottom-color"));
        $copyTo.css("border-bottom-style",
            $copyFrom.css("border-bottom-style"));
    }
    TBLHD.cssCopy = function ($clone, $thead) {
        TBLHD.StyleCopy($clone, $thead);
        for (var i = 0; i < $thead.children("tr").length; i++) {
            var $theadtr = $thead.children("tr").eq(i);
            var $clonetr = $clone.children("tr").eq(i);
            for (var j = 0; j < $theadtr.eq(i).children("th").length; j++) {
                var $theadth = $theadtr.eq(i).children("th").eq(j);
                var $cloneth = $clonetr.eq(i).children("th").eq(j);
                TBLHD.StyleCopy($cloneth, $theadth);
            }
        }
    }
    TBLHD.createClone = function () {
        if (TBLHD.$clone != null) {
            TBLHD.$clone.remove();
        }
        TBLHD.$table = $("#Grid");
        TBLHD.$thead = TBLHD.$table.children("thead");
        TBLHD.$table.children("thead:empty").remove();
        TBLHD.$table.children("tbody:empty").remove();
        TBLHD.$clone = TBLHD.$thead.clone(true);
        let events = $._data($(document).get(0), "events");
        let evs = {};
        for (let p in events) {
            let $es = $(events).prop(p);
            for (let ind in $es) {
                let $e = $es[ind];
                if (String($e.selector).indexOf('#') === 0) {
                    evs[String($e.selector).slice(1)] = String($e.type);
                }
            }
        }
        let $ids = TBLHD.$clone.find('[id]');
        for (var i = 0; i < $ids.length; i++) {
            let $i = $($ids[i]);
            let id = $i.attr('id');
            if (evs[id]) {
                $i.on(evs[id], function () {
                    $('#' + id).trigger(evs[id]);
                });
            }
        }
        TBLHD.$clone.appendTo("body");
        if (TBLHD.view == 0) {
            TBLHD.$clone.css("display", "none");
        } else {
            TBLHD.$clone.css("display", "table");
        }
        TBLHD.$clone.css("position", "fixed");
        TBLHD.$clone.css("border-collapse", "collapse");
        TBLHD.$clone.css("top", "0px");
        TBLHD.$clone.css("z-index", 99);
        TBLHD.$clone.css("left", TBLHD.$table.offset().left - $(window).scrollLeft());
        TBLHD.cssCopy(TBLHD.$clone, TBLHD.$thead);
    }
    TBLHD.observer = new MutationObserver(function (records) {
        if (records.some(function (value) {
            return value.target.className === 'menu-sort'
                || value.target.tagName === 'TH'
        })) { return; }
        TBLHD.createClone();
    });
    TBLHD.observer.observe(
        document.getElementById("ViewModeContainer"),
        {
            attributes: true,
            attributeOldValue: true,
            characterData: true,
            characterDataOldValue: true,
            childList: true,
            subtree: true
        }
    );
    TBLHD.createClone();
    $(window).resize(function () {
        if (TBLHD.$clone != null) {
            TBLHD.cssCopy(TBLHD.$clone, TBLHD.$thead);
        }
    });
    $(window).scroll(function () {
        let toffset = TBLHD.$table.offset();
        if (toffset.top + TBLHD.$table.height() < $(window).scrollTop()
            || toffset.top > $(window).scrollTop()) {
            if (TBLHD.view == 1) {
                TBLHD.view = 0;
                TBLHD.$clone.css("display", "none");
            }
        }
        else if (toffset.top < $(window).scrollTop()) {
            if (TBLHD.view == 0) {
                TBLHD.createClone();
                TBLHD.view = 1;
                TBLHD.$clone.css("display", "table");
            }
            TBLHD.$clone.css("left", toffset.left - $(window).scrollLeft());
        }
    });
});