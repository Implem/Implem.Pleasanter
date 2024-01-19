$p.drawAnaly = function () {
    /*
    変数定義箇所
    ユーザによる変更可能
    */
    const chartWidth = 550;
    const chartHeight = 465;
    const settingFontSizeLarge = 25;
    const settingFontSizeSmall = 15;
    const settingFontHeight = 0;


    /*
    変数定義箇所
    ユーザによる変更不要
    */
    const radius = Math.min(chartWidth - 1, chartHeight - 5) / 2;
    var colorLabelMemorys = [];
    var conditionIllegalFlag = false;
    var colorIndex;
    var colorIndexFlag = false;


    /*
    ロジック定義箇所
    ユーザによる変更不要
    */
    // id=Analyを削除
    document.getElementById('Analy').remove();
    // サーバから返却されたデータをもとに反復処理
    for (var pieChart of JSON.parse($('#AnalyJson').val())) {
        // divタグを設定
        var div = d3
            .select('#TimeSeriesBody')
            .append('div')
            .attr('id', 'ChartDiv_' + pieChart.Setting.Id)
            .style('float', 'left')
            .on('mouseover', function () {
                var id = $(this).attr('id');
                id = id.substring(9);
                id = 'DeleteChartIcon_' + id;
                document.getElementById(id).style.visibility = 'visible';
            })
            .on("mouseout", function () {
                var id = $(this).attr('id');
                id = id.substring(9);
                id = 'DeleteChartIcon_' + id;
                document.getElementById(id).style.visibility = 'hidden';
            });
        // 画面上に「×」を表示
        div
            .append('span')
            .attr('id', 'DeleteChartIcon_' + pieChart.Setting.Id)
            .attr('class', 'material-symbols-outlined')
            .attr('onclick', '$p.send($(\'#DeleteAnalyPart_' + pieChart.Setting.Id + '\'));')
            .style('margin-top', '5%')
            .style('margin-right', '5%')
            .style('visibility', 'hidden')
            .style('cursor', 'pointer')
            .style('float', 'right')
            .text('close');
        // svgタグを設定
        var svg = div
            .append('svg')
            .attr('id', 'ChartSvg_' + pieChart.Setting.Id)
            .style('width', chartWidth)
            .style('height', chartHeight);
        // gタグを設定
        var g = svg
            .append('g')
            .attr('id', 'DeleteAnalyPart_' + pieChart.Setting.Id)
            .attr('data-method', 'post')
            .attr(
                'transform',
                'translate(' + ((chartWidth / 2) + 1) + ',' + ((chartHeight / 2) + 1) + ')');
        // データが抽出できない場合
        if (pieChart.Elements.length === 0) {
            // データがない旨画面に表示
            notIndicateAnalyChart('NoData');
            // データが抽出できた場合
        } else {
            // valueに「0」が設定されているかどうかの判定処理
            for (var element of pieChart.Elements) {
                // 要求が不正だった場合
                if (element.Value === 0) {
                    conditionIllegalFlag = true;
                    // 要求が正しかった場合
                } else {
                    conditionIllegalFlag = false;
                    break;
                }
            }
            // 要求が不正だった場合
            if (conditionIllegalFlag === true) {
                // 要求が不正だった旨画面に表示
                notIndicateAnalyChart('InvalidRequest');
                // 要求が正しかった場合
            } else {
                // 条件「集計種別」を画面に表示
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', settingFontSizeLarge)
                    .attr('text-anchor', 'middle')
                    .attr('dy', settingFontHeight - 25)
                    .text($p.display(pieChart.Setting.AggregationType));
                // 条件「値」を画面に表示
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', settingFontSizeSmall)
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
                    .attr('font-size', settingFontSizeSmall)
                    .attr('text-anchor', 'middle')
                    .attr('dy', settingFontHeight + 25)
                    .text(function () {
                        for (var columnName of JSON.parse($('#Columns').val())) {
                            if (columnName.ColumnName === pieChart.Setting.AggregationTarget) {
                                return columnName.LabelText;
                            }
                        }
                    });
                // 条件「項目」を画面に表示
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', settingFontSizeSmall)
                    .attr('text-anchor', 'middle')
                    .attr('dy', settingFontHeight + 50)
                    .text(function () {
                        if (pieChart.Setting.GroupBy === 'Creator') {
                            return '作成者';
                        } else {
                            for (var columnName of JSON.parse($('#Columns').val())) {
                                if (columnName.ColumnName === pieChart.Setting.GroupBy) {
                                    return columnName.LabelText;
                                }
                            }
                        }
                    })
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
                var arc = d3.arc().outerRadius(radius).innerRadius(100);
                // 初期値設定と円グラフ表示処理
                drawAnalyChart();
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
                        if (d.data.GroupTitle === '? ')
                            return $p.display('NotSet') + ',' + d.data.Value;
                        return d.data.GroupTitle + ',' + d.data.Value;
                    });
            }
        }
    };

    // データが取得できなかった際の画面表示処理
    function notIndicateAnalyChart(text) {
        g.append('text')
            .attr('fill', 'black')
            .attr('font-size', settingFontSizeSmall)
            .attr('text-anchor', 'middle')
            .attr('dy', settingFontHeight)
            .text($p.display(text));
    }

    // 初期値設定と円グラフ表示処理
    function drawAnalyChart() {
        // 円グラフの各要素に対して色を設定
        var color = (pieChart.Elements.length <= 10)
            ? d3.scaleOrdinal(d3.schemeCategory10)
            : d3.scaleSequential(d3.interpolateRainbow).domain([0, 20]);
        // 円グラフの各要素を設定
        pieGroup
            .append('path')
            .attr('d', arc)
            .attr('fill', function (d) {
                // 単数円グラフの表示処理
                if (colorLabelMemorys.length === 0) {
                    colorLabelMemorys.push({ labelName: d.data.GroupTitle, color: d.index });
                    return color(d.index);
                }
                // 複数円グラフの表示処理
                for (var colorLabelMemory of colorLabelMemorys) {
                    // すでにカラーが割り振られたラベル名が存在する場合
                    if (colorLabelMemory.labelName === d.data.GroupTitle) {
                        colorIndex = colorLabelMemory.color;
                        colorIndexFlag = true;
                        break;
                        // カラーラベルが割り振られていない場合
                    } else {
                        colorIndex = d.index;
                    }
                }
                // メモリにラベル名と割り振られたカラーを設定
                if (colorIndexFlag === false) colorLabelMemorys.push({ labelName: d.data.GroupTitle, color: d.index });
                // 要素数が10以下の場合、10色のカラーセットでカラーを設定
                if (pieChart.Elements.length <= 10) {
                    // すでに11色のカラーセットで割り振られ、今回10色のカラーセットでカラーを設定する
                    if (d3.schemeCategory10[colorIndex] === undefined) return d3.schemeCategory10[d.index];
                    return d3.schemeCategory10[colorIndex];
                    // 要素数が11色以上の場合、20色のカラーセットでカラーを設定する
                } else {
                    return color(colorIndex);
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
