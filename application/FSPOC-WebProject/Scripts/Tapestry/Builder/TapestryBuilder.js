var TB = {

    onInit: [],

    init: function () {
        var self = TB;

        for (var i = 0; i < self.onInit.length; i++) {
            self.onInit[i]();
        }
    }
};


$(function () {
    if (CurrentModuleIs("tapestryModule")) {
        TB.init();
    }
});
