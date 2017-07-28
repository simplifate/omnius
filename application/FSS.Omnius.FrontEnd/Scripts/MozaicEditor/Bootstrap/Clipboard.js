MBE.clipboard = {

    init: function () {

    },

    isEmpty: function () {
        var data = $.sessionStorage.get('MBEClipboard');
        return data == null || data.length == 0;
    },

    copy: function () {
        var target = $('.mbe-active', MBE.workspace);
        var tmp = $('<div />');
        var clone = target.clone();
        clone.removeClass('mbe-active context-menu-active');

        tmp.append(clone);

        $.sessionStorage.set('MBEClipboard', tmp.html());
    },

    cut: function () {
        this.copy();
        $('.mbe-active', MBE.workspace).remove();
        MBE.selection.select.apply(MBE.workspace, []);
        MBE.DnD.updateDOM();
    },

    paste: function () {
        var data = $.sessionStorage.get('MBEClipboard');
        $('.mbe-active', MBE.workspace).append(data);
        MBE.DnD.updateDOM();
    }
};

MBE.onInit.push(MBE.clipboard.init);