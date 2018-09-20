MBE.types.athena = {

    data: {},

    templates: {},

    options: {
    'common': {
        'commonOptions' : { 
            name: 'csv options',
            type: 'group',
            groupItems: [{
                label: 'csv value',
                type: 'text',
                attr: 'csv',
                get: MBE.options.hasAttr,
                set: MBE.options.setAttr
            }]
            }
        },
    },


    init: function () {
        this.loadGraphList();

        MBE.DnD.onDrop.push(this._onDrop);
        MBE.io.onLoad.push($.proxy(this._onLoad, this));
    },

    loadGraphList: function () {
        var url = '/api/athena/getGraphList';
        $.ajax(url, {
            dataType: 'json',
            async: false,
            success: $.proxy(this.setGraphList, this)
        });
    },

    setGraphList: function (data) {
        var target = $('ul[data-type="athena"]');

        for (var i = 0; i < data.length; i++) {
            var item = $('<li />');
            item.attr('data-template', data[i].Ident).html(data[i].Name);
            item.appendTo(target);

            this.templates[data[i].Ident] = '<div />';
            this.data[data[i].Ident] = data[i];
        }
    },

    build: function (code, id, data) {
        return code.replace(/\{ident\}/g, id).replace(/\{data\}/g, data.split(/\r?\n/).join('\\n'));
    },

    /*************************************************/
    /* EVENTS                                        */
    /*************************************************/
    _onDrop: function () {
        var elm = $(this); 

        if (elm.is('[data-uic^=athena]') && elm.is(':empty')) {
            var self = MBE.types.athena;
            var kv = elm.data('uic').split('|');
            var type = kv[1];
            var data = self.data[type];
            var css = data.Css ? '<style type="text/css" rel="stylesheet">' + self.build(data.Css, data.Ident + '_graph', '') + '</style>' : "";

            elm.css('height', 300).addClass('graphWrapper').attr('id', data.Ident);
            elm.append(self.build(data.Html, data.Ident + '_graph', ''));
            elm.append(css);

            var run = self.build(data.Js, data.Ident + '_graph', data.DemoData);
            MBE.win.eval.apply(MBE.win, [run]);
        }
    },

    _onLoad: function () {
        $('[data-uic^=athena]', MBE.workspace).each($.proxy(function (index, element) {
            var elm = $(element);
            var elmId = elm.attr('id');
            var kv = elm.data('uic').split('|');
            var type = kv[1];
            var data = this.data[type];
            var style = data.Css ? '<style type="text/css" rel="stylesheet">' + this.build(data.Css, elmId + '_graph', '') + '</style>' : '';

            elm.html('');
            elm.append(this.build(data.Html, elmId + '_graph', ''));
            elm.append(style);

            var run = this.build(data.Js, elmId + '_graph', data.DemoData);
            MBE.win.eval.apply(MBE.win, [run]);
        }, this));
    }
};

MBE.onBeforeInit.push($.proxy(MBE.types.athena.init, MBE.types.athena));