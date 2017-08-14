TO.dialog = {

    dialogList: {},

    dialogDefaults: {
        autoOpen: false,
        width: 'auto',
        height: 'auto',
        create: function () {
            var d = $(this).parents('.ui-dialog');
            var buttons = d.find('.ui-dialog-buttonset button');
            var dialogId = $(this).data('dialogId');

            $.each(TO.dialog.dialogList[dialogId].options.buttons, function (index) {
                buttons.eq(index).addClass(this.className);
                if (this.icon) {
                    buttons.eq(index).prepend('<span class="fa ' + this.icon + '"></span> ');
                }
            });

            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    TO.dialog.dialogList[dialogId].submit.apply(this, []);
                    return false;
                }
            });

            d.find('.ui-dialog-buttonset').css('float', 'none');
        }
    },

    init: function () {
        var self = TO.dialog;
        for (var k in self.dialogList) {
            var d = self.dialogList[k];
            $(d.target).data('dialogId', k);
            $(d.target).dialog($.extend(self.dialogDefaults, d.options));
        }
    },

    open: function (dialogId) {
        $(TO.dialog.dialogList[dialogId].target).dialog('open');
    },

    close: function () {
        $(this).dialog('close');
    }
};

TO.dialog.dialogList = {
    metablockRename: {
        target: '#rename-metablock-dialog',
        submit: TO.metablock.rename,
        options: {
            buttons: [
                { text: 'Save', click: TO.metablock.rename, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TO.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TO.metablock._renameOpen
        }
    },
    metablockAdd: {
        target: '#add-metablock-dialog',
        submit: TO.metablock.add,
        options: {
            buttons: [
                { text: 'Add', click: TO.metablock.add, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TO.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TO.metablock._addOpen
        }
    },
    blockAdd: {
        target: '#add-block-dialog',
        submit: TO.block.add,
        options: {
            buttons: [
                { text: 'Add', click: TO.block.add, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TO.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TO.block._addOpen
        }
    },
    blockCopy: {
        target: '#copy-block-dialog',
        submit: TO.block.copy,
        options: {
            width: 700,
            buttons: [
                { text: 'Copy', click: TO.block.copy, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Move', click: TO.block.move, className: 'btn btn-default pull-right', icon: 'fa-arrow-right' },
                { text: 'Cancel', click: TO.dialog.close, className: 'btn btn-default', icon: 'fa-times'}
            ],
            open: TO.block._copyOpen
        }
    },
    trash: {
        target: '#trash-dialog',
        submit: TO.trash._trashLoad,
        options: {
            buttons: [
                { text: 'Load', click: TO.trash._trashLoad, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TO.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TO.trash._trashOpen
        }
    }
}

TO.onInit.push(TO.dialog.init);