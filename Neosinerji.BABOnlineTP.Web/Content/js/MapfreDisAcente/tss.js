
$(document).ready(function () {
    $("#btn-hesapla").click(function () {

        var oncekiSirket = $("#GenelBilgiler_OncekiSigortaSirketi").val();
        if (oncekiSirket != null && oncekiSirket != "") {
            $("#GenelBilgiler_OncekiPoliceBaslangicTarihi").removeClass("ignore");
        }
        else {
            $("#GenelBilgiler_OncekiPoliceBaslangicTarihi").addClass("ignore");
        }
        if (!$("#Musteri_SigortaEttiren_AdiUnvan").is(":visible")) {
            $("#sigortaettiren-sorgula").trigger("click");
            return false;
        }

        ////App.scrollTo();
        //sigortaliKontrol.Kaydet();

        var boyValue = $("#GenelBilgiler_Boy").val();
        if (boyValue > 250) {
            alert("Boy değeri 250 den büyük olamaz.");
            return;
        }

        var kiloValue = $("#GenelBilgiler_Kilo").val();
        if (kiloValue > 200) {
            alert("Kilo değeri 200 den büyük olamaz.");
            return;
        }

        var isvalid = $("#form1").valid();
        if (isvalid) {
            $("#TeklifId").val("");
            $(this).button("loading");
            $(".switcher").find(":input").switchFix();
            var disabled = $("#form1,#form2").find(':input:disabled').removeAttr('disabled');
            var contents = $("#form1,#form2").find("select, textarea, input").serialize();
            disabled.attr('disabled', 'disabled');
            $.ajax(
                {
                    type: "POST",
                    url: "/Teklif/TamamlayiciSaglik/Hesapla",
                    data: contents,
                    success: function (data) {
                        if (data.id > 0) {
                            teklifFiyat.kontol({ processId: data.id, guid: data.g });
                            $("#btn-hesapla").hide();
                            $("#step3group").css({ "visibility": "visible" });
                            $("#teklif-fiyat-container").css({ "visibility": "visible" });
                            $("#step1").collapse("hide");
                            $("#step2").collapse("hide");
                            $("#step3").collapse("show");
                            $("#fiyat-container").html($("#fiyat-container-template").html());
                            $('#form_wizard_1').bootstrapWizard("next");
                            $("#btn-hesapla").button("reset");
                            $("#btn-hesapla").hide();
                            $("#step-title").html("Adım 2");
                        }
                        else if (data.id == 0) {
                            $("#btn-hesapla").button("reset");
                            alert(data.hata);
                            $('#form_wizard_1').bootstrapWizard("previous");
                            $("#teklif-fiyat-container").css({ "visibility": "hidden" });
                        }
                        $("#btn-hesapla").button("reset");
                    },
                    error: function () {
                        $("#btn-hesapla").button("reset");
                    }
                });
        }

    });
    $("#KrediKartiMi_control").addClass("switcher switcher-small deactivate");
    $("#KrediKartiMi").attr("disabled", "");
    $("#GenelBilgiler_YenilemeMi_control").on("switch-change", EskiPoliceKontrol);

    // ==== Odeme tipi, şekli ve taksit sayısı kontrol ediliyor. ==== //
    $("#Odeme_OdemeSekli_control").on("switch-change", OdemeSekliDegisti);
    $("#GenelBilgiler_TarifeKodu").change(function () {

        if ($("#GenelBilgiler_TarifeKodu option:selected").text() == "Standart 6 ay ( 180 gün )") {
            $("#TedaviTipi_0").attr('checked', true);
            $("#TedaviTipi_1").attr('disabled', true);
            $("#TedaviTipi_0").attr('disabled', false);
            $("#TedaviTipi_1").attr('checked', false);
        }
        else {
            $("#TedaviTipi_1").attr('checked', true);
            $("#TedaviTipi_1").attr('disabled', false);
            $("#TedaviTipi_0").attr('disabled', true);
            $("#TedaviTipi_0").attr('checked', false);
        }
    });
});

function tssTeklifWizardCallback(current) {
    //Hazırlayan bilgileri
    if (current == 1) {

        if (!$("#Musteri_SigortaEttiren_AdiUnvan").is(":visible")) {
            $("#sigortaettiren-sorgula").trigger("click");
            return false;
        }

        ////App.scrollTo();
        //return sigortaliKontrol.Kaydet();

    }
    //Sigorta ettiren / sigortali tab
    else if (current == 2) {
        var isValid = FormWizard.validatePage('#tab2');
        //App.scrollTo();
        return isValid == 1;
    }

    return true;
}

var tssOdeme = new function () {
    return {

        init: function () {
            $("#kk-odeme-btn").click(tssOdeme.kredikarti);
            $(".teklif-satin-al").live("click", tssOdeme.kredikartiOdeme);
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

                $("#nakit-error").hide();
                var NakitOdeme = false;
                if ($("#KK_OdemeTipi_1").is(":checked")) {
                    NakitOdeme = true;
                }
                var contents = $("#krediKartiForm").serialize();
                $.ajax({
                    type: "POST",
                    url: "/Teklif/TamamlayiciSaglik/OdemeAl",
                    data: contents,
                    success: function (data) {

                        if (data.Success) {
                            window.location.href = data.RedirectUrl;
                            return;
                        }
                        else if (data.Hatalar != null && data.Hatalar.length > 0) {

                            if (NakitOdeme) {
                                $("#nakit-error-list").html("");
                                for (var i in data.Hatalar) {
                                    var hata = data.Hatalar[i];

                                    $("#nakit-error-list").append("<span>" + hata + "</span><br/>");
                                }
                                $("#nakit-error").slideDown("fast");
                            }
                            else {
                                $("#kredi-karti-error-list").html("");

                                for (var i in data.Hatalar) {
                                    var hata = data.Hatalar[i];

                                    $("#kredi-karti-error-list").append("<span>" + hata + "</span><br/>");
                                }

                                $("#kredi-karti-error").slideDown("fast");
                            }
                        }

                        $("#kk-odeme-btn").button("reset");
                        $("#kk-odeme-cancel").show();

                        $("#KK_OdemeTipi_1").attr("readonly", true);
                        $("#KK_OdemeTipi_2").attr("readonly", true);
                        $("#KK_OdemeTipi_3").attr("readonly", true);
                        $("#KK_OdemeSekli_1").attr("readonly", true);
                        $("#KK_OdemeSekli_2").attr("readonly", true);

                        $("#taksit-sayisi").attr("readonly", true);

                        $("#KK_OdemeTipi_1").attr("disabled", true);
                        $("#KK_OdemeTipi_2").attr("disabled", true);
                        $("#KK_OdemeTipi_3").attr("disabled", true);
                        $("#KK_OdemeSekli_1").attr("disabled", true);
                        $("#KK_OdemeSekli_2").attr("disabled", true);
                        $("#taksit-sayisi").attr("disabled", true);


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

function EskiPoliceKontrol(e, data) {
    if (data.value) {
        $("#oncekiPolice").show();
    }
    else {
        $("#oncekiPolice").hide();
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