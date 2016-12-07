MBE.types.ui = {

    templates: {
        'horizontal-form-row': '<div></div>',
        'nv-list': '<table class="table name-value-list"></table>',
        'data-table': '<table class="table data-table"></table>',
        'countdown': '<div class="countdown-component">' +
                        '<span class="countdown-row countdown-show3">' + 
                            '<span class="countdown-section">' +
                                '<span class="countdown-amount">0</span>' +
                                '<span class="countdown-period">Hodin</span>' + 
                            '</span>' +
                            '<span class="countdown-section">' +
                                '<span class="countdown-amount">29</span>' +
                                '<span class="countdown-period">Minut</span>' +
                            '</span>' + 
                            '<span class="countdown-section">' + 
                                '<span class="countdown-amount">59</span>' +
                                '<span class="countdown-period">Sekund</span>' +
                            '</span>' + 
                        '</span>' +
                    '</div>',
        'wizzard': '<div class="wizard-phases-frame"></div>'
    },

    templatesName: {
        'horizontal-form-row': 'Horizontal form row',
        'nv-list': 'Name - Value list',
        'data-table': 'Data table',
        'countdown': 'Countdown',
        'wizzard': 'Wizzard phases'
    },

    options: {
        'data-table': {
            'dataTableOptions': {
                name: 'Data table options',
                type: 'group',
                groupItems: [{
                    label: 'Options',
                    type: 'boolean',
                    options: {
                        'data-dtpaging': 'Show pagination',
                        'data-dtinfo': 'Show informations',
                        'data-dtfilter': 'Show filter',
                        'data-dtcolumnfilter': 'Show column filter',
                        'data-dtordering': 'Enable ordering'
                    },
                    get: function (value) {
                        return $(this).attr(value) == '1';
                    },
                    set: function (opt) {
                        $(this).attr(opt.value, opt.checked ? '1' : '0');
                        MBE.types.ui.updateDataTable.apply(this, []);
                    }
                }, {
                    label: 'Order',
                    type: 'text',
                    attr: 'data-dtorder',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Actions',
                    type: 'builder',
                    builder: function (opt, isCollapsed)
                    {
                        if (!MBE.options.isAllowed(opt)) {
                            return '';
                        }

                        var group = $('<div class="option-group form-group"' + (opt.id ? ' id="' + opt.id + '"' : '') + '></div>');
                        var set = opt.set;

                        var lb = $('<label class="control-label col-xs-2">' + opt.label + '</label>');
                        var list = $('<table class="table table-stripped table-condensed dt-action-list"></table>');

                        var head = $('<thead />');
                        var body = $('<tbody />');

                        var row = $('<tr class="info" />');
                        row.html('<th>Icon</th><th>Action ID</th><th>Action title</th><th class="text-center"><span data-action="dt-add-action" class="fa fa-plus-circle" style="cursor: pointer"></span></th>').appendTo(head);

                        head.appendTo(list);
                        body.appendTo(list);

                        group.append(lb);
                        group.append(list);

                        list.wrap('<div class="col-xs-10"></div>');

                        if (isCollapsed) {
                            group.css('display', 'none');
                        }

                        body.sortable({
                            cursor: 'move',
                            forceHelperSize: true,
                            forcePlaceholderSize: true,
                            handle: '.handle',
                            helper: function (e, ui) {
                                ui.children().each(function () {
                                    $(this).width($(this).width());
                                });
                                return ui;
                            },
                            update: function () {
                                MBE.types.ui.dataTableBuildActions.apply(MBE.options.target, [false]);
                            }
                        });

                        var actions = $(MBE.options.target).attr('data-actions');
                        if (actions && actions.length)
                        {
                            actionList = JSON.parse(actions.replace(/'/g, '"'));
                            for (var i = 0; i < actionList.length; i++) {
                                MBE.types.ui.dataTableAddAction.apply(body[0], [null, actionList[i]]);
                            }
                        }

                        return group;
                    },
                    get: function() {},
                    set: function() {}
                }]
            }
        }
    }, 
    
    init: function()
    {
        var self = MBE.types.ui;

        var group = $('<li></li>');
        group.html('<span class="fa fa-caret-right fa-fw"></span>UI').prependTo('ul.category');
    
        var items = $('<ul data-type="ui" style="display: none"></ul>');
        for(template in self.templatesName) {
            var item = $('<li></li>');
            item
                .html('<span class="fa fa-square fa-fw"></span>' + self.templatesName[template])
                .attr({ 'data-template': template, 'draggable': true })
                .data('type', 'ui')
                .appendTo(items);
        }
        items.appendTo(group);

        MBE.DnD.onDrop.push(MBE.types.ui._drop);
        MBE.onBeforeDelete['ui|data-table'] = MBE.types.ui._beforeDelete;

        $(document)
            .on('click', 'span[data-action="dt-add-action"]', self.dataTableAddAction)
            .on('click', 'span[data-action="dt-remove"]', self.dataTableRemoveAction)
            .on('click', '[data-action="dt-select-icon"]', self.dataTableOpenIconDialog)
            .on('change', 'input[name=action_id], input[name=action_title]', self.dataTableBuildActions)
        ;
    },

    _drop: function(target)
    {
        if ($(this).is('[data-uic="ui|horizontal-form-row"]')) {
            MBE.types.ui.buildHorizontalFormRow.apply(this, []);
        }
        if ($(this).is('[data-uic="ui|nv-list"]:empty')) {
            MBE.types.ui.buildNVList.apply(this, []);
        }
        if ($(this).is('[data-uic="ui|data-table"]')) {
            if ($(this).is(':empty'))
                MBE.types.ui.buildDataTable.apply(this, []);
            else
                MBE.types.ui.updateDataTable.apply(this, []);
        }
        if ($(this).is('[data-uic="ui|wizzard"]:empty')) {
            MBE.types.ui.buildWizzard.apply(this, []);
        }
    },

    _beforeDelete: function()
    {
        if ($(this).is('[data-uic="ui|data-table"]')) {
            $(this).parents('.dataTables_wrapper').eq(0).replaceWith(this);
        }
    },

    buildHorizontalFormRow: function()
    {
        var g = MBE.types.grid;
        var f = MBE.types.form;

        var row = $(f.templates['form-group']);
        var label = $(f.templates['label']);
        var input = $(f.templates['input-text']);
        var column = $(g.templates['column']);
        
        row.attr('data-uic', 'form|form-group');
        label.attr('data-uic', 'form|label').addClass('col-sm-2').appendTo(row);
        column.attr('data-uic', 'grid|column').addClass('col-sm-10').appendTo(row);
        input.attr('data-uic', 'form|input-text').appendTo(column);
        
        $(this).replaceWith(row);
    },

    buildNVList: function()
    {
        var body = $('<tbody />');
        
        var names = ["Platform", "Country", "Year"];
        var values = ["Omnius", "Czech Republic", "2006"];

        for (var i = 0; i < names.length; i++) {
            var row = $('<tr />');

            var nameCell = $('<td />');
            var valueCell = $('<td />');
            
            nameCell.addClass('name-cell').html(names[i]).appendTo(row);
            valueCell.addClass('value-cell').html(values[i]).appendTo(row);

            row.appendTo(body);
        }

        body.appendTo(this);
    },

    /********************************************************************/
    /* DATA TABLE CONTEXT METHODS                                       */
    /********************************************************************/
    buildDataTable: function () {
        var body = $('<tbody />');
        var head = $('<thead />');

        var names = ["Column 1", "Column 2", "Column 3"];

        var row = $('<tr />');
        
        for (var i = 0; i < names.length; i++) {
            var cell = $('<th />');
            cell.html(names[i]).appendTo(row);
        }
        row.appendTo(head);

        for (var i = 1; i <= 3; i++) {
            var row = $('<tr />');

            for(var j = 1; j <= names.length; j++)
            {
                var n = i * j;
                var cell = $('<td />');
                cell.html("Value " + n).appendTo(row);
            }
            row.appendTo(body);
        }

        head.appendTo(this);
        body.appendTo(this);

        if (!$(this).is('[data-dtpaging]')) {
            $(this).attr({
                'data-dtpaging': '1',
                'data-dtinfo': '1',
                'data-dtfilter': '1',
                'data-dtordering': '1',
                'data-dtcolumnfilter': '0'
            });
        }
        
        MBE.types.ui.initDataTable.apply(this, []);

        if ($(this).is('[data-actions]')) {
            MBE.types.ui.dataTableBuildActions.apply(this, [true]);
        }
    },

    updateDataTable: function() {
        MBE.types.ui.initDataTable.apply(this, []);
    },

    initDataTable: function() {
        var settings = {
            'destroy': true,
            'paging': $(this).attr('data-dtpaging') == '1',
            'info': $(this).attr('data-dtinfo') == '1',
            'filter': $(this).attr('data-dtfilter') == '1',
            'ordering': $(this).attr('data-dtordering') == '1'
        };

        $(this).DataTable(settings);
        $('> tfoot', this).remove();
        
        if ($(this).attr('data-dtcolumnfilter') == '1')
        {
            if (!$('> tfoot', this).length) {
                var foot = $('<tfoot />');
                var row = $('<tr />');

                $('> thead > tr:first-child > th', this).each(function () {
                    var cell = $('<th />');

                    if (!$(this).hasClass('actionHeader')) {
                        var input = $('<input type="text" value="" class="form-control input-sm" placeholder="Hledat v &quot;' + $(this).text() + '&quot;" />');
                        input.appendTo(cell);
                    }
                    else {
                        cell.html('&nbsp;');
                    }
                    
                    cell.appendTo(row);
                });

                row.appendTo(foot)
                $('> thead', this).after(foot);
            }
        }
    },

    dataTableAddAction: function(event, action) {
        var list = $(this).parents('table').eq(0);
        var body = $('tbody', list);

        var iconClass = action ? action.icon : 'fa fa-pencil';
        var actionId = action ? action.action : '';
        var actionTitle = action ? action.title : '';

        var row = $('<tr />');
        var dd = $('<button type="button" class="btn btn-default" style="padding: 3px 10px" data-action="dt-select-icon"><span class="' + iconClass + '"></span> <span class="caret"></span></button>');
        var id = $('<input type="text" class="form-control input-sm" value="' + actionId + '" name="action_id" />');
        var title = $('<input type="text" class="form-control input-sm" value="' + actionTitle + '" name="action_title" />');
        var move = $('<span class="fa fa-arrows-v fa-fw handle" style="margin-right: 5px; margin-top: 9px; cursor: pointer"></span>');
        var del = $('<span class="fa fa-times fa-fw" data-action="dt-remove" style="margin-top: 9px; cursor: pointer"></span>');

        var cell1 = $('<td />');
        cell1.append(dd).appendTo(row);

        var cell2 = $('<td />');
        cell2.append(id).appendTo(row);

        var cell3 = $('<td />');
        cell3.append(title).appendTo(row);

        var cell4 = $('<td />');
        cell4.append(move).append(del).appendTo(row);

        row.appendTo(body);

        body.sortable('refresh');
    },

    dataTableRemoveAction: function() {
        $(this).parents('tr').eq(0).remove();
        MBE.types.ui.dataTableBuildActions.apply(MBE.options.target, [false]);
    },

    dataTableOpenIconDialog: function() {
        var target      = $(this);
        var active      = '';
        var iconList    = {};
        var sSheetList  = document.styleSheets;
        var fontSets    = { 'fa': 'Font Awesome', 'glyphicon': 'Glyphicons' };
        var d           = $('<div />');
        var f           = $('<fieldset />');
        var group       = $('<div class="option-group form-group"></div>');

        f.append('<legend>Icon</legend>');

        for (var fs in fontSets) {
            var list = $('<div class="icon-list col-xs-12 font-set-' + fs + '"></div>');

            for (var sSheet = 0; sSheet < sSheetList.length; sSheet++) {
                var ruleList = document.styleSheets[sSheet].cssRules;
                for (var rule = 0; rule < ruleList.length; rule++) {
                    var text = ruleList[rule].selectorText;
                    var selectors = text ? text.split(/, ?/) : [];
                    for (var si = 0; si < selectors.length; si++)
                    {
                        var selector = selectors[si];
                        var rx = new RegExp('^\.(' + fs + '-[^ ]+)::before$');
                        if (m = rx.exec(selector)) {
                            var btn = $('<a href="#"></a>');
                            btn.append('<span class="' + fs + ' ' + m[1] + '"></span>');
                            btn.append('<span class="icon-name">' + fs + ' ' + m[1] + '</span>');
                            list.append(btn);

                            var test = '.' + fs + '.' + m[1];
                            if (target.find('span').eq(0).is(test)) {
                                btn.addClass('active');
                                active = fs;
                            }
                        }
                    }
                    
                }
            }
            list.on('click', 'a', function () {
                target.find('span').eq(0).removeClass().addClass($(this).find('span').eq(0)[0].className);
                $('.icon-list a').removeClass('active');
                $(this).addClass('active');
                MBE.types.ui.dataTableBuildActions();
                return false;
            });
            iconList[fs] = list;
        }

        var search = $('<input type="text" value="" placeholder="Find icon..." class="form-control input-sm">');

        search.on('input', function () {
            var text = this.value;
            if (text.length) {
                $('.icon-list').find('a').hide().each(function () {
                    if ($('.icon-name', this).text().indexOf(text) != -1) {
                        $(this).show();
                    }
                });
            }
            else {
                $('.icon-list').find('a').show();
            }
        });

        var sets = $('<select class="form-control input-sm"></select>');

        for (fs in fontSets) {
            var option = $('<option></option>');
            option.val(fs).html(fontSets[fs]).appendTo(sets);

            if (active == fs) {
                option.attr('selected', true);
                iconList[fs].show();
            }
            else {
                iconList[fs].hide();
            }
        }

        sets.change(function () {
            $('[class*="font-set-"]').hide();
            $('.font-set-' + this.value).show();
        });

        group.append(sets);
        group.append(search);
        group.append('<div class="clearfix"></div>');
        for (var fs in iconList) {
            group.append(iconList[fs]);
        }

        sets.wrap('<div class="col-xs-4"></div>');
        search.wrap('<div class="col-xs-6"></div>');

        f.append(group).appendTo(d);

        d.dialog({
            appendTo: 'body',
            modal: true,
            closeOnEscape: true,
            draggable: false,
            resizable: false,
            width: '40%',
            maxHeight: '60%',
            title: 'Select icon...',
            dialogClass: 'dialog-options',
            close: function () { $(this).remove(); }
        });
    },

    dataTableBuildActions: function(rebuild) {
        var target = $(this);

        var validActions = [];

        if (rebuild !== true) {
            $('tbody > tr', '.dt-action-list').each(function () {
                var icon = $('td', this).eq(0).find('button > span').eq(0)[0].className;
                var id = $('td', this).eq(1).find('input').val();
                var title = $('td', this).eq(2).find('input').val();

                if (icon && id && title) {
                    validActions.push({ 'icon': icon, 'action': id, 'title': title });
                }
            });
        }
        else {
            var json = $(this).attr('data-actions').replace(/'/g, '"');
            if (json && json.length) {
                validActions = JSON.parse(json);
            }
        }

        if (validActions.length) {
            if (!$('tbody > tr > td.actionIcons', target).length) {
                $('thead > tr', target).append('<th class="actionHeader">Akce</th>');
                $('tbody > tr', target).append('<td class="actionIcons"></td>');
                $('tfoot > tr', target).append('<th class="actionFooter">&nbsp;</th>');
            }
            $('tbody > tr > td.actionIcons', target).html('');
            for (var i = 0; i < validActions.length; i++) {
                $('tbody > tr > td.actionIcons', target).each(function () {
                    $(this).append('<i class="' + validActions[i].icon + '" data-action="' + validActions[i].action + '" title="' + validActions[i].title + '"></i>');
                });
            }
        }
        else {
            if ($('tbody tr td.actionIcons', target).length) {
                var index = $('tbody tr td.actionIcons', target).eq(0)[0].cellIndex;
                $('> tbody > tr, > thead > tr, > tfoot > tr', target).each(function() {
                    $('> td, > th', this).eq(index).remove();
                });
            }
        }

        target.attr('data-actions', JSON.stringify(validActions).replace(/"/g, "'"));
        
    },

    /********************************************************************/
    /* WIZZARD CONTEXT METHODS                                          */
    /********************************************************************/
    buildWizzard: function() {
        var self = MBE.types.ui;
        var target = $(this);

        var svg = '' + 
'<svg class="phase-background" width="846px" height="84px"><defs>' +
'<linearGradient id="grad-light" x1="0%" y1="0%" x2="0%" y2="100%"><stop offset="0%" style="stop-color:#dceffa ;stop-opacity:1" />' +
'<stop offset="100%" style="stop-color:#8dceed;stop-opacity:1" /></linearGradient><linearGradient id="grad-blue" x1="0%" y1="0%" x2="0%" y2="100%">' +
'<stop offset="0%" style="stop-color:#0099cc;stop-opacity:1" /><stop offset="100%" style="stop-color:#0066aa;stop-opacity:1" />' +
'</linearGradient></defs><path d="M0 0 L0 88 L 280 88 L324 44 L280 0 Z" fill="url(#grad-blue)" /><path d="M280 88 L324 44 L280 0 L560 0 L604 44 L560 88 Z" fill="url(#grad-light)" />' +
'<path d="M560 0 L604 44 L560 88 L850 88 L850 0 Z" fill="url(#grad-light)" /></svg>';
        
        var phase = '' + 
'<div class="phase phase1 phase-active">' +
    '<div class="phase-icon-circle">' +
        '<div class="phase-icon-number">1</div>' +
    '</div>' +
    '<div class="phase-label">Fáze 1</div>' + 
'</div>';
    }
};

MBE.onInit.push(MBE.types.ui.init);
