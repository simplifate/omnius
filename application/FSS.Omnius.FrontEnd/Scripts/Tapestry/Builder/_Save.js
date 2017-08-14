TB.save = {

    onBeforeSave: [],
    onAfterSave: [],
    saveId: 0,

    toolboxMap: {
        Actions: 'actionItem',
        Attributes: 'attributeItem',
        UiComponents: 'uiItem',
        Roles: 'roleItem',
        States: 'stateItem',
        Targets: 'targetItem',
        Templates: 'templateItem',
        Integrations: 'integrationItem'
    },

    init: function () {
        var self = TB.save;
        $(document).on('click', '#btnSave', $.proxy(self._beforeSave, self));
    },

    saveBlock: function () {
        pageSpinner.show();
       
        $('.activeItem, .processedItem').removeClass('activeItem processedItem');

        var commitMessage = $(this).find('#message').val(),
            self = TB.save,
            rrList = [],
            wrList = [],
            toolboxState = {};

        // Sestavíme pole resource rules
        $('#resourceRulesPanel .resourceRule').each(function (index) {
            rrList.push(self.getResourceRule(this, index));
        });

        // Sestavíme pole workflow rules
        $('#workflowRulesPanel .workflowRule').each(function (index) {
            wrList.push(self.getWorkflowRule(this, index));
        });

        // Sestavíme toolbox state
        for (var k in self.toolboxMap) {
            toolboxState[k] = [];

            $('.tapestryToolbox .toolboxItem.' + self.toolboxMap[k]).each(function () {
                var item = $(this);

                toolboxState[k].push({
                    Label: item.find('.itemLabel').text(),
                    ActionId: item.attr('ActionId'),
                    TableName: item.attr('TableName') ? item.attr('TableName') : null,
                    IsShared: item.attr('shared') === 'true',
                    ColumnName: item.attr('ColumnName') ? item.attr('ColumnName') : null,
                    PageId: item.attr('PageId'),
                    ComponentName: item.attr('ComponentName') ? item.attr('ComponentName') : null,
                    IsBootstrap: GetIsBootstrap(item),
                    StateId: item.attr('StateId'),
                    TargetName: item.attr('TargetName') ? item.attr('TargetName') : null,
                    TargetId: item.attr('TargetId'),
                    TypeClass: self.toolboxMap[k]
                });
            });
        }

        // Sestavíme finální data
        var postData = {
            CommitMessage: commitMessage,
            Name: $('#blockHeaderBlockName').text(),
            ResourceRules: rrList,
            WorkflowRules: wrList,
            PortTargets: [],
            ModelTableName: ModelTableName,
            AssociatedTableName: AssociatedTableName,
            AssociatedPageIds: AssociatedPageIds,
            AssociatedBootstrapPageIds: AssociatedBootstrapPageIds,
            AssociatedTableIds: AssociatedTableIds,
            RoleWhitelist: RoleWhitelist,
            ToolboxState: toolboxState,
            ParentMetablockId: $('#parentMetablockId').val()
        };

        var appId = $('#currentAppId').val();
        var blockId = $('#currentBlockId').val();

        $.ajax({
            type: 'POST',
            url: '/api/tapestry/apps/' + appId + '/blocks/' + blockId,
            data: postData,
            complete: pageSpinner.hide,
            success: self._afterSave
        });

        TB.dialog.close.apply(this);
    },

    /*****************************************************************/
    /* GET RESOURCE RULES                                            */
    /*****************************************************************/
    getResourceRule: function(node, index) {
        var items = [],
            connections = [],
            rule = $(node),
            self = this;

        rule.find('.item').each(function () {
            items.push(self.getResourceItem(this));
        });

        $.each(rule.data('jsPlumbInstance').getAllConnections(), function () {
            connections.push(self.getConnection(this));
        });

        return {
            Id: index+1,
            Width: Math.round(rule.width()),
            Height: Math.round(rule.height()),
            PositionX: Math.round(rule.position().left),
            PositionY: Math.round(rule.position().top),
            ResourceItems: items,
            Connections: connections
        };
    },

    getResourceItem: function(node) {
        var item = $(node);
        item.attr('saveId', ++this.saveId);

        return {
            Id: item.attr('saveId'),
            Label: item.text(),
            TypeClass: GetItemTypeClass(item),
            PositionX: Math.round(item.position().left),
            PositionY: Math.round(item.position().top),
            ActionId: item.attr('actionid'),
            StateId: item.attr('stateid'),
            PageId: GetIsBootstrap(item) ? null : item.attr('pageId'),
            ComponentName: item.attr('componentName'),
            IsBootstrap: GetIsBootstrap(item),
            BootstrapPageId: GetIsBootstrap(item) ? item.attr('pageId') : null,
            TableName: item.attr('tableName'),
            IsShared: item.attr('shared') === 'true',
            ColumnName: item.attr('columnName'),
            ColumnFilter: item.data('columnFilter'),
            ConditionSets: item.data('conditionSets')
        };
    },

    getConnection: function(connection) {
        var source = $(connection.source),
            target = $(connection.target);

        return {
            SourceId: source.attr('saveId'),
            SourceSlot: 0,
            TargetId: target.attr('saveId'),
            TargetSlot: 0
        };
    },

    /*****************************************************************/
    /* GET WORKFLOW RULES                                            */
    /*****************************************************************/
    getWorkflowRule: function(node, index) {
        var swimlanes = [],
            connections = [],
            self = this,
            rule = $(node);

        rule.find('.swimlane').each(function (index) {
            swimlanes.push(self.getSwimlane(this, index));
        });

        $.each(rule.data('jsPlumbInstance').getAllConnections(), function () {
            var connection = self.getWorkflowConnection(this);
            if (connection !== false) {
                connections.push(connection);
            }
        });

        return {
            Id: index,
            Name: rule.find('.workflowRuleHeader .verticalLabel').text(),
            Width: Math.round(rule.width()),
            Height: Math.round(rule.height()),
            PositionX: Math.round(rule.position().left),
            PositionY: Math.round(rule.position().top),
            Swimlanes: swimlanes,
            Connections: connections
        };
    },

    getSwimlane: function(node, index) {
        var roles = [],
            items = [],
            subflows = [],
            foreachs = [],
            self = this,
            sl = $(node);

        sl.attr('swimlaneIndex', index);

        sl.find('.swimlaneRolesArea .roleItem').each(function () {
            roles.push($(this).text());
        });

        sl.find('.swimlaneContentArea > .subflow').each(function () {
            subflows.push(self.getSubflow(this));
        });

        sl.find('.swimlaneContentArea > .foreach').each(function () {
            foreachs.push(self.getForeach(this));
        });

        sl.find('.swimlaneContentArea > .item, .swimlaneContentArea > .symbol').each(function () {
            items.push(self.getWorkflowItem(this));
        });

        return {
            SwimlaneIndex: index,
            Height: Math.round(sl.height()),
            Roles: roles,
            WorkflowItems: items,
            Subflow: subflows,
            Foreach: foreachs
        };
    },

    getSubflow: function(node) {
        var items = [],
            self = this,
            subflow = $(node);

        subflow.attr('saveId', ++this.saveId);
        subflow.find('> .item, > .symbol').each(function () {
            items.push(self.getWorkflowItem(this));
        });

        return {
            Id: subflow.attr('saveId'),
            Name: subflow.find('> .subflowName').text(),
            Comment: subflow.find('> .subflowComment').text(),
            CommentBottom: subflow.find('> .subflowComment').hasClass('bottom'),
            PositionX: Math.round(subflow.position().left),
            PositionY: Math.round(subflow.position().top),
            Width: Math.round(subflow.width()),
            Height: Math.round(subflow.height()),
            WorkflowItems: items
        };
    },

    getForeach: function (node) {
        var items = [],
            self = this,
            foreach = $(node);

        foreach.attr('saveId', ++this.saveId);
        foreach.find('> .item, > .symbol').each(function () {
            items.push(self.getWorkflowItem(this));
        });
        
        return {
            Id: foreach.attr('saveId'),
            Name: foreach.find('> .foreachName').text(),
            Comment: foreach.find('> .foreachComment').text(),
            CommentBottom: foreach.find('> .foreachComment').hasClass('bottom'),
            PositionX: Math.round(foreach.position().left),
            PositionY: Math.round(foreach.position().top),
            Width: Math.round(foreach.width()),
            Height: Math.round(foreach.height()),
            WorkflowItems: items,
            DataSource: foreach.attr('data-datasource')
        };
    },

    getWorkflowItem: function(node) {
        var item = $(node);
        item.attr('saveId', ++this.saveId);

        return {
            Id: item.attr('saveId'),
            Label: item.find('.itemLabel').length ? item.find('.itemLabel').text() : item.data('label'),
            Name: item.find('.itemName').text(),
            Comment: item.find('.itemComment').text(),
            CommentBottom: item.find('.itemComment').hasClass('bottom'),
            TypeClass: GetItemTypeClass(item),
            DialogType: item.attr('dialogType'),
            StateId: item.attr('stateid'),
            TargetId: item.attr('targetid'),
            PositionX: Math.round(item.position().left),
            PositionY: Math.round(item.position().top),
            ActionId: item.attr('actionid'),
            InputVariables: item.data('inputVariables'),
            OutputVariables: item.data('outputVariables'),
            PageId: item.attr('pageId'),
            ComponentName: item.attr('componentName'),
            IsBootstrap: GetIsBootstrap(item),
            isAjaxAction: item.data('isAjaxAction'),
            Condition: item.data('condition'),
            ConditionSets: item.data('conditionSets'),
            SymbolType: item.attr('symbolType'),
            IsForeachStart: item.find('.fa-play').length > 0,
            IsForeachEnd: item.find('.fa-stop').length > 0
        };
    },

    getWorkflowConnection: function(connection) {
        var source = $(connection.source),
            target = $(connection.target),
            sourceEndpointUuid = connection.endpoints[0].getUuid();

        var sourceSlot = sourceEndpointUuid.match("BottomCenter$") ? 1 : 0;

        if (!source.hasClass('subSymbol')) {
            return {
                SourceId: source.attr("saveId"),
                SourceSlot: sourceSlot,
                TargetId: target.attr("saveId"),
                TargetSlot: 0
            };
        }
        return false;
    },

    /*****************************************************************/
    /* EVENTS                                                        */
    /*****************************************************************/
    _dialogOpen: function () {
        $(this).find('#message').val('');
    },

    _beforeSave: function () {
        var allow = true;
        for (var i = 0; i < this.onBeforeSave.length; i++) {
            allow = allow && this.onBeforeSave[i]();
        }

        if (allow) {
            TB.dialog.open('save');
        }
    },

    _afterSave: function (lastCommitId) {
        alert("The block has been successfully saved");

        ChangedSinceLastSave = false; /// OBSOLATE
        TB.changedSinceLastSave = false;
        TB.callHooks(TB.save.onAfterSave, this, [lastCommitId]);
    }
};

TB.onInit.push(TB.save.init);