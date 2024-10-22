$(document).ready(function () {
    console.log("asdf");
    $("[rel=tooltip]").tooltip();

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

    $("#pdf-karsilastirma").live("click", function () {
        var teklifId = $("#TeklifId").val();
        //var contents = $("#formTeklif").serialize();
        $("#pdf-karsilastirma").button("loading");
        $.ajax({
            type: "POST",
            url: "/Teklif/Teklif/TeklifPDF",
            data: { id: teklifId },
            success: function (data) {
                $("#pdf-karsilastirma").button("reset");
                if (data.Success) {
                    $(this).attr("pdf", data.PDFUrl);
                    window.open(data.PDFUrl, "_blank");
                    return;
                }
            },
            error: function () {
                $("#pdf-karsilastirma").button("reset");
            }
        });
    });

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
            var tumUyari = $("#tum-uyari-" + index);
            var tumBilgi = $("#tum-bilgi-" + index);
            var hasarsizlik = $("#div-hasarsizlik-" + index);
            var surprim = $("#div-surprim-" + index);
            var divFiyat1 = $("#div-fiyat-1-" + index);
            var divFiyat2 = $("#div-fiyat-2-" + index);
            var divFiyat3 = $("#div-fiyat-3-" + index);
            var fiyat1 = $("#tum-fiyat-1-" + index);
            var fiyat2 = $("#tum-fiyat-2-" + index);
            var fiyat3 = $("#tum-fiyat-3-" + index);
            var satinal1 = $("#tum-satial-1-" + index);
            var satinal2 = $("#tum-satial-2-" + index);
            var satinal3 = $("#tum-satial-3-" + index);

            var tumTeklifNo = $("#tum-teklifno-" + index);
            var komisyonTutari = $("#div-komisyon-tutari-" + index);
            var komisyonOrani = $("#div-komisyon-orani-" + index);

            var tumPDF = $("#tum-teklifPDF-" + index);
            var daskUyari = $("#div-daskUyari-" + index);

            var containerValue = container.attr("tum-kodu");
            if (containerValue !== undefined)
                return;

            container.attr("tum-kodu", teklif.TUMKodu);
            tum.attr({
                "alt": teklif.TUMUnvani,
                "title": teklif.TUMUnvani,
                "src": teklif.TUMLogoUrl
            });
            if (teklif.TUMTeklifUyariMesaji != null && teklif.TUMTeklifUyariMesaji != "") {
                tumUyari.attr({
                    "alt": teklif.TUMTeklifUyariMesaji,
                    "title": "Uyarı Mesajı - " + teklif.TUMTeklifUyariMesaji
                });
                tumUyari.css("display", "normal");
            }
            else {
                tumUyari.css("display", "none");
            }
            if (teklif.TUMTeklifBilgiMesaji != null && teklif.TUMTeklifBilgiMesaji != "") {
                tumBilgi.attr({
                    "alt": teklif.TUMTeklifBilgiMesaji,
                    "title": "Bilgi Mesajı - " + teklif.TUMTeklifBilgiMesaji
                });
                tumBilgi.css("display", "normal");
            }
            else {
                tumBilgi.css("display", "none");
            }
            if (teklif.KomisyonTutari != null && teklif.KomisyonTutari.length > 0) {
                var html = "";
            }

            if (teklif.Hasarsizlik != null && teklif.Hasarsizlik.length > 0) {
                var html = "";

                if (teklif.HasarIndirimSurprim == "I") {
                    html = "<span class='label label-success' style='margin-top:25px'>" + teklif.Hasarsizlik + "</span>";
                }
                else if (teklif.HasarIndirimSurprim == "S") {
                    html = "<span class='label label-important' style='margin-top:25px'>" + teklif.Hasarsizlik + "</span>";
                }
                else {
                    html = "<span class='label label-info' style='margin-top:25px'>%0</span>";
                }

                hasarsizlik.html(html);
            }

            if (teklif.Surprimler != null && teklif.Surprimler.length > 0) {
                var html = "<div style='display: table-cell; vertical-align: middle;height:65px;'>";

                for (var i in teklif.Surprimler) {
                    var surp = teklif.Surprimler[i];
                    html = html + "<div class='row' style='width:200px;margin-left: auto; margin-right: auto;'>"
                    html = html + "<div class='span10' style='text-align:center;'>" + surp.SurprimAciklama + "</div>"

                    if (surp.SurprimIS == "I") {
                        html = html + "<div class='span2' style='text-align:center;'><span class='label label-success'>" + surp.Surprim + "</span></div>"
                    }
                    else if (surp.SurprimIS == "S") {
                        html = html + "<div class='span2' style='text-align:center;'><span class='label label-important'>" + surp.Surprim + "</span></div>"
                    }
                    else {
                        html = html + "<div class='span2' style='text-align:center;'><span class='label label-info'>%0</span></div>"
                    }

                    html = html + "</div>";
                }

                html = html + "</div>";

                surprim.html(html);
            }

            if (teklif.merkezAcenteMi) {
                $(".merkezacente").show();
            }
            else {
                $(".merkezacente").hide();
            }
            if (komisyonTutari.length > 0) {
                komisyonTutari.html(teklif.KomisyonTutari);
            }
            if (komisyonOrani.length > 0) {
                komisyonOrani.html(teklif.KomisyonOrani);
            }

            if (tumTeklifNo.length > 0) {
                tumTeklifNo.html(teklif.TUMTeklifNo);
            }

            if (fiyat1.length > 0) {
                fiyat1.html(teklif.Fiyat1);
                satinal1.attr("teklif-id", teklif.Fiyat1_TeklifId);
                satinal1.attr("fiyat", teklif.Fiyat1);

                var bilgilendirmeFormu = $("#bilgilendirme-formu-" + index);
                if (bilgilendirmeFormu.length > 0) {
                    var adres = bilgilendirmeFormu.attr("data-ref");
                    bilgilendirmeFormu.attr("href", adres + teklif.Fiyat1_TeklifId);
                }
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

            if (teklif.TUMPDF != null && teklif.TUMPDF != "") {
                tumPDF.html(teklif.TUMPDF);
            }

            if (teklif.Hatalar != null && teklif.Hatalar.length > 0) {
                hasarsizlik.removeClass("span1");
                surprim.removeClass("span2").hide();
                divFiyat1.removeClass("span2").hide();
                divFiyat2.removeClass("span2").hide();
                divFiyat3.removeClass("span2").hide();
                tumTeklifNo.removeClass("span2").hide();
                komisyonTutari.removeClass("span1").hide();
                komisyonOrani.removeClass("span2").hide();
                hasarsizlik.addClass("span10");
                hasarsizlik.addClass("alert alert-error");
                hasarsizlik.css({ "text-align": "left", "margin-bottom": "0px" });

                if (teklif.DaskUyariMesaji != "") {
                    daskUyari.html(teklif.TUMPDF);
                    daskUyari.show();
                }
                var urunAdi = $("#UrunAdi").val();
                if (urunAdi == "TSS") {

                    divFiyat1.removeClass("span3").hide();
                    divFiyat2.removeClass("span3").hide();
                    divFiyat3.removeClass("span3").hide();
                    tumTeklifNo.removeClass("span3").hide();
                    tumPDF.removeClass("span3").hide();

                    tumPDF.addClass("span9");
                    tumPDF.addClass("alert alert-error");
                    tumPDF.css({ "text-align": "left", "margin-bottom": "0px" });
                    var hataHtml = "<strong>Teklif hazırlanamadı</strong><br/>";
                    for (var i in teklif.Hatalar) {
                        var hata = teklif.Hatalar[i];
                        //if (hata.length > 150)
                        //    hata = hata.substring(0, 150);

                        hataHtml = hataHtml + "<span title='" + teklif.Hatalar[i] + "'>" + hata + "</span><br/>";
                    }

                    tumPDF.html(hataHtml);
                    tumPDF.show();
                }

                var hataHtml = "<strong>Teklif hazırlanamadı</strong><br/>";
                for (var i in teklif.Hatalar) {
                    var hata = teklif.Hatalar[i];
                    //if (hata.length > 150)
                    //    hata = hata.substring(0, 150);

                    hataHtml = hataHtml + "<span title='" + teklif.Hatalar[i] + "'>" + hata + "</span><br/>";
                }

                hasarsizlik.html(hataHtml);
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
            $("#btn-hesapla").button("reset");
            $("#teklif-no").html("");
            $("#teklif-no-container").hide();
            $('#form_wizard_1').bootstrapWizard("first");
            $("#teklif-button-container").hide();
            $("#teklif-fiyatlar").hide();
            $("#teklif-fiyat-progress").show();
            $("#fiyat-container").html($("#fiyat-container-template").html());
            $("#bilgilendirme-formu").attr("href", "");
        }
}

var sigortaliKontrol = new function () {

    this.init = function () {

        $("#Musteri_SigortaliAyni_control").on("switch-change", sigortaliKontrol.sigortaliFarkli);
        $("#Musteri_SigortaEttiren_MusteriTipKodu").change(sigortaliKontrol.sigortaEttirenTipi);
        $("#Musteri_Sigortali_MusteriTipKodu").change(sigortaliKontrol.sigortaliTipi);
        $("#sigortaettiren-sorgula").click(sigortaliKontrol.sigortaEttirenSorgula);
        $("#sigortali-sorgula").click(sigortaliKontrol.sigortaliSorgula);

        $("#Musteri_SigortaEttiren_UlkeKodu").ulke({ il: "#Musteri_SigortaEttiren_IlKodu", ilce: "#Musteri_SigortaEttiren_IlceKodu" });
        $("#Musteri_Sigortali_UlkeKodu").ulke({ il: "#Musteri_Sigortali_IlKodu", ilce: "#Musteri_Sigortali_IlceKodu" });

        //// ==== Uyruk ve cinsiyet hatalı geliyor düzeltiliyor. ==== //
        //$("#Uyruk_0").val("0");
        //$("#Cinsiyet_K").val("K");

        var musteriKodu = $("#Musteri_SigortaEttiren_MusteriKodu").val();
        if (musteriKodu != "") {
            $(".sigortaettiren-satir.musteri-tipi").show();
            $("#Musteri_SigortaEttiren_MusteriTipKodu").attr("disabled", "disabled");
            $("#sigortaettiren-kimlikno-mesaj").hide();
            sigortaliKontrol.sigortaEttirenTipi();
        }
        var sigortaliAyni = $("#Musteri_SigortaliAyni_control").bootstrapSwitch('status');
        if (sigortaliAyni) {
            $(".sigortali-satir :input").addClass("ignore");
            $("#Musteri_Sigortali_MusteriKodu").addClass("ignore");
            $("#Musteri_Sigortali_KimlikNo").addClass("ignore");
            $("#sigortali").show();
        } else {
            musteriKodu = $("#Musteri_Sigortali_MusteriKodu").val();
            $("#sigortali").hide();
            if (musteriKodu != "") {
                $(".sigortali-kimlikno").show();
                $("#Musteri_Sigortali_MusteriKodu").removeClass("ignore");
                $("#Musteri_Sigortali_KimlikNo").removeClass("ignore");
                $(".sigortali-satir.musteri-tipi").show();
                $("#Musteri_Sigortali_MusteriTipKodu").attr("disabled", "disabled");
                $("#sigortali-kimlikno-mesaj").hide();

                sigortaliKontrol.sigortaliTipi();
            }
        }
    },

        this.sigortaliFarkli = function (e, data) {
            if (data.value) {
                $(".sigortali-kimlikno").slideUp("fast");
                $("#Musteri_Sigortali_MusteriKodu").addClass("ignore");
                $("#Musteri_Sigortali_KimlikNo").addClass("ignore");
                $(".sigortali-satir.ozel").slideUp();
                $(".sigortali-satir.tuzel").slideUp();
                $(".sigortali-satir.tuzel :input").addClass("ignore");
                $(".sigortali-satir.ozel :input").addClass("ignore");
                $("#sigortali").show();
            } else {
                $(".sigortali-kimlikno").slideDown("fast");
                $("#Musteri_Sigortali_MusteriKodu").removeClass("ignore");
                $("#Musteri_Sigortali_KimlikNo").removeClass("ignore");
                $(".sigortali-satir.tuzel :input").removeClass("ignore");
                $(".sigortali-satir.ozel :input").removeClass("ignore");
                $("#sigortali").hide();
            }
        },

        this.sigortaEttirenTipi = function () {
            var musteriTipi = $("#Musteri_SigortaEttiren_MusteriTipKodu").val();
            if (musteriTipi == "1") { //Şahıs
                $(".sigortaettiren-satir.tuzel").slideUp();
                $(".sigortaettiren-satir.ozel").slideDown();
                $(".sigortaettiren-satir.tuzel :input").addClass("ignore");
                $(".sigortaettiren-satir.ozel :input").removeClass("ignore");

                $("#Uyruk_1[name='Musteri.SigortaEttiren.Uyruk']").attr("disabled", "disabled");
                $("#Uyruk_0[name='Musteri.SigortaEttiren.Uyruk']").attr("checked", "checked");
                $("#Uyruk_0[name='Musteri.SigortaEttiren.Uyruk']").removeAttr("disabled");

            } else if (musteriTipi == "2") { //Tüzel
                $(".sigortaettiren-satir.ozel").slideUp();
                $(".sigortaettiren-satir.tuzel").slideDown();
                $(".sigortaettiren-satir.ozel :input").addClass("ignore");
                $(".sigortaettiren-satir.tuzel :input").removeClass("ignore");
            } else if (musteriTipi == "3") { //Şahıs firması
                $(".sigortaettiren-satir.ozel").slideUp();
                $(".sigortaettiren-satir.tuzel").slideDown();
                $(".sigortaettiren-satir.ozel :input").addClass("ignore");
                $(".sigortaettiren-satir.tuzel :input").removeClass("ignore");
            } else if (musteriTipi == "4") { //Yabancı
                $(".sigortaettiren-satir.tuzel").slideUp();
                $(".sigortaettiren-satir.ozel.yabanci").slideDown();
                $(".sigortaettiren-satir.tuzel :input").addClass("ignore");
                $(".sigortaettiren-satir.ozel.yabanci :input").removeClass("ignore");

                $(".sigortaettiren-satir.ozel").slideDown();
                $(".sigortaettiren-satir.ozel :input").removeClass("ignore");

                $("#Uyruk_0[name='Musteri.SigortaEttiren.Uyruk']").attr("disabled", "disabled");
                $("#Uyruk_1[name='Musteri.SigortaEttiren.Uyruk']").attr("checked", "checked");
                $("#Uyruk_1[name='Musteri.SigortaEttiren.Uyruk']").removeAttr("disabled");
            } else {
                $(".sigortaettiren-satir.ozel").slideUp();
                $(".sigortaettiren-satir.tuzel").slideUp();
            }
            var urunAdi = $("#UrunAdi").val();
            if (urunAdi == "dask") {
                $(".sigortaettiren-satir.tuzel :input").addClass("ignore");
                $(".sigortaettiren-satir.ozel :input").addClass("ignore");
                $("#Musteri_SigortaEttiren_CepTelefonu").removeClass("ignore");
                $(".required").html();

            }
            else {
                $(".required").html("*");
            }
        },


        this.sigortaliTipi = function () {
            var musteriTipi = $("#Musteri_Sigortali_MusteriTipKodu").val();

            if (musteriTipi == "1") { //Şahıs
                $(".sigortali-satir.tuzel").slideUp();
                $(".sigortali-satir.ozel").slideDown();
                $(".sigortali-satir.tuzel :input").addClass("ignore");
                $(".sigortali-satir.ozel :input").removeClass("ignore");
                $("#Musteri_Sigortali_CepTelefonu").removeClass("ignore");
                $("#Uyruk_1[name='Musteri.Sigortali.Uyruk']").attr("disabled", "disabled");
                $("#Uyruk_0[name='Musteri.Sigortali.Uyruk']").attr("checked", "checked");
                $("#Uyruk_0[name='Musteri.Sigortali.Uyruk']").removeAttr("disabled");
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

                $("#Uyruk_0[name='Musteri.Sigortali.Uyruk']").attr("disabled", "disabled");
                $("#Uyruk_1[name='Musteri.Sigortali.Uyruk']").attr("checked", "checked");
                $("#Uyruk_1[name='Musteri.Sigortali.Uyruk']").removeAttr("disabled");
            } else {
                $(".sigortali-satir.ozel").slideUp();
                $(".sigortali-satir.tuzel").slideUp();
            }
        },

        this.sigortaEttirenSorgula = function () {
        debugger
        console.log("asd");
            var isValid = $("#Musteri_SigortaEttiren_KimlikNo").valid();
            if (!isValid) {
                $("#sigortaettiren-kimlikno-mesaj").html("Lütfen kimlik no alanına TCKN, VKN, YKN yada Pasaport No giriniz");
                $("#sigortaettiren-kimlikno-mesaj").show();
                $(".sigortaettiren-satir").slideUp();
                return;
            }
            var kimlikNo = $("#Musteri_SigortaEttiren_KimlikNo").val();
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
            sigortaliKontrol.sigortaEttirenTemizle();

            var sorgu = { kimlikNo: kimlikNo };
            var tvmKodu = $("#Hazirlayan_TVMKodu").val();
            if (tvmKodu && tvmKodu !== "" && tvmKodu !== "0") {
                sorgu.TVMKodu = tvmKodu;
            }

            var urunAdi = $("#UrunAdi").val();
            var Gulf = false;
            if (urunAdi == "TSS") {
                $.ajax({
                    type: "post",
                    dataType: "json",
                    url: "/Teklif/Teklif/TSSNipponKimlikSorgula",
                    data: sorgu,
                    success: sigortaliKontrol.sigortaEttirenSuccess
                });
            }
            else {
                var kimlikSorgulandi = false;
                var divSayisi = 0;
                $("html").find(".tum-no").each(function (i) {
                    divSayisi = i + 1;
                });
                for (var i = 0; i < divSayisi; i++) {
                    //Anadolu sigorta şirketinin durumu okunuyor
                    var tumValue = $("#TeklifUM_" + i + "__TUMKodu").val();
                    if (tumValue == 6) {
                        Gulf = $("#TeklifUM_" + i + "__TeklifAl_control").bootstrapSwitch('status');
                        if (Gulf) {
                            $.ajax({
                                type: "post",
                                dataType: "json",
                                url: "/Teklif/Teklif/KimlikNoSorgulaGulf",
                                data: sorgu,
                                success: sigortaliKontrol.sigortaEttirenSuccess

                            });
                            break;
                        }
                    }
                }
                if (Gulf == false) {
                    $.ajax({
                        type: "post",
                        dataType: "json",
                        url: "/Teklif/Teklif/KimlikNoSorgula",
                        data: sorgu,
                        success: sigortaliKontrol.sigortaEttirenSuccess
                    });
                }
            }
            if (tvmKodu == 100) {
                $("#urun-Listele").show();
            }
            else {
                $("#urun-Listele").hide();
            }
        },

        this.sigortaliSorgula = function () {
        debugger
            var isValid = $("#Musteri_Sigortali_KimlikNo").valid();
            if (!isValid) {
                $("#sigortali-kimlikno-mesaj").html("Lütfen kimlik no alanına TCKN, VKN, YKN yada Pasaport No giriniz");
                $("#sigortali-kimlikno-mesaj").show();
                $(".sigortali-satir").slideUp();
                return;
            }
            var kimlikNo = $("#Musteri_Sigortali_KimlikNo").val();
            if (kimlikNo.length < 10) {
                $("#sigortali-kimlikno-mesaj").html("Kimlik numarası uzunluğu TCKN için 11 hane, VKN için 10 hane olmalıdır.");
                $("#sigortali-kimlikno-mesaj").show();
                $(".sigortali-satir").slideUp();
                return;
            }

            $("#button-next").prop("disabled", true);
            $('.button-previous').css('pointer-events', 'none');
            $("#sigortali-sorgula").button("loading");
            $(".sigortali-satir").slideUp();
            sigortaliKontrol.sigortaliTemizle();

            var sorgu = { kimlikNo: kimlikNo };
            var tvmKodu = $("#Hazirlayan_TVMKodu").val();
            if (tvmKodu && tvmKodu !== "" && tvmKodu !== "0") {
                sorgu.TVMKodu = tvmKodu;
            }

            //$.ajax({
            //    type: "post",
            //    dataType: "json",
            //    url: "/Teklif/Teklif/KimlikNoSorgula",
            //    data: sorgu,
            //    success: sigortaliKontrol.sigortaliSuccess
            //});
            var urunAdi = $("#UrunAdi").val();
            if (urunAdi == "TSS") {
                $.ajax({
                    type: "post",
                    dataType: "json",
                    url: "/Teklif/Teklif/TSSNipponKimlikSorgula",
                    data: sorgu,
                    success: sigortaliKontrol.sigortaliSuccess
                });
            }
            else {
                var kimlikSorgulandi = false;
                var divSayisi = 0;
                $("html").find(".tum-no").each(function (i) {
                    divSayisi = i + 1;
                });
                for (var i = 0; i < divSayisi; i++) {
                    //Anadolu sigorta şirketinin durumu okunuyor
                    var tumValue = $("#TeklifUM_" + i + "__TUMKodu").val();
                    if (tumValue == 6) {
                        var Gulf = $("#TeklifUM_" + i + "__TeklifAl_control").bootstrapSwitch('status');
                        if (Gulf) {
                            $.ajax({
                                type: "post",
                                dataType: "json",
                                url: "/Teklif/Teklif/KimlikNoSorgulaGulf",
                                data: sorgu,
                                success: sigortaliKontrol.sigortaliSuccess
                            });
                            break;
                        }
                    }
                }
                if (Gulf == false || Gulf === undefined) {
                    $.ajax({
                        type: "post",
                        dataType: "json",
                        url: "/Teklif/Teklif/KimlikNoSorgula",
                        data: sorgu,
                        success: sigortaliKontrol.sigortaliSuccess
                    });
                }
            }
        },

        this.sigortaEttirenTemizle = function () {
            $("#Musteri_SigortaEttiren_MusteriKodu").val("");
            $("#Musteri_SigortaEttiren_MusteriTipKodu").val("");
            //$("[name='Musteri.SigortaEttiren.Uyruk']").val(1);
            $("#Musteri_SigortaEttiren_AdiUnvan").val("");
            $("#Musteri_SigortaEttiren_SoyadiUnvan").val("");
            $("#Musteri_SigortaEttiren_AdiUnvan").removeAttr('disabled');
            $("#Musteri_SigortaEttiren_SoyadiUnvan").removeAttr('disabled');
            $("#Musteri_SigortaEttiren_IlKodu").val("");
            $("#Musteri_SigortaEttiren_IlceKodu").empty();
            $("#Musteri_SigortaEttiren_IlceKodu").addToDropDown("", "Lütfen Seçiniz");
            $("#Musteri_SigortaEttiren_IlceKodu").val("");
            //$("[name='Musteri.SigortaEttiren.Cinsiyet']").val("E");
            $("#Musteri_SigortaEttiren_DogumTarihi").val("");
            $("#Musteri_SigortaEttiren_CepTelefonu").val("90");
            $("#Musteri_SigortaEttiren_Email").val("");
            $("#Musteri_SigortaEttiren_DogumTarihi").val("");
            $("#Musteri_SigortaEttiren_VergiDairesi").val("");

            $("#form2").find(".control-group.error").removeClass("error");
            $("#form2").find(".field-validation-error").hide();

            $("#Musteri_SigortaEttiren_IlKodu").removeAttr("disabled");
            $("#Musteri_SigortaEttiren_IlceKodu").removeAttr("disabled");
        },

        this.sigortaliTemizle = function () {
            $("#Musteri_Sigortali_MusteriKodu").val("");
            $("#Musteri_Sigortali_MusteriTipKodu").val("");
            //  $("[name='Musteri.Sigortali.Uyruk']").val(1);
            $("#Musteri_Sigortali_AdiUnvan").val("");
            $("#Musteri_Sigortali_SoyadiUnvan").val("");
            $("#Musteri_Sigortali_AdiUnvan").removeAttr('disabled');
            $("#Musteri_Sigortali_SoyadiUnvan").removeAttr('disabled');
            $("#Musteri_Sigortali_IlKodu").val("");
            $("#Musteri_Sigortali_IlceKodu").empty();
            $("#Musteri_Sigortali_IlceKodu").addToDropDown("", "Lütfen Seçiniz");
            $("#Musteri_Sigortali_IlceKodu").val("");
            // $("[name='Musteri.Sigortali.Cinsiyet']").val("E");
            $("#Musteri_Sigortali_DogumTarihi").val("");
            $("#Musteri_Sigortali_CepTelefonu").val("90");
            $("#Musteri_Sigortali_Email").val("");
            $("#Musteri_Sigortali_DogumTarihi").val("");
            $("#Musteri_Sigortali_VergiDairesi").val("");
            $("#form2").find(".control-group.error").removeClass("error");
            $("#form2").find(".field-validation-error").hide();

            $("#Musteri_Sigortali_IlKodu").removeAttr("disabled");
            $("#Musteri_Sigortali_IlceKodu").removeAttr("disabled");
        },

        this.sigortaEttirenSuccess = function (data) {
            if (data.SorgulamaSonuc) {
                $("#Musteri_SigortaEttiren_MusteriKodu").val(data.MusteriKodu);
                $("#Musteri_SigortaEttiren_MusteriTipKodu").val(data.MusteriTipKodu);
                $("[name='Musteri.SigortaEttiren.Uyruk'][value=" + data.Uyruk + "]").attr('checked', 'checked');
                $("#Musteri_SigortaEttiren_AdiUnvan").val(data.AdiUnvan);
                $("#Musteri_SigortaEttiren_AdiUnvan").val($("#Musteri_SigortaEttiren_AdiUnvan").val().replace('i', 'İ'));
                $("#Musteri_SigortaEttiren_AdiUnvan").val($("#Musteri_SigortaEttiren_AdiUnvan").val().toUpperCase());
                $("#Musteri_SigortaEttiren_SoyadiUnvan").val(data.SoyadiUnvan);
                $("#Musteri_SigortaEttiren_SoyadiUnvan").val($("#Musteri_SigortaEttiren_SoyadiUnvan").val().replace('i', 'İ'));
                $("#Musteri_SigortaEttiren_SoyadiUnvan").val($("#Musteri_SigortaEttiren_SoyadiUnvan").val().toUpperCase());
                $("#Musteri_TVMKodu").val(data.TVMKodu);
                $("#Musteri_SigortaEttiren_TVMKodu").val(data.TVMKodu);
                $("#Musteri_Sigortali_TVMKodu").val(data.TVMKodu);
                $("#Musteri_SigortaEttiren_GulfKimlikNo").val(data.GulfKimlikNo);



                if (data.DisableControls) {
                    $("#Musteri_SigortaEttiren_AdiUnvan").prop('disabled', true);
                    $("#Musteri_SigortaEttiren_SoyadiUnvan").prop('disabled', true);
                    $("#Cinsiyet_E").prop('disabled', true);
                    $("#Cinsiyet_K").prop('disabled', true);
                }

                $("#Musteri_SigortaEttiren_IlKodu").val(data.IlKodu);
                $("#Musteri_SigortaEttiren_IlceKodu").dropDownFill(data.Ilceler);
                $("#Musteri_SigortaEttiren_IlceKodu").val(data.IlceKodu);
                if (data.IlceKodu == 0) {
                    $("#Musteri_SigortaEttiren_IlceKodu").val("");
                }

                $("#Musteri_SigortaEttiren_MusteriTelTipKodu").dropDownFill(data.MusteriTelTipleri);
                if (data.MusteriTelTipKodu != "" && data.MusteriTelTipKodu != null) {
                    $("#Musteri_SigortaEttiren_MusteriTelTipKodu").val(data.MusteriTelTipKodu);
                }
                $("#Musteri_SigortaEttiren_CepTelefonu").val(data.CepTelefonu);

                if (data.Email != null && data.Email != "") {
                    $("#Musteri_SigortaEttiren_Email").val(data.Email);
                }

                $("#Musteri_SigortaEttiren_IlKodu").removeAttr("disabled");
                $("#Musteri_SigortaEttiren_IlceKodu").removeAttr("disabled");

                if (data.DisableControls && data.IlKodu && data.IlKodu.length > 0 && data.IlceKodu && data.IlceKodu != 0 && data.IlceKodu != "undefined") {
                    $("#Musteri_SigortaEttiren_IlKodu").prop('disabled', true);
                    $("#Musteri_SigortaEttiren_IlceKodu").prop('disabled', true);
                }

                //Özel müşteri
                if (data.MusteriTipKodu == 1 || data.MusteriTipKodu == 4) {
                    if (data.Cinsiyet == "E") { $("#Cinsiyet_E").prop("checked", true); }
                    else if (data.Cinsiyet == "K") { $("#Cinsiyet_K").prop("checked", true); }

                    $("#Musteri_SigortaEttiren_DogumTarihi").val(data.DogumTarihiText);
                }
                //Tüzel müşteri
                else if (data.MusteriTipKodu == 2 || data.MusteriTipKodu == 3) {
                    $("#Musteri_SigortaEttiren_VergiDairesi").val(data.VergiDairesi);
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
                    $("#Musteri_SigortaEttiren_AcikAdres").val(data.AcikAdres);
                    $("#Musteri_SigortaEttiren_Mahalle").val(data.Mahalle);
                    $("#Musteri_SigortaEttiren_Cadde").val(data.Cadde);
                    $("#Musteri_SigortaEttiren_BinaNo").val(data.BinaNo);
                    $("#Musteri_SigortaEttiren_DaireNo").val(data.DaireNo);
                }

                $("#sigortaettiren-kimlikno-mesaj").hide();
                sigortaliKontrol.sigortaEttirenTipi();
            }
            else {
                debugger
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
                $("#Musteri_SigortaEttiren_MusteriTipKodu").removeAttr("disabled");

            }
            var urunAdi = $("#UrunAdi").val();
            if (urunAdi == "dask") {
                $(".sigortali-satir.tuzel :input").addClass("ignore");
                $(".sigortali-satir.ozel :input").addClass("ignore");
                $(".sigortaettiren-satir ozel :input").addClass("ignore");
                $(".sigortaettiren-satir tuzel :input").addClass("ignore");

                $("#Musteri_SigortaEttiren_CepTelefonu").removeClass("ignore");
            }
            $("#button-next").prop("disabled", false);
            $("#sigortaettiren-sorgula").button("reset");
            $('.button-previous').css('pointer-events', 'auto');
        },


        this.sigortaliSuccess = function (data) {
            if (data.SorgulamaSonuc) {
                $("#Musteri_Sigortali_MusteriKodu").val(data.MusteriKodu);
                $("#Musteri_Sigortali_MusteriTipKodu").val(data.MusteriTipKodu);
                $("[name='Musteri.Sigortali.Uyruk']").val(data.Uyruk);
                $("#Musteri_Sigortali_AdiUnvan").val(data.AdiUnvan);
                $("#Musteri_Sigortali_AdiUnvan").val($("#Musteri_Sigortali_AdiUnvan").val().replace('i', 'İ'));
                $("#Musteri_Sigortali_AdiUnvan").val($("#Musteri_Sigortali_AdiUnvan").val().toUpperCase());

                $("#Musteri_Sigortali_SoyadiUnvan").val(data.SoyadiUnvan);
                $("#Musteri_Sigortali_SoyadiUnvan").val($("#Musteri_Sigortali_SoyadiUnvan").val().replace('i', 'İ'));
                $("#Musteri_Sigortali_SoyadiUnvan").val($("#Musteri_Sigortali_SoyadiUnvan").val().toUpperCase());

                if (data.DisableControls) {
                    $("#Musteri_Sigortali_AdiUnvan").prop('disabled', true);
                    $("#Musteri_Sigortali_SoyadiUnvan").prop('disabled', true);
                }

                $("#Musteri_Sigortali_IlKodu").val(data.IlKodu);
                $("#Musteri_Sigortali_IlceKodu").dropDownFill(data.Ilceler);
                $("#Musteri_Sigortali_IlceKodu").val(data.IlceKodu);
                if (data.IlceKodu == 0) {
                    $("#Musteri_SigortaEttiren_IlceKodu").val("");
                }

                $("#Musteri_Sigortali_CepTelefonu").val(data.CepTelefonu);
                $("#Musteri_Sigortali_Email").val(data.Email);

                $("#Musteri_Sigortali_AdresTipi").val("8");

                $("#Musteri_Sigortali_IlKodu").removeAttr("disabled");
                $("#Musteri_Sigortali_IlceKodu").removeAttr("disabled");

                if (data.DisableControls && data.IlKodu && data.IlKodu.length > 0 && data.IlceKodu && data.IlceKodu != 0 && data.IlceKodu != "undefined") {
                    $("#Musteri_Sigortali_IlKodu").prop('disabled', true);
                    $("#Musteri_Sigortali_IlceKodu").prop('disabled', true);
                }

                //Özel müşteri
                if (data.MusteriTipKodu == 1 || data.MusteriTipKodu == 4) {
                    if (data.Cinsiyet == "E") { $("#Cinsiyet_E").prop("checked", true); }
                    else if (data.Cinsiyet == "K") { $("#Cinsiyet_K").prop("checked", true); }

                    $("#Musteri_Sigortali_DogumTarihi").val(data.DogumTarihiText);
                }
                //Tüzel müşteri
                else if (data.MusteriTipKodu == 2 || data.MusteriTipKodu == 3) {
                    $("#Musteri_Sigortali_VergiDairesi").val(data.VergiDairesi);
                }

                $("#sigortali-kimlikno-mesaj").hide();
                sigortaliKontrol.sigortaliTipi();
            }
            else {
                $("#sigortali-kimlikno-mesaj").html(data.HataMesaj);
                $("#sigortali-kimlikno-mesaj").show();

                if (data.DisableManualGiris) {
                    $("#sigortali-sorgula").button("reset");
                    $("#button-next").prop("disabled", false);
                    $('.button-previous').css('pointer-events', 'auto');
                    return;
                }

                $("#Musteri_Sigortali_MusteriTipKodu").removeAttr("disabled");
                $(".sigortali-satir.musteri-tipi").slideDown();
            }
            var urunAdi = $("#UrunAdi").val();
            if (urunAdi == "dask") {
                $(".sigortali-satir.tuzel :input").addClass("ignore");
                $(".sigortali-satir.ozel :input").addClass("ignore");
                $("#Musteri_Sigortali_CepTelefonu").removeClass("ignore");
            }
            $("#sigortaettiren-kimlikno-mesaj").hide();
            $("#sigortali-sorgula").button("reset");
            $('.button-previous').css('pointer-events', 'auto');
            $("#button-next").prop("disabled", false);
        },

        this.Kaydet = function () {
            debugger;
            var isvalid = FormWizard.validatePage('#tab2');

            if (isvalid) {
                var sigortaEttirenKodu = $("#Musteri_SigortaEttiren_MusteriKodu").val();
                var sigortaliKodu = $("#Musteri_SigortaEttiren_MusteriKodu").val();
                var sigortaliAyni = $("#Musteri_SigortaliAyni_control").bootstrapSwitch('status');

                if (((sigortaEttirenKodu == "" || sigortaEttirenKodu == "0") || ((sigortaliKodu == "" || sigortaliKodu == "0") && !sigortaliAyni)) || $("#Musteri_SigortaEttiren_TVMKodu").val() != $("#Hazirlayan_TVMKodu").val()) {

                    $("#button-next").button("loading");
                    $(".switcher").find(":input").switchFix();

                    var disabled = $("#tab2").find(':input:disabled').removeAttr('disabled');
                    var disabled2 = $("#tab1").find(':input:disabled').removeAttr('disabled');
                    var data = $("#tab2").find("select, textarea, input").serialize();
                    disabled.attr('disabled', 'disabled');

                    var tvmKodu = $("#Hazirlayan_TVMKodu").val();
                    data.TVMKodu = tvmKodu;
                    disabled2.attr('disabled', 'disabled');

                    $.ajax({
                        type: "POST",
                        url: "/Teklif/Teklif/MusteriKaydet",
                        data: data,
                        dataType: "json",
                        success: sigortaliKontrol.KaydetSuccess,
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
        this.KaydetSuccess = function (data) {
            if (data.SigortaEttiren.SorgulamaSonuc && data.Sigortali.SorgulamaSonuc) {
                $("#Musteri_SigortaEttiren_MusteriKodu").val(data.SigortaEttiren.MusteriKodu);
                $("#Musteri_Sigortali_MusteriKodu").val(data.Sigortali.MusteriKodu);

                $("#button-next").button("reset");
                $('#form_wizard_1').bootstrapWizard("next");
            }
            else if (data.MusteriKodu > 0) {
                $("#Musteri_SigortaEttiren_MusteriKodu").val(data.SigortaEttiren.MusteriKodu);
                $("#Musteri_Sigortali_MusteriKodu").val(data.Sigortali.MusteriKodu);

                $("#button-next").button("reset");
                $('#form_wizard_1').bootstrapWizard("next");
            }
            else if (data.SigortaEttiren.MusteriKodu > 0) {
                $("#Musteri_SigortaEttiren_MusteriKodu").val(data.SigortaEttiren.MusteriKodu);
                $("#Musteri_Sigortali_MusteriKodu").val(data.Sigortali.MusteriKodu);

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

// ===  (ORTAK) === //
//$("#Arac_PlakaNo").alphanumeric();

$(".credit-card").keyup(function (event) {
    var key = event.which;

    if (key !== 0) {
        var c = String.fromCharCode(key);
        if (c.match("[0-9]")) {
            if (this.value.length == 4)
                $(this).next("input[type=text]").focus();
        }
        else if (this.value.length == 0)
            $(this).prev("input[type=text]").focus();
    }
});

$("#Musteri_SigortaEttiren_AdiUnvan").blur(function () {
    $(this).val($(this).val().replace('i', 'İ'));
    $(this).val($(this).val().toUpperCase());
});

$("#Musteri_SigortaEttiren_SoyadiUnvan").blur(function () {
    $(this).val($(this).val().replace('i', 'İ'));
    $(this).val($(this).val().toUpperCase());
});

$("#Musteri_Sigortali_AdiUnvan").blur(function () {
    $(this).val($(this).val().replace('i', 'İ'));
    $(this).val($(this).val().toUpperCase());
});

$("#Musteri_Sigortali_SoyadiUnvan").blur(function () {
    $(this).val($(this).val().replace('i', 'İ'));
    $(this).val($(this).val().toUpperCase());
});

var language = new
    function () {
        var getCookie = function (cname) {
            var name = cname + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') c = c.substring(1);
                if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
            }
            return "";
        };
        var lang = ['tr-TR', 'en-US', 'es-ES', 'fr-FR', 'it-IT'];
        var cookieLang = getCookie('lang');
        var currentLang = lang.indexOf(cookieLang) > -1 ? cookieLang : 'tr-TR';
        dictionary = { select: { 'tr-TR': 'Seçiniz', 'en-US': 'Select', 'es-ES': 'Select', 'fr-FR': 'Select', 'it-IT': 'Select' } };
        return {
            get: function (name) { return dictionary[name][currentLang] },
        }
    };
