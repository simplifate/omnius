﻿TB.wfr = {

    onCreateItem: [],

    templates: {
        rule: '<div class="rule workflowRule"><div class="workflowRuleHeader"><div class="verticalLabel" style="margin-top: 0px;"></div></div><div class="swimlaneArea"></div></div>',
        swimlane: '<div class="swimlane"><div class="swimlaneRolesArea"><div class="roleItemContainer"></div><div class="rolePlaceholder"><div class="rolePlaceholderLabel">Pokud chcete specifikovat roli<br />'
            + 'přetáhněte ji do této oblasti</div></div></div><div class="swimlaneContentArea"></div></div>',
        item: ''
    },
    
    contextItems: {
        'add-swimlane': { name: 'Add swimlane', icon: 'add' },
        'rename': { name: 'Rename rule', icon: 'edit' },
        'delete': { name: 'Delete rule', icon: 'delete' }
    },

    swimlaneContextItems: {
        'remove-swimlane': { name: 'Remove swimlane', icon: 'delete' },
    },

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
        .attr('id', AssingID())
        .appendTo("#workflowRulesPanel .scrollArea");
        
        self.aliveRule(rule);
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
        var item;
        if (itemData.TypeClass === "symbol" && itemData.SymbolType === "comment") {
            item = $('<div id="wfItem' + itemData.Id + '" class="symbol" symbolType="comment" endpoints="final" style="left: ' + itemData.PositionX +
            'px; top: ' + itemData.PositionY + 'px; width: 30px; padding: 3px; border: 2px solid grey; border-right: none; min-height: 60px;"> <span class="itemLabel">'
            + itemData.Label + '</span></div>');
        } else if (itemData.TypeClass == "symbol") {
            item = $('<img id="wfItem' + itemData.Id + '" class="symbol" symbolType="' + itemData.SymbolType +
            '" src="/Content/Images/TapestryIcons/' + itemData.SymbolType + '.png" style="left: ' + itemData.PositionX + 'px; top: '
            + itemData.PositionY + 'px;" />');

            if (itemData.SymbolType == "envelope-start") {
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
        if (itemData.TypeClass == "integrationItem")
            item.addClass('integrationItem');
        
        item.appendTo(parentSwimlane.find('.swimlaneContentArea'));
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

    _contextAction: function (key, options) {
        var self = TB.wfr;
        switch (key) {
            case 'delete': self.remove.apply(options.$trigger, []); break;
            case 'rename': self.rename.apply(options.$trigger, []); break;
            case 'add-swimlane': self.addSwimlane.apply(options.$trigger, []); break;
        }
    },
    
    _swimlaneContextAction: function (key, options) {
        if (key == 'remove-swimlane') {
            TB.wfr.removeSwimlane.apply(options.$trigger, []);
        }
    },

    /*************************************************/
    /* SEARCH IN WORKFLOW's                          */
    /*************************************************/

    _keyDown: function (e) {
        if (e.ctrlKey || e.metaKey) {
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