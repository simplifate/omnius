function LoadMetablock() {
    appId = $("#currentAppId").val();
    metablockId = $("#currentMetablockId").val();
    url = "/api/tapestry/apps/" + appId + "/metablocks/" + metablockId;
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        error: function () { alert("ERROR") },
        success: function (data) {
            $("#headerMetablockName").text(data.Name);
            $("#overviewPanel .block, #overviewPanel .metablock").each(function (index, element) {
                instance.removeAllEndpoints(element, true);
                $(element).remove();
            });
            for (i = 0; i < data.Blocks.length; i++) {
                currentBlockData = data.Blocks[i];
                newBlock = $('<div class="block" id="block' + currentBlockData.Id + '" style="left: '
                    + currentBlockData.PositionX + 'px; top: ' + currentBlockData.PositionY + 'px;" blockId="'
                    + currentBlockData.Id + '" tableId="' + currentBlockData.AssociatedTableId + '"><div class="blockName">'
                    + currentBlockData.Name + '</div><div class="tableName">'
                    + currentBlockData.AssociatedTableName + '</div></div>');
                $("#overviewPanel").append(newBlock);
                instance.draggable(newBlock, { containment: "parent" });
                newBlock.on("dblclick", function () {
                    blockToOpen = $(this);
                    SaveMetablock(function () {
                        openBlockForm = $("#openBlockForm");
                        openBlockForm.find("input[name='blockId']").val(blockToOpen.attr("blockId"));
                        openBlockForm.submit();
                    });                    
                });
            }
            for (i = 0; i < data.Metablocks.length; i++) {
                currentMetablockData = data.Metablocks[i];
                newMetablock = $('<div class="metablock" id="metablock' + currentMetablockData.Id + '" style="left: '
                    + currentMetablockData.PositionX + 'px; top: ' + currentMetablockData.PositionY + 'px;" metablockId="' +
                    currentMetablockData.Id + '"><div class="metablockName">' + currentMetablockData.Name +
                    '</div><div class="metablockSymbol fa fa-th-large"></div></div>');
                $("#overviewPanel").append(newMetablock);
                instance.draggable(newMetablock, { containment: "parent" });
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
    });
}
