var language = angular.module('resourceModule', []).factory('resourceService', [
function () {
    var getCookie = function (cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1);
            if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
        }
        return "";
    };
    var lang = ['tr-TR', 'en-US', 'es-ES', 'fr-FR', 'it-IT'];
    var cookieLang = getCookie('lang');
    var currentLang = lang.indexOf(cookieLang) > -1 ? cookieLang : 'tr-TR';
    var dictionary =
    {
        operation: { 'tr-TR': 'İşlem', 'en-US': 'Operation', 'es-ES': 'Operation', 'fr-FR': 'Operation', 'it-IT': 'Operation' }
        , list: { 'tr-TR': 'Listele', 'en-US': 'List', 'es-ES': 'List', 'fr-FR': 'List', 'it-IT': 'List' }
        , commission_rate: { 'tr-TR': 'Oran Belirle', 'en-US': 'Commission Rate', 'es-ES': 'Commission Rate', 'fr-FR': 'Commission Rate', 'it-IT': 'Commission Rate' }
        , detection_rate: { 'tr-TR': 'Oran Belirle', 'en-US': 'Commission Rate', 'es-ES': 'Commission Rate', 'fr-FR': 'Commission Rate', 'it-IT': 'Commission Rate' }
        , sub_agency_detection_rate: { 'tr-TR': 'Satış Kanalı Oran Belirleme', 'en-US': 'Sub Agency Detection Rate', 'es-ES': 'Sub Agency Detection Rate', 'fr-FR': 'Sub Agency Detection Rate', 'it-IT': 'Sub Agency Detection Rate' }
        , sub_agency: { 'tr-TR': 'Satış Kanalı', 'en-US': 'Sub Agency', 'es-ES': 'Sub Agency', 'fr-FR': 'Sub Agency', 'it-IT': 'Sub Agency' }
        , branch: { 'tr-TR': 'Branş', 'en-US': 'Branch', 'es-ES': 'Branch', 'fr-FR': 'Branch', 'it-IT': 'Branch' }
        , insurance_company: { 'tr-TR': 'Sigorta Şirketi', 'en-US': 'Insurance Company', 'es-ES': 'Insurance Company', 'fr-FR': 'Insurance Company', 'it-IT': 'Insurance Company' }
        , validity_start_date: { 'tr-TR': 'Geçerlilik Başlangıç Tarihi', 'en-US': 'Validity Start Date', 'es-ES': 'Validity Start Date', 'fr-FR': 'Validity Start Date', 'it-IT': 'Validity Start Date' }
        , please_waiting: { 'tr-TR': 'Lütfen bekleyiniz.', 'en-US': 'Please Waiting', 'es-ES': 'Please Waiting', 'fr-FR': 'Please Waiting', 'it-IT': 'Please Waiting' }
        , pw_detection_rate: { 'tr-TR': 'Lütfen bekleyiniz. Oran Listesi Hazırlanıyor.', 'en-US': 'Please Waiting while Detection Rate', 'es-ES': 'Please Waiting while Detection Rate', 'fr-FR': 'Please Waiting while Detection Rate', 'it-IT': 'Please Waiting while Detection Rate' }
        , rate: { 'tr-TR': 'Oran', 'en-US': 'Rate', 'es-ES': 'Rate', 'fr-FR': 'Rate', 'it-IT': 'Rate' }
        , update: { 'tr-TR': 'Güncelle', 'en-US': 'Update', 'es-ES': 'Update', 'fr-FR': 'Update', 'it-IT': 'Update' }
        , enter_date: { 'tr-TR': 'Tarih Giriniz.', 'en-US': 'Please,Enter Date', 'es-ES': 'Please,Enter Date', 'fr-FR': 'Please,Enter Date', 'it-IT': 'Please,Enter Date' }
        , sub_agency_title: { 'tr-TR': 'Sigorta Uzmanı Ünvanı', 'en-US': 'Sub Agency Title', 'es-ES': 'Sub Agency Title', 'fr-FR': 'Sub Agency Title', 'it-IT': 'Sub Agency Title' }
        , select_all: { 'tr-TR': 'Tümünü Seç', 'en-US': 'Select All', 'es-ES': 'Select All', 'fr-FR': 'Select All', 'it-IT': 'Select All' }
        , unselect_all: { 'tr-TR': 'Tümünü Kaldır.', 'en-US': 'Unselect All', 'es-ES': 'Unselect All', 'fr-FR': 'Unselect All', 'it-IT': 'Unselect All' }
        , error_occurred: { 'tr-TR': 'Hata Oluştu', 'en-US': 'Error Occurred', 'es-ES': 'Error Occurred', 'fr-FR': 'Error Occurred', 'it-IT': 'Error Occurred' }
        , item_updated: { 'tr-TR': 'Kayıt Güncellendi', 'en-US': 'Item Updated', 'es-ES': 'Item Updated', 'fr-FR': 'Item Updated', 'it-IT': 'Item Updated' }
        , select: { 'tr-TR': 'Seçiniz', 'en-US': 'Select', 'es-ES': 'Select', 'fr-FR': 'Select', 'it-IT': 'Select' }
        , filter: { 'tr-TR': 'Filtre', 'en-US': 'Filter', 'es-ES': 'Filter', 'fr-FR': 'Filter', 'it-IT': 'Filter' }
        , are_you_sure: { 'tr-TR': 'Emin misiniz?', 'en-US': 'Are you sure ?', 'es-ES': 'Are you sure ?', 'fr-FR': 'Are you sure ?', 'it-IT': 'Are you sure ?' }
        , detection_rate_wtih_selected_fitler: { 'tr-TR': 'Filtredeki seçili kritlerde oran belirnecektir.', 'en-US': 'Commission rate with selected filter criteria', 'es-ES': 'Commission rate with selected filter criteria', 'fr-FR': 'Commission rate with selected filter criteria', 'it-IT': 'Commission rate with selected filter criteria' }
        , detectioning_rate: { 'tr-TR': 'Oran belirleniyor', 'en-US': 'Detectioning rate', 'es-ES': 'Detectioning rate', 'fr-FR': 'Detectioning rate', 'it-IT': 'Detectioning rate' }
        , cancel: { 'tr-TR': 'İptal', 'en-US': 'Cancel', 'es-ES': 'Cancel', 'fr-FR': 'Cancel', 'it-IT': 'Cancel' }
        , cancel_detection_rate: { 'tr-TR': 'Oran belirleme iptal edildi.', 'en-US': 'Detection rate is cancelled', 'es-ES': 'Detection rate is cancelled', 'fr-FR': 'Detection rate is cancelled', 'it-IT': 'Detection rate is cancelled' }
        , successful: { 'tr-TR': 'Başarılı', 'en-US': 'Successful', 'es-ES': 'Successful', 'fr-FR': 'Successful', 'it-IT': 'Successful' }
        , policy_state: { 'tr-TR': 'Poliçe Durumu', 'en-US': 'Policy State', 'es-ES': 'Policy State', 'fr-FR': 'Policy State', 'it-IT': 'Policy State' }
        , ps_uncalculated: { 'tr-TR': 'Hesaplanmamış', 'en-US': 'Uncalculated', 'es-ES': 'Uncalculated', 'fr-FR': 'Uncalculated', 'it-IT': 'Uncalculated' }
        , ps_calculated: { 'tr-TR': 'Hesaplanmış', 'en-US': 'Calculated', 'es-ES': 'Calculated', 'fr-FR': 'Calculated', 'it-IT': 'Calculated' }
        , ps_all: { 'tr-TR': 'Hepsi', 'en-US': 'All', 'es-ES': 'All', 'fr-FR': 'All', 'it-IT': 'All' }
        , policy_start_date: { 'tr-TR': 'Poliçe Başlangıç Tarihi', 'en-US': 'Policy Start Date', 'es-ES': 'Policy Start Date', 'fr-FR': 'Policy Start Date', 'it-IT': 'Policy Start Date' }
        , policy_end_date: { 'tr-TR': 'Poliçe Bitiş Tarihi', 'en-US': 'Policy End  Date', 'es-ES': 'Policy End  Date', 'fr-FR': 'Policy End  Date', 'it-IT': 'Policy End  Date' }
        , premium: { 'tr-TR': 'Prim', 'en-US': 'Premium', 'es-ES': 'Premium', 'fr-FR': 'Premium', 'it-IT': 'Premium' }
        , net_premium: { 'tr-TR': 'Net Prim', 'en-US': ' Net Premium', 'es-ES': 'Net Premium', 'fr-FR': ' Net Premium', 'it-IT': 'Net Premium' }
        , received_commission: { 'tr-TR': 'Alınan Komisyon', 'en-US': 'Received Commission', 'es-ES': 'Received Commission', 'fr-FR': 'Received Commission', 'it-IT': 'Received Commission' }
        , policy_no: { 'tr-TR': 'Poliçe No', 'en-US': 'Policy No', 'es-ES': 'Policy No', 'fr-FR': 'Policy No', 'it-IT': 'Policy No' }
        , sub_agency_commission_rate: { 'tr-TR': 'Sigorta Uzmanı Komisyon Oranı', 'en-US': 'Sub Agency Commission Rate', 'es-ES': 'Sub Agency Commission Rate', 'fr-FR': 'Sub Agency Commission Rate', 'it-IT': 'Sub Agency Commission Rate' }
        , sub_agency_commission: { 'tr-TR': 'Sigorta Uzmanı Komisyon Tutarı', 'en-US': 'Sub Agency Commission', 'es-ES': 'Sub Agency Commission', 'fr-FR': 'Sub Agency Commission', 'it-IT': 'Sub Agency Commission' }
        , commission_calculate: { 'tr-TR': 'Komisyon Hesaplama', 'en-US': 'Commission Calculate', 'es-ES': 'Commission Calculate', 'fr-FR': 'Commission Calculate', 'it-IT': 'Commission Calculate' }
        , commission_type: { 'tr-TR': 'Komisyon Türü', 'en-US': 'Commission Type', 'es-ES': 'Commission Type', 'fr-FR': 'Commission Type', 'it-IT': 'Commission Type' }
        , commissionByInsuranceCompany: { 'tr-TR': 'Sigorta Şirketi ile', 'en-US': 'By Insurance Company', 'es-ES': 'By Insurance Company', 'fr-FR': 'By Insurance Company', 'it-IT': 'By Insurance Company' }
        , commissionBygraded: { 'tr-TR': 'Kademeli', 'en-US': 'Gradual', 'es-ES': 'Gradual', 'fr-FR': 'Gradual', 'it-IT': 'Gradual' }
        , canselZeyl: { 'tr-TR': 'İptal Zeylleri', 'en-US': 'İptal Zeylleri', 'es-ES': 'İptal Zeylleri', 'fr-FR': 'İptal Zeylleri', 'it-IT': 'İptal Zeylleri' }
        , accrual: { 'tr-TR': 'Tahakkuk', 'en-US': 'Accrual', 'es-ES': 'Accrual', 'fr-FR': 'Accrual', 'it-IT': 'Accrual' }
        , primary_type: { 'tr-TR': 'Prim Tipi', 'en-US': 'Primary Type', 'es-ES': 'Primary Type', 'fr-FR': 'Primary Type', 'it-IT': 'Primary Type' }
        , policy_issue_date: { 'tr-TR': 'Poliçe Tanzim Tarihi', 'en-US': 'Policy Issue Date', 'es-ES': 'Policy Issue Date', 'fr-FR': 'Policy Issue Date', 'it-IT': 'Policy Issue Date' }
        , outsource: { 'tr-TR': 'Dış Kaynak', 'en-US': 'Outsource', 'es-ES': 'Outsource', 'fr-FR': 'Outsource', 'it-IT': 'Outsource' }
        , Resource_Selection: { 'tr-TR': 'Kaynak Seçimi', 'en-US': 'Resource Selection', 'es-ES': 'Resource Selection', 'fr-FR': 'Resource Selection', 'it-IT': 'Resource Selection' }
        , insured_by: { 'tr-TR': 'Sigortalı Unvanı', 'en-US': 'Insured Title', 'es-ES': 'Insured Title', 'fr-FR': 'Insured Title', 'it-IT': 'Insured Title' }
        , insurer_by: { 'tr-TR': 'Sigorta Ettiren Unvanı', 'en-US': 'Insurer Title', 'es-ES': 'Insurer Title', 'fr-FR': 'Insurer Title', 'it-IT': 'Insurer Title'}

    };
    return {
        get: function (name) { return dictionary[name][currentLang] },
    }
}
]);
