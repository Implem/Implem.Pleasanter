func.drawGantt = function () {
    var $svg = $('#Gantt');
    if ($svg.length !== 1) {
        return;
    }
    $svg.empty();
    var json = JSON.parse($('#GanttJson').val());
    if (json.length === 0) {
        $svg.hide();
        return;
    }
    $svg.show();
    var justTime = new Date();
    var svg = d3.select('#Gantt');
    var padding = 20;
    var width = parseInt(svg.style('width'));
    var minDate = new Date(d3.min(json, function (d) {
        return Math.min.apply(null, [new Date(d.StartTime), justTime]);
    }));
    var maxDate = new Date(d3.max(json, function (d) {
        return Math.max.apply(null, [new Date(d.CompletionTime), justTime]);
    }));
    var xScale = d3.time.scale()
        .domain([minDate, maxDate])
        .range([0, width - 60]);
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
    var groupCount = json.filter(function (d) { return d.GroupSummary }).length === 0
        ? 0
        : -1;
    $.each(json, function (i, d) {
        if (d.GroupSummary) groupCount++;
        d.Y = padding + i * 25 + groupCount * 25;
    });
    $('#Gantt').css('height', d3.max(json, function (d) { return d.Y }) + 45);
    var currentDate = minDate;
    while (currentDate <= maxDate) {
        draw(padding + xScale(currentDate), 'date');
        currentDate = dateAdd('d', 1, currentDate);
    }
    svg.append('g').attr('class', 'planned')
        .selectAll('rect')
        .data(json)
        .enter()
        .append('rect')
        .attr('x', function (d) { return padding + xScale(new Date(d.StartTime)) })
        .attr('y', function (d) {
            return d.Y;
        })
        .attr('width', function (d) {
            return xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime))
        })
        .attr('height', 23)
        .attr('class', function (d) {
            var ret = d.Completed
                ? 'completed'
                : '';
            return d.GroupSummary
                ? ret + ' summary'
                : ret;
        })
        .attr('data-id', function (d) { return d.Id; })
        .append('title')
        .text(function (d) {
            return d.StartTime + ' - ' + d.DisplayCompletionTime;
        });
    svg.append('g').attr('class', 'earned')
        .selectAll('rect')
        .data(json)
        .enter()
        .append('rect')
        .attr('x', function (d) { return padding + xScale(new Date(d.StartTime)) })
        .attr('y', function (d) {
            return d.Y;
        })
        .attr('width', function (d) {
            return (xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime)))
                * d.ProgressRate * 0.01
        })
        .attr('height', 23)
        .attr('class', function (d) {
            var ret = d.ProgressRate < 100 &&
                (padding + xScale(new Date(d.StartTime)) +
                ((xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime)))
                * d.ProgressRate * 0.01)) < now
                    ? 'delay'
                    : d.ProgressRate === 100 && d.Completed
                        ? 'completed'
                        : ''
            return d.GroupSummary
                ? ret + ' summary'
                : ret;
        })
        .attr('data-id', function (d) { return d.Id; })
        .append('title')
        .text(function (d) {
            return d.StartTime + ' - ' + d.DisplayCompletionTime;
        });
    draw(now, 'now');
    svg.append('g').attr('class', 'title')
        .selectAll('text')
        .data(json)
        .enter()
        .append('text')
        .attr('x', function (d) {
            return padding +
                (xScale(new Date(d.StartTime)) < xHarf
                    ? xScale(new Date(d.StartTime)) + 5
                    : xScale(new Date(d.CompletionTime)) - 5)
        })
        .attr('y', function (d) {
            return d.Y + 16;
        })
        .attr('width', function (d) {
            return (xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime)))
                * d.ProgressRate * 0.01
        })
        .attr('height', 23)
        .attr('class', function (d) {
            var ret = d.ProgressRate < 100 &&
                (padding + xScale(new Date(d.StartTime)) +
                ((xScale(new Date(d.CompletionTime)) - xScale(new Date(d.StartTime)))
                * d.ProgressRate * 0.01)) < now
                    ? 'delay'
                    : '';
            return d.GroupSummary
                ? ret + ' summary'
                : ret;
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

    function draw(day, css) {
        var nowLineData = [
            [day, padding - 10],
            [day, (padding + d3.max(json, function (d) { return d.Y })) + 10]];
        var nowLine = d3.svg.line()
            .x(function (d) { return d[0]; })
            .y(function (d) { return d[1]; });
        svg.append('g').attr('class', css).append('path').attr('d', nowLine(nowLineData));
    }

    $(document).on('click', '#Gantt .planned rect,#Gantt .earned rect,#Gantt .title text', function () {
        if ($(this).filter('.summary').length === 0) {
            location.href = $('#BaseUrl').val() + $(this).attr('data-id');
        }
    });
}