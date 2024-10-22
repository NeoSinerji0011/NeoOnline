function odulluBirikimTeklifWizardCallback(current) {
    //Hazırlayan bilgileri
    if (current == 1) {

    }
        //Sigorta ettiren / sigortali tab
    else if (current == 2) {
        var isValid = FormWizard.validatePage('#tab2');

        if (isValid == 1) {
            return OdulluBirikim.musteriBilgileriKontrol()
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

    OdulluBirikim.TekrarTeklifAcilisKontrol();

    //Muşteri adres
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

$("#GenelBilgiler_HesaplamaSecenegi").change(function () {
    OdulluBirikim.HesaplamaSecenegiChange();
});

$("#btn-hesapla").click(function () {
    var isvalid = $("#form1").valid();
    var form3 = $("#form3").valid();
    if (isvalid && form3) {
        $(this).button("loading");
        if (OdulluBirikim.SigortaBilgileriKontrol()) {
            $(this).button("loading");

            $(".switcher").find(":input").switchFix();

            $(".autoNumeric,.autoNumeric-custom").each(function () {
                $(this).val($(this).autoNumeric('get'));
            });

            var contents = $("#form1, #form2, #form3").serialize();

            $(".detay-partial-div").html('');

            $.ajax(
                {
                    type: "POST",
                    url: "/Teklif/OdulluBirikim/Hesapla",
                    data: contents,
                    success: function (data) {
                        if (data.id > 0) {
                            teklifFiyat.kontol({ processId: data.id, guid: data.g });

                            OdulluBirikim.PrimDetayPartialGetir(data.id);

                            $("#step3group").css({ "visibility": "visible" });
                            $("#teklif-fiyat-container").css({ "visibility": "visible" });
                            $("#step1").collapse("hide");
                            $("#step2").collapse("hide");
                            $("#step3").collapse("show");

                            $("#fiyat-container").html($("#fiyat-container-template").html());
                            $('#form_wizard_1').bootstrapWizard("next");

                            $("#odeme-guvence-teklif-link").attr("href", "/Teklif/OdemeGuvence/EkleIlgili/" + data.tid);

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

var OdulluBirikim = new function () {
    return {

        init: function (data) {

        },

        TekrarTeklifAcilisKontrol: function () {
            OdulluBirikim.HesaplamaSecenegiChange();
            OdulluBirikim.musteriZorunlulukKaldir();
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
            var today = new Date();
            var dogum = $("#Musteri_SigortaEttiren_DogumTarihi").datepicker("getDate");
            today.setYear(today.getFullYear() - 18);
            if (dogum > today)
                Message += "<p> --Sigortalı yaşı 18 den küçük olamaz.</p>";

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

        SigortaBilgileriKontrol: function () {

            var Message = "";

            Message += OdulluBirikim.PrimTutariKontrol();
            Message += OdulluBirikim.SigortaSuresiKontrol();

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

        HesaplamaSecenegiChange: function () {
            var hesapTipi = $("#GenelBilgiler_HesaplamaSecenegi").val();
            var yillikPrimTutari = $("#GenelBilgiler_YillikPrimTutari").val();

            if (hesapTipi != "") {
                $("#ortak-prim").show();
                if (hesapTipi == 1) {
                    $("#yillikPrim").html("Yıllık Prim Tutarı <span class='required'>*</span>");
                    $("#span-prim-teminat").html("Vefat Teminat turarı");
                }
                else if (hesapTipi == 2) {
                    $("#yillikPrim").html("Birikim Tutarı <span class='required'>*</span>");
                    $("#span-prim-teminat").html("Prim (Yıllık / Dönem)");
                }
            }
            else { $("#ortak-prim").hide(); }
        },

        SigortaSuresiKontrol: function () {

            var Message = "";

            var sure = $("#GenelBilgiler_SigortaSuresi").val();
            var dogumTarihi = $("#Musteri_SigortaEttiren_DogumTarihi").val();
            var sigortaBaslangic = $("#GenelBilgiler_SigortaBaslangicTarihi").val();

            if (sure != "") {

                $.ajax({
                    url: "/Teklif/OdulluBirikim/SigortaSuresiHesapla",
                    data: { dogumTarihi: dogumTarihi, sigortaBaslangic: sigortaBaslangic, sure: sure },
                    method: "post",
                    async: false,
                    success: function (result) { if (result.Success == "false") { Message = "<p>-- " + result.Message + "</p>"; } },
                    error: function () { Message = "<p>-- Sigorta süresi hesaplanırken hata oluştu. Lütfen tekrar deneyin.</p>"; },
                });
            }

            return Message;
        },

        PrimTutariKontrol: function () {

            var Message = "";

            var primTutari = $("#GenelBilgiler_Ortak_PrimTutari").autoNumeric('get');
            if (primTutari != "") {
                $.ajax({
                    url: "/Teklif/OdulluBirikim/GetMinimumYillikPrimTutari",
                    data: { primTutari: primTutari },
                    method: "post",
                    async: false,
                    success: function (result) { if (result.Success == "false") { Message = "<p>-- " + result.Message + "</p>"; } },
                    error: function () {
                        Message = "<p>-- Minimum prim tutarı getirilirken hata oluştu. Lütfen tekrar deneyin.</p>";
                    }
                });
            }

            return Message;
        },

        PrimDetayPartialGetir: function (IsDurum_id) {
            $.ajax({
                url: "/Teklif/OdulluBirikim/_DetayPartial",
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
