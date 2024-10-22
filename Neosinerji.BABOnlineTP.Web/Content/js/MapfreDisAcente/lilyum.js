$(document).ready(function () {
    $("#AdresAyniMi_control").on("switch-change", function (e, data) {
        if (data.value) {

            $("#TeslimatAdresi").attr("disabled", true);
            $("#TeslimatIlKodu").attr("disabled", true);
            $("#TeslimatIlceKodu").attr("disabled", true);
        }
        else {

            $("#TeslimatAdresi").attr("disabled", false);
            $("#TeslimatIlKodu").attr("disabled", false);
            $("#TeslimatIlceKodu").attr("disabled", false);
        }
    });

    $('#IletisimAdresi').on('change keyup', function () {
        if ($("#AdresAyniMi_control").bootstrapSwitch('status')) {
            var myVal, newVal = $.makeArray($('#IletisimAdresi').map(function () {
                if (myVal = $(this).val()) {
                    return (myVal);
                }
            })).join(', ');
            $("#TeslimatAdresi").val(newVal);

        }
    });
    $("#TeslimatIlKodu").change(function () {
        $("#TeslimatIlKodu").val($(this).val());
        $("#TeslimatIlceKodu").ilceler("TUR", $(this).val());
    });
    $("#IletisimIlKodu").change(function () {
        $("#IletisimIlKodu").val($(this).val());
        $("#IletisimIlceKodu").ilceler("TUR", $(this).val());

        if ($("#AdresAyniMi_control").bootstrapSwitch('status')) {
            $("#TeslimatIlceKodu").ilceler("TUR", $(this).val());
            var myVal, newVal = $.makeArray($('#IletisimIlKodu').map(function () {
                if (myVal = $(this).val()) {
                    return (myVal);
                }
            })).join(', ');
            $("#TeslimatIlKodu").val(newVal);
        }
    });

    $("#IletisimIlceKodu").change(function () {
        $("#IletisimIlceKodu").val($(this).val());
        if ($("#AdresAyniMi_control").bootstrapSwitch('status')) {
            var myVal, newVal = $.makeArray($('#IletisimIlceKodu').map(function () {
                if (myVal = $(this).val()) {
                    return (myVal);
                }
            })).join(', ');
            $("#TeslimatIlceKodu").val(newVal);
        }
    });
    $("#btn-hesapla").click(function () {

        //var eticaret = $("#ETicaretSozlesmesi").is(":checked");
        //var kullaniciSozlesme = $("#KullaniciSozlesmesi").is(":checked");
        //var lilyumHizmeti = $("#LilyumKartHizmeti").is(":checked");
        //if (eticaret == false || kullaniciSozlesme == false || lilyumHizmeti == false) {
        //    swal("Hata", "Lütfen Sözleşmeleri Onaylayınız.");
        //    $("#btn-hesapla").button("reset");
        //    var isvalid = $("#form1").valid();        
        //    return isvalid;
        //}
        //else {

        //var oncekiSirket = $("#GenelBilgiler_OncekiSigortaSirketi").val();
        //if (oncekiSirket != null && oncekiSirket != "") {
        //    $("#GenelBilgiler_OncekiPoliceBaslangicTarihi").removeClass("ignore");
        //}
        //else {
        //    $("#GenelBilgiler_OncekiPoliceBaslangicTarihi").addClass("ignore");
        //}
        var deneme = $(this).text().trim();
        if (deneme != "Lütfen bekleyiniz.") {

            var isvalid = $("#form1").valid();
            if (isvalid) {
                $("#TeklifId").val("");
                $(this).button("loading");
                $(".switcher").find(":input").switchFix();
                var disabled = $("#form1").find(':input:disabled').removeAttr('disabled');
                var contents = $("#form1").serialize();
                disabled.attr('disabled', 'disabled');
                var gosterilenler = [];
                $.ajax(
                    {
                        type: "POST",
                        url: "/Teklif/LilyumKart/Hesapla",
                        data: contents,
                        async: false,
                        success: function (data) {
                            setTimeout(function () {
                                if (data.id > 0) {
                                    $.ajax(
                                        {
                                            type: "POST",
                                            url: "/Teklif/LilyumKart/LilyumParaticaUrl",
                                            data: { isId: data.id, guid: data.g, gosterilenler: gosterilenler },
                                            dataType: "json",
                                            success: function (data) {
                                                if (data.LilyumParaticaURL != "") {
                                                    window.location.href = data.LilyumParaticaURL;
                                                }
                                                if (data.hata != "") {
                                                    $("#btn-hesapla").button("reset");
                                                    swal("Hata", data.hata);
                                                }
                                            }
                                        });

                                    //var url = $("#paraticaURL").html();
                                    //window.location.href = url;
                                }
                                else {
                                    $("#btn-hesapla").button("reset");
                                    alert("hata");
                                }
                            }, 18000)
                            //else if (data.id == 0) {
                            //    $("#btn-hesapla").show();
                            //    $("#btn-hesapla").button("reset");
                            //    alert(data.hata);
                            //    $('#form_wizard_1').bootstrapWizard("previous");
                            //    $("#teklif-fiyat-container").css({ "visibility": "hidden" });
                            //}
                            //$("#btn-hesapla").button("reset");
                        },
                        error: function () {
                            $("#btn-hesapla").button("reset");
                        }
                    });
            }
        }
        //}
    });
    $("#KrediKartiMi_control").addClass("switcher switcher-small deactivate");
    $("#KrediKartiMi").attr("disabled", "");
    $("#GenelBilgiler_YenilemeMi_control").on("switch-change", EskiPoliceKontrol);

    // ==== Odeme tipi, şekli ve taksit sayısı kontrol ediliyor. ==== //
    $("#Odeme_OdemeSekli_control").on("switch-change", OdemeSekliDegisti);

});


function EskiPoliceKontrol(e, data) {
    if (data.value) {
        $("#oncekiPolice").show();
    }
    else {
        $("#oncekiPolice").hide();
    }
}

function OdemeSekliDegisti(e, data) {
    if (data.value) {
        $("#pesin").css("background-color", "#dddddd");
        $("#taksitli").css("background-color", "#ffffff");
    }
    else {
        $("#taksitli").css("background-color", "#dddddd");
        $("#pesin").css("background-color", "#ffffff");
    }
}