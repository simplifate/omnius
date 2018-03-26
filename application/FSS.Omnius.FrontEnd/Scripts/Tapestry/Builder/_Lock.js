TB.lock = {

    isLocked: false,
    isLockedForCurrentUser: false,
    currentCommitId: null,
    lockStatusId: null,

    init: function () {
        var self = TB.lock;

        TB.load.onLoadBlock.push(self._blockLoad);
        TB.save.onBeforeSave.push($.proxy(self._beforeSave, self));
        TB.save.onAfterSave.push($.proxy(self._afterSave, self));
    },

    _blockLoad: function () {

        // this = AjaxTapestryDesignerBlockCommit

        TB.lock.isLocked = this.LockedForUserId != null && this.LockedForUserId != Number($('#currentUserId').val());
        TB.lock.isLockedForCurrentUser = this.LockedForUserId == Number($('#currentUserId').val());
        TB.lock.LockedForUserName = this.LockedForUserName;
        if (TB.lock.isLockedForCurrentUser) {
            $('#btnLock').html('Odemknout');
        } else {
            $('#btnLock').html('Zamknout');

        }

        var appId = $('#currentAppId').val();
        var blockId = $('#currentBlockId').val();
        var userId = $('#currentUserId').val();
        var url = '/api/tapestry/apps/' + appId + '/blocks/' + blockId + '' + '/getLastCommit/';


        $.ajax({
            type: 'GET',
            url: url,
            dataType: 'json',

            success: function (result) {
                TB.lock.currentCommitId = result;
            }
        });
    },


    _btnLockClick: function () {
        pageSpinner.show();
        var appId = $('#currentAppId').val();
        var blockId = $('#currentBlockId').val();
        var userId = $('#currentUserId').val();
        var url = '/api/tapestry/apps/' + appId + '/blocks/' + blockId + '' + '/isBlockLocked/' + userId + '/commits/' + TB.lock.currentCommitId;


        $.ajax({
            type: 'GET',
            url: url,
            dataType: 'json',
            complete: pageSpinner.hide,
            success: TB.lock._lockTheBlock
        });
    },

    _lockTheBlock: function (result) {
        pageSpinner.show();
        var appId = $('#currentAppId').val();
        var blockId = $('#currentBlockId').val();
        var userId = $('#currentUserId').val();
        if (result.lockStatusId == 0 && $('#btnLock').text() == 'Zamknout') { //if block is not locked  and lockBUttonText= Zamknout
            var url = '/api/tapestry/apps/' + appId + '/blocks/' + blockId + '' + '/lockBlock/' + userId;

            $.ajax({
                type: 'GET',
                url: url,
                dataType: 'json',
                complete: pageSpinner.hide,
                success: TB.lock._onLock
            });
            alert("This block has been successfully locked");
        }

        else if (result.lockStatusId == 2 && $('#btnLock').text() == 'Odemknout') { //if block is locked and id = currentuserId and lockBUttonText= Odemknout
            var url = '/api/tapestry/apps/' + appId + '/blocks/' + blockId + '' + '/unlockBlock/';

            $.ajax({
                type: 'GET',
                url: url,
                dataType: 'json',
                complete: pageSpinner.hide,
                success: TB.lock._onUnlock
            });
            alert("This block has been successfully unlocked");

        }

        else if (result.lockStatusId == 3) {
            alert('This block has been recently updated,please reload the latest version of it and try again');
        }
        else {
            //alert('This block has been locked by ' + result.lockedForUserName + ' ,please wait untill this user unlocks it');
            var msg = ('The block is currently locked by user ' + result.lockedForUserName + ',are you sure you want to force locking this block? It can cause overwriting ' + result.lockedForUserName + '\'s work');

            $('<div></div>').appendTo('body')
                .html('<div><h6>' + msg + '</h6></div>')
                .dialog({
                    modal: true, title: 'Force Lock', zIndex: 10000, autoOpen: true,
                    width: 'auto', resizable: false,
                    buttons: {
                        Yes: function () {
                            // $(obj).removeAttr('onclick');                                
                            // $(obj).parents('.Parent').remove();

                            //dosmth
                            TB.lock._lock();

                            $(this).dialog("close");
                        },
                        No: function () {
                            //do smth

                            $(this).dialog("close");
                        }
                    },
                    close: function (event, ui) {
                        $(this).remove();
                    }
                });

        }
    },

    _onLock: function (result) {
        if (result) {
            $('#btnLock').html('Odemknout');
            TB.lock.isLockedForCurrentUser = true;
        } else {
            $('#btnLock').html('Zamknout');
            TB.lock.isLockedForCurrentUser = false;
        }
    },

    _onUnlock: function (result) {
        if (result) {
            $('#btnLock').html('Zamknout');
            TB.lock.isLockedForCurrentUser = false;
        } else {
            $('#btnLock').html('Odemknout');
            TB.lock.isLockedForCurrentUser = true;
        }
    },


    _beforeSave: function () {
        pageSpinner.show();

        var appId = $('#currentAppId').val();
        var blockId = $('#currentBlockId').val();
        var userId = $('#currentUserId').val();
        if (this.currentCommitId == null) {
            this.currentCommitId = -1;
        }
        var url = '/api/tapestry/apps/' + appId + '/blocks/' + blockId + '' + '/isBlockLocked/' + userId + '/commits/' + this.currentCommitId;

        $.ajax({
            type: 'GET',
            url: url,
            dataType: 'json',
            async: false,
            complete: pageSpinner.hide,
            success: $.proxy(this._checkLockBeforeSave, this)
        });

        return this.lockStatusId == 2;
    },

    _checkLockBeforeSave: function (result) {
        this.lockStatusId = result.lockStatusId;
        switch (this.lockStatusId) {
            case 3: {
                alert('The block has been recently updated, please load the latest version of this block to get around overwriting others commit');
                break;
            }
            case 2: { // mohu uložit kdyz lockedUserid == muj id
                break;
            }
            case 1: {
                var msg = 'The block is currently locked by user ' + result.lockedForUserName + ',but u can press Lock button to Force Locking and overwrite his/her work';
                alert(msg);

                break;
            }
            default: {

                var appId = $('#currentAppId').val();
                var blockId = $('#currentBlockId').val();
                var userId = $('#currentUserId').val();
                if (this.currentCommitId == null) {
                    this.currentCommitId = -1;
                }
                var url = '/api/tapestry/apps/' + appId + '/blocks/' + blockId + '' + '/lockBlock/' + userId;

                $.ajax({
                    type: 'GET',
                    url: url,
                    dataType: 'json',
                    complete: pageSpinner.hide,
                    success: TB.lock._onLock
                });
                TB.lock._beforeSave();
            }
        }
    },

    _lock: function () {
        var appId = $('#currentAppId').val();
        var blockId = $('#currentBlockId').val();
        var userId = $('#currentUserId').val();
        if (this.currentCommitId == null) {
            this.currentCommitId = -1;
        }
        var url = '/api/tapestry/apps/' + appId + '/blocks/' + blockId + '' + '/lockBlock/' + userId;

        $.ajax({
            type: 'GET',
            url: url,
            dataType: 'json',
            complete: pageSpinner.hide,
            success: TB.lock._onLock
        });
    }
    ,
    _afterSave: function (lastCommitId) {
        $('#btnLock').html('Zamknout');
        this.isLockedForCurrentUser = false;
        this.currentCommitId = lastCommitId;
    }
};

TB.onInit.push(TB.lock.init);
