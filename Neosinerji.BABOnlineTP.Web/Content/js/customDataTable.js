//var obj = [1, 2, 3];
var temp = "", pdfTitle = "", pdfTitle2 = "";
var element, element2;
var contentIndex = 0;
var tempTest;
var cariHesapHareketYaslandirmaTablosuDahilmi = false;
var testElement;
//var columnAlignment = [{ alignment: 2, list: [6, 7, 8] }, { alignment: 2, list: [1, 2, 3, 4] }]
function createTable(tableName, dizi = [], tagDizi = [], pageName = "", pdfHeaderName = "", pageSize = "A4", columnAlignment = [{ alignment: 2, list: [6, 7, 8] }, { alignment: 2, list: [1, 2, 3, 4] }]) {
    var dizisagahizalanacakstunlar = [];
    //pageName o sayfaya özgü başlık yazabilmek için. 
    //pdfHeaderName pdf başlığını özelleştirmek için.
    //pageSize pdfin kağıt boyutu yani sayfa boyutu.
    //dizi paralarla alakalı sütunları içeriyor.
    //tagDizi html elementleri olan hücreler (yeşiltik leri evet yapıyor) 
    dizisagahizalanacakstunlar = columnAlignment;

    switch (pageName) {
        case "policelistesi":
            pdfTitle = $("#tvm_unvani").text().trim() + " " + pdfHeaderName + " " + $("#bas_bit_tarih").text().trim();
            contentIndex = 1;
            break;
        case "carihesaptekstresi":
            pdfTitle = $("#h2_donem").text().trim() + " " + $("#h3_carihesaptitle").text().trim() + "       " + $("#h5_cariHesapText").text().trim();
            pdfTitle2 = $("#h2_donem2").text().trim() + " " + $("#h3_carihesaptitle2").text().trim() + "       " + $("#h5_cariHesapText2").text().trim();
            contentIndex = 1;
            break;
        default:
            pdfTitle = "";
    }

    var buttonCommon = {
        stripHtml: false,
        exportOptions: {
            format: {
                body: function (data, row, column, node) {
                    result = data.toString();


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
                        result = result.toString().trim();
                    }

                    if ($.inArray(column, dizi) > -1) {
                        if ($.isNumeric(data.replaceAll('.', '').replace(',', '.')))
                            result = paraDuzenle(data)
                        else if ($.isNumeric(result.replaceAll('.', '').replace(',', '.')))
                            result = paraDuzenle(result)
                        //console.log(result+"<<<")
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
    function paraDuzenle(data) {
        var result = data

        if (parseFloat(data.replaceAll('.', '').replace(',', '.')) > -1000 && parseFloat(data.replaceAll('.', '').replace(',', '.')) < 1000) {
            result = parseFloat(data.replace(',', '.')).toFixed(2) 
        }
        else {
            var tempmoney = data.replaceAll('.', '').replaceAll(",", ".")
            var temptamkisim;
            var ondalikkisimbolum; 
             
            if (parseFloat(tempmoney)>=0) {
                temptamkisim = tempmoney.substr(0, tempmoney.indexOf("."));
                ondalikkisimbolum = tempmoney.substr(tempmoney.indexOf(".") + 1);
                result = parseFloat(temptamkisim) + (parseFloat(ondalikkisimbolum) / parseInt("1".padEnd(ondalikkisimbolum.length + 1, "0")))
                result = result.toFixed(2);
            }
            else {
                temptamkisim = tempmoney.substr(0, tempmoney.indexOf("."));
                ondalikkisimbolum = tempmoney.substr(tempmoney.indexOf(".") + 1);
                result = parseFloat(temptamkisim) + (parseFloat(ondalikkisimbolum) / parseInt("1".padEnd(ondalikkisimbolum.length + 1, "0"))*-1)
                result = result.toFixed(2);
            }
            //var tempmoney = data.replaceAll(",", ".") //eski kod
            //result = tempmoney;

            //if (tempmoney.indexOf(".") != tempmoney.lastIndexOf(".")) {
            //    var temptamkisim = tempmoney.substr(0, tempmoney.lastIndexOf(".")).replaceAll(".", "").trim().toString()
            //    var tempondalikkisim = tempmoney.substr(tempmoney.lastIndexOf(".") + 1).trim().toString()
            //    var ondalikkisimbolum = "1"
            //    //console.log(temptamkisim)
            //    //console.log(tempondalikkisim)
            //    //console.log(tempondalikkisim.length)
            //    for (var i = 0; i < tempondalikkisim.length; i++) {
            //        ondalikkisimbolum += "0";
            //    }
            //    console.log(ondalikkisimbolum)

            //    result = (parseFloat(temptamkisim) + parseFloat(tempondalikkisim / ondalikkisimbolum)).toFixed(2).toString();
            //}
        }
        return result.toString();
    }
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
    function createDocDefinition(tableName) {
        var columnCount = $(tableName + " > thead > tr > th").length;
        var tempColumnIndex = 0;
        var tempData = [];
        dataList = []
        var columnWidth = []
        var elementList = $(tableName + " > thead > tr > th")
        for (var i = 0; i < elementList.length; i++) {
            tempData.push({
                text: elementList.eq(i).text(),
                style: [{
                    color: "white",
                    bold: 'true',
                    fontSize: 11,
                    alignment: 'center'
                }]

            })
            columnWidth.push(70);
            //columnWidth.push($.inArray(i,columnWidthIndex)?60:"auto");
        }
        dataList.push(tempData);
        tempData = [];
        elementList = $(tableName + " > tbody > tr > td")
        for (var i = 0; i < elementList.length; i++) {
            tempData.push({
                text: elementList.eq(i).text().trim(),
                style: [{
                    alignment: dizisagahizalanacakstunlar[1] ? ($.inArray(tempColumnIndex, dizisagahizalanacakstunlar[1].list) > -1 ? (dizisagahizalanacakstunlar[1].alignment == 0 ? "left" : dizisagahizalanacakstunlar[1].alignment == 1 ? 'center' : "right") : "left") : "left"
                }]
            })
            tempColumnIndex++;
            if ((i + 1) % columnCount == 0) {
                dataList.push(tempData);
                tempData = [];
                tempColumnIndex = 0;
            }

        }
        var result = [dataList, columnWidth];
        return result;
    }

    $(tableName).DataTable({
        "columnDefs": [{ targets: dizi, className: 'dt-body-right' }],
        destroy: true,
        "bPaginate": true,
        searching: true,
        "ordering": false,
        dom: 'lBfrtip',
        'pageLength': 25,
        "lengthChange": true,
        'lengthMenu': [[10, 25, 50, 100, -1], [10, 25, 50, 100, 'All']],//sağ bölümdeki değişkenler görsel değerler
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
                titleAttr: tableName,
                customize: function (doc) {
                    //console.log(doc.content);
                    //policelistesi content =1
                    //carihesapekstire content =0
                    console.log($(this))
                    console.log(doc)

                    cariHesapHareketYaslandirmaTablosuDahilmi = $(this)[0].titleAttr == "#sample_2" ? false : $("input[id='PdfTip_0']:checked").val() ? true : false;
                    if (cariHesapHareketYaslandirmaTablosuDahilmi) {
                        var resultList = createDocDefinition("#sample_2")

                        docDefinition = [
                            {

                                table: {
                                    headerRows: 1,
                                    body: resultList[0],
                                    widths: resultList[1],
                                },
                                pageBreak: 'before',
                            },
                        ]

                        doc.content.push(docDefinition)
                        if (doc.content[2]) {
                            doc.content[2][0].layout = "noBorders"
                            //var tempTable2 = doc.content[2][0];
                            ////eklenen yeni tablonun header cell background color
                            //for (var i = 0; i < tempTable2.table.body[0].length; i++) {
                            //    tempTable2.table.body[0][i].fillColor = "#2D4154"

                            //}

                            var tableBody = doc.content[2][0].table.body;
                            //eklenen yeni tablonun header cell background color
                            for (var i = 0; i < tableBody[0].length; i++) {
                                tableBody[0][i].fillColor = "#2D4154"
                            }

                            for (var i = 1; i < tableBody.length; i++) {
                                if ((i + 1) % 2 == 0)
                                    for (var j = 0; j < tableBody[i].length; j++) {
                                        tableBody[i][j].fillColor = "#ddd"
                                    }
                            }
                        }
                    }
                    var docContent;
                    for (var i = 0; i < doc.content.length; i++) {
                        if (doc.content[i].table) {
                            docContent = doc.content[i].table;
                            break;
                        }
                    }
                    alignmentProcess(docContent, 1, 0)

                    //for (var i = 1; i < docContent.body.length; i++) {
                    //    for (var j = 0; j < dizi.length; j++) {
                    //        docContent.body[i][dizi[j]].alignment = 'right';
                    //    }
                    //}  

                }
            }
            ,
            'print'
        ],
    });

    $(".buttons-copy").html('<img src="/content/img/copy.png" width="25" height="25">')
    $(".buttons-print").html('<img src="/content/img/printer.png" width="25" height="25">')
    $(".buttons-csv").html('<img src="/content/img/csv.png" width="25" height="25">')
    $(".buttons-excel").html('<img src="/content/img/excel.png" width="25" height="25">')
    $(".buttons-pdf").html('<img src="/content/img/pdf.png" width="25" height="25">')
}

