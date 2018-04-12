DD.lock = {

    appId: null, //current app id
    currentUserId: null, //current user id
    currentUserName: null, //currentUserName
    isLockedForCurrentUser: false,
    isLocked: false,
    CurrentSchemeCommitId: null,

    init: function () {
        pageSpinner.show();
        var self = DD.lock;
        self.appId = $("#currentAppId").val();
        self.currentUserId = $("#currentUserId").val();
        self.currentUserName = $("#currentUserName").val(); //optional: currentUser name
        console.log("appId: " + self.appId + " | currentUser: " + self.currentUserId);
        $('.top-bar-container').html($('#appName').val()); //ApplicationName on top panel

        $.ajax({
            type: "GET",
            url: "/api/database/apps/" + appId + "/commits/latest",
            dataType: "json",
            complete: function () {
                pageSpinner.hide()
            },
            success: function (data) {
                self.CurrentSchemeCommitId = data.CurrentSchemeCommitId;

                DD.lock.isLockedForCurrentUser = data.SchemeLockedForUserId != null && DD.lock.currentUserId == data.SchemeLockedForUserId;
                DD.lock.isLocked = data.SchemeLockedForUserId != null && DD.lock.currentUserId != data.SchemeLockedForUserId;

                if (DD.lock.isLockedForCurrentUser) {
                    $('#btnLockScheme').html('Unlock scheme');
                }
                else if (DD.lock.isLocked) {
                    $('.top-bar-container').html($('#appName').val() + ' - ' + data.SchemeLockedForUserName + ' is working with Entitron!');
                }
                else {

                    $('#btnLockScheme').html('Lock scheme');
                    $('.top-bar-container').html($('#appName').val()); //ApplicationName on top panel
                }
            }
        });
    },

    _lockSchemeClick: function () {  //when user clicks to lock scheme
        pageSpinner.show();
        var appId = self.appId;
        var userId = self.currentUserId;
        var CurrentSchemeCommitId = DD.lock.CurrentSchemeCommitId;

        var url = "/api/database/apps/" + appId + "/isSchemeLocked/" + userId + "/" + CurrentSchemeCommitId;

        $.ajax({
            type: 'GET',
            url: url,
            dataType: 'json',
            complete: pageSpinner.hide,
            success: DD.lock._lockScheme
        });

    },

    _lockScheme: function (result) {
        if (result.lockStatusId == 0) //if scheme is not locked
        {
            pageSpinner.show();
            var appId = self.appId;
            var userId = self.currentUserId;

            var url = "/api/database/apps/" + appId + "/LockScheme/" + userId;

            $.ajax({
                type: 'GET',
                url: url,
                dataType: 'json',
                complete: pageSpinner.hide,
                success: DD.lock._onLock
            });
            alert("Scheme has been successfully locked");

        }
        else if (result.lockStatusId == 2) //if scheme is locked for me => unlock now
        {
            pageSpinner.show();
            var appId = self.appId;
            var userId = self.currentUserId;

            var url = "/api/database/apps/" + appId + "/UnlockScheme/" + userId;

            $.ajax({
                type: 'GET',
                url: url,
                dataType: 'json',
                complete: pageSpinner.hide,
                success: DD.lock._onUnlock
            });
            alert("Scheme has been successfully unlocked");

        }
        else if (result.lockStatusId == 3) {
            alert('Application scheme has been recently updated,please reload the latest version of it and try again');
        }
        else {
            var msg = ('The scheme is currently locked by user ' + result.lockedForUserName + ',are you sure you want to force locking this scheme? It can cause overwriting ' + result.lockedForUserName + '\'s work');

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
                            DD.lock._lock();

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
            $('#btnLockScheme').html('Unlock scheme');
        }
    },
    _onUnlock: function (result) {

        if (result) {
            $('#btnLockScheme').html('Lock scheme');
        }
    },

    _save: function () {
        pageSpinner.show();
        var appId = self.appId;
        var userId = self.currentUserId;
        var CurrentSchemeCommitId = DD.lock.CurrentSchemeCommitId;
        if (CurrentSchemeCommitId == null) {
            CurrentSchemeCommitId = -1;
        }
        var url = "/api/database/apps/" + appId + "/isSchemeLocked/" + userId + "/" + CurrentSchemeCommitId;

        $.ajax({
            type: 'GET',
            url: url,
            dataType: 'json',
            complete: pageSpinner.hide,
            success: DD.lock._onSaveScheme
        });

    },
    _onSaveScheme: function (result) {
        if (result.lockStatusId == 3) {
            alert('The scheme has been recently updated, please firstly load the latest version to get around overwriting others commit');
        }
        else if (result.lockStatusId == 2) {  // mohu uložit kdyz lockedUserid == muj id
            saveDialog.dialog("open");

        }
        else if (result.lockStatusId == 1) {
            var msg = 'The scheme is currently locked by user ' + result.lockedForUserName + ',but u can press Lock button to Force Locking and overwrite his/her work';
            alert(msg);
        }
        else {
            //if scheme is not locked, we will lock the scheme for user and show the saving dialog(Update for convenience)
            DD.lock._lock();
            saveDialog.dialog("open");
        }

    },
    _afterSave: function (commitId) {
        DD.lock.CurrentSchemeCommitId = commitId;
    },

    _lock: function () {
        var appId = $('#currentAppId').val();
        var userId = $('#currentUserId').val();

        var url = "/api/database/apps/" + appId + "/LockScheme/" + userId;

        $.ajax({
            type: 'GET',
            url: url,
            dataType: 'json',
            complete: pageSpinner.hide,
            success: DD.lock._onLock
        });
    }
};

DD.onInit.push(DD.lock.init);