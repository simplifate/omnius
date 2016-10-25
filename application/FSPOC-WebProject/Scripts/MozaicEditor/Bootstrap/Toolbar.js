MBE.toolbar = {

    menu: {},
    toolbar: null,

    init: function () {
        MBE.selection.onSelect.push(MBE.toolbar._select);
        MBE.toolbar.toolbar = $('#mozaicPageContextToolbar .toolbar.pull-left');
    },

    _select: function () {
        var self = MBE.toolbar;
        self.clear();

        var uic = $(this).data('uic');
        if (!uic) {
            return;
        }

        uic = uic.split('|');
        var type = uic[0];
        var template = uic[1];

        if (typeof self.menu[type] == 'undefined' || typeof self.menu[type][template] == 'undefined') {
            return;
        }

        var menu = self.menu[type][template];
        if (typeof menu.allowFor == 'function' && !menu.allowFor.apply(this, []))
            return;

        for (var i = 0; i < menu.items.length; i++) {
            var item = menu.items[i];
            switch (item.type) {
                case 'text': {
                    self.toolbar.append('<p class="navbar-text">' + item.label + '</p>');
                    break;
                }
                case 'button': {
                    var button = $('<button type="button" class="btn navbar-btn">' + item.label + '</button>');
                    self.toolbar.append(button);
                    button.click({callback: item.callback}, function (event) {
                        event.data.callback.apply($('.mbe-active')[0], []);
                    });
                }
            }
        }
    },

    clear: function () {
        MBE.toolbar.toolbar.html('');
    }

};

MBE.onInit.push(MBE.toolbar.init);