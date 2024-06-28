$p.moveGantt = function (type) {
    var $control = $('#GanttStartDate');
    var value = $('#Gantt' + type).val();
    $control.val(value);
    $control.attr('data-previous', value);
    $p.getData($control).GanttStartDate = value;
    $p.send($control);
}

$p.drawGantt = function () {
    let spacing = 25;
    let heightPlaned = 23;
    let heightEarned = 23;
    let fontSize = 'inherit';
    let dYText = 16;
    let heightGantt = 45;
    let dYFirstLineAxis = 20;
    let dYSecondLineAxis = 40;
    let heightAxis = 20;
    if (window.matchMedia("(max-width: 1024px)").matches) {
        spacing = 50;
        heightPlaned = 40;
        heightEarned = 40;
        dYText = 29;
        heightGantt = 60;
        dYFirstLineAxis = 20;
        dYSecondLineAxis = 40;
        heightAxis = 20;
    }
    if (window.matchMedia("(max-width: 767px)").matches) {
        spacing = 30;
        heightPlaned = 23;
        heightEarned = 23;
        fontSize = '2.6vw';
        dYText = 16;
        heightGantt = 45;
        dYFirstLineAxis = 20;
        dYSecondLineAxis = 40;
        heightAxis = 20;
    }
    var $gantt = $('#Gantt');
    var $axis = $('#GanttAxis');
    if ($gantt.length !== 1) {
        return;
    }
    $gantt.empty();
    $axis.empty();
    var json = JSON.parse($('#GanttJson').val());
    if (json.length === 0) {
        $gantt.hide();
        return;
    }
    $gantt.show();
    var TimeZoneOffset = $('#TimeZoneOffset').val();
    var justTime = new Date(moment().utcOffset(TimeZoneOffset).format('YYYY/MM/DD HH:mm:ss'));
    var axis = d3.select('#GanttAxis');
    var svg = d3.select('#Gantt');
    var padding = 20;
    var width = parseInt(svg.style('width'));
    var format = $('#YmdFormat').val();
    var minDate = $p.transferedDate(format, $('#GanttMinDate').val());
    var maxDate = $p.transferedDate(format, $('#GanttMaxDate').val());
    var xScale = d3.scaleTime()
        .domain([minDate, maxDate])
        .range([0, width - 60]);
    var xHarf = xScale(maxDate) / 2;
    var months = [];
    var currentMonth;
    var days = [];
    for (var s = 0; s < $p.dateDiff('d', maxDate, minDate); s++) {
        var d = $p.dateAdd('d', s, minDate);
        days.push(d);
        if (currentMonth !== d.getMonth()) {
            currentMonth = d.getMonth();
            months.push(d);
        }
    }
    axis.append('g')
        .selectAll('rect')
        .data(days)
        .enter()
        .append('rect')
        .attr('x', function (d) { return 23 + xScale(d) })
        .attr('y', 25)
        .attr('width', xScale(days[1]))
        .attr('height', heightAxis)
        .attr('class', function (d) {
            switch (d.getDay()) {
                case 0: return 'sunday';
                case 6: return 'saturday';
                default: return 'weekday';
            }
        });
    var currentDate = minDate;
    while (currentDate <= maxDate) {
        var axisLine = [[30 + xScale(currentDate), 25], [30 + xScale(currentDate), 60]];
        var line = d3.line()
            .x(function (d) { return d[0] - 8; })
            .y(function (d) { return d[1]; });
        axis.append('g').attr('class', 'date').append('path').attr('d', line(axisLine));
        currentDate = $p.dateAdd('d', 1, currentDate);
    }
    axis.append('g')
        .attr('class', 'title')
        .selectAll('text')
        .data(months)
        .enter()
        .append('text')
        .attr('text-anchor', 'middle')
        .attr('x', function (d) {
            return 22 + xScale(d) + (xScale($p.dateAdd('d', 1, d)) - xScale(d)) / 2;
        })
        .attr('y', dYFirstLineAxis)
        .style('font-size', fontSize)
        .text(function (d) {
            return d.getMonth() + 1;
        });
    axis.append('g')
        .attr('class', 'title')
        .selectAll('text')
        .data(days.filter(function (d) {
            return days.length <= 90 || [5, 10, 15, 20, 25, 30].indexOf(d.getDate()) > -1;
        }))
        .enter()
        .append('text')
        .attr('text-anchor', 'middle')
        .attr('x', function (d) {
            return 22 + xScale(d) + (xScale($p.dateAdd('d', 1, d)) - xScale(d)) / 2;
        })
        .attr('y', dYSecondLineAxis)
        .style('font-size', fontSize)
        .text(function (d) {
            return d.getDate();
        });
    var now = padding + xScale(justTime);
    var groupCount = json.filter(function (d) { return d.GroupSummary }).length === 0
        ? 0
        : -1;
    $.each(json, function (i, d) {
        if (d.GroupSummary) groupCount++;
        d.Y = padding + i * spacing + groupCount * 25;
    });
    $('#Gantt').css('height', d3.max(json, function (d) { return d.Y }) + heightGantt);
    svg.append('g')
        .selectAll('rect')
        .data(days.filter(function (d) {
            switch (d.getDay()) {
                case 0: return true;
                case 6: return true;
                default: return false;
            }
        }))
        .enter()
        .append('rect')
        .attr('x', function (d) { return padding + xScale(d) })
        .attr('y', padding - 10)
        .attr('width', xScale(days[1]))
        .attr('height', (padding + d3.max(json, function (d) { return d.Y })))
        .attr('class', function (d) {
            switch (d.getDay()) {
                case 0: return 'sunday';
                case 6: return 'saturday';
                default: return null;
            }
        });
    currentDate = minDate;
    while (currentDate <= maxDate) {
        draw(padding + xScale(currentDate), 'date');
        currentDate = $p.dateAdd('d', 1, currentDate);
    }
    svg.append('g').attr('class', 'planned')
        .selectAll('rect')
        .data(json)
        .enter()
        .append('rect')
        .attr('x', function (d) {
            return padding + xScale($p.transferedDate(format, d.StartTime))
        })
        .attr('y', function (d) {
            return d.Y;
        })
        .attr('width', function (d) {
            return xScale($p.transferedDate(format, d.CompletionTime))
                - xScale($p.transferedDate(format, d.StartTime))
        })
        .attr('height', heightPlaned)
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
        .attr('x', function (d) {
            return padding + xScale($p.transferedDate(format, d.StartTime))
        })
        .attr('y', function (d) {
            return d.Y;
        })
        .attr('width', function (d) {
            return (xScale($p.transferedDate(format, d.CompletionTime))
                - xScale($p.transferedDate(format, d.StartTime)))
                * d.ProgressRate * 0.01
        })
        .attr('height', heightEarned)
        .attr('class', function (d) {
            var ret = d.ProgressRate < 100 &&
                (padding + xScale($p.transferedDate(format, d.StartTime)) +
                    ((xScale($p.transferedDate(format, d.CompletionTime)) - xScale($p.transferedDate(format, d.StartTime)))
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
            return xScale($p.transferedDate(format, d.StartTime)) < 0
                ? padding + 5
                : padding + xScale($p.transferedDate(format, d.StartTime)) + 5
        })
        .attr('y', function (d) {
            return d.Y + dYText;
        })
        .attr('width', function (d) {
            return (xScale($p.transferedDate(format, d.CompletionTime))
                - xScale($p.transferedDate(format, d.StartTime)))
                * d.ProgressRate * 0.01
        })
        .attr('height', 50)
        .attr('class', function (d) {
            var ret = d.ProgressRate < 100 &&
                (padding + xScale($p.transferedDate(format, d.StartTime)) +
                    ((xScale($p.transferedDate(format, d.CompletionTime)) - xScale($p.transferedDate(format, d.StartTime)))
                        * d.ProgressRate * 0.01)) < now &&
                ($('#ShowGanttProgressRate').val() === '1' || !d.Completed)
                ? 'delay'
                : '';
            return d.GroupSummary
                ? ret + ' summary'
                : ret;
        })
        .attr('text-anchor', function (d) {
            return 'start';
        })
        .attr('data-id', function (d) { return d.Id; })
        .style('font-size', fontSize)
        .text(function (d) {
            if (window.matchMedia("(max-width: 1024px)").matches) {
                let labelRange = 0;
                let span = (xScale($p.transferedDate(format, d.CompletionTime))
                    - xScale($p.transferedDate(format, d.StartTime)))
                    * d.ProgressRate * 0.01
                let task = d.Title;
                let label;
                (span > labelRange) ? labelRange = span : labelRange;
                (task.length * 7 > span) ? label = task.substring(0, 50) + "..." : label = task;
                return label;
            } else {
                return d.Title;
            }
        })
        .append('title')
        .text(function (d) {
            return d.StartTime + ' - ' + d.DisplayCompletionTime + ' : ' + d.Title;
        });

    function draw(day, css) {
        var nowLineData = [
            [day, padding - 10],
            [day, (padding + d3.max(json, function (d) { return d.Y })) + 10]];
        var nowLine = d3.line()
            .x(function (d) { return d[0]; })
            .y(function (d) { return d[1]; });
        svg.append('g').attr('class', css).append('path').attr('d', nowLine(nowLineData));
    }
}