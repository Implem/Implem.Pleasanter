$p.drawTimeSeries = function () {
    $('#TimeSeriesValueField').toggle($('#TimeSeriesAggregateType').val() !== 'Count');
    var $svg = $('#TimeSeries');
    if ($svg.length !== 1) {
        return;
    }
    $svg.empty();
    var json = JSON.parse($('#TimeSeriesJson').val());
    var indexes = json.Indexes;
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
    var height = parseInt(svg.style('height'));
    var bodyWidth = width - axisPaddingX - (padding);
    var bodyHeight = height - axisPaddingY - (padding);
    var minDate = new Date(d3.min(elements, function (d) { return d.Day; }));
    var maxDate = new Date(d3.max(elements, function (d) { return d.Day; }));
    var dayWidth = (bodyWidth - padding) / $p.dateDiff('d', maxDate, minDate);
    var xScale = d3.time.scale()
        .domain([minDate, maxDate])
        .range([padding, bodyWidth]);
    var yScale = d3.scale.linear()
        .domain([d3.max(elements, function (d) {
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
        var ds = elements.filter(function (d) { return d.Index === index.Id; });
        draw(ds);
    });
    indexes.forEach(function (index) {
        var ds = elements.filter(function (d) { return d.Index === index.Id; });
        if (ds.length !== 0) {
            var last = ds[ds.length - 1];
            var g = svg.append('g');
            g.append('text')
                .attr('class', 'index')
                .attr('x', ($p.dateDiff('d', new Date(last.Day), minDate) * dayWidth)
                    + axisPaddingX + padding - 10)
                .attr('y', yScale(last.Y - (last.Value / 2)))
                .attr('text-anchor', 'end')
                .attr('dominant-baseline', 'middle')
                .text(indexes.filter(function (d) { return d.Id === last.Index })[0].Text);
        }
    });

    function draw(ds) {
        var area = d3.svg.area()
            .x(function (d) {
                return ($p.dateDiff('d', new Date(d.Day), minDate) * dayWidth)
                    + axisPaddingX + padding;
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
        var c = Math.floor(Math.random() * 50 + 180);
        return '#' + part(c) + part(c) + part(c);
    }

    function part(c) {
        return (c + Math.floor(Math.random() * 10 - 5)).toString(16);
    }
}