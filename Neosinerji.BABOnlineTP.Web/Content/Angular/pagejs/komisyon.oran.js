var app = angular.module('komisyon.oran', ['xeditable', 'ui.multiselect', 'resourceModule']);
app.run(function (editableOptions) {
    editableOptions.theme = 'bs3';
});
app.controller('KomisyonOranCtrl', function ($scope, $http, resourceService, $filter) {
    $scope.resource = function (name) { return resourceService.get(name) };
    $scope.islem = [{ "adi": 'list', "deger": 0 }, { "adi": 'detection_rate', "deger": 1 }];
    $scope.kaynakSecim = [{ "adi": 'sub_agency', "deger": 0 }, { "adi": 'outsource', "deger": 1 }];
    $scope.selectedIslem = $scope.islem[0];
    $scope.selectedkaynakSecim = $scope.kaynakSecim[0];
    $scope.baslangicTarihi = new Date();
    $scope.selectedTVM = [];
    $scope.selectedDisKaynak = [];
    $scope.selectedTaliDisKaynak = '';
    $scope.selectedBrans = [];
    $scope.selectedSigortaSirketi = [];
    $scope.filtre = { oran: '0.01', baslangicTarihi: new Date().getFullYear() + '-01-01', gecerliYil: new Date().getFullYear() };
    $scope.kademeli = false;
    $scope.listemi = true;
    $scope.satisKanaliMi = true;
    $scope.tvmler = [];
    $scope.disKaynaklar = [];
    $scope.taliDisKaynaklar = [];
    $scope.branslar = [];
    $scope.sigortaSirketleri = [];
    $scope.yillar = [];
    $scope.kademeListesi = [{ 'Sira': 0, 'MinUretim': 0, 'MaxUretim': 0, 'Oran': 0, 'CikarShow': false }]
    $scope.fillTVM = function () {
        $http.post('/data/TVMListe').success(function (data) {
            angular.forEach(data, function (item) {
                $scope.tvmler.push(item)
            })
        });
    };

    $scope.fillDisKaynak = function () {
        $http.post('/data/DisKaynakListe').success(function (data) {
            angular.forEach(data, function (item) {
                $scope.disKaynaklar.push(item)
            })
        });
    };
    $scope.fillTaliDisKaynak = function () {
        $http.post('/data/DisKaynakListe').success(function (data) {
            angular.forEach(data, function (item) {
                $scope.taliDisKaynaklar.push(item)
            })
        });
    };
    $scope.fillBrans = function () {
        $http.post('/data/BransListe').success(function (data) {
            angular.forEach(data, function (item) {
                $scope.branslar.push(item)
            })
        });
    };
    $scope.fillSigortaSirketi = function () {
        $http.post('/data/SigortaSirketiListe').success(function (data) {
            angular.forEach(data, function (item) {
                $scope.sigortaSirketleri.push(item)
            })
        });
    };
    $scope.fillYear = function () {
        var min = 2015;
        var max = (new Date).getFullYear() + 1;
        for (var i = min; i <= max; i++) {
            $scope.yillar.push({ 'value': i, 'text': i });
        }
    }
    $scope.fillSelect = function () {
        $scope.fillTVM();
        $scope.fillDisKaynak();
        $scope.fillSigortaSirketi();
        $scope.fillTaliDisKaynak();
        $scope.fillBrans();
        $scope.fillYear();
    }
    $scope.fillSelect();
    $scope.komisyonList = [];
    $scope.komisyonKademeList = [];
    //[
    //{
    //    'TaliUnvani': 'Tali',
    //    'BransAdi': 'Brans',
    //    'SigortaSirketiAdi': 'SigortaSirketi',
    //    'GecerliYil': 2015,
    //    'KademeListesi': [
    //        {
    //            'Sira': 0,
    //            'MinUretim': 0,
    //            'MaxUretim': 99,
    //            'Oran': 45,
    //            'CikarShow': false
    //        },
    //        {
    //            'Sira': 1,
    //            'MinUretim': 99,
    //            'MaxUretim': 199,
    //            'Oran': 55,
    //            'CikarShow': true
    //        }
    //    ]
    //},
    //    {
    //        'TaliUnvani': 'Tali',
    //        'BransAdi': 'Brans',
    //        'SigortaSirketiAdi': 'SigortaSirketi',
    //        'GecerliYil': 2016,
    //        'KademeListesi': [
    //          {
    //              'Sira': 0,
    //              'MinUretim': 0,
    //              'MaxUretim': 99,
    //              'Oran': 45,
    //              'CikarShow': false
    //          },
    //        {
    //            'Sira': 1,
    //            'MinUretim': 99,
    //            'MaxUretim': 199,
    //            'Oran': 55,
    //            'CikarShow': true
    //        }
    //        ]
    //    }
    //];
    $scope.kademeGoster = function (data) {
        data.kademeShow = !data.kademeShow;
    }
    $scope.loading = 0;
    $scope.validation = {
        error: false,
        text: "",
        check: function () {
            this.text = "";
            this.error = false;

            if ($scope.satisKanaliMi) {
                if ($scope.selectedTVM.length == 0) {
                    this.error = true;
                    this.text += "\n Satış Kanalı Seçiniz";
                }
            }
            else {
                if ($scope.selectedDisKaynak.length == 0) {
                    this.error = true;
                    this.text += "\n Dış Kaynak Seçiniz";
                }
            }
            if ($scope.selectedBrans.length == 0) {
                this.error = true;
                this.text += "\n Branş Seçiniz.";
            }

            if ($scope.selectedBrans.length == 0) {
                this.error = true;
                this.text += "\n Branş Seçiniz.";
            }
            if ($scope.selectedSigortaSirketi.length == 0) {
                this.error = true;
                this.text += "\n Sigorta Şirketi Seçiniz.";
            }
            if (!$scope.listemi) {
                if ($scope.filtre.oran > 100) {
                    this.error = true;
                    this.text += "\n Geçerli komisyon oranı giriniz.";
                }
            }
        }
    };
    $scope.postForm = function () {
        $scope.validation.check();
        if ($scope.validation.error) {
            swal({
                title: 'Eksik Bilgi',
                text: $scope.validation.text,
                type: 'warning'
            })
        }
        else {
            $scope.loading = 1;
            if ($scope.selectedIslem.deger == 1) {
                swal({
                    title: $scope.resource('are_you_sure')
                    , text: $scope.resource('detection_rate_wtih_selected_fitler')
                    , type: "warning"
                    , showCancelButton: true
                    , confirmButtonColor: "#DD6B55"
                    , confirmButtonText: $scope.resource('commission_rate')
                    , cancelButtonText: $scope.resource('cancel')
                    , closeOnConfirm: false
                    , closeOnCancel: false
                }, function (isConfirm) {
                    if (isConfirm) { $scope.getList(); $scope.$apply(); swal($scope.resource('detectioning_rate'), $scope.resource('successful')); }
                    else { $scope.loading = 0; $scope.$apply(); swal($scope.resource('cancel'), $scope.resource('cancel_detection_rate'), "error"); }
                });

            }
            else { $scope.getList(); }
        }
    }
    $scope.getList = function () {
        var fModel = {
              'Islem': $scope.selectedIslem.deger
            , 'KaynakSecim': $scope.selectedkaynakSecim.deger
            , 'TVMliste': $scope.selectedTVM
            , 'DisKaynakList': $scope.selectedDisKaynak
            , 'taliDisKaynakKodu': $scope.selectedTaliDisKaynak!=null ? $scope.selectedTaliDisKaynak.Kodu: null
            , 'BransListe': $scope.selectedBrans
            , 'SigortaSirketiListe': $scope.selectedSigortaSirketi
            , 'BaslangicTarihi': $scope.filtre.baslangicTarihi
            , 'Oran': $scope.filtre.oran
            , 'GecerliYil': $scope.filtre.gecerliYil
            , 'KademeListe': $scope.kademeListesi
            , 'Kademeli': $scope.kademeli
            , 'Sayfa': $scope.currentPage
            , 'Adet': $scope.itemsPerPage
        };
        $http.post('/Oran/Liste', { filtreModel: fModel }).success(function (data) {
            $scope.loading = 0;
            $scope.komisyonList = [];
            $scope.komisyonKademeList = [];
            if (data.Hata)
                swal($scope.resource('error_occurred'));
            else {
                $scope.total = data.Toplam;
                if ($scope.currentPage >= $scope.pageCount())
                    $scope.currentPage = 0;
                $scope.setMinMax();
                angular.forEach(data.Liste, function (item) {
                    item.Guncelle = false;
                    if ($scope.kademeli)
                        $scope.komisyonKademeList.push(item)
                    else
                        $scope.komisyonList.push(item)
                });
            }
        });
        $scope.selectedIslem = $scope.islem[0];
        $scope.selectedkaynakSecim = $scope.kaynakSecim[0];
        
    }
    $scope.updateRow = function (data) {
        data.Guncelle = true;
    };
    $scope.postRow = function (data) {
        data.Guncelle = false;
        $http.post('/Oran/Guncelle', { 'oranModel': data }).success(function (nData) {
            if (nData.Hata)
                swal($scope.resource('error_occurred'));
            else
                swal($scope.resource('item_updated'));
            angular.extend(data, nData.Model);
        });
    };
    $scope.showYil = function () {
        var selected = $filter('filter')($scope.yillar, { value: $scope.filtre.gecerliYil });
        return ($scope.filtre.gecerliYil && selected.length) ? selected[0].text : 'Yıl Seçiniz';
    };
    $scope.showListeYil = function (item) {
        var selected = $filter('filter')($scope.yillar, { value: item.GecerliYil });
        return (item.GecerliYil && selected.length) ? selected[0].text : 'Yıl Seçiniz';
    };
    $scope.kademeEkle = function (data) {
        angular.forEach($scope.kademeListesi, function (item) {
            if (item.Sira > data.Sira) item.Sira += 1;
        })
        $scope.kademeListesi.push({ 'Sira': data.Sira + 1, 'MinUretim': data.MaxUretim, 'MaxUretim': data.MaxUretim, 'Oran': 0, 'CikarShow': true })
        $scope.kademeDuzenle();
    }
    $scope.kademeCikar = function (data) {
        $scope.kademeListesi.splice($scope.kademeListesi.indexOf(data), 1);
        angular.forEach($scope.kademeListesi, function (item) {
            if (item.Sira > data.Sira) item.Sira -= 1;
        })
        $scope.kademeDuzenle();
    }
    $scope.kademeDuzenle = function () {
        var total = $scope.kademeListesi.length;
        for (var i = 0; i < total; i++) {
            var item1, item2 = -1;
            for (var j = 0; j < total; j++) {
                if ($scope.kademeListesi[j].Sira == i - 1) item1 = j;
                if ($scope.kademeListesi[j].Sira == i) item2 = j;
            }
            if (item1 > -1) {
                $scope.kademeListesi[item2].MinUretim = $scope.kademeListesi[item1].MaxUretim;
                if ($scope.kademeListesi[item2].MaxUretim < $scope.kademeListesi[item2].MinUretim)
                    $scope.kademeListesi[item2].MaxUretim = $scope.kademeListesi[item2].MinUretim
            }
        }
    }
    $scope.tarihKontrol = function () {
        if ($scope.filtre.baslangicTarihi == '') $scope.filtre.baslangicTarihi = new Date().getFullYear() + '-01-01';
    }
    $scope.listeKademeEkle = function (liste, data) {
        angular.forEach(liste, function (item) {
            if (item.Sira > data.Sira) item.Sira += 1;
        })
        liste.push({ 'Sira': data.Sira + 1, 'MinUretim': data.MaxUretim, 'MaxUretim': data.MaxUretim, 'Oran': 0, 'CikarShow': true })
        $scope.listeKademeDuzenle(liste);
    }
    $scope.listeKademeCikar = function (liste, data) {
        liste.splice(liste.indexOf(data), 1);
        angular.forEach(liste, function (item) {
            if (item.Sira > data.Sira) item.Sira -= 1;
        })
        $scope.listeKademeDuzenle(liste);
    }
    $scope.listeKademeDuzenle = function (liste) {
        var total = liste.length;
        for (var i = 0; i < total; i++) {
            var item1, item2 = -1;
            for (var j = 0; j < total; j++) {
                if (liste[j].Sira == i - 1) item1 = j;
                if (liste[j].Sira == i) item2 = j;
            }
            if (item1 > -1) {
                liste[item2].MinUretim = liste[item1].MaxUretim;
                if (liste[item2].MaxUretim < liste[item2].MinUretim)
                    liste[item2].MaxUretim = liste[item2].MinUretim
            }
        }
    }

    $scope.itemsPerPage = 20;
    $scope.currentPage = 0;
    $scope.total = 0;
    $scope.range = function () {
        var rangeSize = 5;
        var ret = [];
        var start;

        start = $scope.currentPage;
        if (start > $scope.pageCount() - rangeSize) {
            start = $scope.pageCount() - rangeSize;
        }

        for (var i = start; i < start + rangeSize; i++) {
            if (i > -1)
                ret.push(i);
        }
        return ret;
    };


    $scope.prevPage = function () {
        if ($scope.currentPage > 0) {
            $scope.currentPage--;
        }
    };

    $scope.prevPageDisabled = function () {
        return $scope.currentPage === 0 ? "disabled" : "";
    };

    $scope.nextPage = function () {
        if ($scope.currentPage < $scope.pageCount() - 1) {
            $scope.currentPage++;
        }
    };

    $scope.nextPageDisabled = function () {
        return $scope.currentPage === $scope.pageCount() - 1 ? "disabled" : "";
    };

    $scope.pageCount = function () {
        return Math.ceil($scope.total / $scope.itemsPerPage);
    };

    $scope.setPage = function (n) {
        if (n > -1 && n < $scope.pageCount()) {
            $scope.currentPage = n;
        }
        else $scope.currentPage = 0;
    };

    $scope.$watch("currentPage", function (newValue, oldValue) {
        $scope.getList();
        $scope.setMinMax();
    });
    $scope.setMinMax = function () {
        $scope.minKayit = $scope.currentPage * $scope.itemsPerPage + 1;
        $scope.maxKayit = (($scope.currentPage == ($scope.pageCount() - 1)) || $scope.pageCount() == 1)
            ? $scope.total
            : ($scope.currentPage + 1) * $scope.itemsPerPage;
    }
});
