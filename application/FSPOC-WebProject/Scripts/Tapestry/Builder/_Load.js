TB.load = {

    libraryLoadList: {
        attributes: {
            url: '/api/database/apps/{appId}/commits/latest',
            success: 'librarySetAttributes',
        },
        actions: {
            url: '/api/tapestry/actions',
            success: 'librarySetActions'
        },
        roles: {
            url: '/api/Persona/app-roles/{appId}',
            success: 'librarySetRoles'
        },
        states: {
            url: '/api/Persona/app-states/{appId}',
            success: 'librarySetStates'
        },
        targets: {
            url: '/api/Tapestry/apps/{appId}/blocks',
            success: 'librarySetTargets'
        },
        templates: {
            url: '/api/Hermes/{appId}/templates',
            success: 'librarySetTemplates'
        },
        integrations: {
            url: '/api/Nexus/{appId}/gateways',
            success: 'librarySetIntegrations'
        }
    },
    toolboxState: null,
    data: null,

    init: function () {

    },

    loadBlock: function (commitId) {
        pageSpinner.show();

        var appId = $('#currentAppId').val();
        var blockId = $('#currentBlockId').val();

        var url = '/api/tapestry/apps/' + appId + '/blocks/' + blockId + (commitId ? '/commits/' + commitId : '');

        $.ajax({
            type: 'GET',
            url: url,
            dataType: 'json',
            complete: pageSpinner.hide,
            success: TB.load.setData
        });
    },

    setData: function (data)
    {
        var self = TB.load;

        ChangedSinceLastSave = false; /// OBSOLATE
        TB.changedSinceLastSave = false;

        self.toolboxState = data.ToolboxState;
        self.data = data;

        self.clearBuilder();
        self.setBlockName(data.Name);
        self.setResourceRules(data.ResourceRules);
        self.setWorkflowRules(data.WorkflowRules);
        self.setRoleWhiteList(data.RoleWhitelist);
        self.loadPages();
        self.loadLibrary();
    },

    clearBuilder: function() {
        $('#resourceRulesPanel .resourceRule').remove();
        $('#workflowRulesPanel .workflowRule').remove();
    },

    setBlockName: function (name) {
        $('#blockHeaderBlockName').text(name);
    },

    setResourceRules: function(rrList)
    {
        for (var i = 0; i < rrList.length; i++)
        {
            var rule = TB.rr.create(rrList[i]);

            for (var j = 0; j < rrList[i].ResourceItems.length; j++) {
                TB.rr.createItem(rrList[i].ResourceItems[j], rule);
            }

            var currentInstance = rule.data("jsPlumbInstance");
            for (var c = 0; c < rrList[i].Connections.length; c++) {
                var currentConnectionData = rrList[i].Connections[c];
                currentInstance.connect({
                    uuids: ["resItem" + currentConnectionData.SourceId + "RightMiddle"], target: "resItem" + currentConnectionData.TargetId
                });
            }
        }
    },

    setWorkflowRules: function(wfrList) {
        for (var i = 0; i < wfrList.length; i++) {
            var rule = TB.wfr.create(wfrList[i]);

            for (var j = 0; j < wfrList[i].Swimlanes.length; j++) {
                TB.wfr.createSwimlane(wfrList[i].Swimlanes[j], wfrList[i].Swimlanes.length, rule);
            }

            var currentInstance = rule.data('jsPlumbInstance');
            for (var c = 0; c < wfrList[i].Connections.length; c++) {
                var currentConnectionData = wfrList[i].Connections[c];
                var sourceId = "wfItem" + currentConnectionData.SourceId;
                var targetId = "wfItem" + currentConnectionData.TargetId;
                var sourceEndpointUuid;
                if (currentConnectionData.SourceSlot == 1)
                    sourceEndpointUuid = "BottomCenter";
                else
                    sourceEndpointUuid = "RightMiddle";
                currentInstance.connect({ uuids: [sourceId + sourceEndpointUuid], target: targetId });
            }
        }
    },

    setRoleWhiteList: function (whitelist) {
        $('#blockHeaderRolesCount').text(whitelist.length);
    },

    loadLibrary: function()
    {
        pageSpinner.show();
        var appId = $('#currentAppId').val();

        // CLEAN
        TB.library.clean();
        TB.toolbox.clean();

        // LOAD
        for (var k in TB.load.libraryLoadList) {
            var libraryType = TB.load.libraryLoadList[k];
            
            $.ajax({
                type: 'GET',
                url: libraryType.url.replace(/\{appId\}/, appId),
                dataType: 'json',
                success: TB.load[libraryType.success]
            });
        }
        pageSpinner.hide();
    },

    loadPages: function()
    {
        var self = TB.load;

        AssociatedPageIds = self.data.AssociatedPageIds;
        AssociatedBootstrapPageIds = self.data.AssociatedBootstrapPageIds;

        $("#blockHeaderScreenCount").text(AssociatedPageIds.length + AssociatedBootstrapPageIds.length);
        
        for (var i = 0; i < AssociatedPageIds.length; i++) {
            var pageId = AssociatedPageIds[i];

            self.libraryLoadList['page' + pageId] = {
                url: '/api/mozaic-editor/apps/{appId}/pages/' + pageId,
                success: 'librarySetPage'
            };
        }

        for (var i = 0; i < AssociatedBootstrapPageIds.length; i++) {
            var pageId = AssociatedBootstrapPageIds[i];

            self.libraryLoadList['bootstrapPage' + pageId] = {
                url: '/api/mozaic-bootstrap/apps/{appId}/pages/' + pageId,
                success: 'librarySetBootstrapPage'
            };
        }
    },

    librarySetAttributes: function(data)
    {
        var self = TB.load;
        var state = self.toolboxState ? self.toolboxState.Attributes : [];

        AssociatedTableIds = self.data.AssociatedTableIds;
        AssociatedTableName = self.data.AssociatedTableName;
        ModelTableName = self.data.ModelTableName;
        somethingWasAdded = false;
        $("#blockHeaderDbResCount").text(self.data.AssociatedTableName.length);

        for (var ti = 0; ti < data.Tables.length; ti++) 
        {
            var isUsed = state.filter(function (value) { return !value.ColumnName && value.TableName == data.Tables[ti].Name; }).length;
            var params = { tableName: data.Tables[ti].Name };
            var label = 'Table: ' + data.Tables[ti].Name;

            var libId = TB.library.createItem('Attributes', 'table-attribute', params, label, 'tableAttribute', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Attributes', 'attributeItem tableAttribute', params, label);
            }
        }
        for (var vi = 0; vi < data.Views.length; vi++)
        {
            var isUsed = state.filter(function (value) { return !value.ColumnName && value.TableName == data.Views[vi].Name; }).length;
            var params = { tableName: data.Views[vi].Name };
            var label = 'View: ' + data.Views[vi].Name;

            var libId = TB.library.createItem('Attributes', 'view-attribute', params, label, 'viewAttribute', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Attributes', 'attributeItem viewAttribute', params, label);
            }
        }

        for (var ti = 0; ti < self.data.AssociatedTableName.length; ti++)
        {
            var currentTable = data.Tables.filter(function (value) { return value.Name == self.data.AssociatedTableName[ti]; })[0];
            if (currentTable)
            {
                for (var ci = 0; ci < currentTable.Columns.length; ci++)
                {
                    var isUsed = state.filter(function (value) {
                        return value.ColumnName == currentTable.Columns[ci].Name && value.TableName == currentTable.Name;
                    }).length;
                    var params = {tableName: currentTable.Name, columnName: currentTable.Columns[ci].Name };
                    var label = currentTable.Name + '.' + currentTable.Columns[ci].Name;

                    var libId = TB.library.createItem('Attributes', 'column-attribute', params, label, 'columnAttribute', isUsed);
                    if(isUsed) {
                        TB.toolbox.createItem(libId, 'Attributes', 'attributeItem tableAttribute', params, label);
                    }
                }
            }

            var systemTable = SystemTables.filter(function (value) { return value.Name == self.data.AssociatedTableName[ti]; })[0];
            if (systemTable)
            {
                for (var i = 0; i < systemTable.Columns.length; i++)
                {
                    var isUsed = state.filter(function (value) {
                        return value.ColumnName == systemTable.Columns[i].Name && value.TableName == systemTable.Name;
                    }).length;
                    var params = { tableName: systemTable.Name, columnName: systemTable.Columns[i].Name };
                    var label = systemTable.Name + '.' + systemTable.Columns[i].Name;

                    var libId = TB.library.createItem('Attributes', 'column-attribute', params, label, 'columnAttribute', isUsed);
                    if (isUsed) {
                        TB.toolbox.createItem(libId, 'Attributes', 'attributeItem tableAttribute', params, label);
                    }
                }
            }
        }
    },

    librarySetActions: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.Actions : [];

        for (var i = 0; i < data.Items.length; i++)
        {
            var isUsed = state.filter(function (value) { return value.ActionId == data.Items[i].Id; }).length;
            var params = {actionId: data.Items[i].Id };
            var label = data.Items[i].Name;

            var libId = TB.library.createItem('Actions', 'action', params, label, '', isUsed);
            if(isUsed) {
                TB.toolbox.createItem(libId, 'Actions', 'actionItem', params, label);
            }
        }
    },

    librarySetRoles: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.Roles : [];

        for (var i = 0; i < data.Roles.length; i++)
        {
            var isUsed = state.filter(function (value) { return value.Label == data.Roles[i].Name; }).length;
            var params = {};
            var label = data.Roles[i].Name;

            var libId = TB.library.createItem('Roles', 'role', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Roles', 'roleItem', params, label);
            }
        }
    },

    librarySetStates: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.States : [];

        for (var i = 0; i < data.States.length; i++)
        {
            var isUsed = state.filter(function (value) { return value.StateId == data.States[i].Id; }).length;
            var params = {stateId: data.States[i].Id};
            var label = data.States[i].Name;

            var libId = TB.library.createItem('States', 'state', params, label, '', isUsed);
            if(isUsed) {
                TB.toolbox.createItem(libId, 'States', 'stateItem', params, label);
            }
        }
    },

    librarySetTargets: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.Targets : [];

        for (var i = 0; i < data.ListItems.length; i++) {
            var isUsed = state.filter(function (value) { return value.TargetId == data.ListItems[i].Id; }).length;
            var params = { targetId: data.ListItems[i].Id };
            var label = data.ListItems[i].Name;

            var libId = TB.library.createItem('Targets', 'target', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Targets', 'targetItem', params, label);
            }
        }
    },

    librarySetTemplates: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.Templates : [];

        for (var i = 0; i < data.length; i++) {
            var isUsed = state.filter(function (value) { return value.Label == data[i].Name; }).length;
            var params = {};
            var label = data[i].Name;

            var libId = TB.library.createItem('Templates', 'template', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Templates', 'templateItem', params, label);
            }
        }
    },

    librarySetIntegrations: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.Integrations : [];

        // LDAP
        for (var i = 0; i < data.Ldap.length; i++) {
            var params = {};
            var label = 'LDAP: ' + data.Ldap[i].Name;
            var isUsed = state.filter(function (value) { return value.Label == label; }).length;

            var libId = TB.library.createItem('Integration', 'ldap', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Integrations', 'integrationItem', params, label);
            }
        }

        // WS
        for (var i = 0; i < data.WS.length; i++) {
            var params = { libSubType: data.WS[i].Type };
            var label = 'WS: ' + data.WS[i].Name;
            var isUsed = state.filter(function (value) { return value.Label == label; }).length;
            
            var libId = TB.library.createItem('Integration', 'ws', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Integrations', 'integrationItem', params, label);
            }
        }

        // SMTP
        for (var i = 0; i < data.SMTP.length; i++) {
            var params = {};
            var label = 'SMTP: ' + data.SMTP[i].Name;
            var isUsed = state.filter(function (value) { return value.Label == label; }).length;
            
            var libId = TB.library.createItem('Integration', 'smtp', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Integrations', 'integrationItem', params, label);
            }
        }

        // WebDAV
        for (var i = 0; i < data.WebDAV.length; i++) {
            var params = {};
            var label = 'WebDAV: ' + data.WebDAV[i].Name;
            var isUsed = state.filter(function (value) { return value.Label == label; }).length;
            
            var libId = TB.library.createItem('Integration', 'WebDAV', params, label, '', isUsed);
            if (isUsed) {
                TB.toolbox.createItem(libId, 'Integrations', 'integrationItem', params, label);
            }
        }
    },

    librarySetPage: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.UiComponents : [];
        TB.screen.loadLegacyComponents(data, state);
    },

    librarySetBootstrapPage: function(data)
    {
        var state = TB.load.toolboxState ? TB.load.toolboxState.UiComponents : [];
        TB.screen.loadBootstrapComponents(data, state);
    }
};

TB.onInit.push(TB.load.init);