$p.drawAnaly = function () {
    var json = JSON.parse($('#AnalyJson').val());
        if (json[json.length - 1].Elements.length == 0) { 
            alert('データがありません。')
        }
    // データが空でない場合にグラフを描画
    var pieChartWidth = 300;
    var pieChartHeight = 300;
    var count = 0;  // 処理の回数
    var newX = 200;
    var newY = pieChartHeight;
    var screenWidth = window.innerWidth;
    var screenHeight = window.innerHeight;// 画面の幅

    json.forEach(function (pieChart) {
        var pieChartElements = pieChart.Elements;
        if (pieChartElements.length == 0) {
            return;// データがない場合、処理を中断
        }
        var pieChartSetting = pieChart.Setting;
        count = count + 1;
        var radius = Math.min(pieChartWidth, pieChartHeight) / 2 - 10;
        var svg = d3.select("#Analy")
            .attr("width", screenWidth )
            .attr("height", screenHeight);

        // 2回目以降の処理から+400ずつ
            if (count !== 1) {
                newX = newX + 300;
                if (screenWidth >= newX) {
                } else {
                    newX = 200;
                    newY = newY + 600;
                }
                if (screenHeight >= newY) {
                } else {
                    alert('データがいっぱいです。ログインしなおしてください。')
                    return;
                }
            }

        // X軸とY軸を決めている
        var g = svg.append("g")
            .attr("transform", "translate(" + newX + "," + newY / 2 + ")");
        g.append("text")  
            .attr("fill", "black")
            .attr("font-size", "25px")
            .attr("text-anchor", "middle")
            .attr("dy", -20)
            .text(pieChartSetting.AggregationType);
        g.append("text")
            .attr("fill", "black")
            .attr("font-size", "15px")
            .attr("text-anchor", "middle")
            .attr("dy", 0)
            .text(pieChartSetting.TimePeriodValue + "" + pieChartSetting.TimePeriod);
        g.append("text")
            .attr("fill", "black")
            .attr("font-size", "15px")
            .attr("text-anchor", "middle")
            .attr("dy", 20)
            .text(pieChartSetting.AggregationTarget);
        g.append("text")
            .attr("fill", "black")
            .attr("font-size", "15px")
            .attr("text-anchor", "middle")
            .attr("dy", 40)
            .text(pieChartSetting.GroupBy);

        var color = d3.scaleOrdinal()
            .range(["#DC3912", "#3366CC", "#109618", "#FF9900", "#990099"]);

        var pie = d3.pie()
            .value(function (d) { return d.Value; })
            .sort(null);

        var pieGroup = g.selectAll(".pie")  // gのすべてのpie要素を選択
            .data(pie(pieChartElements))
            .enter()
            .append("g")
            .attr("class", "pie");  // idをpieに変更

        var arc = d3.arc()
            .outerRadius(radius)
            .innerRadius(75);

        pieGroup.append("path")  // pieGroupにpathを追加
            .attr("d", arc)
            .attr("fill", function (d) { return color(d.index) })
            .attr("opacity", 0.85)
            .attr("stroke", "white");

        var text = d3.arc()
            .outerRadius(radius - 30)
            .innerRadius(radius - 40);

        pieGroup.append("text")  // pieGroupにtextを追加
            .attr("fill", "black")
            .attr("transform", function (d) { return "translate(" + text.centroid(d) + ")"; })
            .attr("dy", "5px")
            .attr("font-size", "13px")
            .attr("text-anchor", "middle")
            .text(function (d) {
                if (d.data.Value === 0) {
                    return;
                } else {
                return d.data.GroupTitle + "," + d.data.Value;
                }
            });
    });
}

$p.openAnalyPartDialog = function ($control) {
    error = $p.syncSend($control);
    if (error === 0) {
        $('#AnalyPartDialog').dialog({
            modal: true,
            width: '420px',
            resizable: false
        });
    }
}