TO.context = {

    defaultSettings: {
        trigger: 'right',
        zIndex: 300
    },

    init: function () {
        var self = TO.context;
        var ds = self.defaultSettings;

        $.contextMenu($.extend(ds, {
            selector: '.metablock',
            callback: TO.metablock._onContextAction,
            items: TO.metablock.contextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.block',
            callback: TO.block._onContextAction,
            items: TO.block.contextItems
        }));
    }
};

TO.onInit.push(TO.context.init);