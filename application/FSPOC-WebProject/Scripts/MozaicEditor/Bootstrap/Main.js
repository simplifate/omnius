var MBE = {

    types: {},
    options: {},
    sortableOptions: {},

    onInit: [],

    workspace: null,

    // Inicializace
    init: function()
    {
        MBE.workspace = $('#mozaicPageWorkspace');

        $(document)
            .on('click', 'ul.category li', MBE.toggleCategory)
            .on('dblclick', '.mbe-text-node', MBE.editText)
            .on('blur', '[contenteditable]', MBE.editTextDone)
            .on('click', '[data-uic]', MBE.onClick)
            .on('dblclick', '[data-uic]', MBE.options.openDialog)
            .on('keydown', MBE.onKeyDown)
        ;
        
        $('ul.category > li ul').hide();
        $('ul.category > li').prepend('<span class="fa fa-caret-right fa-fw"></span>');
        $('ul.category > li > ul > li').prepend('<span class="fa fa-square fa-fw"></span>');

        $('#mozaicPageContainer').droppable("destroy");
 
        for (i = 0; i < MBE.onInit.length; i++) {
            var f = MBE.onInit[i];
            f();
        }
    },

    onClick: function(event) {
        event.preventDefault();
    },

    onKeyDown: function(event) {
        if (event.which == 46) {
            $('.mbe-active').remove();
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

    // Active state, navigace
    
    // Editace textu
    editText: function(event)
    {
        event.stopImmediatePropagation();
        $(this).attr('contenteditable', true).focus();
        return false;
    },

    editTextDone: function()
    {
        $('[contenteditable]').attr('contenteditable', false);
        $('.mbe-text-node > span').each(function() {
            $(this).parent().html(this.innerHTML);
        });
    },

    // TOOLS
    getComponentName: function(elm)
    {
        var uic = $(elm).data('uic').split(/\|/);
        var template = uic[1];
        return $('li[data-template="' + template + '"]').text();
    }
}

if ($('body').hasClass('mozaicBootstrapEditorModule')) {
    $(MBE.init);
}