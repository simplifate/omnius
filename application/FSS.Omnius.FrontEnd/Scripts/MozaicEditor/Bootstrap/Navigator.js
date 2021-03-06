﻿MBE.navigator = {

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

        if (MBE.workspace.data('navstate') == 'collapsed') {
            root.hide().prev().find('i.fa').toggleClass('fa-caret-down fa-caret-right');
        }
    },

    buildSubNodes: function(node, target) 
    {
        node.children().each(function () {
            var subNode = $(this);
            var item;

            if (subNode.is('[data-uic]')) {
                item = $(MBE.navigator.nodeTemplate);
                var label = item.find('b');

                subNode.data('navitem', label);

                label.html(MBE.getComponentName(this)).data('targetuic', this);

                if (subNode.find('[data-uic]').length) {
                    label.before('<i class="fa fa-caret-down fa-fw"></i>');
                }
                if (subNode.is('[locked]')) {
                    label.after('<span class="fa fa-lock fa-fw"></span>');
                    item.find('.node-handle').attr('draggable', false);
                }
                if (subNode.attr('id') && subNode.attr('id').length) {
                    label.parent().append('<i class="item-id">#' + subNode.attr('id') + '</i>');
                }
                if (subNode.hasClass('mbe-active')) {
                    label.addClass('active');
                }
                target.append(item);

                if (subNode.data('navstate') == 'collapsed') {
                    item.find('i.fa').toggleClass('fa-caret-down fa-caret-right');
                    item.find('.sub-tree').hide();
                }
            }

            MBE.navigator.buildSubNodes(subNode, subNode.is('[data-uic]') ? item.find('.sub-tree') : target);
        });
    },

    toggle: function(event)
    {
        event.stopImmediatePropagation();
        
        $(this).parent().next().slideToggle();
        $(this).toggleClass('fa-caret-right fa-caret-down');
        $($(this).next().data('targetuic')).data('navstate', $(this).hasClass('fa-caret-down') ? 'expanded' : 'collapsed');

        console.log($($(this).next().data('targetuic')));
    },

    selectNode: function(event)
    {
        if (event.stopImmediatePropagation) {
            event.stopImmediatePropagation();
        }
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