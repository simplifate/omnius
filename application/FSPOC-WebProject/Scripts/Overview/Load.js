function LoadMetablock() {
    $("#overviewPanelSpinnerOverlay").show();
    appId = $("#currentAppId").val();
    metablockId = $("#currentMetablockId").val();
    url = "/api/tapestry/apps/" + appId + "/metablocks/" + metablockId;
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        error: function (request, status, error) {
            alert(request.responseText);
        },
        success: function (data) {
            $("#headerMetablockName").text(data.Name);
            $("#overviewPanel .block, #overviewPanel .metablock").each(function (index, element) {
                instance.removeAllEndpoints(element, true);
                $(element).remove();
            });
            for (i = 0; i < data.Blocks.length; i++) {
                currentBlockData = data.Blocks[i];
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
            }
            for (i = 0; i < data.Metablocks.length; i++) {
                currentMetablockData = data.Metablocks[i];
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
            for (i = 0; i < data.Connections.length; i++) {
                console.log("Connection");
                currentConnectonData = data.Connections[i];
                // TODO: implement remote connection representation
                if (currentConnectonData.SourceType == 1)
                    sourceId = "metablock" + currentConnectonData.SourceId;
                else
                    sourceId = "block" + currentConnectonData.SourceId;
                if (currentConnectonData.TargetType == 1)
                    targetId = "metablock" + currentConnectonData.TargetId;
                else
                    targetId = "block" + currentConnectonData.TargetId;
                instance.connect({
                    source: sourceId, target: targetId, editable: false, paintStyle: connectorPaintStyle
                });
            }
            $("#overviewPanelSpinnerOverlay").hide();
        }
    });
}
