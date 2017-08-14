TO.metablock = {

    contextItems: {
        'properties': { name: 'Properties...', icon: 'fa-cog' },
        'initial': { name: 'Set as initial', icon: 'fa-flag-checkered' },
        'delete': { name: 'Delete', icon: 'fa-trash' }
    },

    init: function () {
        var self = TO.metablock;

        $(document).on('dblclick', '.metablock', self._doubleClick);
    },

    rename: function () {
        $('#headerMetablockName').text($(this).find('#metablock-name').val());
        TO.changeSinceLastSave = true;
        TO.dialog.close.apply(this);
    },

    add: function() {
        var metablockName = $(this).find('#metablock-name').val();
        var newMetablock = $('<div class="metablock"><div class="metablockName">'
            + metablockName + '</div><div class="metablockSymbol fa fa-th-large"></div><div class="metablockInfo"></div></div>');

        newMetablock.appendTo('#overviewPanel .scrollArea');
        newMetablock.css({
            'top': $('#overviewPanel').scrollTop() + 20,
            'left': $('#overviewPanel').scrollLeft() + 20
        });
        
        instance.draggable(newMetablock, { containment: 'parent' });
        
        TO.changeSinceLastSave = true;
        TO.dialog.close.apply(this);
    },

    /***********************************************/
    /* EVENTS                                      */
    /***********************************************/
    _doubleClick: function() {
        var metablockToOpen = $(this);
        SaveMetablock(function () {
            var openMetablockForm = $('#openMetablockForm');
            openMetablockForm.find('input[name=metablockId]').val(metablockToOpen.attr('metablockId'));
            openMetablockForm.submit();
        });
    },

    _renameOpen: function () {
        $(this).find('#metablock-name').val($('#headerMetablockName').text());
    },

    _addOpen: function () {
        $(this).find('#metablock-name').val('');
    },

    _onContextAction: function (key, options) {
        switch (key) {
            case "delete": {
                instance.removeAllEndpoints(options.$trigger, true);
                options.$trigger.remove();
                TO.changedSinceLastSave = true;
                SaveMetablock();
                break;
            }
            case "initial": {
                $("#overviewPanel .metablock").each(function (index, element) {
                    $(element).attr("isInitial", false);
                    $(element).find(".metablockInfo").text("");
                });
                options.$trigger.attr("isInitial", true);
                options.$trigger.find(".metablockInfo").text("Initial");
                
                TO.changedSinceLastSave = true;
                break;
            }
            case "properties": {
                currentMetablock = options.$trigger;
                metablockPropertiesDialog.dialog("open");
                break;
            }
        }
    }
};

TO.onInit.push(TO.metablock.init);