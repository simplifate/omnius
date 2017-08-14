TB.subflow = {

    target: null,

    contextItems: {
        'name': { name: 'Name...', icon: 'fa-tag' },
        'comment': { name: 'Comment...', icon: 'fa-comment' },
        'break': { name: 'Break subflow', icon: 'fa-object-ungroup' }
    },

    init: function () {
        var self = TB.subflow;
    },

    cannotBeGruped: function (key, options) {
        if(options.$trigger.is('.item') || options.$trigger.is('.symbol'))
            return options.$trigger.parents('.swimlane').find('.nu-selected').length == 0;
        else
            return options.$trigger.find('.nu-selected').length == 0;
    },

    groupToSubflow: function () {
        var self = TB.subflow;

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

            var subflow = self.createSubflow({
                PositionX: minX - 15,
                PositionY: minY - 15,
                Width: maxX - minX + 30,
                Height: maxY - minY + 30,
                Id: ''
            }, target);

            target.find('.swimlaneContentArea').append(subflow);
            subflow.append(items);

            var sX = subflow.position().left;
            var sY = subflow.position().top;

            subflow.find('> .item, > .symbol').each(function () {
                var e = $(this);
                e.css({
                    top: e.position().top - sY - 1,
                    left: e.position().left - sX - 1
                });
            });

            self.alive(subflow);
            TB.selection._clearSelection();
            TB.changedSinceLastSave = true;
            ChangedSinceLastSave = true; /// OBSOLATE
        }
    },

    breakSubflow: function(subflow) {
        var sX = subflow.position().left + 1;
        var sY = subflow.position().top + 1;

        subflow.find('> .item, > .symbol').each(function () {
            $(this).css({
                left: '+=' + sX,
                top: '+=' + sY
            }).appendTo(subflow.parent());
        });

        subflow.remove();
        TB.changedSinceLastSave = true;
        ChangedSinceLastSave = true; /// OBSOLATE
    },

    createSubflow: function (subflowData, parentSwimlane) {
        var subflow = $('<div class="subflow" />');
        subflow.css({
            width: subflowData.Width,
            height: subflowData.Height,
            left: subflowData.PositionX,
            top: subflowData.PositionY
        }).attr({
            'data-subflowid': subflowData.Id
        });

        if (subflowData.Name) {
            subflow.append('<span class="subflowName">' + subflowData.Name + '</span>');
        }
        if (subflowData.Comment) {
            subflow.append('<span class="subflowComment' + (subflowData.CommentBottom ? ' bottom' : '') + (subflowData.Name ? ' withName' : '') + '">' + subflowData.Comment + '</span>');
        }

        parentSwimlane.find('.swimlaneContentArea').append(subflow);
        this.alive(subflow);

        return subflow;
    },

    alive: function(subflow) {
        subflow.resizable({});
        subflow.draggable({
            drag: this._subflowDrag,
            stop: this._subflowStop
        });
    },

    setName: function() {
        var self = TB.subflow;
        var name = $(this).find('#SubflowName').val();

        if (name.length) {
            if (!self.target.find('> .subflowName').length) {
                self.target.append('<span class="subflowName" />');
            }
            self.target.find('> .subflowName').html(name);
            self.target.find('> .subflowComment').addClass('withName');
        }
        else {
            self.target.find('> .subflowName').remove();
            self.target.find('> .subflowComment').removeClass('withName');
        }

        TB.dialog.close.apply(this);
    },

    setComment: function() {
        var self = TB.subflow;
        var comment = $(this).find('#SubflowComment').val();
        var commentBottom = $(this).find('#SubflowCommentBottom').is(':checked');

        if (comment.length) {
            if (!self.target.find('> .subflowComment').length) {
                self.target.append('<span class="subflowComment" />');
            }
            self.target.find('> .subflowComment')
                .html(comment)
                .toggleClass('bottom', commentBottom)
                .toggleClass('withName', self.target.find('> .subflowName').length > 0);
        }
        else {
            self.target.find('> .subflowComment').remove();
        }

        TB.dialog.close.apply(this);
    },

    /******************************************************/
    /* SUBFLOW EVENTS                                     */
    /******************************************************/
    _subflowDrag: function () {
        var rule = $(this).parents('.rule');
        rule.data('jsPlumbInstance').repaintEverything();
    },

    _subflowStop: function() {
        var instance = $(this).parents('.rule').data('jsPlumbInstance');
        instance.recalculateOffsets();
        instance.repaintEverything();
        TB.changedSinceLastSave = true;
    },

    _contextAction: function(key, options) {
        var self = TB.subflow;
        switch (key) {
            case 'name': {
                self.target = options.$trigger;
                TB.dialog.open('subflowName');
                break;
            }
            case 'comment': {
                self.target = options.$trigger;
                TB.dialog.open('subflowComment');
                break;
            }
            case 'break': {
                self.breakSubflow(options.$trigger);
                break;
            }
        }
    },

    _setNameOpen: function() {
        $(this).find('#SubflowName').val(TB.subflow.target.find('> .subflowName').text());
    },

    _setCommentOpen: function() {
        $(this).find('#SubflowComment').val(TB.subflow.target.find('> .subflowComment').text());
        $(this).find('#SubflowCommentBottom').prop('checked', TB.subflow.target.find('> .subflowComment.bottom').length > 0);
    }
}

TB.onInit.push(TB.subflow.init);