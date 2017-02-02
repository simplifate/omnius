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

    loadBootstrapComponents: function(data, state)
    {
        for (i = 0; i < data.Components.length; i++) {
            if (i == 0) {
                var label = 'Screen: ' + data.Name;
                var params = { pageId: data.Id, isBootstrap: true };
                var isUsed = state.filter && state.filter(function (value) { return value.PageId == data.Id && (!value.ComponentName || value.ComponentName == "undefined") }).length;

                var libId = TB.library.createItem('UI', 'ui', params, label, '', isUsed);
                if (isUsed) {
                    TB.toolbox.createItem(libId, 'UI', 'uiItem', params, label);
                }
            }

            TB.screen.addComponent(data.Components[i], data.Id, state);
        }
    },

    addComponent: function(c, pageId, state)
    {
        if (c.ElmId != "") {
            var label = c.ElmId;
            var params = { pageId: pageId, componentName: label, isBootstrap: true };
            var isUsed = state.filter && state.filter(function (value) { return value.PageId == pageId && value.ComponentName == label; }).length;

            var libId = TB.library.createItem('UI', 'ui', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'UI', 'uiItem', params, label);
            }
        }

        if (c.UIC == 'ui|data-table')
        {
            var actionsText = TB.screen.getAttribute(c.Attributes, 'data-actions');
            if (actionsText !== -1) {
                var actions = JSON.parse(actionsText.replace(/'/g, '"'));
                for (var a in actions) {
                    var label = c.ElmId + '_' + actions[a].action;
                    var params = { pageId: pageId, componentName: label, isBootstrap: true };
                    var isUsed = state.filter && state.filter(function (value) { return value.PageId == pageId && value.ComponentName == label; }).length;

                    var libId = TB.library.createItem('UI', 'ui', params, label, '', isUsed);
                    if (isUsed) {
                        TB.toolbox.createItem(libId, 'UI', 'uiItem', params, label);
                    }
                }
            }
        }

        if (c.ChildComponents) {
            for (var i = 0; i < c.ChildComponents.length; i++) {
                TB.screen.addComponent(c.ChildComponents[i], pageId, state);
            }
        }
    },

    loadLegacyComponents: function(data, state)
    {
        for (i = 0; i < data.Components.length; i++) {
            if (i == 0) {
                var label = 'Screen: ' + data.Name;
                var params = { pageId: data.Id, isBootstrap: false };
                var isUsed = state.filter && state.filter(function (value) { return value.PageId == data.Id && (!value.ComponentName || value.ComponentName == "undefined") }).length;

                var libId = TB.library.createItem('UI', 'ui', params, label, '', isUsed);
                if (isUsed) {
                    TB.toolbox.createItem(libId, 'UI', 'uiItem', params, label);
                }
            }
            
            TB.screen.addLegacyComponent(data.Components[i], data.Id, state);
        }
    },

    addLegacyComponent: function(c, pageId, state)
    {
        var label = c.Name;
        var params = { pageId: pageId, componentName: label, isBootstrap: false };
        var isUsed = state.filter && state.filter(function (value) { return value.PageId == pageId && value.ComponentName == label; }).length;

        var libId = TB.library.createItem('UI', 'ui', params, label, '', isUsed);
        if (isUsed) {
            TB.toolbox.createItem(libId, 'UI', 'uiItem', params, label);
        }

        var actions = ['_EditAction', '_DetailsAction', '_DeleteAction', '_A_Action', '_B_Action'];
        
        if (c.Type == "data-table-with-actions") {
            for (var a = 0; a < actions.length; a++) {
                var label = c.Name + actions[a];
                var params = { pageId: pageId, componentName: label, isBootstrap: false };
                var isUsed = state.filter && state.filter(function (value) { return value.PageId == pageId && value.ComponentName == label; }).length;

                var libId = TB.library.createItem('UI', 'ui', params, label, '', isUsed);
                if (isUsed) {
                    TB.toolbox.createItem(libId, 'UI', 'uiItem', params, label);
                }
            }
        }

        if (c.ChildComponents) {
            for (var i = 0; i < c.ChildComponents.length; i++) {
                TB.screen.addLegacyComponent(c.ChildComponents[i], pageId, state);
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