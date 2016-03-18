var ZoomFactor = 1.0;
var currentBlock, currentMetablock;
var ChangedSinceLastSave = false;

$(function () {
    if (CurrentModuleIs("overviewModule")) {
        $("#headerMetablockName").on("click", function () {
            renameMetablockDialog.dialog("open");
        });
        $("#btnAddBlock").on("click", function () {
            addBlockDialog.dialog("open");
        });
        $("#btnAddMetablock").on("click", function () {
            addMetablockDialog.dialog("open");
        });
        $("#btnLoad").on("click", function () {
            if (ChangedSinceLastSave)
                confirmed = confirm("Máte neuložené změny, opravdu si přejete tyto změny zahodit?");
            else
                confirmed = true;
            if (confirmed) {
                LoadMetablock();
            }
        });
        $("#btnMenuOrder").on("click", function () {
            location.href = "/Tapestry/Overview/MenuOrder/" + $('#currentMetablockId').val();
        });
        $("#btnSave").on("click", function () {
            SaveMetablock();
        });
        $("#btnClear").on("click", function () {
            $("#overviewPanel .block, #overviewPanel .metablock").each(function (index, element) {
                instance.removeAllEndpoints(element, true);
                $(element).remove();
            });
        });
        $("#btnGoUp").on("click", function () {
            SaveMetablock(function () {
                openMetablockForm = $("#openMetablockForm");
                openMetablockForm.find("input[name='metablockId']").val($("#parentMetablockId").val());
                openMetablockForm.submit();
            });
        });
        window.onbeforeunload = function () {
            if (ChangedSinceLastSave)
                SaveMetablock();
            return null;
        };
        $("#btnZoomIn").on("click", function () {
            ZoomFactor += 0.1;
            $("#overviewPanel .scrollArea").css("transform", "scale(" + ZoomFactor + ")");
            $("#zoomLabel").text("Zoom " + Math.floor(ZoomFactor * 100) + "%");
        });
        $("#btnZoomOut").on("click", function () {
            if (ZoomFactor >= 0.2)
                ZoomFactor -= 0.1;
            $("#overviewPanel .scrollArea").css("transform", "scale(" + ZoomFactor + ")");
            $("#zoomLabel").text("Zoom " + Math.floor(ZoomFactor * 100) + "%");
        });
        $.contextMenu({
            selector: '.block, .metablock',
            trigger: 'right',
            zIndex: 300,
            callback: function (key, options) {
                switch(key)
                {
                    case "delete": {
                        instance.removeAllEndpoints(options.$trigger, true);
                        options.$trigger.remove();
                        ChangedSinceLastSave = true;
                        break;
                    }
                    case "initial": {
                        if (options.$trigger.hasClass("metablock")) {
                            $("#overviewPanel .metablock").each(function (index, element) {
                                $(element).attr("isInitial", false);
                                $(element).find(".metablockInfo").text("");
                            });
                            options.$trigger.attr("isInitial", true);
                            options.$trigger.find(".metablockInfo").text("Initial");
                        }
                        else {
                            $("#overviewPanel .block").each(function (index, element) {
                                $(element).attr("isInitial", false);
                                $(element).find(".blockInfo").text("");
                            });
                            options.$trigger.attr("isInitial", true);
                            options.$trigger.find(".blockInfo").text("Initial");
                        }
                        ChangedSinceLastSave = true;
                        break;
                    }
                    case "properties": {
                        if (options.$trigger.hasClass("metablock")) {
                            currentMetablock = options.$trigger;
                            metablockPropertiesDialog.dialog("open");
                        }
                        else {
                            currentBlock = options.$trigger;
                            blockPropertiesDialog.dialog("open");
                        }
                        break;
                    }
                }
            },
            items: {
                "properties": { name: "Properties", icon: "cog" },
                "initial": { name: "Set as initial", icon: "edit" },
                "delete": { name: "Delete", icon: "delete" }
            }
        });
    }
});
