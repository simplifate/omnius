MBE.dialogs = {

    choosePage: function()
    {
        $('#choose-page-dialog').dialog({
            autoOpen: true,
            width: 700,
            height: 540,
            buttons: {
                Open: MBE.io.loadPage,
                Delete: MBE.io.deletePage,
                Cancel: MBE.dialogs.close
            },
            open: MBE.io.loadPageList
        });
    },

    trash: function()
    {
        $('#trash-page-dialog').dialog({
            autoOpen: true,
            width: 700,
            height: 540,
            buttons: {
                Load: MBE.io.loadDeletedPage,
                Cancel: MBE.dialogs.close
            },
            open: MBE.io.loadDeletedPageList
        });
    },

    newPage: function()
    {
        $('#new-page-dialog').dialog({
            autoOpen: true,
            width: 400,
            height: 170,
            buttons: {
                Save: MBE.io.createPage,
                Cancel: MBE.dialogs.close
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        MBE.io.createPage;
                        return false;
                    }
                })
            },
            open: function () { $('#new-page-name').val(''); }
        });
    },

    rename: function() {
        $('#rename-page-dialog').dialog({
            autoOpen: true,
            width: 400,
            height: 190,
            buttons: {
                Save: MBE.io.rename,
                Cancel: MBE.dialogs.close
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        MBE.io.rename.apply(document.getElementById('rename-page-dialog'), []);
                        return false;
                    }
                })
            },
            open: function () {
                $(this).find('#page-name').val($('#headerPageName').text());
            }
        });
    },

    close: function () {
        $(this).dialog("close");
    }
};