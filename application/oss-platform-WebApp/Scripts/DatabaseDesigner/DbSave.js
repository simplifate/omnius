function SaveDbScheme() {
    columnIdCounter = 0;
    tableArray = [];
    relationArray = [];
    $("#database-container .dbTable").each(function (tableIndex, tableDiv) {
        columnArray = [];
        $(tableDiv).attr("dbTableId", tableIndex);
        $(tableDiv).find(".dbColumn").each(function (columnIndex, columnDiv) {
            columnArray.push({
                Id: columnIdCounter,
                Name: $(columnDiv).find(".dbColumnName").text(),
                Type: $(columnDiv).attr("dbColumnType"),
                PrimaryKey: $(columnDiv).hasClass("dbPrimaryKey"),
                AllowNull: ($(columnDiv).attr("dbAllowNull") == "true")});
            $(columnDiv).attr("dbColumnId", columnIdCounter);
            columnIdCounter++;
        });
        tableArray.push({
            Id: tableIndex,
            Name: $(tableDiv).find(".dbTableName").text(),
            PositionX: parseInt($(tableDiv).css("left")),
            PositionY: parseInt($(tableDiv).css("top")),
            Columns: columnArray
        });
    });
    jsPlumbConnections = instance.getAllConnections();

    for (i = 0; i < jsPlumbConnections.length; i++) {
        currentConnection = jsPlumbConnections[i];
        sourceDiv = $(currentConnection.source);
        targetDiv = $(currentConnection.target);
        relationArray.push({
            LeftTable: sourceDiv.parents(".dbTable").attr("dbTableId"),
            rightTable: targetDiv.parents(".dbTable").attr("dbTableId"),
            LeftColumn: sourceDiv.attr("dbcolumnid"),
            RightColumn: targetDiv.attr("dbcolumnid"),
            Type: $(currentConnection).data("relationType")
        });
    }
    postData = {
        Tables: tableArray,
        Relations: relationArray
    }
    $.ajax({
        type: "POST",
        url: "/api/database",
        data: postData,
        success: function () { alert("OK") },
        error: function () { alert("ERROR") }
    });
}