var app = angular.module('komisyon.hesaplama', ['xeditable', 'ui.multiselect', 'resourceModule']);
app.run(function (editableOptions) {
    editableOptions.theme = 'bs3';
});
app.controller('KomisyonHesaplamaCtrl', function ($scope, $filter, $http, resourceService) {
    $scope.resource = function (name) { return resourceService.get(name) };
    var d0 = new Date(); var d1 = new Date(); d1.setDate(d0.getDate() - 15);
    $scope.filtre = {
        PoliceDurumu: 0,
        IptalZeylTahakkuk: 0,
        TarihBaslangic: d1.getFullYear() + '-' + ('0' + (d1.getMonth(1)+1)).slice(-2) + '-' + ('0' + d1.getDate()).slice(-2),
        TarihBitis: d0.getFullYear() + '-' + ('0' + (d0.getMonth()+1)).slice(-2) + '-' + ('0' + d0.getDate()).slice(-2),
        TVMListe: [],
        BransListe: [],
        SigortaSirketiListe: []
    };
    $scope.policeListe = [];
    $scope.loading = 0;
    $scope.postForm = function () {
        $scope.loading = 1;
        $scope.getList();
    }
    $scope.getList = function () {
        $scope.checkboxes = { 'checked': false, items: {} };
        $http.post('/Hesaplama/Liste', { filtre: $scope.filtre }).success(function (data) {
            $scope.loading = 0;
            $scope.policeListe = [];
            if (data.Hata)
                swal($scope.resource('error_occurred'));
            else {
                angular.forEach(data.Liste, function (item) {
                    $scope.policeListe.push(item)
                });
            }
        });
    }
    $scope.tvmler = [];
    $scope.disKaynaklar = [];
    $scope.branslar = [];
    $scope.sigortaSirketleri = [];
    $scope.fillTVM = function () {
        $scope.tvmler.push({ Kodu: '0', Unvani: '' });
        $http.post('/data/TVMListe').success(function (data) {
            angular.forEach(data, function (item) {
                $scope.tvmler.push(item)
            })
        });
    };
    $scope.fillDisKaynak = function () {
        $scope.disKaynaklar.push({ Kodu: '0', Unvani: '' });
        $http.post('/data/DisKaynakListe').success(function (data) {
            angular.forEach(data, function (item) {
                $scope.disKaynaklar.push(item)
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
    $scope.fillSelect = function () {
        $scope.fillTVM();
        $scope.fillDisKaynak();
        $scope.fillSigortaSirketi();
        $scope.fillBrans();
    }
    $scope.fillSelect();
    //$scope.policeListe = [
    //    {
    //        Id: 1, Tali: { Kodu: 1011 }, SigortaSirketi: { Adi: 'SigortaSirketiAdi1' }, Brans: { Adi: 'BransAdi' }, Prim: 500
    //        , AlinanKomisyon: 200
    //        , PoliceBaslangicTarihi: '201501011'
    //        , PoliceNo: '001'
    //        , TaliOran: 0
    //        , TaliKomisyon: 0
    //    }
    //    , {
    //        Id: 2, Tali: { Kodu: 10111 }, SigortaSirketi: { Adi: 'SigortaSirketiAdi2' }, Brans: { Adi: 'BransAdi' }, Prim: 500
    //        , AlinanKomisyon: 200
    //        , PoliceBaslangicTarihi: '20150101'
    //        , PoliceNo: '001'
    //        , TaliOran: 0
    //        , TaliKomisyon: 0
    //    }
    //    , {
    //        Id: 3, Tali: { Kodu: 10102 }, SigortaSirketi: { Adi: 'SigortaSirketiAdi3' }, Brans: { Adi: 'BransAdi' }, Prim: 500
    //        , AlinanKomisyon: 200
    //        , PoliceBaslangicTarihi: '20150101'
    //        , PoliceNo: '001'
    //        , TaliOran: 0
    //        , TaliKomisyon: 0
    //    }
    //    , {
    //        Id: 4, Tali: { Kodu: 1111 }, SigortaSirketi: { Adi: 'SigortaSirketiAdi4' }, Brans: { Adi: 'BransAdi' }, Prim: 500
    //        , AlinanKomisyon: 200
    //        , PoliceBaslangicTarihi: '20150101'
    //        , PoliceNo: '001'
    //        , TaliOran: 0
    //        , TaliKomisyon: 0
    //    }
    //    , {
    //        Id: 5, Tali: { Kodu: -1 }, SigortaSirketi: { Adi: 'SigortaSirketiAdi5' }, Brans: { Adi: 'BransAdi' }, Prim: 500
    //        , AlinanKomisyon: 200
    //        , PoliceBaslangicTarihi: '20150101'
    //        , PoliceNo: '001'
    //        , TaliOran: 0
    //        , TaliKomisyon: 0
    //    }
    //];
    $scope.checkboxes = { 'checked': false, items: {} };
    $scope.$watch('checkboxes.checked', function (value) {
        angular.forEach($scope.policeListe, function (item) {
            if (angular.isDefined(item.Id)) {
                $scope.checkboxes.items[item.Id] = value;
            }
        });
    });
    $scope.$watch('checkboxes.items', function (values) {
        if (!$scope.policeListe) { return; }
        var checked = 0, unchecked = 0, total = $scope.policeListe.length;
        angular.forEach($scope.policeListe, function (item) {
            checked += ($scope.checkboxes.items[item.Id]) || 0;
            unchecked += (!$scope.checkboxes.items[item.Id]) || 0;
        });
        if ((unchecked == 0) || (checked == 0)) {
            $scope.checkboxes.checked = (checked == total);
        }
        angular.element(document.getElementById("select_all")).prop("indeterminate", (checked != 0 && unchecked != 0));
    }, true);
    $scope.updateRow = function (item, type) {
        if ($scope.checkboxes.items[item.Id]) {
            angular.forEach($scope.checkboxes.items, function (value, key) {
                if (value) {
                    $scope.setSelected(item.Id, key, type)
                }
            });
        }
        else { $scope.setSelected(item.Id, item.Id, type) }
        $scope.checkboxes.items[item.Id] = true;
    }
    $scope.setSelected = function (_pId, pId, type) {
        var changed = $filter('filter')($scope.policeListe, { Id: _pId })[0];
        var selected = $filter('filter')($scope.policeListe, { Id: pId })[0];
        var tali = changed.Tali;
        var oran = changed.TaliKomisyonOrani;
        var komisyon = changed.TaliKomisyon;
        var polNo = selected.PoliceNo;
        var SigortaSirketi = selected.SigortaSirketi;
        selected.Guncelle = true;
        switch (type) {
            case 'T':
                if (!tali) oran = 0;
                else {
                    $http.post('/Hesaplama/Oran',
                        {
                            Tali: changed.Tali,
                            SigortaSirketi: SigortaSirketi,
                            Brans: selected.Brans,
                            Tarih: selected.PoliceTanzimTarihi,
                            policeNo:polNo
                        })
                        .success(function (data) {
                            selected.Tali = tali;
                            selected.TaliKomisyonOrani = data.Oran;
                            selected.TaliKomisyon = $scope.setDecimal(selected.AlinanKomisyon * selected.TaliKomisyonOrani / 100, 2);
                        })
                }
               
                break;
            case 'O':
                selected.TaliKomisyonOrani = oran;
                selected.TaliKomisyon = $scope.setDecimal(selected.AlinanKomisyon * selected.TaliKomisyonOrani / 100, 2);
                break;
            case 'K':
                selected.TaliKomisyon = komisyon;
                selected.TaliKomisyonOrani = $scope.setDecimal(selected.TaliKomisyon / selected.AlinanKomisyon * 100, 0);
                break;
            default:
        }
    }
    $scope.showTali = function (item) {
        var selected = $filter('filter')($scope.tvmler, { Kodu: item.Tali.Kodu }, true);
        return (item.Tali && selected.length) ? selected[0].Unvani : $scope.resource('select');
    };
   
    $scope.setDecimal = function (input, places) {
        if (isNaN(input)) return input;
        var factor = "1" + Array(+(places > 0 && places + 1)).join("0");
        return Math.round(input * factor) / factor;
    };
    $scope.selectedRows;
    $scope.postRow = function (item) {
        $scope.selectedRows = item;
        var checked = 0;
        angular.forEach($scope.policeListe, function (item) {
            checked += ($scope.checkboxes.items[item.Id]) || 0;
        });
        if (checked > 1) {
            $scope.showMultiorSingleConfirm();
        }
        else {
            $scope.showSingleConfirm();
        }
    }
    $scope.showMultiorSingleConfirm = function () {
        swal({
            title: "Tüm Kayıtları Güncellensin mi?"
            , text: "Listede Seçili Tüm Kayıtları Güncellemek İstiyor musunuz?"
            , type: "warning"
            , showCancelButton: true
            , confirmButtonColor: "#DD6B55"
            , confirmButtonText: "Seçili Tüm Kayıtları Güncelle"
            , cancelButtonText: "Seçili Kaydı Güncelle"
            , closeOnConfirm: false
            , closeOnCancel: false
        }, function (isConfirm) {
            if (isConfirm) {
                $scope.showMultiConfirm();
            }
            else {
                $scope.showSingleConfirm();
            }
        }
        );
    }
    $scope.showSingleConfirm = function () {
        swal({
            title: "Seçili Kaydı Güncellemek Üzerisiniz."
          , text: "Seçili Kayıt Güncellenecektir."
          , type: "warning"
          , showCancelButton: true
          , confirmButtonColor: "#DD6B55"
          , confirmButtonText: "Güncelle"
          , cancelButtonText: "İptal"
          , closeOnConfirm: false
          , closeOnCancel: false
        }, function (isConfirm) {
            if (isConfirm) {
                $scope.updateSelectedRow($scope.selectedRows);
                $scope.showConfirmMessage();
            }
            else { $scope.showCancelMessage(); }
        }
      );
    }
    $scope.showMultiConfirm = function () {
        swal({
            title: "Tüm Kayıtları Güncellemek Üzerisiniz."
            , text: "Listedeki Tüm Kayıtlar Güncellenecektir"
            , type: "warning"
            , showCancelButton: true
            , confirmButtonColor: "#DD6B55"
            , confirmButtonText: "Güncelle"
            , cancelButtonText: "İptal"
            , closeOnConfirm: false
            , closeOnCancel: false
        }, function (isConfirm) {
            if (isConfirm) {
                $scope.updateAllSelected();
                $scope.showConfirmMessage();
            }
            else {
                $scope.showCancelMessage();
            }
        });
    }
    $scope.showConfirmMessage = function () {
        swal("Güncelleştirildi", "Günleme İşleme Tamamlandı", "success");
    }
    $scope.showCancelMessage = function () {
        swal("İptal", "Güncelleme İşlemi İptal Edildi", "error");
    }
    $scope.updateSelectedRow = function (item) {
        item.Guncelle = false;
        $scope.checkboxes.items[item.Id] = false;
        $http.post('/Hesaplama/Guncelle', { model: item }).success(function (data) {
            angular.extend(item, data.Kayit);
        });
        $scope.$apply();
    }
    $scope.updateAllSelected = function () {
        $scope.updList = [];
        angular.forEach($scope.policeListe, function (item) {
            if ($scope.checkboxes.items[item.Id]) {
                $scope.updList.push(item)
            }
        });
        angular.forEach($scope.updList, function (item) {
            $scope.updateSelectedRow(item);
        });
        $scope.updList = [];
        $scope.$apply();
    }
});
//app.filter('setDecimal', function ($filter) {
//    return function (input, places) {
//        if (isNaN(input)) return input;
//        var factor = "1" + Array(+(places > 0 && places + 1)).join("0");
//        return Math.round(input * factor) / factor;
//    };
//});