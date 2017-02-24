TB.library = {

    onCreate: [],
    onClean: [],

    clean: function () {
        $('.libraryItem').remove();

        TB.callHooks(TB.library.onClean, null, []);
    },

    createItem: function(target, type, params, name, className, highlighted, originalItem)
    {
        var itemLibId = ++lastLibId;
        params.libId = itemLibId;
        params.libType = type;
        
        var item = $('<div class="libraryItem"></div>');
        item.attr(params).html(name).appendTo($('#libraryCategory-'+target));
        
        if (className) { item.addClass(className); }
        if (highlighted) { item.addClass('highlighted'); }

        TB.callHooks(TB.library.onCreate, originalItem, [type]);
        
        return itemLibId;
    }


};
