var OzetRapor = new function () {

    return {
        SeceneklerClick: function (raporKodu) {
            $("#AramaKriteri").val(raporKodu);
        },

        BtnClear: function () {
            $(oTable.fnSettings().aoData).each(function () {
                $(this.nTr).removeClass('row_selected');
            });

            $("#geri-listesi").html('');
        }
    }
}

$(".ozet-rapor-value").click(function () {

    var raporKodu = $(this).attr("rapor-kodu");
    OzetRapor.SeceneklerClick(raporKodu);

});

$("#DataTables_Table_0 tbody").click(function (event) {

    $(oTable.fnSettings().aoData).each(function () {
        $(this.nTr).removeClass('row_selected');
    });

    $(event.target.parentNode).addClass('row_selected');
    var id = event.target.parentNode.cells[0].innerHTML

    $("#BirincilData").val(id);
});

$("#btn-clear").click(function () {
    OzetRapor.BtnClear();
});