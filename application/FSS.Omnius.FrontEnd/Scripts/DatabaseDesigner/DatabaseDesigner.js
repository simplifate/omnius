var DD = {

    onInit: [],

    init: function () {

        var self = DD;

        self.callHooks(self.onInit, null, []);
    },

    callHooks: function (hooks, context, params) {
        for (var i = 0; i < hooks.length; i++) {
            hooks[i].apply(context, params);
        }
    }
};

if (CurrentModuleIs("dbDesignerModule")) {
    $(DD.init);
}