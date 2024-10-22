function ETeklifWizardCallback(current) {
    //Hazırlayan bilgileri
    if (current == 1) {

    }
        //Sigorta ettiren / sigortali tab
    else if (current == 2) {
        var isValid = FormWizard.validatePage('#tab2');

        if (isValid == 1) {
            return Egitim.musteriBilgileriKontrol()
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
    Egitim.musteriZorunlulukKaldir();
    Egitim.TekrarTeklifAcilisKontrol();

    $("#Musteri_SigortaEttiren_UlkeKodu").ulke({ il: "#Musteri_SigortaEttiren_IlKodu", ilce: "#Musteri_SigortaEttiren_IlceKodu" });

    //Doğum Tarihi Sınırlaması
    $("#Musteri_SigortaEttiren_DogumTarihi").datepicker("option", "maxDate", '-216m +0w');
    $("#Musteri_SigortaEttiren_DogumTarihi").datepicker("option", "minDate", '-792m +0w');

    //Sigorta Başlangıç tarihi
    $("#GenelBilgiler_SigortaBaslangicTarihi").datepicker("option", "minDate", '+0m +0w');

    // ==== Para Formatı Belirleniyor.
    $(".autoNumeric").autoNumeric('init', { vMin: '0', vMax: '999999999', mDec: 0 });
    $(".autoNumeric-custom").autoNumeric('init', { vMin: '0', vMax: '999999999', mDec: 2, aSep: ',', aDec: '.' });
});

$("#btn-hesapla").click(function () {
    var isvalid = $("#form1").valid();
    var form3 = $("#form3").valid();

    if (isvalid && form3) {
        $(this).button("loading");
        if (Egitim.SigortaBilgileriKontrol()) {

            $(".switcher").find(":input").switchFix();

            $(".autoNumeric,.autoNumeric-custom").each(function () {
                $(this).val($(this).autoNumeric('get'));
            });

            var contents = $("#form1, #form2, #form3").serialize();

            $(".detay-partial-div").html('');

            $.ajax(
                {
                    type: "POST",
                    url: "/Teklif/Egitim/Hesapla",
                    data: contents,
                    success: function (data) {
                        if (data.id > 0) {
                            teklifFiyat.kontol({ processId: data.id, guid: data.g });

                            Egitim.EgitimDetayPartialGetir(data.id);

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

var Egitim = new function () {
    return {

        init: function (data) {

        },

        TekrarTeklifAcilisKontrol: function () {

            Egitim.musteriZorunlulukKaldir();
        },

        musteriBilgileriKontrol: function () {

            var Message = "";

            //Cinsiyet Kontrol
            if (!$("#Cinsiyet_K").is(':checked') && !$("#Cinsiyet_E").is(':checked')) {
                Message = "<p>--Sigortalı cinsiyeti seçiniz.</p>";
            }

            // Gelir Vergisi Kontrol
            var gelirvergisi = $("#Musteri_SigortaEttiren_GelirVergisiOrani").val();
            if (gelirvergisi == "") {
                Message += "<p>--Lütfen beyan edilen gelir vergisi oranı giriniz.</p>";
            }

            //YAŞ
            var today = new Date();
            var dogum = $("#Musteri_SigortaEttiren_DogumTarihi").datepicker("getDate");
            today.setYear(today.getFullYear() - 18);
            if (dogum > today)
                Message += "<p> --Sigortalının yaşı 18 ile 65 arasında olmalıdır.</p>";

            today = new Date();
            today.setYear(today.getFullYear() - 66);
            if (dogum < today)
                Message += "<p> --Sigortalının yaşı 18 ile 65 arasında olmalıdır.</p>";


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

        SigortaBilgileriKontrol: function () {

            var Message = "";
            Message += Egitim.sigortaSuresiKontrol();
            Message += Egitim.sigortaGeriOdemeSuresiKontrol();
            Message += Egitim.yillikTutarKontrol();
            Message += Egitim.sigortaBasTarKontrol();

            if (Message == "")
                return true;
            else {
                $("#hata-message-div").html(Message);
                $("#hata-modal").modal("show");
                return false;
            }
        },

        sigortaSuresiKontrol: function () {
            var Message = "";
            var sigortaSuresi = $("#GenelBilgiler_SigortaSuresi").val();
            if (sigortaSuresi != "") {
                $.ajax({
                    url: "/Teklif/Egitim/GetSigortaSuresiLimit",
                    data: "",
                    async: false,
                    method: "post",
                    success: function (response) {
                        if (response.minlimit != "" && response.minlimit > sigortaSuresi || response.maxlimit < sigortaSuresi) {
                            Message = "<p>-- Sigorta süresi  " + response.minlimit + " - " + response.maxlimit + " arasında olmalıdır!</p>";
                        }
                    },
                    error: function () {
                        Message = "<p>-- Minimum prim tutarı getirilirken hata oluştu. Lütfen tekrar deneyin.</p>";
                    }
                });
                return Message;
            }
            return Message;
        },

        sigortaBasTarKontrol: function () {

            var date = $("#GenelBilgiler_SigortaBaslangicTarihi").datepicker("getDate");
            var today = new Date();
            date.setDate(date.getDate() + 1);

            if (date < today)
                return "Sigorta başlangıç tarihi olarak geçmiş bir tarih girilemez";
            else return "";
        },

        sigortaGeriOdemeSuresiKontrol: function () {
            var Message = "";
            var sigortaGeriOdemeSuresi = $("#GenelBilgiler_SigortaGeriOdemeSuresi").val();
            var sigortaSuresi = $("#GenelBilgiler_SigortaSuresi").val();
            if (sigortaGeriOdemeSuresi != "") {
                $.ajax({
                    url: "/Teklif/Egitim/GetSigortaGeriOdemeSuresiLimit",
                    data: { sigortaSuresi: sigortaSuresi },
                    async: false,
                    method: "post",
                    success: function (response) {
                        if (response.maxlimit != "" && sigortaGeriOdemeSuresi > response.maxlimit) {
                            Message = "<p>-- Geri Ödeme asgari " + response.minGeriOdemeErteleme + " yıl sonra başlayabilir. Sigorta Geri Ödeme Süresi olarak girilebilecek maksimum süre " + response.maxlimit + " yıldır!</p>";
                        }
                        if (response.minlimit != "" && sigortaGeriOdemeSuresi < response.minlimit) {
                            Message = "<p>-- Sigorta Geri Ödeme Süresi " + response.minlimit + " yıldan az olamaz!</p>";
                        }
                    },
                    error: function () {
                        Message = "<p>-- Minimum prim tutarı getirilirken hata oluştu. Lütfen tekrar deneyin.</p>";
                    }
                });
                return Message;
            }
            return Message;
        },

        yillikTutarKontrol: function () {
            var Message = "";
            var yillikTutar = $("#GenelBilgiler_GeriOdemelerdeAlinacakYillikTutar").autoNumeric('get');
            yillikTutar = parseInt(yillikTutar, 10);
            if (yillikTutar != "") {
                $.ajax({
                    url: "/Teklif/Egitim/GetAsgariAylikPrimLimiti",
                    data: { yillikTutar: yillikTutar },
                    async: false,
                    method: "post",
                    success: function (response) {
                        if (response.aylikTutar < response.aylikLimit) {
                            Message = "<p>--  Hesaplanan aylık prim tutarı olan " + response.aylikTutar + " ABD Doları, asgari aylık prim limiti olan "
                                + response.aylikLimit + " ABD Doları’ndan düşüktür. Lütfen Geri Ödemelerde Alınacak Yıllık Tutar için daha yüksek bir değer giriniz.</p>";
                        }
                    },
                    error: function () {
                        Message = "<p>-- Minimum prim tutarı getirilirken hata oluştu. Lütfen tekrar deneyin.</p>";
                    }
                });
                return Message;
            }
            return Message;
        },

        EgitimDetayPartialGetir: function (IsDurum_id) {
            $.ajax({
                url: "/Teklif/Egitim/_DetayPartial",
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
        }
    }
}
