basicType = {
    connector: "StateMachine",
    paintStyle: { strokeStyle: "red", lineWidth: 2 },
    hoverPaintStyle: { strokeStyle: "blue" },
};
connectorPaintStyle = {
    lineWidth: 2,
    strokeStyle: "#467ea8",
    joinstyle: "round"
};
connectorHoverStyle = {
    lineWidth: 2,
    strokeStyle: "#dc555f"
};
endpointHoverStyle = {
    fillStyle: "#dc555f",
    strokeStyle: "#dc555f"
};
sourceEndpoint = {
    endpoint: "Rectangle",
    paintStyle: { fillStyle: "#f98e4b", width: 10, height: 18 },
    isSource: true,
    connector: ["Straight", { stub: [0, 0], gap: 10 }],
    connectorStyle: connectorPaintStyle,
    hoverPaintStyle: endpointHoverStyle,
    connectorHoverStyle: connectorHoverStyle,
    cssClass: "sourceEndpoint",
    dragOptions: {}
};
yesEndpoint = $.extend({}, sourceEndpoint, {
    overlays: [
        ["Label", {
            location: [0.7, 1.5],
            label: "Yes",
            cssClass: "endpointSourceLabel"
        }]
    ]
});
noEndpoint = $.extend({}, sourceEndpoint, {
    paintStyle: { fillStyle: "#f98e4b", width: 18, height: 10 },
    overlays: [
        ["Label", {
            location: [0.7, 2],
            label: "No",
            cssClass: "endpointSourceLabel"
        }]
    ]
});
dataSourceConnectorPaintStyle = {
    lineWidth: 2,
    strokeStyle: "#467ea8",
    joinstyle: "round",
    "stroke-dasharray": "10, 5"
};
dataSourceEndpoint = $.extend({}, sourceEndpoint, {
    paintStyle: { fillStyle: "#f98e4b", width: 10, height: 18 },
    connectorStyle: dataSourceConnectorPaintStyle,
});

jsPlumb.ready(function () {
    if (CurrentModuleIs("tapestryModule")) {

        $(".resourceRule, .workflowRule").each(function (ruleIndex, rule) {
            currentInstance = CreateJsPlumbInstanceForRule(rule);
        });
        $("#resRule1").data("jsPlumbInstance").connect({
            source: "resItem1", target: "resItem2", anchors: ["RightMiddle", "LeftMiddle"], editable: false, connector: "Straight",
            paintStyle: { lineWidth: 2, strokeStyle: '#54c6f0' },
        }).removeAllOverlays();
        $("#resRule2").data("jsPlumbInstance").connect({
            source: "resItem3", target: "resItem4", anchors: ["RightMiddle", "LeftMiddle"], editable: false, connector: "Straight",
            paintStyle: { lineWidth: 2, strokeStyle: '#54c6f0' },
        }).removeAllOverlays();
        $("#resRule2").data("jsPlumbInstance").connect({
            source: "resItem4", target: "resItem5", anchors: ["RightMiddle", "LeftMiddle"], editable: false, connector: "Straight",
            paintStyle: { lineWidth: 2, strokeStyle: '#54c6f0' },
        }).removeAllOverlays();

        $("#wfRule1").data("jsPlumbInstance").connect({
            source: "wfItem1", target: "wfItem2", anchors: ["RightMiddle", "LeftMiddle"], editable: false, connector: "Straight",
            paintStyle: { lineWidth: 2, strokeStyle: '#54c6f0' },
        });
        $("#wfRule1").data("jsPlumbInstance").connect({
            source: "wfItem6", target: "wfItem7", anchors: ["BottomCenter", "TopCenter"], editable: false, connector: "Straight",
            paintStyle: { lineWidth: 2, strokeStyle: '#54c6f0', "stroke-dasharray": "7, 7" },
        });
        $("#wfRule1").data("jsPlumbInstance").connect({
            source: "wfItem3", target: "wfItem4", anchors: ["RightMiddle", "LeftMiddle"], editable: false, connector: "Straight",
            paintStyle: { lineWidth: 2, strokeStyle: '#54c6f0' },
        });
        $("#wfRule1").data("jsPlumbInstance").connect({
            source: "wfItem2", target: "wfItem5", anchors: ["RightMiddle", "LeftMiddle"], editable: false, connector: "Straight",
            paintStyle: { lineWidth: 2, strokeStyle: '#54c6f0' },
        });

            /*
            $(rule).find(".item").each(function (itemIndex, item) {
                AddIconToItem(item);
                AddToJsPlumb(currentInstance, item);
            });
            $(rule).find(".decisionRhombus").each(function (itemIndex, item) {
                AddToJsPlumb(currentInstance, item);
            });
            */
    }
});
