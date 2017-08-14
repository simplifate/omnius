var TO = {
    
    zoomFactor: 1,
    currentBlock: null,
    currentMetablock: null,
    changeSinceLastSave: false,

    onInit: [],

    init: function ()
    {
        var self = TO;

        $(document)
            .on('click', '#headerMetablockName', function () { TO.dialog.open('metablockRename'); })
            .on('click', '#btnAddMetablock', function () { TO.dialog.open('metablockAdd'); })
            .on('click', '#btnAddBlock', function () { TO.dialog.open('blockAdd'); })
            .on('click', '#btnTrash', function () { TO.dialog.open('trash'); })
            .on('click', '#btnSave', self.save._save)
            .on('click', '#btnLoad', self.load._load)
            .on('click', '#btnMenuOrder', self.sortForMenu)
            .on('click', '#btnClear', self.clear)
            .on('click', '#btnGoUp', self.goUp)
            .on('click', '#btnZoomIn', self.zoomIn)
            .on('click', '#btnZoomOut', self.zoomOut)
        ;

        window.onbeforeunload = self._beforeUnload; 

        self.callHooks(self.onInit, null, []);
    },

    clear: function() {
        $('#overviewPanel .block, #overviewPanel .metablock').each(function () {
            instance.removeAllEndpoints(this, true);
            $(this).remove();
        });
    },

    goUp: function() {
        SaveMetablock(function () {
            var openMetablockForm = $('#openMetablockForm');
            openMetablockForm.find('input[name=metablockId]').val($('#parentMetablockId').val());
            openMetablockForm.submit();
        });
    },

    zoomIn: function() {
        TO.zoomFactor += 0.1;
        $('#overviewPanel .scrollArea').css('transform', 'scale(' + TO.zoomFactor + ')');
        $('#zoomLabel').text('Zoom ' + Math.floor(TO.zoomFactor * 100) + '%');
    },

    zoomOut: function() {
        if (TO.zoomFactor >= 0.2)
            TO.zoomFactor -= 0.1;
        $('#overviewPanel .scrollArea').css('transform', 'scale(' + TO.zoomFactor + ')');
        $('#zoomLabel').text('Zoom ' + Math.floor(TO.zoomFactor * 100) + '%');
    },

    sortForMenu: function() {
        location.href = "/Tapestry/Overview/MenuOrder/" + $('#currentMetablockId').val();
    },

    callHooks: function (hooks, context, params) {
        for (var i = 0; i < hooks.length; i++) {
            hooks[i].apply(context, params);
        }
    },

    _beforeUnload: function() {
        if (TO.changedSinceLastSave)
            SaveMetablock(null, true);
        return null;
    }
};

if (CurrentModuleIs("overviewModule")) {
    $(TO.init);
}