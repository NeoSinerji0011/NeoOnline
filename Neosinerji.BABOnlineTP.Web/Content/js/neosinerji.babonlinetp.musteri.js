var MusteriGenel = new function () {
    return {

        // -----------------ORTAK IŞLEMLERI----------------------- //

        BirHataOlustuMesaj: function () {
            $.gritter.add({ title: 'Hata Mesajı!', text: "Bir hata oluştu. Lüften tekrar deneyin" });
        },

        YetkisizErisimMesaj: function () {
            $.gritter.add({ title: 'Erişim Engellendi!', text: "Yetkisiz Erişim" });
        },

        IslemBasarili: function () {
            $.gritter.add({ title: 'Bilgi Mesajı!', text: "İşlem Başarılı" });
        },



        // -----------------TELEFON IŞLEMLERI----------------------- //

        MusteriTelefonEkleGetPartial: function () {

            var musteriKodu = $("#MusteriGuncelleModel_MusteriKodu").val();

            if (musteriKodu != "") {
                $.ajax({
                    type: "GET",
                    url: "/Musteri/Musteri/TelefonEkle",
                    data: { MusteriKodu: musteriKodu },
                    dataType: "html",
                    success: function (data) {
                        if (data != "null" && data != "") {
                            $("#telefon-modal-div").html(data);
                            $.validator.unobtrusive.parse("#telefon-modal-div");
                            $("#telefon-modal").modal('show');
                        }
                        else { MusteriGenel.YetkisizErisimMesaj(); }
                    },
                    error: function () { MusteriGenel.BirHataOlustuMesaj(); }
                });
            } else { MusteriGenel.BirHataOlustuMesaj(); }
        },

        MusteriTelefonEkle: function () {
            $("#telefon-ekle-form").validate().form();

            if ($("#telefon-ekle-form").valid()) {
                var formData = $("#telefon-ekle-form").serialize();

                $.post("/Musteri/Musteri/TelefonEkle", formData,
					function (data) {
					    if (data == "null" || data == "") {
					        $("#telefon-modal").modal('hide');
					        MusteriGenel.IslemBasarili();
					    }
					    else {
					        $("#telefon-modal").modal('hide');
					        $("#telefon-modal-div").html(data);
					        $("#telefon-modal").modal('show');
					    }
					    MusteriGenel.MusteriTelefonGet();
					}, "html");
            }
        },

        MusteriTelefonGuncelleGetPartial: function (musteriKodu, siraNo) {

            if (musteriKodu != "" && siraNo != "" && musteriKodu != "0") {

                $.ajax({
                    url: "/Musteri/Musteri/TelefonGuncelle",
                    type: "get",
                    dataType: "html",
                    data: { musteriKodu: musteriKodu, siraNo: siraNo },
                    success: function (result) {
                        if (result != "") {
                            $("#telefon-modal-div").html(result);
                            $.validator.unobtrusive.parse("#telefon-modal-div");
                            $("#telefon-modal").modal('show');
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        MusteriGenel.BirHataOlustuMesaj();
                    }
                });
            }
            else { MusteriGenel.BirHataOlustuMesaj(); }

        },

        MusteriTelefonGuncelle: function () {
            $("#telefon-ekle-form").validate().form();

            if ($("#telefon-ekle-form").valid()) {

                var formData = $("#telefon-ekle-form").serialize();

                $.post("/Musteri/Musteri/TelefonGuncelle", formData,
                    function (data) {
                        if (data == "null" || data == "") {
                            $("#telefon-modal").modal('hide');
                            MusteriGenel.IslemBasarili();
                        }
                        else {
                            $("#telefon-modal").modal('hide');
                            $("#telefon-modal-div").html(data);
                            $("#telefon-modal").modal('show');
                        }
                        MusteriGenel.MusteriTelefonGet();
                    }, "html");
            }
        },

        MusteriTelefonGet: function () {
            var musteriKodu = $("#MusteriGuncelleModel_MusteriKodu").val();

            if (musteriKodu != "") {
                $.get("/Musteri/Musteri/TelefonlariDoldur",
                       { musteriKodu: musteriKodu, sayfaAdi: "guncelle" },
                      function (data) {
                          if (data != "" && data != "null") {
                              $("#telefonlar-container").html(data);
                          }
                      },
                      "html");
            }
            else { MusteriGenel.BirHataOlustuMesaj(); }
        },

        MusteriTelefonSil: function (musteriKodu, siraNo) {
            //!!!!!!!!!!!!!!!  TELEFON DELETE !!!!!

            if (musteriKodu != "" && siraNo != "" && musteriKodu != "0") {

                $('#delete-confirm-btn').unbind('click');
                $("#delete-confirm-btn").click(function () {

                    $("#delete-confirmation").modal('hide');

                    $.ajax({
                        url: "/Musteri/Musteri/TelefonSil/",
                        method: "post",
                        dataType: "json",
                        data: { MusteriKodu: musteriKodu, SiraNo: siraNo },
                        success: function (result) {
                            if (result != "") { $.gritter.add({ title: 'Bilgi Mesajı!', text: result }); }
                            else { MusteriGenel.BirHataOlustuMesaj }
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            if (textStatus === "parsererror") { MusteriGenel.YetkisizErisimMesaj(); }
                            else { MusteriGenel.BirHataOlustuMesaj(); }
                        }
                    });
                });

                $("#delete-confirmation").modal('show');
            }
            else { MusteriGenel.BirHataOlustuMesaj(); };
        },



        // -----------------ADRES IŞLEMLERI----------------------- //

        MusteriAdresEkleGetPartial: function () {

            var musteriKodu = $("#MusteriGuncelleModel_MusteriKodu").val();

            if (musteriKodu != "") {

                $.get("/Musteri/Musteri/AdresEkle",
                      { musteriKodu: musteriKodu },
                      function (data) {
                          if (data != "" && data != "null") {
                              $("#adres-modal-div").html(data);
                              $.validator.unobtrusive.parse("#adres-modal-div");
                              $("#adres-modal").modal('show');
                          }
                      },
                      "html");
            }
            else { MusteriGenel.BirHataOlustuMesaj(); }
        },

        MusteriAdresEkle: function () {

            $("#adres-ekle-form").validate().form();

            if ($("#adres-ekle-form").valid()) {

                var formData = $("#adres-ekle-form").serialize();
                $.post("/Musteri/Musteri/AdresEkle", formData,
                    function (data) {
                        if (data == "null") { $("#adres-modal").modal('hide'); }
                        else {
                            $("#adres-modal").modal('hide');
                            $("#adres-modal-div").html(data);
                            $("#adres-modal").modal('show');
                        }
                        MusteriGenel.MusteriAdresGet();
                    }, "html");
            }
        },

        MusteriAdresGuncelleGetPartial: function (musteriKodu, siraNo) {


            if (musteriKodu != "" && siraNo != "") {

                $.get("/Musteri/Musteri/AdresGuncelle",
                      { musteriKodu: musteriKodu, siraNo: siraNo },
                      function (data) {
                          if (data != "" && data != "null") {
                              $("#adres-modal-div").html(data);
                              $.validator.unobtrusive.parse("#adres-modal-div");
                              $("#adres-modal").modal('show');
                          }
                          else { MusteriGenel.YetkisizErisimMesaj(); }
                      },
                      "html");
            }
            else { MusteriGenel.BirHataOlustuMesaj(); }
        },

        MusteriAdresGuncelle: function () {

            $("#adres-ekle-form").validate().form();

            if ($("#adres-ekle-form").valid()) {

                var formData = $("#adres-ekle-form").serialize();
                $.post("/Musteri/Musteri/AdresGuncelle", formData,
                    function (data) {
                        if (data == "null" || data == "") { $("#adres-modal").modal('hide'); }
                        else {
                            $("#adres-modal").modal('hide');
                            $("#adres-modal-div").html(data);
                            $("#adres-modal").modal('show');
                        }
                        MusteriGenel.MusteriAdresGet();
                    }, "html");
            }
        },

        MusteriAdresSil: function (musteriKodu, siraNo) {

            if (musteriKodu != "" && siraNo != "") {

                $.post("/Musteri/Musteri/AdresSilKontrol/", { MusteriKodu: musteriKodu, SiraNo: siraNo },
                          function (data) {

                              if (data.Basarili == "false") {
                                  //Yetkisiz Erişim kontrolu
                                  if (data.Yetkili == "false") { MusteriGenel.YetkisizErisimMesaj(); }
                                  else
                                  {
                                      $("#kayitsilinemez").modal('show');
                                      $.gritter.add({ title: 'Hata Mesajı!', text: data.Message });
                                  }
                              }
                              else {
                                  $("#delete-confirmation").modal('show');
                              }
                          });

                $('#delete-confirm-btn').unbind('click');
                $("#delete-confirm-btn").click(function () {
                    $.post("/Musteri/Musteri/AdresSil/",
                           { MusteriKodu: musteriKodu, SiraNo: siraNo },
                           function (data) {
                               $("#delete-confirmation").modal('hide');
                               if (data.Yetkili == "true") {
                                   if (data.Basarili == "true")
                                       $.gritter.add({ title: 'Bilgi Mesajı!', text: data.Message });
                               }
                               else { MusteriGenel.YetkisizErisimMesaj(); }

                               MusteriGenel.MusteriAdresGet();
                           });
                });
            }
            else { MusteriGenel.BirHataOlustuMesaj(); }
        },

        MusteriAdresGet: function () {

            var musteriKodu = $("#MusteriGuncelleModel_MusteriKodu").val();

            if (musteriKodu != "") {

                $.post("/Musteri/Musteri/AdresleriDoldur", { musteriKodu: musteriKodu, sayfaAdi: "guncelle" },
                      function (data) { if (data != "null" && data != "") { $("#adresler-container").html(data); } }, "html");
            }
            else { MusteriGenel.BirHataOlustuMesaj(); }
        },



        // -----------------DOKUMAN IŞLEMLERI----------------------- //

        MusteriDokumanEkleGetPartial: function () {

            var musterikodu = $("#MusteriGuncelleModel_MusteriKodu").val();
            if (musterikodu != "") {

                $.get("/Musteri/Musteri/Upload/", { musteriKodu: musterikodu },
                      function (data) {

                          if (data != "" && data != "null") {
                              $("#dokuman-modal-div").html(data);
                              $.validator.unobtrusive.parse("#dokuman-modal-div");
                              $("#dokuman-modal").modal('show');
                          } else { MusteriGenel.YetkisizErisimMesaj(); }

                      }, "html");

            } else { MusteriGenel.BirHataOlustuMesaj(); }
        },

        MusteriDokumanEkle: function () {
            $("#dokuman-ekle-form").validate().form();

            if ($("#dokuman-ekle-form").valid()) {
                var formData = new FormData(document.forms.namedItem('dokuman-ekle-form'));

                var oReq = new XMLHttpRequest();
                oReq.open("POST", "/Musteri/Musteri/Upload", true);
                oReq.send(formData);
                oReq.onload = function (oEvent) {
                    $("#dokuman-modal").modal('hide');
                    if (oReq.status == 200 && oReq.responseText == "") {
                        MusteriGenel.MusteriDokumanGet();
                        MusteriGenel.IslemBasarili();
                    } else {
                        $("#dokuman-modal-div").html(oReq.responseText);
                        $("#dokuman-modal").modal('show');
                    }
                }
            }
        },

        MusteriDokumanSil: function (musteriKodu, siraNo) {

            if (musteriKodu != "" && siraNo != "") {

                $('#delete-confirm-btn').unbind('click');
                $("#delete-confirm-btn").click(function () {     
                    
                    $.ajax({
                        type: "Post",
                        url: "/Musteri/Musteri/DokumanSil/",
                        data: { MusteriKodu: musteriKodu, SiraNo: siraNo },
                        success: function (data) {
                            $("#delete-confirmation").modal('hide');

                            if (data.Basarili == "true")
                            { $.gritter.add({ title: 'Bilgi Mesajı!', text: data.Message }); }
                            else
                            {
                                if (data.Yetkili == "false") { MusteriGenel.YetkisizErisimMesaj(); }
                                else { $.gritter.add({ title: 'Bilgi Mesajı!', text: data.Message }); }
                            }
                            MusteriGenel.MusteriDokumanGet();
                        },
                        error: function () { MusteriGenel.BirHataOlustuMesaj(); }
                    });
                });
                $("#delete-confirmation").modal('show');
            }
        },

        MusteriDokumanGet: function () {
            var musteriKodu = $("#MusteriGuncelleModel_MusteriKodu").val();

            if (musteriKodu != "") {

                $.post("/Musteri/Musteri/DokumanlariDoldur", { musteriKodu: musteriKodu, sayfaAdi: "guncelle" },
                      function (data) {
                          if (data != "" && data != "null") { $("#dokumanlar-container").html(data); }
                      }, "html");
            }
        },



        // -----------------NOT IŞLEMLERI----------------------- //

        MusteriNotEkleGetPartial: function () {
            var musteriKodu = $("#MusteriGuncelleModel_MusteriKodu").val();

            if (musteriKodu != "") {

                $.get("/Musteri/Musteri/NotEkle",
                      { musteriKodu: musteriKodu },
                      function (data) {

                          if (data != "" && data != "null") {
                              $("#not-modal-div").html(data);
                              $.validator.unobtrusive.parse("#not-modal-div");
                              $("#not-modal").modal('show');
                          }
                          else { MusteriGenel.YetkisizErisimMesaj(); }

                      }, "html");

            } else { MusteriGenel.BirHataOlustuMesaj(); }
        },

        MusteriNotEkle: function () {

            $("#not-ekle-form").validate().form();

            if ($("#not-ekle-form").valid()) {

                var formData = $("#not-ekle-form").serialize();
                $.post("/Musteri/Musteri/NotEkle", formData,
                    function (data) {
                        if (data == "null" || data == "") {
                            $("#not-modal").modal('hide');
                            $.gritter.add({ title: 'Bilgi Mesajı!', text: "İşlem Başarılı" });
                        }
                        else {
                            $("#not-modal").modal('hide');
                            $("#not-modal-div").html(data);
                            $("#not-modal").modal('show');
                        }
                        MusteriGenel.MusteriNotGet();
                    }, "html");
            }
        },

        MusteriNotSil: function (musteriKodu, siraNo) {

            if (musteriKodu != "" && siraNo != "") {

                $('#delete-confirm-btn').unbind('click');
                $("#delete-confirm-btn").click(function () {

                    $("#delete-confirmation").modal('hide');
                    $.post("/Musteri/Musteri/NotSil/", { MusteriKodu: musteriKodu, SiraNo: siraNo },

                           function (data) {

                               if (data.Basarili == "true")
                               { $.gritter.add({ title: 'Bilgi Mesajı!', text: data.Message }); }
                               else
                               {
                                   if (data.Yetkili == "false") { $.gritter.add({ title: 'Erişim Engellendi!', text: data.Message }); }
                                   else { $.gritter.add({ title: 'Bilgi Mesajı!', text: data.Message }); }

                               }
                               MusteriGenel.MusteriNotGet();
                           });
                });

                $("#delete-confirmation").modal('show');
            }
        },

        MusteriNotGet: function () {
            var musteriKodu = $("#MusteriGuncelleModel_MusteriKodu").val();

            if (musteriKodu != "") {
                $.get("/Musteri/Musteri/NotlariDoldur", { musteriKodu: musteriKodu, sayfaAdi: "guncelle" },
                      function (data) {

                          if (data != "" && data != "null") { $("#notlar-container").html(data); }
                          else { MusteriGenel.YetkisizErisimMesaj(); }

                      }, "html");
            }
            else { $.gritter.add({ title: 'Bilgi Mesajı!', text: "Müşteri notları güncellenemedi. Lütfen sayfayı yenileyiniz." }); }
        },
    }
}


// -----------------TELEFON IŞLEMLERI----------------------- //
//Ekleme
$("#telefon-ekle").click(function () { MusteriGenel.MusteriTelefonEkleGetPartial(); });
$("#telefon-ekle-btn").live("click", function () { MusteriGenel.MusteriTelefonEkle(); });

//Guncelleme 
$(".guncelle-telefon").live("click", function () {
    var musteriKodu = $(this).attr("musteri-kodu");
    var siraNo = $(this).attr("sira-no");

    MusteriGenel.MusteriTelefonGuncelleGetPartial(musteriKodu, siraNo);
});
$("#telefon-guncelle-btn").live("click", function () { MusteriGenel.MusteriTelefonGuncelle(); });

//Silme
$(".delete-telefon").live("click", function () {
    var musteriKodu = $(this).attr("musteri-kodu");
    var siraNo = $(this).attr("sira-no");

    MusteriGenel.MusteriTelefonSil(musteriKodu, siraNo);
});




// -----------------ADRES IŞLEMLERI----------------------- //
//Ekleme
$("#adres-ekle").click(function () { MusteriGenel.MusteriAdresEkleGetPartial() });
$("#adres-ekle-btn").live("click", function () { MusteriGenel.MusteriAdresEkle(); });

//Guncelleme
$(".adres-guncelle").live("click", function () {
    var musteriKodu = $(this).attr("musteri-kodu");
    var siraNo = $(this).attr("sira-no");

    MusteriGenel.MusteriAdresGuncelleGetPartial(musteriKodu, siraNo);
});
$("#adres-guncelle-btn").live("click", function () { MusteriGenel.MusteriAdresGuncelle(); });

//Silme
$(".delete-adres").live("click", function () {
    var musteriKodu = $(this).attr("musteri-kodu");
    var siraNo = $(this).attr("sira-no");

    MusteriGenel.MusteriAdresSil(musteriKodu, siraNo);
});




// -----------------DOKUMAN IŞLEMLERI----------------------- //
//Ekleme
$("#dokuman-ekle").click(function () { MusteriGenel.MusteriDokumanEkleGetPartial(); });
$("#dokuman-ekle-btn").live("click", function () { MusteriGenel.MusteriDokumanEkle(); });

//Silme
$(".delete-dokuman").live("click", function () {
    var musteriKodu = $(this).attr("musteri-kodu");
    var siraNo = $(this).attr("sira-no");

    MusteriGenel.MusteriDokumanSil(musteriKodu, siraNo);
});




// -----------------NOT IŞLEMLERI----------------------- //
//Ekleme
$("#not-ekle").click(function () { MusteriGenel.MusteriNotEkleGetPartial(); });
$("#not-ekle-btn").live("click", function () { MusteriGenel.MusteriNotEkle(); });

//Silme
$(".delete-not").live("click", function () {
    var musteriKodu = $(this).attr("musteri-kodu");
    var siraNo = $(this).attr("sira-no");

    MusteriGenel.MusteriNotSil(musteriKodu, siraNo);
});



