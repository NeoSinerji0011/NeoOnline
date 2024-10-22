//$(document).ready(function () {
//    $(".datepicker").children(".m-wrap").removeAttr("readonly");
//    $(".datepicker").children(".m-wrap").attr("placeholder", "gg.aa.yyyy");
//});

$("[name='Hazirlayan.KendiAdima']").change(function () {
    var kendiAdima = $(this).val();

    if (kendiAdima == "1") {
        tvmFinder.disable();
        userFinder.disable();
    }
    else {
        tvmFinder.enable();
        userFinder.enable();
    }
});

var tvmFinder = $("#Hazirlayan_TVMKodu").tvmfinder();
var userFinder = $("#Hazirlayan_TVMKullaniciKodu").userfinder("Hazirlayan_TVMKodu");

// ==== Email Gönderme İşlemleri ==== //
$("#email-gonder").live("click", function () {
    var teklifId = $("#TeklifId").val();

    $("#email-gonder").button("loading");

    $.get("/Teklif/Teklif/TeklifEposta",
        { id: teklifId },
        function (data) {
            $("#mail-gonder-modal-div").html(data);
            $.validator.unobtrusive.parse("#mail-gonder-modal-div");
            $("#email-modal").modal('show');
            $("#email-gonder").button("reset");
        },
        "html");
});

$("#mail-gonder-btn").live("click", function () {
    $("#mail-gonder-form").validate().form();

    if ($("#mail-gonder-form").valid()) {
        $("#email-modal").modal('hide');

        $.gritter.add({
            title: 'Bilgi Mesajı!',
            text: 'Mail gönderiliyor. Lütfen bekleyiniz.'
        });
        $(".switcher").find(":input").switchFix();
        var formData = $("#mail-gonder-form").serialize();

        $.ajax({
            type: "POST",
            url: "/Teklif/Teklif/TeklifEPosta",
            data: formData,
            timeout: 30000,
            traditional: true,
            success: function (data) {
                $("#email-gonder").button("reset");
                $("#email-modal").modal('hide');
                if (data.Success) {
                    $.gritter.add({
                        title: 'İşlem Başarılı!',
                        text: data.Message
                    });
                    return;
                }
                else {
                    $.gritter.add({
                        title: 'Bir hata oluştu!',
                        text: 'Mail gönderilemedi, lütfen tekrar deneyin.'
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("#email-gonder").button("reset");
                $("#mail-modal").modal('hide');
                var response = jQuery.parseJSON(jqXHR.responseText);
                $.gritter.add({
                    title: 'Bir hata oluştu!',
                    text: errorThrown
                });
            }
        });
    }
});
// ==== Email Gönderme İşlemleri ==== //

//PDF Görüntüle
$("#teklif-pdf").live("click", function () {
    var teklifId = $("#TeklifId").val();

    $("#teklif-pdf").button("loading");
    $.ajax({
        type: "POST",
        url: "/Teklif/Teklif/TeklifPDF",
        data: { id: teklifId },
        success: function (data) {
            $("#teklif-pdf").button("reset");
            if (data.Success) {
                $(this).attr("pdf", data.PDFUrl);
                window.open(data.PDFUrl, "_blank");
                return;
            }
        },
        error: function () {
            $("#teklif-pdf").button("reset");
            $.gritter.add({ title: 'Hata Mesajı!', text: "Teklif PDF'i getirilirken bir hata oluştu." });
        }
    });
});

$(".upper-letter").blur(function () {
    $(this).val($(this).val().replace('i', 'İ'));
    $(this).val($(this).val().toUpperCase());
});

$(".step").click(function () {
    //    teklifFiyat.teklifTekrar();
});


//Uyruk bilgisi değiştirilirse uyarı veriliyor.
$("#Uyruk_0, #Uyruk_1").change(function () {
    if ($("#Uyruk_1").is(":checked")) {
        $.gritter.add({ title: 'Uyarı Mesajı!', text: "Yabancı uyruklu seçildiğinde vergi kimlik no,vergi dairesi ve pasaport fotokobileri aracı onayı ile alınmalıdır." });
    }
});

$("#sigortaettiren-sorgula").click(function () {
    ortakHelper.sigortaEttirenSorgula();
});



var teklifFiyat = new function () {
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
                    data: { isId: teklifFiyat.processId, guid: teklifFiyat.guid, gosterilenler: teklifFiyat.gosterilenler },
                    dataType: "json",
                    success: teklifFiyat.durumSuccess,
                    error: teklifFiyat.durumError,
                    traditional: true
                });

            this.requestCount++;
        },

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
                    teklifFiyat.showTeklif(index, model.teklifler[teklif], bitir, model.teklifNo);

                    teklifFiyat.gosterilenler[teklifFiyat.gosterilenler.length] = model.teklifler[teklif].TUMKodu;

                    index++;
                }
            }

            if (!model.tamamlandi) {
                teklifFiyat.timeout = setTimeout(teklifFiyat.durumKontrol, 5000);
            }
            else {
                $("#teklif-fiyat-progress").slideUp();
                $("#teklif-button-container").show();
            }
        },

        this.durumError = function () {
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

                for (i = 0; i < uyarilar.length; i++) {
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

            if (fiyat2.length > 0) {
                fiyat2.html(teklif.Fiyat2);
                satinal2.attr("teklif-id", teklif.Fiyat2_TeklifId);
                satinal2.attr("fiyat", teklif.Fiyat2);
            }

            if (fiyat3.length > 0) {
                fiyat3.html(teklif.Fiyat3);
                satinal3.attr("teklif-id", teklif.Fiyat3_TeklifId);
                satinal3.attr("fiyat", teklif.Fiyat3);
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
                    if (hata.length > 250)
                        hata = hata.substring(0, 250);

                    hataHtml = hataHtml + "<span title='" + teklif.Hatalar[i] + "'>" + hata + "</span><br/>";
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
        },

        this.teklifTekrar = function () {
            this.processId = 0;
            this.gosterilenler = [];
            $("#TeklifHazirlandi").val("false");
            $("#button-previous").show();
            $("#button-next").show();
            $("#btn-hesapla").show();
            $("#teklif-no").html("");
            $("#teklif-no-container").hide();
            $('#form_wizard_1').bootstrapWizard("first");
            $("#teklif-button-container").hide();
            $("#teklif-fiyatlar").hide();
            $("#teklif-fiyat-progress").show();
            $("#fiyat-container").html($("#fiyat-container-template").html());
            $("#bilgilendirme-formu").attr("href", "");

            // ==== Para Formatı Belirleniyor.
            $(".autoNumeric").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0 });
        }
}

var ortakHelper = new function () {
    return {
        sigortaEttirenSorgula: function () {

            var kimlikno = $("#Musteri_SigortaEttiren_KimlikNo").val();

            if (kimlikno.length == 11) {

                $("#sigortaettiren-sorgula").button("loading");
                $.ajax({
                    type: "post",
                    dataType: "json",
                    url: "/Teklif/Teklif/KimlikNoSorgula",
                    data: { kimlikno: kimlikno },
                    success: ortakHelper.sigortaEttirenBasarili
                });

            }
        },

        sigortaEttirenBasarili: function (data) {
            if (data.SorgulamaSonuc) {
                $("[name='Musteri.SigortaEttiren.Uyruk']").val(data.Uyruk);
                $("#Musteri_SigortaEttiren_AdiUnvan").val(data.AdiUnvan);
                $("#Musteri_SigortaEttiren_AdiUnvan").val($("#Musteri_SigortaEttiren_AdiUnvan").val().replace('i', 'İ'));
                $("#Musteri_SigortaEttiren_AdiUnvan").val($("#Musteri_SigortaEttiren_AdiUnvan").val().toUpperCase());

                $("#Musteri_SigortaEttiren_SoyadiUnvan").val(data.SoyadiUnvan);
                $("#Musteri_SigortaEttiren_SoyadiUnvan").val($("#Musteri_SigortaEttiren_SoyadiUnvan").val().replace('i', 'İ'));
                $("#Musteri_SigortaEttiren_SoyadiUnvan").val($("#Musteri_SigortaEttiren_SoyadiUnvan").val().toUpperCase());

                $("#Musteri_SigortaEttiren_IlKodu").val(data.IlKodu);
                $("#Musteri_SigortaEttiren_IlceKodu").dropDownFill(data.Ilceler);
                $("#Musteri_SigortaEttiren_IlceKodu").val(data.IlceKodu);
                $("#Musteri_SigortaEttiren_CepTelefonu").val(data.CepTelefonu);
                $("#Musteri_SigortaEttiren_Email").val(data.Email);

                if (data.Cinsiyet == "E") { $("#Cinsiyet_E").prop("checked", true); }
                else if (data.Cinsiyet == "K") { $("#Cinsiyet_K").prop("checked", true); }

                $("#Musteri_SigortaEttiren_DogumTarihi").val(data.DogumTarihiText);

                $("#sigortaettiren-kimlikno-mesaj").hide();

                if (data.GelirVergisiOrani != "" || data.GelirVergisiOrani != null) { $("#Musteri_SigortaEttiren_GelirVergisiOrani").val(data.GelirVergisiOrani); }
            }
            else {
                $("#sigortaettiren-kimlikno-mesaj").html(data.HataMesaj);
                $("#sigortali-kimlikno-mesaj").show();

                $(".sigortaettiren-satir.musteri-tipi").slideDown();
                $("#Musteri_SigortaEttiren_MusteriTipKodu").removeAttr("disabled");
            }

            $("#sigortaettiren-sorgula").button("reset");
        }


    }
}



// ======= ON PROVIZYON ======== //

$("#onprovizyon-al").click(function () {
    onProvizyon.GetPartial();
});

$("#onprovizyon-al-btn").live("click", function () {
    onProvizyon.SendPartial();
});

var onProvizyon = new function () {
    return {

        GetPartial: function () {
            try {

                var teklifId = $("#TeklifId").val();

                if (teklifId !== undefined && teklifId != "") {

                    $("#onprovizyon-al").button("loading");

                    $.ajax({
                        url: "/Teklif/Teklif/AegonOnProvizyon",
                        method: "get",
                        data: { teklifId: teklifId },
                        success: function (result) {
                            if (result !== undefined && result != "") {

                                $("#on-provizyon-modal-div").html(result);
                                $.validator.unobtrusive.parse("#on-provizyon-modal-div")
                                $("#onprovizyon-modal").modal("show");
                                $("#onprovizyon-al").button("reset");
                                $("#basvuru-message").tooltip();
                            }
                            else { $.gritter.add({ title: 'Hata Mesajı!', text: "Bir hata oluştu" }); }
                        },
                        error: function () { $.gritter.add({ title: 'Hata Mesajı!', text: "Bir hata oluştu" }); }
                    });
                }
                else { $.gritter.add({ title: 'Hata Mesajı!', text: "Bir hata oluştu" }); }

            } catch (e) {
                alert(e.message);
            }
        },

        SendPartial: function () {

            if ($("#onprovizyon-form").valid()) {

                var form = $("#onprovizyon-form").serialize();

                $("#onprovizyon-modal").modal("hide");

                $.ajax({
                    url: "/Teklif/Teklif/AegonOnProvizyon",
                    method: "post",
                    data: form,
                    success: function (result) {

                        if (result !== undefined) {
                            if (result.Success) {
                                if (result.Message === undefined) {
                                    result.Message = "";
                                }

                                $("#on-provizyon-button-container").html("<button class='btn btn-success' type='button'>Ön Provizyon Yapıldı " +
                                    result.Message + "</button>");
                                var message = "Ön Provizyon bilginiz başarıyla alındı. Onay Kodu = " + result.Message;
                                $.gritter.add({ title: 'Bilgi Mesajı!', text: message });
                            }
                            else {
                                $("#on-provizyon-modal-div").html(result.html);
                                $.validator.unobtrusive.parse("#on-provizyon-modal-div")
                                $("#onprovizyon-modal").modal("show");
                                $("#basvuru-message").tooltip();
                                $.gritter.add({ title: 'Hata Mesajı!', text: result.Message });
                            }
                        }
                        else { $.gritter.add({ title: 'Hata Mesajı!', text: "Bir hata oluştu" }); }
                    },
                    error: function () { $.gritter.add({ title: 'Hata Mesajı!', text: "Bir hata oluştu" }); }
                });
            }
        }
    }
}

// ======= ON PROVIZYON ======== //