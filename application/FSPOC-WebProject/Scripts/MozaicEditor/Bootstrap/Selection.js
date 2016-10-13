MBE.selection = {
    
    onSelect: [],

    init: function()
    {
        $(document)
            .on('click', '[data-uic], #mozaicPageWorkspace', MBE.selection.select)
        ;
    },

    select: function(event)
    {
        if(typeof event != 'undefined') {
            event.stopImmediatePropagation();
        }

        MBE.workspace.find('.mbe-active').removeClass('mbe-active');
        MBE.workspace.find('.mbe-drag-handle').remove();
        MBE.selection.selectElement(this);

        for(var i = 0; i < MBE.selection.onSelect.length; i++) {
            MBE.selection.onSelect[i].apply(this, []);
        }
    },

    selectElement: function(elm)
    {
        elm = $(elm);
        if(elm.is('[data-uic]')) {
            elm.addClass('mbe-active');
            elm.prepend('<span class="mbe-drag-handle" draggable="true"></span>');
        }
    }

};

MBE.onInit.push(MBE.selection.init);