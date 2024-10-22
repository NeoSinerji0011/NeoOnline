function mapfremarka(options) {
    var kullanimTarziKodu;

    // ==== Para Formatı Belirleniyor.
    $("#Arac_AracDeger").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0, aSep: '.', aDec: ',' });
    $("#Teminat_OlumSakatlikTeminat").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0, aSep: '.', aDec: ',' });
    $("#Teminat_TedaviTeminat").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0, aSep: '.', aDec: ',' });
    $(".auto-bedel").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0, aSep: '', aDec: ',' });

    //Marka değiştiğinde
    $(options.marka).change(function () {
        var markaKodu = $(options.marka).val();
        var model = $(options.model).val();

        if (markaKodu === undefined) return;

        $.getJSON('/MapfreKasko/AracTipiGetir', { MarkaKodu: markaKodu },
            function (result) {                
                $(options.tip).dropDownFill(result);
                var tipKodu = $("#type-code").val();
                if (tipKodu != '') {
                    $(options.tip).val(tipKodu);
                    GetAracDegerKisiSayisi();
                }
            });
    });
    $("#Arac_Model").change(GetAracDegerKisiSayisi);
}

var aracKisiSayisiEgm = false;
var ikameTuruSorgulandi = false;
$(document).ready(function () {
    $("#Musteri_SigortaEttiren_CepTelefonu").removeAttr("data-val").removeAttr("data-val-required");
    $("#Musteri_Sigortali_CepTelefonu").removeAttr("data-val").removeAttr("data-val-required");
    $("#Musteri_SigortaEttiren_SoyadiUnvan").closest(".sigortaettiren-satir").removeClass("tuzel").hide();;
    $("#Musteri_SigortaEttiren_SoyadiUnvan").addClass("ignore").removeAttr("data-val").removeAttr("data-val-required");
    $("#Musteri_SigortaEttiren_VergiDairesi").closest(".sigortaettiren-satir").removeClass("tuzel").hide();
    $("#Musteri_SigortaEttiren_VergiDairesi").addClass("ignore").removeAttr("data-val").removeAttr("data-val-required");
    $("#Musteri_Sigortali_SoyadiUnvan").closest(".sigortali-satir").removeClass("tuzel").hide();
    $("#Musteri_Sigortali_SoyadiUnvan").addClass("ignore").removeAttr("data-val").removeAttr("data-val-required");
    $("#Musteri_Sigortali_VergiDairesi").closest(".sigortali-satir").removeClass("tuzel").hide();
    $("#Musteri_Sigortali_VergiDairesi").addClass("ignore").removeAttr("data-val").removeAttr("data-val-required");

    $("#Arac_KisiSayisi").change(function () {
        aracKisiSayisiEgm = false;
    });

    $("#btn-eski-police-sorgula").click(function () {
        var kimlikNo = $("#Musteri_Sigortali_KimlikNo").val();
        if (kimlikNo == "" || kimlikNo == null) {
            kimlikNo = $("#Musteri_SigortaEttiren_KimlikNo").val();
        }
        var sigortaSirketi = $("#Arac_SorguSigortaSirketiKodu").val();
        var acenteNo = $("#SorguAcenteNo").val();
        var policeNo = $("#SorguPoliceNo").val();
        var yenilemeNo = $("#SorguYenilemeNo").val();

        var isValid = 1;
        if (!sigortaSirketi || sigortaSirketi == '') isValid = 0;
        if (!acenteNo || acenteNo == '') isValid = 0;
        if (!policeNo || policeNo == '') isValid = 0;
        if (!yenilemeNo || yenilemeNo == '') isValid = 0;

        if (isValid == 1) {
            $(this).button("loading");
            $("#eski-police-modal-hata").slideUp("fast");
            $.ajax({
                dataType: "json",
                url: "/Teklif/MapfreKasko/EskiPoliceSorgula",
                data: { kimlikNo: kimlikNo, sigortaSirketi: sigortaSirketi, acenteNo: acenteNo, policeNo: policeNo, yenilemeNo: yenilemeNo },
                success: function (data) {
                    $("#btn-eski-police-sorgula").button("reset");
                    $("#eski-police-modal").modal("hide");
                    setPlakaSorgu(data);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    $("#btn-eski-police-sorgula").button("reset");
                    var response = jQuery.parseJSON(jqXHR.responseText);
                    $("#eski-police-hata-text").html(response.message);
                    $("#eski-police-modal-hata").slideDown("fast");
                }
            });
        } else {
            $("#eski-police-hata-text").html("Lütfen eski poliçe bilgilerini giriniz.");
            $("#eski-police-modal-hata").slideDown("fast");
        }
    });

    $("#btn-hasarsizlik-sorgula").click(function () {
        $("#hasarsizlik-sorgu-hata").hide();
        var kimlikNo = $("#Musteri_Sigortali_KimlikNo").val();
        if (kimlikNo == "" || kimlikNo == null) {
            kimlikNo = $("#Musteri_SigortaEttiren_KimlikNo").val();
        }
        var sigortaSirketi = $("#EskiPolice_SigortaSirketiKodu").val();
        var acenteNo = $("#EskiPolice_AcenteNo").val();
        var policeNo = $("#EskiPolice_PoliceNo").val();
        var yenilemeNo = $("#EskiPolice_YenilemeNo").val();

        var isValid = 1;
        if (!sigortaSirketi || sigortaSirketi == '') isValid = 0;
        if (!acenteNo || acenteNo == '') isValid = 0;
        if (!policeNo || policeNo == '') isValid = 0;
        if (!yenilemeNo || yenilemeNo == '') isValid = 0;
        
        if (isValid == 1) {
            $(this).button("loading");
            $.ajax({
                dataType: "json",
                url: "/Teklif/MapfreKasko/HasarsizlikSorgula",
                data: { kimlikNo: kimlikNo, sigortaSirketi: sigortaSirketi, acenteNo: acenteNo, policeNo: policeNo, yenilemeNo: yenilemeNo, bransKodu: "420" },
                success: function (data) {
                    $("#btn-hasarsizlik-sorgula").button("reset");

                    if (data.success) {
                        HasarsizlikBilgiSet(data);
                    } else {
                        HasarsizlikBilgiClear();

                        $("#hasarsizlik-sorgu-hata").html(data.message);
                        $("#hasarsizlik-sorgu-hata").slideDown("fast");
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    $("#btn-hasarsizlik-sorgula").button("reset");
                    HasarsizlikBilgiClear();
                    var response = jQuery.parseJSON(jqXHR.responseText);
                    $("#hasarsizlik-sorgu-hata").html(response.message);
                    $("#hasarsizlik-sorgu-hata").slideDown("fast");
                }
            });
        } else {
            $("#hasarsizlik-sorgu-hata").html("Lütfen eski poliçe bilgilerini giriniz.");
            $("#hasarsizlik-sorgu-hata").show();
        }
    });

    mapfremarka({
        tarz: "#Arac_KullanimTarziKodu",
        marka: "#Arac_MarkaKodu",
        model: "#Arac_Model",
        tip: "#Arac_TipKodu"
    });

    $("#OdemeTipi_1[name='Odeme.OdemeTipi']").attr("checked", "checked");

    // ==== Odeme tipi, şekli ve taksit sayısı kontrol ediliyor. ==== //
    $("#Odeme_OdemeSekli_control").on("switch-change", OdemeSekliDegisti);

    // ==== Tarih ayarları ==== //
    $('#Arac_TrafikTescilTarihi').change(function () {
        var d1 = $(this).datepicker("getDate");
        $("#Arac_TrafigeCikisTarihi").datepicker("setDate", d1);
        setTimeout(function () { $("#Arac_TrafigeCikisTarihi").datepicker("show"); }, 100);
    });

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
    $("#Arac_AracDeger").focusout(function () {
        AracDegerKontrol();
    });

    // ==== Tedavi teminat kontrol ediliyor ==== //
    $("#Teminat_Tedavi_control").on("switch-change", TeminatTedaviKontrol);

    // ==== Yurtdışı teminat kontrol ediliyor ==== //
    $("#Teminat_Yutr_Disi_Teminat_control").on("switch-change", YurtdisiTeminatKontrol);

    // ==== Araç kullanıcı bilgisi kontrol ediliyor ==== //
    $("#Teminat_Kullanici_Teminat_control").on("switch-change", KullaniciTeminatKontrol);

    // ==== Aksesuar teminatı kontrol ediliyor ==== //
    $("#Teminat_Aksesuar_Teminati_control").on("switch-change", TeminatAksesuarKontrol);
    $(".add-aksesuar").live("click", addAksesuarSatir);
    $(".remove-aksesuar").live("click", removeAksesuarSatir);

    // ==== Elektronik cihaz teminatı kontrol ediliyor ==== //
    $("#Teminat_ElektronikCihaz_Teminati_control").on("switch-change", TeminatElekCihazKontrol);
    $(".add-elekcihaz").live("click", addElekCihazSatir);
    $(".remove-elekcihaz").live("click", removeElekCihazSatir);

    // ==== Taşınan yük teminatı kontrol ediliyor ==== //
    $("#Teminat_TasinanYuk_Teminati_control").on("switch-change", TeminatTasinanYukKontrol);

    $("#EskiPolice_EskiPoliceVar_control").on("switch-change", function (e, data) {
        if (data.value) {
            $("#eski-police").slideDown("fast");
            $("#div-hasarsizlik-sorgula").show();
        }
        else {
            $("#eski-police").slideUp("fast");
            $("#div-hasarsizlik-sorgula").hide();
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
        $.getJSON('/MapfreKasko/KaskoDMSorgula', { kurumTipi: $(this).val() },
            function (result) {
                $("#DainiMurtein_KurumKodu").dropDownFill(result);
                $("#DainiMurtein_KurumKodu").prepend("<option value='' selected='selected'>[SEÇİNİZ]</option>");
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

    $("#Arac_PlakaNo").blur(function () {
        $(this).val($(this).val().toUpperCase());
    });

    $("#Arac_TescilBelgeSeriKod").blur(function () {
        $(this).val($(this).val().toUpperCase());
    });

    $("#Arac_PlakaKodu").change(function () {
        var tescilIlKodu = '';
        if ($(this).val() != '') {
            tescilIlKodu = parseInt($(this).val());
        }
        $("#Arac_TescilIl").val(tescilIlKodu);
        $("#Arac_TescilIl").trigger("change");
    });

    $("#Arac_TescilIl").tescilil({ ilce: "#Arac_TescilIlce" });

    $("#brand-code").focusout(function () {
        var marka = $("#brand-code").val();
        if (marka && marka != '') {
            $("#Arac_MarkaKodu").val(marka);
            $.getJSON('/MapfreKasko/AracTipiGetir', { MarkaKodu: marka },
                function (result) {
                    $("#Arac_TipKodu").dropDownFill(result);
                    var tipKodu = $("#type-code").val();
                    if (tipKodu != '') {
                        $("#Arac_TipKodu").val(tipKodu);
                        GetAracDegerKisiSayisi();
                    }
                });
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

    $("#btn-sorgula").click(function () {
        var isValid = $("#Arac_PlakaKodu").valid() && $("#Arac_PlakaNo").valid();

        if ($("#Arac_PlakaKodu").val() == "0") {
            $("#Arac_PlakaKodu").val("");
            isValid = $("#Arac_PlakaKodu").valid();
        }

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
                return;
            }

            var musteriKodu;
            var sigortaliAyni = $("#Musteri_SigortaliAyni_control").bootstrapSwitch('status');
            if (sigortaliAyni) {
                musteriKodu = $("#Musteri_SigortaEttiren_MusteriKodu").val();
            } else {
                musteriKodu = $("#Musteri_Sigortali_MusteriKodu").val();
            }

            $.ajax({
                dataType: "json",
                url: "/Teklif/MapfreKasko/PlakaSorgula",
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
                url: "/Teklif/MapfreKasko/EgmSorgu",
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
        var isvalid = $("#form4").valid();
        if (isvalid) {
            
            if ($("#Teminat_Kullanici_Adi").is(":visible")) {
                if ($("#Teminat_Kullanici_Adi").valid() != 1) {
                    return;
                }
            }

            $(this).button("loading");

            $(".switcher").find(":input").switchFix();
            
            var aracDeger = $("#Arac_AracDeger").autoNumeric('get');
            var olumTeminat = $("#Teminat_OlumSakatlikTeminat").autoNumeric('get');
            var tedavi = $("#Teminat_TedaviTeminat").autoNumeric('get');
            $("#Arac_AracDeger").val(aracDeger);
            $("#Teminat_OlumSakatlikTeminat").val(olumTeminat);
            $("#Teminat_TedaviTeminat").val(tedavi);

            var disabled = $("#form2,#form3,#form4").find(':input:disabled').removeAttr('disabled');
            var contents = $("#form1, #form2, #form3, #form4").serialize();
            disabled.attr('disabled', 'disabled');
            $("#Arac_AracDeger").autoNumeric('set', aracDeger);
            $("#Teminat_OlumSakatlikTeminat").autoNumeric('set', olumTeminat);
            $("#Teminat_TedaviTeminat").autoNumeric('set', tedavi);

            $("#step3group").css({ "visibility": "visible" });
            $("#teklif-fiyat-container").css({ "visibility": "visible" });
            $('#form_wizard_1').bootstrapWizard("next");

            $.ajax(
                {
                    type: "POST",
                    url: "/Teklif/MapfreKasko/Hesapla",
                    data: contents,
                    success: function (data) {
                        if (data.id > 0) {
                            $("#teklif-fiyat-progress").hide();
                            $("#teklif-container").html(data.html);
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
                        $('#form_wizard_1').bootstrapWizard("previous");
                        $("#teklif-fiyat-container").css({ "visibility": "hidden" });
                        $("#btn-hesapla").button("reset");
                    }
                });
        }
    });

    $("#btn-kullanici-sorgula").click(function () {
        var isValid = $("#Teminat_Kullanici_TCKN").valid();
        if (!isValid) {
            $("#kullanici-sorgu-hata").html("Lütfen TC Kimlik No giriniz.");
            $("#kullanici-sorgu-hata").show();
            return;
        }
        var kimlikNo = $("#Teminat_Kullanici_TCKN").val();
        if (kimlikNo.length != 11) {
            $("#kullanici-sorgu-hata").html("TC Kimlik No 11 hane olmalıdır.");
            $("#kullanici-sorgu-hata").show();
            return;
        }

        $("#btn-kullanici-sorgula").button("loading");
        $("#Teminat_Kullanici_Adi").val("");
        $.ajax({
            type: "post",
            dataType: "json",
            url: "/Teklif/Teklif/KimlikNoSorgula",
            data: { kimlikNo: kimlikNo },
            success: function (res) {
                if (res.SorgulamaSonuc) {
                    var adSoyad = res.AdiUnvan + ' ' + res.SoyadiUnvan;
                    $("#Teminat_Kullanici_Adi").val(adSoyad);
                    $("#kullanici-sorgu-hata").hide();
                }
                else {
                    $("#kullanici-sorgu-hata").html(res.HataMesaj);
                    $("#kullanici-sorgu-hata").slideDown();
                }
                $("#btn-kullanici-sorgula").button("reset");
            },
            error: function () {
                $("#kullanici-sorgu-hata").html("Kullanıcı adı sorgulanırken hata oluştu.");
                $("#btn-kullanici-sorgula").button("reset");
            }
        });
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
                        url: "/Teklif/MapfreKasko/DainiKurum",
                        data: { adSoyad : adSoyad },
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

    $("#btn-otorizasyon-mesaj").live("click", function () {
        var teklifId = $("#TeklifId").val();
        var mesaj = $("#teklif_OtorizasyonMesaj").val();

        if (!mesaj || mesaj.length == 0) {
            alert("Lütfen mesajınızı giriniz.");
            return;
        }

        $("#btn-otorizasyon-mesaj").button("loading");
        $.ajax({
            type: "post",
            dataType: "json",
            url: "/Teklif/MapfreKasko/OtorizasyonMesajGonder",
            data: { teklifId: teklifId, mesaj : mesaj },
            success: function (res) {

                $("#btn-otorizasyon-mesaj").button("reset");

                if (res.success) {
                    alert("Mesajınız başarıyla gönderildi.");
                }
                else {
                    alert("Mesajınız gönderilirken hata oluştu:" + res.message);
                }
            },
            error: function () {

                $("#btn-otorizasyon-mesaj").button("reset");

                alert("Mesajınız gönderilirken hata oluştu.");
            }
        });

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

    $("#Teminat_IkameTuru").change(function () {
        var ikame = $(this).val();
        var onarim = $("#Teminat_OnarimYeri").val();

        if (ikame == 'Z-KASKOJET') {
            $("#Teminat_OnarimYeri").val("G");
            $("#Teminat_IkameTuru").attr("disabled", "disabled");
        } else if (onarim == "G") {
            $("#Teminat_OnarimYeri").val("T");
        }
    });
    $("#Teminat_OnarimYeri").change(function () {
        var onarim = $(this).val();

        if (onarim == "G") {
            if ($("#Teminat_IkameTuru option[value='Z-KASKOJET']").length > 0) {
                $("#Teminat_IkameTuru").val("Z-KASKOJET");
                $("#Teminat_IkameTuru").attr("disabled", "disabled");
            } else {
                $("#Teminat_IkameTuru").val("0");
                $("#Teminat_OnarimYeri").val("T");
            }
        } else {
            $("#Teminat_IkameTuru").removeAttr("disabled");
            if ($("#Teminat_IkameTuru option[value='ABC07']").length > 0)
                $("#Teminat_IkameTuru").val("ABC07");
            else
                $("#Teminat_IkameTuru").val("0");
        }
    });

});

function HasarsizlikBilgiSet(data) {
    var show = false;
    if (data.HasarsizlikInd && data.HasarsizlikInd != "" && data.HasarsizlikInd != "0") {
        $("#Arac_HasarsizlikIndirim").val(data.HasarsizlikInd);
        $("#eski-police-indirim").find("span").html(data.HasarsizlikInd);
        $("#eski-police-indirim").show();
        show = true;
    } else {
        $("#Arac_HasarsizlikIndirim").val("");
    }
    if (data.HasarsizlikSur && data.HasarsizlikSur != "" && data.HasarsizlikSur != "0") {
        $("#Arac_HasarSurprim").val(data.HasarsizlikSur);
        $("#eski-police-surprim").find("span").html(data.HasarsizlikSur);
        $("#eski-police-surprim").show();
        show = true;
    } else {
        $("#Arac_HasarSurprim").val("");
    }
    if (data.HasarsizlikKademe && data.HasarsizlikKademe != "" && data.HasarsizlikKademe != "0") {
        $("#Arac_UygulanacakKademe").val(data.HasarsizlikKademe);
        $("#eski-police-kademe").find("span").html(data.HasarsizlikKademe);
        $("#eski-police-kademe").show();
        show = true;
    } else {
        $("#Arac_UygulanacakKademe").val("");
    }
    if(show) $("#eski-police-hasarsizlik").show();
}

function HasarsizlikBilgiClear() {
    $("#eski-police-hasarsizlik").hide();
    $("#eski-police-indirim").find("span").html("");
    $("#eski-police-indirim").hide();
    $("#eski-police-surprim").find("span").html("");
    $("#eski-police-surprim").hide();
    $("#eski-police-kademe").find("span").html("");
    $("#eski-police-kademe").hide();
    $("#Arac_HasarsizlikIndirim").val("");
    $("#Arac_HasarSurprim").val("");
    $("#Arac_UygulanacakKademe").val("");
}
function ikameTuruSorgula() {
    $("#button-next").button("loading");
    //Kullanım tarzı değiştiğinde değilde ileri butonuna basıldığında ikame türleri sorgulanıyor.
    $.getJSON('/MapfreKasko/IkameTurleriListe', {
        KullanimTarziKodu: $("#Arac_KullanimTarziKodu").val(),
        plakaIlKodu: $("#Arac_PlakaKodu").val(),
        plakaNo: $("#Arac_PlakaNo").val(),
        aracRuhsatSeri: $("#Arac_TescilBelgeSeriKod").val(),
        aracRuhsatNo: $("#Arac_TescilBelgeSeriNo").val(),
        asbisNo: $("#Arac_AsbisNo").val(),
        kisiSayisi: $("#Arac_KisiSayisi").val(),
        egmSorgu: aracKisiSayisiEgm,
        markaKodu: $("#Arac_MarkaKodu").val()
    },
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
    }).always(function () {
        ikameTuruSorgulandi = true;
        var currentPage = $('#form_wizard_1').bootstrapWizard("currentIndex");
        if(currentPage == 1)
            $('#form_wizard_1').bootstrapWizard("next");

        $("#button-next").button("reset");
    });
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

function TeminatTedaviKontrol(e, data) {
    if (data.value) {
        var olumSakatlik = parseInt($("#Teminat_OlumSakatlikTeminat").autoNumeric('get'));
        var tedaviTeminat = olumSakatlik * 0.10;
        $("#Teminat_TedaviTeminat").autoNumeric('set', tedaviTeminat);
        $("#teminat-tedavi-tutar").slideDown("fast");
    }
    else {
        $("#teminat-tedavi-tutar").slideUp("fast");
    }
}

function YurtdisiTeminatKontrol(e, data) {
    if (data.value) {
        $(".yurtdisi-teminat").slideDown("fast");
        $("#Teminat_Yurt_Disi_Teminat_Ulke").removeClass("ignore");
    }
    else {
        $(".yurtdisi-teminat").slideUp("fast");
        $("#Teminat_Yurt_Disi_Teminat_Ulke").addClass("ignore");
    }
}
function KullaniciTeminatKontrol(e, data) {
    if (data.value) {
        $(".kullanici-teminat").slideDown("fast");
        $("#Teminat_Kullanici_TCKN").removeClass("ignore");
        $("#Teminat_Kullanici_Adi").removeClass("ignore");
    }
    else {
        $(".kullanici-teminat").slideUp("fast");
        $("#Teminat_Kullanici_TCKN").addClass("ignore");
        $("#Teminat_Kullanici_Adi").addClass("ignore");
    }
}
function TeminatAksesuarKontrol(e, data) {
    if (data.value) {
        $("#aksesuarlar").slideDown("fast");
        if(!satirEkleme)
            addAksesuarSatir();
        satirEkleme = false;
    }
    else {
        $("#aksesuarlar").slideUp("fast");
        removeAksesuarSatirlar();
    }
}
function addAksesuarSatir() {
    _.templateSettings.variable = "rc";
    var template = _.template(
        $("script.template-aksesuar").html()
    );
    var templateData = { no: akseusarSatir };
    var elements = $("#aksesuar-header").parent().append(template(templateData));
    $("#Teminat_Aksesuarlar_" + akseusarSatir + "__Aciklama").rules('add', 'required');
    $("#Teminat_Aksesuarlar_" + akseusarSatir + "__Bedel").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0, aSep: '', aDec: ',' });
    akseusarSatir++;
}
function removeAksesuarSatirlar() {
    akseusarSatir = 0;
    $("#aksesuar-header").siblings().remove();
}
function removeAksesuarSatir() {
    var id = $(this).data("id");
    var row = "#aksesuar-row-"+id;
    $("#aksesuar-header").siblings(row).remove();
}
function TeminatElekCihazKontrol(e, data) {
    if (data.value) {
        $("#elekcihaz").slideDown("fast");
        if (!satirEkleme)
            addElekCihazSatir();
        satirEkleme = false;
    }
    else {
        $("#elekcihaz").slideUp("fast");
        removeElekCihazSatirlar();
    }
}
function addElekCihazSatir() {
    _.templateSettings.variable = "rc";
    var template = _.template(
        $("script.template-elekcihaz").html()
    );
    var templateData = { no: elekcihazSatir };
    $("#elekcihaz-header").parent().append(template(templateData));
    $("#Teminat_Cihazlar_" + elekcihazSatir + "__Aciklama").rules('add', 'required');
    $("#Teminat_Cihazlar_" + elekcihazSatir + "__Bedel").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0, aSep: '', aDec: ',' });
    elekcihazSatir++;
}
function removeElekCihazSatirlar() {
    elekcihazSatir = 0;
    $("#elekcihaz-header").siblings().remove();
}
function removeElekCihazSatir() {
    var id = $(this).data("id");
    var row = "#elekcihaz-row-" + id;
    $("#elekcihaz-header").siblings(row).remove();
}
function TeminatTasinanYukKontrol(e, data) {
    if (data.value) {
        $("#tasinan").slideDown("fast");
        if (!satirEkleme)
            addTasinanSatir();
        satirEkleme = false;
    }
    else {
        $("#tasinan").slideUp("fast");
        removeTasinanSatirlar();
    }
}
function addTasinanSatir() {
    _.templateSettings.variable = "rc";
    var template = _.template(
        $("script.template-tasinan").html()
    );
    var templateData = { no: tasinanSatir };
    $("#tasinan-header").parent().append(template(templateData));
    $("#Teminat_TasinanYukler_" + tasinanSatir + "__Aciklama").rules('add', 'required');
    $("#Teminat_TasinanYukler_" + tasinanSatir + "__Bedel").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0, aSep: '', aDec: ',' });
    $("#Teminat_TasinanYukler_" + tasinanSatir + "__Fiyat").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0, aSep: '', aDec: ',' });
    tasinanSatir++;
}
function removeTasinanSatirlar() {
    tasinanSatir = 0;
    $("#tasinan-header").siblings().remove();
}
function removeTasinanSatir() {
    var id = $(this).data("id");
    var row = "#tasinan-row-" + id;
    $("#tasinan-header").siblings(row).remove();
}
// ==== Aracın Değerini ve kişi sayısını getirir ==== //
function GetAracDegerKisiSayisi() {
    var tipkodu = $("#Arac_TipKodu").val();
    var markakodu = $("#Arac_MarkaKodu").val();
    var model = $("#Arac_Model").val();
    var kullanimtarzi = $("#Arac_KullanimTarziKodu").val();
    var plakaKodu = $("#Arac_PlakaKodu").val();
    var plakaNo = $("#Arac_PlakaNo").val();
    var aracRuhsatSeriNo = $("#Arac_TescilBelgeSeriKod").val();
    var aracRuhsatNo = $("#Arac_TescilBelgeSeriNo").val();
    var asbisNo = $("#Arac_AsbisNo").val();

    if (tipkodu > 0 && markakodu > 0 && model > 0) {
        $.ajax({
            dataType: "json",
            url: "/Teklif/MapfreKasko/GetAracDegerKisiSayisi",
            data: {
                TipKodu: tipkodu,
                MarkaKodu: markakodu,
                Model: model,
                KullanimTarzi: kullanimtarzi,
                PlakaKodu: plakaKodu,
                PlakaNo: plakaNo,
                AracRuhsatSeriNo: aracRuhsatSeriNo,
                AracRuhsatNo: aracRuhsatNo,
                AsbisNo: asbisNo
            },
            success: function (data) {

                $("#Arac_AracDeger").autoNumeric('set', data.AracDeger);
                $("#Arac_KisiSayisi").val(data.KisiSayisi);
                if (data.ProjeKodu == "Mapfre") {
                    $("#Teminat_AMSKodu").dropDownFill(data.IMMList);
                    if (data.IMMList.length > 1) {
                        $("#Teminat_AMSKodu").prop("selectedIndex", 1);
                    }
                    if (data.EgmSorgu) {
                        aracKisiSayisiEgm = true;
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
            url: "/Teklif/MapfreKasko/GetKaskoAMSListe",
            data: { kullanimTarzi: kullanimTarzi },
            success: function (data) {
                $("#Teminat_AMSKodu").dropDownFill(data);
            },
            error: function (data) { }
        });
    }
}
var aracDegerWait = false;
// ==== Aracın değeri kontrol ediliyor ==== //
function AracDegerKontrol() {
    var tipkodu = $("#Arac_TipKodu").val();
    var markakodu = $("#Arac_MarkaKodu").val();
    var model = $("#Arac_Model").val();
    var yenifiyat = $("#Arac_AracDeger").autoNumeric('get');

    if (tipkodu > 0 && markakodu > 0 && model > 0) {
        aracDegerWait = true;
        $("#button-next").attr("disabled", "disabled");
        $.ajax({
            dataType: "json",
            url: "/Teklif/MapfreKasko/AracDegerKontrol",
            data: { TipKodu: tipkodu, MarkaKodu: markakodu, Model: model, AracDeger: yenifiyat },
            success: function (data) {
                aracDegerWait = false;
                $("#button-next").removeAttr("disabled");
                if (data.Result == false) {
                    $("#Arac_AracDeger").autoNumeric('set', data.OrjinalDeger);
                    $("#arac-deger-hata").html("Araç fiyatı, aracın kasko değerinden küçük olamaz.");
                    $("#arac-deger-hata").show();
                } else {
                    $("#arac-deger-hata").hide();
                }
            },
            error: function () {
                aracDegerWait = false;
                $("#button-next").removeAttr("disabled");
            }
        });
    }
}

function setPlakaSorgu(data) {
    $("#plaka-sorgu-hata").hide();
    if (data) {

        if (data.PlakaKodu && data.PlakaKodu.length > 0) {
            $("#Arac_PlakaKodu").val(data.PlakaKodu);
            $("#Arac_PlakaKodu").trigger("change");
        }
        if (data.PlakaNo && data.PlakaNo.length > 0)
            $("#Arac_PlakaNo").val(data.PlakaNo);

        $("#Arac_KullanimTarziKodu").dropDownFill(data.Tarzlar);
        $("#Arac_KullanimSekliKodu").val(data.AracKullanimSekli);
        $("#Arac_KullanimTarziKodu").val(data.AracKullanimTarzi);

        if (data.Markalar && data.Markalar.length > 0)
            $("#Arac_MarkaKodu").dropDownFill(data.Markalar);

        $("#Arac_MarkaKodu").val(data.AracMarkaKodu);
        $("#Arac_KisiSayisi").val(data.AracKoltukSayisi);
        $("#Arac_Model").val(data.AracModelYili);
        $("#Arac_TipKodu").dropDownFill(data.Tipler);
        $("#brand-code").val(data.AracMarkaKodu);
        $("#type-code").val(data.AracTipKodu);
        $("#Arac_TipKodu").val(data.AracTipKodu);
        $("#Arac_AracDeger").val(data.AracDegeri);
        $("#Arac_MotorNo").val(data.AracMotorNo);
        $("#Arac_SaseNo").val(data.AracSasiNo);
        $("#Teminat_AMSKodu").dropDownFill(data.Ams);

        if (data.TescilSeri && data.TescilSeri.length > 0 && data.TescilSeriNo && data.TescilSeriNo.length > 0) {
            $("#Arac_TescilBelgeSeriKod").val(data.TescilSeri);
            $("#Arac_TescilBelgeSeriNo").val(data.TescilSeriNo);
        } else if (data.TescilSeriNo && data.TescilSeriNo.length > 0) {
            $("#Arac_AsbisNo").val(data.TescilSeriNo);
        }

        if (data.Ams && data.Ams.length > 1) {
            $("#Teminat_AMSKodu").prop("selectedIndex", 1);
        }
        else {
            $("#Teminat_AMSKodu").val("");
        }
        if (data.AracTescilTarih && data.AracTescilTarih.length > 0) {
            $('#Arac_TrafikTescilTarihi').datepicker('setDate', data.AracTescilTarih);
            $('#Arac_TrafigeCikisTarihi').datepicker('setDate', data.AracTescilTarih);
        }

        if (data.EskiPoliceNo && data.EskiPoliceNo.length > 0 && data.EskiPoliceNo != "0") {
            $("#EskiPolice_EskiPoliceVar_control").bootstrapSwitch('setState', true);
            $("#EskiPolice_SigortaSirketiKodu").val(data.EskiPoliceSigortaSirkedKodu);
            $("#EskiPolice_AcenteNo").val(data.EskiPoliceAcenteKod);
            $("#EskiPolice_PoliceNo").val(data.EskiPoliceNo);
            $("#EskiPolice_YenilemeNo").val(data.EskiPoliceYenilemeNo);

            $("#EskiPolice_SigortaSirketiKodu").prop('disabled', true);
            $("#EskiPolice_AcenteNo").prop('disabled', true);
            $("#EskiPolice_PoliceNo").prop('disabled', true);
            $("#EskiPolice_YenilemeNo").prop('disabled', true);

            if (data.YeniPoliceBaslangicTarih && data.YeniPoliceBaslangicTarih.length > 0) {
                $('#Arac_PoliceBaslangicTarihi').datepicker('setDate', data.YeniPoliceBaslangicTarih);
            }

            if (data.HasarsizlikInd != "" || data.HasarsizlikSur != "" || data.HasarsizlikKademe != "") {                
                HasarsizlikBilgiSet(data);
            }
        }
        else {
            $("#EskiPolice_EskiPoliceVar_control").bootstrapSwitch('setState', false);
            $("#EskiPolice_SigortaSirketiKodu").val("");
            $("#EskiPolice_AcenteNo").val("");
            $("#EskiPolice_PoliceNo").val("");
            $("#EskiPolice_YenilemeNo").val("");

            $("#EskiPolice_SigortaSirketiKodu").removeAttr('disabled');
            $("#EskiPolice_AcenteNo").removeAttr('disabled');
            $("#EskiPolice_PoliceNo").removeAttr('disabled');
            $("#EskiPolice_YenilemeNo").removeAttr('disabled');

            HasarsizlikBilgiClear();
        }

        if (data.IkameTurleri && data.IkameTurleri.length > 0) {
            $("#Teminat_IkameTuru").dropDownFill(data.IkameTurleri);
            $("#Teminat_IkameTuru").val(data.IkameTuru);
            $("#Teminat_IkameTuru").removeClass("ignore");
            $(".ikame-turu").show();
        } else {
            $("#Teminat_IkameTuru").empty();
            $("#Teminat_IkameTuru").addClass("ignore");
            $(".ikame-turu").hide();
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

function trafikTeklifWizardCallback(current) {
    //Sigorta ettiren / sigortali tab
    if (current == 1) {

        // Merkez / bölge tvm seçmedi ise
        if ($("#Hazirlayan_TVMKodu").length > 0) {
            var tvm = $("#Hazirlayan_TVMKodu").val();
            if (!tvm || tvm == null || tvm == "") {
                $("#tvm-sorgu-hata").html("Lütfen acente seçiniz.");
                $("#tvm-sorgu-hata").show();
                return false;
            }
        }

        if (!$("#Musteri_SigortaEttiren_AdiUnvan").is(":visible")) {
            $("#sigortaettiren-sorgula").trigger("click");
            return false;
        }

        $("#Musteri_SigortaEttiren_CepTelefonu").addClass("ignore");
        $("#Musteri_Sigortali_CepTelefonu").addClass("ignore");
        $("#Musteri_SigortaEttiren_Email").addClass("ignore");
        $("#Musteri_Sigortali_Email").addClass("ignore");

        return sigortaliKontrol.Kaydet();
    }

    // Araç bilgileri
    else if (current == 2) {

        if (aracDegerWait) {
            return;
        }

        var plakaNo = $("#Arac_PlakaNo").val().toUpperCase();
        var tescilNo = $("#Arac_TescilBelgeSeriNo").val();
        var asbisNo = $("#Arac_AsbisNo").val();
        
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

        var aracModelYili = parseInt($("#Arac_Model").val());
        if (thisYear - 10 > aracModelYili) {
            $("#Teminat_Eskime_Payi_Teminati_control").bootstrapSwitch('setActive', true);
            $("#Teminat_Eskime_Payi_Teminati").removeAttr("disabled");
            $("#Teminat_Eskime_Payi_Teminati_control").bootstrapSwitch('setState', false);
        } else {
            $("#Teminat_Eskime_Payi_Teminati_control").bootstrapSwitch('setActive', false);
            $("#Teminat_Eskime_Payi_Teminati").attr("disabled", "disabled");
            $("#Teminat_Eskime_Payi_Teminati_control").bootstrapSwitch('setState', false);
        }
        var kullanimTarzi = $("#Arac_KullanimTarziKodu").val();
        var parts = kullanimTarzi.split('-');
        if (parts.length == 2) {
            if (parts[0] == '511' || parts[0] == '521' || parts[0] == '523' || parts[0] == '526' || parts[0] == '532' || parts[0] == '611') {
                $(".tasinan-yuk").show();
            }
            else {
                $(".tasinan-yuk").hide();
            }
        }
        var isValid = FormWizard.validatePage('#tab3');

        var aracBilgileri = $("#Arac_AracBilgileri_Acik").val();
        if (aracBilgileri == "0") {
            $("#btn-sorgula").trigger("click");
            isValid = 0;
        }

        if (isValid == 1) {

            if (kullanimTarzi == "421-11") {
                $("#Teminat_BelediyeHalkOtobusu_control").bootstrapSwitch('setActive', true);
                $("#Teminat_BelediyeHalkOtobusu").removeAttr("disabled");
            } else {
                $("#Teminat_BelediyeHalkOtobusu_control").bootstrapSwitch('setActive', false);
                $("#Teminat_BelediyeHalkOtobusu").attr("disabled", "");
            }
                       
            if ($('#Teminat_AMSKodu').children('option').length == 0) {
                GetAracAMSKodlari();
            }

            if (!ikameTuruSorgulandi) {
                ikameTuruSorgula();
                return false;
            }
        }
        ikameTuruSorgulandi = false;

        return isValid == 1;
    }

    return true;
}

var mapfeKaskoOdeme = new function () {
    return {

        init: function () {
            $("#kk-odeme-btn").click(mapfeKaskoOdeme.kredikarti);
            $(".teklif-satin-al").live("click", mapfeKaskoOdeme.kredikartiOdeme);
            $("#kk-odeme").on("shown", function () { $("#KrediKarti_KartSahibi").focus(); })

            $("#KK_OdemeTipi_1").live("click", function () {
                var val = $(this).is(':checked');
                if (val) {
                    $(".kk-class").hide();
                    $(".kk-class").find("input").addClass("ignore");
                }
            });
            $("#KK_OdemeTipi_2").live("click", function () {
                var val = $(this).is(':checked');
                if (val) {
                    $(".kk-class").show();
                    $(".kk-class").find("input").removeClass("ignore");
                }
            });
            $("#KK_OdemeSekli_1").live("click", function () {
                var val = $(this).is(':checked');
                if (val) {
                    $(".taksit-sayisi").hide();
                }
            });
            $("#KK_OdemeSekli_2").live("click", function () {
                var val = $(this).is(':checked');
                if (val) {
                    $(".taksit-sayisi").show();
                }
            });
        },

        kredikartiOdeme: function () {
            var teklifId = $(this).attr("teklif-id");
            var fiyat = $(this).attr("fiyat");
            if (teklifId && fiyat) {
                var nakit = $("#KK_OdemeTipi_1").is(':checked');
                if (nakit) {
                    $(".kk-class").hide();
                    $(".kk-class").find("input").addClass("ignore");
                }
                else {
                    $(".kk-class").show();
                    $(".kk-class").find("input").removeClass("ignore");
                }
                $("#kredi-karti-error-list").html("");
                $("#KrediKarti_KK_TeklifId").val(teklifId);
                $("#kk-tutar").html(fiyat);
                $("#kk-odeme").modal("show");
            }
        },

        kredikarti: function () {
            var isvalid = $("#krediKartiForm").valid();
            if (isvalid) {
                $("#kredi-karti-error").hide();
                $(this).button("loading");
                $("#kk-odeme-cancel").hide();

                var contents = $("#krediKartiForm").serialize();

                $.ajax({
                    type: "POST",
                    url: "/Teklif/MapfreKasko/OdemeAl",
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

