MBE.types.containers = {

    templates: {
        'container': '<div class="container"></div>',
        'panel': '<div class="panel panel-default">' +
                    '<div class="panel-heading" data-uic="containers|panel-heading" locked>' +
                        '<h3 class="panel-title" data-uic="text|heading">Panel title</h3>' +
                    '</div>' +
                    '<div class="panel-body" data-uic="containers|panel-body" locked>' +
                        '<p data-uic="text|paragraph">Panel body</p>' +
                    '</div>' +
                    '<div class="panel-footer" data-uic="containers|panel-footer" locked>' + 
                        '<span data-uic="text|span">Panel footer</span>' +
                    '</div>' +
                '</div>',
        'panel-heading': '<div class="panel-heading" data-uic="containers|panel-heading" locked><span data-uic="text|span">Panel heading</span></div>',
        'panel-body': '<div class="panel-body" data-uic="containers|panel-body" locked><p data-uic="text|paragraph">Panel body</p></div>',
        'panel-footer': '<div class="panel-footer" data-uic="containers|panel-footer" locked><span data-uic="text|span">Panel footer</span></div>',
        'tabs': '<div></div>',
        'tab': '<li data-uic="containers|tab"><a href="" data-uic="controls|link" data-toggle="tab" locked></a></li>',
        'tab-items': '<ul class="nav nav-tabs" data-uic="containers|tab-items" locked></ul>',
        'tab-content': '<div class="tab-content" data-uic="containers|tab-content" locked></div>',
        'tab-pane': '<div class="tab-pane" data-uic="containers|tab-pane"><p data-uic="text|paragraph">Tab content</p></div>',
        'accordion': '<div class="panel-group"></div>',
        'well': '<div class="well"><span data-uic="text|span">Text of the well</span></div>',
        'list': '<ul><li data-uic="containers|list-item">Item 1</li><li data-uic="containers|list-item">Item 2</li></ul>',
        'list-item': '<li data-uic="containers|list-item">Item</li>',
        'div': '<div></div>',
        'list-group': '<ul class="list-group"></ul>',
        'list-group-div': '<div class="list-group"></div>',
        'list-group-item':  '<li class="list-group-item" data-uic="containers|list-group-item">List Group Item</li>',
        'list-group-item-link': '<a class="list-group-item" data-uic="containers|list-group-item-link">List Group Item</a>',
        'list-group-item-button': '<button type="button" class="list-group-item" data-uic="containers|list-group-item-button">List Group Item</button>'
    },
    
    options: {
        'container': {
            'containerOptions': {
                name: 'Container options',
                type: 'boolean',
                options: {
                    'container-fluid': 'Fluid'
                },
                set: function (opt) {
                    $(this).toggleClass('container container-fluid');
                },
                get: MBE.options.hasClass
            }
        },
        'panel': {
            'panelOptions': {
                name: 'Panel options',
                type: 'group',
                groupItems: [{
                    label: 'Style',
                    type: 'select',
                    options: {
                        'panel-default': 'Default',
                        'panel-primary': 'Primary',
                        'panel-success': 'Success',
                        'panel-info': 'Info',
                        'panel-warning': 'Warning',
                        'panel-danger': 'Danger',
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Show',
                    type: 'boolean',
                    options: {
                        'panel-heading': 'Show panel heading',
                        'panel-body': 'Show panel body',
                        'panel-footer': 'Show panel footer'
                    },
                    get: function (value) {
                        return $('> .' + value, this).length > 0;
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            switch (opt.value) {
                                case 'panel-heading': $(this).prepend(MBE.types.containers.templates['panel-heading']); break;
                                case 'panel-footer': $(this).append(MBE.types.containers.templates['panel-footer']); break;
                                case 'panel-body':
                                    if ($('> .panel-heading', this).length) {
                                        $('> .panel-heading', this).after(MBE.types.containers.templates['panel-body']);
                                    }
                                    else {
                                        $(this).prepend(MBE.types.containers.templates['panel-body']);
                                    }
                                    break;
                            }
                        }
                        else {
                            $('> .' + opt.value, this).remove();
                        }
                        MBE.DnD.updateDOM();
                    }
                }]
            }
        },
        'tab-items': {
            'tabItemsOptions': {
                name: 'Tab items options',
                type: 'group',
                groupItems: [{
                    label: 'Justified',
                    type: 'boolean',
                    options: {
                        'nav-justified': 'Justified'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'tab-content': {
            'tabContentOptions': {
                name: 'Tab content options',
                type: 'group',
                groupItems: [{
                    label: 'Fade',
                    type: 'boolean',
                    options: {
                        'fade': 'Fade'
                    },
                    get: function () {
                        return $('> .fade', MBE.options.target).length;
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            $('> .tab-pane', this).addClass('fade').filter('.active').addClass('in');
                        }
                        else {
                            $('> .tab-pane', this).removeClass('fade in');
                        }
                    }
                }],
            }
        },
        'well': {
            'wellOptions': {
                name: 'Well options',
                type: 'group',
                groupItems: [{
                    label: 'Size',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'well-lg': 'Large',
                        'well-sm': 'Small'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'list': {
            'listOptions': {
                name: 'List options',
                type: 'group',
                groupItems: [{
                    id: 'ListStyle',
                    label: 'Style',
                    type: 'select',
                    options: {
                        'ul': 'Unordered (UL)',
                        'ol': 'Ordered (OL)',
                        'ul.list-unstyled': 'Unstyled (UL)',
                        'ul.list-inline': 'Inline (UL)'
                    },
                    get: MBE.options.is,
                    set: function (opt) {
                        var tc = opt.value.split('.');
                        var tagName = tc[0];
                        var className = tc.length > 1 ? tc[1] : '';

                        $(this).removeClass().addClass(className);
                        if (!$(this).is(tagName)) {
                            MBE.options.toggleTagName.apply(this, [{ value: tagName }]);
                        }

                        if (opt.value == 'ol') {
                            $('#ListNumberingType, #ListNumberingStart, #ListNumberingReversed').show();
                        }
                        else {
                            $('#ListNumberingType, #ListNumberingStart, #ListNumberingReversed').hide();
                            $(this).removeAttr('type start reversed');
                        }
                    }
                }, {
                    id: 'ListNumberingType',
                    label: 'Numbering type',
                    type: 'select',
                    options: {
                        '1': 'Number (1, 2, 3)',
                        'a': 'Letters (a, b, c)',
                        'A': 'Letters (A, B, C)',
                        'i': 'Roman (i, ii, iii)',
                        'I': 'Roman (I, II, III)'
                    },
                    attr: 'type',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    id: 'ListNumberingStart',
                    label: 'Start',
                    type: 'number',
                    attr: 'start',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    id: 'ListNumberingReversed',
                    label: 'Reversed',
                    type: 'boolean',
                    attr: 'reversed',
                    options: {
                        'reversed': 'Reversed'
                    },
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }],
                onBuild: function () {
                    var opt = $('#ListStyle select');
                    $('#ListNumberingType, #ListNumberingStart, #ListNumberingReversed').toggle(opt.val() == 'ol');
                }
            }
        },
        'list-group': {
            'listGroupOptions': {
                name: 'List group options',
                type: 'group',
                groupItems: [{
                    label: 'Type',
                    type: 'select',
                    options: {
                        'ul': 'Unordered list',
                        'div-links': 'DIV with links',
                        'div-buttons': 'DIV with buttons'
                    },
                    get: function (value) {
                        switch (value) {
                            case 'ul': return $(this).is('ul');
                            case 'div-links': return $(this).is('div') && $(this).find('> a').length > 0;
                            case 'div-buttons': return $(this).is('div') && $(this).find('> button').length > 0;
                        }
                    },
                    set: function (opt) 
                    {
                        var self = MBE.types.containers;
                        var target = $(this);
                        var groupTemplate, itemTemplate;

                        switch(opt.value)
                        {
                            case 'ul': {
                                groupTemplate = 'list-group';
                                itemTemplate = 'list-group-item';
                                break; 
                            }
                            case 'div-links': {
                                groupTemplate = 'list-group-div';
                                itemTemplate = 'list-group-item-link';
                                break;
                            }
                            case 'div-buttons': {
                                groupTemplate = 'list-group-div';
                                itemTemplate = 'list-group-item-button';
                                break;
                            }
                        }

                        var group = $(self.templates[groupTemplate]);
                        target.find('> a, > button, > li').each(function () {
                            var item = $(self.templates[itemTemplate]);
                            item.html(this.innerHTML).addClass(this.className).appendTo(group);
                        });
                        group.attr('data-uic', 'containers|' + groupTemplate);

                        target.replaceWith(group);
                        MBE.DnD.updateDOM();
                        MBE.selection.select.apply(group[0], []);
                        MBE.options.target = group[0];
                    }
                }]
            }
        },
        'list-group-item': {
            'listGroupItemOptions': {
                name: 'List group item options',
                type: 'group',
                groupItems: [{
                    label: 'Style',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'list-group-item-success': 'Success',
                        'list-group-item-info': 'Info',
                        'list-group-item-warning': 'Warning',
                        'list-group-item-danger': 'Danger'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'URL',
                    type: 'text',
                    attr: 'href',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr,
                    allowFor: ['list-group-item-link'],
                }, {
                    label: 'Target',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        '_blank': 'Blank',
                        '_parent': 'Parent',
                        '_top': 'Top'
                    },
                    attr: 'target',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr,
                    allowFor: ['list-group-item-link']
                }, {
                    label: 'Options',
                    type: 'boolean',
                    options: {
                        'active': 'Active',
                        'disabled': 'Disabled'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        }
    },

    init: function()
    {
        var self = MBE.types.containers;
        var menu = MBE.toolbar.menu;

        MBE.DnD.onDrop.push(MBE.types.containers._drop);
        MBE.types.controls.options.link.linkOptions.groupItems[0].change = self.tabIdChange;

        self.options.panel.panelOptions.groupItems[1].disallowFor = self.panelIsNotInAccordion;
        self.options['list-group-div'] = self.options['list-group'];
        self.options['list-group-item-link'] = self.options['list-group-item'];
        self.options['list-group-item-button'] = self.options['list-group-item'];

        menu['containers'] = {};

        menu['containers']['list'] = {
            items: [
                { type: 'text', label: 'ADD TO LIST' },
                { type: 'button', label: 'BEGIN', callback: self.listAddToBegin },
                { type: 'button', label: 'END', callback: self.listAddToEnd },
            ]
        };
        menu['containers']['list-item'] = {
            items: [
                { type: 'text', label: 'ADD TO LIST' },
                { type: 'button', label: 'BEFORE', callback: self.listItemAddBefore },
                { type: 'button', label: 'AFTER', callback: self.listItemAddAfter },
                { type: 'button', label: 'BEGIN', callback: self.listAddToBegin },
                { type: 'button', label: 'END', callback: self.listAddToEnd },
                { type: 'text', label: 'ITEM' },
                { type: 'button', label: 'DELETE', callback: self.listItemDelete }
            ]
        };
        menu['containers']['tab-items'] = {
            items: [
                { type: 'text', label: 'ADD TAB' },
                { type: 'button', label: 'BEGIN', callback: self.tabAddToBegin },
                { type: 'button', label: 'END', callback: self.tabAddToEnd },
            ]
        };
        menu['containers']['tab'] = {
            items: [
                { type: 'text', label: 'TAB' },
                { type: 'button', label: 'SHOW', callback: self.tabShow },
                { type: 'text', label: 'ADD TAB' },
                { type: 'button', label: 'BEGIN', callback: self.tabAddToBegin },
                { type: 'button', label: 'END', callback: self.tabAddToEnd },
                { type: 'button', label: 'LEFT', callback: self.tabAddToLeft },
                { type: 'button', label: 'RIGHT', callback: self.tabAddToRight },
            ]
        };
        menu['containers']['accordion'] = {
            items: [
                { type: 'text', label: 'ADD ITEM' },
                { type: 'button', label: 'BEGIN', callback: self.accordionAddToBegin },
                { type: 'button', label: 'END', callback: self.accordionAddToEnd }
            ]
        };
        menu['containers']['panel'] = {
            allowFor: self.panelIsInAccordion,
            items: [
                { type: 'text', label: 'ADD ITEM' },
                { type: 'button', label: 'BEGIN', callback: self.accordionAddToBegin },
                { type: 'button', label: 'END', callback: self.accordionAddToEnd },
                { type: 'button', label: 'BEFORE', callback: self.accordionAddBefore },
                { type: 'button', label: 'AFTER', callback: self.accordionAddAfter },
            ]
        };
        menu['containers']['list-group'] = {
            items: [
                { type: 'text', label: 'ADD ITEM' },
                { type: 'button', label: 'BEFORE', callback: self.listGroupAddBefore, allowFor: self.listGroupIsItemSelected },
                { type: 'button', label: 'AFTER', callback: self.listGroupAddAfter, allowFor: self.listGroupIsItemSelected },
                { type: 'button', label: 'BEGIN', callback: self.listGroupAddToBegin },
                { type: 'button', label: 'END', callback: self.listGroupAddToEnd },
                { type: 'text', label: 'ITEM' },
                { type: 'button', label: 'ACTIVATE', callback: self.listGroupActivateItem, allowFor: self.listGroupIsInactiveItemSelected },
                { type: 'button', label: 'DEACTIVATE', callback: self.listGroupDeactivateItem, allowFor: self.listGroupIsActiveItemSelected },
                { type: 'button', label: 'DELETE', callback: self.listGroupDeleteItem, allowFor: self.listGroupIsItemSelected },
            ]
        }
        menu['containers']['list-group-div'] = menu['containers']['list-group'];
        menu['containers']['list-group-item'] = menu['containers']['list-group'];
        menu['containers']['list-group-item-link'] = menu['containers']['list-group'];
        menu['containers']['list-group-item-button'] = menu['containers']['list-group'];

        MBE.onBeforeDelete['containers|tab'] = self._tabDelete;
        
    },

    _drop: function(target)
    {
        if (target.is('.panel-heading') && $(this).is('[data-uic="text|heading"]')) {
            $(this).addClass('panel-title');
        }

        if ($(this).is('[data-uic="containers|tabs"]') && $(this).is(':empty')) {
            MBE.types.containers.buildTabs.apply(this, []);
        }

        if ($(this).is('[data-uic="containers|accordion"]') && $(this).is(':empty')) {
            MBE.types.containers.buildAccordion.apply(this, []);
        }

        if ($(this).is('.list-group') && $(this).is(':empty')) {
            MBE.types.containers.buildListGroup.apply(this, []);
        }
    },

    /******************************************************************/
    /* LIST GROUP CONTEXT METHODS                                     */
    /******************************************************************/
    buildListGroup: function() {
        for (var i = 1; i <= 3; i++) {
            var item = $(MBE.types.containers.templates['list-group-item']);
            item.html('List Group Item ' + i).appendTo(this);
        }
    },

    listGroupIsItemSelected: function () {
        console.log(this);
        return $(this).is('.list-group-item');
    },
    
    listGroupIsActiveItemSelected: function() {
        return $(this).is('.list-group-item.active');
    },

    listGroupIsInactiveItemSelected: function() {
        return $(this).is('.list-group-item:not(.active)');
    },

    listGroupActivateItem: function () {
        $(this).addClass('active');
        MBE.selection.select.apply(this, []);
    },

    listGroupDeactivateItem: function() {
        $(this).removeClass('active');
        MBE.selection.select.apply(this, []);
    },

    listGroupDeleteItem: function() {
        $(this).remove();
        $('.mbe-drag-handle', MBE.workspaceDoc).remove();
        MBE.DnD.updateDOM();
        MBE.selection.select.apply(MBE.workspaceDoc.body, []);
    },

    listGroupAdd: function(pos) 
    {
        var self = MBE.types.containers;
        var item = $(self.templates['list-group-item']);
        var target;
        if(pos == 'before' || pos == 'after') {
            target = $(this);
        }
        else {
            if($(this).is('.list-group')) {
                target = $(this);
            }
            else {
                target = $(this).parent();
            }
        }

        switch (pos) {
            case 'before': target.before(item); break;
            case 'after': target.after(item); break;
            case 'begin': target.prepend(item); break;
            case 'end': target.append(item); break;
        }

        MBE.DnD.updateDOM();
    },

    listGroupAddAfter: function() { MBE.types.containers.listGroupAdd.apply(this, ['after']); },
    listGroupAddBefore: function () { MBE.types.containers.listGroupAdd.apply(this, ['before']); },
    listGroupAddToBegin: function () { MBE.types.containers.listGroupAdd.apply(this, ['begin']); },
    listGroupAddToEnd: function() { MBE.types.containers.listGroupAdd.apply(this, ['end']); },

    /******************************************************************/
    /* LIST CONTEXT METHODS                                           */
    /******************************************************************/
    listItemAddBefore: function () {
        $(this).before($(MBE.types.containers.templates['list-item']));
        MBE.DnD.updateDOM();
    },

    listItemAddAfter: function () {
        $(this).after($(MBE.types.containers.templates['list-item']));
        MBE.DnD.updateDOM();
    },

    listItemDelete: function() {
        $(this).remove();
        MBE.DnD.updateDOM();
    },

    listAddToBegin: function () {
        var target = $(this).is('li') ? $(this).parent() : $(this);
        target.prepend($(MBE.types.containers.templates['list-item']));
        MBE.DnD.updateDOM();
    },

    listAddToEnd: function () {
        var target = $(this).is('li') ? $(this).parent() : $(this);
        target.append($(MBE.types.containers.templates['list-item']));
        MBE.DnD.updateDOM();
    },

    /******************************************************************/
    /* TABS CONTEXT METHOD                                            */
    /******************************************************************/
    tabShow: function () {
        var tabs = $(this).siblings('li');
        tabs.each(function() {
            var link = $('a', this);
            $(link.attr('href')).removeClass('active');
            $(this).removeClass('active');
        })
        $(this).addClass('active');
        $($('a', this).attr('href')).addClass('active');
    },

    tabIdChange: function() {
        var elm = $(this);
        if (elm.parent().is('[data-uic="containers|tab"]')) {
            var prevId = elm.data('prevId');
            var newId = elm.attr('href').replace('#', '');
            $('#' + prevId).attr('id', newId);
            elm.data('prevId', newId);
            MBE.DnD.updateDOM();
        }
    },

    tabAdd: function(pos) {
        var self = MBE.types.containers;
        var tabIndex = $('[data-toggle="tab"]', MBE.workspace).length + 1;
        var tab = $(this).is('li') ? $(this) : null;
        var tabs = $(this).is('.nav-tabs') ? $(this) : $(this).parent();
        var content = tabs.next();

        var tabItem = $(self.templates['tab']);
        var tabPane = $(self.templates['tab-pane']);
        var tabId = 'tab-' + tabIndex;

        tabPane.attr('id', tabId);
        tabItem.find('a').attr('href', '#' + tabId).data('prevId', tabId).html('Tab ' + tabIndex);

        switch (pos) {
            case 'begin':
                tabs.prepend(tabItem);
                content.prepend(tabPane);
                break;
            case 'end':
                tabs.append(tabItem);
                content.append(tabPane);
                break;
            case 'left':
                tab.before(tabItem);
                $(tab.find('a').attr('href')).before(tabPane);
                break;
            case 'right':
                tab.after(tabItem);
                $(tab.find('a').attr('href')).after(tabPane);
                break;
        }
        MBE.DnD.updateDOM();
    },

    tabAddToLeft: function() {
        MBE.types.containers.tabAdd.apply(this, ['left']);
    },

    tabAddToRight: function () {
        MBE.types.containers.tabAdd.apply(this, ['right']);
    },

    tabAddToBegin: function () {
        MBE.types.containers.tabAdd.apply(this, ['begin']);
    },

    tabAddToEnd: function () {
        MBE.types.containers.tabAdd.apply(this, ['end']);
    },

    buildTabs: function (tabLabelArray)
    {
        var self = MBE.types.containers;
        var target = $(this);
        var items = $(self.templates['tab-items']);
        var content = $(self.templates['tab-content']);
        var tabIndex = $('[data-toggle="tab"]', MBE.workspace).length + 1;
        var prefix = typeof tabLabelArray == 'array' ? tabLabelArray : ['First tab', 'Second tab', 'Third tab'];

        target.append(items);
        target.append(content);
        for (var i = 0; i < prefix.length; i++) {
            var tab = $(self.templates['tab']);
            var pane = $(self.templates['tab-pane']);

            tab.find('a').attr('href', '#tab-' + tabIndex).html(prefix[i]).data('prevId', 'tab-' + tabIndex);
            pane.find('p').html(prefix[i] + ' content');
            pane.attr('id', 'tab-' + tabIndex);

            if (i == 0) {
                tab.addClass('active');
                pane.addClass('active');
            }

            items.append(tab);
            content.append(pane);
            tabIndex++;
        }
    },

    _tabDelete: function() {
        var tabId = $('a', this).attr('href');
        $(tabId).remove();
    },

    /******************************************************************/
    /* ACCORDION CONTEXT METHODS                                      */
    /******************************************************************/
    buildAccordion: function()
    {
        var self = MBE.types.containers;
        var target = $(this);
        var accordionIndex = MBE.workspace.find('.panel-group').length;
        var id = 'accordion-' + accordionIndex;

        target.attr('id', id);
        for(var i = 1; i <= 3; i++)
        {
            var item = $(self.templates['panel']);
            item.attr('data-uic', 'containers|panel');
            item.find('.panel-title').attr('locked', 'true').html('').append($(MBE.types.controls.templates.link));
            item.find('.panel-title a').html('Accordion item').attr({
                'locked': true,
                'data-uic': 'controls|link',
                'data-toggle': 'collapse',
                'data-parent': '#'+id,
                'href': '#' + id + ' .item-' + i
            });

            var wrapper = $('<div class="panel-collapse collapse item-'+ i + (i == 1 ? ' in' : '') + '"></div>');

            item.find('.panel-body p').html('Item body.').parent().wrap(wrapper);
            item.find('.panel-footer').remove();

            target.append(item);
        }
    },

    accordionAddItem: function (pos)
    {
        var self = MBE.types.containers;
        var target = $(this).is('.panel-group') ? $(this) : $(this).parent();
        var panel = $(this).is('.panel-group') ? null : $(this);
        var id = target.attr('id');
        var panelIndex = target.find('.panel-collapse').length + 1;

        var item = $(self.templates['panel']);
        item.attr('data-uic', 'containers|panel');
        item.find('.panel-title').attr('locked', 'true').html('').append($(MBE.types.controls.templates.link));
        item.find('.panel-title a').html('Accordion item').attr({
            'locked': true,
            'data-uic': 'controls|link',
            'data-toggle': 'collapse',
            'data-parent': '#' + id,
            'href': '#' + id + ' .item-' + panelIndex
        });

        var wrapper = $('<div class="panel-collapse collapse item-' + panelIndex + '"></div>');

        item.find('.panel-body p').html('Item body.').parent().wrap(wrapper);
        item.find('.panel-footer').remove();

        switch (pos) {
            case 'begin':
                target.prepend(item);
                break;
            case 'end':
                target.append(item);
                break;
            case 'before':
                panel.before(item);
                break;
            case 'after':
                panel.after(item);
                break;
        }
        MBE.DnD.updateDOM();
    },

    accordionAddToBegin: function() {
        MBE.types.containers.accordionAddItem.apply(this, ['begin']);
    },

    accordionAddToEnd: function() {
        MBE.types.containers.accordionAddItem.apply(this, ['end']);
    },

    accordionAddBefore: function() {
        MBE.types.containers.accordionAddItem.apply(this, ['before']);
    },

    accordionAddAfter: function() {
        MBE.types.containers.accordionAddItem.apply(this, ['after']);
    },

    panelIsNotInAccordion: function()
    {
        if ($(this).parent().hasClass('panel-group')) {
            return false;
        }
        return true;
    },

    panelIsInAccordion: function()
    {
        if ($(this).parent().hasClass('panel-group')) {
            return true;
        }
        return false;
    }
};

MBE.onInit.push(MBE.types.containers.init);