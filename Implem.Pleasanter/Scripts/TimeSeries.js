func.drawTimeSeries = function () {
    $('#TimeSeriesValueColumnField').toggle($('#TimeSeriesAggregateType').val() !== 'Count');
    var $svg = $('#TimeSeries');
    if ($svg.length !== 1) {
        return;
    }
    $svg.empty();
    var indexes = JSON.parse($('#TimeSeriesJson').val()).Indexes;
    var dataSet = JSON.parse($('#TimeSeriesJson').val()).Elements;
    if (dataSet.length === 0) {
        $svg.hide();
        return;
    }
    $svg.show();
    var svg = d3.select('#TimeSeries');
    var padding = 40;
    var axisPadding = 70;
    var width = parseInt(svg.style('width'));
    var height = parseInt(svg.style('height'));
    var bodyWidth = width - axisPadding - (padding);
    var bodyHeight = height - axisPadding - (padding);
    var minDate = new Date(d3.min(dataSet, function (d) { return d.Day; }));
    var maxDate = new Date(d3.max(dataSet, function (d) { return d.Day; }));
    var dayWidth = (bodyWidth - padding) / dateDiff('d', maxDate, minDate);
    var xScale = d3.time.scale()
        .domain([minDate, maxDate])
        .range([padding, bodyWidth]);
    var yScale = d3.scale.linear()
        .domain([d3.max(dataSet, function (d) {
            return d.Y;
        }), 0])
        .range([padding, bodyHeight])
        .nice();
    var xAxis = d3.svg.axis()
        .scale(xScale)
        .tickFormat(d3.time.format('%m/%d'))
        .ticks(10);
    var yAxis = d3.svg.axis()
        .scale(yScale)
        .orient('left');
    svg.append('g')
        .attr('class', 'axis')
        .attr('transform', 'translate(' + axisPadding + ', ' + (height - axisPadding) + ')')
        .call(xAxis)
        .selectAll('text')
        .attr('x', -20)
        .attr('y', 20)
        .style('text-anchor', 'start');
    svg.append('g')
        .attr('class', 'axis')
        .attr('transform', 'translate(' + axisPadding + ', 0)')
        .call(yAxis)
        .selectAll('text')
        .attr('x', -20);
    indexes.forEach(function (index) {
        var ds = dataSet.filter(function (d) { return d.Index === index.Id; });
        draw(ds);
    });
    indexes.forEach(function (index) {
        var ds = dataSet.filter(function (d) { return d.Index === index.Id; });
        if (ds.length !== 0) {
            var last = ds[ds.length - 1];
            var g = svg.append('g');
            g.append('text')
                .attr('class', 'index')
                .attr('x', (dateDiff('d', new Date(last.Day), minDate) * dayWidth)
                    + axisPadding + padding - 10)
                .attr('y', yScale(last.Y - (last.Value / 2)))
                .attr('text-anchor', 'end')
                .attr('dominant-baseline', 'middle')
                .text(indexes.filter(function (d) { return d.Id === last.Index })[0].Text
                    + ' (' + last.Value + ')');
        }
    });

    function draw(ds) {
        var area = d3.svg.area()
            .x(function (d) {
                return (dateDiff('d', new Date(d.Day), minDate) * dayWidth)
                    + axisPadding + padding;
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

    function color() {
        return '#' + Math.floor(Math.random() * 10000000 + 3000000).toString(16);
    }
}