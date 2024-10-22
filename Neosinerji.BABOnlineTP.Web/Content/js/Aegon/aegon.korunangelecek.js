function korunanGelecekTeklifWizardCallback(current) {
    //Hazırlayan bilgileri
    if (current == 1) {

    }
        //Sigorta ettiren / sigortali tab
    else if (current == 2) {
        var isValid = FormWizard.validatePage('#tab2');

        if (isValid == 1) {
            return korunanGelecek.musteriBilgileriKontrol();
        }
        else
            return false;
    }
        // Riziko Bilgileri
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
    korunanGelecek.startEkTeminatKapali();
    korunanGelecek.musteriZorunlulukKaldir();
    korunanGelecek.TekrarTeklifAcilisKontrol();

    $("#Musteri_SigortaEttiren_UlkeKodu").ulke({ il: "#Musteri_SigortaEttiren_IlKodu", ilce: "#Musteri_SigortaEttiren_IlceKodu" });

    //Doğum Tarihi Sınırlaması
    $("#Musteri_SigortaEttiren_DogumTarihi").datepicker("option", "maxDate", '-216m +0w');
    $("#Musteri_SigortaEttiren_DogumTarihi").datepicker("option", "minDate", '-792m +0w');

    //Sigorta Başlangıç tarihi
    $("#GenelBilgiler_SigortaBaslangicTarihi").datepicker("option", "minDate", '+0m +0w');

    // ==== Para Formatı Belirleniyor.
    $(".autoNumeric").autoNumeric('init', { vMin: '0', vMax: '99999999', mDec: 0 });
    $(".autoNumeric-custom").autoNumeric('init', { vMin: '0', vMax: '99999999', mDec: 2, aSep: ',', aDec: '.' });

    //EK Teminat Değiştiğinde calısacak
    $("#EkTeminatlar_MaluliyetYillikDestekTeminati_control").on("switch-change", korunanGelecek.EkTeminat);
});

$('#GenelBilgiler_KorunanGelecekBaslangicTarihi').change(function () {
    var d1 = $(this).datepicker("getDate");
    $("#GenelBilgiler_KorunanGelecekBitisTarihi").datepicker("option", "minDate", d1);
    //Max seyehat suresi 1 yıldır.

    setTimeout(function () {
        d1.setMonth(d1.getMonth() + 12);
        $("#GenelBilgiler_KorunanGelecekBitisTarihi").datepicker("option", "maxDate", d1);
        $("#GenelBilgiler_KorunanGelecekBitisTarihi").datepicker("show");
    }, 200);
});

$("#GenelBilgiler_ParaBirimi").change(function () { korunanGelecek.ParaBirimiChange(); });

$("#btn-hesapla").click(function () {
    var isvalid = $("#form1").valid();
    var form3 = $("#form3").valid();

    if (isvalid && form3) {
        $(this).button("loading");
        if (korunanGelecek.sigortaBilgileriKontrol()) {
            $(this).button("loading");

            $(".detay-partial-div").html('');
            $(".switcher").find(":input").switchFix();
            $(".autoNumeric,.autoNumeric-custom").each(function () {
                $(this).val($(this).autoNumeric('get'));
            });

            var contents = $("#form1, #form2, #form3").serialize();

            $.ajax(
                {
                    type: "POST",
                    url: "/Teklif/KorunanGelecek/Hesapla",
                    data: contents,
                    success: function (data) {
                        if (data.id > 0) {
                            teklifFiyat.kontol({ processId: data.id, guid: data.g });

                            korunanGelecek.KorunanGelecekDetayPartialGetir(data.id);

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
            $(".autoNumeric").autoNumeric('update', { vMin: '0', vMax: '9999999', mDec: 0 });
            $(".autoNumeric-custom").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 2, aSep: ',', aDec: '.' });
        } else { $("#btn-hesapla").button("reset"); }
    }
});

var korunanGelecek = new function () {
    return {

        init: function (data) {

        },

        TekrarTeklifAcilisKontrol: function () {
            korunanGelecek.musteriZorunlulukKaldir();
            //var data = { value: true };

            ////Ana Teminat Vefat
            //if ($("#AnaTeminatlar_Vefat_control").bootstrapSwitch('status')) {
            //    data = { value: true };
            //    korunanGelecek.vefatTeminatiChange(null, data)
            //}
            //else {
            //    data = { value: false };
            //    korunanGelecek.vefatTeminatiChange(null, data)
            //}           

            ////EK TEminat  Ek Ödeme
            //if ($("#EkTeminatlar_MaluliyetYillikDestekTeminati_control").bootstrapSwitch('status')) {
            //    data = { value: true };
            //    korunanGelecek.MaluliyetYillikDestekTeminatiChange(null, data)
            //}
            //else {
            //    data = { value: false };
            //    korunanGelecek.MaluliyetYillikDestekTeminatiChange(null, data)
            //}

        },

        musteriBilgileriKontrol: function () {
            var Message = "";

            //Cinsiyet Kontrol
            if (!$("#Cinsiyet_K").is(':checked') && !$("#Cinsiyet_E").is(':checked')) {
                Message = "<p>--Lütfen cinsiyet seciniz.</p>";
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
                Message += "<p> --Sigortalı yaşı 18 den küçük olamaz.</p>";

            today = new Date();
            today.setYear(today.getFullYear() - 66);
            if (dogum < today)
                Message += "<p> --Vefat teminatı için maksimum giriş yaşı 65’tir. Maluliyet Yıllık Destek Teminatı için maksimum giriş yaşı 55’tir.</p>";

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

        sigortaBilgileriKontrol: function () {
            var Message = "";

            Message += korunanGelecek.SigortaSuresiKontrol();
            Message += korunanGelecek.TeminatSureKontrol();
            Message += korunanGelecek.VefatTeminatiKontrol();

            if (Message == "")
                return true;
            else {
                $("#hata-message-div").html(Message);
                $("#hata-modal").modal("show");
                return false;
            }

        },

        SigortaSuresiKontrol: function () {

            var Message = "";
            var sure = $("#GenelBilgiler_SigortaSuresi").val();

            if (sure != "") {
                var sigortaBaslangic = $("#GenelBilgiler_SigortaBaslangicTarihi").val();
                var dogumTarihi = $("#Musteri_SigortaEttiren_DogumTarihi").val();



                if (sigortaBaslangic != "" && dogumTarihi != "") {
                    $.ajax({
                        url: "/Teklif/KorunanGelecek/SigortaSuresiHesapla",
                        data: { dogumTarihi: dogumTarihi, sigortaBaslangic: sigortaBaslangic, sure: sure },
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

        VefatTeminatiKontrol: function () {

            var Message = "";
            var vefatTutar = $("#AnaTeminatlar_VefatBedeli").autoNumeric('get');

            if (vefatTutar !== undefined && vefatTutar != "" && vefatTutar != "0") {

                $.ajax({
                    url: "/Teklif/KorunanGelecek/GetVefatTeminatTutariLimit",
                    method: "post",
                    async: false,
                    success: function (result) {

                        if (result !== undefined) {

                            var paraBirimi = $("#GenelBilgiler_ParaBirimi").val();

                            switch (paraBirimi) {
                                case "1":
                                    if (vefatTutar < result.avro)
                                    { Message += "<p>-- Ana teminat tutarı, minimum teminat tutarı limiti olan " + result.avro + " Avro altındadır.</p>"; }
                                    break; //Euro
                                case "2":
                                    if (vefatTutar < result.dolar)
                                    { Message += "<p>-- Ana teminat tutarı, minimum teminat tutarı limiti olan " + result.dolar + " ABD Doları altındadır.</p>"; }
                                    break; //ABD Doları
                                default: Message += "<p>-- Lütfen para birimi seçiniz."; break;
                            }
                        }
                        else { Message += "<p>-- Vefat teminat limiti hesaplanırken bir hata oluştu. Lüften tekrar deneyin.</p>" }
                    },
                    error: function () { Message += "<p>-- Vefat teminat limiti hesaplanırken bir hata oluştu. Lüften tekrar deneyin.</p>" }
                });
            }
            else { Message += "<p>-- Lütfen vefat teminat tutarını giriniz.</p>" }

            return Message;
        },

        KorunanGelecekDetayPartialGetir: function (IsDurum_id) {
            $.ajax({
                url: "/Teklif/KorunanGelecek/_DetayPartial",
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
        },

        startEkTeminatKapali: function () {
            //// ANA Teminatlar Değiştiğinde calısıcak
            $("#AnaTeminatlar_Vefat").attr("disabled", "");

            //Ek Teminat
            var object = $("#EkTeminatlar_MaluliyetYillikDestekTeminatiBedeli");
            $("#MaluliyetYillikDestekTeminati").slideUp("fast");
            object.addClass('ignore');
        },

        //EK TEMİNAT CHANGE
        EkTeminat: function (e, data) {

            var object = $("#EkTeminatlar_MaluliyetYillikDestekTeminatiBedeli");

            if (data.value) {
                $("#MaluliyetYillikDestekTeminati").slideDown("fast");
                object.removeClass('ignore');
            }
            else {
                $("#MaluliyetYillikDestekTeminati").slideUp("fast");
                object.addClass("ignore");
                object.val('');
            }
        },

        TeminatSureKontrol: function () {
            var Message = "";
            var DogTar = $("#Musteri_SigortaEttiren_DogumTarihi").val();
            var SigBas = $("#GenelBilgiler_SigortaBaslangicTarihi").val();
            var SigSure = $("#GenelBilgiler_SigortaSuresi").val();

            var anaTeminat = $("#AnaTeminatlar_Vefat_control").bootstrapSwitch("status");
            var maluliyetyillikDestek = $("#EkTeminatlar_MaluliyetYillikDestekTeminati_control").bootstrapSwitch("status");
            $.ajax({
                url: "/Teklif/KorunanGelecek/SigortaliYasHesapla",
                method: "post",
                async: false,
                data: {
                    DogTar: DogTar,
                    SigBas: SigBas,
                    SigSure: SigSure,
                    AnaTeminat: anaTeminat,
                    Maluliyet: maluliyetyillikDestek
                },
                success: function (data) {
                    if (data != "0" && data != "") {
                        Message += data;
                    }
                },
                error: function () { Message = "Yaş hesaplanırken bir sorun oluştu, Lütfen tekrar deneyin." }
            });

            return Message;
        },

        ParaBirimiChange: function () {
            var paraBirimi = $("#GenelBilgiler_ParaBirimi").val();
            if (paraBirimi == "") {
                $(".ParaBirimi").html("");
            }
            else if (paraBirimi == "1") {
                $(".ParaBirimi").html('€');
            }
            else if (paraBirimi == "2") {
                $(".ParaBirimi").html('$');
            }

        },



    }
}
