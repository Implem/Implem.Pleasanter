$p.drawAnaly = function () {
    // データが空でない場合にグラフを描画
    var pieChartWidth = 300;
    var pieChartHeight = 300;
    var newX = 200;
    var newY = pieChartHeight;
    var screenWidth = window.innerWidth;
    var screenHeight = window.innerHeight; // 画面の幅
    var conditionIllegalFlag = false;

    var svg = d3
        .select('#Analy')
        .attr('width', screenWidth)
        .attr('height', screenHeight);
    for (let pieChart of JSON.parse($('#AnalyJson').val())) {
        var radius = Math.min(pieChartWidth, pieChartHeight) / 2 - 10;

        // 2回目以降の処理から+400ずつ
        // if (pieChart.Setting.Id !== 1) {
        //     newX = newX + 300;
        //     if (screenWidth >= newX) {
        //     } else {
        //         newX = 200;
        //         newY = newY + 600;
        //     }
        //     if (screenHeight >= newY) {
        //     } else {
        //         alert('データがいっぱいです。ログインしなおしてください。');
        //         return;
        //     }
        // }

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
        g
            .append('text')
            .attr('fill', 'black')
            .attr('font-size', '15px')
            .attr('text-anchor', 'middle')
            .attr('dx', -130)
            .attr('dy', -130)
            .attr('class', 'delete-analy')
            .attr('onclick', '$p.send($(\'#DeleteAnalyPart_' + pieChart.Setting.Id + '\'));')
            .text('delete');

        if (pieChart.Elements.length === 0) {
            // X軸とY軸を決めている
            g.append('text')
                .attr('fill', 'black')
                .attr('font-size', '15px')
                .attr('text-anchor', 'middle')
                .attr('dy', 0)
                .text($p.display('NoData'));
        } else {
            for (let element of pieChart.Elements) {
                if (element.Value === 0) {
                    conditionIllegalFlag = true;
                } else {
                    conditionIllegalFlag = false;
                    break;
                }
            }
            if (conditionIllegalFlag === true) {
                // X軸とY軸を決めている
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', '15px')
                    .attr('text-anchor', 'middle')
                    .attr('dy', 0)
                    .text($p.display('InvalidRequest'));
            } else {
                // X軸とY軸を決めている
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', '25px')
                    .attr('text-anchor', 'middle')
                    .attr('dy', -20)
                    .text($p.display(pieChart.Setting.AggregationType));
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

                var color = d3
                    .scaleOrdinal()
                    .range([
                        '#DC3912',
                        '#3366CC',
                        '#109618',
                        '#FF9900',
                        '#990099',
                    ]);

                var pie = d3
                    .pie()
                    .value(function (d) {
                        return d.Value;
                    })
                    .sort(null);

                var pieGroup = g
                    // gのすべてのpie要素を選択
                    .selectAll('.pie')
                    .data(pie(pieChart.Elements))
                    .enter()
                    .append('g')
                    // idをpieに変更
                    .attr('class', 'pie');

                var arc = d3.arc().outerRadius(radius).innerRadius(75);

                pieGroup
                    // pieGroupにpathを追加
                    .append('path')
                    .attr('d', arc)
                    .attr('fill', function (d) {
                        return color(d.index);
                    })
                    .attr('opacity', 0.85)
                    .attr('stroke', 'white');

                var text = d3
                    .arc()
                    .outerRadius(radius - 30)
                    .innerRadius(radius - 40);

                pieGroup
                    // pieGroupにtextを追加
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
