
function createTable(dizi = [], tagDizi = [], pageName = "", pdfHeaderName = "", pageSize = "A2", columnAlignment = [{ alignment: 2, list: [6, 7, 8] }, { alignment: 2, list: [1, 2, 3, 4] }], pdfTitle = "Rapor") {

    var dizisagahizalanacakstunlar = [];
    //pageName o sayfaya özgü başlık yazabilmek için. 
    //pdfHeaderName pdf başlığını özelleştirmek için.
    //pageSize pdfin kağıt boyutu yani sayfa boyutu.
    //dizi paralarla alakalı sütunları içeriyor.
    //tagDizi html elementleri olan hücreler (yeşiltik leri evet yapıyor) 
    dizisagahizalanacakstunlar = columnAlignment;

    var buttonCommon = {
        stripHtml: false,
        exportOptions: {
            format: {
                body: function (data, row, column, node) {
                    result = data;


                    if ($.inArray(column, tagDizi) > -1) {
                        temp = result;

                        if (temp.toLowerCase().indexOf("yesiltik.png") >= 0) {
                            result = "Evet";
                        }
                        else if (temp.toLowerCase().indexOf("<img") >= 0)
                            result = "";
                        else {
                            element = $.parseHTML(result)
                            if (element == null)
                                result = data;
                            else if (element[0].nodeName.indexOf("text") >= 0)
                                result = data;
                            else if (element != null) {
                                element2 = $.parseHTML(element[0].innerHTML)
                                if (element2[0].nodeName.indexOf("text") >= 0)
                                    result = element2[0].textContent;
                            }
                        }
                    }

                    if ($.inArray(column, dizi) > -1) {
                        if ($.isNumeric(data.replace('.', '').replace(',', '.')))
                            if (parseFloat(data.replace('.', '').replace(',', '.')) > -1000 && parseFloat(data.replace('.', '').replace(',', '.')) < 1000) {
                                result = data.replace(',', '.');

                            }
                    }
                    if (result) {
                        result = result.replace('<span class="icon icon-usd"></span>', "$")
                        result = result.replace('<span class="icon icon-euro"></span>', "€")
                        result = result.replace('<span class="icon icon-gbp"></span>', "£")
                        result = result.replace('</b>', '')
                        result = result.replace('<b>', '')
                    }

                    return result;
                }
            }
        }
    };
    //Arama Yapıyor..
    var oTable = $('.data-table').dataTable({
        "bPaginate": true,
        "bLengthChange": true,
        searching: true,
        "bFilter": false,
        "bSort": false,
        "bInfo": true,
        "bProcessing": true,
        "bServerSide": true,
        "sAjaxSource": "/Rapor/Rapor/ListePagerPoliceRapor",
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
        "aoColumns": [
            null, null, null, null, null, null, null, null, null, null,
            { "sClass": "right" },
            null, null, null, null, null, null, null, null, null, null

        ],
        dom: 'Bfrtip',
        buttons: [
            'copy', 'csv',
            $.extend(true, {}, buttonCommon, {
                extend: 'excelHtml5'
            }),
            {
                extend: 'pdfHtml5',
                title: pdfTitle,
                orientation: 'landscape',
                pageSize: pageSize,
                text: '<i class="fa fa-file-pdf-o"> PDF</i>',
                titleAttr: "",
                customize: function (doc) {
                     
                     
                    var docContent;
                    for (var i = 0; i < doc.content.length; i++) {
                        if (doc.content[i].table) {
                            docContent = doc.content[i].table;
                            break;
                        }
                    }
                    alignmentProcess(docContent, 1, 0)
  
                }
            }
            ,
            'print'
        ],
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
            aoData.push({ "name": "PoliceTarihi", "value": $("#PoliceTarihi").val() });
            aoData.push({ "name": "DovizTL", "value": $("#DovizTL").val() });
            aoData.push({ "name": "BransSelectList", "value": $("#BransSelectList").val() });
            aoData.push({ "name": "UrunSelectList", "value": $("#UrunSelectList").val() });

            aoData.push({ "name": "BaslangicTarihi", "value": $("#BaslangicTarihi").val() });
            aoData.push({ "name": "BitisTarihi", "value": $("#BitisTarihi").val() });
            aoData.push({ "name": "TahsIptal", "value": $("#TahsIptal").val() });
            aoData.push({ "name": "TVMLerSelectList", "value": $("#TVMLerSelectList").val() });

            aoData.push({ "name": "PoliceNo", "value": $('#PoliceNo').val() });
            aoData.push({ "name": "OdemeTipi", "value": $("#OdemeTipi").val() });
            aoData.push({ "name": "OdemeSekli", "value": $("#OdemeSekli").val() });
            aoData.push({ "name": "Durumu", "value": $("#Durumu").val() });

            oSettings.jqXHR = $.ajax({
                "dataType": 'json',
                "type": "POST",
                "url": sSource,
                "data": aoData,
                "success": fnCallback
            });
        },
        //"fnServerParams": function (aoData) {
        //    aoData.push({ "name": "PoliceTarihi", "value": $("#PoliceTarihi").val() });
        //    // aoData.push({ "name": "DovizTL", "value": $("#DovizTL").val() });
        //    aoData.push({ "name": "BransSelectList", "value": $("#BransSelectList").val() });
        //    aoData.push({ "name": "UrunSelectList", "value": $("#UrunSelectList").val() });

        //    aoData.push({ "name": "BaslangicTarihi", "value": $("#BaslangicTarihi").val() });
        //    aoData.push({ "name": "BitisTarihi", "value": $("#BitisTarihi").val() });
        //    // aoData.push({ "name": "TahsIptal", "value": $("#TahsIptal").val() });
        //    aoData.push({ "name": "TVMLerSelectList", "value": $("#TVMLerSelectList").val() });

        //    aoData.push({ "name": "PoliceNo", "value": $('#PoliceNo').val() });
        //    aoData.push({ "name": "OdemeTipi", "value": $("#OdemeTipi").val() });
        //    aoData.push({ "name": "OdemeSekli", "value": $("#OdemeSekli").val() });
        //    //aoData.push({ "name": "Durumu", "value": $("#Durumu").val() });

        //},
        "fnDrawCallback": function () {
            $("#search").button("reset");
        }
    });

    $("#search").click(function () {
        $(this).button("loading");
        oTable.fnDraw();
    });

    $(".buttons-copy").html('<img src="/content/img/copy.png" width="25" height="25">')
    $(".buttons-print").html('<img src="/content/img/printer.png" width="25" height="25">')
    $(".buttons-csv").html('<img src="/content/img/csv.png" width="25" height="25">')
    $(".buttons-excel").html('<img src="/content/img/excel.png" width="25" height="25">')
    $(".buttons-pdf").html('<img src="/content/img/pdf.png" width="25" height="25">')


    function alignmentProcess(docContent, alignment = 0) {
        //hizalama işlemi 
        for (var i = 1; i < docContent.body.length; i++) {
            //if (dizisagahizalanacakstunlar[0])
            //    for (var j = 0; j < dizisagahizalanacakstunlar[0].list.length; j++) {
            //        docContent.body[i][dizisagahizalanacakstunlar[0].list[j]].alignment = dizisagahizalanacakstunlar[0].alignment == 0 ? "left" : dizisagahizalanacakstunlar[0].alignment == 1 ? 'center' : "right";
            //    }

            for (var j = 0; j < dizi.length; j++) {
                docContent.body[i][dizi[j]].alignment = dizisagahizalanacakstunlar[0].alignment == 0 ? "left" : dizisagahizalanacakstunlar[0].alignment == 1 ? 'center' : "right";
            }
        }
    }
}
