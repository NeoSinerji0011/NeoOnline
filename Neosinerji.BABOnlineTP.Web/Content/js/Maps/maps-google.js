var MapsGoogle = function () {


    var mapGeocoding = function () {

        var map = new GMaps({
            div: '#gmap_geocoding',
            lat: 41.0688177,
            lng: 29.013949,
            zoom: 12
        });

        var handleAction = function () {
            var text = $.trim($('#gmap_geocoding_address').val());
            GMaps.geocode({
                address: text,
                callback: function (results, status) {
                    if (status == 'OK') {
                        var latlng = results[0].geometry.location;
                        map.setCenter(latlng.lat(), latlng.lng());
                        map.addMarker({
                            lat: latlng.lat(),
                            lng: latlng.lng()
                        });
                        //App.scrollTo($('#gmap_geocoding'));
                    }
                }
            });
        }

        $("#get-location").click(function () {
            $("#get-acente").attr("style", "display:normal");
            $("#refresh").attr("style", "display:normal");
            $(this).attr("disabled", "disabled");

            GMaps.geolocate({
                success: function (position) {
                    map.setCenter(position.coords.latitude, position.coords.longitude);

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
                        lat: position.coords.latitude,
                        lng: position.coords.longitude,
                        title: 'Konum',
                        animation: google.maps.Animation.DROP,
                        icon: pinImage,
                        shadow: pinShadow,
                        infoWindow: {
                            content: '<div id="content">' +
      '<div id="siteNotice">' +
      '</div>' +
      '<h4 id="firstHeading" class="firstHeading">Buradasiniz</h4>' +
      '<div id="bodyContent">' +
      '<a href="javascript:;"class="acente-bul"> Size yakin acenteleri bulun </a>' +
      '</div>' +
      '</div>'

                        }
                    });
                },
                error: function (error) {
                    alert('Geolocation failed: ' + error.message);
                },
                not_supported: function () {
                    alert("Your browser does not support geolocation");
                },
                always: function () {
                    //alert("Geolocation Done!");
                }
            });
        });

        $(".acente-bul").live("click", function () {
            $.ajax({
                url: "/Common/GetTVMLocation",
                data: "",
                type: "post",
                success: function (data) {
                    var kod = "";
                    var Unvan = "";
                    var Tel = "";
                    var Adres = "";
                    var lat = 0; // 41.0688977;
                    var lng = 0; //29.029199;
                    var model = data.model;

                    for (var i = 0; i < model.List.length; i++) {

                        kod = model.List[i].Kod.toString();;
                        Unvan = model.List[i].Unvan;
                        Tel = model.List[i].Telefon;
                        Adres = model.List[i].Adres;
                        lat = model.List[i].Lat;
                        lng = model.List[i].Lgn;


                        map.addMarker({
                            lat: lat,
                            lng: lng,
                            title: kod,
                            click: function (e) {
                                if (console.log) console.log(e);
                                $("#TVMKodu").val(e.title);
                            },
                            infoWindow: {
                                content: '<div id="content">' +
                '<div id="siteNotice">Ad   &nbsp; : ' + Unvan +
                '</br>Tel   &nbsp;  : ' + Tel +
                '</br>Adres : ' + Adres + ' </div>' +
                '<h5 id="firstHeading" class="firstHeading"><a class=" btn green" data-toggle="modal" href="#stack1" style="font-size: 18px; font-style: normal;font-weight: bold;">Acente Benimle iletisime gecsin</a></h5>' +
                '<h5 id="firstHeading1" class="firstHeading"><a class=" btn " data-toggle="modal" href="#stackTeklifPolice" style="background-color:#cce6ff; color:black ; font-family:  Calibri,Candara,Segoe,Segoe UI,Optima,Arial,sans-serif;font-size: 24px; font-style: normal;font-weight: bold;">Online Teklif Al/Poliçeleştir</a></h5>' +
                '<div id="bodyContent">' +
                '</div>' +
                '</div>'
                            }
                        });
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                },
                dataType: "json",
                traditional: true
            })
        });

        $('#gmap_geocoding_btn').click(function (e) {
            e.preventDefault();
            handleAction();
        });

        $("#gmap_geocoding_address").keypress(function (e) {
            var keycode = (e.keyCode ? e.keyCode : e.which);
            if (keycode == '13') {
                e.preventDefault();
                handleAction();
            }
        });

    }

    return {
        init: function () {
            mapGeocoding();
        }
    };
}();

$("#form-gonder").live("click", function () {
    $("#form1").validate().form();
    if ($("#form1").valid()) {
        var formData = $("#form1").serialize();

        $.post("/Common/SendIletisimFormu", formData, function (data) {
            if (data == "True") {
                alert("Bilgileriniz iletildi en kisa zamanda sizinle iletisime gecilecektir.")
            }
            else {
                alert("Bir hata olustu lutfen tekrar deneyin.")
            }
        });
        $("#close").trigger("click");
        alert("İşlem Yapılıyor lütfen bekleyiniz.");
    }
});







