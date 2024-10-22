var $AEGON_TE_AnaTeminatLimiti_Dolar;
var $AEGON_TE_AnaTeminatLimiti_Euro;
var $AEGON_TE_YillikPrimLimiti_Dolar;
var $AEGON_TE_YillikPrimLimiti_Euro;

function TETeklifWizardCallback(current) {
    //Hazırlayan bilgileri
    if (current == 1) {

    }
        //Sigorta ettiren / sigortali tab
    else if (current == 2) {
        var isValid = FormWizard.validatePage('#tab2');

        if (isValid == 1) {
            return TESabitPrimli.musteriBilgileriKontrol()
        }
        else
            return false;
    }
    else if (current == 3) {
        var isValid = FormWizard.validatePage('#tab3');

        if (isValid == 1) {
            return true;
        }
        return false;
    }

    return true;
}

$(document).ready(function () {
    TESabitPrimli.musteriZorunlulukKaldir();
    TESabitPrimli.TekrarTeklifAcilisKontrol();
    TESabitPrimli.ParaBirimiChange();
    TESabitPrimli.FillConst();
    TESabitPrimli.HesaplamaSecenegiChange();

    $("#Musteri_SigortaEttiren_UlkeKodu").ulke({ il: "#Musteri_SigortaEttiren_IlKodu", ilce: "#Musteri_SigortaEttiren_IlceKodu" });

    //Doğum Tarihi Sınırlaması
    $("#Musteri_SigortaEttiren_DogumTarihi").datepicker("option", "maxDate", '-216m +0w');
    $("#Musteri_SigortaEttiren_DogumTarihi").datepicker("option", "minDate", '-852m +0w');

    //Sigorta Başlangıç tarihi
    $("#GenelBilgiler_SigortaBaslangicTarihi").datepicker("option", "minDate", '+0m +0w');


    // EK Teminatlar Değiştiğinde calısıcak
    $("#EkTeminatlar_KritikHastaliklar_control").on("switch-change", TESabitPrimli.KritikHastalikChange);
    $("#EkTeminatlar_KazaSonucuVefat_control").on("switch-change", TESabitPrimli.KazaSonucuVefatChange);
    $("#EkTeminatlar_MaluliyetYillikDestek_control").on("switch-change", TESabitPrimli.MaluliyetYillikDestekChange);
    $("#EkTeminatlar_TamVeDaimiMaluliyet_control").on("switch-change", TESabitPrimli.TamVeDaimiMaluliyetChange);
    $("#EkTeminatlar_TopluTasimaAraclariKSV_control").on("switch-change", TESabitPrimli.TopluTasimaAraclariChange);
    //Yeni Ek Teminatlar
    $("#EkTeminatlar_KazaSonucu_TedaviMasraflari_control").on("switch-change", TESabitPrimli.KazaSonucuTedaviMasraflariChange);
    $("#EkTeminatlar_KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme_control").on("switch-change", TESabitPrimli.KazaSonucuHastanedeYatarakTDHaftalikEkOdemeChange);


    // ==== Para Formatı Belirleniyor.
    $(".autoNumeric").autoNumeric('init', { vMin: '0', vMax: '999999999', mDec: 0 });
    $(".autoNumeric-custom").autoNumeric('init', { vMin: '0', vMax: '999999999', mDec: 2, aSep: ',', aDec: '.' });
});

//$('#GenelBilgiler_TESabitPirimliBaslangicTarihi').change(function () {
//    var d1 = $(this).datepicker("getDate");
//    $("#GenelBilgiler_TESabitPirimliBitisTarihi").datepicker("option", "minDate", d1);
//    //Max seyehat suresi 1 yıldır.

//    setTimeout(function () {
//        d1.setMonth(d1.getMonth() + 12);
//        $("#GenelBilgiler_SeyehatBitisTarihi").datepicker("option", "maxDate", d1);
//        $("#GenelBilgiler_SeyehatBitisTarihi").datepicker("show");
//    }, 200);
//});

$("#btn-hesapla").click(function () {

    var isvalid = $("#form1").valid();
    var form3 = $("#form3").valid();

    if (isvalid && form3) {
        $(this).button("loading");
        if (TESabitPrimli.SigortaBilgileriKontrol()) {

            $(".switcher").find(":input").switchFix();

            $(".autoNumeric,.autoNumeric-custom").each(function () {
                $(this).val($(this).autoNumeric('get'));
            });

            var contents = $("#form1, #form2, #form3").serialize();

            $(".detay-partial-div").html('');

            $.ajax(
                {
                    type: "POST",
                    url: "/Teklif/TESabitPrimli/Hesapla",
                    data: contents,
                    success: function (data) {
                        if (data.id > 0) {
                            teklifFiyat.kontol({ processId: data.id, guid: data.g });

                            TESabitPrimli.PrimDetayPartialGetir(data.id);

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
                        $.gritter.add({ title: 'Hata Mesajı!', text: "Teklif hesaplama başlatılamadı. Lütfen tekrar deneyin." });
                    }
                });

            // ==== Para Formatı Belirleniyor.
            $(".autoNumeric").autoNumeric('update', { vMin: '0', vMax: '999999999', mDec: 0 });
            $(".autoNumeric-custom").autoNumeric('update', { vMin: '0', vMax: '999999999', mDec: 2, aSep: ',', aDec: '.' });
        } else { $("#btn-hesapla").button("reset"); }
    }
});

$("#GenelBilgiler_ParaBirimi").change(function () {
    TESabitPrimli.ParaBirimiChange();
});

$("#GenelBilgiler_HesaplamaSecenegi").change(function () {
    TESabitPrimli.HesaplamaSecenegiChange();
});

var TESabitPrimli = new function () {
    return {

        init: function (data) {

        },

        FillConst: function () {

            $AEGON_TE_AnaTeminatLimiti_Dolar = $("#AEGON_TE_AnaTeminatLimiti_Dolar").val();
            $AEGON_TE_AnaTeminatLimiti_Euro = $("#AEGON_TE_AnaTeminatLimiti_Euro").val();
            $AEGON_TE_YillikPrimLimiti_Dolar = $("#AEGON_TE_YillikPrimLimiti_Dolar").val();
            $AEGON_TE_YillikPrimLimiti_Euro = $("#AEGON_TE_YillikPrimLimiti_Euro").val();

        },

        TekrarTeklifAcilisKontrol: function () {
            var data = { value: true };

            //EK TEminat  Kritik Hastalık
            if ($("#EkTeminatlar_KritikHastaliklar_control").bootstrapSwitch('status')) {
                data = { value: true };
                TESabitPrimli.KritikHastalikChange(null, data);
            }
            else {
                data = { value: false };
                TESabitPrimli.KritikHastalikChange(null, data);
            }


            //EK TEminat  Kaza sonucu vefat
            if ($("#EkTeminatlar_KazaSonucuVefat_control").bootstrapSwitch('status')) {
                data = { value: true };
                TESabitPrimli.KazaSonucuVefatChange(null, data);
            }
            else {
                data = { value: false };
                TESabitPrimli.KazaSonucuVefatChange(null, data);
            }

            //EK TEminat  Maluliyet destek
            if ($("#EkTeminatlar_MaluliyetYillikDestek_control").bootstrapSwitch('status')) {
                data = { value: true };
                TESabitPrimli.MaluliyetYillikDestekChange(null, data);
            }
            else {
                data = { value: false };
                TESabitPrimli.MaluliyetYillikDestekChange(null, data);
            }



            //EK TEminat  TamVeDaimiMaluliyetChange
            if ($("#EkTeminatlar_TamVeDaimiMaluliyet_control").bootstrapSwitch('status')) {
                data = { value: true };
                TESabitPrimli.TamVeDaimiMaluliyetChange(null, data);
            }
            else {
                data = { value: false };
                TESabitPrimli.TamVeDaimiMaluliyetChange(null, data);
            }


            //EK TEminat  TopluTasimaAraclariChange
            if ($("#EkTeminatlar_TopluTasimaAraclariKSV_control").bootstrapSwitch('status')) {
                data = { value: true };
                TESabitPrimli.TopluTasimaAraclariChange(null, data);
            }
            else {
                data = { value: false };
                TESabitPrimli.TopluTasimaAraclariChange(null, data);
            }


            //YENİ EKLENEN TEMİNATLAR
            //EK TEminat  Kaza Sonucu Tedavi Masrafları Ek Teminatı
            if ($("#EkTeminatlar_KazaSonucu_TedaviMasraflari_control").bootstrapSwitch('status')) {
                data = { value: true };
                TESabitPrimli.KazaSonucuTedaviMasraflariChange(null, data);
            }
            else {
                data = { value: false };
                TESabitPrimli.KazaSonucuTedaviMasraflariChange(null, data);
            }

            //EK TEminat  Kaza Sonucu Hastanede Yatarak Tedavi Durumunda Haftalık Ödeme Ek Teminatı
            if ($("#EkTeminatlar_KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme_control").bootstrapSwitch('status')) {
                data = { value: true };
                TESabitPrimli.KazaSonucuHastanedeYatarakTDHaftalikEkOdemeChange(null, data);
            }
            else {
                data = { value: false };
                TESabitPrimli.KazaSonucuHastanedeYatarakTDHaftalikEkOdemeChange(null, data);
            }
        },

        musteriBilgileriKontrol: function () {

            var Message = "";

            //Cinsiyet Kontrol
            if (!$("#Cinsiyet_K").is(':checked') && !$("#Cinsiyet_E").is(':checked')) {
                Message = "<p>--Sigortalı cinsiyeti seçiniz.</p>";
            }

            //Gelir Vergisi Kontrol
            var gelirvergisi = $("#Musteri_SigortaEttiren_GelirVergisiOrani").val();
            if (gelirvergisi == "") {
                Message += "<p>--Lütfen beyan edilen gelir vergisi oranı giriniz.</p>";
            }

            //YAŞ
            var today = new Date();
            var dogum = $("#Musteri_SigortaEttiren_DogumTarihi").datepicker("getDate");
            today.setYear(today.getFullYear() - 18);
            if (dogum > today)
                Message += "<p> --Sigortalının yaşı 18 ile 70 arasında olmalıdır.</p>";

            today = new Date();
            today.setYear(today.getFullYear() - 71);
            if (dogum < today)
                Message += "<p> --Sigortalının yaşı 18 ile 70 arasında olmalıdır.</p>";


            if (Message == "")
                return true;
            else {
                $("#hata-message-div").html(Message);
                $("#hata-modal").modal("show");
                return false;
            }
        },

        SigortaBilgileriKontrol: function () {

            var Message = "";

            Message += TESabitPrimli.SigortaBedeliKontrol();
            Message += TESabitPrimli.SigortaSuresiKontrol();
            Message += TESabitPrimli.KritikHastalıkTeminatı_BedelKontrol();
            Message += TESabitPrimli.MaluliyetYıllıkDestek_BedelKontrol();
            Message += TESabitPrimli.TamVeDaimiMaluliyet_BedelKontrol();
            Message += TESabitPrimli.KSV_TTAKSV_BedelKontrol();
            Message += TESabitPrimli.KSTMT_BedenKontrol();
            Message += TESabitPrimli.KSHYTDHOT_BedenKontrol();

            if (Message == "")
                return true;
            else {
                $("#hata-message-div").html(Message);
                $("#hata-modal").modal("show");
                return false;
            }
        },

        musteriZorunlulukKaldir: function () {
            $("#Musteri_SigortaEttiren_CepTelefonu").addClass("ignore");
            $("#Musteri_SigortaEttiren_Email").addClass("ignore");
            $("#Musteri_SigortaEttiren_IlceKodu").addClass("ignore");
            $("#Musteri_SigortaEttiren_IlKodu").addClass("ignore");
            $("#Musteri_SigortaEttiren_KimlikNo").addClass("ignore");
        },

        //EK TEMİNATLAR CHANGE
        KritikHastalikChange: function (e, data) {
            var object = $("#EkTeminatlar_KritikHastaliklarSigortaBedeli");

            if (data.value) {
                $("#kritik-hastalik").slideDown("fast");
                object.removeClass('ignore');
            }
            else {
                $("#kritik-hastalik").slideUp("fast");
                object.addClass("ignore");
                object.val('');
            }
        },

        TamVeDaimiMaluliyetChange: function (e, data) {
            var object = $("#EkTeminatlar_TamVeDaimiMaluliyetSigortaBedeli");


            if (data.value) {
                $("#tam-ve-daimi-maluliyet").slideDown("fast");
                object.removeClass('ignore');
            }
            else {
                $("#tam-ve-daimi-maluliyet").slideUp("fast");
                object.addClass("ignore");
                object.val('');
            }
        },

        KazaSonucuVefatChange: function (e, data) {
            var object = $("#EkTeminatlar_KazaSonucuVefatSigortaBedeli");

            if (data.value) {
                $("#kaza-sonucu-vefat").slideDown("fast");
                object.removeClass('ignore');
            }
            else {
                $("#kaza-sonucu-vefat").slideUp("fast");
                object.addClass("ignore");
                object.val('');
            }
        },

        TopluTasimaAraclariChange: function (e, data) {
            var object = $("#EkTeminatlar_TopluTasimaAraclariKSVSigortaBedeli");


            if (data.value) {
                $("#toplu-tasima-araclari").slideDown("fast");
                object.removeClass('ignore');
            }
            else {
                $("#toplu-tasima-araclari").slideUp("fast");
                object.addClass("ignore");
                object.val('');
            }
        },

        MaluliyetYillikDestekChange: function (e, data) {
            var object = $("#EkTeminatlar_MaluliyetYillikDestekSigortaBedeli");


            if (data.value) {
                $("#maluliyet-yillik-destek").slideDown("fast");
                object.removeClass('ignore');
            }
            else {
                $("#maluliyet-yillik-destek").slideUp("fast");
                object.addClass("ignore");
                object.val('');
            }
        },

        KazaSonucuTedaviMasraflariChange: function (e, data) {
            var object = $("#EkTeminatlar_KazaSonucu_TedaviMasraflariBedeli");


            if (data.value) {
                $("#kaza-sonucu-tedavi-masraflari").slideDown("fast");
                object.removeClass('ignore');
            }
            else {
                $("#kaza-sonucu-tedavi-masraflari").slideUp("fast");
                object.addClass("ignore");
                object.val('');
            }
        },

        KazaSonucuHastanedeYatarakTDHaftalikEkOdemeChange: function (e, data) {
            var object = $("#EkTeminatlar_KazaSonucu_HastanedeYatarakTD_HaftalikEkOdemeBedeli");


            if (data.value) {
                $("#kaza-sonucu-yatarak-tedavi-ekodeme").slideDown("fast");
                object.removeClass('ignore');
            }
            else {
                $("#kaza-sonucu-yatarak-tedavi-ekodeme").slideUp("fast");
                object.addClass("ignore");
                object.val('');
            }
        },



        ParaBirimiChange: function () {
            var value = $("#GenelBilgiler_ParaBirimi").val();
            switch (value) {
                case "1": $(".para-birimi-icon").html("€"); break;
                case "2": $(".para-birimi-icon").html("$"); break;
                default: $(".para-birimi-icon").html("$"); break;
            }
        },

        HesaplamaSecenegiChange: function () {
            var value = $("#GenelBilgiler_HesaplamaSecenegi").val();
            switch (value) {
                case "1":
                    $("#hesaplama-secenegi-text").html($("#ana-teminat-tutari").text());
                    $("#span-prim-teminat").html("Prim (Yıllık / Dönem)");
                    break;
                case "2":
                    $("#hesaplama-secenegi-text").html($("#yillik-prim-text").text());
                    $("#span-prim-teminat").html("Ana Teminat Tutarı");
                    break;
                default: $("#hesaplama-secenegi-text").html($("#ana-teminat-tutari").text()); break;
            }
        },

        GetTeminatKademesi: function () {

            if ($("#AnaTeminatlar_AnaTeminat").val() == "2")
                return "2";

            if ($("#EkTeminatlar_KritikHastaliklar_control").bootstrapSwitch('status'))
                return "2";

            if ($("#EkTeminatlar_TamVeDaimiMaluliyet_control").bootstrapSwitch('status'))
                return "2";

            if ($("#EkTeminatlar_MaluliyetYillikDestek_control").bootstrapSwitch('status'))
                return "2";

            return "1";
        },

        //===========================KONTROLS==========================//
        SigortaBedeliKontrol: function () {

            var Message = "";
            var anaTeminat = $("#AnaTeminatlar_AnaTeminatSigortaBedeli").autoNumeric('get');
            var paraBirimi = $("#GenelBilgiler_ParaBirimi").val();

            switch ($("#GenelBilgiler_HesaplamaSecenegi").val()) {
                case "1":

                    switch (paraBirimi) {
                        case "1":
                            if (parseInt(anaTeminat, 10) < parseInt($AEGON_TE_AnaTeminatLimiti_Euro, 10)) {
                                Message = "<p>-- Ana teminat tutarı, minimum teminat tutarı limiti olan " + $AEGON_TE_AnaTeminatLimiti_Euro + " Euro altındadır</p>"
                            }
                            break;
                        case "2":
                            if (parseInt(anaTeminat, 10) < parseInt($AEGON_TE_AnaTeminatLimiti_Dolar, 10)) {
                                Message = "<p>-- Ana teminat tutarı, minimum teminat tutarı limiti olan " + $AEGON_TE_AnaTeminatLimiti_Dolar + " ABD Doları altındadır</p>"
                            }
                            break;
                    }

                    break;

                case "2":

                    switch (paraBirimi) {
                        case "1":
                            if (parseInt(anaTeminat, 10) < parseInt($AEGON_TE_YillikPrimLimiti_Euro, 10)) {
                                Message = "<p>-- Yıllık Prim tutarı " + $AEGON_TE_YillikPrimLimiti_Euro + " Euro altında olamaz!</p>"
                            }
                            break;
                        case "2":
                            if (parseInt(anaTeminat, 10) < parseInt($AEGON_TE_YillikPrimLimiti_Dolar, 10)) {
                                Message = "<p>-- Yıllık Prim tutarı " + $AEGON_TE_YillikPrimLimiti_Dolar + " ABD Doları altında olamaz!</p>"
                            }
                            break;
                    }

                    break;
            }

            return Message;
        },

        SigortaSuresiKontrol: function () {

            var Message = "";
            var sure = $("#GenelBilgiler_SigortaSuresi").val();

            if (sure != "") {
                var sigortaBaslangic = $("#GenelBilgiler_SigortaBaslangicTarihi").val();
                var dogumTarihi = $("#Musteri_SigortaEttiren_DogumTarihi").val();

                var kademe = this.GetTeminatKademesi();

                if (sigortaBaslangic != "" && dogumTarihi != "") {
                    $.ajax({
                        url: "/Teklif/TESabitPrimli/SigortaSuresiHesapla",
                        data: { dogumTarihi: dogumTarihi, sigortaBaslangic: sigortaBaslangic, sure: sure, kademe: kademe },
                        dataType: "json",
                        async: false,
                        method: "post",
                        success: function (data) { if (data.Success == "false") { Message = "<p>-- " + data.Message + "</p>"; } },
                        error: function () { Message = "<p>-- Sigorta Süresi hesaplanırken bir hata oluştu. Lüften tekrar deneyin.</p>" }
                    });
                }
            }

            return Message;
        },

        // ==== ==== ==== EK TEMINAT KONTROL

        KritikHastalıkTeminatı_BedelKontrol: function () {

            var Message = "";

            if ($("#EkTeminatlar_KritikHastaliklar_control").bootstrapSwitch('status')) {
                var val = $("#EkTeminatlar_KritikHastaliklarSigortaBedeli").autoNumeric('get');

                if (val !== undefined && val != "" && val != "0") {
                    if (parseInt(val, 10) > 1000) {
                        Message = "<p>-- Kritik Hastalıklar Ek teminatı oranı %1000 den fazla olamaz. </p>";
                    }
                }
            }

            return Message;
        },   // ==== krıtık hastalık
        MaluliyetYıllıkDestek_BedelKontrol: function () {

            var Message = "";

            if ($("#EkTeminatlar_MaluliyetYillikDestek_control").bootstrapSwitch('status')) {

                var tutar = $("#EkTeminatlar_MaluliyetYillikDestekSigortaBedeli").autoNumeric('get');

                if ($("#AnaTeminatlar_AnaTeminat").val() == "2")
                    Message = "<p>-- Vefat veya Kritik Hastalıklar ana teminatı ile beraber Maluliyet Yıllık Destek Teminatı birarada alınamaz</p>"

                if (parseInt(tutar) > 25) {
                    Message = "<p>-- Maluliyet Yıllık destek ek teminatı oranı %25'den fazla olamaz.</p>"
                }
            }

            return Message;
        },    // ==== malulıyet yıllık destek
        TamVeDaimiMaluliyet_BedelKontrol: function () {

            var Message = "";

            if ($("#EkTeminatlar_TamVeDaimiMaluliyet_control").bootstrapSwitch('status')) {

                var tutar = $("#EkTeminatlar_TamVeDaimiMaluliyetSigortaBedeli").autoNumeric('get');

                if ($("#EkTeminatlar_MaluliyetYillikDestek_control").bootstrapSwitch('status')) {

                    if (parseInt(tutar) > 50) {
                        Message = "<p>-- Tam ve Daimi Maluliyet Ek teminatı oranı %50 den fazla olamaz.</p>";
                    }
                }
                else {
                    if (parseInt(tutar) > 100) {
                        Message = "<p>-- Tam ve Daimi Maluliyet Ek teminatı oranı %100 den fazla olamaz.</p>";
                    }
                }
            }

            return Message;
        },      // ==== tam ve daımı malulıyet
        KSV_TTAKSV_BedelKontrol: function () {

            var Message = "";
            var TTAKSV = $("#EkTeminatlar_TopluTasimaAraclariKSVSigortaBedeli").autoNumeric('get');
            var KSV = $("#EkTeminatlar_KazaSonucuVefatSigortaBedeli").autoNumeric('get');

            var KazaSonucuVefat = 0;
            var TopluTasimaArac = 0;
            var toplam = 0;

            if (TTAKSV != "" && TTAKSV != "0") {
                toplam = parseInt(TTAKSV, 10);
                TopluTasimaArac = parseInt(TTAKSV, 10);
            }

            if (KSV != "" && KSV != "0") {
                toplam = toplam + parseInt(KSV, 10);
                KazaSonucuVefat = parseInt(KSV, 10);
            }


            if (KazaSonucuVefat > 0 && TopluTasimaArac > 0) {
                if (KazaSonucuVefat > 999) { Message += "<p>-- Kaza Sonucu Vefat Ek Teminatı oranı %999 dan fazla olamaz. </p>"; }
                if (TopluTasimaArac > 999) { Message += "<p>-- Toplu Taşıtta Kaza Sonucu Vefat Ek teminatı oranı %999 dan fazla olamaz </p>"; }
                if (toplam > 1000) {
                    Message += "<p>-- Kaza Sonucu Vefat Ek Teminatı ,Toplu Taşıtta Kaza Sonucu Vefat Ek teminatı toplamı oranı %1000 dan fazla olamaz.";
                }
            }
            else if (KazaSonucuVefat > 1000) {
                Message = "<p>-- Kaza Sonucu Vefat ek teminatı oranı %1000 den fazla olamaz. </p>";
            }
            else if (TopluTasimaArac > 1000) {
                Message = "<p>-- Toplu Taşıtta Kaza Sonucu Vefat ek teminat oranı %1000 den fazla olamaz. </p>";
            }

            return Message;
        },               // ==== Kaza Sonucu Vefat Teminatı, Toplu Taşıtta  Kaza Sonucu Vefat Teminatı 
        KSTMT_BedenKontrol: function () {

            var Message = "";
            var KSTMET = $("#EkTeminatlar_KazaSonucu_TedaviMasraflariBedeli").autoNumeric('get');

            if (KSTMET != "" && KSTMET != "0") {
                if (parseInt(KSTMET, 10) > 10) {
                    Message += "<p>-- Kaza Sonucu Tedavi Masrafları Ek Teminatı oranı %10 dan fazla olamaz.</p>";
                }
            }

            return Message;
        },                    // ==== Kaza Sonucu Tedavi Masrafları Teminatı
        KSHYTDHOT_BedenKontrol: function () {
            var Message = "";
            var KSHYTDHOT = $("#EkTeminatlar_KazaSonucu_HastanedeYatarakTD_HaftalikEkOdemeBedeli").autoNumeric('get');

            if (KSHYTDHOT != "" && KSHYTDHOT != "0") {
                if (parseInt(KSHYTDHOT, 10) > 2) {
                    Message += "<p>-- Kaza Sonucu Hastanede Yatarak Tedavi Durumunda Haftalık Ödeme Ek Teminatı oranı %2 den fazla olamaz.</p>";
                }
            }

            return Message;
        },                // ==== Kaza Sonucu Hastanede Yatarak Tedavi Durumunda Haftalık Ödeme Teminatı

        PrimDetayPartialGetir: function (IsDurum_id) {
            $.ajax({
                url: "/Teklif/TESabitPrimli/_DetayPartial",
                dataType: "html",
                method: "post",
                data: { IsDurum_id: IsDurum_id },
                success: function (result) {
                    if (result != null && result != "")
                        $(".detay-partial-div").html(result);
                },
                error: function () {
                    $.gritter.add({ title: 'Hata Mesajı!', text: "Teklif özet bilgileri alınamadı. Bir önceki sayfadan teklif özetine bakabilirsiniz." });
                }
            });
        }
    }
}



