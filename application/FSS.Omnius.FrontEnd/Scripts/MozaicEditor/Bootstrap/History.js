MBE.history = {

    undoStack: [],
    redoStack: [],

    btnUndo: null,
    btnRedo: null,

    init: function() 
    {
        var self = MBE.history;
        self.btnUndo = $('[data-action="undo"]');
        self.btnRedo = $('[data-action="redo"]');

        MBE.DnD.onBeforeDrop.push(self._beforeAction);
        MBE.onBeforeDelete['*'].push(self._beforeAction);

        $(document)
            .on('click', '[data-action="undo"]', self.undo)
            .on('click', '[data-action="redo"]', self.redo)
        ;

        self.setButtonState();
    },

    _beforeAction: function()
    {
        var self = MBE.history;
        self.redoStack = [];
        self.undoStack.unshift(self.getCurrentDOM());

        if (self.undoStack.length > 20) {
            delete self.undoStack[20];
        }

        self.setButtonState();
    },

    getCurrentDOM: function() {
        var dom = MBE.workspace.clone(true);
        dom
            .find('#drop-placeholder').remove().end()
            .find('.drag-over').removeClass('drag-over');

        return dom.contents();
    },

    setButtonState: function() {
        var self = MBE.history;

        self.btnUndo.attr('disabled', self.undoStack.length == 0);
        self.btnRedo.attr('disabled', self.redoStack.length == 0);
    },

    undo: function () {
        var self = MBE.history;
        var dom = self.undoStack.shift();

        self.redoStack.unshift(self.getCurrentDOM());

        MBE.workspace.html('');
        MBE.workspace.append(dom);

        MBE.DnD.updateDOM();
        self.setButtonState();
    },

    redo: function () {
        var self = MBE.history;
        var dom = self.redoStack.shift();

        self.undoStack.unshift(self.getCurrentDOM());

        MBE.workspace.html('');
        MBE.workspace.append(dom);

        MBE.DnD.updateDOM();
        self.setButtonState();
    }
};

MBE.onInit.push(MBE.history.init);