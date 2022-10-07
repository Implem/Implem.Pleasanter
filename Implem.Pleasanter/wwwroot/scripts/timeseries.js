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
    var elements = json.Elements;
    if (elements.length === 0) {
        $svg.hide();
        return;
    }
    $svg.show();
    var svg = d3.select('#TimeSeries');
    //var svgLegend = d3.select('#TimeSeries');
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
    var xScale = d3.scaleTime()
        .domain([minDate, maxDate])
        .range([padding, bodyWidth]);
    var yScale = d3.scaleLinear()
        .domain([d3.max(elements, function (d) {
            return (chartType === 'LineChart') ? d.Value : d.Y;
        }), 0])
        .range([padding, bodyHeight])
        .nice();
    var xAxis = d3.axisBottom(xScale)
        .tickFormat(d3.timeFormat('%m/%d'))
        .ticks(10);
    var yAxis = d3.axisLeft(yScale);
    var colorScale = d3.scaleOrdinal(d3.schemeSet1); //色のセット
    //var color = d3.scale.category10();
    //凡例の領域
    /*var legend = svgLegned.selectAll('.legends')　// 凡例の領域作成
        .data(legendVals)
        .enter()
        .append('g')
        .attr('class', "legends")
        .attr("transform", function (d, i) {
            {
                return "translate(0," + i * 20 + ")" // 各凡例をy方向に20px間隔で移動
            }
        });*/
    
  
    

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
        (chartType === 'LineChart') ? drawLine(ds, index.Id) : drawArea(ds);
    });
    indexes.forEach(function (index) {
        var ds = elements.filter(function (d) { return d.Index === index.Id; });
        if (ds.length !== 0) {
            var last = ds[ds.length - 1];
            var g = svg.append('g');
            if (chartType === 'LineChart') {
                g.append('text')
                    .attr('class', 'index')
                    .attr('transform', 'translate(' + 10 + ', ' + 20 + ')')//後で試すtranslate(index.Id * 10, height-5)
                    .attr('x', 300)
                    .attr('y', index.Id * 20)
                    .attr('text-anchor', 'end')
                    .attr('dominant-baseline', 'middle')
                    .text(indexes.filter(function (d) { return d.Id === last.Index })[0].Text)
                g.append("line")
                    .attr('transform', 'translate(' + 10 + ', ' + 20 + ')')
                    .attr("x1", 310)
                    .attr("x2", 330)
                    .attr("y1", index.Id * 20)
                    .attr("y2", index.Id * 20)
                    .attr("stroke-width", 4)
                    .attr("stroke", colorScale(index.Id));
            } else {
                g.append('text')
                    .attr('class', 'index')
                    .attr('x', ($p.dateDiff('d', new Date(last.Day), minDate) * dayWidth)
                        + axisPaddingX + padding - 10)
                    .attr('y', yScale(last.Y - (last.Value / 2)))
                    .attr('text-anchor', 'end')
                    .attr('dominant-baseline', 'middle')
                    .text(indexes.filter(function (d) { return d.Id === last.Index })[0].Text);
            }
            
            //g.append('rect')
            //    .attr('transform', 'translate(' + 10 + ', ' + 20 + ')')
            //    .attr('x', 300)
            //    .attr('y', index.Id * 20)
            //    .attr("width", 10)
            //    .attr("height", 10)
            //    .style("fill", function (d, i) { return color(i); }) // 色付け
        }
    });

    function drawArea(ds) {
        var area = d3.area()
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

    function drawLine(ds, index) {
        var line = d3.line()
            .x(function (d) {
                return ($p.dateDiff('d', new Date(d.Day), minDate) * dayWidth)
                    + axisPaddingX + padding;
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
        //g.selectAll('circle')
        //    .data(ds)
        //    .enter()
        //    .append('circle')
        //    .attr('cx', function (d, i) { return i * dayWidth + axisPadding + padding })
        //    .attr('cy', function (d) { return yScale(prop(d)); })
        //    .attr('r', 4)
        //    .append('title')
        //    .text(function (d) { return prop(d); });
    }

    function color() {
        var c = Math.floor(Math.random() * 50 + 180);
        return '#' + part(c) + part(c) + part(c);
    }

    function part(c) {
        return (c + Math.floor(Math.random() * 10 - 5)).toString(16);
    }
}