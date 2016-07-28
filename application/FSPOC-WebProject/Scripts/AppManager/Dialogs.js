var CurrentAppId;

$(function () {
    appPropertiesDialog = $("#app-properties-dialog").dialog({
        autoOpen: false,
        resizable: false,
        width: 600,
        height: 320,
        buttons: {
            "Uložit": function () {
                appPropertiesDialog_SubmitData();
            },
            "Zrušit": function () {
                appPropertiesDialog.dialog("close");
            }
        },
        create: function () {
            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    appPropertiesDialog_SubmitData();
                    return false;
                }
            });
        },
        open: function () {
            appPropertiesDialog.find("#app-name").val("");
            appPropertiesDialog.find("#tile-width").val(2);
            appPropertiesDialog.find("#tile-height").val(1);
            appPropertiesDialog.find("#bg-color").val(0);
            appPropertiesDialog.find("#icon-class").val("");
            $.ajax({
                type: "GET",
                url: "/api/master/apps/" + CurrentAppId + "/properties",
                dataType: "json",
                success: function (data) {
                    appPropertiesDialog.find("#app-name").val(data.DisplayName);
                    appPropertiesDialog.find("#tile-width").val(data.TileWidth);
                    appPropertiesDialog.find("#tile-height").val(data.TileHeight);
                    appPropertiesDialog.find("#bg-color").val(data.Color);
                    appPropertiesDialog.find("#icon-class").val(data.Icon);
                }
            });
        }
    });
    function appPropertiesDialog_SubmitData() {
        appPropertiesDialog.dialog("close");
        postData = {
            DisplayName: appPropertiesDialog.find("#app-name").val(),
            TileWidth: appPropertiesDialog.find("#tile-width").val(),
            TileHeight: appPropertiesDialog.find("#tile-height").val(),
            Color: appPropertiesDialog.find("#bg-color").val(),
            Icon: appPropertiesDialog.find("#icon-class").val()
        }
        $.ajax({
            type: "POST",
            url: "/api/master/apps/" + CurrentAppId + "/properties",
            data: postData,
            success: function() {
                alert("OK");
                // Reload page to change application name in AppManager table
                // after application name was changed
                location.reload();
            }
        });
    }
    addAppDialog = $("#new-app-dialog").dialog({
        autoOpen: false,
        resizable: false,
        width: 600,
        height: 320,
        buttons: {
            "Přidat": function () {
                addAppDialog_SubmitData();
            },
            "Zrušit": function () {
                addAppDialog.dialog("close");
            }
        },
        create: function () {
            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    addAppDialog_SubmitData();
                    return false;
                }
            });
        },
        open: function () {
            addAppDialog.find("#app-name").val("");
            addAppDialog.find("#tile-width").val(2);
            addAppDialog.find("#tile-height").val(1);
            addAppDialog.find("#bg-color").val(0);
            addAppDialog.find("#icon-class").val("fa-question");
        }
    });
    function addAppDialog_SubmitData() {
        addAppDialog.dialog("close");
        postData = {
            DisplayName: addAppDialog.find("#app-name").val(),
            TileWidth: addAppDialog.find("#tile-width").val(),
            TileHeight: addAppDialog.find("#tile-height").val(),
            Color: addAppDialog.find("#bg-color").val(),
            Icon: addAppDialog.find("#icon-class").val()
        }
        $.ajax({
            type: "POST",
            url: "/api/master/apps",
            data: postData,
            success: function () {
                location.reload();
            }
        });
    }
    appBuildDialog = $("#app-build-dialog").dialog({
        autoOpen: false,
        resizable: false,
        width: 640,
        height: 320
    });
});
