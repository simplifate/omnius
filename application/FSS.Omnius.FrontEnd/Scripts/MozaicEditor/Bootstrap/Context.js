MBE.context = {

    defaultSettings: {
        trigger: 'right',
        zIndex: 300
    },

    init: function () {

        var ds = this.defaultSettings;

        MBE.workspace.contextMenu($.extend(ds, {
            selector: '.mbe-active', 
            callback: MBE.selection._contextAction, 
            items: MBE.selection.contextItems,
            position: function (opt, x, y) {
                opt.$menu.css({
                    top: y + $('#mozaicPageWorkspace > iframe').offset().top,
                    left: x + $('#mozaicPageWorkspace > iframe').offset().left
                }); 
            }
        }));

        $.contextMenu($.extend(ds, {
            selector: '.tree-nav .node-handle b',
            callback: MBE.selection._contextAction,
            items: MBE.selection.contextItems,
            events: {
                show: function (options) {
                    MBE.navigator.selectNode.apply(this, [{}]);
                }
            }
        }));
    }
};

MBE.onInit.push(MBE.context.init);