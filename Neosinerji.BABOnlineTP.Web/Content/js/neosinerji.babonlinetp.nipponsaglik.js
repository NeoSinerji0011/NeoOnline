var seyehatSaglikOdeme = new function () {
    return {

        init: function () {
            $("#kk-odeme-btn").click(seyehatSaglikOdeme.kredikarti);
            $(".teklif-satin-al").live("click", seyehatSaglikOdeme.kredikartiOdeme);
            $("#kk-odeme").on("shown", function () { $("#KrediKarti_KartSahibi").focus(); })
        },

        kredikartiOdeme: function () {
            debugger
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

                var vadeli = $("#Odeme_OdemeSekli").is(':checked');
                if (!vadeli) {
                    $("#KK_OdemeSekli_2").attr('checked', true);
                    $("#taksit-sayisi").show();
                    var taksitliOdeme = $("#Odeme_TaksitSayisi").val();
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
                //$("#KrediKarti_KK_TeklifId").val(teklifId);
                //$("#kk-tutar").html(fiyat);
                //$("#kk-odeme").modal("show");
            }
        },

        kredikarti: function () {
            var isvalid = $("#krediKartiForm").valid();
            if (isvalid) {
                var contents = $("#krediKartiForm").serialize();

                $.ajax({
                    type: "POST",
                    url: "/Teklif/NipponSeyahat/OdemeAl",
                    data: contents,
                    success: function (data) {

                        if (data.Success) {
                            window.location.href = data.RedirectUrl;
                            return;
                        }
                        else if (data.Hatalar !== null && data.Hatalar.length > 0) {
                            $("#kredi-karti-error-list").html("");

                            for (var i in data.Hatalar) {
                                var hata = data.Hatalar[i];

                                $("#kredi-karti-error-list").append("<span>" + hata + "</span><br/>");
                            }
                        }
                    },
                    error: function () {

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

function seyehatSaglikTeklifWizardCallback(current) {
    debugger
    //Hazırlayan bilgileri
    if (current === 1) { 
        $("#button-next").show();
        $("#btn-hesapla").hide();
    }
    //Sigorta ettiren / sigortali tab
    else if (current === 2) {
        if ($("#nipponNotSelfInsuredButton").is(":checked")) {
            if (!$("#Musteri_SigortaEttiren_AdiUnvan").is(":visible")) {
                $("#sigortaettiren-sorgula").trigger("click");
                return false;
            }
        }
        seyehatClass.genelbilgiler();
    
        return sigortaliKontrol.Kaydet();
    }
    // Riziko Bilgileri
    else if (current === 3) {
        var isValid = FormWizard.validatePage('#tab3');

        return isValid === 1;
    }

    return true;
}

$(document).ready(function () {
    $("#OdemeTipi_2[name='Odeme.OdemeTipi']").attr("checked", "checked");

    // ==== Odeme tipi, şekli ve taksit sayısı kontrol ediliyor. ==== //
    $("#Odeme_OdemeSekli_control").on("switch-change", OdemeSekliDegisti);


    $("#GenelBilgiler_KayakTeminati_control").on("switch-change", seyehatClass.kayakteminati);

    $("#GenelBilgiler_SigortalilarAilemi_control").on("switch-change", seyehatClass.ailedenmi);

    $("#GenelBilgiler_SigortaEttirenSigortalilardanBirimi_control").on("switch-change", seyehatClass.sigortaettirensigortalilardanmi);

    $("#sigortalilar-ana-div_0").attr("style", "display:normal");



    var date = new Date();
    date.setDate(date.getDate() - 1);

    //Seyehat başlangıc tarihi en dusuk yarın olmalı
    $("#GenelBilgiler_SeyehatBaslangicTarihi").datepicker("option", { minDate: date });


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

    // No Validation
    seyehatClass.sigortalicheck1();
});

$('#GenelBilgiler_SeyehatBaslangicTarihi').change(function () {
    var currentDate = $(this).datepicker("getDate");
    currentDate.setTime(currentDate.getTime() + 1 * 86400000);
    $("#GenelBilgiler_SeyehatBitisTarihi").datepicker("option", "minDate", currentDate);
    //Max seyehat suresi 1 yıldır.

    setTimeout(function () {
        //d1.setMonth(d1.getMonth() + 12);
        //$("#GenelBilgiler_SeyehatBitisTarihi").datepicker("option", "maxDate", d1);
        $("#GenelBilgiler_SeyehatBitisTarihi").datepicker("show");

    }, 200);
});

//$("#btn-hesapla").click(function () {
//    debugger
//    var isvalid = $("#form1").valid();
//    if (isvalid) {


//        $(this).button("loading");

//        $(".switcher").find(":input").switchFix();

//        var contents = $("#form1, #form2, #form3, #form4, #form5").serialize();

//        $.ajax(
//            {
//                type: "POST",
//                url: "/Teklif/NipponSeyahat/Hesapla",
//                data: contents,
//                success: function (data) {
//                    if (data.id > 0) {
//                        teklifFiyat.kontol({ processId: data.id, guid: data.g });

//                        $("#step3group").css({ "visibility": "visible" });
//                        $("#teklif-fiyat-container").css({ "visibility": "visible" });
//                        $("#step1").collapse("hide");
//                        $("#step2").collapse("hide");
//                        $("#step3").collapse("show");

//                        $("#fiyat-container").html($("#fiyat-container-template").html());
//                        $('#form_wizard_1').bootstrapWizard("next");
//                    } else if (data.id === 0) {

//                        $("#btn-hesapla").button("reset");
//                        alert(data.hata);
//                    }
//                    $("#btn-hesapla").button("reset");
//                },
//                error: function () {
//                    $("#btn-hesapla").button("reset");
//                }
//            });
//        //App.scrollTo();
//    }
//});

$("#GenelBilgiler_UlkeTipi").change(function () {
    seyehatClass.ulketipi($(this).val());
});

$("#GenelBilgiler_KisiSayisi").change(function () {
    var val = $(this).val();
    if (val === undefined) return;
    seyehatClass.kisisayisi(val);
});

$(".upper-letter").blur(function () {
    $(this).val($(this).val().replace('i', 'İ'));
    $(this).val($(this).val().toUpperCase());
});

$(".tckn").blur(function () {
    var val = $(this).val();
    if (val.length > 0)
        if (!seyehatClass.tcknKontrol(val))
            alert("Lütfen geçerli bir tc kimlik no giriniz.");
});

var seyehatClass = new function () {

    return {

        kimliknosorgula: function (data) {
            if (data.SorgulamaSonuc) {
                //$("#birey-tipi_0").attr("style", "display:none");

                //$("#Sigortalilar_SigortaliList_0__Adi").val(data.AdiUnvan);
                //$("#Sigortalilar_SigortaliList_0__Adi").attr("disabled", "");

                //$("#Sigortalilar_SigortaliList_0__Soyadi").val(data.SoyadiUnvan);
                //$("#Sigortalilar_SigortaliList_0__Soyadi").attr("disabled", "");

                //$('#Sigortalilar_SigortaliList_0__DogumTarihi').val(data.DogumTarihiText);
                //$('#Sigortalilar_SigortaliList_0__DogumTarihi').datepicker('disable');

                //$("#Sigortalilar_SigortaliList_0__KimlikNo").val(data.KimlikNo);
                //$("#Sigortalilar_SigortaliList_0__KimlikNo").attr("disabled", "");

                //if (data.Uyruk == 0) {
                //    $("#Sigortalilar_SigortaliList_0__Uyruk").val(0);
                //    $("#Sigortalilar_SigortaliList_0__KimlikTipi").val(1);
                //}
                //else {
                //    $("#Sigortalilar_SigortaliList_0__Uyruk").val(1);
                //    $("#Sigortalilar_SigortaliList_0__KimlikTipi").val(2);
                //}

                //$("#Sigortalilar_SigortaliList_0__Uyruk").attr("disabled", "");
                //$("#Sigortalilar_SigortaliList_0__KimlikTipi").attr("disabled", "");
            }
        },

        genelbilgiler: function () {
            var kimlikNo = $("#Musteri_SigortaEttiren_KimlikNo").val();
            $.ajax({
                type: "post",
                dataType: "json",
                url: "/Teklif/SeyehatSaglik/KimlikNoSorgulaSeyehat",
                data: { kimlikNo: kimlikNo },
                success: seyehatClass.kimliknosorgula
            });
            $("#Sigortalilar_SigortaliList_0__Uyruk_control").bootstrapSwitch('setState', true);

            var sigortaliAyni = $("#Musteri_SigortaliAyni_control").bootstrapSwitch('status');
            if (sigortaliAyni) {
                $("#GenelBilgiler_SigortaEttirenSigortalilardanBirimi_control").bootstrapSwitch('setActive', false);
                //$("#GenelBilgiler_SigortaEttirenSigortalilardanBirimi").attr("disabled", "");
            }
            else {
                $("#GenelBilgiler_SigortaEttirenSigortalilardanBirimi_control").removeClass("deactivate");
                $("#GenelBilgiler_SigortaEttirenSigortalilardanBirimi").removeAttr("disabled", "");
            }
        },

        ulketipi: function (val) {
            var request = true;
            if (val === undefined) { $("#GenelBilgiler_GidilecekUlke").empty(); return; }

            if (val === 0) {
                $("#GenelBilgiler_GidilecekUlke").empty();
                $("#plan").attr("style", "display:none");
                $("#GenelBilgiler_Plan").addClass("ignore");
                return;
            }
            else if (val === 1) {
                request = true;
                $("#plan").attr("style", "display:none");
                $("#GenelBilgiler_Plan").addClass("ignore");
            }
            else if (val === 2) {
                request = false;
                $("#plan").attr("style", "display:normal");
                $("#GenelBilgiler_Plan").removeClass("ignore");
            }

            $.ajax({
                type: "post",
                dataType: "json",
                url: "/Teklif/SeyehatSaglik/SeyehatUlkeleri",
                data: { schengenMi: request },
                success: function (data) {
                    $("#GenelBilgiler_GidilecekUlke").dropDownFill(data);
                }
            });
        },

        kayakteminati: function (e, data) {
            if (data.value) {
                alert("Kayak teminatı profesyonel ve lisanslı sporcular için kapsam dışıdır. Poliçeye kayak teminatının eklenmesi durumunda %25 sürprim uygulanır.");
            }
        },

        sigortaettirensigortalilardanmi: function (e, data) {
            if (data.value) {
                var kimlikNo = $("#Musteri_SigortaEttiren_KimlikNo").val();
                $.ajax({
                    type: "post",
                    dataType: "json",
                    url: "/Teklif/SeyehatSaglik/KimlikNoSorgulaSeyehat",
                    data: { kimlikNo: kimlikNo },
                    success: seyehatClass.kimliknosorgula
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
            if (val === 1) { this.sigortali1(); this.sigortalicheck1(); }
            if (val === 2) { this.sigortali2(); this.sigortalicheck2(); }
            if (val === 3) { this.sigortali3(); this.sigortalicheck3(); }
            if (val === 4) { this.sigortali4(); this.sigortalicheck4(); }
            if (val === 5) { this.sigortali5(); this.sigortalicheck5(); }
        },

        sigortali1: function () {
            $("#sigortalilar-ana-div_1").attr("style", "display:none");
            $("#sigortalilar-ana-div_2").attr("style", "display:none");
            $("#sigortalilar-ana-div_3").attr("style", "display:none");
            $("#sigortalilar-ana-div_4").attr("style", "display:none");
            $(".birey-tipi").attr("style", "display:none");

            $(".sigortali-ailedenmi").attr("style", "display:none");

            $("#GenelBilgiler_SigortalilarAilemi_control").bootstrapSwitch('setState', false);
        },

        sigortali2: function () {
            $("#sigortalilar-ana-div_1").attr("style", "display:normal");
            $("#sigortalilar-ana-div_2").attr("style", "display:none");
            $("#sigortalilar-ana-div_3").attr("style", "display:none");
            $("#sigortalilar-ana-div_4").attr("style", "display:none");
            $(".birey-tipi").attr("style", "display:none");

            $(".sigortali-ailedenmi").attr("style", "display:none");
        },

        sigortali3: function () {
            $("#sigortalilar-ana-div_1").attr("style", "display:normal");
            $("#sigortalilar-ana-div_2").attr("style", "display:normal");
            $("#sigortalilar-ana-div_3").attr("style", "display:none");
            $("#sigortalilar-ana-div_4").attr("style", "display:none");

            $(".sigortali-ailedenmi").attr("style", "display:normal");

            var ailemi = $("#GenelBilgiler_SigortalilarAilemi_control").bootstrapSwitch('status');
            if (ailemi)
                $(".birey-tipi").attr("style", "display:normal");
            else
                $(".birey-tipi").attr("style", "display:none");

        },

        sigortali4: function () {
            $("#sigortalilar-ana-div_1").attr("style", "display:normal");
            $("#sigortalilar-ana-div_2").attr("style", "display:normal");
            $("#sigortalilar-ana-div_3").attr("style", "display:normal");
            $("#sigortalilar-ana-div_4").attr("style", "display:none");

            $(".sigortali-ailedenmi").attr("style", "display:normal");

            var ailemi = $("#GenelBilgiler_SigortalilarAilemi_control").bootstrapSwitch('status');
            if (ailemi)
                $(".birey-tipi").attr("style", "display:normal");
            else
                $(".birey-tipi").attr("style", "display:none");

        },

        sigortali5: function () {
            $("#sigortalilar-ana-div_1").attr("style", "display:normal");
            $("#sigortalilar-ana-div_2").attr("style", "display:normal");
            $("#sigortalilar-ana-div_3").attr("style", "display:normal");
            $("#sigortalilar-ana-div_4").attr("style", "display:normal");

            $(".sigortali-ailedenmi").attr("style", "display:normal");

            var ailemi = $("#GenelBilgiler_SigortalilarAilemi_control").bootstrapSwitch('status');
            if (ailemi)
                $(".birey-tipi").attr("style", "display:normal");
            else
                $(".birey-tipi").attr("style", "display:none");

        },

        ailedenmi: function (e, data) {
            if (data.value)
                $(".birey-tipi").attr("style", "display:normal");
            else
                $(".birey-tipi").attr("style", "display:none");
        },

        sigortalicheck1: function (val) {

            $("#Sigortalilar_SigortaliList_0__BireyTipi").addClass("ignore");

            $("#Sigortalilar_SigortaliList_1__Adi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_1__Soyadi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_1__DogumTarihi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_1__KimlikNo").addClass("ignore");
            $("#Sigortalilar_SigortaliList_1__BireyTipi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_1__Uyruk").addClass("ignore");
            $("#Sigortalilar_SigortaliList_1__KimlikTipi").addClass("ignore");

            $("#Sigortalilar_SigortaliList_2__Adi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_2__Soyadi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_2__DogumTarihi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_2__KimlikNo").addClass("ignore");
            $("#Sigortalilar_SigortaliList_2__BireyTipi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_2__Uyruk").addClass("ignore");
            $("#Sigortalilar_SigortaliList_2__KimlikTipi").addClass("ignore");

            $("#Sigortalilar_SigortaliList_3__Adi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__Soyadi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__DogumTarihi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__KimlikNo").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__BireyTipi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__Uyruk").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__KimlikTipi").addClass("ignore");

            $("#Sigortalilar_SigortaliList_4__Adi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__Soyadi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__DogumTarihi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__KimlikNo").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__BireyTipi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__Uyruk").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__KimlikTipi").addClass("ignore");
        },

        sigortalicheck2: function (val) {

            // ==== NO Validasyon ==== //
            $("#Sigortalilar_SigortaliList_2__Adi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_2__Soyadi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_2__DogumTarihi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_2__KimlikNo").addClass("ignore");
            $("#Sigortalilar_SigortaliList_2__BireyTipi").addClass("ignore");

            $("#Sigortalilar_SigortaliList_3__Adi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__Soyadi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__DogumTarihi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__KimlikNo").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__BireyTipi").addClass("ignore");

            $("#Sigortalilar_SigortaliList_4__Adi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__Soyadi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__DogumTarihi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__KimlikNo").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__BireyTipi").addClass("ignore");


            $("#Sigortalilar_SigortaliList_1__Adi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__Soyadi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__DogumTarihi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__KimlikNo").removeClass("ignore");

            var ailemi = $("#GenelBilgiler_SigortalilarAilemi_control").bootstrapSwitch('status');
            if (ailemi)
                $("#Sigortalilar_SigortaliList_1__BireyTipi").removeClass("ignore");
            else
                $("#Sigortalilar_SigortaliList_1__BireyTipi").addClass("ignore");
        },

        sigortalicheck3: function (val) {
            $("#Sigortalilar_SigortaliList_3__Adi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__Soyadi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__DogumTarihi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__KimlikNo").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__BireyTipi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__Uyruk").addClass("ignore");
            $("#Sigortalilar_SigortaliList_3__KimlikTipi").addClass("ignore");

            $("#Sigortalilar_SigortaliList_4__Adi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__Soyadi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__DogumTarihi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__KimlikNo").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__BireyTipi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__Uyruk").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__KimlikTipi").addClass("ignore");

            $("#Sigortalilar_SigortaliList_1__Adi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__Soyadi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__DogumTarihi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__KimlikNo").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__Uyruk").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__KimlikTipi").removeClass("ignore");

            $("#Sigortalilar_SigortaliList_2__Adi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__Soyadi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__DogumTarihi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__KimlikNo").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__Uyruk").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__KimlikTipi").removeClass("ignore");

            var ailemi = $("#GenelBilgiler_SigortalilarAilemi_control").bootstrapSwitch('status');
            if (ailemi) {
                $("#Sigortalilar_SigortaliList_1__BireyTipi").removeClass("ignore");
                $("#Sigortalilar_SigortaliList_2__BireyTipi").removeClass("ignore");
            }
            else {
                $("#Sigortalilar_SigortaliList_1__BireyTipi").addClass("ignore");
                $("#Sigortalilar_SigortaliList_2__BireyTipi").addClass("ignore");
            }
        },

        sigortalicheck4: function (val) {
            $("#Sigortalilar_SigortaliList_4__Adi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__Soyadi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__DogumTarihi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__KimlikNo").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__BireyTipi").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__Uyruk").addClass("ignore");
            $("#Sigortalilar_SigortaliList_4__KimlikTipi").addClass("ignore");

            $("#Sigortalilar_SigortaliList_1__Adi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__Soyadi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__DogumTarihi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__KimlikNo").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__Uyruk").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__KimlikTipi").removeClass("ignore");

            $("#Sigortalilar_SigortaliList_2__Adi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__Soyadi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__DogumTarihi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__KimlikNo").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__Uyruk").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__KimlikTipi").removeClass("ignore");

            $("#Sigortalilar_SigortaliList_3__Adi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_3__Soyadi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_3__DogumTarihi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_3__KimlikNo").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_3__Uyruk").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_3__KimlikTipi").removeClass("ignore");

            var ailemi = $("#GenelBilgiler_SigortalilarAilemi_control").bootstrapSwitch('status');
            if (ailemi) {
                $("#Sigortalilar_SigortaliList_1__BireyTipi").removeClass("ignore");
                $("#Sigortalilar_SigortaliList_2__BireyTipi").removeClass("ignore");
                $("#Sigortalilar_SigortaliList_3__BireyTipi").removeClass("ignore");
            }
            else {
                $("#Sigortalilar_SigortaliList_1__BireyTipi").addClass("ignore");
                $("#Sigortalilar_SigortaliList_2__BireyTipi").addClass("ignore");
                $("#Sigortalilar_SigortaliList_3__BireyTipi").addClass("ignore");
            }
        },

        sigortalicheck5: function (val) {
            $("#Sigortalilar_SigortaliList_1__Adi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__Soyadi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__DogumTarihi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__KimlikNo").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__Uyruk").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_1__KimlikTipi").removeClass("ignore");

            $("#Sigortalilar_SigortaliList_2__Adi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__Soyadi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__DogumTarihi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__KimlikNo").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__Uyruk").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_2__KimlikTipi").removeClass("ignore");

            $("#Sigortalilar_SigortaliList_3__Adi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_3__Soyadi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_3__DogumTarihi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_3__KimlikNo").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_3__Uyruk").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_3__KimlikTipi").removeClass("ignore");

            $("#Sigortalilar_SigortaliList_4__Adi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_4__Soyadi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_4__DogumTarihi").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_4__KimlikNo").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_4__Uyruk").removeClass("ignore");
            $("#Sigortalilar_SigortaliList_4__KimlikTipi").removeClass("ignore");


            var ailemi = $("#GenelBilgiler_SigortalilarAilemi_control").bootstrapSwitch('status');
            if (ailemi) {
                $("#Sigortalilar_SigortaliList_1__BireyTipi").removeClass("ignore");
                $("#Sigortalilar_SigortaliList_2__BireyTipi").removeClass("ignore");
                $("#Sigortalilar_SigortaliList_3__BireyTipi").removeClass("ignore");
                $("#Sigortalilar_SigortaliList_4__BireyTipi").removeClass("ignore");
            }
            else {
                $("#Sigortalilar_SigortaliList_1__BireyTipi").addClass("ignore");
                $("#Sigortalilar_SigortaliList_2__BireyTipi").addClass("ignore");
                $("#Sigortalilar_SigortaliList_3__BireyTipi").addClass("ignore");
                $("#Sigortalilar_SigortaliList_4__BireyTipi").addClass("ignore");
            }
        },

        tcknKontrol: function (KimlikNo) {
            KimlikNo = String(KimlikNo);
            if (!KimlikNo.match(/^[0-9]{11}$/)) return false;

            pr1 = parseInt(KimlikNo.substr(0, 1));
            pr2 = parseInt(KimlikNo.substr(1, 1));
            pr3 = parseInt(KimlikNo.substr(2, 1));
            pr4 = parseInt(KimlikNo.substr(3, 1));
            pr5 = parseInt(KimlikNo.substr(4, 1));
            pr6 = parseInt(KimlikNo.substr(5, 1));
            pr7 = parseInt(KimlikNo.substr(6, 1));
            pr8 = parseInt(KimlikNo.substr(7, 1));
            pr9 = parseInt(KimlikNo.substr(8, 1));
            pr10 = parseInt(KimlikNo.substr(9, 1));
            pr11 = parseInt(KimlikNo.substr(10, 1));

            if ((pr1 + pr3 + pr5 + pr7 + pr9 + pr2 + pr4 + pr6 + pr8 + pr10) % 10 !== pr11) return false;
            if (((pr1 + pr3 + pr5 + pr7 + pr9) * 7 + (pr2 + pr4 + pr6 + pr8) * 9) % 10 !== pr10) return false;
            if (((pr1 + pr3 + pr5 + pr7 + pr9) * 8) % 10 !== pr11) return false;
            return true;
        }
    }
}



