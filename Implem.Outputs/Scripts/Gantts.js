func.drawGantt = function () {
    var $svg = $('#Gantt');
    if ($svg.length !== 1) {
        return;
    }
    $svg.empty();
    var dataSet = JSON.parse($('#GanttJson').val());
    if (dataSet.length === 0) {
        $svg.hide();
        return;
    }
    $svg.show();
    var justTime = new Date();
    var svg = d3.select('#Gantt');
    var padding = 20;
    var axisPadding = 50;
    var width = parseInt(svg.style('width'));
    var height = parseInt(svg.style('height'));
    var bodyWidth = width - padding * 2;
    var minDate = new Date(d3.min(dataSet, function (d) {
        return Math.min.apply(null, [new Date(d.StartTime), justTime]);
    }));
    var maxDate = new Date(d3.max(dataSet, function (d) {
        return Math.max.apply(null, [new Date(d.CompletionTime), justTime]);
    }));
    var dayWidth = (bodyWidth - padding) / dateDiff('d', maxDate, minDate);
    var xScale = d3.time.scale()
        .domain([minDate, maxDate])
        .range([padding, bodyWidth]);
    var xHarf = xScale(maxDate) / 2;
    var xAxis = d3.svg.axis()
        .scale(xScale)
        .tickFormat(d3.time.format('%m/%d'))
        .ticks(20);
    var now = padding + xScale(justTime);
    d3.select('#GanttAxis').append('g')
        .attr('class', 'axis')
        .attr('transform', 'translate(30, 10)')
        .call(xAxis)
        .selectAll('text')
        .attr('x', -20)
        .attr('y', 20)
        .style('text-anchor', 'start');
    var currentDate = minDate;
    while (currentDate <= maxDate) {
        drawLine(padding + xScale(currentDate), 'date');
        currentDate = dateAdd('d', 1, currentDate);
    }
    svg.append('g').attr('class', 'plan')
        .selectAll('rect')
        .data(dataSet)
        .enter()
        .append('rect')
        .attr('x', function (d) { return padding + xScale(new Date(d.StartTime)) })
        .attr('y', function (d, i) {
            return padding + i * 25;
        })
        .attr('width', function (d) {
            return xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime))
        })
        .attr('height', 23)
        .attr('class', function (d) {
            return d.Completed
                ? 'completed'
                : '';
        })
        .attr('data-id', function (d) { return d.Id; })
        .append('title')
        .text(function (d) {
            return d.StartTime + ' - ' + d.DisplayCompletionTime;
        });
    svg.append('g').attr('class', 'earn')
        .selectAll('rect')
        .data(dataSet)
        .enter()
        .append('rect')
        .attr('x', function (d) { return padding + xScale(new Date(d.StartTime)) })
        .attr('y', function (d, i) {
            return padding + i * 25;
        })
        .attr('width', function (d) {
            return (xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime)))
                * d.ProgressRate * 0.01
        })
        .attr('height', 23)
        .attr('class', function (d) {
            return d.ProgressRate < 100 &&
                (padding + xScale(new Date(d.StartTime)) +
                ((xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime)))
                * d.ProgressRate * 0.01)) < now
                    ? 'delay'
                    : d.ProgressRate === 100 && d.Completed
                        ? 'completed'
                        : '';
        })
        .attr('data-id', function (d) { return d.Id; })
        .append('title')
        .text(function (d) {
            return d.StartTime + ' - ' + d.DisplayCompletionTime;
        });
    drawLine(now, 'now');
    svg.append('g').attr('class', 'title')
        .selectAll('text')
        .data(dataSet)
        .enter()
        .append('text')
        .attr('x', function (d) {
            return padding +
                (xScale(new Date(d.StartTime)) < xHarf
                    ? xScale(new Date(d.StartTime)) + 5
                    : xScale(new Date(d.CompletionTime)) - 5)
        })
        .attr('y', function (d, i) {
            return padding + 16 + i * 25;
        })
        .attr('width', function (d) {
            return (xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime)))
                * d.ProgressRate * 0.01
        })
        .attr('height', 23)
        .attr('class', function (d) {
            return d.ProgressRate < 100 &&
                (padding + xScale(new Date(d.StartTime)) +
                ((xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime)))
                * d.ProgressRate * 0.01)) < now
                    ? 'delay'
                    : '';
        })
        .attr('text-anchor', function (d) {
            return xScale(new Date(d.StartTime)) < xHarf
                ? 'start'
                : 'end';
        })
        .attr('data-id', function (d) { return d.Id; })
        .text(function (d) {
            return d.Title;
        })
        .append('title')
        .text(function (d) {
            return d.StartTime + ' - ' + d.DisplayCompletionTime;
        });

    function drawLine(day, css) {
        var nowLineData = [
            [day, padding - 10],
            [day, (padding + dataSet.length * 25) + 10]];
        var nowLine = d3.svg.line()
            .x(function (d) { return d[0]; })
            .y(function (d) { return d[1]; });
        svg.append('g').attr('class', css).append('path').attr('d', nowLine(nowLineData));
    }

    $(document).on('click', '#Gantt .plan rect,#Gantt .earn rect,#Gantt .title text', function () {
        location.href = $('#BaseUrl').val() + $(this).attr('data-id');
    });
}