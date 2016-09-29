$(function () {
    if (CurrentModuleIs("adminAppModule")) {
        $(".adminAppTable .btnOpenWorkflow").on("click", function () {
            CurrentAppId = $(this).parents("tr").attr("appId");
            openMetablockForm = $("#openMetablockForm");
            openMetablockForm.find("input[name='appId']").val(CurrentAppId);
            openMetablockForm.submit();
        });
        $(".adminAppTable .btnOpenDbScheme").on("click", function () {
            CurrentAppId = $(this).parents("tr").attr("appId");
            openMetablockForm = $("#openDbSchemeForm");
            openMetablockForm.find("input[name='appId']").val(CurrentAppId);
            openMetablockForm.submit();
        });
        $(".adminAppTable .btnOpenMozaic").on("click", function () {
            CurrentAppId = $(this).parents("tr").attr("appId");
            openMetablockForm = $("#openMozaicForm");
            openMetablockForm.find("input[name='appId']").val(CurrentAppId);
            openMetablockForm.submit();
        });

        var currentWs;
        $(".adminAppTable .actions .btnValidate").on("click", function () {
            CurrentAppId = $(this).parents("tr").attr("appId");

            if (typeof WebSocket === "undefined") {
                ShowAppNotification("Váš prohlížeč nepodporuje webSockety, a nemůže být využit k aktualizaci aplikací", "error");
                return;
            }

            appBuildDialog.dialog("option", { title: "aktualizuji " + $(this).parents("tr").data("displayName") }).empty().dialog("open");
            var messagesById = {};

            var ws = new WebSocket('ws://' + window.location.hostname + ':' + window.location.port + '/Master/AppAdminManager/BuildApp/' + CurrentAppId);
            currentWs = ws;
            //var timeLast = Date.now();
            ws.onerror = function () {
                $(document).trigger("ajaxError", {})
            }
            ws.onmessage = function (event) {
                if (currentWs !== ws) return;

                //console.log(Date.now() - timeLast, event.data);
                //timeLast = Date.now();

                var response;
                try{
                    response = JSON.parse(event.data);
                } catch(e) {
                    response = { message: event.data, type: "error" };
                }

                var $message;
                if (response.id && messagesById[response.id]) {
                    $message = messagesById[response.id];
                } else {
                    var $parent = response.childOf ? messagesById[response.childOf] : appBuildDialog;
                    $message = $("<div class='app-alert'><span>").data("messageId", response.id).appendTo($parent);
                    if (!$parent.is("#app-build-dialog, .app-alert-odd")) $message.addClass("app-alert-odd");
                    if (response.id) messagesById[response.id] = $message;
                }

                if (response.message) $message.children("span").html(response.message);

                $message.removeClass("app-alert-info app-alert-error app-alert-success app-alert-warning").addClass("app-alert-" + (response.type || "info"));

                if (response.abort) $message.nextAll().remove();

                if (response.done) {
                    setTimeout(function () { appBuildDialog.dialog("close") }, 1000);
                }

                var childrenHeight = 0;
                appBuildDialog.children().each(function () {
                    childrenHeight += $(this).outerHeight();
                });
                appBuildDialog.css({ height: childrenHeight + 32 });
            };
        });
        $(".adminAppTable .actions .btnProperties").on("click", function () {
            CurrentAppId = $(this).parents("tr").attr("appId");
            appPropertiesDialog.dialog("open");
        });
        $(".adminAppTable .actions .btnToggleEnabled").on("click", function () {
            thisButton = $(this);
            appId = thisButton.parents("tr").attr("appId");
            isEnabledCell = thisButton.parents("tr").find(".isEnabledColumn");
            if (thisButton.hasClass("btnDisable")) {
                postData = {
                    IsEnabled: false
                }
                $.ajax({
                    type: "POST",
                    url: "/api/master/apps/" + appId + "/state",
                    data: postData,
                    success: function () {
                        thisButton.removeClass("btnDisable");
                        thisButton.addClass("btnEnable");
                        thisButton.removeClass("btn-danger");
                        thisButton.addClass("btn-primary");
                        thisButton.text("Povolit");
                        isEnabledCell.text("Ne");
                    }
                });
            }
            else {
                postData = {
                    IsEnabled: true
                }
                $.ajax({
                    type: "POST",
                    url: "/api/master/apps/" + appId + "/state",
                    data: postData,
                    success: function () {
                        thisButton.removeClass("btnEnable");
                        thisButton.addClass("btnDisable");
                        thisButton.removeClass("btn-primary");
                        thisButton.addClass("btn-danger");
                        thisButton.text("Zakázat");
                        isEnabledCell.text("Ano");
                    }
                });
            }
        });
        $("#btnAddApp").on("click", function () {
            addAppDialog.dialog("open");
        });
    }
});

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
            appPropertiesDialog.find("#template").val(1);
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
                    appPropertiesDialog.find("#template").val(data.CSSTemplateId);
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
            CSSTemplateId: appPropertiesDialog.find("#template").val(),
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
            addAppDialog.find("#template").val(1);
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
            Template: addAppDialog.find("#template").val(),
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