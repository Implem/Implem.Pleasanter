$p.drawAnaly = function () {
    // データが空でない場合にグラフを描画
    const pieChartWidth = 300;
    const pieChartHeight = 300;
    const newX = 200;
    const newY = pieChartHeight;
    var conditionIllegalFlag = false;
    const radius = Math.min(pieChartWidth, pieChartHeight) / 2 - 10;

    // サーバから返却されたデータをもとに反復処理
    for (let pieChart of JSON.parse($('#AnalyJson').val())) {

        // SVGタグを設定
        var svg = d3
            .select('#TimeSeriesBody')
            .append('svg')
            .attr('id', 'Chart_' + pieChart.Setting.Id)
            .style('width', newX * 2)
            .style('height', newY)
            .style("float", "left");

        // Gタグを設定
        var g = svg
            .append('g')
            .attr('id', 'DeleteAnalyPart_' + pieChart.Setting.Id)
            .attr('data-method', 'post')
            .attr(
                'transform',
                'translate(' + newX + ',' + newY / 2 + ')')
            .style('width', 100)
            .style('height', 100)
            .style('float', 'left');

        // Gタグに対して削除ボタンを追加
        g
            .append('text')
            .attr('fill', 'black')
            .attr('font-size', '15px')
            .attr('text-anchor', 'middle')
            .attr('dx', -130)
            .attr('dy', -130)
            .attr('class', 'delete-analy')
            .attr('onclick', '$p.send($(\'#DeleteAnalyPart_' + pieChart.Setting.Id + '\'));')
            .text($p.display('Delete'));

        // データが抽出できない場合
        if (pieChart.Elements.length === 0) {
            // データがない旨画面に表示
            g.append('text')
                .attr('fill', 'black')
                .attr('font-size', '15px')
                .attr('text-anchor', 'middle')
                .attr('dy', 0)
                .text($p.display('There is no applicable data.'));

            // データが抽出できた場合
        } else {

            // valueに「0」が設定されているかどうかの判定処理
            for (let element of pieChart.Elements) {

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
                    .text($p.display('Invalid request has been sent.'));

                // 要求が正しかった場合
            } else {

                // 条件「集計種別」を画面に表示
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', '25px')
                    .attr('text-anchor', 'middle')
                    .attr('dy', -20)
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
                    .attr('dy', 20)
                    .text(function (d) {
                        for (let columnNames of JSON.parse($('#Columns').val())) {
                            if (columnNames.ColumnName === pieChart.Setting.AggregationTarget) {
                                return columnNames.LabelText;
                                break;
                            }
                        }
                    });

                // 条件「項目」を画面に表示
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', '15px')
                    .attr('text-anchor', 'middle')
                    .attr('dy', 40)
                    .text(function (d) {
                        if (pieChart.Setting.GroupBy === 'Creator') {
                            return '作成者';
                        } else {
                            for (let columnNames of JSON.parse($('#Columns').val())) {
                                if (columnNames.ColumnName === pieChart.Setting.GroupBy) {
                                    return columnNames.LabelText;
                                    break;
                                }
                            }
                        }
                    })

                // 円グラフの各要素に対して色を設定
                var color = d3
                    .scaleOrdinal()
                    .range([
                        '#DC3912',
                        '#3366CC',
                        '#109618',
                        '#FF9900',
                        '#990099',
                    ]);

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
                    // idをpieに変更
                    .attr('class', 'pie');

                // 円グラフの半径を設定
                var arc = d3.arc().outerRadius(radius).innerRadius(75);

                // 円グラフの各要素の間にある余白を設定
                pieGroup
                    .append('path')
                    .attr('d', arc)
                    .attr('fill', function (d) {
                        return color(d.index);
                    })
                    .attr('opacity', 0.85)
                    .attr('stroke', 'white');

                // 円グラフの各要素に設定するラベル位置を設定
                var text = d3
                    .arc()
                    .outerRadius(radius - 30)
                    .innerRadius(radius - 40);

                // 円グラフの各要素に対して条件「項目」に設定された値と条件「集計種別」で修正されたデータを設定
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
                            return $p.display('(Not set)') + ',' + d.data.Value;
                        return d.data.GroupTitle + ',' + d.data.Value;
                    });
            }
        }
    }
};


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
