function mapfremarka(options) {
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
                }
            });
    });
}
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
    $("#Arac_TescilIl").removeAttr("data-val").removeAttr("data-val-required");
    $("#Arac_TescilIlce").removeAttr("data-val").removeAttr("data-val-required");
    $("#Arac_TrafikTescilTarihi").removeAttr("data-val").removeAttr("data-val-required");
    $("#Arac_TrafigeCikisTarihi").removeAttr("data-val").removeAttr("data-val-required");
    $("#Musteri_SigortaEttiren_CepTelefonu").addClass("ignore");
    $("#Musteri_Sigortali_CepTelefonu").addClass("ignore");
    $("#Arac_TescilIl").addClass("ignore");
    $("#Arac_TescilIlce").addClass("ignore");
    $("#Arac_TrafikTescilTarihi").addClass("ignore");
    $("#Arac_TrafigeCikisTarihi").addClass("ignore");

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
                url: "/Teklif/MapfreTrafik/EskiPoliceSorgula",
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
                data: { kimlikNo: kimlikNo, sigortaSirketi: sigortaSirketi, acenteNo: acenteNo, policeNo: policeNo, yenilemeNo: yenilemeNo, bransKodu: "410" },
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

    // ==== Tarih ayarları ==== //
    $('#Arac_TrafikTescilTarihi').change(function () {
        var d1 = $(this).datepicker("getDate");
        $("#Arac_TrafigeCikisTarihi").datepicker("setDate", d1);
        setTimeout(function () { $("#Arac_TrafigeCikisTarihi").datepicker("show"); }, 100);
    });
    // ==== Tarih Ayarları ==== //

    // ==== Arac tipi ve markası seçildiğinde diğer alanlar da güncelleniyor ==== //
    $("#Arac_TipKodu").change(function () {
        var tipKodu = $("#Arac_TipKodu").val();
        $("#type-code").val(tipKodu);
    });
    $("#Arac_MarkaKodu").change(function () {
        var markaKodu = $("#Arac_MarkaKodu").val();
        $("#brand-code").val(markaKodu);
    });

    $("#Arac_TescilIl").tescilil({ ilce: "#Arac_TescilIlce" });

    $("#Arac_PlakaNo").blur(function () {
        $(this).val($(this).val().toUpperCase());
    });

    $("#Arac_TescilBelgeSeriKod").blur(function () {
        $(this).val($(this).val().toUpperCase());
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

    $("#Arac_KullanimSekliKodu").change(function () {
        var kod = $(this).val();
        for (i = 0; i < aracGrupArray.length; i++) {
            if (aracGrupArray[i].Key == kod) {
                $("#Arac_KullanimTarziKodu").val(aracGrupArray[i].Value);
                $("#Arac_KullanimTarziKodu").trigger("change");
                break;
            }
        }
    });

    $("#Arac_PlakaKodu").change(function () {
        var tescilIlKodu = '';
        if ($(this).val() != '') {
            tescilIlKodu = parseInt($(this).val());
        }
        $("#Arac_TescilIl").val(tescilIlKodu);
        $("#Arac_TescilIl").trigger("change");
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
            var sigortaliAyni = true;

            if ($("#Musteri_SigortaliAyni_control").length > 0) {
                sigortaliAyni = $("#Musteri_SigortaliAyni_control").bootstrapSwitch('status');
                if (sigortaliAyni) {
                    musteriKodu = $("#Musteri_SigortaEttiren_MusteriKodu").val();
                } else {
                    musteriKodu = $("#Musteri_Sigortali_MusteriKodu").val();
                }
            }
            else {
                musteriKodu = $("#Musteri_SigortaEttiren_MusteriKodu").val();
            }

            $.ajax({
                dataType: "json",
                url: "/Teklif/MapfreTrafik/PlakaSorgula",
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
            $("#TeklifId").val("");

            $(this).button("loading");

            $(".switcher").find(":input").switchFix();

            var disabled = $("#form2,#form3").find(':input:disabled').removeAttr('disabled');
            var contents = $("#form1, #form2, #form3, #form4").serialize();
            disabled.attr('disabled', 'disabled');

            $("#step3group").css({ "visibility": "visible" });
            $("#teklif-fiyat-container").css({ "visibility": "visible" });
            $('#form_wizard_1').bootstrapWizard("next");

            $.ajax(
                {
                    type: "POST",
                    url: "/Teklif/MapfreTrafik/Hesapla",
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

    if ($("#Arac_PlakaNo").length > 0) {
        var plakaNo = $("#Arac_PlakaNo").val();
        if (plakaNo.length == 0) {
            $(".arac-bilgileri :input").addClass("ignore");
        } else {
            $(".arac-bilgileri").show();
        }
    }

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
        }
    });

    $("#Arac_TescilBelgeSeriNo").numeric();
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
    if (show) $("#eski-police-hasarsizlik").show();
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

function setPlakaSorgu(data) {
    $("#plaka-sorgu-hata").hide();
    if (data) {

        if (data.PlakaKodu && data.PlakaKodu.length > 0) {
            $("#Arac_PlakaKodu").val(data.PlakaKodu);
            $("#Arac_PlakaKodu").trigger("change");
        }
        if (data.PlakaNo && data.PlakaNo.length > 0)
            $("#Arac_PlakaNo").val(data.PlakaNo);

        if (data.PlakaKodu && data.PlakaKodu.length > 0)
            $("#Arac_PlakaKodu").val(data.PlakaKodu);
        if (data.PlakaNo && data.PlakaNo.length > 0)
            $("#Arac_PlakaNo").val(data.PlakaNo);

        if (data.Markalar && data.Markalar.length > 0)
            $("#Arac_MarkaKodu").dropDownFill(data.Markalar);

        if (data.FesihTarih && data.FesihTarih != null && data.FesihTarih.length > 0) {
            
        } else {

            $("#Arac_KullanimSekliKodu").val(data.AracKullanimSekli);
            $("#Arac_KullanimTarziKodu").val(data.AracKullanimTarzi);

            if (data.AracKullanimTarzi && data.AracKullanimTarzi.length > 0) {
                for (i = 0; i < aracGrupArray.length; i++) {
                    if (aracGrupArray[i].Value == data.AracKullanimTarzi) {
                        $("#Arac_KullanimSekliKodu").val(aracGrupArray[i].Key);
                        break;
                    }
                }
            }

            $("#Arac_MarkaKodu").val(data.AracMarkaKodu);
            $("#Arac_Model").val(data.AracModelYili);
            $("#Arac_TipKodu").dropDownFill(data.Tipler);
            $("#Arac_TipKodu").val(data.AracTipKodu);
            $("#brand-code").val(data.AracMarkaKodu);
            $("#type-code").val(data.AracTipKodu);

            $("#Arac_MotorNo").val(data.AracMotorNo);
            $("#Arac_SaseNo").val(data.AracSasiNo);

            if (data.TescilSeri && data.TescilSeri.length > 0 && data.TescilSeriNo && data.TescilSeriNo.length > 0) {
                $("#Arac_TescilBelgeSeriKod").val(data.TescilSeri);
                $("#Arac_TescilBelgeSeriNo").val(data.TescilSeriNo);
            } else if (data.TescilSeriNo && data.TescilSeriNo.length > 0) {
                $("#Arac_AsbisNo").val(data.TescilSeriNo);
            }

            if (data.AracTescilTarih && data.AracTescilTarih.length > 0) {
                $('#Arac_TrafikTescilTarihi').datepicker('setDate', data.AracTescilTarih);
                $('#Arac_TrafigeCikisTarihi').datepicker('setDate', data.AracTescilTarih);
            }
        }

        //Fesih Tarihi
        $('#Arac_FesihTarihi').val(data.FesihTarih);

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
        }

        if (data.TasiyiciSigPoliceNo != null && data.TasiyiciSigPoliceNo.length > 0 && data.TasiyiciSigPoliceNo != "0") {
            $("#Tasiyici_Sorumluluk_control").bootstrapSwitch('setState', true);
            $("#Tasiyici_SigortaSirketiKodu").val(data.TasiyiciSigSirkerKod);
            $("#Tasiyici_AcenteNo").val(data.TasiyiciSigAcenteNo);
            $("#Tasiyici_PoliceNo").val(data.TasiyiciSigPoliceNo);
            $("#Tasiyici_YenilemeNo").val(data.TasiyiciSigYenilemeNo);
        }
        else {
            $("#Tasiyici_Sorumluluk_control").bootstrapSwitch('setState', false);
            $("#Tasiyici_SigortaSirketiKodu").val("");
            $("#Tasiyici_AcenteNo").val("");
            $("#Tasiyici_PoliceNo").val("");
            $("#Tasiyici_YenilemeNo").val("");
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
    $("#egm-sorgu-row").show();
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
    $("#Arac_TescilIl").addClass("ignore");
    $("#Arac_TescilIlce").addClass("ignore");
    $("#Arac_TrafikTescilTarihi").addClass("ignore");
    $("#Arac_TrafigeCikisTarihi").addClass("ignore");
    $("#Arac_AracBilgileri_Acik").val("1");
    var eskiPolice = $("#EskiPolice_EskiPoliceVar_control").bootstrapSwitch('status');
    eskiPoliceSetValidation(eskiPolice);
    var tasiyiciSorumluluk = $("#Tasiyici_Sorumluluk_control").bootstrapSwitch('status');
    tasiyiciSorumlulukValidation(tasiyiciSorumluluk);
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

        var plakaNo = $("#Arac_PlakaNo").val().toUpperCase();
        var tescilNo = $("#Arac_TescilBelgeSeriNo").val();
        var asbisNo = $("#Arac_AsbisNo").val();

        if ($("#Arac_PlakaKodu").val() == "0") {
            $("#Arac_PlakaKodu").val("");
            $("#Arac_PlakaKodu").valid();
        }

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

        var isValid = FormWizard.validatePage('#tab3');

        var aracBilgileri = $("#Arac_AracBilgileri_Acik").val();
        if (aracBilgileri == "0") {
            $("#btn-sorgula").trigger("click");
            isValid = 0;
        }

        if (isValid == 1) {
            $("#form3").find('.control-group').removeClass('error');
            $("#form3").find('.field-validation-error').removeClass('field-validation-error').addClass('field-validation-valid').find("span").remove();
            $("#form3").find('.input-validation-error').removeClass('input-validation-error').addClass('valid');

            var kullanimTarzi = $("#Arac_KullanimTarziKodu").val();

            if (kullanimTarzi == "421-11" || kullanimTarzi == "411-10") {
                $("#Teminat_BelediyeHalkOtobusu_control").bootstrapSwitch('setActive', true);
                $("#Teminat_BelediyeHalkOtobusu").removeAttr("disabled");
            } else {
                $("#Teminat_BelediyeHalkOtobusu_control").bootstrapSwitch('setActive', false);
                $("#Teminat_BelediyeHalkOtobusu").attr("disabled", "");
            }
        }

        if (isValid == 1 && (hazineYururlukSonuc == '' || hazineYururlukSonuc == 'HATA') && plakaNo != "YK") {
            var policeNo = $("#EskiPolice_PoliceNo").val();
            var hazineSorgula = 0;
            if (policeNo != null && policeNo.length == 0) hazineSorgula = 1;

            if (hazineSorgula == 1) {
                hazineYururlulukSorgula();
                return false;
            }
        }

        return isValid == 1;
    }

    return true;
}

/* Hazine yürürlülük durumu sorgulanıyor */
var hazineYururlukSonuc = '';
function hazineYururlulukSorgula() {
    $("#button-next").button("loading");

    var kimlikNo = $("#Musteri_Sigortali_KimlikNo").val();
    if (kimlikNo == "" || kimlikNo == null) {
        kimlikNo = $("#Musteri_SigortaEttiren_KimlikNo").val();
    }
    var plakaNo = $("#Arac_PlakaNo").val();
    var plakaKodu = $("#Arac_PlakaKodu").val();
    var aracKullanimTarzi = $("#Arac_KullanimTarziKodu").val();

    $("#hazine_mesaj_gecmis").text("");
    $("#hazine_mesaj_yururluk").text("");

    $.ajax({
        dataType: "json",
        type: "post",
        url: "/MapfreTrafik/HazineYururluluk",
        data: { kimlikNo: kimlikNo, plakaNo: plakaNo, plakaIlKodu: plakaKodu, aracKullanimTarzi: aracKullanimTarzi },
        success: function (data) {
            
            if (data.success && data.response) {

                if (!data.response.mesajtipi || data.response.mesajtipi == '') {
                    var currentPage = $('#form_wizard_1').bootstrapWizard("currentIndex");
                    if (currentPage == 1)
                        $('#form_wizard_1').bootstrapWizard("next");
                    $("#button-next").button("reset");
                }

                hazineYururlukSonuc = data.response.mesajtipi;
                if (data.response.mesajtipi == 'HATA') {
                    $("#hazine-modal-header").text("Hazine Yürürlülük Hata");
                    if (data.response.mesaj_gecmis != null) {
                        $("#hazine_mesaj_gecmis").text(data.response.mesaj_gecmis);
                    }
                    if (data.response.mesaj_yururluk != null) {
                        $("#hazine_mesaj_yururluk").text(data.response.mesaj_yururluk);
                    }
                    $("#hazine-modal").modal("show");

                    $("#button-next").button("reset");
                    return;
                }

                if (data.response.mesajtipi == 'UYARI') {
                    $("#hazine-modal-header").text("Hazine Yürürlülük Uyarı");
                    if (data.response.mesaj_gecmis != null) {
                        $("#hazine_mesaj_gecmis").text(data.response.mesaj_gecmis);
                    }
                    if (data.response.mesaj_yururluk != null) {
                        $("#hazine_mesaj_yururluk").text(data.response.mesaj_yururluk);
                    }
                    $("#hazine-modal").modal("show");

                    var currentPage = $('#form_wizard_1').bootstrapWizard("currentIndex");
                    if (currentPage == 1)
                        $('#form_wizard_1').bootstrapWizard("next");
                    $("#button-next").button("reset");
                    return;
                }

                if (data.response.mesajtipi == 'OTORIZASYON') {
                    $("#hazine-modal-header").text("Hazine Yürürlülük Otorizasyon");
                    if (data.response.mesaj_gecmis != null) {
                        $("#hazine_mesaj_gecmis").text(data.response.mesaj_gecmis);
                    }
                    if (data.response.mesaj_yururluk != null) {
                        $("#hazine_mesaj_yururluk").text(data.response.mesaj_yururluk);
                    }
                    $("#hazine-modal").modal("show");

                    var currentPage = $('#form_wizard_1').bootstrapWizard("currentIndex");
                    if (currentPage == 1)
                        $('#form_wizard_1').bootstrapWizard("next");
                    $("#button-next").button("reset");
                    return;
                }
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $("#hazine-modal-header").text("Hazine Yürürlülük");
            $("#hazine_mesaj_gecmis").text("HATA OLUŞTU!");
            $("#hazine_mesaj_yururluk").text("Veri alınamadı.");
            $("#hazine-modal").modal("show");
            $("#button-next").button("reset");
        }
    });
}

var mapfeTrafikOdeme = new function () {
    return {

        init: function () {
            $("#kk-odeme-btn").click(mapfeTrafikOdeme.kredikarti);
            $(".teklif-satin-al").live("click", mapfeTrafikOdeme.kredikartiOdeme);
            $("#kk-odeme").on("shown", function () { $("#KrediKarti_KartSahibi").focus(); })

            $("#KK_OdemeTipi_1").live("click", function () {
                var val = $(this).is(':checked');
                if (val) {
                    $(".kk-class").hide();
                    $(".kk-class").find("input").addClass("ignore");
                    $(".taksit-sayisi").hide();
                    $("#KK_OdemeSekli_1").attr('checked', 'checked');
                    $("#KK_OdemeSekli_2").closest("label").hide();
                }
            });
            $("#KK_OdemeTipi_2").live("click", function () {
                var val = $(this).is(':checked');
                if (val) {
                    $(".kk-class").show();
                    $(".kk-class").find("input").removeClass("ignore");
                    $("#KK_OdemeSekli_2").closest("label").show();
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
                    $("#KK_OdemeSekli_2").closest("label").hide();
                }
                else {
                    $(".kk-class").show();
                    $(".kk-class").find("input").removeClass("ignore");
                    $("#KK_OdemeSekli_2").closest("label").show();
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
                    url: "/Teklif/MapfreTrafik/OdemeAl",
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

