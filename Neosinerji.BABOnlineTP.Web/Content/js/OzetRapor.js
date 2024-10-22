var ozetRaporClass = new function () {
    return {
        setNextDataValue: function (value) {
            if (value == 1) {
                $("#opsiyonel").html("Ana Taliler");
                $("#data3").val("tvm");
            }
            else if (value == 2) {
                $("#opsiyonel").html("Şubeler");
                $("#data3").val("tvm");
            }
            else if (value == 3) {
                $("#opsiyonel").html("Ürünler");
                $("#data3").val("urun");
            }
            else if (value == 4) {
                $("#opsiyonel").html("Sigorta Şirketleri");
                $("#data3").val("tum");
            }
            else if (value == 5) {
                $("#opsiyonel").html("Ödeme Seçenekleri");
                $("#data3").val("os");
            }
            else if (value == 6)
                $("#opsiyonel").html("Satış Türü");
            else if (value == 7) {
                $("#opsiyonel").html("Müşteri Temsilcisi");
                $("#data3").val("mus");
            }
            else if (value == 8) {
                $("#opsiyonel").html("Bölgeler");
                $("#data3").val("bol");
            }
        },

        setPreviewDataValue: function (yenidata) {
            var li = "";
            var asildeger = $("#asildeger").val();
            var data2 = $("#data2").val();
            var data3 = $("#data3").val();
            var text = "";

            if (asildeger == 0)
                $("#geri-listesi").html('');

            if (yenidata == 1) { li = "#anatali"; text = langu.AnaTali; }
            else if (yenidata == 2) { li = "#sube"; text = langu.Subeler; }
            else if (yenidata == 3) { li = "#urun"; text = langu.Urunler; }
            else if (yenidata == 4) { li = "#tum"; text = langu.TUM; }
            else if (yenidata == 5) { li = "#odeme"; text = langu.Odemeler; }
            else if (yenidata == 6) { li = "#satis"; text = langu.SatisTuru; }
            else if (yenidata == 7) { li = "#mt"; text = langu.MT; }
            else if (yenidata == 8) { li = "#bolge"; text = langu.Bolgeler; }

            var paddinleft = 13;
            var length = $("#geri-listesi").children().length
            if (length > 0)
                paddinleft = parseInt($("#geri-listesi").children("*:last").children().css("padding-left"));

            if (li != "") {
                $("#geri-listesi").append('<li id="' + li + '"><a style="padding-left:' + (paddinleft + 5) + 'px;" class="preview-list-item" href="javascript:;" ' +
                                          'custom-value="' + yenidata + '" asildeger="' + asildeger + '" data2="' + data2 + '" data3="' + data3 + '">' + text + '</a></li>');
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
        "sAjaxSource": "/Rapor/Rapor/ListePagerOzetRaporu",
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
            aoData.push({ "name": "BransSelectList", "value": $("#BransSelectList").val() });
            aoData.push({ "name": "UrunSelectList", "value": $("#UrunSelectList").val() });
            aoData.push({ "name": "PoliceTarihi", "value": $("#PoliceTarihi").val() });
            aoData.push({ "name": "OdemeTipi", "value": $("#OdemeTipi").val() });
            
            aoData.push({ "name": "BaslangicTarihi", "value": $("#BaslangicTarihi").val() });
            aoData.push({ "name": "BitisTarihi", "value": $("#BitisTarihi").val() });
            aoData.push({ "name": "TahsIptal", "value": $("#TahsIptal").val() });
            aoData.push({ "name": "DovizTL", "value": $("#DovizTL").val() });

            aoData.push({ "name": "aramakriteri", "value": $("#aramakriteri").val() });
            aoData.push({ "name": "asildeger", "value": $("#asildeger").val() });
            aoData.push({ "name": "data2", "value": $("#data2").val() });
            aoData.push({ "name": "data3", "value": $("#data3").val() });
        }
    });

    $("#search").click(function () {
        var value = "1"
        $("#aramakriteri").val(value);

        var sayac = 0;
        $(oTable.fnSettings().aoData).each(function () {
            if ($(this.nTr).hasClass('row_selected'))
                sayac++;
        });
        if (sayac == 0)
            $("#data3").val("");

        //Geri butonu işlevleri veriliyor.
        ozetRaporClass.setPreviewDataValue(value);

        oTable.fnDraw();
        $("#asildeger").val(0);

        //ileri butonu işlevleri veriliyor.
        ozetRaporClass.setNextDataValue(value);
    });

    $(".preview-list-item").live("click", function () {
        $("#asildeger").val($(this).attr("asildeger"));
        $("#data2").val($(this).attr("data2"));
        $("#data3").val($(this).attr("data3"));
        $("#aramakriteri").val($(this).attr("custom-value"));

        $(this).parent().nextAll().remove();

        oTable.fnDraw();
        ozetRaporClass.setNextDataValue($(this).attr("custom-value"));
    });

    $("#DataTables_Table_0 tbody").click(function (event) {
        $(oTable.fnSettings().aoData).each(function () {
            $(this.nTr).removeClass('row_selected');
        });
        $(event.target.parentNode).addClass('row_selected');
        var id = event.target.parentNode.cells[0].innerHTML
        $("#asildeger").val(id);

        if ($("#data3").val() == "mus")
            $("#data2").val(id);
    });

    $("#btn-clear").click(function () {
        $(oTable.fnSettings().aoData).each(function () {
            $(this.nTr).removeClass('row_selected');
        });
        $("#geri-listesi").html('');
    });

    $(".ozet-rapor-value").click(function () {
        var value = $(this).attr("rapor-kodu");
        var eskidata = $("#aramakriteri").val();
        if (value === undefined) return;
        if (value > 0 & value < 9) {
            $("#aramakriteri").val(value);

            var sayac = 0;
            $(oTable.fnSettings().aoData).each(function () {
                if ($(this.nTr).hasClass('row_selected'))
                    sayac++;
            });
            if (sayac == 0)
                $("#data3").val("");

            //Geri butonu işlevleri veriliyor.
            ozetRaporClass.setPreviewDataValue(value);

            oTable.fnDraw();
            $("#asildeger").val(0);

            //ileri butonu işlevleri veriliyor.
            ozetRaporClass.setNextDataValue(value);
        }
    });
});