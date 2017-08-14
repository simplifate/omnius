TB.context = {

    defaultSettings: {
        trigger: 'right',
        zIndex: 300
    },

    init: function() 
    {
        var ds = TB.context.defaultSettings; 
        
        $.contextMenu($.extend(ds, {
            selector: '.resourceRule',
            callback: TB.rr._contextAction,
            items: TB.rr.contextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.swimlane',
            callback: TB.wfr._swimlaneContextAction,
            items: TB.wfr.swimlaneContextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.workflowRule',
            callback: TB.wfr._contextAction,
            items: TB.wfr.contextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.swimlaneRolesArea .roleItem',
            callback: TB.wfr._roleContextAction,
            items: TB.wfr.roleContextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.item:not(.actionItem), .symbol',
            callback: TB.wfr._itemContextAction,
            items: TB.wfr.itemContextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.item.actionItem',
            callback: TB.wfr._itemContextAction,
            items: TB.wfr.actionItemContextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.subflow',
            callback: TB.subflow._contextAction,
            items: TB.subflow.contextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.foreach',
            callback: TB.foreach._contextAction,
            items: TB.foreach.contextItems
        }));

        $.contextMenu($.extend(ds, {
            selector: '.tableRow',
            callback: function (key, options) {
                if (key == "model") {
                    var tableRow = options.$trigger;
                    tableRow.addClass("highlightedRow");
                    tableRow.parents("table").find(".modelMarker").remove();
                    tableRow.find("td:first").append('<div class="modelMarker">Model</div>');
                }
            },
            items: {
                "model": { name: "Set as model", icon: "edit" }
            }
        }));
    }

};

TB.onInit.push(TB.context.init);