TB.dialog = {

    dialogList: {},

    dialogDefaults: {
        autoOpen: false,
        width: 'auto',
        height: 'auto',
        create: function () {
            var d = $(this).parents('.ui-dialog');
            var buttons = d.find('.ui-dialog-buttonset button');
            var dialogId = $(this).data('dialogId');

            $.each(TB.dialog.dialogList[dialogId].options.buttons, function (index) {
                buttons.eq(index).addClass(this.className);
                if (this.icon) {
                    buttons.eq(index).prepend('<span class="fa ' + this.icon + '"></span> ');
                }
            });

            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    TB.dialog.dialogList[dialogId].submit.apply(this, []);
                    return false;
                }
            });

            d.find('.ui-dialog-buttonset').css('float', 'none');
        }
    },

    init: function()
    {
        var self = TB.dialog;
        for (var k in self.dialogList) {
            var d = self.dialogList[k];
            $(d.target).data('dialogId', k);
            $(d.target).dialog($.extend(self.dialogDefaults, d.options));
        }
    },

    open: function(dialogId) {
        $(TB.dialog.dialogList[dialogId].target).dialog('open');
    },

    close: function () {
        $(this).dialog('close');
    }
};

TB.dialog.dialogList = {
    actionItemName: {
        target: '#action-item-name-dialog',
        submit: TB.wfr._actionItemSetName,
        options: {
            buttons: [
                { text: 'Save', click: TB.wfr._actionItemSetName, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.wfr._actionItemSetNameOpen
        }
    },
    actionItemComment: {
        target: '#action-item-comment-dialog',
        submit: TB.wfr._actionItemSetComment,
        options: {
            buttons: [
                { text: 'Save', click: TB.wfr._actionItemSetComment, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.wfr._actionItemSetCommentOpen
        }
    },
    workflowCopy: {
        target: '#workflow-copy-dialog',
        submit: TB.wfr._ruleCopy,
        options: {
            buttons: [
                { text: 'Copy', click: TB.wfr._ruleCopy, className: 'btn btn-success pull-right', icon: 'fa-clone' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-defult', icon: 'fa-times' }
            ],
            open: TB.wfr._ruleCopyOpen,
            width: 600
        }
    },
    save: {
        target: '#save-dialog',
        submit: TB.save.saveBlock,
        options: {
            buttons: [
                { text: 'Save', click: TB.save.saveBlock, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.save._dialogOpen,
            width: 400
        }
    },
    subflowName: {
        target: '#subflow-name-dialog',
        submit: TB.subflow.setName,
        options: {
            buttons: [
                { text: 'Save', click: TB.subflow.setName, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.subflow._setNameOpen
        }
    },
    subflowComment: {
        target: '#subflow-comment-dialog',
        submit: TB.subflow.setComment,
        options: {
            buttons: [
                { text: 'Save', click: TB.subflow.setComment, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.subflow._setCommentOpen
        }
    },
    foreachDatasource: {
        target: '#foreach-datasource-dialog',
        submit: TB.foreach.setDatasource,
        options: {
            buttons: [
                { text: 'Save', click: TB.foreach.setDatasource, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.foreach._setDatasourceOpen
        }
    },
    foreachItemName: {
        target: '#foreach-itemname-dialog',
        submit: TB.foreach.setItemName,
        options: {
            buttons: [
                { text: 'Save', click: TB.foreach.setItemName, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.foreach._setItemNameOpen
        }
    },
    foreachName: {
        target: '#foreach-name-dialog',
        submit: TB.foreach.setName,
        options: {
            buttons: [
                { text: 'Save', click: TB.foreach.setName, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.foreach._setNameOpen
        }
    },
    foreachComment: {
        target: '#foreach-comment-dialog',
        submit: TB.foreach.setComment,
        options: {
            buttons: [
                { text: 'Save', click: TB.foreach.setComment, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.foreach._setCommentOpen
        }
    }
}

TB.onInit.push(TB.dialog.init);