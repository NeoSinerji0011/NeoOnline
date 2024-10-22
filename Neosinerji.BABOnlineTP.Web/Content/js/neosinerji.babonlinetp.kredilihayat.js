$(document).ready(function () {

    sigortaliKontrol.init();
    if (typeof FormWizard !== "undefined") {
        FormWizard.init(krediHayatTeklifWizardCallback);
    }
    teklifOdeme.init();

    $("#pdf-karsilastirma").live("click", function () {
        var url = $(this).attr("pdf");
        window.open(url, "_blank");
    });

    $("#btn-teklif-tekrar").live("click", teklifFiyat.teklifTekrar);

    $("#Adres_UlkeKodu").ulke({ il: '#Adres_IlKodu', ilce: '#Adres_IlceKodu' });

    $("#Musteri_SigortaEttiren_IlKodu").change(function () {
        $("#Adres_IlKodu").val($(this).val());
        $("#Adres_IlceKodu").ilceler("TUR", $(this).val());
    });
    $("#Musteri_SigortaEttiren_IlceKodu").change(function () {
        $("#Adres_IlceKodu").val($(this).val());
    });

    $("#btn-hesapla").click(function () {
        var isvalid = $("#form3").valid();
        if (isvalid) {
            $("#TeklifId").val("");

            $(this).button("loading");

            $(".switcher").find(":input").switchFix();

            var contents = $("#form1, #form2, #form3").serialize();

            $.ajax(
                {
                    type: "POST",
                    url: "/Teklif/KrediliHayat/Hesapla",
                    data: contents,
                    success: function (data) {
                        if (data.id > 0) {
                            teklifFiyat.kontol({ processId: data.id, guid: data.g });

                            $("#step3group").css({ "visibility": "visible" });
                            $("#teklif-fiyat-container").css({ "visibility": "visible" });
                            $("#step1").collapse("hide");
                            $("#step2").collapse("hide");
                            $("#step3").collapse("show");
                        }

                        $("#btn-hesapla").button("reset");
                        $("#fiyat-container").html($("#fiyat-container-template").html());
                        $('#form_wizard_1').bootstrapWizard("next");
                    },
                    error: function () {
                        $("#btn-hesapla").button("reset");
                    }
                });
        }
    });

});


function krediHayatTeklifWizardCallback(current) {
    //Hazırlayan bilgileri
    if (current == 1) {

    }
        //Sigorta ettiren / sigortali tab
    else if (current == 2) {

        var dogumTarihi = $("#Musteri_SigortaEttiren_DogumTarihi").val();
        var yil = dogumTarihi.substring(6, 11);
        var gecerliYil = ((new Date).getFullYear());

        if (yil > gecerliYil - 18 || yil < gecerliYil - 54) {
            alert("Yaş Aralığı 18 - 54 olmalıdır.");
            return false;
        }

        if (!$("#Musteri_SigortaEttiren_AdiUnvan").is(":visible")) {
            $("#sigortaettiren-sorgula").trigger("click");
            return false;
        }

        return sigortaliKontrol.Kaydet();
    }
        // Kredi / Adres bilgileri
    else if (current == 3) {
        var isValid = FormWizard.validatePage('#tab3');
        return isValid == 1;
    }

    return true;
}

var teklifOdeme = new function () {
    return {

        init: function () {
            $("#kk-odeme-btn").click(teklifOdeme.kredikarti);
            $(".teklif-satin-al").live("click", teklifOdeme.kredikartiOdeme);
            $("#kk-odeme").on("shown", function () { $("#KrediKarti_KartSahibi").focus(); })
        },

        kredikartiOdeme: function () {
            var teklifId = $(this).attr("teklif-id");
            var fiyat = $(this).attr("fiyat");
            if (teklifId && fiyat) {
                $("#KrediKarti_KK_TeklifId").val(teklifId);
                $("#kk-tutar").html(fiyat);
                $("#kk-odeme").modal("show");
            }
        },

        kredikarti: function () {
            var isvalid = $("#krediKartiForm").valid();
            if (isvalid) {
                //$("#krediKartiForm").find("#KrediKarti_KartNumarasi").addClass("ignore");
                var kartnoVal = $("#krediKartiForm")[0].KrediKarti_KartNumarasi;
                $(kartnoVal).remove();

                $("#kredi-karti-error").hide();
                $(this).button("loading");
                $("#kk-odeme-cancel").hide();

                var contents = $("#krediKartiForm").serialize();

                $.ajax({
                    type: "POST",
                    url: "/Teklif/KrediliHayat/OdemeAl",
                    data: contents,
                    success: function (data) {

                        if (data.Success) {
                            window.location.href = data.RedirectUrl;
                            return;
                        }
                        else if (data.Hatalar != null && data.Hatalar.length > 0) {
                            $("#kredi-karti-error-list").html("");

                            for (var i in data.Hatalar) {
                                var hata = data.Hatalar[i];

                                $("#kredi-karti-error-list").append("<span>" + hata + "</span><br/>");
                            }

                            $("#kredi-karti-error").slideDown("fast");
                        }

                        $("#kk-odeme-btn").button("reset");
                        $("#kk-odeme-cancel").show();
                       
                    },
                    error: function () {
                        $("#kk-odeme-btn").button("reset");
                        $("#kk-odeme-cancel").show();
                       
                    }
                });
            }
        }
    }
}