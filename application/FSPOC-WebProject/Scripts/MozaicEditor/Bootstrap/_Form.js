MBE.types.form = {

    templates: {
        'form': '<form class="form-horizontal"></form>',
        'form-group': '<div class="form-group"></div>',
        'label': '<label for="">Label</label>',
        'input-text': '<input type="text" name="" value="" class="form-control">',
        'input-email': '<input type="email" name="" value="" class="form-control">',
        'input-color': '<input type="color" name="" value="" class="form-control">',
        'select': '<select name="" class="form-control"></select>',
        'input-tel': '<input type="tel" name="" value="" class="form-control">',
        'input-date': '<input type="date" name="" value="" class="form-control">',
        'input-number': '<input type="number" name="" value="" class="form-control">',
        'input-range': '<input type="range" name="" value="" class="form-control">',
        'input-hidden': '<input type="hidden" name="" value="">',
        'input-url': '<input type="url" name="" value="" class="form-control">',
        'input-search': '<input type="search" name="" value="" class="form-control">',
        'input-password': '<input type="password" name="" value="" class="form-control">',
        'input-file': '<input type="file" name="" value="" class="form-control">',
        'static-control': '<p class="form-control-static">Static value</p>',
        'help-text': '<p class="help-block">Help text for field</p>',
        'input-group': '<div class="input-group">'
                            + '<div class="input-group-addon" data-uic="form|left-addon" locked><span data-uic="text|span">prefix</span></div>'
                            + '<input type="text" name="" value="" class="form-control" data-uic="form|input-text">'
                            + '<div class="input-group-addon" data-uic="form|right-addon" locked><span data-uic="text|span">suffix</span></div>'
                        + '</div>',
        'left-addon': '<div class="input-group-addon" data-uic="form|left-addon" locked><span data-uic="text|span">prefix</span></div>',
        'right-addon': '<div class="input-group-addon" data-uic="form|right-addon" locked><span data-uic="text|span">suffix</span></div>'
    },

    options: {
        'form': {
            'formOptions': {
                name: 'Form options',
                type: 'group',
                groupItems: [{
                    label: 'Type',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'form-horizontal': 'Horizontal',
                        'form-inline': 'Inline'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Method',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'get': 'GET',
                        'post': 'POST'
                    },
                    attr: 'method',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Encoding',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'application/x-www-form-urlencoded': 'URL Encoded',
                        'multipart/form-data': 'Multipart', 
                        'text/plain': 'Plain'
                    },
                    attr: 'enctype',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]                
            }
        },
        'label': {
            'labelOptions': {
                name: 'Label options',
                type: 'group',
                groupItems: [{
                    label: 'For',
                    type: 'text',
                    attr: 'for',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]
            }
        },
        'select': {
            'selectOptions': {
                name: 'Select options',
                type: 'group',
                groupItems: [{
                    label: 'Default option',
                    type: 'text',
                    get: function () {
                        return $('option', this).length == 1 ? $('option', this).eq(0).text() : '';
                    },
                    set: function (opt) {
                        $('option', this).remove();
                        if (opt.value) {
                            $(this).append('<option value="">' + opt.value + '</option>');
                        }
                    }
                }],
            }
        },
        'static-control': {
            'staticControlOptions': {
                name: 'Static control options',
                type: 'group',
                groupItems: [MBE.types.text.options.paragraph.paragraphOptions]
            }
        },
        'help-text': {
            'helpTextOptions': {
                name: 'Help text block options',
                type: 'group',
                groupItems: [MBE.types.text.options.paragraph.paragraphOptions]
            },
            'textOptions': MBE.types.text.options.common.textOptions
        },
        'input-group': {
            'inputGroupOptions': {
                name: 'Input group options',
                type: 'group',
                groupItems: [{
                    label: 'Size',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'input-group-lg': 'Large',
                        'input-group-sm': 'Small'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Addons',
                    type: 'boolean',
                    options: {
                        'left': 'Show left addon',
                        'right': 'Show right addon'
                    },
                    get: function (value) {
                        return $('[data-uic="form|' + value + '-addon"]', this).length > 0;
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            if (opt.value == 'left') {
                                $(this).prepend(MBE.types.form.templates['left-addon']);
                            }
                            else {
                                $(this).append(MBE.types.form.templates['right-addon']);
                            }
                        }
                        else {
                            $('[data-uic="form|' + opt.value + '-addon"]', this).remove();
                        }
                        MBE.DnD.updateDOM();
                    }
                }]
            }
        },

        common: {
            'mainOptions': {
                name: 'Main',
                allowFor: [
                    'input-text', 'input-email', 'input-color', 'select', 'input-tel', 'input-number', 'input-range', 'input-hidden',
                    'input-url', 'input-search', 'input-password', 'input-file', 'input-date'
                ],
                type: 'group',
                groupItems: [{
                    label: 'Name',
                    type: 'text',
                    attr: 'name',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Min',
                    allowFor: ['input-number', 'input-range', 'input-date'],
                    type: 'text',
                    attr: 'min',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Max',
                    allowFor: ['input-number', 'input-range', 'input-date'],
                    type: 'text',
                    attr: 'max',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Step',
                    allowFor: ['input-number', 'input-range', 'input-date'],
                    type: 'text',
                    attr: 'step',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Size',
                    disallowFor: ['input-hidden'],
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'input-lg': 'Large',
                        'input-sm': 'Small'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Placeholder',
                    allowFor: ['input-text', 'input-email', 'input-tel', 'input-url', 'input-search', 'input-password'],
                    type: 'text',
                    attr: 'placeholder',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Type',
                    allowFor: ['input-date'],
                    type: 'select',
                    attr: 'type',
                    options: {
                        'date': 'Date',
                        'time': 'Time',
                        'datetime-local': 'Datetime local',
                        'month': 'Month',
                        'week': 'Week'
                    },
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]
            },
            'stateOptions': {
                name: 'State',
                disallowFor: ['input-hidden', 'static-control', 'help-text'],
                type: 'boolean',
                options: {
                    'readonly': 'Readonly',
                    'disabled': 'Disabled'
                },
                get: MBE.options.hasAttr,
                set: MBE.options.setAttr
            },
            'inputOptions': {
                name: 'Input',
                allowFor: [
                    'input-text', 'input-email', 'select', 'input-tel', 'input-number', 'input-url', 'input-search', 'input-password',
                    'input-file'
                ],
                type: 'group',
                groupItems: [{
                    label: 'Autofocus',
                    allowFor: ['input-text', 'input-email', 'select', 'input-tel', 'input-number', 'input-url', 'input-search', 'input-password'],
                    type: 'boolean',
                    options: {
                        'autofocus': 'Autofocus'
                    },
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Autocomplete',
                    allowFor: ['input-text', 'input-email', 'input-tel', 'input-number', 'input-url', 'input-search', 'input-password'],
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'on': 'On',
                        'off': 'Off'
                    },
                    attr: 'autocomplete',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Multiple',
                    allowFor: ['input-file', 'select'],
                    type: 'boolean',
                    options: {
                        'multiple': 'Multiple'
                    },
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]
            }
        }
    },

    init: function () {
        MBE.DnD.onDOMUpdate.push(MBE.types.form._domUpdate);
        MBE.DnD.onDrop.push(MBE.types.form._drop);
    },

    _drop: function(target) {
        if (target.is('.input-group-addon') && $(this).is('input, button, select, div')) {
            target.toggleClass('input-group-addon input-group-btn');
        }
        if (target.is('.input-group-btn') && !$(this).is('input, button, select, div')) {
            target.toggleClass('input-group-addon input-group-btn');
        }
    },

    _domUpdate: function () {
        $('.input-group-btn:empty').each(function () {
            $(this).removeClass('input-group-btn').addClass('input-group-addon');
        });
    }
};

MBE.onInit.push(MBE.types.form.init);