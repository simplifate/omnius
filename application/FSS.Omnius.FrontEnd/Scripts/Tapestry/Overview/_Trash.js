TO.trash = {

    init: function () {

    },

    _trashOpen: function () {
        var d = $(this);

        d.find("#metablock-table:first tbody:nth-child(2) tr").remove();
        d.find("#block-table:first tbody:nth-child(2) tr").remove();
        d.find(" .spinner-2").show();
        d.data("selectedMetablock", null);
        d.data("selectedBlock", null);
         
        var appId = $("#currentAppId").val();
        var blockId = $("#currentBlockId").val();

        $.ajax({
            type: "GET",
            url: "/api/database/apps/" + appId + "/trashDialog",
            dataType: "json",
            success: function (data) {
                var tBlockBody = d.find("#block-table tbody:nth-child(2)");
                var tMetablockBody = d.find("#metablock-table tbody:nth-child(2)");

                var blockIdArray = [];
                var metablockIdArray = [];

                // Fill blocks in the block-table rows
                for (i = 0; i < data[0].length; i++) {
                    blockIdArray.push(data[0][i].Id);
                    var newRow = $('<tr class="blockRow"><td>' + data[0][i].Name + '</td></tr>');
                    tBlockBody.append(newRow);
                }

                // Highlight the selected block row
                $(document).on('click', '#block-table tr.blockRow', function (event) {
                    d.find("#block-table tbody:nth-child(2) tr").removeClass("highlightedBlockRow");
                    d.find("#metablock-table tbody:nth-child(2) tr").removeClass("highlightedBlockRow");
                    $(this).addClass("highlightedBlockRow");
                    var rowIndex = $(this).index();
                    d.data("selectedBlockOrMetablock", data[0][rowIndex]);
                    d.data("selectedTypeOfBlock", "block");
                });

                // Fill metablocks in the metablock-table rows
                for (i = 0; i < data[1].length; i++) {
                    metablockIdArray.push(data[1][i].Id);
                    var newRow = $('<tr class="blockRow"><td>' + data[1][i].Name + '</td></tr>');
                    tMetablockBody.append(newRow);
                }

                // Highlight the selected metablock row
                $(document).on('click', '#metablock-table tr.blockRow', function (event) {
                    d.find("#block-table tbody:nth-child(2) tr").removeClass("highlightedBlockRow");
                    d.find("#metablock-table tbody:nth-child(2) tr").removeClass("highlightedBlockRow");
                    $(this).addClass("highlightedBlockRow");
                    var rowIndex = $(this).index();
                    d.data("selectedBlockOrMetablock", data[1][rowIndex]);
                    d.data("selectedTypeOfBlock", "metablock");
                });

                d.find(".spinner-2").hide();
            }
        });
    },

    _trashLoad: function () {
        var d = $(this);
        var confirmed = true;

        if (d.data("selectedBlockOrMetablock")) {
            d.dialog("close");

            if (TO.changedSinceLastSave)
                confirmed = confirm("You have unsaved changes. Do you really want to discard unsaved changes?");
                
            if (confirmed) {
                // Draw block or metablock as user chose
                if (d.data("selectedTypeOfBlock") == "block") {
                    var currentBlockData = d.data("selectedBlockOrMetablock");
                    var newBlock = $('<div class="block" id="block' + currentBlockData.Id + '" isInitial="' + currentBlockData.IsInitial + '" style="left: '
                        + currentBlockData.PositionX + 'px; top: ' + currentBlockData.PositionY + 'px;" blockId="'
                        + currentBlockData.Id + '" tableId="' + currentBlockData.AssociatedTableId + '"><div class="blockName">'
                        + currentBlockData.Name + '</div><div class="blockInfo">'
                        + (currentBlockData.IsInitial ? 'Initial' : '') + '</div></div>');
                    newBlock.data("IsInMenu", currentBlockData.IsInMenu);
                    $("#overviewPanel .scrollArea").append(newBlock);
                    instance.draggable(newBlock, {
                        containment: "parent",
                        stop: function () {
                            TO.changedSinceLastSave = true;
                        }
                    });

                    SaveMetablock();
                } else {
                    var currentMetablockData = d.data("selectedBlockOrMetablock");
                    var newMetablock = $('<div class="metablock" id="metablock' + currentMetablockData.Id + '" isInitial="' + currentMetablockData.IsInitial + '"style="left: '
                    + currentMetablockData.PositionX + 'px; top: ' + currentMetablockData.PositionY + 'px;" metablockId="' +
                    currentMetablockData.Id + '"><div class="metablockName">' + currentMetablockData.Name +
                    '</div><div class="metablockSymbol fa fa-th-large"></div><div class="metablockInfo">'
                    + (currentMetablockData.IsInitial ? 'Initial' : '') + '</div></div>');
                    newMetablock.data("IsInMenu", currentMetablockData.IsInMenu);
                    $("#overviewPanel .scrollArea").append(newMetablock);
                    instance.draggable(newMetablock, {
                        containment: "parent",
                        stop: function () {
                            TO.changedSinceLastSave = true;
                        }
                    });

                    SaveMetablock();
                }
            }
        }
        else {
            alert("Please select a block");
        }
    }
};

TO.onInit.push(TO.trash.init);