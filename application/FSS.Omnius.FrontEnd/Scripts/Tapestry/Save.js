﻿var AssociatedPageIds = [];

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
