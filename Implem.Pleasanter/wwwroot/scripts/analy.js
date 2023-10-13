$p.drawAnaly = function () {
    var json = JSON.parse($('#AnalyJson').val());
    // データが空でない場合にグラフを描画
    var pieChartWidth = 400;
    var pieChartHeight = 400;
    var count = 0;  // 処理の回数
    var newX = 200;
    var newY = pieChartHeight;
    var screenWidth = window.innerWidth;  // 画面の幅

    json.forEach(function (pieChart) {
        var pieChartElements = pieChart.Elements;
        count = count + 1;
        var radius = Math.min(pieChartWidth, pieChartHeight) / 2 - 10;
        var svg = d3.select("#Analy")
            .attr("width", pieChartWidth * 4)
            .attr("height", pieChartHeight * 4);

        // 2回目以降の処理から+400ずつ
        if (count !== 1) {
            newX = newX + 400;
            if (screenWidth >= newX) {
            } else {
                newX = 200;
                newY = newY + 900;
            }
        }

        // X軸とY軸を決めている
        var g = svg.append("g")
            .attr("transform", "translate(" + newX + "," + newY / 2 + ")");
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
            .innerRadius(90);

        pieGroup.append("path")  // pieGroupにpathを追加
            .attr("d", arc)
            .attr("fill", function (d) { return color(d.index) })
            .attr("opacity", 0.85)
            .attr("stroke", "white");

        var text = d3.arc()
            .outerRadius(radius - 30)
            .innerRadius(radius - 50);

        pieGroup.append("text")  // pieGroupにtextを追加
            .attr("fill", "black")
            .attr("transform", function (d) { return "translate(" + text.centroid(d) + ")"; })
            .attr("dy", "5px")
            .attr("font-size", "15px")
            .attr("text-anchor", "middle")
            .text(function (d) { return d.data.GroupTitle; });
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