MBE.options.getProgressBarClass = function (value) {
    return $('.progress-bar', this).hasClass(value);
};
MBE.options.setProgressBarClass = function (opt) {
    if (opt.type == 'select') {
        var classes = new Array();
        $(opt).find('option').each(function () {
            classes.push(this.value);
        });
        $('.progress-bar', this).removeClass(classes.join(' '));
        if (opt.value == 'null') {
            return;
        }
    }
    $('.progress-bar', this).toggleClass(opt.value);
};

MBE.types.misc = {

    templates: {
        'badge': '<span class="badge">4</span>',
        'tag': '<span class="label label-default">label</span>',
        'caret': '<span class="caret"></span>',
        'close': '<button type="button" class="close" aria-label="Close"><span aria-hidden="true">&times;</span></button>',
        'hr': '<hr />',
        'responsive-embed': '<div class="embed-responsive embed-responsive-16by9"><iframe class="embed-responsive-item" src=""></iframe></div>',
        'progressBar': '<div class="progress">' + 
                            '<div class="progress-bar" role="progressbar" style="min-width:2em; width:0%"></div>' +
                        '</div>',
        'breadcrumbs': '<ol class="breadcrumb"></ol>',
        'breadcrumbs-item': '<li data-uic="misc|breadcrumbs-item"></li>',
        'breadcrumbs-active': '<span data-uic="misc|breadcrumbs-active" locked></span>',
        'breadcrumbs-inactive': '<a data-uic="misc|breadcrumbs-inactive" locked></a>'
    },

    options: {
        'tag': {
            'tagOptions': {
                name: 'Label options',
                type: 'group',
                groupItems: [{
                    label: 'Style',
                    type: 'select',
                    options: {
                        'label-default': 'Default',
                        'label-primary': 'Primary',
                        'label-success': 'Success',
                        'label-info': 'Info',
                        'label-warning': 'Warning',
                        'label-danger': 'Danger',
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'progressBar': {
            'progressBarOptions': {
                name: 'Progress bar options',
                type: 'group',
                groupItems: [{
                    label: 'Percentage',
                    type: 'number',
                    get: function () {
                        return parseInt($('.progress-bar', this).css('width'));
                    },
                    set: function (opt) {
                        $('.progress-bar', this).css('width', opt.value + '%');
                        if ($('.progress-bar', this).text().length > 0) {
                            $('.progress-bar', this).html(opt.value + '%');
                        }
                    }
                }, {
                    label: 'Style',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'progress-bar-success': 'Success',
                        'progress-bar-info': 'Info',
                        'progress-bar-warning': 'Warning',
                        'progress-bar-danger': 'Danger'
                    },
                    get: MBE.options.getProgressBarClass,
                    set: MBE.options.setProgressBarClass
                }, {
                    label: 'Options',
                    type: 'boolean',
                    options: {
                        'progress-bar-striped': 'Striped',
                        'active': 'Animated'
                    },
                    get: MBE.options.getProgressBarClass,
                    set: MBE.options.setProgressBarClass
                }, {
                    label: 'Show label',
                    type: 'boolean',
                    options: {
                        'show-label': 'Show label'
                    },
                    get: function () {
                        return $('.progress-bar', this).text().length > 0;
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            $('.progress-bar', this).html($('.progress-bar', this)[0].style.width);
                        }
                        else {
                            $('.progress-bar', this).html('');
                        }
                    }
                }]
            }
        },
        'responsive-embed': {
            'responsiveEmbedOptions': {
                name: 'Responsive embed options',
                type: 'group',
                groupItems: [{
                    label: 'Source URL',
                    type: 'text',
                    get: function () {
                        return $('> iframe', MBE.options.target).attr('src').length ? $('> iframe', MBE.options.target).attr('src') : '';
                    },
                    set: function (opt) {
                        $('> iframe', this).attr('src', opt.value);
                    }
                }, {
                    label: 'Aspect ratio',
                    type: 'select',
                    options: {
                        'embed-responsive-16by9': '16 by 9',
                        'embed-responsive-4by3': '4 by 3'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'breadcrumbs-item': {
            'breadcrumbsItemOptions': {
                name: 'Breadcrumbs item options',
                type: 'boolean',
                options: {
                    'active': 'Active'
                },
                get: MBE.options.hasClass,
                set: function (opt) {
                    $(this).toggleClass(opt.value);
                    var item = $(this).find('a[locked], span[locked]').eq(0);
                    if (opt.checked) {
                        var span = $(MBE.types.misc.templates['breadcrumbs-active']);
                        span.html(item.html());
                        item.replaceWith(span);
                    }
                    else {
                        var link = $(MBE.types.misc.templates['breadcrumbs-inactive']);
                        link.html(item.html());
                        item.replaceWith(link);
                    }
                    MBE.DnD.updateDOM();
                }
            }
        },
        'breadcrumbs-inactive': MBE.types.controls.options.link,
        'breadcrumbs-active': MBE.types.text.options.common
    },

    init: function() 
    {
        var self = MBE.types.misc;
        var menu = MBE.toolbar.menu;

        MBE.DnD.onDrop.push(self._drop);

        menu.misc = {};
        menu.misc.breadcrumbs = {
            items: [
                { type: 'text', label: 'Add item' },
                { type: 'button', label: 'BEFORE', callback: self.bcAddBefore, allowFor: self.bcIsItemSelected },
                { type: 'button', label: 'AFTER', callback: self.bcAddAfter, allowFor: self.bcIsItemSelected },
                { type: 'button', label: 'BEGIN', callback: self.bcAddToBegin },
                { type: 'button', label: 'END', callback: self.bcAddToEnd },
                { type: 'text', label: 'Item' },
                { type: 'button', label: 'ACTIVATE', callback: self.bcActivate, allowFor: self.bcIsInactiveItemSelected },
                { type: 'button', label: 'DEACTIVATE', callback: self.bcDeactivate, allowFor: self.bcIsActiveItemSelected },
                { type: 'button', label: 'DELETE', callback: self.bcDelete, allowFor: self.bcIsItemSelected }
            ]
        }
        menu.misc['breadcrumbs-item'] = menu.misc.breadcrumbs;
        menu.misc['breadcrumbs-active'] = menu.misc.breadcrumbs;
        menu.misc['breadcrumbs-inactive'] = menu.misc.breadcrumbs;
    },

    _drop: function(target)
    {
        if ($(this).is('[data-uic="misc|breadcrumbs"]') && $(this).is(':empty')) {
            MBE.types.misc.buildBreadCrumbs.apply(this, []);
        }
    },

    /*****************************************************/
    /* BREAD CRUMBS CONTEXT METHODS                      */
    /*****************************************************/
    buildBreadCrumbs: function ()
    {
        var self = MBE.types.misc;
        var elm = $(this);
        var linkNames = ['Home', 'Library', 'Data'];

        for (var i = 0; i < linkNames.length; i++) {
            var link = $(MBE.types.misc.templates['breadcrumbs-inactive']);
            var item = $(self.templates['breadcrumbs-item']);

            link.html(linkNames[i]).appendTo(item);
            elm.append(item);
        }
    },

    bcGetItem: function() {
        return $(this).is('[data-uic="misc|breadcrumbs-item"]') ? $(this) : $(this).parents('[data-uic="misc|breadcrumbs-item"]').eq(0);
    },

    bcIsItemSelected: function () {
        return $(this).is('[data-uic="misc|breadcrumbs-item"]') || $(this).parent().is('[data-uic="misc|breadcrumbs-item"]');
    },

    bcIsActiveItemSelected: function () {
        return $(this).is('.active[data-uic="misc|breadcrumbs-item"]') || $(this).parent().is('.active[data-uic="misc|breadcrumbs-item"]');
    },

    bcIsInactiveItemSelected: function () {
        return $(this).is('[data-uic="misc|breadcrumbs-item"]:not(.active)') || $(this).parent().is('[data-uic="misc|breadcrumbs-item"]:not(.active)');
    },

    bcDelete: function () {
        var target = MBE.types.misc.bcGetItem.apply(this, []);
        if (target.length) {
            target.remove();
            $('.mbe-drag-handle').remove();
            MBE.DnD.updateDOM();
        }
    },

    bcToggleState: function (state) {
        var self = MBE.types.misc;
        var item = self.bcGetItem.apply(this, []);
        var target = $('> a, > span', item);
        var replace = $(self.templates['breadcrumbs-' + state]);

        replace.html(target.html());
        target.replaceWith(replace);

        item.toggleClass('active');
        MBE.DnD.updateDOM();

        MBE.selection.select.apply(item.is('.mbe-active') ? item[0] : replace[0], []);
    },

    bcAdd: function(pos) {
        var self = MBE.types.misc;
        var target = $(this).is('.breadcrumb') ? $(this) : $(this).parents('.breadcrumb').eq(0);
        var item = $(self.templates['breadcrumbs-item']);
        var link = $(self.templates['breadcrumbs-inactive']);

        link.html('Item').appendTo(item);
        switch (pos) {
            case 'before': self.bcGetItem.apply(this).before(item); break;
            case 'after': self.bcGetItem.apply(this).after(item); break;
            case 'begin': target.prepend(item); break;
            case 'end': target.append(item); break;
        }
        MBE.DnD.updateDOM();
    },

    bcActivate: function () { MBE.types.misc.bcToggleState.apply(this, ['active']); },
    bcDeactivate: function () { MBE.types.misc.bcToggleState.apply(this, ['inactive']); },

    bcAddBefore: function () { MBE.types.misc.bcAdd.apply(this, ['before']); },
    bcAddAfter: function () { MBE.types.misc.bcAdd.apply(this, ['after']); },
    bcAddToBegin: function () { MBE.types.misc.bcAdd.apply(this, ['begin']); },
    bcAddToEnd: function() { MBE.types.misc.bcAdd.apply(this, ['end']); },
}

MBE.onInit.push(MBE.types.misc.init);