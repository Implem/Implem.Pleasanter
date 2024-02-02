$p.drawAnaly = function () {
    // 定数定義箇所
    // ユーザによる変更可能
    const chartWidth = 550;
    const chartHeight = 550;
    const settingFontSize = 15;
    const settingFontHeight = 0;
    const radius = Math.min(chartWidth - 1, chartHeight - 5) / 2;
    const jsonData = JSON.parse($('#AnalyJson').val());
    const columnDataArray = JSON.parse($('#Columns').val());
    // 変数定義箇所
    // ユーザによる変更不要
    var colorLabelMemorys = [];
    // ロジック定義箇所
    // ユーザによる変更不要
    // サーバから返却されたデータをもとに反復処理
    for (var pieChart of jsonData) {
        // svgタグ上に削除機能を実装した×ボタンを表示させることができなかった。
        // そのため、divタグを実装し、divタグ上に×ボタンを実装するように実装。
        // そのため、divタグ上にsvgタグ、svgタグ上にgタグとなるように実装。
        // divタグを設定
        var div = d3
            .select('#AnalyBody')
            .append('div')
            .attr('id', 'ChartDiv_' + pieChart.Setting.Id)
            .style('float', 'left')
            .on('mouseover', function () {
                // 画面上に「×」を表示
                var id = $(this).attr('id');
                id = id.substring(9);
                id = 'DeleteChartIcon_' + id;
                document.getElementById(id).style.visibility = 'visible';
            })
            .on('mouseout', function () {
                // 画面上の「×」を非表示
                var id = $(this).attr('id');
                id = id.substring(9);
                id = 'DeleteChartIcon_' + id;
                document.getElementById(id).style.visibility = 'hidden';
            });
        // 削除機能を実装した×ボタン要素を実装
        div
            .append('span')
            .attr('id', 'DeleteChartIcon_' + pieChart.Setting.Id)
            .attr('class', 'material-symbols-outlined')
            .attr('onclick', '$p.send($(\'#DeleteAnalyPart_' + pieChart.Setting.Id + '\'));')
            .style('margin-top', '5%')
            .style('margin-right', '5%')
            .style('cursor', 'pointer')
            .style('float', 'right')
            .style('visibility', 'hidden')
            .text('close');
        // svgタグを設定
        var svg = div
            .append('svg')
            .attr('id', 'ChartSvg_' + pieChart.Setting.Id)
            .style('margin-left', '20px')
            .style('margin-right', '20px')
            .style('width', chartWidth)
            .style('height', chartHeight);
        // gタグを設定(このgタグ上に円グラフを実装する)
        var g = svg
            .append('g')
            .attr('id', 'DeleteAnalyPart_' + pieChart.Setting.Id)
            .attr('data-method', 'post')
            .attr(
                'transform',
                'translate(' + ((chartWidth / 2) + 1) + ',' + ((chartHeight / 2) + 1) + ')');
        // データが抽出できない場合
        if (!pieChart.Elements.length) {
            // データがない旨画面に表示
            notIndicateAnalyChart('NoData');
            // データが抽出できた場合
        } else {
            // 取得したデータのうち Value が 0 より上のデータを設定
            var results = pieChart.Elements.filter(element => element.Value > 0);
            // 要求が不正だった場合
            // データは取得できたが Value が 0 より上のデータが存在しなかった場合(リクエストが不正だった場合)
            if (!results.length) {
                // 要求が不正だった旨画面に表示
                notIndicateAnalyChart('InvalidRequest');
                // 要求が正しかった場合
            } else {
                // 条件「集計種別」を画面に表示
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', settingFontSize + 10)
                    .attr('text-anchor', 'middle')
                    .attr('dy', settingFontHeight - 25)
                    .text($p.display(pieChart.Setting.AggregationType));
                // 条件「値」を画面に表示
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', settingFontSize)
                    .attr('text-anchor', 'middle')
                    .attr('dy', settingFontHeight)
                    .text(
                        $p.display(pieChart.Setting.TimePeriodValue) +
                        '' +
                        $p.display(pieChart.Setting.TimePeriod)
                    );
                // 条件「集計対象」を画面に表示
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', settingFontSize)
                    .attr('text-anchor', 'middle')
                    .attr('dy', settingFontHeight + 25)
                    .text(function () {
                        if (!pieChart.Setting.AggregationTarget) return "";
                        var targetColumnData = columnDataArray.filter(function (columnData) {
                            return columnData.ColumnName === pieChart.Setting.AggregationTarget;
                        });
                        return targetColumnData[0].LabelText;
                    });
                // 条件「項目」を画面に表示
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', settingFontSize)
                    .attr('text-anchor', 'middle')
                    .attr('dy', settingFontHeight + 50)
                    .text(function () {
                        if (pieChart.Setting.GroupBy === 'Creator') {
                            return "作成者";
                        } else {
                            var targetColumnData = columnDataArray.filter(function (columnData) {
                                return columnData.ColumnName === pieChart.Setting.GroupBy
                            });
                            return targetColumnData[0].LabelText;
                        }
                    });
                // 円グラフの各要素に対してラベルを設定
                var pie = d3
                    .pie()
                    .value(function (d) {
                        return d.Value;
                    })
                    .sort(null);
                // 円グラフの各要素に対してデータを設定
                var pieGroup = g
                    // gのすべてのpie要素を選択
                    .selectAll('.pie')
                    .data(pie(pieChart.Elements))
                    .enter()
                    .append('g')
                    .attr('class', 'pie');
                // 円グラフの半径を設定
                var arc = d3.arc().outerRadius(radius).innerRadius(130);
                // 円グラフの各要素に対して色を設定
                var color = (pieChart.Elements.length <= 10)
                    ? d3.scaleOrdinal(d3.schemeCategory10)
                    : d3.scaleSequential(d3.interpolateRainbow).domain([0, 20]);
                // 初期値設定と円グラフ表示処理
                // メモリにデータが設定されていない場合
                if (!colorLabelMemorys.length) {
                    // 最初の円グラフを表示
                    singleDrawAnalyChart();
                } else {
                    // 2つ以上の円グラフを表示
                    multiDrawAnalyChart();
                }
                // 円グラフの各要素に設定するラベル位置を設定
                var text = d3
                    .arc()
                    .outerRadius(radius - 50)
                    .innerRadius(radius - 40);
                // 円グラフの各要素に条件「項目」の値と条件「集計種別」で集計されたデータを画面上に表示
                pieGroup
                    .append('text')
                    .attr('fill', 'black')
                    .attr('transform', function (d) {
                        return 'translate(' + text.centroid(d) + ')';
                    })
                    .attr('dy', '5px')
                    .attr('font-size', '13px')
                    .attr('text-anchor', 'middle')
                    .text(function (d) {
                        if (d.data.Value === 0) return;
                        return d.data.GroupTitle + ',' + d.data.Value;
                    });
            }
        }
    };

    // データが取得できなかった際の画面表示処理
    // 引数を「$p.display()」の引数に渡し、引数を表示
    // gタグの中心に表示
    function notIndicateAnalyChart(text) {
        g.append('text')
            .attr('fill', 'black')
            .attr('font-size', settingFontSize)
            .attr('text-anchor', 'middle')
            .attr('dy', settingFontHeight)
            .text($p.display(text));
    }

    // 単数円グラフ各要素の色設定とメモリ登録処理
    function singleDrawAnalyChart() {
        pieGroup
            .append('path')
            .attr('d', arc)
            .attr('fill', function (d) {
                // 初期値としてメモリに登録
                colorLabelMemorys.push({ labelName: d.data.GroupTitle, color: d.index });
                // 要素の色を設定
                return color(d.index);
            })
            .attr('opacity', 0.85)
            .attr('stroke', 'white');
    }

    // 複数円グラフ各要素の色設定とメモリ登録処理
    function multiDrawAnalyChart() {
        pieGroup
            .append('path')
            .attr('d', arc)
            .attr('fill', function (d) {
                // メモリに登録されているラベル名の情報を設定
                // メモリに登録されていないラベル名はnullが設定される
                var filterResults = colorLabelMemorys.find(colorLabelMemory => d.data.GroupTitle === colorLabelMemory.labelName);
                // ラベル名がメモリに登録されていない場合
                if (!filterResults) {
                    // 初期値としてメモリに登録
                    colorLabelMemorys.push({ labelName: d.data.GroupTitle, color: d.index });
                    // 要素の色を設定
                    return color(d.index);
                    // ラベル名がメモリに登録されている場合
                } else {
                    // 取得したデータ件数が10件以下の場合
                    // 10色のカラーセットで要素の色を設定
                    if (pieChart.Elements.length <= 10) {
                        // 10色のカラーセットで対応できない場合
                        // 初期値として11色以上のカラーセットで設定されていたが、10色のカラーセットでは対応できない場合
                        if (d3.schemeCategory10[filterResults.color] === undefined) {
                            // 要素の色を設定
                            return color(d.index);
                            // 10色のカラーセットで対応できる場合
                        } else {
                            // 要素の色を設定
                            return d3.schemeCategory10[filterResults.color];
                        }
                    } else {
                        return color(filterResults.color);
                    }
                }
            })
            .attr('opacity', 0.85)
            .attr('stroke', 'white');
    }
}

$p.openAnalyPartDialog = function ($control) {
    error = $p.syncSend($control);
    if (error === 0) {
        $('#AnalyPartDialog').dialog({
            modal: true,
            width: '420px',
            resizable: false,
        });
    }
};
