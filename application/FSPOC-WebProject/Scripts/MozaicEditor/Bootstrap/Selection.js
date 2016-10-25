MBE.selection = {
    
    onSelect: [],

    init: function()
    {
        $(document)
            .on('click', '[data-uic], #mozaicPageWorkspace', MBE.selection.select)
        ;

        MBE.DnD.onDOMUpdate.push(MBE.selection._update);
    },

    select: function(event)
    {
        if(typeof event != 'undefined') {
            event.stopImmediatePropagation();
        }

        MBE.workspace.find('.mbe-active').removeClass('mbe-active');
        $('.mbe-drag-handle').remove();
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
                $('body').append(handle);
            }
        }
    },

    _update: function () {
        var elm = $('.mbe-active');
        if (elm.length) {
            $('.mbe-drag-handle').css({
                top: elm.offset().top - 4,
                left: elm.offset().left - 4
            });
        }
    }
};

MBE.onInit.push(MBE.selection.init);