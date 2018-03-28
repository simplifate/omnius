/**
 * nuSelectable - jQuery Plugin
 * Copyright (c) 2015, Alex Suyun
 * Copyrights licensed under The MIT License (MIT)
 */
;
(function ($, window, document, undefined) {

    'use strict';

    var plugin = 'nuSelectable';

    var defaults = {
        onSelect: function () { },
        onUnSelect: function () { },
        onClear: function () { }
    };

    var nuSelectable = function (container, options) {
        this.container = $(container);
        this.options = $.extend({}, defaults, options);
        this.selection = $('<div>')
          .addClass(this.options.selectionClass);
        this.items = $(this.options.items);
        this.downTimer = null;
        this.init();
    };

    nuSelectable.prototype.init = function () {
        if (!this.options.autoRefresh) {
            this.itemData = this._cacheItemData();
        }
        this.selecting = false;
        this._normalizeContainer();
        this._bindEvents();
        return true;
    };

    nuSelectable.prototype._normalizeContainer = function () {
        this.container.css({
            '-webkit-touch-callout': 'none',
            '-webkit-user-select': 'none',
            '-khtml-user-select': 'none',
            '-moz-user-select': 'none',
            '-ms-user-select': 'none',
            'user-select': 'none'
        });
    };

    nuSelectable.prototype._cacheItemData = function () {
        var itemData = [],
          itemsLength = this.items.length;

        for (var i = 0, item; item = $(this.items[i]), i <
          itemsLength; i++) {
            itemData.push({
                element: item,
                selected: item.hasClass(this.options.selectedClass),
                selecting: false,
                position: item[0].getBoundingClientRect()
            });
        }
        return itemData;
    };

    nuSelectable.prototype._collisionDetector = function () {
        var selector = this.selection[0].getBoundingClientRect(),
          dataLength = this.itemData.length;
        // Using native for loop vs $.each for performance (no overhead)
        for (var i = dataLength - 1, item; item = this.itemData[i], i >=
          0; i--) {
            var collided = !(selector.right < item.position.left ||
              selector.left > item.position.right ||
              selector.bottom < item.position.top ||
              selector.top > item.position.bottom);

            if (collided) {
                if (item.selected) {
                    item.element.removeClass(this.options.selectedClass);
                    item.selected = false;
                }
                if (!item.selected) {
                    item.element.addClass(this.options.selectedClass);
                    item.selected = true;
                    this.options.onSelect(item.element);
                }
            }
            else {
                if (this.selecting) {
                    item.element.removeClass(this.options.selectedClass);
                    this.options.onUnSelect(item.element);
                }
            }

        }
    };

    nuSelectable.prototype._createSelection = function (x, y) {
        this.selection.css({
            'position': 'absolute',
            'top': y + 'px',
            'left': x + 'px',
            'width': '0',
            'height': '0',
            'z-index': '999',
            'overflow': 'hidden'
        })
          .appendTo(this.container);
    };

    nuSelectable.prototype._drawSelection = function (width, height, x,
      y) {
        this.selection.css({
            'width': width,
            'height': height,
            'top': y,
            'left': x
        });
    };

    nuSelectable.prototype.clear = function () {
        this.items.removeClass(this.options.selectedClass);
        this.options.onClear();
    };

    nuSelectable.prototype._mouseDown = function (event) {
        var e = event;
        var that = this;

        if ($(e.target).is(this.container) && e.shiftKey) {
            this.downTimer = setTimeout(function () {
                that._mouseHold.apply(that, [e]);
            }, 50);
        }
    };

    nuSelectable.prototype._mouseHold = function (event) {
        event.preventDefault();
        event.stopPropagation();
        if (this.options.disable) {
            return false;
        }
        if (event.which !== 1) {
            return false;
        }
        if (this.options.autoRefresh) {
            this.itemData = this._cacheItemData();
        }
        if (event.metaKey || event.ctrlKey) {
            this.selecting = false;
        }
        else {
            this.selecting = true;
        }

        var x = (event.pageX - this.container.offset().left);
        var y = (event.pageY - this.container.offset().top);

        this.pos = [x, y];
        this._createSelection(x, y);

    };

    nuSelectable.prototype._mouseMove = function (event) {
        // Save some bytes
        var pos = this.pos;
        if (!pos) {
            return;
        }

        event.preventDefault();
        event.stopPropagation();

        var x = (event.pageX - this.container.offset().left);
        var y = (event.pageY - this.container.offset().top);

        var newpos = [x, y],
          width = Math.abs(newpos[0] - pos[0]),
          height = Math.abs(newpos[1] - pos[1]),
          top, left;

        top = (newpos[0] < pos[0]) ? (pos[0] - width) : pos[0];
        left = (newpos[1] < pos[1]) ? (pos[1] - height) : pos[1];
        this._drawSelection(width, height, top, left);
        this._collisionDetector();

    };

    nuSelectable.prototype._mouseUp = function (event) {
        clearTimeout(this.downTimer);

        if (!this.pos) {
            this.clear();
            return;
        }

        event.preventDefault();
        event.stopPropagation();

        this.selecting = false;
        this.selection.remove();

        var x = (event.pageX - this.container.offset().left);
        var y = (event.pageY - this.container.offset().top);

        if (x === this.pos[0] && y === this.pos[1]) {
            this.clear();
        }

        this.pos = [];
    };

    nuSelectable.prototype._bindEvents = function () {
        this.container.on('mousedown', $.proxy(this._mouseDown, this));
        this.container.on('mousemove', $.proxy(this._mouseMove, this));
        // Binding to document is 'safer' than the container for mouse up
        $(document)
          .on('mouseup', $.proxy(this._mouseUp, this));
    };

    $.fn[plugin] = function (options) {
        var args = Array.prototype.slice.call(arguments, 1);

        return this.each(function () {
            var item = $(this),
              instance = item.data(plugin);
            if (!instance) {
                item.data(plugin, new nuSelectable(this, options));
            }
            else {

                if (typeof options === 'string' && options[0] !== '_' &&
                  options !== 'init') {
                    instance[options].apply(instance, args);
                }
            }

        });
    };

})(jQuery, window, document);

$(function () {
    $("#TimeSince,#TimeTo").datetimepicker({
        datepicker: true,
        timepicker: true,
        step: 5,
        format: "d.m.Y H:i"
    });

    function ShowDetail(id, width) {
        $.getJSON("/Watchtower/Log/GetRow/" + id, function (data) {
            var d = $('<div title="Record details..."></div>');
            var template = $('#detail').html()
                .replace('{time}', data.Time)
                .replace('{level}', data.Level)
                .replace('{user}', data.User)
                .replace('{server}', data.Server)
                .replace('{source}', data.Source)
                .replace('{application}', data.Application)
                .replace('{block}', data.Block)
                .replace('{action}', data.Action)
                .replace('{message}', data.Message)
                .replace('{vars}', data.Vars)
                .replace('{st}', data.StackTrace)
                .replace('{innerId}', data.Inner && data.Inner.Id ? data.Inner.Id : "");

            d.html(template);

            if (data.Inner == null) {
                d.find('.row-innerex').remove();
            }

            d.dialog({
                modal: true,
                draggable: false,
                resizable: false,
                width: width,
                maxHeight: $(window).height() * 0.9,
                close: function () {
                    $(this).dialog('destroy').remove();
                }
            });
        });
    }

    $(document).on('click', "body.watchtowerModule #lowerPanel .fa-search", function () {
        var row = $(this).parents('tr').eq(0);
        var id = row.attr('data-id');

        ShowDetail(id, '80%');
        return false;
    })
    .on('click', "a[data-innerid]", function () {
        var id = $(this).attr('data-innerid');

        ShowDetail(id, '70%');
        return false;
    })
    .on('click', '.pagination a[data-page]', function () {
        $('#PageNumber').val($(this).attr('data-page'));
        $('#filterLogForm').submit();
        return false;
    })
    .on('click', '#filterLogForm #resetSearchForm', function () {
        $('#filterLogForm').find('select').each(function () { this.selectedIndex = 0; }).end()
                           .find('input[type=text]').val('').end()
                           .submit();
    });
});

sourceEndpoint = {
    endpoint: "Rectangle",
    paintStyle: { fillStyle: "#54c6f0", width: 12, height: 18 },
    hoverPaintStyle: { fillStyle: "#f98e4b" },
    isSource: true,
    connector: ["Flowchart", { stub: [5, 5], gap: 4, cornerRadius: 4 }],
    connectorStyle: {
        lineWidth: 2,
        strokeStyle: "#54c6f0",
        joinstyle: "round"
    },
    connectorHoverStyle: { strokeStyle: "#f98e4b" },
    cssClass: "sourceEndpoint",
    maxConnections: -1
}
trueEndpoint = $.extend({}, sourceEndpoint, {
    overlays: [
        ["Label", {
            location: [1.7, 1.5],
            label: "True",
            cssClass: "endpointSourceLabel"
        }]
    ]
});
falseEndpoint = $.extend({}, sourceEndpoint, {
    paintStyle: { fillStyle: "#54c6f0", width: 18, height: 10 },
    overlays: [
        ["Label", {
            location: [1.5, 2],
            label: "False",
            cssClass: "endpointSourceLabel"
        }]
    ]
});
jsPlumb.ready(function () {
    if (CurrentModuleIs("tapestryModule")) {

        $(".resourceRule, .workflowRule").each(function (ruleIndex, rule) {
            currentInstance = CreateJsPlumbInstanceForRule($(rule));
        });
        $("#resourceRulesPanel .item, #workflowRulesPanel .item, #workflowRulesPanel .symbol").each(function (index, element) {
            AddToJsPlumb($(element));
        });
    }
});

var LastAssignedNumber = 0;
SystemTables = [
    {
        Name: "Omnius::AppRoles",
        Columns: ["Id", "Name", "Priority", "ApplicationId"]
    },
    {
        Name: "Omnius::Users",
        Columns: ["Id", "DisplayName", "Company", "Job", "Address", "Email"]
    },
    {
        Name: "Omnius::LogItems",
        Columns: ["Id", "Timestamp", "LogEventType", "UserId", "IsPlatformEvent", "AppId", "Message"]
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
function GetIsBootstrap(item)
{
    var isBootstrap = item.attr('isbootstrap');
    return isBootstrap && (isBootstrap == true || isBootstrap == 'true') ? true : false;
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
    else if (item.hasClass("integrationItem")) {
        typeClass = "integrationItem";
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
            currentCondition.find(".conditionValueCell").append($('<select><option selected="selected">b$true</option><option>b$false</option></select>'));
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
    + '</td><td class="conditionOperatorCell"><select><option selected="selected">==</option><option>!=</option><option>&gt;</option><option>&gt;=</option><option>&lt;</option><option>&lt;=</option><option inputType="none">is empty</option><option inputType="none">is not empty</option><option>contains</option><option inputType="none">exists</option></select></td><td class="conditionValueCell">'
    + '<input type="text"></input></td><td class="conditionActions"><div class="conditionActionIcon addAndConditionIcon">&</div>'
    + '<div class="conditionActionIcon addOrConditionIcon">|</div><div class="conditionActionIcon removeConditionIcon">X</div></td>'
    + '</tr>';
var HermesConditionSetTemplate = '<div class="conditionSet"><div class="conditionSetHeading"><span class="conditionSetPrefix"> a</span>ll of these conditions must be met</div>'
    + '<div class="removeConditionSetIcon">X</div><table class="conditionTable"></table></div>';
var HermesConditionTemplate = '<tr><td class="conditionOperator"></td><td class="conditionVariableCell"><select class="form-control" style="width:auto"><option value="From">From</option><option value="CC">CC</option><option value="Subject">Subject</option><option value="Body">Body</option></select>'
    + '</td><td class="conditionOperatorCell"><select class="form-control" style="width:auto"><option value="contains" selected="selected">obsahuje</option><option value="BeginWith">začíná</option><option value="EndWith">končí</option><option value="IsEmpty" data-inputType="none">is empty</option><option value="IsNotEmpty" data-inputType="none">is not empty</option></select></td><td class="conditionValueCell">'
    + '<input type="text" value="" class="form-control" /></td><td class="conditionActions"><div class="conditionActionIcon addAndConditionIcon">&</div>'
    + '<div class="conditionActionIcon addOrConditionIcon">|</div><div class="conditionActionIcon removeConditionIcon">X</div></td>'
    + '</tr>';
var AssociatedPageIds = [];

function SaveBlock(commitMessage) {
    pageSpinner.show();
    resourceRulesArray = [];
    workflowRulesArray = [];
    portTargetsArray = [];
    saveId = 0;
    $(".activeItem, .processedItem").removeClass("activeItem processedItem");
    $("#resourceRulesPanel .resourceRule").each(function (ruleIndex, ruleDiv) {
        itemArray = [];
        connectionArray = [];
        currentRule = $(ruleDiv);
        currentRule.find(".item").each(function (itemIndex, itemDiv) {
            currentItem = $(itemDiv);
            currentItem.attr("saveId", saveId);
            saveId++;
            itemArray.push({
                Id: currentItem.attr("saveId"),
                Label: currentItem.text(),
                TypeClass: GetItemTypeClass(currentItem),
                PositionX: parseInt(currentItem.css("left")),
                PositionY: parseInt(currentItem.css("top")),
                ActionId: currentItem.attr("actionid"),
                StateId: currentItem.attr("stateid"),
                PageId: GetIsBootstrap(currentItem) ? null : currentItem.attr("pageId"),
                ComponentName: currentItem.attr("componentName"),
                IsBootstrap: GetIsBootstrap(currentItem),
                BootstrapPageId: GetIsBootstrap(currentItem) ? currentItem.attr("pageId") : null,
                TableName: currentItem.attr("tableName"),
                IsShared: currentItem.attr("shared") === "true",
                ColumnName: currentItem.attr("columnName"),
                ColumnFilter: currentItem.data("columnFilter"),
                ConditionSets: currentItem.data("conditionSets")
            });
        });
        currentInstance = currentRule.data("jsPlumbInstance");
        jsPlumbConnections = currentInstance.getAllConnections();
        for (i = 0; i < jsPlumbConnections.length; i++) {
            currentConnection = jsPlumbConnections[i];
            sourceDiv = $(currentConnection.source);
            targetDiv = $(currentConnection.target);
            connectionArray.push({
                SourceId: sourceDiv.attr("saveId"),
                SourceSlot: 0,
                TargetId: targetDiv.attr("saveId"),
                TargetSlot: 0
            });
        }
        resourceRulesArray.push({
            Id: ruleIndex,
            Width: parseInt(currentRule.css("width")),
            Height: parseInt(currentRule.css("height")),
            PositionX: parseInt(currentRule.css("left")),
            PositionY: parseInt(currentRule.css("top")),
            ResourceItems: itemArray,
            Connections: connectionArray
        });
    });
    $("#workflowRulesPanel .workflowRule").each(function (ruleIndex, ruleDiv) {
        swimlanesArray = [];
        currentRule = $(ruleDiv);

        currentRule.find(".swimlane").each(function (swimlaneIndex, swimlaneDiv) {
            currentSwimlane = $(swimlaneDiv);
            currentSwimlane.attr("swimlaneIndex", swimlaneIndex);
            rolesArray = [];
            itemArray = [];
            symbolArray = [];
            connectionArray = [];
            subflowArray = [];
            currentSwimlane.find(".swimlaneRolesArea .roleItem").each(function (roleIndex, roleDiv) {
                rolesArray.push($(roleDiv).text());
            });

            currentSwimlane.find("> .subflow").each(function (subflowIndex) {
                var subflow = $(this);
                subflow.attr("saveId", saveId);
                saveId++;

                subflowArray.push({
                    Id: subflow.attr('saveId'),
                    Name: "",
                    Comment: "",
                    PositionX: parseInt(subflow.css('left')),
                    PositionY: parseInt(subflow.css('top')),
                    Width: parseInt(subflow.css('width')),
                    Height: parseInt(subflow.css('height'))
                })
            });
            
            currentSwimlane.find(".item, .symbol").each(function (itemIndex, itemDiv) {
                currentItem = $(itemDiv);
                currentItem.attr("saveId", saveId);
                saveId++;
                itemArray.push({
                    Id: currentItem.attr("saveId"),
                    Label: currentItem.find(".itemLabel").length ? currentItem.find(".itemLabel").text() : currentItem.data("label"),
                    Name: currentItem.find('.itemName').text(),
                    Comment: currentItem.find('.itemComment').text(),
                    CommentBottom: currentItem.find('.itemComment').hasClass('bottom'),
                    TypeClass: GetItemTypeClass(currentItem),
                    DialogType: currentItem.attr("dialogType"),
                    StateId: currentItem.attr("stateid"),
                    TargetId: currentItem.attr("targetid"), 
                    PositionX: parseInt(currentItem.css("left")),
                    PositionY: parseInt(currentItem.css("top")),
                    ActionId: currentItem.attr("actionid"),
                    InputVariables: currentItem.data("inputVariables"),
                    OutputVariables: currentItem.data("outputVariables"),
                    PageId: currentItem.attr("pageId"),
                    ComponentName: currentItem.attr("componentName"),
                    IsBootstrap: GetIsBootstrap(currentItem),
                    isAjaxAction: currentItem.data("isAjaxAction"),
                    Condition: currentItem.data("condition"),
                    ConditionSets: currentItem.data("conditionSets"),
                    SymbolType: currentItem.attr("symbolType")
                });
            });

            swimlanesArray.push({
                SwimlaneIndex: swimlaneIndex,
                Height: parseInt(currentSwimlane.css("height")),
                Roles: rolesArray,
                WorkflowItems: itemArray,
                subflow: subflowArray
            });
        });


        currentInstance = currentRule.data("jsPlumbInstance");
        jsPlumbConnections = currentInstance.getAllConnections();
        for (i = 0; i < jsPlumbConnections.length; i++) {
            currentConnection = jsPlumbConnections[i];
            sourceDiv = $(currentConnection.source);
            targetDiv = $(currentConnection.target);
            sourceEndpointUuid = currentConnection.endpoints[0].getUuid();
            if (sourceEndpointUuid.match("BottomCenter$"))
                sourceSlot = 1;
            else
                sourceSlot = 0;
            if (!sourceDiv.hasClass("subSymbol")) {
                connectionArray.push({
                    SourceId: sourceDiv.attr("saveId"),
                    SourceSlot: sourceSlot,
                    TargetId: targetDiv.attr("saveId"),
                    TargetSlot: 0
                });
            }
        }

        workflowRulesArray.push({
            Id: ruleIndex,
            Name: currentRule.find(".workflowRuleHeader .verticalLabel").text(),
            Width: parseInt(currentRule.css("width")),
            Height: parseInt(currentRule.css("height")),
            PositionX: parseInt(currentRule.css("left")),
            PositionY: parseInt(currentRule.css("top")),
            Swimlanes: swimlanesArray,
            Connections: connectionArray
        });
    });

    toolboxState = {
        Actions: [],
        Attributes: [],
        UiComponents: [],
        Roles: [],
        States: [],
        Targets: [],
        Templates: [],
        Integrations: []
    }
    $(".tapestryToolbox .toolboxItem").each(function (itemIndex, itemDiv) {
        toolboxItem = $(itemDiv);
        toolboxItemData = {
            Label: toolboxItem.find(".itemLabel").text(),
            ActionId: toolboxItem.attr("ActionId"),
            TableName: toolboxItem.attr("TableName") ? toolboxItem.attr("TableName") : null,
            IsShared: toolboxItem.attr("shared") === "true",
            ColumnName: toolboxItem.attr("ColumnName") ? toolboxItem.attr("ColumnName") : null,
            PageId: toolboxItem.attr("PageId"),
            ComponentName: toolboxItem.attr("ComponentName") ? toolboxItem.attr("ComponentName") : null,
            IsBootstrap: GetIsBootstrap(toolboxItem),
            StateId: toolboxItem.attr("StateId"),
            TargetName: toolboxItem.attr("TargetName") ? toolboxItem.attr("TargetName") : null,
            TargetId: toolboxItem.attr("TargetId")
        }
        if (toolboxItem.hasClass("actionItem")) {
            toolboxItemData.TypeClass = "actionItem";
            toolboxState.Actions.push(toolboxItemData);
        }
        else if (toolboxItem.hasClass("attributeItem")) {
            toolboxItemData.TypeClass = "attributeItem";
            toolboxState.Attributes.push(toolboxItemData);
        }
        else if (toolboxItem.hasClass("uiItem")) {
            toolboxItemData.TypeClass = "uiItem";
            toolboxState.UiComponents.push(toolboxItemData);
        }
        else if (toolboxItem.hasClass("roleItem")) {
            toolboxItemData.TypeClass = "roleItem";
            toolboxState.Roles.push(toolboxItemData);
        }
        else if (toolboxItem.hasClass("stateItem")) {
            toolboxItemData.TypeClass = "stateItem";
            toolboxState.States.push(toolboxItemData);
        }
        else if (toolboxItem.hasClass("targetItem")) {
            toolboxItemData.TypeClass = "targetItem";
            toolboxState.Targets.push(toolboxItemData);
        }
        else if (toolboxItem.hasClass("templateItem")) {
            toolboxItemData.TypeClass = "templateItem";
            toolboxState.Templates.push(toolboxItemData);
        }
        else if (toolboxItem.hasClass("integrationItem")) {
            toolboxItemData.TypeClass = "integrationItem";
            toolboxState.Integrations.push(toolboxItemData);
        }
    });
    postData = {
        CommitMessage: commitMessage,
        Name: $("#blockHeaderBlockName").text(),
        ResourceRules: resourceRulesArray,
        WorkflowRules: workflowRulesArray,
        PortTargets: portTargetsArray,
        ModelTableName: ModelTableName,
        AssociatedTableName: AssociatedTableName,
        AssociatedPageIds: AssociatedPageIds,
        AssociatedBootstrapPageIds: AssociatedBootstrapPageIds,
        AssociatedTableIds: AssociatedTableIds,
        RoleWhitelist: RoleWhitelist,
        ToolboxState: toolboxState,
        ParentMetablockId: $("#parentMetablockId").val()
    }    
    appId = $("#currentAppId").val();
    blockId = $("#currentBlockId").val();
    $.ajax({
        type: "POST",
        url: "/api/tapestry/apps/" + appId + "/blocks/" + blockId,
        data: postData,
        complete: function () {
            pageSpinner.hide()
        },
        success: function () {
            ChangedSinceLastSave = false;
            alert("The block has been successfully saved");
            $('#btnLock').html('Zamknout');
            TB.lock.isLockedForCurrentUser = false;
        }
    });
}

var ZoomFactor = 1.0;
var ChangedSinceLastSave = false, dragModeActive = false;
var lastLibId = 1000;

var AssociatedPageIds = [];
var AssociatedBootstrapPageIds = [];

$(function () {
    if (CurrentModuleIs("tapestryModule")) {
        RecalculateToolboxHeight();
        TB.load.loadBlock();

        // Buttons and UI effects
        $("#btnClear").on("click", function () {
            $("#resourceRulesPanel .resourceRule").remove();
            $("#workflowRulesPanel .workflowRule").remove();
            ChangedSinceLastSave = true;
        });

        $("#btnLoad").on("click", function () {
            if (ChangedSinceLastSave)
                confirmed = confirm("Máte neuložené změny, opravdu si přejete tyto změny zahodit?");
            else
                confirmed = true;
            if (confirmed) {
                TB.load.loadBlock();
            }
        });
        $("#btnLock").on("click",TB.lock._btnLockClick);
        $("#btnHistory").on("click", function () {
            historyDialog.dialog("open");
        });
        $("#btnOverview").on("click", function () {
            if (ChangedSinceLastSave)
                confirmed = confirm("Máte neuložené změny, opravdu si přejete opustit blok?");
            else
                confirmed = true;
            if (confirmed) {
                ChangedSinceLastSave = false;
                openMetablockForm = $("#openMetablockForm");
                openMetablockForm.find("input[name='metablockId']").val($("#parentMetablockId").val());
                openMetablockForm.submit();
            }
        });
        window.onbeforeunload = function () {
            if (ChangedSinceLastSave) {
                pageSpinner.hide();
                return "Máte neuložené změny, opravdu si přejete opustit blok?";
            }
        };
        $("#btnOpenTableConditions").on("click", function () {
            $("#conditions-dialog").dialog("open");
        });
        $(".toolboxCategoryHeader_Symbols").on("click", function () {
            $(this).toggleClass("hiddenCategory");
            $(".symbolToolboxSpace").slideToggle();
        });
        $(".toolboxCategoryHeader_Actions").on("click", function () {
            if($(".toolboxLi_Actions").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_Actions").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $(".toolboxCategoryHeader_Attributes").on("click", function () {
            if ($(".toolboxLi_Attributes").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_Attributes").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $(".toolboxCategoryHeader_UI").on("click", function () {
            if ($(".toolboxLi_UI").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_UI").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $(".toolboxCategoryHeader_Roles").on("click", function () {
            if ($(".toolboxLi_Roles").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_Roles").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $(".toolboxCategoryHeader_States").on("click", function () {
            if ($(".toolboxLi_States").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_States").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $(".toolboxCategoryHeader_Targets").on("click", function () {
            if ($(".toolboxLi_Targets").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_Targets").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $(".toolboxCategoryHeader_Templates").on("click", function () {
            if ($(".toolboxLi_Templates").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_Templates").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $(".toolboxCategoryHeader_Integrations").on("click", function () {
            if ($(".toolboxLi_Integrations").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_Integrations").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $("#blockHeaderBlockName").on("click", function () {
            renameBlockDialog.dialog("open");
        });
        $("#blockHeaderDbResCount").on("click", function () {
            chooseTablesDialog.dialog("open");
        });
        $("#blockHeaderRolesCount").on("click", function () {
            chooseWhitelistRolesDialog.dialog("open");
        });
        $(window).scroll(function () {
            var leftBar = $("#tapestryLeftBar");
            var scrollTop = $(window).scrollTop();
            var lowerPanelTop = $("#lowerPanel").offset().top;
            var overlay = $("#lowerPanelSpinnerOverlay");
            var topBarHeight = $("#topBar").height() + $("#appNotificationArea").height();

            overlay.css({right: 0, width: 'auto'});
            if (scrollTop > lowerPanelTop - topBarHeight) {
                leftBar.css({ top:topBarHeight, left: 225, position: "fixed" });
                overlay.css({ top:topBarHeight, left: 225, position: "fixed" });
            } else {
                leftBar.css({ top:0, left: 0, position: "absolute" });
                overlay.css({ top:0, left: 0, position: "absolute" });
            }
            RecalculateToolboxHeight();
        });
        $(window).resize(function () {
            RecalculateToolboxHeight();
        });
        $(".toolboxItem, .toolboxSymbol").draggable({
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
        $("#hideTapestryTooboxIcon").on("click", function () {
            $("#tapestryLeftBar").hide();
            $("#tapestryLeftBarMinimized").show();
            $("#tapestryWorkspace").css("left", 32);
            RecalculateToolboxHeight();
        });
        $("#showTapestryTooboxIcon").on("click", function () {
            $("#tapestryLeftBar").show();
            $("#tapestryLeftBarMinimized").hide();
            $("#tapestryWorkspace").css("left", 236);
            RecalculateToolboxHeight();
        });

        // Add rules
        $("#btnAddResRule").on("click", function () {
            ChangedSinceLastSave = true;
            rightmostRuleEdge = 0;
            $("#resourceRulesPanel .resourceRule").each(function (index, element) {
                edge = $(element).position().left + $(element).width() + $("#resourceRulesPanel .scrollContainer").scrollLeft();
                if (edge > rightmostRuleEdge)
                    rightmostRuleEdge = edge;
            });
            newRule = $('<div class="rule resourceRule" style="width: 350px; height: 60px; left: ' + (rightmostRuleEdge + 10) + 'px; top: 10px;"></div>');
            $("#resourceRulesPanel .scrollArea").append(newRule);
            newRule.draggable({
                containment: "parent",
                revert: function (event, ui) {
                    return ($(this).collision("#resourceRulesPanel .resourceRule").length > 1);
                },
                stop: function (event, ui) {
                    ChangedSinceLastSave = true;
                }
            });
            newRule.resizable({
                start: function (event, ui) {
                    rule = $(this);
                    contentsWidth = 120;
                    contentsHeight = 40;
                    rule.find(".item").each(function (index, element) {
                        rightEdge = $(element).position().left + $(element).width();
                        if (rightEdge > contentsWidth)
                            contentsWidth = rightEdge;
                        bottomEdge = $(element).position().top + $(element).height();
                        if (bottomEdge > contentsHeight)
                            contentsHeight = bottomEdge;
                    });
                    rule.css("min-width", contentsWidth + 40);
                    rule.css("min-height", contentsHeight + 20);

                    limits = CheckRuleResizeLimits(rule, true);
                    rule.css("max-width", limits.horizontal - 10);
                    rule.css("max-height", limits.vertical - 10);
                },
                resize: function (event, ui) {
                    rule = $(this);
                    limits = CheckRuleResizeLimits(rule, true);
                    rule.css("max-width", limits.horizontal - 10);
                    rule.css("max-height", limits.vertical - 10);
                },
                stop: function (event, ui) {
                    instance = $(this).data("jsPlumbInstance");
                    instance.recalculateOffsets();
                    instance.repaintEverything();
                    ChangedSinceLastSave = true;
                }
            });
            CreateJsPlumbInstanceForRule(newRule);
            newRule.droppable({
                containment: ".resourceRule",
                tolerance: "touch",
                accept: ".toolboxItem",
                greedy: true,
                drop: function (e, ui) {
                    if (dragModeActive) {
                        dragModeActive = false;
                        droppedElement = ui.helper.clone();
                        droppedElement.removeClass("toolboxItem");
                        droppedElement.addClass("item");
                        droppedElement.css({ width: "", height: "" });
                        $(this).append(droppedElement);
                        ruleContent = $(this);
                        leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left + 20;
                        topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top;
                        droppedElement.offset({ left: droppedElement.offset().left + leftOffset, top: droppedElement.offset().top + topOffset });
                        ui.helper.remove();
                        AddToJsPlumb(droppedElement);
                        if (droppedElement.position().left + droppedElement.width() + 35 > ruleContent.width()) {
                            droppedElement.css("left", ruleContent.width() - droppedElement.width() - 40);
                            instance = ruleContent.data("jsPlumbInstance");
                            instance.repaintEverything();
                        }
                        if (droppedElement.position().top + droppedElement.height() + 5 > ruleContent.height()) {
                            console.log("matched");
                            droppedElement.css("top", ruleContent.height() - droppedElement.height() - 15);
                            instance = ruleContent.data("jsPlumbInstance");
                            instance.repaintEverything();
                        }
                        ChangedSinceLastSave = true;
                    }
                }
            });
        });
        $("#btnAddWfRule").on("click", function () {
            ChangedSinceLastSave = true;
            lowestRuleBottom = 0;
            highestRuleNumber = 0;
            $("#workflowRulesPanel .workflowRule").each(function (index, element) {
                bottom = $(element).position().top + $(element).height() + $("#workflowRulesPanel .scrollContainer").scrollTop();
                if (bottom > lowestRuleBottom)
                    lowestRuleBottom = bottom;
                name = $(element).find(".workflowRuleHeader .verticalLabel").text();
                if (name.startsWith("Pravidlo ") && !isNaN(name.substring(9, name.length))) {
                    ruleNumber = parseInt(name.substring(9, name.length));
                    if (ruleNumber > highestRuleNumber)
                        highestRuleNumber = ruleNumber;
                }
            });
            newRule = $('<div class="rule workflowRule" style="width: 766px; height: 180px; left: 40px; top: ' + (lowestRuleBottom + 20) + 'px;"><div class="workflowRuleHeader"><div class="verticalLabel" style="margin-top: 0px;">Pravidlo ' + (highestRuleNumber + 1) + '</div>'
                + '</div><div class="swimlaneArea"><div class="swimlane" style="height: 100%;"><div class="swimlaneRolesArea"><div class="roleItemContainer"></div><div class="rolePlaceholder"><div class="rolePlaceholderLabel">Pokud chcete specifikovat roli<br />'
                + 'přetáhněte ji do této oblasti</div></div></div><div class="swimlaneContentArea"></div></div>'
                + '</div></div>');
            $("#workflowRulesPanel .scrollArea").append(newRule);
            newRule.draggable({
                containment: "parent",
                handle: ".workflowRuleHeader",
                revert: function (event, ui) {
                    return ($(this).collision("#workflowRulesPanel .workflowRule").length > 1);
                },
                stop: function (event, ui) {
                    ChangedSinceLastSave = true;
                }
            });
            newRule.resizable({
                start: function (event, ui) {
                    rule = $(this);
                    contentsWidth = 120;
                    contentsHeight = 40;
                    rule.find(".item").each(function (index, element) {
                        rightEdge = $(element).position().left + $(element).width();
                        if (rightEdge > contentsWidth)
                            contentsWidth = rightEdge;
                        bottomEdge = $(element).position().top + $(element).height();
                        if (bottomEdge > contentsHeight)
                            contentsHeight = bottomEdge;
                    });
                    rule.css("min-width", contentsWidth + 40);
                    rule.css("min-height", contentsHeight + 20);

                    limits = CheckRuleResizeLimits(rule, false);
                    rule.css("max-width", limits.horizontal - 10);
                    rule.css("max-height", limits.vertical - 10);
                },
                resize: function (event, ui) {
                    rule = $(this);
                    instance = rule.data("jsPlumbInstance");
                    instance.recalculateOffsets();
                    instance.repaintEverything();
                    limits = CheckRuleResizeLimits(rule, false);
                    rule.css("max-width", limits.horizontal - 10);
                    rule.css("max-height", limits.vertical - 10);
                },
                stop: function (event, ui) {
                    ChangedSinceLastSave = true;
                }
            });
            CreateJsPlumbInstanceForRule(newRule);
            newRule.find(".swimlaneRolesArea").droppable({
                containment: ".swimlaneContentArea",
                tolerance: "touch",
                accept: ".toolboxItem.roleItem",
                greedy: true,
                drop: function (e, ui) {
                    if (dragModeActive) {
                        dragModeActive = false;
                        roleExists = false;
                        $(this).find(".roleItem").each(function (index, element) {
                            if ($(element).text() == ui.helper.text())
                                roleExists = true;
                        });
                        if (!roleExists) {
                            droppedElement = ui.helper.clone();
                            $(this).find(".rolePlaceholder").remove();
                            $(this).find(".roleItemContainer").append($('<div class="roleItem">' + droppedElement.text() + '</div>'));
                            ui.helper.remove();
                            ChangedSinceLastSave = true;
                        }
                    }
                }
            });
            newRule.find(".swimlaneContentArea").droppable({
                containment: ".swimlaneContentArea",
                tolerance: "touch",
                accept: ".toolboxSymbol, .toolboxItem",
                greedy: false,
                drop: function (e, ui) {
                    if (dragModeActive) {
                        dragModeActive = false;
                        droppedElement = ui.helper.clone();
                        if (droppedElement.hasClass("roleItem")) {
                            ui.draggable.draggable("option", "revert", true);
                            return false;
                        }
                        ruleContent = $(this);
                        ruleContent.append(droppedElement);
                        if (droppedElement.hasClass("toolboxSymbol")) {
                            droppedElement.removeClass("toolboxSymbol ui-draggable ui-draggable-dragging");
                            droppedElement.addClass("symbol");
                            droppedElement.css({ height: "" });
                            leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left;
                            topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top;
                        }
                        else {
                            droppedElement.removeClass("toolboxItem");
                            droppedElement.addClass("item");
                            droppedElement.css({ width: "", height: "" });
                            leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left + 38;
                            topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top - 18;
                        }
                        droppedElement.offset({ left: droppedElement.offset().left + leftOffset, top: droppedElement.offset().top + topOffset });
                        ui.helper.remove();
                        AddToJsPlumb(droppedElement);
                        if (droppedElement.position().top + droppedElement.height() + 10 > ruleContent.height()) {
                            console.log("matched");
                            droppedElement.css("top", ruleContent.height() - droppedElement.height() - 20);
                            instance = ruleContent.parents(".workflowRule").data("jsPlumbInstance");
                            instance.repaintEverything();
                        }
                        ChangedSinceLastSave = true;
                    }
                }
            });
        });
        $(document).on("click", ".libraryItem", function () {
            currentLibraryItem = $(this);
            libId = currentLibraryItem.attr("libId");
            libType = currentLibraryItem.attr("libType");
            if (libId) {
                if (currentLibraryItem.hasClass("highlighted")) {
                    $('.tapestryToolbox .toolboxLi[libId="' + libId + '"]').remove();
                }
                else {
                    newToolboxLi = null;
                    if (libType == "action") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Actions"><div class="toolboxItem actionItem" actionId="' + currentLibraryItem.attr("actionId") + '"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_Attributes").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Actions").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "column-attribute") {
                        var isShared = "";
                        if (currentLibraryItem.attr("isShared") === "true") {
                            isShared = 'isShared="true"';
                        }
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Attributes"><div class="toolboxItem attributeItem tableAttribute" tableName="' + currentLibraryItem.attr("tableName") + '" '+isShared+' columnName="' + currentLibraryItem.attr("columnName") + '"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_UI").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Attributes").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "table-attribute") {
                        var isShared = "";
                        if (currentLibraryItem.attr("shared") === "true") {
                            isShared = 'shared="true"';
                        }
                        console.log("Transpiling: "+isShared);
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Attributes"><div class="toolboxItem attributeItem tableAttribute" tableName="' + currentLibraryItem.attr("tableName") + '" '+isShared+'><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_UI").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Attributes").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "view-attribute") {
                        var isShared = "";
                        if (currentLibraryItem.attr("isShared") === "true") {
                            isShared = 'isShared="true"';
                        }
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Attributes"><div class="toolboxItem attributeItem viewAttribute" tableName="' + currentLibraryItem.attr("tableName") + '" ' + isShared + '><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_UI").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Attributes").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "ui") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_UI"><div class="toolboxItem uiItem" pageId="' + currentLibraryItem.attr("pageId") + '" componentName="' + currentLibraryItem.attr("componentName") + '" isBootstrap="' + currentLibraryItem.attr('isBootstrap') + '"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_Roles").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_UI").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "page-ui") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_UI"><div class="toolboxItem uiItem pageUi" pageId="' + currentLibraryItem.attr("pageId") + '"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_Roles").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_UI").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "role") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Roles"><div class="toolboxItem roleItem"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_States").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Roles").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "state") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_States"><div class="toolboxItem stateItem" stateId="' + currentLibraryItem.attr("stateId") + '"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_Targets").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_States").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "target") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Targets"><div class="toolboxItem targetItem" targetId="' + currentLibraryItem.attr("targetId") + '"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_Templates").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Targets").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "template") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Templates"><div class="toolboxItem templateItem"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>')
                        $(".tapestryToolbox .toolboxCategoryHeader_Integrations").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Templates").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (["ldap", "ws", "smtp", "webdav", "ExtDB"].indexOf(libType) != -1) {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Integrations"><div class="toolboxItem integrationItem"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>')
                        $(".tapestryToolbox").append(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Integrations").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    if (newToolboxLi)
                        newToolboxLi.find(".toolboxItem").draggable({
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
            }
            $(this).toggleClass("highlighted");
        });
    }
});

var CurrentRule, CurrentItem, AssociatedPageIds = [], AssociatedTableName = [], AssociatedTableIds = [], CurrentTableColumnArray = [], RoleWhitelist = [], ModelTableIsShared, ModelTableName;

$(function () {
    if (CurrentModuleIs("tapestryModule")) {
        renameBlockDialog = $("#rename-block-dialog").dialog({
            autoOpen: false,
            width: 400,
            height: 190,
            buttons: {
                "Save": function () {
                    renameBlockDialog_SubmitData();
                },
                Cancel: function () {
                    renameBlockDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        renameBlockDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                renameBlockDialog.find("#block-name").val($("#blockHeaderBlockName").text());
            }
        });
        function renameBlockDialog_SubmitData() {
            renameBlockDialog.dialog("close");
            $("#blockHeaderBlockName").text(renameBlockDialog.find("#block-name").val());
            ChangedSinceLastSave = true;
        }
        chooseTableDialog = $("#choose-table-dialog").dialog({
            autoOpen: false,
            width: 450,
            height: 500,
            buttons: {
                "Change": function () {
                    chooseTableDialog_SubmitData();
                },
                Cancel: function () {
                    chooseTableDialog.dialog("close");
                }
            },
            open: function (event, ui) {
                $(this).find("#choice-table:first tbody:nth-child(2) tr").remove();
                tbody = $(this).find("#choice-table tbody:nth-child(2)");
                for (i = 1; i <= 5; i++)
                    tbody.append($('<tr class="tableNameRow formRow"><td>' + 'Table' + i + '</td></tr>'));
                $(document).on("click", "tr.tableNameRow", function (event) {
                    chooseTableDialog.find("#choice-table tbody:nth-child(2) tr").removeClass("highlightedRow");
                    $(this).addClass("highlightedRow");
                });
            }
        });
        function chooseTableDialog_SubmitData() {
            somethingWasAdded = false;
            selectedRow = chooseTableDialog.find("#choice-table:first tbody:nth-child(2) tr.highlightedRow");
            if (selectedRow.length) {
                chooseTableDialog.dialog("close");
                $("#headerTableName").text(selectedRow.find("td").text());
                ChangedSinceLastSave = true;
            }
            else
                alert("No table selected");
        }
        chooseEmailTemplateDialog = $("#choose-email-template-dialog").dialog({
            autoOpen: false,
            width: 450,
            height: 500,
            buttons: {
                "Choose": function () {
                    chooseEmailTemplateDialog_SubmitData();
                },
                Cancel: function () {
                    chooseEmailTemplateDialog.dialog("close");
                }
            },
            open: function (event, ui) {
                $(this).find("#choice-template:first tbody:nth-child(2) tr").remove();
                tbody = $(this).find("#choice-template tbody:nth-child(2)");
                for (i = 1; i <= 5; i++)
                    tbody.append($('<tr class="emailTemplateRow formRow" templateId="' + i + '"><td>' + 'Email template ' + i + '</td></tr>'));
                if (CurrentItem.data("emailTemplate"))
                    tbody.find('tr[templateId="' + CurrentItem.data("emailTemplate") + '"]').addClass("highlightedRow");
                $(document).on("click", "tr.emailTemplateRow", function (event) {
                    chooseEmailTemplateDialog.find("#choice-template tbody:nth-child(2) tr").removeClass("highlightedRow");
                    $(this).addClass("highlightedRow");
                });
            }
        });
        function chooseEmailTemplateDialog_SubmitData() {
            selectedRow = chooseEmailTemplateDialog.find("#choice-template:first tbody:nth-child(2) tr.highlightedRow");
            if (selectedRow.length) {
                CurrentItem.data("emailTemplate", selectedRow.attr("templateId"));
                chooseEmailTemplateDialog.dialog("close");
                ChangedSinceLastSave = true;
            }
            else
                alert("No template selected");
        }
        
        renameRuleDialog = $("#rename-rule-dialog").dialog({
            autoOpen: false,
            width: 400,
            height: 190,
            buttons: {
                "Save": function () {
                    renameRuleDialog_SubmitData();
                },
                Cancel: function () {
                    renameRuleDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        renameRuleDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                renameRuleDialog.find("#rule-name").val(CurrentRule.find(".workflowRuleHeader .verticalLabel").text());
            }
        });
        function renameRuleDialog_SubmitData() {
            renameRuleDialog.dialog("close");
            CurrentRule.find(".workflowRuleHeader .verticalLabel").text(renameRuleDialog.find("#rule-name").val());
            ChangedSinceLastSave = true;
        }
        historyDialog = $("#history-dialog").dialog({
            autoOpen: false,
            width: 700,
            height: 540,
            buttons: {
                "Load": function () {
                    historyDialog_SubmitData();
                },
                Cancel: function () {
                    historyDialog.dialog("close");
                }
            },
            open: function (event, ui) {
                historyDialog.find("#commit-table:first tbody:nth-child(2) tr").remove();
                historyDialog.find(" .spinner-2").show();
                historyDialog.data("selectedCommitId", null);
                appId = $("#currentAppId").val();
                blockId = $("#currentBlockId").val();
                $.ajax({
                    type: "GET",
                    url: "/api/tapestry/apps/" + appId + "/blocks/" + blockId + "/commits",
                    dataType: "json",
                    success: function (data) {
                        tbody = historyDialog.find("#commit-table tbody:nth-child(2)");
                        commitIdArray = [];

                        // Fill in the history rows
                        for (i = 0; i < data.length; i++) {
                            commitIdArray.push(data[i].Id);
                            if (data[i].CommitMessage != null)
                                tbody.append($('<tr class="commitRow"><td>' + data[i].TimeString
                                    + '</td><td>' + data[i].CommitMessage + '</td></tr>'));
                            else
                                tbody.append($('<tr class="commitRow"><td>' + data[i].TimeString
                                    + '</td><td style="color: darkgrey;">(no message)</td></tr>'));
                        }

                        // Highlight the selected row
                        $(document).on('click', 'tr.commitRow', function (event) {
                            historyDialog.find("#commit-table tbody:nth-child(2) tr").removeClass("highlightedRow");
                            $(this).addClass("highlightedRow");
                            var rowIndex = $(this).index();
                            historyDialog.data("selectedCommitId", commitIdArray[rowIndex]);
                        });

                        historyDialog.find(".spinner-2").hide();
                    }
                });
            }
        });
        function historyDialog_SubmitData() {
            if (historyDialog.data("selectedCommitId")) {
                historyDialog.dialog("close");
                if (ChangedSinceLastSave)
                    confirmed = confirm("Máte neuložené změny, opravdu si přejete tyto změny zahodit?");
                else
                    confirmed = true;
                if (confirmed) {
                    TB.load.loadBlock(historyDialog.data("selectedCommitId"));
                    //LoadBlock(historyDialog.data("selectedCommitId"));
                }
            }
            else
                alert("Please select a commit");
        }
        
        chooseScreensDialog = $("#choose-screens-dialog").dialog({
            autoOpen: false,
            width: 450,
            height: 550,
            buttons: {
                "Select": TB.screen.selectScreen,
                "Cancel": TB.screen.closeDialog
            }
        });
        
        tableAttributePropertiesDialog = $("#table-attribute-properties-dialog").dialog({
            autoOpen: false,
            width: 450,
            height: 500,
            buttons: {
                "Change": function () {
                    tableAttributePropertiesDialog_SubmitData();
                },
                Cancel: function () {
                    tableAttributePropertiesDialog.dialog("close");
                    CurrentItem.removeClass("activeItem");
                }
            },
            open: function (event, ui) {
                var formTable = tableAttributePropertiesDialog.find(".columnFilterTable tbody");
                formTable.find("tr").remove();
                $("#table-attribute-properties-dialog .spinner-2").show();
                $("#btnOpenTableConditions").hide();
                appId = $("#currentAppId").val();
                url = "/api/database/apps/" + appId + "/commits/latest",
                tableName = CurrentItem.attr("tableName");
                isShared = CurrentItem.attr("shared") === "true";
                $.ajax({
                    type: "GET",
                    url: url,
                    dataType: "json",
                    success: function (data) {
                        var targetList = data.Tables;
                        if (isShared) {
                            targetList = data.Shared.Tables;
                            console.log("Table is shared, targeting shared resources.");
                        }
                        CurrentTableColumnArray = [];
                        columnFilter = CurrentItem.data("columnFilter");
                        if (columnFilter == undefined)
                            columnFilter = [];
                        targetTable = targetList.filter(function (value, index, ar) {
                            console.log(value.Name + " == " + tableName + " = " + (value.Name == tableName));
                            return value.Name == tableName;
                        })[0];
                        if (targetTable == undefined)
                            alert("Požadovaná tabulka již není součástí schématu v Entitronu, nebo má nyní jiné Id.");
                        for (i = 0; i < targetTable.Columns.length; i++) {
                            newRow = $('<tr><td class="nameCell">' + targetTable.Columns[i].Name + '</td>'
                                + '<td><input type="checkbox" class="showColumnCheckbox"></input>Show</td></tr>');
                            formTable.append(newRow);
                            newRow.find(".showColumnCheckbox").prop("checked", columnFilter.indexOf(targetTable.Columns[i].Name) != -1);
                            CurrentTableColumnArray.push({ Id: targetTable.Columns[i].Id, Name: targetTable.Columns[i].Name, Type: targetTable.Columns[i].Type });
                        }
                        $("#btnOpenTableConditions").show();
                        $("#table-attribute-properties-dialog .spinner-2").hide();
                    }
                });
            }
        });
        function tableAttributePropertiesDialog_SubmitData() {
            columnFilter = [];
            formTable = tableAttributePropertiesDialog.find(".columnFilterTable .showColumnCheckbox").each(function (index, checkboxElement) {
                columnName = $(checkboxElement).parents("tr").find(".nameCell").text();
                if($(checkboxElement).is(":checked"))
                    columnFilter.push(columnName);
            });
            CurrentItem.data("columnFilter", columnFilter).removeClass("activeItem");;
            tableAttributePropertiesDialog.dialog("close");
        }
        uiitemPropertiesDialog = $("#uiItem-properties-dialog").dialog({
            autoOpen: false,
            width: 450,
            height: 180,
            buttons: {
                "Save": function () {
                    uiitemPropertiesDialog_SubmitData();
                },
                Cancel: function () {
                    uiitemPropertiesDialog.dialog("close");
                    CurrentItem.removeClass("activeItem");
                }
            },
            open: function (event, ui) {
                uiitemPropertiesDialog.find("#ajax-action").prop('checked', CurrentItem.data("isAjaxAction"));
            }
        });
        function uiitemPropertiesDialog_SubmitData() {
            CurrentItem.data("isAjaxAction", uiitemPropertiesDialog.find("#ajax-action").is(':checked'));
            uiitemPropertiesDialog.dialog("close");
            CurrentItem.removeClass("activeItem");
        }
        chooseTablesDialog = $("#choose-tables-dialog").dialog({
            autoOpen: false,
            width: 450,
            height: 550,
            buttons: {
                "Select": function () {
                    chooseTablesDialog_SubmitData();
                },
                Cancel: function () {
                    chooseTablesDialog.dialog("close");
                }
            },
            create: function () {
                $(document).on("click", "tr.tableRow", function (event) {
                    $(this).toggleClass("highlightedRow");
                });
            },
            open: function (event, ui) {
                chooseTablesDialog.find("#table-table:first tbody:nth-child(2) tr").remove();
                chooseTablesDialog.find(".spinner-2").show();
                appId = $("#currentAppId").val();
                url = "/api/database/apps/" + appId + "/commits/latest";
                $.ajax({
                    type: "GET",
                    url: url,
                    dataType: "json",
                    success: function (data) {
                        tbody = chooseTablesDialog.find("#table-table tbody:nth-child(2)");
                        for (i = 0; i < data.Tables.length; i++) {
                            newTableRow = $('<tr class="tableRow" tableId="' + data.Tables[i].Id + '"><td><span class="tableName">' + data.Tables[i].Name + '</span></td></tr>');
                            if (AssociatedTableName.indexOf(data.Tables[i].Name) != -1)
                                newTableRow.addClass("highlightedRow");
                            if (data.Tables[i].Name == ModelTableName)
                                newTableRow.find("td").append('<div class="modelMarker">Model</div>');
                            tbody.append(newTableRow);
                        }
                        for (i = 0; i < data.Shared.Tables.length; i++) {
                            newTableRow = $('<tr class="tableRow" tableId="' + data.Shared.Tables[i].Id + '"><td><span class="tableName" shared="true">' + data.Shared.Tables[i].Name + '</span></td></tr>');
                            if (AssociatedTableName.indexOf(data.Shared.Tables[i].Name) != -1)
                                newTableRow.addClass("highlightedRow");
                            if (data.Shared.Tables[i].Name == ModelTableName)
                                newTableRow.find("td").append('<div class="modelMarker">Model</div>');
                            tbody.append(newTableRow);
                        }
                        for (i = 0; i < SystemTables.length; i++) {
                            newTableRow = $('<tr class="tableRow" tableId="' + SystemTables[i].Name + '"><td><span class="tableName">' + SystemTables[i].Name + '</span></td></tr>');
                            if (AssociatedTableName.indexOf(SystemTables[i].Name) != -1)
                                newTableRow.addClass("highlightedRow");
                            if (SystemTables[i].Name == ModelTableName)
                                newTableRow.find("td").append('<div class="modelMarker">Model</div>');
                            tbody.append(newTableRow);
                        }
                        chooseTablesDialog.find(".spinner-2").hide();
                    }
                });
            }
        });
        function chooseTablesDialog_SubmitData() {
            chooseTablesDialog.find("#table-table:first tbody:nth-child(2) tr").hide();
            chooseTablesDialog.find(".spinner-2").show();
            appId = $("#currentAppId").val();
            url = "/api/database/apps/" + appId + "/commits/latest";
            $.ajax({
                type: "GET",
                url: url,
                dataType: "json",
                success: function (data) {
                    $("#libraryCategory-Attributes .columnAttribute").remove();
                    somethingWasAdded = false;
                    tableCount = 0;
                    AssociatedTableIds = [];
                    AssociatedTableName = [];
                    chooseTablesDialog.find("#table-table:first tbody:nth-child(2) tr").each(function (index, element) {
                        if ($(element).hasClass("highlightedRow")) {
                            tableCount++;
                            tableId = $(element).attr("tableId");
                            isShared = $(element).find("td .tableName").attr("shared") === "true";
                            tableName = $(element).find('td .tableName').text();
                            AssociatedTableIds.push(parseInt(tableId));
                            AssociatedTableName.push(tableName);
                            var targetList = data.Tables;
                            if (isShared) {
                                targetList = data.Shared.Tables;
                                console.log("Adding shared table columns");
                            }

                            console.log("Target:");
                            console.log($(element));
                            currentTable = targetList.filter(function (value) {
                                return value.Id == tableId;
                            })[0];
                            console.log(currentTable);
                            var sharedClass = "";
                            var prefix = "";
                            if (isShared) {
                                sharedClass = 'shared="true"';
                                prefix = "Shared: ";
                            }
                            if (currentTable)
                                for (i = 0; i < currentTable.Columns.length; i++) {
                                    console.log("Adding column: " + currentTable.Columns[i].Name);
                                    $("#libraryCategory-Attributes").append($('<div libId="' + ++lastLibId + '" libType="column-attribute" class="libraryItem columnAttribute" tableName="'
                                        + currentTable.Name + '" columnName="' + currentTable.Columns[i].Name + '" '+sharedClass+'>'+ prefix + currentTable.Name + '.' + currentTable.Columns[i].Name + '</div>'));
                                }
                            systemTable = SystemTables.filter(function (value) {
                                return value.Name == tableName;
                            })[0];
                            if(systemTable)
                                for (i = 0; i < systemTable.Columns.length; i++) {
                                    $("#libraryCategory-Attributes").append($('<div libId="' + ++lastLibId + '" libType="column-attribute" class="libraryItem columnAttribute" tableName="'
                                        + systemTable.Name + '" columnName="' + systemTable.Columns[i] + '">' + systemTable.Name + '.' + systemTable.Columns[i] + '</div>'));
                                }
                        }
                    });
                    modelMarker = chooseTablesDialog.find("#table-table:first tbody:nth-child(2) .modelMarker");
                    if (modelMarker.length) {
                        ModelTableName = modelMarker.parents("td").find(".tableName").text();
                        ModelTableIsShared = modelMarker.parents("td").find(".tableName").attr("shared") === "true";
                    }
                    $("#blockHeaderDbResCount").text(tableCount);
                    chooseTablesDialog.dialog("close");
                    chooseTablesDialog.find(".spinner-2").hide();
                }
            });
        }
        actionPropertiesDialog = $("#action-properties-dialog").dialog({
            autoOpen: false,
            width: 900,
            height: 200,
            buttons: {
                "Save": function () {
                    actionPropertiesDialog_SubmitData();
                },
                Cancel: function () {
                    actionPropertiesDialog.dialog("close");
                    CurrentItem.removeClass("activeItem");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        actionPropertiesDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                actionPropertiesDialog.find("#input-variables").val(CurrentItem.data("inputVariables"));
                actionPropertiesDialog.find("#output-variables").val(CurrentItem.data("outputVariables"));
            }
        });
        function actionPropertiesDialog_SubmitData() {
            CurrentItem.data("inputVariables", actionPropertiesDialog.find("#input-variables").val());
            CurrentItem.data("outputVariables", actionPropertiesDialog.find("#output-variables").val());
            actionPropertiesDialog.dialog("close");
        }
        labelPropertyDialog = $("#label-property-dialog").dialog({
            autoOpen: false,
            width: 900,
            height: 200,
            buttons: {
                "Save": function () {
                    labelPropertyDialog_SubmitData();
                },
                Cancel: function () {
                    labelPropertyDialog.dialog("close");
                    CurrentItem.removeClass("activeItem processedItem");
                }
            },
            open: function () {
                labelPropertyDialog.find("#label-input").val(CurrentItem.find(".itemLabel").text());
            }
        });
        function labelPropertyDialog_SubmitData() {
            CurrentItem.find(".itemLabel").text(labelPropertyDialog.find("#label-input").val());
            CurrentItem.removeClass("activeItem processedItem");
            labelPropertyDialog.dialog("close");
        }
        conditionsDialog = $("#conditions-dialog").dialog({
            autoOpen: false,
            width: 800,
            height: 560,
            buttons: {
                "Save": function () {
                    conditionsDialog_SubmitData();
                },
                Cancel: function () {
                    conditionsDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        conditionsDialog_SubmitData();
                        return false;
                    }
                });
                $(this).find(".addAndConditionSetIcon").on("click", function () {
                    newConditionSet = $(ConditionSetTemplate);
                    newConditionSet.find(".conditionSetPrefix").text("AND a");
                    newConditionSet.find(".conditionTable").append($(ConditionTemplate))
                    LoadConditionColumns(newConditionSet);
                    conditionsDialog.find(".conditionSetArea").append(newConditionSet);
                    if (newConditionSet.index() == 0)
                        newConditionSet.find(".conditionSetPrefix").text("A");
                });
                $(this).find(".addOrConditionSetIcon").on("click", function () {
                    newConditionSet = $(ConditionSetTemplate);
                    newConditionSet.find(".conditionSetPrefix").text("OR a");
                    newConditionSet.find(".conditionTable").append($(ConditionTemplate))
                    LoadConditionColumns(newConditionSet);
                    conditionsDialog.find(".conditionSetArea").append(newConditionSet);
                    if (newConditionSet.index() == 0)
                        newConditionSet.find(".conditionSetPrefix").text("A");
                });
                $(this).on("click", ".addAndConditionIcon", function () {
                    newCondition = $(ConditionTemplate);
                    newCondition.find(".conditionOperator").text("and");
                    LoadConditionColumns(newCondition);
                    $(this).parents("tr").after(newCondition);
                });
                $(this).on("click", ".addOrConditionIcon", function () {
                    newCondition = $(ConditionTemplate);
                    newCondition.find(".conditionOperator").text("or");
                    LoadConditionColumns(newCondition);
                    $(this).parents("tr").after(newCondition);
                });
                $(this).on("click", ".removeConditionIcon", function () {
                    currentCondition = $(this).parents("tr");
                    if (currentCondition.index() == 0)
                        currentCondition.parents("table").find("tr:eq(1)").find(".conditionOperator").text("");
                    if (currentCondition.parents("table").find("tr").length == 1) {
                        if (currentCondition.parents(".conditionSet").index() == 0)
                            currentCondition.parents(".conditionSetArea").find(".conditionSet:eq(1)").find(".conditionSetPrefix").text("A");
                        currentCondition.parents(".conditionSet").remove();
                    }
                    else
                        currentCondition.remove();
                });
                $(this).on("click", ".removeConditionSetIcon", function () {
                    currentConditionSet = $(this).parents(".conditionSet");
                    if (currentConditionSet.index() == 0)
                        currentConditionSet.parents(".conditionSetArea").find(".conditionSet:eq(1)").find(".conditionSetPrefix").text("A");
                    currentConditionSet.remove();
                });
                $(this).on("change", ".conditionVariableCell select", function () {
                    currentCondition = $(this).parents("tr");
                    var optionSelected = $("option:selected", this);
                    varType = optionSelected.attr("varType");
                    currentCondition.find(".conditionOperatorCell select, .conditionValueCell select, .conditionValueCell input").remove();
                    switch(varType) {
                        case "bool":
                            currentCondition.find(".conditionValueCell").append($('<select><option selected="selected">b$true</option><option>b$false</option></select>'));
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
                });
                $(this).on("change", ".conditionOperatorCell select", function () {
                    currentCondition = $(this).parents("tr");
                    var optionSelected = $("option:selected", this);
                    inputType = optionSelected.attr("inputType");
                    if (inputType === "none")
                        currentCondition.find(".conditionValueCell input, .conditionValueCell select").hide();
                    else
                        currentCondition.find(".conditionValueCell input, .conditionValueCell select").show();
                });
            },
            open: function () {
                conditionSetArea = conditionsDialog.find(".conditionSetArea");
                conditionSetArea.find(".conditionSet").remove();
                conditionSetData = CurrentItem.data("conditionSets");
                for (conditionSetIndex = 0; conditionSetIndex < conditionSetData.length; conditionSetIndex++) {
                    currentConditionSetData = conditionSetData[conditionSetIndex];
                    if (currentConditionSetData.SetRelation == "OR")
                        prefix = "OR a";
                    else
                        prefix = "AND a";
                    newConditionSet = $(ConditionSetTemplate);
                    newConditionSet.find(".conditionSetPrefix").text(prefix);
                    conditionSetArea.append(newConditionSet);
                    if (conditionSetIndex == 0)
                        newConditionSet.find(".conditionSetPrefix").text("A");
                    conditionTable = newConditionSet.find(".conditionTable");
                    for (conditionIndex = 0; conditionIndex < currentConditionSetData.Conditions.length; conditionIndex++)
                    {
                        currentConditionData = currentConditionSetData.Conditions[conditionIndex];
                        newCondition = $(ConditionTemplate);
                        if (conditionIndex > 0)
                            newCondition.find(".conditionOperator").text(currentConditionData.Relation.toLowerCase());
                        conditionTable.append(newCondition);
                        columnSelect = newCondition.find(".conditionVariableCell select");
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
                        columnSelect.val(currentConditionData.Variable);
                        var optionSelected = $("option:selected", columnSelect);
                        varType = optionSelected.attr("varType");
                        newCondition.find(".conditionOperatorCell select, .conditionValueCell select, .conditionValueCell input").remove();
                        conditionValueCell = newCondition.find(".conditionValueCell");
                        conditionOperatorCell = newCondition.find(".conditionOperatorCell");
                        switch (varType) {
                            case "bool":
                                conditionValueCell.append($('<select><option selected="selected">b$true</option><option>b$false</option></select>'));
                                conditionOperatorCell.append($('<select><option>==</option><option>!=</option></select>'));
                                conditionOperatorCell.find("select").val(currentConditionData.Operator);
                                break;
                            case "int":
                                conditionValueCell.append($('<input type="number"></input>'));
                                conditionOperatorCell.append($('<select><option>==</option><option>!=</option><option>&gt;</option><option>&gt;=</option><option>&lt;</option><option>&lt;=</option>'));
                                conditionOperatorCell.find("select").val(currentConditionData.Operator);
                                break;
                            case "string":
                                conditionValueCell.append($('<input type="text"></input>'));
                                conditionOperatorCell.append($('<select><option>==</option><option>!=</option><option>contains</option><option inputType="none">is empty</option><option inputType="none">is not empty</option></select>'));
                                conditionOperatorCell.find("select").val(currentConditionData.Operator);
                                break;
                            case "unknown":
                            default:
                                conditionValueCell.append($('<input type="text"></input>'));
                                conditionOperatorCell.append($('<select><option>==</option><option>!=</option><option>&gt;</option><option>&gt;=</option><option>&lt;</option><option>&lt;=</option><option>contains</option><option inputType="none">is empty</option><option inputType="none">is not empty</option></select>'));
                                conditionOperatorCell.find("select").val(currentConditionData.Operator);
                        }
                        var optionSelected = $("option:selected", conditionOperatorCell);
                        inputType = optionSelected.attr("inputType");
                        if (inputType === "none")
                            conditionValueCell.find("input, select").hide();
                        else {
                            if (conditionValueCell.find("input").length > 0) {
                                conditionValueCell.find("input").show();
                                conditionValueCell.find("input").val(currentConditionData.Value);
                            }
                            else if (conditionValueCell.find("select").length > 0) {
                                conditionValueCell.find("select").show();
                                conditionValueCell.find("select").val(currentConditionData.Value);
                            }
                        }
                    }
                }
            }
        });
        function conditionsDialog_SubmitData() {
            setArray = [];
            conditionsDialog.find(".conditionSet").each(function (setIndex, setElement) {
                currentSet = $(setElement);
                conditionArray = [];
                currentSet.find(".conditionTable tr").each(function (index, element) {
                    currentCondition = $(element);
                    relationCellValue = currentCondition.find(".conditionOperator").text();
                    if (relationCellValue == "")
                        relation = "AND";
                    else
                        relation = relationCellValue.toUpperCase();
                    if (currentCondition.find(".conditionValueCell select").length > 0)
                        value = currentCondition.find(".conditionValueCell select option:selected").text();
                    else
                        value = currentCondition.find(".conditionValueCell input").val();
                    conditionArray.push({
                        Index: index,
                        Relation: relation,
                        Variable: currentCondition.find(".conditionVariableCell select option:selected").text(),
                        Operator: currentCondition.find(".conditionOperatorCell select option:selected").text(),
                        Value: value
                    });
                });
                setPrefix = currentSet.find(".conditionSetPrefix").text();
                if (setPrefix == "OR a")
                    setRelation = "OR";
                else
                    setRelation = "AND";
                setArray.push({
                    SetIndex: setIndex,
                    SetRelation: setRelation,
                    Conditions: conditionArray
                });
            });
            CurrentItem.data("conditionSets", setArray);
            conditionsDialog.dialog("close");
        }
    }
    chooseWhitelistRolesDialog = $("#choose-whitelist-roles-dialog").dialog({
        autoOpen: false,
        width: 450,
        height: 500,
        buttons: {
            "Change": function () {
                chooseWhitelistRolesDialog_SubmitData();
            },
            Cancel: function () {
                chooseWhitelistRolesDialog.dialog("close");
            }
        },
        open: function (event, ui) {
            chooseWhitelistRolesDialog.find("#role-table:first tbody:nth-child(2) tr").remove();
            chooseWhitelistRolesDialog.find(".spinner-2").show();
            appId = $("#currentAppId").val();
            $.ajax({
                type: "GET",
                url: "/api/Persona/app-roles/" + appId,
                dataType: "json",
                success: function (data) {
                    tbody = chooseWhitelistRolesDialog.find("#role-table tbody:nth-child(2)");
                    for (i = 0; i < data.Roles.length; i++) {
                        newTableRow = $('<tr class="roleRow"><td>' + data.Roles[i].Name + '</td></tr>');
                        if (RoleWhitelist.indexOf(data.Roles[i].Name) != -1)
                            newTableRow.addClass("highlightedRow");
                        tbody.append(newTableRow);
                        newTableRow.on("click", function (event) {
                            $(this).toggleClass("highlightedRow");
                        });
                    }
                    chooseWhitelistRolesDialog.find(".spinner-2").hide();
                }
            });
        }
    });
    function chooseWhitelistRolesDialog_SubmitData() {
        RoleWhitelist = [];
        roleCount = 0;
        chooseWhitelistRolesDialog.find("#role-table:first tbody:nth-child(2) tr").each(function (index, element) {
            if ($(element).hasClass("highlightedRow")) {
                RoleWhitelist.push($(element).find("td").text());
                roleCount++;
            }
        });
        $("#blockHeaderRolesCount").text(roleCount);
        chooseWhitelistRolesDialog.dialog("close");
    }
    gatewayConditionsDialog = $("#gateway-conditions-dialog").dialog({
        autoOpen: false,
        width: 800,
        height: 560,
        buttons: {
            "Save": function () {
                gatewayConditionsDialog_SubmitData();
            },
            Cancel: function () {
                gatewayConditionsDialog.dialog("close");
                CurrentItem.removeClass("activeItem");
            }
        },
        create: function () {
            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    gatewayConditionsDialog_SubmitData();
                    return false;
                }
            });
            $(this).find(".addAndConditionSetIcon").on("click", function () {
                newConditionSet = $(ConditionSetTemplate);
                newConditionSet.find(".conditionSetPrefix").text("AND a");
                newConditionSet.find(".conditionTable").append($(ManualInputConditionTemplate));
                gatewayConditionsDialog.find(".conditionSetArea").append(newConditionSet);
                if (newConditionSet.index() == 0)
                    newConditionSet.find(".conditionSetPrefix").text("A");
            });
            $(this).find(".addOrConditionSetIcon").on("click", function () {
                newConditionSet = $(ConditionSetTemplate);
                newConditionSet.find(".conditionSetPrefix").text("OR a");
                newConditionSet.find(".conditionTable").append($(ManualInputConditionTemplate));
                gatewayConditionsDialog.find(".conditionSetArea").append(newConditionSet);
                if (newConditionSet.index() == 0)
                    newConditionSet.find(".conditionSetPrefix").text("A");
            });
            $(this).on("click", ".addAndConditionIcon", function () {
                newCondition = $(ManualInputConditionTemplate);
                newCondition.find(".conditionOperator").text("and");
                $(this).parents("tr").after(newCondition);
            });
            $(this).on("click", ".addOrConditionIcon", function () {
                newCondition = $(ManualInputConditionTemplate);
                newCondition.find(".conditionOperator").text("or");
                $(this).parents("tr").after(newCondition);
            });
            $(this).on("click", ".removeConditionIcon", function () {
                currentCondition = $(this).parents("tr");
                if (currentCondition.index() == 0)
                    currentCondition.parents("table").find("tr:eq(1)").find(".conditionOperator").text("");
                if (currentCondition.parents("table").find("tr").length == 1) {
                    if (currentCondition.parents(".conditionSet").index() == 0)
                        currentCondition.parents(".conditionSetArea").find(".conditionSet:eq(1)").find(".conditionSetPrefix").text("A");
                    currentCondition.parents(".conditionSet").remove();
                }
                else
                    currentCondition.remove();
            });
            $(this).on("click", ".removeConditionSetIcon", function () {
                currentConditionSet = $(this).parents(".conditionSet");
                if (currentConditionSet.index() == 0)
                    currentConditionSet.parents(".conditionSetArea").find(".conditionSet:eq(1)").find(".conditionSetPrefix").text("A");
                currentConditionSet.remove();
            });
            $(this).on("change", ".conditionOperatorCell select", function () {
                currentCondition = $(this).parents("tr");
                var optionSelected = $("option:selected", this);
                inputType = optionSelected.attr("inputType");
                if (inputType === "none")
                    currentCondition.find(".conditionValueCell input, .conditionValueCell select").hide();
                else
                    currentCondition.find(".conditionValueCell input, .conditionValueCell select").show();
            });
        },
        open: function () {
            conditionSetArea = gatewayConditionsDialog.find(".conditionSetArea");
            conditionSetArea.find(".conditionSet").remove();
            conditionSetData = CurrentItem.data("conditionSets");
            if (!conditionSetData)
                conditionSetData = [];
            for (conditionSetIndex = 0; conditionSetIndex < conditionSetData.length; conditionSetIndex++) {
                currentConditionSetData = conditionSetData[conditionSetIndex];
                if (currentConditionSetData.SetRelation == "OR")
                    prefix = "OR a";
                else
                    prefix = "AND a";
                newConditionSet = $(ConditionSetTemplate);
                newConditionSet.find(".conditionSetPrefix").text(prefix);
                conditionSetArea.append(newConditionSet);
                if (conditionSetIndex == 0)
                    newConditionSet.find(".conditionSetPrefix").text("A");
                conditionTable = newConditionSet.find(".conditionTable");
                for (conditionIndex = 0; conditionIndex < currentConditionSetData.Conditions.length; conditionIndex++) {
                    currentConditionData = currentConditionSetData.Conditions[conditionIndex];
                    newCondition = $(ManualInputConditionTemplate);
                    if (conditionIndex > 0)
                        newCondition.find(".conditionOperator").text(currentConditionData.Relation.toLowerCase());
                    conditionTable.append(newCondition);
                    columnSelect = newCondition.find(".conditionVariableCell input");
                    columnSelect.val(currentConditionData.Variable);
                    conditionOperatorCell = newCondition.find(".conditionOperatorCell");
                    conditionOperatorCell.find("select").val(currentConditionData.Operator);
                    conditionValueCell = newCondition.find(".conditionValueCell");
                    var optionSelected = $("option:selected", conditionOperatorCell);
                    inputType = optionSelected.attr("inputType");
                    if (inputType === "none")
                        conditionValueCell.find("input").hide();
                    else {
                        conditionValueCell.find("input").show();
                        conditionValueCell.find("input").val(currentConditionData.Value);
                    }
                }
            }
        }
    });
    function gatewayConditionsDialog_SubmitData() {
        setArray = [];
        gatewayConditionsDialog.find(".conditionSet").each(function (setIndex, setElement) {
            currentSet = $(setElement);
            conditionArray = [];
            currentSet.find(".conditionTable tr").each(function (index, element) {
                currentCondition = $(element);
                relationCellValue = currentCondition.find(".conditionOperator").text();
                if (relationCellValue == "")
                    relation = "AND";
                else
                    relation = relationCellValue.toUpperCase();
                conditionArray.push({
                    Index: index,
                    Relation: relation,
                    Variable: currentCondition.find(".conditionVariableCell input").val(),
                    Operator: currentCondition.find(".conditionOperatorCell select option:selected").text(),
                    Value: currentCondition.find(".conditionValueCell input").val()
                });
            });
            setPrefix = currentSet.find(".conditionSetPrefix").text();
            if (setPrefix == "OR a")
                setRelation = "OR";
            else
                setRelation = "AND";
            setArray.push({
                SetIndex: setIndex,
                SetRelation: setRelation,
                Conditions: conditionArray
            });
        });
        CurrentItem.data("conditionSets", setArray).removeClass("activeItem");
        gatewayConditionsDialog.dialog("close");
    }

    envelopeStartPropertiesDialog = $("#envelopeStart-properties-dialog").dialog({
        autoOpen: false,
        width: 450,
        height: 180,
        buttons: {
            "Save": function () {
                envelopeStartPropertiesDialog_SubmitData();
            },
            Cancel: function () {
                envelopeStartPropertiesDialog.dialog("close");
                CurrentItem.removeClass("activeItem");
            }
        },
        open: function (event, ui) {
            envelopeStartPropertiesDialog.find("#envelopeStartButtonName").val(CurrentItem.data("label"));
        }
    });
    function envelopeStartPropertiesDialog_SubmitData() {
        CurrentItem.data("label", envelopeStartPropertiesDialog.find("#envelopeStartButtonName").val());
        envelopeStartPropertiesDialog.dialog("close");
        CurrentItem.removeClass("activeItem");
    }

    circleEventPropertiesDialog = $("#circleEvent-properties-dialog").dialog({
        autoOpen: false,
        width: 450,
        height: 180,
        buttons: {
            "Save": function () {
                circleEventPropertiesDialog_SubmitData();
            },
            Cancel: function () {
                circleEventPropertiesDialog.dialog("close");
                CurrentItem.removeClass("activeItem");
            }
        },
        open: function (event, ui) {
            circleEventPropertiesDialog.find("#circleEventButtonName").val(CurrentItem.data("label"));
        }
    });
    function circleEventPropertiesDialog_SubmitData() {
        CurrentItem.data("label", circleEventPropertiesDialog.find("#circleEventButtonName").val());
        circleEventPropertiesDialog.dialog("close");
        CurrentItem.removeClass("activeItem");
    }
});

$(function () {
    if (CurrentModuleIs("tapestryModule")) {
    }
});

ConditionData = [];

ConditionsRelationList = ["AND", "OR", "XOR"];

ConditionsOperatorList = ["=", "!=", ">", ">=", "<", "<="];

FakeInputsForTesting = [
    "Username.Value",
    "Password.Value",
    "LogIn.Success",
    "LogIn.AttemptCount",
    "LogIn.UserGroup",
    "Global.BlockReferal",
    "Global.BlockStartTimestamp"
]

function FillConditionsForLogicTableRow(row) {
    row.find(".selectRelation option").remove();
    for (i = 0; i < ConditionsRelationList.length; i++) {
        row.find(".selectRelation").append(
            $('<option value="' + ConditionsRelationList[i] + '">' + ConditionsRelationList[i] + '</option>'));
    }
    row.find(".selectField option").remove();
    for (i = 0; i < FakeInputsForTesting.length; i++) {
        row.find(".selectField").append(
            $('<option value="' + FakeInputsForTesting[i] + '">' + FakeInputsForTesting[i] + '</option>'));
    }
    row.find(".selectOperator option").remove();
    for (i = 0; i < ConditionsOperatorList.length; i++) {
        row.find(".selectOperator").append(
            $('<option value="' + ConditionsOperatorList[i] + '">' + ConditionsOperatorList[i] + '</option>'));
    }
}

var TB = {

    onInit: [],

    changedSinceLastSave: false,

    init: function () {
        var self = TB;
        for (var i = 0; i < self.onInit.length; i++) {
            self.onInit[i]();
        }
    },

    checkRuleResizeLimits: function (rule, resourceRuleMode)
    {
        var horizontalLimit = 1000000;
        var verticalLimit = 1000000;

        var ruleLeft = rule.position().left;
        var ruleRight = ruleLeft + rule.width();
        var ruleTop = rule.position().top;
        var ruleBottom = rule.position().top + rule.height();

        $(resourceRuleMode ? "#resourceRulesPanel .resourceRule" : "#workflowRulesPanel .workflowRule").each(function (index, element) {
            var otherRule = $(element);
            if (otherRule.attr("id") != rule.attr("id")) {
                var otherRuleLeft = otherRule.position().left;
                var otherRuleRight = otherRuleLeft + otherRule.width();
                var otherRuleTop = otherRule.position().top;
                var otherRuleBottom = otherRule.position().top + otherRule.height();

                if (otherRuleTop < ruleBottom && otherRuleBottom > ruleTop
                    && otherRuleLeft + 30 > ruleRight && otherRuleLeft - ruleLeft < horizontalLimit)
                    horizontalLimit = otherRuleLeft - ruleLeft;
                if (otherRuleLeft < ruleRight && otherRuleRight > ruleLeft
                    && otherRuleTop  + 20 > ruleBottom && otherRuleTop - ruleTop < verticalLimit)
                    verticalLimit = otherRuleTop - ruleTop;
            }
        });
        return { horizontal: horizontalLimit, vertical: verticalLimit };
    },

    callHooks: function(hooks, context, params) {
        for (var i = 0; i < hooks.length; i++) {
            hooks[i].apply(context, params);
        }
    }
};


$(function () {
    if (CurrentModuleIs("tapestryModule")) {
        TB.init();
    }
    $(".librarySearchToggler").on("click", function () {
        var search = $(this).parent().parent().find(".librarySearch");
        if (search.hasClass("shown")) {
            search.addClass("hidden");
            search.removeClass("shown");
        } else {
            search.addClass("shown");
            search.removeClass("hidden");
        }
    });
    
    $(".librarySearch").on("keyup", function () {
        var category = $(this).parent();
        var items = category.find(".libraryItem");
        var value = $(this).val();
        items.each(function () {
            $(this).detach();
        });
        items.sort(function (a, b) {
            a = $(a).text();
            b = $(b).text();
            if (value !== "") {
                var aSim = similar(a, value);
                var bSim = similar(b, value);
                return aSim === bSim ? CompareAlphabetical(a, b) : aSim < bSim ? 1 : -1;
            } else {
                return CompareAlphabetical(a, b);
            }
        });
        items.each(function () {
            category.append($(this));
        });
    });
    function CompareAlphabetical(a, b) {
        return a === b ? 0 : a < b ? -1 : 1;
    }
    function similar(a, b) {
        var lengthA = a.length;
        var lengthB = b.length;
        var equivalency = 0;
        var minLength = (a.length > b.length) ? b.length : a.length;
        var maxLength = (a.length < b.length) ? b.length : a.length;
        for (var i = 0; i < minLength; i++) {
            if (a[i] == b[i]) {
                equivalency++;
            }
        }


        var weight = equivalency / maxLength;
        return (weight * 100);
    }
});

TB.screen = {

    init: function()
    {
        var self = TB.screen;

        $(document)
            .on('click', '#blockHeaderScreenCount', self.openDialog)
            .on('click', '#choose-screens-dialog tr.screenRow', self.toggleRow)
        ;
        
    },

    openDialog: function()
    {
        chooseScreensDialog.dialog('open');
        chooseScreensDialog.find("#screen-table tbody tr").remove();
        
        TB.screen.toggleSpinner(true);

        var appId = $("#currentAppId").val();
        $.ajax({
            type: "GET",
            url: "/api/mozaic-bootstrap/apps/" + appId + "/pages",
            dataType: "json",
            success: TB.screen.setData
        });
    },

    closeDialog: function()
    {
        chooseScreensDialog.dialog('close');
    },

    toggleRow: function() 
    {
        $(this).toggleClass("highlightedRow");
    },

    toggleSpinner: function(show) {
        chooseScreensDialog.find(".spinner-2").toggle(show);
    },

    setData: function(data)
    {
        var tbody = chooseScreensDialog.find('#screen-table tbody');
        for (i = 0; i < data.length; i++)
        {
            var row = $('<tr class="screenRow"></tr>');
            row.attr({
                'data-pageid': data[i].Id,
                'data-isbootstrap': data[i].IsBootstrap
            });

            row.append('<td>' + data[i].Name + '</td>');
            row.append('<td>' + (data[i].IsBootstrap ? 'yes' : 'no') + '</td>');

            if (!data[i].IsBootstrap && AssociatedPageIds.indexOf(data[i].Id) != -1) {
                row.addClass("highlightedRow");
            }
            if (data[i].IsBootstrap && AssociatedBootstrapPageIds.indexOf(data[i].Id) != -1) {
                row.addClass("highlightedRow");
            }
            tbody.append(row);
        }
        
        TB.screen.toggleSpinner(false);
    },

    selectScreen: function()
    {
        var self = TB.screen;
        self.toggleSpinner(true);

        var pageCount = $('#screen-table tbody tr.highlightedRow').length;
        var appId = $("#currentAppId").val();

        AssociatedPageIds = [];
        AssociatedBootstrapPageIds = [];

        $('#libraryCategory-UI .libraryItem').remove();
        
        $('#screen-table tbody tr.highlightedRow').each(function () {

            var pageId = $(this).attr('data-pageid');
            var isBootstrap = $(this).attr('data-isbootstrap') == 'true';
            var api = isBootstrap ? '/api/mozaic-bootstrap/apps/' : '/api/mozaic-editor/apps/';
            var callback = isBootstrap ? self.loadBootstrapComponents : self.loadLegacyComponents;

            if (isBootstrap) {
                AssociatedBootstrapPageIds.push(parseInt(pageId));
            }
            else {
                AssociatedPageIds.push(parseInt(pageId));
            }

            $.ajax({
                type: 'GET',
                url: api + appId + "/pages/" + pageId,
                dataType: 'json',
                async: false,
                success: callback
            });
        });

        $('#blockHeaderScreenCount').text(pageCount);
        self.toggleSpinner(false);
        chooseScreensDialog.dialog("close");
    },

    loadBootstrapComponents: function(data, state)
    {
        for (i = 0; i < data.Components.length; i++) {
            if (i == 0) {
                var label = 'Screen: ' + data.Name;
                var params = { pageId: data.Id, isBootstrap: true };
                var isUsed = state.filter ? state.filter(function (value) { return value.PageId == data.Id && (!value.ComponentName || value.ComponentName == "undefined") }).length : false;

                var libId = TB.library.createItem('UI', 'ui', params, label, '', isUsed);
                if (isUsed) {
                    TB.toolbox.createItem(libId, 'UI', 'uiItem', params, label);
                }
            }

            TB.screen.addComponent(data.Components[i], data.Id, state);
        }
    },

    addComponent: function(c, pageId, state)
    {
        if (c.ElmId != "") {
            var label = c.ElmId;
            var params = { pageId: pageId, componentName: label, isBootstrap: true };
            var isUsed = state.filter ? state.filter(function (value) { return value.PageId == pageId && value.ComponentName == label; }).length : false;

            var libId = TB.library.createItem('UI', 'ui', params, label, '', isUsed, c);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'UI', 'uiItem', params, label);
            }
        }

        if (c.UIC == 'ui|data-table')
        {
            var actionsText = TB.screen.getAttribute(c.Attributes, 'data-actions');
            if (actionsText !== -1) {
                var actions = JSON.parse(actionsText.replace(/'/g, '"'));
                for (var a in actions) {
                    var label = c.ElmId + '_' + actions[a].action;
                    var params = { pageId: pageId, componentName: label, isBootstrap: true };
                    var isUsed = state.filter ? state.filter(function (value) { return value.PageId == pageId && value.ComponentName == label; }).length : false;

                    var libId = TB.library.createItem('UI', 'ui', params, label, '', isUsed);
                    if (isUsed) {
                        TB.toolbox.createItem(libId, 'UI', 'uiItem', params, label);
                    }
                }
            }
        }

        if (c.ChildComponents) {
            for (var i = 0; i < c.ChildComponents.length; i++) {
                TB.screen.addComponent(c.ChildComponents[i], pageId, state);
            }
        }
    },

    loadLegacyComponents: function(data, state)
    {
        for (i = 0; i < data.Components.length; i++) {
            if (i == 0) {
                var label = 'Screen: ' + data.Name;
                var params = { pageId: data.Id, isBootstrap: false };
                var isUsed =  state.filter ? state.filter(function (value) { return value.PageId == data.Id && (!value.ComponentName || value.ComponentName == "undefined") }).length : false;

                var libId = TB.library.createItem('UI', 'ui', params, label, '', isUsed);
                if (isUsed) {
                    TB.toolbox.createItem(libId, 'UI', 'uiItem', params, label);
                }
            }
            
            TB.screen.addLegacyComponent(data.Components[i], data.Id, state);
        }
    },

    addLegacyComponent: function(c, pageId, state)
    {
        var label = c.Name;
        var params = { pageId: pageId, componentName: label, isBootstrap: false };
        var isUsed = state.filter ? state.filter(function (value) { return value.PageId == pageId && value.ComponentName == label; }).length : false;

        var libId = TB.library.createItem('UI', 'ui', params, label, '', isUsed, c);
        if (isUsed) {
            TB.toolbox.createItem(libId, 'UI', 'uiItem', params, label);
        }

        var actions = ['_EditAction', '_DetailsAction', '_DeleteAction', '_A_Action', '_B_Action'];
        
        if (c.Type == "data-table-with-actions") {
            for (var a = 0; a < actions.length; a++) {
                var label = c.Name + actions[a];
                var params = { pageId: pageId, componentName: label, isBootstrap: false };
                var isUsed = state.filter ? state.filter(function (value) { return value.PageId == pageId && value.ComponentName == label; }).length : false;

                var libId = TB.library.createItem('UI', 'ui', params, label, '', isUsed);
                if (isUsed) {
                    TB.toolbox.createItem(libId, 'UI', 'uiItem', params, label);
                }
            }
        }

        if (c.ChildComponents) {
            for (var i = 0; i < c.ChildComponents.length; i++) {
                TB.screen.addLegacyComponent(c.ChildComponents[i], pageId, state);
            }
        }
    },

    getAttribute: function(attrString, attrName)
    {
        var attrList = JSON.parse(attrString);
        for (var k in attrList) {
            if (attrList[k].name == attrName) {
                return attrList[k].value;
            }
        }
        return -1;
    }
};

TB.onInit.push(TB.screen.init);
TB.load = {

    libraryLoadList: {
        attributes: {
            url: '/api/database/apps/{appId}/commits/latest',
            success: 'librarySetAttributes',
        },
        actions: {
            url: '/api/tapestry/actions',
            success: 'librarySetActions'
        },
        roles: {
            url: '/api/Persona/app-roles/{appId}',
            success: 'librarySetRoles'
        },
        states: {
            url: '/api/Persona/app-states/{appId}',
            success: 'librarySetStates'
        },
        targets: {
            url: '/api/Tapestry/apps/{appId}/blocks',
            success: 'librarySetTargets'
        },
        templates: {
            url: '/api/Hermes/{appId}/templates',
            success: 'librarySetTemplates'
        },
        integrations: {
            url: '/api/Nexus/{appId}/gateways',
            success: 'librarySetIntegrations'
        }
    },
    toolboxState: null,
    data: null,

    onAttributesLoad: [],
    onLoadBlock: [],

    init: function () {

    },

    loadBlock: function (commitId) {
        pageSpinner.show();

        var appId = $('#currentAppId').val();
        var blockId = $('#currentBlockId').val();

        var url = '/api/tapestry/apps/' + appId + '/blocks/' + blockId + (commitId ? '/commits/' + commitId : '');

        $.ajax({
            type: 'GET',
            url: url,
            dataType: 'json',
            complete: pageSpinner.hide,
            success: TB.load.setData
        });
    },

    setData: function (data)
    {
        var self = TB.load;

        ChangedSinceLastSave = false; /// OBSOLATE
        TB.changedSinceLastSave = false;

        self.toolboxState = data.ToolboxState;
        self.data = data;

        self.clearBuilder();
        self.setBlockName(data.Name);
        self.setResourceRules(data.ResourceRules);
        self.setWorkflowRules(data.WorkflowRules);
        self.setRoleWhiteList(data.RoleWhitelist);
        self.loadPages();
        self.loadLibrary();

        TB.callHooks(self.onLoadBlock, data, []);
    },

    clearBuilder: function() {
        $('#resourceRulesPanel .resourceRule').remove();
        $('#workflowRulesPanel .workflowRule').remove();
    },

    setBlockName: function (name) {
        $('#blockHeaderBlockName').text(name);
    },

    setResourceRules: function(rrList)
    {
        for (var i = 0; i < rrList.length; i++)
        {
            var rule = TB.rr.create(rrList[i]);

            for (var j = 0; j < rrList[i].ResourceItems.length; j++) {
                TB.rr.createItem(rrList[i].ResourceItems[j], rule);
            }

            var currentInstance = rule.data("jsPlumbInstance");
            for (var c = 0; c < rrList[i].Connections.length; c++) {
                var currentConnectionData = rrList[i].Connections[c];
                currentInstance.connect({
                    uuids: ["resItem" + currentConnectionData.SourceId + "RightMiddle"], target: "resItem" + currentConnectionData.TargetId
                });
            }
        }
    },

    setWorkflowRules: function(wfrList) {
        for (var i = 0; i < wfrList.length; i++) {
            var rule = TB.wfr.create(wfrList[i]);

            for (var j = 0; j < wfrList[i].Swimlanes.length; j++) {
                TB.wfr.createSwimlane(wfrList[i].Swimlanes[j], wfrList[i].Swimlanes.length, rule);
            }

            var currentInstance = rule.data('jsPlumbInstance');
            for (var c = 0; c < wfrList[i].Connections.length; c++) {
                var currentConnectionData = wfrList[i].Connections[c];
                var sourceId = "wfItem" + currentConnectionData.SourceId;
                var targetId = "wfItem" + currentConnectionData.TargetId;
                var sourceEndpointUuid;
                if (currentConnectionData.SourceSlot == 1)
                    sourceEndpointUuid = "BottomCenter";
                else
                    sourceEndpointUuid = "RightMiddle";
                currentInstance.connect({ uuids: [sourceId + sourceEndpointUuid], target: targetId });
            }
        }
    },

    setRoleWhiteList: function (whitelist) {
        $('#blockHeaderRolesCount').text(whitelist.length); 
        RoleWhitelist = whitelist;
    },

    loadLibrary: function()
    {
        pageSpinner.show();
        var appId = $('#currentAppId').val();

        // CLEAN
        TB.library.clean();
        TB.toolbox.clean();

        // LOAD
        for (var k in TB.load.libraryLoadList) {
            var libraryType = TB.load.libraryLoadList[k];
            
            $.ajax({
                type: 'GET',
                url: libraryType.url.replace(/\{appId\}/, appId),
                dataType: 'json',
                success: TB.load[libraryType.success]
            });
        }
        pageSpinner.hide();
    },

    loadPages: function()
    {
        var self = TB.load;

        AssociatedPageIds = self.data.AssociatedPageIds;
        AssociatedBootstrapPageIds = self.data.AssociatedBootstrapPageIds;

        $("#blockHeaderScreenCount").text(AssociatedPageIds.length + AssociatedBootstrapPageIds.length);
        
        for (var i = 0; i < AssociatedPageIds.length; i++) {
            var pageId = AssociatedPageIds[i];

            self.libraryLoadList['page' + pageId] = {
                url: '/api/mozaic-editor/apps/{appId}/pages/' + pageId,
                success: 'librarySetPage'
            };
        }

        for (var i = 0; i < AssociatedBootstrapPageIds.length; i++) {
            var pageId = AssociatedBootstrapPageIds[i];

            self.libraryLoadList['bootstrapPage' + pageId] = {
                url: '/api/mozaic-bootstrap/apps/{appId}/pages/' + pageId,
                success: 'librarySetBootstrapPage'
            };
        }
    },

    librarySetAttributes: function(data)
    {
        var self = TB.load;
        var state = self.toolboxState ? self.toolboxState.Attributes : [];

        AssociatedTableIds = self.data.AssociatedTableIds;
        AssociatedTableName = self.data.AssociatedTableName;
        ModelTableName = self.data.ModelTableName;
        somethingWasAdded = false;
        $("#blockHeaderDbResCount").text(self.data.AssociatedTableName.length);

        for (var ti = 0; ti < data.Tables.length; ti++) 
        {
            var isUsed = state.filter(function (value) { return !value.ColumnName && value.TableName == data.Tables[ti].Name; }).length;
            var params = { tableName: data.Tables[ti].Name };
            var label = 'Table: ' + data.Tables[ti].Name;

            var libId = TB.library.createItem('Attributes', 'table-attribute', params, label, 'tableAttribute', isUsed, data.Tables[ti]);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Attributes', 'attributeItem tableAttribute', params, label);
            }
        }
        for (var vi = 0; vi < data.Views.length; vi++)
        {
            var isUsed = state.filter(function (value) { return !value.ColumnName && value.TableName == data.Views[vi].Name; }).length;
            var params = { tableName: data.Views[vi].Name };
            var label = 'View: ' + data.Views[vi].Name;

            var libId = TB.library.createItem('Attributes', 'view-attribute', params, label, 'viewAttribute', isUsed, data.Views[vi]);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Attributes', 'attributeItem viewAttribute', params, label);
            }
        }

        if(data.Shared != null) {
            // Display shared tables
            for (var ti = 0; ti < data.Shared.Tables.length; ti++) {
                var isUsed = state.filter(function (value) { return !value.ColumnName && value.TableName == data.Shared.Tables[ti].Name; }).length;
                var params = { tableName: data.Shared.Tables[ti].Name, shared: true };
                var label = 'Shared table: ' + data.Shared.Tables[ti].Name;

                var libId = TB.library.createItem('Attributes', 'table-attribute', params, label, 'tableAttribute sharedAttribute', isUsed, data.Shared.Tables[ti]);
                if (isUsed) {
                    TB.toolbox.createItem(libId, 'Attributes', 'attributeItem tableAttribute sharedAttribute', params, label);
                }
            }

            // Display shared views
            for (var vi = 0; vi < data.Shared.Views.length; vi++) {
                var isUsed = state.filter(function (value) { return !value.ColumnName && value.TableName == data.Shared.Views[vi].Name; }).length;
                var params = { tableName: data.Shared.Views[vi].Name, shared: true };
                var label = 'Shared view: ' + data.Shared.Views[vi].Name;

                var libId = TB.library.createItem('Attributes', 'view-attribute', params, label, 'viewAttribute sharedAttribute', isUsed, data.Shared.Views[vi]);
                if (isUsed) {
                    TB.toolbox.createItem(libId, 'Attributes', 'attributeItem viewAttribute sharedAttribute', params, label);
                }
            }

            for (var ti = 0; ti < self.data.AssociatedTableName.length; ti++) {
                var currentTable = data.Shared.Tables.filter(function (value) { return value.Name == self.data.AssociatedTableName[ti]; })[0];
                if (currentTable) {
                    for (var ci = 0; ci < currentTable.Columns.length; ci++) {
                        var isUsed = state.filter(function (value) {
                            return value.ColumnName == currentTable.Columns[ci].Name && value.TableName == currentTable.Name;
                        }).length;
                        var params = { tableName: currentTable.Name, columnName: currentTable.Columns[ci].Name, shared: true };
                        var label = currentTable.Name + '.' + currentTable.Columns[ci].Name;

                        var libId = TB.library.createItem('Attributes', 'column-attribute', params, label, 'columnAttribute sharedAttribute', isUsed, { Name: label });
                        if (isUsed) {
                            TB.toolbox.createItem(libId, 'Attributes', 'attributeItem tableAttribute sharedAttribute', params, label);
                        }
                    }
                }
            }
        }

        for (var ti = 0; ti < self.data.AssociatedTableName.length; ti++)
        {
            var currentTable = data.Tables.filter(function (value) { return value.Name == self.data.AssociatedTableName[ti]; })[0];
            if (currentTable)
            {
                for (var ci = 0; ci < currentTable.Columns.length; ci++)
                {
                    var isUsed = state.filter(function (value) {
                        return value.ColumnName == currentTable.Columns[ci].Name && value.TableName == currentTable.Name;
                    }).length;
                    var params = {tableName: currentTable.Name, columnName: currentTable.Columns[ci].Name };
                    var label = currentTable.Name + '.' + currentTable.Columns[ci].Name;

                    var libId = TB.library.createItem('Attributes', 'column-attribute', params, label, 'columnAttribute', isUsed, { Name: label });
                    if(isUsed) {
                        TB.toolbox.createItem(libId, 'Attributes', 'attributeItem tableAttribute', params, label);
                    }
                }
            }

            var systemTable = SystemTables.filter(function (value) { return value.Name == self.data.AssociatedTableName[ti]; })[0];
            if (systemTable)
            {
                for (var i = 0; i < systemTable.Columns.length; i++)
                {
                    var isUsed = state.filter(function (value) {
                        return value.ColumnName == systemTable.Columns[i].Name && value.TableName == systemTable.Name;
                    }).length;
                    var params = { tableName: systemTable.Name, columnName: systemTable.Columns[i].Name };
                    var label = systemTable.Name + '.' + systemTable.Columns[i].Name;

                    var libId = TB.library.createItem('Attributes', 'column-attribute', params, label, 'columnAttribute', isUsed, { Name: label });
                    if (isUsed) {
                        TB.toolbox.createItem(libId, 'Attributes', 'attributeItem tableAttribute', params, label);
                    }
                }
            }
        }

        TB.callHooks(self.onAttributesLoad, null, [data]);
    },

    librarySetActions: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.Actions : [];

        for (var i = 0; i < data.Items.length; i++)
        {
            var isUsed = state.filter(function (value) { return value.ActionId == data.Items[i].Id; }).length;
            var params = {actionId: data.Items[i].Id };
            var label = data.Items[i].Name;

            var libId = TB.library.createItem('Actions', 'action', params, label, '', isUsed, data.Items[i]);
            if(isUsed) {
                TB.toolbox.createItem(libId, 'Actions', 'actionItem', params, label);
            }
        }
    },

    librarySetRoles: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.Roles : [];

        for (var i = 0; i < data.Roles.length; i++)
        {
            var isUsed = state.filter(function (value) { return value.Label == data.Roles[i].Name; }).length;
            var params = {};
            var label = data.Roles[i].Name;

            var libId = TB.library.createItem('Roles', 'role', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Roles', 'roleItem', params, label);
            }
        }
    },

    librarySetStates: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.States : [];

        for (var i = 0; i < data.States.length; i++)
        {
            var isUsed = state.filter(function (value) { return value.StateId == data.States[i].Id; }).length;
            var params = {stateId: data.States[i].Id};
            var label = data.States[i].Name;

            var libId = TB.library.createItem('States', 'state', params, label, '', isUsed);
            if(isUsed) {
                TB.toolbox.createItem(libId, 'States', 'stateItem', params, label);
            }
        }
    },

    librarySetTargets: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.Targets : [];

        for (var i = 0; i < data.ListItems.length; i++) {
            var isUsed = state.filter(function (value) { return value.TargetId == data.ListItems[i].Id; }).length;
            var params = { targetId: data.ListItems[i].Id };
            var label = data.ListItems[i].Name;

            var libId = TB.library.createItem('Targets', 'target', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Targets', 'targetItem', params, label);
            }
        }
    },

    librarySetTemplates: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.Templates : [];

        for (var i = 0; i < data.length; i++) {
            var isUsed = state.filter(function (value) { return value.Label == data[i].Name; }).length;
            var params = {};
            var label = data[i].Name;

            var libId = TB.library.createItem('Templates', 'template', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Templates', 'templateItem', params, label);
            }
        }
    },

    librarySetIntegrations: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.Integrations : [];

        // LDAP
        for (var i = 0; i < data.Ldap.length; i++) {
            var params = {};
            var label = 'LDAP: ' + data.Ldap[i].Name;
            var isUsed = state.filter(function (value) { return value.Label == label; }).length;

            var libId = TB.library.createItem('Integration', 'ldap', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Integrations', 'integrationItem', params, label);
            }
        }

        // WS
        for (var i = 0; i < data.WS.length; i++) {
            var params = { libSubType: data.WS[i].Type };
            var label = 'WS: ' + data.WS[i].Name;
            var isUsed = state.filter(function (value) { return value.Label == label; }).length;
            
            var libId = TB.library.createItem('Integration', 'ws', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Integrations', 'integrationItem', params, label);
            }
        }

        // SMTP
        for (var i = 0; i < data.SMTP.length; i++) {
            var params = {};
            var label = 'SMTP: ' + data.SMTP[i].Name;
            var isUsed = state.filter(function (value) { return value.Label == label; }).length;
            
            var libId = TB.library.createItem('Integration', 'smtp', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Integrations', 'integrationItem', params, label);
            }
        }

        // WebDAV
        for (var i = 0; i < data.WebDAV.length; i++) {
            var params = {};
            var label = 'WebDAV: ' + data.WebDAV[i].Name;
            var isUsed = state.filter(function (value) { return value.Label == label; }).length;
            
            var libId = TB.library.createItem('Integration', 'WebDAV', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Integrations', 'integrationItem', params, label);
            }
        }

        // ExtDB
        for (var i = 0; i < data.ExtDB.length; i++) {
            var params = {};
            var label = 'ExtDB: ' + data.ExtDB[i].Name;
            var isUsed = state.filter(function (value) { return value.Label == label; }).length;

            var libId = TB.library.createItem('Integration', 'ExtDB', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Integrations', 'integrationItem', params, label);
            }
        }
    },

    librarySetPage: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.UiComponents : [];
        TB.screen.loadLegacyComponents(data, state);
    },

    librarySetBootstrapPage: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.UiComponents : [];
        TB.screen.loadBootstrapComponents(data, state);
    }
};

TB.onInit.push(TB.load.init);
TB.save = {

    onBeforeSave: [],
    onAfterSave: [],
    saveId: 0,

    toolboxMap: {
        Actions: 'actionItem',
        Attributes: 'attributeItem',
        UiComponents: 'uiItem',
        Roles: 'roleItem',
        States: 'stateItem',
        Targets: 'targetItem',
        Templates: 'templateItem',
        Integrations: 'integrationItem'
    },

    init: function () {
        var self = TB.save;
        $(document).on('click', '#btnSave', $.proxy(self._beforeSave, self));
    },

    saveBlock: function () {
        pageSpinner.show();
       
        $('.activeItem, .processedItem').removeClass('activeItem processedItem');

        var commitMessage = $(this).find('#message').val(),
            self = TB.save,
            rrList = [],
            wrList = [],
            toolboxState = {};

        // Sestavíme pole resource rules
        $('#resourceRulesPanel .resourceRule').each(function (index) {
            rrList.push(self.getResourceRule(this, index));
        });

        // Sestavíme pole workflow rules
        $('#workflowRulesPanel .workflowRule').each(function (index) {
            wrList.push(self.getWorkflowRule(this, index));
        });

        // Sestavíme toolbox state
        for (var k in self.toolboxMap) {
            toolboxState[k] = [];

            $('.tapestryToolbox .toolboxItem.' + self.toolboxMap[k]).each(function () {
                var item = $(this);

                toolboxState[k].push({
                    Label: item.find('.itemLabel').text(),
                    ActionId: item.attr('ActionId'),
                    TableName: item.attr('TableName') ? item.attr('TableName') : null,
                    IsShared: item.attr('shared') === 'true',
                    ColumnName: item.attr('ColumnName') ? item.attr('ColumnName') : null,
                    PageId: item.attr('PageId'),
                    ComponentName: item.attr('ComponentName') ? item.attr('ComponentName') : null,
                    IsBootstrap: GetIsBootstrap(item),
                    StateId: item.attr('StateId'),
                    TargetName: item.attr('TargetName') ? item.attr('TargetName') : null,
                    TargetId: item.attr('TargetId'),
                    TypeClass: self.toolboxMap[k]
                });
            });
        }

        // Sestavíme finální data
        var postData = {
            CommitMessage: commitMessage,
            Name: $('#blockHeaderBlockName').text(),
            ResourceRules: rrList,
            WorkflowRules: wrList,
            PortTargets: [],
            ModelTableName: ModelTableName,
            AssociatedTableName: AssociatedTableName,
            AssociatedPageIds: AssociatedPageIds,
            AssociatedBootstrapPageIds: AssociatedBootstrapPageIds,
            AssociatedTableIds: AssociatedTableIds,
            RoleWhitelist: RoleWhitelist,
            ToolboxState: toolboxState,
            ParentMetablockId: $('#parentMetablockId').val()
        };

        var appId = $('#currentAppId').val();
        var blockId = $('#currentBlockId').val();

        $.ajax({
            type: 'POST',
            url: '/api/tapestry/apps/' + appId + '/blocks/' + blockId,
            data: postData,
            complete: pageSpinner.hide,
            success: self._afterSave
        });

        TB.dialog.close.apply(this);
    },

    /*****************************************************************/
    /* GET RESOURCE RULES                                            */
    /*****************************************************************/
    getResourceRule: function(node, index) {
        var items = [],
            connections = [],
            rule = $(node),
            self = this;

        rule.find('.item').each(function () {
            items.push(self.getResourceItem(this));
        });

        $.each(rule.data('jsPlumbInstance').getAllConnections(), function () {
            connections.push(self.getConnection(this));
        });

        return {
            Id: index+1,
            Width: Math.round(rule.width()),
            Height: Math.round(rule.height()),
            PositionX: Math.round(rule.position().left),
            PositionY: Math.round(rule.position().top),
            ResourceItems: items,
            Connections: connections
        };
    },

    getResourceItem: function(node) {
        var item = $(node);
        item.attr('saveId', ++this.saveId);

        return {
            Id: item.attr('saveId'),
            Label: item.text(),
            TypeClass: GetItemTypeClass(item),
            PositionX: Math.round(item.position().left),
            PositionY: Math.round(item.position().top),
            ActionId: item.attr('actionid'),
            StateId: item.attr('stateid'),
            PageId: GetIsBootstrap(item) ? null : item.attr('pageId'),
            ComponentName: item.attr('componentName'),
            IsBootstrap: GetIsBootstrap(item),
            BootstrapPageId: GetIsBootstrap(item) ? item.attr('pageId') : null,
            TableName: item.attr('tableName'),
            IsShared: item.attr('shared') === 'true',
            ColumnName: item.attr('columnName'),
            ColumnFilter: item.data('columnFilter'),
            ConditionSets: item.data('conditionSets')
        };
    },

    getConnection: function(connection) {
        var source = $(connection.source),
            target = $(connection.target);

        return {
            SourceId: source.attr('saveId'),
            SourceSlot: 0,
            TargetId: target.attr('saveId'),
            TargetSlot: 0
        };
    },

    /*****************************************************************/
    /* GET WORKFLOW RULES                                            */
    /*****************************************************************/
    getWorkflowRule: function(node, index) {
        var swimlanes = [],
            connections = [],
            self = this,
            rule = $(node);

        rule.find('.swimlane').each(function (index) {
            swimlanes.push(self.getSwimlane(this, index));
        });

        $.each(rule.data('jsPlumbInstance').getAllConnections(), function () {
            var connection = self.getWorkflowConnection(this);
            if (connection !== false) {
                connections.push(connection);
            }
        });

        return {
            Id: index,
            Name: rule.find('.workflowRuleHeader .verticalLabel').text(),
            Width: Math.round(rule.width()),
            Height: Math.round(rule.height()),
            PositionX: parseInt(rule.css('left')),
            PositionY: parseInt(rule.css('top')),
            Swimlanes: swimlanes,
            Connections: connections
        };
    },

    getSwimlane: function(node, index) {
        var roles = [],
            items = [],
            subflows = [],
            foreachs = [],
            self = this,
            sl = $(node);

        sl.attr('swimlaneIndex', index);

        sl.find('.swimlaneRolesArea .roleItem').each(function () {
            roles.push($(this).text());
        });

        sl.find('.swimlaneContentArea > .subflow').each(function () {
            subflows.push(self.getSubflow(this));
        });

        sl.find('.swimlaneContentArea > .foreach').each(function () {
            foreachs.push(self.getForeach(this));
        });

        sl.find('.swimlaneContentArea > .item, .swimlaneContentArea > .symbol').each(function () {
            items.push(self.getWorkflowItem(this));
        });

        return {
            SwimlaneIndex: index,
            Height: Math.round(sl.height()),
            Roles: roles,
            WorkflowItems: items,
            Subflow: subflows,
            Foreach: foreachs
        };
    },

    getSubflow: function(node) {
        var items = [],
            self = this,
            subflow = $(node);

        subflow.attr('saveId', ++this.saveId);
        subflow.find('> .item, > .symbol').each(function () {
            items.push(self.getWorkflowItem(this));
        });

        return {
            Id: subflow.attr('saveId'),
            Name: subflow.find('> .subflowName').text(),
            Comment: subflow.find('> .subflowComment').text(),
            CommentBottom: subflow.find('> .subflowComment').hasClass('bottom'),
            PositionX: Math.round(subflow.position().left),
            PositionY: Math.round(subflow.position().top),
            Width: Math.round(subflow.width()),
            Height: Math.round(subflow.height()),
            WorkflowItems: items
        };
    },

    getForeach: function (node) {
        var items = [],
            self = this,
            foreach = $(node);

        foreach.attr('saveId', ++this.saveId);
        foreach.find('> .item, > .symbol').each(function () {
            items.push(self.getWorkflowItem(this));
        });
        
        return {
            Id: foreach.attr('saveId'),
            Name: foreach.find('> .foreachName').text(),
            Comment: foreach.find('> .foreachComment').text(),
            CommentBottom: foreach.find('> .foreachComment').hasClass('bottom'),
            PositionX: Math.round(foreach.position().left),
            PositionY: Math.round(foreach.position().top),
            Width: Math.round(foreach.width()),
            Height: Math.round(foreach.height()),
            WorkflowItems: items,
            DataSource: foreach.attr('data-datasource')
        };
    },

    getWorkflowItem: function(node) {
        var item = $(node);
        item.attr('saveId', ++this.saveId);

        return {
            Id: item.attr('saveId'),
            Label: item.find('.itemLabel').length ? item.find('.itemLabel').text() : item.data('label'),
            Name: item.find('.itemName').text(),
            Comment: item.find('.itemComment').text(),
            CommentBottom: item.find('.itemComment').hasClass('bottom'),
            TypeClass: GetItemTypeClass(item),
            DialogType: item.attr('dialogType'),
            StateId: item.attr('stateid'),
            TargetId: item.attr('targetid'),
            PositionX: Math.round(item.position().left),
            PositionY: Math.round(item.position().top),
            ActionId: item.attr('actionid'),
            InputVariables: item.data('inputVariables'),
            OutputVariables: item.data('outputVariables'),
            PageId: item.attr('pageId'),
            ComponentName: item.attr('componentName'),
            IsBootstrap: GetIsBootstrap(item),
            isAjaxAction: item.data('isAjaxAction'),
            Condition: item.data('condition'),
            ConditionSets: item.data('conditionSets'),
            SymbolType: item.attr('symbolType'),
            IsForeachStart: item.find('.fa-play').length > 0,
            IsForeachEnd: item.find('.fa-stop').length > 0
        };
    },

    getWorkflowConnection: function(connection) {
        var source = $(connection.source),
            target = $(connection.target),
            sourceEndpointUuid = connection.endpoints[0].getUuid();

        var sourceSlot = sourceEndpointUuid.match("BottomCenter$") ? 1 : 0;

        if (!source.hasClass('subSymbol')) {
            return {
                SourceId: source.attr("saveId"),
                SourceSlot: sourceSlot,
                TargetId: target.attr("saveId"),
                TargetSlot: 0
            };
        }
        return false;
    },

    /*****************************************************************/
    /* EVENTS                                                        */
    /*****************************************************************/
    _dialogOpen: function () {
        $(this).find('#message').val('');
    },

    _beforeSave: function () {
        var allow = true;
        for (var i = 0; i < this.onBeforeSave.length; i++) {
            allow = allow && this.onBeforeSave[i]();
        }

        if (allow) {
            TB.dialog.open('save');
        }
    },

    _afterSave: function (lastCommitId) {
        alert("The block has been successfully saved");

        ChangedSinceLastSave = false; /// OBSOLATE
        TB.changedSinceLastSave = false;
        TB.callHooks(TB.save.onAfterSave, this, [lastCommitId]);
    }
};

TB.onInit.push(TB.save.init);
TB.selection = {

    holdTimer: null,
    pos: [],
    target: null,
    itemsCache: [],
    selecting: false,
    selection: null,
    
    init: function () {
        var self = TB.selection;

        $(document)
            .on('mousedown', $.proxy(self._mouseDown, self))
            .on('mousemove', $.proxy(self._mouseMove, self))
            .on('mouseup', $.proxy(self._mouseUp, self))
            ;

        TB.load.onLoadBlock.push(self._blockLoad);
    },
    
    /******************************************************/
    /* SELECTION EVENTS                                   */
    /******************************************************/
    _blockLoad: function () {
        $('.swimlaneContentArea').css({
            '-webkit-touch-callout': 'none',
            '-webkit-user-select': 'none',
            '-khtml-user-select': 'none',
            '-moz-user-select': 'none',
            '-ms-user-select': 'none',
            'user-select': 'none'
        });
    },

    _mouseDown: function (e) {
        var target = $(e.target);
        var that = this;

        if (target.is('.swimlaneContentArea') && e.shiftKey && e.which === 1) {
            this.holdTimer = setTimeout(function () {
                that.target = target;
                that._mouseHold.apply(that, [e]);
            }, 50);
        }
    },

    _mouseHold: function (e) {
        e.preventDefault();
        e.stopPropagation();

        var x = (e.pageX - this.target.offset().left);
        var y = (e.pageY - this.target.offset().top);

        var items = this.target.find('> .item, > .symbol');
        for (var i = 0; i < items.length; i++) {
            this.itemsCache.push({
                element: items.eq(i),
                selected: items.eq(i).hasClass('nu-selected'),
                selecting: false,
                position: items[i].getBoundingClientRect()
            });
        }

        this.pos = [x, y];
        this.selecting = true;
        this._createSelection(x, y);
    },

    _mouseMove: function (e) {
        var pos = this.pos;
        if (!pos.length)
            return;

        e.preventDefault();
        e.stopPropagation();

        var x = e.pageX - this.target.offset().left;
        var y = e.pageY - this.target.offset().top;

        var newPos = [x, y],
            width = Math.abs(newPos[0] - pos[0]),
            height = Math.abs(newPos[1] - pos[1]),
            left, top;

        left = (newPos[0] < pos[0]) ? (pos[0] - width) : pos[0];
        top = (newPos[1] < pos[1]) ? (pos[1] - height) : pos[1];

        this._drawSelection(width, height, left, top);
        this._detectCollision();
    },

    _mouseUp: function (e) {
        clearTimeout(this.holdTimer);

        if (!this.pos.length) {
            if (e.which !== 3) {
                this._clearSelection();
            }
            return;
        }

        e.preventDefault();
        e.stopPropagation();

        this.selecting = false;
        this.selection.remove();

        var x = e.pageX - this.target.offset().left;
        var y = e.pageY - this.target.offset().top;

        if (x === this.pos[0] && y === this.pos[1]) {
            this._clearSelection();
        }

        this.target = null;
        this.pos = [];
    },

    _createSelection: function (x, y) {
        this.selection = $('<div class="nu-selection-box" />');

        this.selection.css({
            'position': 'absolute',
            'top': y + 'px',
            'left': x + 'px',
            'width': '0',
            'height': '0',
            'z-index': '999',
            'overflow': 'hidden'
        }).appendTo(this.target);
    },

    _drawSelection: function (width, height, x, y) {
        this.selection.css({
            'width': width,
            'height': height,
            'top': y,
            'left': x
        });
    },

    _clearSelection: function () {
        $('.nu-selected').removeClass('nu-selected');
    },

    _detectCollision: function () {
        var selector = this.selection[0].getBoundingClientRect(),
            dataLength = this.itemsCache.length;

        for (var i = dataLength - 1, item; item = this.itemsCache[i], i >= 0; i--) {
            var collided = !(selector.right < item.position.left ||
                selector.left > item.position.right ||
                selector.bottom < item.position.top ||
                selector.top > item.position.bottom);

            if (collided) {
                if (item.selected) {
                    item.element.removeClass('nu-selected');
                    item.selected = false;
                }
                if (!item.selected) {
                    item.element.addClass('nu-selected');
                    item.selected = true;
                }
            }
            else {
                if (this.selecting) {
                    item.element.removeClass('nu-selected');
                }
            }

        }
    }
}

TB.onInit.push(TB.selection.init);
TB.subflow = {

    target: null,

    contextItems: {
        'name': { name: 'Name...', icon: 'fa-tag' },
        'comment': { name: 'Comment...', icon: 'fa-comment' },
        'break': { name: 'Break subflow', icon: 'fa-object-ungroup' }
    },

    init: function () {
        var self = TB.subflow;
    },

    cannotBeGruped: function (key, options) {
        if(options.$trigger.is('.item') || options.$trigger.is('.symbol'))
            return options.$trigger.parents('.swimlane').find('.nu-selected').length == 0;
        else
            return options.$trigger.find('.nu-selected').length == 0;
    },

    groupToSubflow: function () {
        var self = TB.subflow;

        var target = this.is('.item') || this.is('.symbol') ? this.parents('.swimlane') : this;
        var items = target.find('.nu-selected');
        if (items.length) {
            var minX = null, minY = null, maxX = null, maxY = null;

            items.each(function () {
                var p = $(this).position();
                var w = $(this).outerWidth(false);
                var h = $(this).outerHeight(false);

                minX = minX == null || p.left < minX ? p.left : minX;
                minY = minY == null || p.top < minY ? p.top : minY;
                maxX = maxX == null || (p.left + w) > maxX ? p.left + w : maxX;
                maxY = maxY == null || (p.top + h) > maxY ? p.top + h : maxY;
            });

            var subflow = self.createSubflow({
                PositionX: minX - 15,
                PositionY: minY - 15,
                Width: maxX - minX + 30,
                Height: maxY - minY + 30,
                Id: ''
            }, target);

            target.find('.swimlaneContentArea').append(subflow);
            subflow.append(items);

            var sX = subflow.position().left;
            var sY = subflow.position().top;

            subflow.find('> .item, > .symbol').each(function () {
                var e = $(this);
                e.css({
                    top: e.position().top - sY - 1,
                    left: e.position().left - sX - 1
                });
            });

            self.alive(subflow);
            TB.selection._clearSelection();
            TB.changedSinceLastSave = true;
            ChangedSinceLastSave = true; /// OBSOLATE
        }
    },

    breakSubflow: function(subflow) {
        var sX = subflow.position().left + 1;
        var sY = subflow.position().top + 1;

        subflow.find('> .item, > .symbol').each(function () {
            $(this).css({
                left: '+=' + sX,
                top: '+=' + sY
            }).appendTo(subflow.parent());
        });

        subflow.remove();
        TB.changedSinceLastSave = true;
        ChangedSinceLastSave = true; /// OBSOLATE
    },

    createSubflow: function (subflowData, parentSwimlane) {
        var subflow = $('<div class="subflow" />');
        subflow.css({
            width: subflowData.Width,
            height: subflowData.Height,
            left: subflowData.PositionX,
            top: subflowData.PositionY
        }).attr({
            'data-subflowid': subflowData.Id
        });

        if (subflowData.Name) {
            subflow.append('<span class="subflowName">' + subflowData.Name + '</span>');
        }
        if (subflowData.Comment) {
            subflow.append('<span class="subflowComment' + (subflowData.CommentBottom ? ' bottom' : '') + (subflowData.Name ? ' withName' : '') + '">' + subflowData.Comment + '</span>');
        }

        parentSwimlane.find('.swimlaneContentArea').append(subflow);
        this.alive(subflow);

        return subflow;
    },

    alive: function(subflow) {
        subflow.resizable({});
        subflow.draggable({
            drag: this._subflowDrag,
            stop: this._subflowStop
        });
    },

    setName: function() {
        var self = TB.subflow;
        var name = $(this).find('#SubflowName').val();

        if (name.length) {
            if (!self.target.find('> .subflowName').length) {
                self.target.append('<span class="subflowName" />');
            }
            self.target.find('> .subflowName').html(name);
            self.target.find('> .subflowComment').addClass('withName');
        }
        else {
            self.target.find('> .subflowName').remove();
            self.target.find('> .subflowComment').removeClass('withName');
        }

        TB.dialog.close.apply(this);
    },

    setComment: function() {
        var self = TB.subflow;
        var comment = $(this).find('#SubflowComment').val();
        var commentBottom = $(this).find('#SubflowCommentBottom').is(':checked');

        if (comment.length) {
            if (!self.target.find('> .subflowComment').length) {
                self.target.append('<span class="subflowComment" />');
            }
            self.target.find('> .subflowComment')
                .html(comment)
                .toggleClass('bottom', commentBottom)
                .toggleClass('withName', self.target.find('> .subflowName').length > 0);
        }
        else {
            self.target.find('> .subflowComment').remove();
        }

        TB.dialog.close.apply(this);
    },

    /******************************************************/
    /* SUBFLOW EVENTS                                     */
    /******************************************************/
    _subflowDrag: function () {
        var rule = $(this).parents('.rule');
        rule.data('jsPlumbInstance').repaintEverything();
    },

    _subflowStop: function() {
        var instance = $(this).parents('.rule').data('jsPlumbInstance');
        instance.recalculateOffsets();
        instance.repaintEverything();
        TB.changedSinceLastSave = true;
    },

    _contextAction: function(key, options) {
        var self = TB.subflow;
        switch (key) {
            case 'name': {
                self.target = options.$trigger;
                TB.dialog.open('subflowName');
                break;
            }
            case 'comment': {
                self.target = options.$trigger;
                TB.dialog.open('subflowComment');
                break;
            }
            case 'break': {
                self.breakSubflow(options.$trigger);
                break;
            }
        }
    },

    _setNameOpen: function() {
        $(this).find('#SubflowName').val(TB.subflow.target.find('> .subflowName').text());
    },

    _setCommentOpen: function() {
        $(this).find('#SubflowComment').val(TB.subflow.target.find('> .subflowComment').text());
        $(this).find('#SubflowCommentBottom').prop('checked', TB.subflow.target.find('> .subflowComment.bottom').length > 0);
    }
}

TB.onInit.push(TB.subflow.init);
TB.foreach = {

    contextItems: {
        'datasource': { name: 'Set datasource...', icon: 'fa-database' },
        'name': { name: 'Name...', icon: 'fa-tag' },
        'comment': { name: 'Comment...', icon: 'fa-comment' },
        'break': { name: 'Break foreach', icon: 'fa-times-circle' }
    },

    target: null,
    id2VirtualId: {},

    init: function () {
        var self = TB.foreach;

        TB.wfr.onCreateRule.push(self._onCreateRule);
    },
    
    cannotBeGruped: function (key, options) {
        if (options.$trigger.is('.item') || options.$trigger.is('.symbol'))
            return options.$trigger.parents('.swimlane').find('.nu-selected').length == 0;
        else
            return options.$trigger.find('.nu-selected').length == 0;
    },

    isNotInForeach: function (key, options) {
        return !options.$trigger.parent().is('.foreach') || options.$trigger.is('.symbol');
    },

    groupToForeach: function () {
        var self = TB.foreach;

        var target = this.is('.item') || this.is('.symbol') ? this.parents('.swimlane') : this;
        var items = target.find('.nu-selected');
        if (items.length) {
            var minX = null, minY = null, maxX = null, maxY = null;

            items.each(function () {
                var p = $(this).position();
                var w = $(this).outerWidth(false);
                var h = $(this).outerHeight(false);

                minX = minX == null || p.left < minX ? p.left : minX;
                minY = minY == null || p.top < minY ? p.top : minY;
                maxX = maxX == null || (p.left + w) > maxX ? p.left + w : maxX;
                maxY = maxY == null || (p.top + h) > maxY ? p.top + h : maxY;
            });

            var foreach = self.createForeach({
                PositionX: minX - 20,
                PositionY: minY - 20,
                Width: maxX - minX + 40,
                Height: maxY - minY + 40,
                Id: ''
            }, target, items);
            
            var sX = foreach.position().left;
            var sY = foreach.position().top;

            foreach.find('> .item, > .symbol').each(function () {
                var e = $(this);
                e.css({
                    top: e.position().top - sY - 1,
                    left: e.position().left - sX - 1
                });
            });
            self.target = foreach;

            TB.selection._clearSelection();
            TB.dialog.open('foreachDatasource');
            TB.changedSinceLastSave = true;
            ChangedSinceLastSave = true; /// OBSOLATE
        }
    },

    breakForeach: function (foreach) {
        var sX = foreach.position().left + 1;
        var sY = foreach.position().top + 1;

        foreach.find('> .item, > .symbol').each(function () {
            $(this).css({
                left: '+=' + sX,
                top: '+=' + sY
            }).appendTo(foreach.parent()).find('.fa').remove();
        });

        var instance = foreach.parents('.rule').data('jsPlumbInstance');
        instance.removeAllEndpoints(foreach, true);
        
        foreach.remove();
        TB.changedSinceLastSave = true;
        ChangedSinceLastSave = true; /// OBSOLATE
    },

    createForeach: function (foreachData, parentSwimlane, items, blockLoad) {
        var foreach = $('<div class="foreach" />');
        foreach.css({
            width: foreachData.Width,
            height: foreachData.Height,
            left: foreachData.PositionX,
            top: foreachData.PositionY
        }).attr({
            'data-foreachid': foreachData.Id
        });

        if (foreachData.Id) {
            foreach.attr('id', 'wfItem' + this.id2VirtualId[foreachData.Id]);
        }
       
        if (items) {
            foreach.append(items);
        }

        if (foreachData.DataSource) {
            foreach.attr('data-datasource', foreachData.DataSource);
        }
        if (foreachData.Name) {
            foreach.append('<span class="foreachName">' + foreachData.Name + '</span>');
        }
        if (foreachData.Comment) {
            foreach.append('<span class="foreachComment' + (foreachData.CommentBottom ? ' bottom' : '') + (foreachData.Name ? ' withName' : '') + '">' + foreachData.Comment + '</span>');
        }

        foreach.append('<span class="fa fa-repeat fa-spin"></span>');
        parentSwimlane.find('.swimlaneContentArea').append(foreach);
        this.alive(foreach, blockLoad);

        return foreach;
    },

    alive: function (foreach, blockLoad) {
    
        var itemID = foreach.attr('id') || AssingID();

        foreach.attr('id', itemID);
        foreach.resizable({
            resize: this._foreachResize,
            stop: this._foreachResizeStop
        });
        foreach.draggable({
            drag: this._foreachDrag,
            stop: this._foreachDragStop
        });
        foreach.droppable({
            tolerance: 'touch',
            accept: '.toolboxSymbol, .toolboxItem',
            greedy: true,
            drop: TB.wfr._swimlaneItemDrop
        });

        var instance = foreach.parents('.rule').data('jsPlumbInstance');
        instance.addEndpoint(itemID, sourceEndpoint, { anchor: 'RightMiddle', uuid: itemID + "RightMiddle" });
        instance.makeTarget(foreach, {
            dropOptions: { hoverClass: 'dragHover' },
            anchor: 'Continuous',
            allowLoopback: false
        });

        if (blockLoad) {
            return;
        }

        // Nějaký podivný bug - musíme si connections nejdřív naklonovat
        var connections = [];
        $.each(instance.getAllConnections(), function () {
            connections.push(this);
        });
        var s = '#' + itemID;
        var errors = [];

        // Přepojíme šipky, pokud existují
        $.each(connections, function () {
            var source = $(this.source);
            var target = $(this.target);
            
            // Zdroj je mimo FE - cíl je ve FE (považujme ho za počátek)
            if (!source.parents(s).length && target.parents(s).length) {
                var type = this.getType();
                var uuids = this.getUuids();

                if (target.is('.symbol')) {
                    errors.push('Cyklus nemůže začínat symbolem. Upravte spojení manuálně a označte počáteční akci.');
                }
                else if (!foreach.find('.fa-play').length) {
                    target.append('<span class="fa fa-play"></span>');
                    jsPlumb.detach(this);
                    instance.connect({
                        source: source[0],
                        target: foreach[0],
                        uuids: [uuids[0], null],
                        type: type
                    });
                }
                else {
                    foreach.find('.fa-play').remove();
                    errors.push('Nelze jednoznačně určit počáteční akci. Upravte spojení manuálně a označte počáteční akci.');
                }
            }
            // Zdroj je ve FE - cíl je mimo FE (považujme ho za konec)  
            if (source.parents(s).length && !target.parents(s).length) {
                var type = this.getType();

                if (target.is('.symbol')) {
                    errors.push('Cyklus nemůže končit symbolem. Upravte spojení manuálně a označte koncovou akci.');
                }
                else if (!foreach.find('.fa-stop').length) {
                    source.append('<span class="fa fa-stop"></span>');
                    jsPlumb.detach(this);
                    instance.connect({
                        source: foreach[0],
                        target: target[0],
                        uuids: [itemID + "RightMiddle", null],
                        type: type
                    });
                }
                else {
                    foreach.find('.fa-stop').remove();
                    errors.push('Nelze jednoznačně určit koncovou akci. Upravte spojení manuálně a označte koncovou akci.');
                }
            }
        });

        if (!foreach.find('.fa-play').length) {
            errors.push('Nebyl nalezen začátek cyklu. Upravte spojení manuálně a označte počáteční akci.');
        }
        if (!foreach.find('.fa-stop').length) {
            errors.push('Nebyl nalezen konec cyklu. Upravte spojení mauálně a označte koncovou akci.');
        }

        if (errors.length) {
            var d = $('<div title="' + document.title + ' saying"></div>');
            d.append('<p class="text-danger text-nowrap" style="margin:0">' + errors.join('<br>') + '</p>');

            d.dialog({
                resizable: false,
                draggable: false,
                modal: true,
                width: 'auto',
                minHeight: 0
            });
        }
    },

    setStart: function () {
        if (!this.parent().is('.foreach'))
            return;

        this.parent().find('> .item .fa-play').remove();
        this.find('.fa').remove().end().append('<span class="fa fa-play"></span>');
    },

    setEnd: function () {
        if (!this.parent().is('.foreach'))
            return;

        this.parent().find('> .item .fa-stop').remove();
        this.find('.fa').remove().end().append('<span class="fa fa-stop"></span>');
    },

    setDatasource: function () {
        var self = TB.foreach;
        var datasource = $(this).find('#ForeachDatasource').val();

        self.target.attr('data-datasource', datasource);
        TB.dialog.close.apply(this);
    },

    setName: function () {
        var self = TB.foreach;
        var name = $(this).find('#ForeachName').val();

        if (name.length) {
            if (!self.target.find('> .foreachName').length) {
                self.target.append('<span class="foreachName" />');
            }
            self.target.find('> .foreachName').html(name);
            self.target.find('> .foreachComment').addClass('withName');
        }
        else {
            self.target.find('> .foreachName').remove();
            self.target.find('> .foreachComment').removeClass('withName');
        }

        TB.dialog.close.apply(this);
    },

    setComment: function () {
        var self = TB.foreach;
        var comment = $(this).find('#ForeachComment').val();
        var commentBottom = $(this).find('#ForeachCommentBottom').is(':checked');

        if (comment.length) {
            if (!self.target.find('> .foreachComment').length) {
                self.target.append('<span class="foreachComment" />');
            }
            self.target.find('> .foreachComment')
                .html(comment)
                .toggleClass('bottom', commentBottom)
                .toggleClass('withName', self.target.find('> .foreachName').length > 0);
        }
        else {
            self.target.find('> .foreachComment').remove();
        }

        TB.dialog.close.apply(this);
    },

    /******************************************************/
    /* Foreach EVENTS                                     */
    /******************************************************/
    
    _foreachDrag: function () {
        var rule = $(this).parents('.rule');
        rule.data('jsPlumbInstance').repaintEverything();
    },

    _foreachDragStop: function () {
        var instance = $(this).parents('.rule').data('jsPlumbInstance');
        instance.recalculateOffsets();
        instance.repaintEverything();
        TB.changedSinceLastSave = true;
    },

    _foreachResize: function () {
        var rule = $(this).parents('.rule');
        rule.data('jsPlumbInstance').repaintEverything();
    },

    _foreachResizeStop: function () {
        var instance = $(this).parents('.rule').data('jsPlumbInstance');
        instance.recalculateOffsets();
        instance.repaintEverything();
        TB.changedSinceLastSave = true;
    },

    _contextAction: function (key, options) {
        var self = TB.foreach;
        switch (key) {
            case 'datasource': {
                self.target = options.$trigger;
                TB.dialog.open('foreachDatasource');
                break;
            }
            case 'name': {
                self.target = options.$trigger;
                TB.dialog.open('foreachName');
                break;
            }
            case 'comment': {
                self.target = options.$trigger;
                TB.dialog.open('foreachComment');
                break;
            }
            case 'break': {
                self.breakForeach(options.$trigger);
                break;
            }
        }
    },

    _setDatasourceOpen: function () {
        var t = TB.foreach.target;
        $(this).find('#ForeachDatasource').val(t.attr('data-datasource'));

        var categoryName = 'Workflow: ' + t.parents('.rule').find('.verticalLabel').text();
        var variables = [];

        for (var i = 0; i < TB.wizard.variableList.length; i++) {
            var v = TB.wizard.variableList[i];
            if (v.category == categoryName) {
                variables.push(v);
            }
        }

        $(this).find('#ForeachDatasource').autocomplete({
            delay: 0,
            source: variables
        });
    },

    _setNameOpen: function () {
        $(this).find('#ForeachName').val(TB.foreach.target.find('> .foreachName').text());
    },

    _setCommentOpen: function () {
        $(this).find('#ForeachComment').val(TB.foreach.target.find('> .foreachComment').text());
        $(this).find('#ForeachCommentBottom').prop('checked', TB.foreach.target.find('> .foreachComment.bottom').length > 0);
    },

    _beforeConnectionDrop: function (info) {
        console.log(info);

        if ($('#' + info.sourceId).parent().is('#' + info.targetId)) {
            return false;
        }

        return true;
    },

    _onCreateRule: function () {
        var instance = this.data('jsPlumbInstance');
        instance.bind('beforeDrop', TB.foreach._beforeConnectionDrop);
    }
}

TB.onInit.push(TB.foreach.init);
TB.rr = {

    templates: {
        rule: '<div class="rule resourceRule"></div>',
        item: '<div class="item"></div>'
    },

    contextItems: {
        'delete': { name: 'Delete rule', icon: 'delete' }
    },

    create: function(rrData)
    {
        var self = TB.rr;

        var rule = $(self.templates.rule);
        rule.css({
            width: rrData.Width,
            height: rrData.Height,
            left: rrData.PositionX,
            top: rrData.PositionY
        }).attr('id', AssingID()).appendTo('#resourceRulesPanel .scrollArea');

        self.alive(rule);

        return rule;
    },

    createItem: function(itemData, parentRule)
    {
        var self = TB.rr;

        var item = $(self.templates.item);

        var attrs = {};
        attrs.id = 'resItem' + itemData.Id;

        if (itemData.ActionId != null)          attrs.actionId = itemData.ActionId;
        if (itemData.StateId != null)           attrs.stateId = itemData.StateId;
        if (itemData.PageId != null)            attrs.pageId = itemData.PageId;
        if (itemData.ComponentName != null)     attrs.componentName = itemData.ComponentName;
        if (itemData.ColumnName != null)        attrs.columnName = itemData.ColumnName;
        if (itemData.IsShared != null)          attrs.shared = itemData.IsShared;
        if (itemData.IsBootstrap != null)       attrs.isBootstrap = itemData.IsBootstrap;
        if (itemData.BootstrapPageId != null)   attrs.pageId = itemData.BootstrapPageId;

        item
            .attr(attrs)
            .css({
                left: itemData.PositionX,
                top: itemData.PositionY
            })
            .html(itemData.Label)
            .addClass(itemData.TypeClass)
            .appendTo(parentRule);

        if (itemData.ConditionSets != null) {
            item.data('conditionSets', itemData.ConditionSets);
        }
        if (itemData.TableName != null) {
            item.data('columnFilter', itemData.ColumnFilter);
            item.attr('tableName', itemData.TableName);
            if (itemData.Label.indexOf('View:') == 0)
                item.addClass('viewAttribute');
            else
                item.addClass('tableAttribute');
        }

        AddToJsPlumb(item);
    },

    alive: function (rule) {
        var self = TB.rr;
        
        rule.draggable({
            containment: 'parent',
            revert: self._draggableRevert,
            stop: self._draggableStop
        });

        rule.resizable({
            start: self._resizableStart,
            stop: self._resizableStop
        });

        rule.droppable({
            containment: '.resourceRule',
            tolerance: 'touch',
            accept: '.toolboxItem',
            greedy: true,
            drop: self._droppableDrop
        });

        CreateJsPlumbInstanceForRule(rule);
    },

    _draggableRevert: function(event, ui) {
        return ($(this).collision('#resourceRulesPanel .resourceRule').length > 1);
    },

    _draggableStop: function(event, ui) {
        ChangedSinceLastSave = true; /// OBSOLATE
        TB.changedSinceLastSave = true;
    },

    _resizableStart: function(event, ui) {
        var rule = $(this);
        var minWidth = 120;
        var minHeight = 40;

        rule.find('.item').each(function (index, element) {
            var $elm = $(element);
            var rightEdge = $elm.position().left + $elm.width();
            var bottomEdge = $elm.position().top + $elm.height();

            minWidth = rightEdge > minWidth ? rightEdge : minWidth;
            minHeight = bottomEdge > minHeight ? bottomEdge : minHeight;
        });

        var limits = TB.checkRuleResizeLimits(rule, true);

        rule.css({
            'min-width': minWidth + 40,
            'min-height': minHeight + 20,
            'max-width': limits.horizontal - 10,
            'max-height': limits.vertical - 10
        });
    },

    _resizableStop: function(event, ui) {
        var instance = $(this).data("jsPlumbInstance");
        instance.recalculateOffsets();
        instance.repaintEverything();
        ChangedSinceLastSave = true; /// OBSOLATE
        TB.changedSinceLastSave = true;
    },

    _droppableDrop: function (event, ui) {
        if (dragModeActive) {
            dragModeActive = false;

            var target = $(this);

            var leftOffset = $("#tapestryWorkspace").offset().left - target.offset().left + 20;
            var topOffset = $("#tapestryWorkspace").offset().top - target.offset().top;

            var item = ui.helper.clone();
            item.removeClass('toolboxItem')
                .addClass('item')
                .css({ width: '', height: '' })
                .appendTo(target);

            item.offset({
                left: item.offset().left + leftOffset,
                top: item.offset().top + topOffset
            });
            
            ui.helper.remove();
            AddToJsPlumb(item);

            if (item.position().left + item.width() + 35 > target.width()) {
                item.css("left", target.width() - item.width() - 40);
                var instance = target.data("jsPlumbInstance");
                instance.repaintEverything();
            }
            if (item.position().top + item.height() + 5 > target.height()) {
                item.css("top", target.height() - item.height() - 15);
                var instance = target.data("jsPlumbInstance");
                instance.repaintEverything();
            }
            ChangedSinceLastSave = true; /// OBSOLATE
            TB.changedSinceLastSave = true;
        }
    },

    _contextAction: function (key, options) {
        var item = options.$trigger;
        if (key == "delete") {
            item.remove();
            ChangedSinceLastSave = true; /// OBSOLATE
            TB.changedSinceLastSave = true;
        }
    }
};
TB.wfr = {

    onCreateItem: [],
    onCreateRule: [],

    templates: {
        rule: '<div class="rule workflowRule"><div class="workflowRuleHeader"><div class="verticalLabel" style="margin-top: 0px;"></div></div><div class="swimlaneArea"></div></div>',
        swimlane: '<div class="swimlane"><div class="swimlaneRolesArea"><div class="roleItemContainer"></div><div class="rolePlaceholder"><div class="rolePlaceholderLabel">Pokud chcete specifikovat roli<br />'
            + 'přetáhněte ji do této oblasti</div></div></div><div class="swimlaneContentArea"></div></div>',
        item: ''
    },
    
    contextItems: {
        'add-swimlane': { name: 'Add swimlane', icon: 'fa-plus' },
        'rename': { name: 'Rename rule...', icon: 'fa-edit' },
        'copy': { name: 'Copy rule...', icon: 'fa-clone' },
        'delete': { name: 'Delete rule', icon: 'fa-trash' }
    },

    swimlaneContextItems: {
        'group-to-subflow': { name: 'Group to subflow', icon: 'fa-object-group', disabled: TB.subflow.cannotBeGruped },
        'group-to-foreach': { name: 'Group to foreach', icon: 'fa-repeat', disabled: TB.foreach.cannotBeGruped },
        'remove-swimlane': { name: 'Remove swimlane', icon: 'fa-trash' },
    },

    roleContextItems: {
        'delete': { name: 'Remove role', icon: 'fa-trash' }
    },

    itemContextItems: {
        'properties': { name: 'Properties...', icon: 'fa-edit' },
        'group-to-subflow': { name: 'Group to subflow', icon: 'fa-object-group', disabled: TB.subflow.cannotBeGruped },
        'sep1': '---------',
        'group-to-foreach': { name: 'Group to foreach', icon: 'fa-repeat', disabled: TB.foreach.cannotBeGruped },
        'set-as-fe-start': { name: 'Set as foreach start', icon: 'fa-play', disabled: TB.foreach.isNotInForeach },
        'set-as-fe-end': { name: 'Set as foreach end', icon: 'fa-stop', disabled: TB.foreach.isNotInForeach },
        'sep2': '---------',
        'delete': { name: 'Delete', icon: 'fa-trash' }
    },

    actionItemContextItems: {
        'wizard': { name: 'Wizard...', icon: 'fa-magic' },
        'properties': { name: 'Properties...', icon: 'fa-edit' },
        'name': { name: 'Name...', icon: 'fa-tag' },
        'comment': { name: 'Comment...', icon: 'fa-comment' },
        'group-to-subflow': { name: 'Group to subflow', icon: 'fa-object-group', disabled: TB.subflow.cannotBeGruped },
        'sep1': '---------',
        'group-to-foreach': { name: 'Group to foreach', icon: 'fa-repeat', disabled: TB.foreach.cannotBeGruped },
        'set-as-fe-start': { name: 'Set as foreach start', icon: 'fa-play', disabled: TB.foreach.isNotInForeach },
        'set-as-fe-end': { name: 'Set as foreach end', icon: 'fa-stop', disabled: TB.foreach.isNotInForeach },
        'sep2': '---------',
        'delete': { name: 'Delete', icon: 'fa-trash' }
    },


    currentRule: null,

    init: function () {
        var self = TB.wfr;
        $(document)
            .on('keydown', $.proxy(self._keyDown, self))
            .on('click', 'button#Search', $.proxy(self.searchItems, self))
            .on('click', '#SearchBox .close', $.proxy(self.closeSearch, self));
    },

    create: function(ruleData)
    {
        var self = TB.wfr;

        var rule = $(self.templates.rule);
        rule.css({
            width: ruleData.Width,
            height: ruleData.Height,
            left: ruleData.PositionX,
            top: ruleData.PositionY
        })
        .find('.verticalLabel').html(ruleData.Name).end()
        .attr({ 'id': AssingID(), 'data-id': ruleData.Id })
        .appendTo("#workflowRulesPanel .scrollArea");
        
        self.aliveRule(rule);
        TB.callHooks(self.onCreateRule, rule, []);

        return rule;
    },

    remove: function() {
        this.remove();
        ChangedSinceLastSave = true; /// OBSOLATE
        TB.changedSinceLastSave = true;
    },

    rename: function() {
        CurrentRule = this;
        renameRuleDialog.dialog('open');
    },

    addSwimlane: function() {
        var count = this.find('.swimlane').length + 1;
        TB.wfr.createSwimlane({Roles: [], WorkflowItems: []}, count, this);

        this.find('.swimlane').css('height', (100 / count) + '%');
        this.data("jsPlumbInstance").recalculateOffsets();
        this.data("jsPlumbInstance").repaintEverything();

        ChangedSinceLastSave = true /// OBSOLATE
        TB.changedSinceLastSave = true;
    },

    createSwimlane: function(swimlaneData, count, parentRule) {
        var self = TB.wfr;
        
        var swimlane = $(self.templates.swimlane);
        swimlane
            .css('height', (100 / count) + '%')
            .appendTo($('.swimlaneArea', parentRule));

        if (swimlaneData.Roles.length > 0) {
            swimlane.find('.swimlaneRolesArea .rolePlaceholder').remove();
            for (var r = 0; r < swimlaneData.Roles.length; r++) {
                swimlane.find('.swimlaneRolesArea .roleItemContainer').append('<div class="roleItem">' + swimlaneData.Roles[r] + '</div>');
            }
        }

        // Připravíme mapování foreach id => virtual action id
        for (var k = 0; k < swimlaneData.WorkflowItems.length; k++) {
            var item = swimlaneData.WorkflowItems[k];
            if (item.TypeClass == 'virtualAction' && item.SymbolType == 'foreach' && item.ParentForeachId) {
                TB.foreach.id2VirtualId[item.ParentForeachId] = item.Id;
            }
        }

        for (var k = 0; k < swimlaneData.Subflow.length; k++) {
            TB.subflow.createSubflow(swimlaneData.Subflow[k], swimlane);
        }

        for (var k = 0; k < swimlaneData.Foreach.length; k++) {
            TB.foreach.createForeach(swimlaneData.Foreach[k], swimlane, null, true);
        }

        for (var k = 0; k < swimlaneData.WorkflowItems.length; k++) {
            self.createItem(swimlaneData.WorkflowItems[k], swimlane);
        }

        self.aliveSwimlane(swimlane);
    },

    removeSwimlane: function () {
        // this = options.$trigger

        var rule = this.parents('.workflowRule');
        var swimlaneCount = rule.find('.swimlane').length;
        if (swimlaneCount > 1) {
            rule.data('jsPlumbInstance').removeAllEndpoints(this, true);
            this.remove();
            
            rule.find('.swimlane').css('height', (100 / (swimlaneCount - 1)) + '%');
            rule.data('jsPlumbInstance').recalculateOffsets();
            rule.data('jsPlumbInstance').repaintEverything();

            ChangedSinceLastSave = true; /// OBSOLATE
            TB.changedSinceLastSave = true;
        }
        else {
            alert('Pravidlo musí mít alspoň jednu swimlane, nelze smazat všechny.');
        }
    },

    createItem: function(itemData, parentSwimlane)
    {
        if (itemData.TypeClass == 'virtualAction') // Pouze virtuální akce
            return;

        var item;
        if (itemData.TypeClass === "symbol" && itemData.SymbolType === "comment") {
            item = $('<div id="wfItem' + itemData.Id + '" class="symbol" symbolType="comment" endpoints="final" style="left: ' + itemData.PositionX +
            'px; top: ' + itemData.PositionY + 'px; width: 30px; padding: 3px; border: 2px solid grey; border-right: none; min-height: 60px;"> <span class="itemLabel">'
            + itemData.Label + '</span></div>');
        } else if (itemData.TypeClass == "symbol") {
            item = $('<img id="wfItem' + itemData.Id + '" class="symbol" symbolType="' + itemData.SymbolType +
            '" src="/Content/Images/TapestryIcons/' + itemData.SymbolType + '.png" style="left: ' + itemData.PositionX + 'px; top: '
            + itemData.PositionY + 'px;" />');

            if (itemData.SymbolType == "envelope-start" || itemData.SymbolType == "circle-event") {
                item.data('label', itemData.Label);
            }
        } else {
            item = $('<div id="wfItem' + itemData.Id + '" class="item" style="left: ' + itemData.PositionX + 'px; top: '
            + itemData.PositionY + 'px;"><span class="itemLabel">' + itemData.Label + '</span></div>');
        }
        item.addClass(itemData.TypeClass);
        if (itemData.ActionId != null)
            item.attr('actionId', itemData.ActionId);
        if (itemData.InputVariables != null)
            item.data('inputVariables', itemData.InputVariables);
        if (itemData.OutputVariables != null)
            item.data('outputVariables', itemData.OutputVariables);
        if (itemData.PageId != null)
            item.attr("pageId", itemData.PageId);
        if (itemData.ComponentName != null)
            item.attr('componentName', itemData.ComponentName);
        if (itemData.TargetId != null)
            item.attr('targetId', itemData.TargetId);
        if (itemData.StateId != null)
            item.attr("stateId", itemData.StateId);
        if (itemData.isAjaxAction != null)
            item.data('isAjaxAction', itemData.isAjaxAction);
        if (itemData.TypeClass == "circle-thick")
            item.attr("endpoints", "final");
        if (itemData.SymbolType && itemData.SymbolType.substr(0, 8) == "gateway-")
            item.attr("endpoints", "gateway");
        if (itemData.Condition != null)
            item.data("condition", itemData.Condition);
        if (itemData.ConditionSets != null)
            item.data("conditionSets", itemData.ConditionSets);
        if (itemData.IsBootstrap != null)
            item.attr('isBootstrap', itemData.IsBootstrap);
        if (itemData.IsForeachStart)
            item.append('<span class="fa fa-play"></span>');
        if (itemData.IsForeachEnd)
            item.append('<span class="fa fa-stop"></span>');
       
        if (itemData.TypeClass == 'actionItem') {
            if (itemData.Name) item.append('<span class="itemName">' + itemData.Name + '</span>');
            if (itemData.Comment) item.append('<span class="itemComment">' + itemData.Comment + '</span>');
            if (itemData.CommentBottom) item.find('.itemComment').addClass('bottom');
        }

        var target;
        switch (true) {
            case itemData.ParentForeachId && itemData.ParentForeachId > 0:
                target = parentSwimlane.find('.foreach[data-foreachid=' + itemData.ParentForeachId + ']');
                break;
            case itemData.ParentSubflowId && itemData.ParentSubflowId > 0:
                target = parentSwimlane.find('.subflow[data-subflowid=' + itemData.ParentSubflowId + ']');
                break;
            default:
                target = parentSwimlane.find('.swimlaneContentArea');
                break;
        }
        
        item.appendTo(target);
        AddToJsPlumb(item);

        TB.callHooks(TB.wfr.onCreateItem, item, []);
    },

    aliveRule: function(rule)
    {
        var self = TB.wfr;

        rule.draggable({
            containment: 'parent',
            handle: '.workflowRuleHeader',
            revert: self._ruleDraggableRevert,
            stop: self._ruleDraggableStop
        });
        rule.resizable({
            start: self._ruleResizableStart,
            resize: self._ruleResizableResize,
            stop: self._ruleResizableStop
        });
        CreateJsPlumbInstanceForRule(rule);
    },

    aliveSwimlane: function (swimlane) {
        var self = TB.wfr;

        swimlane.find('.swimlaneRolesArea').droppable({
            tolerance: 'touch',
            accept: '.toolboxItem.roleItem',
            greedy: true,
            drop: self._swimlaneRoleDrop
        });
        swimlane.find('.swimlaneContentArea').droppable({
            containment: '.swimlaneContentArea',
            tolerance: 'touch',
            accept: '.toolboxSymbol, .toolboxItem',
            greedy: false,
            drop: self._swimlaneItemDrop
        });
    },

    /******************************************************************/
    /* WORKFLOW RULE EVENTS                                           */
    /******************************************************************/

    _ruleDraggableRevert: function(event, ui) {
        return ($(this).collision('#workflowRulesPanel .workflowRule').length > 1);
    },

    _ruleDraggableStop: function (event, ui) {
        ChangedSinceLastSave = true; /// OBSOLATE
        TB.changedSinceLastSave = true;
    },

    _ruleResizableStart: function(event, ui) {
        var rule = $(this);
        var contentsWidth = 120;
        var contentsHeight = 40;
        var limits = TB.checkRuleResizeLimits(rule, false);

        rule.find('.item').each(function (index, element) {
            var rightEdge = $(element).position().left + $(element).width();
            var bottomEdge = $(element).position().top + $(element).height();

            contentsWidth = rightEdge > contentsWidth ? rightEdge : contentsWidth;
            contentsHeight = bottomEdge > contentsHeight ? bottomEdge : contentsHeight;
        });

        rule.css({
            'min-width': contentsWidth + 40, 'min-height': contentsHeight + 20,
            'max-width': limits.horizontal - 10, 'max-height': limits.vertical - 10
        });
    },

    _ruleResizableResize: function(event, ui) {
        var rule = $(this);
        var instance = rule.data("jsPlumbInstance");
        var limits = TB.checkRuleResizeLimits(rule, false);

        instance.recalculateOffsets();
        instance.repaintEverything();

        rule.css({'max-width': limits.horizontal - 10, 'max-height': limits.vertical - 10});
    },

    _ruleCopy: function () {
        var d = this;
        var appId = $('#currentAppId').val();
        var blockId = $('#currentBlockId').val();
        var wfrId = TB.wfr.currentRule.attr('data-id');
        var targetBlockId = $('#wr-copy-target').val();

        if (!targetBlockId) {
            alert('Vyberte cílový blok.');
        }
        else {
            var url = '/api/tapestry/apps/' + appId + '/blocks/' + blockId + '/copyWorkflow/' + wfrId + /target/ + targetBlockId;
            $.ajax({
                url: url,
                type: 'GET',
                data: {},
                success: function (data) {
                    if (data) {
                        alert('Workflow bylo úspěšně zkopírováno.');
                        TB.dialog.close.apply(d);
                    }
                    else {
                        alert('Workflow se nepodařilo zkopírovat.');
                    }
                }
            })
        }
    },

    _ruleCopyOpen: function() {
        $('#wr-copy-source').html(TB.wfr.currentRule.find('.verticalLabel').text());
        $('#wr-copy-target').val('');
    },

    /******************************************************************/
    /* SWIMLANE EVENTS                                                */
    /******************************************************************/
    _ruleResizableStop: function (event, ui) {
        ChangedSinceLastSave = true; /// OBSOLATE
        TB.changedSinceLastSave = true;
    },

    _swimlaneRoleDrop: function(event, ui) {
        if (dragModeActive)
        {
            dragModeActive = false;

            var roleExists = false;
            $(this).find('.roleItem').each(function (index, element) {
                if ($(element).text() == ui.helper.text())
                    roleExists = true;
            });
            if (!roleExists) {
                var role = ui.helper.clone();

                $(this).find('.rolePlaceholder').remove();
                $(this).find('.roleItemContainer').append($('<div class="roleItem">' + role.text() + '</div>'));
                ui.helper.remove();
                ChangedSinceLastSave = true; /// OBSOLATE
                TB.changedSinceLastSave = true;
            }
        }
    },

    _swimlaneItemDrop: function (event, ui) {
        if (dragModeActive)
        {
            dragModeActive = false;
            var leftOffset, topOffset;
            var item = ui.helper.clone();

            if (item.hasClass('roleItem')) {
                ui.draggable.draggable('option', 'revert', true);
                return false;
            }
            var ruleContent = $(this);
            ruleContent.append(item);

            if (item.hasClass('toolboxSymbol')) {
                item.removeClass('toolboxSymbol ui-draggable ui-draggable-dragging');
                item.addClass('symbol');
                item.css({ height: '' });
                leftOffset = $('#tapestryWorkspace').offset().left - ruleContent.offset().left;
                topOffset = $('#tapestryWorkspace').offset().top - ruleContent.offset().top;
            }
            else {
                item.removeClass('toolboxItem');
                item.addClass('item');
                item.css({ width: '', height: '' });
                leftOffset = $('#tapestryWorkspace').offset().left - ruleContent.offset().left + 38;
                topOffset = $('#tapestryWorkspace').offset().top - ruleContent.offset().top - 18;
            }
            item.offset({ left: item.offset().left + leftOffset, top: item.offset().top + topOffset });
            ui.helper.remove();
            AddToJsPlumb(item);
            if (item.position().top + item.height() + 10 > ruleContent.height()) {
                item.css('top', ruleContent.height() - item.height() - 20);
                var instance = ruleContent.parents('.workflowRule').data('jsPlumbInstance');
                instance.repaintEverything();
            }
            ChangedSinceLastSave = true; /// OBSOLATE
            TB.changedSinceLastSave = true;
        }
    },

    /******************************************************************/
    /* CONTEXT MENU ACTIONS                                           */
    /******************************************************************/
    _contextAction: function (key, options) {
        var self = TB.wfr;
        switch (key) {
            case 'delete': self.remove.apply(options.$trigger, []); break;
            case 'rename': self.rename.apply(options.$trigger, []); break;
            case 'copy':
                self.currentRule = options.$trigger;
                TB.dialog.open('workflowCopy');
                break;
            case 'add-swimlane': self.addSwimlane.apply(options.$trigger, []); break;
        }
    },
    
    _swimlaneContextAction: function (key, options) {
        if (key == 'remove-swimlane') {
            TB.wfr.removeSwimlane.apply(options.$trigger, []);
        }
        else if (key == 'group-to-subflow') {
            TB.subflow.groupToSubflow.apply(options.$trigger, []);
        }
        else if (key == 'group-to-foreach') {
            TB.foreach.groupToForeach.apply(options.$trigger, []);
        }
    },

    _roleContextAction: function (key, options) {
        var item = options.$trigger;
        if (key == "delete") {
            var swimlaneRolesArea = item.parents(".swimlaneRolesArea");
            item.remove();
            if (swimlaneRolesArea.find(".roleItem").length == 0)
                swimlaneRolesArea.append($('<div class="rolePlaceholder"><div class="rolePlaceholderLabel">'
                    + 'Pokud chcete specifikovat roli<br />přetáhněte ji do této oblasti</div></div>'));
            ChangedSinceLastSave = true; /// OBSOLATE
            TB.changedSinceLastSave = true;
        }
    },

    _itemContextAction: function (key, options) {
        item = options.$trigger;
        switch (key) {
            case "delete": {
                currentInstance = item.parents(".rule").data("jsPlumbInstance");
                currentInstance.removeAllEndpoints(item, true);
                item.remove();
                ChangedSinceLastSave = true;
                break;
            }
            case "wizard": {
                item.addClass("activeItem processedItem");

                if (item.hasClass("actionItem") && item.parents(".rule").hasClass("workflowRule")) {
                    CurrentItem = item;
                    TB.wizard.open.apply(item, []);
                }
                else {
                    alert("Pro tento typ objektu nejsou dostupná žádná nastavení.");
                    item.removeClass("activeItem");
                }
                break;
            }
            case "properties": {
                item.addClass("activeItem processedItem");
                if (item.hasClass("tableAttribute")) {
                    CurrentItem = item;
                    tableAttributePropertiesDialog.dialog("open");
                }
                else if (item.hasClass("viewAttribute")) {
                    CurrentItem = item;
                    gatewayConditionsDialog.dialog("open");
                }
                else if (item.hasClass("actionItem") && item.parents(".rule").hasClass("workflowRule")) {
                    CurrentItem = item;
                    actionPropertiesDialog.dialog("open");
                }
                else if (item.hasClass("symbol") && item.attr("symboltype") == "gateway-x") {
                    CurrentItem = item;
                    gatewayConditionsDialog.dialog("open");
                }
                else if (item.hasClass("symbol") && item.attr("symboltype") == "envelope-start") {
                    CurrentItem = item;
                    envelopeStartPropertiesDialog.dialog("open");
                }
                else if (item.hasClass("symbol") && item.attr("symboltype") == "envelope-start") {
                    CurrentItem = item;
                    envelopeStartPropertiesDialog.dialog("open");
                }
                else if (item.hasClass("symbol") && item.attr("symboltype") == "circle-event") {
                    CurrentItem = item;
                    circleEventPropertiesDialog.dialog("open");
                }
                else if (item.hasClass("uiItem")) {
                    CurrentItem = item;
                    uiitemPropertiesDialog.dialog("open");
                }
                else if (item.hasClass("symbol") && item.attr("symbolType") === "comment") {
                    CurrentItem = item;
                    labelPropertyDialog.dialog("open");
                }
                else {
                    alert("Pro tento typ objektu nejsou dostupná žádná nastavení.");
                    item.removeClass("activeItem");
                }
                break;
            }
            case 'name': {
                CurrentItem = item;
                TB.dialog.open('actionItemName');
                break;
            }
            case 'comment': {
                CurrentItem = item;
                TB.dialog.open('actionItemComment');
                break;
            }
            case 'group-to-subflow': {
                TB.subflow.groupToSubflow.apply(options.$trigger, []);
                break;
            }
            case 'group-to-foreach': {
                TB.foreach.groupToForeach.apply(options.$trigger, []);
                break;
            }
            case 'set-as-fe-start': {
                TB.foreach.setStart.apply(options.$trigger, []);
                break;
            }
            case 'set-as-fe-end': {
                TB.foreach.setEnd.apply(options.$trigger, []);
                break;
            }
        }
    },

    /*******************************************************************/
    /* DIALOG ACTIONS                                                  */
    /*******************************************************************/
    _actionItemSetNameOpen: function() {
        $(this).find('#ActionName').val($(CurrentItem).find('.itemName').text());
    },

    _actionItemSetName: function () {
        var name = $(this).find('#ActionName').val();
        var item = $(CurrentItem);

        if (name.length) {
            if (!item.find('.itemName').length) {
                item.append('<span class="itemName" />');
            }
            item.find('.itemName').html(name);
        }
        else {
            item.find('.itemName').remove();
        }
        
        CurrentItem = null;
        ChangedSinceLastSave = true; /// OBSOLATE
        TB.changedSinceLastSave = true;
        TB.dialog.close.apply(this);
    },

    _actionItemSetCommentOpen: function() {
        $(this).find('#ActionComment').val($(CurrentItem).find('.itemComment').text());
        $(this).find('#ActionCommentBottom').prop('checked', $(CurrentItem).find('.itemComment').hasClass('bottom'));
    },

    _actionItemSetComment: function () {
        var comment = $(this).find('#ActionComment').val();
        var item = $(CurrentItem);

        if (comment.length) {
            if (!item.find('.itemComment').length) {
                item.append('<span class="itemComment" />');
            }
            item.find('.itemComment').toggleClass('bottom', $('#ActionCommentBottom').is(':checked')).html(comment);
        }
        else {
            item.find('.itemComment').remove();
        }

        CurrentItem = null;
        ChangedSinceLastSave = true; /// OBSOLATE
        TB.changedSinceLastSave = true;
        TB.dialog.close.apply(this);
    },
    
    /*************************************************/
    /* SEARCH IN WORKFLOW's                          */
    /*************************************************/

    _keyDown: function (e) {
        if ((e.ctrlKey && e.shiftKey) || e.metaKey) {
            if (String.fromCharCode(e.which).toLowerCase() == 'f') {
                e.preventDefault();

                var $box = $('<div></div>');
                $box.css({
                    position: 'fixed',
                    right: 0,
                    top: 75,
                    border: '2px solid #457fa9',
                    background: '#49c3f1',
                    width: '20%',
                    padding: '10px'
                }).attr('id', 'SearchBox').appendTo('body');

                var $c = $('<button type="button" class="close" aria-label="Close" style="opacity:1"><span aria-hidden="true" style="color:#fff">&times;</span></button>');
                $c.appendTo($box);

                $box.append('<label class="control-label">Search in variables:</label>');

                var $g = $('<div class="from-group"></div>');
                $g.appendTo($box);
                
                var $ig = $('<div class="input-group input-group-sm"></div>');
                $ig.appendTo($g);

                var $f = $('<input type="text" id="SearchTerm" class="form-control" value="">');
                $f.appendTo($ig);

                var $b = $('<span class="input-group-btn"><button type="button" id="Search" class="btn btn-default"><span class="fa fa-search"></span></button></span>');
                $b.appendTo($ig);
            }
        }  
    },

    searchItems: function () {
        var term = $('#SearchTerm').val();
        $('.swimlaneContentArea .item')
            .removeClass('isMatch')
            .each(function () {
                if (!term.length) {
                    return false;
                }
                var inputVars = $(this).data('inputVariables');
                var outputVars = $(this).data('outputVariables');

                if ((inputVars && inputVars.indexOf(term) !== -1) || (outputVars && outputVars.indexOf(term) !== -1)) {
                    $(this).addClass('isMatch');
                }
            });

    },

    closeSearch: function () {
        $('#SearchBox').remove();
        $('.swimlaneContentArea .item').removeClass('isMatch')
    }
}

TB.onInit.push(TB.wfr.init);
TB.library = {

    onCreate: [],
    onClean: [],

    clean: function () {
        $('.libraryItem').remove();

        TB.callHooks(TB.library.onClean, null, []);
    },

    createItem: function(target, type, params, name, className, highlighted, originalItem)
    {
        var itemLibId = ++lastLibId;
        params.libId = itemLibId;
        params.libType = type;
        
        var item = $('<div class="libraryItem"></div>');
        item.attr(params).html(name).appendTo($('#libraryCategory-'+target));
        
        if (className) { item.addClass(className); }
        if (highlighted) { item.addClass('highlighted'); }

        TB.callHooks(TB.library.onCreate, originalItem, [type]);
        
        return itemLibId;
    }


};

TB.toolbox = {

    clean: function () {
        $('.tapestryToolbox .toolboxLi').remove();
    },

    createItem: function (libId, itemSuffix, divClass, divAttr, label) {
        var item = $('<li class="toolboxLi"><div class="toolboxItem"><span class="itemLabel"></span></div></li>');
        item.attr('libId', libId).addClass('toolboxLi_' + itemSuffix)
            .find('> div').addClass(divClass).attr(divAttr)
            .find('.itemLabel').html(label);

        var items = $('.toolboxCategoryHeader_' + itemSuffix).nextUntil('[class^=toolboxCategoryHeader]');
        var target = items.length ? items.last() : $('.toolboxCategoryHeader_' + itemSuffix);

        target.after(item);
        TB.toolbox.alive(item);
    },

    alive: function(item)
    {
        item.find('.toolboxItem').draggable({
            helper: 'clone',
            appendTo: '#tapestryWorkspace',
            containment: 'window',
            tolerance: 'fit',
            revert: true,
            scroll: true,
            start: function () {
                dragModeActive = true;
            }
        });
    }
};

TB.dialog = {

    dialogList: {},

    dialogDefaults: {
        autoOpen: false,
        width: 'auto',
        height: 'auto',
        create: function () {
            var d = $(this).parents('.ui-dialog');
            var buttons = d.find('.ui-dialog-buttonset button');
            var dialogId = $(this).data('dialogId');

            $.each(TB.dialog.dialogList[dialogId].options.buttons, function (index) {
                buttons.eq(index).addClass(this.className);
                if (this.icon) {
                    buttons.eq(index).prepend('<span class="fa ' + this.icon + '"></span> ');
                }
            });

            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    TB.dialog.dialogList[dialogId].submit.apply(this, []);
                    return false;
                }
            });

            d.find('.ui-dialog-buttonset').css('float', 'none');
        }
    },

    init: function()
    {
        var self = TB.dialog;
        for (var k in self.dialogList) {
            var d = self.dialogList[k];
            $(d.target).data('dialogId', k);
            $(d.target).dialog($.extend(self.dialogDefaults, d.options));
        }
    },

    open: function(dialogId) {
        $(TB.dialog.dialogList[dialogId].target).dialog('open');
    },

    close: function () {
        $(this).dialog('close');
    }
};

TB.dialog.dialogList = {
    actionItemName: {
        target: '#action-item-name-dialog',
        submit: TB.wfr._actionItemSetName,
        options: {
            buttons: [
                { text: 'Save', click: TB.wfr._actionItemSetName, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.wfr._actionItemSetNameOpen
        }
    },
    actionItemComment: {
        target: '#action-item-comment-dialog',
        submit: TB.wfr._actionItemSetComment,
        options: {
            buttons: [
                { text: 'Save', click: TB.wfr._actionItemSetComment, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.wfr._actionItemSetCommentOpen
        }
    },
    workflowCopy: {
        target: '#workflow-copy-dialog',
        submit: TB.wfr._ruleCopy,
        options: {
            buttons: [
                { text: 'Copy', click: TB.wfr._ruleCopy, className: 'btn btn-success pull-right', icon: 'fa-clone' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-defult', icon: 'fa-times' }
            ],
            open: TB.wfr._ruleCopyOpen,
            width: 600
        }
    },
    save: {
        target: '#save-dialog',
        submit: TB.save.saveBlock,
        options: {
            buttons: [
                { text: 'Save', click: TB.save.saveBlock, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.save._dialogOpen,
            width: 400
        }
    },
    subflowName: {
        target: '#subflow-name-dialog',
        submit: TB.subflow.setName,
        options: {
            buttons: [
                { text: 'Save', click: TB.subflow.setName, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.subflow._setNameOpen
        }
    },
    subflowComment: {
        target: '#subflow-comment-dialog',
        submit: TB.subflow.setComment,
        options: {
            buttons: [
                { text: 'Save', click: TB.subflow.setComment, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.subflow._setCommentOpen
        }
    },
    foreachDatasource: {
        target: '#foreach-datasource-dialog',
        submit: TB.foreach.setDatasource,
        options: {
            buttons: [
                { text: 'Save', click: TB.foreach.setDatasource, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.foreach._setDatasourceOpen
        }
    },
    foreachName: {
        target: '#foreach-name-dialog',
        submit: TB.foreach.setName,
        options: {
            buttons: [
                { text: 'Save', click: TB.foreach.setName, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.foreach._setNameOpen
        }
    },
    foreachComment: {
        target: '#foreach-comment-dialog',
        submit: TB.foreach.setComment,
        options: {
            buttons: [
                { text: 'Save', click: TB.foreach.setComment, className: 'btn btn-success pull-right', icon: 'fa-check' },
                { text: 'Cancel', click: TB.dialog.close, className: 'btn btn-default', icon: 'fa-times' }
            ],
            open: TB.foreach._setCommentOpen
        }
    }
}

TB.onInit.push(TB.dialog.init);
TB.context = {

    defaultSettings: {
        trigger: 'right',
        zIndex: 300
    },

    init: function() 
    {
        var ds = TB.context.defaultSettings; 
        
        $.contextMenu($.extend(ds, {
            selector: '.resourceRule',
            callback: TB.rr._contextAction,
            items: TB.rr.contextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.swimlane',
            callback: TB.wfr._swimlaneContextAction,
            items: TB.wfr.swimlaneContextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.workflowRule',
            callback: TB.wfr._contextAction,
            items: TB.wfr.contextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.swimlaneRolesArea .roleItem',
            callback: TB.wfr._roleContextAction,
            items: TB.wfr.roleContextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.item:not(.actionItem), .symbol',
            callback: TB.wfr._itemContextAction,
            items: TB.wfr.itemContextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.item.actionItem',
            callback: TB.wfr._itemContextAction,
            items: TB.wfr.actionItemContextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.subflow',
            callback: TB.subflow._contextAction,
            items: TB.subflow.contextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.foreach',
            callback: TB.foreach._contextAction,
            items: TB.foreach.contextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.tableRow',
            callback: function (key, options) {
                if (key == "model") {
                    var tableRow = options.$trigger;
                    tableRow.addClass("highlightedRow");
                    tableRow.parents("table").find(".modelMarker").remove();
                    tableRow.find("td:first").append('<div class="modelMarker">Model</div>');
                }
            },
            items: {
                "model": { name: "Set as model", icon: "edit" }
            }
        }));
    }

};

TB.onInit.push(TB.context.init);
TB.wizard = {

    actions: {},
    dataTypeList: {
        's$': 'string',
        'b$': 'boolean',
        'i$': 'integer',
        'f$': 'float',
        'd$': 'datetime',
        '_var_': 'variable'
    },

    variableList: [
        { label: '__Model__', category: 'System' },
        { label: '__ModelId__', category: 'System' },
        { label: '__TableName__', category: 'System' },
        { label: '__CORE__', category: 'System' },
        { label: '__User__', category: 'System' },
    ],

    dataPrefixList: {
        // BOOTSTRAP COMPONENTS
        'ui|nv-list':           ['tableData'],
        'ui|data-table':        ['tableData'],
        'ui|countdown':         ['countdownTargetData'],
        'form|label':           ['inputData'],
        'form|input-text':      ['inputData'],
        'form|input-email':     ['inputData'],
        'form|input-color':     ['inputData'],
        'form|input-tel':       ['inputData'],
        'form|input-date':      ['inputData'],
        'form|input-number':    ['inputData'],
        'form|input-range':     ['inputData'],
        'form|input-hidden':    ['inputData'],
        'form|input-url':       ['inputData'],
        'form|input-search':    ['inputData'],
        'form|input-password':  ['inputData'],
        'form|input-file':      ['inputData'],
        'form|textarea':        ['inputData'],
        'form|checkbox':        ['checkboxData'],
        'form|radio':           ['checkboxData'],
        'form|select':          ['dropdownData', 'dropdownSelection'],
        // LEGACY COMPONENTS
        'checkbox':                 ['checkboxData'],
        'countdown':                ['countdownTargetData'],
        'data-table-read-only':     ['tableData'],
        'data-table-with-actions':  ['tableData'],
        'dropdown-select':          ['dropdownData', 'dropdownSelection'],
        'input-single-line':        ['inputData'],
        'input-multiline':          ['inputData'],
        'label':                    ['inputData'],
        'multiple-select':          ['dropdownData', 'dropdownSelection'],
        'name-value-list':          ['tableData']
    },

    uiTypeList: {},

    target: null,

    init: function () 
    {
        var self = TB.wizard;

        TB.load.onAttributesLoad.push(self._attributesLoad);
        TB.library.onClean.push(self._libraryClean);
        TB.library.onCreate.push(self._libraryCreateItem);
        TB.wfr.onCreateItem.push(self._workflowCreateItem);

        $.widget('custom.catcomplete', $.ui.autocomplete, {
            _create: function () {
                this._super();
                this.widget().menu('option', 'items', '> :not(.ui-autocomplete-category)');
            },
            _renderMenu: function (ul, items) {
                var self = this;
                var w = TB.wizard;
                var currentCategory = '';
                var requestedCategory;
                var requestedTable;
                var requestedView;
                var requestedWorkflow = w.getRequestedWorkflow();
                
                var type = $(this.element).parents('.form-group').find('select[name=var_type]').val();

                if (type) {
                    switch (type) {
                        case '_var_': requestedCategory = '*'; break;
                        case 's$': requestedCategory = w.getRequestedCategory.apply(this.element, []); break;
                        default: requestedCategory = ''; break;
                    }
                }
                else {
                    requestedCategory = w.getRequestedCategory.apply(this.element, []);
                }

                if (requestedCategory == 'Column') {
                    requestedTable = w.getRequestedTable.apply(this.element, []);
                    requestedView = w.getRequestedView.apply(this.element, []);
                }

                $.each(items, function () {
                    var li;
                    if (requestedCategory == '*' ||
                        requestedCategory == this.category ||
                        requestedCategory.indexOf(this.category) !== -1 ||
                        (requestedCategory.indexOf('Workflow') !== -1 && this.category.indexOf('Workflow') !== -1) ||
                        (requestedCategory == 'Column' && ((this.category == 'Tables' && requestedTable) || (this.category == 'Views' && requestedView)))
                       ) {

                        if (this.category != currentCategory &&
                            (this.category.indexOf('Workflow') === -1 || this.category == 'Workflow: ' + requestedWorkflow) &&
                            (requestedCategory != 'Column' || (this.category == 'Tables' && requestedTable) || (this.category == 'Views' && requestedView))
                           ) {
                            ul.append('<li class="ui-autocomplete-category bg-info">' + this.category + '</li>');
                            currentCategory = this.category;
                        }

                        if (requestedCategory == 'Tables' && this.label.indexOf('.') !== -1) return;
                        if (requestedCategory == 'Column' && this.category == 'Tables' && (!requestedTable || this.label.indexOf(requestedTable + '.') === -1)) return;
                        if (requestedCategory == 'Column' && this.category == 'Views' && (!requestedView || this.label.indexOf(requestedView + '.') === -1)) return;
                        if (this.category.indexOf('Workflow') !== -1 && this.category != 'Workflow: ' + requestedWorkflow) return;

                        li = self._renderItemData(ul, this);
                    }
                });
            }
        });
    },

    open: function()
    {
        var self = TB.wizard;
        var target = $(this);
        var action = self.actions[target.attr('actionId')];
        var form = $('<form class="form-horizontal" onsubmit="return false"></form>');
        var inputVarsValues = self.parseInputValue(target);
        var outputVarsValues = self.parseOutputValue(target);
        var knownInputVars = [];
        var knownOutputVars = [];

        self.target = target;

        var d = $('<div title="Průvodce parametry akce \'{action_name}\'"></div>');
        d.attr('title', d.attr('title').replace(/\{action_name\}/, action.name));
        d.append(form);

        d.dialog({
            autoOpen: false,
            width: '50%',
            draggable: true,
            resizable: true,
            appendTo: 'body',
            dialogClass: 'dialog-wizard',
            buttons: [{
                text: 'Použít',
                click: TB.wizard.validate
            }],
            create: function () {
                var btn = $(this).parents('.ui-dialog').find('.ui-dialog-buttonset button');
                btn.addClass('btn btn-primary').prepend('<span class="fa fa-check"></span> ');
            },
            close: function () {
                $(this).remove();
            }
        });

        if (action.inputVars.length || action.outputVars.length || inputVarsValues.length || outputVarsValues.length) 
        {
            var iSet = $('<fieldset class="inputVars"><legend>Input vars</legend></fieldset>');
            var oSet = $('<fieldset class="outputVars"><legend>Output vars</legend></fieldset>');

            iSet.appendTo(form);
            oSet.appendTo(form);

            if (action.inputVars.length || inputVarsValues.length) {
                for (var i = 0; i < action.inputVars.length; i++) {
                    var inputVar = action.inputVars[i];
                    if (!inputVar.isArray) {
                        self.createInputVar(inputVar, iSet, inputVarsValues.items);
                        knownInputVars.push(inputVar.name);
                    }
                    else {
                        var matchFound = false;
                        var rx = new RegExp('^' + inputVar.name + '\\[(\\d+)\\]$');
                        for (k in inputVarsValues.items) {
                            if (m = k.match(rx)) {
                                self.createInputVar(inputVar, iSet, inputVarsValues.items, m[1]);
                                knownInputVars.push(m[0]);
                                matchFound = true;
                            }
                        }

                        if (!matchFound) {
                            self.createInputVar(inputVar, iSet, inputVarsValues.items, 0);
                            knownInputVars.push(inputVar.name + '[0]');
                        }
                    }
                }

                for (k in inputVarsValues.items) {
                    if ($.inArray(k, knownInputVars) == -1) {
                        self.createInputVar({
                            required: false,
                            type: null,
                            name: k,
                            isArray: false,
                            isEnum: false,
                            enumItems: [],
                            unknown: true
                        }, iSet, inputVarsValues.items);
                    }
                }
            }
            else {
                iSet.append('<div class="form-group no-vars"><div class="col-xs-12"><p class="alert alert-info">Tato akce nemá žádné vstupní parametry</p></div></div>');
            }

            if (action.outputVars.length || outputVarsValues.length) {
                for (var i = 0; i < action.outputVars.length; i++) {
                    self.createOutputVar(action.outputVars[i], oSet, outputVarsValues.items);
                    knownOutputVars.push(action.outputVars[i].name);
                }

                for (k in outputVarsValues.items) {
                    if ($.inArray(k, knownOutputVars) == -1) {
                        self.createOutputVar({
                            required: false,
                            type: null,
                            name: k,
                            isArray: false,
                            isEnum: false,
                            enumItems: [],
                            unknown: true
                        }, oSet, outputVarsValues.items);
                    }
                }
            }
            else {
                oSet.append('<div class="form-group no-vars"><div class="col-xs-12"><p class="alert alert-info">Tato akce nemá žádné výstupní parametry</p></div></div>');
            }
        }
        else {
            form.html('<div class="form-group no-vars"><div class="col-xs-12"><p class="alert alert-info">Tato akce nemá žádné vstupní ani výstupní parametry</p></div></div>');
        }

        d.dialog('open');
    },

    validate: function()
    {
        var d = $(this);
        var isValid = true;

        d.find('fieldset.inputVars > .form-group:not(.no-vars)').each(function () {
            if ($(this).data('required') == 'true') {
                var value = $(this).find('input[name=var_value], select[name=var_value]').not(':disabled').val();
                isValid = isValid && value.length > 0;
            }

            /*if ($(this).find('select[name=var_type]').val() == '_var_') {
                var value = $(this).find('input[name=var_value]').val();
                if (value.length) {
                    var exists = false;
                    var requestedWorkflow = TB.wizard.getRequestedWorkflow();

                    $.each(TB.wizard.variableList, function () {

                    });
                }
            }*/
        });

        if (isValid) {
            TB.wizard.build.apply(this, []);
        }
        else {
            var confirm = $('<div title="Jste si jistí?"><p class="text-nowrap">Nemáte vyplněné všechny povinné proměnné.<br><b>Opravdu chcete parametry uložit?</b></p></div>');
            var context = this;

            confirm.dialog({
                autoOpen: true,
                width: 450,
                draggable: false,
                resizable: false,
                modal: true,
                appendTo: 'body',
                dialogClass: 'dialog-wizard-confirm',
                buttons: [{
                    text: 'Ano',
                    click: function () {
                        TB.wizard.build.apply(context);
                        $(this).dialog('close');
                    }
                }, {
                    text: 'Ne',
                    click: function() {
                        $(this).dialog('close');
                    }
                }],
                create: function () {
                    var buttons = $(this).parents('.ui-dialog').find('.ui-dialog-buttonset button');
                    buttons.eq(0).addClass('btn btn-success pull-right').prepend('<span class="fa fa-check"></span> ');
                    buttons.eq(1).addClass('btn btn-default').prepend('<span class="fa fa-times"></span> ');

                    $(this).parents('.ui-dialog').find('.ui-dialog-buttonset').css('float', 'none');
                },
                close: function () {
                    $(this).remove();
                }
            });
        }
    },

    build: function()
    {
        var d = $(this);

        var inputVars = [];
        var outputVars = [];

        d.find('fieldset.inputVars > .form-group:not(.no-vars)').each(function () {
            var variable = $(this).find('.control-label').data('invar');
            var index = $(this).find('.control-label').data('index');
            var dataType = $(this).find('select[name=var_type]').val();
            var value = $(this).find('input[name=var_value], select[name=var_value]').not(':disabled').val();

            if (value.length) {
                inputVars.push(variable + (typeof index != 'undefined' ? '[' + index + ']' : '') + '=' + (dataType == '_var_' ? '' : dataType) + value);
            }
        });
        d.find('fieldset.outputVars > .form-group:not(.no-vars)').each(function () {
            var variable = $(this).find('.input-group-addon:last-child').data('outvar');
            var value = $(this).find('input[name=out_value]').val();

            if (value.length) {
                outputVars.push(value + '=' + variable);
            }
        });

        TB.wizard.target.data('inputVariables', inputVars.join(';'));
        TB.wizard.target.data('outputVariables', outputVars.join(';'));
        TB.wizard.rebuildWorkflowVars();
        TB.wizard.target = null;

        d.dialog('close');
    },

    rebuildWorkflowVars: function()
    {
        var self = TB.wizard;
        var workflowName = self.getRequestedWorkflow();

        var newVars = [];
        $.each(self.variableList, function () {
            if (this.category != 'Workflow: ' + workflowName) {
                newVars.push(this);
            }
        });

        self.target.parents('.swimlaneArea').find('.actionItem').each(function () {
            var outputVars = self.parseOutputValue($(this));
            if (outputVars.length > 0) {
                for(var k in outputVars.items) {
                    newVars.push({
                        label: outputVars.items[k],
                        category: 'Workflow: ' + workflowName
                    });
                }
            }

        });

        self.variableList = newVars;
        self.variableList.sort(self.sort);
    },

    /******************************************************/
    /* TOOLS                                              */
    /******************************************************/
    parseVars: function(source) {
        var vars = [];
        if (source) {
            for (var i = 0; i < source.length; i++) {
                var m = source[i].match(/^(\?)?([a-z]\$)?([a-z0-9]+)(\[(index)?\])?(\[([^\]]*)\])?$/i);
                // 1 = ?    = volitelná?
                // 2 = s$   = typ
                // 3 = název
                // 4 = pole
                // 6 = enum
                // 7 = enum items

                vars.push({
                    required: m[1] == '?' ? false : true,
                    type: m[2],
                    name: m[3],
                    isArray: m[4] ? true : false,
                    isEnum: m[6] && (m[7] && m[7] != 'index') ? true : false,
                    enumItems: (m[7] && m[7] != 'index') ? m[7].split('|') : [],
                    unknown: false
                });
            }
        }
        return vars;
    },

    parseInputValue: function(target) {
        var data = target.data('inputVariables');
        var values = {
            length: 0,
            items: {}
        };

        if (data) {
            var list = data.split(/;/);
            for (var i = 0; i < list.length; i++) {
                var pair = list[i].split(/=/);
                var m = pair[1].match(/^([a-z]\$)?(.+)/);

                values.length++;
                values.items[pair[0]] = {
                    dataType: m[1],
                    value: m[2]
                };
            }
        }

        return values;
    },

    parseOutputValue: function(target) {
        var data = target.data('outputVariables');
        var values = {
            length: 0,
            items: {}
        };
        
        if (data) {
            var list = data.split(/;/);
            for (var i = 0; i < list.length; i++) {
                var pair = list[i].split(/=/);
                values.length++;
                values.items[pair[1]] = pair[0];
            }
        }

        return values;
    },

    createInputVar: function(inputVar, target, values, index) {
        var group = $('<div class="form-group form-group-sm" />');
        var typeWrapper = $('<div class="col-md-2 col-sm-6" />');
        var valueWrapper = $('<div class="col-md-7 col-sm-6" />');
        var valueInputGroup = $('<div class="input-group" />');
        var label = $('<label class="control-label col-md-3" data-invar="' + inputVar.name + '">' + inputVar.name + (inputVar.isArray ? '[' + index + ']' : '') + ' =</label>');
        var type = $('<select name="var_type" class="form-control"></select>');
        var value = $('<input type="text" name="var_value" class="form-control" />');
        var enumValue = $('<select name="var_value" class="form-control enum-value"></select>');
        var booleanValue = $('<select name="var_value" class="form-control boolean-value"></select>');
        var addOn = $('<div class="input-group-addon"></div>');

        group.data('required', inputVar.required ? 'true' : 'false');

        label.appendTo(group);
        typeWrapper.appendTo(group);
        valueWrapper.appendTo(group);

        type.appendTo(typeWrapper);

        valueInputGroup.appendTo(valueWrapper);
        addOn.appendTo(valueInputGroup);
        value.appendTo(valueInputGroup);
        booleanValue.appendTo(valueInputGroup);

        group.appendTo(target);

        booleanValue.append('<option value="True">True</option>');
        booleanValue.append('<option value="False">False</option>');

        if (inputVar.isEnum) {
            valueInputGroup.append(enumValue);
            for (var i = 0; i < inputVar.enumItems.length; i++) {
                enumValue.append('<option value="' + inputVar.enumItems[i] + '">' + inputVar.enumItems[i] + '</option>');
            }

            value.attr('disabled', true).hide();
        }

        if (inputVar.isArray) {
            var addOn2 = $('<div class="input-group-addon"></div>');
            var add = $('<span class="fa fa-plus fa-fw"></span>');
            var del = $('<span class="fa fa-times fa-fw"></span>');

            addOn2.appendTo(valueInputGroup);
            add.appendTo(addOn2);
            del.appendTo(addOn2);

            add.click(TB.wizard._addArrayItem);
            del.click(TB.wizard._deleteArrayItem);

            label.attr('data-index', index);
        }

        if (inputVar.required) {
            addOn.append('<span class="fa fa-asterisk fa-fw"></span>');
        }
        else {
            if (inputVar.unknown) {
                addOn.append('<span class="fa fa-warning fa-fw" title="Neočekávaná vstupní proměnná"></span>');

                var addOn2 = $('<div class="input-group-addon"></div>');
                var del = $('<span class="fa fa-times fa-fw"></span>');

                addOn2.appendTo(valueInputGroup);
                del.appendTo(addOn2);

                del.click(TB.wizard._deleteUnexpectedVar);
            }
            else {
                addOn.append('<span class="fa fa-question fa-fw"></span>');
            }
        }

        type.data('hasEnum', inputVar.isEnum);
        type.change(TB.wizard._changeType); 

        for (var k in TB.wizard.dataTypeList) {
            var opt = $('<option value="' + k + '">' + TB.wizard.dataTypeList[k] + '</option>');
            opt.attr('disabled', inputVar.type && k != inputVar.type && k != '_var_');
            
            type.append(opt);
        }

        var key = inputVar.isArray ? inputVar.name + '[' + index + ']' : inputVar.name;
        if (values[key]) {
            var v = values[key];

            type.val(v.dataType ? v.dataType : '_var_');
            if (inputVar.isEnum && type.val() != '_var_') {
                enumValue.val(v.value);
            }
            else {
                if (type.val() == 'b$') {
                    var b = v.value.toLowerCase() == 'true' ? 'True' : 'False';
                    booleanValue.val(b);
                    value.attr('disabled', true).hide();
                }
                else {
                    value.val(v.value);
                }
            }
        }

        booleanValue.attr('disabled', type.val() != 'b$').toggle(type.val() == 'b$');

        TB.wizard.setInputVarAutocomplete(value);

        type.change();
    },

    createOutputVar: function (outputVar, target, values) {
        var group = $('<div class="form-group form-group-sm" />');
        var wrapper = $('<div class="col-xs-12"></div>');
        var valueWrapper = $('<div class="input-group" />');
        var value = $('<input type="text" name="out_value" class="form-control text-right" />');
        var addOn = $('<div class="input-group-addon" data-outvar="' + outputVar.name + '">= ' + outputVar.name + '</div>');

        wrapper.appendTo(group);
        valueWrapper.appendTo(wrapper);
        value.appendTo(valueWrapper);
        addOn.appendTo(valueWrapper);

        if (values[outputVar.name]) {
            value.val(values[outputVar.name]);
        }

        if (outputVar.unknown) {
            var del = $('<span class="fa fa-times fa-fw" style="margin-left: 7px" title="Smazat"></span>');

            valueWrapper.prepend('<div class="input-group-addon"><span class="fa fa-warning fa-fw" title="Neočekávaná výstupní proměnná"></span></div>');
            addOn.append(del);
            del.click(TB.wizard._deleteUnexpectedVar);
        }

        group.appendTo(target);

        value.catcomplete({
            delay: 0,
            source: TB.wizard.variableList
        });
    },

    loadViewColumns: function(viewName) {
        $.ajax({
            url: '/api/database/apps/' + $('#currentAppId').val() + '/viewscheme/' + viewName,
            type: 'post',
            dataType: 'json',
            success: function (data) {
                var self = TB.wizard;

                for (var i = 0; i < data.Columns.length; i++) {
                    self.variableList.push({
                        label: viewName + '.[' + data.Columns[i] + ']',
                        category: 'Views'
                    });
                }
            }
        });
    }, 

    sort: function (a, b) {
        var sortKeyA = a.category + '_' + a.label;
        var sortKeyB = b.category + '_' + b.label;
        return sortKeyA.localeCompare(sortKeyB);
    },

    getRequestedCategory: function()
    {
        var inVar = $(this).parents('.form-group').find('.control-label').data('invar');
        var outVar = $(this).parents('.form-group').find('.input-group-addon:last-child').data('outvar');

        if (inVar) {
            switch (inVar) {
                case 'TableName': return 'Tables';
                case 'ViewName': return 'Views';
            }
            if (inVar.indexOf('Column') !== -1) return 'Column';
        }
        if (outVar) {
            return 'System|Workflow|UI input data';
        }
        
        return '';
    },

    getRequestedWorkflow: function()
    {
        return TB.wizard.target.parents('.workflowRule').find('.workflowRuleHeader .verticalLabel').text();
    },

    getRequestedTable: function() {
        var tableRow = $(this).parents('.inputVars').find('.control-label[data-invar=TableName]').parent();
        if(tableRow.find('select[name=var_type]').val() == 's$') {
            return tableRow.find('input[name=var_value]').val();
        }
        return '';
    },

    getRequestedView: function() {
        var viewRow = $(this).parents('.inputVars').find('.control-label[data-invar=ViewName]').parent();
        if(viewRow.find('select[name=var_type]').val() == 's$') {
            return viewRow.find('input[name=var_value]').val();
        }
        return '';
    },

    setInputVarAutocomplete: function(target) {
        target.catcomplete({
            delay: 0,
            source: TB.wizard.variableList,
            search: TB.wizard._autocompleteSearch,
            select: TB.wizard._autocompleteSelect
        });
    },

    /******************************************************/
    /* EVENT CALLBACKS                                    */
    /******************************************************/
    _libraryClean: function () {
        TB.wizard.actions = {};
    },

    _libraryCreateItem: function (type) {
        var self = TB.wizard;

        switch(type) {
            case 'action':
                self.actions[this.Id] = {
                    name: this.Name,
                    inputVars: self.parseVars(this.InputVars),
                    outputVars: self.parseVars(this.OutputVars)
                }
                break;
            case 'ui':
                if(typeof this.ElmId != 'undefined' || typeof this.ComponentName != 'undefined') {
                    self.variableList.push({
                        label: typeof this.ElmId != 'undefined' ? this.ElmId : this.ComponentName,
                        category: 'UI elements'
                    });

                    if (typeof this.UIC != 'undefined' && typeof self.dataPrefixList[this.UIC] != 'undefined') {
                        for (var i = 0; i < self.dataPrefixList[this.UIC].length; i++) {
                            self.variableList.push({
                                label: '_uic_' + self.dataPrefixList[this.UIC][i] + '_' + this.ElmId,
                                category: 'UI input data'
                            });
                        }
                    }
                    if (typeof this.ComponentName != 'undefined' && typeof self.dataPrefixList[this.Type] != 'undefined') {
                        for (var i = 0; i < self.dataPrefixList[this.Type].length; i++) {
                            self.variableList.push({
                                label: '_uic_' + self.dataPrefixList[this.Type][i] + '_' + this.ComponentName,
                                category: 'UI input data'
                            });
                        }
                    }

                    self.variableList.sort(self.sort);
                }
                break;
        }
    },

    _attributesLoad: function(data) {
        var self = TB.wizard;

        for (var ti = 0; ti < data.Tables.length; ti++) {
            self.variableList.push({
                label: data.Tables[ti].Name,
                category: 'Tables'
            });
            self.variableList.push({
                label: '__Model.' + data.Tables[ti].Name,
                category: 'System'
            });

            if (data.Tables[ti].Columns.length) {
                for (var ci = 0; ci < data.Tables[ti].Columns.length; ci++) {
                    self.variableList.push({
                        label: data.Tables[ti].Name + '.' + data.Tables[ti].Columns[ci].Name,
                        category: 'Tables'
                    });
                    self.variableList.push({
                        label: '__Model.' + data.Tables[ti].Name + '.' + data.Tables[ti].Columns[ci].Name,
                        category: 'System'
                    });
                }
            }
        }
        for (var vi = 0; vi < data.Views.length; vi++) {
            self.variableList.push({
                label: data.Views[vi].Name,
                category: 'Views'
            });
            self.loadViewColumns(data.Views[vi].Name);
        }
        
        // Shared tables & views
        if (data.Shared != null) {
            for (var ti = 0; ti < data.Shared.Tables.length; ti++) {
                self.variableList.push({
                    label: data.Shared.Tables[ti].Name,
                    category: 'Tables'
                });
                self.variableList.push({
                    label: '__Model.' + data.Shared.Tables[ti].Name,
                    category: 'System'
                });

                if (data.Shared.Tables[ti].Columns.length) {
                    for (var ci = 0; ci < data.Shared.Tables[ti].Columns.length; ci++) {
                        self.variableList.push({
                            label: data.Shared.Tables[ti].Name + '.' + data.Shared.Tables[ti].Columns[ci].Name,
                            category: 'Tables'
                        });
                        self.variableList.push({
                            label: '__Model.' + data.Shared.Tables[ti].Name + '.' + data.Shared.Tables[ti].Columns[ci].Name,
                            category: 'System'
                        });
                    }
                }
            }

            for (var vi = 0; vi < data.Shared.Views.length; vi++) {
                self.variableList.push({
                    label: data.Shared.Views[vi].Name,
                    category: 'Views'
                });
            }
        }

        self.variableList.sort(self.sort);
    },

    _workflowCreateItem: function() {
        var self = TB.wizard;
        if (this.attr('actionId') && this.attr('actionId').length && this.data('outputVariables')) {
            var values = TB.wizard.parseOutputValue(this);
            if (values.length) {
                for (var k in values.items) {
                    self.variableList.push({
                        label: values.items[k],
                        category: 'Workflow: ' + this.parents('.workflowRule').find('.workflowRuleHeader .verticalLabel').text()
                    });
                }
                self.variableList.sort(self.sort);
            }
        }
    },

    _changeType: function () {
        var hasEnum = $(this).data('hasEnum');

        $(this).parents('.form-group')
            .find('input[name=var_value]').attr('disabled', (this.value != '_var_' && hasEnum) || this.value == 'b$').toggle((this.value == '_var_' || !hasEnum) && this.value != 'b$').end()
            .find('select.enum-value').attr('disabled', this.value == '_var_' || !hasEnum).toggle(this.value != '_var_' && hasEnum).end()
            .find('select.boolean-value').attr('disabled', this.value != 'b$').toggle(this.value == 'b$');

        if (this.value == 'f$' || this.value == 'i$') {
            $(this).parents('.form-group').find('input[name=var_value]').attr('type', 'number');
        }
        else {
            $(this).parents('.form-group').find('input[name=var_value]').attr('type', 'text');
        }
    },

    _deleteUnexpectedVar: function () {
        $(this).parents('.form-group').remove();
    },

    _autocompleteSearch: function (event, ui) {
        var requestedCategory;
        var type = $(this).parents('.form-group').find('select[name=var_type]').val();

        switch (type) {
            case '_var_': requestedCategory = '*'; break;
            case 's$': requestedCategory = TB.wizard.getRequestedCategory.apply(this, []); break;
            default: requestedCategory = ''; break;
        }

        if (requestedCategory == '') {
            return false;
        }
    },

    _autocompleteSelect: function (event, ui) {
        var requestedCategory = TB.wizard.getRequestedCategory.apply(this, []);
        
        if (requestedCategory == 'Column') {
            this.value = ui.item.value.replace(/^[^\.]+\./, '');
            return false;
        }
    },

    _addArrayItem: function () {
        var group = $(this).parents('.form-group');
        var label = group.find('.control-label');
        var inputVar = label.data('invar');

        var lastGroup = $('.control-label[data-invar=' + inputVar + ']').last().parents('.form-group');
        var lastIndex = Number(lastGroup.find('.control-label').data('index'));

        lastGroup.find('input[name=var_value]').catcomplete('destroy');

        var newIndex = lastIndex + 1;
        var newGroup = lastGroup.clone(true);
        var newLabel = newGroup.find('.control-label')
        
        newLabel.attr('data-index', newIndex).html(newLabel.html().replace(/\[\d+\]/, '[' + newIndex + ']'));
        newGroup.find('[name=var_value]').val('');
        newGroup.insertAfter(lastGroup);

        TB.wizard.setInputVarAutocomplete(newGroup.find('input[name=var_value]'));
        TB.wizard.setInputVarAutocomplete(lastGroup.find('input[name=var_value]'));
    },

    _deleteArrayItem: function () {
        var group = $(this).parents('.form-group');
        var label = group.find('.control-label');
        var inputVar = label.data('invar');

        group.remove();

        $('.control-label[data-invar=' + inputVar + ']').each(function (index) {
            $(this).attr('data-index', index).html(this.innerHTML.replace(/\[\d+\]/, '[' + index + ']'));
        });
    }
};

TB.onInit.push(TB.wizard.init);
function LoadModuleAdminScript() {
    $("#moduleAdminPanel .moduleSquare").on("click", function () {
        $("#moduleAdminPanel .moduleSquare").removeClass("selectedSquare");
        $(this).addClass("selectedSquare");
        $("#moduleConfigPanel .currentModuleIcon").css("background-image", $(this).css("background-image"));
        $("#moduleConfigPanel .currentModuleName").text($(this).attr("moduleName"));
    });
}
$(function () {
    if ($("#moduleAdminPanel").length) {
        LoadModuleAdminScript();
    }
});
var maintenanceModeActive = false;

var pageSpinner = (function () {
    var debug = false;
    var uses = 1;
    return {
        show: function (n) {
            if (!arguments.length) {
                n = 1;
            }
            uses += n;
            if (uses > 0) {
                $(document.body).addClass("pageSpinnerShown");
            }
            if (debug) {
                console.log("page spinner shown %d times, %d total", n, uses);
                console.trace();
            }
        },
        hide: function (n) {
            if (!arguments.length) {
                n = 1;
            }
            uses -= n;
            if (uses <= 0) {
                $(document.body).removeClass("pageSpinnerShown");
            }
            if (debug) {
                console.log("page spinner hidden %d times, %d remaining", n, uses);
                console.trace();
            }
        }
    }
})()

$(function () {
    var currentModule = document.body.getAttribute("data-module");

    $(document).on("ajaxError", function (event, jqxhr, settings, thrownError) {
        ShowAppNotification(jqxhr.responseText || "nastala chyba sítě", "error");
    })
    $(window).on("error", function () {
        ShowAppNotification("Nastala neočekávaná chyba", "error");
    })
    $("[data-ajax='true']").data("ajax-failure", function (xhr) {
        ShowAppNotification(xhr.responseText || "nastala chyba sítě", "error");
    }.toString()); 

    pageSpinner.hide();
    $(window).on("beforeunload", function () {
        pageSpinner.show();
    });

    $("#identitySuperMenu").on("click", function () {
        $("#leftBar .leftMenu li.identitySubMenu").slideToggle();
    });
    $("#appSuperMenu").on("click", function () {
        $("#leftBar .leftMenu li.appSubMenu").slideToggle();
    });

    if (CurrentModuleIs("portalModule")) {
        $("#adminMenuPortal").addClass("active");
    }
    else if (CurrentModuleIs("adminAppModule")) {
        $("#adminMenuApps").addClass("active");
        
    }
    else if (CurrentModuleIs("nexusModule")) {
        $("#adminMenuNexus").addClass("active");
    }
    else if (CurrentModuleIs("tapestryModule") || CurrentModuleIs("overviewModule")) {
        $("#adminMenuTapestry").addClass("active");
        $("#leftBar .leftMenu li.appSubMenu").show();
    }
    else if (CurrentModuleIs("mozaicModule") || CurrentModuleIs("mozaicEditorModule")) {
        $("#adminMenuMozaic").addClass("active");
        $("#leftBar .leftMenu li.appSubMenu").show();
    }
    else if (CurrentModuleIs("dbDesignerModule")) {
        $("#adminMenuDbDesigner").addClass("active");
        $("#leftBar .leftMenu li.appSubMenu").show();
    }
    else if (CurrentModuleIs("personaModule") || CurrentModuleIs("personaRolesModule")) {
        $("#adminMenuPersona").addClass("active");
        $("#leftBar .leftMenu li.identitySubMenu").show();
    }
    else if (CurrentModuleIs("personaModulesModule")) {
        $("#adminMenuPersonaModules").addClass("active");
        $("#leftBar .leftMenu li.identitySubMenu").show();
    }
    else if (CurrentModuleIs("watchtowerModule")) {
        $("#adminMenuWatchtower").addClass("active");
    }
    else if (CurrentModuleIs("hermesModule")) {
        $("#adminMenuHermes").addClass("active");
    }
    else if (CurrentModuleIs("cortexModule")) {
        $("#adminMenuCortex").addClass("active");
    }

    $("#usersOnlineIndicator").on("click", function () {
        $(".clickableIndicatorRectangle").removeClass("highlighted");
        $("#usersOnlineIndicator").addClass("highlighted");
        $.get("/CORE/Portal/UsersOnline").success(function (result) {
            $("#lowerPanelDynamicContainer").html(result);
        });
    });
    $("#activeProfileIndicator").on("click", function () {
        $(".clickableIndicatorRectangle").removeClass("highlighted");
        $("#activeProfileIndicator").addClass("highlighted");
        $.get("/CORE/Portal/ActiveProfile").success(function (result) {
            $("#lowerPanelDynamicContainer").html(result);
        });
    });
    $("#activeModulesIndicator").on("click", function () {
        $(".clickableIndicatorRectangle").removeClass("highlighted");
        $("#activeModulesIndicator").addClass("highlighted");
        $.get("/CORE/Portal/ModuleAdmin").success(function (result) {
            $("#lowerPanelDynamicContainer").html(result);
            LoadModuleAdminScript();
        });
    });
    $("#maintenanceIndicator").on("click", function () {
        if (maintenanceModeActive) {
            $("#maintenanceIndicator").removeClass("maintenanceActive");
            $("#maintenanceIndicator .indicatorLabel").text("vypnuta");
            maintenanceModeActive = false;
        }
        else {
            $("#maintenanceIndicator").addClass("maintenanceActive");
            $("#maintenanceIndicator .indicatorLabel").text("zapnuta");
            maintenanceModeActive = true;
        }
    });
    $("#notificationArea .indicatorBar").on("click", function () {
        $(this).remove();
    });

    $("#hideUpperPanelIcon").on("click", function () {
        $("#minimizedUpperPanel").show();
        $("#lowerPanel").css({ top: "+=" + $("#minimizedUpperPanel").height() + "px" });
        $("#lowerPanel").css({ top: "-=" + $("#upperPanel").height() + "px" });
        $("#upperPanel").hide();
        if (CurrentModuleIs("tapestryModule"))
            RecalculateToolboxHeight();
        else if (CurrentModuleIs("mozaicEditorModule"))
            RecalculateMozaicToolboxHeight();
    });
    $("#showUpperPanelIcon").on("click", function () {
        $("#upperPanel").show();
        $("#lowerPanel").css({ top: "+=" + $("#upperPanel").height() + "px" });
        $("#lowerPanel").css({ top: "-=" + $("#minimizedUpperPanel").height() + "px" });
        $("#minimizedUpperPanel").hide();
        if (CurrentModuleIs("tapestryModule"))
            RecalculateToolboxHeight();
        else if (CurrentModuleIs("mozaicEditorModule"))
            RecalculateMozaicToolboxHeight();
    });
    $("#topBar").width($(window).width());
    $("#upperPanel").width($(window).width() - 225);
    $("#minimizedUpperPanel").width($(window).width() - 225);
    $(window).on("resize", function () {
        $("#topBar").width($(window).width());
        $("#upperPanel").width($(window).width() - 225);
        $("#minimizedUpperPanel").width($(window).width() - 225);
    });
});

function SaveModulePermissions() {
    permissionArray = [];

    var rows = $("#moduleAccessTable").dataTable().fnGetNodes();
    for (var i = 0; i < rows.length; i++) {
        var userId = parseInt($(rows[i]).find("td:eq(0)").text());
      
        permissionArray.push({
            UserId: userId,
            Core: ($(rows[i]).find("td[moduleId=Core]").hasClass("yesCell")),
            Master: ($(rows[i]).find("td[moduleId=Master]").hasClass("yesCell")),
            Tapestry: ($(rows[i]).find("td[moduleId=Tapestry]").hasClass("yesCell")),
            Entitron: ($(rows[i]).find("td[moduleId=Entitron]").hasClass("yesCell")),
            Persona: ($(rows[i]).find("td[moduleId=Persona]").hasClass("yesCell")),
            Nexus: ($(rows[i]).find("td[moduleId=Nexus]").hasClass("yesCell")),
            Sentry: ($(rows[i]).find("td[moduleId=Sentry]").hasClass("yesCell")),
            Hermes: ($(rows[i]).find("td[moduleId=Hermes]").hasClass("yesCell")),
            Athena: ($(rows[i]).find("td[moduleId=Athena]").hasClass("yesCell")),
            Watchtower: ($(rows[i]).find("td[moduleId=Watchtower]").hasClass("yesCell")),
            Cortex: ($(rows[i]).find("td[moduleId=Cortex]").hasClass("yesCell")),
            Mozaic: ($(rows[i]).find("td[moduleId=Mozaic]").hasClass("yesCell"))
        });
    }
   
    postData = {
        PermissionList: permissionArray
    };
    $.ajax({
        type: "POST",
        url: "/api/persona/module-permissions",
        data: postData,
        success: function () { alert("Module permissions has been updated!") }
    });
}

$(function () {
    if (CurrentModuleIs("personaModulesModule") || CurrentModuleIs("personaRolesModule")) {
        $('body').on('click','.checkboxCell', function () {

            checkboxCell = $(this);
            if (checkboxCell.hasClass("yesCell")) {
                checkboxCell.removeClass("yesCell");
                checkboxCell.addClass("noCell");
                checkboxCell.find(".fa").removeClass("fa-check").addClass("fa-times");
            }
            else {
                checkboxCell.removeClass("noCell");
                checkboxCell.addClass("yesCell");
                checkboxCell.find(".fa").removeClass("fa-times").addClass("fa-check");
            }
        });
    }
    if (CurrentModuleIs("personaModulesModule")) {
        $('body').on('click','#btnSaveModuleAccessTable', function () {
            SaveModulePermissions();
        });
        $('body').on('click','#btnReloadModuleAccessTable', function () {

            location.reload();
        });
    }
});

var instance;

jsPlumb.ready(function () {
    if (CurrentModuleIs("overviewModule")) {
        instance = jsPlumb.getInstance({
            ConnectionOverlays: [
                ["Arrow", { location: 1 }]
            ],
            Container: "#overviewPanel .scrollArea",
            Endpoint: "Blank",
            Anchor: "Continuous",
            Connector: ["Straight", { stub: [0, 0], gap: 0 }]
        });
        connectorPaintStyle = {
            lineWidth: 3,
            strokeStyle: "#455d73"
        };
        LoadMetablock();
    }
});

function SaveMetablock(callback, pageUnloading) {
    pageSpinner.show();
    blockArray = [];
    metablockArray = [];

    $("#overviewPanel .block").each(function (blockIndex, blockDiv) {
        currentBlock = $(blockDiv);
        currentBlock.attr("tempId", blockIndex);
        if (currentBlock.attr("blockId") == undefined) {
            isNew = true;
            currentBlock.attr("blockId", blockIndex);
        }
        else
            isNew = false;
        blockArray.push({
            Id: currentBlock.attr("blockId"),
            Name: currentBlock.find(".blockName").text(),
            AssociatedTableName: "",
            AssociatedTableId: currentBlock.attr("tableId"),
            PositionX: parseInt(currentBlock.css("left")),
            PositionY: parseInt(currentBlock.css("top")),
            IsNew: isNew,
            IsInitial: (currentBlock.attr("isInitial") == "true"),
            IsInMenu: currentBlock.data("IsInMenu") ? true : false
        });
    });
    $("#overviewPanel .metablock").each(function (metablockIndex, metablockDiv) {
        currentMetablock = $(metablockDiv);
        currentMetablock.attr("tempId", metablockIndex);
        if (currentMetablock.attr("metablockId") == undefined) {
            isNew = true;
            currentMetablock.attr("metablockId", metablockIndex);
        }
        else
            isNew = false;
        metablockArray.push({
            Id: currentMetablock.attr("metablockId"),
            Name: currentMetablock.find(".metablockName").text(),
            PositionX: parseInt(currentMetablock.css("left")),
            PositionY: parseInt(currentMetablock.css("top")),
            IsNew: isNew,
            IsInitial: (currentMetablock.attr("isInitial") == "true"),
            IsInMenu: currentMetablock.data("IsInMenu") ? true : false
        });
    });
    postData = {
        Name: $("#headerMetablockName").text(),
        Blocks: blockArray,
        Metablocks: metablockArray
    };
    appId = $("#currentAppId").val();
    metablockId = $("#currentMetablockId").val();
    $.ajax({
        type: "POST",
        url: "/api/tapestry/apps/" + appId + "/metablocks/" + metablockId,
        dataType: "json",
        data: postData,
        async: !pageUnloading,
        complete: function () {
            pageSpinner.hide()
        },
        success: function (data) {
            for (i = 0; i < data.BlockIdPairs.length; i++) {
                temporaryId = data.BlockIdPairs[i].TemporaryId;
                realId = data.BlockIdPairs[i].RealId;
                $("#overviewPanel .block[tempId='" + temporaryId + "']").attr("blockId", realId);
            }
            for (i = 0; i < data.MetablockIdPairs.length; i++) {
                temporaryId = data.MetablockIdPairs[i].TemporaryId;
                realId = data.MetablockIdPairs[i].RealId;
                $("#overviewPanel .metablock[tempId='" + temporaryId + "']").attr("metablockId", realId);
            }
            ChangedSinceLastSave = false;
            if (callback) callback();
        }
    });
}

$(function ()
{
    if($('body').hasClass('menuOrderModule'))
    {
        $('ul.sortable').sortable();

        $('#menuOrderForm').on('submit', function () {
            return false;
        });

        $('#btnOverview').click(function () {
            $('#openMetablockForm').submit();
        });

        $('#btnSave').click(function () {

            pageSpinner.show();
            
            var metablockOrder = {};
            var blockOrder = {};

            var i = 1;
            $('.sortable input[type=hidden]').each(function () {
                if ($(this).is('.metablock')) {
                    metablockOrder[this.value] = i;
                }
                else {
                    blockOrder[this.value] = i;
                }
                i++;
            });

            var postData = {
                Blocks: blockOrder,
                Metablocks: metablockOrder
            };

            $.ajax({
                type: "POST",
                url: "/api/tapestry/saveMenuOrder",
                contentType: 'application/json; charset=UTF-8',
                data: JSON.stringify(postData),
                complete: function () {
                    pageSpinner.hide()
                },
                success: function () {
                    ChangedSinceLastSave = true;
                }
            });
        });
    }
});
function LoadMetablock() {
    pageSpinner.show();
    appId = $("#currentAppId").val();
    metablockId = $("#currentMetablockId").val();
    url = "/api/tapestry/apps/" + appId + "/metablocks/" + metablockId;
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        complete: function () {
            pageSpinner.hide()
        },
        success: function (data) {
            $("#headerMetablockName").text(data.Name);
            $("#overviewPanel .block, #overviewPanel .metablock").each(function (index, element) {
                instance.removeAllEndpoints(element, true);
                $(element).remove();
            });
            for (i = 0; i < data.Blocks.length; i++) {
                currentBlockData = data.Blocks[i];
                newBlock = $('<div class="block" id="block' + currentBlockData.Id + '" isInitial="' + currentBlockData.IsInitial + '" style="left: '
                    + currentBlockData.PositionX + 'px; top: ' + currentBlockData.PositionY + 'px;" blockId="'
                    + currentBlockData.Id + '" tableId="' + currentBlockData.AssociatedTableId + '"><div class="blockName">'
                    + currentBlockData.Name + '</div><div class="blockInfo">'
                    + (currentBlockData.IsInitial ? 'Initial' : '') + '</div></div>');
                newBlock.data("IsInMenu", currentBlockData.IsInMenu);
                $("#overviewPanel .scrollArea").append(newBlock);
                instance.draggable(newBlock, {
                    containment: "parent",
                    stop: function () {
                        ChangedSinceLastSave = true;
                    }
                });
                newBlock.on("dblclick", function () {
                    blockToOpen = $(this);
                    SaveMetablock(function () {
                        openBlockForm = $("#openBlockForm");
                        openBlockForm.find("input[name='blockId']").val(blockToOpen.attr("blockId"));
                        openBlockForm.submit();
                    });                    
                });
            }
            for (i = 0; i < data.Metablocks.length; i++) {
                currentMetablockData = data.Metablocks[i];
                newMetablock = $('<div class="metablock" id="metablock' + currentMetablockData.Id + '" isInitial="' + currentMetablockData.IsInitial + '"style="left: '
                    + currentMetablockData.PositionX + 'px; top: ' + currentMetablockData.PositionY + 'px;" metablockId="' +
                    currentMetablockData.Id + '"><div class="metablockName">' + currentMetablockData.Name +
                    '</div><div class="metablockSymbol fa fa-th-large"></div><div class="metablockInfo">'
                    + (currentMetablockData.IsInitial ? 'Initial' : '') + '</div></div>');
                newMetablock.data("IsInMenu", currentMetablockData.IsInMenu);
                $("#overviewPanel .scrollArea").append(newMetablock);
                instance.draggable(newMetablock, {
                    containment: "parent",
                    stop: function () {
                        ChangedSinceLastSave = true;
                    }
                });
                newMetablock.on("dblclick", function () {
                    metablockToOpen = $(this);
                    SaveMetablock(function () {
                        openMetablockForm = $("#openMetablockForm");
                        openMetablockForm.find("input[name='metablockId']").val(metablockToOpen.attr("metablockId"));
                        openMetablockForm.submit();
                    });
                });
            }
            for (i = 0; i < data.Connections.length; i++) {
                console.log("Connection");
                currentConnectonData = data.Connections[i];
                // TODO: implement remote connection representation
                if (currentConnectonData.SourceType == 1)
                    sourceId = "metablock" + currentConnectonData.SourceId;
                else
                    sourceId = "block" + currentConnectonData.SourceId;
                if (currentConnectonData.TargetType == 1)
                    targetId = "metablock" + currentConnectonData.TargetId;
                else
                    targetId = "block" + currentConnectonData.TargetId;
                instance.connect({
                    source: sourceId, target: targetId, editable: false, paintStyle: connectorPaintStyle
                });
            }
        }
    });
}

$(function () {
    if (CurrentModuleIs("overviewModule")) {
        
        
        blockPropertiesDialog = $('#block-properties-dialog').dialog({
            autoOpen: false,
            width: 500,
            height: 250,
            buttons: {
                "Save": function () {
                    blockPropertiesDialog_SubmitData();
                },
                "Cancel": function () {
                    blockPropertiesDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        blockPropertiesDialog_SubmitData();
                        return false;
                    }
                });
            },
            open: function () {
                blockPropertiesDialog.find("#p-block-name").val(currentBlock.find('.blockName').text());
                blockPropertiesDialog.find("#block-is-in-menu").prop('checked', currentBlock.data('IsInMenu'));
                blockPropertiesDialog.find("#block-set-as-initial").prop('checked', currentBlock.attr('isinitial') == 'true');
            }
        });
        function blockPropertiesDialog_SubmitData() {
            blockPropertiesDialog.dialog("close");
            currentBlock.data("IsInMenu", blockPropertiesDialog.find("#block-is-in-menu").is(':checked'));
            
            var isInitial = blockPropertiesDialog.find("#block-set-as-initial").is(':checked') ? true : false;
            if (isInitial) {
                $("#overviewPanel .block").each(function (index, element) {
                    $(element).attr("isInitial", false);
                    $(element).find(".blockInfo").text("");
                });
            }
            currentBlock.attr("isInitial", isInitial);
            currentBlock.find(".blockInfo").text(isInitial ? "Initial" : "");

            if (blockPropertiesDialog.find("#p-block-name").val().length) {
                currentBlock.find('.blockName').html(blockPropertiesDialog.find("#p-block-name").val());
            }
            ChangedSinceLastSave = true;
        }

        metablockPropertiesDialog = $('#metablock-properties-dialog').dialog({
            autoOpen: false,
            width: 500,
            height: 250,
            buttons: {
                "Save": function () {
                    metablockPropertiesDialog_SubmitData();
                },
                "Cancel": function () {
                    metablockPropertiesDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        metablockPropertiesDialog_SubmitData();
                        return false;
                    }
                });
            },
            open: function () {
                metablockPropertiesDialog.find("#p-metablock-name").val(currentMetablock.find('.metablockName').text());
                metablockPropertiesDialog.find("#metablock-is-in-menu").prop('checked', currentMetablock.data('IsInMenu'));
                metablockPropertiesDialog.find("#metablock-set-as-initial").prop('checked', currentMetablock.attr('isinitial') == 'true');
            }
        });
        function metablockPropertiesDialog_SubmitData() {
            metablockPropertiesDialog.dialog("close");

            var isInitial = metablockPropertiesDialog.find("#metablock-set-as-initial").is(':checked') ? true : false;
            if(isInitial)
            {
                $("#overviewPanel .metablock").each(function (index, element) {
                    $(element).attr("isInitial", false);
                    $(element).find(".metablockInfo").text("");
                });
            }
            currentMetablock.attr("isInitial", isInitial);
            currentMetablock.find(".metablockInfo").text(isInitial ? "Initial" : "");

            currentMetablock.data("IsInMenu", metablockPropertiesDialog.find("#metablock-is-in-menu").is(':checked'));
            if (metablockPropertiesDialog.find("#p-metablock-name").val().length) {
                currentMetablock.find('.metablockName').html(metablockPropertiesDialog.find("#p-metablock-name").val());
            }
            ChangedSinceLastSave = true;
        }
    }
});

$(function () {
    if (CurrentModuleIs("nexusModule")) {
        if ($("#ldapMenuArea").length) {
            $("#nexusMenuLDAP").addClass("highlighted");
        }
        else if ($("#wsMenuArea").length) {
            $("#nexusMenuWebServices").addClass("highlighted");
        }
        else if ($("#extDbMenuArea").length) {
            $("#nexusMenuExtDB").addClass("highlighted");
        }
        else if ($("#webDavMenuArea").length) {
            $("#nexusMenuWebDav").addClass("highlighted");
        }
    }
});
function ShowWsdlButtonClick(button) {
    encodedString = $(button).parents("td").find(".wsdlFileString").text();
    CurrentWsdlFile = $("<div/>").html(encodedString).text();
    showWsdlDialog.dialog("open");
};

var CurrentWsdlFile;
$(function () {
    if (CurrentModuleIs("nexusModule")) {
        showWsdlDialog = $("#show-wsdl-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 800,
            height: 600,
            buttons: {
                "Zavřít": function () {
                    showWsdlDialog.dialog("close");
                }
            },
            open: function () {
                $(this).find("#wsdlFileText").text(CurrentWsdlFile);
            }
        });
    }
});

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
                CreateDroppableMozaicContainer(droppedElement, false);
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

function SaveMozaicPage() {
    pageSpinner.show();
    SaveRequested = false;
    componentArray = GetMozaicContainerComponentArray($("#mozaicPageContainer"), false);
    postData = {
        Name: $("#headerPageName").text(),
        IsModal: $("#currentPageIsModal").prop("checked"),
        ModalWidth: $("#modalWidthInput").val(),
        ModalHeight: $("#modalHeightInput").val(),
        Components: componentArray
    }
    appId = $("#currentAppId").val();
    pageId = $("#currentPageId").val();
    $.ajax({
        type: "POST",
        url: "/api/mozaic-editor/apps/" + appId + "/pages/" + pageId,
        data: postData,
        complete: function () {
            pageSpinner.hide();
        },
        success: function () { alert("OK") },
        error: function (request, status, error) {
            alert(request.responseText);
        }
    });
}
function GetMozaicContainerComponentArray(container, nested) {
    if (nested)
        componentArrayLevel2 = [];
    else
        componentArrayLevel1 = [];
    container.find(".uic").each(function (uicIndex, uicElement) {
        currentUic = $(uicElement);
        if (!nested && currentUic.parents(".panel-component").length > 0)
            return true;
        tag = null;
        label = null;
        content = null;
        if (currentUic.hasClass("button-simple") || currentUic.hasClass("button-dropdown")) {
            label = currentUic.text();
        }
        else if (currentUic.hasClass("info-container")) {
            label = currentUic.find(".info-container-header").text();
            content = currentUic.find(".info-container-body").text();
        }
        else if (currentUic.hasClass("static-html")) {
            content = currentUic.html();
        }
        if (currentUic.hasClass("info-container"))
            type = "info-container";
        else if (currentUic.hasClass("breadcrumb-navigation"))
            type = "breadcrumb";
        else if (currentUic.hasClass("button-simple"))
            type = "button-simple";
        else if (currentUic.hasClass("button-dropdown"))
            type = "button-dropdown";
        else if (currentUic.hasClass("button-browse"))
            type = "button-browse";
        else if (currentUic.hasClass("checkbox-control")) {
            type = "checkbox";
            label = currentUic.find(".checkbox-label").text();
        }
        else if (currentUic.hasClass("radio-control")) {
            type = "radio";
            label = currentUic.find(".radio-label").text();
        }
        else if (currentUic.hasClass("form-heading") || currentUic.hasClass("control-label")) {
            label = currentUic.html();
            content = currentUic.attr("contentTemplate");
            type = "label";
        }
        else if (currentUic.hasClass("input-single-line"))
            type = "input-single-line";
        else if (currentUic.hasClass("input-multiline"))
            type = "input-multiline";
        else if (currentUic.hasClass("dropdown-select"))
            type = "dropdown-select";
        else if (currentUic.hasClass("multiple-select"))
            type = "multiple-select";
        else if (currentUic.hasClass("data-table-with-actions"))
            type = "data-table-with-actions";
        else if (currentUic.hasClass("data-table"))
            type = "data-table-read-only";
        else if (currentUic.hasClass("name-value-list"))
            type = "name-value-list";
        else if (currentUic.hasClass("tab-navigation")) {
            type = "tab-navigation";
            tabString = "";
            currentUic.find("li").each(function (index, element) {
                if (index > 0)
                    tabString += $(element).find("a").text() + ";";
            });
            content = tabString;
        }
        else if (currentUic.hasClass("color-picker"))
            type = "color-picker";
        else if (currentUic.hasClass("countdown-component"))
            type = "countdown";
        else if (currentUic.hasClass("wizard-phases")) {
            type = "wizard-phases";
            var phaseLabels = "";
            currentUic.find(".phase-label").each(function (index, element) {
                phaseLabels += $(element).text() + ";";
            });
            phaseLabels = phaseLabels.slice(0, -1);
            content = phaseLabels;
        }
        else if (currentUic.hasClass("named-panel")) {
            type = "panel";
            label = currentUic.find(".named-panel-header").text();
        }
        else if (currentUic.hasClass("panel-component"))
            type = "panel";
        else
            type = "control";
        if (currentUic.hasClass("data-table")) {
            wrapper = currentUic.parents("");
            positionX = wrapper.css("left");
            positionY = wrapper.css("top");
        }
        else {
            positionX = currentUic.css("left");
            positionY = currentUic.css("top");
        }
        if (currentUic.hasClass("color-picker"))
            tag = "input";
        else
            tag = currentUic.prop("tagName").toLowerCase();
        name = currentUic.attr("uicName");
        if (!name || name == "")
            name = type + uicIndex;
        componentData = {
            Name: name,
            Type: type,
            PositionX: positionX,
            PositionY: positionY,
            Width: currentUic.css("width"),
            Height: currentUic.css("height"),
            Tag: tag,
            Attributes: currentUic.data("uicAttributes"),
            Classes: currentUic.attr("uicClasses"),
            Styles: currentUic.attr("uicStyles"),
            Properties: currentUic.attr("uicProperties") ? currentUic.attr("uicProperties") : "",
            Content: content,
            Label: label,
            Placeholder: currentUic.attr("placeholder"),
            TabIndex: currentUic.attr("tabindex"),
            ChildComponents: currentUic.hasClass("panel-component") ? GetMozaicContainerComponentArray(currentUic, true) : []
        };
        if (nested)
            componentArrayLevel2.push(componentData);
        else
            componentArrayLevel1.push(componentData);
    });
    if (nested)
        return componentArrayLevel2;
    else
        return componentArrayLevel1;
}
function LoadMozaicPage(pageId) {
    pageSpinner.show();
    appId = $("#currentAppId").val();
    if (pageId == "current")
        pageId = $("#currentPageId").val();
    url = "/api/mozaic-editor/apps/" + appId + "/pages/" + pageId;
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        complete: function () {
            pageSpinner.hide()
        },
        error: function (request, status, error) {
            alert(request.responseText);
        },
        success: function (data) {
            if ($('body').hasClass('mozaicBootstrapEditorModule')) {
                MBE.io.convert(data);
            }
            else
            {
                $("#mozaicPageContainer .uic").remove();
                $("#mozaicPageContainer .dataTables_wrapper").remove();
                $("#mozaicPageContainer .color-picker").remove();

                for (i = 0; i < data.Components.length; i++) {
                    LoadMozaicEditorComponents($("#mozaicPageContainer"), data.Components[i]);
                }
                $("#currentPageId").val(data.Id);
                $("#headerPageName").text(data.Name);
                $("#currentPageIsModal").prop("checked", data.IsModal);
                $("#modalWidthInput").val(data.ModalWidth);
                $("#modalHeightInput").val(data.ModalHeight);
                if ($("#currentPageIsModal").is(":checked")) {
                    $("#modalSizeVisualization").css("width", parseInt($("#modalWidthInput").val()));
                    $("#modalSizeVisualization").css("height", parseInt($("#modalHeightInput").val()));
                    $("#modalSizeVisualization").show();
                }

                var panels = $(".mozaicEditorAbsolute, .mozaicEditorBootstrap").removeClass("mozaicEditorAbsolute mozaicEditorBootstrap");
                switch (data.version) {
                    case "0":
                    default:
                        panels.addClass("mozaicEditorAbsolute");
                        break;
                    case "1":
                        panels.addClass("mozaicEditorBootstrap");
                        break;
                }
            }
        }
    });
}
function LoadMozaicEditorComponents(targetContainer, cData) {
    newComponent = $('<' + cData.Tag + ' id="' + cData.Id + '" uicName="' + cData.Name + /*'" uicAttributes="' + (cData.Attributes || "") + */'" class="uic ' + cData.Classes
                    + '" uicClasses="' + cData.Classes + '" uicStyles="' + cData.Styles + '" style="left: ' + cData.PositionX + '; top: ' + cData.PositionY + '; width: '
                    + cData.Width + '; height: ' + cData.Height + '; ' + cData.Styles + '"></' + cData.Tag + '>');
    newComponent.data("uicAttributes", cData.Attributes);

    targetContainer.append(newComponent);
    if (cData.Placeholder)
        newComponent.attr("placeholder", cData.Placeholder);
    if (cData.TabIndex)
        newComponent.attr("tabindex", cData.TabIndex);
   
    if (cData.Properties)
        newComponent.attr("uicProperties", cData.Properties);
    if (newComponent.hasClass("button-simple"))
        newComponent.text(cData.Label);
    else if (newComponent.hasClass("button-dropdown"))
        newComponent.html(cData.Label + '<i class="fa fa-caret-down"></i>');
    else if (newComponent.hasClass("info-container")) {
        newComponent.append($('<div class="fa fa-info-circle info-container-icon"></div>'
            + '<div class="info-container-header"></div>'
            + '<div class="info-container-body"></div>'));
        newComponent.find(".info-container-header").text(cData.Label);
        newComponent.find(".info-container-body").text(cData.Content);
    }
    else if (newComponent.hasClass("static-html")) {
        newComponent.html(cData.Content);
    }
    else if (newComponent.hasClass("named-panel")) {
        newComponent.append($('<div class="named-panel-header"></div>'));
        newComponent.find(".named-panel-header").text(cData.Label);
    }
    else if (newComponent.hasClass("multiple-select")) {
        newComponent.append($('<option value="1">Multiple</option><option value="2">Choice</option><option value="3">Select</option>'));
        newComponent.attr("multiple", "");
    }
    else if (newComponent.hasClass("button-browse")) {
        newComponent.attr("type", "file");
    }
    else if (newComponent.hasClass("form-heading") || newComponent.hasClass("control-label")) {
        newComponent.html(cData.Label);
        newComponent.attr("contentTemplate", cData.Content);
    }
    else if (newComponent.hasClass("checkbox-control")) {
        newComponent.append($('<input type="checkbox" /><span class="checkbox-label">' + cData.Label + '</span>'));
    }
    else if (newComponent.hasClass("radio-control")) {
        newComponent.append($('<input type="radio" name="' + cData.Name + '" /><span class="radio-label">' + cData.Label + '</span>'));
    }
    else if (newComponent.hasClass("breadcrumb-navigation")) {
        newComponent.append($('<div class="app-icon fa fa-question"></div><div class="nav-text">APP NAME &gt; Nav</div>'));
    }
    else if (newComponent.hasClass("data-table")) {
        newComponent.append($('<thead><tr><th>Column 1</th><th>Column 2</th><th>Column 3</th></tr></thead>'
            + '<tbody><tr><td>Value1</td><td>Value2</td><td>Value3</td></tr><tr><td>Value4</td><td>Value5</td><td>Value6</td></tr>'
            + '<tr><td>Value7</td><td>Value8</td><td>Value9</td></tr></tbody>'));
        CreateCzechDataTable(newComponent, newComponent.hasClass("data-table-simple-mode"));
        newComponent.css("width", cData.Width);
        wrapper = newComponent.parents(".dataTables_wrapper");
        wrapper.css("position", "absolute");
        wrapper.css("left", cData.PositionX);
        wrapper.css("top", cData.PositionY);
        newComponent.css("position", "relative");
        newComponent.css("left", "0px");
        newComponent.css("top", "0px");
    }
    else if (newComponent.hasClass("name-value-list")) {
        newComponent.append($('<tr><td class="name-cell">Platform</td><td class="value-cell">Omnius</td></tr><tr><td class="name-cell">Country</td>'
            + '<td class="value-cell">Czech Republic</td></tr><tr><td class="name-cell">Year</td><td class="value-cell">2016</td></tr>'));
    }
    else if (newComponent.hasClass("tab-navigation")) {
        tabLabelArray = cData.Content.split(";");
        newComponent.append($('<li class="active"><a class="fa fa-home"></a></li>'));
        for (k = 0; k < tabLabelArray.length; k++) {
            if (tabLabelArray[k].length > 0)
                newComponent.append($("<li><a>" + tabLabelArray[k] + "</a></li>"));
        }
        newComponent.css("width", "auto");
    }
    else if (newComponent.hasClass("color-picker")) {
        CreateColorPicker(newComponent);
        newReplacer = targetContainer.find(".sp-replacer:last");
        newReplacer.css("position", "absolute");
        newReplacer.css("left", newComponent.css("left"));
        newReplacer.css("top", newComponent.css("top"));
        newComponent.removeClass("uic");
        newReplacer.addClass("uic color-picker");
        newReplacer.attr("uicClasses", "color-picker");
        newReplacer.attr("uicName", newComponent.attr("uicName"));
    }
    else if (newComponent.hasClass("countdown-component")) {
        newComponent.html('<span class="countdown-row countdown-show3"><span class="countdown-section"><span class="countdown-amount">0</span>'
            + '<span class="countdown-period">Hodin</span></span><span class="countdown-section"><span class="countdown-amount">29</span>'
            + '<span class="countdown-period">Minut</span></span><span class="countdown-section"><span class="countdown-amount">59</span>'
            + '<span class="countdown-period">Sekund</span></span></span>');
    }
    else if (newComponent.hasClass("wizard-phases")) {
        newComponent.html(WizardPhasesContentTemplate);
        var phaseLabelArray = cData.Content.split(";");
        newComponent.find(".phase1 .phase-label").text(phaseLabelArray[0] ? phaseLabelArray[0] : "Fáze 1");
        newComponent.find(".phase2 .phase-label").text(phaseLabelArray[1] ? phaseLabelArray[1] : "Fáze 2");
        newComponent.find(".phase3 .phase-label").text(phaseLabelArray[2] ? phaseLabelArray[2] : "Fáze 3");
    }

    if (newComponent.hasClass("panel-component")) { //mšebela: odstraněno else před if (kvůli named-component)
        CreateDroppableMozaicContainer(newComponent, false);
    }
    if (newComponent.hasClass("data-table"))
        draggableElement = wrapper;
    else if (newComponent.hasClass("color-picker"))
        draggableElement = newReplacer;
    else
        draggableElement = newComponent;
    draggableElement.draggable({
        cancel: false,
        containment: "parent",
        drag: function (event, ui) {
            if (GridResolution > 0) {
                ui.position.left -= (ui.position.left % GridResolution);
                ui.position.top -= (ui.position.top % GridResolution);
            }
        }
    });
    if (cData.ChildComponents) {
        currentPanel = newComponent;
        for (j = 0; j < cData.ChildComponents.length; j++) {
            LoadMozaicEditorComponents(currentPanel, cData.ChildComponents[j]);
        }
    }
}

var GridResolution = 0;
$(function () {
    if (CurrentModuleIs("mozaicEditorModule") && !$('body').hasClass('mozaicBootstrapEditorModule')) {
        RecalculateMozaicToolboxHeight();
        pageId = $("#currentPageId").val();
        if (pageId)
            LoadMozaicPage(pageId);

        $("#headerPageName").on("click", function () {
            renamePageDialog.dialog("open");
        });
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
            if (pageId == 0) {
                SaveRequested = true;
                newPageDialog.dialog("open");
            }
            else
                SaveMozaicPage();
        });
        $("#btnLoad").on("click", function () {
            LoadMozaicPage("current");
        });
        $("#btnTrashPage").on("click", function () {
            trashPageDialog.dialog("open");
        });
        $("#btnToBootstrap").on("click", function() {
            $(".mozaicEditorAbsolute").removeClass("mozaicEditorAbsolute").addClass("mozaicEditorBootstrap");
            RecalculateMozaicToolboxHeight();
            convertAbsoluteToBootstrap();
        });
        $("#hideMozaicTooboxIcon").on("click", function () {
            $("#mozaicLeftBar").hide();
            $("#mozaicLeftBarMinimized").show();
            $("#mozaicPageContainer").css("left", 32);
            $("#mozaicPageContext").css('left', 225 + 32);
            RecalculateMozaicToolboxHeight();
        });
        $("#showMozaicTooboxIcon").on("click", function () {
            $("#mozaicLeftBar").show();
            $("#mozaicLeftBarMinimized").hide();
            $("#mozaicPageContainer").css("left", $('body').hasClass('mozaicBootstrapEditorModule') ? 250 : 300);
            $("#mozaicPageContext").css('left', 225 + 250);
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
                    ui.position.left = Math.round(ui.position.left / GridResolution) * GridResolution;
                    ui.position.top = Math.round(ui.position.top / GridResolution) * GridResolution;
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
            var topBarHeight = $("#topBar").height() + $("#appNotificationArea").height();
            var overlay = $("#lowerPanelSpinnerOverlay");
            var context = $("#mozaicPageContext");
            var tree = $("#mozaicPageTree");

            overlay.css({ right: 0, width: 'auto' });
            if (scrollTop > lowerPanelTop - topBarHeight) {
                leftBar.css({ top: topBarHeight, left: 225, position: "fixed" });
                overlay.css({ top: topBarHeight, left: 225, position: "fixed" });
                context.css({ top: topBarHeight, right: 0, left: $("#mozaicLeftBar").is(':visible') ? 475 : 225 + 32, position: "fixed", zIndex: 1 });
                tree.css({ top: topBarHeight, right: 0, position: "fixed" });
            } else {
                leftBar.css({ top: 0, left: 0, position: "absolute" });
                overlay.css({ top: 0, left: 0, position: "absolute" });
                context.css({ position: "static" });
                tree.css({ top: 0, right: 0, position: "absolute" });
            }
            RecalculateMozaicToolboxHeight();
        });
        $(window).resize(function () {
            RecalculateMozaicToolboxHeight();
        });

        function convertAbsoluteToBootstrap() {

        }

    } else if (CurrentModuleIs("mozaicComponentManagerModule")) {
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

var CurrentComponent, SaveRequested = false;
$(function () {
    if (CurrentModuleIs("mozaicEditorModule") && !$('body').hasClass('mozaicBootstrapEditorModule')) {
        componentPropertiesDialog = $("#component-properties-dialog").dialog({
            autoOpen: false,
            width: 700,
            height: 'auto',
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
                componentPropertiesDialog.find("#component-attributes").val(CurrentComponent.data("uicAttributes"))

                // Show table row for relevant attributes by default
                componentPropertiesDialog.find("#component-placeholder").parents('tr').show();
                componentPropertiesDialog.find("#component-tabindex").parents('tr').show();
                componentPropertiesDialog.find("#component-label").parents('tr').show();
                if (CurrentComponent.hasClass("input-single-line") || CurrentComponent.hasClass("input-multiline")) {
                    componentPropertiesDialog.find("#component-placeholder").val(CurrentComponent.attr("placeholder"));
                    componentPropertiesDialog.find("#component-label").parents('tr').hide();
                }
                else if (CurrentComponent.hasClass("info-container")) {
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.find(".info-container-header").text());
                    componentPropertiesDialog.find("#component-content").val(CurrentComponent.find(".info-container-body").text());
                    componentPropertiesDialog.find("#component-placeholder").parents('tr').hide();
                    componentPropertiesDialog.find("#component-tabindex").parents('tr').hide();
                }
                else if (CurrentComponent.hasClass("static-html")) {
                    componentPropertiesDialog.find("#component-content").val(CurrentComponent.html());
                }
                else if (CurrentComponent.hasClass("named-panel")) {
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.find(".named-panel-header").text());
                }
                else if (CurrentComponent.hasClass("form-heading") || CurrentComponent.hasClass("control-label")) {
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.html());
                    componentPropertiesDialog.find("#component-content").val(CurrentComponent.attr("contentTemplate"));
                    componentPropertiesDialog.find("#component-placeholder").parents('tr').hide();
                    componentPropertiesDialog.find("#component-tabindex").parents('tr').hide();
                }
                else if (CurrentComponent.hasClass("checkbox-control")) {
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.find(".checkbox-label").text());
                    componentPropertiesDialog.find("#component-content").val("");
                    componentPropertiesDialog.find("#component-placeholder").parents('tr').hide();
                }
                else if (CurrentComponent.hasClass("radio-control")) {
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.find(".radio-label").text());
                    componentPropertiesDialog.find("#component-content").val("");
                    componentPropertiesDialog.find("#component-placeholder").parents('tr').hide();
                }
                else if (CurrentComponent.hasClass("tab-navigation")) {
                    componentPropertiesDialog.find("#component-label").val("");
                    tabString = "";
                    CurrentComponent.find("li").each(function (index, element) {
                        if (index > 0)
                            tabString += $(element).find("a").text() + ";";
                    });
                    componentPropertiesDialog.find("#component-content").val(tabString);
                    componentPropertiesDialog.find("#component-placeholder").parents('tr').hide();
                    componentPropertiesDialog.find("#component-label").parents('tr').hide();
                }
                else if (CurrentComponent.hasClass("wizard-phases")) {
                    componentPropertiesDialog.find("#component-label").val("");
                    var phaseLabels = "";
                    CurrentComponent.find(".phase-label").each(function (index, element) {
                        phaseLabels += $(element).text() + ";";
                    });
                    phaseLabels = phaseLabels.slice(0, -1);
                    componentPropertiesDialog.find("#component-content").val(phaseLabels);
                    componentPropertiesDialog.find("#component-placeholder").parents('tr').hide();
                    componentPropertiesDialog.find("#component-label").parents('tr').hide();
                }
                else if (CurrentComponent.hasClass("button-simple") || CurrentComponent.hasClass("button-dropdown")) {
                    componentPropertiesDialog.find("#component-label").val(CurrentComponent.text());
                    componentPropertiesDialog.find("#component-placeholder").parents('tr').hide();
                }
                else if (CurrentComponent.hasClass("name-value-list") || CurrentComponent.hasClass("panel-component")
                    || CurrentComponent.hasClass("countdown-component")) {
                    componentPropertiesDialog.find("#component-label").val("");
                    componentPropertiesDialog.find("#component-content").val("");
                    componentPropertiesDialog.find("#component-placeholder").parents('tr').hide();
                    componentPropertiesDialog.find("#component-tabindex").parents('tr').hide();
                }
                else {
                    // Classes: data-dable, color-picker
                    componentPropertiesDialog.find("#component-label").val("");
                    componentPropertiesDialog.find("#component-content").val("");
                    componentPropertiesDialog.find("#component-placeholder").parents('tr').hide();
                }
            }
        });
        function componentPropertiesDialog_SubmitData() {

            CurrentComponent.attr("uicName", componentPropertiesDialog.find("#component-name").val());
            CurrentComponent.css("width", componentPropertiesDialog.find("#component-width").val());

            CurrentComponent.parents(".dataTables_wrapper").css("width", CurrentComponent.css("width"));   //to stejné v userinit

            CurrentComponent.css("height", componentPropertiesDialog.find("#component-height").val());
            CurrentComponent.attr("uicStyles", componentPropertiesDialog.find("#component-styles").val());
            CurrentComponent.attr("uicProperties", componentPropertiesDialog.find("#component-props").val());
            CurrentComponent.attr("tabindex", componentPropertiesDialog.find("#component-tabindex").val());
            CurrentComponent.data("uicAttributes", componentPropertiesDialog.find("#component-attributes").val());
            if (CurrentComponent.hasClass("button-simple"))
                CurrentComponent.text(componentPropertiesDialog.find("#component-label").val());
            else if (CurrentComponent.hasClass("button-dropdown"))
                CurrentComponent.html(componentPropertiesDialog.find("#component-label").val() + '<i class="fa fa-caret-down">');
            else if (CurrentComponent.hasClass("input-single-line") || CurrentComponent.hasClass("input-multiline"))
                CurrentComponent.attr("placeholder", componentPropertiesDialog.find("#component-placeholder").val());
            else if (CurrentComponent.hasClass("info-container")) {
                CurrentComponent.find(".info-container-header").text(componentPropertiesDialog.find("#component-label").val());
                CurrentComponent.find(".info-container-body").text(componentPropertiesDialog.find("#component-content").val());
            }
            else if (CurrentComponent.hasClass("static-html")) {
                CurrentComponent.html(componentPropertiesDialog.find("#component-content").val());
            }
            else if (CurrentComponent.hasClass("named-panel")) {
                CurrentComponent.find(".named-panel-header").text(componentPropertiesDialog.find("#component-label").val());
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
                    // Clear MozaicPageContainer, but only when deleted page is currently opened
                    if ($("#currentPageId").val() == pageId) {
                        $("#mozaicPageContainer .uic").remove();
                        $("#mozaicPageContainer .dataTables_wrapper").remove();
                        $("#mozaicPageContainer .color-picker").remove();
                        $("#headerPageName").remove();
                    }
                },
                error: function (request, status, error) {
                    alert(request.responseText);
                }
            });    
        }
        newPageDialog = $("#new-page-dialog").dialog({
            autoOpen: false,
            width: 400,
            height: 170,
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
                newPageDialog.find('#new-page-name').val("");
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
            newPageName = newPageDialog.find("#new-page-name").val()
            $.ajax({
                type: "POST",
                url: "/api/mozaic-editor/apps/" + appId + "/pages",
                data: postData,
                error: function (request, status, error) {
                    alert(request.responseText);
                },
                success: function (data) {
                    $("#currentPageId").val(data);
                    if (newPageName == "")
                        $("#headerPageName").text("Nepojmenovaná stránka");
                    else
                        $("#headerPageName").text(newPageName);
                    
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

$(function () {
    if (CurrentModuleIs("hermesModule")) {
        if ($("#smtpMenuArea").length) {
            $("#hermesMenuSMTP").addClass("highlighted");
        }
        if ($("#templateMenuArea").length) {
            $("#hermesMenuTemplate").addClass("highlighted");
        }
        if ($("#queueMenuArea").length) {
            $("#hermesMenuQueue").addClass("highlighted");
        }
    }
});

var instance;

jsPlumb.ready(function () {
    if (CurrentModuleIs("dbDesignerModule")) {
        instance = jsPlumb.getInstance({
            Endpoint: ["Blank", {}],
            HoverPaintStyle: { strokeStyle: "#ff4000", lineWidth: 2 },
            ConnectionOverlays: [
                ["Arrow", {
                    location: 1,
                    id: "arrow",
                    length: 14,
                    foldback: 0.8
                }],
            ],
            Container: "database-container"
        });

        instance.bind("click", function (con) {
            CurrentConnection = con;
            editRelationDialog.dialog("open");
        });

        instance.bind("connection", function (info) {
            if ($(info.connection.source).attr("dbColumnType") != $(info.connection.target).attr("dbColumnType")) {
                instance.detach(info.connection);
                alert("These columns have different types. Relation can only be created between columns of the same type.");
                return false;
            }
            info.connection.addClass("relationConnection");
            info.connection.removeOverlay("arrow");
            info.connection.addOverlay(["Arrow", {
                location: 0,
                id: "arrow0",
                length: 8,
                width: 8,
                height: 8,
                foldback: 0.8,
                direction: -1
            }]);
            info.connection.addOverlay(["Arrow", {
                location: 1,
                id: "arrow1",
                length: 8,
                width: 8,
                height: 8,
                foldback: 0.8
            }]);
            info.connection.addOverlay(["Label", {
                location: 0.1,
                id: "label0",
                cssClass: "relationLabel",
                label: "1"
            }]);
            info.connection.addOverlay(["Label", {
                location: 0.9,
                id: "label1",
                cssClass: "relationLabel",
                label: "1"
            }]);
        });

        instance.batch(function () {
            $(".dbTable").each(function (index, element) {
                instance.draggable(element);
            });

            $(".dbColumn").each(function (index, element) {
                AddColumnToJsPlumb(element);
            });
        });
    }
});

function ClearDbScheme() {
    jQuery.each($("#database-container .dbTable"), function (i, val) {
        instance.removeAllEndpoints(val, true);
        val.remove();
    });
    jQuery.each($("#database-container .dbView"), function (i, val) {
        val.remove();
    });
};

function AddColumnToJsPlumb(item) {
    instance.makeSource(item, {
        anchor: ["Continuous", { faces: ["left", "right"] }],
        faces: ["left", "right"],
        container: "database-container",
        connector: ["Bezier", {curviness: 150}],
        connectorStyle: { strokeStyle: "#1092bd", lineWidth: 2, outlineColor: "transparent", outlineWidth: 4 }
    });

    instance.makeTarget(item, {
        dropOptions: { hoverClass: "dragHover" },
        anchor: ["Continuous", { faces: ["left", "right"] }],
        faces: ["left", "right"],
        container: "database-container",
        allowLoopback: false
    });
}

function EditRelation(connection, sourceLabel, targetLabel) {
    connection.removeOverlay("label0");
    connection.removeOverlay("label1");
    connection.addOverlay(["Label", {
        location: 0.1,
        id: "label0",
        cssClass: "relationLabel",
        label: sourceLabel
    }]);
    connection.addOverlay(["Label", {
        location: 0.9,
        id: "label1",
        cssClass: "relationLabel",
        label: targetLabel
    }]);
}

function AddTable(tableName) {
    var tableAllowed = true;
    $("#database-container .dbTable").each(function (tableIndex, tableDiv) {
        if ($(tableDiv).find(".dbTableName").text().toLowerCase() == tableName.toLowerCase()) {
            tableAllowed = false;
            alert("This table name is already used.");
            return false;
        }
       
        var regex = /^[0-9a-zA-Z_]+$/;
        if (!regex.test(tableName)) {
            tableAllowed = false;
            alert("Incorrect table name.");
            return false;
        }    
    });

    if (!tableAllowed) {
        return;
    }

    newTable = $('<div class="dbTable"><div class="dbTableHeader"><div class="deleteTableIcon fa fa-remove"></div><div class="dbTableName">'
        + tableName + '</div><div class="editTableIcon fa fa-pencil"></div><div class="addColumnIcon fa fa-plus"></div></div>'
        + '<div class="dbTableBody"><div class="dbColumn idColumn dbPrimaryKey" dbColumnType="integer">'
        + '<div class="dbColumnName">id</div></div></div>'
        + '<div class="dbTableIndexArea"></div></div>');
    newTable.find(".dbColumn").data("dbColumnType", "integer");
    $("#database-container").append(newTable);
    newTable.find(".editTableIcon").on("click", function () {
        CurrentTable = $(this).parents(".dbTable");
        editTableDialog.dialog("open");
    });
    newTable.find(".deleteTableIcon").on("click", function () {
        $(this).parents(".dbTable").remove();
        instance.removeAllEndpoints($(this).parents(".dbTable"), true);
    });
    newTable.find(".addColumnIcon").on("click", function () {
        addColumnDialog.data("currentTable", $(this).parents(".dbTable"));
        addColumnDialog.dialog("open");
    })
    instance.draggable(newTable);
    AddColumnToJsPlumb(newTable.find(".dbColumn"));
}

function AddColumn(table, columnName, type, isPrimaryKey, allowNull, defaultValue, length, lengthMax, unique, displayName) {
    newColumn = $('<div class="dbColumn"><div class="deleteColumnIcon fa fa-remove"></div><div class="dbColumnName">'
        + columnName + '</div><div class="editColumnIcon fa fa-pencil"></div></div>');

    newColumn.children(".deleteColumnIcon").on("mousedown", function () {
        $(this).parents(".dbColumn").remove();
        instance.removeAllEndpoints($(this).parents(".dbColumn"), true);
        instance.recalculateOffsets();
        instance.repaintEverything();
        return false;
    });
    newColumn.children(".editColumnIcon").on("mousedown", function () {
        CurrentColumn = $(this).parents(".dbColumn");
        editColumnDialog.dialog("open");
        return false;
    });
    table.children(".dbTableBody").append(newColumn);
    if (isPrimaryKey) {
        //table.find(".dbColumn").removeClass("dbPrimaryKey");
        newColumn.addClass("dbPrimaryKey");
    }
    newColumn.data("dbAllowNull", allowNull);
    newColumn.data("dbUnique", unique);
    newColumn.data("dbDefaultValue", defaultValue);
    newColumn.attr("dbColumnType", type);
    newColumn.data("dbColumnLength", length);
    newColumn.data("dbColumnLengthMax", lengthMax);
    newColumn.data("dbColumnDisplayName", displayName);
    AddColumnToJsPlumb(newColumn);
}

function AddIndex(table, name, indexColumnArray, unique) {
    indexLabel = "Index: ";
    for (i = 0; i < indexColumnArray.length - 1; i++)
        indexLabel += indexColumnArray[i] + ", ";
    indexLabel += indexColumnArray[indexColumnArray.length - 1];
    if (unique)
        indexLabel += " - unique";
    newIndex = $('<div class="dbIndex"><div class="deleteIndexIcon fa fa-remove"></div><div class="dbIndexText">' + indexLabel + '</div><div class="editIndexIcon fa fa-pencil"></div></div>');
    newIndex.data("indexName", name);
    filteredIndexColumnArray = [];
    for (i = 0; i < indexColumnArray.length; i++) {
        if (indexColumnArray[i] != "-none-")
            filteredIndexColumnArray.push(indexColumnArray[i]);
    }
    newIndex.data("indexColumnArray", filteredIndexColumnArray);
    newIndex.data("indexColumnArray", indexColumnArray);
    newIndex.data("unique", unique);
    newIndex.children(".deleteIndexIcon").on("mousedown", function () {
        $(this).parents(".dbIndex").remove();
        return false;
    });
    newIndex.children(".editIndexIcon").on("mousedown", function () {
        CurrentIndex = $(this).parents(".dbIndex");
        CurrentTable = $(this).parents(".dbTable");
        editIndexDialog.dialog("open");
        return false;
    });
    table.children(".dbTableIndexArea").append(newIndex);
}

function AddView(viewName, viewQuery) {
    newView = $('<div class="dbView" style="top: 100px; left: 20px;"><div class="dbViewHeader"><div class="deleteViewIcon fa fa-remove"></div>'
    + '<div class="dbViewName">View: ' + viewName + '</div><div class="editViewIcon fa fa-pencil"></div></div></div>');

    $("#database-container").append(newView);
    newView.find(".editViewIcon").on("click", function () {
        CurrentView = $(this).parents(".dbView");
        editViewDialog.dialog("open");
    });
    newView.find(".deleteViewIcon").on("click", function () {
        $(this).parents(".dbView").remove();
    });
    newView.data("dbViewName", viewName);
    newView.data("dbViewQuery", viewQuery);
    instance.draggable(newView);
}

function CheckColumnLengthSupport(dialog, typeCode) {
    // Check if data type supports column length
    currentType = SqlServerDataTypes.filter(function (val) {
        return val[0] == typeCode;
    });
    if (!typeCode) {
        dialog.find("#columnLengthNotSupported").show();
        dialog.find("#column-length").hide();
        dialog.find("#column-length-max").hide();
        dialog.find("label[for=column-length-max]").hide();
    } else if (currentType[0][2]) {
        dialog.find("#columnLengthNotSupported").hide();
        dialog.find("#column-length").show();
        dialog.find("#column-length-max").show();
        dialog.find("label[for=column-length-max]").show();
    } else {
        dialog.find("#columnLengthNotSupported").show();
        dialog.find("#column-length").hide();
        dialog.find("#column-length-max").hide();
        dialog.find("label[for=column-length-max]").hide();
    }
}

// Format: 1-code, 2-label, 3-supports column length?
SqlServerDataTypes = [
    ["varchar", "Varchar", true],
    ["boolean", "Boolean", false],
    ["integer", "Integer", false],
    ["float", "Float", false],
    ["decimal", "Decimal", false],
    ["datetime", "DateTime", false],
    ["blob", "Blob", true],
];

function SaveDbScheme(commitMessage) {
    pageSpinner.show();
    columnIdCounter = 0;
    tableArray = [];
    relationArray = [];
    viewArray = [];
    $("#database-container .dbTable").each(function (tableIndex, tableDiv) {
        columnArray = [];
        indexArray = [];
        $(tableDiv).data("dbTableId", tableIndex);
        $(tableDiv).find(".dbColumn").each(function (columnIndex, columnDiv) {
            columnArray.push({
                Id: columnIdCounter,
                Name: $(columnDiv).find(".dbColumnName").text(),
                DisplayName: $(columnDiv).data("dbColumnDisplayName"),
                Type: $(columnDiv).attr("dbColumnType"),
                PrimaryKey: $(columnDiv).hasClass("dbPrimaryKey"),
                Unique: $(columnDiv).data("dbUnique"),
                AllowNull: $(columnDiv).data("dbAllowNull"),
                DefaultValue: $(columnDiv).data("dbDefaultValue"),
                ColumnLength: $(columnDiv).data("dbColumnLength"),
                ColumnLengthIsMax: $(columnDiv).data("dbColumnLengthMax")
            });
            $(columnDiv).data("dbColumnId", columnIdCounter);
            columnIdCounter++;
        });
        $(tableDiv).find(".dbIndex").each(function (indexIndex, indexDiv) {
            originalIndexColumnArray = $(indexDiv).data("indexColumnArray");
            filteredIndexColumnArray = [];
            for (i = 0; i < originalIndexColumnArray.length; i++) {
                if (originalIndexColumnArray[i] != "-none-")
                    filteredIndexColumnArray.push(originalIndexColumnArray[i]);
            }
            indexArray.push({
                Id: indexIndex,
                Name: $(indexDiv).data("indexName"),
                ColumnNames: filteredIndexColumnArray,
                Unique: $(indexDiv).data("unique")
            });
        });
        tableArray.push({
            Id: tableIndex,
            Name: $(tableDiv).find(".dbTableName").text(),
            PositionX: parseInt($(tableDiv).css("left")),
            PositionY: parseInt($(tableDiv).css("top")),
            Columns: columnArray,
            Indices: indexArray
        });
    });
    jsPlumbConnections = instance.getAllConnections();

    for (i = 0; i < jsPlumbConnections.length; i++) {
        currentConnection = jsPlumbConnections[i];
        sourceDiv = $(currentConnection.source);
        targetDiv = $(currentConnection.target);
        relationArray.push({
            LeftTable: sourceDiv.parents(".dbTable").data("dbTableId"),
            rightTable: targetDiv.parents(".dbTable").data("dbTableId"),
            LeftColumn: sourceDiv.data("dbColumnId"),
            RightColumn: targetDiv.data("dbColumnId"),
            Type: $(currentConnection).data("relationType")
        });
    }
    $("#database-container .dbView").each(function (viewIndex, viewDiv) {
        viewArray.push({
            Id: viewIndex,
            Name: $(viewDiv).data("dbViewName"),
            Query: $(viewDiv).data("dbViewQuery"),
            PositionX: parseInt($(viewDiv).css("left")),
            PositionY: parseInt($(viewDiv).css("top"))
        });
    });
    postData = {
        CommitMessage: commitMessage,
        Tables: tableArray,
        Relations: relationArray,
        Views: viewArray
    }
    appId = $("#currentAppId").val();
    $.ajax({
        type: "POST",
        url: "/api/database/apps/" + appId + "/commits",
        data: postData,
        complete: function () {
            pageSpinner.hide();
        },
        success: function () {
            alert("The database scheme has been successfully saved!");
            $('#btnLockScheme').html('Lock scheme');
            $.ajax({
                type: "GET",
                url: "/api/database/apps/" + appId + "/getLastCommitId",
                dataType: "json",
                complete: function () {
                },
                success: function (data) {
                    DD.lock.CurrentSchemeCommitId = data;


                }
            });
        }
    });

  
}

function LoadDbScheme(commitId) {
    pageSpinner.show();
    appId = $("#currentAppId").val();
    currentUserId = $("#currentUserId").val();

    $.ajax({
        type: "GET",
        url: "/api/database/apps/" + appId + "/commits/" + commitId,
        dataType: "json",
        complete: function () {
            pageSpinner.hide()
        },
        success: function (data) {
          
            ClearDbScheme();
            for (i = 0; i < data.Tables.length; i++) {
                newTable = $('<div class="dbTable"><div class="dbTableHeader"><div class="deleteTableIcon fa fa-remove"></div><div class="dbTableName">'
                    + data.Tables[i].Name + '</div><div class="editTableIcon fa fa-pencil"></div><div class="addColumnIcon fa fa-plus"></div></div>'
                    + '<div class="dbTableBody"><div class="dbColumn idColumn dbPrimaryKey" dbColumnType="integer" dbColumnId="'
                    + data.Tables[i].Columns[0].Id + '"><div class="dbColumnName">id</div></div></div>'
                    + '<div class="dbTableIndexArea"></div></div>');
                $("#database-container").append(newTable);
                $(".editTableIcon").on("click", function () {
                    CurrentTable = $(this).parents(".dbTable");
                    editTableDialog.dialog("open");
                });
                newTable.find(".deleteTableIcon").on("click", function () {
                    $(this).parents(".dbTable").remove();
                    instance.removeAllEndpoints($(this).parents(".dbTable"), true);
                });
                newTable.find(".addColumnIcon").on("click", function () {
                    addColumnDialog.data("currentTable", $(this).parents(".dbTable"));
                    addColumnDialog.dialog("open");
                })
                newTable.find(".deleteColumnIcon").on("mousedown", function () {
                    $(this).parents(".dbColumn").remove();
                    instance.removeAllEndpoints($(this).parents(".dbColumn"), true);
                    return false;
                });
                newTable.find(".editColumnIcon").on("mousedown", function () {
                    CurrentColumn = $(this).parents(".dbColumn");
                    editColumnDialog.dialog("open");
                    return false;
                });
                newTable.css("left", data.Tables[i].PositionX);
                newTable.css("top", data.Tables[i].PositionY);
                instance.draggable(newTable);
                for (j = 1; j < data.Tables[i].Columns.length; j++) {
                    if (data.Tables[i].Columns[j].DefaultValue != null)
                        defaultValue = data.Tables[i].Columns[j].DefaultValue;
                    else
                        defaultValue = "";
                    newColumn = $('<div class="dbColumn"><div class="deleteColumnIcon fa fa-remove"></div><div class="dbColumnName">'
                        + data.Tables[i].Columns[j].Name + '</div><div class="editColumnIcon fa fa-pencil"></div></div>');
                    newColumn.attr("dbColumnType", data.Tables[i].Columns[j].Type);
                    newColumn.attr("dbColumnId", data.Tables[i].Columns[j].Id);
                    newColumn.data("dbUnique", data.Tables[i].Columns[j].Unique);
                    newColumn.data("dbAllowNull", data.Tables[i].Columns[j].AllowNull);
                    newColumn.data("dbDefaultValue", defaultValue);
                    newColumn.data("dbColumnLength", data.Tables[i].Columns[j].ColumnLength);
                    newColumn.data("dbColumnLengthMax", data.Tables[i].Columns[j].ColumnLengthIsMax);
                    newColumn.data("dbColumnDisplayName", data.Tables[i].Columns[j].DisplayName);

                    newColumn.children(".deleteColumnIcon").on("mousedown", function () {
                        $(this).parents(".dbColumn").remove();
                        instance.removeAllEndpoints($(this).parents(".dbColumn"), true);
                        instance.recalculateOffsets();
                        instance.repaintEverything();
                        return false;
                    });
                    newColumn.children(".editColumnIcon").on("mousedown", function () {
                        CurrentColumn = $(this).parents(".dbColumn");
                        editColumnDialog.dialog("open");
                        return false;
                    });
                    newTable.children(".dbTableBody").append(newColumn);
                    if (data.Tables[i].Columns[j].PrimaryKey) {
                        newColumn.addClass("dbPrimaryKey");
                    }
                    newColumn.attr("dbColumnType", data.Tables[i].Columns[j].Type);
                }
                AddColumnToJsPlumb(newTable.find(".dbColumn"));
                for (j = 0; j < data.Tables[i].Indices.length; j++) {
                    indexLabel = "Index: ";
                    for (k = 0; k < data.Tables[i].Indices[j].ColumnNames.length - 1; k++)
                        indexLabel += data.Tables[i].Indices[j].ColumnNames[k] + ", ";
                    indexLabel += data.Tables[i].Indices[j].ColumnNames[data.Tables[i].Indices[j].ColumnNames.length - 1];
                    if (data.Tables[i].Indices[j].Unique)
                        indexLabel += " - unique";
                    newIndex = $('<div class="dbIndex"><div class="deleteIndexIcon fa fa-remove"></div><div class="dbIndexText">' + indexLabel + '</div><div class="editIndexIcon fa fa-pencil"></div></div>');
                    newIndex.data("indexName", data.Tables[i].Indices[j].Name);
                    newIndex.data("indexColumnArray", data.Tables[i].Indices[j].ColumnNames);
                    newIndex.data("unique", data.Tables[i].Indices[j].Unique);
                    newIndex.children(".deleteIndexIcon").on("mousedown", function () {
                        $(this).parents(".dbIndex").remove();
                        return false;
                    });
                    newIndex.children(".editIndexIcon").on("mousedown", function () {
                        CurrentIndex = $(this).parents(".dbIndex");
                        CurrentTable = $(this).parents(".dbTable");
                        editIndexDialog.dialog("open");
                        return false;
                    });
                    newTable.children(".dbTableIndexArea").append(newIndex);
                }
            }
            for (i = 0; i < data.Relations.length; i++) {
                sourceDiv = $("#database-container .dbColumn[dbColumnId='" + data.Relations[i].LeftColumn + "']");
                targetDiv = $("#database-container .dbColumn[dbColumnId='" + data.Relations[i].RightColumn + "']");
                newConnection = instance.connect({ source: sourceDiv.attr("id"), target: targetDiv.attr("id"), editable: true });
                $(newConnection).data("relationType", data.Relations[i].Type);
                switch (data.Relations[i].Type) {
                    case 2:
                        EditRelation(newConnection, "1", "N");
                        break;
                    case 3:
                        EditRelation(newConnection, "N", "1");
                        break;
                    case 4:
                        EditRelation(newConnection, "M", "N");
                        break;
                }
            }
            for (i = 0; i < data.Views.length; i++) {
                newView = $('<div class="dbView"><div class="dbViewHeader"><div class="deleteViewIcon fa fa-remove"></div>'
                    + '<div class="dbViewName">View: ' + data.Views[i].Name + '</div><div class="editViewIcon fa fa-pencil"></div></div></div>');

                $("#database-container").append(newView);
                newView.find(".editViewIcon").on("click", function () {
                    CurrentView = $(this).parents(".dbView");
                    editViewDialog.dialog("open");
                });
                newView.find(".deleteViewIcon").on("click", function () {
                    $(this).parents(".dbView").remove();
                });
                newView.css("left", data.Views[i].PositionX);
                newView.css("top", data.Views[i].PositionY);
                newView.data("dbViewName", data.Views[i].Name);
                newView.data("dbViewQuery", data.Views[i].Query);
                instance.draggable(newView);
            }
        }
    });
}

var ZoomFactor = 1.0;
$(function () {
    if (CurrentModuleIs("dbDesignerModule")) {
        $("#btnAddTable").on("click", function () {
            addTableDialog.dialog("open");
        });
        $("#btnAddView").on("click", function () {
            addViewDialog.dialog("open");
        });
        $("#btnLockScheme").on("click", function () {
            console.log(DD);
            DD.lock._lockSchemeClick();
        });

        $("#switchToWorkflow").on("click", function () {
            window.location = "/workflow";
        });
        $("#btnSaveScheme").on("click", function () {
            DD.lock._save();
        });
        $("#btnLoadScheme").on("click", function () {
            LoadDbScheme("latest");
        });
        $("#btnOpenHistory").on("click", function () {
            historyDialog.dialog("open");
        });
        $("#btnClearScheme").on("click", function () {
            ClearDbScheme();
        });
        $("#btnZoomIn").on("click", function () {
            ZoomFactor += 0.1;
            $(".database-container").css("transform", "scale(" + ZoomFactor + ")");
            $("#zoomLabel").text("Zoom " + Math.floor(ZoomFactor * 100) + "%");
            instance.repaintEverything();
        });
        $("#btnZoomOut").on("click", function () {
            if (ZoomFactor >= 0.2)
                ZoomFactor -= 0.1;
            $(".database-container").css("transform", "scale(" + ZoomFactor + ")");
            $("#zoomLabel").text("Zoom " + Math.floor(ZoomFactor * 100) + "%");
            instance.repaintEverything();
        });

        LoadDbScheme("latest");
    }
});

var CurrentTable, CurrentColumn, CurrentConnection, CurrentView, CurrentIndex;

$(function () {
    if (CurrentModuleIs("dbDesignerModule")) {

        var cmList = {};
        var cmConfig = {
            lineNumbers: true,
            lineWrapping: true,
            mode: "sql",
            extraKeys: { "Ctrl-Space": "autocomplete" }
        };

        addTableDialog = $("#add-table-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 150,
            buttons: {
                "Add": function () {
                    addTableDialog_SubmitData();
                },
                Cancel: function () {
                    addTableDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        addTableDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                addTableDialog.find("#new-table-name").val("");
            }
        });
        function addTableDialog_SubmitData() {
            AddTable(addTableDialog.find("#new-table-name").val());
            addTableDialog.dialog("close");
        }

        editTableDialog = $("#edit-table-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 170,
            buttons: {
                "Save": function () {
                    editTableDialog_SubmitData();
                },
                Cancel: function () {
                    editTableDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        editTableDialog_SubmitData();
                        return false;
                    }
                });
                $(this).find("#add-index-button").on("click", function () {
                    addIndexDialog.dialog("open");
                });
            },
            open: function () {
                editTableDialog.find("#table-name").val(CurrentTable.find(".dbTableName").text());
            }
        });
        function editTableDialog_SubmitData() {
            CurrentTable.find(".dbTableName").text(editTableDialog.find("#table-name").val());
            editTableDialog.dialog("close");
        }

        addColumnDialog = $("#add-column-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 330,
            buttons: {
                "Add": function () {
                    addColumnDialog_SubmitData();
                },
                Cancel: function () {
                    addColumnDialog.dialog("close");
                }
            },
            create: function () {
                for (i = 0; i < SqlServerDataTypes.length; i++) {
                    $("#add-column-dialog #column-type-dropdown").append(
                        $('<option value="' + SqlServerDataTypes[i][0] + '">' + SqlServerDataTypes[i][1] + '</option>'));
                }
                $("#add-column-dialog #column-type-dropdown").change(function () {
                    CheckColumnLengthSupport(addColumnDialog, this.value);
                    if (addColumnDialog.find("#column-length-max").is(":checked")) {
                        addColumnDialog.find("#column-length").hide();
                    }
                });
                $("#add-column-dialog #column-length-max").change(function () {
                    if ($(this).is(":checked")) {
                        addColumnDialog.find("#column-length").hide();
                    } else {
                        addColumnDialog.find("#column-length").show();
                    }
                });
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        addColumnDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                addColumnDialog.find("#column-name").val("");
                addColumnDialog.find("#column-display-name").val("");
                addColumnDialog.find("#primary-key-checkbox").prop("checked", false);
                addColumnDialog.find("#unique-checkbox").prop("checked", false);
                addColumnDialog.find("#allow-null-checkbox").prop("checked", false);
                addColumnDialog.find("#column-type-dropdown").val("varchar");
                addColumnDialog.find("#default-value").val("");
                addColumnDialog.find("#column-length").val(100);
                addColumnDialog.find("#column-length-max").prop("checked", true);
                addColumnDialog.find("#columnLengthNotSupported").hide();
                CheckColumnLengthSupport(addColumnDialog, "varchar");
                addColumnDialog.find("#column-length").hide();
            }
        });
        function addColumnDialog_SubmitData() {
            AddColumn(addColumnDialog.data("currentTable"),
                addColumnDialog.find("#column-name").val(),
                addColumnDialog.find("#column-type-dropdown").val(),
                addColumnDialog.find("#primary-key-checkbox").prop("checked"),
                addColumnDialog.find("#allow-null-checkbox").prop("checked"),
                addColumnDialog.find("#default-value").val(),
                addColumnDialog.find("#column-length").val(),
                addColumnDialog.find("#column-length-max").prop("checked"),
                addColumnDialog.find("#unique-checkbox").prop("checked"),
                addColumnDialog.find("#column-display-name").val());
            addColumnDialog.dialog("close");
        }

        editColumnDialog = $("#edit-column-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 330,
            buttons: {
                "Save": function () {
                    editColumnDialog_SubmitData();
                },
                Cancel: function () {
                    editColumnDialog.dialog("close");
                }
            },
            create: function () {
                for (i = 0; i < SqlServerDataTypes.length; i++) {
                    $("#edit-column-dialog #column-type-dropdown").append(
                        $('<option value="' + SqlServerDataTypes[i][0] + '">' + SqlServerDataTypes[i][1] + '</option>'));
                }
                $("#edit-column-dialog #column-type-dropdown").change(function () {
                    CheckColumnLengthSupport(editColumnDialog, this.value);
                    if (editColumnDialog.find("#column-length-max").is(":checked")) {
                        editColumnDialog.find("#column-length").hide();
                    }
                });
                $("#edit-column-dialog #column-length-max").change(function () {
                    if ($(this).is(":checked")) {
                        editColumnDialog.find("#column-length").hide();
                    } else {
                        editColumnDialog.find("#column-length").show();
                    }
                });
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        editColumnDialog_SubmitData();
                        return false;
                    }
                });
            },
            open: function () {
                editColumnDialog.find("#column-name").val(CurrentColumn.find(".dbColumnName").text());
                editColumnDialog.find("#column-display-name").val(CurrentColumn.data("dbColumnDisplayName"));
                editColumnDialog.find("#primary-key-checkbox").prop("checked", CurrentColumn.hasClass("dbPrimaryKey"));
                editColumnDialog.find("#unique-checkbox").prop("checked", CurrentColumn.data("dbUnique"));
                editColumnDialog.find("#allow-null-checkbox").prop("checked", CurrentColumn.data("dbAllowNull"));
                editColumnDialog.find("#column-type-dropdown").val(CurrentColumn.attr("dbColumnType"));
                editColumnDialog.find("#default-value").val(CurrentColumn.data("dbDefaultValue"));
                editColumnDialog.find("#column-length").val(CurrentColumn.data("dbColumnLength"));
                editColumnDialog.find("#column-length-max").prop("checked", CurrentColumn.data("dbColumnLengthMax"));
                CheckColumnLengthSupport(editColumnDialog, CurrentColumn.attr("dbColumnType"));
                if (CurrentColumn.data("dbColumnLengthMax"))
                    editColumnDialog.find("#column-length").hide();
            }
        });
        function editColumnDialog_SubmitData() {
            CurrentColumn.find(".dbColumnName").text(editColumnDialog.find("#column-name").val());
            CurrentColumn.attr("dbColumnType", editColumnDialog.find("#column-type-dropdown").val());
            CurrentColumn.data("dbUnique", editColumnDialog.find("#unique-checkbox").prop("checked"));
            CurrentColumn.data("dbAllowNull", editColumnDialog.find("#allow-null-checkbox").prop("checked"));
            CurrentColumn.data("dbDefaultValue", editColumnDialog.find("#default-value").val());
            CurrentColumn.data("dbColumnLength", editColumnDialog.find("#column-length").val());
            CurrentColumn.data("dbColumnLengthMax", editColumnDialog.find("#column-length-max").prop("checked"));
            CurrentColumn.data("dbColumnDisplayName", editColumnDialog.find("#column-display-name").val());
            if (CurrentColumn.hasClass("dbPrimaryKey") && !editColumnDialog.find("#primary-key-checkbox").prop("checked"))
                CurrentColumn.removeClass("dbPrimaryKey");
            else if (!CurrentColumn.hasClass("dbPrimaryKey") && editColumnDialog.find("#primary-key-checkbox").prop("checked")) {
                //CurrentColumn.parents(".dbTable").find(".dbColumn").removeClass("dbPrimaryKey"); // Uncomment this line to allow only one primary key per table
                CurrentColumn.addClass("dbPrimaryKey");
            }
            editColumnDialog.dialog("close");
        }

        editRelationDialog = $("#edit-relation-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 250,
            buttons: {
                "Save": function () {
                    editRelationDialog_SubmitData()
                },
                Cancel: function () {
                    editRelationDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        editRelationDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                if ($(CurrentConnection).data("relationType"))
                    editRelationDialog.find("input:radio[value=" + $(CurrentConnection).data("relationType") + "]").prop("checked", "checked");
                else
                    editRelationDialog.find("input:radio[value=1]").prop("checked", "checked");
            }
        });
        function editRelationDialog_SubmitData() {
            $(CurrentConnection).data("relationType", editRelationDialog.find("input[type='radio']:checked").val());
            switch (editRelationDialog.find("input[type='radio']:checked").val()) {
                case "1":
                    EditRelation(CurrentConnection, "1", "1");
                    break;
                case "2":
                    EditRelation(CurrentConnection, "1", "N");
                    break;
                case "3":
                    EditRelation(CurrentConnection, "N", "1");
                    break;
                case "4":
                    EditRelation(CurrentConnection, "M", "N");
                    break;
                case "Delete":
                    instance.detach(CurrentConnection);
                    break;
            }
            editRelationDialog.dialog("close");
        }

        historyDialog = $("#history-dialog").dialog({
            autoOpen: false,
            width: 700,
            height: 540,
            buttons: {
                "Load": function () {
                    historyDialog_SubmitData();
                },
                Cancel: function () {
                    historyDialog.dialog("close");
                }
            },
            open: function (event, ui) {
                historyDialog.data("selectedCommitId", null);
                appId = $("#currentAppId").val();
                $.ajax({
                    type: "GET",
                    url: "/api/database/apps/" + appId + "/commits",
                    dataType: "json",
                    error: function (request, status, error) {
                        alert(request.responseText);
                    },
                    success: function (data) {
                        historyDialog.find("#commit-table:first tbody:nth-child(2) tr").remove();
                        tbody = historyDialog.find("#commit-table tbody:nth-child(2)");
                        commitIdArray = [];

                        // Fill in the history rows
                        for (i = 0; i < data.length; i++) {
                            commitIdArray.push(data[i].Id);
                            if (data[i].CommitMessage != null)
                                tbody.append($('<tr class="commitRow"><td>' + data[i].TimeString
                                    + '</td><td>' + data[i].CommitMessage + '</td></tr>'));
                            else
                                tbody.append($('<tr class="commitRow"><td>' + data[i].TimeString
                                    + '</td><td style="color: darkgrey;">(no message)</td></tr>'));
                        }

                        // Highlight the selected row
                        $(document).on('click', 'tr.commitRow', function (event) {
                            historyDialog.find("#commit-table tbody:nth-child(2) tr").removeClass("highlightedCommitRow");
                            $(this).addClass("highlightedCommitRow");
                            var rowIndex = $(this).index();
                            historyDialog.data("selectedCommitId", commitIdArray[rowIndex]);
                        });
                    }
                });
            }
        });
        function historyDialog_SubmitData() {
            if (historyDialog.data("selectedCommitId")) {
                LoadDbScheme(historyDialog.data("selectedCommitId"));
                historyDialog.dialog("close");
            }
            else
                alert("Please select a commit");
        }
        saveDialog = $("#save-dialog").dialog({
            autoOpen: false,
            width: 400,
            height: 190,
            buttons: {
                "Save": function () {
                    saveDialog_SubmitData();
                },
                Cancel: function () {
                    saveDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        saveDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                saveDialog.find("#message").val("");
            }
        });
        function saveDialog_SubmitData() {
            saveDialog.dialog("close");
            SaveDbScheme(saveDialog.find("#message").val());
        }

        addViewDialog = $("#add-view-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: '75%',
            //height: 600,
            buttons: {
                "Add": function () {
                    addViewDialog_SubmitData();
                },
                Cancel: function () {
                    addViewDialog.dialog("close");
                }
            },
            create: function () {
                $(this).find("#new-view-name").keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        addViewDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                addViewDialog.find("#new-view-name").val("");
                addViewDialog.find("#new-view-query").val("");

                if (typeof cmList['new-view-query'] == 'undefined') {
                    cmList['new-view-query'] = CodeMirror.fromTextArea(document.getElementById('new-view-query'), cmConfig);
                    cmList['new-view-query'].on('blur', function (i) {
                        i.save();
                    });
                }
                else {
                    cmList['new-view-query'].setValue('');
                }
            }
        });
        function addViewDialog_SubmitData() {
            AddView(addViewDialog.find("#new-view-name").val(),
                addViewDialog.find("#new-view-query").val());
            addViewDialog.dialog("close");
        }

        editViewDialog = $("#edit-view-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: '75%',
            //height: 600,
            buttons: {
                "Save": function () {
                    editViewDialog_SubmitData();
                },
                Cancel: function () {
                    editViewDialog.dialog("close");
                }
            },
            create: function () {
                $(this).find("#view-name").keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        editViewDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                editViewDialog.find("#view-name").val(CurrentView.data("dbViewName"));
                editViewDialog.find("#view-query").val(CurrentView.data("dbViewQuery"));

                if (typeof cmList['view-query'] == 'undefined') {
                    cmList['view-query'] = CodeMirror.fromTextArea(document.getElementById('view-query'), cmConfig);
                    cmList['view-query'].on('blur', function (i) {
                        i.save();
                    });
                }
                else {
                    cmList['view-query'].setValue(CurrentView.data("dbViewQuery"));
                }
            }
        });
        function editViewDialog_SubmitData() {
            CurrentView.find(".dbViewName").text("View: " + editViewDialog.find("#view-name").val());
            CurrentView.data("dbViewName", editViewDialog.find("#view-name").val());
            CurrentView.data("dbViewQuery", editViewDialog.find("#view-query").val());
            editViewDialog.dialog("close");
        }

        addIndexDialog = $("#add-index-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 260,
            buttons: {
                "Add": function () {
                    addIndexDialog_SubmitData();
                },
                Cancel: function () {
                    addIndexDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        addIndexDialog_SubmitData();
                        return false;
                    }
                });
                $("#add-index-dialog #btn-add-index-column").on("click", function () {
                    newColumnNumber = addIndexDialog.data("columnsShown") + 1;
                    newFormRow = $('<tr class="additionalFormRow"><td><label for="additional-column">' + newColumnNumber + '. column</label></td>'
                        + '<td><select name="additional-column" class="additionalColumn"></select></td></tr>');
                    newFormRow.find(".additionalColumn").append($('<option value="-none-">-none-</option>'));
                    CurrentTable.find(".dbColumn").each(function (i, val) {
                        newFormRow.find(".additionalColumn").append(
                            $('<option value="' + $(val).find(".dbColumnName").text() + '">' + $(val).find(".dbColumnName").text() + '</option>'));
                    });
                    $("#add-index-dialog").find("#addIndexColumnFormRow").before(newFormRow);
                    addIndexDialog.data("columnsShown", newColumnNumber);
                });
            },
            open: function () {
                addIndexDialog.find("#first-column option").remove();
                CurrentTable.find(".dbColumn").each(function (i, val) {
                    addIndexDialog.find("#first-column").append(
                        $('<option value="' + $(val).find(".dbColumnName").text() + '">' + $(val).find(".dbColumnName").text() + '</option>'));
                });
                addIndexDialog.find("#second-column option").remove();
                addIndexDialog.find("#second-column").append(
                        $('<option value="-none-">-none-</option>'));
                CurrentTable.find(".dbColumn").each(function (i, val) {
                    addIndexDialog.find("#second-column").append(
                        $('<option value="' + $(val).find(".dbColumnName").text() + '">' + $(val).find(".dbColumnName").text() + '</option>'));
                });
                addIndexDialog.find("#index-name").val("");
                addIndexDialog.find("#first-column").val("id");
                addIndexDialog.find("#second-column").val("-none-");
                addIndexDialog.find("#unique-checkbox").prop("checked", false);
                addIndexDialog.find(".additionalFormRow").remove();
                addIndexDialog.data("columnsShown", 1);
            }
        });
        function addIndexDialog_SubmitData() {
            indexColumnArray = [
                addIndexDialog.find("#first-column").val()
            ];
            addIndexDialog.find(".additionalFormRow .additionalColumn").each(function (i, element) {
                indexColumnArray.push($(element).val());
            });
            filteredIndexColumnArray = [];
            for (i = 0; i < indexColumnArray.length; i++) {
                if (indexColumnArray[i] != "-none-")
                    filteredIndexColumnArray.push(indexColumnArray[i]);
            }
            AddIndex(CurrentTable,
                addIndexDialog.find("#index-name").val(),
                filteredIndexColumnArray,
                addIndexDialog.find("#unique-checkbox").prop("checked")
                );
            addIndexDialog.dialog("close");
        }

        editIndexDialog = $("#edit-index-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 230,
            buttons: {
                "Save": function () {
                    editIndexDialog_SubmitData();
                },
                Cancel: function () {
                    editIndexDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        editIndexDialog_SubmitData();
                        return false;
                    }
                });
                $("#edit-index-dialog #btn-add-index-column").on("click", function () {
                    newColumnNumber = editIndexDialog.data("columnsShown") + 1;
                    newFormRow = $('<tr class="additionalFormRow"><td><label for="additional-column">' + newColumnNumber + '. column</label></td>'
                        + '<td><select name="additional-column" class="additionalColumn"></select></td></tr>');
                    newFormRow.find(".additionalColumn").append($('<option value="-none-">-none-</option>'));
                    CurrentTable.find(".dbColumn").each(function (i, val) {
                        newFormRow.find(".additionalColumn").append(
                            $('<option value="' + $(val).find(".dbColumnName").text() + '">' + $(val).find(".dbColumnName").text() + '</option>'));
                    });
                    $("#edit-index-dialog").find("#addIndexColumnFormRow").before(newFormRow);
                    editIndexDialog.data("columnsShown", newColumnNumber);
                });
            },
            open: function () {
                editIndexDialog.find("#first-column option").remove();
                CurrentTable.find(".dbColumn").each(function (i, val) {
                    editIndexDialog.find("#first-column").append(
                        $('<option value="' + $(val).find(".dbColumnName").text() + '">' + $(val).find(".dbColumnName").text() + '</option>'));
                });
                editIndexDialog.find("#second-column option").remove();
                editIndexDialog.find("#second-column").append(
                        $('<option value="-none-">-none-</option>'));
                CurrentTable.find(".dbColumn").each(function (i, val) {
                    editIndexDialog.find("#second-column").append(
                        $('<option value="' + $(val).find(".dbColumnName").text() + '">' + $(val).find(".dbColumnName").text() + '</option>'));
                });
                indexColumnArray = CurrentIndex.data("indexColumnArray");
                if (!indexColumnArray)
                    indexColumnArray = ["id"];
                editIndexDialog.data("columnsShown", indexColumnArray.length);
                editIndexDialog.find("#index-name").val(CurrentIndex.data("indexName"));
                editIndexDialog.find("#first-column").val(indexColumnArray[0]);
                editIndexDialog.find("#second-column").val(indexColumnArray[1]);
                editIndexDialog.find("#unique-checkbox").prop("checked", CurrentIndex.data("unique"));
                editIndexDialog.find(".additionalFormRow").remove();
                for (i = 1; i < indexColumnArray.length; i++) {
                    newFormRow = $('<tr class="additionalFormRow"><td><label for="additional-column">' + (i + 1) + '. column</label></td>'
                        + '<td><select name="additional-column" class="additionalColumn"></select></td></tr>');
                    newFormRow.find(".additionalColumn").append($('<option value="-none-">-none-</option>'));
                    CurrentTable.find(".dbColumn").each(function (i, val) {
                        newFormRow.find(".additionalColumn").append(
                            $('<option value="' + $(val).find(".dbColumnName").text() + '">' + $(val).find(".dbColumnName").text() + '</option>'));
                    });
                    newFormRow.find(".additionalColumn").val(indexColumnArray[i]);
                    $("#edit-index-dialog").find("#addIndexColumnFormRow").before(newFormRow);
                };
            }
        });
        function editIndexDialog_SubmitData() {
            indexColumnArray = [
                editIndexDialog.find("#first-column").val()
            ];
            editIndexDialog.find(".additionalFormRow .additionalColumn").each(function (i, element) {
                indexColumnArray.push($(element).val());
            });
            filteredIndexColumnArray = [];
            for (i = 0; i < indexColumnArray.length; i++) {
                if (indexColumnArray[i] != "-none-")
                    filteredIndexColumnArray.push(indexColumnArray[i]);
            }
            indexLabel = "Index: ";
            for (i = 0; i < filteredIndexColumnArray.length - 1; i++)
                indexLabel += filteredIndexColumnArray[i] + ", ";
            indexLabel += filteredIndexColumnArray[filteredIndexColumnArray.length - 1];
            if (editIndexDialog.find("#unique-checkbox").prop("checked"))
                indexLabel += " - unique";

            newIndex = $('<div class="dbIndex"><div class="deleteIndexIcon fa fa-remove"></div><span class="dbIndexText">'
                + indexLabel + '</span><div class="editIndexIcon fa fa-pencil"></div></div>');
            newIndex.children(".deleteIndexIcon").on("click", function () {
                $(this).parents(".dbIndex").remove();
            });
            newIndex.children(".editIndexIcon").on("click", function () {
                CurrentIndex = $(this).parents(".dbIndex");
                CurrentTable = $(this).parents(".dbTable");
                editIndexDialog.dialog("open");
            });
            newIndex.data("indexName", editIndexDialog.find("#index-name").val());
            newIndex.data("indexColumnArray", filteredIndexColumnArray);
            newIndex.data("unique", editIndexDialog.find("#unique-checkbox").prop("checked"));
            CurrentIndex.replaceWith(newIndex);
            editIndexDialog.dialog("close");
        }
    }
});

var CurrentAppId;
var CurrentExportUrl;

$(function () {
    appPropertiesDialog = $("#app-properties-dialog").dialog({
        autoOpen: false,
        resizable: false,
        width: 600,
        height: 320,
        buttons: {
            "Uložit": function () {
                appPropertiesDialog_SubmitData();
            },
            "Zrušit": function () {
                appPropertiesDialog.dialog("close");
            }
        },
        create: function () {
            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    appPropertiesDialog_SubmitData();
                    return false;
                }
            });
        },
        open: function () {
            appPropertiesDialog.find("#app-name").val("");
            appPropertiesDialog.find("#cbAllowAll").val("");
            appPropertiesDialog.find("#cbAllowGuests").val("");
            appPropertiesDialog.find("#template").val(1);
            appPropertiesDialog.find("#tile-width").val(2);
            appPropertiesDialog.find("#tile-height").val(1);
            appPropertiesDialog.find("#bg-color").val(0);
            appPropertiesDialog.find("#icon-class").val("");
            $.ajax({
                type: "GET",
                url: "/api/master/apps/" + CurrentAppId + "/properties",
                dataType: "json",
                success: function (data) {
                    if (data.IsAllowedForAll) { //set if application is allowed for all users.
                        $("#cbAllowAll").prop("checked",true);
                    } else {
                        $("#cbAllowAll").prop("checked", false);

                    }
                    if (data.IsAllowedGuests) { //set if application is allowed for guests.
                        $("#cbAllowGuests").prop("checked", true);
                    } else {
                        $("#cbAllowGuests").prop("checked", false);

                    }
                    appPropertiesDialog.find("#app-name").val(data.DisplayName);
                    appPropertiesDialog.find("#template").val(data.CSSTemplateId);
                    appPropertiesDialog.find("#tile-width").val(data.TileWidth);
                    appPropertiesDialog.find("#tile-height").val(data.TileHeight);
                    appPropertiesDialog.find("#bg-color").val(data.Color);
                    appPropertiesDialog.find("#icon-class").val(data.Icon);
                }
            });
        }
    });
    function appPropertiesDialog_SubmitData() {

        appPropertiesDialog.dialog("close");
        postData = {
            DisplayName: appPropertiesDialog.find("#app-name").val(),
            CSSTemplateId: appPropertiesDialog.find("#template").val(),
            TileWidth: appPropertiesDialog.find("#tile-width").val(),
            TileHeight: appPropertiesDialog.find("#tile-height").val(),
            Color: appPropertiesDialog.find("#bg-color").val(),
            Icon: appPropertiesDialog.find("#icon-class").val(),
            IsAllowedForAll: $("#cbAllowAll").is(":checked") == true ? true : false,
            IsAllowedGuests: $("#cbAllowGuests").is(":checked") == true ? true : false
        }
        $.ajax({
            type: "POST",
            url: "/api/master/apps/" + CurrentAppId + "/properties",
            data: postData,
            success: function () {
                alert("OK");
                // Reload page to change application name in AppManager table
                // after application name was changed
                location.reload();
            }
        });
    }
    addAppDialog = $("#new-app-dialog").dialog({
        autoOpen: false,
        resizable: false,
        width: 600,
        height: 320,
        buttons: {
            "Přidat": function () {
                addAppDialog_SubmitData();
            },
            "Zrušit": function () {
                addAppDialog.dialog("close");
            }
        },
        create: function () {
            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    addAppDialog_SubmitData();
                    return false;
                }
            });
        },
        open: function () {
            addAppDialog.find("#app-name").val("");
            addAppDialog.find("#template").val(1);
            addAppDialog.find("#tile-width").val(2);
            addAppDialog.find("#tile-height").val(1);
            addAppDialog.find("#bg-color").val(0);
            addAppDialog.find("#icon-class").val("fa-question");
        }
    });
    function addAppDialog_SubmitData() {
        addAppDialog.dialog("close");
        postData = {
            DisplayName: addAppDialog.find("#app-name").val(),
            Template: addAppDialog.find("#template").val(),
            TileWidth: addAppDialog.find("#tile-width").val(),
            TileHeight: addAppDialog.find("#tile-height").val(),
            Color: addAppDialog.find("#bg-color").val(),
            Icon: addAppDialog.find("#icon-class").val()
        }
        $.ajax({
            type: "POST",
            url: "/api/master/apps",
            data: postData,
            success: function () {
                location.reload();
            }
        });
    }
    appBuildDialog = $("#app-build-dialog").dialog({
        autoOpen: false,
        resizable: false,
        width: 640,
        height: 320
    });
    exportAppDialog = $("#export-application-dialog").dialog({
        autoOpen: false,
        resizable: false,
        width: 660,
        height: 380,
        buttons: {
            "Exportovat": function () {
                exportAppDialog_SubmitData();
            },
            "Zrušit": function () {
                exportAppDialog.dialog("close");
            }
        },
        create: function () {
            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    exportAppDialog_SubmitData();
                    return false;
                }
            });
        }
    });
    function exportAppDialog_SubmitData() {
        $("#export-application-dialog form").attr('action', CurrentExportUrl).submit();
        exportAppDialog.dialog('close');
    }
});

$(function () {
    if (CurrentModuleIs("adminAppModule")) {
        $(".adminAppTable .btnOpenWorkflow").on("click", function () {
            CurrentAppId = $(this).parents("tr").attr("appId");
            openMetablockForm = $("#openMetablockForm");
            openMetablockForm.find("input[name='appId']").val(CurrentAppId);
            openMetablockForm.submit();
        });
        $(".adminAppTable .btnOpenDbScheme").on("click", function () {
            CurrentAppId = $(this).parents("tr").attr("appId");
            openMetablockForm = $("#openDbSchemeForm");
            openMetablockForm.find("input[name='appId']").val(CurrentAppId);
            openMetablockForm.submit();
        });
        $(".adminAppTable .btnOpenMozaic").on("click", function () {
            CurrentAppId = $(this).parents("tr").attr("appId");
            openMetablockForm = $("#openMozaicForm");
            openMetablockForm.find("input[name='appId']").val(CurrentAppId);
            openMetablockForm.submit();
        });

        var currentWs;
        $(".adminAppTable .actions .btnValidate").on("click", function () {
            CurrentAppId = $(this).parents("tr").attr("appId");

            if (typeof WebSocket === "undefined") {
                ShowAppNotification("Váš prohlížeč nepodporuje webSockety, a nemůže být využit k aktualizaci aplikací", "error");
                return;
            }

            appBuildDialog.dialog("option", { title: "aktualizuji " + $(this).parents("tr").data("displayName") }).empty().dialog("open");
            var messagesById = {};

            var ws = new WebSocket('ws://' + window.location.hostname + ':' + window.location.port + '/Master/AppAdminManager/BuildApp/' + CurrentAppId);
            currentWs = ws;
            //var timeLast = Date.now();
            ws.onerror = function () {
                $(document).trigger("ajaxError", {})
            }
            ws.onmessage = function (event) {
                if (currentWs !== ws) return;

                //console.log(Date.now() - timeLast, event.data);
                //timeLast = Date.now();

                var response;
                try{
                    response = JSON.parse(event.data);
                } catch(e) {
                    response = { message: event.data, type: "error" };
                }

                var $message;
                if (response.id && messagesById[response.id]) {
                    $message = messagesById[response.id];
                } else {
                    var $parent = response.childOf ? messagesById[response.childOf] : appBuildDialog;
                    $message = $("<div class='app-alert'><span>").data("messageId", response.id).appendTo($parent);
                    if (!$parent.is("#app-build-dialog, .app-alert-odd")) $message.addClass("app-alert-odd");
                    if (response.id) messagesById[response.id] = $message;
                }

                if (response.message) $message.children("span").html(response.message);

                $message.removeClass("app-alert-info app-alert-error app-alert-success app-alert-warning").addClass("app-alert-" + (response.type || "info"));

                if (response.abort) $message.nextAll().remove();

                if (response.done) {
                    setTimeout(function () { appBuildDialog.dialog("close") }, 1000);
                }

                var childrenHeight = 0;
                appBuildDialog.children().each(function () {
                    childrenHeight += $(this).outerHeight();
                });
                appBuildDialog.css({ height: childrenHeight + 32 });
            };
        });
        $(".adminAppTable .actions .btnRebuild").on("click", function () {
            CurrentAppId = $(this).parents("tr").attr("appId");

            if (typeof WebSocket === "undefined") {
                ShowAppNotification("Váš prohlížeč nepodporuje webSockety, a nemůže být využit k aktualizaci aplikací", "error");
                return;
            }

            appBuildDialog.dialog("option", { title: "aktualizuji " + $(this).parents("tr").data("displayName") }).empty().dialog("open");
            var messagesById = {};

            var ws = new WebSocket('ws://' + window.location.hostname + ':' + window.location.port + '/Master/AppAdminManager/RebuildApp/' + CurrentAppId);
            currentWs = ws;
            //var timeLast = Date.now();
            ws.onerror = function () {
                $(document).trigger("ajaxError", {})
            }
            ws.onmessage = function (event) {
                if (currentWs !== ws) return;

                //console.log(Date.now() - timeLast, event.data);
                //timeLast = Date.now();

                var response;
                try {
                    response = JSON.parse(event.data);
                } catch (e) {
                    response = { message: event.data, type: "error" };
                }

                var $message;
                if (response.id && messagesById[response.id]) {
                    $message = messagesById[response.id];
                } else {
                    var $parent = response.childOf ? messagesById[response.childOf] : appBuildDialog;
                    $message = $("<div class='app-alert'><span>").data("messageId", response.id).appendTo($parent);
                    if (!$parent.is("#app-build-dialog, .app-alert-odd")) $message.addClass("app-alert-odd");
                    if (response.id) messagesById[response.id] = $message;
                }

                if (response.message) $message.children("span").html(response.message);

                $message.removeClass("app-alert-info app-alert-error app-alert-success app-alert-warning").addClass("app-alert-" + (response.type || "info"));

                if (response.abort) $message.nextAll().remove();

                if (response.done) {
                    setTimeout(function () { appBuildDialog.dialog("close") }, 1000);
                }

                var childrenHeight = 0;
                appBuildDialog.children().each(function () {
                    childrenHeight += $(this).outerHeight();
                });
                appBuildDialog.css({ height: childrenHeight + 32 });
            };
        });
        $(".adminAppTable .actions .btnProperties").on("click", function () {
            CurrentAppId = $(this).parents("tr").attr("appId");
            appPropertiesDialog.dialog("open");
        });
        $(".adminAppTable .actions .btnToggleEnabled").on("click", function () {
            thisButton = $(this);
            appId = thisButton.parents("tr").attr("appId");
            isEnabledCell = thisButton.parents("tr").find(".isEnabledColumn");
            if (thisButton.hasClass("btnDisable")) {
                postData = {
                    IsEnabled: false
                }
                $.ajax({
                    type: "POST",
                    url: "/api/master/apps/" + appId + "/state",
                    data: postData,
                    success: function () {
                        thisButton.removeClass("btnDisable");
                        thisButton.addClass("btnEnable");
                        thisButton.removeClass("btn-danger");
                        thisButton.addClass("btn-primary");
                        thisButton.text("Povolit");
                        isEnabledCell.text("Ne");
                    }
                });
            }
            else {
                postData = {
                    IsEnabled: true
                }
                $.ajax({
                    type: "POST",
                    url: "/api/master/apps/" + appId + "/state",
                    data: postData,
                    success: function () {
                        thisButton.removeClass("btnEnable");
                        thisButton.addClass("btnDisable");
                        thisButton.removeClass("btn-primary");
                        thisButton.addClass("btn-danger");
                        thisButton.text("Zakázat");
                        isEnabledCell.text("Ano");
                    }
                });
            }
        });
        $("#btnAddApp").on("click", function () {
            addAppDialog.dialog("open");
        });
        $('.btn-export').on("click", function () {
            CurrentExportUrl = this.href;
            exportAppDialog.dialog('open');

            return false;
        });
    }
});

/**!
 * Sortable
 * @author	RubaXa   <trash@rubaxa.org>
 * @license MIT
 */

(function sortableModule(factory) {
    if (typeof define === "function" && define.amd) {
        define(factory);
    }
    else if (typeof module != "undefined" && typeof module.exports != "undefined") {
        module.exports = factory();
    }
    else if (typeof Package !== "undefined") {
        //noinspection JSUnresolvedVariable
        Sortable = factory();  // export for Meteor.js
    }
    else {
        /* jshint sub:true */
        window["Sortable"] = factory();
    }
})(function sortableFactory() {
    "use strict";

    if (typeof window == "undefined" || !window.document) {
        return function sortableError() {
            throw new Error("Sortable.js requires a window with a document");
        };
    }

    var dragEl,
		parentEl,
		ghostEl,
		cloneEl,
		rootEl,
		nextEl,

		scrollEl,
		scrollParentEl,
		scrollCustomFn,

		lastEl,
		lastCSS,
		lastParentCSS,

		oldIndex,
		newIndex,

		activeGroup,
		putSortable,

		autoScroll = {},

		tapEvt,
		touchEvt,

		moved,

		/** @const */
		RSPACE = /\s+/g,

		expando = 'Sortable' + (new Date).getTime(),

		win = window,
		document = win.document,
		parseInt = win.parseInt,

		$ = win.jQuery || win.Zepto,
		Polymer = win.Polymer,

		supportDraggable = !!('draggable' in document.createElement('div')),
		supportCssPointerEvents = (function (el) {
		    // false when IE11
		    if (!!navigator.userAgent.match(/Trident.*rv[ :]?11\./)) {
		        return false;
		    }
		    el = document.createElement('x');
		    el.style.cssText = 'pointer-events:auto';
		    return el.style.pointerEvents === 'auto';
		})(),

		_silent = false,

		abs = Math.abs,
		min = Math.min,
		slice = [].slice,

		touchDragOverListeners = [],

		_autoScroll = _throttle(function (/**Event*/evt, /**Object*/options, /**HTMLElement*/rootEl) {
		    // Bug: https://bugzilla.mozilla.org/show_bug.cgi?id=505521
		    if (rootEl && options.scroll) {
		        var el,
					rect,
					sens = options.scrollSensitivity,
					speed = options.scrollSpeed,

					x = evt.clientX,
					y = evt.clientY,

					winWidth = window.innerWidth,
					winHeight = window.innerHeight,

					vx,
					vy,

					scrollOffsetX,
					scrollOffsetY
		        ;

		        // Delect scrollEl
		        if (scrollParentEl !== rootEl) {
		            scrollEl = options.scroll;
		            scrollParentEl = rootEl;
		            scrollCustomFn = options.scrollFn;

		            if (scrollEl === true) {
		                scrollEl = rootEl;

		                do {
		                    if ((scrollEl.offsetWidth < scrollEl.scrollWidth) ||
								(scrollEl.offsetHeight < scrollEl.scrollHeight)
							) {
		                        break;
		                    }
		                    /* jshint boss:true */
		                } while (scrollEl = scrollEl.parentNode);
		            }
		        }

		        if (scrollEl) {
		            el = scrollEl;
		            rect = scrollEl.getBoundingClientRect();
		            vx = (abs(rect.right - x) <= sens) - (abs(rect.left - x) <= sens);
		            vy = (abs(rect.bottom - y) <= sens) - (abs(rect.top - y) <= sens);
		        }


		        if (!(vx || vy)) {
		            vx = (winWidth - x <= sens) - (x <= sens);
		            vy = (winHeight - y <= sens) - (y <= sens);

		            /* jshint expr:true */
		            (vx || vy) && (el = win);
		        }


		        if (autoScroll.vx !== vx || autoScroll.vy !== vy || autoScroll.el !== el) {
		            autoScroll.el = el;
		            autoScroll.vx = vx;
		            autoScroll.vy = vy;

		            clearInterval(autoScroll.pid);

		            if (el) {
		                autoScroll.pid = setInterval(function () {
		                    scrollOffsetY = vy ? vy * speed : 0;
		                    scrollOffsetX = vx ? vx * speed : 0;

		                    if ('function' === typeof (scrollCustomFn)) {
		                        return scrollCustomFn.call(_this, scrollOffsetX, scrollOffsetY, evt);
		                    }

		                    if (el === win) {
		                        win.scrollTo(win.pageXOffset + scrollOffsetX, win.pageYOffset + scrollOffsetY);
		                    } else {
		                        el.scrollTop += scrollOffsetY;
		                        el.scrollLeft += scrollOffsetX;
		                    }
		                }, 24);
		            }
		        }
		    }
		}, 30),

		_prepareGroup = function (options) {
		    function toFn(value, pull) {
		        if (value === void 0 || value === true) {
		            value = group.name;
		        }

		        if (typeof value === 'function') {
		            return value;
		        } else {
		            return function (to, from) {
		                var fromGroup = from.options.group.name;

		                return pull
							? value
							: value && (value.join
								? value.indexOf(fromGroup) > -1
								: (fromGroup == value)
							);
		            };
		        }
		    }

		    var group = {};
		    var originalGroup = options.group;

		    if (!originalGroup || typeof originalGroup != 'object') {
		        originalGroup = { name: originalGroup };
		    }

		    group.name = originalGroup.name;
		    group.checkPull = toFn(originalGroup.pull, true);
		    group.checkPut = toFn(originalGroup.put);

		    options.group = group;
		}
    ;



    /**
	 * @class  Sortable
	 * @param  {HTMLElement}  el
	 * @param  {Object}       [options]
	 */
    function Sortable(el, options) {
        if (!(el && el.nodeType && el.nodeType === 1)) {
            throw 'Sortable: `el` must be HTMLElement, and not ' + {}.toString.call(el);
        }

        this.el = el; // root element
        this.options = options = _extend({}, options);


        // Export instance
        el[expando] = this;


        // Default options
        var defaults = {
            group: Math.random(),
            sort: true,
            disabled: false,
            store: null,
            handle: null,
            scroll: true,
            scrollSensitivity: 30,
            scrollSpeed: 10,
            draggable: /[uo]l/i.test(el.nodeName) ? 'li' : '>*',
            ghostClass: 'sortable-ghost',
            chosenClass: 'sortable-chosen',
            dragClass: 'sortable-drag',
            ignore: 'a, img',
            filter: null,
            animation: 0,
            setData: function (dataTransfer, dragEl) {
                dataTransfer.setData('Text', dragEl.textContent);
            },
            dropBubble: false,
            dragoverBubble: false,
            dataIdAttr: 'data-id',
            delay: 0,
            forceFallback: false,
            fallbackClass: 'sortable-fallback',
            fallbackOnBody: false,
            fallbackTolerance: 0,
            fallbackOffset: { x: 0, y: 0 }
        };


        // Set default options
        for (var name in defaults) {
            !(name in options) && (options[name] = defaults[name]);
        }

        _prepareGroup(options);

        // Bind all private methods
        for (var fn in this) {
            if (fn.charAt(0) === '_' && typeof this[fn] === 'function') {
                this[fn] = this[fn].bind(this);
            }
        }

        // Setup drag mode
        this.nativeDraggable = options.forceFallback ? false : supportDraggable;

        // Bind events
        _on(el, 'mousedown', this._onTapStart);
        _on(el, 'touchstart', this._onTapStart);

        if (this.nativeDraggable) {
            _on(el, 'dragover', this);
            _on(el, 'dragenter', this);
        }

        touchDragOverListeners.push(this._onDragOver);

        // Restore sorting
        options.store && this.sort(options.store.get(this));
    }


    Sortable.prototype = /** @lends Sortable.prototype */ {
        constructor: Sortable,

        _onTapStart: function (/** Event|TouchEvent */evt) {
            var _this = this,
				el = this.el,
				options = this.options,
				type = evt.type,
				touch = evt.touches && evt.touches[0],
				target = (touch || evt).target,
				originalTarget = evt.target.shadowRoot && evt.path[0] || target,
				filter = options.filter,
				startIndex;

            // Don't trigger start event when an element is been dragged, otherwise the evt.oldindex always wrong when set option.group.
            if (dragEl) {
                return;
            }

            if (type === 'mousedown' && evt.button !== 0 || options.disabled) {
                return; // only left button or enabled
            }

            if (options.handle && !_closest(originalTarget, options.handle, el)) {
                return;
            }

            target = _closest(target, options.draggable, el);

            if (!target) {
                return;
            }

            // Get the index of the dragged element within its parent
            startIndex = _index(target, options.draggable);

            // Check filter
            if (typeof filter === 'function') {
                if (filter.call(this, evt, target, this)) {
                    _dispatchEvent(_this, originalTarget, 'filter', target, el, startIndex);
                    evt.preventDefault();
                    return; // cancel dnd
                }
            }
            else if (filter) {
                filter = filter.split(',').some(function (criteria) {
                    criteria = _closest(originalTarget, criteria.trim(), el);

                    if (criteria) {
                        _dispatchEvent(_this, criteria, 'filter', target, el, startIndex);
                        return true;
                    }
                });

                if (filter && !$(originalTarget).is(options.handle)) {
                    evt.preventDefault();
                    return; // cancel dnd
                }
            }

            if (options.handle && $(originalTarget).is(options.handle)) {
                target = $(originalTarget).parent()[0];
            }

            // Prepare `dragstart`
            this._prepareDragStart(evt, touch, target, startIndex);
        },

        _prepareDragStart: function (/** Event */evt, /** Touch */touch, /** HTMLElement */target, /** Number */startIndex) {
            var _this = this,
				el = _this.el,
				options = _this.options,
				ownerDocument = el.ownerDocument,
				dragStartFn;

            if (target && !dragEl && (target.parentNode === el)) {
                tapEvt = evt;

                rootEl = el;
                dragEl = target;
                parentEl = dragEl.parentNode;
                nextEl = dragEl.nextSibling;
                activeGroup = options.group;
                oldIndex = startIndex;

                this._lastX = (touch || evt).clientX;
                this._lastY = (touch || evt).clientY;

                dragEl.style['will-change'] = 'transform';

                dragStartFn = function () {
                    // Delayed drag has been triggered
                    // we can re-enable the events: touchmove/mousemove
                    _this._disableDelayedDrag();

                    // Make the element draggable
                    dragEl.draggable = _this.nativeDraggable;

                    // Chosen item
                    _toggleClass(dragEl, options.chosenClass, true);

                    // Bind the events: dragstart/dragend
                    _this._triggerDragStart(touch);

                    // Drag start event
                    _dispatchEvent(_this, rootEl, 'choose', dragEl, rootEl, oldIndex);
                };

                // Disable "draggable"
                options.ignore.split(',').forEach(function (criteria) {
                    _find(dragEl, criteria.trim(), _disableDraggable);
                });

                _on(ownerDocument, 'mouseup', _this._onDrop);
                _on(ownerDocument, 'touchend', _this._onDrop);
                _on(ownerDocument, 'touchcancel', _this._onDrop);

                if (options.delay) {
                    // If the user moves the pointer or let go the click or touch
                    // before the delay has been reached:
                    // disable the delayed drag
                    _on(ownerDocument, 'mouseup', _this._disableDelayedDrag);
                    _on(ownerDocument, 'touchend', _this._disableDelayedDrag);
                    _on(ownerDocument, 'touchcancel', _this._disableDelayedDrag);
                    _on(ownerDocument, 'mousemove', _this._disableDelayedDrag);
                    _on(ownerDocument, 'touchmove', _this._disableDelayedDrag);

                    _this._dragStartTimer = setTimeout(dragStartFn, options.delay);
                } else {
                    dragStartFn();
                }
            }
        },

        _disableDelayedDrag: function () {
            var ownerDocument = this.el.ownerDocument;

            clearTimeout(this._dragStartTimer);
            _off(ownerDocument, 'mouseup', this._disableDelayedDrag);
            _off(ownerDocument, 'touchend', this._disableDelayedDrag);
            _off(ownerDocument, 'touchcancel', this._disableDelayedDrag);
            _off(ownerDocument, 'mousemove', this._disableDelayedDrag);
            _off(ownerDocument, 'touchmove', this._disableDelayedDrag);
        },

        _triggerDragStart: function (/** Touch */touch) {
            if (touch) {
                // Touch device support
                tapEvt = {
                    target: dragEl,
                    clientX: touch.clientX,
                    clientY: touch.clientY
                };

                this._onDragStart(tapEvt, 'touch');
            }
            else if (!this.nativeDraggable) {
                this._onDragStart(tapEvt, true);
            }
            else {
                _on(dragEl, 'dragend', this);
                _on(rootEl, 'dragstart', this._onDragStart);
            }

            try {
                if (document.selection) {
                    // Timeout neccessary for IE9					
                    setTimeout(function () {
                        document.selection.empty();
                    });
                } else {
                    window.getSelection().removeAllRanges();
                }
            } catch (err) {
            }
        },

        _dragStarted: function () {
            if (rootEl && dragEl) {
                var options = this.options;

                // Apply effect
                _toggleClass(dragEl, options.ghostClass, true);
                _toggleClass(dragEl, options.dragClass, false);

                Sortable.active = this;

                // Drag start event
                _dispatchEvent(this, rootEl, 'start', dragEl, rootEl, oldIndex);
            }
        },

        _emulateDragOver: function () {
            if (touchEvt) {
                if (this._lastX === touchEvt.clientX && this._lastY === touchEvt.clientY) {
                    return;
                }

                this._lastX = touchEvt.clientX;
                this._lastY = touchEvt.clientY;

                if (!supportCssPointerEvents) {
                    _css(ghostEl, 'display', 'none');
                }

                var target = document.elementFromPoint(touchEvt.clientX, touchEvt.clientY),
					parent = target,
					i = touchDragOverListeners.length;

                if (parent) {
                    do {
                        if (parent[expando]) {
                            while (i--) {
                                touchDragOverListeners[i]({
                                    clientX: touchEvt.clientX,
                                    clientY: touchEvt.clientY,
                                    target: target,
                                    rootEl: parent
                                });
                            }

                            break;
                        }

                        target = parent; // store last element
                    }
                        /* jshint boss:true */
                    while (parent = parent.parentNode);
                }

                if (!supportCssPointerEvents) {
                    _css(ghostEl, 'display', '');
                }
            }
        },


        _onTouchMove: function (/**TouchEvent*/evt) {
            if (tapEvt) {
                var options = this.options,
					fallbackTolerance = options.fallbackTolerance,
					fallbackOffset = options.fallbackOffset,
					touch = evt.touches ? evt.touches[0] : evt,
					dx = (touch.clientX - tapEvt.clientX) + fallbackOffset.x,
					dy = (touch.clientY - tapEvt.clientY) + fallbackOffset.y,
					translate3d = evt.touches ? 'translate3d(' + dx + 'px,' + dy + 'px,0)' : 'translate(' + dx + 'px,' + dy + 'px)';

                // only set the status to dragging, when we are actually dragging
                if (!Sortable.active) {
                    if (fallbackTolerance &&
						min(abs(touch.clientX - this._lastX), abs(touch.clientY - this._lastY)) < fallbackTolerance
					) {
                        return;
                    }

                    this._dragStarted();
                }

                // as well as creating the ghost element on the document body
                this._appendGhost();

                moved = true;
                touchEvt = touch;

                _css(ghostEl, 'webkitTransform', translate3d);
                _css(ghostEl, 'mozTransform', translate3d);
                _css(ghostEl, 'msTransform', translate3d);
                _css(ghostEl, 'transform', translate3d);

                evt.preventDefault();
            }
        },

        _appendGhost: function () {
            if (!ghostEl) {
                var rect = dragEl.getBoundingClientRect(),
					css = _css(dragEl),
					options = this.options,
					ghostRect;

                ghostEl = dragEl.cloneNode(true);

                _toggleClass(ghostEl, options.ghostClass, false);
                _toggleClass(ghostEl, options.fallbackClass, true);
                _toggleClass(ghostEl, options.dragClass, true);

                _css(ghostEl, 'top', rect.top - parseInt(css.marginTop, 10));
                _css(ghostEl, 'left', rect.left - parseInt(css.marginLeft, 10));
                _css(ghostEl, 'width', rect.width);
                _css(ghostEl, 'height', rect.height);
                _css(ghostEl, 'opacity', '0.8');
                _css(ghostEl, 'position', 'fixed');
                _css(ghostEl, 'zIndex', '100000');
                _css(ghostEl, 'pointerEvents', 'none');

                options.fallbackOnBody && document.body.appendChild(ghostEl) || rootEl.appendChild(ghostEl);

                // Fixing dimensions.
                ghostRect = ghostEl.getBoundingClientRect();
                _css(ghostEl, 'width', rect.width * 2 - ghostRect.width);
                _css(ghostEl, 'height', rect.height * 2 - ghostRect.height);
            }
        },

        _onDragStart: function (/**Event*/evt, /**boolean*/useFallback) {
            var dataTransfer = evt.dataTransfer,
				options = this.options;

            this._offUpEvents();

            if (activeGroup.checkPull(this, this, dragEl, evt) == 'clone') {
                cloneEl = _clone(dragEl);
                _css(cloneEl, 'display', 'none');
                rootEl.insertBefore(cloneEl, dragEl);
                _dispatchEvent(this, rootEl, 'clone', dragEl);
            }

            _toggleClass(dragEl, options.dragClass, true);

            if (useFallback) {
                if (useFallback === 'touch') {
                    // Bind touch events
                    _on(document, 'touchmove', this._onTouchMove);
                    _on(document, 'touchend', this._onDrop);
                    _on(document, 'touchcancel', this._onDrop);
                } else {
                    // Old brwoser
                    _on(document, 'mousemove', this._onTouchMove);
                    _on(document, 'mouseup', this._onDrop);
                }

                this._loopId = setInterval(this._emulateDragOver, 50);
            }
            else {
                if (dataTransfer) {
                    dataTransfer.effectAllowed = 'move';
                    options.setData && options.setData.call(this, dataTransfer, dragEl);
                }

                _on(document, 'drop', this);
                setTimeout(this._dragStarted, 0);
            }
        },

        _onDragOver: function (/**Event*/evt) {
            var el = this.el,
				target,
				dragRect,
				targetRect,
				revert,
				options = this.options,
				group = options.group,
				activeSortable = Sortable.active,
				isOwner = (activeGroup === group),
				canSort = options.sort;

            if (evt.preventDefault !== void 0) {
                evt.preventDefault();
                !options.dragoverBubble && evt.stopPropagation();
            }

            moved = true;

            if (activeGroup && !options.disabled &&
				(isOwner
					? canSort || (revert = !rootEl.contains(dragEl)) // Reverting item into the original list
					: (
						putSortable === this ||
						activeGroup.checkPull(this, activeSortable, dragEl, evt) && group.checkPut(this, activeSortable, dragEl, evt)
					)
				) &&
				(evt.rootEl === void 0 || evt.rootEl === this.el) // touch fallback
			) {
                // Smart auto-scrolling
                _autoScroll(evt, options, this.el);

                if (_silent) {
                    return;
                }

                target = _closest(evt.target, options.draggable, el);
                dragRect = dragEl.getBoundingClientRect();
                putSortable = this;

                if(typeof options.onDragOver == 'function') {
                    options.onDragOver(el);
                }

                if (revert) {
                    _cloneHide(true);
                    parentEl = rootEl; // actualization

                    if (cloneEl || nextEl) {
                        rootEl.insertBefore(dragEl, cloneEl || nextEl);
                    }
                    else if (!canSort) {
                        rootEl.appendChild(dragEl);
                    }

                    return;
                }


                if ((el.children.length === 0) || (el.children[0] === ghostEl) ||
					(el === evt.target) && (target = _ghostIsLast(el, evt))
				) {
                    if (target) {
                        if (target.animated) {
                            return;
                        }

                        targetRect = target.getBoundingClientRect();
                    }

                    _cloneHide(isOwner);

                    if (_onMove(rootEl, el, dragEl, dragRect, target, targetRect, evt) !== false) {
                        if (!dragEl.contains(el)) {
                            el.appendChild(dragEl);
                            parentEl = el; // actualization
                        }

                        this._animate(dragRect, dragEl);
                        target && this._animate(targetRect, target);
                    }
                }
                else if (target && !target.animated && target !== dragEl && (target.parentNode[expando] !== void 0)) {
                    if (lastEl !== target) {
                        lastEl = target;
                        lastCSS = _css(target);
                        lastParentCSS = _css(target.parentNode);
                    }

                    targetRect = target.getBoundingClientRect();

                    var width = targetRect.right - targetRect.left,
						height = targetRect.bottom - targetRect.top,
						floating = /left|right|inline/.test(lastCSS.cssFloat + lastCSS.display)
							|| (lastParentCSS.display == 'flex' && lastParentCSS['flex-direction'].indexOf('row') === 0),
						isWide = (target.offsetWidth > dragEl.offsetWidth),
						isLong = (target.offsetHeight > dragEl.offsetHeight),
						halfway = (floating ? (evt.clientX - targetRect.left) / width : (evt.clientY - targetRect.top) / height) > 0.5,
						nextSibling = target.nextElementSibling,
						moveVector = _onMove(rootEl, el, dragEl, dragRect, target, targetRect, evt),
						after
                    ;

                    if (moveVector !== false) {
                        _silent = true;
                        setTimeout(_unsilent, 30);

                        _cloneHide(isOwner);

                        if (moveVector === 1 || moveVector === -1) {
                            after = (moveVector === 1);
                        }
                        else if (floating) {
                            var elTop = dragEl.offsetTop,
								tgTop = target.offsetTop;

                            if (elTop === tgTop) {
                                after = (target.previousElementSibling === dragEl) && !isWide || halfway && isWide;
                            }
                            else if (target.previousElementSibling === dragEl || dragEl.previousElementSibling === target) {
                                after = (evt.clientY - targetRect.top) / height > 0.5;
                            } else {
                                after = tgTop > elTop;
                            }
                        } else {
                            after = (nextSibling !== dragEl) && !isLong || halfway && isLong;
                        }

                        if (!dragEl.contains(el)) {
                            if (after && !nextSibling) {
                                el.appendChild(dragEl);
                            } else {
                                target.parentNode.insertBefore(dragEl, after ? nextSibling : target);
                            }
                        }

                        parentEl = dragEl.parentNode; // actualization

                        this._animate(dragRect, dragEl);
                        this._animate(targetRect, target);
                    }
                }
            }
        },

        _animate: function (prevRect, target) {
            var ms = this.options.animation;

            if (ms) {
                var currentRect = target.getBoundingClientRect();

                _css(target, 'transition', 'none');
                _css(target, 'transform', 'translate3d('
					+ (prevRect.left - currentRect.left) + 'px,'
					+ (prevRect.top - currentRect.top) + 'px,0)'
				);

                target.offsetWidth; // repaint

                _css(target, 'transition', 'all ' + ms + 'ms');
                _css(target, 'transform', 'translate3d(0,0,0)');

                clearTimeout(target.animated);
                target.animated = setTimeout(function () {
                    _css(target, 'transition', '');
                    _css(target, 'transform', '');
                    target.animated = false;
                }, ms);
            }
        },

        _offUpEvents: function () {
            var ownerDocument = this.el.ownerDocument;

            _off(document, 'touchmove', this._onTouchMove);
            _off(ownerDocument, 'mouseup', this._onDrop);
            _off(ownerDocument, 'touchend', this._onDrop);
            _off(ownerDocument, 'touchcancel', this._onDrop);
        },

        _onDrop: function (/**Event*/evt) {
            var el = this.el,
				options = this.options;

            clearInterval(this._loopId);
            clearInterval(autoScroll.pid);
            clearTimeout(this._dragStartTimer);

            // Unbind events
            _off(document, 'mousemove', this._onTouchMove);

            if (this.nativeDraggable) {
                _off(document, 'drop', this);
                _off(el, 'dragstart', this._onDragStart);
            }

            this._offUpEvents();

            if (evt) {
                if (moved) {
                    evt.preventDefault();
                    !options.dropBubble && evt.stopPropagation();
                }

                ghostEl && ghostEl.parentNode.removeChild(ghostEl);

                if (dragEl) {
                    if (this.nativeDraggable) {
                        _off(dragEl, 'dragend', this);
                    }

                    _disableDraggable(dragEl);
                    dragEl.style['will-change'] = '';

                    // Remove class's
                    _toggleClass(dragEl, this.options.ghostClass, false);
                    _toggleClass(dragEl, this.options.chosenClass, false);

                    if (rootEl !== parentEl) {
                        newIndex = _index(dragEl, options.draggable);

                        if (newIndex >= 0) {

                            // Add event
                            _dispatchEvent(null, parentEl, 'add', dragEl, rootEl, oldIndex, newIndex);

                            // Remove event
                            _dispatchEvent(this, rootEl, 'remove', dragEl, rootEl, oldIndex, newIndex);

                            // drag from one list and drop into another
                            _dispatchEvent(null, parentEl, 'sort', dragEl, rootEl, oldIndex, newIndex);
                            _dispatchEvent(this, rootEl, 'sort', dragEl, rootEl, oldIndex, newIndex);
                        }
                    }
                    else {
                        // Remove clone
                        cloneEl && cloneEl.parentNode.removeChild(cloneEl);

                        if (dragEl.nextSibling !== nextEl) {
                            // Get the index of the dragged element within its parent
                            newIndex = _index(dragEl, options.draggable);

                            if (newIndex >= 0) {
                                // drag & drop within the same list
                                _dispatchEvent(this, rootEl, 'update', dragEl, rootEl, oldIndex, newIndex);
                                _dispatchEvent(this, rootEl, 'sort', dragEl, rootEl, oldIndex, newIndex);
                            }
                        }
                    }

                    if (Sortable.active) {
                        /* jshint eqnull:true */
                        if (newIndex == null || newIndex === -1) {
                            newIndex = oldIndex;
                        }

                        _dispatchEvent(this, rootEl, 'end', dragEl, rootEl, oldIndex, newIndex);

                        // Save sorting
                        this.save();
                    }
                }

            }

            this._nulling();
        },

        _nulling: function () {
            rootEl =
			dragEl =
			parentEl =
			ghostEl =
			nextEl =
			cloneEl =

			scrollEl =
			scrollParentEl =

			tapEvt =
			touchEvt =

			moved =
			newIndex =

			lastEl =
			lastCSS =

			putSortable =
			activeGroup =
			Sortable.active = null;
        },

        handleEvent: function (/**Event*/evt) {
            var type = evt.type;

            if (type === 'dragover' || type === 'dragenter') {
                if (dragEl) {
                    this._onDragOver(evt);
                    _globalDragOver(evt);
                }
            }
            else if (type === 'drop' || type === 'dragend') {
                this._onDrop(evt);
            }
        },


        /**
		 * Serializes the item into an array of string.
		 * @returns {String[]}
		 */
        toArray: function () {
            var order = [],
				el,
				children = this.el.children,
				i = 0,
				n = children.length,
				options = this.options;

            for (; i < n; i++) {
                el = children[i];
                if (_closest(el, options.draggable, this.el)) {
                    order.push(el.getAttribute(options.dataIdAttr) || _generateId(el));
                }
            }

            return order;
        },


        /**
		 * Sorts the elements according to the array.
		 * @param  {String[]}  order  order of the items
		 */
        sort: function (order) {
            var items = {}, rootEl = this.el;

            this.toArray().forEach(function (id, i) {
                var el = rootEl.children[i];

                if (_closest(el, this.options.draggable, rootEl)) {
                    items[id] = el;
                }
            }, this);

            order.forEach(function (id) {
                if (items[id]) {
                    rootEl.removeChild(items[id]);
                    rootEl.appendChild(items[id]);
                }
            });
        },


        /**
		 * Save the current sorting
		 */
        save: function () {
            var store = this.options.store;
            store && store.set(this);
        },


        /**
		 * For each element in the set, get the first element that matches the selector by testing the element itself and traversing up through its ancestors in the DOM tree.
		 * @param   {HTMLElement}  el
		 * @param   {String}       [selector]  default: `options.draggable`
		 * @returns {HTMLElement|null}
		 */
        closest: function (el, selector) {
            return _closest(el, selector || this.options.draggable, this.el);
        },


        /**
		 * Set/get option
		 * @param   {string} name
		 * @param   {*}      [value]
		 * @returns {*}
		 */
        option: function (name, value) {
            var options = this.options;

            if (value === void 0) {
                return options[name];
            } else {
                options[name] = value;

                if (name === 'group') {
                    _prepareGroup(options);
                }
            }
        },


        /**
		 * Destroy
		 */
        destroy: function () {
            var el = this.el;

            el[expando] = null;

            _off(el, 'mousedown', this._onTapStart);
            _off(el, 'touchstart', this._onTapStart);

            if (this.nativeDraggable) {
                _off(el, 'dragover', this);
                _off(el, 'dragenter', this);
            }

            // Remove draggable attributes
            Array.prototype.forEach.call(el.querySelectorAll('[draggable]'), function (el) {
                el.removeAttribute('draggable');
            });

            touchDragOverListeners.splice(touchDragOverListeners.indexOf(this._onDragOver), 1);

            this._onDrop();

            this.el = el = null;
        }
    };


    function _cloneHide(state) {
        if (cloneEl && (cloneEl.state !== state)) {
            _css(cloneEl, 'display', state ? 'none' : '');
            !state && cloneEl.state && rootEl.insertBefore(cloneEl, dragEl);
            cloneEl.state = state;
        }
    }


    function _closest(/**HTMLElement*/el, /**String*/selector, /**HTMLElement*/ctx) {
        if (el) {
            ctx = ctx || document;

            do {
                if ((selector === '>*' && el.parentNode === ctx) || _matches(el, selector)) {
                    return el;
                }
                /* jshint boss:true */
            } while (el = _getParentOrHost(el));
        }

        return null;
    }


    function _getParentOrHost(el) {
        var parent = el.host;

        return (parent && parent.nodeType) ? parent : el.parentNode;
    }


    function _globalDragOver(/**Event*/evt) {
        if (evt.dataTransfer) {
            evt.dataTransfer.dropEffect = 'move';
        }
        evt.preventDefault();
    }


    function _on(el, event, fn) {
        el.addEventListener(event, fn, false);
    }


    function _off(el, event, fn) {
        el.removeEventListener(event, fn, false);
    }


    function _toggleClass(el, name, state) {
        if (el) {
            if (el.classList) {
                el.classList[state ? 'add' : 'remove'](name);
            }
            else {
                var className = (' ' + el.className + ' ').replace(RSPACE, ' ').replace(' ' + name + ' ', ' ');
                el.className = (className + (state ? ' ' + name : '')).replace(RSPACE, ' ');
            }
        }
    }


    function _css(el, prop, val) {
        var style = el && el.style;

        if (style) {
            if (val === void 0) {
                if (document.defaultView && document.defaultView.getComputedStyle) {
                    val = document.defaultView.getComputedStyle(el, '');
                }
                else if (el.currentStyle) {
                    val = el.currentStyle;
                }

                return prop === void 0 ? val : val[prop];
            }
            else {
                if (!(prop in style)) {
                    prop = '-webkit-' + prop;
                }

                style[prop] = val + (typeof val === 'string' ? '' : 'px');
            }
        }
    }


    function _find(ctx, tagName, iterator) {
        if (ctx) {
            var list = ctx.getElementsByTagName(tagName), i = 0, n = list.length;

            if (iterator) {
                for (; i < n; i++) {
                    iterator(list[i], i);
                }
            }

            return list;
        }

        return [];
    }



    function _dispatchEvent(sortable, rootEl, name, targetEl, fromEl, startIndex, newIndex) {
        sortable = (sortable || rootEl[expando]);

        var evt = document.createEvent('Event'),
			options = sortable.options,
			onName = 'on' + name.charAt(0).toUpperCase() + name.substr(1);

        evt.initEvent(name, true, true);

        evt.to = rootEl;
        evt.from = fromEl || rootEl;
        evt.item = targetEl || rootEl;
        evt.clone = cloneEl;

        evt.oldIndex = startIndex;
        evt.newIndex = newIndex;

        rootEl.dispatchEvent(evt);

        if (options[onName]) {
            options[onName].call(sortable, evt);
        }
    }


    function _onMove(fromEl, toEl, dragEl, dragRect, targetEl, targetRect, originalEvt) {
        var evt,
			sortable = fromEl[expando],
			onMoveFn = sortable.options.onMove,
			retVal;

        evt = document.createEvent('Event');
        evt.initEvent('move', true, true);

        evt.to = toEl;
        evt.from = fromEl;
        evt.dragged = dragEl;
        evt.draggedRect = dragRect;
        evt.related = targetEl || toEl;
        evt.relatedRect = targetRect || toEl.getBoundingClientRect();

        fromEl.dispatchEvent(evt);

        if (onMoveFn) {
            retVal = onMoveFn.call(sortable, evt, originalEvt);
        }

        return retVal;
    }


    function _disableDraggable(el) {
        el.draggable = false;
    }


    function _unsilent() {
        _silent = false;
    }


    /** @returns {HTMLElement|false} */
    function _ghostIsLast(el, evt) {
        var lastEl = el.lastElementChild,
			rect = lastEl.getBoundingClientRect();

        // 5 — min delta
        // abs — нельзя добавлять, а то глюки при наведении сверху
        return (
			(evt.clientY - (rect.top + rect.height) > 5) ||
			(evt.clientX - (rect.right + rect.width) > 5)
		) && lastEl;
    }


    /**
	 * Generate id
	 * @param   {HTMLElement} el
	 * @returns {String}
	 * @private
	 */
    function _generateId(el) {
        var str = el.tagName + el.className + el.src + el.href + el.textContent,
			i = str.length,
			sum = 0;

        while (i--) {
            sum += str.charCodeAt(i);
        }

        return sum.toString(36);
    }

    /**
	 * Returns the index of an element within its parent for a selected set of
	 * elements
	 * @param  {HTMLElement} el
	 * @param  {selector} selector
	 * @return {number}
	 */
    function _index(el, selector) {
        var index = 0;

        if (!el || !el.parentNode) {
            return -1;
        }

        while (el && (el = el.previousElementSibling)) {
            if ((el.nodeName.toUpperCase() !== 'TEMPLATE') && (selector === '>*' || _matches(el, selector))) {
                index++;
            }
        }

        return index;
    }

    function _matches(/**HTMLElement*/el, /**String*/selector) {
        if (el) {
            selector = selector.split('.');

            var tag = selector.shift().toUpperCase(),
				re = new RegExp('\\s(' + selector.join('|') + ')(?=\\s)', 'g');

            return (
				(tag === '' || el.nodeName.toUpperCase() == tag) &&
				(!selector.length || ((' ' + el.className + ' ').match(re) || []).length == selector.length)
			);
        }

        return false;
    }

    function _throttle(callback, ms) {
        var args, _this;

        return function () {
            if (args === void 0) {
                args = arguments;
                _this = this;

                setTimeout(function () {
                    if (args.length === 1) {
                        callback.call(_this, args[0]);
                    } else {
                        callback.apply(_this, args);
                    }

                    args = void 0;
                }, ms);
            }
        };
    }

    function _extend(dst, src) {
        if (dst && src) {
            for (var key in src) {
                if (src.hasOwnProperty(key)) {
                    dst[key] = src[key];
                }
            }
        }

        return dst;
    }

    function _clone(el) {
        return $
			? $(el).clone(true)[0]
			: (Polymer && Polymer.dom
				? Polymer.dom(el).cloneNode(true)
				: el.cloneNode(true)
			);
    }


    // Export utils
    Sortable.utils = {
        on: _on,
        off: _off,
        css: _css,
        find: _find,
        is: function (el, selector) {
            return !!_closest(el, selector, el);
        },
        extend: _extend,
        throttle: _throttle,
        closest: _closest,
        toggleClass: _toggleClass,
        clone: _clone,
        index: _index
    };


    /**
	 * Create sortable instance
	 * @param {HTMLElement}  el
	 * @param {Object}      [options]
	 */
    Sortable.create = function (el, options) {
        return new Sortable(el, options);
    };


    // Export
    Sortable.version = '1.4.2';
    return Sortable;
});
var MBE = {

    types: {},
    options: {},
    sortableOptions: {},

    onInit: [],
    onBeforeInit: [],
    onBeforeDelete: { '*': []},

    win: null,
    workspace: null,
    workspaceDoc: null,
    changedSinceLastSave: false,
    saveRequested: false,

    // Inicializace
    preInit: function ()
    {
        setTimeout(function () {
            MBE.win = $('#mozaicPageWorkspace > iframe')[0].contentWindow;
            MBE.workspaceDoc = MBE.win.document ? MBE.win.document : MBE.win.contentDocument;

            MBE.workspace = $('body', MBE.workspaceDoc);
            MBE.workspace
                    .html('')
                    .css({
                        'min-width': '100%',
                        'min-height': '100%'
                    })
                    .addClass('mozaicEditorBody')
                    .parent()
                        .css({
                            'width': '100%',
                            'height': '100%'
                        });
            MBE.init();
        }, 2000);
    },

    init: function()
    {
        for (i = 0; i < MBE.onBeforeInit.length; i++) {
            MBE.onBeforeInit[i]();
        }

        $(document)
            .on('click', 'ul.category li', MBE.toggleCategory)
            .on('dblclick', '.mbe-text-node', MBE.editText)
            .on('blur', '[contenteditable]', MBE.editTextDone)
            .on('click', '[data-uic]', MBE.onClick)
            .on('dblclick', '[data-uic]', MBE.options.openDialog)
            .on('keydown', MBE.onKeyDown)
            .on('click', '[data-action="fullscreen"]', MBE.toggleFullscreen)
            .on('click', '.device-button', MBE.setDevice)
            .on('webkitfullscreenchange mozfullscreenchange msfullscreenchange ofullscreenchange fullscreenchange', MBE.fullscreenResize)
            .on('click', '#btnChoosePage', MBE.dialogs.choosePage)
            .on('click', '#btnTrashPage', MBE.dialogs.trash)
            .on('click', '#btnNewPage', MBE.dialogs.newPage)
            .on('click', '#headerPageName', MBE.dialogs.rename)
            .on('click', 'tr.pageRow', MBE.selectPage)
            .on('click', '#btnClear', MBE.clearWorkspace)
            .on('click', '#btnLoad', MBE.io.reloadPage)
            .on('click', '#btnSave', MBE.io.savePage)
        
        ;
        $(MBE.workspaceDoc)
            .on('keydown', MBE.onKeyDown)
            .on('dblclick', '.mbe-text-node', MBE.editText)
            .on('blur', '[contenteditable]', MBE.editTextDone)
            .on('click', '[data-uic]', MBE.onClick)
            .on('dblclick', '[data-uic]', MBE.options.openDialog)
        ;
        
        $('ul.category > li ul').hide();
        $('ul.category > li').prepend('<span class="fa fa-caret-right fa-fw"></span>');
        $('ul.category > li > ul > li').prepend('<span class="fa fa-square fa-fw"></span>');

        for (i = 0; i < MBE.onInit.length; i++) {
            MBE.onInit[i]();
        }
    },

    onClick: function(event) {
        event.preventDefault();
    },

    onKeyDown: function(event) {
        if (event.which == 46 && $('.dialog-options').length == 0) {
            MBE.deleteItem();   
        }
    },

    deleteItem: function () {
        var target = $('.mbe-active', MBE.workspace);
        if (target.length && !target.is('[locked]') && !target.is('[contenteditable=true]') && !target.find('[contenteditable=true]').length) {
            if (typeof MBE.onBeforeDelete[target.data('uic')] == 'function') {
                MBE.onBeforeDelete[target.data('uic')].apply(target[0], []);
            }
            for (var i = 0; i < MBE.onBeforeDelete['*'].length; i++) {
                MBE.onBeforeDelete['*'][i].apply(target[0], []);
            }

            $('.mbe-active', MBE.workspace).remove();
            $('.mbe-drag-handle', MBE.workspace).remove();
            MBE.path.update.apply(MBE.workspace, []);
            MBE.DnD.updateDOM();
        }
    },

    // Kategorie
    toggleCategory: function(event)
    {
        if (!$(this).parent().hasClass('category')) {
            event.stopImmediatePropagation();
            return false;
        }
        $('> ul', this).slideToggle();
        $('> .fa', this).toggleClass('fa-caret-right fa-caret-down');
    },

    // Editace textu
    editText: function(event)
    {
        event.stopImmediatePropagation();
        $(this).attr('contenteditable', true).focus();
        return false;
    },

    editTextDone: function()
    {
        $('[contenteditable]', MBE.workspace).attr('contenteditable', false);
        $('.mbe-text-node > span', MBE.workspace).each(function () {
            $(this).parent().html(this.innerHTML);
        });
    },

    // TOOLS
    getComponentName: function(elm)
    {
        var uic = $(elm).data('uic').split(/\|/);
        var template = uic[1];
        return $('li[data-template="' + template + '"]').text();
    },

    setDevice: function () {
        $('.device-button').removeClass('active');
        $(this).addClass('active');

        $('#mozaicPageWorkspace > iframe').removeClass('xs sm md lg').addClass($(this).attr('data-action'));
    },

    toggleFullscreen: function()
    {
        if (MBE.runPrefixMethod(document, "FullScreen") || MBE.runPrefixMethod(document, "IsFullScreen")) {
            MBE.runPrefixMethod(document, "CancelFullScreen");
        }
        else {
            MBE.runPrefixMethod(document.body, "RequestFullScreen");
        }
        return false;
    },
	
    isFullscreen: function()
    {
        return MBE.runPrefixMethod(document, "FullScreen") || MBE.runPrefixMethod(document, "IsFullScreen");
    },

    fullscreenResize: function()
    {
        $('#lowerPanel').toggleClass('fullscreen');
    },

    runPrefixMethod: function (obj, method, testAPI) 
    {
        var p = 0, m, t;
        var prefixList = ["webkit", "moz", "ms", "o", ""];
        while (p < prefixList.length && !obj[m]) {
            m = method;
            if (prefixList[p] == "") {
                m = m.substr(0,1).toLowerCase() + m.substr(1);
            }
            m = prefixList[p] + m;
            t = typeof obj[m];
            if (t != "undefined") {
                prefixList = [prefixList[p]];
                return (t == "function" ? obj[m]() : obj[m]);
            }
            p++;
        }
        if(testAPI)
            return -1;
    },

    selectPage: function () {
        $(this).addClass('highlightedRow').siblings().removeClass('highlightedRow');
    },

    clearWorkspace: function () {
        MBE.workspace.html('');
        MBE.DnD.updateDOM();
    },

    ajaxError: function (request, status, error) {
        alert(request.responseText);
    }
}

if ($('body').hasClass('mozaicBootstrapEditorModule')) {
    $(MBE.preInit);
}
MBE.DnD = {
    
    currentElement: null,
    placeholder: null,
    onDOMUpdate: [],
    onDragEnter: [],
    onDragLeave: [],
    onDragEnd: [],
    onDrop: [],
    onBeforeDrop: [],

    isNavNodeDragging: false,
    isUICDragging: false,
    domNeedUpdate: false,

    init: function()
    {
        var self = MBE.DnD;

        self.placeholder = $('#drop-placeholder');
        self.placeholder.hide();

        $('ul.category > li > ul > li').attr('draggable', true);
        $('ul.category > li > ul > li').each(function () {
            $(this).data('type', $(this).parent().data('type'));
        });

        $(document)
            .on('dragstart', 'ul.category li[draggable=true]', self._componentDragStart)
            .on('dragstart', '.tree-nav .node-handle[draggable=true]', self._navNodeDragStart)
            .on('dragover', '.tree-nav .node:not(.dragged)', self._navNodeDragOver)
            .on('drop', '.tree-nav .node:not(.dragged)', self._navNodeDrop)
            .on('dragend dragstop', self._dragEnd)
        ;

        $(MBE.workspaceDoc)
            .on('dragover', "body, [data-uic]:not(.dragged)", self._dragOver)
            .on('dragenter', "body, [data-uic]", self._dragEnter)
            .on('dragleave', "body, [data-uic]", self._dragLeave)
            .on('dragstart', '.mbe-drag-handle[draggable=true]', self._uicDragStart)
            .on('drop', "body, [data-uic]", self._workSpaceDrop)
            .on('dragend dragstop', self._dragEnd)
        ;
    },

    getUIC: function() 
    {
        var self = MBE.DnD;
        var item = $(self.currentElement)
        
        if (item.hasClass('node')) {
            return item.find('> .node-handle b').data('targetuic');
        }
        else if (!item.is('[data-uic]')) {
            return self.createUIC(item);
        }
        
        return item;
    },

    createUIC: function (item)
    {
        var type = item.data('type');
        var template = item.data('template');

        var elm = $(MBE.types[type].templates[template]);
        if (!elm.data('uic')) {
            elm.attr('data-uic', type + "|" + template);
        }

        return elm;
    },

    createGhost: function()
    {
        var ghost = $('<span class="drag-ghost" style="position:absolute; left: -100000px; top: -100000px"></span>');
        ghost.appendTo('body');

        return ghost[0];
    },

    /*******************************************************/
    /* START                                               */
    /*******************************************************/
    _componentDragStart: function(event)
    {
        var e = event.originalEvent; 
        
        MBE.DnD.currentElement = this;

        var ghost = MBE.DnD.createGhost();
        $('body').addClass('dragging');

        e.dataTransfer.setData('text/plain', '...');
        e.dataTransfer.effectAllowed = 'all';
        e.dataTransfer.setDragImage(ghost, -12, -12);
    }, 

    _navNodeDragStart: function(event)
    {
        var e = event.originalEvent;

        MBE.DnD.currentElement = $(this).parent();
        MBE.DnD.currentElement.addClass('dragged');
        MBE.DnD.isNavNodeDragging = true;

        var ghost = MBE.DnD.createGhost();
        $('body').addClass('dragging');

        e.dataTransfer.setData('text/plain', '...');
        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setDragImage(ghost, -12, -12);
    },

    _uicDragStart: function (event) {
        var e = event.originalEvent;

        MBE.DnD.currentElement = $('.mbe-active', MBE.workspace);
        MBE.DnD.currentElement.addClass('dragged');
        MBE.DnD.isUICDragging = true;

        var ghost = MBE.DnD.createGhost();
        $('body').addClass('dragging');

        e.dataTransfer.setData('text/plain', '...');
        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setDragImage(ghost, -12, -12);
    },

    /*******************************************************/
    /* OVER                                                */
    /*******************************************************/
    _dragOver: function(event)
    {
        event.preventDefault();
        var target = $(this);
        var childs = target.find(' > *');

        if (target.is('.dragged') || target.parents('.dragged').length) {
            event.originalEvent.dataTransfer.effectAllowed = 'none';
            MBE.DnD.placeholder.hide();
            return false;
        }

        if(!childs.length) { // cíl neobsahuje žádné další prvky - vložíme na začátek
            target.append(MBE.DnD.placeholder);
        }
        else { // Vypočítáme pozici
            var y = event.pageY;
        
            for (var i = 0; i < childs.length; i++) {
                var top = childs.eq(i).offset().top;
        
                if (top >= y - 5 && top <= y + 5) { // Umístíme placeholder před element
                    childs.eq(i).before(MBE.DnD.placeholder);
                    break;
                }
                else { // Umístíme placeholder za element
                    childs.eq(i).after(MBE.DnD.placeholder);
                }
            }
        }
        MBE.DnD.placeholder.show();

        return false;
    },

    _navNodeDragOver: function(event)
    {
        event.preventDefault();
        
        var target = $(this);
        
        var y = event.pageY;
        var top = target.offset().top;
        var height = target.outerHeight();

        $('b.drop-target').removeClass('drop-target');

        if (top >= y - 3 && top <= y + 3 && !target.hasClass('root')) {
            target.before(MBE.DnD.placeholder);
        }
        else if(top + height >= y - 3 && top + height <= y + 3 && !target.hasClass('root')) {
            target.after(MBE.DnD.placeholder);
        }
        else {
            target.find('> .sub-tree').append(MBE.DnD.placeholder);
            target.find('> .node-handle b').addClass('drop-target');
        }
        
        return false;
    },
     
    /*******************************************************/
    /* DROP                                                */
    /*******************************************************/
    _workSpaceDrop: function(event)
    {
        event.stopImmediatePropagation();
        event.preventDefault();
            
        var self = MBE.DnD;
        var uic = self.getUIC();

        self.callListeners('onBeforeDrop', uic[0], [self.placeholder.parent()]);
        self.placeholder.after(uic);
        self.callListeners('onDrop', uic[0], [self.placeholder.parent()]);
        self.domNeedUpdate = true;
    },

    _navNodeDrop: function(event)
    {
        event.stopImmediatePropagation();
        event.preventDefault();

        var self = MBE.DnD;
        var target = self.placeholder;
        
        var item = $(self.currentElement);
        var uic;

        if (item.is('.node')) {
            uic = item.find('> .node-handle b').data('targetuic');
        }
        else if (!item.is('[data-uic]')) {
            uic = self.createUIC(item);
        }
        else {
            uic = item;
        }

        self.callListeners('onBeforeDrop', $(uic)[0], [$(uic).parent()]);

        if (target.prev('.node').length) {
            $(target.prev('.node').find('> .node-handle b').data('targetuic')).after(uic);
        }
        else if (target.next('.node').length) {
            $(target.next('.node').find('> .node-handle b').data('targetuic')).before(uic);
        }
        else {
            $(target.parents('.node').eq(0).find('> .node-handle b').data('targetuic')).append(uic);
        }

        self.callListeners('onDrop', $(uic)[0], [$(uic).parent()]);
        self.domNeedUpdate = true;
    },

    /*******************************************************/
    /* COMMON                                              */
    /*******************************************************/
    _dragEnter: function(event)
    {
        event.stopImmediatePropagation();
        if ($(this).is('.dragged') || $(this).parents('.dragged').length) {
            return;
        }
        $(this).addClass('drag-over');
        MBE.DnD.callListeners('onDragEnter', this);
    },

    _dragLeave: function(event)
    {
        event.stopImmediatePropagation();
        if ($(this).is('.dragged') || $(this).parents('.dragged').length) {
            return;
        }
        $(this).removeClass('drag-over');
        MBE.DnD.callListeners('onDragLeave', this);
    },

    _dragEnd: function(event)
    {
        $('body').removeClass('dragging');
        $('.drag-over').removeClass('drag-over');
        $('.drag-over', MBE.workspace).removeClass('drag-over');
        $('.drag-ghost').remove();

        $('#mozaicPageWorkspace').after(MBE.DnD.placeholder);
        MBE.DnD.placeholder.hide();

        if (MBE.DnD.isNavNodeDragging) {
            MBE.DnD.currentElement.removeClass('dragged');
            MBE.DnD.isNavNodeDragging = false;
        }
        if (MBE.DnD.isUICDragging) {
            MBE.DnD.currentElement.removeClass('dragged');
            MBE.DnD.isUICDragging = false;
        }

        MBE.DnD.currentElement = null;
        MBE.DnD.callListeners('onDragEnd', this);

        if (MBE.DnD.domNeedUpdate) {
            MBE.DnD.updateDOM();
        }
    },

    updateDOM: function()
    {
        $('[data-uic]', MBE.workspace).removeClass('empty-element');
        $('[data-uic]:not(input, select, hr, img, .caret, li.divider, .fa, .glyphicon):empty', MBE.workspace).addClass('empty-element');

        MBE.workspace.find('*:not(iframe, script, style, svg, svg *)').contents().filter(function () {
            return this.nodeType == Node.TEXT_NODE && !$(this).parent().hasClass('mbe-text-node') && !$(this).parents('[data-uic="misc|custom-code"]').length;
        }).wrap('<span class="mbe-text-node" />');

        MBE.workspace.find('.has-feedback').each(function() {
            if (!$(this).find('.form-control-feedback').length) {
                $(this).removeClass('has-feedback');
            }
        })

        MBE.DnD.callListeners('onDOMUpdate', MBE.workspace[0]);

        MBE.DnD.domNeedUpdate = false;
    },

    callListeners: function(eventType, context, params)
    {
        for (var i = 0; i < MBE.DnD[eventType].length; i++) {
            var f = MBE.DnD[eventType][i];
            f.apply(context, params ? params : []);
        }
    }
}

MBE.onInit.push(MBE.DnD.init);
MBE.navigator = {

    navBase: null,
    nodeTemplate: '<div class="node"><span class="node-handle" draggable="true"><b></b></span><div class="sub-tree"></div></div>',

    init: function()
    {
        var self = MBE.navigator;

        MBE.DnD.onDOMUpdate.push(self.rebuild);
        MBE.DnD.onDragEnter.push(self.addHighlight);
        MBE.DnD.onDragLeave.push(self.removeHighlight);
        MBE.DnD.onDragEnd.push(self.unHighlight);

        MBE.selection.onSelect.push(self._select);

        self.navBase = $('.tree-nav > .node');
        self.navBase.find('b').data('targetuic', MBE.workspace[0]);
        MBE.workspace.data('navitem', self.navBase.find('b'));

        $(document)
            .on('click', '.tree-nav .fa', self.toggle)
            .on('click', '.tree-nav b', self.selectNode)
            .on('dblclick', '.tree-nav b', self.showOptions)
        ;
    },

    rebuild: function()
    {
        var self = MBE.navigator;

        var root = self.navBase.find('> .sub-tree');
        root.html('');
        
        self.buildSubNodes(MBE.workspace, root);

        if (MBE.workspace.data('navstate') == 'collapsed') {
            root.hide().prev().find('i.fa').toggleClass('fa-caret-down fa-caret-right');
        }
    },

    buildSubNodes: function(node, target) 
    {
        node.children().each(function () {
            var subNode = $(this);
            var item;

            if (subNode.is('[data-uic]')) {
                item = $(MBE.navigator.nodeTemplate);
                var label = item.find('b');

                subNode.data('navitem', label);

                label.html(MBE.getComponentName(this)).data('targetuic', this);

                if (subNode.find('[data-uic]').length) {
                    label.before('<i class="fa fa-caret-down fa-fw"></i>');
                }
                if (subNode.is('[locked]')) {
                    label.after('<span class="fa fa-lock fa-fw"></span>');
                    item.find('.node-handle').attr('draggable', false);
                }
                if (subNode.attr('id') && subNode.attr('id').length) {
                    label.parent().append('<i class="item-id">#' + subNode.attr('id') + '</i>');
                }
                if (subNode.hasClass('mbe-active')) {
                    label.addClass('active');
                }
                target.append(item);

                if (subNode.data('navstate') == 'collapsed') {
                    item.find('i.fa').toggleClass('fa-caret-down fa-caret-right');
                    item.find('.sub-tree').hide();
                }
            }

            MBE.navigator.buildSubNodes(subNode, subNode.is('[data-uic]') ? item.find('.sub-tree') : target);
        });
    },

    toggle: function(event)
    {
        event.stopImmediatePropagation();
        
        $(this).parent().next().slideToggle();
        $(this).toggleClass('fa-caret-right fa-caret-down');
        $($(this).next().data('targetuic')).data('navstate', $(this).hasClass('fa-caret-down') ? 'expanded' : 'collapsed');

        console.log($($(this).next().data('targetuic')));
    },

    selectNode: function(event)
    {
        if (event.stopImmediatePropagation) {
            event.stopImmediatePropagation();
        }
        MBE.selection.select.apply($(this).data('targetuic'), [event]);
    },

    addHighlight: function()
    {
        var target = $(this);
        if(target.data('navitem')) {
            target.data('navitem').addClass('drop-target');
        }
    },

    removeHighlight: function()
    {
        var target = $(this);
        if(target.data('navitem')) {
            target.data('navitem').removeClass('drop-target');
        }
    },

    unHighlight: function()
    {
        MBE.navigator.navBase.find('.drop-target').removeClass('drop-target');
    },

    showOptions: function(event)
    {
        MBE.navigator.selectNode.apply(this, [event]);
        MBE.options.openDialog.apply($(this).data('targetuic'), [event]);
    },

    _select: function()
    {
        MBE.navigator.navBase.find('b').removeClass('active');
        var target = $(this);
        if(target.is('.mbe-active')) {
            target.data('navitem').addClass('active');
        }
    }
};

MBE.onInit.push(MBE.navigator.init);
MBE.clipboard = {

    init: function () {

    },

    isEmpty: function () {
        var data = $.sessionStorage.get('MBEClipboard');
        return data == null || data.length == 0;
    },

    copy: function () {
        var target = $('.mbe-active', MBE.workspace);
        var tmp = $('<div />');
        var clone = target.clone();
        clone.removeClass('mbe-active context-menu-active');

        tmp.append(clone);

        $.sessionStorage.set('MBEClipboard', tmp.html());
    },

    cut: function () {
        this.copy();
        $('.mbe-active', MBE.workspace).remove();
        MBE.selection.select.apply(MBE.workspace, []);
        MBE.DnD.updateDOM();
    },

    paste: function () {
        var data = $.sessionStorage.get('MBEClipboard');
        $('.mbe-active', MBE.workspace).append(data);
        MBE.DnD.updateDOM();
    }
};

MBE.onInit.push(MBE.clipboard.init);
MBE.selection = {
    
    onSelect: [],

    contextItems: {
        copy: { name: 'Copy', icon: 'fa-files-o' },
        cut: { name: 'Cut', icon: 'fa-cut' },
        paste: { name: 'Paste', icon: 'fa-clipboard', disabled: MBE.clipboard.isEmpty },
        properties: { name: 'Properties...', icon: 'fa-cog' },
        delete: { name: 'Delete', icon: 'fa-trash' }
    },

    init: function()
    {
        $(MBE.workspaceDoc)
            .on('click', '[data-uic], body', MBE.selection.select)
        ;

        MBE.DnD.onDOMUpdate.push(MBE.selection._update);
    },

    select: function(event)
    {
        if (typeof event != 'undefined' && event.stopImmediatePropagation) {
            event.stopImmediatePropagation();
        }

        MBE.workspace.find('.mbe-active').removeClass('mbe-active');
        $('.mbe-drag-handle', MBE.workspace).remove();
        MBE.selection.selectElement(this);

        for(var i = 0; i < MBE.selection.onSelect.length; i++) {
            MBE.selection.onSelect[i].apply(this, []);
        }
    },

    selectElement: function(elm)
    {
        elm = $(elm);
        if(elm.is('[data-uic]')) {
            elm.addClass('mbe-active');
            if (!elm.is('[locked]')) {
                var handle = $('<span class="mbe-drag-handle" draggable="true"></span>');
                handle.css({
                    top: elm.offset().top - 4,
                    left: elm.offset().left - 4
                });
                MBE.workspace.append(handle);
            }
        }
    },

    _update: function () {
        var elm = $('.mbe-active', MBE.workspace);
        if (elm.length) {
            $('.mbe-drag-handle', MBE.workspace).css({
                top: elm.offset().top - 4,
                left: elm.offset().left - 4
            });
        }
    },

    _contextAction: function(key, options) {
        switch (key) {
            case 'copy': MBE.clipboard.copy(); break;
            case 'cut': MBE.clipboard.cut(); break;
            case 'paste': MBE.clipboard.paste(); break;
            case 'properties': MBE.options.openDialog.apply($('.mbe-active', MBE.workspace)[0], [{}]); break;
            case 'delete': MBE.deleteItem(); break;
        }
    }
};

MBE.onInit.push(MBE.selection.init);
MBE.path = {
    
    root: null,
    template: '<li><a></a></li>',

    init: function() 
    {
        var self = MBE.path;
        self.root = $('#mozaicPageBreadcrumbs');

        MBE.selection.onSelect.push(MBE.path.update);

        $(document)
            .on('click', '#mozaicPageBreadcrumbs a', self.selectNode)
        ;

    },

    update: function()
    {
        var target = $(this);
        MBE.path.root.find('li:not(:first-child)').remove();

        if(target.is('[data-uic]')) {
            
            $(target.parentsUntil('#mozaicPageWorkspace').get().reverse()).each(function () {
                MBE.path.add(this);
            });
            MBE.path.add(this);
        }
    },

    add: function (elm) {
        if (!$(elm).is('[data-uic]'))
            return;

        var item = $(MBE.path.template);
        item.find('a').html(MBE.getComponentName(elm)).data('targetuic', elm);
        item.appendTo(MBE.path.root);
    },

    selectNode: function(event)
    {
        MBE.selection.select.apply($(this).data('targetuic'), [event]);
    }
};

MBE.onInit.push(MBE.path.init);
MBE.options = {

    target: null,
    targetType: null,
    targetTemplate: null,

    init: function() {
        $(document).on('click', '.dialog-options legend', function () {
            $('.fa', this).toggleClass('fa-caret-down fa-caret-right');
            $(this).nextAll().toggle();
        });
    },
    
    openDialog: function(event)
    {
        if(event.stopImmediatePropagation)
            event.stopImmediatePropagation();

        var self = MBE.options;
        self.target = this;

        var uic = $(this).data('uic').split(/\|/);
        self.targetType = uic[0];
        self.targetTemplate = uic[1];

        var d = $('<div />');
        var onBuild = [];

        if (typeof MBE.types[self.targetType].options[self.targetTemplate] != 'undefined') {
            var optSet = MBE.types[self.targetType].options[self.targetTemplate];
            for (var opt in optSet) {
                d.append(self.createOptions(optSet[opt]));
                if (typeof optSet[opt].onBuild == 'function') {
                    onBuild.push(optSet[opt].onBuild);
                }
            }
        }

        if (typeof MBE.types[self.targetType].options.common != 'undefined') {
            var optSet = MBE.types[self.targetType].options.common;
            for (var opt in optSet) {
                d.append(self.createOptions(optSet[opt]));
            }
        }
        
        for(var opt in self.common) {
            d.append(self.createOptions(self.common[opt]));
        }
        
        d.dialog({
            appendTo: 'body',
            modal: true,
            closeOnEscape: true,
            draggable: false,
            resizable: false,
            width: '70%',
            title: 'Options',
            dialogClass: 'dialog-options',
            close: function () { $(this).remove(); }
        });

        for(var i = 0; i < onBuild.length; i++) {
            onBuild[i]();
        }
    },

    isAllowed: function(opt) {
        if (typeof opt.allowFor != 'undefined' && $.inArray(MBE.options.targetTemplate, opt.allowFor) == -1) {
            return false;
        }
        if (typeof opt.disallowFor != 'undefined') {
            if (typeof opt.disallowFor == 'function') {
                return opt.disallowFor.apply(MBE.options.target, []);
            }
            else if ($.inArray(MBE.options.targetTemplate, opt.disallowFor) != -1) {
                return false;
            }
        }
        return true;
    },

    createCheckBoxList: function(opt, isCollapsed) {
        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var group = $('<div class="option-group"' + (opt.id ? ' id="' + opt.id + '"' : '') + '></div>');
        var set = opt.set;

        for (var k in opt.options) {
            var ch = $('<input type="checkbox" value="' + k + '"' + (opt.get.apply(MBE.options.target, [k, opt]) ? ' checked' : '' ) + '>');
            var lb = $('<label />');

            ch.on('click', function () {
                set.apply(MBE.options.target, [this]);
                MBE.selection._update();
            });

            lb.append(ch)
            lb.append(' ' + opt.options[k]);
            group.append(lb);
            group.append('<br>');
        }

        if (isCollapsed) {
            group.css('display', 'none');
        }

        return group;
    },

    createSelect: function(opt, isCollapsed)
    {
        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var group = $('<div class="option-group form-group"' + (opt.id ? ' id="' + opt.id + '"' : '') + '></div>');
        var set = opt.set;

        var lb = $('<label class="control-label col-xs-2">' + opt.label + '</label>');
        var sel = $('<select class="form-control input-sm"></select>');
        for (var k in opt.options) {
            sel.append('<option value="' + k + '"' + (opt.get.apply(MBE.options.target, [k, opt]) ? ' selected' : '') + '>' + opt.options[k] + '</option>');
        };

        if(typeof opt.attr != 'undefined') {
            sel.data('attr', opt.attr);
        }

        sel.on('change', function () {
            set.apply(MBE.options.target, [this]);
            MBE.selection._update();
        });

        group.append(lb);
        group.append(sel);

        sel.wrap('<div class="col-xs-10"></div>');

        if (isCollapsed) {
            group.css('display', 'none');
        }

        return group;
    },

    createText: function(opt, isCollapsed)
    {
        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var group = $('<div class="option-group form-group"' + (opt.id ? ' id="' + opt.id + '"' : '') + '></div>');
        var set = opt.set;

        var lb = $('<label class="control-label col-xs-2">' + opt.label + '</label>');
        var inp = $('<input type="'+opt.type+'" value="' + opt.get(false, opt) + '" class="form-control input-sm">');
        
        if(typeof opt.attr != 'undefined') {
            inp.data('attr', opt.attr);
        }

        inp.on('change', function () {
            set.apply(MBE.options.target, [this]);
            MBE.selection._update();
        });
        if (typeof opt.change == 'function') {
            var onChange = opt.change;
            inp.on('change', function () {
                onChange.apply(MBE.options.target, [this]);
            });
        }

        group.append(lb);
        group.append(inp);

        inp.wrap('<div class="col-xs-10"></div>');

        if (isCollapsed) {
            group.css('display', 'none');
        }

        return group;
    },

    createCM: function (opt, isCollapsed) {
        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var group = $('<div class="option-group form-group"' + (opt.id ? ' id="' + opt.id + '"' : '') + '></div>');
        var set = opt.set;

        var inp = $('<textarea class="form-control" rows="15">' + opt.get.apply(MBE.options.target, [opt]) + '</textarea>');

        inp.on('change', function () {
            set.apply(MBE.options.target, [this]);
        });
        if (typeof opt.change == 'function') {
            var onChange = opt.change;
            inp.on('change', function () {
                onChange.apply(MBE.options.target, [this]);
                MBE.selection._update();
            });
        }

        group.append(inp);

        if (isCollapsed) {
            group.css('display', 'none');
        }

        setTimeout(function () {
            var cm = CodeMirror.fromTextArea(inp[0], {
                lineNumbers: true,
                lineWrapping: true,
                mode: "htmlmixed",
                autoCloseBrackets: true,
                autoCloseTags: true,
                matchBrackets: true,
                matchTags: true,
                extraKeys: {
                    "Ctrl-Space": "autocomplete"
                },
            });
            cm.on('blur', function (instance) {
                cm.save();
                inp.change();
            });
        }, 10);

        return group;
    },

    createIcon: function (opt, isCollapsed)
    {
        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var group = $('<div class="option-group form-group"' + (opt.id ? ' id="' + opt.id + '"' : '') + '></div>');
        var set = opt.set;

        var lb = $('<label class="col-xs-2 control-label">' + opt.label + '</label>');
       
        var active = '';
        var iconList = {};
        var sSheetList = document.styleSheets;

        for (var f in opt.fontSets)
        {
            var list = $('<div class="icon-list col-xs-12 font-set-' + f + '"></div>');

            for (var sSheet = 0; sSheet < sSheetList.length; sSheet++) {
                var ruleList = document.styleSheets[sSheet].cssRules;
                for (var rule = 0; rule < ruleList.length; rule++) {
                    var text = ruleList[rule].selectorText;
                    var selectors = text ? text.split(/, ?/) : [];
                    for (var si = 0; si < selectors.length; si++)
                    {
                        var selector = selectors[si];
                        var rx = new RegExp('^\.(' + f + '-[^ ]+)::before$');
                        if (m = rx.exec(selector)) {
                            var btn = $('<a href="#"></a>');
                            btn.append('<span class="' + f + ' ' + m[1] + '"></span>');
                            btn.append('<span class="icon-name">' + f + ' ' + m[1] + '</span>');
                            list.append(btn);

                            var test = '.' + f + '.' + m[1];
                            if ($(MBE.options.target).is(test)) {
                                btn.addClass('active');
                                active = f;
                            }
                        }
                    }
                }
            }
            list.on('click', 'a', function () {
                set.apply(MBE.options.target, [this]);
                MBE.selection._update();
                return false;
            });
            iconList[f] = list;
        }

        var search = $('<input type="text" value="" placeholder="Find icon..." class="form-control input-sm">');

        search.on('input', function () {
            var text = this.value;
            if (text.length) {
                $('.icon-list').find('a').hide().each(function () {
                    if ($('.icon-name', this).text().indexOf(text) != -1) {
                        $(this).show();
                    }
                });
            }
            else {
                $('.icon-list').find('a').show();
            }
        });

        var sets = $('<select class="form-control input-sm"></select>');

        for (f in opt.fontSets) {
            var option = $('<option></option>');
            option.val(f).html(opt.fontSets[f]).appendTo(sets);

            if (active == f) {
                option.attr('selected', true);
                iconList[f].show();
            }
            else {
                iconList[f].hide();
            }
        }

        sets.change(function () {
            $('[class*="font-set-"]').hide();
            $('.font-set-' + this.value).show();
        });
        
        group.append(lb);
        group.append(sets);
        group.append(search);
        group.append('<div class="clearfix"></div>');
        for (var f in iconList) {
            group.append(iconList[f]);
        }

        sets.wrap('<div class="col-xs-4"></div>');
        search.wrap('<div class="col-xs-6"></div>');

        if (isCollapsed) {
            group.css('display', 'none');
        }
        
        return group;
    },

    createOptions: function(opt)
    {
        var self = MBE.options;
        var isCollapsed = typeof opt.state != 'undefined' && opt.state == 'collapsed';

        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var f = $('<fieldset />');
        f.append('<legend><span class="fa fa-caret-'+(isCollapsed ? 'right' : 'down')+' fa-fw"></span>' + opt.name + '</legend>');

        switch (opt.type) {
            case 'boolean': {
                f.append(self.createCheckBoxList(opt, isCollapsed));        
                break;
            }
            case 'select': {
                f.append(self.createSelect(opt, isCollapsed));
                break;
            }
            case 'icon': {
                f.append(self.createIcon(opt, isCollapsed));
                break;
            }
            case 'cm': {
                f.append(self.createCM(opt, isCollapsed));
                break;
            }
            case 'group': {
                for(var i = 0; i < opt.groupItems.length; i++) {
                    var item = opt.groupItems[i];
                    switch(item.type) {
                        case 'boolean': {
                            f.append(self.createCheckBoxList(item, isCollapsed));
                            break;
                        }
                        case 'select': {
                            f.append(self.createSelect(item, isCollapsed));
                            break;
                        }
                        case 'number':
                        case 'text': {
                            f.append(self.createText(item, isCollapsed));
                            break;
                        }
                        case 'icon': {
                            f.append(self.createIcon(item, isCollapsed));
                            break;
                        }
                        case 'cm': {
                            f.append(self.createCM(item, isCollapsed));
                            break;
                        }
                        case 'builder':
                        {
                            f.append(item.builder(item, isCollapsed));
                            break;
                        }
                    }
                }
                break;
            }
            case 'builder':
            {
                f.append(opt.builder(opt, isCollapsed));
                break;
            }
        }
        return f;
    },

    hasClass: function(value) {
        return $(MBE.options.target).hasClass(value);
    },

    is: function(tagName) {
        return $(MBE.options.target).is(tagName);
    },

    hasAttr: function (value, opt) {
        var t = $(MBE.options.target);
        if (value !== false) {
            return opt.attr ? t.attr(opt.attr) == value : t.is('[' + value + ']');
        }
        else {
            return t.is('[' + opt.attr + ']') ? t.attr(opt.attr) : '';
        }
    },

    setAttr: function(opt) {
        var attr = $(opt).data('attr');
        var t = $(MBE.options.target);
        if (attr) {
            if (!opt.value.length) {
                t.removeAttr(attr);
            }
            else  {
                t.attr(attr, opt.value == 'null' ? '' : opt.value);
            }
            
            if (attr == 'id') {
                MBE.navigator.rebuild();
            }
        }
        else { // Je to checkbox
            if(opt.checked) {
                t.attr(opt.value, opt.value);
            }
            else {
                t.removeAttr(opt.value);
            }
        }
    },

    hasProp: function (value, opt) {
        var t = $(MBE.options.target);
        if (value !== false) {
            return t.is(':' + value);
        }
        else {
            return t.is(':' + opt.attr);
        }
    },

    setProp: function (opt) {
        var attr = $(opt).data('attr');
        var t = $(MBE.options.target);
        if (attr) {
            t.prop(attr, opt.value == 'null' ? '' : opt.value);
        }
        else { // Je to checkbox
            t.prop(opt.value, opt.checked);
        }
    },

    toggleClass: function (opt) {
        if (opt.tagName.toLowerCase() == 'select') {
            var classes = new Array();
            $(opt).find('option').each(function() {
                classes.push(this.value);
            });
            $(this).removeClass(classes.join(' '));
            if(opt.value == 'null') {
                return;
            }
        }

        $(this).toggleClass(opt.value);
        MBE.selection.select.apply(this, []);
    },

    toggleTagName: function (opt) {
        var newTag = $('<' + opt.value + '></' + opt.value + '>');
        var l = this.attributes.length;

        for (var i = 0; i < l; i++) {
            var nodeName = this.attributes.item(i).nodeName;
            var nodeValue = this.attributes.item(i).nodeValue;

            newTag[0].setAttribute(nodeName, nodeValue);
        }
        newTag.html(this.innerHTML);

        $(this).replaceWith(newTag);
        MBE.options.target = newTag[0];
        MBE.DnD.updateDOM();
    },

    setIcon: function (icon) {
        var elm = $(this);
        var currentClass = $('.icon-list a.active .icon-name').text();
        var newClass = $('.icon-name', icon).text();

        elm.removeClass(currentClass).addClass(newClass);
        $('.icon-list .active').removeClass('active');
        $(icon).addClass('active');
    },

    getCustomClasses: function (value, opt) {
        var c = $(MBE.options.target).attr('data-custom-classes');
        return c ? c : '';
    },

    setCustomClasses: function (opt) {
        var currentClasses = $(this).attr('data-custom-classes');
        if (currentClasses) {
            $(this).removeClass(currentClasses);
        }

        $(this).addClass(opt.value).attr('data-custom-classes', opt.value)
    },

    getCustomAttributes: function (value, opt) {
        var t = $(MBE.options.target);
        var customAttributes = t.attr('data-custom-attributes');
        if (customAttributes && customAttributes.length) {
            var data = new Array();
            var attrList = customAttributes.split(/,/g);
            for (var i = 0; i < attrList.length; i++) {
                var attrName = attrList[i];
                var attrValue = t.attr(attrName);
                
                data.push(attrName + '=' + attrValue + '');
            }
            return data.join(';');
        }
        else {
            return '';
        }
    },

    setCustomAttributes: function (opt) {
        var customAttributes = new Array();
        if (opt.value.length) {
            var data = opt.value.split(/; */g);
            for (var i = 0; i < data.length; i++) {
                var pair = data[i].split(/=/);
                $(this).attr(pair[0], pair.length > 1 ? pair[1] : pair[0]);

                customAttributes.push(pair[0]);
            }
        }
        $(this).attr('data-custom-attributes', customAttributes.join(','));
    }
};

// Obecné vlastnosti - musí být na konci kvůli referencím na set metody
MBE.options.common = {
    common: {
        name: 'Common',
        type: 'group',
        groupItems: [{
            label: 'ID',
            type: 'text',
            attr: 'id',
            id: 'AttrID',
            get: MBE.options.hasAttr,
            set: MBE.options.setAttr
        }, {
            label: 'Style',
            type: 'text',
            attr: 'style',
            get: MBE.options.hasAttr,
            set: MBE.options.setAttr
        }, {
            label: 'Tab index',
            type: 'number',
            attr: 'tabindex',
            get: MBE.options.hasAttr,
            set: MBE.options.setAttr
        }, {
            label: 'Custom classes',
            type: 'text',
            get: MBE.options.getCustomClasses,
            set: MBE.options.setCustomClasses
        }, {
            label: 'Custom attributes',
            type: 'text',
            get: MBE.options.getCustomAttributes,
            set: MBE.options.setCustomAttributes
        }, {
            label: 'Build properties',
            type: 'text',
            attr: 'data-properties',
            get: MBE.options.hasAttr,
            set: MBE.options.setAttr
        }]
    },
    visibility: {
        name: 'Responsive visibility',
        disallowFor: ['input-hidden'],
        type: 'boolean',
        options: {
            'visible-xs-block': 'visible-xs-block',
            'visible-sm-block': 'visible-sm-block',
            'visible-md-block': 'visible-md-block',
            'visible-lg-block': 'visible-lg-block',
            'visible-xs-inline': 'visible-xs-inline',
            'visible-sm-inline': 'visible-sm-inline',
            'visible-md-inline': 'visible-md-inline',
            'visible-lg-inline': 'visible-lg-inline',
            'visible-xs-inline-block': 'visible-xs-inline-block',
            'visible-sm-inline-block': 'visible-sm-inline-block',
            'visible-md-inline-block': 'visible-md-inline-block',
            'visible-lg-inline-block': 'visible-lg-inline-block',
            'hidden-xs': 'hidden-xs',
            'hidden-sm': 'hidden-sm',
            'hidden-md': 'hidden-md',
            'hidden-lg': 'hidden-lg'
        },
        set: MBE.options.toggleClass,
        get: MBE.options.hasClass,
        state: 'collapsed',
    },

    accessibility: {
        name: 'Accessibility',
        disallowFor: ['input-hidden'],
        type: 'boolean',
        options: {
            'show': 'show',
            'hidden': 'hidden',
            'sr-only': 'sr-only'
        },
        set: MBE.options.toggleClass,
        get: MBE.options.hasClass,
        state: 'collapsed'
    }
};

MBE.onInit.push(MBE.options.init);
MBE.toolbar = {

    menu: {},
    toolbar: null,

    init: function () {
        MBE.selection.onSelect.push(MBE.toolbar._select);
        MBE.toolbar.toolbar = $('#mozaicPageContextToolbar .toolbar.pull-left');
    },

    _select: function () {
        var self = MBE.toolbar;
        self.clear();

        var uic = $(this).data('uic');
        if (!uic) {
            return;
        }

        uic = uic.split('|');
        var type = uic[0];
        var template = uic[1];

        if (typeof self.menu[type] == 'undefined' || typeof self.menu[type][template] == 'undefined') {
            return;
        }

        var menu = self.menu[type][template];
        if (typeof menu.allowFor == 'function' && !menu.allowFor.apply(this, []))
            return;

        for (var i = 0; i < menu.items.length; i++) {
            var item = menu.items[i];
            switch (item.type) {
                case 'text': {
                    self.toolbar.append('<p class="navbar-text">' + item.label + '</p>');
                    break;
                }
                case 'button': {
                    var button = $('<button type="button" class="btn navbar-btn">' + item.label + '</button>');
                    self.toolbar.append(button);
                    button.click({callback: item.callback}, function (event) {
                        event.data.callback.apply($('.mbe-active', MBE.workspaceDoc)[0], []);
                    });

                    if (typeof item.allowFor == 'function') {
                        if (!item.allowFor.apply(this, [])) {
                            button.attr('disabled', true);
                        }
                    }
                }
            }
        }
    },

    clear: function () {
        MBE.toolbar.toolbar.html('');
    }

};

MBE.onInit.push(MBE.toolbar.init);
MBE.io = {

    allComponents: null,
    onLoad: [],

    loadPageList: function () {
        $('#page-table tbody tr').remove();
        $('#choose-page-dialog .spinner-2').show();

        var appId = $('#currentAppId').val();
        $.ajax({
            type: "GET",
            url: "/api/mozaic-bootstrap/apps/" + appId + "/pages",
            dataType: "json",
            error: MBE.ajaxError,
            success: function (data) {
                var tbody = $('#page-table tbody');
                for (i = 0; i < data.length; i++) {
                    var row = $('<tr class="pageRow">');
                    row.attr({
                        'data-id': data[i].Id,
                        'data-isbootstrap': data[i].IsBootstrap
                    });
                    row.append('<td width="95%">' + data[i].Name + '</td>');
                    row.append('<td width="5%" align="center">' + (data[i].IsBootstrap ? 'yes' : 'no') + '</td>');
                    row.appendTo(tbody);
                }

                $('#choose-page-dialog .spinner-2').hide();
            }
        });
    },

    loadDeletedPageList: function () {
        $('#trash-page-table tbody tr').remove();
        $('#trash-page-dialog .spinner-2').show();

        var appId = $("#currentAppId").val();
        $.ajax({
            type: "GET",
            url: "/api/mozaic-bootstrap/apps/" + appId + "/deletedPages",
            dataType: "json",
            error: MBE.ajaxError,
            success: function (data) {
                var tbody = $('#trash-page-table tbody');
                for (i = 0; i < data.length; i++) {
                    var row = $('<tr class="pageRow"></tr>');
                    row.attr({
                        'data-id': data[i].Id,
                        'data-isbootstrap': data[i].IsBootstrap
                    });
                    row.append('<td width="95%">' + data[i].Name + '</td>');
                    row.append('<td width="5%" align="center">' + (data[i].IsBootstrap ? 'yes' : 'no') + '</td>');
                    row.appendTo(tbody);
                }
                $("#trash-page-dialog .spinner-2").hide();
            }
        });
    },

    loadPage: function () {
        var selected = $('#page-table tr.highlightedRow');
        if (selected.length) {
            $('#choose-page-dialog').dialog('close');
            var pageId = selected.attr('data-id');
            if (selected.attr('data-isbootstrap') == 'true') {
                MBE.io.loadBootstrapPage(pageId);
            }
            else {
                MBE.io.loadLegacyPage(pageId);
            }
        }
        else {
            alert('Please select a page');
        }
    },

    loadDeletedPage: function () {
        var selected = $('#trash-page-table tr.highlightedRow');
        if (selected.length) {
            $('#trash-page-dialog').dialog('close');
            var pageId = selected.attr('data-id');
            if (selected.attr('data-isbootstrap') == 'true') {
                MBE.io.loadBootstrapPage(pageId);
            }
            else {
                MBE.io.loadLegacyPage(pageId);
            }
        }
        else {
            alert('Please select a page');
        }
    },

    reloadPage: function () {
        if ($('#currentPageVersion').val() == 'Legacy') {
            MBE.io.loadLegacyPage('current');
        }
        else {
            MBE.io.loadBootstrapPage('current');
        }
    },

    loadLegacyPage: function (pageId) {
        pageSpinner.show();
        pageId = pageId == "current" ? $('#currentPageId').val() : pageId;

        var appId = $('#currentAppId').val();
        var url = "/api/mozaic-editor/apps/" + appId + "/pages/" + pageId;

        $.ajax({
            type: "GET",
            url: url,
            dataType: "json",
            complete: function () { pageSpinner.hide() },
            error: MBE.ajaxError,
            success: function (data) { MBE.io.convert(data); }
        });
    },

    loadBootstrapPage: function (pageId) {
        pageSpinner.show();
        pageId = pageId == "current" ? $('#currentPageId').val() : pageId;

        var appId = $('#currentAppId').val();
        var url = "/api/mozaic-bootstrap/apps/" + appId + "/pages/" + pageId;

        $.ajax({
            type: "GET",
            url: url,
            dataType: "json",
            complete: function () { pageSpinner.hide() },
            error: MBE.ajaxError,
            success: MBE.io.drawBootstrapPage
        });
    },

    createPage: function () {
        pageSpinner.show();
        $('#new-page-dialog').dialog('close');

        var appId = $('#currentAppId').val();
        var newPageName = $('#new-page-name').val();
        var postData = {
            Name: newPageName,
            Content: '',
            IsDeleted: false,
            Components: []
        };

        $.ajax({
            type: "POST",
            url: "/api/mozaic-bootstrap/apps/" + appId + "/pages",
            data: postData,
            error: MBE.ajaxError,
            success: function (data) {
                $('#currentPageId').val(data);
                $('#currentPageVersion').val('Bootstrap');
                $('#headerPageName').text(newPageName == '' ? 'Nepojmenovaná stránka' : newPageName);

                if (MBE.saveRequested) {
                    MBE.io.doSave();
                }
                else {
                    pageSpinner.hide();
                }
            }
        });
    },

    deletePage: function () {
        var selected = $('#page-table tr.highlightedRow');
        if (selected.length) {
            if (confirm('Are you sure you want to delete this page?')) {
                pageSpinner.show();
                var appId = $('#currentAppId').val();
                var pageId = selected.attr('data-id');
                var api = selected.attr('data-isbootstrap') == 'true' ? 'mozaic-bootstrap' : 'mozaic-editor';

                $.ajax({
                    type: "POST",
                    url: '/api/' + api + '/apps/' + appId + '/pages/' + pageId + '/delete',
                    complete: function () {
                        pageSpinner.hide();
                    },
                    success: function () {
                        alert("OK. Page deleted.");
                        $('#choose-page-dialog').dialog('close');
                        // Clear workspace, but only when deleted page is currently opened
                        if ($('#currentPageId').val() == pageId) {
                            MBE.clearWorkspace();
                            $('#headerPageName').html('');
                            $('#currentPageId').val('');
                        }
                    },
                    error: MBE.ajaxError
                });
            }
        }
        else {
            alert('Please select a page to delete.');
        }
    },

    rename: function () {
        $('#headerPageName').text($(this).find('#page-name').val());
        $('#rename-page-dialog').dialog('close');
        MBE.changedSinceLastSave = true;
    },

    savePage: function () {
        var pageId = Number($('#currentPageId').val());
        if (!pageId || isNaN(pageId)) {
            MBE.saveRequested = true;
            MBE.dialogs.newPage();
        }
        else {
            MBE.io.doSave();
        }
    },

    doSave: function () {
        pageSpinner.show();
        MBE.saveRequested = false;

        var postData = {
            Name: $('#headerPageName').text(),
            Content: MBE.io.getPageDOM(),
            IsDeleted: false,
            Components: MBE.io.getComponentsArray(MBE.workspace.clone(true))
        };

        var appId = $('#currentAppId').val();
        var pageId = $('#currentPageId').val();

        $.ajax({
            type: "POST",
            url: "/api/mozaic-bootstrap/apps/" + appId + "/pages/" + pageId,
            data: postData,
            complete: function () {
                pageSpinner.hide();
            },
            success: function () { alert("OK") },
            error: MBE.ajaxError
        });
    },

    getPageDOM: function () {
        var dom = $(MBE.workspace).clone(true);

        dom.find('.dataTables_wrapper').each(function () {
            $(this).replaceWith($('table', this));
        });

        return dom.html();
    },

    getComponentsArray: function (parent) {
        var components = new Array();

        parent.find('.dataTables_wrapper').each(function () {
            $(this).replaceWith($('table', this));
        });

        var uicList = parent.find('> [data-uic]');

        for (var i = 0; i < uicList.length; i++) {
            var node = uicList.eq(i);
            var component = {
                ElmId: node.attr('id') ? node.attr('id') : '',
                Tag: node[0].tagName.toLowerCase(),
                UIC: node.attr('data-uic'),
                Attributes: MBE.io.filterAttrs(MBE.io.getAttrs(node[0])),
                Properties: node.attr('data-properties') ? node.attr('data-properties') : '',
                Content: MBE.io.filterContent(node),
                ChildComponents: MBE.io.getComponentsArray(node)
            };
            components.push(component);
        }

        return components;
    },

    drawBootstrapPage: function(data)
    {
        MBE.clearWorkspace();
        MBE.workspace.append(data.Content);

        for (var i = 0; i < MBE.io.onLoad.length; i++) {
            MBE.io.onLoad[i]();
        }

        MBE.DnD.updateDOM();

        $('#currentPageId').val(data.Id);
        $('#currentPageVersion').val('Bootstrap');
        $('#headerPageName').text(data.Name);
    },

    /*************************************************************/
    /* CONVERSION METHODS                                        */
    /*************************************************************/
    convert: function (data) {
        if (!data.Id) {
            return;
        }
        MBE.io.allComponents = new Array();
        MBE.clearWorkspace();

        var container = $(MBE.types.containers.templates['container']);
        container.attr('data-uic', 'containers|container').appendTo(MBE.workspace);

        var form = $(MBE.types.form.templates.form);
        form.attr('data-uic', 'form|form').appendTo(container);

        for (var i = 0; i < data.Components.length; i++) {
            MBE.io.convertComponent(form, data.Components[i]);
        }

        // Pokusíme se naskládat inputy ke správným labelům

        var boundInputs = [];
        for (var i = 0; i < MBE.io.allComponents.length; i++) {
            var c1 = MBE.io.allComponents[i];
            if (c1.item && c1.item.is('[data-uic="form|label"]')) {
                var mostLikelyInput = null;

                c1.PositionX = parseInt(c1.PositionX);
                c1.PositionY = parseInt(c1.PositionY);
                c1.Width = parseInt(c1.Width);

                for (var j = 0; j < MBE.io.allComponents.length; j++) {
                    var c2 = MBE.io.allComponents[j];

                    if (c2.item.is('input:not([type=radio],[type=checkbox]), textarea, select, .form-control-static') && $.inArray(c2.Id, boundInputs) === -1) {
                        c2.PositionX = parseInt(c2.PositionX);
                        c2.PositionY = parseInt(c2.PositionY);

                        if (c1.PositionY + 5 >= c2.PositionY && c1.PositionY - 5 <= c2.PositionY) {
                            if (mostLikelyInput == null) {
                                if (c2.PositionX > (c1.PositionX + c1.Width)) {
                                    mostLikelyInput = c2;
                                }
                            }
                            else {
                                if (c2.PositionX < mostLikelyInput.PositionX && c2.PositionX > (c1.PositionX + c1.Width)) {
                                    mostLikelyInput = c2;
                                }
                            }
                        }
                    }
                }

                if (mostLikelyInput != null) {
                    c1.item.next().append(mostLikelyInput.item);
                    c1.item.attr('for', mostLikelyInput.Name);
                    boundInputs.push(mostLikelyInput.Id);
                }
            }
        }

        $('#currentPageId').val(data.Id);
        $('#currentPageVersion').val('Legacy');
        $('#headerPageName').text(data.Name);

        MBE.DnD.updateDOM();
    },

    convertDiv: function (nc, c) {
        if (c.Classes.indexOf('panel-component') !== -1) {
            nc.item = $(MBE.types.containers.templates.panel);
            nc.item
                .find('.panel-body').html('').end()
                .find('.panel-footer').remove().end()
                .attr('data-uic', 'containers|panel');

            if (c.Name) {
                nc.item.attr('id', c.Name);
            }
            if (c.Classes.indexOf('named-panel') === -1) {
                nc.item.find('.panel-heading').remove();
            }
            else {
                nc.item.find('.panel-title').html(c.Label);
            }
            nc.newTarget = nc.item.find('.panel-body');
        }
        if (c.Classes.indexOf('control-label') !== -1) {
            if (c.Label == '{var1}') { // Pravděpodobně je to statický text ve formuláři
                nc.item = $(MBE.types.form.templates['static-control']);
                nc.item
                    .html(c.Label)
                    .attr('data-uic', 'form|static-control');
            }
            else {
                nc.item = $(MBE.types.form.templates.label);
                nc.item
                    .html(c.Label)
                    .attr('data-uic', 'form|label')
                    .addClass('col-sm-2');
                nc.item.wrap('<div class="form-group" data-uic="form|form-group"></div>');
                nc.item.after('<div class="col-sm-10" data-uic="grid|column"></div>');
            }
        }
        if (c.Classes.indexOf('form-heading') !== -1) {
            nc.item = $('<h2 data-uic="text|heading">' + c.Label + '</h2>');
        }
        if (c.Classes.indexOf('checkbox-control') !== -1) {
            nc.item = $(MBE.types.form.templates['checkbox']);
            nc.item.attr({ name: c.Name, 'data-uic': 'form|checkbox' });
            nc.item.wrap('<label for="' + c.Name + '" data-uic="form|label"></label>');
            nc.item.parent().append(' ' + c.Label);
            nc.item.parent().wrap('<div class="checkbox" data-uic="form|checkbox-group"></div>');
            nc.item.parent().parent().wrap('<div class="col-sm-10 col-sm-push-2" data-uic="grid|column"></div>');
            nc.item.parent().parent().parent().wrap('<div class="form-group" data-uic="form|form-group"></div>');
        }
        if (c.Classes.indexOf('radio-control') !== -1) {
            nc.item = $(MBE.types.form.templates['radio']);
            nc.item.attr({ name: c.Name, 'data-uic': 'form|radio' });
            nc.item.wrap('<label for="' + c.Name + '" data-uic="form|label"></label>');
            nc.item.parent().append(' ' + c.Label);
            nc.item.parent().wrap('<div class="radio" data-uic="form|radio-group"></div>');
            nc.item.parent().parent().wrap('<div class="col-sm-10 col-sm-push-2" data-uic="grid|column"></div>');
            nc.item.parent().parent().parent().wrap('<div class="form-group" data-uic="form|form-group"></div>');
        }
        if (c.Classes.indexOf('info-container') !== -1) {
            nc.item = $(MBE.types.containers.templates.div);
            nc.item.attr('data-uic', 'containers|div');
            nc.item.append('<div class="info-container-header" data-uic="containers|div"><span class="fa fa-info-circle" data-uic="image|icon"></span>' + c.Label + '</div>');
            nc.item.append('<div class="info-container-body" data-uic="containers|div">' + c.Content + '</div>');
        }
        if (c.Classes.indexOf("breadcrumb-navigation") !== -1) {

            nc.item = $(MBE.types.misc.templates.breadcrumbs);
            nc.item.attr('data-uic', 'misc|breadcrumbs');

            var item = $(MBE.types.misc.templates['breadcrumbs-item']);
            var itemActive = $(MBE.types.misc.templates['breadcrumbs-active']);
            var itemInactive = $(MBE.types.misc.templates['breadcrumbs-inactive']);

            var i1 = item.clone();
            i1.append(itemActive.clone());
            i1.find('> span').addClass('fa fa-question app-icon').end().appendTo(nc.item);

            var i2 = item.clone();
            i2.append(itemInactive.clone());
            i2.find('> a').html('APP NAME').end().appendTo(nc.item);

            var i3 = item.clone();
            i3.append(itemActive.clone());
            i3.find('> span').html('Nav').end().appendTo(nc.item);
        }
        if (c.Classes.indexOf('countdown-component') !== -1) {
            nc.item = $(MBE.types.ui.templates.countdown);
        }
        if (c.Classes.indexOf('wizard-phases') !== -1) {
            nc.item = $(MBE.types.ui.templates.wizzard);
            nc.item.attr({
                'data-phases': c.Content,
                'data-activephase': MBE.io.getProperty('ActivePhase', c.Properties)
            });
            nc.afterAppend = MBE.types.ui.buildWizzard;
        }
    },

    convertSelect: function (nc, c) {
        nc.item = $(MBE.types.form.templates.select);
        nc.item.attr({
            'data-uic': 'form|select',
            'name': c.Name
        });
        if (c.Classes.indexOf('multiple-select') !== -1) {
            nc.item.attr('multiple', 'multiple');
        }

        if (c.Properties && c.Properties.indexOf('defaultoption') !== -1) {
            var defaultOption = MBE.io.getProperty('defaultoption', c.Properties);
            nc.item.append('<option value="">' + defaultOption + '</option>');
        }
    },

    convertInput: function (nc, c) {
        if (c.Classes.indexOf('input-single-line') !== -1) {
            nc.item = $(MBE.types.form.templates['input-text']);
            nc.item.attr('data-uic', 'form|input-text');
        }
        if (c.Classes.indexOf('button-browse') !== -1) {
            nc.item = $(MBE.types.form.templates['input-file']);
            nc.item.attr('data-uic', 'form|input-file');
        }
        if (c.Classes.indexOf('color-picker') !== -1) {
            nc.item = $(MBE.types.form.templates['input-color']);
            nc.item.attr({
                'name': c.Name,
                'data-uic': 'form|input-color'
            });
        }

        nc.item.attr({ name: c.Name });
    },

    convertTextArea: function (nc, c) {
        nc.item = $(MBE.types.form.templates.textarea);
        nc.item.attr({
            'data-uic': 'form|textarea',
            'name': c.Name,
            'rows': 5
        });
    },

    convertButton: function (nc, c) {
        nc.item = $(MBE.types.controls.templates.button);
        nc.item.attr({
            'data-uic': 'controls|button',
            'type': 'submit',
            'name': c.Name
        }).html(c.Label).addClass('btn-primary');

        if (c.Classes.indexOf('button-large') !== -1) {
            nc.item.addClass('btn-lg');
        }
        if (c.Classes.indexOf('button-small') !== -1) {
            nc.item.addClass('btn-sm');
        }
        if (c.Classes.indexOf('button-extra-small') !== -1) {
            nc.item.addClass('btn-xs');
        }

        if (c.Classes.indexOf('button-dropdown') !== -1) {
            nc.item.addClass('dropdown-toggle').append(' <span class="caret"></span>');
            nc.item.wrap('<div class="btn-group" data-uic="controls|button-dropdown"></div>');
            nc.item.parent().append('<ul class="dropdown-menu" data-uic="controls|dropdown-menu" locked></ul>');
        }
    },

    convertTable: function (nc, c) {
        if (c.Classes.indexOf('data-table') !== -1) {
            nc.item = $(MBE.types.ui.templates['data-table']);
            nc.afterAppend = MBE.types.ui.buildDataTable;

            nc.item.attr('data-uic', 'ui|data-table');
            if (c.Classes.indexOf('data-table-simple-mode') !== -1) {
                nc.item.attr({
                    'data-dtpaging': '0',
                    'data-dtinfo': '0',
                    'data-dtfilter': '0',
                    'data-dtordering': '0'
                });
            }
            if (c.Classes.indexOf('data-table-with-actions') !== -1) {
                var actions = MBE.io.getProperty('actions', c.Properties);
                actions = actions && actions.length > 0 ? actions.split(/-/) : ['edit', 'detail', 'delete'];

                var actionList = [];
                for (var i = 0; i < actions.length; i++) {
                    var a = null;
                    switch (actions[i]) {
                        case 'edit': a = { icon: 'fa fa-edit', action: 'edit', title: 'upravit' }; break;
                        case 'details': a = { icon: 'fa fa-search', action: 'details', title: 'detail' }; break;
                        case 'delete': a = { icon: 'fa fa-remove', action: 'delete', title: 'smazat' }; break;
                        case 'download': a = { icon: 'fa fa-download', action: 'download', title: 'stáhnout' }; break;
                        case 'enter': a = { icon: 'fa fa-sign-in', action: 'enter', title: 'vstoupit' }; break;
                    }
                    if (a !== null) {
                        actionList.push(a);
                    }
                }
                nc.item.attr('data-actions', JSON.stringify(actionList).replace(/"/g, "'"));
            }
        }
        if (c.Classes.indexOf('name-value-list') !== -1) {
            nc.item = $(MBE.types.ui.templates['nv-list']);
            nc.afterAppend = MBE.types.ui.buildNVList;
        }
    },

    convertUL: function (nc, c) {
        if (c.Classes.indexOf('tab-navigation') !== -1) {
            var tabLabelArray = c.Content.split(";");
            tabLabelArray.unshift('<span class="fa fa-home" data-uic="image|icon"></span>');

            nc.item = $(MBE.types.containers.templates.tabs);
            nc.item.attr('data-uic', 'containers|tabs');

            MBE.types.containers.buildTabs.apply(nc.item[0], tabLabelArray);
        }
    },

    convertComponent: function (targetContainer, c) {
        var self = MBE.io;
        var nc = {
            item: null,
            newTarget: null,
            afterAppend: null
        };
        var systemClasses = [
            'panel-component', 'named-panel', 'control-label', 'dropdown-select', 'input-single-line', 'button-browse',
            'input-multiline', 'uic', 'button-simple', 'button-large', 'button-small', 'button-extra-small', 'data-table'
        ];

        switch (c.Tag.toLowerCase()) {
            case 'div':
                self.convertDiv(nc, c);
                break;
            case 'select':
                self.convertSelect(nc, c);
                break;
            case 'input':
                self.convertInput(nc, c);
                break;
            case 'textarea':
                self.convertTextArea(nc, c);
                break;
            case 'button':
                self.convertButton(nc, c);
                break;
            case 'table':
                self.convertTable(nc, c);
                break;
            case 'ul':
                self.convertUL(nc, c);
                break;
        }

        var item = nc.item;
        var newTarget = nc.newTarget;

        if (item) {
            var systemClassesRegExp = new RegExp('(' + systemClasses.join('( |$))|(') + '( |$))', 'g');
            var customClass = c.Classes.replace(systemClassesRegExp, '');
            customClass = customClass.replace(/ {2,}/, ' ').replace(/(^ )|( $)/g, '');

            if (customClass.length) {
                item.attr('data-custom-classes', customClass).addClass(customClass);
            }
            if (c.Placeholder) {
                item.attr("placeholder", c.Placeholder);
            }
            if (c.TabIndex) {
                item.attr("tabindex", c.TabIndex);
            }
            if (c.Attributes) {
                var customAttributes = [];
                var fake = $('<span ' + c.Attributes + '></span>')[0];

                for (var i = 0; i < fake.attributes.length; i++) {
                    var attr = fake.attributes[i];
                    customAttributes.push(attr.name);

                    item.attr(attr.name, attr.value);
                }

                item.attr('data-custom-attributes', customAttributes.join(','));
            }
            if (c.Properties) {

                var propList = c.Properties.split(/; */g);
                var newPropList = [];
                for (var i = 0; i < propList.length; i++) {
                    var pair = propList[i].split(/=/);
                    if (pair[0] == 'defaultoption') {
                        continue;
                    }
                    newPropList.push(pair[0] + '=' + pair[1]);
                }

                item.attr('data-properties', newPropList.join(';'));
            }

            item.attr('id', c.Name);

            if (item.parents('.form-group').length) {
                item.parents('.form-group').appendTo(targetContainer);
            }
            else if (item.parents('.btn-group').length) {
                item.parents('.btn-group').appendTo(targetContainer);
            }
            else {
                item.appendTo(targetContainer);
            }

            if (typeof nc.afterAppend == 'function') {
                nc.afterAppend.apply(item[0], []);
            }
        }

        if (c.ChildComponents) {
            for (j = 0; j < c.ChildComponents.length; j++) {
                self.convertComponent(newTarget ? newTarget : item, c.ChildComponents[j]);
            }
        }

        c.item = item;
        self.allComponents.push(c);
    },

    /************************************************/
    /* TOOLS                                        */
    /************************************************/
    getProperty: function (name, properties) {
        var propList = properties ? properties.split(/; */g) : [];
        for (var i = 0; i < propList.length; i++) {
            var pair = propList[i].split(/=/);
            if (pair[0] == name) {
                return pair[1];
            }
        }
        return '';
    },

    getAttrs: function (el) {
        return [].slice.call(el.attributes).map(function(attr) {
            var attrs = {
                name: attr.name,
                value: attr.value
            };
            return attrs;
        });
    },

    filterAttrs: function (attrs) {
        var finalAttrs = new Array();

        for (var i = 0; i < attrs.length; i++) {
            if ($.inArray(attrs[i].name, ['data-uic', 'data-custom-classes', 'data-custom-attributes', 'data-properties', 'locked']) != -1) {
                continue;
            }
            finalAttrs.push(attrs[i]);
        }

        return JSON.stringify(finalAttrs);
    },

    filterContent: function (node) {
        var tmpNode = node.clone(true);

        tmpNode.find('span.mbe-text-node').each(function () {
            $(this).replaceWith(this.innerHTML);
        });
        var uicNumOrder = 1;
        tmpNode.find('> [data-uic]').each(function () {
            $(this).replaceWith('__UIC_' + uicNumOrder + '__');
            uicNumOrder++;
        });

        if (tmpNode.is('[data-uic^=athena]')) {
            tmpNode.html('');
            tmpNode.append('__AthenaHTML____AthenaCSS____AthenaJS__');
        }
        tmpNode.find('> .uic-embed-preview').remove();
        tmpNode.find('> .embed-code').each(function () {
            $(this).replaceWith($(this).text());
        });

        return tmpNode.html();
    }
};

MBE.history = {

    undoStack: [],
    redoStack: [],

    btnUndo: null,
    btnRedo: null,

    init: function() 
    {
        var self = MBE.history;
        self.btnUndo = $('[data-action="undo"]');
        self.btnRedo = $('[data-action="redo"]');

        MBE.DnD.onBeforeDrop.push(self._beforeAction);
        MBE.onBeforeDelete['*'].push(self._beforeAction);

        $(document)
            .on('click', '[data-action="undo"]', self.undo)
            .on('click', '[data-action="redo"]', self.redo)
        ;

        self.setButtonState();
    },

    _beforeAction: function()
    {
        var self = MBE.history;
        self.redoStack = [];
        self.undoStack.unshift(self.getCurrentDOM());

        if (self.undoStack.length > 20) {
            delete self.undoStack[20];
        }

        self.setButtonState();
    },

    getCurrentDOM: function() {
        var dom = MBE.workspace.clone(true);
        dom
            .find('#drop-placeholder').remove().end()
            .find('.drag-over').removeClass('drag-over');

        return dom.contents();
    },

    setButtonState: function() {
        var self = MBE.history;

        self.btnUndo.attr('disabled', self.undoStack.length == 0);
        self.btnRedo.attr('disabled', self.redoStack.length == 0);
    },

    undo: function () {
        var self = MBE.history;
        var dom = self.undoStack.shift();

        self.redoStack.unshift(self.getCurrentDOM());

        MBE.workspace.html('');
        MBE.workspace.append(dom);

        MBE.DnD.updateDOM();
        self.setButtonState();
    },

    redo: function () {
        var self = MBE.history;
        var dom = self.redoStack.shift();

        self.undoStack.unshift(self.getCurrentDOM());

        MBE.workspace.html('');
        MBE.workspace.append(dom);

        MBE.DnD.updateDOM();
        self.setButtonState();
    }
};

MBE.onInit.push(MBE.history.init);
MBE.dialogs = {

    choosePage: function()
    {
        $('#choose-page-dialog').dialog({
            autoOpen: true,
            width: 700,
            height: 540,
            buttons: {
                Open: MBE.io.loadPage,
                Delete: MBE.io.deletePage,
                Cancel: MBE.dialogs.close
            },
            open: MBE.io.loadPageList
        });
    },

    trash: function()
    {
        $('#trash-page-dialog').dialog({
            autoOpen: true,
            width: 700,
            height: 540,
            buttons: {
                Load: MBE.io.loadDeletedPage,
                Cancel: MBE.dialogs.close
            },
            open: MBE.io.loadDeletedPageList
        });
    },

    newPage: function()
    {
        $('#new-page-dialog').dialog({
            autoOpen: true,
            width: 400,
            height: 170,
            buttons: {
                Save: MBE.io.createPage,
                Cancel: MBE.dialogs.close
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        MBE.io.createPage;
                        return false;
                    }
                })
            },
            open: function () { $('#new-page-name').val(''); }
        });
    },

    rename: function() {
        $('#rename-page-dialog').dialog({
            autoOpen: true,
            width: 400,
            height: 190,
            buttons: {
                Save: MBE.io.rename,
                Cancel: MBE.dialogs.close
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        MBE.io.rename.apply(document.getElementById('rename-page-dialog'), []);
                        return false;
                    }
                })
            },
            open: function () {
                $(this).find('#page-name').val($('#headerPageName').text());
            }
        });
    },

    close: function () {
        $(this).dialog("close");
    }
};
MBE.context = {

    defaultSettings: {
        trigger: 'right',
        zIndex: 300
    },

    init: function () {

        var ds = this.defaultSettings;

        MBE.workspace.contextMenu($.extend(ds, {
            selector: '.mbe-active', 
            callback: MBE.selection._contextAction, 
            items: MBE.selection.contextItems,
            position: function (opt, x, y) {
                opt.$menu.css({
                    top: y + $('#mozaicPageWorkspace > iframe').offset().top,
                    left: x + $('#mozaicPageWorkspace > iframe').offset().left
                }); 
            }
        }));

        $.contextMenu($.extend(ds, {
            selector: '.tree-nav .node-handle b',
            callback: MBE.selection._contextAction,
            items: MBE.selection.contextItems,
            events: {
                show: function (options) {
                    MBE.navigator.selectNode.apply(this, [{}]);
                }
            }
        }));
    }
};

MBE.onInit.push(MBE.context.init);
MBE.types.containers = {

    templates: {
        'container': '<div class="container"></div>',
        'panel': '<div class="panel panel-default">' +
                    '<div class="panel-heading" data-uic="containers|panel-heading" locked>' +
                        '<h3 class="panel-title" data-uic="text|heading">Panel title</h3>' +
                    '</div>' +
                    '<div class="panel-body" data-uic="containers|panel-body" locked>' +
                        '<p data-uic="text|paragraph">Panel body</p>' +
                    '</div>' +
                    '<div class="panel-footer" data-uic="containers|panel-footer" locked>' + 
                        '<span data-uic="text|span">Panel footer</span>' +
                    '</div>' +
                '</div>',
        'panel-heading': '<div class="panel-heading" data-uic="containers|panel-heading" locked><span data-uic="text|span">Panel heading</span></div>',
        'panel-body': '<div class="panel-body" data-uic="containers|panel-body" locked><p data-uic="text|paragraph">Panel body</p></div>',
        'panel-footer': '<div class="panel-footer" data-uic="containers|panel-footer" locked><span data-uic="text|span">Panel footer</span></div>',
        'tabs': '<div></div>',
        'tab': '<li data-uic="containers|tab"><a href="" data-uic="controls|link" data-toggle="tab" locked></a></li>',
        'tab-items': '<ul class="nav nav-tabs" data-uic="containers|tab-items" locked></ul>',
        'tab-content': '<div class="tab-content" data-uic="containers|tab-content" locked></div>',
        'tab-pane': '<div class="tab-pane" data-uic="containers|tab-pane"><p data-uic="text|paragraph">Tab content</p></div>',
        'accordion': '<div class="panel-group"></div>',
        'well': '<div class="well"><span data-uic="text|span">Text of the well</span></div>',
        'list': '<ul><li data-uic="containers|list-item">Item 1</li><li data-uic="containers|list-item">Item 2</li></ul>',
        'list-item': '<li data-uic="containers|list-item">Item</li>',
        'div': '<div></div>',
        'list-group': '<ul class="list-group"></ul>',
        'list-group-div': '<div class="list-group"></div>',
        'list-group-item':  '<li class="list-group-item" data-uic="containers|list-group-item">List Group Item</li>',
        'list-group-item-link': '<a class="list-group-item" data-uic="containers|list-group-item-link">List Group Item</a>',
        'list-group-item-button': '<button type="button" class="list-group-item" data-uic="containers|list-group-item-button">List Group Item</button>',
        'dl-dl': '<dl></dl>',
        'dl-dt': '<dt data-uic="containers|dl-dt">item</dt>',
        'dl-dd': '<dd data-uic="containers|dl-dd">definition</dd>'
    },
    
    options: {
        'container': {
            'containerOptions': {
                name: 'Container options',
                type: 'boolean',
                options: {
                    'container-fluid': 'Fluid'
                },
                set: function (opt) {
                    $(this).toggleClass('container container-fluid');
                },
                get: MBE.options.hasClass
            }
        },
        'panel': {
            'panelOptions': {
                name: 'Panel options',
                type: 'group',
                groupItems: [{
                    label: 'Style',
                    type: 'select',
                    options: {
                        'panel-default': 'Default',
                        'panel-primary': 'Primary',
                        'panel-success': 'Success',
                        'panel-info': 'Info',
                        'panel-warning': 'Warning',
                        'panel-danger': 'Danger',
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Show',
                    type: 'boolean',
                    options: {
                        'panel-heading': 'Show panel heading',
                        'panel-body': 'Show panel body',
                        'panel-footer': 'Show panel footer'
                    },
                    get: function (value) {
                        return $('> .' + value, this).length > 0;
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            switch (opt.value) {
                                case 'panel-heading': $(this).prepend(MBE.types.containers.templates['panel-heading']); break;
                                case 'panel-footer': $(this).append(MBE.types.containers.templates['panel-footer']); break;
                                case 'panel-body':
                                    if ($('> .panel-heading', this).length) {
                                        $('> .panel-heading', this).after(MBE.types.containers.templates['panel-body']);
                                    }
                                    else {
                                        $(this).prepend(MBE.types.containers.templates['panel-body']);
                                    }
                                    break;
                            }
                        }
                        else {
                            $('> .' + opt.value, this).remove();
                        }
                        MBE.DnD.updateDOM();
                    }
                }]
            }
        },
        'tab-items': {
            'tabItemsOptions': {
                name: 'Tab items options',
                type: 'group',
                groupItems: [{
                    label: 'Justified',
                    type: 'boolean',
                    options: {
                        'nav-justified': 'Justified'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'tab-content': {
            'tabContentOptions': {
                name: 'Tab content options',
                type: 'group',
                groupItems: [{
                    label: 'Fade',
                    type: 'boolean',
                    options: {
                        'fade': 'Fade'
                    },
                    get: function () {
                        return $('> .fade', MBE.options.target).length;
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            $('> .tab-pane', this).addClass('fade').filter('.active').addClass('in');
                        }
                        else {
                            $('> .tab-pane', this).removeClass('fade in');
                        }
                    }
                }],
            }
        },
        'well': {
            'wellOptions': {
                name: 'Well options',
                type: 'group',
                groupItems: [{
                    label: 'Size',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'well-lg': 'Large',
                        'well-sm': 'Small'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'list': {
            'listOptions': {
                name: 'List options',
                type: 'group',
                groupItems: [{
                    id: 'ListStyle',
                    label: 'Style',
                    type: 'select',
                    options: {
                        'ul': 'Unordered (UL)',
                        'ol': 'Ordered (OL)',
                        'ul.list-unstyled': 'Unstyled (UL)',
                        'ul.list-inline': 'Inline (UL)'
                    },
                    get: MBE.options.is,
                    set: function (opt) {
                        var tc = opt.value.split('.');
                        var tagName = tc[0];
                        var className = tc.length > 1 ? tc[1] : '';

                        $(this).removeClass().addClass(className);
                        if (!$(this).is(tagName)) {
                            MBE.options.toggleTagName.apply(this, [{ value: tagName }]);
                        }

                        if (opt.value == 'ol') {
                            $('#ListNumberingType, #ListNumberingStart, #ListNumberingReversed').show();
                        }
                        else {
                            $('#ListNumberingType, #ListNumberingStart, #ListNumberingReversed').hide();
                            $(this).removeAttr('type start reversed');
                        }
                    }
                }, {
                    id: 'ListNumberingType',
                    label: 'Numbering type',
                    type: 'select',
                    options: {
                        '1': 'Number (1, 2, 3)',
                        'a': 'Letters (a, b, c)',
                        'A': 'Letters (A, B, C)',
                        'i': 'Roman (i, ii, iii)',
                        'I': 'Roman (I, II, III)'
                    },
                    attr: 'type',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    id: 'ListNumberingStart',
                    label: 'Start',
                    type: 'number',
                    attr: 'start',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    id: 'ListNumberingReversed',
                    label: 'Reversed',
                    type: 'boolean',
                    attr: 'reversed',
                    options: {
                        'reversed': 'Reversed'
                    },
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }],
                onBuild: function () {
                    var opt = $('#ListStyle select');
                    $('#ListNumberingType, #ListNumberingStart, #ListNumberingReversed').toggle(opt.val() == 'ol');
                }
            }
        },
        'list-group': {
            'listGroupOptions': {
                name: 'List group options',
                type: 'group',
                groupItems: [{
                    label: 'Type',
                    type: 'select',
                    options: {
                        'ul': 'Unordered list',
                        'div-links': 'DIV with links',
                        'div-buttons': 'DIV with buttons'
                    },
                    get: function (value) {
                        switch (value) {
                            case 'ul': return $(this).is('ul');
                            case 'div-links': return $(this).is('div') && $(this).find('> a').length > 0;
                            case 'div-buttons': return $(this).is('div') && $(this).find('> button').length > 0;
                        }
                    },
                    set: function (opt) 
                    {
                        var self = MBE.types.containers;
                        var target = $(this);
                        var groupTemplate, itemTemplate;

                        switch(opt.value)
                        {
                            case 'ul': {
                                groupTemplate = 'list-group';
                                itemTemplate = 'list-group-item';
                                break; 
                            }
                            case 'div-links': {
                                groupTemplate = 'list-group-div';
                                itemTemplate = 'list-group-item-link';
                                break;
                            }
                            case 'div-buttons': {
                                groupTemplate = 'list-group-div';
                                itemTemplate = 'list-group-item-button';
                                break;
                            }
                        }

                        var group = $(self.templates[groupTemplate]);
                        target.find('> a, > button, > li').each(function () {
                            var item = $(self.templates[itemTemplate]);
                            item.html(this.innerHTML).addClass(this.className).appendTo(group);
                        });
                        group.attr('data-uic', 'containers|' + groupTemplate);

                        target.replaceWith(group);
                        MBE.DnD.updateDOM();
                        MBE.selection.select.apply(group[0], []);
                        MBE.options.target = group[0];
                    }
                }]
            }
        },
        'list-group-item': {
            'listGroupItemOptions': {
                name: 'List group item options',
                type: 'group',
                groupItems: [{
                    label: 'Style',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'list-group-item-success': 'Success',
                        'list-group-item-info': 'Info',
                        'list-group-item-warning': 'Warning',
                        'list-group-item-danger': 'Danger'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'URL',
                    type: 'text',
                    attr: 'href',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr,
                    allowFor: ['list-group-item-link'],
                }, {
                    label: 'Target',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        '_blank': 'Blank',
                        '_parent': 'Parent',
                        '_top': 'Top'
                    },
                    attr: 'target',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr,
                    allowFor: ['list-group-item-link']
                }, {
                    label: 'Options',
                    type: 'boolean',
                    options: {
                        'active': 'Active',
                        'disabled': 'Disabled'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'dl-dl': {
            'definitionListOptions': {
                name: 'Definition list options',
                type: 'group',
                groupItems: [{
                    label: 'Type',
                    type: 'select',
                    options: {
                        'null': 'default',
                        'dl-horizontal': 'horizontal'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        }
    },

    init: function()
    {
        var self = MBE.types.containers;
        var menu = MBE.toolbar.menu;

        MBE.DnD.onDrop.push(MBE.types.containers._drop);
        MBE.types.controls.options.link.linkOptions.groupItems[0].change = self.tabIdChange;

        self.options.panel.panelOptions.groupItems[1].disallowFor = self.panelIsNotInAccordion;
        self.options['list-group-div'] = self.options['list-group'];
        self.options['list-group-item-link'] = self.options['list-group-item'];
        self.options['list-group-item-button'] = self.options['list-group-item'];

        menu['containers'] = {};

        menu['containers']['list'] = {
            items: [
                { type: 'text', label: 'ADD TO LIST' },
                { type: 'button', label: 'BEGIN', callback: self.listAddToBegin },
                { type: 'button', label: 'END', callback: self.listAddToEnd },
            ]
        };
        menu['containers']['list-item'] = {
            items: [
                { type: 'text', label: 'ADD TO LIST' },
                { type: 'button', label: 'BEFORE', callback: self.listItemAddBefore },
                { type: 'button', label: 'AFTER', callback: self.listItemAddAfter },
                { type: 'button', label: 'BEGIN', callback: self.listAddToBegin },
                { type: 'button', label: 'END', callback: self.listAddToEnd },
                { type: 'text', label: 'ITEM' },
                { type: 'button', label: 'DELETE', callback: self.listItemDelete }
            ]
        };
        menu['containers']['tab-items'] = {
            items: [
                { type: 'text', label: 'ADD TAB' },
                { type: 'button', label: 'BEGIN', callback: self.tabAddToBegin },
                { type: 'button', label: 'END', callback: self.tabAddToEnd },
            ]
        };
        menu['containers']['tab'] = {
            items: [
                { type: 'text', label: 'TAB' },
                { type: 'button', label: 'SHOW', callback: self.tabShow },
                { type: 'text', label: 'ADD TAB' },
                { type: 'button', label: 'BEGIN', callback: self.tabAddToBegin },
                { type: 'button', label: 'END', callback: self.tabAddToEnd },
                { type: 'button', label: 'LEFT', callback: self.tabAddToLeft },
                { type: 'button', label: 'RIGHT', callback: self.tabAddToRight },
            ]
        };
        menu['containers']['accordion'] = {
            items: [
                { type: 'text', label: 'ADD ITEM' },
                { type: 'button', label: 'BEGIN', callback: self.accordionAddToBegin },
                { type: 'button', label: 'END', callback: self.accordionAddToEnd }
            ]
        };
        menu['containers']['panel'] = {
            allowFor: self.panelIsInAccordion,
            items: [
                { type: 'text', label: 'ADD ITEM' },
                { type: 'button', label: 'BEGIN', callback: self.accordionAddToBegin },
                { type: 'button', label: 'END', callback: self.accordionAddToEnd },
                { type: 'button', label: 'BEFORE', callback: self.accordionAddBefore },
                { type: 'button', label: 'AFTER', callback: self.accordionAddAfter },
            ]
        };
        menu['containers']['list-group'] = {
            items: [
                { type: 'text', label: 'ADD ITEM' },
                { type: 'button', label: 'BEFORE', callback: self.listGroupAddBefore, allowFor: self.listGroupIsItemSelected },
                { type: 'button', label: 'AFTER', callback: self.listGroupAddAfter, allowFor: self.listGroupIsItemSelected },
                { type: 'button', label: 'BEGIN', callback: self.listGroupAddToBegin },
                { type: 'button', label: 'END', callback: self.listGroupAddToEnd },
                { type: 'text', label: 'ITEM' },
                { type: 'button', label: 'ACTIVATE', callback: self.listGroupActivateItem, allowFor: self.listGroupIsInactiveItemSelected },
                { type: 'button', label: 'DEACTIVATE', callback: self.listGroupDeactivateItem, allowFor: self.listGroupIsActiveItemSelected },
                { type: 'button', label: 'DELETE', callback: self.listGroupDeleteItem, allowFor: self.listGroupIsItemSelected },
            ]
        }
        menu['containers']['list-group-div'] = menu['containers']['list-group'];
        menu['containers']['list-group-item'] = menu['containers']['list-group'];
        menu['containers']['list-group-item-link'] = menu['containers']['list-group'];
        menu['containers']['list-group-item-button'] = menu['containers']['list-group'];

        menu['containers']['dl-dl'] = {
            items: [
                { type: 'text', label: 'ADD DEFINITION' },
                { type: 'button', label: 'BEFORE', callback: self.dlAddBefore, allowFor: self.dlIsItemSelected },
                { type: 'button', label: 'AFTER', callback: self.dlAddAfter, allowFor: self.dlIsItemSelected },
                { type: 'button', label: 'BEGIN', callback: self.dlAddToBegin },
                { type: 'button', label: 'END', callback: self.dlAddToEnd },
                { type: 'text', label: 'ITEM' },
                { type: 'button', label: 'DELETE', callback: self.dlDeleteItem, allowFor: self.dlIsItemSelected },
            ]
        }
        menu['containers']['dl-dt'] = menu['containers']['dl-dl'];
        menu['containers']['dl-dd'] = menu['containers']['dl-dl'];

        MBE.onBeforeDelete['containers|tab'] = self._tabDelete;
        MBE.onBeforeDelete['containers|dl-dt'] = self._dtDelete;
        MBE.onBeforeDelete['containers|dl-dd'] = self._ddDelete;
    },

    _drop: function(target)
    {
        var elm = $(this);
        if (target.is('.panel-heading') && elm.is('[data-uic="text|heading"]')) {
            elm.addClass('panel-title');
        }

        if(elm.is(':empty')) {
            if (elm.is('[data-uic="containers|tabs"]')) {
                MBE.types.containers.buildTabs.apply(this, []);
            }

            if (elm.is('[data-uic="containers|accordion"]')) {
                MBE.types.containers.buildAccordion.apply(this, []);
            }

            if (elm.is('.list-group')) {
                MBE.types.containers.buildListGroup.apply(this, []);
            }
            if (elm.is('dl')) {
                MBE.types.containers.buildDL.apply(this, []);
            }
        }

        if (elm.is('dt')) { MBE.types.containers._dtDrop.apply(this, []); }
        if (elm.is('dd')) { MBE.types.containers._ddDrop.apply(this, []); }
    },

    /******************************************************************/
    /* LIST GROUP CONTEXT METHODS                                     */
    /******************************************************************/
    buildListGroup: function() {
        for (var i = 1; i <= 3; i++) {
            var item = $(MBE.types.containers.templates['list-group-item']);
            item.html('List Group Item ' + i).appendTo(this);
        }
    },

    listGroupIsItemSelected: function () {
        console.log(this);
        return $(this).is('.list-group-item');
    },
    
    listGroupIsActiveItemSelected: function() {
        return $(this).is('.list-group-item.active');
    },

    listGroupIsInactiveItemSelected: function() {
        return $(this).is('.list-group-item:not(.active)');
    },

    listGroupActivateItem: function () {
        $(this).addClass('active');
        MBE.selection.select.apply(this, []);
    },

    listGroupDeactivateItem: function() {
        $(this).removeClass('active');
        MBE.selection.select.apply(this, []);
    },

    listGroupDeleteItem: function() {
        $(this).remove();
        $('.mbe-drag-handle', MBE.workspaceDoc).remove();
        MBE.DnD.updateDOM();
        MBE.selection.select.apply(MBE.workspaceDoc.body, []);
    },

    listGroupAdd: function(pos) 
    {
        var self = MBE.types.containers;
        var item = $(self.templates['list-group-item']);
        var target;
        if(pos == 'before' || pos == 'after') {
            target = $(this);
        }
        else {
            if($(this).is('.list-group')) {
                target = $(this);
            }
            else {
                target = $(this).parent();
            }
        }

        switch (pos) {
            case 'before': target.before(item); break;
            case 'after': target.after(item); break;
            case 'begin': target.prepend(item); break;
            case 'end': target.append(item); break;
        }

        MBE.DnD.updateDOM();
    },

    listGroupAddAfter: function() { MBE.types.containers.listGroupAdd.apply(this, ['after']); },
    listGroupAddBefore: function () { MBE.types.containers.listGroupAdd.apply(this, ['before']); },
    listGroupAddToBegin: function () { MBE.types.containers.listGroupAdd.apply(this, ['begin']); },
    listGroupAddToEnd: function() { MBE.types.containers.listGroupAdd.apply(this, ['end']); },

    /******************************************************************/
    /* LIST CONTEXT METHODS                                           */
    /******************************************************************/
    listItemAddBefore: function () {
        $(this).before($(MBE.types.containers.templates['list-item']));
        MBE.DnD.updateDOM();
    },

    listItemAddAfter: function () {
        $(this).after($(MBE.types.containers.templates['list-item']));
        MBE.DnD.updateDOM();
    },

    listItemDelete: function() {
        $(this).remove();
        MBE.DnD.updateDOM();
    },

    listAddToBegin: function () {
        var target = $(this).is('li') ? $(this).parent() : $(this);
        target.prepend($(MBE.types.containers.templates['list-item']));
        MBE.DnD.updateDOM();
    },

    listAddToEnd: function () {
        var target = $(this).is('li') ? $(this).parent() : $(this);
        target.append($(MBE.types.containers.templates['list-item']));
        MBE.DnD.updateDOM();
    },

    /******************************************************************/
    /* TABS CONTEXT METHOD                                            */
    /******************************************************************/
    tabShow: function () {
        var tabs = $(this).siblings('li');
        tabs.each(function() {
            var link = $('a', this);
            $(link.attr('href')).removeClass('active');
            $(this).removeClass('active');
        })
        $(this).addClass('active');
        $($('a', this).attr('href')).addClass('active');
    },

    tabIdChange: function() {
        var elm = $(this);
        if (elm.parent().is('[data-uic="containers|tab"]')) {
            var prevId = elm.data('prevId');
            var newId = elm.attr('href').replace('#', '');
            $('#' + prevId).attr('id', newId);
            elm.data('prevId', newId);
            MBE.DnD.updateDOM();
        }
    },

    tabAdd: function(pos) {
        var self = MBE.types.containers;
        var tabIndex = $('[data-toggle="tab"]', MBE.workspace).length + 1;
        var tab = $(this).is('li') ? $(this) : null;
        var tabs = $(this).is('.nav-tabs') ? $(this) : $(this).parent();
        var content = tabs.next();

        var tabItem = $(self.templates['tab']);
        var tabPane = $(self.templates['tab-pane']);
        var tabId = 'tab-' + tabIndex;

        tabPane.attr('id', tabId);
        tabItem.find('a').attr('href', '#' + tabId).data('prevId', tabId).html('Tab ' + tabIndex);

        switch (pos) {
            case 'begin':
                tabs.prepend(tabItem);
                content.prepend(tabPane);
                break;
            case 'end':
                tabs.append(tabItem);
                content.append(tabPane);
                break;
            case 'left':
                tab.before(tabItem);
                $(tab.find('a').attr('href')).before(tabPane);
                break;
            case 'right':
                tab.after(tabItem);
                $(tab.find('a').attr('href')).after(tabPane);
                break;
        }
        MBE.DnD.updateDOM();
    },

    tabAddToLeft: function() {
        MBE.types.containers.tabAdd.apply(this, ['left']);
    },

    tabAddToRight: function () {
        MBE.types.containers.tabAdd.apply(this, ['right']);
    },

    tabAddToBegin: function () {
        MBE.types.containers.tabAdd.apply(this, ['begin']);
    },

    tabAddToEnd: function () {
        MBE.types.containers.tabAdd.apply(this, ['end']);
    },

    buildTabs: function (tabLabelArray)
    {
        var self = MBE.types.containers;
        var target = $(this);
        var items = $(self.templates['tab-items']);
        var content = $(self.templates['tab-content']);
        var tabIndex = $('[data-toggle="tab"]', MBE.workspace).length + 1;
        var prefix = typeof tabLabelArray == 'array' ? tabLabelArray : ['First tab', 'Second tab', 'Third tab'];

        target.append(items);
        target.append(content);
        for (var i = 0; i < prefix.length; i++) {
            var tab = $(self.templates['tab']);
            var pane = $(self.templates['tab-pane']);

            tab.find('a').attr('href', '#tab-' + tabIndex).html(prefix[i]).data('prevId', 'tab-' + tabIndex);
            pane.find('p').html(prefix[i] + ' content');
            pane.attr('id', 'tab-' + tabIndex);

            if (i == 0) {
                tab.addClass('active');
                pane.addClass('active');
            }

            items.append(tab);
            content.append(pane);
            tabIndex++;
        }
    },

    _tabDelete: function() {
        var tabId = $('a', this).attr('href');
        $(tabId).remove();
    },

    /******************************************************************/
    /* ACCORDION CONTEXT METHODS                                      */
    /******************************************************************/
    buildAccordion: function()
    {
        var self = MBE.types.containers;
        var target = $(this);
        var accordionIndex = MBE.workspace.find('.panel-group').length;
        var id = 'accordion-' + accordionIndex;

        target.attr('id', id);
        for(var i = 1; i <= 3; i++)
        {
            var item = $(self.templates['panel']);
            item.attr('data-uic', 'containers|panel');
            item.find('.panel-title').attr('locked', 'true').html('').append($(MBE.types.controls.templates.link));
            item.find('.panel-title a').html('Accordion item').attr({
                'locked': true,
                'data-uic': 'controls|link',
                'data-toggle': 'collapse',
                'data-parent': '#'+id,
                'href': '#' + id + ' .item-' + i
            });

            var wrapper = $('<div class="panel-collapse collapse item-'+ i + (i == 1 ? ' in' : '') + '"></div>');

            item.find('.panel-body p').html('Item body.').parent().wrap(wrapper);
            item.find('.panel-footer').remove();

            target.append(item);
        }
    },

    accordionAddItem: function (pos)
    {
        var self = MBE.types.containers;
        var target = $(this).is('.panel-group') ? $(this) : $(this).parent();
        var panel = $(this).is('.panel-group') ? null : $(this);
        var id = target.attr('id');
        var panelIndex = target.find('.panel-collapse').length + 1;

        var item = $(self.templates['panel']);
        item.attr('data-uic', 'containers|panel');
        item.find('.panel-title').attr('locked', 'true').html('').append($(MBE.types.controls.templates.link));
        item.find('.panel-title a').html('Accordion item').attr({
            'locked': true,
            'data-uic': 'controls|link',
            'data-toggle': 'collapse',
            'data-parent': '#' + id,
            'href': '#' + id + ' .item-' + panelIndex
        });

        var wrapper = $('<div class="panel-collapse collapse item-' + panelIndex + '"></div>');

        item.find('.panel-body p').html('Item body.').parent().wrap(wrapper);
        item.find('.panel-footer').remove();

        switch (pos) {
            case 'begin':
                target.prepend(item);
                break;
            case 'end':
                target.append(item);
                break;
            case 'before':
                panel.before(item);
                break;
            case 'after':
                panel.after(item);
                break;
        }
        MBE.DnD.updateDOM();
    },

    accordionAddToBegin: function() {
        MBE.types.containers.accordionAddItem.apply(this, ['begin']);
    },

    accordionAddToEnd: function() {
        MBE.types.containers.accordionAddItem.apply(this, ['end']);
    },

    accordionAddBefore: function() {
        MBE.types.containers.accordionAddItem.apply(this, ['before']);
    },

    accordionAddAfter: function() {
        MBE.types.containers.accordionAddItem.apply(this, ['after']);
    },

    panelIsNotInAccordion: function()
    {
        if ($(this).parent().hasClass('panel-group')) {
            return false;
        }
        return true;
    },

    panelIsInAccordion: function()
    {
        if ($(this).parent().hasClass('panel-group')) {
            return true;
        }
        return false;
    },

    /******************************************************************/
    /* LIST GROUP CONTEXT METHODS                                     */
    /******************************************************************/
    buildDL: function () {
        for (var i = 1; i <= 3; i++) {
            var dt = $(MBE.types.containers.templates['dl-dt']);
            var dd = $(MBE.types.containers.templates['dl-dd']);
            dt.html(dt.html() + ' ' + i).appendTo(this);
            dd.html(dd.html() + ' ' + i).appendTo(this);
        }
    },

    dlIsItemSelected: function () {
        return $(this).is('dt') || $(this).is('dd');
    },

    dlDeleteItem: function () {
        var elm = $(this);
        if (elm.is('dt')) { elm.next().remove(); }
        if (elm.is('dd')) { elm.prev().remove(); }

        elm.remove();

        $('.mbe-drag-handle', MBE.workspaceDoc).remove();
        MBE.DnD.updateDOM();
        MBE.selection.select.apply(MBE.workspaceDoc.body, []);
    },

    dlAdd: function (pos) {
        var self = MBE.types.containers;
        var dt = $(self.templates['dl-dt']);
        var dd = $(self.templates['dl-dd']);
        var target;

        if (pos == 'before' || pos == 'after') {
            if ($(this).is('dt')) {
                target = pos == 'before' ? $(this) : $(this).next();
            }
            if ($(this).is('dd')) {
                target = pos == 'before' ? $(this).prev() : $(this);
            }
        }
        else {
            target = $(this).is('dl') ? $(this) : $(this).parent();
        }


        switch (pos) {
            case 'before': target.before(dt); target.before(dd); break;
            case 'after': target.after(dd); target.after(dt); break;
            case 'begin': target.prepend(dd); target.prepend(dt); break;
            case 'end': target.append(dt); target.append(dd); break;
        }

        MBE.DnD.updateDOM();
    },

    dlAddAfter: function () { MBE.types.containers.dlAdd.apply(this, ['after']); },
    dlAddBefore: function () { MBE.types.containers.dlAdd.apply(this, ['before']); },
    dlAddToBegin: function () { MBE.types.containers.dlAdd.apply(this, ['begin']); },
    dlAddToEnd: function () { MBE.types.containers.dlAdd.apply(this, ['end']); },

    _dtDelete: function () { $(this).next().remove(); },
    _ddDelete: function () { $(this).prev().remove(); },

    _dtDrop: function () {
        var elm = $(this);
        elm.parent().find('dd').each(function () {
            if (!$(this).prev().is('dt')) {
                elm.after(this);
                return false;
            }
        });
    },

    _ddDrop: function () {
        var elm = $(this);
        elm.parent().find('dt').each(function () {
            if (!$(this).next().is('dd')) {
                elm.before(this);
                return false;
            }
        });
    }
};

MBE.onInit.push(MBE.types.containers.init);
MBE.types.text = {

    templates: {
        'heading': '<h1>Heading</h1>',
        'paragraph': '<p>Paragraph</p>',
        'alert': '<div class="alert alert-success">Alert text</div>',
        'blockquote': '<blockquote>'
                        + '<p data-uic="text|paragraph">Lorem ipsum dolor sit amet...</p>'
                        + '<footer data-uic="page|footer">Someone famous in <cite title="Source Title">Source Title</cite></footer>'
                      + '</blockquote>',
        'small': '<small>Text</small>',
        'strong': '<strong>Bold</strong>',
        'italic': '<em>Italic</em>',
        'span': '<span>Text</span>'
    },

    options: {
        'heading': { 
            'headingOptions': {
                name: 'Heading options',
                type: 'select',
                label: 'Type',
                options: {
                    'h1': 'H1',
                    'h2': 'H2',
                    'h3': 'H3',
                    'h4': 'H4',
                    'h5': 'H5',
                    'h6': 'H6'
                },
                set: MBE.options.toggleTagName,
                get: MBE.options.is
            }
        },
        'paragraph': {
            'paragraphOptions': {
                name: 'Paragraph options',
                type: 'boolean',
                options: {
                    'lead': 'Lead'
                },
                set: MBE.options.toggleClass,
                get: MBE.options.hasClass
            }
        },
        'alert': {
            'alertOptions': {
                name: 'Alert options',
                type: 'group',
                groupItems: [{
                    type: 'select',
                    label: 'Style',
                    options: {
                        'alert-success': 'Success',
                        'alert-info': 'Info',
                        'alert-warning': 'Warning',
                        'alert-danger': 'Danger'
                    },
                    set: MBE.options.toggleClass,
                    get: MBE.options.hasClass
                }, {
                    type: 'boolean',
                    options: {
                        'alert-dismissible': 'Dismissable'
                    },
                    set: function (opt) {
                        var elm = $(this);
                        if (elm.hasClass(opt.value)) {
                            elm.removeClass(opt.value);
                            elm.find('.close').remove();
                        }
                        else {
                            elm.addClass(opt.value);
                            elm.append('<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
                        }
                    },
                    get: MBE.options.hasClass
                }]
            }
        },
        'blockquote': {
            'blockquoteOptions': {
                name: 'Blockquote options',
                type: 'group',
                groupItems: [{
                    type: 'select',
                    label: 'Type',
                    options: {
                        'null': 'Normal',
                        'blockquote-reverse': 'Reverse'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    type: 'boolean',
                    label: 'Show footer',
                    options: {
                        'show-footer': 'Show footer'
                    },
                    get: function () {
                        return $(this).find('footer').length > 0;
                    },
                    set: function (opt) {
                        if($(opt).is(':checked')) {
                            $(this).append('<footer data-uic="page|footer">Some famous in <cite>Source Title</cite></footer>');
                        }
                        else {
                            $(this).find('footer').remove();
                        }
                        MBE.DnD.updateDOM();
                    }
                }]
            }
        },
        'strong': {
            'strongOptions': {
                name: 'Bold options',
                type: 'group',
                groupItems: [{
                    label: 'Tag',
                    type: 'select',
                    options: {
                        'strong': 'Strong',
                        'b': 'B'
                    },
                    get: MBE.options.is,
                    set: MBE.options.toggleTagName
                }]
            }
        },
        'italic': {
            'italicOptions': {
                name: 'Italic options',
                type: 'group',
                groupItems: [{
                    label: 'Tag',
                    type: 'select',
                    options: {
                        'em': 'Em',
                        'i': 'I'
                    },
                    get: MBE.options.is,
                    set: MBE.options.toggleTagName
                }]
            }
        },

        common: {
            'textOptions': {
                name: 'Text options',
                type: 'group',
                allowFor: [
                    'heading', 'paragraph', 'alert', 'small', 'strong', 'italic', 'span', 'link', 'help-text', 'caption',
                    'th', 'td', 'breadcrumbs-active', 'breadcrumbs-inactive'
                ],
                groupItems: [{
                    type: 'select',
                    label: 'Alignment',
                    allowFor: ['heading', 'paragraph', 'alert', 'help-text', 'caption', 'th', 'td'],
                    options: {
                        'null': 'Default',
                        'text-left': 'Left',
                        'text-center': 'Center',
                        'text-right': 'Right',
                        'text-justify': 'Justify'
                    },
                    set: MBE.options.toggleClass,
                    get: MBE.options.hasClass
                }, {
                    type: 'select',
                    label: 'Transformation',
                    allowFor: [
                        'heading', 'paragraph', 'alert', 'small', 'strong', 'italic', 'span', 'link', 'help-text', 'caption',
                        'th', 'td', 'breadcrumbs-active', 'breadcrumbs-inactive'
                    ],
                    options: {
                        'null': 'None',
                        'text-lowercase': 'Lowercase',
                        'text-uppercase': 'Uppercase',
                        'text-capitalize': 'Capitalized'
                    },
                    set: MBE.options.toggleClass,
                    get: MBE.options.hasClass
                }, {
                    type: 'select',
                    label: 'Color',
                    allowFor: [
                        'heading', 'paragraph', 'small', 'strong', 'italic', 'span', 'link', 'help-text', 'caption', 'th', 'td',
                        'breadcrumbs-active', 'breadcrumbs-inactive'
                    ],
                    options: {
                        'null': 'Default',
                        'text-muted': 'Muted',
                        'text-primary': 'Primary',
                        'text-success': 'Success',
                        'text-info': 'Info',
                        'text-warning': 'Warning',
                        'text-danger': 'Danger'
                    },
                    set: MBE.options.toggleClass,
                    get: MBE.options.hasClass
                }, {
                    type: 'select',
                    label: 'Background',
                    allowFor: [
                        'heading', 'paragraph', 'small', 'strong', 'italic', 'span', 'link', 'help-text', 'caption',
                        'breadcrumbs-active', 'breadcrumbs-inactive'
                    ],
                    options: {
                        'null': 'Default',
                        'bg-primary': 'Primary',
                        'bg-success': 'Success',
                        'bg-info': 'Info',
                        'bg-warning': 'Warning',
                        'bg-danger': 'Danger'
                    },
                    set: MBE.options.toggleClass,
                    get: MBE.options.hasClass
                }, {
                    type: 'boolean',
                    allowFor: ['heading', 'paragraph', 'alert', 'help-text', 'caption', 'th', 'td'],
                    options: {
                        'text-nowrap': 'No wrap'
                    },
                    set: MBE.options.toggleClass,
                    get: MBE.options.hasClass
                }],
            }
        }
    }
};
MBE.types.image = {

    templates: {
        'image': '<img src="" width="80" height="80" alt="">',
        'icon': '<span class="fa fa-star"></span>',
        'figure': '<figure></figure>',
        'figcaption': '<figcaption>Caption</figcaption>'
    },

    options: {
        'image': {
            'imageOptions': {
                name: 'Image options',
                type: 'group',
                groupItems: [{
                    label: 'Source URL',
                    type: 'text',
                    attr: 'src',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Width',
                    type: 'text',
                    attr: 'width',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Height',
                    type: 'text',
                    attr: 'height',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Alt',
                    type: 'text',
                    attr: 'alt',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Style',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'img-rounded': 'Rounded',
                        'img-circle': 'Circle',
                        'img-thumbnail': 'Thumbnail'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Responsive',
                    type: 'boolean',
                    options: {
                        'img-responsive': 'Responsive'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'icon': {
            'iconOptions': {
                name: 'Icon options',
                type: 'group',
                groupItems: [{
                    label: 'Icon',
                    type: 'icon',
                    fontSets: { 'fa': 'Font Awesome', 'glyphicon': 'Glyphicons' },
                    set: MBE.options.setIcon
                }]
            }
        }
    }
}
MBE.types.controls = {

    templates: {
        'button': '<button type="button" class="btn btn-default" name="button">Button</button>',
        'button-group': '<div class="btn-group" role="group">'
                            + '<button type="button" class="btn btn-default" data-uic="controls|button">Left</button>'
                            + '<button type="button" class="btn btn-default" data-uic="controls|button">Middle</button>'
                            + '<button type="button" class="btn btn-default" data-uic="controls|button">Right</button>'
                        + '</div>',
        'button-toolbar': '<div class="btn-toolbar" role="toolbar">'
                            + '<div class="btn-group" role="group" data-uic="controls|button-group">'
                                + '<button type="button" class="btn btn-default" data-uic="controls|button">Button 1</button>'
                                + '<button type="button" class="btn btn-default" data-uic="controls|button">Button 2</button>'
                            + '</div>'
                            + '<div class="btn-group" role="group" data-uic="controls|button-group">'
                                + '<button type="button" class="btn btn-default" data-uic="controls|button">Button 3</button>'
                                + '<button type="button" class="btn btn-default" data-uic="controls|button">Button 4</button>'
                            + '</div>'
                        + '</div>',
        'split-button': '<div class="btn-group"></div>',
        'button-dropdown': '<div class="btn-group"></div>',
        'dropdown-menu': '<ul class="dropdown-menu" data-uic="controls|dropdown-menu" locked></ul>',
        'dropdown-menu-item': '<li data-uic="controls|dropdown-menu-item"></li>',
        'dropdown-menu-header': '<li class="dropdown-header" data-uic="controls|dropdown-menu-header">Header</li>',
        'dropdown-menu-divider': '<li class="divider" data-uic="controls|dropdown-menu-divider"></li>',
        'link': '<a href="#" target="">Link</a>'
    },

    options: {
        'button': {
            'buttonOptions': {
                name: 'Button options',
                type: 'group',
                groupItems: [{
                    id: 'controls_button_element',
                    label: 'Element',
                    type: 'select',
                    options: {
                        'button': 'Button',
                        'a': 'Link'
                    },
                    get: MBE.options.is,
                    set: function (opt) {
                        $('#controls_button_button_type').toggle(opt.value == 'button');
                        $('#controls_button_link_url').toggle(opt.value != 'button');
                        $('#controls_button_link_target').toggle(opt.value != 'button');

                        if (opt.value == 'button') {
                            $(this).removeAttr('href target').attr('type', 'button');
                            $('#controls_button_button_type select').val('button');
                        }
                        if (opt.value == 'a') {
                            $(this).removeAttr('type').attr({ 'href': '#', 'target': '' });
                            $('#controls_button_link_url input').val('#');
                            $('#controls_button_link_target select').val('null');
                        }
                        
                        MBE.options.toggleTagName.apply(this, [opt]);
                    }
                }, {
                    id: 'controls_button_button_type',
                    label: 'Button type',
                    type: 'select',
                    options: {
                        'button': 'Button',
                        'submit': 'Submit',
                        'reset': 'Reset',
                    },
                    attr: 'type',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    id: 'controls_button_link_url',
                    label: 'Link URL',
                    type: 'text',
                    attr: 'href',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    id: 'controls_button_link_target',
                    label: 'Link target',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        '_blank': 'Blank',
                        '_parent': 'Parent',
                        '_top': 'Top'
                    },
                    attr: 'target',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Name',
                    type: 'text',
                    attr: 'name',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Value',
                    type: 'text',
                    attr: 'value',
                    id: 'AttrButtonValue',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr,
                    change: function (opt) {
                        $('#AttrID input').val(opt.value).change();
                    }
                }, {
                    label: 'Style',
                    type: 'select',
                    options: {
                        'btn-default': 'Default',
                        'btn-primary': 'Primary',
                        'btn-success': 'Success',
                        'btn-info': 'Info',
                        'btn-warning': 'Warning',
                        'btn-danger': 'Danger',
                        'btn-link': 'Link'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Size',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'btn-lg': 'Large',
                        'btn-sm': 'Small',
                        'btn-xs': 'Extra small'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Disabled',
                    type: 'boolean',
                    options: {
                        'disabled': 'Disabled'
                    },
                    get: function() { return $(this).is(':disabled'); },
                    set: function(opt) { $(this).attr('disabled', opt.checked); }
                }, {
                    label: 'Misc',
                    type: 'boolean',
                    options: {
                        'active': 'Active',
                        'btn-block': 'Block'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }],
                onBuild: function () {
                    var opt = $('#controls_button_element select');
                    console.log(opt.val());
                    $('#controls_button_button_type').toggle(opt.val() == 'button');
                    $('#controls_button_link_url').toggle(opt.val() != 'button');
                    $('#controls_button_link_target').toggle(opt.val() != 'button');
                }
            }
        },
        'button-group': {
            'buttonGroupOptions': {
                name: 'Button group options',
                type: 'group',
                groupItems: [{
                    label: 'Justified',
                    type: 'boolean',
                    options: {
                        'btn-group-justified': 'Justified'
                    },
                    get: MBE.options.hasClass,
                    set: function (opt) {
                        if (opt.checked) {
                            $('> button', this).each(function () {
                                var newTag = $('<a />');
                                for (var i = 0; i < this.attributes.length; i++) {
                                    var nodeName = this.attributes.item(i).nodeName;
                                    if (nodeName == 'type')
                                        continue;
                                    newTag[0].setAttribute(nodeName, this.attributes.item(i).nodeValue);
                                }
                                newTag.attr({ 'href': '#', 'target': '' }).html(this.innerHTML);
                                $(this).replaceWith(newTag);
                            });
                        }
                        else {
                            $('> a', this).each(function () {
                                var newTag = $('<button />');
                                for (var i = 0; i < this.attributes.length; i++) {
                                    var nodeName = this.attributes.item(i).nodeName;
                                    if (nodeName == 'href' || nodeName == 'target')
                                        continue;
                                    newTag[0].setAttribute(nodeName, this.attributes.item(i).nodeValue);
                                }
                                newTag.attr('type', 'button').html(this.innerHTML);
                                $(this).replaceWith(newTag);
                            });
                        }
                        $(this).toggleClass(opt.value);
                        MBE.DnD.updateDOM();
                    }
                }, {
                    label: 'Type',
                    type: 'select',
                    options: {
                        'btn-group': 'Horizontal',
                        'btn-group-vertical': 'Vertical'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Size',
                    type: 'select',
                    options: {
                        'btn-group-lg': 'Large',
                        'null': 'Default',
                        'btn-group-sm': 'Small',
                        'btn-group-xs': 'Extra small'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'link': {
            'linkOptions': {
                name: 'Link options',
                type: 'group',
                groupItems: [{
                    label: 'Link URL',
                    type: 'text',
                    attr: 'href',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Link target',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        '_blank': 'Blank',
                        '_parent': 'Parent',
                        '_top': 'Top'
                    },
                    attr: 'target',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]
            },
            'textOptions': MBE.types.text.options.common.textOptions
        }
    },

    init: function () 
    {
        var self = MBE.types.controls;

        MBE.DnD.onDrop.push(MBE.types.controls._drop);

        var menu = MBE.toolbar.menu;
        menu['controls'] = {};
        menu['controls']['button-dropdown'] = {
            allowFor: self.isDropdown,
            items: [
                { type: 'text', label: 'ADD TO MENU' },
                { type: 'button', label: 'ITEM', callback: self.dropdownAddItem },
                { type: 'button', label: 'HEADER', callback: self.dropdownAddHeader },
                { type: 'button', label: 'DIVIDER', callback: self.dropdownAddDivider },
                { type: 'text', label: 'MENU' },
                { type: 'button', label: 'SHOW', callback: self.dropdownShowMenu, allowFor: self.dropdownIsMenuHidden },
                { type: 'button', label: 'HIDE', callback: self.dropdownHideMenu, allowFor: self.dropdownIsMenuVisible },
            ]
        };
        menu['controls']['split-button'] = menu['controls']['button-dropdown'];
        menu['controls']['button'] = menu['controls']['button-dropdown'];
        menu['controls']['dropdown-menu'] = menu['controls']['button-dropdown'];
        menu['controls']['dropdown-menu-item'] = menu['controls']['button-dropdown'];
        menu['controls']['dropdown-menu-header'] = menu['controls']['button-dropdown'];
        menu['controls']['dropdown-menu-divider'] = menu['controls']['button-dropdown'];
        menu['controls']['link'] = menu['controls']['button-dropdown'];
    },

    _drop: function(target)
    {
        if ($(this).is('[data-uic="controls|button-dropdown"]:empty') || $(this).is('[data-uic="controls|split-button"]:empty')) {
            MBE.types.controls.dropdownBuild.apply(this, []);
        }
    },

    /**************************************************************/
    /* DROPDOWN CONTEXT METHODS                                   */
    /**************************************************************/
    isDropdown: function() {
        return $(this).is('[data-uic="controls|button-dropdown"]') ||
               $(this).parents('[data-uic="controls|button-dropdown"]').length ||
               $(this).is('[data-uic="controls|split-button"]') ||
               $(this).parents('[data-uic="controls|split-button"]').length;
    },

    dropdownBuild: function()
    {
        var self = MBE.types.controls;
        var dd = $(this);
        var button = $(self.templates['button']);
        var menu = $(self.templates['dropdown-menu']);
        var caret = $(MBE.types.misc.templates['caret']);
        var isSplitButton = dd.is('[data-uic="controls|split-button"]');
        
        caret.attr('data-uic', 'misc|caret');

        button.attr({
            'locked': true,
            'data-uic': 'controls|button',
            'data-toggle': 'dropdown'
        }).addClass('dropdown-toggle').html(isSplitButton ? '' : 'Dropdown ');
        button.append(caret);

        if (isSplitButton) {
            var actionButton = $(self.templates['button']);
            actionButton.attr({
                'locked': true,
                'data-uic': 'controls|button'
            }).html('Action').appendTo(dd);
        }
        
        var names = ['First item', 'Second item', 'Third item'];
        for (var i = 0; i < 3; i++)
        {
            var item = $(self.templates['dropdown-menu-item']);
            var link = $(self.templates.link);

            link.attr({
                'data-uic': 'controls|link',
                'locked': true
            }).html(names[i]).appendTo(item);
            
            item.appendTo(menu);
        }

        dd.append(button);
        dd.append(menu);
        MBE.DnD.updateDOM();
    },

    dropdownGetTarget: function()
    {
        var elm = $(this);
        if (elm.is('[data-uic="controls|button-dropdown"]')) {
            return $(this);
        }
        if (elm.parents('[data-uic="controls|button-dropdown"]').length) {
            return elm.parents('[data-uic="controls|button-dropdown"]').eq(0);
        }
        if (elm.is('[data-uic="controls|split-button"]')) {
            return $(this);
        }
        if (elm.parents('[data-uic="controls|split-button"]').length) {
            return elm.parents('[data-uic="controls|split-button"]').eq(0);
        }
        return $('<div></div>');
    },

    dropdownAddItem: function()
    {
        var self = MBE.types.controls;
        var target = self.dropdownGetTarget.apply(this, []);
        var item = $(self.templates['dropdown-menu-item']);
        var link = $(self.templates.link);

        link.attr({
            'data-uic': 'controls|link',
            'locked': true
        }).html('Menu item').appendTo(item);
        
        target.find('ul.dropdown-menu').append(item);
        MBE.DnD.updateDOM();

        setTimeout(function () { target.addClass('open'); }, 1);
    },

    dropdownAddHeader: function()
    {
        var self = MBE.types.controls;
        var target = self.dropdownGetTarget.apply(this, []);
        var item = $(self.templates['dropdown-menu-header']);

        target.find('ul.dropdown-menu').append(item);
        MBE.DnD.updateDOM();

        setTimeout(function () { target.addClass('open'); }, 1);
    },

    dropdownAddDivider: function()
    {
        var self = MBE.types.controls;
        var target = self.dropdownGetTarget.apply(this, []);
        var item = $(self.templates['dropdown-menu-divider']);

        target.find('ul.dropdown-menu').append(item);
        MBE.DnD.updateDOM();

        setTimeout(function () { target.addClass('open'); }, 1);
    },

    dropdownIsMenuHidden: function()
    {
        var self = MBE.types.controls;
        var target = self.dropdownGetTarget.apply(this, []);

        return !target.find('ul.dropdown-menu').is(':visible');
    },

    dropdownIsMenuVisible: function () {
        var self = MBE.types.controls;
        var target = self.dropdownGetTarget.apply(this, []);

        return target.find('ul.dropdown-menu').is(':visible');
    },

    dropdownShowMenu: function () {
        var self = MBE.types.controls;
        var target = self.dropdownGetTarget.apply(this, []);
        target.addClass('open');
        MBE.toolbar._select.apply($('.mbe-active', MBE.workspace)[0], []);
    },

    dropdownHideMenu: function () {
        var self = MBE.types.controls;
        var target = self.dropdownGetTarget.apply(this, []);
        target.removeClass('open');
        MBE.toolbar._select.apply($('.mbe-active', MBE.workspace)[0], []);
    }
}

MBE.onInit.push(MBE.types.controls.init);
MBE.types.grid = {

    templates: {
        'row': '<div class="row"></div>',
        'column': '<div class="col-xs-12"></div>',
        'clearfix': '<div class="clearfix"></div>'
    },

    options: {
        'column': {
            'columnSize': {
                name: 'Column size',
                type: 'group',
                groupItems: []
            },
            'columnOffset': {
                name: 'Column offset',
                type: 'group',
                groupItems: []
            },
            'columnPush': {
                name: 'Column push',
                type: 'group',
                groupItems: []
            },
            'columnPull': {
                name: 'Column pull',
                type: 'group',
                groupItems: []
            }
        }
    },

    initColumnOptions: function()
    {
        var formatList = {
            columnSize: 'col-{l}-{c}',
            columnOffset: 'col-{l}-offset-{c}',
            columnPush: 'col-{l}-push-{c}',
            columnPull: 'col-{l}-pull-{c}'
        };
        var sizeList = ['xs', 'sm', 'md', 'lg'];

        for (var k in MBE.types.grid.options.column) {
            var format = formatList[k];
            for (var i = 0; i < sizeList.length; i++) {
                var size = sizeList[i];
                var opt = {
                    label: size.toUpperCase(),
                    type: 'select',
                    options: {'null': 'None'},
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                };
                for (var j = 1; j <= 12; j++) {
                    var ok = format.replace(/\{l\}/, size).replace(/\{c\}/, j);
                    opt.options[ok] = j;
                }
                MBE.types.grid.options.column[k].groupItems.push(opt);
            }
        }
    }
};

if ($('body').hasClass('mozaicBootstrapEditorModule')) {
    $(MBE.types.grid.initColumnOptions);
}
MBE.types.page = {
    
    templates: {
        'page-header': '<div class="page-header" data-uic="containers|div">'
                        + '<h1 data-uic="text|heading">Page header <small data-uic="text|small">Subtext</small></h1>'
                      + '</div>',
        'header': '<header></header>',
        'footer': '<footer></footer>',
        'hgroup': '<hgroup></hgroup>',
        'section': '<section></section>',
        'article': '<article></article>',
        'aside': '<aside></aside>'
    },

    options: {

    }
};
MBE.types.table = {

    templates: {
        'table': '<table class="table"></table>',
        'tr': '<tr data-uic="table|tr"></tr>',
        'cell': '<td></td>',
        'td': '<td data-uic="table|td">Column</td>',
        'th': '<th data-uic="table|th">Cell</th>',
        'thead': '<thead data-uic="table|thead" locked></thead>',
        'tbody': '<tbody data-uic="table|tbody" locked></tbody>',
        'tfoot': '<tfoot data-uic="table|tfoot" locked></tfoot>',
        'caption': '<caption data-uic="table|caption" locked>Caption</caption>'
    },

    options: {
        'table': {
            'tableOptions': {
                name: 'Table options',
                type: 'group',
                groupItems: [{
                    label: 'Style',
                    type: 'boolean',
                    options: {
                        'table-striped': 'Striped',
                        'table-bordered': 'Bordered',
                        'table-hover': 'Hover',
                        'table-condensed': 'Condensed'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    name: 'Responsive',
                    type: 'boolean',
                    options: {
                        'table-responsive': 'Responsive'
                    },
                    get: function (value) {
                        return $(MBE.options.target).parent().is('.' + value);
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            $(this).wrap('<div class="table-responsive"></div>');
                        }
                        else {
                            $(this).unwrap();
                        }
                    }
                }, {
                    name: 'Show',
                    type: 'boolean',
                    options: {
                        'thead': 'Show table header',
                        'tfoot': 'Show table footer',
                        'caption': 'Show caption'
                    },
                    get: function (value) {
                        return $('> ' + value, MBE.options.target).length > 0;
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            switch (opt.value) {
                                case 'caption': {
                                    $(this).prepend($(MBE.types.table.templates['caption']));
                                    break;
                                }
                                case 'thead': {
                                    $('tbody', this).before(MBE.types.table.createTHead.apply(this, []));
                                    break;
                                }
                                case 'tfoot': {
                                    $('tbody', this).after(MBE.types.table.createTFoot.apply(this, []));
                                    break;
                                }
                            }
                        }
                        else {
                            $('> ' + opt.value, this).remove();
                        }
                        MBE.DnD.updateDOM();
                    }
                }]
            }
        },
        'caption': {
            'captionOptions': {
                name: 'Caption options',
                type: 'boolean',
                options: {
                    'lead': 'Lead'
                },
                set: MBE.options.toggleClass,
                get: MBE.options.hasClass
            },
            'textOptions': MBE.types.text.options.common.textOptions
        },
        'tr': {
            'trOptions': {
                name: 'Table row options',
                type: 'group',
                groupItems: [{
                    label: 'Style',
                    type: 'select',
                    options: {
                        'null': 'Defaut',
                        'active': 'Active',
                        'success': 'Success',
                        'info': 'Info',
                        'warning': 'Warning',
                        'danger': 'Danger'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'td': {
            'tdOptions': {
                name: 'Table cell options',
                type: 'group',
                groupItems: [{
                    label: 'Style',
                    type: 'select',
                    options: {
                        'null': 'Defaut',
                        'active': 'Active',
                        'success': 'Success',
                        'info': 'Info',
                        'warning': 'Warning',
                        'danger': 'Danger'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Rowspan',
                    type: 'number',
                    attr: 'rowspan',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Colspan',
                    type: 'number',
                    attr: 'colspan',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]
            },
            'textOptions': MBE.types.text.options.common.textOptions
        }
    },

    init: function()
    {
        var self = MBE.types.table;
        var menu = MBE.toolbar.menu;

        self.options.th = self.options.td;

        MBE.DnD.onDrop.push(self._drop);

        menu.table = {};
        menu.table.table = {
            items: [
                { type: 'text',   label: 'ADD ROW' },
                { type: 'button', label: 'ABOVE',   callback: self.rowAddAbove, allowFor: self.isCellOrRow },
                { type: 'button', label: 'BELOW',   callback: self.rowAddBelow, allowFor: self.isCellOrRow },
                { type: 'button', label: 'TOP',     callback: self.rowAddTop },
                { type: 'button', label: 'BOTTOM',  callback: self.rowAddBottom },
                { type: 'text',   label: 'ADD COLUMN' },
                { type: 'button', label: 'LEFT',    callback: self.columnAddLeft, allowFor: self.isCell },
                { type: 'button', label: 'RIGHT',   callback: self.columnAddRight, allowFor: self.isCell },
                { type: 'button', label: 'BEGIN',   callback: self.columnAddBegin },
                { type: 'button', label: 'END',     callback: self.columnAddEnd }
            ]
        };
        menu.table.thead = menu.table.tbody = menu.table.tfoot = menu.table.tr = menu.table.td = menu.table.th = menu.table.table;
    },

    _drop: function()
    {
        if ($(this).is('[data-uic="table|table"]:empty')) {
            MBE.types.table.build.apply(this, []);
        }
        if ($(this).is('[data-uic="table|cell"]')) {
            MBE.types.table.insertCell.apply(this, []);
        }
    },

    /****************************************************/
    /* TABLE CONTEXT METHODS                            */
    /****************************************************/
    build: function()
    {
        var self = MBE.types.table;
        var target = $(this);
        var thead = $(self.templates['thead']);
        var tbody = $(self.templates['tbody']);
        var row = $(self.templates['tr']);

        target.append(thead);
        target.append(tbody);

        var r = row.clone();
        thead.append(r);

        for (var i = 1; i <= 3; i++) {
            var cell = $(self.templates['th']);
            cell.html('Column ' + i);
            cell.appendTo(r);
        }

        for (var i = 0; i < 2; i++) {
            var r = row.clone();
            for (var j = 1; j <= 3; j++) {
                var cell = $(self.templates['td']);
                cell.html('Cell ' + j);
                cell.appendTo(r);
            }
            tbody.append(r);
        }
        MBE.DnD.updateDOM();
    },

    insertCell: function () {
        var self = MBE.types.table;
        var elm = $(this);
        if (elm.parent().parent().is('thead')) {
            elm.replaceWith($(self.templates['th']));
        }
        else {
            elm.replaceWith($(self.templates['td']));
        }
    },

    getCellCount: function() {
        var maxCellCount = 0;
        $('tbody tr', this).each(function () {
            var cellCount = 0;
            $('td', this).each(function () {
                cellCount += this.colSpan ? Number(this.colSpan) : 1;
            });
            if (cellCount > maxCellCount) {
                maxCellCount = cellCount;
            }
        });
        return maxCellCount;
    },

    createTHead: function ()
    {
        var self = MBE.types.table;
        var thead = $(self.templates['thead']);
        var row = $(self.templates['tr']);
        var maxCellCount = self.getCellCount.apply(this, []);

        for (var i = 1; i <= maxCellCount; i++) {
            var cell = $(self.templates['th']);
            cell.html('Column ' + i).appendTo(row);
        }
        thead.append(row);
        return thead;
    },

    createTFoot: function ()
    {
        var self = MBE.types.table;
        var tfoot = $(self.templates['tfoot']);
        var row = $(self.templates['tr']);
        var maxCellCount = self.getCellCount.apply(this, []);

        for (var i = 1; i <= maxCellCount; i++) {
            var cell = $(self.templates['td']);
            cell.html('Summary ' + i).appendTo(row);
        }
        tfoot.append(row);
        return tfoot;
    },

    isCell: function() {
        return $(this).is('td, th');
    },

    isCellOrRow: function() {
        return $(this).is('td, th, tr');
    },

    addRow: function(pos) 
    {
        var self = MBE.types.table;
        var elm = $(this);
        var table = elm.is('table') ? elm : elm.parents('table').eq(0);
        var target = elm.is('table') ? elm.find('tbody') : elm.parents('tbody, thead, tfoot').eq(0);
        var currentRow = elm.is('tr') ? elm : (elm.is('td, th') ? elm.parent() : null);
        var row = $(self.templates['tr']);
        var maxCellCount = self.getCellCount.apply(table[0], []);

        for (var i = 1; i <= maxCellCount; i++) {
            var cell = $(self.templates[target.is('thead') ? 'th' : 'td']);
            cell.html((target.is('thead') ? 'Column' : (target.is('tfoot') ? 'Summary' : 'Cell')) + ' ' + i).appendTo(row);
        } 

        switch (pos) {
            case 'top': target.prepend(row); break;
            case 'bottom': target.append(row); break;
            case 'above': currentRow.before(row); break;
            case 'below': currentRow.after(row); break;
        }
        MBE.DnD.updateDOM();
    },

    addColumn: function (pos) {
        var self = MBE.types.table;
        var elm = $(this);
        var table = elm.is('table') ? elm : elm.parents('table').eq(0);
        var index = elm.is('th, td') ? elm[0].cellIndex : null;

        // Pro thead, tbody i tfoot ....
        table.find('> thead, > tbody, > tfoot').each(function () {
            var e = $(this);
            var cell = $(self.templates[e.is('thead') ? 'th' : 'td']);
            cell.html(e.is('thead') ? 'Column' : (e.is('tfoot') ? 'Summary' : 'Cell'));

            // ... projdeme všechny řádky ...
            e.find('> tr').each(function () {
                // ... a umístíme novou buňku na správnou pozici
                switch (pos) {
                    case 'begin': $(this).prepend(cell.clone()); break;
                    case 'end': $(this).append(cell.clone()); break;
                    case 'left': $(this).find('> td, > th').eq(index).before(cell.clone()); break;
                    case 'right': $(this).find('> td, > th').eq(index).after(cell.clone()); break;
                }
            });
        });

        MBE.DnD.updateDOM();
    },

    rowAddAbove:    function () { MBE.types.table.addRow.apply(this, ['above']);    },
    rowAddBelow:    function () { MBE.types.table.addRow.apply(this, ['below']);    },
    rowAddTop:      function () { MBE.types.table.addRow.apply(this, ['top']);      },
    rowAddBottom:   function () { MBE.types.table.addRow.apply(this, ['bottom']);   },

    columnAddLeft:  function () { MBE.types.table.addColumn.apply(this, ['left']);  },
    columnAddRight: function () { MBE.types.table.addColumn.apply(this, ['right']); },
    columnAddBegin: function () { MBE.types.table.addColumn.apply(this, ['begin']); },
    columnAddEnd:   function () { MBE.types.table.addColumn.apply(this, ['end']);   }
};
MBE.onInit.push(MBE.types.table.init);
MBE.types.form = {

    templates: {
        'form': '<form class="form-horizontal" method="post"></form>',
        'form-group': '<div class="form-group"></div>',
        'label': '<label for="">Label</label>',
        'input-text': '<input type="text" name="" value="" class="form-control">',
        'input-email': '<input type="email" name="" value="" class="form-control">',
        'input-color': '<input type="color" name="" value="" class="form-control">',
        'select': '<select name="" class="form-control"></select>',
        'input-tel': '<input type="tel" name="" value="" class="form-control">',
        'input-date': '<input type="date" name="" value="" class="form-control">',
        'input-number': '<input type="number" name="" value="" class="form-control">',
        'input-range': '<input type="range" name="" value="" class="form-control">',
        'input-hidden': '<input type="hidden" name="" value="">',
        'input-url': '<input type="url" name="" value="" class="form-control">',
        'input-search': '<input type="search" name="" value="" class="form-control">',
        'input-password': '<input type="password" name="" value="" class="form-control">',
        'input-file': '<input type="file" name="" value="" class="form-control">',
        'textarea': '<textarea class="form-control" name="" value=""></textarea>',
        'checkbox-group': '<div class="checkbox"></div>',
        'radio-group': '<div class="radio"></div>',
        'checkbox': '<input type="checkbox" name="" value="">',
        'checkbox-label': '<label for="" data-uic="form|label"><input type="checkbox" name="" value="" data-uic="form|checkbox"> Checkbox</label>',
        'radio': '<input type="radio" name="" value="">',
        'radio-label': '<label for="" data-uic="form|label"><input type="radio" name="" value="" data-uic="form|radio"> Radio</label>',
        'static-control': '<p class="form-control-static">Static value</p>',
        'help-text': '<p class="help-block">Help text for field</p>',
        'input-group': '<div class="input-group">'
                            + '<div class="input-group-addon" data-uic="form|left-addon" locked><span data-uic="text|span">prefix</span></div>'
                            + '<input type="text" name="" value="" class="form-control" data-uic="form|input-text">'
                            + '<div class="input-group-addon" data-uic="form|right-addon" locked><span data-uic="text|span">suffix</span></div>'
                        + '</div>',
        'fieldset': '<fieldset><legend data-uic="form|legend" locked>Field group</legend></fieldset>',
        'legend': '<legend data-uic="form|legend" locked>Field group</legend>',
        'left-addon': '<div class="input-group-addon" data-uic="form|left-addon" locked><span data-uic="text|span">prefix</span></div>',
        'right-addon': '<div class="input-group-addon" data-uic="form|right-addon" locked><span data-uic="text|span">suffix</span></div>',
        'form-control-feedback': '<span class="glyphicon glyphicon-remove form-control-feedback"></span>'
    },

    options: {
        'form': {
            'formOptions': {
                name: 'Form options',
                type: 'group',
                groupItems: [{
                    label: 'Type',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'form-horizontal': 'Horizontal',
                        'form-inline': 'Inline'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Method',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'get': 'GET',
                        'post': 'POST'
                    },
                    attr: 'method',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Encoding',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'application/x-www-form-urlencoded': 'URL Encoded',
                        'multipart/form-data': 'Multipart', 
                        'text/plain': 'Plain'
                    },
                    attr: 'enctype',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]                
            }
        },
        'label': {
            'labelOptions': {
                name: 'Label options',
                type: 'group',
                groupItems: [{
                    label: 'For',
                    type: 'text',
                    attr: 'for',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]
            },
            'labelSize': {
                name: 'Label size',
                type: 'group',
                groupItems: MBE.types.grid.options.column.columnSize.groupItems,
                get: MBE.options.hasClass,
                set: MBE.options.toggleClass
            },
        },
        'select': {
            'selectOptions': {
                name: 'Select options',
                type: 'group',
                groupItems: [{
                    label: 'Default option',
                    type: 'text',
                    get: function () {
                        return $('option', MBE.options.target).length == 1 ? $('option', MBE.options.target).eq(0).text() : '';
                    },
                    set: function (opt) {
                        $('option', this).remove();
                        if (opt.value) {
                            $(this).append('<option value="">' + opt.value + '</option>');
                        }
                    }
                }],
            }
        },
        'static-control': {
            'staticControlOptions': {
                name: 'Static control options',
                type: 'group',
                groupItems: [MBE.types.text.options.paragraph.paragraphOptions]
            }
        },
        'help-text': {
            'helpTextOptions': {
                name: 'Help text block options',
                type: 'group',
                groupItems: [MBE.types.text.options.paragraph.paragraphOptions]
            },
            'textOptions': MBE.types.text.options.common.textOptions
        },
        'input-group': {
            'inputGroupOptions': {
                name: 'Input group options',
                type: 'group',
                groupItems: [{
                    label: 'Size',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'input-group-lg': 'Large',
                        'input-group-sm': 'Small'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Addons',
                    type: 'boolean',
                    options: {
                        'left': 'Show left addon',
                        'right': 'Show right addon'
                    },
                    get: function (value) {
                        return $('[data-uic="form|' + value + '-addon"]', this).length > 0;
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            if (opt.value == 'left') {
                                $(this).prepend(MBE.types.form.templates['left-addon']);
                            }
                            else {
                                $(this).append(MBE.types.form.templates['right-addon']);
                            }
                        }
                        else {
                            $('[data-uic="form|' + opt.value + '-addon"]', this).remove();
                        }
                        MBE.DnD.updateDOM();
                    }
                }]
            }
        },
        'fieldset': {
            'FieldsetOptions': {
                name: 'Fieldset options',
                type: 'group',
                groupItems: [{
                    label: 'Name',
                    type: 'text',
                    attr: 'name',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'State',
                    type: 'boolean',
                    options: {
                        'disabled': 'Disabled'
                    },
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Legend',
                    type: 'boolean',
                    options: {
                        'legend': 'Show legend'
                    },
                    get: function (value) {
                        return $('[data-uic="form|' + value + '"]', this).length > 0;
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            if (opt.value == 'legend') {
                                $(this).prepend(MBE.types.form.templates['legend']);
                            }
                        }
                        else {
                            $('[data-uic="form|' + opt.value + '"]', this).remove();
                        }
                        MBE.DnD.updateDOM();
                    }
                }]
            }
        },
        'checkbox-group': {
            'checkboxGroupOptions': {
                name: 'Checkbox group options',
                type: 'group',
                groupItems: [{
                    label: 'Type',
                    type: 'select',
                    options: {
                        'checkbox': 'Default',
                        'checkbox-inline': 'Inline'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'radio-group': {
            'radioGroupOptions': {
                name: 'Radio group options',
                type: 'group',
                groupItems: [{
                    label: 'Type',
                    type: 'select',
                    options: {
                        'radio': 'Default',
                        'radio-inline': 'Inline'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'form-control-feedback': {
            'formControlFeedbackOptions': {
                name: 'Form control feedback options',
                type: 'group',
                groupItems: [{
                    label: 'Icon',
                    type: 'icon',
                    fontSets: { 'glyphicon': 'Glyphicons' },
                    set: MBE.options.setIcon
                }]
            }
        },
        'form-group': {
            'formGroupOptions': {
                name: 'Form group options',
                type: 'group',
                groupItems: [{
                    label: 'Size',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'form-group-lg': 'Large',
                        'form-group-sm': 'Small'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },

        common: {
            'mainOptions': {
                name: 'Main',
                allowFor: [
                    'input-text', 'input-email', 'input-color', 'select', 'input-tel', 'input-number', 'input-range', 'input-hidden',
                    'input-url', 'input-search', 'input-password', 'input-file', 'input-date', 'textarea', 'checkbox', 'radio'
                ],
                type: 'group',
                groupItems: [{
                    label: 'Name',
                    type: 'text',
                    attr: 'name',
                    id: 'AttrName',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr,
                    change: function (opt) {
                        if ($(this).is('[type=radio]')) {
                            if (opt.value.length && $('#AttrValue input').val().length) {
                                $('#AttrID input').val(opt.value + '_' + $('#AttrValue input').val());
                            }
                        }
                        else {
                            $('#AttrID input').val(opt.value).change();
                        }
                    }
                }, {
                    label: 'Rows',
                    allowFor: ['textarea'],
                    type: 'number',
                    attr: 'rows',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Min',
                    allowFor: ['input-number', 'input-range', 'input-date'],
                    type: 'number',
                    attr: 'min',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Max',
                    allowFor: ['input-number', 'input-range', 'input-date'],
                    type: 'number',
                    attr: 'max',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Step',
                    allowFor: ['input-number', 'input-range', 'input-date'],
                    type: 'number',
                    attr: 'step',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Size',
                    disallowFor: ['input-hidden', 'checkbox', 'radio'],
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'input-lg': 'Large',
                        'input-sm': 'Small'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Placeholder',
                    allowFor: ['input-text', 'input-email', 'input-tel', 'input-url', 'input-search', 'input-password', 'textarea'],
                    type: 'text',
                    attr: 'placeholder',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Type',
                    allowFor: ['input-date'],
                    type: 'select',
                    attr: 'type',
                    options: {
                        'date': 'Date',
                        'time': 'Time',
                        'datetime-local': 'Datetime local',
                        'month': 'Month',
                        'week': 'Week'
                    },
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Value',
                    allowFor: ['radio'],
                    type: 'text',
                    attr: 'value',
                    id: 'AttrValue',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr,
                    change: function (opt) {
                        if ($(this).is('[type=radio]')) {
                            if (opt.value.length && $('#AttrName input').val().length) {
                                $('#AttrID input').val($('#AttrName input').val() + '_' + opt.value);
                            }
                        }
                    }
                }, {
                    label: 'Checked',
                    allowFor: ['checkbox', 'radio'],
                    type: 'boolean',
                    options: {
                        'checked': 'Checked'
                    },
                    get: MBE.options.hasProp,
                    set: MBE.options.setProp
                }]
            },
            'stateOptions': {
                name: 'State',
                disallowFor: ['input-hidden', 'static-control', 'help-text', 'legend', 'checkbox', 'radio'],
                type: 'boolean',
                options: {
                    'readonly': 'Readonly',
                    'disabled': 'Disabled'
                },
                get: MBE.options.hasAttr,
                set: MBE.options.setAttr
            },
            'crStateOptions': {
                name: 'State',
                allowFor: ['checkbox', 'radio'],
                type: 'boolean',
                options: {
                    'disabled': 'Disabled'
                },
                get: MBE.options.hasAttr,
                set: MBE.options.setAttr
            },
            'inputOptions': {
                name: 'Input',
                allowFor: [
                    'input-text', 'input-email', 'select', 'input-tel', 'input-number', 'input-url', 'input-search', 'input-password',
                    'input-file', 'textarea'
                ],
                type: 'group',
                groupItems: [{
                    label: 'Autofocus',
                    allowFor: [
                        'input-text', 'input-email', 'select', 'input-tel', 'input-number', 'input-url', 'input-search', 'input-password',
                        'textarea'
                    ],
                    type: 'boolean',
                    options: {
                        'autofocus': 'Autofocus'
                    },
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Autocomplete',
                    allowFor: [
                        'input-text', 'input-email', 'input-tel', 'input-number', 'input-url', 'input-search', 'input-password',
                        'textarea'
                    ],
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'on': 'On',
                        'off': 'Off'
                    },
                    attr: 'autocomplete',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Multiple',
                    allowFor: ['input-file', 'select'],
                    type: 'boolean',
                    options: {
                        'multiple': 'Multiple'
                    },
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]
            },
            
            validationOptions: {
                name: 'Validation',
                type: 'group',
                disallowFor: [
                    'form', 'label', 'form-group', 'input-hidden', 'input-range', 'form-control-feedback', 'static-control',
                    'help-text', 'input-group', 'fieldset', 'legend', 'left-addon', 'right-addon'
                ],
                groupItems: [{
                    label: 'Required',
                    type: 'boolean',
                    options: {
                        'required': 'Required'
                    },
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    disallowFor: ['checkbox', 'radio', 'input-color', 'select', 'input-file'],
                    label: 'Min length',
                    type: 'text',
                    attr: 'minlength',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    disallowFor: ['checkbox', 'radio', 'input-color', 'select', 'input-file'],
                    label: 'Max length',
                    type: 'text',
                    attr: 'maxlength',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]
            }
        }
    },

    init: function () {
        MBE.DnD.onDOMUpdate.push(MBE.types.form._domUpdate);
        MBE.DnD.onDrop.push(MBE.types.form._drop);
    },

    _drop: function(target) {
        if (target.is('.input-group-addon') && $(this).is('input, button, select, div')) {
            target.toggleClass('input-group-addon input-group-btn');
        }
        if (target.is('.input-group-btn') && !$(this).is('input, button, select, div')) {
            target.toggleClass('input-group-addon input-group-btn');
        }
        if ($(this).is('.form-control-feedback')) {
            target.addClass('has-feedback');
        }
    },

    _domUpdate: function () {
        $('.input-group-btn:empty').each(function () {
            $(this).removeClass('input-group-btn').addClass('input-group-addon');
        });
    }
};

MBE.onInit.push(MBE.types.form.init);
MBE.options.getProgressBarClass = function (value) {
    return $('.progress-bar', this).hasClass(value);
};
MBE.options.setProgressBarClass = function (opt) {
    if (opt.type == 'select') {
        var classes = new Array();
        $(opt).find('option').each(function () {
            classes.push(this.value);
        });
        $('.progress-bar', this).removeClass(classes.join(' '));
        if (opt.value == 'null') {
            return;
        }
    }
    $('.progress-bar', this).toggleClass(opt.value);
};

MBE.types.misc = {

    templates: {
        'custom-code': '<div></div>',
        'modal': '<div class="modal fade" tabindex="-1" role="dialog"></div>',
        'modal-dialog': '<div class="modal-dialog" data-uic="misc|modal-dialog" locked role="document"></div>',
        'modal-content': '<div class="modal-content" data-uic="misc|modal-content" locked></div>',
        'modal-header': '<div class="modal-header" data-uic="misc|modal-header" locked></div>',
        'modal-body': '<div class="modal-body" data-uic="misc|modal-body" locked></div>',
        'modal-footer': '<div class="modal-footer" data-uic="misc|modal-footer" locked></div>',
        'badge': '<span class="badge">4</span>',
        'tag': '<span class="label label-default">label</span>',
        'caret': '<span class="caret"></span>',
        'close': '<button type="button" class="close" aria-label="Close"><span aria-hidden="true">&times;</span></button>',
        'hr': '<hr />',
        'responsive-embed': '<div class="embed-responsive embed-responsive-16by9"><iframe class="embed-responsive-item" src=""></iframe></div>',
        'progressBar': '<div class="progress">' + 
                            '<div class="progress-bar" role="progressbar" style="min-width:2em; width:0%"></div>' +
                        '</div>',
        'breadcrumbs': '<ol class="breadcrumb"></ol>',
        'breadcrumbs-item': '<li data-uic="misc|breadcrumbs-item"></li>',
        'breadcrumbs-active': '<span data-uic="misc|breadcrumbs-active" locked></span>',
        'breadcrumbs-inactive': '<a data-uic="misc|breadcrumbs-inactive" locked></a>',
        'embed': '<div><div class="embed-code"></div><div class="uic-embed-preview"></div></div>'
    },

    options: {
        'custom-code': {
            'customCodeOptions': {
                name: 'Custom code',
                type: 'cm',
                get: function () {
                    var html = $(this.innerHTML);
                    html.find('.mbe-text-node').contents().unwrap();
                    var helper = $('<div></div>').html(html);
                    return helper.html();
                },
                set: function (opt) {
                    this.innerHTML = opt.value;
                    MBE.DnD.updateDOM();
                }
            }
        },
        'modal': {
            'modalOptions': {
                name: 'Modal options',
                type: 'group',
                groupItems: [{
                    label: 'Size',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'modal-lg': 'Large',
                        'modal-sm': 'Small',
                    },
                    get: function (value) {
                        return $('.modal-dialog', this).hasClass(value);
                    },
                    set: function(opt) {
                        $('.modal-dialog', this).removeClass('modal-sm modal-lg');
                        if (opt.value != 'null') {
                            $('.modal-dialog', this).addClass(opt.value);
                        }
                    }
                }, {
                    label: 'Options',
                    type: 'boolean',
                    options: {
                        'fade': 'Fade',
                        'show-header': 'Show header',
                        'show-footer': 'Show footer'
                    },
                    get: function (value) {
                        switch (value) {
                            case 'fade': return $(this).is('.fade');
                            case 'show-header': return $('.modal-header', this).length > 0;
                            case 'show-footer': return $('.modal-footer', this).length > 0;
                        }
                    },
                    set: function (opt) {
                        var self = MBE.types.misc;

                        switch (opt.value) {
                            case 'fade': 
                                $(this).toggleClass('fade'); 
                                break;
                            case 'show-header':
                                if (opt.checked) {
                                    self.modalCreateHeader().prependTo($('.modal-content', this));            
                                }
                                else {
                                    $('.modal-header', this).remove();
                                }
                                break;
                            case 'show-footer':
                                if (opt.checked) {
                                    self.modalCreateFooter().appendTo($('.modal-content', this));            
                                }
                                else {
                                    $('.modal-footer', this).remove();
                                }
                                break;
                        }

                        MBE.DnD.updateDOM();
                    }
                }]
            }
        },
        'tag': {
            'tagOptions': {
                name: 'Label options',
                type: 'group',
                groupItems: [{
                    label: 'Style',
                    type: 'select',
                    options: {
                        'label-default': 'Default',
                        'label-primary': 'Primary',
                        'label-success': 'Success',
                        'label-info': 'Info',
                        'label-warning': 'Warning',
                        'label-danger': 'Danger',
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'progressBar': {
            'progressBarOptions': {
                name: 'Progress bar options',
                type: 'group',
                groupItems: [{
                    label: 'Percentage',
                    type: 'number',
                    get: function () {
                        return parseInt($('.progress-bar', this).css('width'));
                    },
                    set: function (opt) {
                        $('.progress-bar', this).css('width', opt.value + '%');
                        if ($('.progress-bar', this).text().length > 0) {
                            $('.progress-bar', this).html(opt.value + '%');
                        }
                    }
                }, {
                    label: 'Style',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'progress-bar-success': 'Success',
                        'progress-bar-info': 'Info',
                        'progress-bar-warning': 'Warning',
                        'progress-bar-danger': 'Danger'
                    },
                    get: MBE.options.getProgressBarClass,
                    set: MBE.options.setProgressBarClass
                }, {
                    label: 'Options',
                    type: 'boolean',
                    options: {
                        'progress-bar-striped': 'Striped',
                        'active': 'Animated'
                    },
                    get: MBE.options.getProgressBarClass,
                    set: MBE.options.setProgressBarClass
                }, {
                    label: 'Show label',
                    type: 'boolean',
                    options: {
                        'show-label': 'Show label'
                    },
                    get: function () {
                        return $('.progress-bar', this).text().length > 0;
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            $('.progress-bar', this).html($('.progress-bar', this)[0].style.width);
                        }
                        else {
                            $('.progress-bar', this).html('');
                        }
                    }
                }]
            }
        },
        'responsive-embed': {
            'responsiveEmbedOptions': {
                name: 'Responsive embed options',
                type: 'group',
                groupItems: [{
                    label: 'Source URL',
                    type: 'text',
                    get: function () {
                        return $('> iframe', MBE.options.target).attr('src').length ? $('> iframe', MBE.options.target).attr('src') : '';
                    },
                    set: function (opt) {
                        $('> iframe', this).attr('src', opt.value);
                    }
                }, {
                    label: 'Aspect ratio',
                    type: 'select',
                    options: {
                        'embed-responsive-16by9': '16 by 9',
                        'embed-responsive-4by3': '4 by 3'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'breadcrumbs-item': {
            'breadcrumbsItemOptions': {
                name: 'Breadcrumbs item options',
                type: 'boolean',
                options: {
                    'active': 'Active'
                },
                get: MBE.options.hasClass,
                set: function (opt) {
                    $(this).toggleClass(opt.value);
                    var item = $(this).find('a[locked], span[locked]').eq(0);
                    if (opt.checked) {
                        var span = $(MBE.types.misc.templates['breadcrumbs-active']);
                        span.html(item.html());
                        item.replaceWith(span);
                    }
                    else {
                        var link = $(MBE.types.misc.templates['breadcrumbs-inactive']);
                        link.html(item.html());
                        item.replaceWith(link);
                    }
                    MBE.DnD.updateDOM();
                }
            }
        },
        'breadcrumbs-inactive': MBE.types.controls.options.link,
        'breadcrumbs-active': MBE.types.text.options.common,
        'embed': {
            'embedOptions': {
                name: 'Embed options',
                type: 'group',
                groupItems: [{
                    label: 'Code',
                    type: 'cm',
                    get: function () { return $(this).find('.embed-code').text(); },
                    set: function (opt) {
                        $(this).find('.embed-code').text(opt.value);
                        if (opt.value.indexOf('<script') === -1) { // preview of script embed is buggy
                            var preview = $(this).find('.uic-embed-preview');
                            preview[0].innerHTML = opt.value; 
                        }
                        
                        MBE.DnD.updateDOM();
                    }
                }]
            }
        },
    },

    init: function() 
    {
        var self = MBE.types.misc;
        var menu = MBE.toolbar.menu;

        MBE.DnD.onDrop.push(self._drop);

        menu.misc = {};
        menu.misc.breadcrumbs = {
            items: [
                { type: 'text', label: 'Add item' },
                { type: 'button', label: 'BEFORE', callback: self.bcAddBefore, allowFor: self.bcIsItemSelected },
                { type: 'button', label: 'AFTER', callback: self.bcAddAfter, allowFor: self.bcIsItemSelected },
                { type: 'button', label: 'BEGIN', callback: self.bcAddToBegin },
                { type: 'button', label: 'END', callback: self.bcAddToEnd },
                { type: 'text', label: 'Item' },
                { type: 'button', label: 'ACTIVATE', callback: self.bcActivate, allowFor: self.bcIsInactiveItemSelected },
                { type: 'button', label: 'DEACTIVATE', callback: self.bcDeactivate, allowFor: self.bcIsActiveItemSelected },
                { type: 'button', label: 'DELETE', callback: self.bcDelete, allowFor: self.bcIsItemSelected }
            ]
        }
        menu.misc['breadcrumbs-item'] = menu.misc.breadcrumbs;
        menu.misc['breadcrumbs-active'] = menu.misc.breadcrumbs;
        menu.misc['breadcrumbs-inactive'] = menu.misc.breadcrumbs;

        menu.misc.modal = {
            items: [
                { type: 'text', label: 'Modal' },
                { type: 'button', label: 'Show', callback: self.modalShow, allowFor: self.modalIsInActive },
                { type: 'button', label: 'Hide', callback: self.modalHide, allowFor: self.modalIsActive }
            ]
        };
        menu.misc['modal-header'] = menu.misc.modal;
        menu.misc['modal-body'] = menu.misc.modal;
        menu.misc['modal-footer'] = menu.misc.modal;
    },

    _drop: function(target)
    {
        if ($(this).is('[data-uic="misc|breadcrumbs"]') && $(this).is(':empty')) {
            MBE.types.misc.buildBreadCrumbs.apply(this, []);
        }

        if ($(this).is('.modal') && $(this).is(':empty')) {
            MBE.types.misc.buildModal.apply(this, []);
        }
    },

    /*****************************************************/
    /* MODAL CONTEXT METHODS                             */
    /*****************************************************/
    buildModal: function() 
    {
        var self = MBE.types.misc;
        var modal = $(this);
        var dialog = $(self.templates['modal-dialog']);
        var content = $(self.templates['modal-content']);
        var body = $(self.templates['modal-body']);
        
        self.modalCreateHeader().appendTo(content);

        body.append('<p data-uic="text|paragraph">The content of your modal</p>');
        body.appendTo(content);

        self.modalCreateFooter().appendTo(content);

        content.appendTo(dialog);
        dialog.appendTo(modal);

        MBE.DnD.updateDOM();
    },

    modalCreateHeader: function() {
        var self = MBE.types.misc;
        var header = $(self.templates['modal-header']);
        header.append('<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
        header.append('<h4 class="modal-title" data-uic="text|heading">Modal title</h4>');

        return header;
    },

    modalCreateFooter: function () {
        var self = MBE.types.misc;
        var footer = $(self.templates['modal-footer']);
        footer.append('<button type="button" class="btn btn-default" data-dismiss="modal" data-uic="controls|button">Close</button>');
        footer.append('<button type="button" class="btn btn-primary" data-uic="controls|button">Save changes</button>');

        return footer;
    },

    modalGetTarget: function() {
        return $(this).is('.modal') ? $(this) : $(this).parents('.modal').eq(0);
    },

    modalIsActive: function () { return MBE.types.misc.modalGetTarget.apply(this, []).is('.in'); },
    modalIsInActive: function() { return MBE.types.misc.modalGetTarget.apply(this, []).is(':not(.in)'); },

    modalShow: function() {
        var m = MBE.types.misc.modalGetTarget.apply(this, []);
        m.addClass('in')
         .show()
         .after('<div class="modal-backdrop fade in" bs-hidden="" bs-system-element="" style="display: block;"></div>');
        
        MBE.selection.select.apply(m[0], []);
    },

    modalHide: function() {
        var m = MBE.types.misc.modalGetTarget.apply(this, []);
        m.removeClass('in')
         .hide()
         .next('.modal-backdrop').remove();

        MBE.selection.select.apply(m[0], []);
    },

    /*****************************************************/
    /* BREAD CRUMBS CONTEXT METHODS                      */
    /*****************************************************/
    buildBreadCrumbs: function ()
    {
        var self = MBE.types.misc;
        var elm = $(this);
        var linkNames = ['Home', 'Library', 'Data'];

        for (var i = 0; i < linkNames.length; i++) {
            var link = $(MBE.types.misc.templates['breadcrumbs-inactive']);
            var item = $(self.templates['breadcrumbs-item']);

            link.html(linkNames[i]).appendTo(item);
            elm.append(item);
        }
    },

    bcGetItem: function() {
        return $(this).is('[data-uic="misc|breadcrumbs-item"]') ? $(this) : $(this).parents('[data-uic="misc|breadcrumbs-item"]').eq(0);
    },

    bcIsItemSelected: function () {
        return $(this).is('[data-uic="misc|breadcrumbs-item"]') || $(this).parent().is('[data-uic="misc|breadcrumbs-item"]');
    },

    bcIsActiveItemSelected: function () {
        return $(this).is('.active[data-uic="misc|breadcrumbs-item"]') || $(this).parent().is('.active[data-uic="misc|breadcrumbs-item"]');
    },

    bcIsInactiveItemSelected: function () {
        return $(this).is('[data-uic="misc|breadcrumbs-item"]:not(.active)') || $(this).parent().is('[data-uic="misc|breadcrumbs-item"]:not(.active)');
    },

    bcDelete: function () {
        var target = MBE.types.misc.bcGetItem.apply(this, []);
        if (target.length) {
            target.remove();
            $('.mbe-drag-handle').remove();
            MBE.DnD.updateDOM();
        }
    },

    bcToggleState: function (state) {
        var self = MBE.types.misc;
        var item = self.bcGetItem.apply(this, []);
        var target = $('> a, > span', item);
        var replace = $(self.templates['breadcrumbs-' + state]);

        replace.html(target.html());
        target.replaceWith(replace);

        item.toggleClass('active');
        MBE.DnD.updateDOM();

        MBE.selection.select.apply(item.is('.mbe-active') ? item[0] : replace[0], []);
    },

    bcAdd: function(pos) {
        var self = MBE.types.misc;
        var target = $(this).is('.breadcrumb') ? $(this) : $(this).parents('.breadcrumb').eq(0);
        var item = $(self.templates['breadcrumbs-item']);
        var link = $(self.templates['breadcrumbs-inactive']);

        link.html('Item').appendTo(item);
        switch (pos) {
            case 'before': self.bcGetItem.apply(this).before(item); break;
            case 'after': self.bcGetItem.apply(this).after(item); break;
            case 'begin': target.prepend(item); break;
            case 'end': target.append(item); break;
        }
        MBE.DnD.updateDOM();
    },

    bcActivate: function () { MBE.types.misc.bcToggleState.apply(this, ['active']); },
    bcDeactivate: function () { MBE.types.misc.bcToggleState.apply(this, ['inactive']); },

    bcAddBefore: function () { MBE.types.misc.bcAdd.apply(this, ['before']); },
    bcAddAfter: function () { MBE.types.misc.bcAdd.apply(this, ['after']); },
    bcAddToBegin: function () { MBE.types.misc.bcAdd.apply(this, ['begin']); },
    bcAddToEnd: function() { MBE.types.misc.bcAdd.apply(this, ['end']); },
}

MBE.onInit.push(MBE.types.misc.init);
MBE.types.functions = {

    templates: {
        'foreach': '<div></div>',
        'if': '<div></div>',
    },

    options: {
        'foreach': {
            'foreachOptions': {
                name: 'Foreach options',
                type: 'group',
                groupItems: [{
                    label: 'Wrapper tag',
                    type: 'select',
                    options: {
                        'tr': 'tr',
                        'td': 'td',
                        'div': 'div',
                        'li': 'list item'
                    },
                    get: MBE.options.is,
                    set: MBE.options.toggleTagName
                }, {
                    label: 'Input variable name',
                    type: 'text',
                    attr: 'data-varname',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Data type',
                    type: 'select',
                    options: {
                        'null': '-- chose one --',
                        'jtoken': 'JToken',
                        'dbitem': 'List&lt;DbItem&gt;',
                        'object': 'List&lt;Object&gt;'
                    },
                    attr: 'data-type',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]
            }
        },
        'if': {
            'ifOptions': {
                name: 'IF options',
                type: 'group',
                groupItems: [{
                    label: 'Wrapper tag',
                    type: 'select',
                    options: {
                        'tr': 'tr',
                        'div': 'div',
                        'li': 'list item'
                    },
                    get: MBE.options.is,
                    set: MBE.options.toggleTagName
                }, {
                    label: 'Variable name',
                    type: 'text',
                    attr: 'data-varname',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]
            }
        }
    },

    init: function () {

    }
};

MBE.onInit.push(MBE.types.functions.init);
MBE.types.ui = {

    templates: {
        'horizontal-form-row': '<div></div>',
        'nv-list': '<table class="table name-value-list"></table>',
        'data-table': '<table class="table data-table"></table>',
        'countdown': '<div class="countdown-component">' +
                        '<span class="countdown-row countdown-show3">' + 
                            '<span class="countdown-section">' +
                                '<span class="countdown-amount">0</span>' +
                                '<span class="countdown-period">Hodin</span>' + 
                            '</span>' +
                            '<span class="countdown-section">' +
                                '<span class="countdown-amount">29</span>' +
                                '<span class="countdown-period">Minut</span>' +
                            '</span>' + 
                            '<span class="countdown-section">' + 
                                '<span class="countdown-amount">59</span>' +
                                '<span class="countdown-period">Sekund</span>' +
                            '</span>' + 
                        '</span>' +
                    '</div>',
        'wizzard': '<div class="wizard-phases wizard-phases-frame"></div>',
        'wizzard-body': '<div class="wizzard-body" data-uic="ui|wizzard-body" locked></div>',
        'wizzard-phase': '<div class="phase" data-uic="ui|wizzard-phase" locked>' +
                            '<div class="phase-icon-circle">' +
                                '<div class="phase-icon-number">1</div>' +
                            '</div>' +
                            '<div class="phase-label">Fáze 1</div>' + 
                        '</div>'
    },

    templatesName: {
        'horizontal-form-row': 'Horizontal form row',
        'nv-list': 'Name - Value list',
        'data-table': 'Data table',
        'countdown': 'Countdown',
        'wizzard': 'Wizzard phases',
        'wizzard-body': 'Wizzard body#hide',
        'wizzard-phase': 'Wizzard phase#hide'
    },

    options: {
        'data-table': {
            'dataTableOptions': {
                name: 'Data table options',
                type: 'group',
                groupItems: [{
                    label: 'Options',
                    type: 'boolean',
                    options: {
                        'data-dtpaging': 'Show pagination',
                        'data-dtinfo': 'Show informations',
                        'data-dtfilter': 'Show filter',
                        'data-dtcolumnfilter': 'Show column filter',
                        'data-dtordering': 'Enable ordering',
                        'data-dtserverside': 'Server-side proccessing',
                        'data-dtselect': 'Show selection checkboxes'
                    },
                    get: function (value) {
                        return $(this).attr(value) == '1';
                    },
                    set: function (opt) {
                        $(this).attr(opt.value, opt.checked ? '1' : '0');
                        MBE.types.ui.updateDataTable.apply(this, []);
                    }
                }, {
                    label: 'Order',
                    type: 'text',
                    attr: 'data-dtorder',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Actions',
                    type: 'builder',
                    builder: function (opt, isCollapsed)
                    {
                        if (!MBE.options.isAllowed(opt)) {
                            return '';
                        }

                        var group = $('<div class="option-group form-group"' + (opt.id ? ' id="' + opt.id + '"' : '') + '></div>');
                        var set = opt.set;

                        var lb = $('<label class="control-label col-xs-2">' + opt.label + '</label>');
                        var list = $('<table class="table table-stripped table-condensed dt-action-list"></table>');

                        var head = $('<thead />');
                        var body = $('<tbody />');

                        var row = $('<tr class="info" />');
                        row.html('<th>Icon</th><th>Action ID</th><th>ID param</th><th>Action title</th><th class="text-center"><span data-action="dt-add-action" class="fa fa-plus-circle" style="cursor: pointer"></span></th>').appendTo(head);

                        head.appendTo(list);
                        body.appendTo(list);

                        group.append(lb);
                        group.append(list);

                        list.wrap('<div class="col-xs-10"></div>');

                        if (isCollapsed) {
                            group.css('display', 'none');
                        }

                        body.sortable({
                            cursor: 'move',
                            forceHelperSize: true,
                            forcePlaceholderSize: true,
                            handle: '.handle',
                            helper: function (e, ui) {
                                ui.children().each(function () {
                                    $(this).width($(this).width());
                                });
                                return ui;
                            },
                            update: function () {
                                MBE.types.ui.dataTableBuildActions.apply(MBE.options.target, [false]);
                            }
                        });

                        var actions = $(MBE.options.target).attr('data-actions');
                        if (actions && actions.length)
                        {
                            actionList = JSON.parse(actions.replace(/'/g, '"'));
                            for (var i = 0; i < actionList.length; i++) {
                                MBE.types.ui.dataTableAddAction.apply(body[0], [null, actionList[i]]);
                            }
                        }

                        return group;
                    },
                    get: function() {},
                    set: function() {}
                }]
            }
        },
        'wizzard': {
            'wizzardOptions': {
                name: 'Wizzard options',
                type: 'group',
                groupItems: [{
                    label: 'Phases',
                    type: 'text',
                    attr: 'data-phases',
                    get: MBE.options.hasAttr,
                    set: function(opt) {
                        $(this).attr('data-phases', opt.value);
                        MBE.types.ui.wizzardBuildPhases.apply(this, []);
                    }
                }, {
                    label: 'Active phase',
                    type: 'number',
                    attr: 'data-activephase',
                    get: MBE.options.hasAttr,
                    set: function (opt) {
                        $(this).attr('data-activephase', opt.value);
                        MBE.types.ui.wizzardBuildPhases.apply(this, []);
                    }
                }]
            }
        }
    }, 
    
    init: function()
    {
        var self = MBE.types.ui;

        var group = $('<li></li>');
        group.html('UI').prependTo('ul.category');
    
        var items = $('<ul data-type="ui" style="display: none"></ul>');
        for(template in self.templatesName) {
            var tmp = self.templatesName[template].split(/#/);
            var item = $('<li></li>');
            item
                .html(tmp[0])
                .attr({ 'data-template': template, 'draggable': true })
                .data('type', 'ui')
                .appendTo(items);

            if(tmp.length == 2) {
                item.addClass(tmp[1]);
            }
        }
        items.appendTo(group);

        MBE.io.onLoad.push(MBE.types.ui._onLoad);
        MBE.DnD.onDrop.push(MBE.types.ui._drop);
        MBE.onBeforeDelete['ui|data-table'] = MBE.types.ui._beforeDelete;

        $(document)
            .on('click', 'span[data-action="dt-add-action"]', self.dataTableAddAction)
            .on('click', 'span[data-action="dt-remove"]', self.dataTableRemoveAction)
            .on('click', 'span[data-action="dt-advanced"]', self.dataTableAdvancedAction)
            .on('click', '[data-action="dt-select-icon"]', self.dataTableOpenIconDialog)
            .on('change', 'input[name=action_id]', self.dataTableFillIdParam)
            .on('change', 'input[name=action_id], input[name=action_title], input[name=action_idParam], input[name=action_confirm]', self.dataTableBuildActions)
        ;
    },

    _drop: function(target)
    {
        if ($(this).is('[data-uic="ui|horizontal-form-row"]')) {
            MBE.types.ui.buildHorizontalFormRow.apply(this, []);
        }
        if ($(this).is('[data-uic="ui|nv-list"]:empty')) {
            MBE.types.ui.buildNVList.apply(this, []);
        }
        if ($(this).is('[data-uic="ui|data-table"]')) {
            if ($(this).is(':empty'))
                MBE.types.ui.buildDataTable.apply(this, []);
            else
                MBE.types.ui.updateDataTable.apply(this, []);
        }
        if ($(this).is('[data-uic="ui|wizzard"]:empty')) {
            MBE.types.ui.buildWizzard.apply(this, []);
        }
    },

    _beforeDelete: function()
    {
        if ($(this).is('[data-uic="ui|data-table"]')) {
            $(this).parents('.dataTables_wrapper').eq(0).replaceWith(this);
        }
    },

    _onLoad: function()
    {
        $('table.dataTable', MBE.workspace).each(function () {
            MBE.types.ui.initDataTable.apply(this, []);
        });
    },

    buildHorizontalFormRow: function()
    {
        var g = MBE.types.grid;
        var f = MBE.types.form;

        var row = $(f.templates['form-group']);
        var label = $(f.templates['label']);
        var input = $(f.templates['input-text']);
        var column = $(g.templates['column']);
        
        row.attr('data-uic', 'form|form-group');
        label.attr('data-uic', 'form|label').addClass('col-sm-2').appendTo(row);
        column.attr('data-uic', 'grid|column').addClass('col-sm-10').appendTo(row);
        input.attr('data-uic', 'form|input-text').appendTo(column);
        
        $(this).replaceWith(row);
    },

    buildNVList: function()
    {
        var body = $('<tbody />');
        
        var names = ["Platform", "Country", "Year"];
        var values = ["Omnius", "Czech Republic", "2006"];

        for (var i = 0; i < names.length; i++) {
            var row = $('<tr />');

            var nameCell = $('<td />');
            var valueCell = $('<td />');
            
            nameCell.addClass('name-cell').html(names[i]).appendTo(row);
            valueCell.addClass('value-cell').html(values[i]).appendTo(row);

            row.appendTo(body);
        }

        body.appendTo(this);
    },

    /********************************************************************/
    /* DATA TABLE CONTEXT METHODS                                       */
    /********************************************************************/
    buildDataTable: function () {
        var body = $('<tbody />');
        var head = $('<thead />');

        var names = ["Column 1", "Column 2", "Column 3"];

        var row = $('<tr />');
        
        for (var i = 0; i < names.length; i++) {
            var cell = $('<th />');
            cell.html(names[i]).appendTo(row);
        }
        row.appendTo(head);

        for (var i = 1; i <= 3; i++) {
            var row = $('<tr />');

            for(var j = 1; j <= names.length; j++)
            {
                var n = i * j;
                var cell = $('<td />');
                cell.html("Value " + n).appendTo(row);
            }
            row.appendTo(body);
        }

        head.appendTo(this);
        body.appendTo(this);

        if (!$(this).is('[data-dtpaging]')) {
            $(this).attr({
                'data-dtpaging': '1',
                'data-dtinfo': '1',
                'data-dtfilter': '1',
                'data-dtordering': '1',
                'data-dtcolumnfilter': '0',
                'data-dtserverside': '0'
            });
        }
        
        MBE.types.ui.initDataTable.apply(this, []);

        if ($(this).is('[data-actions]')) {
            MBE.types.ui.dataTableBuildActions.apply(this, [true]);
        }
    },

    updateDataTable: function() {
        MBE.types.ui.initDataTable.apply(this, []);
    },

    initDataTable: function() {
        var settings = {
            'destroy': true,
            'paging': $(this).attr('data-dtpaging') == '1',
            'info': $(this).attr('data-dtinfo') == '1',
            'filter': $(this).attr('data-dtfilter') == '1',
            'ordering': $(this).attr('data-dtordering') == '1',
            'select': $(this).attr('data-dtselect') == '1'
        };

        $(this).DataTable(settings);
        $('> tfoot', this).remove();
        
        if ($(this).attr('data-dtcolumnfilter') == '1')
        {
            if (!$('> tfoot', this).length) {
                var foot = $('<tfoot />');
                var row = $('<tr />');

                $('> thead > tr:first-child > th', this).each(function () {
                    var cell = $('<th />');

                    if (!$(this).hasClass('actionHeader')) {
                        var input = $('<input type="text" value="" class="form-control input-sm" placeholder="Hledat v &quot;' + $(this).text() + '&quot;" />');
                        input.appendTo(cell);
                    }
                    else {
                        cell.html('&nbsp;');
                    }
                    
                    cell.appendTo(row);
                });

                row.appendTo(foot)
                $('> thead', this).after(foot);
            }
        }
    },

    dataTableAddAction: function(event, action) {
        var list = $(this).parents('table').eq(0);
        var body = $('tbody', list);

        var iconClass = action ? action.icon : 'fa fa-pencil';
        var actionId = action ? action.action : '';
        var actionIdParam = action && action.idParam ? action.idParam : 'modelId';
        var actionTitle = action ? action.title : '';
        var actionConfirm = action && action.confirm ? action.confirm : '';

        if (action && action.action == 'delete') {
            actionIdParam = 'deleteId';
        }

        var row = $('<tr />');
        var dd = $('<button type="button" class="btn btn-default" style="padding: 3px 10px" data-action="dt-select-icon"><span class="' + iconClass + '"></span> <span class="caret"></span></button>');
        var id = $('<input type="text" class="form-control input-sm" value="' + actionId + '" name="action_id" />');
        var idParam = $('<input type="text" class="form-control input-sm" value="' + actionIdParam + '" name="action_idParam" />');
        var title = $('<input type="text" class="form-control input-sm" value="' + actionTitle + '" name="action_title" />');
        var confirm = $('<input type="hidden" value="'+actionConfirm+'" name="action_confirm" />');
        var move = $('<span class="fa fa-arrows-v fa-fw handle" title="move..." style="margin-right: 5px; margin-top: 9px; cursor: pointer"></span>');
        var conf = $('<span class="fa fa-gear fa-fw" title="advanced..." data-action="dt-advanced" style="margin-right: 5px; margin-top: 9px; cursor: pointer"></span>"');
        var del = $('<span class="fa fa-times fa-fw" title="remove..." data-action="dt-remove" style="margin-top: 9px; cursor: pointer"></span>');

        var cell1 = $('<td />');
        cell1.append(dd).appendTo(row);

        var cell2 = $('<td />');
        cell2.append(id).appendTo(row);

        var cell3 = $('<td />');
        cell3.append(idParam).appendTo(row);

        var cell4 = $('<td />');
        cell4.append(title).append(confirm).appendTo(row);

        var cell5 = $('<td />');
        cell5.append(move).append(conf).append(del).appendTo(row);

        row.appendTo(body);

        body.sortable('refresh');
    },

    dataTableRemoveAction: function() {
        $(this).parents('tr').eq(0).remove();
        MBE.types.ui.dataTableBuildActions.apply(MBE.options.target, [false]);
    },

    dataTableAdvancedAction: function() {
        var targetRow = $(this).parents('tr').eq(0);
        var d = $('<div />');
        var f = $('<fieldset />');
        var group = $('<div class="option-group form-group"></div>');

        f.append('<legend>Advanced action settings</legend>');

        group.append('<label class="control-label col-xs-2">Confirm message</label>');
        group.append('<div class="col-xs-10"><input type="text" value="" class="form-control input-sm"></div>');

        group.find('input')
            .val(targetRow.find('input[name="action_confirm"]').val())
            .change(function () {
                targetRow.find('input[name="action_confirm"]').val(this.value).change();
            });

        f.append(group).appendTo(d);

        d.dialog({
            appendTo: 'body',
            modal: true,
            closeOnEscape: true,
            draggable: false,
            resizable: false,
            width: '40%',
            maxHeight: '60%',
            title: 'Properties...',
            dialogClass: 'dialog-options',
            close: function () { $(this).remove(); }
        });
    },

    dataTableOpenIconDialog: function() {
        var target      = $(this);
        var active      = '';
        var iconList    = {};
        var sSheetList  = document.styleSheets;
        var fontSets    = { 'fa': 'Font Awesome', 'glyphicon': 'Glyphicons' };
        var d           = $('<div />');
        var f           = $('<fieldset />');
        var group       = $('<div class="option-group form-group"></div>');

        f.append('<legend>Icon</legend>');

        for (var fs in fontSets) {
            var list = $('<div class="icon-list col-xs-12 font-set-' + fs + '"></div>');

            for (var sSheet = 0; sSheet < sSheetList.length; sSheet++) {
                var ruleList = document.styleSheets[sSheet].cssRules;
                for (var rule = 0; rule < ruleList.length; rule++) {
                    var text = ruleList[rule].selectorText;
                    var selectors = text ? text.split(/, ?/) : [];
                    for (var si = 0; si < selectors.length; si++)
                    {
                        var selector = selectors[si];
                        var rx = new RegExp('^\.(' + fs + '-[^ ]+)::before$');
                        if (m = rx.exec(selector)) {
                            var btn = $('<a href="#"></a>');
                            btn.append('<span class="' + fs + ' ' + m[1] + '"></span>');
                            btn.append('<span class="icon-name">' + fs + ' ' + m[1] + '</span>');
                            list.append(btn);

                            var test = '.' + fs + '.' + m[1];
                            if (target.find('span').eq(0).is(test)) {
                                btn.addClass('active');
                                active = fs;
                            }
                        }
                    }
                    
                }
            }
            list.on('click', 'a', function () {
                target.find('span').eq(0).removeClass().addClass($(this).find('span').eq(0)[0].className);
                $('.icon-list a').removeClass('active');
                $(this).addClass('active');
                MBE.types.ui.dataTableBuildActions();
                return false;
            });
            iconList[fs] = list;
        }

        var search = $('<input type="text" value="" placeholder="Find icon..." class="form-control input-sm">');

        search.on('input', function () {
            var text = this.value;
            if (text.length) {
                $('.icon-list').find('a').hide().each(function () {
                    if ($('.icon-name', this).text().indexOf(text) != -1) {
                        $(this).show();
                    }
                });
            }
            else {
                $('.icon-list').find('a').show();
            }
        });

        var sets = $('<select class="form-control input-sm"></select>');

        for (fs in fontSets) {
            var option = $('<option></option>');
            option.val(fs).html(fontSets[fs]).appendTo(sets);

            if (active == fs) {
                option.attr('selected', true);
                iconList[fs].show();
            }
            else {
                iconList[fs].hide();
            }
        }

        sets.change(function () {
            $('[class*="font-set-"]').hide();
            $('.font-set-' + this.value).show();
        });

        group.append(sets);
        group.append(search);
        group.append('<div class="clearfix"></div>');
        for (var fs in iconList) {
            group.append(iconList[fs]);
        }

        sets.wrap('<div class="col-xs-4"></div>');
        search.wrap('<div class="col-xs-6"></div>');

        f.append(group).appendTo(d);

        d.dialog({
            appendTo: 'body',
            modal: true,
            closeOnEscape: true,
            draggable: false,
            resizable: false,
            width: '40%',
            maxHeight: '60%',
            title: 'Select icon...',
            dialogClass: 'dialog-options',
            close: function () { $(this).remove(); }
        });
    },

    dataTableFillIdParam: function() {
        var fieldIdParam = $(this).parent().next().find('input');
        if (!fieldIdParam.val().length) {
            fieldIdParam.val(this.value == 'delete' ? 'deleteId' : 'modelId');
        }
        else if (this.value == 'delete' && fieldIdParam.val() != 'deleteId') {
            fieldIdParam.val('deleteId');
        }
    },

    dataTableBuildActions: function(rebuild) {
        var target = $(this);

        if (!target.is('table')) {
            target = $(MBE.options.target);
        }

        var validActions = [];

        if (rebuild !== true) {
            $('tbody > tr', '.dt-action-list').each(function () {
                var icon = $('td', this).eq(0).find('button > span').eq(0)[0].className;
                var id = $('td', this).eq(1).find('input').val();
                var idParam = $('td', this).eq(2).find('input').val();
                var title = $('td', this).eq(3).find('input[type="text"]').val();
                var confirm = $('td', this).eq(3).find('input[type="hidden"]').val();

                if (icon && id && idParam && title) {
                    validActions.push({ 'icon': icon, 'action': id, 'idParam': idParam, 'title': title, 'confirm': confirm });
                }
            });
        }
        else {
            var json = $(this).attr('data-actions').replace(/'/g, '"');
            if (json && json.length) {
                validActions = JSON.parse(json);
            }
        }

        if (validActions.length) {
            if (!$('tbody > tr > td.actionIcons', target).length) {
                $('thead > tr', target).append('<th class="actionHeader">Akce</th>');
                $('tbody > tr', target).append('<td class="actionIcons"></td>');
                $('tfoot > tr', target).append('<th class="actionFooter">&nbsp;</th>');
            }
            $('tbody > tr > td.actionIcons', target).html('');
            for (var i = 0; i < validActions.length; i++) {
                $('tbody > tr > td.actionIcons', target).each(function () {
                    $(this).append('<i class="' + validActions[i].icon + '" data-action="' + validActions[i].action + '" data-idparam="' + validActions[i].idParam + '" data-confirm="' + validActions[i].confirm + '" title="' + validActions[i].title + '"></i>');
                });
            }
        }
        else {
            if ($('tbody tr td.actionIcons', target).length) {
                var index = $('tbody tr td.actionIcons', target).eq(0)[0].cellIndex;
                $('> tbody > tr, > thead > tr, > tfoot > tr', target).each(function() {
                    $('> td, > th', this).eq(index).remove();
                });
            }
        }

        target.attr('data-actions', JSON.stringify(validActions).replace(/"/g, "'"));
        
    },

    /********************************************************************/
    /* WIZZARD CONTEXT METHODS                                          */
    /********************************************************************/
    buildWizzard: function(phasesList) {
        var self = MBE.types.ui;
        var target = $(this);

        var phases = target.attr('data-phases') ? target.attr('data-phases') : 'Fáze 1,Fáze 2,Fáze 3';
        var activePhase = target.attr('data-activephase') ? target.attr('data-activephase') : '1';

        var svg = '';/* +
'<svg class="phase-background" width="846px" height="84px">' +
    '<defs>' +
        '<linearGradient id="grad-light" x1="0%" y1="0%" x2="0%" y2="100%">' +
            '<stop offset="0%" style="stop-color:#dceffa ;stop-opacity:1" />' +
            '<stop offset="100%" style="stop-color:#8dceed;stop-opacity:1" />' +
        '</linearGradient>' + 
        '<linearGradient id="grad-blue" x1="0%" y1="0%" x2="0%" y2="100%">' +
            '<stop offset="0%" style="stop-color:#0099cc;stop-opacity:1" />' + 
            '<stop offset="100%" style="stop-color:#0066aa;stop-opacity:1" />' +
        '</linearGradient>' +
    '</defs>' + 
    '<path d="M0 0 L0 88 L 280 88 L324 44 L280 0 Z" fill="url(#grad-blue)" />' + 
    '<path d="M280 88 L324 44 L280 0 L560 0 L604 44 L560 88 Z" fill="url(#grad-light)" />' +
    '<path d="M560 0 L604 44 L560 88 L850 88 L850 0 Z" fill="url(#grad-light)" />' + 
'</svg>';*/
        
        var body = $(self.templates['wizzard-body']);
        var phase = $(self.templates['wizzard-phase']);
        target
            .append(svg)
            .append(body)
            .attr({
                'data-phases': phases,
                'data-activephase': activePhase
            });

        self.wizzardBuildPhases.apply(this, []);
    },

    wizzardBuildPhases: function()
    {
        var self = MBE.types.ui;
        var target = $(this);

        var phases = target.attr('data-phases').split(';');
        var activePhase = Number(target.attr('data-activephase')) - 1;

        target.find('.phase').remove();

        for (var i = 0; i < phases.length; i++) {
            var phase = $(self.templates['wizzard-phase']);
            phase.find('.phase-icon-number').html(i + 1).end()
                 .find('.phase-label').html(phases[i]).end()
                 .appendTo(target.find('.wizzard-body'));

            if (i < activePhase) {
                phase.addClass('phase-done')
                     .find('.phase-icon-number')
                        .addClass('fa fa-check phase-icon-symbol').removeClass('phase-icon-number').html('');
            }
            if (i == activePhase) {
                phase.addClass('phase-active');
            }
        }

        MBE.DnD.updateDOM();
    }
};

MBE.onBeforeInit.push(MBE.types.ui.init);

MBE.types.athena = {

    data: {},

    templates: {},

    options: {},

    init: function () {
        this.loadGraphList();

        MBE.DnD.onDrop.push(this._onDrop);
        MBE.io.onLoad.push($.proxy(this._onLoad, this));
    },

    loadGraphList: function () {
        var url = '/api/athena/getGraphList';
        $.ajax(url, {
            dataType: 'json',
            async: false,
            success: $.proxy(this.setGraphList, this)
        });
    },

    setGraphList: function (data) {
        var target = $('ul[data-type="athena"]');

        for (var i = 0; i < data.length; i++) {
            var item = $('<li />');
            item.attr('data-template', data[i].Ident).html(data[i].Name);
            item.appendTo(target);

            this.templates[data[i].Ident] = '<div />';
            this.data[data[i].Ident] = data[i];
        }
    },

    build: function (code, id, data) {
        return code.replace(/\{ident\}/g, id).replace(/\{data\}/g, data.split(/\r?\n/).join('\\n'));
    },

    /*************************************************/
    /* EVENTS                                        */
    /*************************************************/
    _onDrop: function () {
        var elm = $(this); 

        if (elm.is('[data-uic^=athena]') && elm.is(':empty')) {
            var self = MBE.types.athena;
            var kv = elm.data('uic').split('|');
            var type = kv[1];
            var data = self.data[type];
            var css = data.Css ? '<style type="text/css" rel="stylesheet">' + self.build(data.Css, data.Ident + '_graph', '') + '</style>' : "";

            elm.css('height', 300).addClass('graphWrapper').attr('id', data.Ident);
            elm.append(self.build(data.Html, data.Ident + '_graph', ''));
            elm.append(css);

            var run = self.build(data.Js, data.Ident + '_graph', data.DemoData);
            MBE.win.eval.apply(MBE.win, [run]);
        }
    },

    _onLoad: function () {
        $('[data-uic^=athena]', MBE.workspace).each($.proxy(function (index, element) {
            var elm = $(element);
            var elmId = elm.attr('id');
            var kv = elm.data('uic').split('|');
            var type = kv[1];
            var data = this.data[type];
            var style = data.Css ? '<style type="text/css" rel="stylesheet">' + this.build(data.Css, elmId + '_graph', '') + '</style>' : '';

            elm.html('');
            elm.append(this.build(data.Html, elmId + '_graph', ''));
            elm.append(style);

            var run = this.build(data.Js, elmId + '_graph', data.DemoData);
            MBE.win.eval.apply(MBE.win, [run]);
        }, this));
    }
};

MBE.onBeforeInit.push($.proxy(MBE.types.athena.init, MBE.types.athena));
var Athena = {

    onInit: [],
    gallery: null,
    svg: null,
    js: null,
    css: null,
    html: null,
    ident: null,
    preview: null,
    data: null,
    dataSource: null,

    jsEditor: null,
    cssEditor: null,
    htmlEditor: null,

    init: function () {

        this.gallery = $('#GalleryItems');
        this.svg = $('.graphPreview');
        this.js = $('.workspace .js textarea');
        this.css = $('.workspace .css textarea');
        this.data = $('.workspace .data textarea');
        this.html = $('.workspace .html textarea');
        this.preview = $('.workspace .preview .wrapper');

        this.ident = $('#Ident');
        
        $(document)
            .on('click', '#GalleryItems .item', this._definitionSelect)
            .on('blur', '#Name', this.makeIdent)
            .on('keyup', '#Name', this.sanitizeName)
            .on('blur', '.workspace .js textarea', $.proxy(this.render, this))
            .on('blur', '.workspace .data textarea', $.proxy(this.render, this))
            .on('blur', '.workspace .html textarea', $.proxy(this.render, this))
            .on('blur', '.workspace .css textarea', $.proxy(this.renderCss, this))
            .on('change', '#Library', $.proxy(this.changeLibrary, this));

        setTimeout($.proxy(this.initEditors, this), 250);
        
        this.changeLibrary();
        this.callHooks(this.onInit);

        this.render();
        this.renderCss();
    },

    initEditors: function () {

        this.jsEditor = CodeMirror.fromTextArea(this.js[0], {
            lineNumbers: true,
            lineWrapping: false,
            matchBrackets: true,
            autoCloseBrackets: true,
            mode: "text/javascript",
            foldGutter: true,
            gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
            extraKeys: {
                "Ctrl-Q": function (cm) { cm.foldCode(cm.getCursor()); },
                "Ctrl-Space": "autocomplete"
            }
        });
        this.jsEditor.setSize(null, this.js.parent().height());
    },

    callHooks: function (hooks, context, params) {
        context = context || this;
        for (var i = 0; i < hooks.length; i++) {
            hooks[i].apply(context, params);
        }
    },

    /*****************************************************/
    /* METHODS                                           */
    /*****************************************************/
    reset: function () {
        var varName = this.ident.val() + '_chart';
        if (typeof window[varName] != 'undefined' && typeof window[varName].destroy != 'undefined') {
            window[varName].destroy();
            delete window[varName];
        }

        this.preview.html(this.build(this.html.val()));
    },

    changeLibrary: function () {
        $('#BtnWizzard').attr('disabled', $('#Library').val() == 'd3');
        this.loadDefinitions();
    },

    renderDefault: function (definitionId) {
        this.js.val($('#' + definitionId).find('.jsSource').html());
        this.css.val($('#' + definitionId).find('.cssSource').html());
        this.data.val($('#' + definitionId).find('.dataSource').text());
        this.html.val($('#' + definitionId).find('.htmlSource').html());
        this.preview.html($('#' + definitionId).find('.htmlSource').html());
        this.jsEditor.setValue(this.js.val());

        this.render();
        this.renderCss();
    },

    loadDefinitions: function () {
        var lib = $('#Library').val();
        this.gallery.html('');

        $('#GraphDefinition #'+lib+' .item').each($.proxy(function (index, element) {
            var name = $(element).data('name');
            var icon = $(element).data('icon');
            var item = $('<a class="item" />');

            item.attr('data-id', $(element).attr('id'))
                .append('<span class="fa ' + icon + '"></span>')
                .append('<span class="label">' + name + '</span>')
                .appendTo(this.gallery);
        }, this));
    },

    render: function () {
        this.reset();
        if (this.jsEditor) {
            this.jsEditor.save();
        }
        try {
            eval.apply(window, [this.setData(this.build(this.js.val()))]);
        }
        catch (e) {
            var error = $('<span class="label label-danger" style="position:absolute;left:100px;top:8px"></span>');
            error.html(e.message);
            $('.workspace .js').append(error);

            setTimeout(function () {
                error.fadeOut('slow', function () { error.remove() });
            }, 2000);
        }
    },

    renderCss: function () {
        $('style#cssPreview').remove();
        if (this.css.val().length) {
            var style = $('<style type="text/css" rel="stylesheet" id="cssPreview"></style>');
            style.html(this.build(this.css.val()));
            style.appendTo('body'); 
        }
    },

    makeIdent: function () {
        var self = Athena;
        var name = this.value;
        name = RemoveDiacritics(name);
        name = name.replace(/ /g, '-');
        name = name.replace(/-{2,}/g, '-');

        self.preview.find('> *').eq(0).attr('id', name.toLowerCase());
        self.ident.val(name.toLowerCase());
        self.render();
        self.renderCss();
    },

    sanitizeName: function (e) {
        if (e.which == 32) {
            this.value = this.value.replace(/ /g, '_');
        }
    },

    build: function (code) {
        return code.replace(/\{ident\}/g, this.ident.val());
    },

    setData: function (code) {
        return code.replace(/\{data\}/g, this.data.val().split(/\n/).join('\\n'));  
    },

    /*****************************************************/
    /* EVENTS                                            */
    /*****************************************************/
    _definitionSelect: function () {
        var self = Athena;
        var definitionId = $(this).data('id');
        self.renderDefault(definitionId);
    }
};

if ($('body').hasClass('athenaForm')) {
    $($.proxy(Athena.init, Athena));
}