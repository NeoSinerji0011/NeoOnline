var MusterilerimHelper = new function () {

    var mapGeocoding = function () {

        var map = new GMaps({
            div: '#gmap_geocoding',
            lat: 41.0688177,
            lng: 29.013949,
            zoom: 10
        });
    }

    return {
        init: function () {
            mapGeocoding();
        },

        HataOlustu: function () {
            alert("Bir hata oluştu.");
        },

        YetkisizErisim: function () { alert("Yetkisiz Erişim"); },

        MusteriAra: function () {
            var model = {
                TVMKodu: $("#TVMKodu").val(),
                MusteriTipi: $("#MusteriTipKodu").val(),
                Ad: $("#AdiUnvan").val(),
                Soyad: $("#SoyadiUnvan").val(),
                MusteriSayisi: $("#MusteriSayisi").val()
            }

            $.ajax({
                url: "/Musteri/Musteri/GetMusterilerim",
                data: model,
                type: "post",
                dataType: "json",
                traditional: true,
                success: function (data) { MusterilerimHelper.MusteriAra_Success(data); $("#search").button("reset"); },
                error: function (XMLHttpRequest, textStatus, errorThrown) { MusterilerimHelper.HataOlustu(); $("#search").button("reset"); }
            });
        },

        MusteriAra_Success: function (data) {
            if (data == null || data == "" || data.Success == "False") { MusterilerimHelper.HataOlustu(); }
            if (data.Authority == "False") { MusterilerimHelper.YetkisizErisim(); }

            var tip = "";
            var AdiSoyadi = "";
            var Tel = "";
            var Email = "";
            var lat = 0; // 41.0688977;
            var lng = 0; //29.029199;
            var model = data.MusteriList;

            if (model.length == 0) {
                alert("Aradığınız kriterlerde müşteri bulunamadı.");
            }
            else {

                $("#gmap_geocoding").html('');

                var map = new GMaps({
                    div: '#gmap_geocoding',
                    lat: 41.0688177,
                    lng: 29.013949,
                    zoom: 10
                });

                var sahisIMG = {
                    url: '/Content/img/Maps/blue_MarkerS.png',
                    // This marker is 20 pixels wide by 32 pixels tall.
                    size: new google.maps.Size(20, 32),
                    // The origin for this image is 0,0.
                    origin: new google.maps.Point(0, 0),
                    // The anchor for this image is the base of the flagpole at 0,32.
                    anchor: new google.maps.Point(0, 32)
                };

                var yabanciIMG = {
                    url: '/Content/img/Maps/brown_MarkerY.png',
                    size: new google.maps.Size(20, 32),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(0, 32)
                };

                var tuzelIMG = {
                    url: '/Content/img/Maps/darkgreen_MarkerT.png',
                    size: new google.maps.Size(20, 32),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(0, 32)
                };

                var firmaIMG = {
                    url: '/Content/img/Maps/orange_MarkerF.png',
                    size: new google.maps.Size(20, 32),
                    origin: new google.maps.Point(0, 0),
                    anchor: new google.maps.Point(0, 32)
                };


                var pinShadow = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_shadow",
                   new google.maps.Size(40, 37),
                   new google.maps.Point(0, 0),
                   new google.maps.Point(12, 35));

                var sahisM = 0;
                var tuzelM = 0;
                var yabanciM = 0;
                var firmaM = 0;
                var kordinatsizM = 0;
                var toplamM = 0;
                var link = "";

                for (var i = 0; i < model.length; i++) {

                    toplamM++;

                    AdiSoyadi = model[i].AdiSoyadi;
                    Tel = model[i].Tel;
                    Email = model[i].Email;
                    lat = model[i].Latitude;
                    lng = model[i].Longitude;
                    tip = model[i].MusteriTipi;
                    link = "/Musteri/Musteri/Detay/" + model[i].MusteriKodu;


                    if (lng == "" || lng == null || lng == "null" || lat == "" || lat == null || lat == "null") {
                        kordinatsizM++;
                        continue;
                    }

                    var image = sahisIMG;

                    switch (tip) {
                        case 1: image = sahisIMG; sahisM++; break;
                        case 2: image = tuzelIMG; tuzelM++; break;
                        case 3: image = firmaIMG; firmaM++; break;
                        case 4: image = yabanciIMG; yabanciM++; break;
                    }

                    map.addMarker({
                        lat: lat,
                        lng: lng,
                        icon: image,
                        animation: google.maps.Animation.DROP,
                        title: AdiSoyadi,
                        //click: function (e) {
                        //    //if (console.log) console.log(e);
                        //    //$("#TVMKodu").val(e.title);
                        //},
                        infoWindow: {
                            content: '<div id="content">' +
                '<div id="siteNotice">Ad Soyad  &nbsp; : ' + AdiSoyadi +
                '</br>Tel   &nbsp;  : ' + Tel +
                '</br>Email : ' + Email + ' </div>' +
                '<h5 id="firstHeading" class="firstHeading"><a class=" btn green"  href="' + link + '">Detay</a></h5>' +
                '<div id="bodyContent">' +
                '</div>' +
                '</div>'
                        }
                    });
                }
                var infoModel = {
                    SahisM: sahisM,
                    TuzelM: tuzelM,
                    YabanciM: yabanciM,
                    FirmaM: firmaM,
                    KordinatsizM: kordinatsizM,
                    ToplamM: toplamM
                };

                MusterilerimHelper.InfoDivSet(infoModel);
            }
        },

        InfoDivSet: function (infoModel) {
            var text = "";
            if (infoModel.SahisM > 0) {
                text = "Şahis Müşterisi : " + infoModel.SahisM + " adet  <img src='/Content/img/Maps/blue_MarkerS.png' alt='Sahis'/> <br/>"
            }

            if (infoModel.TuzelM > 0) {
                text += "Tüzel Müşteri : " + infoModel.TuzelM + " adet  <img src='/Content/img/Maps/darkgreen_MarkerT.png' alt='Tüzel'/><br/>"
            }

            if (infoModel.YabanciM > 0) {
                text += "Yabancı Müşteri : " + infoModel.YabanciM + " adet  <img src='/Content/img/Maps/brown_MarkerY.png' alt='Yabanci'/><br/>"
            }

            if (infoModel.FirmaM > 0) {
                text += "Firma Müşterisi : " + infoModel.FirmaM + " adet  <img src='/Content/img/Maps/orange_MarkerF.png' alt='Firma'/><br/>"
            }

            if (infoModel.KordinatsizM > 0) {
                text += "Kordinat bilgisi eksik müşteri : " + infoModel.KordinatsizM + " adet <br/>"
            }

            if (infoModel.ToplamM > 0) {
                text += "Toplam Müşteri : " + infoModel.ToplamM + " adet"
            }


            $("#info-div-text").html(text);
            $("#info-div").slideDown("fast");
        }
    }
}

$("#search").click(function () {
    $("#search").button("loading");
    MusterilerimHelper.MusteriAra();
    $("#info-div").slideUp("fast");
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

$(document).ready(function () {
    MusterilerimHelper.init();
});