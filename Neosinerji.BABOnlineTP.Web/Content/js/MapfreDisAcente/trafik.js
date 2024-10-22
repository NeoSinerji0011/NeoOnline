$(document).ready(function () {
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

    // ==== Arac tipi ve markası seçildiğinde diğer alanlar da güncelleniyor ==== //
    $("#Arac_TipKodu").change(function () {
        var tipKodu = $("#Arac_TipKodu").val();
        $("#type-code").val(tipKodu);
    });
    $("#Arac_MarkaKodu").change(function () {
        var markaKodu = $("#Arac_MarkaKodu").val();
        $("#brand-code").val(markaKodu);

        var modelYili = $("#Arac_Model").val();

        if (modelYili > 0) {
            anadoluSigortaKullanimTipiSorgula();
        }

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
                url: "/Teklif/Trafik/EskiPoliceSorgula",
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
    // ==== Odeme tipi, şekli ve taksit sayısı kontrol ediliyor. ==== //
    $("#TaksitliOdeme_OdemeSekli_control").on("switch-change", OdemeSekliDegisti);

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

    $("#Arac_PlakaKodu").change(function () {
        var tescilIlKodu = '';
        if ($(this).val() != '') {
            tescilIlKodu = parseInt($(this).val());
        }
        $("#Arac_TescilIl").val(tescilIlKodu);
        $("#Arac_TescilIl").trigger("change");
    });

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
                url: "/Teklif/Trafik/EgmSorgula",
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
            $("#TeklifId").val("");

            $(this).button("loading");

            $(".switcher").find(":input").switchFix();

            var disabled = $("#form2,#form3").find(':input:disabled').removeAttr('disabled');
            var contents = $("#form1, #form2, #form3, #form4,#form5").serialize();
            disabled.attr('disabled', 'disabled');
            $.ajax(
                {
                    type: "POST",
                    url: "/Teklif/Trafik/Hesapla",
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
                            //var tavsiyeler = "";
                            //if (data.trafikTavsiyeEt) {
                            //    tavsiyeler += " <p> <img src='https://neoonlinestrg.blob.core.windows.net/sirket-logo/trafik.png' style='width: 50px; height: 50px;'> <font size='5'>Trafik Tavsiye Et</font></p>"
                            //}
                            //if (data.kaskoTavsiyeEt) {
                            //    tavsiyeler += "<p> <img src='https://neoonlinestrg.blob.core.windows.net/sirket-logo/kasko.png' style='width: 50px; height: 50px;'> <font size='5'>Kasko Tavsiye Et </font></p>"
                            //}
                            //if (data.daskTavsiyeEt) {
                            //    tavsiyeler += "<p><img src='https://neoonlinestrg.blob.core.windows.net/sirket-logo/dask.jpg' style='width: 50px; height: 50px;'> <font size='5'> Dask Tavsiye Et </font></p>"
                            //}
                            //if (data.yanginTavsiyeEt) {
                            //    tavsiyeler += "<p><img src='https://neoonlinestrg.blob.core.windows.net/sirket-logo/fire.png' style='width: 50px; height: 50px;'> <font size='5'> Yangın Tavsiye Et </font></p>"
                            //}
                            //if (tavsiyeler != "") {
                            //    Swal.fire({
                            //        imageUrl: 'https://neoonlinestrg.blob.core.windows.net/sirket-logo/only_logo.jpg',
                            //        imageWidth: 200,
                            //        imageHeight: 200,
                            //        imageAlt: 'A tall image',
                            //        title: "Yeni Ürün Önerme Sistemi",
                            //        html: tavsiyeler,
                            //        animation: false,
                            //        customClass: 'animated pulse'
                            //    })
                            //}
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

    $("#Arac_AnadoluKullanimTip").change(function () {
        $("#kullanimSekli_Anadolu").hide();
        $("#anadolu-kullanimsekli-progress").show();

        $("#anadolu-trafik-ksekli-hata").css('display', 'none');
        var KullanimTarzi = $("#Arac_KullanimTarziKodu").val();
        var Model = $("#Arac_Model").val();
        var MarkaKodu = $("#Arac_MarkaKodu").val();
        var KullanimTipi = $("#Arac_AnadoluKullanimTip").val();
        var AnadoluMarkaKodu = $("#Arac_AnadoluMarkaKodu").val();

        if ((KullanimTarzi != null && KullanimTarzi != '') && (Model != null && Model != '') && (MarkaKodu != null && MarkaKodu != '') && (KullanimTipi != null && KullanimTipi != '')) {

            $.getJSON('/Teklif/Trafik/KullanimSekliSorgulaAnadolu', { AnadoluMarkaKodu: AnadoluMarkaKodu, AracKullanimTarzi: KullanimTarzi, AracModelYili: Model, AracMarkaKodu: MarkaKodu, KullanimTipi: KullanimTipi },
               function (result) {
                   if (result.list.hata != "" && result.hata != null) {
                       $("#anadolu-trafik-ksekil-hata-satir").css('display', 'block');
                       $("#anadolu-trafik-ksekli-hata").html(result.hata);
                   }
                   else {
                       $("#anadolu-trafik-ksekil-hata-satir").css('display', 'none');
                       $("#anadolu-trafik-ksekli-hata").html("");
                   }
                   if (result.list.length > 1) {
                       $('#anadolu-trafik-ksekil-hata-satir').css('display', 'none');
                       $("#anadolu-kasko-ktip-hata-satir").css({ "display": "none" });
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

    $("#EskiPolice_SigortaSirketKodu").focusout(function () {
        var sirketKod = $(this).val();
        sirketKod = padLeft(sirketKod, 3);
        $("#EskiPolice_SigortaSirketiKodu").val(sirketKod);
    });

    $("#EskiPolice_SigortaSirketiKodu").change(function () {
        $("#EskiPolice_SigortaSirketKodu").val($("#EskiPolice_SigortaSirketiKodu").val());
    });

});

function setPlakaSorgu(data) {
    debugger;

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

        $("#Arac_KullanimSekliKodu").val(data.AracKullanimSekli);
        $("#Arac_KullanimTarziKodu").dropDownFill(data.Tarzlar);
        $("#Arac_KullanimTarziKodu").val(data.AracKullanimTarzi);
        $("#Arac_MarkaKodu").dropDownFill(data.Markalar);
        $("#Arac_MarkaKodu").val(data.AracMarkaKodu);

        $("#brand-code").val(data.AracMarkaKodu);
        $("#type-code").val(data.AracTipKodu);

        $("#Arac_Model").val(data.AracModelYili);
        $("#Arac_TipKodu").dropDownFill(data.Tipler);
        $("#Arac_TipKodu").val(data.AracTipKodu);

        $("#Arac_MotorNo").val(data.AracMotorNo);
        $("#Arac_SaseNo").val(data.AracSasiNo);

        if (data.HasarsizlikHata == "") {
            $("#Arac_HasarsizlikIndirim").val(data.HasarsizlikInd);
            $("#Arac_HasarSurprim").val(data.HasarsizlikSur);
            $("#Arac_UygulananKademe").val(data.HasarsizlikKademe);
            $("#Arac_UygulananOncekiKademe").val(data.UygulanmisHasarsizlikKademe);

            $("#Arac_HasarsizlikIndirim").prop('disabled', true);
            $("#Arac_HasarSurprim").prop('disabled', true);
            $("#Arac_UygulananKademe").prop('disabled', true);
            $("#Arac_UygulananOncekiKademe").prop('disabled', true);
        }
        else {
            $("#Arac_HasarsizlikIndirim").val("0");
            $("#Arac_HasarSurprim").val("0");
            $("#Arac_UygulananKademe").val("0");
            $("#Arac_UygulananOncekiKademe").val("0");

            $("#Arac_HasarsizlikIndirim").removeAttr('disabled');
            $("#Arac_HasarSurprim").removeAttr('disabled');
            $("#Arac_UygulananKademe").removeAttr('disabled');
            $("#Arac_UygulananOncekiKademe").removeAttr('disabled');
        }

        if (data.TramerBelgeNumarasi != "" && data.TramerBelgeNumarasi != null) {
            $("#Arac_BelgeNumarasiTramer").val(data.TramerBelgeNumarasi);
            $("#Arac_BelgeNumarasiTramer").prop('disabled', true);
        }
        else {

            $("#Arac_BelgeNumarasiTramer").val("0");
            $("#Arac_BelgeNumarasiTramer").removeAttr('disabled');
        }

        if (data.TramerBelgeTarihi != "" && data.TramerBelgeTarihi != null) {
            $("#Arac_BelgeTarihTramer").val(data.TramerBelgeTarihi);
            $("#Arac_BelgeTarihTramer").prop('disabled', true);
        }
        else {
            $("#Arac_BelgeTarihTramer").val("01/01/1001");
            $("#Arac_BelgeTarihTramer").removeAttr('disabled');
        }

        $("#Arac_KisiSayisi").val(data.AracKoltukSayisi);

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

                                if (data.AnadoluHata != "" && data.AnadoluHata != null) {
                                    $("#anadolu-trafik-ktip-hata-satir").css('display', 'block');
                                    $("#anadolu-trafik-ktip-hata").html(data.AnadoluHata);
                                }
                                else {
                                    $("#anadolu-trafik-ktip-hata-satir").css('display', 'none');
                                    $("#anadolu-trafik-ktip-hata").html("");
                                }

                                $("#Arac_AnadoluKullanimTip").dropDownFill(data.AnadoluKullanimTipleri);
                                $("#kullanimTip_Anadolu").show();
                                $("#Arac_AnadoluMarkaKodu").val(data.anadoluMarkaKodu);
                            }
                            else {
                                $("#anadolu-ozel").hide();
                                $("#kullanimTip_Anadolu").hide();
                            }
                        }
                    }
                }
                else {
                    $("#kullanimTip_Anadolu").hide();
                }
            }
            else {
                $("#kullanimTip_Anadolu").hide();
            }
        }
        else {
            $("#anadolu-ozel").show();
            $("#anadolu-trafik-ktip-hata-satir").css('display', 'block');
            $("#anadolu-trafik-ktip-hata").html("Hata! Anadolu Sigorta'nın özel alanları yüklenemedi. Anadolu Sigorta, Markası Motosiklet olan araçlar için Online Teklif düzenletmemektedir.");
        }

        $("#Arac_MotorGucu").val(data.MotorGucu);
        $("#Arac_SilindirHacmi").val(data.SilindirHacmi);
        //$("#Arac_ImalatYeri").val(data.ImalatYeri);       
        //$("#Arac_Renk").val(data.Renk);


        if (data.AracTescilTarih.length > 0) {
            $('#Arac_TrafikTescilTarihi').datepicker('setDate', data.AracTescilTarih);
            $('#Arac_TrafigeCikisTarihi').datepicker('setDate', data.AracTescilTarih);
        }

        if (data.EskiPoliceNo != null && data.EskiPoliceNo.length > 0 && data.EskiPoliceNo != "0") {
            $("#EskiPolice_EskiPoliceVar_control").bootstrapSwitch('setState', true);
            $("#EskiPolice_SigortaSirketiKodu").val(data.EskiPoliceSigortaSirkedKodu);
            $("#EskiPolice_AcenteNo").val(data.EskiPoliceAcenteKod);
            $("#EskiPolice_PoliceNo").val(data.EskiPoliceNo);
            $("#EskiPolice_YenilemeNo").val(data.EskiPoliceYenilemeNo);
            $("#EskiPolice_SigortaSirketKodu").val(data.EskiPoliceSigortaSirkedKodu);
            if (data.YeniPoliceBaslangicTarih.length > 0) {
                $('#Arac_PoliceBaslangicTarihi').datepicker('setDate', data.YeniPoliceBaslangicTarih);
            }

            $("#EskiPolice_SigortaSirketiKodu").prop('disabled', true);
            $("#EskiPolice_AcenteNo").prop('disabled', true);
            $("#EskiPolice_PoliceNo").prop('disabled', true);
            $("#EskiPolice_YenilemeNo").prop('disabled', true);
            $("#EskiPolice_SigortaSirketKodu").prop('disabled', true);
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

    $("#form4").show();
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
        $("#EskiPolice_SigortaSirketKodu").removeClass("ignore");
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

    }
        //Sigorta ettiren / sigortali tab
    else if (current == 2) {
        debugger;
        if (!$("#Musteri_SigortaEttiren_AdiUnvan").is(":visible")) {
            $("#sigortaettiren-sorgula").trigger("click");
            return false;
        }

        if ($("#Arac_PlakaNo").val() != "") {
            var AnadoluKullanimTip = $("#Arac_AnadoluKullanimTip").val();
            var AnadoluKullanimSekli = $("#Arac_AnadoluKullanimSekli").val();

            if (AnadoluKullanimTip == null && AnadoluKullanimSekli == null) {
                anadoluSigortaKullanimTipiSorgula();
            }
        }
        return sigortaliKontrol.Kaydet();
    }
        // Araç bilgileri
    else if (current == 3) {

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


        var isValid = FormWizard.validatePage('#tab3');

        var aracBilgileri = $("#Arac_AracBilgileri_Acik").val();
        if (aracBilgileri == "0") {
            $("#btn-sorgula").trigger("click");
            isValid = 0;
        }


        if (isValid) {
            var aracKullanimTarzi = $("#Arac_KullanimTarziKodu").val();
            $.getJSON('/Common/TrafikIMM', { KullanimTarzi: aracKullanimTarzi },
                function (result) {
                    $("#Teminat_IMMKodu").empty();
                    $("#Teminat_IMMKodu").dropDownFill(result);
                });

            $.getJSON('/Common/TrafikFK', { KullanimTarzi: aracKullanimTarzi },
                function (result) {
                    $("#Teminat_FKKodu").empty();
                    $("#Teminat_FKKodu").dropDownFill(result);
                });

        }

        return isValid == 1;
    }

    return true;
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

                var vadeli = $("#TaksitliOdeme_OdemeSekli").is(':checked');
                if (!vadeli) {
                    $("#KK_OdemeSekli_2").attr('checked', true);
                    $("#taksit-sayisi").show();
                    var taksitliOdeme = $("#TaksitliOdeme_TaksitSayisi").val();
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
                    url: "/Teklif/Trafik/OdemeAl",
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
                        //$("#KrediKarti_TaksitSayisi").attr("readonly", true);

                        $("#KK_OdemeTipi_1").attr("disabled", true);
                        $("#KK_OdemeTipi_2").attr("disabled", true);
                        $("#KK_OdemeTipi_3").attr("disabled", true);
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
                if ($("#Arac_MarkaKodu").val() != 600) {
                    $("#kullanimTip_Anadolu").hide();
                    $("#kullanimSekli_Anadolu").hide();
                    $('#anadolu-trafik-ktip-hata-satir').css('display', 'none');
                    $('#anadolu-trafik-ksekil-hata-satir').css('display', 'none');
                    $("#anadolu-kullanimtipi-progress").show();
                    var AracKodu = $("#Arac_MarkaKodu").val();
                    var kullanimTarzi = $("#Arac_KullanimTarziKodu").val();
                    var modelYili = $("#Arac_Model").val();

                    if ((AracKodu != null && AracKodu != '') && (kullanimTarzi != null && kullanimTarzi != '') && (modelYili != null && modelYili != '')) {

                        $.getJSON('/Teklif/Trafik/KullanimTipListAnadolu', { AracKullanimTarzi: kullanimTarzi, AracModelYili: modelYili, AracMarkaKodu: AracKodu },
                     function (result) {
                         if (result.list.length > 0) {

                             if (result.hata != "" && result.hata != null) {

                                 $("#anadolu-trafik-ktip-hata-satir").css('display', 'block');
                                 $("#anadolu-trafik-ktip-hata").html(result.hata);
                             }
                             else {
                                 $('#anadolu-trafik-ktip-hata-satir').css('display', 'none');
                                 //$("#anadolu-trafik-ktip-hata-satir").hide();
                                 $("#anadolu-trafik-ktip-hata").html("");
                             }

                             $("#Arac_AnadoluKullanimTip").dropDownFill(result.list);
                             $("#kullanimTip_Anadolu").show();
                             $("#anadolu-kullanimtipi-progress").hide();
                             $("#Arac_AnadoluMarkaKodu").val(result.AnadoluMarkaKodu);


                             if (result.list.length > 1) {
                                 $('#anadolu-trafik-ktip-hata-satir').css('display', 'none');
                                 $('#anadolu-trafik-ksekil-hata-satir').css('display', 'none');
                             }
                         }
                         else {
                             $("#anadolu-ozel").hide();
                             $("#kullanimTip_Anadolu").hide();
                             $("#kullanimSekli_Anadolu").hide();
                             $("#anadolu-kullanimtipi-progress").hide();
                         }
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
                    $("#anadolu-trafik-ktip-hata-satir").css('display', 'block');
                    //$("#anadolu-trafik-ktip-hata-satir").show();
                    $("#anadolu-trafik-ktip-hata").html("Hata! Anadolu Sigorta'nın özel alanları yüklenemedi. Anadolu Sigorta, Markası Motosiklet olan araçlar için Online Teklif düzenletmemektedir.");
                }
            }
        }

    }
}

function padLeft(str, max) {
    str = str.toString();
    return str.length < max ? pad("0" + str, max) : str;
}