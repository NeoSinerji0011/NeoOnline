$(document).ready(function () {

    $("#paritus-dogrula").click(function () {

        var adres = $("#MusteriAdresModel_ParitusAdresDogrulama").val();
        if (adres != "") {
            var el = $("#paritus-adres-dogrulama");

            App.blockUI(el);


            $.ajax({
                url: "/Musteri/Musteri/ParitusAdresDogrulama",
                type: "post",
                dataType: "json",
                data: { paritusAdres: adres },
                statusCode: {
                    200: function (data) {
                        if (data.Durum == 0) {
                            alert("Adres doğrulama sırasında bir hata oluştu. Lütfen tekrar deneyin.");
                        }
                        else if (data.Durum == 2 || data.Durum == 1) {
                            if (data.Iller != null) {

                                $("#MusteriAdresModel_IlKodu").empty();
                                $("#MusteriAdresModel_IlKodu").dropDownFill(data.Iller);
                                $("#MusteriAdresModel_IlKodu").val(data.IlKodu);

                                if (data.IlceLer != null) {
                                    $("#MusteriAdresModel_IlceKodu").empty();
                                    $("#MusteriAdresModel_IlceKodu").dropDownFill(data.IlceLer);
                                    $("#MusteriAdresModel_IlceKodu").val(data.IlceKodu);
                                }
                            }

                            $(".paritus-adres").val("");

                            $("#MusteriAdresModel_Adres").val(data.FullAdres);
                            $("#MusteriAdresModel_Mahalle").val(data.Mahalle);
                            $("#MusteriAdresModel_Cadde").val(data.Cadde);
                            $("#MusteriAdresModel_Sokak").val(data.Sokak);
                            $("#MusteriAdresModel_DaireNo").val(data.DaireNo);
                            $("#MusteriAdresModel_BinaNo").val(data.BinaNo);
                            $("#MusteriAdresModel_PostaKodu").val(data.PostaKodu);
                            $("#MusteriAdresModel_Apartman").val(data.Apartman);

                            //Diğer bölümüne eklenenler
                            var diger = "";

                            if (data.IsMerkezi != '' || data.IsMerkezi != null || data.IsMerkezi.lenght > 0)
                                diger += (data.IsMerkezi + " İş Merkezi ");

                            if (data.Kat != "" || data.Kat != null || data.Kat.lenght > 0)
                                diger += (data.Kat + ".Kat ");

                            if (data.Blok != "" || data.Blok != null || data.Blok.lenght > 0)
                                diger += (data.Blok + " Blok ");

                            if (data.Undefined != "" || data.Undefined != null || data.Undefined.lenght > 0)
                                diger += (data.Undefined);

                            $("#MusteriAdresModel_Diger").val(diger);
                            $("#paritus-coklu-adres").slideUp("fast");

                            MusteriEkle.CaddeSokakDegisti();
                            DogrulamaSkoru(data.VerificationScore);
                            MapsGoogle.HaritadaGoster(data.Latitude, data.Longitude);
                        }
                        else if (data.Durum == 3) {
                            $('#tablebodycommendation').empty();
                            if (Array.isArray(data.CokluAdres)) {
                                $.each(data.CokluAdres, function (i, adres) {
                                    $('#tablebodycommendation').append('<tr><td>' + adres + '</td></tr>');
                                });
                            }
                            $("#paritus-coklu-adres").slideDown("fast");
                            DogrulamaSkoru(data.VerificationScore);
                            MapsGoogle.HaritadaGoster(data.Latitude, data.Longitude);
                        }
                        App.unblockUI(el);

                    },
                    403: function ()
                    { alert("Yetkisiz Erişim"); App.unblockUI(el); },
                    404: function ()
                    { alert("404 Hatası"); App.unblockUI(el); },
                    500: function ()
                    { alert("Bir hata oluştu"); App.unblockUI(el); }
                }
            });
        }
    });

    function DogrulamaSkoru(VerificationScore) {
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
    }

    $("#tablebodycommendation").on('click', 'td', function () {
        var t = $(this).text();
        $("#MusteriAdresModel_ParitusAdresDogrulama").val(t);
    });

    $("#ornek").click(function () {
        $("#MusteriAdresModel_ParitusAdresDogrulama").val("Hasanpaşa Mh. Yeniyol Sk. Etap İş Mrkz. C Blok Kat:4 Daire:12 Kadıköy İSTANBUL");
    });


    $("#temizle").click(function () {
        $("#MusteriAdresModel_ParitusAdresDogrulama").val("");
        $("#dogrulama-alert-div").attr("style", "display:none");
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

                $("#MusteriAdresModel_Latitude").val(latitude);
                $("#MusteriAdresModel_Longitude").val(longitude);

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



