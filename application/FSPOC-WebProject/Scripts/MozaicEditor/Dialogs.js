var CurrentComponent, SaveRequested = false;
$(function () {
    if (CurrentModuleIs("mozaicEditorModule")) {
        componentPropertiesDialog = $("#component-properties-dialog").dialog({
            autoOpen: false,
            width: 700,
            height: 400,
            buttons: {
                "Save": function () {
                    componentPropertiesDialog_SubmitData();
                },
                Cancel: function () {
                    componentPropertiesDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        componentPropertiesDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                componentPropertiesDialog.find("#component-name").val(CurrentComponent.attr("uicName"));
                componentPropertiesDialog.find("#component-width").val(CurrentComponent.css("width"));
                componentPropertiesDialog.find("#component-height").val(CurrentComponent.css("height"));            
                componentPropertiesDialog.find("#component-styles").val(CurrentComponent.attr("uicStyles"));
                componentPropertiesDialog.find("#component-props").val(CurrentComponent.attr("uicProperties"));
                componentPropertiesDialog.find("#component-tabindex").val(CurrentComponent.attr("tabindex"));
                if (CurrentComponent.hasClass("input-single-line") || CurrentComponent.hasClass("input-multiline"))
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.attr("placeholder"));
                else if (CurrentComponent.hasClass("info-container")) {
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.find(".info-container-header").text());
                    componentPropertiesDialog.find("#component-content").val(CurrentComponent.find(".info-container-body").text());
                }
                else if (CurrentComponent.hasClass("form-heading") || CurrentComponent.hasClass("control-label")) {
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.html());
                    componentPropertiesDialog.find("#component-content").val(CurrentComponent.attr("contentTemplate"));
                }
                else if (CurrentComponent.hasClass("checkbox-control")) {
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.find(".checkbox-label").text());
                    componentPropertiesDialog.find("#component-content").val("");
                }
                else if (CurrentComponent.hasClass("radio-control")) {
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.find(".radio-label").text());
                    componentPropertiesDialog.find("#component-content").val("");
                }
                else if (CurrentComponent.hasClass("tab-navigation")) {
                    componentPropertiesDialog.find("#component-label").val("");
                    tabString = "";
                    CurrentComponent.find("li").each(function (index, element) {
                        if (index > 0)
                            tabString += $(element).find("a").text() + ";";
                    });
                    componentPropertiesDialog.find("#component-content").val(tabString);
                }
                else if (CurrentComponent.hasClass("wizard-phases")) {
                    componentPropertiesDialog.find("#component-label").val("");
                    var phaseLabels = "";
                    CurrentComponent.find(".phase-label").each(function (index, element) {
                        phaseLabels += $(element).text() + ";";
                    });
                    phaseLabels = phaseLabels.slice(0, -1);
                    componentPropertiesDialog.find("#component-content").val(phaseLabels);
                }
                else if (CurrentComponent.hasClass("button-simple") || CurrentComponent.hasClass("button-dropdown")) {
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.text());
                }
                else {
                    componentPropertiesDialog.find("#component-label").val("");
                    componentPropertiesDialog.find("#component-content").val("");
                }
            }
        });
        function componentPropertiesDialog_SubmitData() {
            CurrentComponent.attr("uicName", componentPropertiesDialog.find("#component-name").val());
            CurrentComponent.css("width", componentPropertiesDialog.find("#component-width").val());
            CurrentComponent.css("height", componentPropertiesDialog.find("#component-height").val());
            CurrentComponent.attr("uicStyles", componentPropertiesDialog.find("#component-styles").val());
            CurrentComponent.attr("uicProperties", componentPropertiesDialog.find("#component-props").val());
            CurrentComponent.attr("tabindex", componentPropertiesDialog.find("#component-tabindex").val());
            if (CurrentComponent.hasClass("button-simple"))
                CurrentComponent.text(componentPropertiesDialog.find("#component-label").val());
            else if (CurrentComponent.hasClass("button-dropdown"))
                CurrentComponent.html(componentPropertiesDialog.find("#component-label").val() + '<i class="fa fa-caret-down">');
            else if (CurrentComponent.hasClass("input-single-line") || CurrentComponent.hasClass("input-multiline"))
                CurrentComponent.attr("placeholder", componentPropertiesDialog.find("#component-label").val());
            else if (CurrentComponent.hasClass("info-container")) {
                CurrentComponent.find(".info-container-header").text(componentPropertiesDialog.find("#component-label").val());
            }
            else if (CurrentComponent.hasClass("form-heading") || CurrentComponent.hasClass("control-label")) {
                CurrentComponent.html(componentPropertiesDialog.find("#component-label").val());
                CurrentComponent.attr("contentTemplate", componentPropertiesDialog.find("#component-content").val());
            }
            else if (CurrentComponent.hasClass("checkbox-control")) {
                CurrentComponent.find(".checkbox-label").text(componentPropertiesDialog.find("#component-label").val());
                CurrentComponent.css("width", "auto");
            }
            else if (CurrentComponent.hasClass("radio-control")) {
                CurrentComponent.find(".radio-label").text(componentPropertiesDialog.find("#component-label").val());
                CurrentComponent.find("input").attr("name", componentPropertiesDialog.find("#component-name").val());
                CurrentComponent.css("width", "auto");
            }
            else if (CurrentComponent.hasClass("tab-navigation")) {
                tabString = componentPropertiesDialog.find("#component-content").val();
                tabLabelArray = tabString.split(";");
                CurrentComponent.find("li").remove();
                CurrentComponent.append($('<li class="active"><a class="fa fa-home"></a></li>'));
                for (i = 0; i < tabLabelArray.length; i++) {
                    if (tabLabelArray[i].length > 0)
                        CurrentComponent.append($("<li><a>" + tabLabelArray[i] + "</a></li>"));
                }
                CurrentComponent.css("width", "auto");
            }
            else if (CurrentComponent.hasClass("wizard-phases")) {
                var phaseLabelArray = componentPropertiesDialog.find("#component-content").val().split(";");
                CurrentComponent.find(".phase1 .phase-label").text(phaseLabelArray[0] ? phaseLabelArray[0] : "Fáze 1");
                CurrentComponent.find(".phase2 .phase-label").text(phaseLabelArray[1] ? phaseLabelArray[1] : "Fáze 2");
                CurrentComponent.find(".phase3 .phase-label").text(phaseLabelArray[2] ? phaseLabelArray[2] : "Fáze 3");
            }
            componentPropertiesDialog.dialog("close");
        }
        renamePageDialog = $("#rename-page-dialog").dialog({
            autoOpen: false,
            width: 400,
            height: 190,
            buttons: {
                "Save": function () {
                    renamePageDialog_SubmitData();
                },
                Cancel: function () {
                    renamePageDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        renamePageDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                renamePageDialog.find("#page-name").val($("#headerPageName").text());
            }
        });
        function renamePageDialog_SubmitData() {
            renamePageDialog.dialog("close");
            $("#headerPageName").text(renamePageDialog.find("#page-name").val());
            ChangedSinceLastSave = true;
        }
        choosePageDialog = $("#choose-page-dialog").dialog({
            autoOpen: false,
            width: 700,
            height: 540,
            buttons: {
                "Open": function () {
                    historyDialog_SubmitData();
                },
                "Delete": function () {
                    if (confirm('Are you sure you want to delete this page?')) {
                        deleteMozaicPage_SubmitData();
                    }
                },
                Cancel: function () {
                    choosePageDialog.dialog("close");
                }
            },
            open: function (event, ui) {
                choosePageDialog.find("#page-table:first tbody tr").remove();
                $("#choose-page-dialog .spinner-2").show();
                choosePageDialog.data("selectedPageId", null);
                appId = $("#currentAppId").val();
                $.ajax({
                    type: "GET",
                    url: "/api/mozaic-editor/apps/" + appId + "/pages",
                    dataType: "json",
                    error: function (request, status, error) {
                        alert(request.responseText);
                    },
                    success: function (data) {
                        tbody = choosePageDialog.find("#page-table tbody:nth-child(2)");
                        for (i = 0; i < data.length; i++) {
                            tbody.append($('<tr class="pageRow" pageId="' + data[i].Id + '"><td>' + data[i].Name + '</td></tr>'));
                        }
                        $(document).on('click', 'tr.pageRow', function (event) {
                            choosePageDialog.find("#page-table tbody:nth-child(2) tr").removeClass("highlightedRow");
                            $(this).addClass("highlightedRow");
                            choosePageDialog.data("selectedPageId", $(this).attr("pageId"));
                        });
                        $("#choose-page-dialog .spinner-2").hide();
                    }
                });
            }
        });
        function historyDialog_SubmitData() {
            if (choosePageDialog.data("selectedPageId")) {
                choosePageDialog.dialog("close");
                LoadMozaicPage(choosePageDialog.data("selectedPageId"));
            }
            else
                alert("Please select a commit");
        }
        function deleteMozaicPage_SubmitData() {
            pageSpinner.show();
            appId = $("#currentAppId").val();
            pageId = choosePageDialog.data("selectedPageId");
            $.ajax({
                type: "POST",
                url: "/api/mozaic-editor/apps/" + appId + "/pages/" + pageId + "/delete",
                complete: function () {
                    pageSpinner.hide();
                },
                success: function () {
                    alert("OK. Page deleted.");
                    choosePageDialog.dialog("close");
                },
                error: function (request, status, error) {
                    alert(request.responseText);
                }
            });    
        }
        newPageDialog = $("#new-page-dialog").dialog({
            autoOpen: false,
            width: 400,
            height: 190,
            buttons: {
                "Save": function () {
                    newPageDialog_SubmitData();
                },
                Cancel: function () {
                    newPageDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        newPageDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                newPageDialog.find("#new-page-name").val("");
            }
        });
        function newPageDialog_SubmitData() {
            pageSpinner.show();
            newPageDialog.dialog("close");
            postData = {
                Name: newPageDialog.find("#new-page-name").val(),
                Components: []
            };
            appId = $("#currentAppId").val();
            $.ajax({
                type: "POST",
                url: "/api/mozaic-editor/apps/" + appId + "/pages",
                data: postData,
                error: function (request, status, error) {
                    alert(request.responseText);
                },
                success: function (data) {
                    $("#currentPageId").val(data);
                    $("#headerPageName").text(newPageDialog.find("#new-page-name").val());
                    if (SaveRequested) {
                        SaveMozaicPage();
                    }
                    else {
                        pageSpinner.hide();
                    }
                }
            });
        }
        trashPageDialog = $("#trash-page-dialog").dialog({
            autoOpen: false,
            width: 700,
            height: 540,
            buttons: {
                "Load": function () {
                    trashPageDialog_SubmitData();
                },
                Cancel: function () {
                    trashPageDialog.dialog("close");
                }
            },
            open: function (event, ui) {
                trashPageDialog.find("#trash-page-table:first tbody tr").remove();
                $("#trash-page-dialog .spinner-2").show();
                trashPageDialog.data("selectedPageId", null);
                appId = $("#currentAppId").val();
                $.ajax({
                    type: "GET",
                    url: "/api/mozaic-editor/apps/" + appId + "/deletedPages",
                    dataType: "json",
                    error: function (request, status, error) {
                        alert(request.responseText);
                    },
                    success: function (data) {
                        tbody = trashPageDialog.find("#trash-page-table tbody:nth-child(2)");
                        for (i = 0; i < data.length; i++) {
                            tbody.append($('<tr class="pageRow" pageId="' + data[i].Id + '"><td>' + data[i].Name + '</td></tr>'));
                        }
                        $(document).on('click', 'tr.pageRow', function (event) {
                            trashPageDialog.find("#trash-page-table tbody:nth-child(2) tr").removeClass("highlightedRow");
                            $(this).addClass("highlightedRow");
                            trashPageDialog.data("selectedPageId", $(this).attr("pageId"));
                        });
                        $("#trash-page-dialog .spinner-2").hide();
                    }
                });
            }
        });
        function trashPageDialog_SubmitData() {
            if (trashPageDialog.data("selectedPageId")) {
                trashPageDialog.dialog("close");
                LoadMozaicPage(trashPageDialog.data("selectedPageId"));
            }
            else
                alert("Please select a commit");
        }
    }
});
