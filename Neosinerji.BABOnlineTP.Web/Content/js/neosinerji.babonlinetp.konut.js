var KonutGenelBilgiler = new function () {
    return {
        YururluktePolice: function (e, data) {
            if (data.value) {
                $(".dask-policesi").slideDown("fast");
                $("#RizikoGenelBilgiler_DaskSigortaSirketi").removeClass("ignore");
                $("#RizikoGenelBilgiler_DaskPoliceninVadeTarihi").removeClass("ignore");
                $("#RizikoGenelBilgiler_DaskSigortaBedeli").removeClass("ignore");
            }
            else {
                $(".dask-policesi").slideUp("fast");
                $("#RizikoGenelBilgiler_DaskSigortaSirketi").addClass("ignore");
                $("#RizikoGenelBilgiler_DaskPoliceninVadeTarihi").addClass("ignore");
                $("#RizikoGenelBilgiler_DaskSigortaBedeli").addClass("ignore");
            }
        },

        RehinliAlacakli: function (e, data) {
            if (data.value) {
                $(".rehinli-alacak").slideDown("fast");
                $("#RizikoGenelBilgiler_Tipi").removeClass("ignore");
                $("#RizikoGenelBilgiler_KurumBanka").removeClass("ignore");
                $("#RizikoGenelBilgiler_Sube").removeClass("ignore");
                $("#RizikoGenelBilgiler_KrediReferansNo_HesapSozlesmeNo").removeClass("ignore");
            }
            else {
                $(".rehinli-alacak").slideUp("fast");
                $("#RizikoGenelBilgiler_Tipi").addClass("ignore");
                $("#RizikoGenelBilgiler_KurumBanka").addClass("ignore");
                $("#RizikoGenelBilgiler_Sube").addClass("ignore");
                $("#RizikoGenelBilgiler_KrediReferansNo_HesapSozlesmeNo").addClass("ignore");
            }
        },

        EsyaBedeliDegisti: function () {

            var esyaBedeli = $("#KonutTeminatBedelBilgileri_EsyaBedeli").autoNumeric('get');
            var binaBedeli = $("#KonutTeminatBedelBilgileri_BinaBedeli").autoNumeric('get');

            if (esyaBedeli === undefined) return;
            if (binaBedeli === undefined) return;
            if (esyaBedeli == "") esyaBedeli = 0;
            if (binaBedeli == "") binaBedeli = 0;

            this.SetEsyaBedelTeminat(esyaBedeli);
            this.SetikiliTeminatlari(parseInt(esyaBedeli, 10) + parseInt(binaBedeli, 10));

        },

        BinaBedeliDegisti: function () {
            var esyaBedeli = $("#KonutTeminatBedelBilgileri_EsyaBedeli").autoNumeric('get');
            var binaBedeli = $("#KonutTeminatBedelBilgileri_BinaBedeli").autoNumeric('get');

            if (esyaBedeli === undefined) return;
            if (binaBedeli === undefined) return;
            if (esyaBedeli == "") esyaBedeli = 0;
            if (binaBedeli == "") binaBedeli = 0;

            this.SetBinaBedelTeminat(binaBedeli);
            this.SetikiliTeminatlari(parseInt(esyaBedeli, 10) + parseInt(binaBedeli, 10));

            ////BİNA DEPREM
            if ($("#RizikoGenelBilgiler_YururlukteDaskPolicesiVarmi_control").bootstrapSwitch('status')) {
                var daskBedeli = $("#RizikoGenelBilgiler_DaskSigortaBedeli").autoNumeric('get');
                $("#KonutTeminatBilgileri_BinaDepremBedel").val(parseInt(binaBedeli, 10) - parseInt(daskBedeli, 10));
            } else $("#KonutTeminatBilgileri_BinaDepremBedel").val(binaBedeli);

            $("#KonutTeminatBilgileri_BinaDepremBedel").autoNumeric('update', { vMin: '0', vMax: '9999999', mDec: 0 });
            KonutGenelBilgiler.SetBinaTeminatDisabled(binaBedeli);
        },

        DaskBedeliDegisti: function () {
            var sigortaBedeli = $("#RizikoGenelBilgiler_DaskSigortaBedeli").autoNumeric('get');
            var binaBedeli = $("#KonutTeminatBedelBilgileri_BinaBedeli").autoNumeric('get');

            if (binaBedeli == "") binaBedeli = 0;
            if (sigortaBedeli == "") sigortaBedeli = 0;

            if (sigortaBedeli == 0) {
                $("#KonutTeminatBilgileri_BinaDepremBedel").val(parseInt(binaBedeli, 10));
            }
            else {
                $("#KonutTeminatBilgileri_BinaDepremBedel").val(parseInt(binaBedeli, 10) - parseInt(sigortaBedeli, 10));
            }
            var bedel = $("#KonutTeminatBilgileri_BinaDepremBedel").autoNumeric('get');
            if (bedel < 1)
            { $("#KonutTeminatBilgileri_BinaDepremBedel").val("0"); }
            $("#KonutTeminatBilgileri_BinaDepremBedel").autoNumeric('update', { vMin: '0', vMax: '9999999', mDec: 0 });
        },

        SetikiliTeminatlari: function (data) {
            $(".teminat-cift").val(data);
            $(".teminat-cift").autoNumeric('update', { vMin: '0', vMax: '9999999', mDec: 0 });
        },

        SetEsyaBedelTeminat: function (data) {
            $(".teminat-esya").val(data);
            $(".teminat-esya").autoNumeric('update', { vMin: '0', vMax: '9999999', mDec: 0 });
        },

        SetBinaBedelTeminat: function (data) {
            $(".teminat-bina").val(data);
            $(".teminat-bina").autoNumeric('update', { vMin: '0', vMax: '9999999', mDec: 0 });
        },

        SetStartDisabledTeminat: function () {
            //==== TEMİNATLAR disabled ediliyor. ==== //

            //SOL
            $("#KonutTeminatBilgileri_EsyaYangin").attr("disabled", "");
            $("#KonutTeminatBilgileri_EsyaDeprem").attr("disabled", "");
            $("#KonutTeminatBilgileri_EkTeminatEsya").attr("disabled", "");
            $("#KonutTeminatBilgileri_Hirsizlik").attr("disabled", "");
            $("#KonutTeminatBilgileri_Firtina").attr("disabled", "");
            $("#KonutTeminatBilgileri_DepremYanardagPuskurmesiEsya").attr("disabled", "");
            $("#KonutTeminatBilgileri_SelVeSuBaskini").attr("disabled", "");
            $("#KonutTeminatBilgileri_DahiliSu").attr("disabled", "");
            $("#KonutTeminatBilgileri_KaraTasitlariCarpmasi").attr("disabled", "");
            $("#KonutTeminatBilgileri_KarAgirligi").attr("disabled", "");
            $("#KonutTeminatBilgileri_EnkazKaldirmaEsya").attr("disabled", "");

            //SAG
            $("#KonutTeminatBilgileri_YerKaymasi").attr("disabled", "");
            $("#KonutTeminatBilgileri_DepremYanardagPuskurmesi").attr("disabled", "");
            $("#KonutTeminatBilgileri_GLKHHKNHTeror").attr("disabled", "");
            $("#KonutTeminatBilgileri_Duman").attr("disabled", "");
            $("#KonutTeminatBilgileri_HavaTasitlariCarpmasi").attr("disabled", "");
        },

        SetBinaTeminatDisabled: function (data) {
            if (data == 0 || data == "") {
                $('.teminat-bina-switch').bootstrapSwitch('setActive', true);
                $("#KonutTeminatBilgileri_EnkazKaldirmaBina").removeAttr("disabled");
                $("#KonutTeminatBilgileri_TemellerYangin").removeAttr("disabled");
                $("#KonutTeminatBilgileri_DepremYanardagPuskurmesiBina").removeAttr("disabled");
                $("#KonutTeminatBilgileri_EkTeminatBina").removeAttr("disabled");
                $("#KonutTeminatBilgileri_BinaDeprem").removeAttr("disabled");
                $("#KonutTeminatBilgileri_BinaYangin_control").removeAttr("disabled");
            }
            else {
                $(".teminat-bina-switch").bootstrapSwitch('setState', true);
                $('.teminat-bina-switch').bootstrapSwitch('setActive', false);

                $("#KonutTeminatBilgileri_EnkazKaldirmaBina").attr("disabled", "");
                $("#KonutTeminatBilgileri_TemellerYangin").attr("disabled", "");
                $("#KonutTeminatBilgileri_DepremYanardagPuskurmesiBina").attr("disabled", "");
                $("#KonutTeminatBilgileri_EkTeminatBina").attr("disabled", "");
                $("#KonutTeminatBilgileri_BinaDeprem").attr("disabled", "");
                $("#KonutTeminatBilgileri_BinaYangin").attr("disabled", "");
            }
        },

        CamKrilmasiChange: function (e, data) {
            if (data.value) {
                $("#cam-kirilmasi-tutar").slideDown("fast");
                var object = $("#KonutTeminatBilgileri_CamKirilmasiBedel");
                object.val('');
                object.removeClass('ignore');
            }
            else {
                var obj = $("#KonutTeminatBilgileri_CamKirilmasiBedel");
                KonutGenelBilgiler.ObjectErrorClear(obj);
            }
        },

        KapkacChange: function (e, data) {
            if (data.value) {
                $("#kapkac-tutar").slideDown("fast");
                var object = $("#KonutTeminatBilgileri_KapkacBedel");
                object.val('');
                object.removeClass('ignore');
            }
            else {
                var obj = $("#KonutTeminatBilgileri_KapkacBedel");
                KonutGenelBilgiler.ObjectErrorClear(obj);
            }
        },

        MaliSorumlulukYanginChange: function (e, data) {
            if (data.value) {
                $("#mali-sorumluluk-yangin-tutar").slideDown("fast");
                var object = $("#KonutTeminatBilgileri_MaliMesuliyetYanginBedel");
                object.val('');
                object.removeClass('ignore');
            }
            else {
                var obj = $("#KonutTeminatBilgileri_MaliMesuliyetYanginBedel");
                KonutGenelBilgiler.ObjectErrorClear(obj);
            }
        },

        //DegerliEsyaYanginChange: function (e, data) {
        //    if (data.value) {
        //        $("#KonutTeminatBilgileri_DegerliEsyaYanginBedel").removeClass("tutar-text-disabled");
        //        $("#KonutTeminatBilgileri_DegerliEsyaYanginBedel").removeAttr("disabled", "disabled");
        //    }
        //    else {
        //        $("#KonutTeminatBilgileri_DegerliEsyaYanginBedel").addClass("m-wrap onlynumbers tutar-text-disabled");
        //        $("#KonutTeminatBilgileri_DegerliEsyaYanginBedel").attr("disabled", "disabled");
        //        $("#KonutTeminatBilgileri_DegerliEsyaYanginBedel").val("0");
        //    }
        //},

        KiraKaybiChange: function (e, data) {
            if (data.value) {
                $("#kira-kaybi-tutar").slideDown("fast");
                var object = $("#KonutTeminatBilgileri_KiraKaybiBedel");
                object.val('');
                object.removeClass('ignore');
            }
            else {
                $("#kira-kaybi-tutar").slideUp("fast");
                var obj = $("#KonutTeminatBilgileri_KiraKaybiBedel");
                KonutGenelBilgiler.ObjectErrorClear(obj);
            }
        },

        MaliSorumlulukEkTeminatChange: function (e, data) {
            if (data.value) {
                $("#mali-sorumlulukek-tutar").slideDown("fast");
                var object = $("#KonutTeminatBilgileri_MaliSorumlulukEkTeminatBedel");
                object.val('');
                object.removeClass('ignore');
            }
            else {
                $("#mali-sorumlulukek-tutar").slideUp("fast");
                var obj = $("#KonutTeminatBilgileri_MaliSorumlulukEkTeminatBedel");
                KonutGenelBilgiler.ObjectErrorClear(obj);
            }
        },

        IzolasyonOlayBasinaChange: function (e, data) {
            if (data.value) {
                $("#izolasyon-olay-tutar").slideDown("fast");
                var object = $("#KonutTeminatBilgileri_IzolasOlayBsYilBedel");
                object.val('');
                object.removeClass('ignore');
            }
            else {
                $("#izolasyon-olay-tutar").slideUp("fast");
                var obj = $("#KonutTeminatBilgileri_IzolasOlayBsYilBedel");
                KonutGenelBilgiler.ObjectErrorClear(obj);
            }
        },

        DaskPolicesiVarmiChange: function (e, data) {
            $("#RizikoGenelBilgiler_DaskSigortaBedeli").val("");
            KonutGenelBilgiler.DaskBedeliDegisti();
        },

        SetMoneyFormat: function () {
            // ==== Para Formatı Belirleniyor.
            $(".autoNumeric").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0 });
            $(".teminat-esya").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0 });
            $(".teminat-bina").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0 });
            $(".teminat-cift").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0 });
        },

        RizikoCaddeSokakChange: function () {
            var sokak = $("#RizikoAdresBilgiler_Sokak");
            var cadde = $("#RizikoAdresBilgiler_Cadde");
            if (sokak.val() === undefined || cadde.val() === undefined) return;


            if (sokak.val() == "" & cadde.val() == "") {
                sokak.removeClass('ignore');
                cadde.removeClass('ignore');
            }
            else if (sokak.val() == "" & cadde.val() != "") {
                sokak.parent().parent().removeClass('error');
                sokak.addClass("ignore");
                sokak.removeClass('error');
                sokak.next().attr('class', 'field-validation-valid');
                sokak.next().empty();
            }
            else if (sokak.val() != "" & cadde.val() == "") {
                cadde.parent().parent().removeClass('error');
                cadde.addClass("ignore");
                cadde.removeClass('error');
                cadde.next().attr('class', 'field-validation-valid');
                cadde.next().empty();
            }
        },

        ObjectErrorClear: function (object) {
            object.val('0');
            object.parent().parent().removeClass('error');
            object.addClass("ignore");
            object.parent().parent().parent().removeClass('error');
            object.parent().next().attr('class', 'field-validation-valid');
            object.parent().next().empty();
            object.parent().slideUp("fast");
        }
    }
}

var konutOdeme = new function () {
    return {

        init: function () {
            $("#kk-odeme-btn").click(konutOdeme.kredikarti);
            $(".teklif-satin-al").live("click", konutOdeme.kredikartiOdeme);
            $("#kk-odeme").on("shown", function () { $("#KrediKarti_KartSahibi").focus(); })
        },

        kredikartiOdeme: function () {
            var teklifId = $(this).attr("teklif-id");
            var fiyat = $(this).attr("fiyat");
            if (teklifId && fiyat) {
                var nakit = $("#KK_OdemeTipi_1").is(':checked');
                var havale = $("#KK_OdemeTipi_3").is(':checked');
                if (nakit) {
                    $("#KK_OdemeTipi_1").attr('checked', true);
                    $("#kredi-kart-bilgi").hide();
                    $(".kk-class").find("input").addClass("ignore");
                }
                else if (havale) {
                    $("#KK_OdemeTipi_3").attr('checked', true);
                    $("#kredi-kart-bilgi").hide();
                    $("#kredi-kart-bilgi").find("input").addClass("ignore");
                }
                else {
                    $("#KK_OdemeTipi_2").attr('checked', true);
                    $("#kredi-kart-bilgi").show();
                    $(".kk-class").find("input").removeClass("ignore");
                }

                var vadeli = $("#KK_OdemeSekli_1").is(':checked');
                if (!vadeli) {
                    $("#KK_OdemeSekli_2").attr('checked', true);
                    $("#taksit-sayisi").show();
                    //var taksitliOdeme = $("#TaksitliOdeme_TaksitSayisi").val();
                    //$("#KrediKarti_TaksitSayisi").val(taksitliOdeme);
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
                    url: "/Teklif/Konut/OdemeAl",
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

function konutTeklifWizardCallback(current) {
    //Hazırlayan bilgileri
    if (current == 1) {

    }
        //Sigorta ettiren / sigortali tab
    else if (current == 2) {
        if (!$("#Musteri_SigortaEttiren_AdiUnvan").is(":visible")) {
            $("#sigortaettiren-sorgula").trigger("click");
            return false;
        }

        return sigortaliKontrol.Kaydet();
    }
        // Riziko Bilgileri
    else if (current == 3) {
        var isValid = FormWizard.validatePage('#tab3');

        if ($("#form3").valid()) {
            $("#form3").find('div.control-group').each(function () {
                $(this).removeClass('error');
                var element = $(this).find('span.field-validation-error');
                element.attr('class', 'field-validation-valid')
                element.empty();
            });
        }
        return isValid == 1;
    }
    return true;
}

$(document).ready(function () {
    var handleChoosenSelect = function () {
        if (!jQuery().chosen) {
            return;
        }
        $(".chosen").each(function () {
            $(this).chosen({
                allow_single_deselect: $(this).attr("data-with-diselect") === "1" ? true : false
            });

        });
        $(".chosen").trigger("liszt:updated");
    }
    handleChoosenSelect();

    //Para Formatı ayarlanıyor
    KonutGenelBilgiler.SetMoneyFormat();

    //Acılışta Tüm Eşya teminatları disable ediliyor.
    KonutGenelBilgiler.SetStartDisabledTeminat();

    // =======================KONUT GENEL BİLGİLER =============================//
    // ==== Kurum Sube ==== //
    $("#RizikoGenelBilgiler_KurumBanka").KurumSube({ Sube: '#RizikoGenelBilgiler_Sube' });

    // ==== Swich ==== //
    $("#RizikoGenelBilgiler_YururlukteDaskPolicesiVarmi_control").on("switch-change", KonutGenelBilgiler.YururluktePolice);
    $("#RizikoGenelBilgiler_RehinliAlacakliDainMurtehinVarmi_control").on("switch-change", KonutGenelBilgiler.RehinliAlacakli);

    // ======================= Başlangıçta bu kontroller kapalı
    //Rehinli alacak
    $("#RizikoGenelBilgiler_Tipi").addClass("ignore");
    $("#RizikoGenelBilgiler_KurumBanka").addClass("ignore");
    $("#RizikoGenelBilgiler_Sube").addClass("ignore");
    $("#RizikoGenelBilgiler_DovizKodu").addClass("ignore");
    $("#RizikoGenelBilgiler_KrediBitisTarihi").addClass("ignore");
    $("#RizikoGenelBilgiler_KrediTutari").addClass("ignore");
    $("#RizikoGenelBilgiler_KrediReferansNo_HesapSozlesmeNo").addClass("ignore");

    //Dask Police
    $("#RizikoGenelBilgiler_SigortaSirketi").addClass("ignore");
    $("#RizikoGenelBilgiler_PoliceninVadeTarihi").addClass("ignore");
    $("#RizikoGenelBilgiler_PoliceNumarasi").addClass("ignore");
    $("#RizikoGenelBilgiler_DaskSigortaBedeli").addClass("ignore");

    //Adres
    $("#RizikoAdresBilgiler_Han_Aprt_Fab").addClass("ignore");

    // ======================= Konut ADRES BİLGİLER =============================//
    $("#RizikoAdresBilgiler_PostaKodu").numeric();

    // ======================= Konut DİGER BİLGİLER =============================//
    $("#RizikoDigerBilgiler_DaireBrutYuzolcumu").numeric();
    $("#OdemeTipi_2[name='Odeme.OdemeTipi']").attr("checked", "checked");

    // ==== Odeme tipi, şekli ve taksit sayısı kontrol ediliyor. ==== //
    $("#Odeme_OdemeSekli_control").on("switch-change", OdemeSekliDegisti);

    $("#RizikoGenelBilgiler_YururlukteDaskPolicesiVarmi_control").on("switch-change", KonutGenelBilgiler.DaskPolicesiVarmiChange);

    //Teminatlar Değiştiğinde calısıcak
    $("#KonutTeminatBilgileri_CamKirilmasi_control").on("switch-change", KonutGenelBilgiler.CamKrilmasiChange);
    $("#KonutTeminatBilgileri_Kapkac_control").on("switch-change", KonutGenelBilgiler.KapkacChange);
    $("#KonutTeminatBilgileri_MaliMesuliyetYangin_control").on("switch-change", KonutGenelBilgiler.MaliSorumlulukYanginChange);
    //$("#KonutTeminatBilgileri_DegerliEsyaYangin_control").on("switch-change", KonutGenelBilgiler.DegerliEsyaYanginChange);
    $("#KonutTeminatBilgileri_KiraKaybi_control").on("switch-change", KonutGenelBilgiler.KiraKaybiChange);
    $("#KonutTeminatBilgileri_IzolasOlayBsYil_control").on("switch-change", KonutGenelBilgiler.IzolasyonOlayBasinaChange);
    $("#KonutTeminatBilgileri_MaliSorumlulukEkTeminat_control").on("switch-change", KonutGenelBilgiler.MaliSorumlulukEkTeminatChange);
});

$("#btn-hesapla").click(function () {
    var isvalid = $("#form1").valid();
    if (isvalid) {

        $(this).button("loading");

        $(".switcher").find(":input").switchFix();

        //Ana Teminatlar
        $("#KonutTeminatBedelBilgileri_EsyaBedeli").val($("#KonutTeminatBedelBilgileri_EsyaBedeli").autoNumeric('get'));
        $("#KonutTeminatBedelBilgileri_BinaBedeli").val($("#KonutTeminatBedelBilgileri_BinaBedeli").autoNumeric('get'));

        //Rehinli Alacaklı
        $("#RizikoGenelBilgiler_KrediTutari").val($("#RizikoGenelBilgiler_KrediTutari").autoNumeric('get'));

        //Dask Teminatı Varsa
        $("#RizikoGenelBilgiler_DaskSigortaBedeli").val($("#RizikoGenelBilgiler_DaskSigortaBedeli").autoNumeric('get'));

        //Ek TEminatlar
        $("#KonutTeminatBilgileri_CamKirilmasiBedel").val($("#KonutTeminatBilgileri_CamKirilmasiBedel").autoNumeric('get'));
        $("#KonutTeminatBilgileri_KapkacBedel").val($("#KonutTeminatBilgileri_KapkacBedel").autoNumeric('get'));
        $("#KonutTeminatBilgileri_MaliMesuliyetYanginBedel").val($("#KonutTeminatBilgileri_MaliMesuliyetYanginBedel").autoNumeric('get'));

        $("#KonutTeminatBilgileri_KiraKaybiBedel").val($("#KonutTeminatBilgileri_KiraKaybiBedel").autoNumeric('get'));
        $("#KonutTeminatBilgileri_MaliSorumlulukEkTeminatBedel").val($("#KonutTeminatBilgileri_MaliSorumlulukEkTeminatBedel").autoNumeric('get'));
        $("#KonutTeminatBilgileri_IzolasOlayBsYilBedel").val($("#KonutTeminatBilgileri_IzolasOlayBsYilBedel").autoNumeric('get'));

        var contents = $("#form1, #form2, #form3, #form4").serialize();

        $.ajax(
            {
                type: "POST",
                url: "/Teklif/Konut/Hesapla",
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

$("#RizikoAdresBilgiler_Il").change(function () {

    var ulkeKodu = "TUR";
    var ilKodu = $(this).val();
    if (ilKodu === undefined) return;
    if (ilKodu == "" || ilKodu < 1) {
        $("#RizikoAdresBilgiler_Ilce").empty(); return;
    }
    $.getJSON('/Common/IlceleriGetir', { UlkeKodu: ulkeKodu, IlKodu: ilKodu },
                function (result) {
                    $("#RizikoAdresBilgiler_Ilce").dropDownFill(result);
                });

    //Belediye kodu ayarlanıyor
    if ($(this).val() == 0) { $("#RizikoDigerBilgiler_BelediyeKodu").empty(); }
    if (ilKodu < 10) { ilKodu = ilKodu.replace('0', ''); }
    $.get("/Teklif/Konut/GetListBelediye", { IlKodu: ilKodu }, function (data) {
        if (data != null) {
            $("#RizikoDigerBilgiler_BelediyeKodu").dropDownFill(data);
        }
    })
});

$("#RizikoAdresBilgiler_Cadde").change(function () {
    KonutGenelBilgiler.RizikoCaddeSokakChange();

});

$("#RizikoAdresBilgiler_Sokak").change(function () {
    KonutGenelBilgiler.RizikoCaddeSokakChange();
});

//TEMİNAT BEDELLERİ
$("#KonutTeminatBedelBilgileri_EsyaBedeli").change(function () {
    KonutGenelBilgiler.EsyaBedeliDegisti();
});

$("#KonutTeminatBedelBilgileri_BinaBedeli").change(function () {
    KonutGenelBilgiler.BinaBedeliDegisti();
});

$("#RizikoGenelBilgiler_DaskSigortaBedeli").change(function () {
    KonutGenelBilgiler.DaskBedeliDegisti();
});















