TB.toolbox = {

    clean: function () {
        $('.tapestryToolbox .toolboxLi').remove();
    },

    createItem: function (libId, itemSuffix, divClass, divAttr, label) {
        var item = $('<li class="toolboxLi"><div class="toolboxItem"><span class="itemLabel"></span></div></li>');
        item.attr('libId', libId).addClass('toolboxLi_' + itemSuffix)
            .find('> div').addClass(divClass).attr(divAttr)
            .find('.itemLabel').html(label);

        var items = $('.toolboxCategoryHeader_' + itemSuffix).nextUntil('[class^=toolboxCategoryHeader]');
        var target = items.length ? items.last() : $('.toolboxCategoryHeader_' + itemSuffix);

        target.after(item);
        TB.toolbox.alive(item);
    },

    alive: function(item)
    {
        item.find('.toolboxItem').draggable({
            helper: 'clone',
            appendTo: '#tapestryWorkspace',
            containment: 'window',
            tolerance: 'fit',
            revert: true,
            scroll: true,
            start: function () {
                dragModeActive = true;
            }
        });
    }
};
