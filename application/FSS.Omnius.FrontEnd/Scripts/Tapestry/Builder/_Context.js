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

        $.contextMenu($.extend(ds, {
            selector: '.swimlaneRolesArea .roleItem',
            callback: function (key, options) {
                var item = options.$trigger;
                if (key == "delete") {
                    var swimlaneRolesArea = item.parents(".swimlaneRolesArea");
                    item.remove();
                    if (swimlaneRolesArea.find(".roleItem").length == 0)
                        swimlaneRolesArea.append($('<div class="rolePlaceholder"><div class="rolePlaceholderLabel">'
                            + 'Pokud chcete specifikovat roli<br />přetáhněte ji do této oblasti</div></div>'));
                    ChangedSinceLastSave = true; /// OBSOLATE
                    TB.changedSinceLastSave = true;
                }
            },
            items: {
                "delete": { name: "Remove role", icon: "delete" }
            }
        }));

        $.contextMenu($.extend(ds, {
            selector: '.item, .symbol',
            callback: function (key, options) {
                item = options.$trigger;
                if (key == "delete") {
                    currentInstance = item.parents(".rule").data("jsPlumbInstance");
                    currentInstance.removeAllEndpoints(item, true);
                    item.remove();
                    ChangedSinceLastSave = true;
                }
                else if (key == "wizard") {
                    item.addClass("activeItem processedItem");

                    if (item.hasClass("actionItem") && item.parents(".rule").hasClass("workflowRule")) {
                        CurrentItem = item;
                        TB.wizard.open.apply(item, []);
                    }
                    else {
                        alert("Pro tento typ objektu nejsou dostupná žádná nastavení.");
                        item.removeClass("activeItem");
                    }
                }
                else if (key == "properties") {
                    item.addClass("activeItem processedItem");
                    if (item.hasClass("tableAttribute")) {
                        CurrentItem = item;
                        tableAttributePropertiesDialog.dialog("open");
                    }
                    else if (item.hasClass("viewAttribute")) {
                        CurrentItem = item;
                        gatewayConditionsDialog.dialog("open");
                    }
                    else if (item.hasClass("actionItem") && item.parents(".rule").hasClass("workflowRule")) {
                        CurrentItem = item;
                        actionPropertiesDialog.dialog("open");
                    }
                    else if (item.hasClass("symbol") && item.attr("symboltype") == "gateway-x")
                    {
                        CurrentItem = item;
                        gatewayConditionsDialog.dialog("open");
                    }
                    else if (item.hasClass("symbol") && item.attr("symboltype") == "envelope-start")
                    { 
                        CurrentItem = item;
                        envelopeStartPropertiesDialog.dialog("open");
                    }
                    else if (item.hasClass("uiItem")) {
                        CurrentItem = item;
                        uiitemPropertiesDialog.dialog("open");
                    }
                    else if (item.hasClass("symbol") && item.attr("symbolType") === "comment") {
                        CurrentItem = item;
                        labelPropertyDialog.dialog("open");
                    }
                    else {
                        alert("Pro tento typ objektu nejsou dostupná žádná nastavení.");
                        item.removeClass("activeItem");
                    }
                }
            },
            items: {
                "wizard": { name: "Wizard", icon: "fa-magic" },
                "properties": { name: "Properties", icon: "edit" },
                "delete": { name: "Delete", icon: "delete" }
            }
        }));
    }

};

TB.onInit.push(TB.context.init);