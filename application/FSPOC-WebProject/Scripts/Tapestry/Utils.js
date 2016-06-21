var LastAssignedNumber = 0;
SystemTables = [
    {
        Name: "Omnius::Users",
        Columns: ["DisplayName", "Company", "Job", "Address", "Email"]
    }
];

function CreateJsPlumbInstanceForRule(ruleElement) {
    newInstance = jsPlumb.getInstance({
        Endpoint: ["Blank", {}],
        HoverPaintStyle: { strokeStyle: "#ff4000", lineWidth: 2 },
        ConnectionOverlays: [
            ["Arrow", {
                location: 1,
                length: 12,
                width: 15,
                height: 12,
                foldback: 0.8,
            }]
        ]
    });
    if(!ruleElement.attr("id"))
        ruleElement.attr("id", AssingID());
    newInstance.setContainer(ruleElement);
    newInstance.bind("click", function (con) {
        this.detach(con);
        ChangedSinceLastSave = true;
    });
    ruleElement.data("jsPlumbInstance", newInstance);
    return newInstance;
}
function AddToJsPlumb(item) {
    if (!item.attr("id")) {
        itemId = AssingID();
        item.attr("id", itemId);
    }
    else {
        itemId = item.attr("id");
    }
    item.draggable({
        revert: false,
        drag: function (event, ui) {
            element = $(this);
            rule = element.parents(".rule");
            rule.data("jsPlumbInstance").repaintEverything();
            resourceRuleMode = rule.hasClass("resourceRule");

            ui.position.left = Math.round((ui.position.left + element.width()/2) / 20) * 20 - element.width()/2;
            ui.position.top = Math.round((ui.position.top + element.height()/2) / 20) * 20 - element.height()/2;

            rightEdge = ui.position.left + element.width() + (resourceRuleMode ? 20 : 122);
            bottomEdge = ui.position.top + element.height() + 20;

            limitChecked = false;

            if (rule.width() < rightEdge + 30) {
                limits = CheckRuleResizeLimits(rule, resourceRuleMode);
                limitChecked = true;
                rule.width(rightEdge + 30);
            }
            if (rule.height() < bottomEdge) {
                if (!limitChecked) {
                    limits = CheckRuleResizeLimits(rule, resourceRuleMode);
                    limitChecked = true;
                }
                rule.height(bottomEdge);
            }
            if (limitChecked) {
                if (rule.width() > limits.horizontal - 10)
                    rule.width(limits.horizontal - 10);
                if (rule.height() > limits.vertical - 10)
                    rule.height(limits.vertical - 10);
                limitChecked = false;
            }
            if (resourceRuleMode) {
                if (ui.position.left < 10)
                    ui.position.left = 10;
                else if (ui.position.left + element.width() + 40 > rule.width())
                    ui.position.left = rule.width() - element.width() - 40;
                if (ui.position.top < 10)
                    ui.position.top = 10;
                else if (ui.position.top + element.height() + 20 > rule.height())
                    ui.position.top = rule.height() - element.height() - 20;
            }
            else {
                swimlane = element.parents(".swimlane");
                if (ui.position.left < 0)
                    ui.position.left = 0;
                else if (ui.position.left + element.width() + 40 > swimlane.width())
                    ui.position.left = swimlane.width() - element.width() - 40;
                if (ui.position.top < 0 && ui.position.top > -50)
                    ui.position.top = 0;
                else if (ui.position.top <= -50) {
                    currentSwimlaneIndex = swimlane.index();
                    swimlaneCount = rule.find(".swimlane").length;
                    if (currentSwimlaneIndex > 0) {
                        higherSwimlane = rule.find(".swimlane").eq(currentSwimlaneIndex-1).find(".swimlaneContentArea");
                        element.detach();
                        higherSwimlane.append(element);
                        element.position.top = 0;
                        return false;
                    }
                    else
                        ui.position.top = 0;
                }
                else if (ui.position.top + element.height() > swimlane.height() - 20 && ui.position.top + element.height() <= swimlane.height() + 30)
                    ui.position.top = swimlane.height() - element.height() - 20;
                else if (ui.position.top + element.height() > swimlane.height() + 30) {
                    currentSwimlaneIndex = swimlane.index();
                    swimlaneCount = rule.find(".swimlane").length;
                    if (currentSwimlaneIndex < swimlaneCount - 1) {
                        lowerSwimlane = rule.find(".swimlane").eq(currentSwimlaneIndex + 1).find(".swimlaneContentArea");
                        element.detach();
                        lowerSwimlane.append(element);
                        element.position.top = lowerSwimlane.height() - element.height();
                        return false;
                    }
                    else
                        ui.position.top = swimlane.height() - element.height() - 20;
                }
            }
        },
        stop: function (event, ui) {
            instance = $(this).parents(".rule").data("jsPlumbInstance");
            instance.recalculateOffsets();
            instance.repaintEverything();
            ChangedSinceLastSave = true;
        }
    });
    item.css({
        left: Math.round((item.position().left + item.width()/2) / 20) * 20 - item.width()/2,
        top: Math.round((item.position().top + item.height()/2) / 20) * 20 - item.height()/2
    });
    instance = item.parents(".rule").data("jsPlumbInstance");
    specialEndpointsType = item.attr("endpoints");
    if (item.attr("symbolType") && item.attr("symbolType").indexOf("gateway-") == 0) {
        specialEndpointsType = "gateway";
    }
    if (specialEndpointsType == "gateway") {
        instance.addEndpoint(itemId, trueEndpoint, {
            anchor: "RightMiddle", uuid: itemId + "RightMiddle"
        });
        instance.addEndpoint(itemId, falseEndpoint, {
            anchor: "BottomCenter", uuid: itemId + "BottomCenter"
        });
    }
    else if (specialEndpointsType == "final" || item.hasClass("targetItem")) { }
    else {
        instance.addEndpoint(itemId, sourceEndpoint, {
            anchor: "RightMiddle", uuid: itemId + "RightMiddle"
        });
    }
    instance.makeTarget(item, {
        dropOptions: { hoverClass: "dragHover" },
        anchor: "Continuous",
        allowLoopback: false
    });
}
function ToolboxItemDraggable(item) {
    item.find(".toolboxItem").draggable({
        helper: "clone",
        appendTo: '#tapestryWorkspace',
        containment: 'window',
        tolerance: "fit",
        revert: true,
        scroll: true,
        start: function () {
            dragModeActive = true;
        }
    });
}
function AssingID() {
    LastAssignedNumber++;
    return "tapestryElement" + LastAssignedNumber;
}
function AddIconToItem(element) {
    item = $(element);
    if (item.hasClass("attribute")) {
        item.prepend($('<i class="fa fa-database" style="margin-right: 6px;"></i>'));
    }
    else if (item.hasClass("port")) {
        item.prepend($('<i class="fa fa-sign-out" style="margin-left: 1px; margin-right: 5px;"></i>'));
    }
    else if (item.hasClass("role")) {
        item.prepend($('<i class="fa fa-user" style="margin-left: 1px; margin-right: 6px;"></i>'));
    }
    else if (item.hasClass("view")) {
        item.prepend($('<i class="fa fa-paint-brush" style="margin-left: 1px; margin-right: 6px;"></i>'));
    }
    else if (item.hasClass("action")) {
        item.prepend($('<i class="fa fa-cogs" style="margin-left: 1px; margin-right: 6px;"></i>'));
    }
    else if (item.hasClass("state")) {
        item.prepend($('<i class="fa fa-ellipsis-v" style="margin-left: 4px; margin-right: 8px;"></i>'));
    }
}
function CheckRuleResizeLimits(rule, resourceRuleMode) {
    horizontalLimit = 1000000;
    verticalLimit = 1000000;

    ruleLeft = rule.position().left;
    ruleRight = ruleLeft + rule.width();
    ruleTop = rule.position().top;
    ruleBottom = rule.position().top + rule.height();

    $(resourceRuleMode ? "#resourceRulesPanel .resourceRule" : "#workflowRulesPanel .workflowRule").each(function (index, element) {
        otherRule = $(element);
        if (otherRule.attr("id") != rule.attr("id")) {
            otherRuleLeft = otherRule.position().left;
            otherRuleRight = otherRuleLeft + otherRule.width();
            otherRuleTop = otherRule.position().top;
            otherRuleBottom = otherRule.position().top + otherRule.height();

            if (otherRuleTop < ruleBottom && otherRuleBottom > ruleTop
                && otherRuleLeft + 30 > ruleRight && otherRuleLeft - ruleLeft < horizontalLimit)
                horizontalLimit = otherRuleLeft - ruleLeft;
            if (otherRuleLeft < ruleRight && otherRuleRight > ruleLeft
                && otherRuleTop  + 20 > ruleBottom && otherRuleTop - ruleTop < verticalLimit)
                verticalLimit = otherRuleTop - ruleTop;
        }
    });
    return { horizontal: horizontalLimit, vertical: verticalLimit }
}
function GetItemTypeClass(item) {
    if (item.hasClass("actionItem")) {
        typeClass = "actionItem";
    }
    else if (item.hasClass("attributeItem")) {
        typeClass = "attributeItem";
    }
    else if (item.hasClass("uiItem")) {
        typeClass = "uiItem";
    }
    else if (item.hasClass("roleItem")) {
        typeClass = "roleItem";
    }
    else if (item.hasClass("stateItem")) {
        typeClass = "stateItem";
    }
    else if (item.hasClass("targetItem")) {
        typeClass = "targetItem";
    }
    else if (item.hasClass("templateItem")) {
        typeClass = "templateItem";
    }
    else if (item.hasClass("symbol")) {
        typeClass = "symbol";
    }
    else
        typeClass = "";
    return typeClass;
}
function RecalculateToolboxHeight() {
    var leftBar = $("#tapestryLeftBar");
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
    $("#workflowRulesPanel").height($(window).height() - 105);
    $("#tapestryLeftBarMinimized").height($("#workflowRulesPanel").offset().top + $("#workflowRulesPanel").height() - lowerPanelTop);
    
}
function LoadConditionColumns(parent) {
    columnSelect = parent.find(".conditionVariableCell select");
    for (i = 0; i < CurrentTableColumnArray.length; i++) {
        cData = CurrentTableColumnArray[i];
        switch (cData.Type) {
            case "varchar":
                columnType = "string";
                break;
            case "boolean":
                columnType = "bool";
                break;
            case "integer":
                columnType = "int";
                break;
            default:
                columnType = "unknown";
        }
        columnSelect.append($('<option varType="' + columnType + '">' + cData.Name + '</option>'));
    }
    var optionSelected = $("option:selected", columnSelect);
    varType = optionSelected.attr("varType");
    currentCondition = columnSelect.parents("tr");
    currentCondition.find(".conditionOperatorCell select, .conditionValueCell select, .conditionValueCell input").remove();
    switch (varType) {
        case "bool":
            currentCondition.find(".conditionValueCell").append($('<select><option selected="selected">true</option><<option>false</option></select>'));
            currentCondition.find(".conditionOperatorCell").append($('<select><option selected="selected">==</option><option>!=</option></select>'));
            break;
        case "int":
            currentCondition.find(".conditionValueCell").append($('<input type="number"></input>'));
            currentCondition.find(".conditionOperatorCell").append($('<select><option selected="selected">==</option><option>!=</option><option>&gt;</option><option>&gt;=</option><option>&lt;</option><option>&lt;=</option>'));
            break;
        case "string":
            currentCondition.find(".conditionValueCell").append($('<input type="text"></input>'));
            currentCondition.find(".conditionOperatorCell").append($('<select><option selected="selected">==</option><option>!=</option><option>contains</option><option inputType="none">is empty</option><option inputType="none">is not empty</option></select>'));
            break;
        case "unknown":
        default:
            currentCondition.find(".conditionValueCell").append($('<input type="text"></input>'));
            currentCondition.find(".conditionOperatorCell").append($('<select><option selected="selected">==</option><option>!=</option><option>&gt;</option><option>&gt;=</option><option>&lt;</option><option>&lt;=</option><option>contains</option><option inputType="none">is empty</option><option inputType="none">is not empty</option></select>'));
    }
}
var ConditionSetTemplate = '<div class="conditionSet"><div class="conditionSetHeading"><span class="conditionSetPrefix"> a</span>ll of these conditions must be met</div>'
    + '<div class="removeConditionSetIcon">X</div><table class="conditionTable"></table></div>';
var ConditionTemplate = '<tr><td class="conditionOperator"></td><td class="conditionVariableCell"><select></select>'
    + '</td><td class="conditionOperatorCell"><select><option selected="selected">==</option><option>!=</option><option>&gt;</option><option>&gt;=</option><option>&lt;</option><option>&lt;=</option><option>is empty</option><option>is not empty</option></select></td><td class="conditionValueCell">'
    + '<select><option selected="selected">True</option></select></td><td class="conditionActions"><div class="conditionActionIcon addAndConditionIcon">&</div>'
    + '<div class="conditionActionIcon addOrConditionIcon">|</div><div class="conditionActionIcon removeConditionIcon">X</div></td>'
    + '</tr>';
var ManualInputConditionTemplate = '<tr><td class="conditionOperator"></td><td class="conditionVariableCell"><input type="text"></input>'
    + '</td><td class="conditionOperatorCell"><select><option selected="selected">==</option><option>!=</option><option>&gt;</option><option>&gt;=</option><option>&lt;</option><option>&lt;=</option><option inputType="none">is empty</option><option inputType="none">is not empty</option><option>contains</option></select></td><td class="conditionValueCell">'
    + '<input type="text"></input></td><td class="conditionActions"><div class="conditionActionIcon addAndConditionIcon">&</div>'
    + '<div class="conditionActionIcon addOrConditionIcon">|</div><div class="conditionActionIcon removeConditionIcon">X</div></td>'
    + '</tr>';
