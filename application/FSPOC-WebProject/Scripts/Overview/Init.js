var ZoomFactor = 1.0;
$(function () {
    if (CurrentModuleIs("overviewModule")) {
        LoadMetablock();

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
            LoadMetablock();
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
                if (key == "delete") {
                    instance.removeAllEndpoints(options.$trigger, true);
                    options.$trigger.remove();
                }
                else if (key == "initial") {
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
                }
            },
            items: {
                "initial": { name: "Set as initial", icon: "edit" },
                "delete": { name: "Delete", icon: "delete" }
            }
        });
    }
});
