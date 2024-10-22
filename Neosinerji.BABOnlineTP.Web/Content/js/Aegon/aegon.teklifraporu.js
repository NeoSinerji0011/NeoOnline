$(document).ready(function () {
    // ==== Tarih ayarları ==== //
    $('#BaslangicTarihi').change(function () {
        var d1 = $(this).datepicker("getDate");
        $("#BitisTarihi").datepicker("option", "minDate", d1);
        setTimeout(function () { $("#BitisTarihi").datepicker("show"); }, 100);
    });

    // ==== Para Formatı Belirleniyor.
    $(".autoNumeric").autoNumeric('init', { vMin: '0', vMax: '99999999', mDec: 0 });

    $("[rel=tooltip]").tooltip();

    //Arama Yapıyor..
    var oTable = $('.data-table').dataTable({
        "bPaginate": true,
        "bLengthChange": true,
        "bFilter": false,
        "bSort": false,
        "bInfo": true,
        "bProcessing": true,
        "aLengthMenu": [10, 25, 50, 100, 500],
        "bServerSide": true,
        "sAjaxSource": "/Rapor/AegonRapor/ListePagerTeklifRaporu",
        "bDeferLoading": true,
        "iDeferLoading": 0,
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
            aoData.push({ "name": "TeklifTarihi", "value": $("#TeklifTarihi").val() });
            aoData.push({ "name": "DovizTL", "value": $("#DovizTL").val() });
            aoData.push({ "name": "UrunSelectList", "value": $("#UrunSelectList").val() });
            aoData.push({ "name": "TVMLerSelectList", "value": $("#TVMLerSelectList").val() });
            aoData.push({ "name": "BaslangicTarihi", "value": $("#BaslangicTarihi").val() });
            aoData.push({ "name": "BitisTarihi", "value": $("#BitisTarihi").val() });
            aoData.push({ "name": "TeklifNo", "value": $("#TeklifNo").val() });
            aoData.push({ "name": "ParaBirimi", "value": $("#ParaBirimi").val() });
            aoData.push({ "name": "BolgeKodu", "value": $("#BolgeKodu").val() });
            aoData.push({ "name": "YillikPrimMin", "value": $("#YillikPrimMin").autoNumeric('get') });
            aoData.push({ "name": "YillikPrimMax", "value": $("#YillikPrimMax").autoNumeric('get') });
        },
        "fnDrawCallback": function () {
            $("#search").button("reset");
            jQuery('.popovers').popover();
        }
    });

    $("#search").click(function () {
        $(this).button("loading");
        oTable.fnDraw();
    });
});

//$.fn.dataTable.defaults.aLengthMenu = [[100, 200, 500, 1000], [100, 200, 500, 1000]];

$("#BolgeKodu").change(function () {
    var bolgeKodu = $(this).val();

    $.ajax({
        url: "/Rapor/AegonRapor/GetTVMByBolgeKodu",
        type: "post",
        data: { BolgeKodu: bolgeKodu },
        success: function (data) {
            $("#TVMLerSelectList").dropDownFill(data);
        },
        error: function () { alert("Acenteler getirilirken bir hata oluştu.") }
    })
});

$(".on-provizyon-detay").live("click", function () {

    try {
        var teklifId = $(this).attr("teklif-id");

        if (teklifId !== undefined && teklifId != "") {
            $.ajax({
                url: "/Rapor/AegonRapor/GetOnProvizyonDetay",
                method: "post",
                data: { teklifId: teklifId },
                success: function (result) {

                    if (result !== undefined && result != "") {
                        $("#onprovizyon-modal-div").html(result);
                        $("#onprovizyon-detay-modal").modal("show");
                    }
                    else { alert("Ön provizyon detay getirilirken bir hata oluştu!") }
                },
                error: function () { alert("Ön provizyon detay getirilirken bir hata oluştu!") }
            });
        }

    } catch (e) {
        alert(e.message);
    }
});

$("#tumunu-excel-aktar").click(function () {

    try {

        // #region Model Set

        var link = "TeklifTarihi=" + $("#TeklifTarihi").val();
        link += "&DovizTL=" + $("#DovizTL").val();
        link += "&UrunSelectList=" + $("#UrunSelectList").val();
        link += "&TVMLerSelectList=" + $("#TVMLerSelectList").val();
        link += "&BaslangicTarihi=" + $("#BaslangicTarihi").val();
        link += "&BitisTarihi=" + $("#BitisTarihi").val();
        link += "&TeklifNo=" + $("#TeklifNo").val();
        link += "&ParaBirimi=" + $("#ParaBirimi").val();
        link += "&BolgeKodu=" + $("#BolgeKodu").val();
        link += "&YillikPrimMin=" + $("#YillikPrimMin").val();
        link += "&YillikPrimMax=" + $("#YillikPrimMax").val();

        window.open("/Rapor/AegonRapor/GetAegonRaporAll?" + link);

        //#endregion

    } catch (e) {
        alert(e.message);
    }

});