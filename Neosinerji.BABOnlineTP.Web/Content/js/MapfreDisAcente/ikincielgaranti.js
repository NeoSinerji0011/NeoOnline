var ikinciElGarantiOdeme = new function () {
    return {

        init: function () {
            $("#kk-odeme-btn").click(ikinciElGarantiOdeme.kredikarti);
            $(".teklif-satin-al").live("click", ikinciElGarantiOdeme.kredikartiOdeme);
            $("#kk-odeme").on("shown", function () { $("#KrediKarti_KartSahibi").focus(); })
        },

        kredikartiOdeme: function () {
            var teklifId = $(this).attr("teklif-id");
            var fiyat = $(this).attr("fiyat");
            if (teklifId && fiyat) {
                var nakit = $("#OdemeTipi_1").is(':checked');
                var havale = $("#OdemeTipi_3").is(':checked');
                if (nakit) {
                    $("#KK_OdemeTipi_1").attr('checked', true);
                    $("#kredi-kart-bilgi").hide();
                    $("#kredi-kart-bilgi").find("input").addClass("ignore");
                }
                else if (havale) {
                    $("#KK_OdemeTipi_3").attr('checked', true);
                    $("#kredi-kart-bilgi").hide();
                    $("#kredi-kart-bilgi").find("input").addClass("ignore");
                }
                else {
                    $("#KK_OdemeTipi_2").attr('checked', true);
                    $("#kredi-kart-bilgi").show();
                    $("#kredi-kart-bilgi").find("input").removeClass("ignore");
                }

                var vadeli = $("#Odeme_OdemeSekli").is(':checked');
                if (!vadeli) {
                    $("#KK_OdemeSekli_2").attr('checked', true);
                    $("#taksit-sayisi").show();
                    var taksitliOdeme = $("#Odeme_TaksitSayisi").val();
                    $("#KrediKarti_TaksitSayisi").val(taksitliOdeme);
                }
                else {
                    $("#KK_OdemeSekli_1").attr('checked', true);
                    $("#taksit-sayisi").hide();
                }

                $("#kredi-karti-error-list").html("");
                $("#KrediKarti_KK_TeklifId").val(teklifId);
                $("#kk-tutar").html(fiyat);
                $("#kk-odeme").modal("show");

                $("#KK_OdemeTipi_1").attr("readonly", true);
                $("#KK_OdemeTipi_2").attr("readonly", true);
                $("#KK_OdemeTipi_3").attr("readonly", true);
                $("#KK_OdemeSekli_1").attr("readonly", true);
                $("#KK_OdemeSekli_2").attr("readonly", true);

                $("#taksit-sayisi").attr("readonly", true);
                $("#KrediKarti_TaksitSayisi").attr("readonly", true);

                $("#KK_OdemeTipi_1").attr("disabled", true);
                $("#KK_OdemeTipi_2").attr("disabled", true);
                $("#KK_OdemeTipi_3").attr("disabled", true);
                $("#KK_OdemeSekli_1").attr("disabled", true);
                $("#KK_OdemeSekli_2").attr("disabled", true);
                $("#taksit-sayisi").attr("disabled", true);
                $("#KrediKarti_TaksitSayisi").attr("disabled", true);
            }
        },

        kredikarti: function () {
            var isvalid = $("#krediKartiForm").valid();
            if (isvalid) {
                $("#KK_OdemeTipi_1").attr("readonly", false);
                $("#KK_OdemeTipi_2").attr("readonly", false);
                $("#KK_OdemeTipi_3").attr("readonly", false);
                $("#KK_OdemeSekli_1").attr("readonly", false);
                $("#KK_OdemeSekli_2").attr("readonly", false);
                $("#taksit-sayisi").attr("readonly", false);
                $("#KrediKarti_TaksitSayisi").attr("readonly", false);

                $("#KK_OdemeTipi_1").attr("disabled", false);
                $("#KK_OdemeTipi_2").attr("disabled", false);
                $("#KK_OdemeTipi_3").attr("disabled", false);
                $("#KK_OdemeSekli_1").attr("disabled", false);
                $("#KK_OdemeSekli_2").attr("disabled", false);
                $("#taksit-sayisi").attr("disabled", false);
                $("#KrediKarti_TaksitSayisi").attr("disabled", false);
                //$("#krediKartiForm").find("#KrediKarti_KartNumarasi").addClass("ignore");
                var kartnoVal = $("#krediKartiForm")[0].KrediKarti_KartNumarasi;
                $(kartnoVal).remove();

                $("#kredi-karti-error").hide();
                $(this).button("loading");
                $("#kk-odeme-cancel").hide();

                var contents = $("#krediKartiForm").serialize();

                $.ajax({
                    type: "POST",
                    url: "/Teklif/IkinciElGaranti/OdemeAl",
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

                        $("#KK_OdemeTipi_1").attr("readonly", true);
                        $("#KK_OdemeTipi_2").attr("readonly", true);
                        $("#KK_OdemeTipi_3").attr("readonly", true);
                        $("#KK_OdemeSekli_1").attr("readonly", true);
                        $("#KK_OdemeSekli_2").attr("readonly", true);

                        $("#taksit-sayisi").attr("readonly", true);
                        $("#KrediKarti_TaksitSayisi").attr("readonly", true);

                        $("#KK_OdemeTipi_1").attr("disabled", true);
                        $("#KK_OdemeTipi_2").attr("disabled", true);
                        $("#KK_OdemeTipi_3").attr("disabled", true);
                        $("#KK_OdemeSekli_1").attr("disabled", true);
                        $("#KK_OdemeSekli_2").attr("disabled", true);
                        $("#taksit-sayisi").attr("disabled", true);
                        $("#KrediKarti_TaksitSayisi").attr("disabled", true);
                    },
                    error: function () {
                        $("#kk-odeme-btn").button("reset");
                        $("#kk-odeme-cancel").show();

                        $("#KK_OdemeTipi_1").attr("readonly", true);
                        $("#KK_OdemeTipi_2").attr("readonly", true);
                        $("#KK_OdemeTipi_3").attr("readonly", true);
                        $("#KK_OdemeSekli_1").attr("readonly", true);
                        $("#KK_OdemeSekli_2").attr("readonly", true);

                        $("#taksit-sayisi").attr("readonly", true);
                        $("#KrediKarti_TaksitSayisi").attr("readonly", true);

                        $("#KK_OdemeTipi_1").attr("disabled", true);
                        $("#KK_OdemeTipi_2").attr("disabled", true);
                        $("#KK_OdemeTipi_3").attr("disabled", true);
                        $("#KK_OdemeSekli_1").attr("disabled", true);
                        $("#KK_OdemeSekli_2").attr("disabled", true);
                        $("#taksit-sayisi").attr("disabled", true);
                        $("#KrediKarti_TaksitSayisi").attr("disabled", true);
                    }
                });
            }
        }
    }
}

function OdemeSekliDegisti(e, data) {
    if (data.value) {
        $(".taksit-sayisi").slideUp("fast");
    }
    else {
        $(".taksit-sayisi").slideDown("fast");
        $("#Odeme_TaksitSayisi").val("5");
    }
}

function ikinciElGarantiTeklifWizardCallback(current) {
    //Hazırlayan bilgileri
    if (current == 1) {

    }
        //Sigorta ettiren / sigortali tab
    else if (current == 2) {
        if (!$("#Musteri_SigortaEttiren_AdiUnvan").is(":visible")) {
            $("#sigortaettiren-sorgula").trigger("click");
            return false;
        }
        //ikinciElGarantiClass.genelbilgiler();

        return sigortaliKontrol.Kaydet();
    }
        // Riziko Bilgileri
    else if (current == 3) {
        var tescilNo = $("#Arac_TescilBelgeSeriNo").val();
        var asbisNo = $("#Arac_AsbisNo").val();

        $("#Arac_TescilBelgeSeriNo").removeClass("ignore");
        $("#Arac_AsbisNo").removeClass("ignore");
        if (tescilNo.length > 0) {
            $("#Arac_AsbisNo").addClass("ignore");
        }
        if (tescilNo.length > 0) {
            $("#Arac_TescilBelgeSeriNo").addClass("ignore");
        }

        var isValid = FormWizard.validatePage('#tab3');

        var aracBilgileri = $("#Arac_AracBilgileri_Acik").val();
        if (aracBilgileri == "0") {
            $("#btn-sorgula").trigger("click");
            isValid = 0;
        }

        return isValid == 1;
    }

    return true;
}

$(document).ready(function () {
    $("#OdemeTipi_2[name='Odeme.OdemeTipi']").attr("checked", "checked");

    // ==== Odeme tipi, şekli ve taksit sayısı kontrol ediliyor. ==== //
    $("#Odeme_OdemeSekli_control").on("switch-change", OdemeSekliDegisti);

    $("#Arac_TescilIl").tescilil({ ilce: "#Arac_TescilIlce" });

    $("#Arac_TescilBelgeSeriNo").numeric();

    if ($("#Arac_PlakaNo").length > 0) {
        var plakaNo = $("#Arac_PlakaNo").val();
        if (plakaNo.length == 0) {
            $(".arac-bilgileri :input").addClass("ignore");
        } else {
            $(".arac-bilgileri").show();
        }
    }
});

$("#Arac_PlakaNo").blur(function () {
    $(this).val($(this).val().toUpperCase());
});

$("#btn-sorgula").click(function () {
    var isValid = $("#Arac_PlakaNo").valid();

    if (isValid) {

        $("#btn-sorgula").button("loading");

        var plakaKodu = $("#Arac_PlakaKodu").val();
        var plakaNo = $("#Arac_PlakaNo").val();
        var musteriKodu;

        var sigortaliAyni = $("#Musteri_SigortaliAyni_control").bootstrapSwitch('status');
        if (sigortaliAyni) {
            musteriKodu = $("#Musteri_SigortaEttiren_MusteriKodu").val();
        } else {
            musteriKodu = $("#Musteri_Sigortali_MusteriKodu").val();
        }

        $.ajax({
            dataType: "json",
            url: "/Teklif/Trafik/PlakaSorgula",
            data: { PlakaKodu: plakaKodu, PlakaNo: plakaNo, MusteriKodu: musteriKodu },
            success: ikinciElGarantiClass.setPlakaSorgu,
            error: ikinciElGarantiClass.errorPlakaSorgu
        });
    }
});

$("#Arac_KullanimSekliKodu").aracmarka({
    tarz: "#Arac_KullanimTarziKodu",
    marka: "#Arac_MarkaKodu",
    model: "#Arac_Model",
    tip: "#Arac_TipKodu"
});

// ==== Tarih ayarları ==== //
$('#Arac_TrafikTescilTarihi').change(function () {
    var d1 = $(this).datepicker("getDate");
    $("#Arac_TrafigeCikisTarihi").datepicker("setDate", d1);
    setTimeout(function () { $("#Arac_TrafigeCikisTarihi").datepicker("show"); }, 100);
});
// ==== Tarih Ayarları ==== //


$("#Arac_TescilBelgeSeriKod").blur(function () {
    $(this).val($(this).val().toUpperCase());
});

$("#btn-hesapla").click(function () {
    var isvalid = $("#form1").valid();
    if (isvalid) {

        $(this).button("loading");

        $(".switcher").find(":input").switchFix();

        var contents = $("#form1, #form2, #form3, #form4").serialize();

        $.ajax(
            {
                type: "POST",
                url: "/Teklif/IkinciElGaranti/Hesapla",
                data: contents,
                success: function (data) {
                    if (data.id > 0) {
                        teklifFiyat.kontol({ processId: data.id, guid: data.g });

                        $("#step3group").css({ "visibility": "visible" });
                        $("#teklif-fiyat-container").css({ "visibility": "visible" });
                        $("#step1").collapse("hide");
                        $("#step2").collapse("hide");
                        $("#step3").collapse("show");

                        $("#fiyat-container").html($("#fiyat-container-template").html());
                        $('#form_wizard_1').bootstrapWizard("next");
                    } else if (data.id == 0) {
                        $("#btn-hesapla").button("reset");
                        alert(data.hata);
                    }
                    $("#btn-hesapla").button("reset");
                },
                error: function () {
                    $("#btn-hesapla").button("reset");
                }
            });
        //App.scrollTo();
    }
});

$(".upper-letter").blur(function () {
    $(this).val($(this).val().replace('i', 'İ'));
    $(this).val($(this).val().toUpperCase());
});

var ikinciElGarantiClass = new function () {
    return {
        genelbilgiler: function () {

        },

        setPlakaSorgu: function (data) {
            $("#plaka-sorgu-hata").hide();
            $("#Arac_KullanimSekliKodu").val(data.AracKullanimSekli);
            $("#Arac_KullanimTarziKodu").dropDownFill(data.Tarzlar);
            $("#Arac_KullanimTarziKodu").val(data.AracKullanimTarzi);
            $("#Arac_MarkaKodu").dropDownFill(data.Markalar);
            $("#Arac_MarkaKodu").val(data.AracMarkaKodu);

            $("#Arac_Model").val(data.AracModelYili);
            $("#Arac_TipKodu").dropDownFill(data.Tipler);
            $("#Arac_TipKodu").val(data.AracTipKodu);

            $("#Arac_MotorNo").val(data.AracMotorNo);
            $("#Arac_SaseNo").val(data.AracSasiNo);

            if (data.AracTescilTarih.length > 0) {
                $('#Arac_TrafikTescilTarihi').datepicker('setDate', data.AracTescilTarih);
                $('#Arac_TrafigeCikisTarihi').datepicker('setDate', data.AracTescilTarih);
            }


            ikinciElGarantiClass.aracAlanlariGoster();

            $("#btn-sorgula").button("reset");

            if (data != null) {
                var date = new Date();

                if (data.AracModelYili < (date.getFullYear() - 7)) alert("Araç 7 yaşını aşmış olamaz.");
                else $("#GenelBilgiler_Model").val(data.AracModelYili);

                if (data.AracSilindir < 2000)
                    $("#GenelBilgiler_SilindirHacmi").val(1);
                if (data.AracSilindir > 2000 & data.AracSilindir < 3000)
                    $("#GenelBilgiler_SilindirHacmi").val(2);
                else
                { alert("Motor hacmi 3000 cc'yi aşamaz"); }
            }
        },

        errorPlakaSorgu: function (jqXHR, textStatus, errorThrown) {
            $("#btn-sorgula").button("reset");
            var response = jQuery.parseJSON(jqXHR.responseText);
            $("#plaka-sorgu-hata").html(response.message);
            $("#plaka-sorgu-hata").show();

            ikinciElGarantiClass.aracAlanlariGoster();
        },

        aracAlanlariGoster: function () {
            $(".arac-bilgileri").slideDown("fast");
            $(".arac-bilgileri :input").removeClass("ignore");
            $("#Arac_AracBilgileri_Acik").val("1");
        }
    }
}