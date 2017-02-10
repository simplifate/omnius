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
