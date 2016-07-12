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
            newBlock = $('<div class="block"><div class="blockName">' + blockName + '</div><div class="blockInfo"></div></div>');
            $("#overviewPanel .scrollArea").append(newBlock);
            instance.draggable(newBlock, { containment: "parent" });
            newBlock.css("top", $("#overviewPanel").scrollTop() + 20);
            newBlock.css("left", $("#overviewPanel").scrollLeft() + 20);
            newBlock.on("dblclick", function () {
                blockToOpen = $(this);
                SaveMetablock(function () {
                    openBlockForm = $("#openBlockForm");
                    openBlockForm.find("input[name='blockId']").val(blockToOpen.attr("blockId"));
                    openBlockForm.submit();
                });
            });
            ChangedSinceLastSave = true;
            addBlockDialog.dialog("close");
        }
        addMetablockDialog = $("#add-metablock-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 400,
            height: 180,
            buttons: {
                "Add": function () {
                    addMetablockDialog_SubmitData();
                },
                Cancel: function () {
                    addMetablockDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        addMetablockDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                $(this).find("#metablock-name").val("");
            }
        });
        function addMetablockDialog_SubmitData() {
            metablockName = addMetablockDialog.find("#metablock-name").val();
            newMetablock = $('<div class="metablock"><div class="metablockName">'
                + metablockName + '</div><div class="metablockSymbol fa fa-th-large"></div><div class="metablockInfo"></div></div>');
            $("#overviewPanel .scrollArea").append(newMetablock);
            instance.draggable(newMetablock, { containment: "parent" });
            newMetablock.css("top", $("#overviewPanel").scrollTop() + 20);
            newMetablock.css("left", $("#overviewPanel").scrollLeft() + 20);
            newMetablock.on("dblclick", function () {
                metablockToOpen = $(this);
                SaveMetablock(function () {
                    openMetablockForm = $("#openMetablockForm");
                    openMetablockForm.find("input[name='metablockId']").val(metablockToOpen.attr("metablockId"));
                    openMetablockForm.submit();
                });
            });
            ChangedSinceLastSave = true;
            addMetablockDialog.dialog("close");
        }
        trashDialog = $("#trash-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 700,
            height: 540,
            buttons: {
                "Load": function () {
                    trashDialog_SubmitData();
                },
                Cancel: function () {
                    trashDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        trashDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function (event, ui) {
                trashDialog.find("#metablock-table:first tbody:nth-child(2) tr").remove();
                trashDialog.find("#block-table:first tbody:nth-child(2) tr").remove();
                trashDialog.find(" .spinner-2").show();
                trashDialog.data("selectedMetablock", null);
                trashDialog.data("selectedBlock", null);
                appId = $("#currentAppId").val();
                blockId = $("#currentBlockId").val();
                $.ajax({
                    type: "GET",
                    url: "/api/database/apps/" + appId + "/trashDialog",
                    dataType: "json",
                    success: function (data) {
                        tBlockBody = trashDialog.find("#block-table tbody:nth-child(2)");
                        tMetablockBody = trashDialog.find("#metablock-table tbody:nth-child(2)");
                        blockIdArray = [];
                        metablockIdArray = [];

                        // Fill blocks in the block-table rows
                        for (i = 0; i < data[0].length; i++) {
                            blockIdArray.push(data[0][i].Id);
                            newRow = $('<tr class="blockRow"><td>' + data[0][i].Name + '</td></tr>');
                            tBlockBody.append(newRow);
                        }

                        // Highlight the selected block row
                        $(document).on('click', '#block-table tr.blockRow', function (event) {
                            trashDialog.find("#block-table tbody:nth-child(2) tr").removeClass("highlightedBlockRow");
                            trashDialog.find("#metablock-table tbody:nth-child(2) tr").removeClass("highlightedBlockRow");
                            $(this).addClass("highlightedBlockRow");
                            var rowIndex = $(this).index();
                            trashDialog.data("selectedBlockOrMetablock", data[0][rowIndex]);
                            trashDialog.data("selectedTypeOfBlock", "block");
                        });

                        // Fill metablocks in the metablock-table rows
                        for (i = 0; i < data[1].length; i++) {
                            metablockIdArray.push(data[1][i].Id);
                            newRow = $('<tr class="blockRow"><td>' + data[1][i].Name + '</td></tr>');
                            tMetablockBody.append(newRow);
                        }

                        // Highlight the selected metablock row
                        $(document).on('click', '#metablock-table tr.blockRow', function (event) {
                            trashDialog.find("#block-table tbody:nth-child(2) tr").removeClass("highlightedBlockRow");
                            trashDialog.find("#metablock-table tbody:nth-child(2) tr").removeClass("highlightedBlockRow");
                            $(this).addClass("highlightedBlockRow");
                            var rowIndex = $(this).index();
                            trashDialog.data("selectedBlockOrMetablock", data[1][rowIndex]);
                            trashDialog.data("selectedTypeOfBlock", "metablock");
                        });
                     
                        trashDialog.find(".spinner-2").hide();
                    }
                });
            }
        });
        function trashDialog_SubmitData() {
            if (trashDialog.data("selectedBlockOrMetablock")) {
                trashDialog.dialog("close");
                if (ChangedSinceLastSave)
                    confirmed = confirm("You have unsaved changes. Do you really want to discard unsaved changes?");
                else
                    confirmed = true;
                if (confirmed) {
                    // Draw block or metablock as user chose
                    if (trashDialog.data("selectedTypeOfBlock") == "block") {
                        currentBlockData = trashDialog.data("selectedBlockOrMetablock");
                        newBlock = $('<div class="block" id="block' + currentBlockData.Id + '" isInitial="' + currentBlockData.IsInitial + '" style="left: '
                            + currentBlockData.PositionX + 'px; top: ' + currentBlockData.PositionY + 'px;" blockId="'
                            + currentBlockData.Id + '" tableId="' + currentBlockData.AssociatedTableId + '"><div class="blockName">'
                            + currentBlockData.Name + '</div><div class="blockInfo">'
                            + (currentBlockData.IsInitial ? 'Initial' : '') + '</div></div>');
                        newBlock.data("IsInMenu", currentBlockData.IsInMenu);
                        $("#overviewPanel .scrollArea").append(newBlock);
                        instance.draggable(newBlock, {
                            containment: "parent",
                            stop: function () {
                                ChangedSinceLastSave = true;
                            }
                        });
                        newBlock.on("dblclick", function () {
                            blockToOpen = $(this);
                            SaveMetablock(function () {
                                openBlockForm = $("#openBlockForm");
                                openBlockForm.find("input[name='blockId']").val(blockToOpen.attr("blockId"));
                                openBlockForm.submit();
                            });
                        });
                    } else {
                        currentMetablockData = trashDialog.data("selectedBlockOrMetablock");
                        newMetablock = $('<div class="metablock" id="metablock' + currentMetablockData.Id + '" isInitial="' + currentMetablockData.IsInitial + '"style="left: '
                        + currentMetablockData.PositionX + 'px; top: ' + currentMetablockData.PositionY + 'px;" metablockId="' +
                        currentMetablockData.Id + '"><div class="metablockName">' + currentMetablockData.Name +
                        '</div><div class="metablockSymbol fa fa-th-large"></div><div class="metablockInfo">'
                        + (currentMetablockData.IsInitial ? 'Initial' : '') + '</div></div>');
                        newMetablock.data("IsInMenu", currentMetablockData.IsInMenu);
                        $("#overviewPanel .scrollArea").append(newMetablock);
                        instance.draggable(newMetablock, {
                            containment: "parent",
                            stop: function () {
                                ChangedSinceLastSave = true;
                            }
                        });

                        newMetablock.on("dblclick", function () {
                            metablockToOpen = $(this);
                            SaveMetablock(function () {
                                openMetablockForm = $("#openMetablockForm");
                                openMetablockForm.find("input[name='metablockId']").val(metablockToOpen.attr("metablockId"));
                                openMetablockForm.submit();
                            });
                        });
                    }             
                }
            }
            else
                alert("Please select a block");
        }
        renameMetablockDialog = $("#rename-metablock-dialog").dialog({
            autoOpen: false,
            width: 400,
            height: 190,
            buttons: {
                "Save": function () {
                    renameMetablockDialog_SubmitData();
                },
                Cancel: function () {
                    renameMetablockDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        renameMetablockDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                renameMetablockDialog.find("#metablock-name").val($("#headerMetablockName").text());
            }
        });
        function renameMetablockDialog_SubmitData() {
            renameMetablockDialog.dialog("close");
            $("#headerMetablockName").text(renameMetablockDialog.find("#metablock-name").val());
            ChangedSinceLastSave = true;
        }

        blockPropertiesDialog = $('#block-properties-dialog').dialog({
            autoOpen: false,
            width: 500,
            height: 250,
            buttons: {
                "Save": function () {
                    blockPropertiesDialog_SubmitData();
                },
                "Cancel": function () {
                    blockPropertiesDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        blockPropertiesDialog_SubmitData();
                        return false;
                    }
                });
            },
            open: function () {
                blockPropertiesDialog.find("#p-block-name").val(currentBlock.find('.blockName').text());
                blockPropertiesDialog.find("#block-is-in-menu").prop('checked', currentBlock.data('IsInMenu'));
                blockPropertiesDialog.find("#block-set-as-initial").prop('checked', currentBlock.attr('isinitial') == 'true');
            }
        });
        function blockPropertiesDialog_SubmitData() {
            blockPropertiesDialog.dialog("close");
            currentBlock.data("IsInMenu", blockPropertiesDialog.find("#block-is-in-menu").is(':checked'));
            
            var isInitial = blockPropertiesDialog.find("#block-set-as-initial").is(':checked') ? true : false;
            if (isInitial) {
                $("#overviewPanel .block").each(function (index, element) {
                    $(element).attr("isInitial", false);
                    $(element).find(".blockInfo").text("");
                });
            }
            currentBlock.attr("isInitial", isInitial);
            currentBlock.find(".blockInfo").text(isInitial ? "Initial" : "");

            if (blockPropertiesDialog.find("#p-block-name").val().length) {
                currentBlock.find('.blockName').html(blockPropertiesDialog.find("#p-block-name").val());
            }
            ChangedSinceLastSave = true;
        }

        metablockPropertiesDialog = $('#metablock-properties-dialog').dialog({
            autoOpen: false,
            width: 500,
            height: 250,
            buttons: {
                "Save": function () {
                    metablockPropertiesDialog_SubmitData();
                },
                "Cancel": function () {
                    metablockPropertiesDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        metablockPropertiesDialog_SubmitData();
                        return false;
                    }
                });
            },
            open: function () {
                metablockPropertiesDialog.find("#p-metablock-name").val(currentMetablock.find('.metablockName').text());
                metablockPropertiesDialog.find("#metablock-is-in-menu").prop('checked', currentMetablock.data('IsInMenu'));
                metablockPropertiesDialog.find("#metablock-set-as-initial").prop('checked', currentMetablock.attr('isinitial') == 'true');
            }
        });
        function metablockPropertiesDialog_SubmitData() {
            metablockPropertiesDialog.dialog("close");

            var isInitial = metablockPropertiesDialog.find("#metablock-set-as-initial").is(':checked') ? true : false;
            if(isInitial)
            {
                $("#overviewPanel .metablock").each(function (index, element) {
                    $(element).attr("isInitial", false);
                    $(element).find(".metablockInfo").text("");
                });
            }
            currentMetablock.attr("isInitial", isInitial);
            currentMetablock.find(".metablockInfo").text(isInitial ? "Initial" : "");

            currentMetablock.data("IsInMenu", metablockPropertiesDialog.find("#metablock-is-in-menu").is(':checked'));
            if (metablockPropertiesDialog.find("#p-metablock-name").val().length) {
                currentMetablock.find('.metablockName').html(metablockPropertiesDialog.find("#p-metablock-name").val());
            }
            ChangedSinceLastSave = true;
        }
    }
});
