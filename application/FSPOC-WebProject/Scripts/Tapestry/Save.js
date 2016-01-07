function SaveBlock(commitMessage) {
    ruleArray = [];
    saveId = 0;
    $("#rulesPanel .rule").each(function (ruleIndex, ruleDiv) {
        itemArray = [];
        operatorArray = [];
        connectionArray = [];
        currentRule = $(ruleDiv);
        currentRule.find(".item").each(function (itemIndex, itemDiv) {
            currentItem = $(itemDiv);
            currentItem.attr("saveId", saveId);
            saveId++;
            typeClass = "";
            if (currentItem.hasClass("attribute")) {
                typeClass = "attribute";
            }
            else if (currentItem.hasClass("port")) {
                typeClass = "port";
            }
            else if (currentItem.hasClass("role")) {
                typeClass = "role";
            }
            else if (currentItem.hasClass("view")) {
                typeClass = "view";
            }
            else if (currentItem.hasClass("action")) {
                typeClass = "action";
            }
            else if (currentItem.hasClass("state")) {
                typeClass = "state";
            }
            itemArray.push({
                Id: currentItem.attr("saveId"),
                Label: currentItem.text(),
                TypeClass: typeClass,
                IsDataSource: currentItem.hasClass("dataSource"),
                DialogType: currentItem.attr("dialogType"),
                PositionX: parseInt(currentItem.css("left")),
                PositionY: parseInt(currentItem.css("top"))
            });
        });
        currentRule.find(".operatorSymbol").each(function (operatorIndex, operatorDiv) {
            currentOperator = $(operatorDiv);
            currentOperator.attr("saveId", saveId);
            saveId++;
            operatorType = "";
            if (currentOperator.hasClass("decisionRhombus")) {
                operatorType = "decision";
            }
            else if (currentOperator.hasClass("conditionEllipse")) {
                operatorType = "condition";
            }
            operatorArray.push({
                Id: currentOperator.attr("saveId"),
                Type: operatorType,
                DialogType: currentOperator.attr("dialogType"),
                PositionX: parseInt(currentOperator.css("left")),
                PositionY: parseInt(currentOperator.css("top"))
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
            connectionArray.push({
                Source: sourceDiv.attr("saveId"),
                Target: targetDiv.attr("saveId"),
                SourceSlot: sourceSlot
            });
        }
        ruleArray.push({
            Name: currentRule.find(".ruleHeader").text(),
            Width: parseInt(currentRule.css("width")),
            Height: parseInt(currentRule.css("height")),
            PositionX: parseInt(currentRule.css("left")),
            PositionY: parseInt(currentRule.css("top")),
            Items: itemArray,
            Operators: operatorArray,
            Connections: connectionArray
        });
    });

    postData = {
        CommitMessage: commitMessage,
        Name: $("#headerBlockName").text(),
        AssociatedTableName: $("#headerTableName").text(),
        AssociatedTableId: $("#associatedTableId").val(),
        Rules: ruleArray,
        ParentMetablockId: $("#parentMetablockId").val()
    }
    appId = $("#currentAppId").val();
    blockId = $("#currentBlockId").val();
    $.ajax({
        type: "POST",
        url: "/api/tapestry/apps/" + appId + "/blocks/" + blockId,
        data: postData,
        success: function () { alert("OK") },
        error: function () { alert("ERROR") }
    });
}
