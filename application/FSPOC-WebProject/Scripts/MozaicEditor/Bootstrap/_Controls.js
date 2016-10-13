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
                        'btn-lg': 'Large',
                        'null': 'Default',
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
    }
}