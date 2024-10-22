function telefonAlanKoduChange(element) {
    if (element === undefined) return;
    if (element.value.length == 3) {
        var numara = $(element).next(":input");
        numara.focus();
    }
}

////SAdece Rakam girilebilecek Alanlar için
$(".onlynumbers").live("keypress", function (e) {
    return ((e.which >= 48 && e.which <= 57) || e.which == 8 || e.which == 0);
});
//Sadece Harf Girilebilecek Alanlar için
$(".onlyalpha").live("keypress", function (e) {
    if (e.target.id == "GenelBilgiler_SoyadiUnvan") {
        return ((e.which >= 65 && e.which <= 90) || e.which == 8 || e.which == 46 || e.which == 0 || (e.which > 128 && e.which > 165)
                                                 || e.which == 32 || (e.which >= 97 && e.which <= 122));
    } else
        return ((e.which >= 65 && e.which <= 90) || e.which == 8 || e.which == 0 || (e.which > 128 && e.which > 165)
                                                 || e.which == 32 || (e.which >= 97 && e.which <= 122));
});

//Tarih için
$(".date-custom").datepicker({
    changeMonth: true,
    changeYear: true,
    dateFormat: 'dd.mm.yy',
    showOn: "button",
    buttonImage: "/Content/img/glyphicons_045_calendar.png",
    buttonImageOnly: true,
    yearRange: '-100:+20',
});




//türkce dil desteği
jQuery(function ($) {
    $.datepicker.regional['tr'] = {
        closeText: 'kapat',
        prevText: '&#x3c;geri',
        nextText: 'ileri&#x3e',
        currentText: 'bugün',
        monthNames: ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran',
        'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'],
        monthNamesShort: ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran',
        'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'],
        dayNames: ['Pazar', 'Pazartesi', 'Salı', 'Çarşamba', 'Perşembe', 'Cuma', 'Cumartesi'],
        dayNamesShort: ['Pz', 'Pt', 'Sa', 'Ça', 'Pe', 'Cu', 'Ct'],
        dayNamesMin: ['Pz', 'Pt', 'Sa', 'Ça', 'Pe', 'Cu', 'Ct'],
        weekHeader: 'Hf',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };

    $.datepicker.setDefaults($.datepicker.regional['tr']);
});

var okkontrol = function () {
    if ($("#page-container").hasClass('sidebar-closed')) {
        $("#btn-event").removeClass('sidebar-toggler sidebar-toggler-left hidden-phone').addClass('sidebar-toggler sidebar-toggler-right hidden-phone');
    }
    else {
        $("#btn-event").removeClass('sidebar-toggler sidebar-toggler-right hidden-phone').addClass('sidebar-toggler sidebar-toggler-left hidden-phone');
    }
}


//Sol tarafta menü active kontrolu yapıyor
$(document).ready(function () {
    $(".urun-menu").each(function () {
        if ($(this).hasClass("active")) {
            $(this).parent(".sub-menu").attr("style", "display:block");
            $(this).parent(".sub-menu").parent().addClass("open");
            $(this).parent(".sub-menu").prev().children("span").attr("class", "arrow open");
        }
    });
});

$("#btn-event").click(function () {
    okkontrol();
});


/*
    jQuery extended metodlar

    ajax metodları
    -----------------
    ulke
    tvmfinder
    userfinder
    tvmdepartmanlar
    aracmarka
    tescilil

    datatable metodlar
    ------------------
    getSelectedRows : $.fn.getSelectedRows(teminatEkleTable);
    getSelectedRowIds: var array = $.fn.getSelectedRowIds(teminatEkleTable);

    dom methodları
    ------------------
    defaultAction
*/
(function ($) {
    $.fn.extend({

        dropDownFill: function (data) {
            var ddl = $(this)
            ddl.empty();
            $(data).each(function () {
                $(document.createElement('option'))
                    .attr('value', this.Value)
                    .text(this.Text)
                    .appendTo(ddl);
            });
        },

        addToDropDown: function (value, text) {
            var ddl = $(this);
            $(document.createElement('option'))
                .attr('value', value)
                .text(text)
                .appendTo(ddl);
        },

        /*
            bootsrap switch kontrolünün serialization öncesi 
            on yerine true değerini servera göndermesi için
        */
        switchFix: function () {
            if ($(this).is(':checked'))
                $(this).val('true');
            else
                $(this).val('false');
        },


        /*
            Tüzel kişi icin faaliyet alanı bilgisini getiriyor..
            $('#AnaSektor').Sektor({ AltSektor: '#AltSektor' });
        */

        sektor: function (options) {
            var anasektor = $(this).val();

            $(this).change(function () {
                anasektor = $(this).val();

                if (anasektor === undefined) return;
                if (anasektor < 1000 || anasektor > 6000) {
                    $(options.altsektor).empty();
                    return;
                }

                $.getJSON('/Common/SektorleriGetir', { AnaSektor: anasektor }, function (result) {
                    $(options.altsektor).dropDownFill(result);
                });
            });
        },


        /*
            Ülke - İl - İlçe dropdown kontrollerinin server üzerinden otomatik veri alması için kullanılır
            $('#UlkeKodu').ulke({ il: '#IlKodu', ilce: '#IlceKodu' });
        */
        ulke: function (options) {
            var ulkeKodu = $(this).val();

            //ulke kodu değiştiğinde 
            $(this).change(function () {
                ulkeKodu = $(this).val();

                if (ulkeKodu === undefined) return;
                if (ulkeKodu == '0') {
                    $(options.il).empty();
                    $(options.ilce).empty();
                    return;
                }

                $.getJSON('/Common/IlleriGetir', { UlkeKodu: ulkeKodu },
                    function (result) {
                        $(options.il).dropDownFill(result);
                    });
            });

            $(options.il).change(function () {
                var ilKodu = $(this).val();

                if (ilKodu === undefined) return;
                if (ilKodu == '0') {
                    $(options.ilce).empty();
                    return;
                }

                $.getJSON('/Common/IlceleriGetir', { UlkeKodu: ulkeKodu, IlKodu: ilKodu },
                    function (result) {
                        $(options.ilce).dropDownFill(result);
                    });
            });
        },

        iller: function (ulkeKodu) {
            var ilSelector = $(this);
            $.getJSON('/Common/IlleriGetir', { UlkeKodu: ulkeKodu },
                function (result) {
                    ilSelector.dropDownFill(result);
                });
        },

        ilceler: function (ulkeKodu, ilKodu) {
            var ilceSelector = $(this);
            $.getJSON('/Common/IlceleriGetir', { UlkeKodu: ulkeKodu, IlKodu: ilKodu },
                function (result) {
                    ilceSelector.dropDownFill(result);
                });
        },

        /*
            Kurum-Sube dropdown kontrollerinin server üzerinden otomatik veri alması için kullanılır
            $('#KurumKodu').KurumSube({ SubeKodu: '#SubeKodu' });
        */
        KurumSube: function (options) {
            var kurumKodu = $(this).val();

            //kurum kodu değiştiğinde 
            $(this).change(function () {
                kurumKodu = $(this).val();

                if (kurumKodu === undefined) return;
                if (kurumKodu == '0') {
                    $(options.Sube).empty();
                    return;
                }

                $.getJSON('/Teklif/Dask/GetListSube', { KurumKodu: kurumKodu },
                    function (result) {
                        $(options.Sube).dropDownFill(result);
                    });
            });
        },


        /*
            İl - İlçe -Belde dropdown kontrollerinin server üzerinden otomatik veri alması için kullanılır
            $('#Ilkodu').IlIlceBelde({ Ilce: '#IlceKodu', Belde: '#BeldeKodu' ,Mahalle: '#Mahalle' ,CaddeSokakBulvar:'#CaddeSokakBulvar'});
        */
        UAVTSorgu: function (options) {
            var ilKodu = $(this).val();

            //Il Kodu Değiştiğinde
            $(this).change(function () {
                ilKodu = $(this).val();

                if (ilKodu === undefined) return;
                if (ilKodu == "0" || ilKodu == "") {
                    $(options.Ilce).empty().trigger("liszt:updated");
                    $(options.Belde).empty().trigger("liszt:updated");
                    $(options.Mahalle).empty().trigger("liszt:updated");
                    $(options.CaddeSokakBulvar).empty().trigger("liszt:updated");
                    $(options.Binalar).empty().trigger("liszt:updated");
                    $(options.Daireler).empty().trigger("liszt:updated");
                    return;
                }


                $.getJSON("/Teklif/Dask/GetListIlce", { IlKodu: ilKodu }, function (result) {
                    $(options.Ilce).dropDownFill(result);
                    $(options.Ilce).trigger("liszt:updated");
                    $(options.Belde).empty().trigger("liszt:updated");
                    $(options.Mahalle).empty().trigger("liszt:updated");
                    $(options.CaddeSokakBulvar).empty().trigger("liszt:updated");
                    $(options.Binalar).empty().trigger("liszt:updated");
                    $(options.Daireler).empty().trigger("liszt:updated");
                });
            });

            //Ilce Kodu Değiştiğinde
            $(options.Ilce).change(function () {
                var IlceKodu = $(this).val();
                if (IlceKodu === undefined) return;
                if (IlceKodu == "0" || IlceKodu == "") {
                    $(options.Belde).empty().trigger("liszt:updated");
                    $(options.Mahalle).empty().trigger("liszt:updated");
                    $(options.CaddeSokakBulvar).empty().trigger("liszt:updated");
                    $(options.Binalar).empty().trigger("liszt:updated");
                    $(options.Daireler).empty().trigger("liszt:updated");
                    return;
                }

                $.getJSON("/Teklif/Dask/GetListBelde", { IlceKodu: IlceKodu }, function (result) {
                    $(options.Belde).dropDownFill(result);
                    $(options.Belde).trigger("liszt:updated");
                    $(options.Mahalle).empty().trigger("liszt:updated");
                    $(options.CaddeSokakBulvar).empty().trigger("liszt:updated");
                    $(options.Binalar).empty().trigger("liszt:updated");
                    $(options.Daireler).empty().trigger("liszt:updated");
                });
            });

            //Belde Kodu Değiştiğinde
            $(options.Belde).change(function () {
                var BeldeKodu = $(this).val();
                if (BeldeKodu === undefined) return;
                if (BeldeKodu == "0" || BeldeKodu == "") {
                    $(options.Mahalle).empty().trigger("liszt:updated");
                    $(options.CaddeSokakBulvar).empty().trigger("liszt:updated");
                    $(options.Binalar).empty().trigger("liszt:updated");
                    $(options.Daireler).empty().trigger("liszt:updated");
                    return;
                }

                $.getJSON("/Teklif/Dask/GetListMahalle", { BeldeKodu: BeldeKodu }, function (result) {
                    $(options.Mahalle).dropDownFill(result);
                    $(options.Mahalle).trigger("liszt:updated");
                    $(options.CaddeSokakBulvar).empty().trigger("liszt:updated");
                    $(options.Binalar).empty().trigger("liszt:updated");
                    $(options.Daireler).empty().trigger("liszt:updated");
                });
            });

            //Mahalle Kodu Değiştiğinde
            $(options.Mahalle).change(function () {
                var MahalleKodu = $(this).val();
                if (MahalleKodu === undefined) return;
                if (MahalleKodu == "0" || MahalleKodu == "") {
                    $(options.CaddeSokakBulvar).empty().trigger("liszt:updated");
                    $(options.Binalar).empty().trigger("liszt:updated");
                    $(options.Daireler).empty().trigger("liszt:updated");
                    return;
                }

                $.getJSON("/Teklif/Dask/GetListCadSkBulMeydan", { MahalleKodu: MahalleKodu, Aciklama: "" }, function (result) {
                    $(options.CaddeSokakBulvar).dropDownFill(result);
                    $(options.CaddeSokakBulvar).trigger("liszt:updated");
                    $(options.Binalar).empty().trigger("liszt:updated");
                    $(options.Daireler).empty().trigger("liszt:updated");
                });
            });

            //Cadde Sk Bulvar Kodu Değiştiğinde
            $(options.CaddeSokakBulvar).change(function () {
                var CaddeSokakBulvarKodu = $(this).val();
                if (CaddeSokakBulvarKodu === undefined) return;
                if (CaddeSokakBulvarKodu == "0" || CaddeSokakBulvarKodu == "") {
                    $(options.Binalar).empty().trigger("liszt:updated");
                    $(options.Daireler).empty().trigger("liszt:updated");
                    return;
                }

                $.getJSON("/Teklif/Dask/GetListCadSkBulMeydan_BinaAd", { CadSkBulMeyKodu: CaddeSokakBulvarKodu, Aciklama: "" }, function (result) {
                    $(options.Binalar).dropDownFill(result);
                    $(options.Binalar).trigger("liszt:updated");
                    $(options.Daireler).empty().trigger("liszt:updated");
                });
            });

            //Bina Kodu Değiştiğinde
            $(options.Binalar).change(function () {
                var BinaNo = $(this).val();
                if (BinaNo === undefined) return;
                if (BinaNo == "0" || BinaNo == "") {
                    $(options.Daireler).empty().trigger("liszt:updated");
                    return;
                }

                $.getJSON("/Teklif/Dask/GetListDaireler", { BinaNo: BinaNo }, function (result) {
                    $(options.Daireler).dropDownFill(result);
                    $(options.Daireler).trigger("liszt:updated");
                });
            });
        },



        /*
           Araç marka - model dropdown kontrollerinin server üzerinden otomatik veri alması için kullanılır
           $('#MarkaKodu').marka({ model: '#TipKodu' });
       */
        marka: function (options) {
            var markaKodu = $(this).val();

            //marka kodu değiştiğinde 
            $(this).change(function () {
                markaKodu = $(this).val();

                if (markaKodu === undefined) return;
                if (markaKodu == '0') {
                    $(options.model).empty();
                    return;
                }

                $.getJSON('/Common/ModelleriGetir', { MarkaKodu: markaKodu },
                    function (result) {
                        $(options.model).dropDownFill(result);
                    });
            });
        },

        /*
            oTable dataTable nesnesindeki seçili olan satırları alır.
            var selectedRows = $.fn.getSelectedRows(teminatEkleTable);
        */
        getSelectedRows: function (oTable) {
            var aReturn = new Array();
            var aTrs = oTable.fnGetNodes();

            for (var i = 0 ; i < aTrs.length ; i++) {
                if ($(aTrs[i]).hasClass('active')) {
                    aReturn.push(aTrs[i]);
                }
            }
            return aReturn;
        },

        /*
            oTable dataTable nesnesindeki seçili olan satırların id değerlerini alır.
            var array = $.fn.getSelectedRowIds(teminatEkleTable);
        */
        getSelectedRowIds: function (oTable) {
            var selectedRows = $.fn.getSelectedRows(oTable);
            var array = new Array();

            for (var i = 0 ; i < selectedRows.length ; i++) {
                var data = oTable.fnGetData(selectedRows[i]);

                if (data !== undefined && data.DT_RowId !== undefined) {
                    array.push(data.DT_RowId);
                }
            }

            return array;
        },

        /*
            tvm bulma alanını göstermek için kullanılır
            tvm seçildiğinde callback fonksiyonu çağrılır.
            callback fonksiyonu seçilen tvm'nin kodu ve unvanini bulundurur. 
            $("#TVMKodu").tvmfinder(function (tvm) {
                var kodu = tvm.kodu;
                var unvani = tvm.unvani;
            });
        */
        tvmfinder: function (callback) {
            var oTVMTable;
            var tvmKoduInput = $(this);
            var tvmUnvaniInput = $("#" + $(this).attr("id") + "_text");

            if ($(tvmUnvaniInput).length == 0)
                tvmUnvaniInput = $("#" + $(this).attr("name") + "_text");

            //alanın sağında bulunan arama butonuna basıldığında çalışır.
            $("#tvm-select-btn").click(function () {
                $("#tvm-select-btn").attr("disabled", "disabled");
                $('.field-validation-error').empty();

                $.post("/Manage/TVM/TVMListe", "", function (data) {
                    $("#tmv-list-container").hide();
                    $("#tvm-select-cancel-btn").show("fast");
                    $("#tmv-list-container").html(data);
                    oTVMTable = $('#tvm-table').dataTable({
                        "bPaginate": true,
                        "bLengthChange": false,
                        "iDisplayLength": 5,
                        "bFilter": true,
                        "bSort": true,
                        "bInfo": true,
                        "bProcessing": false,
                    });
                    $("#tmv-list-container").show('fast');
                });
            });

            //liste ekrana geldikten sonra listeyi kapatmak için kulllanılan buton
            $("#tvm-select-cancel-btn").click(function () {
                $("#tmv-list-container").hide('fast');
                $("#tvm-select-btn").removeAttr("disabled");
                oTVMTable.fnDestroy();
                $("#tmv-list-container").html("");
                $("#tvm-select-cancel-btn").hide("fast");
            });

            //listeden tvm seçildiğinde çalışır.
            $(".tvm-sec-btn").live("click", function () {
                var kodu = $(this).attr("tvm-kodu");
                var unvani = $(this).attr("tvm-unvani");

                $(tvmKoduInput).val(kodu);
                $(tvmUnvaniInput).val(unvani);

                $("#tmv-list-container").hide('fast');
                $("#tvm-select-btn").removeAttr("disabled");
                oTVMTable.fnDestroy();
                $("#tmv-list-container").html("");
                $("#tvm-select-cancel-btn").hide("fast");

                if (callback !== undefined) {
                    callback({ kodu: kodu, unvani: unvani });
                }
            });


            return {
                disable: function () {
                    $("#tvm-select-btn").hide();
                },
                enable: function () {
                    $("#tvm-select-btn").show();
                }
            }
        },

        /*
            kullanıcı arama tablosunu göstermek için kullanılır
            kullanıcı seçildiğinde callback fonksiyonu çağrılır.
            callback fonksiyonu seçilen kullanıcının kodu ve adini bulundurur. 
            $("#YoneticiKodu").tvmfinder("TVMKodu", function (user) {
                var kodu = user.kodu;
                var adi = user.adi;
            });
        */
        userfinder: function (tvmkodu, callback) {
            var oUSERTable;
            var kullaniciKoduInput = $(this);
            var kullaniciAdiInput = $("#" + $(this).attr("id") + "_text");
            var tvmKoduInput = $("#" + tvmkodu);

            //alanın sağında bulunan arama butonuna basıldığında çalışır.
            $("#user-select-btn").click(function () {
                var tvmKodu = $(tvmKoduInput).val();
                $("#user-select-btn").attr("disabled", "disabled");
                $('.field-validation-error').empty();

                $.post("/Manage/Kullanici/KullaniciListe",
                    { tvmKodu: tvmKodu },
                    function (data) {
                        $("#user-list-container").hide();
                        $("#user-select-cancel-btn").show("fast");
                        $("#user-list-container").html(data);
                        oUSERTable = $('#user-table').dataTable({
                            "bPaginate": true,
                            "bLengthChange": false,
                            "iDisplayLength": 5,
                            "bFilter": false,
                            "bSort": true,
                            "bInfo": false,
                            "bProcessing": false,
                        });
                        $("#user-list-container").show('fast');
                    });
            });

            //liste ekrana geldikten sonra listeyi kapatmak için kulllanılan buton
            $("#user-select-cancel-btn").click(function () {
                $("#user-list-container").hide('fast');
                $("#user-select-btn").removeAttr("disabled");
                oUSERTable.fnDestroy();
                $("#user-list-container").html("");
                $("#user-select-cancel-btn").hide("fast");
            });

            //listeden tvm seçildiğinde çalışır.
            $(".user-sec-btn").live("click", function () {
                var kodu = $(this).attr("kullanici-kodu");
                var adi = $(this).attr("kullanici-adi");

                $(kullaniciKoduInput).val(kodu);
                $(kullaniciAdiInput).val(adi);

                $("#user-list-container").hide('fast');
                $("#user-select-btn").removeAttr("disabled");
                oUSERTable.fnDestroy();
                $("#user-list-container").html("");
                $("#user-select-cancel-btn").hide("fast");

                if (callback !== undefined) {
                    callback({ kodu: kodu, adi: adi });
                }
            });

            return {
                disable: function () {
                    $("#user-select-btn").hide();
                },
                enable: function () {
                    $("#user-select-btn").show();
                }
            }
        },

        /*
           musteri arama tablosunu göstermek için kullanılır
           musteri seçildiğinde callback fonksiyonu çağrılır.
           callback fonksiyonu seçilen musterinin kodu ve adini bulundurur. 
           $("#MusteriKodu").tvmfinder("TVMKodu", function (customer) {
               var kodu = customer.kodu;
               var adi = customer.adi;
           });
       */
        customerfinder: function (tvmkodu, callback) {
            var oUSERTable;
            var MusteriKoduInput = $(this);
            var MusteriAdiInput = $("#" + $(this).attr("id") + "_text");
            var tvmKoduInput = $("#" + tvmkodu);

            //alanın sağında bulunan arama butonuna basıldığında çalışır.
            $("#user-select-btn").click(function () {
                var tvmKodu = $(tvmKoduInput).val();
                $("#user-select-btn").attr("disabled", "disabled");
                $('.field-validation-error').empty();

                $.post("/Musteri/Musteri/GetMusteriByTVMKodu",
                    { tvmKodu: tvmKodu },
                    function (data) {
                        if (data != "" && data != "null") {
                            $("#user-list-container").hide();
                            $("#user-select-cancel-btn").show("fast");
                            $("#user-list-container").html(data);
                            oUSERTable = $('#user-table').dataTable({
                                "bPaginate": true,
                                "bLengthChange": false,
                                "iDisplayLength": 5,
                                "bFilter": true,
                                "bSort": true,
                                "bInfo": false,
                                "bProcessing": true,
                            });
                            $("#user-list-container").show('fast');
                        }
                    });
            });

            //liste ekrana geldikten sonra listeyi kapatmak için kulllanılan buton
            $("#user-select-cancel-btn").click(function () {
                $("#user-list-container").hide('fast');
                $("#user-select-btn").removeAttr("disabled");
                oUSERTable.fnDestroy();
                $("#user-list-container").html("");
                $("#user-select-cancel-btn").hide("fast");
            });

            //listeden tvm seçildiğinde çalışır.
            $(".user-sec-btn").live("click", function () {
                var kodu = $(this).attr("musteri-kodu");
                var adi = $(this).attr("musteri-adi");

                $(MusteriKoduInput).val(kodu);
                $(MusteriAdiInput).val(adi);

                $("#user-list-container").hide('fast');
                $("#user-select-btn").removeAttr("disabled");
                oUSERTable.fnDestroy();
                $("#user-list-container").html("");
                $("#user-select-cancel-btn").hide("fast");

                if (callback !== undefined) {
                    callback({ kodu: kodu, adi: adi });
                }
            });

            return {
                disable: function () {
                    $("#user-select-btn").hide();
                },
                enable: function () {
                    $("#user-select-btn").show();
                }
            }
        },

        /*
            verilem tvm koduna uygun departman listesini yüklemek için
            $("#DeparmanKodu").tvmdepartmanlar(1)
        */
        tvmdepartmanlar: function (tvmKodu) {
            var ddl = $(this);
            $.getJSON('/Manage/TVM/TVMDepartmanlar', { tvmKodu: tvmKodu },
                function (result) {
                    ddl.dropDownFill(result);
                });
        },


        /*
            verilem tvm koduna uygun Yetkiler listesini yüklemek için
            $("#YetkiKodu").tvmyetkiler(1)
        */
        tvmyetkiler: function (tvmKodu) {
            var ddl = $(this);
            $.getJSON('/Manage/TVM/TVMYetkiler', { tvmKodu: tvmKodu },
                function (result) {
                    ddl.dropDownFill(result);
                });
        },

        /*
            Araç Kullanım Şekli - Marka - Araç Tipi dropdown kontrollerinin server üzerinden otomatik 
            veri alması için kullanılır
            $('#KullanimSekliKodu').aracmarka({ tarz: '#TarzKodu', marka: '#MarkaKodu', model: '#Model', tip: '#TipKodu' });
        */
        aracmarka: function (options) {
            var kullanimSekliKodu = $(this).val();
            var kullanimTarziKodu;

            //Kullanım şekli değiştiğinde
            $(this).change(function () {
                kullanimSekliKodu = $(this).val();

                if (kullanimSekliKodu === undefined) return;
                if (kullanimSekliKodu == '') {
                    $(options.tarz).empty();
                    $(options.marka).empty();
                    $(options.model).empty();
                    $(options.tip).empty();
                    return;
                }

                $.getJSON('/Common/AracTarzGetir', { KullanimSekliKodu: kullanimSekliKodu },
                    function (result) {
                        $(options.tarz).dropDownFill(result);
                    });
            }); //$(this).change

            //Kullanım tarzı değiştiğinde
            $(options.tarz).change(function () {
                kullanimTarziKodu = $(this).val();

                if (kullanimTarziKodu === undefined) return;
                if (kullanimTarziKodu == '') {
                    $(options.marka).empty();
                    $(options.model).empty();
                    $(options.tip).empty();
                    return;
                }

                $.getJSON('/Common/AracMarkaGetir', { KullanimTarziKodu: kullanimTarziKodu },
                    function (result) {
                        $(options.marka).dropDownFill(result);
                    });
            }); //$(this).change

            //Marka veya model değiştiğinde
            $(options.marka + ", " + options.model).change(function () {
                var kullanimTarziKodu = $(options.tarz).val();
                var markaKodu = $(options.marka).val();
                var model = $(options.model).val();

                if (markaKodu === undefined) return;
                if (model === undefined) return;
                if (markaKodu == '' || model == '') {
                    $(options.tip).empty();
                    return;
                }

                $.getJSON('/Common/AracTipiGetir', {
                    KullanimTarziKodu: kullanimTarziKodu,
                    MarkaKodu: markaKodu,
                    Model: model
                },
                    function (result) {
                        $(options.tip).dropDownFill(result);
                    });
            });
        },

        /*
            Tescil il dropdown'ı değiştirildiğinde tescil ilçe dropdown kontrolünün yüklenmesi için kullanılır.
            $("#TescilIl").tescilil({ ilce : "#TescilIlce"})
        */
        tescilil: function (options) {
            $(this).change(function () {
                var ilKodu = $(this).val();

                if (ilKodu === undefined) return;
                if (ilKodu == '0') {
                    $(options.ilce).empty();
                    return;
                }

                $.getJSON('/Common/TescilIlceleriGetir', { IlKodu: ilKodu },
                    function (result) {
                        $(options.ilce).dropDownFill(result);
                    });
            });
        },

        /*
            kullanılan buton'un sayfada default olarak çalışması için kullanılır
            $("#search").defaultAction();
        */
        defaultAction: function () {
            var controlId = $(this).attr('id');

            $("input").bind("keydown", function (event) {
                var keycode = (event.keyCode ? event.keyCode : (event.which ? event.which : event.charCode));
                if (keycode == 13) {
                    var btn = document.getElementById(controlId);
                    if (btn != null) {
                        btn.click();
                    }
                    return false;
                }
                return true;
            });
        },

        /*
            Ekranda bekleme mesajını gösterir
            Bütün sayfayı bloklar : $(window).pleaseWait();
            Kullanım alanını bloklar : $("#page-body-container").pleaseWait();
        */
        pleaseWait: function () {
            var message = "<div style='height:40px;'><div class='loading9 pull-left'></div>" +
                          "<div><h4 style='padding-top:6px;'>Lütfen bekleyiniz...</h4></div></div>";

            var opts = { message: message, css: { border: 'solid 1px black' } };
            if ($(this).length == 0) {
                $.blockUI(opts);
            }
            else {
                $(this).block(opts);
            }
        },

        krediKarti: function () {
            $(".credit-card").keyup(function (event) {
                var key = event.which;

                if (key !== 0) {
                    var c = String.fromCharCode(key);
                    if (c.match("[0-9]")) {
                        if (this.value.length == 4)
                            $(this).next("input[type=text]").focus();

                        if (this.value.length == 0)
                            $(this).prev("input[type=text]").focus();
                    }
                }
            });
        }
    });
})(jQuery);