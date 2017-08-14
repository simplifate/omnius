DD.lock = {

    appId: null, //current app id
    currentUserId: null, //current user id
    currentUserName:null, //currentUserName
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

    _lockScheme: function(result){
        if (result.lockStatusId == 0) //if scheme is not locked
        {
            pageSpinner.show();
            var appId = self.appId;
            var userId = self.currentUserId;

            var url = "/api/database/apps/" + appId + "/LockScheme/" + userId  ;

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
            alert('Applicaton scheme has been locked by ' + result.lockedForUserName + ' ,please wait untill this user unlocks it');

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
            alert('The scheme is currently locked by user ' + result.lockedForUserName + ',please wait until this user unlock it');
        }
        else {
            alert('The scheme is not locked,please lock the scheme before saving it');
        }

    }
  
};

DD.onInit.push(DD.lock.init);