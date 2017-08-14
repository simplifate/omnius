TO.block = {
    
    contextItems: {
        'copy': { name: 'Copy...', icon: 'fa-clone' },
        'properties': { name: 'Properties...', icon: 'fa-cog' },
        'initial': { name: 'Set as initial', icon: 'fa-flag-checkered' },
        'delete': { name: 'Delete', icon: 'fa-trash' }
    },

    init: function () {
        var self = TO.block;

        $(document).on('dblclick', '.block', self._doubleClick);
    },

    add: function() {
        var blockName = $(this).find('#block-name').val();
        var newBlock = $('<div class="block"><div class="blockName">' + blockName + '</div><div class="blockInfo"></div></div>');

        newBlock.appendTo('#overviewPanel .scrollArea');
        newBlock.css({
            'top': $('#overviewPanel').scrollTop() + 20,
            'left': $('#overviewPanel').scrollLeft() + 20
        });

        instance.draggable(newBlock, { containment: 'parent' });
        
        TO.changedSinceLastSave = true;
        TO.dialog.close.apply(this);
    },

    copy: function() {

        if (!$('#c-target-name').val().length) {
            alert('Vyberte cílový metablok.');
            return false;
        }

        var url = '/api/tapestry/{appId}/blocks/{blockId}/copy/{targetMetablockId}';
        url = url.replace(/\{appId\}/, $('#currentAppId').val())
                 .replace(/\{blockId\}/, $(currentBlock).attr('blockid'))
                 .replace(/\{targetMetablockId\}/, $('#c-target-name').val());

        var d = this;

        $.ajax({
            url: url,
            type: 'GET',
            data: {},
            success: function (data) {
                if (data == true) {
                    alert('Blok byl úspěšně zkopírován');

                    if ($('#currentMetablockId').val() == $('#c-target-name').val()) {
                        window.location.reload();
                    } 
                }
                else {
                    alert('Blok se nepodařilo zkopírovat');
                }
                TO.dialog.close.apply(d);
            }
        });
    },

    move: function () {

        if (!$('#c-target-name').val().length) {
            alert('Vyberte cílový metablok.');
            return false;
        }

        if ($('#currentMetablockId').val() == $('#c-target-name').val()) {
            alert('Block nelze přesunout. Již se ve vybraném metabloku nachází.');
            return false;
        }

        var url = '/api/tapestry/{appId}/blocks/{blockId}/move/{targetMetablockId}';
        url = url.replace(/\{appId\}/, $('#currentAppId').val())
                 .replace(/\{blockId\}/, $(currentBlock).attr('blockid'))
                 .replace(/\{targetMetablockId\}/, $('#c-target-name').val());

        var d = this;

        $.ajax({
            url: url,
            type: 'GET',
            data: {},
            success: function (data) {
                if (data == true) {
                    alert('Blok byl úspěšně přesunut');
                    window.location.reload();
                }
                else {
                    alert('Blok se nepodařilo přesunout');
                    TO.dialog.close.apply(d);
                }
            }
        });
    },

    /*************************************************/
    /* EVENTS                                        */
    /*************************************************/
    _doubleClick: function() {
        var blockToOpen = $(this);

        SaveMetablock(function () {
            var openBlockForm = $('#openBlockForm');
            openBlockForm.find('input[name=blockId]').val(blockToOpen.attr('blockId'));
            openBlockForm.submit();
        });
    },

    _addOpen: function() {
        $(this).find('#block-name').val('');
    },

    _copyOpen: function() {
        $('#c-block-name').html($(currentBlock).find('.blockName').text());
        $('#c-target-name').val('');
    },

    _onContextAction: function (key, options) {
        switch (key) {
            case 'delete': {
                instance.removeAllEndpoints(options.$trigger, true);
                options.$trigger.remove();
                TO.changedSinceLastSave = true;
                SaveMetablock();
                break;
            }
            case 'initial': {
                $('#overviewPanel .block').each(function (index, element) {
                    $(element).attr('isInitial', false);
                    $(element).find('.blockInfo').text('');
                });
                options.$trigger.attr('isInitial', true);
                options.$trigger.find('.blockInfo').text('Initial');
                
                TO.changedSinceLastSave = true;
                break;
            }
            case 'properties': {
                currentBlock = options.$trigger;
                blockPropertiesDialog.dialog('open');
                break;
            }
            case 'copy': {
                currentBlock = options.$trigger;
                TO.dialog.open('blockCopy');
                break;
            }
        }
    }
};

TO.onInit.push(TO.block.init);