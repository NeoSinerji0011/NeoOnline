$(document).ready(function() {
    console.log("asdf");
    $("[rel=tooltip]").tooltip();

    $("[name='Hazirlayan.KendiAdima']").change(function() {
        var kendiAdima = $(this).val();

        if (kendiAdima === "1") {
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

    $("#pdf-karsilastirma").live("click", function() {
        var teklifId = $("#TeklifId").val();
        //var contents = $("#formTeklif").serialize();
        $("#pdf-karsilastirma").button("loading");
        $.ajax({
            type: "POST",
            url: "/Teklif/Teklif/TeklifPDF",
            data: { id: teklifId },
            success: function(data) {
                $("#pdf-karsilastirma").button("reset");
                if (data.Success) {
                    $(this).attr("pdf", data.PDFUrl);
                    window.open(data.PDFUrl, "_blank");
                    return;
                }
            },
            error: function() {
                $("#pdf-karsilastirma").button("reset");
            }
        });
    });

    // ==== Email Gönderme İşlemleri ==== //
    $("#email-gonder").live("click", function() {
        var teklifId = $("#TeklifId").val();

        $("#email-gonder").button("loading");

        $.get("/Teklif/Teklif/TeklifEposta",
            { id: teklifId },
            function(data) {
                $("#mail-gonder-modal-div").html(data);
                $.validator.unobtrusive.parse("#mail-gonder-modal-div");
                $("#email-modal").modal('show');
                $("#email-gonder").button("reset");
            },
            "html");
    });

    $("#mail-gonder-btn").live("click", function() {
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
                success: function(data) {
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
                error: function(jqXHR, textStatus, errorThrown) {
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

var teklifFiyat = new function() {
    this.gosterilenler = [];
    this.requestCount = 0;
    this.processId = 0;
    this.guid = '';
    this.timeout;

    this.kontol = function(options) {
        this.processId = options.processId;
        this.guid = options.guid;
        this.gosterilenler = [];
        this.timeout = setTimeout(this.durumKontrol, 5000);
    },

        this.durumKontrol = function() {
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

        this.durumSuccess = function(data) {
            var model = data.model;

            $("#TeklifId").val(model.teklifId);
            if (model.teklifler.length > 0) {
                var index = teklifFiyat.gosterilenler.length;

                for (var teklif in model.teklifler) {
                    var bitir = false;
                    if (model.tamamlandi && (parseInt(teklif) + 1) === model.teklifler.length) {
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

        this.durumError = function() {
        },

        this.showTeklif = function(index, teklif, tamamlandi, teklifNo) {
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
            if (teklif.TUMTeklifUyariMesaji !== null && teklif.TUMTeklifUyariMesaji !== "") {
                tumUyari.attr({
                    "alt": teklif.TUMTeklifUyariMesaji,
                    "title": "Uyarı Mesajı - " + teklif.TUMTeklifUyariMesaji
                });
                tumUyari.css("display", "normal");
            }
            else {
                tumUyari.css("display", "none");
            }
            if (teklif.TUMTeklifBilgiMesaji !== null && teklif.TUMTeklifBilgiMesaji !== "") {
                tumBilgi.attr({
                    "alt": teklif.TUMTeklifBilgiMesaji,
                    "title": "Bilgi Mesajı - " + teklif.TUMTeklifBilgiMesaji
                });
                tumBilgi.css("display", "normal");
            }
            else {
                tumBilgi.css("display", "none");
            }
            if (teklif.KomisyonTutari !== null && teklif.KomisyonTutari.length > 0) {
                var html = "";
            }

            if (teklif.Hasarsizlik !== null && teklif.Hasarsizlik.length > 0) {
                var html = "";

                if (teklif.HasarIndirimSurprim === "I") {
                    html = "<span class='label label-success' style='margin-top:25px'>" + teklif.Hasarsizlik + "</span>";
                }
                else if (teklif.HasarIndirimSurprim === "S") {
                    html = "<span class='label label-important' style='margin-top:25px'>" + teklif.Hasarsizlik + "</span>";
                }
                else {
                    html = "<span class='label label-info' style='margin-top:25px'>%0</span>";
                }

                hasarsizlik.html(html);
            }

            if (teklif.Surprimler !== null && teklif.Surprimler.length > 0) {
                var html = "<div style='display: table-cell; vertical-align: middle;height:65px;'>";

                for (var i in teklif.Surprimler) {
                    var surp = teklif.Surprimler[i];
                    html = html + "<div class='row' style='width:200px;margin-left: auto; margin-right: auto;'>"
                    html = html + "<div class='span10' style='text-align:center;'>" + surp.SurprimAciklama + "</div>"

                    if (surp.SurprimIS === "I") {
                        html = html + "<div class='span2' style='text-align:center;'><span class='label label-success'>" + surp.Surprim + "</span></div>"
                    }
                    else if (surp.SurprimIS === "S") {
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

            if (teklif.TUMPDF !== null && teklif.TUMPDF !== "") {
                tumPDF.html(teklif.TUMPDF);
            }

            if (teklif.Hatalar !== null && teklif.Hatalar.length > 0) {
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

                if (teklif.DaskUyariMesaji !== "") {
                    daskUyari.html(teklif.TUMPDF);
                    daskUyari.show();
                }
                var urunAdi = $("#UrunAdi").val();
                if (urunAdi === "TSS") {

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

            $("#teklif-fiyatlar").slideDown("fast", function() {
                container.css({ "width": "0" });
                container.show();
                container.animate({ "width": "100%" }, "slow", "linear", function() {
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

        this.teklifTekrar = function() {
            debugger;
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

var sigortaliKontrol = new function() {

    this.init = function() {

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
            if (musteriKodu !== "") {
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

        this.sigortaliFarkli = function(e, data) {
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

        this.sigortaEttirenTipi = function() {
            var musteriTipi = $("#Musteri_SigortaEttiren_MusteriTipKodu").val();
            if (musteriTipi === "1") { //Şahıs
                $(".sigortaettiren-satir.tuzel").slideUp();
                $(".sigortaettiren-satir.ozel").slideDown();
                $(".sigortaettiren-satir.tuzel :input").addClass("ignore");
                $(".sigortaettiren-satir.ozel :input").removeClass("ignore");

                $("#Uyruk_1[name='Musteri.SigortaEttiren.Uyruk']").attr("disabled", "disabled");
                $("#Uyruk_0[name='Musteri.SigortaEttiren.Uyruk']").attr("checked", "checked");
                $("#Uyruk_0[name='Musteri.SigortaEttiren.Uyruk']").removeAttr("disabled");

            } else if (musteriTipi === "2") { //Tüzel
                $(".sigortaettiren-satir.ozel").slideUp();
                $(".sigortaettiren-satir.tuzel").slideDown();
                $(".sigortaettiren-satir.ozel :input").addClass("ignore");
                $(".sigortaettiren-satir.tuzel :input").removeClass("ignore");
            } else if (musteriTipi === "3") { //Şahıs firması
                $(".sigortaettiren-satir.ozel").slideUp();
                $(".sigortaettiren-satir.tuzel").slideDown();
                $(".sigortaettiren-satir.ozel :input").addClass("ignore");
                $(".sigortaettiren-satir.tuzel :input").removeClass("ignore");
            } else if (musteriTipi === "4") { //Yabancı
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
            if (urunAdi === "dask") {
                $(".sigortaettiren-satir.tuzel :input").addClass("ignore");
                $(".sigortaettiren-satir.ozel :input").addClass("ignore");
                $("#Musteri_SigortaEttiren_CepTelefonu").removeClass("ignore");
                $(".required").html();

            }
            else {
                $(".required").html("*");
            }
        },


        this.sigortaliTipi = function() {
            var musteriTipi = $("#Musteri_Sigortali_MusteriTipKodu").val();

            if (musteriTipi === "1") { //Şahıs
                $(".sigortali-satir.tuzel").slideUp();
                $(".sigortali-satir.ozel").slideDown();
                $(".sigortali-satir.tuzel :input").addClass("ignore");
                $(".sigortali-satir.ozel :input").removeClass("ignore");
                $("#Musteri_Sigortali_CepTelefonu").removeClass("ignore");
                $("#Uyruk_1[name='Musteri.Sigortali.Uyruk']").attr("disabled", "disabled");
                $("#Uyruk_0[name='Musteri.Sigortali.Uyruk']").attr("checked", "checked");
                $("#Uyruk_0[name='Musteri.Sigortali.Uyruk']").removeAttr("disabled");
            } else if (musteriTipi === "2") { //Tüzel
                $(".sigortali-satir.ozel").slideUp();
                $(".sigortali-satir.tuzel").slideDown();
                $(".sigortali-satir.ozel :input").addClass("ignore");
                $(".sigortali-satir.tuzel :input").removeClass("ignore");
            } else if (musteriTipi === "3") { //Şahıs firması
                $(".sigortali-satir.ozel").slideUp();
                $(".sigortali-satir.tuzel").slideDown();
                $(".sigortali-satir.ozel :input").addClass("ignore");
                $(".sigortali-satir.tuzel :input").removeClass("ignore");
            } else if (musteriTipi === "4") { //Yabancı
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

        this.sigortaEttirenSorgula = function() {
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

            $.ajax({
                type: "post",
                dataType: "json",
                url: "/Teklif/Teklif/KimlikNoSorgula",
                data: sorgu,
                success: sigortaliKontrol.sigortaEttirenSuccess
            });

            if (tvmKodu === 100) {
                $("#urun-Listele").show();
            }
            else {
                $("#urun-Listele").hide();
            }
        },

        this.sigortaliSorgula = function() {
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
            if (urunAdi === "TSS") {
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
                $("html").find(".tum-no").each(function(i) {
                    divSayisi = i + 1;
                });
                for (var i = 0; i < divSayisi; i++) {
                    //Anadolu sigorta şirketinin durumu okunuyor
                    var tumValue = $("#TeklifUM_" + i + "__TUMKodu").val();
                    if (tumValue === 6) {
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
                if (Gulf === false || Gulf === undefined) {
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

        this.sigortaEttirenTemizle = function() {
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

        this.sigortaliTemizle = function() {
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

        this.sigortaEttirenSuccess = function(data) {
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
                if (data.IlceKodu === 0) {
                    $("#Musteri_SigortaEttiren_IlceKodu").val("");
                }

                $("#Musteri_SigortaEttiren_MusteriTelTipKodu").dropDownFill(data.MusteriTelTipleri);
                if (data.MusteriTelTipKodu !== "" && data.MusteriTelTipKodu !== null) {
                    $("#Musteri_SigortaEttiren_MusteriTelTipKodu").val(data.MusteriTelTipKodu);
                }
                $("#Musteri_SigortaEttiren_CepTelefonu").val(data.CepTelefonu);

                if (data.Email !== null && data.Email !== "") {
                    $("#Musteri_SigortaEttiren_Email").val(data.Email);
                }

                $("#Musteri_SigortaEttiren_IlKodu").removeAttr("disabled");
                $("#Musteri_SigortaEttiren_IlceKodu").removeAttr("disabled");

                if (data.DisableControls && data.IlKodu && data.IlKodu.length > 0 && data.IlceKodu && data.IlceKodu !== 0 && data.IlceKodu !== "undefined") {
                    $("#Musteri_SigortaEttiren_IlKodu").prop('disabled', true);
                    $("#Musteri_SigortaEttiren_IlceKodu").prop('disabled', true);
                }

                //Özel müşteri
                if (data.MusteriTipKodu === 1 || data.MusteriTipKodu === 4) {
                    if (data.Cinsiyet === "E") { $("#Cinsiyet_E").prop("checked", true); }
                    else if (data.Cinsiyet === "K") { $("#Cinsiyet_K").prop("checked", true); }

                    $("#Musteri_SigortaEttiren_DogumTarihi").val(data.DogumTarihiText);
                }
                //Tüzel müşteri
                else if (data.MusteriTipKodu === 2 || data.MusteriTipKodu === 3) {
                    $("#Musteri_SigortaEttiren_VergiDairesi").val(data.VergiDairesi);
                }

                if ($("#Adres_AdresTipi").length === 1) {
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
            if (urunAdi === "dask") {
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


        this.sigortaliSuccess = function(data) {
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
                if (data.IlceKodu === 0) {
                    $("#Musteri_SigortaEttiren_IlceKodu").val("");
                }

                $("#Musteri_Sigortali_CepTelefonu").val(data.CepTelefonu);
                $("#Musteri_Sigortali_Email").val(data.Email);

                $("#Musteri_Sigortali_AdresTipi").val("8");

                $("#Musteri_Sigortali_IlKodu").removeAttr("disabled");
                $("#Musteri_Sigortali_IlceKodu").removeAttr("disabled");

                if (data.DisableControls && data.IlKodu && data.IlKodu.length > 0 && data.IlceKodu && data.IlceKodu !== 0 && data.IlceKodu !== "undefined") {
                    $("#Musteri_Sigortali_IlKodu").prop('disabled', true);
                    $("#Musteri_Sigortali_IlceKodu").prop('disabled', true);
                }

                //Özel müşteri
                if (data.MusteriTipKodu === 1 || data.MusteriTipKodu === 4) {
                    if (data.Cinsiyet === "E") { $("#Cinsiyet_E").prop("checked", true); }
                    else if (data.Cinsiyet === "K") { $("#Cinsiyet_K").prop("checked", true); }

                    $("#Musteri_Sigortali_DogumTarihi").val(data.DogumTarihiText);
                }
                //Tüzel müşteri
                else if (data.MusteriTipKodu === 2 || data.MusteriTipKodu === 3) {
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
            if (urunAdi === "dask") {
                $(".sigortali-satir.tuzel :input").addClass("ignore");
                $(".sigortali-satir.ozel :input").addClass("ignore");
                $("#Musteri_Sigortali_CepTelefonu").removeClass("ignore");
            }
            $("#sigortaettiren-kimlikno-mesaj").hide();
            $("#sigortali-sorgula").button("reset");
            $('.button-previous').css('pointer-events', 'auto');
            $("#button-next").prop("disabled", false);
        },

        this.Kaydet = function() {
            debugger;
            var isvalid = FormWizard.validatePage('#tab2');

            if (isvalid) {
                var sigortaEttirenKodu = $("#Musteri_SigortaEttiren_MusteriKodu").val();
                var sigortaliKodu = $("#Musteri_SigortaEttiren_MusteriKodu").val();
                var sigortaliAyni = $("#Musteri_SigortaliAyni_control").bootstrapSwitch('status');

                if (((sigortaEttirenKodu === "" || sigortaEttirenKodu === "0") || ((sigortaliKodu === "" || sigortaliKodu === "0") && !sigortaliAyni)) || $("#Musteri_SigortaEttiren_TVMKodu").val() !== $("#Hazirlayan_TVMKodu").val()) {

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

            return true;
        },
        this.KaydetSuccess = function(data) {
            debugger
            if (data.SigortaEttiren.SorgulamaSonuc && data.Sigortali.SorgulamaSonuc) {
                $("#Musteri_SigortaEttiren_MusteriKodu").val(data.SigortaEttiren.MusteriKodu);
                $("#Musteri_Sigortali_MusteriKodu").val(data.Sigortali.MusteriKodu);

                $("#button-next").button("reset");
                //$('#form_wizard_1').bootstrapWizard("next");
            }
            else if (data.MusteriKodu > 0) {
                $("#Musteri_SigortaEttiren_MusteriKodu").val(data.SigortaEttiren.MusteriKodu);
                $("#Musteri_Sigortali_MusteriKodu").val(data.Sigortali.MusteriKodu);

                $("#button-next").button("reset");
                //$('#form_wizard_1').bootstrapWizard("next");
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
        this.KaydetError = function() {
            $("#button-next").button("reset");
        }
}

// ===  (ORTAK) === //
//$("#Arac_PlakaNo").alphanumeric();

$(".credit-card").keyup(function(event) {
    var key = event.which;

    if (key !== 0) {
        var c = String.fromCharCode(key);
        if (c.match("[0-9]")) {
            if (this.value.length === 4)
                $(this).next("input[type=text]").focus();
        }
        else if (this.value.length === 0)
            $(this).prev("input[type=text]").focus();
    }
});

$("#Musteri_SigortaEttiren_AdiUnvan").blur(function() {
    $(this).val($(this).val().replace('i', 'İ'));
    $(this).val($(this).val().toUpperCase());
});

$("#Musteri_SigortaEttiren_SoyadiUnvan").blur(function() {
    $(this).val($(this).val().replace('i', 'İ'));
    $(this).val($(this).val().toUpperCase());
});

$("#Musteri_Sigortali_AdiUnvan").blur(function() {
    $(this).val($(this).val().replace('i', 'İ'));
    $(this).val($(this).val().toUpperCase());
});

$("#Musteri_Sigortali_SoyadiUnvan").blur(function() {
    $(this).val($(this).val().replace('i', 'İ'));
    $(this).val($(this).val().toUpperCase());
});

var language = new
    function() {
        var getCookie = function(cname) {
            var name = cname + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) === ' ') c = c.substring(1);
                if (c.indexOf(name) === 0) return c.substring(name.length, c.length);
            }
            return "";
        };
        var lang = ['tr-TR', 'en-US', 'es-ES', 'fr-FR', 'it-IT'];
        var cookieLang = getCookie('lang');
        var currentLang = lang.indexOf(cookieLang) > -1 ? cookieLang : 'tr-TR';
        dictionary = { select: { 'tr-TR': 'Seçiniz', 'en-US': 'Select', 'es-ES': 'Select', 'fr-FR': 'Select', 'it-IT': 'Select' } };
        return {
            get: function(name) { return dictionary[name][currentLang] },
        }
    };

$("#Arac_AnadoluKullanimTip").change(function() {
    $("#kullanimSekli_Anadolu").hide();
    $("#anadolu-kullanimsekli-progress").show();

    $("#anadolu-trafik-ksekli-hata").css('display', 'none');
    var KullanimTarzi = $("#Arac_KullanimTarziKodu").val();
    var Model = $("#Arac_Model").val();
    var MarkaKodu = $("#Arac_MarkaKodu").val();
    var KullanimTipi = $("#Arac_AnadoluKullanimTip").val();
    var AnadoluMarkaKodu = $("#Arac_AnadoluMarkaKodu").val();

    if ((KullanimTarzi !== null && KullanimTarzi !== '') && (Model !== null && Model !== '') && (MarkaKodu !== null && MarkaKodu !== '') && (KullanimTipi !== null && KullanimTipi !== '')) {

        $.getJSON('/Teklif/Trafik/KullanimSekliSorgulaAnadolu', { AnadoluMarkaKodu: AnadoluMarkaKodu, AracKullanimTarzi: KullanimTarzi, AracModelYili: Model, AracMarkaKodu: MarkaKodu, KullanimTipi: KullanimTipi },
            function(result) {
                if (result.list.hata !== "" && result.hata !== null) {
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

var nipponPlanCodeDiv = $("#nipponPlanCodeDiv");
var nipponScopeDiv = $("#nipponScopeDiv");
var nipponAlternativeDiv = $("#nipponAlternativeDiv");
var nipponCountryDiv = $("#nipponCountryDiv");
var nipponProgressDiv = $("#nipponProgress");

var nipponPlanCodeList = $('#nipponPlanCodeList');
var nipponScopeList = $('#nipponScopeList');
//var isDomestic;
$("#nipponIsDomesticList").change(function() {
    debugger
    isDomestic = $(this).val() === "true" ? true : false;
    if ($(this).children('option').length === 3) {
        $(this).find('option').get(0).remove();
    }
    nipponScopeDiv.css("display", "none"); // make nipponScopeList hidden
    nipponAlternativeDiv.css("display", "none"); // make nipponAlternativeList hidden
    nipponCountryDiv.css("display", "none"); // make nipponCountryList hidden
    nipponPlanCodeDiv.css("display", "none"); // make nipponPlanCodeList hidden
    nipponProgressDiv.css("display", "block"); // make nipponProgress visible
    if (isDomestic) {
        setNipponListsForDomesticTravel(); // Add required data to Nippon DropDownLists for domestic travels in Turkey.
        //disableNipponLists(); // Disable Scope, Alternative and Country Selection DropDownLists.
        nipponProgressDiv.css("display", "none"); // make nipponProgress hidden
    }
    else {
        enableNipponLists();
        nipponPlanCodeDiv.css("display", "block");
        $.getJSON("/Teklif/NipponSeyahat/GetScopeList", { isDomestic: isDomestic }, function(response) {
            debugger
            nipponScopeDiv.css("display", "block"); // make nipponScopeList visible
            nipponProgressDiv.css("display", "none"); // make nipponProgress hidden
            nipponScopeList.empty(); // remove any existing options
            nipponScopeList.append($('<option></option>').val("").html("Lütfen Seçim Yapınız."));
            $.each(response, function(index, item) { // Add each element in returned array to scopeList
                nipponScopeList.append($('<option></option>').val(item.ScopeOrPocket).html(item.Description));
            });
        });
    }
});

var nipponAlternativeList = $('#nipponAlternativeList');
$("#nipponScopeList").change(function() {
    resetOffers();
    var scopeOrPocket = $(this).val(); // Use $(this) so you don't traverse the DOM again
    nipponAlternativeDiv.css("display", "none"); // make nipponScopeList visible
    nipponCountryDiv.css("display", "none"); // make nipponScopeList visible
    nipponProgressDiv.css("display", "block"); // make nipponProgress visible
    $.getJSON("/Teklif/NipponSeyahat/GetAlternativeList", { isDomestic: isDomestic, scopeOrPocket: scopeOrPocket }, function(response) {
        nipponAlternativeDiv.css("display", "block"); // make nipponAlternativeList visible
        nipponProgressDiv.css("display", "none"); // make nipponProgress hidden
        nipponAlternativeList.empty(); // remove any existing options
        nipponAlternativeList.append($('<option></option>').val("").html("Lütfen Seçim Yapınız."));
        $.each(response, function(index, item) { // Add each element in returned array to scopeList
            nipponAlternativeList.append($('<option></option>').val(item.Alternative).html(item.Description));
        });
    });
});

var nipponCountryList = $('#nipponCountryList');
$("#nipponAlternativeList").change(function() {
    resetOffers();
    var alternative = $(this).val(); // Use $(this) so you don't traverse the DOM again
    nipponCountryDiv.css("display", "none"); // make nipponScopeList hidden
    nipponProgressDiv.css("display", "block"); // make nipponProgress visible
    $.getJSON("/Teklif/NipponSeyahat/GetCountryList", { Alternative: alternative }, function(response) {
        nipponCountryDiv.css("display", "block"); // make nipponCountryList visible
        nipponProgressDiv.css("display", "none"); // make nipponProgress hidden
        nipponCountryList.empty(); // remove any existing options
        nipponCountryList.append($('<option></option>').val("").html("Lütfen Seçim Yapınız."));
        $.each(response, function(index, item) { // Add each element in returned array to scopeList
            nipponCountryList.append($('<option></option>').val(item.CountryCode).html(item.CountryName));
        });
    });
});

nipponCountryList.change(function() {
    resetOffers();
});

function disableNipponLists() {
    document.getElementById("nipponScopeList").disabled = true;
    document.getElementById("nipponAlternativeList").disabled = true;
    document.getElementById("nipponCountryList").disabled = true;
}

function enableNipponLists() {
    document.getElementById("nipponScopeList").disabled = false;
    document.getElementById("nipponAlternativeList").disabled = false;
    document.getElementById("nipponCountryList").disabled = false;
}

function setNipponListsForDomesticTravel() {
    nipponScopeDiv.css("display", "block"); // make nipponScopeList visible
    nipponScopeList.empty(); // remove any existing options
    nipponScopeList.append($('<option></option>').val(1).html("PAKET 1"));

    nipponAlternativeDiv.css("display", "block"); // make nipponAlternativeList visible
    nipponAlternativeList.empty(); // remove any existing options
    nipponAlternativeList.append($('<option></option>').val(1).html("ALTERNATİF 1"));

    nipponCountryDiv.css("display", "block"); // make nipponCountryList visible
    nipponCountryList.empty(); // remove any existing option
    nipponCountryList.append($('<option></option>').val(9999).html("TÜRKİYE"));
}

var tckTextBoxes = $(".tckText");
$(document).on('keyup', '.tckText', function() {
    var tckText = $(this);
    if (tckText.val().length === 11 && !tckText.is('[readonly]')) {
        debugger
        var TCKNo = $(this).val();
        var nipponidentityName = tckText.parent().parent().find("td.nipponIdentityName");
        var nipponBirthDate = tckText.parent().parent().find("td.nipponBirthDate");
        var nipponStatus = tckText.parent().parent().find("td.nipponStatus");

        $.ajax({
            url: "/Teklif/NipponSeyahat/QueryTCKNo",
            type: "POST",
            dataType: "json",
            data: {
                "ClientType": "O",
                "IdentityNo": TCKNo
            },
            beforeSend: function() {
                tckText.attr("readonly", "readonly");
                nipponStatus.html('Sorgulanıyor...');
            },
            success: function(response) {
                if (response.Status) {
                    nipponidentityName.text(response.Data.IdentityName);
                    nipponBirthDate.text(response.Data.BirthDate);
                    nipponStatus.html("");
                    if (tckText.hasClass("error"))
                        tckText.removeClass("error");

                    var customerJson = {};
                    customerJson["CustomerIdentityNo"] = TCKNo;
                    customerJson["CustomerName"] = response.Data.IdentityName;
                    customerJson["CustomerBirthDate"] = response.Data.BirthDate;
                    $.ajax({
                        url: "/Teklif/NipponSeyahat/GetCustomerId",
                        type: "POST",
                        dataType: "text",
                        data: { "customerJson": JSON.stringify(customerJson) },
                        success: function(customerCode) {
                            tckText.parent().parent().attr("data-customer-code", customerCode);
                        }
                    });
                }
                else {
                    var errorIconHtml = '<i class="icon-warning-sign" style="color:red"></i>';
                    tckText.removeAttr("readonly");
                    nipponidentityName.text("");
                    nipponBirthDate.text("");
                    nipponStatus.html('Hata... <br>' + response.Data + " " + errorIconHtml);
                    if (response.ErrorType !== "WebServiceUserError")
                        tckText.addClass("error");
                }
            },
            error: function() {
                tckText.removeAttr("readonly");
                nipponStatus.html("Sunucu ile iletişimde hata oluştu...<br>Sayfayı yenileyip tekrar deneyiniz.");
            },
            complete: function() {
                //ShowRefresh();
            }
        });
    }
});

$(document).on('click', ".delete-nipponSigortali", function() {
    debugger;
    var rowCount = $('#nipponInsuredTable tr').length;
    if (rowCount <= 2)
        return;
    else {
        $(this).closest('tr').remove();
        nipponInsuredRowCount--;
        nipponInsuredCountLabel.text("Sigortalı Adedi : " + nipponInsuredRowCount);
        updateTotalPriceLabel();
    }
    var nipponInsuredRows = $('#nipponInsuredTable > tbody  > tr');
    var isErrorPresent = false;
    var isAllHavePolicyNumber = true;
    nipponInsuredRows.each(function() {
        if ($(this).find("input").hasClass("error")) {
            isErrorPresent = true;
        }
        if (!$(this).find("input").hasClass("data-policy-no")) {
            isAllHavePolicyNumber = false;
        }
    });

    if (!isErrorPresent && isAllHavePolicyNumber) {
        $("#btn-policelestir").show();
    }

});


var nipponAddIdentityNoListButton = $("#nipponAddIdentityNoList");
nipponAddIdentityNoListButton.click(function() {
    $("#tckNoList").modal("show");
});

var nipponInsuredRowCount = 0;
var nipponInsuredCountLabel = $("#nipponInsuredCountLabel");
var nipponAddInsuredButton = $("#nipponAddInsured");
nipponAddInsuredButton.click(function() {
    addInsured();
});

function addInsured(tckNo) {
    debugger;
    resetOffers();
    nipponInsuredRowCount++;
    nipponInsuredCountLabel.text("Sigortalı Adedi : " + nipponInsuredRowCount);
    var nipponInsuredRow = $("#nipponInsuredFirstRow").clone();
    nipponInsuredRow.attr("style", "display: normal;");
    nipponInsuredRow.attr("id", "insured-" + nipponInsuredRowCount);
    var nipponInsuredTable = $("#nipponInsuredTable");
    if (tckNo !== undefined) {
        var tckNoInput = nipponInsuredRow.find("input");
        tckNoInput.val(tckNo);
    }
    nipponInsuredTable.append(nipponInsuredRow);

}

nipponAddInsuredButton.click(); // Adds first table row.

var nipponSelfInsuredButton = $("#nipponSelfInsuredButton");
var nipponNotSelfInsuredButton = $("#nipponNotSelfInsuredButton");
var nipponInsurerInfo = $("#nipponInsurerInfo");

nipponSelfInsuredButton.click(function() {
    debugger;
    $(".sigortaettiren-satir").slideUp();
    nipponInsurerInfo.slideUp();
});

nipponNotSelfInsuredButton.click(function() {
    debugger;
    nipponInsurerInfo.slideDown();
});

var nipponComputeButton = $("#btn-hesapla");
nipponComputeButton.click(function() {
    var nipponInsuredRows = $('#nipponInsuredTable > tbody  > tr');

    nipponInsuredRows.each(function(index) { // If there is a row present with same TCKNo remove it

        debugger;
        var CitizenshipNumber = $(this).find("input").val();
        if (!CitizenshipNumber && nipponInsuredRowCount > 1) { // If TCKNo is empty and total row count is more than 2 (including first info tr) remove the row
            $(this).remove();
            nipponInsuredRows.splice(index, 1);
            nipponInsuredRowCount--;
            nipponInsuredCountLabel.text("Sigortalı Adedi : " + nipponInsuredRowCount);
            return true;
        }
        nipponInsuredRows.each(function(innerLoopIndex) {
            if (index !== innerLoopIndex && CitizenshipNumber === $(this).find("input").val()) {
                $(this).remove();
                nipponInsuredRowCount--;
                nipponInsuredCountLabel.text("Sigortalı Adedi : " + nipponInsuredRowCount);
                nipponInsuredRows.splice(innerLoopIndex, 1);
            }
        });
    });

    if (validateNipponSelectLists() < 0) { // If the return code is negative it means there is an error
        return;
    }

    if (validateTravelInputs() < 0) { // If the return code is negative it means there is an error
        return;
    }

    if (validateTCKNo(nipponInsuredRows) < 0) {
        return;
    }

    var completedOfferCount = 0;
    var totalOfferCount = nipponInsuredRows.length;

    $("#btn-policelestir").hide();

    nipponInsuredRows.each(function() {
        $(this).removeAttr("data-premium");
        $(this).removeAttr("data-exchange-rate");
    });
    debugger;

    nipponInsuredRows.each(function() {
        var currentRow = $(this);
        var nipponStatus = $(this).find("td.nipponStatus");
        var insuredJson = getNipponSelectListValuesJson();
        var CitizenshipNumber = $(this).find("input").val();
        var isErrorPresent = false;
        var isNetworkErrorPresent = false;

        showInfoSweetAlert("Teklifleriniz alınıyor...", "Lütfen bekleyiniz.\n Durum : 0 / " + totalOfferCount, false, "/Content/img/loading9.gif");

        var IsDomestic = $("#nipponIsDomesticList").val() === "true" ? true : false;
        insuredJson["CitizenshipNumber"] = CitizenshipNumber;

        var isSelfInsured = $("#nipponSelfInsuredButton").is(":checked");
        var insurerIdentityNo = isSelfInsured ? currentRow.find("input").val() : $("#Musteri_SigortaEttiren_KimlikNo").val();
        var insurerType = insurerIdentityNo.length === 11 ? "Private" : "Corporate"; // Özel - Tüzel

        insuredJson["InsurerType"] = insurerType;
        insuredJson["InsurerIdentityNo"] = insurerIdentityNo;
        debugger;

        var tckText = $(this).find("input");
        if (tckText.hasClass("error"))
            tckText.removeClass("error");

        $.ajax({
            url: "/Teklif/NipponSeyahat/Compute",
            type: "POST",
            dataType: "json",
            data: { "insuredJson": JSON.stringify(insuredJson) },
            beforeSend: function() {
                nipponStatus.html('Teklif Alınıyor...');
            },
            success: function(JSON) {

                var currencySymbol = IsDomestic ? "₺" : "€";
                var okIconHtml = '<i class="icon-ok-sign" style="color:green"></i>';
                var errorIconHtml = '<i class="icon-warning-sign" style="color:red"></i>';

                if (JSON.IsSuccess) {
                    nipponStatus.html(JSON.Premium + " " + currencySymbol + " " + "(Poliçe No: " + JSON.PolicyNo + ") " + okIconHtml);
                    currentRow.attr("data-premium", JSON.Premium);
                    currentRow.attr("data-exchange-rate", JSON.ExchangeRate);
                    currentRow.attr("data-tracking-code", JSON.TrackingCode);
                    currentRow.attr("data-client-no", JSON.ClientNo);
                    currentRow.attr("data-unit-no", JSON.UnitNo);
                    currentRow.attr("data-policy-no", JSON.PolicyNo);
                    updateTotalPriceLabel();
                }
                else {
                    nipponStatus.html('Hata... <br>' + JSON.StatusDescription + " " + errorIconHtml);
                    tckText.addClass("error");
                    isErrorPresent = true;
                }
            },
            error: function() {
                //nipponStatus.html("Sunucu ile iletişimde hata oluştu...");
                //tckText.addClass("error");
                isNetworkErrorPresent = true;
            },
            complete: function() {
                debugger;
                if (isNetworkErrorPresent) {
                    isNetworkErrorPresent = false;
                    $.ajax(this);
                    return;
                }

                completedOfferCount++;
                if (completedOfferCount < totalOfferCount) {
                    updateSweetAlertText("Teklifleriniz alınıyor...", "Lütfen bekleyiniz.\n Durum : " + completedOfferCount + " / " + totalOfferCount);
                }
                else {
                    if (isErrorPresent) {
                        showErrorSweetAlert("Teklif alımı sırasında hata oluştu!", "Hatalı kayıtlar mevcut.");
                    }
                    else {
                        $("#btn-policelestir").show();
                        showConfirmSweetAlert("Teklif alımı tamamlandı.", "");
                    }
                }
            }
        });
    });
});

var nipponApproveButton = $("#btn-policelestir");
nipponApproveButton.click(function() {

    var totalTryPrice = parseFloat($("#nipponTotalPriceLabel").attr("data-total-try-price"));
    Swal.fire({
        title: "Devam etmek istiyor musunuz ?",
        text: "İşlem sonucunda kartınızdan toplamda " + totalTryPrice + " TL tutarında çekim yapılacaktır.",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Evet",
        cancelButtonText: "Hayır",
    }).then(function(result) {
        if (result.value) {
            console.log("confirm1");
            $("#kk-tutar").text(totalTryPrice + " TRY");
            $("#kk-odeme").modal("show");
        } else {
            console.log("deny1");
        }
    });
});

var creditCardPaymentConfirmButton = $("#kk-odeme-confirm");
creditCardPaymentConfirmButton.hover(function() {
    if ($("#creditCardHolderFirstName").val() === "") {
        showInfoSweetAlert("Kredi kartı sahibi adını giriniz.", "", true);
        return;
    }
    else if ($("#creditCardHolderLastName").val() === "") {
        showInfoSweetAlert("Kredi kartı sahibi soyadını giriniz.", "", true);
        return;
    }
    else if ($("#creditCardNumber").val() === "") {
        showInfoSweetAlert("Kredi kartı numarasını giriniz.", "", true);
        return;
    }
    else if ($("#creditCardCVVCode").val() === "") {
        showInfoSweetAlert("Kredi kartı güvenlik kodunu giriniz.", "", true);
        return;
    }
});

creditCardPaymentConfirmButton.click(function() {
    policyApprovalConfirmed();
});

function policyApprovalConfirmed() {
    var nipponInsuredRows = $('#nipponInsuredTable > tbody  > tr');
    var completedPolicyCount = 0;
    var totalPolicyCount = nipponInsuredRows.length;

    nipponInsuredRows.each(function() {
        var currentRow = $(this);
        var nipponPolicyStatus = $(this).find("td.nipponPolicyStatus");
        var isErrorPresent = false;
        var isNetworkErrorPresent = false;

        var insuredJson = getNipponSelectListValuesJson();
        insuredJson["TrackingCode"] = currentRow.attr("data-tracking-code");
        insuredJson["ClientNo"] = currentRow.attr("data-client-no");
        insuredJson["UnitNo"] = currentRow.attr("data-unit-no");
        insuredJson["PolicyNo"] = currentRow.attr("data-policy-no");
        insuredJson["CreditCard"] = getCreditCardCredentialsJson();

        showInfoSweetAlert("Poliçeleşme işlemleri başlatıldı...", "Lütfen bekleyiniz.\n Durum : 0 / " + totalPolicyCount, false, "/Content/img/loading9.gif");

        $.ajax({
            url: "/Teklif/NipponSeyahat/Approve",
            type: "POST",
            dataType: "json",
            data: { "insuredJson": JSON.stringify(insuredJson) },
            beforeSend: function() {
                nipponPolicyStatus.html('Poliçeleşme işlemi başlatıldı...');
            },
            success: function(JSON) {
                debugger;
                var okIconHtml = '<i class="icon-ok-sign" style="color:green"></i>';
                var errorIconHtml = '<i class="icon-warning-sign" style="color:red"></i>';

                if (JSON.IsSuccess) {
                    nipponPolicyStatus.html("Poliçe başarıyla satın alındı." + " " + okIconHtml);
                }
                else {
                    nipponPolicyStatus.html('Hata... <br>' + JSON.StatusDescription + " " + errorIconHtml);
                    isErrorPresent = true;
                }
            },
            error: function() {
                //nipponPolicyStatus.html("Sunucu ile iletişimde hata oluştu...");
                isNetworkErrorPresent = true;
            },
            complete: function() {
                debugger;
                if (isNetworkErrorPresent) {
                    isNetworkErrorPresent = false;
                    $.ajax(this);
                    return;
                }

                completedPolicyCount++;
                if (completedPolicyCount < totalPolicyCount)
                    updateSweetAlertText("Poliçeleşme işlemleri başlatıldı...", "Lütfen bekleyiniz.\n Durum : " + completedPolicyCount + " / " + totalPolicyCount);
                else {
                    if (isErrorPresent) {
                        showErrorSweetAlert("Poliçeleşme işlemi sırasında hata oluştu!", "Hatalı kayıtlar mevcut.");
                    }
                    else {
                        //showConfirmSweetAlert("Poliçeleşme işlemleri tamamlandı.", "");
                        getTurkishNipponPolicyPdfFiles();
                    }
                }
            }
        });
    });
}

function getTurkishNipponPolicyPdfFiles() {
    var insuredsJson = {};
    var nipponInsuredRows = $('#nipponInsuredTable > tbody  > tr');

    var IsDomestic = $("#nipponIsDomesticList").val() === "true" ? true : false;
    var completedPolicyPdfCount = 0;
    var totalPolicyPdfCount = nipponInsuredRows.length;

    nipponInsuredRows.each(function() {
        var currentRow = $(this);
        var nipponPolicyPdfStatus = $(this).find("td.nipponPolicyPdfStatus");
        var turkishPDFDownloadButton = nipponPolicyPdfStatus.find(".turkish-pdf-button");
        var isErrorPresent = false;
        var isNetworkErrorPresent = false;

        var insuredJson = getNipponSelectListValuesJson();
        insuredJson["TrackingCode"] = currentRow.attr("data-tracking-code");
        insuredJson["ClientNo"] = currentRow.attr("data-client-no");
        insuredJson["UnitNo"] = currentRow.attr("data-unit-no");
        insuredJson["PolicyNo"] = currentRow.attr("data-policy-no");
        insuredJson["PrintType"] = 1; // Türkçe PDF TürkNippon Kodu
        insuredJson["CreditCard"] = getCreditCardCredentialsJson();

        showInfoSweetAlert("Türkçe Poliçe PDF dosyalarınız alınıyor...", "Lütfen bekleyiniz.\n Durum : " + completedPolicyPdfCount + " / " + totalPolicyPdfCount, false, "/Content/img/loading9.gif");

        $.ajax({
            url: "/Teklif/NipponSeyahat/GetPolicyPDFFile",
            type: "POST",
            dataType: "json",
            data: { "insuredJson": JSON.stringify(insuredJson) },
            beforeSend: function() {
                //nipponPolicyPdfStatus.html('Türkçe Poliçe PDF dosyalarınız alınıyor...');
            },
            success: function(JSON) {
                var okIconHtml = '<i class="icon-ok-sign" style="color:green"></i>';
                var errorIconHtml = '<i class="icon-warning-sign" style="color:red"></i>';

                if (JSON.IsSuccess) {
                    //nipponPolicyPdfStatus.html(JSON.StatusDescription + " " + okIconHtml);
                    currentRow.attr("data-turkish-print-download-url", JSON.PrintDownloadUrl);
                    currentRow.find(".turkishPdfButton").attr("href", JSON.PrintDownloadUrl);
                    currentRow.find(".turkishPdfButton").attr("style", "display: normal");
                }
                else {
                    nipponPolicyPdfStatus.html('Hata... <br>' + JSON.StatusDescription + " " + errorIconHtml);
                    isErrorPresent = true;
                }
            },
            error: function() {
                //nipponPolicyPdfStatus.html("Sunucu ile iletişimde hata oluştu...");
                isNetworkErrorPresent = true;
            },
            complete: function() {
                if (isNetworkErrorPresent) {
                    isNetworkErrorPresent = false;
                    $.ajax(this);
                    return;
                }

                completedPolicyPdfCount++;
                if (completedPolicyPdfCount < totalPolicyPdfCount)
                    updateSweetAlertText("Türkçe Poliçe PDF dosyalarınız alınıyor...", "Lütfen bekleyiniz.\n Durum : " + completedPolicyPdfCount + " / " + totalPolicyPdfCount);
                else {
                    if (IsDomestic) {
                        createNipponPolicyRecords(isErrorPresent);
                    } else {
                        getEnglishNipponPolicyPdfFiles();
                    }
                }
            }
        });
    });
}

function getEnglishNipponPolicyPdfFiles() {
    var insuredsJson = {};
    var nipponInsuredRows = $('#nipponInsuredTable > tbody  > tr');

    var completedPolicyPdfCount = 0;
    var totalPolicyPdfCount = nipponInsuredRows.length;

    nipponInsuredRows.each(function() {
        var currentRow = $(this);
        var nipponPolicyPdfStatus = $(this).find("td.nipponPolicyPdfStatus");
        var isErrorPresent = false;
        var isNetworkErrorPresent = false;

        var insuredJson = getNipponSelectListValuesJson();
        insuredJson["TrackingCode"] = currentRow.attr("data-tracking-code");
        insuredJson["ClientNo"] = currentRow.attr("data-client-no");
        insuredJson["UnitNo"] = currentRow.attr("data-unit-no");
        insuredJson["PolicyNo"] = currentRow.attr("data-policy-no");
        insuredJson["PrintType"] = 4; // İngilizce PDF TürkNippon Kodu
        insuredJson["CreditCard"] = getCreditCardCredentialsJson();

        showInfoSweetAlert("İngilizce Poliçe PDF dosyalarınız alınıyor...", "Lütfen bekleyiniz.\n Durum : " + completedPolicyPdfCount + " / " + totalPolicyPdfCount, false, "/Content/img/loading9.gif");

        $.ajax({
            url: "/Teklif/NipponSeyahat/GetPolicyPDFFile",
            type: "POST",
            dataType: "json",
            data: { "insuredJson": JSON.stringify(insuredJson) },
            beforeSend: function() {
                //nipponPolicyPdfStatus.html('İngilizce Poliçe PDF dosyalarınız alınıyor...');
            },
            success: function(JSON) {
                var okIconHtml = '<i class="icon-ok-sign" style="color:green"></i>';
                var errorIconHtml = '<i class="icon-warning-sign" style="color:red"></i>';

                if (JSON.IsSuccess) {
                    //nipponPolicyPdfStatus.html(JSON.StatusDescription + " " + okIconHtml);
                    currentRow.attr("data-english-print-download-url", JSON.PrintDownloadUrl);
                    currentRow.find(".englishfPdfButton").attr("href", JSON.PrintDownloadUrl);
                    currentRow.find(".englishfPdfButton").attr("style", "margin-left: 10px;display: normal");
                }
                else {
                    nipponPolicyPdfStatus.html('Hata... <br>' + JSON.StatusDescription + " " + errorIconHtml);
                    isErrorPresent = true;
                }
            },
            error: function() {
                //nipponPolicyPdfStatus.html("Sunucu ile iletişimde hata oluştu...");
                isNetworkErrorPresent = true;
            },
            complete: function() {
                if (isNetworkErrorPresent) {
                    isNetworkErrorPresent = false;
                    $.ajax(this);
                    return;
                }

                completedPolicyPdfCount++;
                if (completedPolicyPdfCount < totalPolicyPdfCount)
                    updateSweetAlertText("İngilizce Poliçe PDF dosyalarınız alınıyor...", "Lütfen bekleyiniz.\n Durum : " + completedPolicyPdfCount + " / " + totalPolicyPdfCount);
                else {
                    createNipponPolicyRecords(isErrorPresent);
                }
            }
        });
    });
}


function createNipponPolicyRecords(isPdfErrorPresent) {
    var nipponInsuredRows = $('#nipponInsuredTable > tbody  > tr');
    var completedPolicyCount = 0;

    var totalPolicyCount = nipponInsuredRows.length;

    nipponInsuredRows.each(function() {
        var currentRow = $(this);
        var isNetworkErrorPresent = false;
        var insuredsJson = getNipponSelectListValuesJson();
        var IsDomestic = $("#nipponIsDomesticList").val() === "true" ? true : false;


        insuredsJson["PreparerTVMCode"] = $("#Hazirlayan_TVMKodu").val();
        insuredsJson["PreparerTVMUserCode"] = $("#Hazirlayan_TVMKullaniciKodu").val();
        insuredsJson["InsurerCustomerCode"] = currentRow.attr("data-customer-code");//isSelfInsured ? currentRow.attr("data-customer-code") : $("#Musteri_SigortaEttiren_MusteriKodu").val();

        insuredsJson["TrackingCode"] = currentRow.attr("data-tracking-code");
        insuredsJson["ClientNo"] = currentRow.attr("data-client-no");
        insuredsJson["UnitNo"] = currentRow.attr("data-unit-no");
        insuredsJson["PolicyNo"] = currentRow.attr("data-policy-no");
        insuredsJson["Premium"] = currentRow.attr("data-premium");
        insuredsJson["ExchangeRate"] = currentRow.attr("data-exchange-rate");

        if (!isPdfErrorPresent) {
            insuredsJson["TurkishPolicyPdfUrl"] = currentRow.attr("data-turkish-print-download-url");
            if (!IsDomestic) {
                insuredsJson["EnglishPolicyPdfUrl"] = currentRow.attr("data-english-print-download-url");
            }
        }
        else {
            insuredsJson["EnglishPolicyPdfUrl"] = "null";
            insuredsJson["TurkishPolicyPdfUrl"] = "null";
        }
        insuredsJson["InsuredCount"] = 1;
        insuredsJson["IsMergedPolicy"] = false;

        var isSelfInsured = $("#nipponSelfInsuredButton").is(":checked");
        var insurerIdentityNo = isSelfInsured ? currentRow.find("input").val() : $("#Musteri_SigortaEttiren_KimlikNo").val();

        var insureds = [];
        insureds.push({
            "CustomerCode": currentRow.attr("data-customer-code"),
            "InsurerIdentityNo": insurerIdentityNo
        });
        insuredsJson["Insureds"] = insureds;

        showInfoSweetAlert("Poliçeleriniz sisteme kaydediliyor...", "Lütfen bekleyiniz.\n Durum : " + completedPolicyCount + " / " + totalPolicyCount, false, "/Content/img/loading9.gif");

        $.ajax({
            url: "/Teklif/NipponSeyahat/SavePolicy",
            type: "POST",
            dataType: "json",
            data: { "insuredsJson": JSON.stringify(insuredsJson) },
            error: function() {
                isNetworkErrorPresent = true;
            },
            complete: function() {
                if (isNetworkErrorPresent) {
                    isNetworkErrorPresent = false;
                    $.ajax(this);
                    return;
                }

                completedPolicyCount++;
                if (completedPolicyCount < totalPolicyCount)
                    updateSweetAlertText("Poliçeleriniz sisteme kaydediliyor...", "Lütfen bekleyiniz.\n Durum : " + completedPolicyCount + " / " + totalPolicyCount);
                else {
                    if (totalPolicyCount > 1) {
                        saveTurkishNipponMergedPdfFile();
                    }
                    else {
                        showConfirmSweetAlert("Poliçe kayıt işlemi tamamlandı.", "Poliçeyi indirmek için 'Poliçe PDF İndir' butonuna basınız.");
                        $("#btn-hesapla").hide();
                        $("#btn-policelestir").hide();
                        $("#btn-pdfindir").html('Poliçe PDF İndir <i class="fas fa-download"></i>');
                        $("#btn-pdfindir").attr("href", currentRow.attr("data-print-download-url"));
                        $("#btn-pdfindir").show();

                        var win = window.open(currentRow.attr("data-print-download-url"), '_blank');
                        if (win) {
                            //Browser has allowed it to be opened
                            win.focus();
                        } else {
                            //Browser has blocked it
                            alert('Lütfen poliçeyi indirmek için popuplara izin veriniz.');
                        }
                    }
                }
            }
        });
    });
}

function saveTurkishNipponMergedPdfFile() {
    var pdfFilesArray = [];
    var jsonObject = {};
    var nipponInsuredRows = $('#nipponInsuredTable > tbody  > tr');

    nipponInsuredRows.each(function() {
        var pdfFile = {};
        var currentRow = $(this);

        pdfFile["Url"] = currentRow.attr("data-turkish-print-download-url");
        pdfFilesArray.push(pdfFile);
    });

    jsonObject["PdfFilesArray"] = pdfFilesArray;
    jsonObject["IsDomestic"] = $("#nipponIsDomesticList").val() === "true" ? true : false;

    $.ajax({
        url: "/Teklif/NipponSeyahat/MergePDFFiles",
        type: "POST",
        dataType: "json",
        data: { "jsonObject": JSON.stringify(jsonObject) },
        beforeSend: function() {
            showInfoSweetAlert("Poliçe toplu PDF dosyanız kaydediliyor...", "Lütfen bekleyiniz.");
        },
        success: function(JSON) {
            if (JSON.IsSuccess) {
                $("#btn-turkce-pdfindir").attr("href", JSON.MergedPdfUrl);
                $("#btn-turkce-pdfindir").attr("style", "display: normal;");

                var isDomestic = $("#nipponIsDomesticList").val() === "true" ? true : false;
                if (isDomestic) {
                    saveNipponMergedPolicy(JSON.MergedPdfUrl);
                } else {
                    saveEnglishNipponMergedPdfFile();
                }
            }
            else {
                showErrorSweetAlert("Poliçe toplu PDF dosyası kaydedilirken hata oluştu...");
            }
        },
        error: function() {
            showErrorSweetAlert("Poliçe toplu Türkçe PDF dosyası kaydedilirken hata oluştu...");
        },
        complete: function() {
            var isDomestic = $("#nipponIsDomesticList").val() === "true" ? true : false;

            if (isDomestic) {
                showConfirmSweetAlert("Poliçe kayıt işlemi tamamlandı.", "Toplu poliçeyi indirmek için 'Toplu Poliçe PDF İndir' butonuna basınız.");
                $("#btn-hesapla").hide();
                $("#btn-policelestir").hide();
                $("#btn-turkce-pdfindir").show();
            }
        }
    });
}

function saveEnglishNipponMergedPdfFile() {
    var pdfFilesArray = [];
    var jsonObject = {};
    var nipponInsuredRows = $('#nipponInsuredTable > tbody  > tr');

    nipponInsuredRows.each(function() {
        var pdfFile = {};
        var currentRow = $(this);

        pdfFile["Url"] = currentRow.attr("data-english-print-download-url");
        pdfFilesArray.push(pdfFile);
    });

    jsonObject["PdfFilesArray"] = pdfFilesArray;
    jsonObject["IsDomestic"] = $("#nipponIsDomesticList").val() === "true" ? true : false;

    $.ajax({
        url: "/Teklif/NipponSeyahat/MergePDFFiles",
        type: "POST",
        dataType: "json",
        data: { "jsonObject": JSON.stringify(jsonObject) },
        beforeSend: function() {
            showInfoSweetAlert("İngilizce Poliçe toplu PDF dosyanız kaydediliyor...", "Lütfen bekleyiniz.");
        },
        success: function(JSON) {
            if (JSON.IsSuccess) {
                $("#btn-ingilizce-pdfindir").attr("href", JSON.MergedPdfUrl);
                $("#btn-ingilizce-pdfindir").attr("style", "display: normal;");


                saveNipponMergedPolicy($("#btn-turkce-pdfindir").attr("href"), JSON.MergedPdfUrl);
            }
            else {
                showErrorSweetAlert("İngilizce Poliçe toplu PDF dosyası kaydedilirken hata oluştu...");
            }
        },
        error: function() {
            showErrorSweetAlert("İngilizce Poliçe toplu PDF dosyası kaydedilirken hata oluştu...");
        },
        complete: function() {
            $("#btn-hesapla").hide();
            $("#btn-policelestir").hide();
            $("#btn-ingilizce-pdfindir").show();
            $("#btn-turkce-pdfindir").show();
        }
    });
}


function saveNipponMergedPolicy(mergedPdfUrl, englishMergedPdfUrl) {
    var insuredsJson = getNipponSelectListValuesJson();
    var isSelfInsured = $("#nipponSelfInsuredButton").is(":checked");

    insuredsJson["PreparerTVMCode"] = $("#Hazirlayan_TVMKodu").val();
    insuredsJson["PreparerTVMUserCode"] = $("#Hazirlayan_TVMKullaniciKodu").val();

    var insureds = [];
    var nipponInsuredRows = $('#nipponInsuredTable > tbody  > tr');
    insuredsJson["InsurerCustomerCode"] = $(nipponInsuredRows[0]).attr("data-customer-code");
    nipponInsuredRows.each(function() {
        var currentRow = $(this);
        insureds.push({ "CustomerCode": currentRow.attr("data-customer-code") });
    });

    insuredsJson["Insureds"] = insureds;
    insuredsJson["InsuredCount"] = nipponInsuredRows.length;
    insuredsJson["TurkishPolicyPdfUrl"] = mergedPdfUrl;
    insuredsJson["EnglishPolicyPdfUrl"] = englishMergedPdfUrl;
    insuredsJson["PolicyNo"] = "0";
    insuredsJson["Premium"] = "0";
    insuredsJson["ExchangeRate"] = "0";
    insuredsJson["IsMergedPolicy"] = true;

    $.ajax({
        url: "/Teklif/NipponSeyahat/SavePolicy",
        type: "POST",
        dataType: "json",
        data: { "insuredsJson": JSON.stringify(insuredsJson) },
        beforeSend: function() {
            showInfoSweetAlert("Toplu poliçeniz kaydediliyor...", "Lütfen bekleyiniz.");
        },
        success: function(JSON) {
            showConfirmSweetAlert("Toplu poliçe kayıt işlemi tamamlandı.", "Toplu poliçeyi indirmek için 'Toplu Poliçe PDF İndir' butonuna basınız.");
            $("#btn-hesapla").hide();
            $("#btn-policelestir").hide();
            $("#btn-turkce-pdfindir").show();
            $("#btn-turkce-pdfindir").attr("href", mergedPdfUrl);

            var isDomestic = $("#nipponIsDomesticList").val() === "true" ? true : false;
            if (!isDomestic) {
                $("#btn-ingilizce-pdfindir").show();
                $("#btn-ingilizce-pdfindir").attr("href", englishMergedPdfUrl);
            }
        },
        error: function() {
            showErrorSweetAlert("Toplu poliçeniz kaydedilirken hata oluştu...");
        },
        complete: function() {
            showConfirmSweetAlert("Toplu poliçe kayıt işlemi tamamlandı.", "Toplu poliçeyi indirmek için 'Toplu Poliçe PDF İndir' butonuna basınız.");
            $("#btn-hesapla").hide();
            $("#btn-policelestir").hide();
            $("#btn-pdfindir").show();
        }
    });
}

//insuredsJson["policiesPDFUrl"] = $("#policiesPDFUrl");


function showInfoSweetAlert(title, text = "", showConfirmButton = false, imageUrl = null) {
    Swal.fire({
        title: title,
        text: text,
        type: "info",
        imageUrl: imageUrl,
        showCancelButton: false,
        imageWidth: 32,
        imageHeight: 32,
        showConfirmButton: showConfirmButton,
        closeOnConfirm: false,
        allowOutsideClick: false
    });
}

function updateSweetAlertText(title, text) {
    Swal.getTitle().textContent = title;
    Swal.getContent().textContent = text;
}

function showConfirmSweetAlert(title, text) {
    Swal.fire({
        title: title,
        text: text,
        type: "success",
        showCancelButton: false,

        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Tamam",
        closeOnConfirm: true
    });
}

function showErrorSweetAlert(title, text) {
    Swal.fire({
        title: title,
        text: text,
        type: "error",
        showCancelButton: false,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Tamam",
        closeOnConfirm: true
    });
}

function validateNipponSelectLists() {
    debugger;
    if ($("#nipponIsDomesticList").val() === "") {
        showInfoSweetAlert("Seyahat Tipini Seçiniz.", "", true);
        return -1;
    } else if (nipponScopeList.val() === "") {
        showInfoSweetAlert("Kapsam bilgisi seçiniz.", "", true);
        return -2;
    } else if (nipponAlternativeList.val() === "") {
        showInfoSweetAlert("Alternatif bilgisi seçiniz.", "", true);
        return -3;
    } else if (nipponCountryList.val() === "") {
        showInfoSweetAlert("Seyahat edilecek ülkeyi seçiniz.", "", true);
        return -4;
    }
    else if ($("#nipponIsDomesticList").val() === "false" && nipponPlanCodeList.val() === "") {
        showInfoSweetAlert("Plan kodunu seçiniz.", "", true);
        return -5;
    }
    else
        return 0;
}

function validateTravelInputs() {
    if ($("#GenelBilgiler_SeyehatBaslangicTarihi").val() === "") {
        showInfoSweetAlert("Seyahat başlangıç tarihini seçiniz.", "", true);
        return -1;
    } else if ($("#GenelBilgiler_SeyehatBitisTarihi").val() === "") {
        showInfoSweetAlert("Seyahat bitiş tarihini seçiniz.", "", true);
        return -2;
    }
}

function validateTCKNo(nipponInsuredRows) {
    var errorPresent = false;
    nipponInsuredRows.each(function() {
        if ($(this).find("input").hasClass("error")) {
            showInfoSweetAlert("Hatalı TC Numaralarını düzeltiniz.", "", true);
            $(this).find("input").focus();
            errorPresent = true;
            return false;
        }
    });
    return errorPresent ? -1 : 0;
}

function updateTotalPriceLabel() {
    debugger;
    var nipponTotalPriceLabel = $("#nipponTotalPriceLabel");
    var totalTryPrice = 0;
    var totalEurPrice = 0;

    var nipponInsuredRows = $('#nipponInsuredTable > tbody  > tr');
    nipponInsuredRows.each(function() {
        if (!$(this).attr("data-premium") || !$(this).attr("data-exchange-rate")) {
            return;
        }
        var exchangeRate = parseFloat($(this).attr("data-exchange-rate"));
        var premium = parseFloat($(this).attr("data-premium"));

        if (exchangeRate === 1.0) {
            totalTryPrice += premium;
        }
        else {
            totalTryPrice += premium * exchangeRate;
            totalEurPrice += premium;
        }
    });

    if (totalEurPrice === 0) {
        nipponTotalPriceLabel.text("Toplam Tutar : " + totalTryPrice.toFixed(2) + " ₺");
    }
    else {
        nipponTotalPriceLabel.text("Toplam Tutar : " + totalTryPrice.toFixed(2) + " ₺ | " + totalEurPrice.toFixed(2) + " €");
    }
    nipponTotalPriceLabel.attr("data-total-try-price", totalTryPrice.toFixed(2));
}

function resetOffers() {
    var nipponTotalPriceLabel = $("#nipponTotalPriceLabel");
    nipponTotalPriceLabel.text("Toplam Tutar: 0 TRY | 0 EUR");

    var nipponInsuredRows = $('#nipponInsuredTable > tbody  > tr');
    nipponInsuredRows.each(function() {
        var nipponStatus = $(this).find("td.nipponStatus");
        var nipponPolicyStatus = $(this).find("td.nipponPolicyStatus");
        nipponPolicyStatus.text("");
        nipponStatus.text("");
        $(this).removeAttr("data-premium");
        $(this).removeAttr("data-exchange-rate");
    });

    $("#btn-policelestir").hide();
}

function getNipponSelectListValuesJson() {
    var insuredJson = {};
    var BeginDate = $("#GenelBilgiler_SeyehatBaslangicTarihi").val();
    var EndDate = $("#GenelBilgiler_SeyehatBitisTarihi").val();
    var IsDomestic = $("#nipponIsDomesticList").val() === "true" ? true : false;
    var PlanCode = $("#nipponPlanCodeList").val();
    var Scope = $("#nipponScopeList").val();
    var TravelPocket = IsDomestic ? "1" : Scope;
    var Alternative = $("#nipponAlternativeList").val();
    var Country = $("#nipponCountryList").val();
    var IsSkiing = $("#GenelBilgiler_KayakTeminati").parent().hasClass("switcher-on");

    insuredJson["BeginDate"] = BeginDate;
    insuredJson["EndDate"] = EndDate;
    insuredJson["IsDomestic"] = IsDomestic;
    insuredJson["PlanCode"] = PlanCode;
    insuredJson["Scope"] = Scope;
    insuredJson["TravelPocket"] = TravelPocket;
    insuredJson["Alternative"] = Alternative;
    insuredJson["Country"] = Country;
    insuredJson["IsSkiing"] = IsSkiing;
    return insuredJson;
}

function getCreditCardCredentialsJson() {
    var creditCard = {};
    creditCard["CardNumber"] = $("#creditCardNumber").val();
    creditCard["Month"] = $("#creditCardMonth").val();
    creditCard["Year"] = $("#creditCardYear").val();
    creditCard["CVV"] = $("#creditCardCVVCode").val();
    creditCard["CardHolderFirstname"] = $("#creditCardHolderFirstName").val();
    creditCard["CardHolderLastname"] = $("#creditCardHolderLastName").val();

    return creditCard;
}

var cleave = new Cleave('#creditCardNumber', {
    creditCard: true,
    onCreditCardTypeChanged: function(type) {
        debugger;
        // update UI ...
    }
});

$("#btn-pdfindir").click(function() {
    //window.location.href = $("#policiesMergedPDFUrl").val();
});

var tckNoListConfirmButton = $("#tckNoListConfirm");
var tckNoListContent = $("#tckNoListContent");
tckNoListConfirmButton.click(function() {
    debugger;
    $("#nipponInsuredTable tbody").empty();
    nipponInsuredRowCount = 0;
    var tckNoArray = tckNoListContent.val().split('\n');
    $.each(tckNoArray, function(i, tckNo) {
        addInsured(tckNo);
    });

    var nipponInsuredRows = $('#nipponInsuredTable > tbody  > tr');
    nipponInsuredRows.each(function() {
        $(this).find("input").keyup();
    });
});

//$("#tckNoListContent").on('paste', function () {
//    var element = this;
//    setTimeout(function () {
//        var text = $(element).val();
//        text = text.replace(/ /g, "\n");
//        element.text(text);
//    }, 100);
//});
