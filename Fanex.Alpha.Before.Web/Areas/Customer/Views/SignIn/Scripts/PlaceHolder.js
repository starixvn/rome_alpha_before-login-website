"use strict";

var Sphinx = Sphinx || {};
Sphinx.PlaceHolder = {
    createInputPlaceHolder: function () {
        var supportsPlaceholder = 'placeholder' in document.createElement('input');
        $('[placeholder]').each(function () {
            if (!supportsPlaceholder) {
                var self = $(this);
                self.val(self.attr('placeholder')).addClass('placeholder');
                self.focus(function () {
                    if (self.val() === self.attr('placeholder')) {
                       self.val('').removeClass('placeholder');
                    }
                });

                self.blur(function () {
                    if (self.val() === '') {
                        self.val(self.attr('placeholder')).addClass('placeholder');
                    }
                });
            }
        });
    }
};
$(document).ready(function () {
    Sphinx.PlaceHolder.createInputPlaceHolder();
});


