var GridResolution = 0;
$(function () {
    if (CurrentModuleIs("mozaicEditorModule")) {
        RecalculateMozaicToolboxHeight();

        $("#btnNewPage").on("click", function () {
            newPageDialog.dialog("open");
        });
        $("#btnChoosePage").on("click", function () {
            choosePageDialog.dialog("open");
        });
        $("#btnClear").on("click", function () {
            $("#mozaicPageContainer .uic").remove();
            $("#mozaicPageContainer .dataTables_wrapper").remove();
            $("#mozaicPageContainer .color-picker").remove();
        });
        $("#btnSave").on("click", function () {
            SaveMozaicPage();
        });
        $("#btnLoad").on("click", function () {
            LoadMozaicPage("current");
        });

        $("#hideMozaicTooboxIcon").on("click", function () {
            $("#mozaicLeftBar").hide();
            $("#mozaicLeftBarMinimized").show();
            $("#mozaicPageContainer").css("left", 32);
            RecalculateMozaicToolboxHeight();
        });
        $("#showMozaicTooboxIcon").on("click", function () {
            $("#mozaicLeftBar").show();
            $("#mozaicLeftBarMinimized").hide();
            $("#mozaicPageContainer").css("left", 300);
            RecalculateMozaicToolboxHeight();
        });
        $("#gridShowCheckbox").prop("checked", false);
        $("#gridResolutionDropdown").val("off");
        $("#gridShowCheckbox").on("change", function () {
            if ($(this).is(":checked")) {
                grid = $("#gridResolutionDropdown").val();
                if (grid != "off") {
                    resolutionValue = parseInt(grid);
                    $("#mozaicPageContainer").addClass("showGrid");
                    $("#mozaicPageContainer").css("background-size", resolutionValue);
                }
            }
            else {
                $("#mozaicPageContainer").removeClass("showGrid");
            }
        });
        if ($("#currentPageIsModal").is(":checked")) {
            $("#modalSizeVisualization").css("width", parseInt($("#modalWidthInput").val()));
            $("#modalSizeVisualization").css("height", parseInt($("#modalHeightInput").val()));
            $("#modalSizeVisualization").show();
        }
        $("#currentPageIsModal").on("change", function () {
            if ($(this).is(":checked")) {
                $("#modalSizeVisualization").css("width", parseInt($("#modalWidthInput").val()));
                $("#modalSizeVisualization").css("height", parseInt($("#modalHeightInput").val()));
                $("#modalSizeVisualization").show();
            }
            else {
                $("#modalSizeVisualization").hide();
            }
        });
        $("#modalWidthInput").on("change", function () {
            $("#modalSizeVisualization").css("width", parseInt($("#modalWidthInput").val()));
        });
        $("#modalHeightInput").on("change", function () {
            $("#modalSizeVisualization").css("height", parseInt($("#modalHeightInput").val()));
        });
        $("#gridResolutionDropdown").on("change", function () {
            grid = $(this).val();
            if (grid == "off") {
                $("#mozaicPageContainer").removeClass("showGrid");
                $("#gridShowCheckbox").prop("checked", false);
                GridResolution = 0;
            }
            else {
                resolutionValue = parseInt(grid);
                GridResolution = resolutionValue;
                if ($("#gridShowCheckbox").is(":checked")) {
                    $("#mozaicPageContainer").addClass("showGrid");
                    $("#mozaicPageContainer").css("background-size", resolutionValue);
                }
            }
        });
        $("#mozaicContainer button").off("click");
        $("#mozaicLeftBar .toolboxItem").draggable({
            helper: "clone",
            appendTo: '#mozaicPageContainer',
            containment: 'window',
            revert: true,
            scroll: true,
            cancel: false
        });
        $("#mozaicPageContainer").droppable({
            containment: "parent",
            tolerance: "fit",
            accept: ".toolboxItem",
            greedy: true,
            drop: function (e, ui) {
                droppedElement = ui.helper.clone();
                droppedElement.removeClass("toolboxItem");
                droppedElement.removeClass("ui-draggable-dragging");
                droppedElement.addClass("uic");
                if (!droppedElement.hasClass("radio-control"))
                    droppedElement.attr("uicName", "");
                droppedElement.attr("uicStyles", "");
                droppedElement.attr("placeholder", "");
                $(this).append(droppedElement);
                if (droppedElement.hasClass("breadcrumb-navigation")) {
                    droppedElement.css("width", "600px");
                }
                else if (droppedElement.hasClass("data-table")) {
                    CreateCzechDataTable(droppedElement);
                    droppedElement.css("width", "1000px");
                    wrapper = droppedElement.parents(".dataTables_wrapper");
                    wrapper.css("position", "absolute");
                    wrapper.css("left", droppedElement.css("left"));
                    wrapper.css("top", droppedElement.css("top"));
                    droppedElement.css("position", "relative");
                    droppedElement.css("left", "0px");
                    droppedElement.css("top", "0px");
                }
                else if (droppedElement.hasClass("color-picker")) {
                    droppedElement.val("#f00");
                    CreateColorPicker(droppedElement);
                    newReplacer = $("#mozaicPageContainer .sp-replacer:last");
                    newReplacer.css("position", "absolute");
                    newReplacer.css("left", droppedElement.css("left"));
                    newReplacer.css("top", droppedElement.css("top"));
                    droppedElement.removeClass("uic");
                    newReplacer.addClass("uic color-picker");
                    newReplacer.attr("uicClasses", "color-picker");
                }
                if (GridResolution > 0) {
                    droppedElement.css("left", droppedElement.position().left - (droppedElement.position().left % GridResolution));
                    droppedElement.css("top", droppedElement.position().top - (droppedElement.position().top % GridResolution));
                }
                ui.helper.remove();
                if (droppedElement.hasClass("data-table"))
                    wrapper.draggable({
                        cancel: false,
                        containment: "parent",
                        drag: function (event, ui) {
                            if (GridResolution > 0) {
                                ui.position.left -= (ui.position.left % GridResolution);
                                ui.position.top -= (ui.position.top % GridResolution);
                            }
                        }
                    });
                else if (droppedElement.hasClass("color-picker"))
                    newReplacer.draggable({
                        cancel: false,
                        containment: "parent",
                        drag: function (event, ui) {
                            if (GridResolution > 0) {
                                ui.position.left -= (ui.position.left % GridResolution);
                                ui.position.top -= (ui.position.top % GridResolution);
                            }
                        }
                    });
                else
                    droppedElement.draggable({
                        cancel: false,
                        containment: "parent",
                        drag: function (event, ui) {
                            if (GridResolution > 0) {
                                ui.position.left -= (ui.position.left % GridResolution);
                                ui.position.top -= (ui.position.top % GridResolution);
                            }
                        }
                    });
            }
        });
        $("#mozaicPageContainer .uic").draggable({
            cancel: false,
            containment: "parent",
            drag: function (event, ui) {
                if (GridResolution > 0) {
                    ui.position.left -= (ui.position.left % GridResolution);
                    ui.position.top -= (ui.position.top % GridResolution);
                }
            }
        });
        $.contextMenu({
            selector: '.uic',
            trigger: 'right',
            zIndex: 300,
            callback: function (key, options) {
                item = options.$trigger;
                if (key == "delete") {
                    if (item.hasClass("data-table"))
                        item.parents(".dataTables_wrapper").remove();
                    else
                        item.remove();
                }
                else if (key == "properties") {
                    CurrentComponent = item;
                    componentPropertiesDialog.dialog("open");
                }
            },
            items: {
                "properties": { name: "Properties", icon: "edit" },
                "delete": { name: "Delete", icon: "delete" }
            }
        });
        $(window).scroll(function () {
            leftBar = $("#mozaicLeftBar");
            scrollTop = $(window).scrollTop();
            lowerPanelTop = $("#lowerPanel").offset().top;
            if (scrollTop > lowerPanelTop)
                leftBar.css("top", scrollTop - lowerPanelTop);
            else
                leftBar.css("top", 0);
            RecalculateMozaicToolboxHeight();
        });
        $(window).resize(function () {
            RecalculateMozaicToolboxHeight();
        });
    }
});
