TB.rr = {

    templates: {
        rule: '<div class="rule resourceRule"></div>',
        item: '<div class="item"></div>'
    },

    create: function(rrData)
    {
        var self = TB.rr;

        var rule = $(self.templates.rule);
        rule.css({
            width: rrData.Width,
            height: rrData.Height,
            left: rrData.PositionX,
            top: rrData.PositionY
        }).attr('id', AssingID()).appendTo('#resourceRulesPanel .scrollArea');

        self.alive(rule);

        return rule;
    },

    createItem: function(itemData, parentRule)
    {
        var self = TB.rr;

        var item = $(self.templates.item);

        var attrs = {};
        attrs.id = 'resItem' + itemData.Id;

        if (itemData.ActionId != null)          attrs.actionId = itemData.ActionId;
        if (itemData.StateId != null)           attrs.stateId = itemData.StateId;
        if (itemData.PageId != null)            attrs.pageId = itemData.PageId;
        if (itemData.ComponentName != null)     attrs.componentName = itemData.ComponentName;
        if (itemData.ColumnName != null)        attrs.columnName = itemData.ColumnName;
        if (itemData.IsShared != null)          attrs.shared = itemData.IsShared;
        if (itemData.IsBootstrap != null)       attrs.isBootstrap = itemData.IsBootstrap;
        if (itemData.BootstrapPageId != null)   attrs.pageId = itemData.BootstrapPageId;

        item
            .attr(attrs)
            .css({
                left: itemData.PositionX,
                top: itemData.PositionY
            })
            .html(itemData.Label)
            .addClass(itemData.TypeClass)
            .appendTo(parentRule);

        if (itemData.ConditionSets != null) {
            item.data('conditionSets', itemData.ConditionSets);
        }
        if (itemData.TableName != null) {
            item.data('columnFilter', itemData.ColumnFilter);
            item.attr('tableName', itemData.TableName);
            if (itemData.Label.indexOf('View:') == 0)
                item.addClass('viewAttribute');
            else
                item.addClass('tableAttribute');
        }

        AddToJsPlumb(item);
    },

    alive: function (rule) {
        var self = TB.rr;
        
        rule.draggable({
            containment: 'parent',
            revert: self._draggableRevert,
            stop: self._draggableStop
        });

        rule.resizable({
            start: self._resizableStart,
            stop: self._resizableStop
        });

        rule.droppable({
            containment: '.resourceRule',
            tolerance: 'touch',
            accept: '.toolboxItem',
            greedy: true,
            drop: self._droppableDrop
        });

        CreateJsPlumbInstanceForRule(rule);
    },

    _draggableRevert: function(event, ui) {
        return ($(this).collision('#resourceRulesPanel .resourceRule').length > 1);
    },

    _draggableStop: function(event, ui) {
        ChangedSinceLastSave = true; /// OBSOLATE
        TB.changedSinceLastSave = true;
    },

    _resizableStart: function(event, ui) {
        var rule = $(this);
        var minWidth = 120;
        var minHeight = 40;

        rule.find('.item').each(function (index, element) {
            var $elm = $(element);
            var rightEdge = $elm.position().left + $elm.width();
            var bottomEdge = $elm.position().top + $elm.height();

            minWidth = rightEdge > minWidth ? rightEdge : minWidth;
            minHeight = bottomEdge > minHeight ? bottomEdge : minHeight;
        });

        var limits = TB.checkRuleResizeLimits(rule, true);

        rule.css({
            'min-width': minWidth + 40,
            'min-height': minHeight + 20,
            'max-width': limits.horizontal - 10,
            'max-height': limits.vertical - 10
        });
    },

    _resizableStop: function(event, ui) {
        var instance = $(this).data("jsPlumbInstance");
        instance.recalculateOffsets();
        instance.repaintEverything();
        ChangedSinceLastSave = true; /// OBSOLATE
        TB.changedSinceLastSave = true;
    },

    _droppableDrop: function (event, ui) {
        if (dragModeActive) {
            dragModeActive = false;

            var target = $(this);

            var leftOffset = $("#tapestryWorkspace").offset().left - target.offset().left + 20;
            var topOffset = $("#tapestryWorkspace").offset().top - target.offset().top;

            var item = ui.helper.clone();
            item.removeClass('toolboxItem')
                .addClass('item')
                .css({ width: '', height: '' })
                .appendTo(target);

            item.offset({
                left: item.offset().left + leftOffset,
                top: item.offset().top + topOffset
            });
            
            ui.helper.remove();
            AddToJsPlumb(item);

            if (item.position().left + item.width() + 35 > target.width()) {
                item.css("left", target.width() - item.width() - 40);
                var instance = target.data("jsPlumbInstance");
                instance.repaintEverything();
            }
            if (item.position().top + item.height() + 5 > target.height()) {
                item.css("top", target.height() - item.height() - 15);
                var instance = target.data("jsPlumbInstance");
                instance.repaintEverything();
            }
            ChangedSinceLastSave = true; /// OBSOLATE
            TB.changedSinceLastSave = true;
        }
    }
};