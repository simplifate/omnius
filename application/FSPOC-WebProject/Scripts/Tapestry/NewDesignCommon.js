function RecalculateToolboxHeight() {
    leftBar = $("#tapestryLeftBar");
    scrollTop = $(window).scrollTop();
    lowerPanelTop = $("#lowerPanel").offset().top;
    leftBar.height($(window).height() + scrollTop - lowerPanelTop - leftBar.position().top);
};
$(function () {
    if (CurrentModuleIs("tapestryModule")) {
        RecalculateToolboxHeight();
        $(".toolboxCategoryHeader_Symbols").on("click", function () {
            $(".symbolToolboxSpace").slideToggle();
        });
        $(".toolboxCategoryHeader_Actions").on("click", function () {
            $(".toolboxLi_Actions").slideToggle();
        });
        $(".toolboxCategoryHeader_Attributes").on("click", function () {
            $(".toolboxLi_Attributes").slideToggle();
        });
        $(".toolboxCategoryHeader_UI").on("click", function () {
            $(".toolboxLi_UI").slideToggle();
        });
        $(".toolboxCategoryHeader_Roles").on("click", function () {
            $(".toolboxLi_Roles").slideToggle();
        });
        $(".toolboxCategoryHeader_States").on("click", function () {
            $(".toolboxLi_States").slideToggle();
        });
        $(".toolboxCategoryHeader_Targets").on("click", function () {
            $(".toolboxLi_Targets").slideToggle();
        });
        $(".toolboxCategoryHeader_Templates").on("click", function () {
            $(".toolboxLi_Templates").slideToggle();
        });
        $("#btnAddResRule").on("click", function () {
            newRule = $('<div class="ruleItem resourceRule" style="width: 250px; height: 60px; left: 10px; top: 110px;"></div>');
            $("#assignResourcesPanel").append(newRule);
            newRule.draggable({ containment: "#assignResourcesPanel" });
            newRule.resizable();
            newRule.droppable({
                containment: ".resourceRule",
                tolerance: "touch",
                accept: ".toolboxItem",
                greedy: true,
                drop: function (e, ui) {
                    droppedElement = ui.helper.clone();
                    droppedElement.removeClass("toolboxItem");
                    droppedElement.addClass("item");
                    $(this).append(droppedElement);
                    ruleContent = $(this);
                    leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left + 20;
                    topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top;
                    droppedElement.offset({ left: droppedElement.offset().left + leftOffset, top: droppedElement.offset().top + topOffset });
                    droppedElement.draggable({ containment: "parent" });
                    ui.helper.remove();
                }
            });
        });
        $("#btnAddWfRule").on("click", function () {
            newRule = $('<div id="wfRule1" class="ruleItem workflowRule" style="width: 766px; height: 360px; left: 46px; top: 480px;"><div class="workflowRuleHeader"><div class="vericalLabel">Nové pravidlo</div>'
                + '</div><div class="swimlaneArea"><div class="swimlane"><div class="swimlaneRolesArea"><div class="rolePlaceholder"><div class="rolePlaceholderLabel">Pokud chcete specifikovat roli<br />'
                + 'přetáhněte ji do této oblasti</div></div></div><div class="swimlaneContentArea"></div></div>'
                + '<div class="swimlane"><div class="swimlaneRolesArea"><div class="rolePlaceholder"><div class="rolePlaceholderLabel">Pokud chcete specifikovat roli<br />'
                + 'přetáhněte ji do této oblasti</div></div></div><div class="swimlaneContentArea"></div></div></div></div>');
            $("#workflowRulesPanel").append(newRule);
            newRule.draggable({ containment: "#workflowRulesPanel" });
            newRule.resizable();
            newRule.find(".swimlaneRolesArea").droppable({
                containment: ".swimlaneContentArea",
                tolerance: "touch",
                accept: ".toolboxItem.roleItem",
                greedy: true,
                drop: function (e, ui) {
                    droppedElement = ui.helper.clone();
                    $(this).find(".rolePlaceholder, .roleItem").remove();
                    $(this).append($('<div class="roleItem">' + droppedElement.text() + '</div>'));
                    ui.helper.remove();
                }
            });
            newRule.find(".swimlaneContentArea").droppable({
                containment: ".swimlaneContentArea",
                tolerance: "touch",
                accept: ".toolboxSymbol, .toolboxItem",
                greedy: false,
                drop: function (e, ui) {
                    droppedElement = ui.helper.clone();
                    if (droppedElement.hasClass("roleItem")) {
                        ui.draggable.draggable("option", "revert", true);
                        return false;
                    }
                    $(this).append(droppedElement);
                    ruleContent = $(this);
                    if (droppedElement.hasClass("toolboxSymbol")) {
                        droppedElement.removeClass("toolboxSymbol ui-draggable ui-draggable-dragging");
                        droppedElement.addClass("symbol");
                        leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left;
                        topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top;
                    }
                    else {
                        droppedElement.removeClass("toolboxItem");
                        droppedElement.addClass("item");
                        leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left + 38;
                        topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top - 18;
                    }
                    droppedElement.offset({ left: droppedElement.offset().left + leftOffset, top: droppedElement.offset().top + topOffset });
                    droppedElement.draggable({ containment: "parent" });
                    ui.helper.remove();
                }
            });
        });
        $("#tapestryLibraryArea .libraryItem").on("click", function () {
            $(this).toggleClass("highlighted");
        });

        $("#assignResourcesPanel .resourceRule").draggable({ containment: "#assignResourcesPanel" });
        $("#assignResourcesPanel .resourceRule").resizable();
        $("#assignResourcesPanel .item").draggable({
            containment: "parent",
            drag: function () {
                instance = $(this).parents(".resourceRule").data("jsPlumbInstance");
                instance.recalculateOffsets();
                instance.repaintEverything();
            }
        });
        $("#workflowRulesPanel .item, #workflowRulesPanel .symbol").draggable({
            containment: "parent",
            drag: function () {
                instance = $(this).parents(".workflowRule").data("jsPlumbInstance");
                instance.recalculateOffsets();
                instance.repaintEverything();
            }
        });
        $("#workflowRulesPanel .workflowRule").draggable({ containment: "#workflowRulesPanel", handle: ".workflowRuleHeader" });
        $("#workflowRulesPanel .workflowRule").resizable({
            resize: function () {
                instance = $(this).data("jsPlumbInstance");
                instance.recalculateOffsets();
                instance.repaintEverything();
            }
        });
        $(".toolboxItem, .toolboxSymbol").draggable({
            helper: "clone",
            appendTo: '#tapestryWorkspace',
            containment: 'window',
            tolerance: "fit",
            revert: true,
            scroll: true
        });
        $(".resourceRule").droppable({
            containment: ".resourceRule",
            tolerance: "touch",
            accept: ".toolboxItem",
            greedy: true,
            drop: function (e, ui) {
                droppedElement = ui.helper.clone();
                droppedElement.removeClass("toolboxItem");
                droppedElement.addClass("item");
                $(this).append(droppedElement);
                ruleContent = $(this);
                leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left + 20;
                topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top;
                droppedElement.offset({ left: droppedElement.offset().left + leftOffset, top: droppedElement.offset().top + topOffset });
                droppedElement.draggable({ containment: "parent" });
                ui.helper.remove();
            }
        });
        $(".swimlaneRolesArea").droppable({
            containment: ".swimlaneContentArea",
            tolerance: "touch",
            accept: ".toolboxItem.roleItem",
            greedy: true,
            drop: function (e, ui) {
                droppedElement = ui.helper.clone();
                $(this).find(".rolePlaceholder, .roleItem").remove();
                $(this).append($('<div class="roleItem">'+droppedElement.text()+'</div>'));
                ui.helper.remove();
            }
        });
        $(".swimlaneContentArea").droppable({
            containment: ".swimlaneContentArea",
            tolerance: "touch",
            accept: ".toolboxSymbol, .toolboxItem",
            greedy: false,
            drop: function (e, ui) {
                droppedElement = ui.helper.clone();
                if (droppedElement.hasClass("roleItem")) {
                    ui.draggable.draggable("option", "revert", true);
                    return false;
                }
                $(this).append(droppedElement);
                ruleContent = $(this);
                if (droppedElement.hasClass("toolboxSymbol")) {
                    droppedElement.removeClass("toolboxSymbol ui-draggable ui-draggable-dragging");
                    droppedElement.addClass("symbol");
                    leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left;
                    topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top;
                }
                else {
                    droppedElement.removeClass("toolboxItem");
                    droppedElement.addClass("item");
                    leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left + 38;
                    topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top - 18;
                }
                droppedElement.offset({ left: droppedElement.offset().left + leftOffset, top: droppedElement.offset().top + topOffset });
                droppedElement.draggable({ containment: "parent" });
                ui.helper.remove();
            }
        });
        $.contextMenu({
            selector: '.item, .symbol',
            trigger: 'right',
            zIndex: 300,
            callback: function (key, options) {
                item = options.$trigger;
                if (key == "delete") {
                    currentInstance = item.parents(".ruleItem").data("jsPlumbInstance");
                    currentInstance.removeAllEndpoints(item, true);
                    item.remove();
                }
            },
            items: {
                "delete": { name: "Delete", icon: "delete" }
            }
        });
        $.contextMenu({
            selector: '.ruleItem',
            trigger: 'right',
            zIndex: 300,
            callback: function (key, options) {
                item = options.$trigger;
                if (key == "delete") {
                    item.remove();
                }
            },
            items: {
                "delete": { name: "Delete rule", icon: "delete" }
            }
        });
        $.contextMenu({
            selector: '.swimlaneRolesArea .roleItem',
            trigger: 'right',
            zIndex: 300,
            callback: function (key, options) {
                item = options.$trigger;
                if (key == "delete") {
                    item.parents(".swimlaneRolesArea").append($('<div class="rolePlaceholder"><div class="rolePlaceholderLabel">'
                        + 'Pokud chcete specifikovat roli<br />přetáhněte ji do této oblasti</div></div>'));
                    item.remove();
                }
            },
            items: {
                "delete": { name: "Remove role", icon: "delete" }
            }
        });
        $(window).scroll(function () {
            leftBar = $("#tapestryLeftBar");
            scrollTop = $(window).scrollTop();
            lowerPanelTop = $("#lowerPanel").offset().top;
            if (scrollTop > lowerPanelTop)
                leftBar.css("top", scrollTop - lowerPanelTop);
            else
                leftBar.css("top", 0);
            RecalculateToolboxHeight();
        });
        $(window).resize(function () {
            RecalculateToolboxHeight();
        });
    }
});
