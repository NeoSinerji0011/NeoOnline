function odemeGuvenceTeklifWizardCallback(current) {
    //Hazırlayan bilgileri
    if (current == 1) {

    }
        //Sigorta ettiren / sigortali tab
    else if (current == 2) {
        var isValid = FormWizard.validatePage('#tab2');

        if (isValid == 1) {
            return OdemeGuvence.musteriBilgileriKontrol()
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

var OdemeGuvence = new function () {
    return {
        TekrarTeklifAcilisKontrol: function () {

            OdemeGuvence.musteriZorunlulukKaldir();
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
            //YAŞ
            var today = new Date();
            var dogum = $("#Musteri_SigortaEttiren_DogumTarihi").datepicker("getDate");
            today.setYear(today.getFullYear() - 18);
            if (dogum > today)
                Message += "<p> --Sigortalı yaşı 18 den küçük olamaz.</p>";

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

            Message += OdemeGuvence.EkTeminatlarKontrol();
            Message += OdemeGuvence.TeminatSureKontrol();

            if (Message == "")
                return true;
            else {
                $("#hata-message-div").html(Message);
                $("#hata-modal").modal("show");
                return false;
            }
        },

        EkTeminatlarKontrol: function () {
            var Message = "";

            if ($("#GenelBilgiler_IssizlikDPM_control").bootstrapSwitch("status")) {
                if (!$("#GenelBilgiler_KritikHastalikDPM_control").bootstrapSwitch("status")) {
                    Message = "<p>İşsizlik durumunda prim muafiyeti teminatın alınabilmesi için Kritik Hastalıklar Durumunda Prim Muafiyeti Teminatı’nın " +
                              "alınması zorunludur.</p>";
                }
            }

            if ($("#GenelBilgiler_KazaSonucuHastanedeyatarakTDPM_control").bootstrapSwitch("status")) {
                if (!$("#GenelBilgiler_KritikHastalikDPM_control").bootstrapSwitch("status")) {
                    Message += "<p>Kaza Sonucu Hastanede Yatarak Tedavi Durumunda Prim Muafiyeti Teminatı alınabilmesi için Kritik Hastalıklar Durumunda" +
                               " Prim Muafiyeti Teminatı’nın alınması zorunludur.</p>";
                }
            }

            return Message;
        },

        musteriZorunlulukKaldir: function () {
            $("#Musteri_SigortaEttiren_CepTelefonu").addClass("ignore");
            $("#Musteri_SigortaEttiren_Email").addClass("ignore");
            $("#Musteri_SigortaEttiren_IlceKodu").addClass("ignore");
            $("#Musteri_SigortaEttiren_IlKodu").addClass("ignore");
            $("#Musteri_SigortaEttiren_KimlikNo").addClass("ignore");
        },

        PrimDetayPartialGetir: function (IsDurum_id) {
            $.ajax({
                url: "/Teklif/OdemeGuvence/_DetayPartial",
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

        TeminatSureKontrol: function () {
            var Message = "";

            var DogTar = $("#Musteri_SigortaEttiren_DogumTarihi").val();
            var SigBas = $("#GenelBilgiler_SigortaBaslangicTarihi").val();
            var SigSure = $("#OdemeGuvenceKAP_sigortaSuresi").val();


            var KritikHastalik = $("#GenelBilgiler_KritikHastalikDPM_control").bootstrapSwitch("status");
            var Maluliyet = $("#GenelBilgiler_TamVeTaimiMaluliyetDPM_control").bootstrapSwitch("status");
            var Issizlik = $("#GenelBilgiler_IssizlikDPM_control").bootstrapSwitch("status");
            var Hastane = $("#GenelBilgiler_KazaSonucuHastanedeyatarakTDPM_control").bootstrapSwitch("status");

            $.ajax({
                url: "/Teklif/OdemeGuvence/SigortaliYasHesapla",
                method: "post",
                async: false,
                data: {
                    DogTar: DogTar,
                    SigBas: SigBas,
                    KritikHastalik: KritikHastalik,
                    Maluliyet: Maluliyet,
                    Issizlik: Issizlik,
                    Hastane: Hastane,
                    SigSure: SigSure
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

        TekrarTeklif: function (tip) {

            $("#btn-teklif-tekrar").trigger("click");

            setTimeout(function () {
                $(".button-next").trigger("click");
                console.log("button-next trigger");


                setTimeout(function () {
                    $(".button-next").trigger("click");
                    console.log("button-next trigger");


                    setTimeout(function () {
                        $("html, body").animate({ scrollTop: $(document).height() }, "slow");

                        switch (tip) {

                            case 1:
                                {
                                    if ($("#GenelBilgiler_IssizlikDPM_control").bootstrapSwitch("status") == false) {
                                        $("#GenelBilgiler_IssizlikDPM_control").bootstrapSwitch("setState", true);
                                        $("#GenelBilgiler_IssizlikDPM_FaydalanabilecekDurumdami").val("True");

                                        console.log("btn-hesapla trigger");
                                        $("#btn-hesapla").trigger("click");
                                    }
                                } break;
                            case 2:
                                {
                                    $("#GenelBilgiler_IssizlikDPM_FaydalanabilecekDurumdami").val("False");

                                    console.log("btn-hesapla trigger");
                                    $("#btn-hesapla").trigger("click");
                                } break;
                            case 3:
                                alert("Lütfen başka ek teminat ekleyiniz ve 'Teklif Al' butonuna basınız.");
                                break;
                        }

                    }, 500);
                }, 500);
            }, 500);

        }
    }
}

var TeklifKontrol = new function () {
    this.gosterilenler = [];
    this.requestCount = 0;
    this.processId = 0;
    this.guid = '';
    this.timeout;

    this.kontol = function (options) {
        this.processId = options.processId;
        this.guid = options.guid;
        this.gosterilenler = [];
        this.timeout = setTimeout(this.durumKontrol, 5000);
    },

    this.durumKontrol = function () {
        $.ajax(
        {
            type: "POST",
            url: "/Teklif/Teklif/TeklifDurumu",
            data: { isId: TeklifKontrol.processId, guid: TeklifKontrol.guid, gosterilenler: TeklifKontrol.gosterilenler },
            dataType: "json",
            success: TeklifKontrol.durumSuccess,
            error: TeklifKontrol.durumError,
            traditional: true
        });

        this.requestCount++;
    }

    this.durumSuccess = function (data) {
        var model = data.model;

        $("#TeklifId").val(model.teklifId);
        if (model.teklifler.length > 0) {
            var index = teklifFiyat.gosterilenler.length;

            for (var teklif in model.teklifler) {
                var bitir = false;
                if (model.tamamlandi && (parseInt(teklif) + 1) == model.teklifler.length) {
                    bitir = true;
                }
                TeklifKontrol.showTeklif(index, model.teklifler[teklif], bitir, model.teklifNo);

                TeklifKontrol.gosterilenler[TeklifKontrol.gosterilenler.length] = model.teklifler[teklif].TUMKodu;
                index++;
            }
        }

        if (!model.tamamlandi) {
            TeklifKontrol.timeout = setTimeout(TeklifKontrol.durumKontrol, 5000);
        }
        else {
            $("#teklif-fiyat-progress").slideUp();
            $("#teklif-button-container").show();
        }
    },

    this.showTeklif = function (index, teklif, tamamlandi, teklifNo) {
        var container = $("#fiyat-container-" + index);
        var tum = $("#tum-unvan-" + index);
        var tetkik = $("#tetkik-" + index);
        var uyari = $("#uyari-" + index);
        var hatalar = $("#hata-div-" + index);
        var divFiyat1 = $("#div-fiyat-1-" + index);
        var divFiyat2 = $("#div-fiyat-2-" + index);
        var divFiyat3 = $("#div-fiyat-3-" + index);
        var fiyat1 = $("#tum-fiyat-1-" + index);
        var fiyat2 = $("#tum-fiyat-2-" + index);
        var fiyat3 = $("#tum-fiyat-3-" + index);
        var satinal1 = $("#tum-satial-1-" + index);
        var satinal2 = $("#tum-satial-2-" + index);
        var satinal3 = $("#tum-satial-3-" + index);


        var containerValue = container.attr("tum-kodu");
        if (containerValue !== undefined)
            return;

        container.attr("tum-kodu", teklif.TUMKodu);
        tum.attr({
            "alt": teklif.TUMUnvani,
            "title": teklif.TUMUnvani,
            "src": teklif.TUMLogoUrl
        });


        if (teklif.Surprimler != null && teklif.Surprimler.length > 0) {
            var html = "<div class='span12' style='margin-top:10px;'>";

            html = html + "<div class='span3' style='text-align: center; color: red; font-style: italic;'>Yapılması Gereken Tetkikler </div><div class='span9'>";

            for (var i in teklif.Surprimler) {
                var surp = teklif.Surprimler[i];
                html = html + "<p>" + surp.Surprim + "</p>";
            }

            html = html + "</div></div>";

            tetkik.html(html);
        }

        //Uyarılar buraya ekleniyor.
        if (teklif.Hasarsizlik != null && teklif.Hasarsizlik != "") {

            var html = "<div class='span12' style='margin-top:10px;'>";

            html = html + "<div class='span3' style='text-align: center; color: red; font-style: italic;'>Uyarılar </div><div class='span9'><p>";

            var uyarilar = teklif.Hasarsizlik.split('|');

            for (i = 0 ; i < uyarilar.length; i++) {
                html = html + " <span style='color: red;'> -- </span> " + uyarilar[i] + "<br/>";
            }

            html = html + "</p></div></div>";

            uyari.html(html);
        }


        if (fiyat1.length > 0) {
            fiyat1.html(teklif.Fiyat1);
            satinal1.attr("teklif-id", teklif.Fiyat1_TeklifId);
            satinal1.attr("fiyat", teklif.Fiyat1);
        }


        if (teklif.Hatalar != null && teklif.Hatalar.length > 0) {
            divFiyat1.removeClass("span3").hide();
            divFiyat2.removeClass("span3").hide();
            divFiyat3.removeClass("span3").hide();
            hatalar.attr("class", "span10");
            hatalar.addClass("alert alert-error");
            hatalar.css({ "text-align": "left", "margin-bottom": "0px" });

            var hataHtml = "<strong>Teklif hazırlanamadı</strong><br/>";
            for (var i in teklif.Hatalar) {
                var hata = teklif.Hatalar[i];
                //if (hata.length > 250)
                //    hata = hata.substring(0, 250);
                if (hata != undefined && hata != "" && hata != "\r\n") {
                    hataHtml = hataHtml + "<span >" + hata + "</span><br/>";
                }
            }

            hatalar.html(hataHtml);

            $("#teklif-pdf").hide();
            $("#email-gonder").hide();
            $("#onprovizyon-al").hide();
        }
        else {
            $("#teklif-pdf").show();
            $("#email-gonder").show();
            $("#onprovizyon-al").show();
        }

        $("#teklif-fiyatlar").slideDown("fast", function () {
            container.css({ "width": "0" });
            container.show();
            container.animate({ "width": "100%" }, "slow", "linear", function () {
                if (tamamlandi) {
                    $("#teklif-fiyat-progress").slideUp("fast");
                    $("#teklif-no").html(teklifNo);
                    $("#teklif-no-container").show();
                    $("#TeklifHazirlandi").val("true");
                    $("#button-previous").hide();
                    $("#button-next").hide();
                    $("#btn-hesapla").hide();
                }
            });
        });
    }
}


$(document).ready(function () {

    OdemeGuvence.TekrarTeklifAcilisKontrol();

    //Muşteri adres
    $("#Musteri_SigortaEttiren_UlkeKodu").ulke({ il: "#Musteri_SigortaEttiren_IlKodu", ilce: "#Musteri_SigortaEttiren_IlceKodu" });

    //Doğum Tarihi Sınırlaması
    $("#Musteri_SigortaEttiren_DogumTarihi").datepicker("option", "maxDate", '-216m +0w');
    $("#Musteri_SigortaEttiren_DogumTarihi").datepicker("option", "minDate", '-852m +0w');

    //Sigorta Başlangıç tarihi
    $("#GenelBilgiler_SigortaBaslangicTarihi").datepicker("option", "minDate", '+0m +0w');

    // ==== Para Formatı Belirleniyor.
    $(".autoNumeric").autoNumeric('init', { vMin: '0', vMax: '999999999', mDec: 0 });
});

$("#btn-hesapla").click(function () {

    var isvalid = $("#form1").valid();
    var form3 = $("#form3").valid();

    if (isvalid && form3) {
        $(this).button("loading");

        if (OdemeGuvence.SigortaBilgileriKontrol()) {
            $(this).button("loading");

            $(".detay-partial-div").html('');
            $(".switcher").find(":input").switchFix();
            $(".autoNumeric").each(function () {
                $(this).val($(this).autoNumeric('get'));
            });
            $(".autoNumeric-custom").each(function () {
                $(this).val($(this).autoNumeric('get'));
            });

            var contents = $("#form1, #form2, #form3").serialize();

            $.ajax(
                {
                    type: "POST",
                    url: "/Teklif/OdemeGuvence/Hesapla",
                    data: contents,
                    success: function (data) {
                        if (data.id > 0) {
                            TeklifKontrol.kontol({ processId: data.id, guid: data.g });

                            OdemeGuvence.PrimDetayPartialGetir(data.id);

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

$("#OdemeGuvenceKAP_paraBirimi").change(function () {
    switch ($(this).val()) {
        case "1": $("#para-birimi").html("EUR"); break;
        case "2": $("#para-birimi").html("USD"); break;
        case "3": $("#para-birimi").html("TL"); break;
    }
});

$("#prim-muafiyeti-evet").live("click", function () {

    OdemeGuvence.TekrarTeklif(1);

    //$("#btn-teklif-tekrar").trigger("click");

    //setTimeout(function () {
    //    $(".button-next").trigger("click");
    //    console.log("button-next trigger");

    //    setTimeout(function () {
    //        $(".button-next").trigger("click");
    //        console.log("button-next trigger");

    //        setTimeout(function () {
    //            if ($("#GenelBilgiler_IssizlikDPM_control").bootstrapSwitch("status") == false) {
    //                $("#GenelBilgiler_IssizlikDPM_control").bootstrapSwitch("setState", true);

    //                $("html, body").animate({ scrollTop: $(document).height() }, "slow");

    //                $("#btn-hesapla").trigger("click");
    //                console.log("btn-hesapla trigger");
    //            }
    //        }, 500);
    //    }, 500);
    //}, 500);

});

$("#prim-muafiyeti-hayir").live("click", function () {
    OdemeGuvence.TekrarTeklif(2);
});

$("#3AdimaGit").live("click", function () {
    OdemeGuvence.TekrarTeklif(3);
});