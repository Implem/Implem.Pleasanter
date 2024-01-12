$p.drawAnaly = function () {
    console.log('円グラフ描画処理：start');

    // 各変数の初期化
    const pieChartWidth = 400;
    const pieChartHeight = 400;
    var newX = 300;
    var newY = pieChartHeight;
    var conditionIllegalFlag = false;
    const radius = Math.min(480, 390) / 2;
    var colorLabelMemorys = [];

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
                id = 'deleteChartIcon_' + id;
                document.getElementById(id).style.visibility = 'visible';
            })
            .on("mouseout", function () {
                var id = $(this).attr('id');
                id = id.substring(9);
                id = 'deleteChartIcon_' + id;
                document.getElementById(id).style.visibility = 'hidden';
            });

        div
            .append('span')
            .attr('id', 'deleteChartIcon_' + pieChart.Setting.Id)
            .attr('class', 'ui-icon ui-icon-closethick')
            .attr('onclick', '$p.send($(\'#DeleteAnalyPart_' + pieChart.Setting.Id + '\'));')
            .style('margin-right', '50%')
            .style('visibility', 'hidden')
            .style('cursor', 'pointer')
            .style('float', 'right');

        var svg = div
            .append('svg')
            .attr('id', 'ChartSvg_' + pieChart.Setting.Id)
            .style('width', '600')
            .style('height', '500')
            .style('float', 'left');

        // Gタグを設定
        var g = svg
            .append('g')
            .attr('id', 'DeleteAnalyPart_' + pieChart.Setting.Id)
            .attr('data-method', 'post')
            .attr(
                'transform',
                'translate(270,195)');


        // データが抽出できない場合
        if (pieChart.Elements.length === 0) {
            // データがない旨画面に表示
            g.append('text')
                .attr('fill', 'black')
                .attr('font-size', '15px')
                .attr('text-anchor', 'middle')
                .attr('dy', 0)
                .text($p.display('NoData'));
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
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', '15px')
                    .attr('text-anchor', 'middle')
                    .attr('dy', 0)
                    .text($p.display('InvalidRequest'));

                // 要求が正しかった場合
            } else {

                // 条件「集計種別」を画面に表示
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', '25px')
                    .attr('text-anchor', 'middle')
                    .attr('dy', -30)
                    .text($p.display(pieChart.Setting.AggregationType));

                // 条件「値」と「期間」を画面に表示
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', '15px')
                    .attr('text-anchor', 'middle')
                    .attr('dy', 0)
                    .text(
                        $p.display(pieChart.Setting.TimePeriodValue) +
                        '' +
                        $p.display(pieChart.Setting.TimePeriod)
                    );

                // 条件「集計対象」を画面に表示
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', '15px')
                    .attr('text-anchor', 'middle')
                    .attr('dy', 30)
                    .text(function (d) {
                        for (var columnNames of JSON.parse($('#Columns').val())) {
                            if (columnNames.ColumnName === pieChart.Setting.AggregationTarget) {
                                return columnNames.LabelText;
                            }
                        }
                    });

                // 条件「項目」を画面に表示
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', '15px')
                    .attr('text-anchor', 'middle')
                    .attr('dy', 60)
                    .text(function (d) {
                        if (pieChart.Setting.GroupBy === 'Creator') {
                            return '作成者';
                        } else {
                            for (var columnNames of JSON.parse($('#Columns').val())) {
                                if (columnNames.ColumnName === pieChart.Setting.GroupBy) {
                                    return columnNames.LabelText;
                                }
                            }
                        }
                    })

                // 円グラフの各要素に対して色を設定
                var color = (pieChart.Elements.length <= 10)
                    ? d3.scaleOrdinal(d3.schemeCategory10)
                    : d3.scaleSequential(d3.interpolateRainbow).domain([0, 20]);

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
                var arc = d3.arc().outerRadius(radius).innerRadius(80);

                if (colorLabelMemorys.length === 0) {
                    drawSingleAnalyChart();
                } else {
                    drawMultiAnalyChart();
                }


                // 円グラフの各要素に設定するラベル位置を設定
                var text = d3
                    .arc()
                    .outerRadius(radius - 40)
                    .innerRadius(radius - 40);

                // 円グラフの各要素に対して条件「項目」に設定された値と条件「集計種別」で修正されたデータを設定
                pieGroup
                    .append('text')
                    .attr('fill', 'black')
                    .attr('transform', function (d) {
                        return 'translate(' + text.centroid(d) + ')';
                    })
                    .attr('dy', '20px')
                    .attr('font-size', '20px')
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


    function drawSingleAnalyChart() {
        console.log('単数円グラフ描画処理：start');

        // 円グラフの各要素の間にある余白を設定
        pieGroup
            .append('path')
            .attr('d', arc)
            .attr('fill', function (d) {
                colorLabelMemorys.push({ labelName: d.data.GroupTitle, color: d.index, flag: false });
                console.log('初期カラーを設定');
                console.log(d.data.GroupTitle);
                console.log(d.index);
                console.log(color(d.index));
                console.log(colorLabelMemorys);
                return color(d.index);
            })
            .attr('opacity', 0.85)
            .attr('stroke', 'white');

        console.log('単数円グラフ描画処理：end');
    }


    function drawMultiAnalyChart() {
        console.log('複数円グラフ描画処理：start');

        // メモリのラベル名とグラフのラベル名との比較処理
        for (var element of pieChart.Elements) {
            for (var colorLabelMemory of colorLabelMemorys) {
                if (element.GroupTitle.toString() === colorLabelMemory.labelName.toString()) {
                    colorLabelMemory.flag = true;
                    break;
                }
            }
        }

        // 円グラフの各要素の間にある余白を設定
        pieGroup
            .append('path')
            .attr('d', arc)
            .attr('fill', function (d) {
                console.log('d.data.GroupTitle：' + d.data.GroupTitle);
                console.log(colorLabelMemorys);
                for (var colorLabelMemory of colorLabelMemorys) {
                    console.log(colorLabelMemory.color);
                    if (colorLabelMemory.labelName == d.data.GroupTitle && colorLabelMemory.flag == true) {
                        console.log('メモリカラーを設定');
                        console.log(d.data.GroupTitle);
                        console.log(colorLabelMemory.labelName);
                        console.log(colorLabelMemory.color);
                        console.log(color(colorLabelMemory.color));
                        return color(colorLabelMemory.color);
                    }
                }
            })
            .attr('opacity', 0.85)
            .attr('stroke', 'white');

        console.log('複数円グラフ描画処理：end');
    }

    console.log('円グラフ描画処理：end');
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
