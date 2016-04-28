var GridResolution = 0;
$(function () {
    if (CurrentModuleIs("mozaicEditorModule")) {
        RecalculateMozaicToolboxHeight();
        pageId = $("#currentPageId").val();
        if (pageId)
            LoadMozaicPage(pageId);

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
            pageId = $("#currentPageId").val();
            if (!pageId) {
                SaveRequested = true;
                newPageDialog.dialog("open");
            }
            else
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
        CreateDroppableMozaicContainer($("#mozaicPageContainer"), true);
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
            var leftBar = $("#mozaicLeftBar");
            var scrollTop = $(window).scrollTop();
            var lowerPanelTop = $("#lowerPanel").offset().top;
            var overlay = $("#lowerPanelSpinnerOverlay");

            leftBar.css({ top: 0 });
            overlay.css({ top: 0, right: 0, width: 'auto' });
            if (scrollTop > lowerPanelTop) {
                leftBar.css({ left: 225, position: "fixed" });
                overlay.css({ left: 225, position: "fixed" });
            } else {
                leftBar.css({ left: 0, position: "absolute" });
                overlay.css({ left: 0, position: "absolute" });
            }
            RecalculateMozaicToolboxHeight();
        });
        $(window).resize(function () {
            RecalculateMozaicToolboxHeight();
        });
    } else {
        $(window).on("scroll resize", function () {
            var scrollTop = $(window).scrollTop();
            var upperPanelBottom = $("#upperPanel").offset().top + $("#upperPanel").height();
            var overlay = $("#lowerPanelSpinnerOverlay");
            overlay.css({ left: 225, top: 0, right: 0, width: "auto" });
            if (scrollTop > upperPanelBottom) {
                overlay.css({ top: 0, position: "fixed" });
                overlay.css({ height: window.innerHeight });
            } else {
                overlay.css({ top: upperPanelBottom + 1, position: "absolute" });
                overlay.css({ height: window.innerHeight - upperPanelBottom + scrollTop - 20 });
            }
        })
    }
});
