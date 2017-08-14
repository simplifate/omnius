TB.selection = {

    holdTimer: null,
    pos: [],
    target: null,
    itemsCache: [],
    selecting: false,
    selection: null,
    
    init: function () {
        var self = TB.selection;

        $(document)
            .on('mousedown', $.proxy(self._mouseDown, self))
            .on('mousemove', $.proxy(self._mouseMove, self))
            .on('mouseup', $.proxy(self._mouseUp, self))
            ;

        TB.load.onLoadBlock.push(self._blockLoad);
    },
    
    /******************************************************/
    /* SELECTION EVENTS                                   */
    /******************************************************/
    _blockLoad: function () {
        $('.swimlaneContentArea').css({
            '-webkit-touch-callout': 'none',
            '-webkit-user-select': 'none',
            '-khtml-user-select': 'none',
            '-moz-user-select': 'none',
            '-ms-user-select': 'none',
            'user-select': 'none'
        });
    },

    _mouseDown: function (e) {
        var target = $(e.target);
        var that = this;

        if (target.is('.swimlaneContentArea') && e.shiftKey && e.which === 1) {
            this.holdTimer = setTimeout(function () {
                that.target = target;
                that._mouseHold.apply(that, [e]);
            }, 50);
        }
    },

    _mouseHold: function (e) {
        e.preventDefault();
        e.stopPropagation();

        var x = (e.pageX - this.target.offset().left);
        var y = (e.pageY - this.target.offset().top);

        var items = this.target.find('> .item, > .symbol');
        for (var i = 0; i < items.length; i++) {
            this.itemsCache.push({
                element: items.eq(i),
                selected: items.eq(i).hasClass('nu-selected'),
                selecting: false,
                position: items[i].getBoundingClientRect()
            });
        }

        this.pos = [x, y];
        this.selecting = true;
        this._createSelection(x, y);
    },

    _mouseMove: function (e) {
        var pos = this.pos;
        if (!pos.length)
            return;

        e.preventDefault();
        e.stopPropagation();

        var x = e.pageX - this.target.offset().left;
        var y = e.pageY - this.target.offset().top;

        var newPos = [x, y],
            width = Math.abs(newPos[0] - pos[0]),
            height = Math.abs(newPos[1] - pos[1]),
            left, top;

        left = (newPos[0] < pos[0]) ? (pos[0] - width) : pos[0];
        top = (newPos[1] < pos[1]) ? (pos[1] - height) : pos[1];

        this._drawSelection(width, height, left, top);
        this._detectCollision();
    },

    _mouseUp: function (e) {
        clearTimeout(this.holdTimer);

        if (!this.pos.length) {
            if (e.which !== 3) {
                this._clearSelection();
            }
            return;
        }

        e.preventDefault();
        e.stopPropagation();

        this.selecting = false;
        this.selection.remove();

        var x = e.pageX - this.target.offset().left;
        var y = e.pageY - this.target.offset().top;

        if (x === this.pos[0] && y === this.pos[1]) {
            this._clearSelection();
        }

        this.target = null;
        this.pos = [];
    },

    _createSelection: function (x, y) {
        this.selection = $('<div class="nu-selection-box" />');

        this.selection.css({
            'position': 'absolute',
            'top': y + 'px',
            'left': x + 'px',
            'width': '0',
            'height': '0',
            'z-index': '999',
            'overflow': 'hidden'
        }).appendTo(this.target);
    },

    _drawSelection: function (width, height, x, y) {
        this.selection.css({
            'width': width,
            'height': height,
            'top': y,
            'left': x
        });
    },

    _clearSelection: function () {
        $('.nu-selected').removeClass('nu-selected');
    },

    _detectCollision: function () {
        var selector = this.selection[0].getBoundingClientRect(),
            dataLength = this.itemsCache.length;

        for (var i = dataLength - 1, item; item = this.itemsCache[i], i >= 0; i--) {
            var collided = !(selector.right < item.position.left ||
                selector.left > item.position.right ||
                selector.bottom < item.position.top ||
                selector.top > item.position.bottom);

            if (collided) {
                if (item.selected) {
                    item.element.removeClass('nu-selected');
                    item.selected = false;
                }
                if (!item.selected) {
                    item.element.addClass('nu-selected');
                    item.selected = true;
                }
            }
            else {
                if (this.selecting) {
                    item.element.removeClass('nu-selected');
                }
            }

        }
    }
}

TB.onInit.push(TB.selection.init);