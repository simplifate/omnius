$(function () {
    if (CurrentModuleIs("overviewModule")) {
        addBlockDialog = $("#add-block-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 180,
            buttons: {
                "Add": function () {
                    addBlockDialog_SubmitData();
                },
                Cancel: function () {
                    addBlockDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        addBlockDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                $(this).find("#block-name").val("");
                $(this).find("#table-dropdown option").remove();
                for (i = 1; i < 6; i++) {
                    fakeTableName = "Table" + i; // TODO: replace with real data - load from a list of FSPOC tables
                    $(this).find("#table-dropdown").append($('<option value="' + fakeTableName + '">' + fakeTableName + '</option>'));
                }
            }
        });
        function addBlockDialog_SubmitData() {
            blockName = addBlockDialog.find("#block-name").val();
            tableName = addBlockDialog.find("#table-dropdown").val();
            newBlock = $('<div class="block"><div class="blockName">' + blockName + '</div><div class="tableName">'
                + tableName + '</div></div>');
            newBlock.on("dblclick", function () {
                window.location.href = "/tapestry";
            });
            $("#overviewPanel").append(newBlock);
            instance.draggable(newBlock, { containment: "parent" });
            addBlockDialog.dialog("close");
        }
        addMetaBlockDialog = $("#add-metablock-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 180,
            buttons: {
                "Add": function () {
                    addMetaBlockDialog_SubmitData();
                },
                Cancel: function () {
                    addMetaBlockDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        addMetaBlockDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                $(this).find("#metablock-name").val("");
            }
        });
        function addMetaBlockDialog_SubmitData() {
            metaBlockName = addMetaBlockDialog.find("#metablock-name").val();
            newMetaBlock = $('<div class="metablock"><div class="metablockName">'
                + metaBlockName + '</div><div class="metablockSymbol fa fa-th-large"></div></div>');
            $("#overviewPanel").append(newMetaBlock);
            instance.draggable(newMetaBlock, { containment: "parent" });
            addMetaBlockDialog.dialog("close");
        }
        renameMetaBlockDialog = $("#rename-metablock-dialog").dialog({
            autoOpen: false,
            width: 400,
            height: 190,
            buttons: {
                "Save": function () {
                    renameMetaBlockDialog_SubmitData();
                },
                Cancel: function () {
                    renameMetaBlockDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        renameMetaBlockDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                renameMetaBlockDialog.find("#metablock-name").val($("#headerMetablockName").text());
            }
        });
        function renameMetaBlockDialog_SubmitData() {
            renameMetaBlockDialog.dialog("close");
            $("#headerMetablockName").text(renameMetaBlockDialog.find("#metablock-name").val());
        }
    }
});
