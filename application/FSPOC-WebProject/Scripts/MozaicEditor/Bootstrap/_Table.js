MBE.types.table = {

    templates: {
        'table': '<table class="table"></table>',
        'tr': '<tr data-uic="table|tr"></tr>',
        'cell': '<td></td>',
        'td': '<td data-uic="table|td">Column</td>',
        'th': '<th data-uic="table|th">Cell</th>',
        'thead': '<thead data-uic="table|thead" locked></thead>',
        'tbody': '<tbody data-uic="table|tbody" locked></tbody>',
        'tfoot': '<tfoot data-uic="table|tfoot" locked></tfoot>',
        'caption': '<caption data-uic="table|caption" locked>Caption</caption>'
    },

    options: {
        'table': {
            'tableOptions': {
                name: 'Table options',
                type: 'group',
                groupItems: [{
                    label: 'Style',
                    type: 'boolean',
                    options: {
                        'table-striped': 'Striped',
                        'table-bordered': 'Bordered',
                        'table-hover': 'Hover',
                        'table-condensed': 'Condensed'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    name: 'Responsive',
                    type: 'boolean',
                    options: {
                        'table-responsive': 'Responsive'
                    },
                    get: function (value) {
                        return $(MBE.options.target).parent().is('.' + value);
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            $(this).wrap('<div class="table-responsive"></div>');
                        }
                        else {
                            $(this).unwrap();
                        }
                    }
                }, {
                    name: 'Show',
                    type: 'boolean',
                    options: {
                        'thead': 'Show table header',
                        'tfoot': 'Show table footer',
                        'caption': 'Show caption'
                    },
                    get: function (value) {
                        return $('> ' + value, MBE.options.target).length > 0;
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            switch (opt.value) {
                                case 'caption': {
                                    $(this).prepend($(MBE.types.table.templates['caption']));
                                    break;
                                }
                                case 'thead': {
                                    $('tbody', this).before(MBE.types.table.createTHead.apply(this, []));
                                    break;
                                }
                                case 'tfoot': {
                                    $('tbody', this).after(MBE.types.table.createTFoot.apply(this, []));
                                    break;
                                }
                            }
                        }
                        else {
                            $('> ' + opt.value, this).remove();
                        }
                        MBE.DnD.updateDOM();
                    }
                }]
            }
        },
        'caption': {
            'captionOptions': {
                name: 'Caption options',
                type: 'boolean',
                options: {
                    'lead': 'Lead'
                },
                set: MBE.options.toggleClass,
                get: MBE.options.hasClass
            },
            'textOptions': MBE.types.text.options.common.textOptions
        },
        'tr': {
            'trOptions': {
                name: 'Table row options',
                type: 'group',
                groupItems: [{
                    label: 'Style',
                    type: 'select',
                    options: {
                        'null': 'Defaut',
                        'active': 'Active',
                        'success': 'Success',
                        'info': 'Info',
                        'warning': 'Warning',
                        'danger': 'Danger'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'td': {
            'tdOptions': {
                name: 'Table cell options',
                type: 'group',
                groupItems: [{
                    label: 'Style',
                    type: 'select',
                    options: {
                        'null': 'Defaut',
                        'active': 'Active',
                        'success': 'Success',
                        'info': 'Info',
                        'warning': 'Warning',
                        'danger': 'Danger'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Rowspan',
                    type: 'number',
                    attr: 'rowspan',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Colspan',
                    type: 'number',
                    attr: 'colspan',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]
            },
            'textOptions': MBE.types.text.options.common.textOptions
        }
    },

    init: function()
    {
        var self = MBE.types.table;
        var menu = MBE.toolbar.menu;

        self.options.th = self.options.td;

        MBE.DnD.onDrop.push(self._drop);

        menu.table = {};
        menu.table.table = {
            items: [
                { type: 'text',   label: 'ADD ROW' },
                { type: 'button', label: 'ABOVE',   callback: self.rowAddAbove, allowFor: self.isCellOrRow },
                { type: 'button', label: 'BELOW',   callback: self.rowAddBelow, allowFor: self.isCellOrRow },
                { type: 'button', label: 'TOP',     callback: self.rowAddTop },
                { type: 'button', label: 'BOTTOM',  callback: self.rowAddBottom },
                { type: 'text',   label: 'ADD COLUMN' },
                { type: 'button', label: 'LEFT',    callback: self.columnAddLeft, allowFor: self.isCell },
                { type: 'button', label: 'RIGHT',   callback: self.columnAddRight, allowFor: self.isCell },
                { type: 'button', label: 'BEGIN',   callback: self.columnAddBegin },
                { type: 'button', label: 'END',     callback: self.columnAddEnd }
            ]
        };
        menu.table.thead = menu.table.tbody = menu.table.tfoot = menu.table.tr = menu.table.td = menu.table.th = menu.table.table;
    },

    _drop: function()
    {
        if ($(this).is('table:empty')) {
            MBE.types.table.build.apply(this, []);
        }
        if ($(this).is('[data-uic="table|cell"]')) {
            MBE.types.table.insertCell.apply(this, []);
        }
    },

    /****************************************************/
    /* TABLE CONTEXT METHODS                            */
    /****************************************************/
    build: function()
    {
        var self = MBE.types.table;
        var target = $(this);
        var thead = $(self.templates['thead']);
        var tbody = $(self.templates['tbody']);
        var row = $(self.templates['tr']);

        target.append(thead);
        target.append(tbody);

        var r = row.clone();
        thead.append(r);

        for (var i = 1; i <= 3; i++) {
            var cell = $(self.templates['th']);
            cell.html('Column ' + i);
            cell.appendTo(r);
        }

        for (var i = 0; i < 2; i++) {
            var r = row.clone();
            for (var j = 1; j <= 3; j++) {
                var cell = $(self.templates['td']);
                cell.html('Cell ' + j);
                cell.appendTo(r);
            }
            tbody.append(r);
        }
        MBE.DnD.updateDOM();
    },

    insertCell: function () {
        var self = MBE.types.table;
        var elm = $(this);
        if (elm.parent().parent().is('thead')) {
            elm.replaceWith($(self.templates['th']));
        }
        else {
            elm.replaceWith($(self.templates['td']));
        }
    },

    getCellCount: function() {
        var maxCellCount = 0;
        $('tbody tr', this).each(function () {
            var cellCount = 0;
            $('td', this).each(function () {
                cellCount += this.colSpan ? Number(this.colSpan) : 1;
            });
            if (cellCount > maxCellCount) {
                maxCellCount = cellCount;
            }
        });
        return maxCellCount;
    },

    createTHead: function ()
    {
        var self = MBE.types.table;
        var thead = $(self.templates['thead']);
        var row = $(self.templates['tr']);
        var maxCellCount = self.getCellCount.apply(this, []);

        for (var i = 1; i <= maxCellCount; i++) {
            var cell = $(self.templates['th']);
            cell.html('Column ' + i).appendTo(row);
        }
        thead.append(row);
        return thead;
    },

    createTFoot: function ()
    {
        var self = MBE.types.table;
        var tfoot = $(self.templates['tfoot']);
        var row = $(self.templates['tr']);
        var maxCellCount = self.getCellCount.apply(this, []);

        for (var i = 1; i <= maxCellCount; i++) {
            var cell = $(self.templates['td']);
            cell.html('Summary ' + i).appendTo(row);
        }
        tfoot.append(row);
        return tfoot;
    },

    isCell: function() {
        return $(this).is('td, th');
    },

    isCellOrRow: function() {
        return $(this).is('td, th, tr');
    },

    addRow: function(pos) 
    {
        var self = MBE.types.table;
        var elm = $(this);
        var table = elm.is('table') ? elm : elm.parents('table').eq(0);
        var target = elm.is('table') ? elm.find('tbody') : elm.parents('tbody, thead, tfoot').eq(0);
        var currentRow = elm.is('tr') ? elm : (elm.is('td, th') ? elm.parent() : null);
        var row = $(self.templates['tr']);
        var maxCellCount = self.getCellCount.apply(table[0], []);

        for (var i = 1; i <= maxCellCount; i++) {
            var cell = $(self.templates[target.is('thead') ? 'th' : 'td']);
            cell.html((target.is('thead') ? 'Column' : (target.is('tfoot') ? 'Summary' : 'Cell')) + ' ' + i).appendTo(row);
        } 

        switch (pos) {
            case 'top': target.prepend(row); break;
            case 'bottom': target.append(row); break;
            case 'above': currentRow.before(row); break;
            case 'below': currentRow.after(row); break;
        }
        MBE.DnD.updateDOM();
    },

    addColumn: function (pos) {
        var self = MBE.types.table;
        var elm = $(this);
        var table = elm.is('table') ? elm : elm.parents('table').eq(0);
        var index = elm.is('th, td') ? elm[0].cellIndex : null;

        // Pro thead, tbody i tfoot ....
        table.find('> thead, > tbody, > tfoot').each(function () {
            var e = $(this);
            var cell = $(self.templates[e.is('thead') ? 'th' : 'td']);
            cell.html(e.is('thead') ? 'Column' : (e.is('tfoot') ? 'Summary' : 'Cell'));

            // ... projdeme všechny řádky ...
            e.find('> tr').each(function () {
                // ... a umístíme novou buňku na správnou pozici
                switch (pos) {
                    case 'begin': $(this).prepend(cell.clone()); break;
                    case 'end': $(this).append(cell.clone()); break;
                    case 'left': $(this).find('> td, > th').eq(index).before(cell.clone()); break;
                    case 'right': $(this).find('> td, > th').eq(index).after(cell.clone()); break;
                }
            });
        });

        MBE.DnD.updateDOM();
    },

    rowAddAbove:    function () { MBE.types.table.addRow.apply(this, ['above']);    },
    rowAddBelow:    function () { MBE.types.table.addRow.apply(this, ['below']);    },
    rowAddTop:      function () { MBE.types.table.addRow.apply(this, ['top']);      },
    rowAddBottom:   function () { MBE.types.table.addRow.apply(this, ['bottom']);   },

    columnAddLeft:  function () { MBE.types.table.addColumn.apply(this, ['left']);  },
    columnAddRight: function () { MBE.types.table.addColumn.apply(this, ['right']); },
    columnAddBegin: function () { MBE.types.table.addColumn.apply(this, ['begin']); },
    columnAddEnd:   function () { MBE.types.table.addColumn.apply(this, ['end']);   }
};
MBE.onInit.push(MBE.types.table.init);