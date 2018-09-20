MBE.types.form = {

    templates: {
        'form': '<form class="form-horizontal" method="post"></form>',
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
        'textarea': '<textarea class="form-control" name="" value=""></textarea>',
        'checkbox-group': '<div class="checkbox"></div>',
        'radio-group': '<div class="radio"></div>',
        'checkbox': '<input type="checkbox" name="" value="">',
        'checkbox-label': '<label for="" data-uic="form|label"><input type="checkbox" name="" value="" data-uic="form|checkbox"> Checkbox</label>',
        'radio': '<input type="radio" name="" value="">',
        'radio-label': '<label for="" data-uic="form|label"><input type="radio" name="" value="" data-uic="form|radio"> Radio</label>',
        'static-control': '<p class="form-control-static">Static value</p>',
        'help-text': '<p class="help-block">Help text for field</p>',
        'input-group': '<div class="input-group">'
                            + '<div class="input-group-addon" data-uic="form|left-addon" locked><span data-uic="text|span">prefix</span></div>'
                            + '<input type="text" name="" value="" class="form-control" data-uic="form|input-text">'
                            + '<div class="input-group-addon" data-uic="form|right-addon" locked><span data-uic="text|span">suffix</span></div>'
                        + '</div>',
        'fieldset': '<fieldset><legend data-uic="form|legend" locked>Field group</legend></fieldset>',
        'legend': '<legend data-uic="form|legend" locked>Field group</legend>',
        'left-addon': '<div class="input-group-addon" data-uic="form|left-addon" locked><span data-uic="text|span">prefix</span></div>',
        'right-addon': '<div class="input-group-addon" data-uic="form|right-addon" locked><span data-uic="text|span">suffix</span></div>',
        'form-control-feedback': '<span class="glyphicon glyphicon-remove form-control-feedback"></span>',
        'option': '<option value="" data-uic="form|option" locked></option>'
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
                }, {
                    label: 'Switch',
                    type: 'boolean',
                    disallowFor: function () {
                        return !($(this).find(':checkbox').length == 0);
                    },
                    options: {
                        'switch': 'Switch'
                    },
                    get: MBE.options.hasClass,
                    set: function (opt) {
                        switch (opt.value) {
                            case 'switch': {
                                if ($(opt).is(':checked')) {
                                    $(this).find('> .mbe-text-node').remove();
                                    $(this).addClass(opt.value).append('<span data-uic="text|span" class="switch-slider"></span>');
                                }
                                else {
                                    $(this).find('> .switch-slider').remove();
                                    $(this).removeClass(opt.value).append('<span class="mbe-text-node">Checkbox</span>');
                                }
                                MBE.DnD.updateDOM();
                                break;
                            }
                        }
                    }
                }]
            },
            'labelSize': {
                name: 'Label size',
                type: 'group',
                groupItems: MBE.types.grid.options.column.columnSize.groupItems,
                get: MBE.options.hasClass,
                set: MBE.options.toggleClass
            },
        },
        'select': {
            'selectOptions': {
                name: 'Select options',
                type: 'group',
                groupItems: [{
                    label: 'Default option',
                    type: 'text',
                    get: function () {
                        return $('option', MBE.options.target).length == 1 ? $('option', MBE.options.target).eq(0).text() : '';
                    },
                    set: function (opt) {
                        $('option', this).remove();
                        if (opt.value) {
                            var node = $(MBE.types.form.templates.option);
                            node.html(opt.value);
                            $(this).append(node);
                        }
                        MBE.DnD.updateDOM();
                    }
                }],
            }
        },
        'option': {
            'optionOptions': {
                name: 'Option options',
                type: 'group',
                groupItems: [{
                    label: 'Value',
                    type: 'text',
                    attr: 'for',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Text',
                    type: 'text',
                    get: function () {
                        return $(MBE.options.target).html();
                    },
                    set: function (opt) {
                        $(this).html(opt.value);
                    }
                }]
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
        'fieldset': {
            'FieldsetOptions': {
                name: 'Fieldset options',
                type: 'group',
                groupItems: [{
                    label: 'Name',
                    type: 'text',
                    attr: 'name',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'State',
                    type: 'boolean',
                    options: {
                        'disabled': 'Disabled'
                    },
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Legend',
                    type: 'boolean',
                    options: {
                        'legend': 'Show legend'
                    },
                    get: function (value) {
                        return $('[data-uic="form|' + value + '"]', this).length > 0;
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            if (opt.value == 'legend') {
                                $(this).prepend(MBE.types.form.templates['legend']);
                            }
                        }
                        else {
                            $('[data-uic="form|' + opt.value + '"]', this).remove();
                        }
                        MBE.DnD.updateDOM();
                    }
                }]
            }
        },
        'checkbox-group': {
            'checkboxGroupOptions': {
                name: 'Checkbox group options',
                type: 'group',
                groupItems: [{
                    label: 'Type',
                    type: 'select',
                    options: {
                        'checkbox': 'Default',
                        'checkbox-inline': 'Inline'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'radio-group': {
            'radioGroupOptions': {
                name: 'Radio group options',
                type: 'group',
                groupItems: [{
                    label: 'Type',
                    type: 'select',
                    options: {
                        'radio': 'Default',
                        'radio-inline': 'Inline'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'form-control-feedback': {
            'formControlFeedbackOptions': {
                name: 'Form control feedback options',
                type: 'group',
                groupItems: [{
                    label: 'Icon',
                    type: 'icon',
                    fontSets: { 'glyphicon': 'Glyphicons' },
                    set: MBE.options.setIcon
                }]
            }
        },
        'form-group': {
            'formGroupOptions': {
                name: 'Form group options',
                type: 'group',
                groupItems: [{
                    label: 'Size',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'form-group-lg': 'Large',
                        'form-group-sm': 'Small'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },

        common: {
            'mainOptions': {
                name: 'Main',
                allowFor: [
                    'input-text', 'input-email', 'input-color', 'select', 'input-tel', 'input-number', 'input-range', 'input-hidden',
                    'input-url', 'input-search', 'input-password', 'input-file', 'input-date', 'textarea', 'checkbox', 'radio'
                ],
                type: 'group',
                groupItems: [{
                    label: 'Name',
                    type: 'text',
                    attr: 'name',
                    id: 'AttrName',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr,
                    change: function (opt) {
                        if ($(this).is('[type=radio]')) {
                            if (opt.value.length && $('#AttrValue input').val().length) {
                                $('#AttrID input').val(opt.value + '_' + $('#AttrValue input').val());
                            }
                        }
                        else {
                            $('#AttrID input').val(opt.value).change();
                        }
                    }
                }, {
                    label: 'Rows',
                    allowFor: ['textarea'],
                    type: 'number',
                    attr: 'rows',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Min',
                    allowFor: ['input-number', 'input-range', 'input-date'],
                    type: 'number',
                    attr: 'min',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Max',
                    allowFor: ['input-number', 'input-range', 'input-date'],
                    type: 'number',
                    attr: 'max',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Step',
                    allowFor: ['input-number', 'input-range', 'input-date'],
                    type: 'number',
                    attr: 'step',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Size',
                    disallowFor: ['input-hidden', 'checkbox', 'radio'],
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
                    allowFor: ['input-text', 'input-email', 'input-tel', 'input-url', 'input-search', 'input-password', 'textarea'],
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
                }, {
                    label: 'Value',
                    allowFor: ['radio'],
                    type: 'text',
                    attr: 'value',
                    id: 'AttrValue',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr,
                    change: function (opt) {
                        if ($(this).is('[type=radio]')) {
                            if (opt.value.length && $('#AttrName input').val().length) {
                                $('#AttrID input').val($('#AttrName input').val() + '_' + opt.value);
                            }
                        }
                    }
                }, {
                    label: 'Checked',
                    allowFor: ['checkbox', 'radio'],
                    type: 'boolean',
                    options: {
                        'checked': 'Checked'
                    },
                    get: MBE.options.hasProp,
                    set: MBE.options.setProp
                }]
            },
            'stateOptions': {
                name: 'State',
                disallowFor: ['input-hidden', 'static-control', 'help-text', 'legend', 'checkbox', 'radio'],
                type: 'boolean',
                options: {
                    'readonly': 'Readonly',
                    'disabled': 'Disabled'
                },
                get: MBE.options.hasAttr,
                set: MBE.options.setAttr
            },
            'crStateOptions': {
                name: 'State',
                allowFor: ['checkbox', 'radio'],
                type: 'boolean',
                options: {
                    'disabled': 'Disabled'
                },
                get: MBE.options.hasAttr,
                set: MBE.options.setAttr
            },
            'inputOptions': {
                name: 'Input',
                allowFor: [
                    'input-text', 'input-email', 'select', 'input-tel', 'input-number', 'input-url', 'input-search', 'input-password',
                    'input-file', 'textarea'
                ],
                type: 'group',
                groupItems: [{
                    label: 'Autofocus',
                    allowFor: [
                        'input-text', 'input-email', 'select', 'input-tel', 'input-number', 'input-url', 'input-search', 'input-password',
                        'textarea'
                    ],
                    type: 'boolean',
                    options: {
                        'autofocus': 'Autofocus'
                    },
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Autocomplete',
                    allowFor: [
                        'input-text', 'input-email', 'input-tel', 'input-number', 'input-url', 'input-search', 'input-password',
                        'textarea'
                    ],
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
            },
            
            validationOptions: {
                name: 'Validation',
                type: 'group',
                disallowFor: [
                    'form', 'label', 'form-group', 'input-hidden', 'input-range', 'form-control-feedback', 'static-control',
                    'help-text', 'input-group', 'fieldset', 'legend', 'left-addon', 'right-addon'
                ],
                groupItems: [{
                    label: 'Required',
                    type: 'boolean',
                    options: {
                        'required': 'Required'
                    },
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    disallowFor: ['checkbox', 'radio', 'input-color', 'select', 'input-file'],
                    label: 'Min length',
                    type: 'text',
                    attr: 'minlength',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    disallowFor: ['checkbox', 'radio', 'input-color', 'select', 'input-file'],
                    label: 'Max length',
                    type: 'text',
                    attr: 'maxlength',
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
        if ($(this).is('.form-control-feedback')) {
            target.addClass('has-feedback');
        }
    },

    _domUpdate: function () {
        $('.input-group-btn:empty').each(function () {
            $(this).removeClass('input-group-btn').addClass('input-group-addon');
        });
    }
};

MBE.onInit.push(MBE.types.form.init);