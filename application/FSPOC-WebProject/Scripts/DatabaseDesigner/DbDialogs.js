 var CurrentTable, CurrentColumn, CurrentConnection, CurrentView, CurrentIndex;

$(function () {
    if (CurrentModuleIs("dbDesignerModule")) {
        addTableDialog = $("#add-table-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 150,
            buttons: {
                "Add": function () {
                    addTableDialog_SubmitData();
                },
                Cancel: function () {
                    addTableDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        addTableDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                addTableDialog.find("#new-table-name").val("");
            }
        });
        function addTableDialog_SubmitData() {
            AddTable(addTableDialog.find("#new-table-name").val());
            addTableDialog.dialog("close");
        }

        editTableDialog = $("#edit-table-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 170,
            buttons: {
                "Save": function () {
                    editTableDialog_SubmitData();
                },
                Cancel: function () {
                    editTableDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        editTableDialog_SubmitData();
                        return false;
                    }
                });
                $(this).find("#add-index-button").on("click", function () {
                    addIndexDialog.dialog("open");
                });
            },
            open: function () {
                editTableDialog.find("#table-name").val(CurrentTable.find(".dbTableName").text());
            }
        });
        function editTableDialog_SubmitData() {
            CurrentTable.find(".dbTableName").text(editTableDialog.find("#table-name").val());
            editTableDialog.dialog("close");
        }

        addColumnDialog = $("#add-column-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 330,
            buttons: {
                "Add": function () {
                    addColumnDialog_SubmitData();
                },
                Cancel: function () {
                    addColumnDialog.dialog("close");
                }
            },
            create: function () {
                for (i = 0; i < SqlServerDataTypes.length; i++) {
                    $("#add-column-dialog #column-type-dropdown").append(
                        $('<option value="' + SqlServerDataTypes[i][0] + '">' + SqlServerDataTypes[i][1] + '</option>'));
                }
                $("#add-column-dialog #column-type-dropdown").change(function () {
                    CheckColumnLengthSupport(addColumnDialog, this.value);
                    if (addColumnDialog.find("#column-length-max").is(":checked")) {
                        addColumnDialog.find("#column-length").hide();
                    }
                });
                $("#add-column-dialog #column-length-max").change(function () {
                    if ($(this).is(":checked")) {
                        addColumnDialog.find("#column-length").hide();
                    } else {
                        addColumnDialog.find("#column-length").show();
                    }
                });
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        addColumnDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                addColumnDialog.find("#column-name").val("");
                addColumnDialog.find("#column-display-name").val("");
                addColumnDialog.find("#primary-key-checkbox").prop("checked", false);
                addColumnDialog.find("#unique-checkbox").prop("checked", false);
                addColumnDialog.find("#allow-null-checkbox").prop("checked", false);
                addColumnDialog.find("#column-type-dropdown").val("varchar");
                addColumnDialog.find("#default-value").val("");
                addColumnDialog.find("#column-length").val(100);
                addColumnDialog.find("#column-length-max").prop("checked", true);
                addColumnDialog.find("#columnLengthNotSupported").hide();
                CheckColumnLengthSupport(addColumnDialog, "varchar");
                addColumnDialog.find("#column-length").hide();
            }
        });
        function addColumnDialog_SubmitData() {
            AddColumn(addColumnDialog.data("currentTable"),
                addColumnDialog.find("#column-name").val(),
                addColumnDialog.find("#column-type-dropdown").val(),
                addColumnDialog.find("#primary-key-checkbox").prop("checked"),
                addColumnDialog.find("#allow-null-checkbox").prop("checked"),
                addColumnDialog.find("#default-value").val(),
                addColumnDialog.find("#column-length").val(),
                addColumnDialog.find("#column-length-max").prop("checked"),
                addColumnDialog.find("#unique-checkbox").prop("checked"),
                addColumnDialog.find("#column-display-name").val());
            addColumnDialog.dialog("close");
        }

        editColumnDialog = $("#edit-column-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 330,
            buttons: {
                "Save": function () {
                    editColumnDialog_SubmitData();
                },
                Cancel: function () {
                    editColumnDialog.dialog("close");
                }
            },
            create: function () {
                for (i = 0; i < SqlServerDataTypes.length; i++) {
                    $("#edit-column-dialog #column-type-dropdown").append(
                        $('<option value="' + SqlServerDataTypes[i][0] + '">' + SqlServerDataTypes[i][1] + '</option>'));
                }
                $("#edit-column-dialog #column-type-dropdown").change(function () {
                    CheckColumnLengthSupport(editColumnDialog, this.value);
                    if (editColumnDialog.find("#column-length-max").is(":checked")) {
                        editColumnDialog.find("#column-length").hide();
                    }
                });
                $("#edit-column-dialog #column-length-max").change(function () {
                    if ($(this).is(":checked")) {
                        editColumnDialog.find("#column-length").hide();
                    } else {
                        editColumnDialog.find("#column-length").show();
                    }
                });
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        editColumnDialog_SubmitData();
                        return false;
                    }
                });
            },
            open: function () {
                editColumnDialog.find("#column-name").val(CurrentColumn.find(".dbColumnName").text());
                editColumnDialog.find("#column-display-name").val(CurrentColumn.data("dbColumnDisplayName"));
                editColumnDialog.find("#primary-key-checkbox").prop("checked", CurrentColumn.hasClass("dbPrimaryKey"));
                editColumnDialog.find("#unique-checkbox").prop("checked", CurrentColumn.data("dbUnique"));
                editColumnDialog.find("#allow-null-checkbox").prop("checked", CurrentColumn.data("dbAllowNull"));
                editColumnDialog.find("#column-type-dropdown").val(CurrentColumn.attr("dbColumnType"));
                editColumnDialog.find("#default-value").val(CurrentColumn.data("dbDefaultValue"));
                editColumnDialog.find("#column-length").val(CurrentColumn.data("dbColumnLength"));
                editColumnDialog.find("#column-length-max").prop("checked", CurrentColumn.data("dbColumnLengthMax"));
                CheckColumnLengthSupport(editColumnDialog, CurrentColumn.attr("dbColumnType"));
                if (CurrentColumn.data("dbColumnLengthMax"))
                    editColumnDialog.find("#column-length").hide();
            }
        });
        function editColumnDialog_SubmitData() {
            CurrentColumn.find(".dbColumnName").text(editColumnDialog.find("#column-name").val());
            CurrentColumn.attr("dbColumnType", editColumnDialog.find("#column-type-dropdown").val());
            CurrentColumn.data("dbUnique", editColumnDialog.find("#unique-checkbox").prop("checked"));
            CurrentColumn.data("dbAllowNull", editColumnDialog.find("#allow-null-checkbox").prop("checked"));
            CurrentColumn.data("dbDefaultValue", editColumnDialog.find("#default-value").val());
            CurrentColumn.data("dbColumnLength", editColumnDialog.find("#column-length").val());
            CurrentColumn.data("dbColumnLengthMax", editColumnDialog.find("#column-length-max").prop("checked"));
            CurrentColumn.data("dbColumnDisplayName", editColumnDialog.find("#column-display-name").val());
            if (CurrentColumn.hasClass("dbPrimaryKey") && !editColumnDialog.find("#primary-key-checkbox").prop("checked"))
                CurrentColumn.removeClass("dbPrimaryKey");
            else if (!CurrentColumn.hasClass("dbPrimaryKey") && editColumnDialog.find("#primary-key-checkbox").prop("checked")) {
                //CurrentColumn.parents(".dbTable").find(".dbColumn").removeClass("dbPrimaryKey"); // Uncomment this line to allow only one primary key per table
                CurrentColumn.addClass("dbPrimaryKey");
            }
            editColumnDialog.dialog("close");
        }

        editRelationDialog = $("#edit-relation-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 250,
            buttons: {
                "Save": function () {
                    editRelationDialog_SubmitData()
                },
                Cancel: function () {
                    editRelationDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        editRelationDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                if ($(CurrentConnection).data("relationType"))
                    editRelationDialog.find("input:radio[value=" + $(CurrentConnection).data("relationType") + "]").prop("checked", "checked");
                else
                    editRelationDialog.find("input:radio[value=1]").prop("checked", "checked");
            }
        });
        function editRelationDialog_SubmitData() {
            $(CurrentConnection).data("relationType", editRelationDialog.find("input[type='radio']:checked").val());
            switch (editRelationDialog.find("input[type='radio']:checked").val()) {
                case "1":
                    EditRelation(CurrentConnection, "1", "1");
                    break;
                case "2":
                    EditRelation(CurrentConnection, "1", "N");
                    break;
                case "3":
                    EditRelation(CurrentConnection, "N", "1");
                    break;
                case "4":
                    EditRelation(CurrentConnection, "M", "N");
                    break;
                case "Delete":
                    instance.detach(CurrentConnection);
                    break;
            }
            editRelationDialog.dialog("close");
        }

        historyDialog = $("#history-dialog").dialog({
            autoOpen: false,
            width: 700,
            height: 540,
            buttons: {
                "Load": function () {
                    historyDialog_SubmitData();
                },
                Cancel: function () {
                    historyDialog.dialog("close");
                }
            },
            open: function (event, ui) {
                historyDialog.data("selectedCommitId", null);
                appId = $("#currentAppId").val();
                $.ajax({
                    type: "GET",
                    url: "/api/database/apps/" + appId + "/commits",
                    dataType: "json",
                    error: function () { alert("Error loading commit history") },
                    success: function (data) {
                        historyDialog.find("#commit-table:first tbody:nth-child(2) tr").remove();
                        tbody = historyDialog.find("#commit-table tbody:nth-child(2)");
                        commitIdArray = [];

                        // Fill in the history rows
                        for (i = 0; i < data.length; i++) {
                            commitIdArray.push(data[i].Id);
                            if (data[i].CommitMessage != null)
                                tbody.append($('<tr class="commitRow"><td>' + data[i].TimeString
                                    + '</td><td>' + data[i].CommitMessage + '</td></tr>'));
                            else
                                tbody.append($('<tr class="commitRow"><td>' + data[i].TimeString
                                    + '</td><td style="color: darkgrey;">(no message)</td></tr>'));
                        }

                        // Highlight the selected row
                        $(document).on('click', 'tr.commitRow', function (event) {
                            historyDialog.find("#commit-table tbody:nth-child(2) tr").removeClass("highlightedCommitRow");
                            $(this).addClass("highlightedCommitRow");
                            var rowIndex = $(this).index();
                            historyDialog.data("selectedCommitId", commitIdArray[rowIndex]);
                        });
                    }
                });
            }
        });
        function historyDialog_SubmitData() {
            if (historyDialog.data("selectedCommitId")) {
                LoadDbScheme(historyDialog.data("selectedCommitId"));
                historyDialog.dialog("close");
            }
            else
                alert("Please select a commit");
        }
        saveDialog = $("#save-dialog").dialog({
            autoOpen: false,
            width: 400,
            height: 190,
            buttons: {
                "Save": function () {
                    saveDialog_SubmitData();
                },
                Cancel: function () {
                    saveDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        saveDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                saveDialog.find("#message").val("");
            }
        });
        function saveDialog_SubmitData() {
            saveDialog.dialog("close");
            SaveDbScheme(saveDialog.find("#message").val());
        }

        addViewDialog = $("#add-view-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 310,
            buttons: {
                "Add": function () {
                    addViewDialog_SubmitData();
                },
                Cancel: function () {
                    addViewDialog.dialog("close");
                }
            },
            create: function () {
                $(this).find("#new-view-name").keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        addViewDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                addViewDialog.find("#new-view-name").val("");
                addViewDialog.find("#new-view-query").val("");
            }
        });
        function addViewDialog_SubmitData() {
            AddView(addViewDialog.find("#new-view-name").val(),
                addViewDialog.find("#new-view-query").val());
            addViewDialog.dialog("close");
        }

        editViewDialog = $("#edit-view-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 310,
            buttons: {
                "Save": function () {
                    editViewDialog_SubmitData();
                },
                Cancel: function () {
                    editViewDialog.dialog("close");
                }
            },
            create: function () {
                $(this).find("#view-name").keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        editViewDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                editViewDialog.find("#view-name").val(CurrentView.data("dbViewName"));
                editViewDialog.find("#view-query").val(CurrentView.data("dbViewQuery"));
            }
        });
        function editViewDialog_SubmitData() {
            CurrentView.find(".dbViewName").text("View: " + editViewDialog.find("#view-name").val());
            CurrentView.data("dbViewName", editViewDialog.find("#view-name").val());
            CurrentView.data("dbViewQuery", editViewDialog.find("#view-query").val());
            editViewDialog.dialog("close");
        }

        addIndexDialog = $("#add-index-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 260,
            buttons: {
                "Add": function () {
                    addIndexDialog_SubmitData();
                },
                Cancel: function () {
                    addIndexDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        addIndexDialog_SubmitData();
                        return false;
                    }
                });
                $("#add-index-dialog #btn-add-index-column").on("click", function () {
                    newColumnNumber = addIndexDialog.data("columnsShown") + 1;
                    newFormRow = $('<tr class="additionalFormRow"><td><label for="additional-column">' + newColumnNumber + '. column</label></td>'
                        + '<td><select name="additional-column" class="additionalColumn"></select></td></tr>');
                    newFormRow.find(".additionalColumn").append($('<option value="-none-">-none-</option>'));
                    CurrentTable.find(".dbColumn").each(function (i, val) {
                        newFormRow.find(".additionalColumn").append(
                            $('<option value="' + $(val).find(".dbColumnName").text() + '">' + $(val).find(".dbColumnName").text() + '</option>'));
                    });
                    $("#add-index-dialog").find("#addIndexColumnFormRow").before(newFormRow);
                    addIndexDialog.data("columnsShown", newColumnNumber);
                });
            },
            open: function () {
                addIndexDialog.find("#first-column option").remove();
                CurrentTable.find(".dbColumn").each(function (i, val) {
                    addIndexDialog.find("#first-column").append(
                        $('<option value="' + $(val).find(".dbColumnName").text() + '">' + $(val).find(".dbColumnName").text() + '</option>'));
                });
                addIndexDialog.find("#second-column option").remove();
                addIndexDialog.find("#second-column").append(
                        $('<option value="-none-">-none-</option>'));
                CurrentTable.find(".dbColumn").each(function (i, val) {
                    addIndexDialog.find("#second-column").append(
                        $('<option value="' + $(val).find(".dbColumnName").text() + '">' + $(val).find(".dbColumnName").text() + '</option>'));
                });
                addIndexDialog.find("#index-name").val("");
                addIndexDialog.find("#first-column").val("id");
                addIndexDialog.find("#second-column").val("-none-");
                addIndexDialog.find("#unique-checkbox").prop("checked", false);
                addIndexDialog.find(".additionalFormRow").remove();
                addIndexDialog.data("columnsShown", 1);
            }
        });
        function addIndexDialog_SubmitData() {
            indexColumnArray = [
                addIndexDialog.find("#first-column").val()
            ];
            addIndexDialog.find(".additionalFormRow .additionalColumn").each(function (i, element) {
                indexColumnArray.push($(element).val());
            });
            filteredIndexColumnArray = [];
            for (i = 0; i < indexColumnArray.length; i++) {
                if (indexColumnArray[i] != "-none-")
                    filteredIndexColumnArray.push(indexColumnArray[i]);
            }
            AddIndex(CurrentTable,
                addIndexDialog.find("#index-name").val(),
                filteredIndexColumnArray,
                addIndexDialog.find("#unique-checkbox").prop("checked")
                );
            addIndexDialog.dialog("close");
        }

        editIndexDialog = $("#edit-index-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 230,
            buttons: {
                "Save": function () {
                    editIndexDialog_SubmitData();
                },
                Cancel: function () {
                    editIndexDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        editIndexDialog_SubmitData();
                        return false;
                    }
                });
                $("#edit-index-dialog #btn-add-index-column").on("click", function () {
                    newColumnNumber = editIndexDialog.data("columnsShown") + 1;
                    newFormRow = $('<tr class="additionalFormRow"><td><label for="additional-column">' + newColumnNumber + '. column</label></td>'
                        + '<td><select name="additional-column" class="additionalColumn"></select></td></tr>');
                    newFormRow.find(".additionalColumn").append($('<option value="-none-">-none-</option>'));
                    CurrentTable.find(".dbColumn").each(function (i, val) {
                        newFormRow.find(".additionalColumn").append(
                            $('<option value="' + $(val).find(".dbColumnName").text() + '">' + $(val).find(".dbColumnName").text() + '</option>'));
                    });
                    $("#edit-index-dialog").find("#addIndexColumnFormRow").before(newFormRow);
                    editIndexDialog.data("columnsShown", newColumnNumber);
                });
            },
            open: function () {
                editIndexDialog.find("#first-column option").remove();
                CurrentTable.find(".dbColumn").each(function (i, val) {
                    editIndexDialog.find("#first-column").append(
                        $('<option value="' + $(val).find(".dbColumnName").text() + '">' + $(val).find(".dbColumnName").text() + '</option>'));
                });
                editIndexDialog.find("#second-column option").remove();
                editIndexDialog.find("#second-column").append(
                        $('<option value="-none-">-none-</option>'));
                CurrentTable.find(".dbColumn").each(function (i, val) {
                    editIndexDialog.find("#second-column").append(
                        $('<option value="' + $(val).find(".dbColumnName").text() + '">' + $(val).find(".dbColumnName").text() + '</option>'));
                });
                indexColumnArray = CurrentIndex.data("indexColumnArray");
                if (!indexColumnArray)
                    indexColumnArray = ["id"];
                editIndexDialog.data("columnsShown", indexColumnArray.length);
                editIndexDialog.find("#index-name").val(CurrentIndex.data("indexName"));
                editIndexDialog.find("#first-column").val(indexColumnArray[0]);
                editIndexDialog.find("#second-column").val(indexColumnArray[1]);
                editIndexDialog.find("#unique-checkbox").prop("checked", CurrentIndex.data("unique"));
                editIndexDialog.find(".additionalFormRow").remove();
                for (i = 1; i < indexColumnArray.length; i++) {
                    newFormRow = $('<tr class="additionalFormRow"><td><label for="additional-column">' + (i + 1) + '. column</label></td>'
                        + '<td><select name="additional-column" class="additionalColumn"></select></td></tr>');
                    newFormRow.find(".additionalColumn").append($('<option value="-none-">-none-</option>'));
                    CurrentTable.find(".dbColumn").each(function (i, val) {
                        newFormRow.find(".additionalColumn").append(
                            $('<option value="' + $(val).find(".dbColumnName").text() + '">' + $(val).find(".dbColumnName").text() + '</option>'));
                    });
                    newFormRow.find(".additionalColumn").val(indexColumnArray[i]);
                    $("#edit-index-dialog").find("#addIndexColumnFormRow").before(newFormRow);
                };
            }
        });
        function editIndexDialog_SubmitData() {
            indexColumnArray = [
                editIndexDialog.find("#first-column").val()
            ];
            editIndexDialog.find(".additionalFormRow .additionalColumn").each(function (i, element) {
                indexColumnArray.push($(element).val());
            });
            filteredIndexColumnArray = [];
            for (i = 0; i < indexColumnArray.length; i++) {
                if (indexColumnArray[i] != "-none-")
                    filteredIndexColumnArray.push(indexColumnArray[i]);
            }
            indexLabel = "Index: ";
            for (i = 0; i < filteredIndexColumnArray.length - 1; i++)
                indexLabel += filteredIndexColumnArray[i] + ", ";
            indexLabel += filteredIndexColumnArray[filteredIndexColumnArray.length - 1];
            if (editIndexDialog.find("#unique-checkbox").prop("checked"))
                indexLabel += " - unique";

            newIndex = $('<div class="dbIndex"><div class="deleteIndexIcon fa fa-remove"></div><span class="dbIndexText">'
                + indexLabel + '</span><div class="editIndexIcon fa fa-pencil"></div></div>');
            newIndex.children(".deleteIndexIcon").on("click", function () {
                $(this).parents(".dbIndex").remove();
            });
            newIndex.children(".editIndexIcon").on("click", function () {
                CurrentIndex = $(this).parents(".dbIndex");
                CurrentTable = $(this).parents(".dbTable");
                editIndexDialog.dialog("open");
            });
            newIndex.data("indexName", editIndexDialog.find("#index-name").val());
            newIndex.data("indexColumnArray", filteredIndexColumnArray);
            newIndex.data("unique", editIndexDialog.find("#unique-checkbox").prop("checked"));
            CurrentIndex.replaceWith(newIndex);
            editIndexDialog.dialog("close");
        }
    }
});
