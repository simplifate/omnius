var instance;
var DeleteModeActive = false;

function AddEndpointsByType(newElement) {
    newElementId = newElement.attr("id");
    newElementActType = newElement.attr("acttype");
    newElementInput = ActivityDef[newElementActType].Input;
    newElementOutput = ActivityDef[newElementActType].Output;

    if (newElementInput == "1")
        instance.addEndpoint(newElementId, targetEndpoint, {
            anchor: "TopCenter", uuid: newElementId + "TopCenter"
        });
    else if (newElementInput == "2") {
        instance.addEndpoint(newElementId, targetEndpoint, {
            anchor: "TopLeft", uuid: newElementId + "TopLeft"
        });
        instance.addEndpoint(newElementId, targetEndpoint, {
            anchor: "TopRight", uuid: newElementId + "TopRight"
        });
    }

    if (newElementOutput == "1")
        instance.addEndpoint(newElementId, sourceEndpoint, {
            anchor: "BottomCenter", uuid: newElementId + "BottomCenter"
        });
    else if (newElementOutput == "2") {
        instance.addEndpoint(newElementId, sourceEndpoint, {
            anchor: "BottomLeft", uuid: newElementId + "BottomLeft"
        });
        instance.addEndpoint(newElementId, sourceEndpoint, {
            anchor: "BottomRight", uuid: newElementId + "BottomRight"
        });
    }
    else if (newElementOutput == "3") {
        instance.addEndpoint(newElementId, sourceEndpoint, {
            anchor: "BottomLeft", uuid: newElementId + "BottomLeft"
        });
        instance.addEndpoint(newElementId, sourceEndpoint, {
            anchor: "BottomCenter", uuid: newElementId + "BottomCenter"
        });
        instance.addEndpoint(newElementId, sourceEndpoint, {
            anchor: "BottomRight", uuid: newElementId + "BottomRight"
        });
    }
    else if (newElementOutput == "YesNo") {
        instance.addEndpoint(newElementId, yesEndpoint, {
            anchor: "BottomLeft", uuid: newElementId + "BottomLeft"
        });
        instance.addEndpoint(newElementId, noEndpoint, {
            anchor: "BottomRight", uuid: newElementId + "BottomRight"
        });
    }
    else if (newElementOutput == "OkTimeout") {
        instance.addEndpoint(newElementId, OKEndpoint, {
            anchor: "BottomLeft", uuid: newElementId + "BottomLeft"
        });
        instance.addEndpoint(newElementId, TimeoutEndpoint, {
            anchor: "BottomRight", uuid: newElementId + "BottomRight"
        });
    }
}

jsPlumb.ready(function () {
    instance = jsPlumb.getInstance({
        DragOptions: { cursor: 'pointer', zIndex: 2000 },
        ConnectionOverlays: [
            ["Arrow", { location: 1 }]
        ],
        Container: "workflow-container"
    });
    
    instance.registerConnectionType("basic", basicType);

    instance.batch(function () {

        for (i = 0; i < ActivityDef.length; i++) {
            $("#activityMenu").append('<div class="menuItem" acttype="' + i + '"><strong>' + ActivityDef[i].Name + '</strong><br /><br /></div>');
        }

        $("#activityMenu .menuItem").draggable({
            helper: "clone",
            tolerance: "fit",
            revert: true
        });

        $("#workflow-container").droppable({
            accept: ".menuItem",
            containment: "workflow-container",
            drop: function (e, ui) {
                droppedElement = ui.helper.clone();
                ui.helper.remove();
                $(droppedElement).removeAttr("class");
                $(droppedElement).removeAttr("onClick");
                $(droppedElement).addClass("activity");
                droppedElement.appendTo("#workflow-container");
                droppedElement.css("left", parseInt(droppedElement.css("left")) + $("#workflow-container").scrollLeft());
                droppedElement.css("top", parseInt(droppedElement.css("top")) + $("#workflow-container").scrollTop());
                instance.draggable(droppedElement, {});
                AddEndpointsByType(droppedElement);
                droppedElement.on("mousedown", function () {
                    if (DeleteModeActive == true) {
                        instance.removeAllEndpoints(this, true);
                        $(this).remove();
                    }
                })
            }
        });
    });
});

$(".workflow-container").scroll(function() {
    jsPlumb.repaintEverything();
});