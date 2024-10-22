var ParitusHelper = new function () {
    return {

        AdresBilgisiKapat: function () {
            $(".adres-bilgileri").addClass("ignore");
            $("#RizikoAdresBilgiler_Il").addClass("ignore");
            $("#UATV_Adres").slideUp("fast");
        },

        AdresBilgisiAc: function () {
            $(".adres-bilgileri").removeClass("ignore");
            $("#RizikoAdresBilgiler_Il").removeClass("ignore");
            $("#UATV_Adres").slideDown("fast");
        },

        AdresTemizle: function () {
            $("#adres-text").html('');
            $("#adres-info").removeClass("in");
            $("#adres-info").attr("style", "display:none");
            $(".adres-bilgileri").empty().trigger("liszt:updated");
        },

        ParitusHata: function () {
            alert("Adres doğrulama sırasında bir hata oluştu. Lütfen tekrar deneyin.");
            ParitusHelper.AdresBilgisiAc("AC");
        },

        ParitusBasarili_Tekli: function (data) {

            if (data.uavtAddressCode != "" && data.uavtAddressCode != "null" && data.uavtAddressCode != null) {
                ParitusHelper.AdresBilgisiKapat();
                $("#RizikoAdresBilgiler_UATVKodu").val(data.uavtAddressCode);
            }
            else {
                ParitusHelper.AdresBilgisiAc();
            }

            if (data.IlKodu != "") {
                $("#RizikoAdresBilgiler_IlKodu").val(data.IlKodu).trigger("liszt:updated");
            }

            if (data.IlceLer != null) {
                $("#RizikoAdresBilgiler_IlceKodu").empty().trigger("liszt:updated");
                $("#RizikoAdresBilgiler_IlceKodu").dropDownFill(data.IlceLer);
                $("#RizikoAdresBilgiler_IlceKodu").val(data.IlceKodu).trigger("liszt:updated");
            }

            if (data.Beldeler != null) {
                $("#RizikoAdresBilgiler_SemtBeldeKodu").empty().trigger("liszt:updated");
                $("#RizikoAdresBilgiler_SemtBeldeKodu").dropDownFill(data.Beldeler);
                $("#RizikoAdresBilgiler_SemtBeldeKodu").val(data.BeldeKodu).trigger("liszt:updated");
            }

            if (data.Mahalleler != null) {
                $("#RizikoAdresBilgiler_MahalleKodu").empty().trigger("liszt:updated");
                $("#RizikoAdresBilgiler_MahalleKodu").dropDownFill(data.Mahalleler);
            }

            var uavtkodu = "";
            if (data.uavtAddressCode != null && data.uavtAddressCode != "null")
                uavtkodu = data.uavtAddressCode;

            $("#adres-text").html(data.FullAdres + " | UAVT Kodu : " + uavtkodu);
            $("#adres-info").addClass("in");
            $("#adres-info").attr("style", "display:normal");

            if (data.PostaKodu != "") { $("#RizikoAdresBilgiler_PostaKodu").val(data.PostaKodu); }
            if (data.Pafta != "") { $("#RizikoDigerBilgiler_PaftaNo").val(data.Pafta); }
            if (data.Parsel != "") { $("#RizikoDigerBilgiler_Parsel").val(data.Parsel); }
            if (data.Ada != "") { $("#RizikoDigerBilgiler_Ada").val(data.Ada); }
            if (data.Kat != "") { $("#RizikoDigerBilgiler_KatNo").val(data.Kat); }

            $("#paritus-coklu-adres").slideUp("fast");

            ParitusHelper.DogrulamaSkoru(data.VerificationScore);
            if (data.Latitude != "" && data.Longitude != "") {

                $("#RizikoAdresBilgiler_Latitude").val(data.Latitude);
                $("#RizikoAdresBilgiler_Longitude").val(data.Longitude);

                MapsGoogle.HaritadaGoster(data.Latitude, data.Longitude);
            }

            DaskGenelBilgiler.AdresTextAktarma();
        },

        ParitusBasarili_Coklu: function (data) {

            $('#tablebodycommendation').empty();
            if (Array.isArray(data.CokluAdres)) {
                $.each(data.CokluAdres, function (i, adres) {
                    $('#tablebodycommendation').append('<tr><td>' + adres + '</td></tr>');
                });
            }

            $("#paritus-coklu-adres").slideDown("fast");
            ParitusHelper.DogrulamaSkoru(data.VerificationScore);
            MapsGoogle.HaritadaGoster(data.Latitude, data.Longitude);
            ParitusHelper.AdresBilgisiAc();
        },

        DogrulamaSkoru: function (VerificationScore) {
            if (VerificationScore >= 700) {
                $("#dogrulama-alert-div").attr("class", "alert alert-success");
                $("#dogrulama-alert-div").attr("style", "display:normal");
            }
            else if (VerificationScore >= 500 & VerificationScore < 700) {
                $("#dogrulama-alert-div").attr("class", "alert alert-info");
                $("#dogrulama-alert-div").attr("style", "display:normal");
            }
            else if (VerificationScore < 500 & VerificationScore >= 250) {
                $("#dogrulama-alert-div").attr("class", "alert");
                $("#dogrulama-alert-div").attr("style", "display:normal");
            }
            else if (VerificationScore < 250) {
                $("#dogrulama-alert-div").attr("class", "alert alert-error");
                $("#dogrulama-alert-div").attr("style", "display:normal");
            }
            $("#dogrulama-skoru").text(VerificationScore);
        },

        ParitusAdresDogrula: function () {
            var adres = $("#RizikoAdresBilgiler_ParitusAdresDogrulama").val();
            if (adres != "" && adres != null) {
                var el = $("#paritus-adres-dogrulama");
                App.blockUI(el);

                ParitusHelper.AdresTemizle();
                ParitusHelper.AdresBilgisiAc();

                $.ajax({
                    url: "/Teklif/Dask/ParitusAdresDogrulama",
                    type: "post",
                    dataType: "json",
                    data: { paritusAdres: adres },
                    statusCode: {
                        200: function (data) {

                            if (data.Durum == 0) {
                                ParitusHelper.ParitusHata();
                            }
                            else if (data.Durum == 2 || data.Durum == 1) {
                                ParitusHelper.ParitusBasarili_Tekli(data);
                            }
                            else if (data.Durum == 3) {
                                ParitusHelper.ParitusBasarili_Coklu(data);
                            }
                            App.unblockUI($("#paritus-adres-dogrulama"));

                        },
                        403: function () {
                            alert("Yetkisiz Erişim"); App.unblockUI($("#paritus-adres-dogrulama"));
                            ParitusHelper.AdresBilgisiAc("AC");
                        },
                        404: function () {
                            alert("404 Hatası"); App.unblockUI($("#paritus-adres-dogrulama"));
                            ParitusHelper.AdresBilgisiAc("AC");
                        },
                        500: function () {
                            alert("Bir hata oluştu"); App.unblockUI($("#paritus-adres-dogrulama"));
                            ParitusHelper.AdresBilgisiAc("AC");
                        }
                    }
                });
            }
        },

        TekrarTeklifParitusAdresDogrula: function () {
            var daire = $("#RizikoAdresBilgiler_DaireKodu").val();
            var bina = $("#RizikoAdresBilgiler_BinaKodu").val();
            var adres = $("#RizikoAdresBilgiler_ParitusAdresDogrulama").val();
            if (adres != "" && adres != null) {
                if ((bina == null || bina == "") && (daire == null || daire == ""))
                { ParitusHelper.ParitusAdresDogrula(); }
            }
        }
    }
}

$(document).ready(function () {

    $("#paritus-dogrula").click(function () {
        ParitusHelper.ParitusAdresDogrula();
    });

    $("#tablebodycommendation").on('click', 'td', function () {
        var t = $(this).text();
        $("#RizikoAdresBilgiler_ParitusAdresDogrulama").val(t);
        $("#paritus-coklu-adres").attr("style", "display:none");
    });

    $("#ornek").click(function () {
        $("#RizikoAdresBilgiler_ParitusAdresDogrulama").val("3499614802");
    });

    $("#temizle").click(function () {
        $("#RizikoAdresBilgiler_ParitusAdresDogrulama").val("");
        $("#dogrulama-alert-div").attr("style", "display:none");
        ParitusHelper.AdresTemizle();
    });
});

var MapsGoogle = function () {

    var mapGeocoding = function () {

        var map = new GMaps({
            div: '#gmap_marker',
            lat: 41.0688177,
            lng: 29.013949,
            zoom: 12
        });
    }

    return {
        init: function () {
            mapGeocoding();
        },

        HaritadaGoster: function (latitude, longitude) {

            if (latitude != "" && longitude != "") {
                var map = new GMaps({
                    div: '#gmap_marker',
                    lat: latitude,
                    lng: longitude,
                    zoom: 12
                });

                $("#RizikoAdresBilgiler_Latitude").val(latitude);
                $("#RizikoAdresBilgile_Longitude").val(longitude);

                var pinColor = "15FF00";
                var pinImage = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|" + pinColor,
                    new google.maps.Size(21, 34),
                    new google.maps.Point(0, 0),
                    new google.maps.Point(10, 34));
                var pinShadow = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_shadow",
                    new google.maps.Size(40, 37),
                    new google.maps.Point(0, 0),
                    new google.maps.Point(12, 35));


                map.addMarker({
                    lat: latitude,
                    lng: longitude,
                    title: 'Konum',
                    animation: google.maps.Animation.DROP,
                    icon: pinImage,
                    shadow: pinShadow,
                });
            }
        }
    };
}();