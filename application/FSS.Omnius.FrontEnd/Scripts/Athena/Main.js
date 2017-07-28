var Athena = {

    onInit: [],
    gallery: null,
    svg: null,
    js: null,
    css: null,
    html: null,
    ident: null,
    preview: null,
    data: null,
    dataSource: null,

    jsEditor: null,
    cssEditor: null,
    htmlEditor: null,

    init: function () {

        this.gallery = $('#GalleryItems');
        this.svg = $('.graphPreview');
        this.js = $('.workspace .js textarea');
        this.css = $('.workspace .css textarea');
        this.data = $('.workspace .data textarea');
        this.html = $('.workspace .html textarea');
        this.preview = $('.workspace .preview .wrapper');

        this.ident = $('#Ident');
        
        $(document)
            .on('click', '#GalleryItems .item', this._definitionSelect)
            .on('blur', '#Name', this.makeIdent)
            .on('keyup', '#Name', this.sanitizeName)
            .on('blur', '.workspace .js textarea', $.proxy(this.render, this))
            .on('blur', '.workspace .data textarea', $.proxy(this.render, this))
            .on('blur', '.workspace .html textarea', $.proxy(this.render, this))
            .on('blur', '.workspace .css textarea', $.proxy(this.renderCss, this))
            .on('change', '#Library', $.proxy(this.changeLibrary, this));

        setTimeout($.proxy(this.initEditors, this), 250);
        
        this.changeLibrary();
        this.callHooks(this.onInit);

        this.render();
        this.renderCss();
    },

    initEditors: function () {

        this.jsEditor = CodeMirror.fromTextArea(this.js[0], {
            lineNumbers: true,
            lineWrapping: false,
            matchBrackets: true,
            autoCloseBrackets: true,
            mode: "text/javascript",
            foldGutter: true,
            gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
            extraKeys: {
                "Ctrl-Q": function (cm) { cm.foldCode(cm.getCursor()); },
                "Ctrl-Space": "autocomplete"
            }
        });
        this.jsEditor.setSize(null, this.js.parent().height());
    },

    callHooks: function (hooks, context, params) {
        context = context || this;
        for (var i = 0; i < hooks.length; i++) {
            hooks[i].apply(context, params);
        }
    },

    /*****************************************************/
    /* METHODS                                           */
    /*****************************************************/
    reset: function () {
        var varName = this.ident.val() + '_chart';
        if (typeof window[varName] != 'undefined' && typeof window[varName].destroy != 'undefined') {
            window[varName].destroy();
            delete window[varName];
        }

        this.preview.html(this.build(this.html.val()));
    },

    changeLibrary: function () {
        $('#BtnWizzard').attr('disabled', $('#Library').val() == 'd3');
        this.loadDefinitions();
    },

    renderDefault: function (definitionId) {
        this.js.val($('#' + definitionId).find('.jsSource').html());
        this.css.val($('#' + definitionId).find('.cssSource').html());
        this.data.val($('#' + definitionId).find('.dataSource').text());
        this.html.val($('#' + definitionId).find('.htmlSource').html());
        this.preview.html($('#' + definitionId).find('.htmlSource').html());
        this.jsEditor.setValue(this.js.val());

        this.render();
        this.renderCss();
    },

    loadDefinitions: function () {
        var lib = $('#Library').val();
        this.gallery.html('');

        $('#GraphDefinition #'+lib+' .item').each($.proxy(function (index, element) {
            var name = $(element).data('name');
            var icon = $(element).data('icon');
            var item = $('<a class="item" />');

            item.attr('data-id', $(element).attr('id'))
                .append('<span class="fa ' + icon + '"></span>')
                .append('<span class="label">' + name + '</span>')
                .appendTo(this.gallery);
        }, this));
    },

    render: function () {
        this.reset();
        if (this.jsEditor) {
            this.jsEditor.save();
        }
        try {
            eval.apply(window, [this.setData(this.build(this.js.val()))]);
        }
        catch (e) {
            var error = $('<span class="label label-danger" style="position:absolute;left:100px;top:8px"></span>');
            error.html(e.message);
            $('.workspace .js').append(error);

            setTimeout(function () {
                error.fadeOut('slow', function () { error.remove() });
            }, 2000);
        }
    },

    renderCss: function () {
        $('style#cssPreview').remove();
        if (this.css.val().length) {
            var style = $('<style type="text/css" rel="stylesheet" id="cssPreview"></style>');
            style.html(this.build(this.css.val()));
            style.appendTo('body'); 
        }
    },

    makeIdent: function () {
        var self = Athena;
        var name = this.value;
        name = RemoveDiacritics(name);
        name = name.replace(/ /g, '-');
        name = name.replace(/-{2,}/g, '-');

        self.preview.find('> *').eq(0).attr('id', name.toLowerCase());
        self.ident.val(name.toLowerCase());
        self.render();
        self.renderCss();
    },

    sanitizeName: function (e) {
        if (e.which == 32) {
            this.value = this.value.replace(/ /g, '_');
        }
    },

    build: function (code) {
        return code.replace(/\{ident\}/g, this.ident.val());
    },

    setData: function (code) {
        return code.replace(/\{data\}/g, this.data.val().split(/\n/).join('\\n'));  
    },

    /*****************************************************/
    /* EVENTS                                            */
    /*****************************************************/
    _definitionSelect: function () {
        var self = Athena;
        var definitionId = $(this).data('id');
        self.renderDefault(definitionId);
    }
};

if ($('body').hasClass('athenaForm')) {
    $($.proxy(Athena.init, Athena));
}