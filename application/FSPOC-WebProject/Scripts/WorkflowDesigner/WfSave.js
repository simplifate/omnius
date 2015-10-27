function SaveWorkflow(commitMessage) {
    activityDivs = $("#workflow-container .activity");
    activityArray = [];
    connectionArray = [];

    jQuery.each(activityDivs, function (i, val) {
        currentDiv = $(this);
        activityArray.push({
            Id: i,
            ActType: currentDiv.attr("acttype"),
            PositionX: parseInt(currentDiv.css("left")),
            PositionY: parseInt(currentDiv.css("top"))
        });
        currentDiv.attr("actid", i);
    });

    jsPlumbConnections = instance.getAllConnections();
    for (i = 0; i < jsPlumbConnections.length; i++)
    {
        currentConnection = jsPlumbConnections[i];
        sourceDiv = $(currentConnection.source);
        targetDiv = $(currentConnection.target);
        srcEndpointUuid = currentConnection.endpoints[0].getUuid();
        targetEndpointUuid = currentConnection.endpoints[1].getUuid();

        if (srcEndpointUuid.indexOf("BottomRight") != -1)
            sourceSlot = 2;
        else if (srcEndpointUuid.indexOf("BottomCenter") != -1)
            sourceSlot = 1;
        else
            sourceSlot = 0;

        if (targetEndpointUuid.indexOf("TopRight") != -1)
            targetSlot = 1;
        else
            targetSlot = 0;

        connectionArray.push({
            Source: sourceDiv.attr("actid"),
            SourceSlot: sourceSlot,
            Target: targetDiv.attr("actid"),
            TargetSlot: targetSlot
        });
    }

    postData = {
        CommitMessage: commitMessage,
        Activities: activityArray,
        Connections: connectionArray
    }

    $.ajax({
        type: "POST",
        url: "/api/workflows/" + CurrentWorkflowId + "/commits",
        data: postData,
        success: function(){ alert("OK") },
        error: function() { alert("ERROR") }
    });
}