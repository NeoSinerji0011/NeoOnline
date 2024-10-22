var DaskGenelBilgiler = new function () {
    return {
        YururluktePolice: function (e, data) {
            if (data.value) {
                $(".dask-policesi").slideDown("fast");
                $("#RizikoGenelBilgiler_DaskSigortaSirketi").removeClass("ignore");
                $("#RizikoGenelBilgiler_DaskPoliceninVadeTarihi").removeClass("ignore");
                $("#RizikoGenelBilgiler_DaskPoliceNo").removeClass("ignore");
            }
            else {
                $(".dask-policesi").slideUp("fast");
                $("#RizikoGenelBilgiler_DaskSigortaSirketi").addClass("ignore");
                $("#RizikoGenelBilgiler_DaskPoliceninVadeTarihi").addClass("ignore");
                $("#RizikoGenelBilgiler_DaskPoliceNo").addClass("ignore");
            }
        },

        YanginPolicesi: function (e, data) {
            if (data.value) {
                $(".yangin-policesi").slideDown("fast");
                $("#RizikoGenelBilgiler_YanginSigortaSirketi").removeClass("ignore");
                $("#RizikoGenelBilgiler_YanginPoliceNumarasi").removeClass("ignore");
            }
            else {
                $(".yangin-policesi").slideUp("fast");
                $("#RizikoGenelBilgiler_YanginSigortaSirketi").addClass("ignore");
                $("#RizikoGenelBilgiler_YanginPoliceNumarasi").addClass("ignore");
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

        EskiPoliceSorgula: function () {

            $("#adres-text").html('');
            $("#adres-info").removeClass("in");
            $("#adres-info").attr("style", "display:none");

            var eskipoliceno = $("#RizikoGenelBilgiler_DaskPoliceNo").val();
            if (eskipoliceno != "" && eskipoliceno.length > 4) {
                var input = $(".dask-policesi");
                App.blockUI(input);
                DaskGenelBilgiler.EskiPoliceYenidenSorgula();
                $.ajax({
                    type: "POST",
                    dataType: "json",
                    url: "/Teklif/Dask/EskiPoliceSorgulama",
                    data: { EskiPoliceNo: eskipoliceno },
                    success: function (data) {
                        App.unblockUI(input);

                        if (data.Durum == "0") {
                            DaskGenelBilgiler.EskiPoliceBasarili(data);
                        }
                        else { alert(data.DurumAciklama); }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        App.unblockUI(input);
                        var response = jQuery.parseJSON(jqXHR.responseText);
                        alert(response.message);
                    }
                });
            }
        },

        EskiPoliceBasarili: function (data) {

            //Poliçe Bilgileri
            var PoliceBilgileri = data.PoliceBilgileri;
            if (PoliceBilgileri.PoliceBitTarihi != "")
                $("#RizikoGenelBilgiler_DaskPoliceninVadeTarihi").val(PoliceBilgileri.PoliceBitTarihi);


            // -------  Eski Poliçe Bilgileri  ---------
            var EskiPoliceBilgileri = PoliceBilgileri.EskiPoliceBilgileri;
            if (EskiPoliceBilgileri != null)
                DaskGenelBilgiler.EskiPoliceBilgileriSet(EskiPoliceBilgileri);


            // --------- SigortaEttiren -------
            var SigortaEttiren = PoliceBilgileri.SigortaEttiren;
            if (SigortaEttiren != null)
                DaskGenelBilgiler.SigortaEttirenSet(SigortaEttiren);


            // -------  Bina Bilgileri  ---------
            var BinaBilgileri = PoliceBilgileri.BinaBilgileri;
            if (BinaBilgileri != null)
                DaskGenelBilgiler.BinaBilgileriSet(BinaBilgileri);


            // --------RizikoBilgileri----------
            var RizikoBilgileri = PoliceBilgileri.RizikoBilgileri;
            if (RizikoBilgileri != null)
                DaskGenelBilgiler.RizikoBilgileriSet(RizikoBilgileri);


            //*--------RehinAlacakBilgileri------
            var RehinAlacakBilgileri = PoliceBilgileri.RehinAlacakBilgileri;
            if (RehinAlacakBilgileri != null)
                DaskGenelBilgiler.RehinAlacakBilgileriSet(RehinAlacakBilgileri);


            //----------Sigortalilar---------
            var Sigortalilar = PoliceBilgileri.Sigortalilar;
            if (Sigortalilar != null)
                DaskGenelBilgiler.SigortalilarSet(Sigortalilar, PoliceBilgileri.SigortaliSayisi);
        },

        BinaBilgileriSet: function (BinaBilgileri) {
            //DaireYuzOlcumu
            if (BinaBilgileri.DaireYuzOlcumu != "" && BinaBilgileri.DaireYuzOlcumu != "0")
                $("#RizikoDigerBilgiler_DaireBrutYuzolcumu").val(BinaBilgileri.DaireYuzOlcumu);

            //BinaYapiTarzi
            if (BinaBilgileri.BinaYapiTarzi != "" && BinaBilgileri.BinaYapiTarzi != "0")
                $("#RizikoDigerBilgiler_YapiTarzi").val(BinaBilgileri.BinaYapiTarzi);

            //BinaInsaatYili
            if (BinaBilgileri.BinaInsaatYili != "" && BinaBilgileri.BinaInsaatYili != "0")
                $("#RizikoDigerBilgiler_BinaInsaYili").val(BinaBilgileri.BinaYapiTarzi);

            //ToplamKatSayisi
            if (BinaBilgileri.ToplamKatSayisi != "" && BinaBilgileri.ToplamKatSayisi != "0")
                $("#RizikoDigerBilgiler_BinaKatSayisi").val(BinaBilgileri.ToplamKatSayisi);

            //DaireKullanimSekli
            if (BinaBilgileri.DaireKullanimSekli != "" && BinaBilgileri.DaireKullanimSekli != "0")
                $("#RizikoDigerBilgiler_DaireKullanimSekli").val(BinaBilgileri.DaireKullanimSekli);

            //BinaHasar
            if (BinaBilgileri.BinaHasar != "" && BinaBilgileri.BinaHasar != "0")
                $("#RizikoDigerBilgiler_HasarDurumu").val(BinaBilgileri.BinaHasar);

            //BinaHasar
            if (BinaBilgileri.Ada != "" && BinaBilgileri.Ada != "0")
                $("#RizikoDigerBilgiler_Ada").val(BinaBilgileri.Ada);

            //Pafta
            if (BinaBilgileri.Pafta != "" && BinaBilgileri.Pafta != "0")
                $("#RizikoDigerBilgiler_PaftaNo").val(BinaBilgileri.Pafta);

            //Parsel
            if (BinaBilgileri.Parsel != "" && BinaBilgileri.Parsel != "0")
                $("#RizikoDigerBilgiler_Parsel").val(BinaBilgileri.Parsel);

            //Sayfa
            if (BinaBilgileri.Sayfa != "" && BinaBilgileri.Sayfa != "0")
                $("#RizikoDigerBilgiler_SayfaNo").val(BinaBilgileri.Sayfa);
        },

        EskiPoliceBilgileriSet: function (EskiPoliceBilgileri) {
            $("#eski-police-basarili").attr("style", "display:normal");
            $("#RizikoGenelBilgiler_DaskSigortaSirketi").val(EskiPoliceBilgileri.EskiPoliceSirket);
        },

        SigortaEttirenSet: function (SigortaEttiren) {

            if (SigortaEttiren.SigortaEttirenSifati != "") {
                var value = SigortaEttiren.SigortaEttirenSifati.replace("0", "");
                $("#RizikoDigerBilgiler_SigortaEttirenSifati").val(value);
            }

        },

        RizikoBilgileriSet: function (RizikoBilgileri) {

            if (RizikoBilgileri.IlKod != "" && RizikoBilgileri.IlKod != "0") {
                var ADRES = {
                    Il: "#RizikoAdresBilgiler_IlKodu",
                    Ilce: "#RizikoAdresBilgiler_IlceKodu",
                    Belde: "#RizikoAdresBilgiler_SemtBeldeKodu",
                    Mahalle: '#RizikoAdresBilgiler_MahalleKodu',
                    CaddeSokakBulvar: '#RizikoAdresBilgiler_CaddeKodu',
                    Binalar: '#RizikoAdresBilgiler_BinaKodu',
                    Daireler: '#RizikoAdresBilgiler_DaireKodu'
                };

                $(ADRES.Il).chosen().val(RizikoBilgileri.IlKod);
                $(ADRES.Il).trigger("liszt:updated");

                $(ADRES.Ilce).dropDownFill(RizikoBilgileri.Ilceler);
                $(ADRES.Ilce).trigger("liszt:updated");
                $(ADRES.Belde).empty().trigger("liszt:updated");
                $(ADRES.Mahalle).empty().trigger("liszt:updated");
                $(ADRES.CaddeSokakBulvar).empty().trigger("liszt:updated");
                $(ADRES.Binalar).empty().trigger("liszt:updated");
                $(ADRES.Daireler).empty().trigger("liszt:updated");

                setTimeout(function () {
                    if (RizikoBilgileri.Ilce != "" && RizikoBilgileri.Ilce != "0") {
                        $(ADRES.Ilce).chosen().val(RizikoBilgileri.Ilce);
                        $(ADRES.Ilce).trigger("liszt:updated");

                        $(ADRES.Belde).dropDownFill(RizikoBilgileri.Beldeler);
                        $(ADRES.Belde).trigger("liszt:updated");
                        $(ADRES.Mahalle).empty().trigger("liszt:updated");
                        $(ADRES.CaddeSokakBulvar).empty().trigger("liszt:updated");
                        $(ADRES.Binalar).empty().trigger("liszt:updated");
                        $(ADRES.Daireler).empty().trigger("liszt:updated");

                        setTimeout(function () {
                            if (RizikoBilgileri.Belde != "" && RizikoBilgileri.Belde != "0") {
                                $(ADRES.Belde).chosen().val(RizikoBilgileri.Belde);
                                $(ADRES.Belde).trigger("liszt:updated");
                                $(ADRES.Belde).trigger("change");
                            }
                        }, 500);
                    }
                }, 500);
            }

            if (RizikoBilgileri.PostaKod != "") {
                $("#RizikoAdresBilgiler_PostaKodu").val(RizikoBilgileri.PostaKod);
            }

            var adresText = "";

            adresText += RizikoBilgileri.Semt + " ";
            adresText += RizikoBilgileri.Mahalle + " ";
            adresText += RizikoBilgileri.Cadde + " ";
            adresText += RizikoBilgileri.Sokak + " ";
            adresText += RizikoBilgileri.SiteApartmanAd + " ";
            adresText += RizikoBilgileri.BinaNo + " ";
            adresText += RizikoBilgileri.Kat + " ";
            adresText += RizikoBilgileri.Daire + " ";
            adresText += RizikoBilgileri.BeldeAciklama + " ";
            adresText += RizikoBilgileri.IlceAciklama + " ";
            adresText += RizikoBilgileri.IlAciklama + " ";

            $("#RizikoAdresBilgiler_ParitusAdresDogrulama").val(adresText);

            DaskGenelBilgiler.AdresTextAktarma();
        },


        RehinAlacakBilgileriSet: function (RehinAlacakBilgileri) {

            if (RehinAlacakBilgileri.RehinAlacak == "H") {
                $("#RizikoGenelBilgiler_RehinliAlacakliDainMurtehinVarmi_control").bootstrapSwitch('setState', false);
                $(".rehinli-alacakli").val('');
            }
            else {
                $("#RizikoGenelBilgiler_RehinliAlacakliDainMurtehinVarmi_control").bootstrapSwitch('setState', true);

                //Kurum
                if (RehinAlacakBilgileri.Kurum != "")
                    $("#RizikoGenelBilgiler_Tipi").val(RehinAlacakBilgileri.Kurum);

                //KurumID
                if (RehinAlacakBilgileri.KurumID != "")
                    $("#RizikoGenelBilgiler_KurumBanka").val(RehinAlacakBilgileri.KurumID);

                //Subeler
                if (RehinAlacakBilgileri.Subeler != "") {
                    $("#RizikoGenelBilgiler_Sube").dropDownFill(RehinAlacakBilgileri.Subeler);

                    //SubeID
                    if (RehinAlacakBilgileri.SubeID != "")
                        $("#RizikoGenelBilgiler_Sube").val(RehinAlacakBilgileri.SubeID);
                }
                //HesapSozlesmeNo
                if (RehinAlacakBilgileri.HesapSozlesmeNo != "")
                    $("#RizikoGenelBilgiler_KrediReferansNo_HesapSozlesmeNo").val(RehinAlacakBilgileri.HesapSozlesmeNo);

                //KrediBitisTarih
                if (RehinAlacakBilgileri.KrediBitisTarih != "")
                    $("#RizikoGenelBilgiler_KrediBitisTarihi").val(RehinAlacakBilgileri.KrediBitisTarih);

                //KrediTutari
                if (RehinAlacakBilgileri.KrediTutari != "")
                    $("#RizikoGenelBilgiler_KrediTutari").val(RehinAlacakBilgileri.KrediTutari);

                //DovizKodu
                if (RehinAlacakBilgileri.DovizKodu != "")
                    $("#RizikoGenelBilgiler_DovizKodu").val(RehinAlacakBilgileri.DovizKodu);
            }
        },

        SigortalilarSet: function (Sigortalilar, count) {

            if (count > 0) {
                $("#RizikoDigerBilgiler_TapudaBirdenFazlaSigortaliVarmi_control").bootstrapSwitch("setState", true);
                for (var i = 0; i < count; i++) {
                    $("#btn-ekle").trigger("click");

                    var Sigortali = Sigortalilar[i];

                    var sigortaliID = "#RizikoDigerBilgiler_SigortaliList_" + i + "_";

                    $(sigortaliID).val(Sigortali.Ad + " " + Sigortali.Soyad);
                }
            }
            else { $("#RizikoDigerBilgiler_TapudaBirdenFazlaSigortaliVarmi_control").bootstrapSwitch("setState", false); }
        },

        EskiPoliceYenidenSorgula: function () {

            $("#eski-police-basarili").attr("style", "display:none");
            $(".rehinli-alacak").attr("style", "display:none");
            $(".eski-police-sorgu").val('');

            $("#RizikoAdresBilgiler_IlKodu").chosen().val('');
            $("#RizikoAdresBilgiler_IlKodu").trigger("liszt:updated");
            $("#RizikoAdresBilgiler_IlKodu").trigger("change");

            $("#RizikoDigerBilgiler_TapudaBirdenFazlaSigortaliVarmi_control").bootstrapSwitch("setState", false);
            $("#RizikoGenelBilgiler_RehinliAlacakliDainMurtehinVarmi_control").bootstrapSwitch('setState', false);
        },

        UATVKoduSorgula: function (UavtKodu) {
            $.getJSON("/Teklif/Dask/GetUAVTAdres", { UavtKodu: UavtKodu }, function (result) {

                if (result === undefined || result == null) return;
                var adres = result.Mahalle + " ";
                adres += result.CSBMAd + " ";
                adres += result.BinaNo + " / ";
                adres += result.DaireNo + " ";
                adres += result.IlAd + " ";
                adres += result.IlceAd + " ";
                adres += result.BeldeAd + " ";
                adres += " | UAVT Kodu : " + UavtKodu;


                $("#RizikoDigerBilgiler_PaftaNo").val(result.Pafta);
                $("#RizikoDigerBilgiler_Parsel").val(result.Parsel);
                $("#RizikoDigerBilgiler_Ada").val(result.Ada);
                $("#adres-text").html(adres);
                $("#adres-info").addClass("in");
                $("#adres-info").attr("style", "display:normal");
                $("#RizikoAdresBilgiler_UATVKodu").val(UavtKodu);
            });
        },

        AdresTextAktarma: function () {
            $("#RizikoAdresBilgiler_Il").val($("#RizikoAdresBilgiler_IlKodu_chzn").children().children("span").text());
            $("#RizikoAdresBilgiler_Ilce").val($("#RizikoAdresBilgiler_IlceKodu_chzn").children().children("span").text());
            $("#RizikoAdresBilgiler_Mahalle").val($("#RizikoAdresBilgiler_MahalleKodu_chzn").children().children("span").text());
            $("#RizikoAdresBilgiler_SemtBelde").val($("#RizikoAdresBilgiler_SemtBeldeKodu_chzn").children().children("span").text());
            $("#RizikoAdresBilgiler_Cadde").val($("#RizikoAdresBilgiler_CaddeKodu_chzn").children().children("span").text());
            $("#RizikoAdresBilgiler_Bina").val($("#RizikoAdresBilgiler_BinaKodu_chzn").children().children("span").text());
            $("#RizikoAdresBilgiler_Daire").val($("#RizikoAdresBilgiler_DaireKodu_chzn").children().children("span").text());
        }
    }
}

var DaskAdresBilgileri = function () {
    return {
    }
}

var daskOdeme = new function () {
    return {

        init: function () {
            $("#kk-odeme-btn").click(daskOdeme.kredikarti);
            $(".teklif-satin-al").live("click", daskOdeme.kredikartiOdeme);
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

                //if (teklifId && fiyat) {
                //    $("#KrediKarti_KK_TeklifId").val(teklifId);
                //    $("#kk-tutar").html(fiyat);
                //    $("#kk-odeme").modal("show");
                //}
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
                    url: "/Teklif/Dask/OdemeAl",
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

function daskTeklifWizardCallback(current) {
    //Hazırlayan bilgileri
    if (current == 1) {

    }
    //Sigorta ettiren / sigortali tab
    else if (current == 2) {
        if (!$("#Musteri_SigortaEttiren_AdiUnvan").is(":visible")) {
            $("#sigortaettiren-sorgula").trigger("click");
            return false;
        }

        ParitusHelper.TekrarTeklifParitusAdresDogrula();


        return sigortaliKontrol.Kaydet();
    }
    // Riziko Bilgileri
    else if (current == 3) {
        var isValid = FormWizard.validatePage('#tab3');

        var sayac = 0;

        $(".sigortali-element").each(function (index, value) {
            if (this.style.display == "normal" || this.style.display == "") {
                var div = $("#" + this.id);
                if (div.find(".sigortali-inputs").val() == "") {
                    sayac = 1;
                }
            }
        });

        if (sayac == 1) {
            isValid = 0;
            alert("Sigortalı bilgisi eksik");
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

    // ======================= DASK GENEL BİLGİLER =============================//

    // ==== Kurum Sube ==== //
    $("#RizikoGenelBilgiler_KurumBanka").KurumSube({ Sube: '#RizikoGenelBilgiler_Sube' });

    // ==== Swich ==== //
    $("#RizikoGenelBilgiler_YururlukteDaskPolicesiVarmi_control").on("switch-change", DaskGenelBilgiler.YururluktePolice);
    $("#RizikoGenelBilgiler_YanginPolicesiVarmi_control").on("switch-change", DaskGenelBilgiler.YanginPolicesi);
    $("#RizikoGenelBilgiler_RehinliAlacakliDainMurtehinVarmi_control").on("switch-change", DaskGenelBilgiler.RehinliAlacakli);

    // ======================= Başlangıçta bu kontroller kapalı

    //Rehinli alacak
    $("#RizikoGenelBilgiler_Tipi").addClass("ignore");
    $("#RizikoGenelBilgiler_KurumBanka").addClass("ignore");
    $("#RizikoGenelBilgiler_Sube").addClass("ignore");
    $("#RizikoGenelBilgiler_DovizKodu").addClass("ignore");
    $("#RizikoGenelBilgiler_KrediBitisTarihi").addClass("ignore");
    $("#RizikoGenelBilgiler_KrediTutari").addClass("ignore");
    $("#RizikoGenelBilgiler_KrediReferansNo_HesapSozlesmeNo").addClass("ignore");

    //Yangın
    $("#RizikoGenelBilgiler_YanginSigortaSirketi").addClass("ignore");
    $("#RizikoGenelBilgiler_YanginPoliceNumarasi").addClass("ignore");

    //Dask
    $("#RizikoGenelBilgiler_DaskSigortaSirketi").addClass("ignore");
    $("#RizikoGenelBilgiler_DaskPoliceninVadeTarihi").addClass("ignore");
    $("#RizikoGenelBilgiler_DaskPoliceNo").addClass("ignore");

    //Adres
    $("#RizikoAdresBilgiler_Han_Aprt_Fab").addClass("ignore");

    // ======================= DASK ADRES BİLGİLER =============================//

    // ==== UAVTSorgu ==== //
    $("#RizikoAdresBilgiler_IlKodu").UAVTSorgu({
        Ilce: "#RizikoAdresBilgiler_IlceKodu",
        Belde: "#RizikoAdresBilgiler_SemtBeldeKodu",
        Mahalle: '#RizikoAdresBilgiler_MahalleKodu',
        CaddeSokakBulvar: '#RizikoAdresBilgiler_CaddeKodu',
        Binalar: '#RizikoAdresBilgiler_BinaKodu',
        Daireler: '#RizikoAdresBilgiler_DaireKodu'
    });


    $("#RizikoAdresBilgiler_PostaKodu").numeric();
    $("#RizikoDigerBilgiler_DaireBrutYuzolcumu").numeric();
    $("#RizikoGenelBilgiler_DaskPoliceNo").numeric();

    // ======================= DASK DİGER BİLGİLER =============================//

    $("#OdemeTipi_2[name='Odeme.OdemeTipi']").attr("checked", "checked");

    // ==== Odeme tipi, şekli ve taksit sayısı kontrol ediliyor. ==== //
    $("#Odeme_OdemeSekli_control").on("switch-change", OdemeSekliDegisti);
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
                url: "/Teklif/Dask/Hesapla",
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

$("#btn-ekle").click(function () {
    var sayac = 0;
    App.scrollTo($(".sigortali-container"));
    $(".sigortali-element").each(function (index, value) {
        if (this.style.display == "none" & sayac == 0) {
            var div = $("#" + this.id);
            div.attr("style", "display:normal");
            sayac = 1;
            App.scrollTo(div)
            return;
        }
    });
});

$("#btn-cikar").click(function () {
    var sayac = 0;
    App.scrollTo($(".sigortali-container"));
    $($(".sigortali-element").get().reverse()).each(function (index, value) {
        if ((this.style.display == "normal" || this.style.display == "") & sayac == 0) {
            var div = $("#" + this.id);
            div.attr("style", "display:none");
            div.find(".sigortali-inputs").val("");
            sayac = 1;
            App.scrollTo(div);
            return;
        }
    });
});

$("#RizikoDigerBilgiler_TapudaBirdenFazlaSigortaliVarmi_control").on("switch-change", function (e, data) {
    if (data.value) {
        $(".sigortali-container").slideDown("fast");
        $(".sigortali-inputs").val("");
        $(".sigortali-element").attr("style", "display:none");
    }
    else {
        $(".sigortali-container").slideUp("fast");
        $(".sigortali-inputs").val("");
        $(".sigortali-element").attr("style", "display:none");
    }
});

$("#RizikoAdresBilgiler_DaireKodu").change(function () {
    var UavtKodu = $(this).val();
    if (UavtKodu === undefined || UavtKodu == "0") return;
    DaskGenelBilgiler.UATVKoduSorgula(UavtKodu);
});

$("#eski-police-sorgulama").click(function () {
    DaskGenelBilgiler.EskiPoliceSorgula();
});


//Adres aktarma methodları
$("#RizikoAdresBilgiler_IlKodu").change(function () {
    DaskGenelBilgiler.AdresTextAktarma();
});

$(".adres-bilgileri").change(function () {
    DaskGenelBilgiler.AdresTextAktarma();
});



var sigortaliKontrol = new function () {

    this.daskInit = function () {

        $("#DaskMusteri_SigortaliAyni_control").on("switch-change", sigortaliKontrol.sigortaliFarkliDask);
        $("#DaskMusteri_SigortaEttiren_MusteriTipKodu").change(sigortaliKontrol.sigortaEttirenTipiDask);
        $("#DaskMusteri_Sigortali_MusteriTipKodu").change(sigortaliKontrol.sigortaliTipiDask);
        $("#daskSigortaettiren-sorgula").click(sigortaliKontrol.sigortaEttirenSorgulaDask);
        $("#sigortali-sorgula").click(sigortaliKontrol.sigortaliSorgula);

        $("#DaskMusteri_SigortaEttiren_UlkeKodu").ulke({ il: "#DaskMusteri_SigortaEttiren_IlKodu", ilce: "#DaskMusteri_SigortaEttiren_IlceKodu" });
        $("#DaskMusteri_Sigortali_UlkeKodu").ulke({ il: "#DaskMusteri_Sigortali_IlKodu", ilce: "#DaskMusteri_Sigortali_IlceKodu" });

        //// ==== Uyruk ve cinsiyet hatalı geliyor düzeltiliyor. ==== //
        //$("#Uyruk_0").val("0");
        //$("#Cinsiyet_K").val("K");

        var musteriKodu = $("#DaskMusteri_SigortaEttiren_MusteriKodu").val();
        if (musteriKodu != "") {
            $(".sigortaettiren-satir.musteri-tipi").show();
            $("#DaskMusteri_SigortaEttiren_MusteriTipKodu").attr("disabled", "disabled");
            $("#sigortaettiren-kimlikno-mesaj").hide();
            sigortaliKontrol.sigortaEttirenTipi();
        }
        var sigortaliAyni = $("#DaskMusteri_SigortaliAyni_control").bootstrapSwitch('status');
        if (sigortaliAyni) {
            $(".sigortali-satir :input").addClass("ignore");
            $("#DaskMusteri_Sigortali_MusteriKodu").addClass("ignore");
            $("#DaskMusteri_Sigortali_KimlikNo").addClass("ignore");
            $("#sigortali").show();
        } else {
            musteriKodu = $("#DaskMusteri_Sigortali_MusteriKodu").val();
            $("#sigortali").hide();
            if (musteriKodu != "") {
                $(".sigortali-kimlikno").show();
                $("#DaskMusteri_Sigortali_MusteriKodu").removeClass("ignore");
                $("#DaskMusteri_Sigortali_KimlikNo").removeClass("ignore");
                $(".sigortali-satir.musteri-tipi").show();
                $("#DaskMusteri_Sigortali_MusteriTipKodu").attr("disabled", "disabled");
                $("#sigortali-kimlikno-mesaj").hide();

                sigortaliKontrol.sigortaliTipi();
            }
        }
    },

        this.sigortaliFarkliDask = function (e, data) {
            if (data.value) {
                $(".sigortali-kimlikno").slideUp("fast");
                $("#DaskMusteri_Sigortali_MusteriKodu").addClass("ignore");
                $("#DaskMusteri_Sigortali_KimlikNo").addClass("ignore");
                $(".sigortali-satir.ozel").slideUp();
                $(".sigortali-satir.tuzel").slideUp();
                $(".sigortali-satir.tuzel :input").addClass("ignore");
                $(".sigortali-satir.ozel :input").addClass("ignore");
                $("#sigortali").show();
            } else {
                $(".sigortali-kimlikno").slideDown("fast");
                $("#DaskMusteri_Sigortali_MusteriKodu").removeClass("ignore");
                $("#DaskMusteri_Sigortali_KimlikNo").removeClass("ignore");
                $(".sigortali-satir.tuzel :input").removeClass("ignore");
                $(".sigortali-satir.ozel :input").removeClass("ignore");
                $("#sigortali").hide();
            }
        },

        this.sigortaEttirenTipiDask = function () {
            var musteriTipi = $("#DaskMusteri_SigortaEttiren_MusteriTipKodu").val();
            if (musteriTipi == "1") { //Şahıs
                $(".sigortaettiren-satir.tuzel").slideUp();
                $(".sigortaettiren-satir.ozel").slideDown();
                $(".sigortaettiren-satir.tuzel :input").addClass("ignore");
                $(".sigortaettiren-satir.ozel :input").addClass("ignore");

                $("#Uyruk_1[name='DaskMusteri.SigortaEttiren.Uyruk']").attr("disabled", "disabled");
                $("#Uyruk_0[name='DaskMusteri.SigortaEttiren.Uyruk']").attr("checked", "checked");
                $("#Uyruk_0[name='DaskMusteri.SigortaEttiren.Uyruk']").removeAttr("disabled");

            } else if (musteriTipi == "2") { //Tüzel
                $(".sigortaettiren-satir.ozel").slideUp();
                $(".sigortaettiren-satir.tuzel").slideDown();
                $(".sigortaettiren-satir.ozel :input").addClass("ignore");
                $(".sigortaettiren-satir.tuzel :input").addClass("ignore");
            } else if (musteriTipi == "3") { //Şahıs firması
                $(".sigortaettiren-satir.ozel").slideUp();
                $(".sigortaettiren-satir.tuzel").slideDown();
                $(".sigortaettiren-satir.ozel :input").addClass("ignore");
                $(".sigortaettiren-satir.tuzel :input").addClass("ignore");
            } else if (musteriTipi == "4") { //Yabancı
                $(".sigortaettiren-satir.tuzel").slideUp();
                $(".sigortaettiren-satir.ozel.yabanci").slideDown();
                $(".sigortaettiren-satir.tuzel :input").addClass("ignore");
                $(".sigortaettiren-satir.ozel.yabanci :input").addClass("ignore");

                $(".sigortaettiren-satir.ozel").slideDown();
                $(".sigortaettiren-satir.ozel :input").removeClass("ignore");

                $("#Uyruk_0[name='DaskMusteri.SigortaEttiren.Uyruk']").attr("disabled", "disabled");
                $("#Uyruk_1[name='DaskMusteri.SigortaEttiren.Uyruk']").attr("checked", "checked");
                $("#Uyruk_1[name='DaskMusteri.SigortaEttiren.Uyruk']").removeAttr("disabled");
            } else {
                $(".sigortaettiren-satir.ozel").slideUp();
                $(".sigortaettiren-satir.tuzel").slideUp();
            }
        },
        this.sigortaliTipiDask = function () {
            var musteriTipi = $("#DaskMusteri_Sigortali_MusteriTipKodu").val();

            if (musteriTipi == "1") { //Şahıs
                $(".sigortali-satir.tuzel").slideUp();
                $(".sigortali-satir.ozel").slideDown();
                $(".sigortali-satir.tuzel :input").addClass("ignore");
                $(".sigortali-satir.ozel :input").removeClass("ignore");

                $("#Uyruk_1[name='DaskMusteri.Sigortali.Uyruk']").attr("disabled", "disabled");
                $("#Uyruk_0[name='DaskMusteri.Sigortali.Uyruk']").attr("checked", "checked");
                $("#Uyruk_0[name='DaskMusteri.Sigortali.Uyruk']").removeAttr("disabled");
            } else if (musteriTipi == "2") { //Tüzel
                $(".sigortali-satir.ozel").slideUp();
                $(".sigortali-satir.tuzel").slideDown();
                $(".sigortali-satir.ozel :input").addClass("ignore");
                $(".sigortali-satir.tuzel :input").removeClass("ignore");
            } else if (musteriTipi == "3") { //Şahıs firması
                $(".sigortali-satir.ozel").slideUp();
                $(".sigortali-satir.tuzel").slideDown();
                $(".sigortali-satir.ozel :input").addClass("ignore");
                $(".sigortali-satir.tuzel :input").removeClass("ignore");
            } else if (musteriTipi == "4") { //Yabancı
                $(".sigortali-satir.tuzel").slideUp();
                $(".sigortali-satir.ozel.yabanci").slideDown();
                $(".sigortali-satir.tuzel :input").addClass("ignore");
                $(".sigortali-satir.ozel.yabanci :input").removeClass("ignore");

                $("#Uyruk_0[name='DaskMusteri.Sigortali.Uyruk']").attr("disabled", "disabled");
                $("#Uyruk_1[name='DaskMusteri.Sigortali.Uyruk']").attr("checked", "checked");
                $("#Uyruk_1[name='DaskMusteri.Sigortali.Uyruk']").removeAttr("disabled");
            } else {
                $(".sigortali-satir.ozel").slideUp();
                $(".sigortali-satir.tuzel").slideUp();
            }
        },

        this.sigortaEttirenSorgulaDask = function () {
            var isValid = $("#DaskMusteri_SigortaEttiren_KimlikNo").valid();
            if (!isValid) {
                $("#sigortaettiren-kimlikno-mesaj").html("Lütfen kimlik no alanına TCKN, VKN, YKN yada Pasaport No giriniz");
                $("#sigortaettiren-kimlikno-mesaj").show();
                $(".sigortaettiren-satir").slideUp();
                return;
            }
            var kimlikNo = $("#DaskMusteri_SigortaEttiren_KimlikNo").val();
            if (kimlikNo.length < 10) {
                $("#sigortaettiren-kimlikno-mesaj").html("Kimlik numarası uzunluğu TCKN için 11 hane, VKN için 10 hane olmalıdır.");
                $("#sigortaettiren-kimlikno-mesaj").show();
                $(".sigortaettiren-satir").slideUp();
                return;
            }

            $("#sigortaettiren-sorgula").button("loading");
            $("#button-next").prop("disabled", true);
            $('.button-previous').css('pointer-events', 'none');
            $(".sigortaettiren-satir").slideUp();
            sigortaliKontrol.sigortaEttirenTemizleDask();

            var sorgu = { kimlikNo: kimlikNo };
            var tvmKodu = $("#Hazirlayan_TVMKodu").val();
            if (tvmKodu && tvmKodu !== "" && tvmKodu !== "0") {
                sorgu.TVMKodu = tvmKodu;
            }

            $.ajax({
                type: "post",
                dataType: "json",
                url: "/Teklif/Teklif/KimlikNoSorgula",
                data: sorgu,
                success: sigortaliKontrol.sigortaEttirenSuccessDask
            });
        },


        this.sigortaEttirenTemizleDask = function () {
            $("#DaskMusteri_SigortaEttiren_MusteriKodu").val("");
            $("#DaskMusteri_SigortaEttiren_MusteriTipKodu").val("");
            //$("[name='Musteri.SigortaEttiren.Uyruk']").val(1);
            $("#DaskMusteri_SigortaEttiren_AdiUnvan").val("");
            $("#DaskMusteri_SigortaEttiren_SoyadiUnvan").val("");
            $("#DaskMusteri_SigortaEttiren_AdiUnvan").removeAttr('disabled');
            $("#DaskMusteri_SigortaEttiren_SoyadiUnvan").removeAttr('disabled');
            $("#DaskMusteri_SigortaEttiren_IlKodu").val("");
            $("#DaskMusteri_SigortaEttiren_IlceKodu").empty();
            $("#DaskMusteri_SigortaEttiren_IlceKodu").addToDropDown("", "Lütfen Seçiniz");
            $("#DaskMusteri_SigortaEttiren_IlceKodu").val("");
            //$("[name='Musteri.SigortaEttiren.Cinsiyet']").val("E");
            $("#DaskMusteri_SigortaEttiren_DogumTarihi").val("");
            $("#DaskMusteri_SigortaEttiren_CepTelefonu").val("90");
            $("#DaskMusteri_SigortaEttiren_Email").val("");
            $("#DaskMusteri_SigortaEttiren_DogumTarihi").val("");
            $("#DaskMusteri_SigortaEttiren_VergiDairesi").val("");

            $("#form2").find(".control-group.error").removeClass("error");
            $("#form2").find(".field-validation-error").hide();

            $("#DaskMusteri_SigortaEttiren_IlKodu").removeAttr("disabled");
            $("#DaskMusteri_SigortaEttiren_IlceKodu").removeAttr("disabled");
        },

        this.sigortaEttirenSuccessDask = function (data) {
            if (data.SorgulamaSonuc) {
                $("#DaskMusteri_SigortaEttiren_MusteriKodu").val(data.MusteriKodu);
                $("#DaskMusteri_SigortaEttiren_MusteriTipKodu").val(data.MusteriTipKodu);
                $("[name='DaskMusteri.SigortaEttiren.Uyruk'][value=" + data.Uyruk + "]").attr('checked', 'checked');
                $("#DaskMusteri_SigortaEttiren_AdiUnvan").val(data.AdiUnvan);
                $("#DaskMusteri_SigortaEttiren_AdiUnvan").val($("#DaskMusteri_SigortaEttiren_AdiUnvan").val().replace('i', 'İ'));
                $("#DaskMusteri_SigortaEttiren_AdiUnvan").val($("#DaskMusteri_SigortaEttiren_AdiUnvan").val().toUpperCase());
                $("#DaskMusteri_SigortaEttiren_SoyadiUnvan").val(data.SoyadiUnvan);
                $("#DaskMusteri_SigortaEttiren_SoyadiUnvan").val($("#DaskMusteri_SigortaEttiren_SoyadiUnvan").val().replace('i', 'İ'));
                $("#DaskMusteri_SigortaEttiren_SoyadiUnvan").val($("#DaskMusteri_SigortaEttiren_SoyadiUnvan").val().toUpperCase());
                $("#DaskMusteri_TVMKodu").val(data.TVMKodu);
                $("#DaskMusteri_SigortaEttiren_TVMKodu").val(data.TVMKodu);
                $("#DaskMusteri_Sigortali_TVMKodu").val(data.TVMKodu);



                if (data.DisableControls) {
                    $("#DaskMusteri_SigortaEttiren_AdiUnvan").prop('disabled', true);
                    $("#DaskMusteri_SigortaEttiren_SoyadiUnvan").prop('disabled', true);
                    $("#Cinsiyet_E").prop('disabled', true);
                    $("#Cinsiyet_K").prop('disabled', true);
                }

                $("#DaskMusteri_SigortaEttiren_IlKodu").val(data.IlKodu);
                $("#DaskMusteri_SigortaEttiren_IlceKodu").dropDownFill(data.Ilceler);
                $("#DaskMusteri_SigortaEttiren_IlceKodu").val(data.IlceKodu);
                if (data.IlceKodu == 0) {
                    $("#DaskMusteri_SigortaEttiren_IlceKodu").val("");
                }

                $("#DaskMusteri_SigortaEttiren_MusteriTelTipKodu").dropDownFill(data.MusteriTelTipleri);
                if (data.MusteriTelTipKodu != "" && data.MusteriTelTipKodu != null) {
                    $("#DaskMusteri_SigortaEttiren_MusteriTelTipKodu").val(data.MusteriTelTipKodu);
                }
                $("#DaskMusteri_SigortaEttiren_CepTelefonu").val(data.CepTelefonu);

                if (data.Email != null && data.Email != "") {
                    $("#DaskMusteri_SigortaEttiren_Email").val(data.Email);
                }

                $("#DaskMusteri_SigortaEttiren_IlKodu").removeAttr("disabled");
                $("#DaskMusteri_SigortaEttiren_IlceKodu").removeAttr("disabled");

                if (data.DisableControls && data.IlKodu && data.IlKodu.length > 0 && data.IlceKodu && data.IlceKodu != 0 && data.IlceKodu != "undefined") {
                    $("#DaskMusteri_SigortaEttiren_IlKodu").prop('disabled', true);
                    $("#DaskMusteri_SigortaEttiren_IlceKodu").prop('disabled', true);
                }

                //Özel müşteri
                if (data.MusteriTipKodu == 1 || data.MusteriTipKodu == 4) {
                    if (data.Cinsiyet == "E") { $("#Cinsiyet_E").prop("checked", true); }
                    else if (data.Cinsiyet == "K") { $("#Cinsiyet_K").prop("checked", true); }

                    $("#DaskMusteri_SigortaEttiren_DogumTarihi").val(data.DogumTarihiText);
                }
                //Tüzel müşteri
                else if (data.MusteriTipKodu == 2 || data.MusteriTipKodu == 3) {
                    $("#DaskMusteri_SigortaEttiren_VergiDairesi").val(data.VergiDairesi);
                }

                if ($("#Adres_AdresTipi").length == 1) {
                    $("#Adres_AdresTipi").val(data.AdresTipi);
                    $("#Adres_IlKodu").val(data.IlKodu);
                    $("#Adres_IlceKodu").val(data.IlceKodu);
                    $("#Adres_Semt").val(data.Semt);
                    $("#Adres_Mahalle").val(data.Mahalle);
                    $("#Adres_Cadde").val(data.Cadde);
                    $("#Adres_Sokak").val(data.Sokak);
                    $("#Adres_Apartman").val(data.Apartman);
                    $("#Adres_BinaNo").val(data.BinaNo);
                    $("#Adres_DaireNo").val(data.DaireNo);
                    $("#Adres_PostaKodu").val(data.PostaKodu);
                }

                if (data.AcikAdres && data.AcikAdres.length > 0) {
                    $("#DaskMusteri_SigortaEttiren_AcikAdres").val(data.AcikAdres);
                    $("#DaskMusteri_SigortaEttiren_Mahalle").val(data.Mahalle);
                    $("#DaskMusteri_SigortaEttiren_Cadde").val(data.Cadde);
                    $("#DaskMusteri_SigortaEttiren_BinaNo").val(data.BinaNo);
                    $("#DaskMusteri_SigortaEttiren_DaireNo").val(data.DaireNo);
                }

                $("#sigortaettiren-kimlikno-mesaj").hide();
                sigortaliKontrol.sigortaEttirenTipiDask();
            }
            else {
                $("#sigortaettiren-kimlikno-mesaj").html(data.HataMesaj);
                $("#sigortaettiren-kimlikno-mesaj").show();
                $("#sigortali-kimlikno-mesaj").slideDown();

                if (data.DisableManualGiris) {
                    $("#sigortaettiren-sorgula").button("reset");
                    $("#button-next").prop("disabled", false);
                    $('.button-previous').css('pointer-events', 'auto');
                    return;
                }

                $(".sigortaettiren-satir.musteri-tipi").slideDown();
                $("#DaskMusteri_SigortaEttiren_MusteriTipKodu").removeAttr("disabled");

                var kimlikNo = $("#DaskMusteri_SigortaEttiren_KimlikNo").val();
                if (kimlikNo.length == 11) {
                    $("#DaskMusteri_SigortaEttiren_MusteriTipKodu").val("1");
                }
                else {
                    $("#DaskMusteri_SigortaEttiren_MusteriTipKodu").val("2");
                }
                $("#DaskMusteri_SigortaEttiren_MusteriTipKodu").trigger("change");

            }
            $("#button-next").prop("disabled", false);
            $("#sigortaettiren-sorgula").button("reset");
            $('.button-previous').css('pointer-events', 'auto');
        },

        this.KaydetDask = function () {
            var isvalid = FormWizard.validatePage('#tab2');

            if (isvalid) {
                var sigortaEttirenKodu = $("#DaskMusteri_SigortaEttiren_MusteriKodu").val();
                var sigortaliKodu = $("#DaskMusteri_SigortaEttiren_MusteriKodu").val();
                var sigortaliAyni = $("#DaskMusteri_SigortaliAyni_control").bootstrapSwitch('status');

                if (((sigortaEttirenKodu == "" || sigortaEttirenKodu == "0") || ((sigortaliKodu == "" || sigortaliKodu == "0") && !sigortaliAyni)) ||
                    $("#DaskMusteri_SigortaEttiren_TVMKodu").val() != $("#Hazirlayan_TVMKodu").val()) {

                    $("#button-next").button("loading");
                    $(".switcher").find(":input").switchFix();

                    var disabled = $("#tab2").find(':input:disabled').removeAttr('disabled');
                    var disabled2 = $("#tab1").find(':input:disabled').removeAttr('disabled');
                    var data = $("#tab2").find("select, textarea, input").serialize();
                    disabled.attr('disabled', 'disabled');

                    var tvmKodu = $("#Hazirlayan_TVMKodu").val();
                    data.TVMKodu = tvmKodu;
                    data.kimlikNo = $("#DaskMusteri_SigortaEttiren_KimlikNo").val();
                    disabled2.attr('disabled', 'disabled');

                    $.ajax({
                        type: "POST",
                        url: "/Teklif/Teklif/MusteriKaydetDask",
                        data: data,
                        dataType: "json",
                        success: sigortaliKontrol.KaydetSuccessDask,
                        error: sigortaliKontrol.KaydetError,
                        traditional: true
                    });
                    return true;
                }
                else {
                    return true;
                }
            }

            return false;
        },

        this.KaydetSuccessDask = function (data) {
            if (data.SigortaEttiren.SorgulamaSonuc && data.Sigortali.SorgulamaSonuc) {
                $("#DaskMusteri_SigortaEttiren_MusteriKodu").val(data.SigortaEttiren.MusteriKodu);
                $("#DaskMusteri_Sigortali_MusteriKodu").val(data.Sigortali.MusteriKodu);

                $("#button-next").button("reset");
                $('#form_wizard_1').bootstrapWizard("next");
            }
            else if (data.MusteriKodu > 0) {
                $("#DaskMusteri_SigortaEttiren_MusteriKodu").val(data.SigortaEttiren.MusteriKodu);
                $("#DaskMusteri_Sigortali_MusteriKodu").val(data.Sigortali.MusteriKodu);

                $("#button-next").button("reset");
                $('#form_wizard_1').bootstrapWizard("next");
            }
            else if (data.SigortaEttiren.MusteriKodu > 0) {
                $("#DaskMusteri_SigortaEttiren_MusteriKodu").val(data.SigortaEttiren.MusteriKodu);
                $("#DaskMusteri_Sigortali_MusteriKodu").val(data.Sigortali.MusteriKodu);

                $("#button-next").button("reset");
                //$('#form_wizard_1').bootstrapWizard("next");
            }
            else {
                $("#button-next").button("reset");
            }
        },
        this.KaydetError = function () {
            $("#button-next").button("reset");
        }
}
