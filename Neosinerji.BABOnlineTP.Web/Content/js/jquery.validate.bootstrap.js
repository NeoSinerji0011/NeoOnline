jQuery.validator.setDefaults({
    highlight: function(element) {
        $(element).parents ('.control-group').removeClass ('success').addClass('error');
    },
    success: function(element) {
        $(element).parents ('.control-group').removeClass ('error').addClass('success');
        $(element).parents ('.controls:not(:has(.clean))').find ('div:last').before ('<div class="clean"></div>');
    }
});