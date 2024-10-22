var IsYeriGenelBilgiler = new function () {
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

        DemirbasBedeliDegisti: function () {
            IsYeriGenelBilgiler.TeminatlarSet();
        },

        DekorasyonBedeliDegisti: function () {

            //Sadece Dekorasyon Bedeline bağlı değişen teminatlar set ediliyor
            var dekorasyonBedeli = $("#IsYeriTeminatBedelBilgileri_DekorasyonBedeli").autoNumeric('get');
            $(".dekorasyon-bedel").val($("#IsYeriTeminatBedelBilgileri_DekorasyonBedeli").val());

            IsYeriGenelBilgiler.TeminatlarSet();
            IsYeriGenelBilgiler.SetDekorasyonTeminatDisabled(dekorasyonBedeli);
        },

        EmteaBedeliDegisti: function () {

            //Sadece Emtea Bedeline bağlı değişen teminatlar set ediliyor
            var emteaBedeli = $("#IsYeriTeminatBedelBilgileri_EmteaBedeli").autoNumeric('get');
            $(".emtea-bedel").val($("#IsYeriTeminatBedelBilgileri_EmteaBedeli").val());

            IsYeriGenelBilgiler.TeminatlarSet();
            IsYeriGenelBilgiler.SetEmteaTeminatDisabled(emteaBedeli);
        },

        BinaBedeliDegisti: function () {

            //Sadece Bina Bedeline bağlı değişen teminatlar set ediliyor
            var binaBedeli = $("#IsYeriTeminatBedelBilgileri_BinaBedeli").autoNumeric('get');
            $(".bina-bedel").val($("#IsYeriTeminatBedelBilgileri_BinaBedeli").val());

            IsYeriGenelBilgiler.DaskBedeliDegisti();
            IsYeriGenelBilgiler.TeminatlarSet();
            IsYeriGenelBilgiler.SetBinaTeminatDisabled(binaBedeli);
        },

        DaskBedeliDegisti: function () {
            var sigortaBedeli = $("#RizikoGenelBilgiler_DaskSigortaBedeli").autoNumeric('get');
            var binaBedeli = $("#IsYeriTeminatBedelBilgileri_BinaBedeli").autoNumeric('get');

            if (binaBedeli == "") binaBedeli = 0;
            if (sigortaBedeli == "") sigortaBedeli = 0;

            if (sigortaBedeli == 0) {
                $("#IsYeriTeminatBilgileri_BinaDepremBedel").val(parseInt(binaBedeli, 10));
            }
            else {
                $("#IsYeriTeminatBilgileri_BinaDepremBedel").val(parseInt(binaBedeli, 10) - parseInt(sigortaBedeli, 10));
            }
            var bedel = $("#IsYeriTeminatBilgileri_BinaDepremBedel").autoNumeric('get');
            if (bedel < 1)
            { $("#IsYeriTeminatBilgileri_BinaDepremBedel").val("0"); }
            $("#IsYeriTeminatBilgileri_BinaDepremBedel").autoNumeric('update', { vMin: '0', vMax: '9999999', mDec: 0 });
        },

        TeminatlarSet: function () {

            var demirbasbedel = $("#IsYeriTeminatBedelBilgileri_DemirbasBedeli").autoNumeric('get');
            var binabedel = $("#IsYeriTeminatBedelBilgileri_BinaBedeli").autoNumeric('get');
            var dekorasyonbedel = $("#IsYeriTeminatBedelBilgileri_DekorasyonBedeli").autoNumeric('get');
            var emteabedel = $("#IsYeriTeminatBedelBilgileri_EmteaBedeli").autoNumeric('get');

            if (demirbasbedel === undefined || demirbasbedel == "") demirbasbedel = 0;
            if (binabedel === undefined || binabedel == "") binabedel = 0;
            if (dekorasyonbedel === undefined || dekorasyonbedel == "") dekorasyonbedel = 0;
            if (emteabedel === undefined || emteabedel == "") emteabedel = 0;

            var dortluteminattutar = (parseInt(demirbasbedel, 10) + parseInt(binabedel, 10) + parseInt(dekorasyonbedel, 10) + parseInt(emteabedel, 10));
            var ucluteminattutar = (parseInt(demirbasbedel, 10) + parseInt(dekorasyonbedel, 10) + parseInt(emteabedel, 10));
            var ikiilteminattutar = (parseInt(demirbasbedel, 10) + parseInt(emteabedel, 10));

            //Sadece Demirbaş Bedeline bağlı değişen teminatlar set ediliyor
            $(".demirbas-bedel").val(demirbasbedel);
            $(".uclu-teminat").val(ucluteminattutar);
            $(".ikili-teminat").val(ikiilteminattutar);
            $(".dortlu-teminat").val(dortluteminattutar);

            $(".autoNumeric").autoNumeric('update', { vMin: '0', vMax: '9999999', mDec: 0 });
            //$(".dortlu-teminat").autoNumeric('update', { vMin: '0', vMax: '9999999', mDec: 0 });
            //$(".uclu-teminat").autoNumeric('update', { vMin: '0', vMax: '9999999', mDec: 0 });
            //$(".ikili-teminat").autoNumeric('update', { vMin: '0', vMax: '9999999', mDec: 0 });

        },

        EkTeminatlarChange: function (e, data) {
            if (data.value) {
                $(this).next().slideDown("slow");
            }
            else {
                $(this).next().slideUp("slow");
            }
        },

        TutarliTeminatlarChange: function (e, data) {
            if (data.value) {
                $(this).next().slideDown("slow");
                $(this).next().children("input").removeClass("ignore");
            }
            else {
                $(this).next().slideUp("slow");
                $(this).next().children("input").addClass("ignore");
                IsYeriGenelBilgiler.ObjectErrorClear($(this).next().children("input"));
            }
        },

        SetMoneyFormat: function () {
            // ==== Para Formatı Belirleniyor.
            $(".autoNumeric").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0 });
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
            object.val('');
            object.parent().parent().removeClass('error');
            object.addClass("ignore");
            object.parent().parent().parent().removeClass('error');
            object.parent().next().attr('class', 'field-validation-valid');
            object.parent().next().empty();
            object.parent().slideUp("fast");
        },

        SetEmteaTeminatDisabled: function (data) {
            if (data == "" || data == 0) {
                $('.emtea-disabled').bootstrapSwitch('setActive', false);
                $('.emtea-disabled').bootstrapSwitch('setState', false);

                $("#IsYeriTeminatBilgileri_EmteaYangin").attr("disabled", "");
                $("#IsYeriTeminatBilgileri_EmteaDeprem").attr("disabled", "");
            }
            else if (data > 0) {
                $('.emtea-disabled').bootstrapSwitch('setActive', true);

                $("#IsYeriTeminatBilgileri_EmteaYangin").removeAttr("disabled");
                $("#IsYeriTeminatBilgileri_EmteaDeprem").removeAttr("disabled");
            }
        },

        SetDekorasyonTeminatDisabled: function (data) {
            if (data == "" || data == 0) {
                $('.dekorasyon-disabled').bootstrapSwitch('setActive', false);
                $('.dekorasyon-disabled').bootstrapSwitch('setState', false);

                $("#IsYeriTeminatBilgileri_DekorasyonYangin").attr("disabled", "");
                $("#IsYeriTeminatBilgileri_DekorasyonDeprem").attr("disabled", "");
            }
            else if (data > 0) {
                $('.dekorasyon-disabled').bootstrapSwitch('setActive', true);

                $("#IsYeriTeminatBilgileri_DekorasyonYangin").removeAttr("disabled");
                $("#IsYeriTeminatBilgileri_DekorasyonDeprem").removeAttr("disabled");
            }
        },

        SetBinaTeminatDisabled: function (data) {
            if (data == "" || data == 0) {
                $('.bina-disabled').bootstrapSwitch('setActive', false);
                $('.bina-disabled').bootstrapSwitch('setState', false);

                $("#IsYeriTeminatBilgileri_BinaYangin").attr("disabled", "");
                $("#IsYeriTeminatBilgileri_EkTeminatBina").attr("disabled", "");
                $("#IsYeriTeminatBilgileri_DepremYanardagPuskurmesiBina").attr("disabled", "");
                $("#IsYeriTeminatBilgileri_EnkazKaldirmaBina").attr("disabled", "");
                $("#IsYeriTeminatBilgileri_BinaDeprem").attr("disabled", "");
            }
            else if (data > 0) {
                $('.bina-disabled').bootstrapSwitch('setActive', true);

                $("#IsYeriTeminatBilgileri_BinaYangin").removeAttr("disabled", "");
                $("#IsYeriTeminatBilgileri_EkTeminatBina").removeAttr("disabled");
                $("#IsYeriTeminatBilgileri_DepremYanardagPuskurmesiBina").removeAttr("disabled");
                $("#IsYeriTeminatBilgileri_EnkazKaldirmaBina").attr("disabled", "");
                $("#IsYeriTeminatBilgileri_BinaDeprem").removeAttr("disabled", "");
            }
        },

        DaskPolicesiVarmiChange: function (e, data) {
            $("#RizikoGenelBilgiler_DaskSigortaBedeli").val("");
            IsYeriGenelBilgiler.DaskBedeliDegisti();
        },

    }
}

var isYeriOdeme = new function () {
    return {

        init: function () {
            $("#kk-odeme-btn").click(isYeriOdeme.kredikarti);
            $(".teklif-satin-al").live("click", isYeriOdeme.kredikartiOdeme);
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
                    url: "/Teklif/IsYeri/OdemeAl",
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

function isYeriTeklifWizardCallback(current) {
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
    IsYeriGenelBilgiler.SetMoneyFormat();

    //Ana Teminatlar
    $('#IsYeriTeminatBilgileri_DemirbasYangin').attr("disabled", "");

    // =======================KONUT GENEL BİLGİLER =============================//
    // ==== Kurum Sube ==== //
    $("#RizikoGenelBilgiler_KurumBanka").KurumSube({ Sube: '#RizikoGenelBilgiler_Sube' });

    // ==== Swich ==== //
    $("#RizikoGenelBilgiler_YururlukteDaskPolicesiVarmi_control").on("switch-change", IsYeriGenelBilgiler.YururluktePolice);
    $("#RizikoGenelBilgiler_RehinliAlacakliDainMurtehinVarmi_control").on("switch-change", IsYeriGenelBilgiler.RehinliAlacakli);

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


    // ======================= İş Yeri ADRES BİLGİLER =============================//
    $("#RizikoAdresBilgiler_PostaKodu").numeric();

    // ======================= İş Yeri DİGER BİLGİLER =============================//
    $("#RizikoDigerBilgiler_DaireBrutYuzolcumu").numeric();
    $("#OdemeTipi_2[name='Odeme.OdemeTipi']").attr("checked", "checked");

    // ==== Odeme tipi, şekli ve taksit sayısı kontrol ediliyor. ==== //
    $("#Odeme_OdemeSekli_control").on("switch-change", OdemeSekliDegisti);

    // Dask Policesi Varmı 
    $("#RizikoGenelBilgiler_YururlukteDaskPolicesiVarmi_control").on("switch-change", IsYeriGenelBilgiler.DaskPolicesiVarmiChange);


    //Teminatlar Değiştiğinde calısıcak
    $(".switch-teminat").on("switch-change", IsYeriGenelBilgiler.EkTeminatlarChange)
    $(".switch-teminat-tutar").on("switch-change", IsYeriGenelBilgiler.TutarliTeminatlarChange);

    //Açılışta ignore clası ekleniyor.
    $(".switch-teminat-tutar").each(function () {
        $(this).next().children("input").addClass("ignore");
    });
});

$("#btn-hesapla").click(function () {
    var isvalid = $("#form1").valid();
    if (isvalid) {

        $(this).button("loading");

        $(".switcher").find(":input").switchFix();

        //Dask Teminatı Varsa
        $("#RizikoGenelBilgiler_DaskSigortaBedeli").val($("#RizikoGenelBilgiler_DaskSigortaBedeli").autoNumeric('get'));

        //Rehinli Alacaklı
        $("#RizikoGenelBilgiler_KrediTutari").val($("#RizikoGenelBilgiler_KrediTutari").autoNumeric('get'));

        //Ana Teminatlar
        $("#IsYeriTeminatBedelBilgileri_DemirbasBedeli").val($("#IsYeriTeminatBedelBilgileri_DemirbasBedeli").autoNumeric('get'));
        $("#IsYeriTeminatBedelBilgileri_BinaBedeli").val($("#IsYeriTeminatBedelBilgileri_BinaBedeli").autoNumeric('get'));
        $("#IsYeriTeminatBedelBilgileri_DekorasyonBedeli").val($("#IsYeriTeminatBedelBilgileri_DekorasyonBedeli").autoNumeric('get'));
        $("#IsYeriTeminatBedelBilgileri_EmteaBedeli").val($("#IsYeriTeminatBedelBilgileri_EmteaBedeli").autoNumeric('get'));


        //Tutarlar ,den kurtarılıyor.
        $(".switch-teminat-tutar").each(function () {
            $(this).next().children("input").val($(this).next().children("input").autoNumeric('get'));
        });

        var contents = $("#form1, #form2, #form3, #form4").serialize();

        $.ajax(
            {
                type: "POST",
                url: "/Teklif/IsYeri/Hesapla",
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
        $(".autoNumeric").autoNumeric('update', { vMin: '0', vMax: '9999999', mDec: 0 });
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
    IsYeriGenelBilgiler.RizikoCaddeSokakChange();
});

$("#RizikoAdresBilgiler_Sokak").change(function () {
    IsYeriGenelBilgiler.RizikoCaddeSokakChange();
});

//TEMİNAT BEDELLERİ
$("#IsYeriTeminatBedelBilgileri_DemirbasBedeli").change(function () {
    IsYeriGenelBilgiler.DemirbasBedeliDegisti();
});

$("#IsYeriTeminatBedelBilgileri_BinaBedeli").change(function () {
    IsYeriGenelBilgiler.BinaBedeliDegisti();
});

$("#IsYeriTeminatBedelBilgileri_DekorasyonBedeli").change(function () {
    IsYeriGenelBilgiler.DekorasyonBedeliDegisti();
});

$("#IsYeriTeminatBedelBilgileri_EmteaBedeli").change(function () {
    IsYeriGenelBilgiler.EmteaBedeliDegisti();
});

$("#RizikoGenelBilgiler_DaskSigortaBedeli").change(function () {
    IsYeriGenelBilgiler.DaskBedeliDegisti();
});















