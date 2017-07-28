MBE.types.functions = {

    templates: {
        'foreach': '<div></div>',
        'if': '<div></div>',
    },

    options: {
        'foreach': {
            'foreachOptions': {
                name: 'Foreach options',
                type: 'group',
                groupItems: [{
                    label: 'Wrapper tag',
                    type: 'select',
                    options: {
                        'tr': 'tr',
                        'div': 'div',
                        'li': 'list item'
                    },
                    get: MBE.options.is,
                    set: MBE.options.toggleTagName
                }, {
                    label: 'Input variable name',
                    type: 'text',
                    attr: 'data-varname',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Data type',
                    type: 'select',
                    options: {
                        'null': '-- chose one --',
                        'jtoken': 'JToken',
                        'dbitem': 'DbItem'
                    },
                    attr: 'data-type',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]
            }
        },
        'if': {
            'ifOptions': {
                name: 'IF options',
                type: 'group',
                groupItems: [{
                    label: 'Wrapper tag',
                    type: 'select',
                    options: {
                        'tr': 'tr',
                        'div': 'div',
                        'li': 'list item'
                    },
                    get: MBE.options.is,
                    set: MBE.options.toggleTagName
                }, {
                    label: 'Variable name',
                    type: 'text',
                    attr: 'data-varname',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }]
            }
        }
    },

    init: function () {

    }
};

MBE.onInit.push(MBE.types.functions.init);