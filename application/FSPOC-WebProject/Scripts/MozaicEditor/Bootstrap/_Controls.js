MBE.types.controls = {

    templates: {
        'button': '<button type="button" class="btn btn-default">Button</button>',
        'button-group': '<div class="btn-group" role="group">'
                            + '<button type="button" class="btn btn-default" data-uic="controls|button">Left</button>'
                            + '<button type="button" class="btn btn-default" data-uic="controls|button">Middle</button>'
                            + '<button type="button" class="btn btn-default" data-uic="controls|button">Right</button>'
                        + '</div>',
        'button-toolbar': '<div class="btn-toolbar" role="toolbar">'
                            + '<div class="btn-group" role="group" data-uic="controls|button-group">'
                                + '<button type="button" class="btn btn-default" data-uic="controls|button">Button 1</button>'
                                + '<button type="button" class="btn btn-default" data-uic="controls|button">Button 2</button>'
                            + '</div>'
                            + '<div class="btn-group" role="group" data-uic="controls|button-group">'
                                + '<button type="button" class="btn btn-default" data-uic="controls|button">Button 3</button>'
                                + '<button type="button" class="btn btn-default" data-uic="controls|button">Button 4</button>'
                            + '</div>'
                        + '</div>',
        'split-button': '<div class="btn-group"></div>',
        'button-dropdown': '<div class="btn-group"></div>',
        'dropdown-menu': '<ul class="dropdown-menu" data-uic="controls|dropdown-menu" locked></ul>',
        'dropdown-menu-item': '<li data-uic="controls|dropdown-menu-item"></li>',
        'dropdown-menu-header': '<li class="dropdown-header" data-uic="controls|dropdown-menu-header">Header</li>',
        'dropdown-menu-divider': '<li class="divider" data-uic="controls|dropdown-menu-divider"></li>',
        'link': '<a href="#" target="">Link</a>'
    },

    options: {
        'button': {
            'buttonOptions': {
                name: 'Button options',
                type: 'group',
                groupItems: [{
                    id: 'controls_button_element',
                    label: 'Element',
                    type: 'select',
                    options: {
                        'button': 'Button',
                        'a': 'Link'
                    },
                    get: MBE.options.is,
                    set: function (opt) {
                        $('#controls_button_button_type').toggle(opt.value == 'button');
                        $('#controls_button_link_url').toggle(opt.value != 'button');
                        $('#controls_button_link_target').toggle(opt.value != 'button');

                        if (opt.value == 'button') {
                            $(this).removeAttr('href target').attr('type', 'button');
                            $('#controls_button_button_type select').val('button');
                        }
                        if (opt.value == 'a') {
                            $(this).removeAttr('type').attr({ 'href': '#', 'target': '' });
                            $('#controls_button_link_url input').val('#');
                            $('#controls_button_link_target select').val('null');
                        }
                        
                        MBE.options.toggleTagName.apply(this, [opt]);
                    }
                }, {
                    id: 'controls_button_button_type',
                    label: 'Button type',
                    type: 'select',
                    options: {
                        'button': 'Button',
                        'submit': 'Submit',
                        'reset': 'Reset',
                    },
                    attr: 'type',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    id: 'controls_button_link_url',
                    label: 'Link URL',
                    type: 'text',
                    attr: 'href',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    id: 'controls_button_link_target',
                    label: 'Link target',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        '_blank': 'Blank',
                        '_parent': 'Parent',
                        '_top': 'Top'
                    },
                    attr: 'target',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Style',
                    type: 'select',
                    options: {
                        'btn-default': 'Default',
                        'btn-primary': 'Primary',
                        'btn-success': 'Success',
                        'btn-info': 'Info',
                        'btn-warning': 'Warning',
                        'btn-danger': 'Danger',
                        'btn-link': 'Link'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Size',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'btn-lg': 'Large',
                        'btn-sm': 'Small',
                        'btn-xs': 'Extra small'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Disabled',
                    type: 'boolean',
                    options: {
                        'disabled': 'Disabled'
                    },
                    get: function() { return $(this).is(':disabled'); },
                    set: function(opt) { $(this).attr('disabled', opt.checked); }
                }, {
                    label: 'Misc',
                    type: 'boolean',
                    options: {
                        'active': 'Active',
                        'btn-block': 'Block'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }],
                onBuild: function () {
                    var opt = $('#controls_button_element select');
                    console.log(opt.val());
                    $('#controls_button_button_type').toggle(opt.val() == 'button');
                    $('#controls_button_link_url').toggle(opt.val() != 'button');
                    $('#controls_button_link_target').toggle(opt.val() != 'button');
                }
            }
        },
        'button-group': {
            'buttonGroupOptions': {
                name: 'Button group options',
                type: 'group',
                groupItems: [{
                    label: 'Justified',
                    type: 'boolean',
                    options: {
                        'btn-group-justified': 'Justified'
                    },
                    get: MBE.options.hasClass,
                    set: function (opt) {
                        if (opt.checked) {
                            $('> button', this).each(function () {
                                var newTag = $('<a />');
                                for (var i = 0; i < this.attributes.length; i++) {
                                    var nodeName = this.attributes.item(i).nodeName;
                                    if (nodeName == 'type')
                                        continue;
                                    newTag[0].setAttribute(nodeName, this.attributes.item(i).nodeValue);
                                }
                                newTag.attr({ 'href': '#', 'target': '' }).html(this.innerHTML);
                                $(this).replaceWith(newTag);
                            });
                        }
                        else {
                            $('> a', this).each(function () {
                                var newTag = $('<button />');
                                for (var i = 0; i < this.attributes.length; i++) {
                                    var nodeName = this.attributes.item(i).nodeName;
                                    if (nodeName == 'href' || nodeName == 'target')
                                        continue;
                                    newTag[0].setAttribute(nodeName, this.attributes.item(i).nodeValue);
                                }
                                newTag.attr('type', 'button').html(this.innerHTML);
                                $(this).replaceWith(newTag);
                            });
                        }
                        $(this).toggleClass(opt.value);
                        MBE.DnD.updateDOM();
                    }
                }, {
                    label: 'Type',
                    type: 'select',
                    options: {
                        'btn-group': 'Horizontal',
                        'btn-group-vertical': 'Vertical'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Size',
                    type: 'select',
                    options: {
                        'btn-group-lg': 'Large',
                        'null': 'Default',
                        'btn-group-sm': 'Small',
                        'btn-group-xs': 'Extra small'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'link': {
            'linkOptions': {
                name: 'Link options',
                type: 'group',
                groupItems: [{
                    label: 'Link URL',
                    type: 'text',
                    attr: 'href',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Link target',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        '_blank': 'Blank',
                        '_parent': 'Parent',
                        '_top': 'Top'
                    },
                    attr: 'target',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]
            },
            'textOptions': MBE.types.text.options.common.textOptions
        }
    },

    init: function () 
    {
        var self = MBE.types.controls;

        MBE.DnD.onDrop.push(MBE.types.controls._drop);

        var menu = MBE.toolbar.menu;
        menu['controls'] = {};
        menu['controls']['button-dropdown'] = {
            allowFor: self.isDropdown,
            items: [
                { type: 'text', label: 'ADD TO MENU' },
                { type: 'button', label: 'ITEM', callback: self.dropdownAddItem },
                { type: 'button', label: 'HEADER', callback: self.dropdownAddHeader },
                { type: 'button', label: 'DIVIDER', callback: self.dropdownAddDivider },
                { type: 'text', label: 'MENU' },
                { type: 'button', label: 'SHOW', callback: self.dropdownShowMenu, allowFor: self.dropdownIsMenuHidden },
                { type: 'button', label: 'HIDE', callback: self.dropdownHideMenu, allowFor: self.dropdownIsMenuVisible },
            ]
        };
        menu['controls']['split-button'] = menu['controls']['button-dropdown'];
        menu['controls']['button'] = menu['controls']['button-dropdown'];
        menu['controls']['dropdown-menu'] = menu['controls']['button-dropdown'];
        menu['controls']['dropdown-menu-item'] = menu['controls']['button-dropdown'];
        menu['controls']['dropdown-menu-header'] = menu['controls']['button-dropdown'];
        menu['controls']['dropdown-menu-divider'] = menu['controls']['button-dropdown'];
        menu['controls']['link'] = menu['controls']['button-dropdown'];
    },

    _drop: function(target)
    {
        if ($(this).is('[data-uic="controls|button-dropdown"]:empty') || $(this).is('[data-uic="controls|split-button"]:empty')) {
            MBE.types.controls.dropdownBuild.apply(this, []);
        }
    },

    /**************************************************************/
    /* DROPDOWN CONTEXT METHODS                                   */
    /**************************************************************/
    isDropdown: function() {
        return $(this).is('[data-uic="controls|button-dropdown"]') ||
               $(this).parents('[data-uic="controls|button-dropdown"]').length ||
               $(this).is('[data-uic="controls|split-button"]') ||
               $(this).parents('[data-uic="controls|split-button"]').length;
    },

    dropdownBuild: function()
    {
        var self = MBE.types.controls;
        var dd = $(this);
        var button = $(self.templates['button']);
        var menu = $(self.templates['dropdown-menu']);
        var caret = $(MBE.types.misc.templates['caret']);
        var isSplitButton = dd.is('[data-uic="controls|split-button"]');
        
        caret.attr('data-uic', 'misc|caret');

        button.attr({
            'locked': true,
            'data-uic': 'controls|button',
            'data-toggle': 'dropdown'
        }).addClass('dropdown-toggle').html(isSplitButton ? '' : 'Dropdown ');
        button.append(caret);

        if (isSplitButton) {
            var actionButton = $(self.templates['button']);
            actionButton.attr({
                'locked': true,
                'data-uic': 'controls|button'
            }).html('Action').appendTo(dd);
        }
        
        var names = ['First item', 'Second item', 'Third item'];
        for (var i = 0; i < 3; i++)
        {
            var item = $(self.templates['dropdown-menu-item']);
            var link = $(self.templates.link);

            link.attr({
                'data-uic': 'controls|link',
                'locked': true
            }).html(names[i]).appendTo(item);
            
            item.appendTo(menu);
        }

        dd.append(button);
        dd.append(menu);
        MBE.DnD.updateDOM();
    },

    dropdownGetTarget: function()
    {
        var elm = $(this);
        if (elm.is('[data-uic="controls|button-dropdown"]')) {
            return $(this);
        }
        if (elm.parents('[data-uic="controls|button-dropdown"]').length) {
            return elm.parents('[data-uic="controls|button-dropdown"]').eq(0);
        }
        if (elm.is('[data-uic="controls|split-button"]')) {
            return $(this);
        }
        if (elm.parents('[data-uic="controls|split-button"]').length) {
            return elm.parents('[data-uic="controls|split-button"]').eq(0);
        }
        return $('<div></div>');
    },

    dropdownAddItem: function()
    {
        var self = MBE.types.controls;
        var target = self.dropdownGetTarget.apply(this, []);
        var item = $(self.templates['dropdown-menu-item']);
        var link = $(self.templates.link);

        link.attr({
            'data-uic': 'controls|link',
            'locked': true
        }).html('Menu item').appendTo(item);
        
        target.find('ul.dropdown-menu').append(item);
        MBE.DnD.updateDOM();

        setTimeout(function () { target.addClass('open'); }, 1);
    },

    dropdownAddHeader: function()
    {
        var self = MBE.types.controls;
        var target = self.dropdownGetTarget.apply(this, []);
        var item = $(self.templates['dropdown-menu-header']);

        target.find('ul.dropdown-menu').append(item);
        MBE.DnD.updateDOM();

        setTimeout(function () { target.addClass('open'); }, 1);
    },

    dropdownAddDivider: function()
    {
        var self = MBE.types.controls;
        var target = self.dropdownGetTarget.apply(this, []);
        var item = $(self.templates['dropdown-menu-divider']);

        target.find('ul.dropdown-menu').append(item);
        MBE.DnD.updateDOM();

        setTimeout(function () { target.addClass('open'); }, 1);
    },

    dropdownIsMenuHidden: function()
    {
        var self = MBE.types.controls;
        var target = self.dropdownGetTarget.apply(this, []);

        return !target.find('ul.dropdown-menu').is(':visible');
    },

    dropdownIsMenuVisible: function () {
        var self = MBE.types.controls;
        var target = self.dropdownGetTarget.apply(this, []);

        return target.find('ul.dropdown-menu').is(':visible');
    },

    dropdownShowMenu: function () {
        var self = MBE.types.controls;
        var target = self.dropdownGetTarget.apply(this, []);
        target.addClass('open');
        MBE.toolbar._select.apply($('.mbe-active', MBE.workspace)[0], []);
    },

    dropdownHideMenu: function () {
        var self = MBE.types.controls;
        var target = self.dropdownGetTarget.apply(this, []);
        target.removeClass('open');
        MBE.toolbar._select.apply($('.mbe-active', MBE.workspace)[0], []);
    }
}

MBE.onInit.push(MBE.types.controls.init);