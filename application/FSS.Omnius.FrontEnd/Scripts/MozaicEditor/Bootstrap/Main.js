﻿var MBE = {

    types: {},
    options: {},
    sortableOptions: {},

    onInit: [],
    onBeforeInit: [],
    onBeforeDelete: { '*': []},

    win: null,
    workspace: null,
    workspaceDoc: null,
    changedSinceLastSave: false,
    saveRequested: false,

    // Inicializace
    preInit: function ()
    {
        setTimeout(function () {
            MBE.win = $('#mozaicPageWorkspace > iframe')[0].contentWindow;
            MBE.workspaceDoc = MBE.win.document ? MBE.win.document : MBE.win.contentDocument;

            MBE.workspace = $('body', MBE.workspaceDoc);
            MBE.workspace
                    .html('')
                    .css({
                        'min-width': '100%',
                        'min-height': '100%'
                    })
                    .addClass('mozaicEditorBody')
                    .parent()
                        .css({
                            'width': '100%',
                            'height': '100%'
                        });
            MBE.init();
        }, 2000);
    },

    init: function()
    {
        for (i = 0; i < MBE.onBeforeInit.length; i++) {
            MBE.onBeforeInit[i]();
        }

        $(document)
            .on('click', 'ul.category li', MBE.toggleCategory)
            .on('dblclick', '.mbe-text-node', MBE.editText)
            .on('blur', '[contenteditable]', MBE.editTextDone)
            .on('click', '[data-uic]', MBE.onClick)
            .on('dblclick', '[data-uic]', MBE.options.openDialog)
            .on('keydown', MBE.onKeyDown)
            .on('click', '[data-action="fullscreen"]', MBE.toggleFullscreen)
            .on('click', '.device-button', MBE.setDevice)
            .on('webkitfullscreenchange mozfullscreenchange msfullscreenchange ofullscreenchange fullscreenchange', MBE.fullscreenResize)
            .on('click', '#btnChoosePage', MBE.dialogs.choosePage)
            .on('click', '#btnTrashPage', MBE.dialogs.trash)
            .on('click', '#btnNewPage', MBE.dialogs.newPage)
            .on('click', '#headerPageName', MBE.dialogs.rename)
            .on('click', 'tr.pageRow', MBE.selectPage)
            .on('click', '#btnClear', MBE.clearWorkspace)
            .on('click', '#btnLoad', MBE.io.reloadPage)
            .on('click', '#btnSave', MBE.io.savePage)
        
        ;
        $(MBE.workspaceDoc)
            .on('keydown', MBE.onKeyDown)
            .on('dblclick', '.mbe-text-node', MBE.editText)
            .on('blur', '[contenteditable]', MBE.editTextDone)
            .on('click', '[data-uic]', MBE.onClick)
            .on('dblclick', '[data-uic]', MBE.options.openDialog)
        ;
        
        $('ul.category > li ul').hide();
        $('ul.category > li').prepend('<span class="fa fa-caret-right fa-fw"></span>');
        $('ul.category > li > ul > li').prepend('<span class="fa fa-square fa-fw"></span>');

        for (i = 0; i < MBE.onInit.length; i++) {
            MBE.onInit[i]();
        }
    },

    onClick: function(event) {
        event.preventDefault();
    },

    onKeyDown: function(event) {
        if (event.which == 46 && $('.dialog-options').length == 0) {
            MBE.deleteItem();   
        }
    },

    deleteItem: function () {
        var target = $('.mbe-active', MBE.workspace);
        if (target.length && !target.is('[locked]') && !target.is('[contenteditable=true]') && !target.find('[contenteditable=true]').length) {
            if (typeof MBE.onBeforeDelete[target.data('uic')] == 'function') {
                MBE.onBeforeDelete[target.data('uic')].apply(target[0], []);
            }
            for (var i = 0; i < MBE.onBeforeDelete['*'].length; i++) {
                MBE.onBeforeDelete['*'][i].apply(target[0], []);
            }

            $('.mbe-active', MBE.workspace).remove();
            $('.mbe-drag-handle', MBE.workspace).remove();
            MBE.path.update.apply(MBE.workspace, []);
            MBE.DnD.updateDOM();
        }
    },

    // Kategorie
    toggleCategory: function(event)
    {
        if (!$(this).parent().hasClass('category')) {
            event.stopImmediatePropagation();
            return false;
        }
        $('> ul', this).slideToggle();
        $('> .fa', this).toggleClass('fa-caret-right fa-caret-down');
    },

    // Editace textu
    editText: function(event)
    {
        event.stopImmediatePropagation();
        $(this).attr('contenteditable', true).focus();
        return false;
    },

    editTextDone: function()
    {
        $('[contenteditable]', MBE.workspace).attr('contenteditable', false);
        $('.mbe-text-node > span', MBE.workspace).each(function () {
            $(this).parent().html(this.innerHTML);
        });
    },

    // TOOLS
    getComponentName: function(elm)
    {
        var uic = $(elm).data('uic').split(/\|/);
        var template = uic[1];
        return $('li[data-template="' + template + '"]').text();
    },

    setDevice: function () {
        $('.device-button').removeClass('active');
        $(this).addClass('active');

        $('#mozaicPageWorkspace > iframe').removeClass('xs sm md lg').addClass($(this).attr('data-action'));
    },

    toggleFullscreen: function()
    {
        if (MBE.runPrefixMethod(document, "FullScreen") || MBE.runPrefixMethod(document, "IsFullScreen")) {
            MBE.runPrefixMethod(document, "CancelFullScreen");
        }
        else {
            MBE.runPrefixMethod(document.body, "RequestFullScreen");
        }
        return false;
    },
	
    isFullscreen: function()
    {
        return MBE.runPrefixMethod(document, "FullScreen") || MBE.runPrefixMethod(document, "IsFullScreen");
    },

    fullscreenResize: function()
    {
        $('#lowerPanel').toggleClass('fullscreen');
    },

    runPrefixMethod: function (obj, method, testAPI) 
    {
        var p = 0, m, t;
        var prefixList = ["webkit", "moz", "ms", "o", ""];
        while (p < prefixList.length && !obj[m]) {
            m = method;
            if (prefixList[p] == "") {
                m = m.substr(0,1).toLowerCase() + m.substr(1);
            }
            m = prefixList[p] + m;
            t = typeof obj[m];
            if (t != "undefined") {
                prefixList = [prefixList[p]];
                return (t == "function" ? obj[m]() : obj[m]);
            }
            p++;
        }
        if(testAPI)
            return -1;
    },

    selectPage: function () {
        $(this).addClass('highlightedRow').siblings().removeClass('highlightedRow');
    },

    clearWorkspace: function () {
        MBE.workspace.html('');
        MBE.DnD.updateDOM();
    },

    ajaxError: function (request, status, error) {
        alert(request.responseText);
    }
}

if ($('body').hasClass('mozaicBootstrapEditorModule')) {
    $(MBE.preInit);
}