var FormWizard = function () {
    var beforeTabChangeCallback;

    return {
        //main function to initiate the module
        init: function (tabChange) {
            if (!jQuery().bootstrapWizard) {
                return;
            }

            beforeTabChangeCallback = tabChange;

            // default form wizard
            $('#form_wizard_1').bootstrapWizard({
                'nextSelector': '.button-next',
                'previousSelector': '.button-previous',

                onTabClick: function (tab, navigation, index) {
                    var teklifHazirlandi = $("#TeklifHazirlandi").val();

                    if (teklifHazirlandi == "true") {
                        return true;
                    }

                    return false;
                },

                onNext: function (tab, navigation, index) {
                    var total = navigation.find('li').length;
                    var current = index + 1;

                    var changeTab = true;
                    if (beforeTabChangeCallback !== undefined) {
                        var changeTab = beforeTabChangeCallback(index);
                    }
                    if (changeTab) {
                        // set wizard title
                        $('.step-title', $('#form_wizard_1')).text(langu.Step + " " + (index + 1) + ' / ' + total);

                        // set done steps
                        jQuery('li', $('#form_wizard_1')).removeClass("done");
                        var li_list = navigation.find('li');
                        for (var i = 0; i < index; i++) {
                            jQuery(li_list[i]).addClass("done");
                        }

                        if (current == 1) {
                            $('#form_wizard_1').find('.button-previous').hide();
                        } else {
                            $('#form_wizard_1').find('.button-previous').show();
                        }

                        debugger;
                        if (current >= total) {
                            $('#form_wizard_1').find('.button-next').hide();
                            $('#form_wizard_1').find('.button-submit').show();
                        } else {
                            $('#form_wizard_1').find('.button-next').show();
                            $('#form_wizard_1').find('.button-submit').hide();
                        }

                        App.scrollTo();
                        //App.scrollTo($('.page-title'));
                    }

                    return changeTab;
                },

                onPrevious: function (tab, navigation, index) {
                    var total = navigation.find('li').length;
                    var current = index + 1;
                    // set wizard title
                    $('.step-title', $('#form_wizard_1')).text(langu.Step + " " + (index + 1) + ' / ' + total);
                    // set done steps
                    jQuery('li', $('#form_wizard_1')).removeClass("done");
                    var li_list = navigation.find('li');
                    for (var i = 0; i < index; i++) {
                        jQuery(li_list[i]).addClass("done");
                    }

                    if (current == 1) {
                        $('#form_wizard_1').find('.button-previous').hide();
                    } else {
                        $('#form_wizard_1').find('.button-previous').show();
                    }

                    if (current >= total) {
                        $('#form_wizard_1').find('.button-next').hide();
                        $('#form_wizard_1').find('.button-submit').show();
                    } else {
                        $('#form_wizard_1').find('.button-next').show();
                        $('#form_wizard_1').find('.button-submit').hide();
                    }

                    App.scrollTo();
                    //App.scrollTo($('.page-title'));
                },

                onTabShow: function (tab, navigation, index) {
                    var total = navigation.find('li').length;
                    var current = index + 1;
                    var $percent = (current / total) * 100;
                    $('#form_wizard_1').find('.bar').css({
                        width: $percent + '%'
                    });
                    debugger;
                    if (current == (total)) {
                        $('#form_wizard_1').find('.button-next').hide();
                        $('#form_wizard_1').find('.button-submit').show();
                    }
                    else if (current == total) {
                        $('#form_wizard_1').find('.button-next').hide();
                        $('#form_wizard_1').find('.button-submit').hide();
                        $('#form_wizard_1').find('.button-previous').hide();
                    }
                    else {
                        $('#form_wizard_1').find('.button-next').show();
                        $('#form_wizard_1').find('.button-submit').hide();
                    }
                }
            });

            $('#form_wizard_1').find('.button-previous').hide();
            $('#form_wizard_1 .button-submit').click(function () {
            }).hide();
        },

        validatePage: function (tabId) {
            var isValid = true;

            $(tabId).find("input, select").each(function () {

                if ($(this).not($.validator.defaults.ignore).length === 0) {
                }
                else {
                    if (isValid) {
                        isValid = $(this).valid();
                    } else {
                        $(this).valid();
                    }
                }
            });

            return isValid;
        }
    };

}();