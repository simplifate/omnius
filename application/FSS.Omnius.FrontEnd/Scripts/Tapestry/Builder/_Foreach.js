TB.foreach = {

    contextItems: {
        'datasource': { name: 'Set datasource...', icon: 'fa-database' },
        'name': { name: 'Name...', icon: 'fa-tag' },
        'comment': { name: 'Comment...', icon: 'fa-comment' },
        'break': { name: 'Break foreach', icon: 'fa-times-circle' }
    },

    target: null,
    id2VirtualId: {},

    init: function () {
        var self = TB.foreach;

        TB.wfr.onCreateRule.push(self._onCreateRule);
    },
    
    cannotBeGruped: function (key, options) {
        if (options.$trigger.is('.item') || options.$trigger.is('.symbol'))
            return options.$trigger.parents('.swimlane').find('.nu-selected').length == 0;
        else
            return options.$trigger.find('.nu-selected').length == 0;
    },

    isNotInForeach: function (key, options) {
        return !options.$trigger.parent().is('.foreach') || options.$trigger.is('.symbol');
    },

    groupToForeach: function () {
        var self = TB.foreach;

        var target = this.is('.item') || this.is('.symbol') ? this.parents('.swimlane') : this;
        var items = target.find('.nu-selected');
        if (items.length) {
            var minX = null, minY = null, maxX = null, maxY = null;

            items.each(function () {
                var p = $(this).position();
                var w = $(this).outerWidth(false);
                var h = $(this).outerHeight(false);

                minX = minX == null || p.left < minX ? p.left : minX;
                minY = minY == null || p.top < minY ? p.top : minY;
                maxX = maxX == null || (p.left + w) > maxX ? p.left + w : maxX;
                maxY = maxY == null || (p.top + h) > maxY ? p.top + h : maxY;
            });

            var foreach = self.createForeach({
                PositionX: minX - 20,
                PositionY: minY - 20,
                Width: maxX - minX + 40,
                Height: maxY - minY + 40,
                Id: ''
            }, target, items);
            
            var sX = foreach.position().left;
            var sY = foreach.position().top;

            foreach.find('> .item, > .symbol').each(function () {
                var e = $(this);
                e.css({
                    top: e.position().top - sY - 1,
                    left: e.position().left - sX - 1
                });
            });
            self.target = foreach;

            TB.selection._clearSelection();
            TB.dialog.open('foreachDatasource');
            TB.changedSinceLastSave = true;
            ChangedSinceLastSave = true; /// OBSOLATE
        }
    },

    breakForeach: function (foreach) {
        var sX = foreach.position().left + 1;
        var sY = foreach.position().top + 1;

        foreach.find('> .item, > .symbol').each(function () {
            $(this).css({
                left: '+=' + sX,
                top: '+=' + sY
            }).appendTo(foreach.parent()).find('.fa').remove();
        });

        var instance = foreach.parents('.rule').data('jsPlumbInstance');
        instance.removeAllEndpoints(foreach, true);
        
        foreach.remove();
        TB.changedSinceLastSave = true;
        ChangedSinceLastSave = true; /// OBSOLATE
    },

    createForeach: function (foreachData, parentSwimlane, items, blockLoad) {
        var foreach = $('<div class="foreach" />');
        foreach.css({
            width: foreachData.Width,
            height: foreachData.Height,
            left: foreachData.PositionX,
            top: foreachData.PositionY
        }).attr({
            'data-foreachid': foreachData.Id
        });

        if (foreachData.Id) {
            foreach.attr('id', 'wfItem' + this.id2VirtualId[foreachData.Id]);
        }
       
        if (items) {
            foreach.append(items);
        }

        if (foreachData.DataSource) {
            foreach.attr('data-datasource', foreachData.DataSource);
        }
        if (foreachData.Name) {
            foreach.append('<span class="foreachName">' + foreachData.Name + '</span>');
        }
        if (foreachData.Comment) {
            foreach.append('<span class="foreachComment' + (foreachData.CommentBottom ? ' bottom' : '') + (foreachData.Name ? ' withName' : '') + '">' + foreachData.Comment + '</span>');
        }

        foreach.append('<span class="fa fa-repeat fa-spin"></span>');
        parentSwimlane.find('.swimlaneContentArea').append(foreach);
        this.alive(foreach, blockLoad);

        return foreach;
    },

    alive: function (foreach, blockLoad) {
    
        var itemID = foreach.attr('id') || AssingID();

        foreach.attr('id', itemID);
        foreach.resizable({
            resize: this._foreachResize,
            stop: this._foreachResizeStop
        });
        foreach.draggable({
            drag: this._foreachDrag,
            stop: this._foreachDragStop
        });
        foreach.droppable({
            tolerance: 'touch',
            accept: '.toolboxSymbol, .toolboxItem',
            greedy: true,
            drop: TB.wfr._swimlaneItemDrop
        });

        var instance = foreach.parents('.rule').data('jsPlumbInstance');
        instance.addEndpoint(itemID, sourceEndpoint, { anchor: 'RightMiddle', uuid: itemID + "RightMiddle" });
        instance.makeTarget(foreach, {
            dropOptions: { hoverClass: 'dragHover' },
            anchor: 'Continuous',
            allowLoopback: false
        });

        if (blockLoad) {
            return;
        }

        // Nějaký podivný bug - musíme si connections nejdřív naklonovat
        var connections = [];
        $.each(instance.getAllConnections(), function () {
            connections.push(this);
        });
        var s = '#' + itemID;
        var errors = [];

        // Přepojíme šipky, pokud existují
        $.each(connections, function () {
            var source = $(this.source);
            var target = $(this.target);
            
            // Zdroj je mimo FE - cíl je ve FE (považujme ho za počátek)
            if (!source.parents(s).length && target.parents(s).length) {
                var type = this.getType();
                var uuids = this.getUuids();

                if (target.is('.symbol')) {
                    errors.push('Cyklus nemůže začínat symbolem. Upravte spojení manuálně a označte počáteční akci.');
                }
                else if (!foreach.find('.fa-play').length) {
                    target.append('<span class="fa fa-play"></span>');
                    jsPlumb.detach(this);
                    instance.connect({
                        source: source[0],
                        target: foreach[0],
                        uuids: [uuids[0], null],
                        type: type
                    });
                }
                else {
                    foreach.find('.fa-play').remove();
                    errors.push('Nelze jednoznačně určit počáteční akci. Upravte spojení manuálně a označte počáteční akci.');
                }
            }
            // Zdroj je ve FE - cíl je mimo FE (považujme ho za konec)  
            if (source.parents(s).length && !target.parents(s).length) {
                var type = this.getType();

                if (target.is('.symbol')) {
                    errors.push('Cyklus nemůže končit symbolem. Upravte spojení manuálně a označte koncovou akci.');
                }
                else if (!foreach.find('.fa-stop').length) {
                    source.append('<span class="fa fa-stop"></span>');
                    jsPlumb.detach(this);
                    instance.connect({
                        source: foreach[0],
                        target: target[0],
                        uuids: [itemID + "RightMiddle", null],
                        type: type
                    });
                }
                else {
                    foreach.find('.fa-stop').remove();
                    errors.push('Nelze jednoznačně určit koncovou akci. Upravte spojení manuálně a označte koncovou akci.');
                }
            }
        });

        if (!foreach.find('.fa-play').length) {
            errors.push('Nebyl nalezen začátek cyklu. Upravte spojení manuálně a označte počáteční akci.');
        }
        if (!foreach.find('.fa-stop').length) {
            errors.push('Nebyl nalezen konec cyklu. Upravte spojení mauálně a označte koncovou akci.');
        }

        if (errors.length) {
            var d = $('<div title="' + document.title + ' saying"></div>');
            d.append('<p class="text-danger text-nowrap" style="margin:0">' + errors.join('<br>') + '</p>');

            d.dialog({
                resizable: false,
                draggable: false,
                modal: true,
                width: 'auto',
                minHeight: 0
            });
        }
    },

    setStart: function () {
        if (!this.parent().is('.foreach'))
            return;

        this.parent().find('> .item .fa-play').remove();
        this.find('.fa').remove().end().append('<span class="fa fa-play"></span>');
    },

    setEnd: function () {
        if (!this.parent().is('.foreach'))
            return;

        this.parent().find('> .item .fa-stop').remove();
        this.find('.fa').remove().end().append('<span class="fa fa-stop"></span>');
    },

    setDatasource: function () {
        var self = TB.foreach;
        var datasource = $(this).find('#ForeachDatasource').val();

        self.target.attr('data-datasource', datasource);
        TB.dialog.close.apply(this);
    },

    setName: function () {
        var self = TB.foreach;
        var name = $(this).find('#ForeachName').val();

        if (name.length) {
            if (!self.target.find('> .foreachName').length) {
                self.target.append('<span class="foreachName" />');
            }
            self.target.find('> .foreachName').html(name);
            self.target.find('> .foreachComment').addClass('withName');
        }
        else {
            self.target.find('> .foreachName').remove();
            self.target.find('> .foreachComment').removeClass('withName');
        }

        TB.dialog.close.apply(this);
    },

    setComment: function () {
        var self = TB.foreach;
        var comment = $(this).find('#ForeachComment').val();
        var commentBottom = $(this).find('#ForeachCommentBottom').is(':checked');

        if (comment.length) {
            if (!self.target.find('> .foreachComment').length) {
                self.target.append('<span class="foreachComment" />');
            }
            self.target.find('> .foreachComment')
                .html(comment)
                .toggleClass('bottom', commentBottom)
                .toggleClass('withName', self.target.find('> .foreachName').length > 0);
        }
        else {
            self.target.find('> .foreachComment').remove();
        }

        TB.dialog.close.apply(this);
    },

    /******************************************************/
    /* Foreach EVENTS                                     */
    /******************************************************/
    
    _foreachDrag: function () {
        var rule = $(this).parents('.rule');
        rule.data('jsPlumbInstance').repaintEverything();
    },

    _foreachDragStop: function () {
        var instance = $(this).parents('.rule').data('jsPlumbInstance');
        instance.recalculateOffsets();
        instance.repaintEverything();
        TB.changedSinceLastSave = true;
    },

    _foreachResize: function () {
        var rule = $(this).parents('.rule');
        rule.data('jsPlumbInstance').repaintEverything();
    },

    _foreachResizeStop: function () {
        var instance = $(this).parents('.rule').data('jsPlumbInstance');
        instance.recalculateOffsets();
        instance.repaintEverything();
        TB.changedSinceLastSave = true;
    },

    _contextAction: function (key, options) {
        var self = TB.foreach;
        switch (key) {
            case 'datasource': {
                self.target = options.$trigger;
                TB.dialog.open('foreachDatasource');
                break;
            }
            case 'name': {
                self.target = options.$trigger;
                TB.dialog.open('foreachName');
                break;
            }
            case 'comment': {
                self.target = options.$trigger;
                TB.dialog.open('foreachComment');
                break;
            }
            case 'break': {
                self.breakForeach(options.$trigger);
                break;
            }
        }
    },

    _setDatasourceOpen: function () {
        var t = TB.foreach.target;
        $(this).find('#ForeachDatasource').val(t.attr('data-datasource'));

        var categoryName = 'Workflow: ' + t.parents('.rule').find('.verticalLabel').text();
        var variables = [];

        for (var i = 0; i < TB.wizard.variableList.length; i++) {
            var v = TB.wizard.variableList[i];
            if (v.category == categoryName) {
                variables.push(v);
            }
        }

        $(this).find('#ForeachDatasource').autocomplete({
            delay: 0,
            source: variables
        });
    },

    _setNameOpen: function () {
        $(this).find('#ForeachName').val(TB.foreach.target.find('> .foreachName').text());
    },

    _setCommentOpen: function () {
        $(this).find('#ForeachComment').val(TB.foreach.target.find('> .foreachComment').text());
        $(this).find('#ForeachCommentBottom').prop('checked', TB.foreach.target.find('> .foreachComment.bottom').length > 0);
    },

    _beforeConnectionDrop: function (info) {
        console.log(info);

        if ($('#' + info.sourceId).parent().is('#' + info.targetId)) {
            return false;
        }

        return true;
    },

    _onCreateRule: function () {
        var instance = this.data('jsPlumbInstance');
        instance.bind('beforeDrop', TB.foreach._beforeConnectionDrop);
    }
}

TB.onInit.push(TB.foreach.init);