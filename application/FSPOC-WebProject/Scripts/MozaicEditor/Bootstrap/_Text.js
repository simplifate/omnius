MBE.types.text = {

    templates: {
        'heading': '<h1>Heading</h1>',
        'paragraph': '<p>Paragraph</p>',
        'alert': '<div class="alert alert-success">Alert text</div>',
        'blockquote': '<blockquote>'
                        + '<p data-uic="text|paragraph">Lorem ipsum dolor sit amet...</p>'
                        + '<footer data-uic="page|footer">Someone famous in <cite title="Source Title">Source Title</cite></footer>'
                      + '</blockquote>',
        'small': '<small>Text</small>',
        'strong': '<strong>Bold</strong>',
        'italic': '<em>Italic</em>',
        'span': '<span>Text</span>'
    },

    options: {
        'heading': {
            'headingOptions': {
                name: 'Heading options',
                type: 'select',
                label: 'Type',
                options: {
                    'h1': 'H1',
                    'h2': 'H2',
                    'h3': 'H3',
                    'h4': 'H4',
                    'h5': 'H5',
                    'h6': 'H6'
                },
                set: MBE.options.toggleTagName,
                get: MBE.options.is
            }
        },
        'paragraph': {
            'paragraphOptions': {
                name: 'Paragraph options',
                type: 'boolean',
                options: {
                    'lead': 'Lead'
                },
                set: MBE.options.toggleClass,
                get: MBE.options.hasClass
            }
        },
        'alert': {
            'alertOptions': {
                name: 'Alert options',
                type: 'group',
                groupItems: [{
                    type: 'select',
                    label: 'Style',
                    options: {
                        'alert-success': 'Success',
                        'alert-info': 'Info',
                        'alert-warning': 'Warning',
                        'alert-danger': 'Danger'
                    },
                    set: MBE.options.toggleClass,
                    get: MBE.options.hasClass
                }, {
                    type: 'boolean',
                    options: {
                        'alert-dismissible': 'Dismissable'
                    },
                    set: function (opt) {
                        var elm = $(this);
                        if (elm.hasClass(opt.value)) {
                            elm.removeClass(opt.value);
                            elm.find('.close').remove();
                        }
                        else {
                            elm.addClass(opt.value);
                            elm.append('<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
                        }
                    },
                    get: MBE.options.hasClass
                }]
            }
        },
        'blockquote': {
            'blockquoteOptions': {
                name: 'Blockquote options',
                type: 'group',
                groupItems: [{
                    type: 'select',
                    label: 'Type',
                    options: {
                        'null': 'Normal',
                        'blockquote-reverse': 'Reverse'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    type: 'boolean',
                    label: 'Show footer',
                    options: {
                        'show-footer': 'Show footer'
                    },
                    get: function () {
                        return $(this).find('footer').length > 0;
                    },
                    set: function (opt) {
                        if($(opt).is(':checked')) {
                            $(this).append('<footer data-uic="page|footer">Some famous in <cite>Source Title</cite></footer>');
                        }
                        else {
                            $(this).find('footer').remove();
                        }
                        MBE.DnD.updateDOM();
                    }
                }]
            }
        },
        'strong': {
            'strongOptions': {
                name: 'Bold options',
                type: 'group',
                groupItems: [{
                    label: 'Tag',
                    type: 'select',
                    options: {
                        'strong': 'Strong',
                        'b': 'B'
                    },
                    get: MBE.options.is,
                    set: MBE.options.toggleTagName
                }]
            }
        },
        'italic': {
            'italicOptions': {
                name: 'Italic options',
                type: 'group',
                groupItems: [{
                    label: 'Tag',
                    type: 'select',
                    options: {
                        'em': 'Em',
                        'i': 'I'
                    },
                    get: MBE.options.is,
                    set: MBE.options.toggleTagName
                }]
            }
        },

        common: {
            'textOptions': {
                name: 'Text options',
                type: 'group',
                allowFor: ['heading', 'paragraph', 'alert', 'small', 'strong', 'italic', 'span', 'link', 'help-text'],
                groupItems: [{
                    type: 'select',
                    label: 'Alignment',
                    allowFor: ['heading', 'paragraph', 'alert', 'help-text'],
                    options: {
                        'null': 'Default',
                        'text-left': 'Left',
                        'text-center': 'Center',
                        'text-right': 'Right',
                        'text-justify': 'Justify'
                    },
                    set: MBE.options.toggleClass,
                    get: MBE.options.hasClass
                }, {
                    type: 'select',
                    label: 'Transformation',
                    allowFor: ['heading', 'paragraph', 'alert', 'small', 'strong', 'italic', 'span', 'link', 'help-text'],
                    options: {
                        'null': 'None',
                        'text-lowercase': 'Lowercase',
                        'text-uppercase': 'Uppercase',
                        'text-capitalize': 'Capitalized'
                    },
                    set: MBE.options.toggleClass,
                    get: MBE.options.hasClass
                }, {
                    type: 'select',
                    label: 'Color',
                    allowFor: ['heading', 'paragraph', 'small', 'strong', 'italic', 'span', 'link', 'help-text'],
                    options: {
                        'null': 'Default',
                        'text-muted': 'Muted',
                        'text-primary': 'Primary',
                        'text-success': 'Success',
                        'text-info': 'Info',
                        'text-warning': 'Warning',
                        'text-danger': 'Danger'
                    },
                    set: MBE.options.toggleClass,
                    get: MBE.options.hasClass
                }, {
                    type: 'select',
                    label: 'Background',
                    allowFor: ['heading', 'paragraph', 'small', 'strong', 'italic', 'span', 'link', 'help-text'],
                    options: {
                        'null': 'Default',
                        'bg-primary': 'Primary',
                        'bg-success': 'Success',
                        'bg-info': 'Info',
                        'bg-warning': 'Warning',
                        'bg-danger': 'Danger'
                    },
                    set: MBE.options.toggleClass,
                    get: MBE.options.hasClass
                }, {
                    type: 'boolean',
                    allowFor: ['heading', 'paragraph', 'alert', 'help-text'],
                    options: {
                        'text-nowrap': 'No wrap'
                    },
                    set: MBE.options.toggleClass,
                    get: MBE.options.hasClass
                }],
            }
        }
    }
};