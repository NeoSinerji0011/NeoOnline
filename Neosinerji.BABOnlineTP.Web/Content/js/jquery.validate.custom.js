jQuery.validator.setDefaults({ ignore: ".ignore" });
jQuery.validator.methods["date"] = function (value, element)
{
    if (value === undefined)
        return true;
    if (value.length = 0)
        return true;
	var dateFormat = $(element).attr("data-date-format");
	var datepicker = $(element).data("datepicker");
	var result = true;
	try {
		result = ValidateTRDate(value);
	} catch (error) {
		result = false;
	}
	return result;
}

jQuery.validator.addMethod("telefon", function (value, element) {
    var ukod = $('#' + element.id + '_UluslararasiKod').val();
    var akod = $('#' + element.id + '_AlanKodu').val();
    var numara = $('#' + element.id + '_Numara').val();

    if (ukod === undefined) return false;
    if (akod === undefined) return false;
    if (numara === undefined) return false;

    if (akod == 0 && numara.length == 0)
        return true;

    return ukod.length > 0 && akod.length == 3 && numara.length == 7;
});
jQuery.validator.unobtrusive.adapters.addBool("telefon");

jQuery.validator.addMethod("telefonRequired", function (value, element) {
    var ukod = $('#' + element.id + '_UluslararasiKod').val();
    var akod = $('#' + element.id + '_AlanKodu').val();
    var numara = $('#' + element.id + '_Numara').val();

    if (ukod === undefined) return false;
    if (akod === undefined) return false;
    if (numara === undefined) return false;

    return ukod.length > 0 && akod.length == 3 && numara.length == 7;
});
jQuery.validator.unobtrusive.adapters.addBool("telefonRequired");

jQuery.validator.addMethod("kredikarti", function (value, element) {
    var kk1 = $('#' + element.id + '_KK1').val();
    var kk2 = $('#' + element.id + '_KK2').val();
    var kk3 = $('#' + element.id + '_KK3').val();
    var kk4 = $('#' + element.id + '_KK4').val();

    if (kk1 === undefined) return false;
    if (kk2 === undefined) return false;
    if (kk3 === undefined) return false;
    if (kk4 === undefined) return false;

    return kk1.length == 4 && kk2.length == 4 && kk3.length == 4 && kk4.length == 4;
});
jQuery.validator.unobtrusive.adapters.addBool("kredikarti");

jQuery.validator.addMethod("mapfretedaviteminati", function (value, element) {
    var tedaviVar = $("#Teminat_Tedavi_control").bootstrapSwitch('status');
    if (tedaviVar) {
        var olumSakatlikTutar = parseInt($("#Teminat_OlumSakatlikTeminat").autoNumeric('get'));
        var tedaviTutar = parseInt($('#Teminat_TedaviTeminat').autoNumeric('get'));
        var maxTedavi = parseInt(olumSakatlikTutar * 0.1);

        return maxTedavi >= tedaviTutar;
    }

    return true;
});
jQuery.validator.unobtrusive.adapters.addBool("mapfretedaviteminati");

function ValidateTRDate(value) {
	var dateParts = value.split(".");
	if (dateParts.length != 3) { return false; }

	var day = dateParts[0];
	var dayDigit1 = Math.floor(day / 10);
	var dayDigit2 = day % 10;

	var month = dateParts[1];
	var monthDigit1 = Math.floor(month / 10);
	var monthDigit2 = month % 10;

	var isoDate = dateParts[2] + '/' + monthDigit1 + '' + monthDigit2 + '/' + dayDigit1 + '' + dayDigit2;
	var newDate = new Date(isoDate);

	return (dateParts[1] >= 1 && dateParts[1] <= 12)
        && (dateParts[0] >= 1 && dateParts[0] <= 31)
        && /^\d{4}[\/-]\d{1,2}[\/-]\d{1,2}$/.test(isoDate)
        && !/Invalid|NaN/.test(newDate);
}