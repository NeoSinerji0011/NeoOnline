var chart = AmCharts.makeChart("chartdiv", {
    "type": "pie",
    "theme": "light",
    "dataProvider": [{
        "urun": "Trafik",
        "value": 200,
       
    }, {
        "urun": "Kasko",
        "value": 211
    }, {
        "urun": "Dask",
        "value": 105
    }, {
        "urun": "Konut",
        "value": 39
    }, {
        "urun": "İş Yeri",
        "value": 19
    }],
    "valueField": "value",
    "titleField": "urun",
    "outlineAlpha": 0.4,
    "depth3D": 15,
    "balloonText": "[[title]]<br><span style='font-size:14px'><b>[[value]]</b> ([[percents]]%)</span>",
    "angle": 30,
    "export": {
        "enabled": true
    }
});
//var chart = AmCharts.makeChart("chartdiv-police", {
//    "type": "pie",
//    "theme": "light",
//    "dataProvider": [{
//        "urun": "Trafik",
//        "value": 200
//    }, {
//        "urun": "Kasko",
//        "value": 211
//    }, {
//        "urun": "Dask",
//        "value": 105
//    }, {
//        "urun": "Konut",
//        "value": 39
//    }, {
//        "urun": "İş Yeri",
//        "value": 19
//    }],
//    "valueField": "value",
//    "titleField": "urun",
//    "outlineAlpha": 0.4,
//    "depth3D": 15,
//    "balloonText": "[[title]]<br><span style='font-size:14px'><b>[[value]]</b> ([[percents]]%)</span>",
//    "angle": 30,
//    "export": {
//        "enabled": true
//    }
//});
jQuery('.chart-input').off().on('input change', function () {
    var property = jQuery(this).data('property');
    var target = chart;
    var value = Number(this.value);
    chart.startDuration = 0;

    if (property == 'innerRadius') {
        value += "%";
    }

    target[property] = value;
    chart.validateNow();
});