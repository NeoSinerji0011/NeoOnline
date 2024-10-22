var Performans = function () {

    //function showTooltip(title, x, y, contents) {
    //    $('<div id="tooltip" class="chart-tooltip"><div class="date">' + title + '<\/div><div class="label label-success">Total: ' + contents + '<\/div>').css({
    //        position: 'absolute',
    //        display: 'none',
    //        top: y - 100,
    //        width: 90,
    //        left: x - 40,
    //        border: '0px solid #ccc',
    //        padding: '2px 6px',
    //        'background-color': '#fff',
    //    }).appendTo("body").fadeIn(200);
    //}

    return {

        initDaterange: function (startDate, endDate) {

            $('#dashboard-report-range').daterangepicker({
                ranges: {
                    'Today': ['today', 'today'],
                    'Yesterday': ['yesterday', 'yesterday'],
                    'The Last 7 Days': [Date.today().add({ days: -6 }), 'today'],
                    'The Last 30 Days': [Date.today().add({ days: -29 }), 'today'],
                    'This Month': [Date.today().moveToFirstDayOfMonth(), Date.today().moveToLastDayOfMonth()],
                    'Past Month': [Date.today().moveToFirstDayOfMonth().add({ months: -1 }), Date.today().moveToFirstDayOfMonth().add({ days: -1 })]
                },
                opens: (App.isRTL() ? 'right' : 'left'),
                format: 'dd/MM/yyyy',
                separator: ' to ',
                startDate: startDate,
                endDate: endDate,
                minDate: '01/01/2013',
                maxDate: Date.today(),
                locale: {
                    applyLabel: 'Confirm',
                    clearLabel: "Exit",
                    fromLabel: 'From',
                    toLabel: 'To',
                    customRangeLabel: 'Custom Date',
                    daysOfWeek: ['Sun', 'Mon', 'Tues', 'Wed', 'Thurs', 'Fri', 'Sat'],
                    monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
                    firstDay: 1
                },
                showWeekNumbers: true,
                buttonClasses: ['btn-danger']
            },

            function (start, end) {
                App.blockUI(jQuery("#dashboard"));

                var baslamaT = start;
                var bitisT = end;

                window.location.href = "/TVM/TVM/Index?baslamaT='" + baslamaT + "'&bitisT='" + bitisT + "'"
                //setTimeout(function () {
                //    App.unblockUI(jQuery("#dashboard"));
                //    $.gritter.add({
                //        title: 'Bilgi Mesajı',
                //        text: 'Performans tarih aralığı değiştirildi.'
                //    });
                //    App.scrollTo();
                //}, 1000);
                $('#dashboard-report-range span').html(start.toString('MMMM d, yyyy') + ' - ' + end.toString('MMMM d, yyyy'));
            });

            $('#dashboard-report-range').show();
            $('#dashboard-report-range span').html(startDate.toString('MMMM d, yyyy') + ' - ' + endDate.toString('MMMM d, yyyy'));
            //$('#dashboard-report-range span').html(startDate).toString('MMMM d, yyyy') + ' - ' + endDate.toString('MMMM d, yyyy');
        },

        //initCharts: function (charts) {
        //    this.initTeklifChart(charts.Teklif);
        //    this.initPoliceChart(charts.Police);
        //    this.initPoliceTutarChart(charts.PoliceTutar);
        //},

        //initTeklifChart: function (teklif) {

        //    if (teklif.Trafik > 0) {
        //        var trafik = [
        //            [1, teklif.Trafik]
        //        ];
        //    }

        //    if (teklif.Kasko > 0) {
        //        var kasko = [
        //           [2, teklif.Kasko]
        //        ];
        //    }
        //    if (teklif.Dask > 0) {
        //        var dask = [
        //          [3, teklif.Dask]
        //        ];
        //    }
        //    if (teklif.KrediliHayat > 0) {
        //        var kredilihayat = [
        //         [4, teklif.KrediliHayat]
        //        ];
        //    }

        //    var previousPoint2 = null;
        //    $('#site_teklif_loading').hide();
        //    $('#site_teklif_content').show();

        //    var plot_activities = $.plot(
        //        $("#site_teklif"), [{
        //            data: trafik,
        //            label: "Traffic " + teklif.Trafik,
        //            color: "rgba(181,162,172, 0.7)",
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        }, {
        //            data: kasko,
        //            label: "Car Insurance " + teklif.Kasko,
        //            color: "rgba(170,102,142, 0.7)",
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        },
        //        {
        //            data: dask,
        //            label: "Dask " + teklif.Dask,
        //            color: "rgba(170,190,142, 0.7)",
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        },
        //        {
        //            data: kredilihayat,
        //            label: "Credit Life " + teklif.KrediliHayat,
        //            color: "rgba(170,60,80, 0.7)",
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        }
        //        ], {
        //            series: {
        //                bars: {
        //                    show: true,
        //                    barWidth: 0.9
        //                }
        //            },
        //            grid: {
        //                show: true,
        //                hoverable: true,
        //                clickable: false,
        //                autoHighlight: true,
        //                borderWidth: 0
        //            },
        //            yaxis: {
        //                min: 0,
        //                max: teklif.Bar
        //            },
        //            xaxis: {
        //                show: false
        //            },
        //        });

        //    //$("#site_teklif").bind("plothover", function (event, pos, item) {
        //    //    $("#x").text(pos.x.toFixed(2));
        //    //    $("#y").text(pos.y.toFixed(2));
        //    //    if (item) {
        //    //        if (previousPoint2 != item.dataIndex) {
        //    //            previousPoint2 = item.dataIndex;
        //    //            $("#tooltip").remove();
        //    //            var x = item.datapoint[0],
        //    //                y = item.datapoint[1].toFixed(2);
        //    //            var sayi = item.datapoint[1];
        //    //            showTooltip('Teklif', item.pageX, item.pageY, sayi);
        //    //        }
        //    //    }
        //    //});

        //    //$('#site_teklif, #load_statistics').bind("mouseleave", function () {
        //    //    $("#tooltip").remove();
        //    //});
        //},

        //initPoliceChart: function (police) {
        //    if (police.Trafik > 0) {
        //        var trafik = [
        //            [1, police.Trafik]
        //        ];
        //    }

        //    if (police.Kasko > 0) {
        //        var kasko = [
        //            [2, police.Kasko]
        //        ];
        //    }

        //    if (police.Dask > 0) {
        //        var dask = [
        //             [3, police.Dask]
        //        ];
        //    }

        //    if (police.KrediliHayat > 0) {
        //        var kredilihayat = [
        //            [4, police.KrediliHayat]
        //        ];
        //    }

        //    var previousPoint2 = null;
        //    $('#site_police_loading').hide();
        //    $('#site_police_content').show();


        //    var plot_activities = $.plot(
        //        $("#site_police"), [{
        //            data: trafik,
        //            color: "rgba(124,137,255, 0.7)",
        //            label: "Traffic " + police.Trafik,
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        },
        //        {
        //            data: kasko,
        //            color: "rgba(75,66,255, 0.7)",
        //            label: "Car Insurance " + police.Kasko,
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        },
        //        {
        //            data: dask,
        //            color: "rgba(20,90,255, 0.7)",
        //            label: "Dask " + police.Dask,
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        },
        //        {
        //            data: kredilihayat,
        //            color: "rgba(90,05,255, 0.7)",
        //            label: "Credit Life " + police.KrediliHayat,
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        }
        //        ], {
        //            series: {
        //                bars: {
        //                    show: true,
        //                    barWidth: 0.9
        //                }
        //            },
        //            grid: {
        //                show: true,
        //                hoverable: true,
        //                clickable: false,
        //                autoHighlight: true,
        //                borderWidth: 0
        //            },
        //            yaxis: {
        //                min: 0,
        //                max: police.Bar
        //            },
        //            xaxis: {
        //                show: false
        //            }
        //        });

        //    //$("#site_police").bind("plothover", function (event, pos, item) {
        //    //    $("#x").text(pos.x.toFixed(2));
        //    //    $("#y").text(pos.y.toFixed(2));
        //    //    if (item) {
        //    //        if (previousPoint2 != item.dataIndex) {
        //    //            previousPoint2 = item.dataIndex;
        //    //            $("#tooltip").remove();
        //    //            var x = item.datapoint[0],
        //    //                y = item.datapoint[1].toFixed(2);
        //    //            var sayi = item.datapoint[1];
        //    //            showTooltip('Police', item.pageX, item.pageY, sayi);
        //    //        }
        //    //    }
        //    //});

        //    //$('#site_police, #load_statistics').bind("mouseleave", function () {
        //    //    $("#tooltip").remove();
        //    //});

        //},

        //initPoliceTutarChart: function (policetutar) {
        //    var previousPoint2 = null;
        //    $('#site_policetutar_loading').hide();
        //    $('#site_policetutar_content').show();

        //    if (policetutar.TrafikTutar > 0) {
        //        var trafik = [
        //            [1, policetutar.TrafikTutar],
        //        ];
        //    }

        //    if (policetutar.TrafikKomisyon > 0) {
        //        var trafikKomisyon = [
        //            [2, policetutar.TrafikKomisyon],
        //        ];
        //    }

        //    if (policetutar.KaskoTutar > 0) {
        //        var kasko = [
        //            [3, policetutar.KaskoTutar]
        //        ];
        //    }

        //    if (policetutar.KaskoKomisyon > 0) {
        //        var kaskoKomisyon = [
        //            [4, policetutar.KaskoKomisyon]
        //        ];
        //    }

        //    if (policetutar.DaskTutar > 0) {
        //        var dask = [
        //            [5, policetutar.DaskTutar],
        //        ];
        //    }

        //    if (policetutar.DaskKomisyon > 0) {
        //        var daskKomisyon = [
        //            [6, policetutar.DaskKomisyon],
        //        ];
        //    }

        //    if (policetutar.KrediliHayatTutar > 0) {
        //        var kredilihayat = [
        //            [7, policetutar.KrediliHayatTutar]
        //        ];
        //    }

        //    if (policetutar.KrediliHayatKomisyon > 0) {
        //        var kredilihayatKomisyon = [
        //            [8, policetutar.KrediliHayatKomisyon]
        //        ];
        //    }

        //    var plot_activities = $.plot(
        //        $("#site_policetutar"), [{
        //            data: trafik,
        //            color: "rgba(132,206,255, 0.7)",
        //            label: "Traffic Premium " + policetutar.TrafikTutar,
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        },
        //        {
        //            data: trafikKomisyon,
        //            color: "rgba(132,206,255, 0.7)",
        //            label: "Traffic Commission " + policetutar.TrafikKomisyon,
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        },
        //        {
        //            data: kasko,
        //            color: "rgba(5,180,255, 0.7)",
        //            label: "Kasko Premium " + policetutar.KaskoTutar,
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        },
        //        {
        //            data: kaskoKomisyon,
        //            color: "rgba(5,180,255, 0.7)",
        //            label: "Kasko Commission " + policetutar.KaskoKomisyon,
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        }, {
        //            data: dask,
        //            color: "rgba(95,180,255, 0.7)",
        //            label: "Dask Premium " + policetutar.DaskTutar,
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        },
        //        {
        //            data: daskKomisyon,
        //            color: "rgba(90,180,255, 0.7)",
        //            label: "Dask Commission " + policetutar.DaskKomisyon,
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        },
        //        {
        //            data: kredilihayat,
        //            color: "rgba(5,70,255, 0.7)",
        //            label: "Credit Life Premium " + policetutar.KrediliHayatTutar,
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        },
        //        {
        //            data: kredilihayatKomisyon,
        //            color: "rgba(5,90,255, 0.7)",
        //            label: "Credit Life Commission " + policetutar.KrediliHayatKomisyon,
        //            shadowSize: 0,
        //            bars: {
        //                show: true,
        //                lineWidth: 0,
        //                fill: true,
        //                fillColor: {
        //                    colors: [{
        //                        opacity: 1
        //                    }, {
        //                        opacity: 1
        //                    }
        //                    ]
        //                }
        //            }
        //        }
        //        ], {
        //            series: {
        //                bars: {
        //                    show: true,
        //                    barWidth: 0.9
        //                }
        //            },
        //            grid: {
        //                show: true,
        //                hoverable: true,
        //                clickable: false,
        //                autoHighlight: true,
        //                borderWidth: 0
        //            },
        //            yaxis: {
        //                min: 0,
        //                max: policetutar.Bar
        //            },
        //            xaxis: {
        //                show: false
        //            }
        //        });

        //    //$("#site_policetutar").bind("plothover", function (event, pos, item) {
        //    //    $("#x").text(pos.x.toFixed(2));
        //    //    $("#y").text(pos.y.toFixed(2));
        //    //    if (item) {
        //    //        if (previousPoint2 != item.dataIndex) {
        //    //            previousPoint2 = item.dataIndex;
        //    //            $("#tooltip").remove();
        //    //            var x = item.datapoint[0],
        //    //                y = item.datapoint[1].toFixed(2);
        //    //            var sayi = item.datapoint[1];
        //    //            showTooltip('Tutar', item.pageX, item.pageY, sayi);
        //    //        }
        //    //    }
        //    //});

        //    //$('#site_policetutar, #load_statistics').bind("mouseleave", function () {
        //    //    $("#tooltip").remove();
        //    //});
        //},

        initChat: function () {

            var cont = $('#chats');
            var list = $('.chats', cont);
            var form = $('.chat-form', cont);
            var input = $('input', form);
            var btn = $('.btn', form);

            var handleClick = function () {
                var text = input.val();
                if (text.length == 0) {
                    return;
                }

                var time = new Date();
                var time_str = time.toString('MMM dd, yyyy hh:mm');
                var tpl = '';
                tpl += '<li class="out">';
                tpl += '<img class="avatar" alt="" src="assets/img/avatar1.jpg"/>';
                tpl += '<div class="message">';
                tpl += '<span class="arrow"></span>';
                tpl += '<a href="#" class="name">Bob Nilson</a>&nbsp;';
                tpl += '<span class="datetime">at ' + time_str + '</span>';
                tpl += '<span class="body">';
                tpl += text;
                tpl += '</span>';
                tpl += '</div>';
                tpl += '</li>';

                var msg = list.append(tpl);
                input.val("");
                $('.scroller', cont).slimScroll({
                    scrollTo: list.height()
                });
            }

            $('.scroller', cont).slimScroll({
                scrollTo: list.height()
            });

            btn.click(handleClick);
            input.keypress(function (e) {
                if (e.which == 13) {
                    handleClick();
                    return false; //<---- Add this line
                }
            });
        }

    };
}();