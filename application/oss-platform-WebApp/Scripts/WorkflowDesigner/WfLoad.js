function LoadWorkflow(commitId) {
    ClearWorkflow();

    $.ajax({
        type: "GET",
        url: "/api/workflows/" + CurrentWorkflowId + "/commits/" + commitId,
        dataType: "json",
        error: function () { alert("ERROR") },
        success: function (data) {
            for (i = 0; i < data.Activities.length; i++) {
                currentActivity = data.Activities[i];
                newActivity = $('<div class="activity" actid="' + currentActivity.Id
                    + '" acttype="' + currentActivity.ActType + '"><strong>'
                    + ActivityDef[currentActivity.ActType].Name + '</strong><br /><br /></div>');
                $("#workflow-container").append(newActivity);
                newActivity.css("left", currentActivity.PositionX);
                newActivity.css("top", currentActivity.PositionY);
                instance.draggable(newActivity.get(0), {});
                AddEndpointsByType(newActivity);
                newActivity.on("mousedown", function () {
                    if (DeleteModeActive == true) {
                        instance.removeAllEndpoints(this, true);
                        $(this).remove();
                    }
                })
            }
            for (i = 0; i < data.Connections.length; i++) {
                currentConnection = data.Connections[i];
                sourceDiv = $("#workflow-container .activity[actid='" + currentConnection.Source + "']");
                targetDiv = $("#workflow-container .activity[actid='" + currentConnection.Target + "']");
                targetInputType = ActivityDef[targetDiv.attr("actType")].Input;
                sourceOutputType = ActivityDef[sourceDiv.attr("actType")].Output;

                if (targetInputType == "2") {
                    if (currentConnection.TargetSlot == 0)
                        targetEndpointName = "TopLeft";
                    else
                        targetEndpointName = "TopRight";
                }
                else
                    targetEndpointName = "TopCenter";

                if (sourceOutputType == "1")
                    sourceEndpointName = "BottomCenter";
                else if (sourceOutputType == "2" || sourceOutputType == "YesNo" || sourceOutputType == "OkTimeout") {
                    if (currentConnection.SourceSlot == 0)
                        sourceEndpointName = "BottomLeft";
                    else
                        sourceEndpointName = "BottomRight";
                }
                else if (sourceOutputType == "3") {
                    if (currentConnection.SourceSlot == 0)
                        sourceEndpointName = "BottomLeft";
                    else if (currentConnection.SourceSlot == 1)
                        sourceEndpointName = "BottomCenter";
                    else
                        sourceEndpointName = "BottomRight";
                }

                instance.connect({ uuids: [sourceDiv.attr("id") + sourceEndpointName, targetDiv.attr("id") + targetEndpointName], editable: true });
            }
        }
    });
};
        