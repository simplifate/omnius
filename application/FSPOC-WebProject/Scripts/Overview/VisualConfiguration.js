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
        $(".block, .metablock").each(function (itemIndex, item) {
            instance.draggable(item, { containment: "parent" });
        });
    }
});
