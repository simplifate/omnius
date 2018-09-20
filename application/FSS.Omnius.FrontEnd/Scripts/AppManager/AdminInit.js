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
        $(".adminAppTable .actions .btnValidate, .adminAppTable .actions .btnRebuild").on("click", function () {
            CurrentAppId = $(this).parents("tr").attr("appId");

            if (typeof WebSocket === "undefined") {
                ShowAppNotification("Your browser does not support WebSockets, cannot continue!", "error");
                return;
            }

            appBuildDialog.dialog("option", { title: "Actualization " + $(this).parents("tr").data("displayName") + " in progress "}).empty().dialog("open");
            var messagesById = {};
            
            var ws = new WebSocket('ws://' + window.location.hostname + ':' + window.location.port + '/Master/AppAdminManager/' + ($(this).hasClass('btnValidate') ? 'BuildApp' : 'RebuildApp') +'/' + CurrentAppId);
            currentWs = ws;
            //var timeLast = Date.now();
            ws.onerror = function () {
                $(document).trigger("ajaxError", {})
            }
            ws.onmessage = function (event) {
                if (currentWs !== ws) return;
                var response = JSON.parse(event.data);

                /// create
                // section
                if (!appBuildDialog.find('#buildDialog_' + response.section + '_').length) {
                    appBuildDialog.append('<div id="buildDialog_' + response.section + '_" class="app-alert app-alert-' + response.type + '"><span></span></div>');
                }
                // subsection
                if (!appBuildDialog.find('#buildDialog_' + response.section + '_' + response.subSection).length) {
                    appBuildDialog.find('#buildDialog_' + response.section + '_').append('<div id="buildDialog_' + response.section + '_' + response.subSection + '" class="app-alert app-alert-odd"><span></span></div>');
                }

                /// message
                var section = appBuildDialog.find('#buildDialog_' + response.section + '_' + response.subSection);
                section.find('> span').html(response.message);
                section.removeClass('app-alert-success app-alert-info app-alert-warning app-alert-error app-alert-inprogress').addClass('app-alert-' + response.type);

                /// scroll
                document.location.href = '#buildDialog_' + response.section + '_';
            };
            ws.onclose = function (event) {
                if (event.code === 1006) {
                    alert('unknown error');
                }
            }
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
        $('.btn-export').on("click", function () {
            CurrentExportUrl = this.href;
            exportAppDialog.dialog('open');

            return false;
        });
    }
});
