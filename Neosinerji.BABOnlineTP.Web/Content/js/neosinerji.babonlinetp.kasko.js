$(document).ready(function () {
    $("#Arac_KullanimSekliKodu").aracmarka({
        tarz: "#Arac_KullanimTarziKodu",
        marka: "#Arac_MarkaKodu",
        model: "#Arac_Model",
        tip: "#Arac_TipKodu"
    });


    $("#OdemeTipi_6[name='Odeme.OdemeTipi']").attr("checked", "checked");

    // ==== Odeme tipi, şekli ve taksit sayısı kontrol ediliyor. ==== //
    $("#Odeme_OdemeSekli_control").on("switch-change", OdemeSekliDegisti);

    // ==== Tarih ayarları ==== //
    $('#Arac_TrafikTescilTarihi').change(function () {
        var d1 = $(this).datepicker("getDate");
        $("#Arac_TrafigeCikisTarihi").datepicker("setDate", d1);
        setTimeout(function () { $("#Arac_TrafigeCikisTarihi").datepicker("show"); }, 100);
    });

    // ==== Validasyon başlangıcta yapılmıyor ==== //
    $("#Teminat_LPGAracModel_Markasi").addClass("ignore");
    $("#Teminat_LPGAracModel_Bedeli").addClass("ignore");

    // ==== Arac tipi secildiginde fiyat ve kişi sayısı geliyor ==== //
    $("#Arac_TipKodu").change(function () {
        var tipKodu = $("#Arac_TipKodu").val();
        $("#type-code").val(tipKodu);
        GetAracDegerKisiSayisi();
    });
    $("#Arac_MarkaKodu").change(function () {
        var markaKodu = $("#Arac_MarkaKodu").val();
        $("#brand-code").val(markaKodu);
    });

    // ==== Aracın fiyatı değiştiğinde %10 dan fazla olmaması kontrol ediliyor ==== //
    //$("#Arac_AracDeger").change(function () {
    //    AracDegerKontrol();
    //});
    $("#Arac_AracDeger").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0, aSep: '.', aDec: ',' });

    // ==== Sağlık teminatı değiştiğinde kişi sayısı soruluyor. ==== // 
    $("#Teminat_Saglik_control").on("switch-change", SaglikKontrol);

    // ==== Yurtdışı teminat kontrol ediliyor ==== //
    $("#Teminat_Yutr_Disi_Teminat_control").on("switch-change", YurtdisiTeminatKontrol);

    // ==== Lpgli Arac varmı kontrol ediliyor ==== //
    $("#Teminat_LPGLi_Arac_control").on("switch-change", LPGVarmiKontrol);

    // ==== Lpgli Arac varmı kontrol ediliyor ==== //
    $("#Teminat_LPG_Arac_Orjinalmi_control").on("switch-change", LPGOrjinalmi);

    // ==== Daini murtein kontrol ediliyor. ==== //
    $("#DainiMurtein_DainiMurtein_control").on("switch-change", function (e, data) {
        if (data.value) {
            $("#dainimurtein").slideDown("fast");
        }
        else {
            $("#dainimurtein").slideUp("fast");
        }
        dainiMurteinSetValidation(data.value);
    });

    $("#DainiMurtein_KurumTipi").change(function () {
        $.getJSON('/Kasko/KaskoDMSorgula', { kurumTipi: $(this).val() },
            function (result) {
                $("#DainiMurtein_KurumKodu").dropDownFill(result);
                $("#DainiMurtein_KurumKodu").prepend("<option value='' selected='selected'>" + language.get('select') + "</option>");
            });
    });

    $("#DainiMurtein_KurumKodu").change(function () {
        $("#DainiMurtein_KurumKodu1").val($(this).val());
    });

    $("#DainiMurtein_KurumKodu1").focusout(function () {
        if ($("#DainiMurtein_KurumKodu option[value='" + $(this).val() + "']").length > 0) {
            $("#DainiMurtein_KurumKodu").val($(this).val());
        }
    });


    $("#btn-daini-sorgula").click(function () {
        var isValid = $("#DainiMurtein_KimlikNo").valid();
        if (!isValid) {
            $("#daini-sorgu-hata").html("Lütfen Vergi Kimlik No giriniz.");
            $("#daini-sorgu-hata").show();
            return;
        }
        var kimlikNo = $("#DainiMurtein_KimlikNo").val();
        if (kimlikNo.length != 10) {
            $("#daini-sorgu-hata").html("Vergi Kimlik No 10 hane olmalıdır.");
            $("#daini-sorgu-hata").show();
            return;
        }

        $("#btn-daini-sorgula").button("loading");
        $("#DainiMurtein_Unvan").val("");
        $.ajax({
            type: "post",
            dataType: "json",
            url: "/Teklif/Teklif/KimlikNoSorgula",
            data: { kimlikNo: kimlikNo },
            success: function (res) {
                if (res.SorgulamaSonuc && res.AdiUnvan != null) {
                    var adSoyad = res.AdiUnvan + ' ' + res.SoyadiUnvan;
                    $("#DainiMurtein_Unvan").val(adSoyad);
                    $("#DainiMurtein_Unvan").valid();
                    $("#daini-sorgu-hata").hide();

                    $.ajax({
                        type: "post",
                        dataType: "json",
                        url: "/Teklif/Kasko/DainiKurum",
                        data: { adSoyad: adSoyad },
                        success: function (res) {
                            if (res.success) {
                                $("#DainiMurtein_KurumTipi").val(res.KurumTipi);
                                $("#DainiMurtein_KurumKodu").dropDownFill(res.Kurumlar);
                                $("#DainiMurtein_KurumKodu").val(res.KurumKodu);
                            }
                        },
                        error: function () {
                        }
                    });
                }
                else {
                    $("#daini-sorgu-hata").html(res.HataMesaj);
                    $("#daini-sorgu-hata").slideDown();
                }
                $("#btn-daini-sorgula").button("reset");
            },
            error: function () {
                $("#daini-sorgu-hata").html("Kullanıcı adı sorgulanırken hata oluştu.");
                $("#btn-daini-sorgula").button("reset");
            }
        });
    });

    $("#EskiPolice_EskiPoliceVar_control").on("switch-change", function (e, data) {
        if (data.value) {
            $("#eski-police").slideDown("fast");
        }
        else {
            $("#eski-police").slideUp("fast");
        }
        eskiPoliceSetValidation(data.value);
    });
    $("#Tasiyici_Sorumluluk_control").on("switch-change", function (e, data) {
        if (data.value) {
            $("#tasiyici-sorumluluk").slideDown("fast");
        }
        else {
            $("#tasiyici-sorumluluk").slideUp("fast");
        }

        tasiyiciSorumlulukValidation(data.value);
    });

    $("#Arac_PlakaNo").blur(function () {
        $(this).val($(this).val().toUpperCase());
    });

    $("#Arac_TescilBelgeSeriKod").blur(function () {
        $(this).val($(this).val().toUpperCase());
    });

    $("#Arac_PlakaKodu").change(function () {
        $("#Arac_TescilIl").val($(this).val());
        $("#Arac_TescilIl").trigger("change");
    });

    $("#Arac_TescilIl").tescilil({ ilce: "#Arac_TescilIlce" });

    $("#btn-sorgula").click(function () {
        var isValid = $("#Arac_PlakaNo").valid();

        if (isValid) {

            $("#btn-sorgula").button("loading");
            $("#button-next").prop("disabled", true);
            $('.button-previous').css('pointer-events', 'none');
            var plakaKodu = $("#Arac_PlakaKodu").val();
            var plakaNo = $("#Arac_PlakaNo").val();

            if (plakaNo == "YK") {
                $("#Arac_TrafikTescilTarihi").val(bugunTarih);
                $("#Arac_TrafigeCikisTarihi").val(bugunTarih);

                setPlakaSorgu();

                $("#Arac_TescilIlce").addClass("ignore");
                return;
            }


            var sigortaliAyni = $("#Musteri_SigortaliAyni_control").bootstrapSwitch('status');
            var musteriKodu;
            if (sigortaliAyni) {
                musteriKodu = $("#Musteri_SigortaEttiren_MusteriKodu").val();
            } else {
                musteriKodu = $("#Musteri_Sigortali_MusteriKodu").val();
            }


            $.ajax({
                dataType: "json",
                url: "/Teklif/Kasko/PlakaSorgula", //PlakaSorgulaMapfre
                data: { PlakaKodu: plakaKodu, PlakaNo: plakaNo, MusteriKodu: musteriKodu },
                success: setPlakaSorgu,
                error: errorPlakaSorgu
            });
        }
    });

    $("#btn-egm-sorgula").click(function () {
        var isValid = $("#Arac_PlakaKodu").valid() && $("#Arac_PlakaNo").valid()

        if ($("#Arac_PlakaKodu").val() == "0") {
            $("#Arac_PlakaKodu").val("");
            isValid = $("#Arac_PlakaKodu").valid();
        }

        if (isValid) {
            var plakaKodu = $("#Arac_PlakaKodu").val();
            var plakaNo = $("#Arac_PlakaNo").val();

            if (plakaNo == "YK") {
                return;
            }
            var aracRuhsatSeriNo = $("#Arac_TescilBelgeSeriKod").val();
            var aracRuhsatNo = $("#Arac_TescilBelgeSeriNo").val();
            var asbisNo = $("#Arac_AsbisNo").val();
            if (aracRuhsatNo.length == 0 && asbisNo.length == 0) {
                $("#egm-sorgu-hata").html("Egm sorgusu yapabilmek için tescil belge seri no yada asbis no bilgisini girmelisiniz.");
                $("#egm-sorgu-hata").show();
                return;
            }

            $("#btn-egm-sorgula").button("loading");
            $("#button-next").prop("disabled", true);
            $('.button-previous').css('pointer-events', 'none');

            $.ajax({
                dataType: "json",
                url: "/Teklif/Kasko/EgmSorgu",
                data: {
                    PlakaKodu: plakaKodu,
                    PlakaNo: plakaNo,
                    AracRuhsatSeriNo: aracRuhsatSeriNo,
                    AracRuhsatNo: aracRuhsatNo,
                    AsbisNo: asbisNo
                },
                success: setEgmSorgu,
                error: errorEgmSorgu
            });
        }
    });

    $("#btn-hesapla").click(function () {

        var isvalid = $("#form1").valid();
        if (isvalid) {

            $(this).button("loading");

            var aracDeger = $("#Arac_AracDeger").autoNumeric('get');
            $("#Arac_AracDeger").val(aracDeger);

            $(".switcher").find(":input").switchFix();

            var disabled = $("#form2,#form3,#form4").find(':input:disabled').removeAttr('disabled');
            var contents = $("#form1, #form2, #form3, #form4, #form5").serialize();
            disabled.attr('disabled', 'disabled');


            $("#Arac_AracDeger").autoNumeric('set', aracDeger);

            $.ajax(
                {
                    type: "POST",
                    url: "/Teklif/Kasko/Hesapla",
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
                        }
                        else if (data.id == 0) {
                            $("#btn-hesapla").button("reset");
                            alert(data.hata);
                        }
                        $("#btn-hesapla").button("reset");
                    },
                    error: function () {
                        $("#btn-hesapla").button("reset");
                    }
                });
        }
    });

    $("#brand-code").focusout(function () {
        var marka = $("#brand-code").val();
        if (marka && marka != '') {
            $("#Arac_MarkaKodu").val(marka);
            $.getJSON('/Kasko/AracTipiGetir', { MarkaKodu: marka },
                function (result) {
                    $("#Arac_TipKodu").dropDownFill(result);
                    var tipKodu = $("#type-code").val();
                    if (tipKodu != '') {
                        $("#Arac_TipKodu").val(tipKodu);
                        GetAracDegerKisiSayisi();
                    }
                });
            var modelYili = $("#Arac_Model").val();

            if (modelYili > 0) {
                anadoluSigortaKullanimTipiSorgula();
            }
        }
    });

    $("#type-code").focusout(function () {
        var marka = $("#brand-code").val();
        var tip = $("#type-code").val();
        if (marka && marka != '' && tip && tip != '') {
            $("#type-sorgu-hata").html("");
            $("#type-sorgu-hata").hide();

            $("#Arac_MarkaKodu").val(marka);
            $("#Arac_TipKodu").val(tip);
            GetAracDegerKisiSayisi();
        }
    });

    if ($("#Arac_PlakaNo").length > 0) {
        var plakaNo = $("#Arac_PlakaNo").val();
        if (plakaNo.length == 0) {
            $(".arac-bilgileri :input").addClass("ignore");
        } else {
            $(".arac-bilgileri").show();
        }
    }

    $("#Arac_TescilBelgeSeriNo").numeric();


    $("#EskiPolice_SigortaSirketKodu").focusout(function () {
        var sirketKod = $(this).val();
        sirketKod = padLeft(sirketKod, 3);
        $("#EskiPolice_SigortaSirketiKodu").val(sirketKod);
    });

    $("#EskiPolice_SigortaSirketiKodu").change(function () {
        $("#EskiPolice_SigortaSirketKodu").val($("#EskiPolice_SigortaSirketiKodu").val());
    });

    $("#Arac_AnadoluKullanimSekli").change(function () {
        var AracKodu = $("#Arac_MarkaKodu").val() + $("#Arac_TipKodu").val();
        var KullanimSekli = $("#Arac_AnadoluKullanimSekli").val();
        var KullanimTipi = $("#Arac_AnadoluKullanimTip").val();

        if ((AracKodu != null && AracKodu != '') && (KullanimSekli != null && KullanimSekli != '') && (KullanimTipi != null && KullanimTipi != '')) {

            $("#anadolu-ikame-progress").show();

            $.getJSON('/Teklif/Kasko/IkameListAnadolu', { aracKodu: AracKodu, kullanimSekliKodu: KullanimSekli, kullanimTipKodu: KullanimTipi },
                function (result) {
                    if (result.hata != "" && result.hata != null) {
                        $("#anadolu-kasko-ikame-hata-satir").css({ "display": "block" });
                        $("#anadolu-ikame-hata").html(result.hata);
                        $("#anadolu-ikame-progress").hide();
                    }
                    else {
                        $("#anadolu-kasko-ikame-hata-satir").css({ "display": "none" });
                        $("#anadolu-ikame-hata").hide();
                        $("#anadolu-ikame-hata").html("");
                    }
                    if (result.list.length > 0) {
                        $("#anadolu-ikame-progress").hide();
                        $("#Arac_IkameTuruAnadolu").dropDownFill(result.list);
                        $("#ikame_Anadolu").show();

                    }
                    else {
                        $("#ikame_Anadolu").hide();
                        $("#anadolu-ikame-progress").hide();
                    }
                });
        }
        else {
            $("#ikame_Anadolu").hide();
            $("#anadolu-ikame-progress").hide();
        }
    });

    $("#Arac_KullanimTarziKodu").change(function () {
        var kTarzi = $("#Arac_KullanimTarziKodu").val();
        $.ajax({
            dataType: "json",
            url: "/Teklif/Kasko/TasinanYukKademeleri",
            data: { AracKullanimTarzi: kTarzi },
            success: function (data) {
                $("#Teminat_TasinanYukKademe").dropDownFill(data.list);
            },
            error: {

            }
        });
    });

    $("#Teminat_TicariBireysel_control").on("switch-change", function () {
        var Tip = $("#Teminat_TicariBireysel_control").bootstrapSwitch('status');
        $("#Hazirlayan_TVMKodu").removeAttr('disabled');
        var tvmKod = $("#Hazirlayan_TVMKodu").val();
        if (!Tip) {
            $.ajax({
                dataType: "json",
                url: "/Teklif/Kasko/SompoJapanFaaliyetKodlari",
                data: { tvmKodu: tvmKod },
                success: function (data) {
                    $("#Teminat_FaaliyetKodu").dropDownFill(data.list);
                    $("#SompoJapanFaaliyetKod").show();
                    $("#Hazirlayan_TVMKodu").attr("disabled", true);
                },
                error: {

                }
            });
        }
        else {
            $("#SompoJapanFaaliyetKod").hide();
            $("#Hazirlayan_TVMKodu").attr("disabled", true);
        }
    });

    $("#Teminat_KazaDestekVarMi_control").on("switch-change", function () {
        var Teminat = $("#Teminat_KazaDestekVarMi_control").bootstrapSwitch('status');
        if (Teminat) {
            $.ajax({
                dataType: "json",
                url: "/Teklif/Kasko/GetGroupamaTeminatLimiti",
                success: function (data) {
                    $("#Teminat_GroupamaTeminatLimiti").dropDownFill(data.list);
                    $("#kazaDestekLimiti").show();
                },
                error: {

                }
            });
        }
        else {
            $("#kazaDestekLimiti").hide();
        }
    });

    $("#btn-digerTeklifEkle").click(function () {

        var contents = $(".formTeklif").serialize();
        $.ajax(
            {
                type: "POST",
                url: "/Teklif/Kasko/DigerTeklifKaydetDetay",
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
                    }
                    else if (data.id == 0) {
                        $("#btn-hesapla").button("reset");
                        alert(data.hata);
                    }
                    $("#btn-hesapla").button("reset");
                },
                error: function () {
                    $("#btn-hesapla").button("reset");
                }
            });
    });

});


$("#Arac_AnadoluKullanimTip").change(function () {

    $("#kullanimSekli_Anadolu").hide();
    $("#ikame_Anadolu").hide();
    $("#anadolu-kasko-ikame-hata-satir").css({ "display": "none" });
    $("#anadolu-kullanimsekli-progress").show();
    $("#anadolu-kasko-ksekli-hata-satir").css({ "display": "none" });
    var KullanimTarzi = $("#Arac_KullanimTarziKodu").val();
    var Model = $("#Arac_Model").val();
    var MarkaKodu = $("#Arac_MarkaKodu").val();
    var KullanimTipi = $("#Arac_AnadoluKullanimTip").val();
    var AnadoluMarkaKodu = $("#Arac_AnadoluMarkaKodu").val();

    if ((KullanimTarzi != null && KullanimTarzi != '') && (Model != null && Model != '') && (MarkaKodu != null && MarkaKodu != '') && (KullanimTipi != null && KullanimTipi != '')) {
        //$.getJSON('/Teklif/Kasko/KullanimSekliSorgulaAnadolu', { AnadoluMarkaKodu: AnadoluMarkaKodu, AracKullanimTarzi: KullanimTarzi, AracModelYili: Model, AracMarkaKodu: MarkaKodu, KullanimTipi:
        //    KullanimTipi
        //},
            $.getJSON('/Teklif/Kasko/KullanimSekliSorgulaAnadolu', {
                AnadoluKullanimTipi:
                    KullanimTipi
            },
            
            function (result) {
                if (result.hata != "" && result.hata != null) {
                    $("#anadolu-kasko-ksekli-hata-satir").css({ "display": "block" });
                    $("#anadolu-kasko-ksekli-hata").html(result.hata);
                }
                else {
                    $("#anadolu-kasko-ktip-hata").html("");
                    $("#anadolu-kasko-ksekli-hata").html("");
                    $("#anadolu-kasko-ktip-hata-satir").css({ "display": "none" });
                    $("#anadolu-kasko-ksekli-hata-satir").css({ "display": "none" });
                }
                if (result.list.length > 1) {
                    $("#anadolu-kasko-ktip-hata-satir").css({ "display": "none" });
                    $('#anadolu-kasko-ksekil-hata-satir').css('display', 'none');
                }

                $("#Arac_AnadoluKullanimSekli").dropDownFill(result.list);
                $("#kullanimSekli_Anadolu").show();
                $("#anadolu-kullanimsekli-progress").hide();
            });
    }
    else {
        $("#kullanimSekli_Anadolu").hide();
    }
});

$("#Arac_Model").change(function () {

    anadoluSigortaKullanimTipiSorgula();
});

$("#Arac_MarkaKodu").change(function () {

    var modelYili = $("#Arac_Model").val();

    if (modelYili > 0) {
        anadoluSigortaKullanimTipiSorgula();
    }
});

var aracKisiSayisiEgm = false;
var ikameTuruSorgulandi = false;

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
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

function SaglikKontrol(e, data) {
    if (data.value) {
        $("#saglik-kisi-sayisi").slideDown("fast");
    }
    else {
        $("#saglik-kisi-sayisi").slideUp("fast");
    }
}

function YurtdisiTeminatKontrol(e, data) {
    if (data.value) {
        $("#yurtdisi-teminat").slideDown("fast");
    }
    else {
        $("#yurtdisi-teminat").slideUp("fast");
    }
}

function LPGVarmiKontrol(e, data) {
    if (data.value) {
        $("#lpg-arac-orjinalmi").slideDown("fast");
    }
    else {
        $("#lpg-arac-orjinalmi").slideUp("fast");
        $("#lpg-marka-bedel").slideUp("fast");
        $("#Teminat_LPG_Arac_Orjinalmi_control").bootstrapSwitch('setState', true);
    }
}

function LPGOrjinalmi(e, data) {
    if (data.value) {
        $("#lpg-marka-bedel").slideUp("fast");
        $("#Teminat_LPGAracModel_Markasi").addClass("ignore");
        $("#Teminat_LPGAracModel_Markasi").val("");
        $("#Teminat_LPGAracModel_Bedeli").addClass("ignore");
        $("#Teminat_LPGAracModel_Bedeli").val("");
    }
    else {
        $("#lpg-marka-bedel").slideDown("fast");
        $("#Teminat_LPGAracModel_Markasi").removeClass("ignore");
        $("#Teminat_LPGAracModel_Markasi").val("");
        $("#Teminat_LPGAracModel_Bedeli").removeClass("ignore");
        $("#Teminat_LPGAracModel_Bedeli").val("");
    }
}

// ==== Aracın Değerini ve kişi sayısını getirir ==== //
function GetAracDegerKisiSayisi() {
    var tipkodu = $("#Arac_TipKodu").val();
    var markakodu = $("#Arac_MarkaKodu").val();
    var model = $("#Arac_Model").val();
    var kullanimtarzi = $("#Arac_KullanimTarziKodu").val();

    if (tipkodu > 0 && markakodu > 0 && model > 0) {
        $.ajax({
            dataType: "json",
            url: "/Teklif/Kasko/GetAracDegerKisiSayisi",
            data: { TipKodu: tipkodu, MarkaKodu: markakodu, Model: model, KullanimTarzi: kullanimtarzi },
            success: function (data) {

                $("#Arac_AracDeger").val(data.AracDeger);
                $("#Arac_KisiSayisi").val(data.KisiSayisi);
                if (data.ProjeKodu == "Mapfre") {
                    $("#Teminat_AMSKodu").dropDownFill(data.IMMList);
                    if (data.IMMList.length > 1) {
                        $("#Teminat_AMSKodu").prop("selectedIndex", 1);
                    }
                } else {
                    $("#Teminat_IMMKodu").dropDownFill(data.IMMList);
                    $("#Teminat_FKKodu").dropDownFill(data.FKList);
                }
            },
            error: function (data) { }
        });
    }
}

// ==== Mapfre sigorta AMS kodlarını getirir ==== //
function GetAracAMSKodlari() {
    var kullanimTarzi = $("#Arac_KullanimTarziKodu").val();
    if (kullanimTarzi.length > 0) {
        $.ajax({
            dataType: "json",
            type: "post",
            url: "/Teklif/Kasko/GetKaskoAMSListe",
            data: { kullanimTarzi: kullanimTarzi },
            success: function (data) {
                $("#Teminat_AMSKodu").dropDownFill(data);
            },
            error: function (data) { }
        });
    }
}

// ==== Aracın değeri kontrol ediliyor ==== //
function AracDegerKontrol() {
    var tipkodu = $("#Arac_TipKodu").val();
    var markakodu = $("#Arac_MarkaKodu").val();
    var model = $("#Arac_Model").val();
    var yenifiyat = $("#Arac_AracDeger").val()

    if (tipkodu > 0 && markakodu > 0 && model > 0) {
        $.ajax({
            dataType: "json",
            url: "/Teklif/Kasko/AracDegerKontrol",
            data: { TipKodu: tipkodu, MarkaKodu: markakodu, Model: model, AracDeger: yenifiyat },
            success: function (data) {
                if (data.Result == true) {
                }
                else if (data.Result == false) {
                    $("#Arac_AracDeger").val(data.OrjinalDeger);
                    alert("Aracın fiyatı 10% fazla değiştirilemez")
                }
                else {
                    $("#Arac_AracDeger").val(data.OrjinalDeger)
                    alert("Bir hata oluştu lütfen tekrar deneyin.");
                }
            },
            error: function () {
                $("#Arac_AracDeger").val("")
                alert("Bir hata oluştu lütfen tekrar deneyin.");
            }
        });
    }
}

// ==== Daini Munteir değişiklik ==== //
function dainiMurteinSetValidation(value) {
    if (value) {
        //enable control validation
        $("#DainiMurtein_KimlikNo").removeClass("ignore");
        $("#DainiMurtein_Unvan").removeClass("ignore");

        $("#DainiMurtein_KurumTipi").removeClass("ignore");
        $("#DainiMurtein_KurumKodu").removeClass("ignore");
        $("#DainiMurtein_KurumKodu1").removeClass("ignore");
    }
    else {
        //disable control validation
        $("#DainiMurtein_KimlikNo").addClass("ignore");
        $("#DainiMurtein_Unvan").addClass("ignore");

        $("#DainiMurtein_KurumTipi").addClass("ignore");
        $("#DainiMurtein_KurumKodu").addClass("ignore");
        $("#DainiMurtein_KurumKodu1").addClass("ignore");
    }
}

function setPlakaSorgu(data) {
    $("#plaka-sorgu-hata").hide();
    if (data) {

        $("#Arac_KullanimSekliKodu").val(data.AracKullanimSekli);
        $("#Arac_KullanimTarziKodu").dropDownFill(data.Tarzlar);
        $("#Arac_KullanimTarziKodu").val(data.AracKullanimTarzi);
        $("#Arac_MarkaKodu").dropDownFill(data.Markalar);
        $("#Arac_MarkaKodu").val(data.AracMarkaKodu);

        $("#brand-code").val(data.AracMarkaKodu);
        $("#type-code").val(data.AracTipKodu);

        $("#Arac_KisiSayisi").val(data.AracKoltukSayisi);
        $("#Arac_Model").val(data.AracModelYili);
        $("#Arac_TipKodu").dropDownFill(data.Tipler);
        $("#Arac_TipKodu").val(data.AracTipKodu);
        $("#Arac_AracDeger").val(data.AracDegeri);
        $("#Arac_MotorNo").val(data.AracMotorNo);
        $("#Arac_SaseNo").val(data.AracSasiNo);

        if (data.HasarsizlikHata == "") {
            $("#Arac_HasarsizlikIndirim").val(data.HasarsizlikInd);
            $("#Arac_HasarSurprim").val(data.HasarsizlikSur);
            $("#Arac_UygulananKademe").val(data.HasarsizlikKademe);
        }



        if (data.AracMarkaKodu != 600) {
            if (data.AnadoluKullanimTipleri != null) {
                if (data.AnadoluKullanimTipleri.length > 0) {
                    var divSayisi = 0;
                    //Sigorta şirketlerinin listesi okunuyor
                    $("html").find(".tum-no").each(function (i) {
                        divSayisi = i + 1;
                    });

                    for (var i = 0; i < divSayisi; i++) {
                        //Anadolu sigorta şirketinin E/H durumu okunuyor
                        var tumValue = $("#TeklifUM_" + i + "__TUMKodu").val();
                        if (tumValue == 4) {
                            var Anadolu = $("#TeklifUM_" + i + "__TeklifAl_control").bootstrapSwitch('status');
                            if (Anadolu) {

                                $("#anadolu-ozel").show();
                                $("#anadolu-kasko-ktip-hata-satir").css({ "display": "none" });
                                $("#anadolu-kasko-ksekli-hata-satir").css({ "display": "none" });
                                $("#anadolu-kasko-ksekli-hata").html("");
                                $("#anadolu-kasko-ikame-hata-satir").css({ "display": "none" });
                                $("#anadolu-ikame-hata").html("");
                                $("#kullanimSekli_Anadolu").hide();

                                if (data.AnadoluHata != "" && data.AnadoluHata != null) {
                                    $("#anadolu-kasko-ktip-hata-satir").css({ "display": "block" });
                                    $("#anadolu-kasko-ktip-hata").html(data.AnadoluHata);
                                }
                                else {
                                    $("#anadolu-kasko-ktip-hata-satir").css({ "display": "none" });
                                    $("#anadolu-kasko-ktip-hata").html("");
                                }

                                $("#Arac_AnadoluKullanimTip").dropDownFill(data.AnadoluKullanimTipleri);
                                $("#kullanimTip_Anadolu").show();
                                $("#Arac_AnadoluMarkaKodu").val(data.anadoluMarkaKodu);
                            }
                            else {
                                $("#anadolu-ozel").hide();
                                $("#kullanimTip_Anadolu").hide();
                                $("#anadolu-hata").html(data.AnadoluHata);
                                $("#anadolu-hata").show();
                            }
                        }
                    }
                }
                else {
                    $("#kullanimTip_Anadolu").hide();
                    $("#anadolu-hata").html(data.AnadoluHata);
                    $("#anadolu-hata").show();
                }
            }
            else {
                $("#kullanimTip_Anadolu").hide();
                $("#anadolu-hata").html(data.AnadoluHata);
            }
        }
        else {
            $("#anadolu-ozel").show();
            $("#anadolu-kasko-ktip-hata-satir").css('display', 'block');
            $("#anadolu-kasko-ktip-hata").html("Hata! Anadolu Sigorta'nın özel alanları yüklenemedi. Anadolu Sigorta, Markası Motosiklet olan araçlar için Online Teklif düzenletmemektedir.");
        }


        if (data.AracTescilTarih != null && data.AracTescilTarih.length > 0) {
            $('#Arac_TrafikTescilTarihi').datepicker('setDate', data.AracTescilTarih);
            $('#Arac_TrafigeCikisTarihi').datepicker('setDate', data.AracTescilTarih);
        }

        if (data.EskiPoliceNo != null && data.EskiPoliceNo.length > 0 && data.EskiPoliceNo != "0" && data.EskiPoliceNo != "") {
            $("#EskiPolice_EskiPoliceVar_control").bootstrapSwitch('setState', true);
            $("#EskiPolice_SigortaSirketiKodu").val(data.EskiPoliceSigortaSirkedKodu);
            $("#EskiPolice_AcenteNo").val(data.EskiPoliceAcenteKod);
            $("#EskiPolice_PoliceNo").val(data.EskiPoliceNo);
            $("#EskiPolice_YenilemeNo").val(data.EskiPoliceYenilemeNo);
            $("#EskiPolice_SigortaSirketKodu").val(data.EskiPoliceSigortaSirkedKodu);
            $("#EskiPolice_SigortaSirketiKodu").prop('disabled', true);
            $("#EskiPolice_AcenteNo").prop('disabled', true);
            $("#EskiPolice_PoliceNo").prop('disabled', true);
            $("#EskiPolice_YenilemeNo").prop('disabled', true);
            $("#EskiPolice_SigortaSirketKodu").prop('disabled', true);
            if (data.YeniPoliceBaslangicTarih.length > 0) {
                $('#Arac_PoliceBaslangicTarihi').datepicker('setDate', data.YeniPoliceBaslangicTarih);
            }
        }
        else {
            $("#EskiPolice_EskiPoliceVar_control").bootstrapSwitch('setState', false);
            $("#EskiPolice_SigortaSirketiKodu").val("");
            $("#EskiPolice_AcenteNo").val("");
            $("#EskiPolice_PoliceNo").val("");
            $("#EskiPolice_YenilemeNo").val("");
            $("#EskiPolice_SigortaSirketKodu").val("");

            $("#EskiPolice_SigortaSirketiKodu").removeAttr('disabled');
            $("#EskiPolice_AcenteNo").removeAttr('disabled');
            $("#EskiPolice_PoliceNo").removeAttr('disabled');
            $("#EskiPolice_YenilemeNo").removeAttr('disabled');
            $("#EskiPolice_SigortaSirketKodu").removeAttr('disabled');
            $("#EskiPolice_SigortaSirketKodu").removeAttr('disabled');
        }
    }
    aracAlanlariGoster();
    $("#button-next").prop("disabled", false);
    $('.button-previous').css('pointer-events', 'auto');
    $("#btn-sorgula").button("reset");
}

function errorPlakaSorgu(jqXHR, textStatus, errorThrown) {
    $("#btn-sorgula").button("reset");
    $("#button-next").prop("disabled", false);
    $('.button-previous').css('pointer-events', 'auto');
    var response = jQuery.parseJSON(jqXHR.responseText);
    $("#plaka-sorgu-hata").html(response.message);
    $("#plaka-sorgu-hata").show();
    //$("#egm-sorgu-row").show();
    aracAlanlariGoster();
}

function setEgmSorgu(data) {
    $("#egm-sorgu-hata").hide();
    setPlakaSorgu(data);
    $("#btn-egm-sorgula").button("reset");
    $("#button-next").prop("disabled", false);
    $('.button-previous').css('pointer-events', 'auto');
}

function errorEgmSorgu(jqXHR, textStatus, errorThrown) {
    $("#btn-egm-sorgula").button("reset");
    $("#button-next").prop("disabled", false);
    $('.button-previous').css('pointer-events', 'auto');
    var response = jQuery.parseJSON(jqXHR.responseText);
    $("#egm-sorgu-hata").html(response.message);
    $("#egm-sorgu-hata").show();

    aracAlanlariGoster();
}

function aracAlanlariGoster() {
    $(".arac-bilgileri").slideDown("fast");
    $(".arac-bilgileri :input").removeClass("ignore");
    $("#Arac_AracBilgileri_Acik").val("1");
    var eskiPolice = $("#EskiPolice_EskiPoliceVar_control").bootstrapSwitch('status');
    eskiPoliceSetValidation(eskiPolice);
    var tasiyiciSorumluluk = $("#Tasiyici_Sorumluluk_control").bootstrapSwitch('status');
    tasiyiciSorumlulukValidation(tasiyiciSorumluluk);
    var dainiMurtein = $("#DainiMurtein_DainiMurtein_control").bootstrapSwitch('status');
    dainiMurteinSetValidation(dainiMurtein);
}

function eskiPoliceSetValidation(value) {
    if (value) {
        //enable control validation
        $("#EskiPolice_SigortaSirketiKodu").removeClass("ignore");
        $("#EskiPolice_AcenteNo").removeClass("ignore");
        $("#EskiPolice_PoliceNo").removeClass("ignore");
        $("#EskiPolice_YenilemeNo").removeClass("ignore");
    }
    else {
        //disable control validation
        $("#EskiPolice_SigortaSirketiKodu").addClass("ignore");
        $("#EskiPolice_AcenteNo").addClass("ignore");
        $("#EskiPolice_PoliceNo").addClass("ignore");
        $("#EskiPolice_YenilemeNo").addClass("ignore");
        $("#EskiPolice_SigortaSirketKodu").addClass("ignore");


    }
}

function tasiyiciSorumlulukValidation(value) {
    if (value) {
        //enable control validation
        $("#Tasiyici_SigortaSirketiKodu").removeClass("ignore");
        $("#Tasiyici_AcenteNo").removeClass("ignore");
        $("#Tasiyici_PoliceNo").removeClass("ignore");
        $("#Tasiyici_YenilemeNo").removeClass("ignore");
    }
    else {
        //disable control validation
        $("#Tasiyici_SigortaSirketiKodu").addClass("ignore");
        $("#Tasiyici_AcenteNo").addClass("ignore");
        $("#Tasiyici_PoliceNo").addClass("ignore");
        $("#Tasiyici_YenilemeNo").addClass("ignore");
    }
}

function trafikTeklifWizardCallback(current) {
    //Hazırlayan bilgileri
    if (current == 1) {
        //App.scrollTo();

    }
    //Sigorta ettiren / sigortali tab
    else if (current == 2) {

        if (!$("#Musteri_SigortaEttiren_AdiUnvan").is(":visible")) {
            $("#sigortaettiren-sorgula").trigger("click");
            return false;
        }

        if ($("#Arac_PlakaNo").val() != "") {

            var AnadoluKullanimTip = $("#Arac_AnadoluKullanimTip").val();
            var AnadoluKullanimSekli = $("#Arac_AnadoluKullanimSekli").val();

            if ((AnadoluKullanimTip == null || AnadoluKullanimTip == "") && (AnadoluKullanimSekli == null || AnadoluKullanimSekli == "")) {
                anadoluSigortaKullanimTipiSorgula();
            }

        }
        //App.scrollTo();
        return sigortaliKontrol.Kaydet();
    }
    // Araç bilgileri
    else if (current == 3) {

        var plakaNo = $("#Arac_PlakaNo").val().toUpperCase();
        var tescilNo = $("#Arac_TescilBelgeSeriNo").val();
        var asbisNo = $("#Arac_AsbisNo").val();

        $("#Arac_TescilBelgeSeriKod").addClass("ignore");
        $("#Arac_TescilBelgeSeriNo").addClass("ignore");
        $("#Arac_AsbisNo").addClass("ignore");


        if (plakaNo == "YK") {
            $("#Arac_TescilBelgeSeriKod").addClass("ignore");
            $("#Arac_TescilBelgeSeriNo").addClass("ignore");
            $("#Arac_AsbisNo").addClass("ignore");
        }
        else {
            $("#Arac_TescilBelgeSeriKod").removeClass("ignore");
            $("#Arac_TescilBelgeSeriNo").removeClass("ignore");
            $("#Arac_AsbisNo").removeClass("ignore");
            if (tescilNo.length > 0) {
                $("#Arac_AsbisNo").addClass("ignore");
            }
            if (asbisNo.length > 0) {
                $("#Arac_TescilBelgeSeriNo").addClass("ignore");
                $("#Arac_TescilBelgeSeriKod").addClass("ignore");
            }
        }
        var kullanimTarzi = $("#Arac_KullanimTarziKodu").val();
        var parts = kullanimTarzi.split('-');
        if (parts.length == 2) {
            if (parts[0] == '511' || parts[0] == '521' || parts[0] == '523' || parts[0] == '526' || parts[0] == '532') {
                $(".tasinan-yuk").show();
                var kademe = $("#Teminat_TasinanYukBedel").val();
                if (kademe != null && kademe != "" && kademe != "0") {
                    $("#tasinan").show();
                    $("#Teminat_TasinanYukKademe").removeClass("ignore");
                    $("#Teminat_TasinanYukAciklama").removeClass("ignore");
                    $("#Teminat_TasinanYukBedel").removeClass("ignore");
                }
                else {
                    $("#tasinan").hide();
                    $("#Teminat_TasinanYukKademe").addClass("ignore");
                    $("#Teminat_TasinanYukAciklama").addClass("ignore");
                    $("#Teminat_TasinanYukBedel").addClass("ignore");
                }
            }
            else {
                $(".tasinan-yuk").hide();
                $("#Teminat_TasinanYukKademe").addClass("ignore");
                $("#Teminat_TasinanYukAciklama").addClass("ignore");
                $("#Teminat_TasinanYukBedel").addClass("ignore");
            }
        }

        GetTurkNipponOzelAlanlar();
        GetSompoJapanOzelAlanlar();
        GetGroupamaOzelAlanlar();
        GetGulfOzelAlanlar();
        GetErgoOzelAlanlar();
        GetUnicoOzelAlanlar();
        GetAxaOzelAlanlar();
        GetHDIOzelAlanlar();

        var sigortaliTipi = $("#Musteri_SigortaEttiren_KimlikNo").val().length;
        if (sigortaliTipi == 11) {
            $("#axaHayatTeminati").show();
        }

        var kullanimTarzi = $("#Arac_KullanimTarziKodu").val();
        //if (kullanimTarzi == "111-10") {
        //    $("#axaIkame").show();
        //} else
        if (kullanimTarzi == "511-11" || kullanimTarzi == "511-15" || kullanimTarzi == "521-11") {
            $("#axaSorumlulukLimiti").show();
        }
        else {
            //$("#axaIkame").hide();
            $("#axaSorumlulukLimiti").hide();
        }
        if ($("#Arac_Model").val() != null && $("#Arac_Model").val() != "") {
            var aracModel = parseInt($("#Arac_Model").val());
            var d = new Date();
            if (aracModel >= d.getFullYear() - 1) {
                $("#AracBirYasSorulari").show();
            }
            else {

                $("#AracBirYasSorulari").hide();
            }
        }

        var isValid = FormWizard.validatePage('#tab3');

        var aracBilgileri = $("#Arac_AracBilgileri_Acik").val();
        if (aracBilgileri == "0") {
            $("#btn-sorgula").trigger("click");
            isValid = 0;
        }

        if (isValid) {
            var aracKullanimTarzi = $("#Arac_KullanimTarziKodu").val();

            var imm = $("#Teminat_IMMKodu").val();
            var fk = $("#Teminat_FKKodu").val();
            var ikame = $("#Teminat_IkameTuru").val();

            if (imm == null || imm == "" || imm == 0) {
                $.getJSON('/Common/KaskoIMM', { KullanimTarzi: aracKullanimTarzi },
                    function (result) {
                        $("#Teminat_IMMKodu").empty();
                        $("#Teminat_IMMKodu").dropDownFill(result);
                        $("#Teminat_IMMKodu").val("39");
                    });
            }
            if (fk == null || fk == "" || fk == 0) {
                $.getJSON('/Common/KaskoFK', { KullanimTarzi: aracKullanimTarzi },
                    function (result) {
                        $("#Teminat_FKKodu").empty();
                        $("#Teminat_FKKodu").dropDownFill(result);
                        $("#Teminat_FKKodu").val("65");
                    });
            }
            if (ikame == null || ikame == "" || ikame == 0) {
                $.getJSON('/Kasko/IkameTurleriListe', { KullanimTarziKodu: $("#Arac_KullanimTarziKodu").val(), },
                    function (result) {
                        if (result.success) {
                            $("#Teminat_IkameTuru").dropDownFill(result.list);
                            $("#Teminat_IkameTuru").val(result.def);
                            $("#Teminat_IkameTuru").removeClass("ignore");
                            $(".ikame-turu").show();
                        } else {
                            $("#Teminat_IkameTuru").empty();
                            $("#Teminat_IkameTuru").addClass("ignore");
                            $(".ikame-turu").hide();
                        }
                    })
            }

        }
        //App.scrollTo();
        return isValid == 1;
    }

    return true;
}

//function trafikTeklifWizardCallback(current) {
//    //Hazırlayan bilgileri
//    if (current == 1) {
//        //App.scrollTo();
//    }
//        //Sigorta ettiren / sigortali tab
//    else if (current == 2) {

//        if (!$("#Musteri_SigortaEttiren_AdiUnvan").is(":visible")) {
//            $("#sigortaettiren-sorgula").trigger("click");
//            return false;
//        }
//        //if ($("#Arac_PlakaNo").val() != "") {

//        var AnadoluKullanimTip = $("#Arac_AnadoluKullanimTip").val();
//        var AnadoluKullanimSekli = $("#Arac_AnadoluKullanimSekli").val();

//        if (AnadoluKullanimTip == "" && AnadoluKullanimSekli == "") {
//            anadoluSigortaKullanimTipiSorgula();
//        }

//        //}
//        //App.scrollTo();
//        return sigortaliKontrol.Kaydet();
//    }
//        // Araç bilgileri
//    else if (current == 3) {

//        $("#Arac_TescilBelgeSeriKod").addClass("ignore");
//        $("#Arac_TescilBelgeSeriNo").addClass("ignore");
//        $("#Arac_AsbisNo").addClass("ignore");

//        var tescilNo = $("#Arac_TescilBelgeSeriNo").val();
//        var asbisNo = $("#Arac_AsbisNo").val();

//        $("#Arac_TescilBelgeSeriKod").removeClass("ignore");
//        $("#Arac_TescilBelgeSeriNo").removeClass("ignore");
//        $("#Arac_AsbisNo").removeClass("ignore");
//        if (tescilNo.length > 0) {
//            $("#Arac_AsbisNo").addClass("ignore");
//        }
//        if (asbisNo.length > 0) {
//            $("#Arac_TescilBelgeSeriNo").addClass("ignore");
//            $("#Arac_TescilBelgeSeriKod").addClass("ignore");
//        }
//        GetTurkNipponOzelAlanlar();
//        GetSompoJapanOzelAlanlar();
//        GetGroupamaOzelAlanlar();
//        GetErgoOzelAlanlar();

//        var isValid = FormWizard.validatePage('#tab3');

//        var aracBilgileri = $("#Arac_AracBilgileri_Acik").val();
//        if (aracBilgileri == "0") {
//            $("#btn-sorgula").trigger("click");
//            isValid = 0;
//        }

//        if (isValid) {
//            var aracKullanimTarzi = $("#Arac_KullanimTarziKodu").val();

//            var imm = $("#Teminat_IMMKodu").val();
//            var fk = $("#Teminat_FKKodu").val();
//            var ikame = $("#Teminat_IkameTuru").val();

//            if (imm == null || imm == "" || imm == 0) {
//                $.getJSON('/Common/KaskoIMM', { KullanimTarzi: aracKullanimTarzi },
//               function (result) {
//                   $("#Teminat_IMMKodu").empty();
//                   $("#Teminat_IMMKodu").dropDownFill(result);
//                   $("#Teminat_IMMKodu").val("38");
//               });
//            }
//            if (fk == null || fk == "" || fk == 0) {
//                $.getJSON('/Common/KaskoFK', { KullanimTarzi: aracKullanimTarzi },
//                function (result) {
//                    $("#Teminat_FKKodu").empty();
//                    $("#Teminat_FKKodu").dropDownFill(result);
//                    $("#Teminat_FKKodu").val("65");
//                });
//            }
//            if (ikame == null || ikame == "" || ikame == 0) {
//                $.getJSON('/Kasko/IkameTurleriListe', { KullanimTarziKodu: $("#Arac_KullanimTarziKodu").val(), },
//               function (result) {
//                   if (result.success) {
//                       $("#Teminat_IkameTuru").dropDownFill(result.list);
//                       $("#Teminat_IkameTuru").val(result.def);
//                       $("#Teminat_IkameTuru").removeClass("ignore");
//                       $(".ikame-turu").show();
//                   } else {
//                       $("#Teminat_IkameTuru").empty();
//                       $("#Teminat_IkameTuru").addClass("ignore");
//                       $(".ikame-turu").hide();
//                   }
//               })
//            }
//        }
//        //App.scrollTo();
//        return isValid == 1;
//    }

//    return true;
//}


var trafikOdeme = new function () {
    return {

        init: function () {
            $("#kk-odeme-btn").click(trafikOdeme.kredikarti);
            $(".teklif-satin-al").live("click", trafikOdeme.kredikartiOdeme);
            $("#kk-odeme").on("shown", function () { $("#KrediKarti_KartSahibi").focus(); })
        },

        kredikartiOdeme: function () {
            var teklifId = $(this).attr("teklif-id");
            var fiyat = $(this).attr("fiyat");

            if (teklifId && fiyat) {
                var nakit = $("#KK_OdemeTipi_1").is(':checked');
                var havale = $("#KK_OdemeTipi_3").is(':checked');
                var blokeliKart = $("#KK_OdemeTipi_6").is(':checked');
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
                else if (blokeliKart) {
                    $("#KK_OdemeTipi_6").attr('checked', true);
                    $("#kredi-kart-bilgi").show();
                    $("#kredi-kart-bilgi").find("input").addClass("ignore");
                }
                else {
                    $("#KK_OdemeTipi_2").attr('checked', true);
                    $("#kredi-kart-bilgi").show();
                    $("#kredi-kart-bilgi").find("input").removeClass("ignore");
                }

                var vadeli = $("#KK_OdemeSekli_1").is(':checked');
                if (!vadeli) {
                    $("#KK_OdemeSekli_2").attr('checked', true);
                    $("#taksit-sayisi").show();
                    //var taksitliOdeme = $("#Odeme_TaksitSayisi").val();
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
                $("#KK_OdemeTipi_6").attr("readonly", true);
                $("#KK_OdemeSekli_1").attr("readonly", true);
                $("#KK_OdemeSekli_2").attr("readonly", true);

                $("#taksit-sayisi").attr("readonly", true);
                $("#KrediKarti_TaksitSayisi").attr("readonly", true);

                $("#KK_OdemeTipi_1").attr("disabled", true);
                $("#KK_OdemeTipi_2").attr("disabled", true);
                $("#KK_OdemeTipi_3").attr("disabled", true);
                $("#KK_OdemeTipi_6").attr("disabled", true);
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
                $("#KK_OdemeTipi_6").attr("readonly", false);
                $("#KK_OdemeSekli_1").attr("readonly", false);
                $("#KK_OdemeSekli_2").attr("readonly", false);
                $("#taksit-sayisi").attr("readonly", false);
                $("#KrediKarti_TaksitSayisi").attr("readonly", false);

                $("#KK_OdemeTipi_1").attr("disabled", false);
                $("#KK_OdemeTipi_2").attr("disabled", false);
                $("#KK_OdemeTipi_3").attr("disabled", false);
                $("#KK_OdemeTipi_6").attr("disabled", false);
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
                    url: "/Teklif/Kasko/OdemeAl",
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
                        $("#KK_OdemeTipi_6").attr("readonly", true);
                        $("#KK_OdemeSekli_1").attr("readonly", true);
                        $("#KK_OdemeSekli_2").attr("readonly", true);

                        $("#taksit-sayisi").attr("readonly", true);
                        //$("#KrediKarti_TaksitSayisi").attr("readonly", true);

                        $("#KK_OdemeTipi_1").attr("disabled", true);
                        $("#KK_OdemeTipi_2").attr("disabled", true);
                        $("#KK_OdemeTipi_3").attr("disabled", true);
                        $("#KK_OdemeTipi_6").attr("disabled", true);
                        $("#KK_OdemeSekli_1").attr("disabled", true);
                        $("#KK_OdemeSekli_2").attr("disabled", true);
                        $("#taksit-sayisi").attr("disabled", true);
                        // $("#KrediKarti_TaksitSayisi").attr("disabled", true);

                        if (data.SompoJapanTaksitliMi) {
                            $("#KrediKarti_TaksitSayisi").removeAttr("readonly");
                            $("#KrediKarti_TaksitSayisi").removeAttr('disabled');
                        }
                        else {
                            $("#KrediKarti_TaksitSayisi").attr("readonly", true);
                            $("#KrediKarti_TaksitSayisi").attr("disabled", true);
                        }
                    },
                    error: function () {
                        $("#kk-odeme-btn").button("reset");
                        $("#kk-odeme-cancel").show();
                        $("#KK_OdemeTipi_1").attr("readonly", true);
                        $("#KK_OdemeTipi_2").attr("readonly", true);
                        $("#KK_OdemeTipi_3").attr("readonly", true);
                        $("#KK_OdemeTipi_6").attr("readonly", true);
                        $("#KK_OdemeSekli_1").attr("readonly", true);
                        $("#KK_OdemeSekli_2").attr("readonly", true);

                        $("#taksit-sayisi").attr("readonly", true);
                        $("#KrediKarti_TaksitSayisi").attr("readonly", true);

                        $("#KK_OdemeTipi_1").attr("disabled", true);
                        $("#KK_OdemeTipi_2").attr("disabled", true);
                        $("#KK_OdemeTipi_3").attr("disabled", true);
                        $("#KK_OdemeTipi_6").attr("disabled", true);
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

function anadoluSigortaKullanimTipiSorgula() {
    var divSayisi = 0;
    $("html").find(".tum-no").each(function (i) {
        divSayisi = i + 1;
    });
    for (var i = 0; i < divSayisi; i++) {
        //Anadolu sigorta şirketinin durumu okunuyor
        var tumValue = $("#TeklifUM_" + i + "__TUMKodu").val();

        if (tumValue == 4) {
            var Anadolu = $("#TeklifUM_" + i + "__TeklifAl_control").bootstrapSwitch('status');
            if (Anadolu) {
                $("#anadolu-ozel").show();
                $("#anadolu-kasko-ktip-hata-satir").css({ "display": "none" });
                $("#anadolu-kasko-ksekli-hata-satir").css({ "display": "none" });
                $("#anadolu-kasko-ksekli-hata").html("");
                $("#anadolu-kasko-ikame-hata-satir").css({ "display": "none" });
                $("#anadolu-ikame-hata").html("");
                $("#kullanimSekli_Anadolu").hide();

                if ($("#Arac_MarkaKodu").val() != 600) {

                    $("#kullanimTip_Anadolu").hide();
                    $("#kullanimSekli_Anadolu").hide();
                    $("#anadolu-kullanimtipi-progress").show();

                    var AracKodu = $("#Arac_MarkaKodu").val();
                    var kullanimTarzi = $("#Arac_KullanimTarziKodu").val();
                    var modelYili = $("#Arac_Model").val();

                    if ((AracKodu != null && AracKodu != '') && (kullanimTarzi != null && kullanimTarzi != '') && (modelYili != null && modelYili != '')) {

                        //$.getJSON('/Teklif/Kasko/KullanimTipListAnadolu', { AracKullanimTarzi: kullanimTarzi, AracModelYili: modelYili, AracMarkaKodu: AracKodu },
                        $.getJSON('/Teklif/Kasko/KullanimTipListAnadolu', { AracKullanimTarzi: kullanimTarzi, AracMarkaKodu: AracKodu },
                            function (result) {
                                if (result.hata != "" && result.hata != null) {
                                    $("#anadolu-kasko-ktip-hata-satir").css('display', 'block');
                                    $("#anadolu-kasko-ktip-hata").html(result.hata);
                                }
                                else {

                                    $("#anadolu-kasko-ktip-hata-satir").css({ "display": "none" });
                                    $("#anadolu-kasko-ksekli-hata-satir").css({ "display": "none" });
                                    $("#anadolu-kasko-ktip-hata").html("");
                                    $("#anadolu-kasko-ksekli-hata").html("");
                                }
                                if (result.list.length > 1) {
                                    $('#anadolu-kasko-ktip-hata-satir').css('display', 'none');
                                    $('#anadolu-kasko-ksekil-hata-satir').css('display', 'none');
                                }
                                $("#Arac_AnadoluKullanimTip").dropDownFill(result.list);
                                $("#kullanimTip_Anadolu").show();
                                $("#anadolu-kullanimtipi-progress").hide();
                                $("#Arac_AnadoluMarkaKodu").val(result.anadoluMarkaKodu);
                            });

                    }
                    else {
                        $("#kullanimTip_Anadolu").hide();
                        $("#kullanimSekli_Anadolu").hide();
                    }
                }
                else {
                    $("#kullanimTip_Anadolu").hide();
                    $("#kullanimSekli_Anadolu").hide();
                    $("#anadolu-kasko-ktip-hata-satir").css('display', 'block');
                    $("#anadolu-kasko-ktip-hata").html("Hata! Anadolu Sigorta'nın özel alanları yüklenemedi. Anadolu Sigorta, Markası Motosiklet olan araçlar için Online Teklif düzenletmemektedir.");
                }
            }
        }
    }
    return;
};
function GetTurkNipponOzelAlanlar() {
    var divSayisi = 0;
    $("html").find(".tum-no").each(function (i) {
        divSayisi = i + 1;
    });
    for (var i = 0; i < divSayisi; i++) {
        //Türk Nippon sigorta şirketinin durumu okunuyor
        var tumValue = $("#TeklifUM_" + i + "__TUMKodu").val();
        if (tumValue == 11) {
            var TurkNippon = $("#TeklifUM_" + i + "__TeklifAl_control").bootstrapSwitch('status');
            if (TurkNippon) {
                $("#TurkNipponOzel").show();
            }
            else {
                $("#TurkNipponOzel").hide();
            }
        }
    }
}

function GetSompoJapanOzelAlanlar() {
    var divSayisi = 0;
    $("html").find(".tum-no").each(function (i) {
        divSayisi = i + 1;
    });
    for (var i = 0; i < divSayisi; i++) {
        //Sompo Japan sigorta şirketinin durumu okunuyor
        var tumValue = $("#TeklifUM_" + i + "__TUMKodu").val();
        if (tumValue == 10) {
            var SompoJapan = $("#TeklifUM_" + i + "__TeklifAl_control").bootstrapSwitch('status');
            if (SompoJapan) {
                $(".SompoJapanOzel").show();
            }
            else {
                $(".SompoJapanOzel").hide();
            }
        }
    }
}

function GetGroupamaOzelAlanlar() {
    var divSayisi = 0;
    $("html").find(".tum-no").each(function (i) {
        divSayisi = i + 1;
    });
    for (var i = 0; i < divSayisi; i++) {
        //Groupama sigorta şirketinin durumu okunuyor
        var tumValue = $("#TeklifUM_" + i + "__TUMKodu").val();
        if (tumValue == 26) {
            var SompoJapan = $("#TeklifUM_" + i + "__TeklifAl_control").bootstrapSwitch('status');
            if (SompoJapan) {
                $("#GroupamaOzel").show();
            }
            else {
                $("#GroupamaOzel").hide();
            }
        }
    }
}

function GetGulfOzelAlanlar() {
    var divSayisi = 0;
    $("html").find(".tum-no").each(function (i) {
        divSayisi = i + 1;
    });
    for (var i = 0; i < divSayisi; i++) {
        //Gulf sigorta şirketinin durumu okunuyor
        var tumValue = $("#TeklifUM_" + i + "__TUMKodu").val();
        if (tumValue == 6) {
            var Gulf = $("#TeklifUM_" + i + "__TeklifAl_control").bootstrapSwitch('status');
            if (Gulf) {
                $(".GulfOzel").show();
            }
            else {
                $(".GulfOzel").hide();
            }
        }
    }
}

function GetErgoOzelAlanlar() {
    var divSayisi = 0;
    $("html").find(".tum-no").each(function (i) {
        divSayisi = i + 1;
    });
    for (var i = 0; i < divSayisi; i++) {
        //Ergo sigorta şirketinin durumu okunuyor
        var tumValue = $("#TeklifUM_" + i + "__TUMKodu").val();
        if (tumValue == 21) {
            var Ergo = $("#TeklifUM_" + i + "__TeklifAl_control").bootstrapSwitch('status');
            if (Ergo) {
                $("#ErgoOzel").show();
            }
            else {
                $("#ErgoOzel").hide();
            }
        }
    }
}

function GetUnicoOzelAlanlar() {
    var divSayisi = 0;
    $("html").find(".tum-no").each(function (i) {
        divSayisi = i + 1;
    });
    for (var i = 0; i < divSayisi; i++) {
        //Unico sigorta şirketinin durumu okunuyor
        var tumValue = $("#TeklifUM_" + i + "__TUMKodu").val();
        if (tumValue == 17) {
            var Unico = $("#TeklifUM_" + i + "__TeklifAl_control").bootstrapSwitch('status');
            if (Unico) {
                $("#UnicoOzel").show();
            }
            else {
                $("#UnicoOzel").hide();
            }
        }
    }
}

function GetAxaOzelAlanlar() {
    var divSayisi = 0;
    $("html").find(".tum-no").each(function (i) {
        divSayisi = i + 1;
    });
    for (var i = 0; i < divSayisi; i++) {
        //Unico sigorta şirketinin durumu okunuyor
        var tumValue = $("#TeklifUM_" + i + "__TUMKodu").val();
        if (tumValue == 2) {
            var Axa = $("#TeklifUM_" + i + "__TeklifAl_control").bootstrapSwitch('status');
            if (Axa) {
                $("#AxaOzel").show();
            }
            else {
                $("#AxaOzel").hide();
            }
        }
    }
}

function GetHDIOzelAlanlar() {
    var divSayisi = 0;
    $("html").find(".tum-no").each(function (i) {
        divSayisi = i + 1;
    });
    for (var i = 0; i < divSayisi; i++) {
        //Axa sigorta şirketinin durumu okunuyor
        var tumValue = $("#TeklifUM_" + i + "__TUMKodu").val();
        if (tumValue == 1) {
            var HDI = $("#TeklifUM_" + i + "__TeklifAl_control").bootstrapSwitch('status');
            if (HDI) {
                $("#HDIOzel").show();
            }
            else {
                $("#HDIOzel").hide();
            }
        }
    }
}
function padLeft(str, max) {
    str = str.toString();
    return str.length < max ? pad("0" + str, max) : str;
}

