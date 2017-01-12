TB.screen = {

    init: function()
    {
        var self = TB.screen;

        $(document)
            .on('click', '#blockHeaderScreenCount', self.openDialog)
            .on('click', '#choose-screens-dialog tr.screenRow', self.toggleRow)
        ;
        
    },

    openDialog: function()
    {
        chooseScreensDialog.dialog('open');
        chooseScreensDialog.find("#screen-table tbody tr").remove();
        
        TB.screen.toggleSpinner(true);

        var appId = $("#currentAppId").val();
        $.ajax({
            type: "GET",
            url: "/api/mozaic-bootstrap/apps/" + appId + "/pages",
            dataType: "json",
            success: TB.screen.setData
        });
    },

    closeDialog: function()
    {
        chooseScreensDialog.dialog('close');
    },

    toggleRow: function() 
    {
        $(this).toggleClass("highlightedRow");
    },

    toggleSpinner: function(show) {
        chooseScreensDialog.find(".spinner-2").toggle(show);
    },

    setData: function(data)
    {
        var tbody = chooseScreensDialog.find('#screen-table tbody');
        for (i = 0; i < data.length; i++)
        {
            var row = $('<tr class="screenRow"></tr>');
            row.attr({
                'data-pageid': data[i].Id,
                'data-isbootstrap': data[i].IsBootstrap
            });

            row.append('<td>' + data[i].Name + '</td>');
            row.append('<td>' + (data[i].IsBootstrap ? 'yes' : 'no') + '</td>');

            if (!data[i].IsBootstrap && AssociatedPageIds.indexOf(data[i].Id) != -1) {
                row.addClass("highlightedRow");
            }
            if (data[i].IsBootstrap && AssociatedBootstrapPageIds.indexOf(data[i].Id) != -1) {
                row.addClass("highlightedRow");
            }
            tbody.append(row);
        }
        
        TB.screen.toggleSpinner(false);
    },

    selectScreen: function()
    {
        var self = TB.screen;
        self.toggleSpinner(true);

        var pageCount = $('#screen-table tbody tr.highlightedRow').length;
        var appId = $("#currentAppId").val();

        AssociatedPageIds = [];
        AssociatedBootstrapPageIds = [];

        $('#libraryCategory-UI .libraryItem').remove();
        
        $('#screen-table tbody tr.highlightedRow').each(function () {

            var pageId = $(this).attr('data-pageid');
            var isBootstrap = $(this).attr('data-isbootstrap') == 'true';
            var api = isBootstrap ? '/api/mozaic-bootstrap/apps/' : '/api/mozaic-editor/apps/';
            var callback = isBootstrap ? self.loadBootstrapComponents : self.loadLegacyComponents;

            if (isBootstrap) {
                AssociatedBootstrapPageIds.push(parseInt(pageId));
            }
            else {
                AssociatedPageIds.push(parseInt(pageId));
            }

            $.ajax({
                type: 'GET',
                url: api + appId + "/pages/" + pageId,
                dataType: 'json',
                async: false,
                success: callback
            });
        });

        $('#blockHeaderScreenCount').text(pageCount);
        self.toggleSpinner(false);
        chooseScreensDialog.dialog("close");
    },

    loadBootstrapComponents: function(data)
    {
        var list = $("#libraryCategory-UI");

        for (i = 0; i < data.Components.length; i++) {
            if (i == 0) {
                var item = $('<div class="libraryItem" />');
                item.attr({
                    'libId': ++lastLibId,
                    'pageId': data.Id,
                    'isBootstrap': true,
                    'libType': 'ui'
                }).html('Screen: ' + data.Name).appendTo(list);
            }

            TB.screen.addComponent(data.Components[i], data.Id, list);
        }
    },

    addComponent: function(c, pageId, list)
    {
        if (c.ElmId != "") {
            var item = $('<div class="libraryItem" />');
            item.attr({
                'libId': ++lastLibId,
                'pageId': pageId,
                'componentName': c.ElmId,
                'isBootstrap': true,
                'libType': 'ui'
            }).html(c.ElmId).appendTo(list);
        }

        if (c.UIC == 'ui|data-table')
        {
            var actionsText = TB.screen.getAttribute(c.Attributes, 'data-actions');
            if (actionsText !== -1) {
                var actions = JSON.parse(actionsText.replace(/'/g, '"'));
                for (var a in actions) {
                    var item = $('<div class="libraryItem" />');
                    item.attr({
                        'libId': ++lastLibId,
                        'pageId': pageId,
                        'componentName': c.ElmId + '_' + actions[a].action,
                        'isBootstrap': true,
                        'libType': 'ui'
                    }).html(c.ElmId + '_' + actions[a].action).appendTo(list);
                }
            }
        }

        if (c.ChildComponents) {
            for (var i = 0; i < c.ChildComponents.length; i++) {
                TB.screen.addComponent(c.ChildComponents[i], pageId, list);
            }
        }
    },

    loadLegacyComponents: function(data)
    {
        var list = $("#libraryCategory-UI");

        for (i = 0; i < data.Components.length; i++) {
            if (i == 0) {
                var item = $('<div class="libraryItem" />');
                item.attr({
                    'libId': ++lastLibId,
                    'pageId': data.Id,
                    'isBootstrap': false,
                    'libType': 'ui'
                }).html('Screen: ' + data.Name).appendTo(list);
            }
            
            TB.screen.addLegacyComponent(data.Components[i], data.Id, list);
        }
    },

    addLegacyComponent: function(c, pageId, list)
    {
        var item = $('<div class="libraryItem" />');
        item.attr({
            'libId': ++lastLibId,
            'pageId': pageId,
            'componentName': c.Name,
            'isBootstrap': false,
            'libType': 'ui'
        }).html(c.Name).appendTo(list);

        var actions = ['_EditAction', '_DetailsAction', '_DeleteAction', '_A_Action', '_B_Action'];
        
        if (c.Type == "data-table-with-actions") {
            for(var a = 0; a < actions.length; a++) {
                var item = $('<div class="libraryItem" />');
                item.attr({
                    'libId': ++lastLibId,
                    'pageId': pageId,
                    'componentName': c.Name + actions[a],
                    'isBootstrap': false,
                    'libType': 'ui'
                }).html(c.Name + actions[a]).appendTo(list);
            }
        }

        if (c.ChildComponents) {
            for (var i = 0; i < c.ChildComponents.length; i++) {
                TB.screen.addLegacyComponent(c.ChildComponents[i], pageId, list);
            }
        }
    },

    getAttribute: function(attrString, attrName)
    {
        var attrList = JSON.parse(attrString);
        for (var k in attrList) {
            if (attrList[k].name == attrName) {
                return attrList[k].value;
            }
        }
        return -1;
    }
};

TB.onInit.push(TB.screen.init);