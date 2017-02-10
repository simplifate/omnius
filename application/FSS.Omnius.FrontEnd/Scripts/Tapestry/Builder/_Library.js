TB.library = {

    clean: function () {
        $('.libraryItem').remove();
    },

    createItem: function(target, type, params, name, className, highlighted)
    {
        var itemLibId = ++lastLibId;
        params.libId = itemLibId;
        params.libType = type;
        
        var item = $('<div class="libraryItem"></div>');
        item.attr(params).html(name).appendTo($('#libraryCategory-'+target));
        
        if (className) { item.addClass(className); }
        if (highlighted) { item.addClass('highlighted'); }
        
        return itemLibId;
    }


};
