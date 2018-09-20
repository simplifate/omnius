var instance;

jsPlumb.ready(function () {
    if (CurrentModuleIs("dbDesignerModule")) {
        instance = jsPlumb.getInstance({
            Endpoint: ["Blank", {}],
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
            instance.detach(con);
            //CurrentConnection = con;
            //editRelationDialog.dialog("open");
        });

        instance.bind("connection", function (info) {
            if ($(info.connection.source).attr("dbColumnType") != $(info.connection.target).attr("dbColumnType")) {
                instance.detach(info.connection);
                alert("These columns have different types. Relation can only be created between columns of the same type.");
                return false;
            }
            info.connection.addClass("relationConnection");
            info.connection.removeOverlay("arrow");
            info.connection.addOverlay(["Arrow", {
                location: 1,
                id: "arrow0",
                length: 8,
                width: 8,
                height: 8,
                foldback: 0.8
            }]);
            //info.connection.addOverlay(["Arrow", {
            //    location: 0,
            //    id: "arrow1",
            //    length: 8,
            //    width: 8,
            //    height: 8,
            //    foldback: 0.8,
            //    direction: -1
            //}]);
            //info.connection.addOverlay(["Label", {
            //    location: 0.1,
            //    id: "label0",
            //    cssClass: "relationLabel",
            //    label: "1"
            //}]);
            //info.connection.addOverlay(["Label", {
            //    location: 0.9,
            //    id: "label1",
            //    cssClass: "relationLabel",
            //    label: "1"
            //}]);
        });

        instance.batch(function () {
            $(".dbTable").each(function (index, element) {
                instance.draggable(element);
            });

            $(".dbColumn").each(function (index, element) {
                AddColumnToJsPlumb(element);
            });
        });
    }
});
