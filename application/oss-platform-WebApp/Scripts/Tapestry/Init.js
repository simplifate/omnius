$(function () {
    $("#headerBlockName").on("click", function () {
        renameBlockDialog.dialog("open");
    });
    $("#headerTableName").on("click", function () {
        chooseTableDialog.dialog("open");
    });
    $("#btnAddActions").on("click", function () {
        addActionsDialog.dialog("open");
    });
    $("#btnAddRule").on("click", function () {
        newRule = $('<div class="rule"><div class="ruleHeader"></div></div>');
        $("#rulesPanel").append(newRule);
        newRule.resizable();
        newRule.draggable({ handle: ".ruleHeader" });
        CreateJsPlumbInstanceForRule(newRule);
        newRule.droppable({
            accept: ".menuItem",
            drop: function (e, ui) {
                droppedElement = ui.helper.clone();
                ui.helper.remove();
                droppedElement.appendTo(this);
                leftOffset = ui.draggable.parent().offset().left - $(this).offset().left;
                topOffset = ui.draggable.parent().offset().top - $(this).offset().top;
                droppedElement.offset({ left: droppedElement.offset().left + leftOffset, top: droppedElement.offset().top + topOffset })
                droppedElement.removeClass("menuItem");
                droppedElement.addClass("item");
                AddToJsPlumb($(this).data("jsPlumbInstance"), droppedElement);
            }
        });
    });
    $(".blockPanel").resizable({
        resize: function (event, ui) {
            $("#rolesPanel").css("left", $("#upperVerticalDivider").position.left);
            $("#statesPanel").css("left", $("#lowerVerticalDivider").position.left);
        }
    });
    $(".rule").resizable();
    $(".rule").draggable({ handle: ".ruleHeader" });
    $("#upperVerticalDivider").draggable({
        axis: "x",
        drag: function (event, ui) {
            shift = ui.position.left - 722;
            $("#attributesPanel").width(676 + shift);
            $("#rolesPanel").css("left", 728 + shift);
        }
    });
    $("#lowerVerticalDivider").draggable({
        axis: "x",
        drag: function (event, ui) {
            shift = ui.position.left - 722;
            $("#viewsPanel").width(676 + shift);
            $("#statesPanel").css("left", 728 + shift);
        }
    });
    $(".menuItem").draggable({
        helper: "clone",
        tolerance: "fit",
        revert: true
    });
    $.contextMenu({
        selector: '.rule .item',
        trigger: 'right',
        zIndex: 300,
        callback: function (key, options) {
            if (key == "delete") {
                item = options.$trigger;
                currentInstance = item.parents(".rule").data("jsPlumbInstance");
                currentInstance.removeAllEndpoints(item, true);
                item.remove();
            }
            else if (key == "edit") {
                alert("TODO: Show a dialog with item properties.");
            }

        },
        items: {
            "edit": { name: "Edit", icon: "edit" },
            "sep1": "---------",
            "delete": { name: "Delete", icon: "delete" }
        }
    });
    $.contextMenu({
        selector: '.rule',
        trigger: 'right',
        zIndex: 300,
        callback: function (key, options) {
            if (key == "delete") {
                options.$trigger.remove();
            }
        },
        items: {
            "delete": { name: "Delete rule", icon: "delete" }
        }
    });
    $(".rule").droppable({
        accept: ".menuItem",
        containment: ".rule",
        drop: function (e, ui) {
            droppedElement = ui.helper.clone();
            ui.helper.remove();
            droppedElement.appendTo(this);
            leftOffset = ui.draggable.parent().offset().left - $(this).offset().left;
            topOffset = ui.draggable.parent().offset().top - $(this).offset().top;
            droppedElement.offset({ left: droppedElement.offset().left + leftOffset + 8, top: droppedElement.offset().top + topOffset + 8 })
            droppedElement.removeClass("menuItem");
            droppedElement.addClass("item");
            AddToJsPlumb($(this).data("jsPlumbInstance"), droppedElement);
        }
    });
});
