TB.wfr = {

    onCreateItem: [],
    onCreateRule: [],

    templates: {
        rule: '<div class="rule workflowRule"><div class="workflowRuleHeader"><div class="verticalLabel" style="margin-top: 0px;"></div></div><div class="swimlaneArea"></div></div>',
        swimlane: '<div class="swimlane"><div class="swimlaneRolesArea"><div class="roleItemContainer"></div><div class="rolePlaceholder"><div class="rolePlaceholderLabel">Pokud chcete specifikovat roli<br />'
            + 'přetáhněte ji do této oblasti</div></div></div><div class="swimlaneContentArea"></div></div>',
        item: ''
    },
    
    contextItems: {
        'add-swimlane': { name: 'Add swimlane', icon: 'fa-plus' },
        'rename': { name: 'Rename rule...', icon: 'fa-edit' },
        'copy': { name: 'Copy rule...', icon: 'fa-clone' },
        'delete': { name: 'Delete rule', icon: 'fa-trash' }
    },

    swimlaneContextItems: {
        'group-to-subflow': { name: 'Group to subflow', icon: 'fa-object-group', disabled: TB.subflow.cannotBeGruped },
        'group-to-foreach': { name: 'Group to foreach', icon: 'fa-repeat', disabled: TB.foreach.cannotBeGruped },
        'remove-swimlane': { name: 'Remove swimlane', icon: 'fa-trash' },
    },

    roleContextItems: {
        'delete': { name: 'Remove role', icon: 'fa-trash' }
    },

    itemContextItems: {
        'properties': { name: 'Properties...', icon: 'fa-edit' },
        'group-to-subflow': { name: 'Group to subflow', icon: 'fa-object-group', disabled: TB.subflow.cannotBeGruped },
        'sep1': '---------',
        'group-to-foreach': { name: 'Group to foreach', icon: 'fa-repeat', disabled: TB.foreach.cannotBeGruped },
        'set-as-fe-start': { name: 'Set as foreach start', icon: 'fa-play', disabled: TB.foreach.isNotInForeach },
        'set-as-fe-end': { name: 'Set as foreach end', icon: 'fa-stop', disabled: TB.foreach.isNotInForeach },
        'sep2': '---------',
        'delete': { name: 'Delete', icon: 'fa-trash' }
    },

    actionItemContextItems: {
        'wizard': { name: 'Wizard...', icon: 'fa-magic' },
        'properties': { name: 'Properties...', icon: 'fa-edit' },
        'name': { name: 'Name...', icon: 'fa-tag' },
        'comment': { name: 'Comment...', icon: 'fa-comment' },
        'group-to-subflow': { name: 'Group to subflow', icon: 'fa-object-group', disabled: TB.subflow.cannotBeGruped },
        'sep1': '---------',
        'group-to-foreach': { name: 'Group to foreach', icon: 'fa-repeat', disabled: TB.foreach.cannotBeGruped },
        'set-as-fe-start': { name: 'Set as foreach start', icon: 'fa-play', disabled: TB.foreach.isNotInForeach },
        'set-as-fe-end': { name: 'Set as foreach end', icon: 'fa-stop', disabled: TB.foreach.isNotInForeach },
        'sep2': '---------',
        'delete': { name: 'Delete', icon: 'fa-trash' }
    },


    currentRule: null,

    init: function () {
        var self = TB.wfr;
        $(document)
            .on('keydown', $.proxy(self._keyDown, self))
            .on('click', 'button#Search', $.proxy(self.searchItems, self))
            .on('click', '#SearchBox .close', $.proxy(self.closeSearch, self));
    },

    create: function(ruleData)
    {
        var self = TB.wfr;

        var rule = $(self.templates.rule);
        rule.css({
            width: ruleData.Width,
            height: ruleData.Height,
            left: ruleData.PositionX,
            top: ruleData.PositionY
        })
        .find('.verticalLabel').html(ruleData.Name).end()
        .attr({ 'id': AssingID(), 'data-id': ruleData.Id })
        .appendTo("#workflowRulesPanel .scrollArea");
        
        self.aliveRule(rule);
        TB.callHooks(self.onCreateRule, rule, []);

        return rule;
    },

    remove: function() {
        this.remove();
        ChangedSinceLastSave = true; /// OBSOLATE
        TB.changedSinceLastSave = true;
    },

    rename: function() {
        CurrentRule = this;
        renameRuleDialog.dialog('open');
    },

    addSwimlane: function() {
        var count = this.find('.swimlane').length + 1;
        TB.wfr.createSwimlane({Roles: [], WorkflowItems: []}, count, this);

        this.find('.swimlane').css('height', (100 / count) + '%');
        this.data("jsPlumbInstance").recalculateOffsets();
        this.data("jsPlumbInstance").repaintEverything();

        ChangedSinceLastSave = true /// OBSOLATE
        TB.changedSinceLastSave = true;
    },

    createSwimlane: function(swimlaneData, count, parentRule) {
        var self = TB.wfr;
        
        var swimlane = $(self.templates.swimlane);
        swimlane
            .css('height', (100 / count) + '%')
            .appendTo($('.swimlaneArea', parentRule));

        if (swimlaneData.Roles.length > 0) {
            swimlane.find('.swimlaneRolesArea .rolePlaceholder').remove();
            for (var r = 0; r < swimlaneData.Roles.length; r++) {
                swimlane.find('.swimlaneRolesArea .roleItemContainer').append('<div class="roleItem">' + swimlaneData.Roles[r] + '</div>');
            }
        }

        // Připravíme mapování foreach id => virtual action id
        for (var k = 0; k < swimlaneData.WorkflowItems.length; k++) {
            var item = swimlaneData.WorkflowItems[k];
            if (item.TypeClass == 'virtualAction' && item.SymbolType == 'foreach' && item.ParentForeachId) {
                TB.foreach.id2VirtualId[item.ParentForeachId] = item.Id;
            }
        }

        for (var k = 0; k < swimlaneData.Subflow.length; k++) {
            TB.subflow.createSubflow(swimlaneData.Subflow[k], swimlane);
        }

        for (var k = 0; k < swimlaneData.Foreach.length; k++) {
            TB.foreach.createForeach(swimlaneData.Foreach[k], swimlane, null, true);
        }

        for (var k = 0; k < swimlaneData.WorkflowItems.length; k++) {
            self.createItem(swimlaneData.WorkflowItems[k], swimlane);
        }

        self.aliveSwimlane(swimlane);
    },

    removeSwimlane: function () {
        // this = options.$trigger

        var rule = this.parents('.workflowRule');
        var swimlaneCount = rule.find('.swimlane').length;
        if (swimlaneCount > 1) {
            rule.data('jsPlumbInstance').removeAllEndpoints(this, true);
            this.remove();
            
            rule.find('.swimlane').css('height', (100 / (swimlaneCount - 1)) + '%');
            rule.data('jsPlumbInstance').recalculateOffsets();
            rule.data('jsPlumbInstance').repaintEverything();

            ChangedSinceLastSave = true; /// OBSOLATE
            TB.changedSinceLastSave = true;
        }
        else {
            alert('Pravidlo musí mít alspoň jednu swimlane, nelze smazat všechny.');
        }
    },

    createItem: function(itemData, parentSwimlane)
    {
        if (itemData.TypeClass == 'virtualAction') // Pouze virtuální akce
            return;

        var item;
        if (itemData.TypeClass === "symbol" && itemData.SymbolType === "comment") {
            item = $('<div id="wfItem' + itemData.Id + '" class="symbol" symbolType="comment" endpoints="final" style="left: ' + itemData.PositionX +
            'px; top: ' + itemData.PositionY + 'px; width: 30px; padding: 3px; border: 2px solid grey; border-right: none; min-height: 60px;"> <span class="itemLabel">'
            + itemData.Label + '</span></div>');
        } else if (itemData.TypeClass == "symbol") {
            item = $('<img id="wfItem' + itemData.Id + '" class="symbol" symbolType="' + itemData.SymbolType +
            '" src="/Content/Images/TapestryIcons/' + itemData.SymbolType + '.png" style="left: ' + itemData.PositionX + 'px; top: '
            + itemData.PositionY + 'px;" />');

            if (itemData.SymbolType == "envelope-start" || itemData.SymbolType == "circle-event") {
                item.data('label', itemData.Label);
            }
        } else {
            item = $('<div id="wfItem' + itemData.Id + '" class="item" style="left: ' + itemData.PositionX + 'px; top: '
            + itemData.PositionY + 'px;"><span class="itemLabel">' + itemData.Label + '</span></div>');
        }
        item.addClass(itemData.TypeClass);
        if (itemData.ActionId != null)
            item.attr('actionId', itemData.ActionId);
        if (itemData.InputVariables != null)
            item.data('inputVariables', itemData.InputVariables);
        if (itemData.OutputVariables != null)
            item.data('outputVariables', itemData.OutputVariables);
        if (itemData.PageId != null)
            item.attr("pageId", itemData.PageId);
        if (itemData.ComponentName != null)
            item.attr('componentName', itemData.ComponentName);
        if (itemData.TargetId != null)
            item.attr('targetId', itemData.TargetId);
        if (itemData.StateId != null)
            item.attr("stateId", itemData.StateId);
        if (itemData.isAjaxAction != null)
            item.data('isAjaxAction', itemData.isAjaxAction);
        if (itemData.TypeClass == "circle-thick")
            item.attr("endpoints", "final");
        if (itemData.SymbolType && itemData.SymbolType.substr(0, 8) == "gateway-")
            item.attr("endpoints", "gateway");
        if (itemData.Condition != null)
            item.data("condition", itemData.Condition);
        if (itemData.ConditionSets != null)
            item.data("conditionSets", itemData.ConditionSets);
        if (itemData.IsBootstrap != null)
            item.attr('isBootstrap', itemData.IsBootstrap);
        if (itemData.IsForeachStart)
            item.append('<span class="fa fa-play"></span>');
        if (itemData.IsForeachEnd)
            item.append('<span class="fa fa-stop"></span>');
       
        if (itemData.TypeClass == 'actionItem') {
            if (itemData.Name) item.append('<span class="itemName">' + itemData.Name + '</span>');
            if (itemData.Comment) item.append('<span class="itemComment">' + itemData.Comment + '</span>');
            if (itemData.CommentBottom) item.find('.itemComment').addClass('bottom');
        }

        var target;
        switch (true) {
            case itemData.ParentForeachId && itemData.ParentForeachId > 0:
                target = parentSwimlane.find('.foreach[data-foreachid=' + itemData.ParentForeachId + ']');
                break;
            case itemData.ParentSubflowId && itemData.ParentSubflowId > 0:
                target = parentSwimlane.find('.subflow[data-subflowid=' + itemData.ParentSubflowId + ']');
                break;
            default:
                target = parentSwimlane.find('.swimlaneContentArea');
                break;
        }
        
        item.appendTo(target);
        AddToJsPlumb(item);

        TB.callHooks(TB.wfr.onCreateItem, item, []);
    },

    aliveRule: function(rule)
    {
        var self = TB.wfr;

        rule.draggable({
            containment: 'parent',
            handle: '.workflowRuleHeader',
            revert: self._ruleDraggableRevert,
            stop: self._ruleDraggableStop
        });
        rule.resizable({
            start: self._ruleResizableStart,
            resize: self._ruleResizableResize,
            stop: self._ruleResizableStop
        });
        CreateJsPlumbInstanceForRule(rule);
    },

    aliveSwimlane: function (swimlane) {
        var self = TB.wfr;

        swimlane.find('.swimlaneRolesArea').droppable({
            tolerance: 'touch',
            accept: '.toolboxItem.roleItem',
            greedy: true,
            drop: self._swimlaneRoleDrop
        });
        swimlane.find('.swimlaneContentArea').droppable({
            containment: '.swimlaneContentArea',
            tolerance: 'touch',
            accept: '.toolboxSymbol, .toolboxItem',
            greedy: false,
            drop: self._swimlaneItemDrop
        });
    },

    /******************************************************************/
    /* WORKFLOW RULE EVENTS                                           */
    /******************************************************************/

    _ruleDraggableRevert: function(event, ui) {
        return ($(this).collision('#workflowRulesPanel .workflowRule').length > 1);
    },

    _ruleDraggableStop: function (event, ui) {
        ChangedSinceLastSave = true; /// OBSOLATE
        TB.changedSinceLastSave = true;
    },

    _ruleResizableStart: function(event, ui) {
        var rule = $(this);
        var contentsWidth = 120;
        var contentsHeight = 40;
        var limits = TB.checkRuleResizeLimits(rule, false);

        rule.find('.item').each(function (index, element) {
            var rightEdge = $(element).position().left + $(element).width();
            var bottomEdge = $(element).position().top + $(element).height();

            contentsWidth = rightEdge > contentsWidth ? rightEdge : contentsWidth;
            contentsHeight = bottomEdge > contentsHeight ? bottomEdge : contentsHeight;
        });

        rule.css({
            'min-width': contentsWidth + 40, 'min-height': contentsHeight + 20,
            'max-width': limits.horizontal - 10, 'max-height': limits.vertical - 10
        });
    },

    _ruleResizableResize: function(event, ui) {
        var rule = $(this);
        var instance = rule.data("jsPlumbInstance");
        var limits = TB.checkRuleResizeLimits(rule, false);

        instance.recalculateOffsets();
        instance.repaintEverything();

        rule.css({'max-width': limits.horizontal - 10, 'max-height': limits.vertical - 10});
    },

    _ruleCopy: function () {
        var d = this;
        var appId = $('#currentAppId').val();
        var blockId = $('#currentBlockId').val();
        var wfrId = TB.wfr.currentRule.attr('data-id');
        var targetBlockId = $('#wr-copy-target').val();

        if (!targetBlockId) {
            alert('Vyberte cílový blok.');
        }
        else {
            var url = '/api/tapestry/apps/' + appId + '/blocks/' + blockId + '/copyWorkflow/' + wfrId + /target/ + targetBlockId;
            $.ajax({
                url: url,
                type: 'GET',
                data: {},
                success: function (data) {
                    if (data) {
                        alert('Workflow bylo úspěšně zkopírováno.');
                        TB.dialog.close.apply(d);
                    }
                    else {
                        alert('Workflow se nepodařilo zkopírovat.');
                    }
                }
            })
        }
    },

    _ruleCopyOpen: function() {
        $('#wr-copy-source').html(TB.wfr.currentRule.find('.verticalLabel').text());
        $('#wr-copy-target').val('');
    },

    /******************************************************************/
    /* SWIMLANE EVENTS                                                */
    /******************************************************************/
    _ruleResizableStop: function (event, ui) {
        ChangedSinceLastSave = true; /// OBSOLATE
        TB.changedSinceLastSave = true;
    },

    _swimlaneRoleDrop: function(event, ui) {
        if (dragModeActive)
        {
            dragModeActive = false;

            var roleExists = false;
            $(this).find('.roleItem').each(function (index, element) {
                if ($(element).text() == ui.helper.text())
                    roleExists = true;
            });
            if (!roleExists) {
                var role = ui.helper.clone();

                $(this).find('.rolePlaceholder').remove();
                $(this).find('.roleItemContainer').append($('<div class="roleItem">' + role.text() + '</div>'));
                ui.helper.remove();
                ChangedSinceLastSave = true; /// OBSOLATE
                TB.changedSinceLastSave = true;
            }
        }
    },

    _swimlaneItemDrop: function (event, ui) {
        if (dragModeActive)
        {
            dragModeActive = false;
            var leftOffset, topOffset;
            var item = ui.helper.clone();

            if (item.hasClass('roleItem')) {
                ui.draggable.draggable('option', 'revert', true);
                return false;
            }
            var ruleContent = $(this);
            ruleContent.append(item);

            if (item.hasClass('toolboxSymbol')) {
                item.removeClass('toolboxSymbol ui-draggable ui-draggable-dragging');
                item.addClass('symbol');
                item.css({ height: '' });
                leftOffset = $('#tapestryWorkspace').offset().left - ruleContent.offset().left;
                topOffset = $('#tapestryWorkspace').offset().top - ruleContent.offset().top;
            }
            else {
                item.removeClass('toolboxItem');
                item.addClass('item');
                item.css({ width: '', height: '' });
                leftOffset = $('#tapestryWorkspace').offset().left - ruleContent.offset().left + 38;
                topOffset = $('#tapestryWorkspace').offset().top - ruleContent.offset().top - 18;
            }
            item.offset({ left: item.offset().left + leftOffset, top: item.offset().top + topOffset });
            ui.helper.remove();
            AddToJsPlumb(item);
            if (item.position().top + item.height() + 10 > ruleContent.height()) {
                item.css('top', ruleContent.height() - item.height() - 20);
                var instance = ruleContent.parents('.workflowRule').data('jsPlumbInstance');
                instance.repaintEverything();
            }
            ChangedSinceLastSave = true; /// OBSOLATE
            TB.changedSinceLastSave = true;
        }
    },

    /******************************************************************/
    /* CONTEXT MENU ACTIONS                                           */
    /******************************************************************/
    _contextAction: function (key, options) {
        var self = TB.wfr;
        switch (key) {
            case 'delete': self.remove.apply(options.$trigger, []); break;
            case 'rename': self.rename.apply(options.$trigger, []); break;
            case 'copy':
                self.currentRule = options.$trigger;
                TB.dialog.open('workflowCopy');
                break;
            case 'add-swimlane': self.addSwimlane.apply(options.$trigger, []); break;
        }
    },
    
    _swimlaneContextAction: function (key, options) {
        if (key == 'remove-swimlane') {
            TB.wfr.removeSwimlane.apply(options.$trigger, []);
        }
        else if (key == 'group-to-subflow') {
            TB.subflow.groupToSubflow.apply(options.$trigger, []);
        }
        else if (key == 'group-to-foreach') {
            TB.foreach.groupToForeach.apply(options.$trigger, []);
        }
    },

    _roleContextAction: function (key, options) {
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

    _itemContextAction: function (key, options) {
        item = options.$trigger;
        switch (key) {
            case "delete": {
                currentInstance = item.parents(".rule").data("jsPlumbInstance");
                currentInstance.removeAllEndpoints(item, true);
                item.remove();
                ChangedSinceLastSave = true;
                break;
            }
            case "wizard": {
                item.addClass("activeItem processedItem");

                if (item.hasClass("actionItem") && item.parents(".rule").hasClass("workflowRule")) {
                    CurrentItem = item;
                    TB.wizard.open.apply(item, []);
                }
                else {
                    alert("Pro tento typ objektu nejsou dostupná žádná nastavení.");
                    item.removeClass("activeItem");
                }
                break;
            }
            case "properties": {
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
                else if (item.hasClass("symbol") && item.attr("symboltype") == "gateway-x") {
                    CurrentItem = item;
                    gatewayConditionsDialog.dialog("open");
                }
                else if (item.hasClass("symbol") && item.attr("symboltype") == "envelope-start") {
                    CurrentItem = item;
                    envelopeStartPropertiesDialog.dialog("open");
                }
                else if (item.hasClass("symbol") && item.attr("symboltype") == "envelope-start") {
                    CurrentItem = item;
                    envelopeStartPropertiesDialog.dialog("open");
                }
                else if (item.hasClass("symbol") && item.attr("symboltype") == "circle-event") {
                    CurrentItem = item;
                    circleEventPropertiesDialog.dialog("open");
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
                break;
            }
            case 'name': {
                CurrentItem = item;
                TB.dialog.open('actionItemName');
                break;
            }
            case 'comment': {
                CurrentItem = item;
                TB.dialog.open('actionItemComment');
                break;
            }
            case 'group-to-subflow': {
                TB.subflow.groupToSubflow.apply(options.$trigger, []);
                break;
            }
            case 'group-to-foreach': {
                TB.foreach.groupToForeach.apply(options.$trigger, []);
                break;
            }
            case 'set-as-fe-start': {
                TB.foreach.setStart.apply(options.$trigger, []);
                break;
            }
            case 'set-as-fe-end': {
                TB.foreach.setEnd.apply(options.$trigger, []);
                break;
            }
        }
    },

    /*******************************************************************/
    /* DIALOG ACTIONS                                                  */
    /*******************************************************************/
    _actionItemSetNameOpen: function() {
        $(this).find('#ActionName').val($(CurrentItem).find('.itemName').text());
    },

    _actionItemSetName: function () {
        var name = $(this).find('#ActionName').val();
        var item = $(CurrentItem);

        if (name.length) {
            if (!item.find('.itemName').length) {
                item.append('<span class="itemName" />');
            }
            item.find('.itemName').html(name);
        }
        else {
            item.find('.itemName').remove();
        }
        
        CurrentItem = null;
        ChangedSinceLastSave = true; /// OBSOLATE
        TB.changedSinceLastSave = true;
        TB.dialog.close.apply(this);
    },

    _actionItemSetCommentOpen: function() {
        $(this).find('#ActionComment').val($(CurrentItem).find('.itemComment').text());
        $(this).find('#ActionCommentBottom').prop('checked', $(CurrentItem).find('.itemComment').hasClass('bottom'));
    },

    _actionItemSetComment: function () {
        var comment = $(this).find('#ActionComment').val();
        var item = $(CurrentItem);

        if (name.length) {
            if (!item.find('.itemComment').length) {
                item.append('<span class="itemComment" />');
            }
            item.find('.itemComment').toggleClass('bottom', $('#ActionCommentBottom').is(':checked')).html(comment);
        }
        else {
            item.find('.itemComment').remove();
        }

        CurrentItem = null;
        ChangedSinceLastSave = true; /// OBSOLATE
        TB.changedSinceLastSave = true;
        TB.dialog.close.apply(this);
    },
    
    /*************************************************/
    /* SEARCH IN WORKFLOW's                          */
    /*************************************************/

    _keyDown: function (e) {
        if ((e.ctrlKey && e.shiftKey) || e.metaKey) {
            if (String.fromCharCode(e.which).toLowerCase() == 'f') {
                e.preventDefault();

                var $box = $('<div></div>');
                $box.css({
                    position: 'fixed',
                    right: 0,
                    top: 75,
                    border: '2px solid #457fa9',
                    background: '#49c3f1',
                    width: '20%',
                    padding: '10px'
                }).attr('id', 'SearchBox').appendTo('body');

                var $c = $('<button type="button" class="close" aria-label="Close" style="opacity:1"><span aria-hidden="true" style="color:#fff">&times;</span></button>');
                $c.appendTo($box);

                $box.append('<label class="control-label">Search in variables:</label>');

                var $g = $('<div class="from-group"></div>');
                $g.appendTo($box);
                
                var $ig = $('<div class="input-group input-group-sm"></div>');
                $ig.appendTo($g);

                var $f = $('<input type="text" id="SearchTerm" class="form-control" value="">');
                $f.appendTo($ig);

                var $b = $('<span class="input-group-btn"><button type="button" id="Search" class="btn btn-default"><span class="fa fa-search"></span></button></span>');
                $b.appendTo($ig);
            }
        }  
    },

    searchItems: function () {
        var term = $('#SearchTerm').val();
        $('.swimlaneContentArea .item')
            .removeClass('isMatch')
            .each(function () {
                if (!term.length) {
                    return false;
                }
                var inputVars = $(this).data('inputVariables');
                var outputVars = $(this).data('outputVariables');

                if ((inputVars && inputVars.indexOf(term) !== -1) || (outputVars && outputVars.indexOf(term) !== -1)) {
                    $(this).addClass('isMatch');
                }
            });

    },

    closeSearch: function () {
        $('#SearchBox').remove();
        $('.swimlaneContentArea .item').removeClass('isMatch')
    }
}

TB.onInit.push(TB.wfr.init);