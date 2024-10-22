var OdemeGuvenceIlgiliUrun = new function () {
    return {
        Init: function () {
            setTimeout(function () {

                $(".sigortali-bilgi-girisi-readonly").attr("readonly", true);
                $("#sigortaettiren-sorgula").attr("disabled", true);


                // MUSTERI GELIR VERGISI ORANI
                var gelirVergisiOrani = $("#Musteri_SigortaEttiren_GelirVergisiOrani").val();
                $("#Musteri_SigortaEttiren_GelirVergisiOrani").attr("disabled", true);
                $("<input name='Musteri.SigortaEttiren.GelirVergisiOrani' id='Musteri_SigortaEttiren_GelirVergisiOrani' value='" + gelirVergisiOrani + "'" +
                          "type='hidden'></input>").appendTo("#form2");

                // Musteri_SigortaEttiren_IlKodu
                var Musteri_SigortaEttiren_IlKodu = $("#Musteri_SigortaEttiren_IlKodu").val();
                $("#Musteri_SigortaEttiren_IlKodu").attr("disabled", true);
                $("<input name='Musteri.SigortaEttiren.IlKodu' class='ignore' value='" + Musteri_SigortaEttiren_IlKodu + "' type='hidden'></input>").appendTo("#form2");

                // Musteri_SigortaEttiren_IlceKodu
                var Musteri_SigortaEttiren_IlceKodu = $("#Musteri_SigortaEttiren_IlceKodu").val();
                $("#Musteri_SigortaEttiren_IlceKodu").attr("disabled", true);
                $("<input name='Musteri.SigortaEttiren.IlceKodu' class='ignore' value='" + Musteri_SigortaEttiren_IlceKodu + "' type='hidden'></input>").appendTo("#form2");

                // Musteri_SigortaEttiren_DogumTarihi
                var Musteri_SigortaEttiren_DogumTarihi = $("#Musteri_SigortaEttiren_DogumTarihi").val();
                $("#Musteri_SigortaEttiren_DogumTarihi").datepicker("option", "disabled", true);
                $("<input name='Musteri.SigortaEttiren.DogumTarihi' value='" + Musteri_SigortaEttiren_DogumTarihi + "' type='hidden'></input>").appendTo("#form2");

                // GenelBilgiler_primOdemeDonemi
                var GenelBilgiler_primOdemeDonemi = $("#GenelBilgiler_primOdemeDonemi").val();
                $("#GenelBilgiler_primOdemeDonemi").attr("disabled", true);
                $("<input name='GenelBilgiler.primOdemeDonemi' id='GenelBilgiler_primOdemeDonemi' value='" + GenelBilgiler_primOdemeDonemi + "'" +
                         "type='hidden'></input>").appendTo("#form3");

                $("#form2 input:radio:not(:checked)").attr("disabled", true);
            }, 500);
        }

    }
}