MBE.path = {
    
    root: null,
    template: '<li><a></a></li>',

    init: function() 
    {
        var self = MBE.path;
        self.root = $('#mozaicPageBreadcrumbs');

        MBE.selection.onSelect.push(MBE.path.update);

        $(document)
            .on('click', '#mozaicPageBreadcrumbs a', self.selectNode)
        ;

    },

    update: function()
    {
        var target = $(this);
        MBE.path.root.find('li:not(:first-child)').remove();

        if(target.is('[data-uic]')) {
            
            target.parentsUntil('#mozaicPageWorkspace').each(function () {
                MBE.path.add(this);
            });
            MBE.path.add(this);
        }
    },

    add: function(elm) {
        var item = $(MBE.path.template);
        item.find('a').html(MBE.getComponentName(elm)).data('targetuic', elm);
        item.appendTo(MBE.path.root);
    },

    selectNode: function(event)
    {
        MBE.selection.select.apply($(this).data('targetuic'), [event]);
    }
};

MBE.onInit.push(MBE.path.init);