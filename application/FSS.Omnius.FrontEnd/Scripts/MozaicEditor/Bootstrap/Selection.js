MBE.selection = {
    
    onSelect: [],

    init: function()
    {
        $(MBE.workspaceDoc)
            .on('click', '[data-uic], body', MBE.selection.select)
        ;

        MBE.DnD.onDOMUpdate.push(MBE.selection._update);
    },

    select: function(event)
    {
        if(typeof event != 'undefined') {
            event.stopImmediatePropagation();
        }

        MBE.workspace.find('.mbe-active').removeClass('mbe-active');
        $('.mbe-drag-handle', MBE.workspace).remove();
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
            if (!elm.is('[locked]')) {
                var handle = $('<span class="mbe-drag-handle" draggable="true"></span>');
                handle.css({
                    top: elm.offset().top - 4,
                    left: elm.offset().left - 4
                });
                MBE.workspace.append(handle);
            }
        }
    },

    _update: function () {
        var elm = $('.mbe-active', MBE.workspace);
        if (elm.length) {
            $('.mbe-drag-handle', MBE.workspace).css({
                top: elm.offset().top - 4,
                left: elm.offset().left - 4
            });
        }
    }
};

MBE.onInit.push(MBE.selection.init);