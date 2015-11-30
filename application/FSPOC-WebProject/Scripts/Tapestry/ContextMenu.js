$(function () {
    if ($("body.tapestryModule").length) {
        $.contextMenu({
            selector: '.rule .item, .rule .operatorSymbol',
            trigger: 'right',
            zIndex: 300,
            callback: function (key, options) {
                item = options.$trigger;
                if (key == "delete") {
                    currentInstance = item.parents(".rule").data("jsPlumbInstance");
                    currentInstance.removeAllEndpoints(item, true);
                    item.remove();
                }
                else if (key == "edit") {
                    switch (item.attr("dialogType")) {
                        case "emailTemplate":
                            CurrentItem = item;
                            chooseEmailTemplateDialog.dialog("open");
                            break;
                        case "port":
                            CurrentItem = item;
                            choosePortDialog.dialog("open");
                            break;
                        case "condition":
                            CurrentItem = item;
                            editConditionDialog.dialog("open");
                            break;
                        default:
                            alert("This item doesn't have any properties to edit.");
                            break;
                    }
                }
            },
            items: {
                "edit": { name: "Edit", icon: "edit" },
                "sep1": "---------",
                "delete": { name: "Delete", icon: "delete" }
            }
        });
        /*$.contextMenu({
            selector: '.rule',
            trigger: 'right',
            zIndex: 300,
            callback: function (key, options) {
                if (key == "delete") {
                    options.$trigger.remove();
                }
            },
            items: {
                "delete": { name: "Delete rule", icon: "delete" }
            }
        });*/
    };
});