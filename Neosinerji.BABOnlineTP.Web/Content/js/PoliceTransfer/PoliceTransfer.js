var url_186 = "";
var policeKayit = new function () {
    return {

        policeKaydet: function () {

            if ($("#toplam-police").html() >= 1000) {
                if (confirm("Maximum yüklenebilecek poliçe sayısı 1000'dir. Poliçe transfer dosyanızı haftalık veya 10 günlük alarak hazırlamanız önerilir.")) {

                    $("#pol-kayit-btn").button("reset");
                    $("#police-progress").hide();
                    window.location.href = '/PoliceTransfer/PoliceTransfer/PoliceTransfer';
                }
            }
            else {
                if (confirm("Kaydetmeyi onaylıyor musunuz?")) {
                    $("#pol-kayit-btn").button("loading");
                    $.ajax({
                        timeout: 190000000,
                        method: "post",
                        url: "/PoliceTransfer/PoliceTransfer/PoliceKaydet",
                        data: $("#policeTransferForm").serialize(),
                        success: function (res) {
                            $("#police-progress").hide();
                            //if (res.BasariliKayit) {
                            $("#pol-kayit").modal('hide');
                            //if (confirm("Başarılı Kayıt Edilen Poliçe Sayısı: " + res.BasariliKayit + " Başarılı Güncellenen Poliçe Sayısı : " + res.BasarisizKayit))
                            //{
                            //    window.location.href = '@Url.Action("Liste")'; }
                            //}

                            $("#div-modal-polTransferSonuc").html(res);
                            $("#pol-transfer-sonuc").modal('show');

                            // alert("Başarılı Kayıt Edilen Poliçe Sayısı: " + res.BasariliKayit + " Daha önceden kayıt edilen Poliçe Sayısı : " + res.BasarisizKayit);
                            //}
                            //else { alert("Bir hata oluştu"); }
                            $("#pol-kayit-btn").button("reset");
                        },
                        error: function () { alert("Bir hata oluştu") }
                    });
                }
            }
        },

        otomatikPoliceKaydet: function () {

            if ($("#toplam-police").html() >= 1000) {
                if (confirm("Maximum yüklenebilecek poliçe sayısı 1000'dir. Poliçe transfer dosyanızı haftalık veya 10 günlük alarak hazırlamanız önerilir.")) {

                    $("#policeKaydet").button("reset");
                    $("#policeyukle-progress").hide();
                    window.location.href = '/PoliceTransfer/PoliceTransfer/PoliceTransfer';

                }
            }
            else {
                var seciliSirket = $("#AutoPoliceTransferSirketiKodu").val();
                var formData = new FormData(document.forms.namedItem('form-police-transfer'));
                $("#form-police-transfer").validate().form();
                if ($("#form-police-transfer").valid()) {
                    if (confirm("Kaydetmeyi onaylıyor musunuz?")) {
                        $("#policeyukle-progress").show();
                        $("#policeKaydet").button("loading");
                        $.ajax({
                            timeout: 190000000,
                            method: "post",
                            contentType: false,
                            processData: false,
                            url: "/PoliceTransfer/PoliceTransfer/OtomatikPoliceKaydet",
                            data: formData,
                            success: function (res) {
                                $("#policeyukle-progress").hide();
                                if (res.Success) {
                                    $("#pol-kayit").modal('hide');
                                    var TransferTipi = $("#TransferTipi").val();
                                    if (TransferTipi == "1") {
                                        showConfirmSweetAlert("İşlem Başarılı", res.Mesaj);
                                        $("#policeKaydet").button("reset");
                                    }
                                    else {
                                        if (res.updateKayit > 0) {
                                            showConfirmSweetAlert("İşlem Başarılı", "Poliçe Araç Bilgileri Güncellendi. Kayıt Sayısı: " + res.updateKayit);
                                        }
                                        else {
                                            showConfirmSweetAlert("İşlem Başarılı", "Başarılı Kayıt Edilen Poliçe Sayısı: " + res.BasariliKayit + "<br> Daha önceden kayıt edilen Poliçe Sayısı : " + res.BasarisizKayit + "<br> Hata sebebiyle eklenmeyen Kayit Sayısı: " + res.HataliEklenmeyenKayitlar + "<br> Toplam Okunan Poliçe Sayısı: " + res.toplamPoliceSayisi);
                                        }

                                        $("#policeKaydet").button("reset");
                                    }
                                }
                                else {
                                    $("#policeyukle-progress").hide();
                                    $("#policeKaydet").button("reset");
                                    swal("Hata!", res.Mesaj, "error");
                                }

                            },
                            error: function () { alert("Bir hata oluştu"); $("#policeKaydet").button("reset"); }
                        });
                    }
                }
            }
        },

        DosyaGonder: function () {
            $("#form-police-transfer").validate().form();

            if ($("#form-police-transfer").valid()) {

                $('#file').on('change', function () {
                    var byte = this.files[0].size;
                    var size = bytesToSize(byte);
                    alert(size);
                });

                var formData = new FormData(document.forms.namedItem('form-police-transfer'));

                $("#btn-kaydet").button("loading");

                var oReq = new XMLHttpRequest();
                oReq.open("POST", "/PoliceTransfer/PoliceTransfer/Upload", true);
                oReq.send(formData);
                oReq.timeout = 100000000;
                oReq.onload = function (oEvent) {
                    $("#policeyukle-progress").hide();
                    var check_file = false;
                    var mesaj_ = ""
                    try {
                        var objJson = JSON.parse(oEvent.target.response);
                         
                        check_file = objJson.Success186
                        mesaj_ = objJson.Mesaj;
                    } catch (e) {
                        url_186 = "";
                    }
                    if (check_file) {
                        Swal.fire({
                            title: mesaj_,
                            type: 'warning',
                            showCancelButton: true,
                            confirmButtonColor: '#36c6d3',
                            cancelButtonColor: '#d33',
                            confirmButtonText: 'Evet',
                            cancelButtonText: 'İptal'
                        }).then(function (result) {
                            if (result.value) {
                              
                                policeKayit.DosyaGonder();
                            } else if (result.dismiss == 'cancel') {
                            }
                        });
                    }
                    else if (oReq.status == 200 && oReq.responseText != "") {
                        $("#div-modal-helper").html(oReq.responseText);
                        $("#pol-kayit").modal('show');
                    }
                    else {
                        alert(oReq.responseText);
                    }
                    $("#btn-kaydet").button("reset");
                }
            }
        }

    }
}
