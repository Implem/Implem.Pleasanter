$p.drawTimeSeries = function () {
    $('#TimeSeriesValueField').toggle($('#TimeSeriesAggregateType').val() !== 'Count');
    var chartType = $('#TimeSeriesChartType').val();
    var $svg = $('#TimeSeries');
    if ($svg.length !== 1) {
        return;
    }
    $svg.empty();
    var json = JSON.parse($('#TimeSeriesJson').val());
    var indexes = json.Indexes;
    if (chartType === 'LineChart') {
        indexes.sort(function (a, b) {
            return b.LegendValue - a.LegendValue;
        });
    }
    var elements = json.Elements;
    if (elements.length === 0) {
        $svg.hide();
        return;
    }
    $svg.show();
    var svg = d3.select('#TimeSeries');
    var padding = 40;
    var axisPaddingX = 130;
    var axisPaddingY = 50;
    var width = parseInt(svg.style('width'));
    //折れ線チャートの場合は凡例を表示するためにグラフ右側の領域を確保する
    if (chartType === 'LineChart') {
        width = width - axisPaddingX;
    }
    var height = parseInt(svg.style('height'));
    var bodyWidth = width - axisPaddingX - padding;
    var bodyHeight = height - axisPaddingY - padding;
    var minDate = new Date(
        d3.min(elements, function (d) {
            return d.Day;
        })
    );
    var maxDate = new Date(
        d3.max(elements, function (d) {
            return d.Day;
        })
    );
    var dayWidth = (bodyWidth - padding) / $p.dateDiff('d', maxDate, minDate);
    var xScale = d3.scaleTime().domain([minDate, maxDate]).range([padding, bodyWidth]);
    var yScale = d3
        .scaleLinear()
        .domain([
            d3.max(elements, function (d) {
                return chartType === 'LineChart' ? d.Value : d.Y;
            }),
            0
        ])
        .range([padding, bodyHeight])
        .nice();
    var xAxis = d3.axisBottom(xScale).tickFormat(d3.timeFormat('%m/%d')).ticks(10);
    var yAxis = d3.axisLeft(yScale);
    //チャート数が10以下の場合は色の識別がしやすい10色のカラーセット、
    //そうでない場合はグラデーションの20色のカラーセットを利用する
    var colorScale =
        indexes.length <= 10
            ? d3.scaleOrdinal(d3.schemeCategory10)
            : d3.scaleSequential(d3.interpolateRainbow).domain([0, 20]);
    svg.append('g')
        .attr('class', 'axis')
        .attr('transform', 'translate(' + axisPaddingX + ', ' + (height - axisPaddingY) + ')')
        .call(xAxis)
        .selectAll('text')
        .attr('x', -20)
        .attr('y', 20)
        .style('text-anchor', 'start');
    svg.append('g')
        .attr('class', 'axis')
        .attr('transform', 'translate(' + axisPaddingX + ', 0)')
        .call(yAxis)
        .selectAll('text')
        .attr('x', -20);
    indexes.forEach(function (index) {
        var ds = elements.filter(function (d) {
            return d.Index === index.Id;
        });
        if (chartType === 'LineChart') {
            drawLine(ds, index.Id);
        } else {
            drawArea(ds);
        }
    });
    var lineCount = 0;
    indexes.forEach(function (index) {
        var ds = elements.filter(function (d) {
            return d.Index === index.Id;
        });
        if (ds.length !== 0) {
            var last = ds[ds.length - 1];
            if (chartType === 'LineChart') {
                drawLineLegend(index, lineCount);
                lineCount++;
            } else {
                drawAreaLegend(last);
            }
        }
    });

    function drawArea(ds) {
        var area = d3
            .area()
            .x(function (d) {
                return (
                    $p.dateDiff('d', new Date(d.Day), minDate) * dayWidth + axisPaddingX + padding
                );
            })
            .y0(function (d) {
                return yScale(0);
            })
            .y1(function (d) {
                return yScale(d.Y);
            });
        var g = svg.append('g').attr('class', 'surface');
        g.append('path').attr('d', area(ds)).attr('fill', color());
    }

    function drawLine(ds, index) {
        var line = d3
            .line()
            .x(function (d) {
                return (
                    $p.dateDiff('d', new Date(d.Day), minDate) * dayWidth + axisPaddingX + padding
                );
            })
            .y(function (d) {
                return yScale(d.Value);
            });
        var g = svg.append('g').attr('class', 'surface');
        g.append('path')
            .attr('d', line(ds))
            .attr('fill', 'none')
            .attr('stroke', colorScale(index))
            .attr('stroke-width', 1.5);
        g.selectAll('circle')
            .data(ds)
            .enter()
            .append('circle')
            .attr('cx', function (d) {
                return (
                    $p.dateDiff('d', new Date(d.Day), minDate) * dayWidth + axisPaddingX + padding
                );
            })
            .attr('cy', function (d) {
                return yScale(d.Value);
            })
            .attr('r', 4)
            .attr('stroke', colorScale(index))
            .attr('fill', colorScale(index));
    }

    function drawLineLegend(index, lineCount) {
        var g = svg.append('g');
        g.append('text')
            .attr('class', 'index')
            .attr('transform', 'translate(' + (width + padding) + ', ' + axisPaddingY + ')')
            .attr('x', padding)
            .attr('y', lineCount * 20)
            .attr('text-anchor', 'end')
            .attr('dominant-baseline', 'central')
            .text(index.LegendText);
        g.append('line')
            .attr('transform', 'translate(' + (width + padding) + ', ' + axisPaddingY + ')')
            .attr('x1', padding + 10)
            .attr('x2', padding + 20)
            .attr('y1', lineCount * 20)
            .attr('y2', lineCount * 20)
            .attr('stroke-width', 4)
            .attr('stroke', colorScale(index.Id));
    }

    function drawAreaLegend(last) {
        var g = svg.append('g');
        g.append('text')
            .attr('class', 'index')
            .attr(
                'x',
                $p.dateDiff('d', new Date(last.Day), minDate) * dayWidth +
                    axisPaddingX +
                    padding -
                    10
            )
            .attr('y', yScale(last.Y - last.Value / 2))
            .attr('text-anchor', 'end')
            .attr('dominant-baseline', 'middle')
            .text(
                indexes.filter(function (d) {
                    return d.Id === last.Index;
                })[0].Text
            );
    }

    function color() {
        var c = Math.floor(Math.random() * 50 + 180);
        return '#' + part(c) + part(c) + part(c);
    }

    function part(c) {
        return (c + Math.floor(Math.random() * 10 - 5)).toString(16);
    }
};
