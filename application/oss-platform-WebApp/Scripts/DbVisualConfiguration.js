var instance;

function AddColumnToJsPlumb(item) {
    instance.makeSource(item, {
        anchor: ["Continuous", { faces: ["left", "right"] }],
        faces: ["left", "right"],
        container: "database-container",
        connector: ["Straight", { stub: [0, 0], gap: 5 }],
        connectorStyle: { strokeStyle: "#1092bd", lineWidth: 2, outlineColor: "transparent", outlineWidth: 4 }
    });

    instance.makeTarget(item, {
        dropOptions: { hoverClass: "dragHover" },
        anchor: ["Continuous", { faces: ["left", "right"] }],
        faces: ["left", "right"],
        container: "database-container",
        allowLoopback: false
    });
}

function EditRelation(connection, sourceLabel, targetLabel) {
    connection.removeOverlay("label0");
    connection.removeOverlay("label1");
    connection.addOverlay(["Label", {
        location: 0.1,
        id: "label0",
        cssClass: "relationLabel",
        label: sourceLabel
    }]);
    connection.addOverlay(["Label", {
        location: 0.9,
        id: "label1",
        cssClass: "relationLabel",
        label: targetLabel
    }]);
}

jsPlumb.ready(function() {
    instance = jsPlumb.getInstance({
        Endpoint: ["Blank", { }],
        HoverPaintStyle: { strokeStyle: "#ff4000", lineWidth: 2 },
        ConnectionOverlays: [
            ["Arrow", {
                location: 1,
                id: "arrow",
                length: 14,
                foldback: 0.8
            }],
        ],
        Container: "database-container"
    });

    instance.bind("click", function (con) {
        CurrentConnection = con;
        editRelationDialog.dialog("open");
    });

    instance.bind("connection", function (info) {
        info.connection.addClass("relationConnection");
        info.connection.removeOverlay("arrow");
        info.connection.addOverlay(["Arrow", {
            location: 0,
            id: "arrow0",
            length: 8,
            width: 8,
            height: 8,
            foldback: 0.8,
            direction: -1
        }]);
        info.connection.addOverlay(["Arrow", {
            location: 1,
            id: "arrow1",
            length: 8,
            width: 8,
            height: 8,
            foldback: 0.8
        }]);
        info.connection.addOverlay(["Label", {
            location: 0.1,
            id: "label0",
            cssClass: "relationLabel",
            label: "1"
        }]);
        info.connection.addOverlay(["Label", {
            location: 0.9,
            id: "label1",
            cssClass: "relationLabel",
            label: "1"
        }]);
    });

    instance.batch(function () {
        $(".dbTable").each(function (index, element) {
            instance.draggable(element);
        });

        $(".dbColumn").each(function (index, element) {
            AddColumnToJsPlumb(element);
        });
    });
});
