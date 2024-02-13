$p.drawAnaly = function () {
    const chartWidth = 550;
    const chartHeight = 550;
    const settingFontSize = 15;
    const settingFontHeight = 0;
    const radius = Math.min(chartWidth - 1, chartHeight - 5) / 2;
    const jsonData = JSON.parse($('#AnalyJson').val());
    const columnDataArray = JSON.parse($('#Columns').val());
    var colorLabelMemorys = [];
    for (var pieChart of jsonData) {
        var div = d3
            .select('#AnalyBody')
            .append('div')
            .attr('id', 'ChartDiv_' + pieChart.Setting.Id)
            .style('float', 'left')
            .on('mouseover', function () {
                var id = $(this).attr('id');
                id = id.substring(9);
                id = 'DeleteChartIcon_' + id;
                document.getElementById(id).style.visibility = 'visible';
            })
            .on('mouseout', function () {
                var id = $(this).attr('id');
                id = id.substring(9);
                id = 'DeleteChartIcon_' + id;
                document.getElementById(id).style.visibility = 'hidden';
            });
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
        var svg = div
            .append('svg')
            .attr('id', 'ChartSvg_' + pieChart.Setting.Id)
            .style('margin-left', '20px')
            .style('margin-right', '20px')
            .style('width', chartWidth)
            .style('height', chartHeight);
        var g = svg
            .append('g')
            .attr('id', 'DeleteAnalyPart_' + pieChart.Setting.Id)
            .attr('data-method', 'post')
            .attr(
                'transform',
                'translate(' + ((chartWidth / 2) + 1) + ',' + ((chartHeight / 2) + 1) + ')');
        if (!pieChart.Elements.length) {
            notIndicateAnalyChart('NoData');
        } else {
            var results = pieChart.Elements.filter(element => element.Value > 0);
            if (!results.length) {
                notIndicateAnalyChart('InvalidRequest');
            } else {
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', settingFontSize + 10)
                    .attr('text-anchor', 'middle')
                    .attr('dy', settingFontHeight - 25)
                    .text($p.display(pieChart.Setting.AggregationType));
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
                g.append('text')
                    .attr('fill', 'black')
                    .attr('font-size', settingFontSize)
                    .attr('text-anchor', 'middle')
                    .attr('dy', settingFontHeight + 50)
                    .text(function () {
                        if (pieChart.Setting.GroupBy === 'Creator') {
                            return $p.display('Creator');
                        } else {
                            var targetColumnData = columnDataArray.filter(function (columnData) {
                                return columnData.ColumnName === pieChart.Setting.GroupBy
                            });
                            return targetColumnData[0].LabelText;
                        }
                    });
                var pie = d3
                    .pie()
                    .value(function (d) {
                        return d.Value;
                    })
                    .sort(null);
                var pieGroup = g
                    .selectAll('.pie')
                    .data(pie(pieChart.Elements))
                    .enter()
                    .append('g')
                    .attr('class', 'pie');
                var arc = d3.arc().outerRadius(radius).innerRadius(130);
                var color = (pieChart.Elements.length <= 10)
                    ? d3.scaleOrdinal(d3.schemeCategory10)
                    : d3.scaleSequential(d3.interpolateRainbow).domain([0, 20]);
                if (!colorLabelMemorys.length) {
                    singleDrawAnalyChart();
                } else {
                    multiDrawAnalyChart();
                }
                var text = d3
                    .arc()
                    .outerRadius(radius - 50)
                    .innerRadius(radius - 40);
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

    function notIndicateAnalyChart(text) {
        g.append('text')
            .attr('fill', 'black')
            .attr('font-size', settingFontSize)
            .attr('text-anchor', 'middle')
            .attr('dy', settingFontHeight)
            .text($p.display(text));
    }

    function singleDrawAnalyChart() {
        pieGroup
            .append('path')
            .attr('d', arc)
            .attr('fill', function (d) {
                colorLabelMemorys.push({ labelName: d.data.GroupTitle, color: d.index });
                return color(d.index);
            })
            .attr('opacity', 0.85)
            .attr('stroke', 'white');
    }

    function multiDrawAnalyChart() {
        pieGroup
            .append('path')
            .attr('d', arc)
            .attr('fill', function (d) {
                var filterResults = colorLabelMemorys.find(colorLabelMemory => d.data.GroupTitle === colorLabelMemory.labelName);
                if (!filterResults) {
                    colorLabelMemorys.push({ labelName: d.data.GroupTitle, color: d.index });
                    return color(d.index);
                } else {
                    if (pieChart.Elements.length <= 10) {
                        if (d3.schemeCategory10[filterResults.color] === undefined) {
                            return color(d.index);
                        } else {
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
