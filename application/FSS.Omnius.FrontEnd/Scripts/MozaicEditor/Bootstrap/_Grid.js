MBE.types.grid = {

    templates: {
        'row': '<div class="row"></div>',
        'column': '<div class="col-xs-12"></div>',
        'clearfix': '<div class="clearfix"></div>'
    },

    options: {
        'column': {
            'columnSize': {
                name: 'Column size',
                type: 'group',
                groupItems: []
            },
            'columnOffset': {
                name: 'Column offset',
                type: 'group',
                groupItems: []
            },
            'columnPush': {
                name: 'Column push',
                type: 'group',
                groupItems: []
            },
            'columnPull': {
                name: 'Column pull',
                type: 'group',
                groupItems: []
            }
        }
    },

    initColumnOptions: function()
    {
        var formatList = {
            columnSize: 'col-{l}-{c}',
            columnOffset: 'col-{l}-offset-{c}',
            columnPush: 'col-{l}-push-{c}',
            columnPull: 'col-{l}-pull-{c}'
        };
        var sizeList = ['xs', 'sm', 'md', 'lg'];

        for (var k in MBE.types.grid.options.column) {
            var format = formatList[k];
            for (var i = 0; i < sizeList.length; i++) {
                var size = sizeList[i];
                var opt = {
                    label: size.toUpperCase(),
                    type: 'select',
                    options: {'null': 'None'},
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                };
                for (var j = 1; j <= 12; j++) {
                    var ok = format.replace(/\{l\}/, size).replace(/\{c\}/, j);
                    opt.options[ok] = j;
                }
                MBE.types.grid.options.column[k].groupItems.push(opt);
            }
        }
    }
};

if ($('body').hasClass('mozaicBootstrapEditorModule')) {
    $(MBE.types.grid.initColumnOptions);
}