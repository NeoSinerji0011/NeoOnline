var aracRaporClass = new function () {
    return {
        setNextDataValue: function (value) {
            if (value == 1) {
                $("#opsiyonel").html("Marka");
                $("#sorguturu").val("marka");

            }
            else if (value == 2) {
                //Kullanım tipi kriteri belirleniyor.
                var tur = $("#sorguturu").val();
                switch (tur) {
                    case "marka": $("#data2").val('1'); break;
                    case 'tip': $("#data2").val('3'); break;
                    case 'ksekli': $("#data2").val('4'); break;
                    case 'ktarzi': $("#data2").val('5'); break;
                    default: $("#data2").val('0'); break;
                }
                $("#opsiyonel").html("Model");
                $("#sorguturu").val("model");
            }
            else if (value == 3) {
                //Kullanım tipi kriteri belirleniyor.
                var tur = $("#sorguturu").val();

                switch (tur) {
                    case "marka": $("#data2").val('1'); break;
                    case 'model': $("#data2").val('2'); break;
                    case 'ksekli': $("#data2").val('4'); break;
                    case 'ktarzi': $("#data2").val('5'); break;
                    default: $("#data2").val('0'); break;
                }
                $("#opsiyonel").html("Tip");
                $("#sorguturu").val("tip");
            }
            else if (value == 4) {
                $("#opsiyonel").html("K. Şekli");
                $("#sorguturu").val("ksekli");
            }
            else if (value == 5) {
                //Kullanım tipi kriteri belirleniyor.
                var tur = $("#sorguturu").val();
                switch (tur) {
                    case "marka": $("#data2").val('1'); break;
                    case 'model': $("#data2").val('2'); break;
                    case 'tip': $("#data2").val('3'); break;
                    case 'ksekli': $("#data2").val('4'); break;
                    default: $("#data2").val('0'); break;
                }
                $("#opsiyonel").html("K. Tarzı");
                $("#sorguturu").val("ktarzi");
            }
        },

        setPreviewDataValue: function (yenidata) {
            var li = "";
            var data = $("#kodu").val();
            var data2 = $("#data2").val();
            var sorguturu = $("#sorguturu").val();
            var text = "";

            if (data == 0)
                $("#geri-listesi").html('');

            if (yenidata == 1) { li = "#marka"; text = langu.Marka; }
            else if (yenidata == 2) { li = "#model"; text = langu.Model; }
            else if (yenidata == 3) { li = "#tip"; text = langu.Tip; }
            else if (yenidata == 4) { li = "#ksekli"; text = langu.K_Sekli; }
            else if (yenidata == 5) { li = "#ktarzi"; text = langu.K_Tarzi; }

            var paddinleft = 13;
            var length = $("#geri-listesi").children().length
            if (length > 0)
                paddinleft = parseInt($("#geri-listesi").children("*:last").children().css("padding-left"));

            if (li != "") {
                $("#geri-listesi").append('<li id="' + li + '"><a style="padding-left:' + (paddinleft + 5) + 'px;" class="preview-list-item" href="javascript:;" ' +
                                          'custom-value="' + yenidata + '" kodu="' + data + '" data2="' + data2 + '" sorguturu="' + sorguturu + '">' + text + '</a></li>');
            }
        },

        setDisabledListItem: function (item) {
            if (item == 1) {
                //$("#list-marka").attr("class", "disabled");
                //$("#list-model").removeClass("disabled");
                //$("#list-tip").removeClass("disabled");
                //$("#list-sekli").removeClass("disabled");
                //$("#list-tarzi").removeClass("disabled");
            } else if (item == 2) {
                $("#list-marka").removeClass("disabled");
                $("#list-model").attr("class", "disabled");
                $("#list-tip").removeClass("disabled");
                $("#list-sekli").removeClass("disabled");
                $("#list-tarzi").removeClass("disabled");
            }
            else if (item == 3) {
                $("#list-marka").removeClass("disabled");
                $("#list-model").removeClass("disabled");
                $("#list-tip").attr("class", "disabled");
                $("#list-sekli").removeClass("disabled");
                $("#list-tarzi").removeClass("disabled");
            }
            else if (item == 4) {
                $("#list-marka").removeClass("disabled");
                $("#list-model").removeClass("disabled");
                $("#list-tip").removeClass("disabled");
                $("#list-sekli").attr("class", "disabled");
                $("#list-tarzi").removeClass("disabled");
            }
            else if (item == 5) {
                $("#list-marka").removeClass("disabled");
                $("#list-model").removeClass("disabled");
                $("#list-tip").removeClass("disabled");
                $("#list-sekli").removeClass("disabled");
                $("#list-tarzi").attr("class", "disabled");
            }
        }
    }
}

$(document).ready(function () {
    //Arama Yapıyor..
    var oTable = $('.data-table').dataTable({
        "bPaginate": true,
        "bLengthChange": true,
        "bFilter": false,
        "bSort": false,
        "bInfo": true,
        "bProcessing": true,
        "bServerSide": true,
        "sAjaxSource": "/Rapor/Rapor/ListePagerAracSigortalariIstatistikRaporu",
        "bDeferLoading": true,
        "iDeferLoading": 0,
        "sPaginationType": "full_numbers",
        "sPaginationType": "bootstrap",
        "sPaginationType": "full_numbers",
        "sPaginationType": "bootstrap",
        "show": function (event, ui) {
            var jqTable = $('table.display', ui.panel);
            if (jqTable.length > 0) {
                var oTableTools = TableTools.fnGetInstance(jqTable[0]);
                if (oTableTools != null && oTableTools.fnResizeRequired()) {
                    jqTable.dataTable().fnAdjustColumnSizing();
                    oTableTools.fnResizeButtons();
                }
            }
        },
        "sDom": 'T<"clear">lfrtip',
        "oTableTools": { "aButtons": ["pdf", "xls", "print", "copy"] },
        "fnServerParams": function (aoData) {
            aoData.push({ "name": "TVMKodu", "value": $("#TVMKodu").val() });
            aoData.push({ "name": "Urun", "value": $("#Urun").val() });
            aoData.push({ "name": "PoliceTarihi", "value": $("#PoliceTarihi").val() });
            aoData.push({ "name": "DovizTL", "value": $("#DovizTL").val() });
            aoData.push({ "name": "BaslangicTarihi", "value": $("#BaslangicTarihi").val() });
            aoData.push({ "name": "BitisTarihi", "value": $("#BitisTarihi").val() });
            aoData.push({ "name": "TahsIptal", "value": $("#TahsIptal").val() });
            aoData.push({ "name": "customvalue", "value": $("#customvalue").val() });
            aoData.push({ "name": "kodu", "value": $("#kodu").val() });
            aoData.push({ "name": "data2", "value": $("#data2").val() });
            aoData.push({ "name": "sorguturu", "value": $("#sorguturu").val() });
        }
    });

    //Boş Arama Yapılıyor.
    $("#search").live("click", function () {
        $("#sorguturu").val('marka');
        oTable.fnDraw();
        $("#data2").val('');
    });

    $(".preview-list-item").live("click", function () {
        $("#kodu").val($(this).attr("kodu"));
        $("#data2").val($(this).attr("data2"));
        $("#sorguturu").val($(this).attr("sorguturu"));
        $("#customvalue").val($(this).attr("custom-value"));

        $(this).parent().nextAll().remove();

        oTable.fnDraw();
        aracRaporClass.setNextDataValue($(this).attr("custom-value"));
    });

    $("#DataTables_Table_0 tbody").click(function (event) {
        $(oTable.fnSettings().aoData).each(function () {
            $(this.nTr).removeClass('row_selected');
        });
        $(event.target.parentNode).addClass('row_selected');
        var id = event.target.parentNode.cells[0].innerHTML
        $("#kodu").val(id);
    });


    //Temizleme yapıyor.
    $("#btn-clear").click(function () {
        $(oTable.fnSettings().aoData).each(function () {
            $(this.nTr).removeClass('row_selected');
        });
        $("#geri-listesi").html('');
        $("#data2").val('');
        $("#kodu").val('');
    });

    $(".ozet-rapor-value").click(function () {
        var value = $(this).attr("rapor-kodu");
        var eskidata = $("#customvalue").val();
        if (value === undefined) return;
        if (value > 0 && value < 6) {
            $("#customvalue").val(value);

            //var sayac = 0;
            //$(oTable.fnSettings().aoData).each(function () {
            //    if ($(this.nTr).hasClass('row_selected'))
            //        sayac++;
            //});
            //if (sayac == 0)
            //    $("#sorguturu").val("marka");

            //ileri butonu işlevleri veriliyor.
            aracRaporClass.setNextDataValue(value);

            //Geri butonu işlevleri veriliyor.
            aracRaporClass.setPreviewDataValue(value);

            oTable.fnDraw();
            $("#kodu").val(0);

        }
    });



    function AramaYap() {

    }
});
