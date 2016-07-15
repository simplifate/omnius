function RecalculateMozaicToolboxHeight() {
    var leftBar = $("#mozaicLeftBar");
    var leftBarMinimized = $("#mozaicLeftBarMinimized");
    var scrollTop = $(window).scrollTop();
    var lowerPanelTop = $("#lowerPanel").offset().top;
    var topBarHeight = $("#topBar").height() + $("#appNotificationArea").height();
    var bottomPanelHeight;
    if (scrollTop > lowerPanelTop - topBarHeight) {
        bottomPanelHeight = window.innerHeight - topBarHeight;
    } else {
        bottomPanelHeight = $(window).height() + scrollTop - lowerPanelTop - leftBar.position().top;
    }
    leftBar.height(bottomPanelHeight);
    $("#lowerPanelSpinnerOverlay").height(bottomPanelHeight);
    leftBarMinimized.height($(window).height() + scrollTop - lowerPanelTop - leftBarMinimized.position().top);
}
function CreateDroppableMozaicContainer(target, allowNesting) {
    target.droppable({
        containment: "parent",
        tolerance: "fit",
        accept: ".toolboxItem",
        greedy: true,
        accept: function (element) {
            if (!element.hasClass("toolboxItem") || (element.hasClass("panel-component") && !allowNesting))
                return false;
            else return true;
        },
        drop: function (e, ui) {
            droppedElement = ui.helper.clone();
            droppedElement.removeClass("toolboxItem");
            droppedElement.removeClass("ui-draggable-dragging");
            droppedElement.addClass("uic");
            var newDraggable = droppedElement;
            if (!droppedElement.hasClass("radio-control"))
                droppedElement.attr("uicName", "");
            droppedElement.attr("uicStyles", "");
            droppedElement.attr("placeholder", "");
            thisContainer = $(this);
            thisContainer.append(droppedElement);
            if (thisContainer.hasClass("panel-component")) {
                droppedElement.css("left", parseInt(droppedElement.css("left")) - parseInt(thisContainer.css("left")));
                droppedElement.css("top", parseInt(droppedElement.css("top")) - parseInt(thisContainer.css("top")));
            }
            if (droppedElement.hasClass("breadcrumb-navigation")) {
                droppedElement.css("width", "600px");
            }
            else if (droppedElement.hasClass("data-table")) {
                CreateCzechDataTable(droppedElement, droppedElement.hasClass("data-table-simple-mode"));
                droppedElement.css("width", "1000px");
                wrapper = droppedElement.parents(".dataTables_wrapper");
                newDraggable = wrapper;
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
                newReplacer = target.find(".sp-replacer:last");
                newDraggable = newReplacer;
                newReplacer.css("position", "absolute");
                newReplacer.css("left", droppedElement.css("left"));
                newReplacer.css("top", droppedElement.css("top"));
                droppedElement.removeClass("uic");
                newReplacer.addClass("uic color-picker");
                newReplacer.attr("uicClasses", "color-picker");
            }
            else if (droppedElement.hasClass("wizard-phases")) {
                droppedElement.css("width", "");
            }
            else if(droppedElement.hasClass("bootstrap-row")) {
                droppedElement.css({left: 20, right: 20, width: "auto"});
            }
            else if (droppedElement.hasClass("panel-component")) {
                droppedElement.css("width", 500);
                droppedElement.css("height", 120);
                CreateDroppableMozaicContainer(droppedElement, false);
            }
            if (GridResolution > 0) {
                newDraggable.css("left", Math.round(newDraggable.position().left / GridResolution) * GridResolution);
                newDraggable.css("top", Math.round(newDraggable.position().top / GridResolution) * GridResolution);
            }
            ui.helper.remove();
            newDraggable.draggable({
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
};
WizardPhasesContentTemplate = '<div class="wizard-phases-frame"></div><svg class="phase-background" width="846px" height="84px"><defs>' +
'<linearGradient id="grad-light" x1="0%" y1="0%" x2="0%" y2="100%"><stop offset="0%" style="stop-color:#dceffa ;stop-opacity:1" />' +
'<stop offset="100%" style="stop-color:#8dceed;stop-opacity:1" /></linearGradient><linearGradient id="grad-blue" x1="0%" y1="0%" x2="0%" y2="100%">' +
'<stop offset="0%" style="stop-color:#0099cc;stop-opacity:1" /><stop offset="100%" style="stop-color:#0066aa;stop-opacity:1" />' +
'</linearGradient></defs><path d="M0 0 L0 88 L 280 88 L324 44 L280 0 Z" fill="url(#grad-blue)" /><path d="M280 88 L324 44 L280 0 L560 0 L604 44 L560 88 Z" fill="url(#grad-light)" />' +
'<path d="M560 0 L604 44 L560 88 L850 88 L850 0 Z" fill="url(#grad-light)" /></svg><div class="phase phase1 phase-active"><div class="phase-icon-circle">' +
'<div class="phase-icon-number">1</div></div><div class="phase-label">Fáze 1</div></div><div class="phase phase2"><div class="phase-icon-circle">' +
'<div class="phase-icon-number">2</div></div><div class="phase-label">Fáze 2</div></div><div class="phase phase3"><div class="phase-icon-circle">' +
'<div class="phase-icon-number">3</div></div><div class="phase-label">Fáze 3</div></div>';
