﻿function LoadDbScheme(commitId) {
    pageSpinner.show();
    appId = $("#currentAppId").val();
    currentUserId = $("#currentUserId").val();

    $.ajax({
        type: "GET",
        url: "/api/database/apps/" + appId + "/commits/" + commitId,
        dataType: "json",
        complete: function () {
            pageSpinner.hide()
        },
        success: function (data) {
          
            ClearDbScheme();
            for (i = 0; i < data.Tables.length; i++) {
                newTable = $('<div class="dbTable"><div class="dbTableHeader"><div class="deleteTableIcon fa fa-remove"></div><div class="dbTableName">'
                    + data.Tables[i].Name + '</div><div class="editTableIcon fa fa-pencil"></div><div class="addColumnIcon fa fa-plus"></div></div>'
                    + '<div class="dbTableBody"><div class="dbColumn idColumn dbPrimaryKey" dbColumnType="integer" dbColumnId="'
                    + data.Tables[i].Columns[0].Id + '"><div class="dbColumnName">id</div></div></div>'
                    + '<div class="dbTableIndexArea"></div></div>');
                $("#database-container").append(newTable);
                $(".editTableIcon").on("click", function () {
                    CurrentTable = $(this).parents(".dbTable");
                    editTableDialog.dialog("open");
                });
                newTable.find(".deleteTableIcon").on("click", function () {
                    $(this).parents(".dbTable").remove();
                    instance.removeAllEndpoints($(this).parents(".dbTable"), true);
                });
                newTable.find(".addColumnIcon").on("click", function () {
                    addColumnDialog.data("currentTable", $(this).parents(".dbTable"));
                    addColumnDialog.dialog("open");
                })
                newTable.find(".deleteColumnIcon").on("mousedown", function () {
                    $(this).parents(".dbColumn").remove();
                    instance.removeAllEndpoints($(this).parents(".dbColumn"), true);
                    return false;
                });
                newTable.find(".editColumnIcon").on("mousedown", function () {
                    CurrentColumn = $(this).parents(".dbColumn");
                    editColumnDialog.dialog("open");
                    return false;
                });
                newTable.css("left", data.Tables[i].PositionX);
                newTable.css("top", data.Tables[i].PositionY);
                instance.draggable(newTable);
                for (j = 1; j < data.Tables[i].Columns.length; j++) {
                    if (data.Tables[i].Columns[j].DefaultValue != null)
                        defaultValue = data.Tables[i].Columns[j].DefaultValue;
                    else
                        defaultValue = "";
                    newColumn = $('<div class="dbColumn"><div class="deleteColumnIcon fa fa-remove"></div><div class="dbColumnName">'
                        + data.Tables[i].Columns[j].Name + '</div><div class="editColumnIcon fa fa-pencil"></div></div>');
                    newColumn.attr("dbColumnType", data.Tables[i].Columns[j].Type);
                    newColumn.attr("dbColumnId", data.Tables[i].Columns[j].Id);
                    newColumn.data("dbUnique", data.Tables[i].Columns[j].Unique);
                    newColumn.data("dbAllowNull", data.Tables[i].Columns[j].AllowNull);
                    newColumn.data("dbDefaultValue", defaultValue);
                    newColumn.data("dbColumnLength", data.Tables[i].Columns[j].ColumnLength);
                    newColumn.data("dbColumnLengthMax", data.Tables[i].Columns[j].ColumnLengthIsMax);
                    newColumn.data("dbColumnDisplayName", data.Tables[i].Columns[j].DisplayName);

                    newColumn.children(".deleteColumnIcon").on("mousedown", function () {
                        $(this).parents(".dbColumn").remove();
                        instance.removeAllEndpoints($(this).parents(".dbColumn"), true);
                        instance.recalculateOffsets();
                        instance.repaintEverything();
                        return false;
                    });
                    newColumn.children(".editColumnIcon").on("mousedown", function () {
                        CurrentColumn = $(this).parents(".dbColumn");
                        editColumnDialog.dialog("open");
                        return false;
                    });
                    newTable.children(".dbTableBody").append(newColumn);
                    if (data.Tables[i].Columns[j].PrimaryKey) {
                        newColumn.addClass("dbPrimaryKey");
                    }
                    newColumn.attr("dbColumnType", data.Tables[i].Columns[j].Type);
                }
                AddColumnToJsPlumb(newTable.find(".dbColumn"));
                for (j = 0; j < data.Tables[i].Indices.length; j++) {
                    indexLabel = "Index: ";
                    for (k = 0; k < data.Tables[i].Indices[j].ColumnNames.length - 1; k++)
                        indexLabel += data.Tables[i].Indices[j].ColumnNames[k] + ", ";
                    indexLabel += data.Tables[i].Indices[j].ColumnNames[data.Tables[i].Indices[j].ColumnNames.length - 1];
                    if (data.Tables[i].Indices[j].Unique)
                        indexLabel += " - unique";
                    newIndex = $('<div class="dbIndex"><div class="deleteIndexIcon fa fa-remove"></div><div class="dbIndexText">' + indexLabel + '</div><div class="editIndexIcon fa fa-pencil"></div></div>');
                    newIndex.data("indexName", data.Tables[i].Indices[j].Name);
                    newIndex.data("indexColumnArray", data.Tables[i].Indices[j].ColumnNames);
                    newIndex.data("unique", data.Tables[i].Indices[j].Unique);
                    newIndex.children(".deleteIndexIcon").on("mousedown", function () {
                        $(this).parents(".dbIndex").remove();
                        return false;
                    });
                    newIndex.children(".editIndexIcon").on("mousedown", function () {
                        CurrentIndex = $(this).parents(".dbIndex");
                        CurrentTable = $(this).parents(".dbTable");
                        editIndexDialog.dialog("open");
                        return false;
                    });
                    newTable.children(".dbTableIndexArea").append(newIndex);
                }
            }
            for (i = 0; i < data.Relations.length; i++) {
                sourceDiv = $("#database-container .dbColumn[dbColumnId='" + data.Relations[i].SourceColumn + "']");
                targetDiv = $("#database-container .dbColumn[dbColumnId='" + data.Relations[i].TargetColumn + "']");
                newConnection = instance.connect({ source: sourceDiv.attr("id"), target: targetDiv.attr("id"), editable: true });
                $(newConnection).data("relationType", data.Relations[i].Type);
                switch (data.Relations[i].Type) {
                    case 2:
                        EditRelation(newConnection, "1", "N");
                        break;
                    case 3:
                        EditRelation(newConnection, "N", "1");
                        break;
                    case 4:
                        EditRelation(newConnection, "M", "N");
                        break;
                }
            }
            for (i = 0; i < data.Views.length; i++) {
                newView = $('<div class="dbView"><div class="dbViewHeader"><div class="deleteViewIcon fa fa-remove"></div>'
                    + '<div class="dbViewName">View: ' + data.Views[i].Name + '</div><div class="editViewIcon fa fa-pencil"></div></div></div>');

                $("#database-container").append(newView);
                newView.find(".editViewIcon").on("click", function () {
                    CurrentView = $(this).parents(".dbView");
                    editViewDialog.dialog("open");
                });
                newView.find(".deleteViewIcon").on("click", function () {
                    $(this).parents(".dbView").remove();
                });
                newView.css("left", data.Views[i].PositionX);
                newView.css("top", data.Views[i].PositionY);
                newView.data("dbViewName", data.Views[i].Name);
                newView.data("dbViewQuery", data.Views[i].Query);
                instance.draggable(newView);
            }
        }
    });
}
