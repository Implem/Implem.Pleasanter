$p.drawBurnDown = function () {
    var $svg = $('#BurnDown');
    if ($svg.length !== 1) {
        return;
    }
    $svg.empty();
    var json = JSON.parse($('#BurnDownJson').val());
    if (json.length === 0) {
        $svg.hide();
        return;
    }
    $svg.show();
    var svg = d3.select('#BurnDown');
    var padding = 40;
    var axisPadding = 70;
    var width = parseInt(svg.style('width'));
    var height = parseInt(svg.style('height'));
    var bodyWidth = width - axisPadding - (padding);
    var bodyHeight = height - axisPadding - (padding);
    var minDate = new Date(d3.min(json, function (d) { return d.Day; }));
    var maxDate = new Date(d3.max(json, function (d) { return d.Day; }));
    var dayWidth = (bodyWidth - padding) / $p.dateDiff('d', maxDate, minDate);
    var xScale = d3.time.scale()
        .domain([minDate, maxDate])
        .range([padding, bodyWidth]);
    var yScale = d3.scale.linear()
        .domain([d3.max(json, function (d) {
            return d.Total !== undefined || d.Earned !== undefined
                ? Math.max.apply(null, [d.Total, d.Planned, d.Earned])
                : d.Planned;
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
    var now = axisPadding + xScale(new $p.shortDate());
    var nowLineData = [
        [now, axisPadding - 40],
        [now, yScale(0) + 20]];
    var nowLine = d3.svg.line()
        .x(function (d) { return d[0]; })
        .y(function (d) { return d[1]; });
    svg.append('g').attr('class', 'now').append('path').attr('d', nowLine(nowLineData));
    draw('total', 0, json.filter(function (d) { return d.Total !== undefined; }));
    draw('planned', 1, json.filter(function (d) { return d.Planned !== undefined; }));
    draw('earned', 2, json.filter(function (d) { return d.Earned !== undefined; }));

    function draw(css, n, ds) {
        var line = d3.svg.line()
            .x(function (d) {
                return ($p.dateDiff('d', new Date(d.Day), minDate) * dayWidth)
                    + axisPadding + padding;
            })
            .y(function (d) {
                return yScale(prop(d));
            });
        var g = svg.append('g').attr('class', css);
        g.append('path').attr('d', line(ds));
        g.selectAll('circle')
            .data(ds)
            .enter()
            .append('circle')
            .attr('cx', function (d, i) { return i * dayWidth + axisPadding + padding })
            .attr('cy', function (d) { return yScale(prop(d)); })
            .attr('r', 4)
            .append('title')
            .text(function (d) { return prop(d); });

        function prop(d) {
            switch (n) {
                case 0: return d.Total;
                case 1: return d.Planned;
                case 2: return d.Earned;
            }
        }
    }
}