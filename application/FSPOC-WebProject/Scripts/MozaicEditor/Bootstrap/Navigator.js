MBE.navigator = {

    navBase: null,
    nodeTemplate: '<div class="node"><span class="node-handle" draggable="true"><b></b></span><div class="sub-tree"></div></div>',

    init: function()
    {
        var self = MBE.navigator;

        MBE.DnD.onDOMUpdate.push(self.rebuild);
        MBE.DnD.onDragEnter.push(self.addHighlight);
        MBE.DnD.onDragLeave.push(self.removeHighlight);
        MBE.DnD.onDragEnd.push(self.unHighlight);

        MBE.selection.onSelect.push(self._select);

        self.navBase = $('.tree-nav > .node');
        self.navBase.find('b').data('targetuic', MBE.workspace[0]);
        MBE.workspace.data('navitem', self.navBase.find('b'));

        $(document)
            .on('click', '.tree-nav .fa', self.toggle)
            .on('click', '.tree-nav b', self.selectNode)
            .on('dblclick', '.tree-nav b', self.showOptions)
        ;
    },

    rebuild: function()
    {
        var self = MBE.navigator;

        var root = self.navBase.find('> .sub-tree');
        root.html('');
        
        self.buildSubNodes(MBE.workspace, root);
    },

    buildSubNodes: function(node, target) 
    {
        node.find('> [data-uic]').each(function () {
            var item = $(MBE.navigator.nodeTemplate);
            var label = item.find('b');
            var subNode = $(this);
            
            subNode.data('navitem', label);
            
            label.html(MBE.getComponentName(this)).data('targetuic', this);

            if (subNode.find('[data-uic]').length) {
                label.before('<i class="fa fa-caret-down fa-fw"></i>');
            }
            target.append(item);

            MBE.navigator.buildSubNodes(subNode, item.find('.sub-tree'));
        });
    },

    toggle: function(event)
    {
        event.stopImmediatePropagation();
        
        $(this).parent().next().slideToggle();
        $(this).toggleClass('fa-caret-right fa-caret-down');
    },

    selectNode: function(event)
    {
        event.stopImmediatePropagation();
        MBE.selection.select.apply($(this).data('targetuic'), [event]);
    },

    addHighlight: function()
    {
        var target = $(this);
        if(target.data('navitem')) {
            target.data('navitem').addClass('drop-target');
        }
    },

    removeHighlight: function()
    {
        var target = $(this);
        if(target.data('navitem')) {
            target.data('navitem').removeClass('drop-target');
        }
    },

    unHighlight: function()
    {
        MBE.navigator.navBase.find('.drop-target').removeClass('drop-target');
    },

    showOptions: function(event)
    {
        MBE.navigator.selectNode.apply(this, [event]);
        MBE.options.openDialog.apply($(this).data('targetuic'), [event]);
    },

    _select: function()
    {
        MBE.navigator.navBase.find('b').removeClass('active');
        var target = $(this);
        if(target.is('.mbe-active')) {
            target.data('navitem').addClass('active');
        }
    }
};

MBE.onInit.push(MBE.navigator.init);