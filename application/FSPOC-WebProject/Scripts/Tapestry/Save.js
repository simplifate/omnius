var AssociatedPageIds = [];

function SaveBlock(commitMessage) {
    resourceRulesArray = [];
    workflowRulesArray = [];
    portTargetsArray = [];
    saveId = 0;
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
                PageId: currentItem.attr("pageId"),
                ComponentId: currentItem.attr("componentId"),
                TableId: currentItem.attr("tableId"),
                ColumnId: currentItem.attr("columnId"),
                ColumnFilter: currentItem.data("columnFilter")
            });
        });
        currentInstance = currentRule.data("jsPlumbInstance");
        jsPlumbConnections = currentInstance.getAllConnections();
        for (i = 0; i < jsPlumbConnections.length; i++) {
            currentConnection = jsPlumbConnections[i];
            sourceDiv = $(currentConnection.source);
            targetDiv = $(currentConnection.target);
            connectionArray.push({
                Source: sourceDiv.attr("saveId"),
                SourceType: 0,
                SourceSlot: 0,
                Target: targetDiv.attr("saveId"),
                TargetType: 0,
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
            currentSwimlane.find(".swimlaneRolesArea .roleItem").each(function (roleIndex, roleDiv) {
                rolesArray.push($(roleDiv).text());
            });
            currentSwimlane.find(".item").each(function (itemIndex, itemDiv) {
                currentItem = $(itemDiv);
                statesArray = [];
                currentSwimlane.find(".stateItem").each(function(roleIndex, roleDiv) {
                    statesArray.push($(roleDiv).text());
                });

                currentItem.attr("saveId", saveId);
                saveId++;
                itemArray.push({
                    Id: currentItem.attr("saveId"),
                    Label: currentItem.find(".itemLabel").text(),
                    TypeClass: GetItemTypeClass(currentItem),
                    DialogType: currentItem.attr("dialogType"),
                    States : statesArray,
                    PositionX: parseInt(currentItem.css("left")),
                    PositionY: parseInt(currentItem.css("top")),
                    ActionId: currentItem.attr("actionid"),
                    InputVariables: currentItem.data("inputVariables"),
                    OutputVariables: currentItem.data("outputVariables")
                });
            });
            currentSwimlane.find(".symbol").each(function (symbolIndex, symbolDiv) {
                currentSymbol = $(symbolDiv);
                currentSymbol.attr("saveId", saveId);
                saveId++;
                symbolArray.push({
                    Id: currentSymbol.attr("saveId"),
                    Label: currentSymbol.find(".itemLabel").text(),
                    Type: currentSymbol.attr("symbolType"),
                    DialogType: currentSymbol.attr("dialogType"),
                    PositionX: parseInt(currentSymbol.css("left")),
                    PositionY: parseInt(currentSymbol.css("top"))
                });
            });
            swimlanesArray.push({
                SwimlaneIndex: swimlaneIndex,
                Height: parseInt(currentSwimlane.css("height")),
                Roles: rolesArray,
                States : statesArray,
                WorkflowItems: itemArray,
                WorkflowSymbols: symbolArray
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
                    Source: sourceDiv.attr("saveId"),
                    SourceType: sourceDiv.hasClass("symbol") ? 1 : 0,
                    SourceSlot: sourceSlot,
                    Target: targetDiv.attr("saveId"),
                    TargetType: targetDiv.hasClass("symbol") ? 1 : 0,
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
    postData = {
        CommitMessage: commitMessage,
        Name: $("#blockHeaderBlockName").text(),
        ResourceRules: resourceRulesArray,
        WorkflowRules: workflowRulesArray,
        PortTargets: portTargetsArray,
        AssociatedPageIds: AssociatedPageIds,
        AssociatedTableIds: AssociatedTableIds,
        ParentMetablockId: $("#parentMetablockId").val()
    }    
    appId = $("#currentAppId").val();
    blockId = $("#currentBlockId").val();
    $.ajax({
        type: "POST",
        url: "/api/tapestry/apps/" + appId + "/blocks/" + blockId,
        data: postData,
        success: function () {
            ChangedSinceLastSave = false;
            alert("OK");
        },
        error: function () { alert("ERROR") }
    });
}
