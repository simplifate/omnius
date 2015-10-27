var instance;

jsPlumb.ready(function () {
    instance = jsPlumb.getInstance({
        ConnectionOverlays: [
            ["Arrow", { location: 1 }]
        ],
        Container: "overviewPanel",
        Endpoint: "Blank",
        Anchor: "Continuous",
        Connector: ["Bezier", { curviness: 100 }]
    });
    connectorPaintStyle = {
        lineWidth: 4,
        strokeStyle: "#455d73"
    };
    $(".block").each(function (itemIndex, item) {
        instance.draggable(item, { containment: "parent" });
    });
    instance.connect({
        source: "block1", target: "block2", editable: false, paintStyle: connectorPaintStyle
    });
    instance.connect({
        source: "block2", target: "block3", editable: false, paintStyle: connectorPaintStyle
    });
    instance.connect({
        source: "block2", target: "block4", editable: false, paintStyle: connectorPaintStyle
    });
});
