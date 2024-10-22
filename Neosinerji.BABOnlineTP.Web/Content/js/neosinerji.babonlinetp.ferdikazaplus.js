var $Telefon1;
var $Adres;
var $Email;
var $KrediKarti_K1;
var $KrediKarti_K2;
var $KrediKarti_K3;
var $KrediKarti_K4;
var $SonKullanma_ay;
var $SonKullanma_yil;

function ferdiKazaPlusTeklifWizardCallback(current) {
    return true;
};

$(document).ready(function () {

    //$("#ilerlet").attr("disabled","disabled");
    $(".autoNumeric").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0 });

    var date1 = new Date();
    date1.setMonth(date1.getMonth() - 6);
    //Dogum tarihi 6 aydan küçük olamaz.
    $("#Lehtar_LehterList_0__DogumTarihi").datepicker("option", "maxDate", date1);
    $("#Lehtar_LehterList_1__DogumTarihi").datepicker("option", "maxDate", date1);
    $("#Lehtar_LehterList_2__DogumTarihi").datepicker("option", "maxDate", date1);
    $("#Lehtar_LehterList_3__DogumTarihi").datepicker("option", "maxDate", date1);
    $("#Lehtar_LehterList_4__DogumTarihi").datepicker("option", "maxDate", date1);

    //Sigorta Başlangıç tarihi
    $("#PrimOdeme_SigortaBaslangicTarihi").datepicker("option", "minDate", '+0m +0w');

    // No Validation
    ferdiKazaPlus.sigortali1();
    ferdiKazaPlus.sigortalicheck1();

    ferdiKazaPlus.SetStarTeminatKapali();

    //Kanuni Varisler
    $('#kisiSayisi').hide();
    $('.lehtar').hide();
  
    setTimeout(function () { $("#btn-hesapla").show() }, 1000);

    //var gunceltarih = new Date();
    $("#PrimOdeme_SigortaBaslangicTarihi").val($.datepicker.formatDate('dd.mm.yy', new Date()))
});

//Doküman Ekleme - Silme İşlemleri
$("#dokuman-ekle").click(function () {
    var teklifid = $("#TeklifId").val();
    $.get("/Teklif/FerdiKazaPlus/Dokuman",
          { teklifId: teklifid },
          function (data) {
              $("#dokuman-modal-div").html(data);
              $.validator.unobtrusive.parse("#dokuman-modal-div");
              $("#dokuman-modal").modal('show');
          },
          "html");
});

$("#dokuman-ekle-btn").live("click", function () {
    $("#dokuman-ekle-form").validate().form();
    if ($("#dokuman-ekle-form").valid()) {
        var formData = new FormData(document.forms.namedItem('dokuman-ekle-form'));
        var oReq = new XMLHttpRequest();
        oReq.open("POST", "/Teklif/FerdiKazaPlus/Dokuman", true);       
        oReq.send(formData);
        oReq.onload = function (oEvent) {
            $("#dokuman-modal").modal('hide');
            $("#dokuman-modal-div").html("");
            if (oReq.status == 200) {
                if (oReq.response != "" || oReq.response != null) {
                    $("#dokuman-modal-div").html(oReq.response);
                    $.validator.unobtrusive.parse("#dokuman-modal-div");
                    $("#dokuman-modal").modal('show');
                    alert("Döküman eklendi");
                }
                               
            } else if (oReq.status == 500) {
                $("#dokuman-modal").modal('hide');
                alert("Bir hata oluştu.");
            }
           
        };     
       
    }
   
});


//İlerlet
$("#ilerlet").click(function () {

    var TeklifId = $("#TeklifId").val();
    $(this).button("loading");
    $.ajax(
               {
                   url: "/Teklif/FerdiKazaPlus/Ilerlet",
                   data: { id: TeklifId },
                   dataType: "json",
                   async: false,
                   method: "post",
                   success: function (data) {
                       if (data.Success == "false")
                       {
                           confirm("Hata Mesajı!  Başvuru Formu eksik. Başvuru Formunu eklemeden ilerleyemezsiniz.");
                           $("#ilerlet").button("reset");
                       }
                       else
                       {
                           $.gritter.add({ title: 'Bilgi!', text: "Metlife Operasyon Grubuna iş takip bilgisi e-maili gönderildi", time: 3000 });

                           window.location.href = "/Teklif/FerdiKazaPlus/Islerim";
                           $("#ilerlet").button("reset");

                       }
                       $("#ilerlet").button("reset");
                   },
                   error: function () { $.gritter.add({ title: 'Hata Mesajı!', text: "İşlem Tamamlanmadı" }); $("#ilerlet").button("reset"); }

               });

    $("#ilerlet").button("reset");
});

$("#Teminatlar_teminatTutari").change(function () {

    var teminatVal = $(this).val();
    if (teminatVal == 1) {
        $("#Teminatlar_KazaSonucuVefatBedeli").val("75,000");
        $("#Teminatlar_kazaSonucuMaluliyetBedeli").val("75,000");
        $("#Teminatlar_SigortaPrimTutari").val("75");
    }
    else if (teminatVal == 2) {
        $("#Teminatlar_KazaSonucuVefatBedeli").val("100,000");
        $("#Teminatlar_kazaSonucuMaluliyetBedeli").val("100,000");
        $("#Teminatlar_SigortaPrimTutari").val("100");
    }
    else if (teminatVal == 3) {
        $("#Teminatlar_KazaSonucuVefatBedeli").val("150,000");
        $("#Teminatlar_kazaSonucuMaluliyetBedeli").val("150,000");
        $("#Teminatlar_SigortaPrimTutari").val("150");
    }
    else {
        $("#Teminatlar_KazaSonucuVefatBedeli").val("");
        $("#Teminatlar_kazaSonucuMaluliyetBedeli").val("");
        $("#Teminatlar_SigortaPrimTutari").val("");
    }
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

$("#sigortali-sorgula").click(function () {
    ferdiKazaPlus.sigortaliSorgula();
    ferdiKazaPlus.SigortaliBilgileriReadOnly();
});

$("#btn-hesapla").click(function () {
    var isvalid = $("#form1").valid();
    if (isvalid) {
        $(this).button("loading");
        $(".detay-partial-div").html('');
        $(".switcher").find(":input").switchFix();
        $(".autoNumeric").each(function () {
            $(this).val($(this).autoNumeric('get'));
        });
        $(".autoNumeric-custom").each(function () {
            $(this).val($(this).autoNumeric('get'));
        });

        //Enabled
        $('#Teminatlar_taksitSayisi').prop("disabled", false);
        var contents = $("#form1").serialize();

        $.ajax({
            type: "POST",
            url: "/Teklif/FerdiKazaPlus/Hesapla",
            data: contents,
            success: function (data) {
                if (data.id > 0) {
                    teklifFiyat.kontol({ processId: data.id, guid: data.g });

                    $("#fiyat-container").html($("#fiyat-container-template").html());
                    $('#form_wizard_1').bootstrapWizard("next");

                    //$("#step3group").css({ "visibility": "visible" });
                    $("#teklif-fiyat-container").css({ "visibility": "visible" });
                    $("#step1").collapse("hide");
                    $("#step2").collapse("show");

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
        $(".autoNumeric").autoNumeric('update', { vMin: '0', vMax: '9999999', mDec: 0 });

    }
    else { $("#btn-hesapla").button("reset"); alert("eksik bilgi") }
});

$(".upper-letter").blur(function () {
    $(this).val($(this).val().replace('i', 'İ'));
    $(this).val($(this).val().toUpperCase());
});

$('#Teminatlar_taksitSayisi').prop("disabled", true);


//Mevcut checked 
$('#Iletisim_AdresMevcut').click(function () {
    if (!$('#Iletisim_AdresMevcut').is(':checked')) {
        $("#Iletisim_AdresTipi").removeAttr('disabled');
        $("#Iletisim_Adres").prop('readonly', false);
        $("#Iletisim_Adres").val("");
    }
    else {
        $("#Iletisim_AdresTipi").attr("disabled", "disabled");
        $("#Iletisim_Adres").prop('readonly', true);
        $("#Iletisim_Adres").val($Adres);
    }
});

$('#Iletisim_EmailMevcut').click(function () {
    if (!$('#Iletisim_EmailMevcut').is(':checked')) {
        $("#Iletisim_Email").prop('readonly', false);
        $("#Iletisim_Email").val("");
    }
    else {
        $("#Iletisim_Email").prop('readonly', true);
        $("#Iletisim_Email").val($Email);
    }
});

$('#Iletisim_Tel1Mevcut').click(function () {
    if (!$('#Iletisim_Tel1Mevcut').is(':checked')) {
        $("#Iletisim_Tel1").prop('readonly', false);
        $("#Iletisim_Tel1").val("");
    }
    else {
        $("#Iletisim_Tel1").prop('readonly', true);
        $("#Iletisim_Tel1").val($Telefon1);
    }
});

$('#PrimOdeme_KartNoMevcut').click(function () {
    if (!$('#PrimOdeme_KartNoMevcut').is(':checked')) {
        $("#PrimOdeme_KartNo_KK1").prop('readonly', false);
        $("#PrimOdeme_KartNo_KK2").prop('readonly', false);
        $("#PrimOdeme_KartNo_KK3").prop('readonly', false);
        $("#PrimOdeme_KartNo_KK4").prop('readonly', false);

        $("#PrimOdeme_ay").removeAttr('disabled');
        $("#PrimOdeme_yil").removeAttr('disabled');

        $("#PrimOdeme_KartNo_KK1").val("");
        $("#PrimOdeme_KartNo_KK2").val("");
        $("#PrimOdeme_KartNo_KK3").val("");
        $("#PrimOdeme_KartNo_KK4").val("");

        $("#PrimOdeme_ay").val("");
        $("#PrimOdeme_yil").val("");
    }
    else {

        $("#PrimOdeme_KartNo_KK1").prop('readonly', true);
        $("#PrimOdeme_KartNo_KK1").val($KrediKarti_K1);
        $("#PrimOdeme_KartNo_KK2").prop('readonly', true);
        $("#PrimOdeme_KartNo_KK2").val($KrediKarti_K2);
        $("#PrimOdeme_KartNo_KK3").prop('readonly', true);
        $("#PrimOdeme_KartNo_KK3").val($KrediKarti_K3);
        $("#PrimOdeme_KartNo_KK4").prop('readonly', true);
        $("#PrimOdeme_KartNo_KK4").val($KrediKarti_K4);

        $("#PrimOdeme_ay").attr("disabled", "disabled");
        $("#PrimOdeme_yil").attr("disabled", "disabled");

        $("#PrimOdeme_ay").val($SonKullanma_ay);
        $("#PrimOdeme_yil").val($SonKullanma_yil);

    }
});


$("[name='Lehtar.Lehtar']").change(function () {
    var lehtar = $(this).val();

    if (lehtar == "1") {
        if ($('#kisiSayisi').val() == 1);
        {
            $("#Lehtar_LehterList_0__DogumTarihi").addClass("ignore");
        }
        $('#kisiSayisi').hide();
        $('#Lehtar_kisiSayisi').val("1");
        $('.lehtar').hide();
    }
    else {
        $('#kisiSayisi').show();
        ferdiKazaPlus.kisisayisi($("#Lehtar_kisiSayisi").val());
    }
});

$('#Teminatlar_OdemeSecenegi').change(function () {
    var value = $('#Teminatlar_OdemeSecenegi').val();
    if (value == 1) $('#Teminatlar_taksitSayisi').val(1);
    else if (value == 2) $('#Teminatlar_taksitSayisi').val(9);
    else $('#Teminatlar_taksitSayisi').val("");
});

$("#Lehtar_kisiSayisi").change(function () {
    var val = $(this).val();
    if (val === undefined) return;
    ferdiKazaPlus.kisisayisi(val);
});

var ferdiKazaPlus = new function () {
    return {
        sigortaliSorgula: function () {

            var kimlikno = $("#Sigortali_TCKimlikNo").val();

            if (kimlikno.length == 11) {

                $("#sigortali-sorgula").button("loading");
                $.ajax({
                    type: "post",
                    dataType: "json",
                    url: "/Teklif/FerdiKazaPlus/SigortaliKimlikSorgula",
                    data: { kimlikno: kimlikno },
                    success: ferdiKazaPlus.sigortaliBasarili

                });

            }
        },

        sigortaliBasarili: function (data) {
            if (data.SorgulamaSonuc) {

                //sol
                $("#Sigortali_Ad").val(data.Sigortali.Ad);
                $("#MSigortali_Ad").val($("#Sigortali_Ad").val().replace('i', 'İ'));
                $("#Sigortali_Ad").val($("#Sigortali_Ad").val().toUpperCase());

                $("#Sigortali_DogumTarihi").val(data.Sigortali.DogumTarihiText);

                if (data.Sigortali.Cinsiyet == "E") {
                    $("#Cinsiyet_E").prop("checked", true);
                }
                else if (data.Sigortali.Cinsiyet == "K") {
                    $("#Cinsiyet_K").prop("checked", true);
                }

                $("#Sigortali_MeslekGrubu").dropDownFill(data.Sigortali.MeslekGruplari);
                $("#Sigortali_MeslekGrubu").val(data.Sigortali.MeslekGrubu);

                $("#Sigortali_UrunAd").dropDownFill(data.Sigortali.Urunler);
                $("#Sigortali_UrunAd").val(data.Sigortali.UrunAd);

                $("#Sigortali_SigortaSuresi").dropDownFill(data.Sigortali.SigortaSureler);
                $("#Sigortali_SigortaSuresi").val(data.Sigortali.SigortaSuresi);

                //Sağ

                $("#Sigortali_SoyAd").val(data.Sigortali.SoyAd);
                $("#Sigortali_SoyAd").val($("#Sigortali_SoyAd").val().replace('i', 'İ'));
                $("#Sigortali_SoyAd").val($("#Sigortali_SoyAd").val().toUpperCase());

                $("#Sigortali_MusteriNo").val(data.Sigortali.MusteriNo);
                $("#Sigortali_DogumYeri").val(data.Sigortali.DogumYeri);
                $("#Sigortali_BabaAdi").val(data.Sigortali.BabaAdi);

                //Prim Ödeyen Bigileri
                $("#PrimOdeme_KartNo_KK1").val(data.PrimOdeme.KartNo.KK1);
                $KrediKarti_K1 = data.PrimOdeme.KartNo.KK1;
                $("#PrimOdeme_KartNo_KK2").val(data.PrimOdeme.KartNo.KK2);
                $KrediKarti_K2 = data.PrimOdeme.KartNo.KK2;
                $("#PrimOdeme_KartNo_KK3").val(data.PrimOdeme.KartNo.KK3);
                $KrediKarti_K3 = data.PrimOdeme.KartNo.KK3;
                $("#PrimOdeme_KartNo_KK4").val(data.PrimOdeme.KartNo.KK4);
                $KrediKarti_K4 = data.PrimOdeme.KartNo.KK4;
                $("#PrimOdeme_ay").val(data.PrimOdeme.ay);
                $SonKullanma_ay = data.PrimOdeme.ay
                $("#PrimOdeme_yil").val(data.PrimOdeme.yil);
                $SonKullanma_yil = data.PrimOdeme.yil;

                $("#Iletisim_AdresMevcut").prop("checked", true);
                $("#Iletisim_EmailMevcut").prop("checked", true);
                $("#Iletisim_Tel1Mevcut").prop("checked", true);
                $("#PrimOdeme_KartNoMevcut").prop("checked", true);


                //İletişim Bilgileri
                //sol
                $("#Iletisim_AdresTipi").dropDownFill(data.Iletisim.AdresTipleri);
                $("#Iletisim_AdresTipi").val(data.Iletisim.AdresTipi);
                $("#Iletisim_Adres").val(data.Iletisim.Adres);
                $Adres = data.Iletisim.Adres;

                //Sağ
                $("#Iletisim_Email").val(data.Iletisim.Email);
                $Email = data.Iletisim.Email;
                $("#Iletisim_Tel1").val(data.Iletisim.Tel1);
                $Telefon1 = data.Iletisim.Tel1;

                $("#sigortaettiren-kimlikno-mesaj").hide();
            }
            else {
                $("#sigortaettiren-kimlikno-mesaj").html(data.HataMesaj);
                $("#sigortali-kimlikno-mesaj").show();

                $(".sigortaettiren-satir.musteri-tipi").slideDown();
                $("#Musteri_SigortaEttiren_MusteriTipKodu").removeAttr("disabled");

            }
            $("#sigortali-sorgula").button("reset");
        },

        SetStarTeminatKapali: function () {
            $("#Lehtar_LehterList_0__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_1__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_2__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_3__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_4__DogumTarihi").addClass("ignore");

        },

        kisisayisi: function (val) {
            if (val == 1) { this.sigortali1(); this.sigortalicheck1(); }
            if (val == 2) { this.sigortali2(); this.sigortalicheck2(); }
            if (val == 3) { this.sigortali3(); this.sigortalicheck3(); }
            if (val == 4) { this.sigortali4(); this.sigortalicheck4(); }
            if (val == 5) { this.sigortali5(); this.sigortalicheck5(); }
        },

        sigortali1: function () {
            $("#Lehtar-ana-div_0").attr("style", "display:normal");
            $("#Lehtar-ana-div_1").attr("style", "display:none");
            $("#Lehtar-ana-div_2").attr("style", "display:none");
            $("#Lehtar-ana-div_3").attr("style", "display:none");
            $("#Lehtar-ana-div_4").attr("style", "display:none");
        },

        sigortali2: function () {
            $("#Lehtar-ana-div_0").attr("style", "display:normal");
            $("#Lehtar-ana-div_1").attr("style", "display:normal");
            $("#Lehtar-ana-div_2").attr("style", "display:none");
            $("#Lehtar-ana-div_3").attr("style", "display:none");
            $("#Lehtar-ana-div_4").attr("style", "display:none");


            $(".sigortali-ailedenmi").attr("style", "display:none");
        },

        sigortali3: function () {
            $("#Lehtar-ana-div_0").attr("style", "display:normal");
            $("#Lehtar-ana-div_1").attr("style", "display:normal");
            $("#Lehtar-ana-div_2").attr("style", "display:normal");
            $("#Lehtar-ana-div_3").attr("style", "display:none");
            $("#Lehtar-ana-div_4").attr("style", "display:none");

            $(".sigortali-ailedenmi").attr("style", "display:normal");




        },

        sigortali4: function () {
            $("#Lehtar-ana-div_0").attr("style", "display:normal");
            $("#Lehtar-ana-div_1").attr("style", "display:normal");
            $("#Lehtar-ana-div_2").attr("style", "display:normal");
            $("#Lehtar-ana-div_3").attr("style", "display:normal");
            $("#Lehtar-ana-div_4").attr("style", "display:none");

            $(".sigortali-ailedenmi").attr("style", "display:normal");




        },

        sigortali5: function () {
            $("#Lehtar-ana-div_0").attr("style", "display:normal");
            $("#Lehtar-ana-div_1").attr("style", "display:normal");
            $("#Lehtar-ana-div_2").attr("style", "display:normal");
            $("#Lehtar-ana-div_3").attr("style", "display:normal");
            $("#Lehtar-ana-div_4").attr("style", "display:normal");





        },

        sigortalicheck1: function (val) {

            $("#Lehtar_LehterList_1__AdiSoyadi").addClass("ignore");
            $("#Lehtar_LehterList_1__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_1__Oran").addClass("ignore");


            $("#Lehtar_LehterList_2__AdiSoyadi").addClass("ignore");
            $("#Lehtar_LehterList_2__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_2__Oran").addClass("ignore");


            $("#Lehtar_LehterList_3__AdiSoyadi").addClass("ignore");
            $("#Lehtar_LehterList_3__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_3__Oran").addClass("ignore");


            $("#Lehtar_LehterList_4__AdiSoyadi").addClass("ignore");
            $("#Lehtar_LehterList_4__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_4__Oran").addClass("ignore");

        },

        sigortalicheck2: function (val) {

            // ==== NO Validasyon ==== //

            $("#Lehtar_LehterList_2__AdiSoyadi").addClass("ignore");
            $("#Lehtar_LehterList_2__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_2__Oran").addClass("ignore");

            $("#Lehtar_LehterList_3__AdiSoyadi").addClass("ignore");
            $("#Lehtar_LehterList_3__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_3__Oran").addClass("ignore");

            $("#Lehtar_LehterList_4__AdiSoyadi").addClass("ignore");
            $("#Lehtar_LehterList_4__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_4__Oran").addClass("ignore");

            $("#Lehtar_LehterList_1__AdiSoyadi").removeClass("ignore");
            $("#Lehtar_LehterList_1__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_1__Oran").removeClass("ignore");


        },

        sigortalicheck3: function (val) {


            $("#Lehtar_LehterList_3__AdiSoyadi").addClass("ignore");
            $("#Lehtar_LehterList_3__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_3__Oran").addClass("ignore");

            $("#Lehtar_LehterList_4__AdiSoyadi").addClass("ignore");
            $("#Lehtar_LehterList_4__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_4__Oran").addClass("ignore");

            $("#Lehtar_LehterList_2__AdiSoyadi").removeClass("ignore");
            $("#Lehtar_LehterList_2__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_2__Oran").removeClass("ignore");

            $("#Lehtar_LehterList_1__AdiSoyadi").removeClass("ignore");
            $("#Lehtar_LehterList_1__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_1__Oran").removeClass("ignore");


        },

        sigortalicheck4: function (val) {
            $("#Lehtar_LehterList_4__AdiSoyadi").addClass("ignore");
            $("#Lehtar_LehterList_4__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_4__Oran").addClass("ignore");

            $("#Lehtar_LehterList_1__AdiSoyadi").removeClass("ignore");
            $("#Lehtar_LehterList_1__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_1__Oran").removeClass("ignore");


            $("#Lehtar_LehterList_2__AdiSoyadi").removeClass("ignore");
            $("#Lehtar_LehterList_2__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_2__Oran").removeClass("ignore");

            $("#Lehtar_LehterList_3__AdiSoyadi").removeClass("ignore");
            $("#Lehtar_LehterList_3__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_3__Oran").removeClass("ignore");


        },

        sigortalicheck5: function (val) {

            $("#Lehtar_LehterList_1__AdiSoyadi").removeClass("ignore");
            $("#Lehtar_LehterList_1__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_1__Oran").removeClass("ignore");

            $("#Lehtar_LehterList_2__AdiSoyadi").removeClass("ignore");
            $("#Lehtar_LehterList_2__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_2__Oran").removeClass("ignore");

            $("#Lehtar_LehterList_3__AdiSoyadi").removeClass("ignore");
            $("#Lehtar_LehterList_3__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_3__Oran").removeClass("ignore");

            $("#Lehtar_LehterList_4__AdiSoyadi").removeClass("ignore");
            $("#Lehtar_LehterList_4__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_4__Oran").removeClass("ignore");
        },

        SigortaliBilgileriReadOnly: function () {

            $("#Sigortali_Ad").attr("readonly", true);
            $("#Sigortali_DogumTarihi").attr("disabled", "disabled");
            $("#Cinsiyet_E").attr("disabled", "disabled");
            $("#Cinsiyet_K").attr("disabled", "disabled");
            $("#Sigortali_MusteriNo").attr("readonly", true);
            $("#Sigortali_SoyAd").attr("readonly", true);
            $("#Sigortali_DogumYeri").attr("readonly", true);
            $("#Sigortali_BabaAdi").attr("readonly", true);
            $("#Teminatlar_taksitSayisi").attr("readonly", true);

            $("#PrimOdeme_KartNo_KK1").attr("readonly", true);
            $("#PrimOdeme_KartNo_KK2").attr("readonly", true);
            $("#PrimOdeme_KartNo_KK3").attr("readonly", true);
            $("#PrimOdeme_KartNo_KK4").attr("readonly", true);
            $("#PrimOdeme_ay").attr("disabled", "disabled");
            $("#PrimOdeme_yil").attr("disabled", "disabled");

            $("#Iletisim_AdresTipi").attr("disabled", "disabled");
            $("#Iletisim_Adres").attr("readonly", true);
            $("#Iletisim_Email").attr("readonly", true);
            $("#Iletisim_Tel1").attr("readonly", true);
            
            

        },

        SetStarTeminatKapali: function () {
            $("#Lehtar_LehterList_0__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_1__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_2__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_3__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_4__DogumTarihi").addClass("ignore");

        },

        LehtarOranKontrol: function () {
            var Message = "";
            var val = $('#Teminatlar_taksitSayisi').val();

            if (val < 0 || val > 100)
                Message += "<p> --Lehtar bilgilerindeki Oran alanı 100'den büyük olamaz </p>";

            if (Message == "")
                return true;
            else {
                $("#hata-message-div").html(Message);
                $("#hata-modal").modal("show");
                return false;
            }
        },

        IsTakipSoruSorgu: function () {
            var teklifId = $("#TeklifId").val();
            if (teklifId != null && teklifId > 0) {
                $.ajax({
                    type: "post",
                    dataType: "json",
                    url: "/Teklif/FerdiKazaPlus/IsTakipSoruSorgula",
                    data: { id: teklifId },
                    success: function (data) {
                        if (data.Success == "true")
                        { return true; }
                        else
                        { return false; }
                    }
                });
            }
        }
    }
}