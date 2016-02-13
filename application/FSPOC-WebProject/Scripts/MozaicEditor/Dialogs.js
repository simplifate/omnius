var CurrentComponent;
$(function () {
    if (CurrentModuleIs("mozaicEditorModule")) {
        componentPropertiesDialog = $("#component-properties-dialog").dialog({
            autoOpen: false,
            width: 400,
            height: 190,
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
                if (CurrentComponent.hasClass("input-single-line") || CurrentComponent.hasClass("input-multiline"))
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.attr("placeholder"));
                else if (CurrentComponent.hasClass("info-container")) {
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.find(".info-container-header").text());
                    componentPropertiesDialog.find("#component-body").val(CurrentComponent.find(".info-container-body").text());
                }
                else
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.text());
            }
        });
        function componentPropertiesDialog_SubmitData() {
            if (CurrentComponent.hasClass("button-simple"))
                CurrentComponent.text(componentPropertiesDialog.find("#component-label").val());
            else if (CurrentComponent.hasClass("button-dropdown"))
                CurrentComponent.html(componentPropertiesDialog.find("#component-label").val() + '<i class="fa fa-caret-down">');
            else if (CurrentComponent.hasClass("input-single-line") || CurrentComponent.hasClass("input-multiline"))
                CurrentComponent.attr("placeholder", componentPropertiesDialog.find("#component-label").val());
            else if (CurrentComponent.hasClass("info-container")) {
                CurrentComponent.find(".info-container-header").text(componentPropertiesDialog.find("#component-label").val());
                CurrentComponent.find(".info-container-body").text(componentPropertiesDialog.find("#component-body").val());
            }
            componentPropertiesDialog.dialog("close");
        }
        choosePageDialog = $("#choose-page-dialog").dialog({
            autoOpen: false,
            width: 700,
            height: 540,
            buttons: {
                "Open": function () {
                    historyDialog_SubmitData();
                },
                Cancel: function () {
                    choosePageDialog.dialog("close");
                }
            },
            open: function (event, ui) {
                choosePageDialog.data("selectedCommitId", null);
                appId = $("#currentAppId").val();
                $.ajax({
                    type: "GET",
                    url: "/api/mozaic-editor/apps/" + appId + "/pages",
                    dataType: "json",
                    error: function () { alert("Error loading page list") },
                    success: function (data) {
                        choosePageDialog.find("#page-table:first tbody:nth-child(2) tr").remove();
                        tbody = choosePageDialog.find("#page-table tbody:nth-child(2)");
                        for (i = 0; i < data.length; i++) {
                            tbody.append($('<tr class="pageRow" pageId="' + data[i].Id + '"><td>' + data[i].Name + '</td></tr>'));
                        }
                        $(document).on('click', 'tr.pageRow', function (event) {
                            choosePageDialog.find("#page-table tbody:nth-child(2) tr").removeClass("highlightedRow");
                            $(this).addClass("highlightedRow");
                            choosePageDialog.data("selectedCommitId", $(this).attr("pageId"));
                        });
                    }
                });
            }
        });
        function historyDialog_SubmitData() {
            if (choosePageDialog.data("selectedCommitId")) {
                choosePageDialog.dialog("close");
                LoadMozaicPage(choosePageDialog.data("selectedCommitId"));
            }
            else
                alert("Please select a commit");
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
                error: function () { alert("ERROR") },
                success: function (data) {
                    $("#currentPageId").val(data);
                    $("#headerPageName").text(newPageDialog.find("#new-page-name").val());
                    alert("OK");
                }
            });
        }
    }
});