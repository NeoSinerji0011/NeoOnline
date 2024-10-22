//var seyehatSaglikOdeme = new function () {
//    return {

//        init: function () {
//            $("#kk-odeme-btn").click(seyehatSaglikOdeme.kredikarti);
//            $(".teklif-satin-al").live("click", seyehatSaglikOdeme.kredikartiOdeme);
//            $("#kk-odeme").on("shown", function () { $("#KrediKarti_KartSahibi").focus(); })
//        },

//        kredikartiOdeme: function () {
//            var teklifId = $(this).attr("teklif-id");
//            var fiyat = $(this).attr("fiyat");
//            if (teklifId && fiyat) {
//                $("#KrediKarti_KK_TeklifId").val(teklifId);
//                $("#kk-tutar").html(fiyat);
//                $("#kk-odeme").modal("show");
//            }
//        },

//        kredikarti: function () {
//            var isvalid = $("#krediKartiForm").valid();
//            if (isvalid) {
//                //$("#krediKartiForm").find("#KrediKarti_KartNumarasi").addClass("ignore");
//                var kartnoVal = $("#krediKartiForm")[0].KrediKarti_KartNumarasi;
//                $(kartnoVal).remove();

//                $("#kredi-karti-error").hide();
//                $(this).button("loading");
//                $("#kk-odeme-cancel").hide();

//                var contents = $("#krediKartiForm").serialize();

//                $.ajax({
//                    type: "POST",
//                    url: "/Teklif/SeyehatSaglik/OdemeAl",
//                    data: contents,
//                    success: function (data) {

//                        if (data.Success) {
//                            window.location.href = data.RedirectUrl;
//                            return;
//                        }
//                        else if (data.Hatalar != null && data.Hatalar.length > 0) {
//                            $("#kredi-karti-error-list").html("");

//                            for (var i in data.Hatalar) {
//                                var hata = data.Hatalar[i];

//                                $("#kredi-karti-error-list").append("<span>" + hata + "</span><br/>");
//                            }

//                            $("#kredi-karti-error").slideDown("fast");
//                        }

//                        $("#kk-odeme-btn").button("reset");
//                        $("#kk-odeme-cancel").show();
//                    },
//                    error: function () {
//                        $("#kk-odeme-btn").button("reset");
//                        $("#kk-odeme-cancel").show();
//                    }
//                });
//            }
//        }
//    }
//}

function OdemeSekliDegisti(e, data) {
    if (data.value) {
        $(".taksit-sayisi").slideUp("fast");
    }
    else {
        $(".taksit-sayisi").slideDown("fast");
        $("#Odeme_TaksitSayisi").val("5");
    }
}

function kritikHastalikTeklifWizardCallback(current) {
    //Hazırlayan bilgileri
    if (current == 1) {

    }
        //Sigorta ettiren / sigortali tab
    else if (current == 2) {
        if (!$("#Musteri_SigortaEttiren_AdiUnvan").is(":visible")) {
            // $("#sigortaettiren-sorgula").trigger("click");
            return false;
        }
        //kritikHastalik.genelbilgiler();

        return true; sigortaliKontrol.Kaydet();
    }
        // Riziko Bilgileri
    else if (current == 3) {
        var isValid = FormWizard.validatePage('#tab3');

        return isValid == 1;
    }

    return true;
}

$(document).ready(function () {

    kritikHastalik.SetStarTeminatKapali();

    $("#Teminatlar_vefatTeminati_control").on("switch-change", kritikHastalik.VefatTeminatiChange);
    $("#Teminatlar_kazaSonucuMaluliyet_control").on("switch-change", kritikHastalik.KazaSonucuMaluliyetChange);
    $("#Teminatlar_hastalikSonucuMaluliyet_control").on("switch-change", kritikHastalik.HastalikSonucuMaluliyetChange);
    $("#Teminatlar_tehlikeliHastalik_control").on("switch-change", kritikHastalik.TehlikeliHastalikChange);


    $("#OdemeTipi_2[name='Odeme.OdemeTipi']").attr("checked", "checked");

    // ==== Odeme tipi, şekli ve taksit sayısı kontrol ediliyor. ==== //
    $("#Odeme_OdemeSekli_control").on("switch-change", OdemeSekliDegisti);

    // ==== Para Formatı Belirleniyor.
    $(".autoNumeric").autoNumeric('init', { vMin: '0', vMax: '9999999', mDec: 0 });

    $("#GenelBilgiler_SigortalilarAilemi_control").on("switch-change", kritikHastalik.ailedenmi);

    $("#GenelBilgiler_SigortaEttirenSigortalilardanBirimi_control").on("switch-change", kritikHastalik.sigortaettirensigortalilardanmi);


    // No Validation
    kritikHastalik.sigortali1();
    kritikHastalik.sigortalicheck1();

    var date = new Date();
    date.setDate(date.getDate() + 1);

    setTimeout(function () {
        date.setDate(date.getDate() + 1);
        $("#GenelBilgiler_SeyehatBitisTarihi").datepicker("option", "minDate", date);
    }, 500);

    var date1 = new Date();
    date1.setMonth(date1.getMonth() - 6);
    //Dogum tarihi 6 aydan küçük olamaz.
    $("#Sigortalilar_SigortaliList_0__DogumTarihi").datepicker("option", "maxDate", date1);
    $("#Sigortalilar_SigortaliList_1__DogumTarihi").datepicker("option", "maxDate", date1);
    $("#Sigortalilar_SigortaliList_2__DogumTarihi").datepicker("option", "maxDate", date1);
    $("#Sigortalilar_SigortaliList_3__DogumTarihi").datepicker("option", "maxDate", date1);
    $("#Sigortalilar_SigortaliList_4__DogumTarihi").datepicker("option", "maxDate", date1);


    //Seyehat Planı Başlangıcta yok
    $("#GenelBilgiler_Plan").addClass("ignore");
});

// ==== Email Gönderme İşlemleri ==== //
$("#email-gonder-test").live("click", function () {

    $("#email-gonder-test").button("loading");

    $.get("/Teklif/KritikHastalik/TeklifEPostaTest",
          null,
          function (data) {
              $("#mail-gonder-modal-div").html(data);
              $.validator.unobtrusive.parse("#mail-gonder-modal-div");
              $("#email-modal").modal('show');
              $("#email-gonder-test").button("reset");
          },
          "html");
});

$("#btn-hesapla").click(function () {
    $('#form_wizard_1').bootstrapWizard("next");
});

$("#Lehtar_kisiSayisi").change(function () {
    var val = $(this).val();
    if (val === undefined) return;
    kritikHastalik.kisisayisi(val);
});

$(".upper-letter").blur(function () {
    $(this).val($(this).val().replace('i', 'İ'));
    $(this).val($(this).val().toUpperCase());
});

//$(".tckn").blur(function () {
//    var val = $(this).val();
//    if (val.length > 0)
//        if (!kritikHastalik.tcknKontrol(val))
//            alert("Lütfen geçerli bir tc kimlik no giriniz.");
//});


$("#Teminatlar_teminatTutari").change(function () { kritikHastalik.TeminatTutariChange(); });
$("#GenelBilgiler_paraBirimi").change(function () { kritikHastalik.ParaBirimiChange(); });


var kritikHastalik = new function () {
    return {

        //Teminatlar
        TeminatTutariChange: function () {
            var TeminatTutarVal = $("#Teminatlar_teminatTutari").val();
            if (TeminatTutarVal == 4) {
                $("#teminatTutarDiger").show();
                $("#Teminatlar_teminatTutariDiger").val('');
            }
            else $("#teminatTutarDiger").hide();
        },

        VefatTeminatiChange: function (e, data) {
            var object = $("#Teminatlar_VefatBedeli");

            if (data.value) {
                $("#vefat").slideDown("fast");
                object.removeClass('ignore');
            }
            else {
                $("#vefat").slideUp("fast");
                object.addClass("ignore");
                object.val('');
            }
        },

        KazaSonucuMaluliyetChange: function (e, data) {

            var object = $("#Teminatlar_kazaSonucuMaluliyetBedeli");

            if (data.value) {
                $("#kaza-sonucu-maluliyet").slideDown("fast");
                object.removeClass('ignore');
            }
            else {
                $("#kaza-sonucu-maluliyet").slideUp("fast");
                object.addClass("ignore");
                object.val('');
            }

        },

        HastalikSonucuMaluliyetChange: function (e, data) {
            var object = $("#Teminatlar_hastalikSonucuMaluliyetBedeli");

            if (data.value) {
                $("#hastalik-sonucu-maluliyet").slideDown("fast");
                object.removeClass('ignore');
            }
            else {
                $("#hastalik-sonucu-maluliyet").slideUp("fast");
                object.addClass("ignore");
                object.val('');
            }
        },

        TehlikeliHastalikChange: function (e, data) {
            var object = $("#Teminatlar_tehlikeliHastalikBedeli");

            if (data.value) {
                $("#tehlikeli-hastalik").slideDown("fast");
                object.removeClass('ignore');
            }
            else {
                $("#tehlikeli-hastalik").slideUp("fast");
                object.addClass("ignore");
                object.val('');
            }
        },

        SetStarTeminatKapali: function () {
            $("#Lehtar_LehterList_0__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_1__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_2__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_3__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_4__DogumTarihi").addClass("ignore");
         
        },

        ParaBirimiChange: function () {
            var paraBirimiVal = $("#GenelBilgiler_paraBirimi").val();
            if (paraBirimiVal == 1) $(".paraBirimi").html('TL');
            else if (paraBirimiVal == 2) $(".paraBirimi").html('€');
            else if (paraBirimiVal == 3) $(".paraBirimi").html('$');
            else $(".paraBirimi").html('TL');
        },

        kimliknosorgula: function (data) {
            if (data.SorgulamaSonuc) {
                $("#birey-tipi_0").attr("style", "display:none");

                $("#Sigortalilar_SigortaliList_0__Adi").val(data.AdiUnvan);
                $("#Sigortalilar_SigortaliList_0__Adi").attr("disabled", "");

                $("#Sigortalilar_SigortaliList_0__Soyadi").val(data.SoyadiUnvan);
                $("#Sigortalilar_SigortaliList_0__Soyadi").attr("disabled", "");

                $('#Sigortalilar_SigortaliList_0__DogumTarihi').val(data.DogumTarihiText);
                $('#Sigortalilar_SigortaliList_0__DogumTarihi').datepicker('disable');

                $("#Sigortalilar_SigortaliList_0__KimlikNo").val(data.KimlikNo);
                $("#Sigortalilar_SigortaliList_0__KimlikNo").attr("disabled", "");

                if (data.Uyruk == 0) {
                    $("#Sigortalilar_SigortaliList_0__Uyruk").val(0);
                    $("#Sigortalilar_SigortaliList_0__KimlikTipi").val(1);
                }
                else {
                    $("#Sigortalilar_SigortaliList_0__Uyruk").val(1);
                    $("#Sigortalilar_SigortaliList_0__KimlikTipi").val(2);
                }

                $("#Sigortalilar_SigortaliList_0__Uyruk").attr("disabled", "");
                $("#Sigortalilar_SigortaliList_0__KimlikTipi").attr("disabled", "");
            }
        },

        genelbilgiler: function () {

            //var kimlikNo = $("#Musteri_SigortaEttiren_KimlikNo").val();
            //$.ajax({
            //    type: "post",
            //    dataType: "json",
            //    url: "/Teklif/SeyehatSaglik/KimlikNoSorgulaSeyehat",
            //    data: { kimlikNo: kimlikNo },
            //    success: kritikHastalik.kimliknosorgula
            //});
            //$("#Sigortalilar_SigortaliList_0__Uyruk_control").bootstrapSwitch('setState', true);

            //var sigortaliAyni = $("#Musteri_SigortaliAyni_control").bootstrapSwitch('status');
            //if (sigortaliAyni) {
            //    $("#GenelBilgiler_SigortaEttirenSigortalilardanBirimi_control").bootstrapSwitch('setActive', false);
            //    //$("#GenelBilgiler_SigortaEttirenSigortalilardanBirimi").attr("disabled", "");
            //}
            //else {
            //    $("#GenelBilgiler_SigortaEttirenSigortalilardanBirimi_control").removeClass("deactivate");
            //    $("#GenelBilgiler_SigortaEttirenSigortalilardanBirimi").removeAttr("disabled", "");
            //}
        },

        sigortaettirensigortalilardanmi: function (e, data) {
            if (data.value) {
                var kimlikNo = $("#Musteri_SigortaEttiren_KimlikNo").val();
                $.ajax({
                    type: "post",
                    dataType: "json",
                    url: "/Teklif/SeyehatSaglik/KimlikNoSorgulaSeyehat",
                    data: { kimlikNo: kimlikNo },
                    success: kritikHastalik.kimliknosorgula
                });
            }
            else {
                $("#Sigortalilar_SigortaliList_0__Adi").val('');
                $("#Sigortalilar_SigortaliList_0__Adi").removeAttr("disabled");

                $("#Sigortalilar_SigortaliList_0__Soyadi").val('');
                $("#Sigortalilar_SigortaliList_0__Soyadi").removeAttr("disabled");

                $('#Sigortalilar_SigortaliList_0__DogumTarihi').val('');
                $('#Sigortalilar_SigortaliList_0__DogumTarihi').datepicker('enable');

                $("#Sigortalilar_SigortaliList_0__KimlikNo").val('');
                $("#Sigortalilar_SigortaliList_0__KimlikNo").removeAttr("disabled");

                $("#Sigortalilar_SigortaliList_0__Uyruk_control").removeClass("deactivate");
                $("#Sigortalilar_SigortaliList_0__Uyruk").removeAttr("disabled");

                $("#Sigortalilar_SigortaliList_0__KimlikTipi_control").removeClass("deactivate");
                $("#Sigortalilar_SigortaliList_0__KimlikTipi").removeAttr("disabled");
            }
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

            $("#Lehtar_LehterList_1__Adi").addClass("ignore");
            $("#Lehtar_LehterList_1__Soyadi").addClass("ignore");
            $("#Lehtar_LehterList_1__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_1__Oran").addClass("ignore");


            $("#Lehtar_LehterList_2__Adi").addClass("ignore");
            $("#Lehtar_LehterList_2__Soyadi").addClass("ignore");
            $("#Lehtar_LehterList_2__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_2__Oran").addClass("ignore");


            $("#Lehtar_LehterList_3__Adi").addClass("ignore");
            $("#Lehtar_LehterList_3__Soyadi").addClass("ignore");
            $("#Lehtar_LehterList_3__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_3__Oran").addClass("ignore");


            $("#Lehtar_LehterList_4__Adi").addClass("ignore");
            $("#Lehtar_LehterList_4__Soyadi").addClass("ignore");
            $("#Lehtar_LehterList_4__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_4__Oran").addClass("ignore");

        },

        sigortalicheck2: function (val) {

            // ==== NO Validasyon ==== //

            $("#Lehtar_LehterList_2__Adi").addClass("ignore");
            $("#Lehtar_LehterList_2__Soyadi").addClass("ignore");
            $("#Lehtar_LehterList_2__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_2__Oran").addClass("ignore");

            $("#Lehtar_LehterList_3__Adi").addClass("ignore");
            $("#Lehtar_LehterList_3__Soyadi").addClass("ignore");
            $("#Lehtar_LehterList_3__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_3__Oran").addClass("ignore");

            $("#Lehtar_LehterList_4__Adi").addClass("ignore");
            $("#Lehtar_LehterList_4__Soyadi").addClass("ignore");
            $("#Lehtar_LehterList_4__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_4__Oran").addClass("ignore");

            $("#Lehtar_LehterList_1__Adi").removeClass("ignore");
            $("#Lehtar_LehterList_1__Soyadi").removeClass("ignore");
            $("#Lehtar_LehterList_1__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_1__Oran").removeClass("ignore");


        },

        sigortalicheck3: function (val) {


            $("#Lehtar_LehterList_3__Adi").addClass("ignore");
            $("#Lehtar_LehterList_3__Soyadi").addClass("ignore");
            $("#Lehtar_LehterList_3__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_3__Oran").addClass("ignore");

            $("#Lehtar_LehterList_4__Adi").addClass("ignore");
            $("#Lehtar_LehterList_4__Soyadi").addClass("ignore");
            $("#Lehtar_LehterList_4__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_4__Oran").addClass("ignore");

            $("#Lehtar_LehterList_2__Adi").removeClass("ignore");
            $("#Lehtar_LehterList_2__Soyadi").removeClass("ignore");
            $("#Lehtar_LehterList_2__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_2__Oran").removeClass("ignore");

            $("#Lehtar_LehterList_1__Adi").removeClass("ignore");
            $("#Lehtar_LehterList_1__Soyadi").removeClass("ignore");
            $("#Lehtar_LehterList_1__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_1__Oran").removeClass("ignore");


        },

        sigortalicheck4: function (val) {
            $("#Lehtar_LehterList_4__Adi").addClass("ignore");
            $("#Lehtar_LehterList_4__Soyadi").addClass("ignore");
            $("#Lehtar_LehterList_4__DogumTarihi").addClass("ignore");
            $("#Lehtar_LehterList_4__Oran").addClass("ignore");

            $("#Lehtar_LehterList_1__Adi").removeClass("ignore");
            $("#Lehtar_LehterList_1__Soyadi").removeClass("ignore");
            $("#Lehtar_LehterList_1__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_1__Oran").removeClass("ignore");


            $("#Lehtar_LehterList_2__Adi").removeClass("ignore");
            $("#Lehtar_LehterList_2__Soyadi").removeClass("ignore");
            $("#Lehtar_LehterList_2__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_2__Oran").removeClass("ignore");

            $("#Lehtar_LehterList_3__Adi").removeClass("ignore");
            $("#Lehtar_LehterList_3__Soyadi").removeClass("ignore");
            $("#Lehtar_LehterList_3__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_3__Oran").removeClass("ignore");


        },

        sigortalicheck5: function (val) {

            $("#Lehtar_LehterList_1__Adi").removeClass("ignore");
            $("#Lehtar_LehterList_1__Soyadi").removeClass("ignore");
            $("#Lehtar_LehterList_1__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_1__Oran").removeClass("ignore");

            $("#Lehtar_LehterList_2__Adi").removeClass("ignore");
            $("#Lehtar_LehterList_2__Soyadi").removeClass("ignore");
            $("#Lehtar_LehterList_2__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_2__Oran").removeClass("ignore");

            $("#Lehtar_LehterList_3__Adi").removeClass("ignore");
            $("#Lehtar_LehterList_3__Soyadi").removeClass("ignore");
            $("#Lehtar_LehterList_3__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_3__Oran").removeClass("ignore");

            $("#Lehtar_LehterList_4__Adi").removeClass("ignore");
            $("#Lehtar_LehterList_4__Soyadi").removeClass("ignore");
            $("#Lehtar_LehterList_4__DogumTarihi").removeClass("ignore");
            $("#Lehtar_LehterList_4__Oran").removeClass("ignore");
        },

        //tcknKontrol: function (KimlikNo) {
        //    KimlikNo = String(KimlikNo);
        //    if (!KimlikNo.match(/^[0-9]{11}$/)) return false;

        //    pr1 = parseInt(KimlikNo.substr(0, 1));
        //    pr2 = parseInt(KimlikNo.substr(1, 1));
        //    pr3 = parseInt(KimlikNo.substr(2, 1));
        //    pr4 = parseInt(KimlikNo.substr(3, 1));
        //    pr5 = parseInt(KimlikNo.substr(4, 1));
        //    pr6 = parseInt(KimlikNo.substr(5, 1));
        //    pr7 = parseInt(KimlikNo.substr(6, 1));
        //    pr8 = parseInt(KimlikNo.substr(7, 1));
        //    pr9 = parseInt(KimlikNo.substr(8, 1));
        //    pr10 = parseInt(KimlikNo.substr(9, 1));
        //    pr11 = parseInt(KimlikNo.substr(10, 1));

        //    if ((pr1 + pr3 + pr5 + pr7 + pr9 + pr2 + pr4 + pr6 + pr8 + pr10) % 10 != pr11) return false;
        //    if (((pr1 + pr3 + pr5 + pr7 + pr9) * 7 + (pr2 + pr4 + pr6 + pr8) * 9) % 10 != pr10) return false;
        //    if (((pr1 + pr3 + pr5 + pr7 + pr9) * 8) % 10 != pr11) return false;
        //    return true;
        //}

    }
}


