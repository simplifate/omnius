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
        'custom-code': '<div></div>',
        'modal': '<div class="modal fade" tabindex="-1" role="dialog"></div>',
        'modal-dialog': '<div class="modal-dialog" data-uic="misc|modal-dialog" locked role="document"></div>',
        'modal-content': '<div class="modal-content" data-uic="misc|modal-content" locked></div>',
        'modal-header': '<div class="modal-header" data-uic="misc|modal-header" locked></div>',
        'modal-body': '<div class="modal-body" data-uic="misc|modal-body" locked></div>',
        'modal-footer': '<div class="modal-footer" data-uic="misc|modal-footer" locked></div>',
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
        'breadcrumbs-inactive': '<a data-uic="misc|breadcrumbs-inactive" locked></a>',
        'embed': '<div><div class="embed-code"></div><div class="uic-embed-preview"></div></div>'
    },

    options: {
        'custom-code': {
            'customCodeOptions': {
                name: 'Custom code',
                type: 'cm',
                get: function () {
                    var html = $(this.innerHTML);
                    html.find('.mbe-text-node').contents().unwrap();
                    var helper = $('<div></div>').html(html);
                    return helper.html();
                },
                set: function (opt) {
                    this.innerHTML = opt.value;
                    MBE.DnD.updateDOM();
                }
            }
        },
        'modal': {
            'modalOptions': {
                name: 'Modal options',
                type: 'group',
                groupItems: [{
                    label: 'Size',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'modal-lg': 'Large',
                        'modal-sm': 'Small',
                    },
                    get: function (value) {
                        return $('.modal-dialog', this).hasClass(value);
                    },
                    set: function(opt) {
                        $('.modal-dialog', this).removeClass('modal-sm modal-lg');
                        if (opt.value != 'null') {
                            $('.modal-dialog', this).addClass(opt.value);
                        }
                    }
                }, {
                    label: 'Options',
                    type: 'boolean',
                    options: {
                        'fade': 'Fade',
                        'show-header': 'Show header',
                        'show-footer': 'Show footer'
                    },
                    get: function (value) {
                        switch (value) {
                            case 'fade': return $(this).is('.fade');
                            case 'show-header': return $('.modal-header', this).length > 0;
                            case 'show-footer': return $('.modal-footer', this).length > 0;
                        }
                    },
                    set: function (opt) {
                        var self = MBE.types.misc;

                        switch (opt.value) {
                            case 'fade': 
                                $(this).toggleClass('fade'); 
                                break;
                            case 'show-header':
                                if (opt.checked) {
                                    self.modalCreateHeader().prependTo($('.modal-content', this));            
                                }
                                else {
                                    $('.modal-header', this).remove();
                                }
                                break;
                            case 'show-footer':
                                if (opt.checked) {
                                    self.modalCreateFooter().appendTo($('.modal-content', this));            
                                }
                                else {
                                    $('.modal-footer', this).remove();
                                }
                                break;
                        }

                        MBE.DnD.updateDOM();
                    }
                }]
            }
        },
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
        'breadcrumbs-active': MBE.types.text.options.common,
        'embed': {
            'embedOptions': {
                name: 'Embed options',
                type: 'group',
                groupItems: [{
                    label: 'Code',
                    type: 'cm',
                    get: function () { return $(this).find('.embed-code').text(); },
                    set: function (opt) {
                        $(this).find('.embed-code').text(opt.value);
                        if (opt.value.indexOf('<script') === -1) { // preview of script embed is buggy
                            var preview = $(this).find('.uic-embed-preview');
                            preview[0].innerHTML = opt.value; 
                        }
                        
                        MBE.DnD.updateDOM();
                    }
                }]
            }
        },
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

        menu.misc.modal = {
            items: [
                { type: 'text', label: 'Modal' },
                { type: 'button', label: 'Show', callback: self.modalShow, allowFor: self.modalIsInActive },
                { type: 'button', label: 'Hide', callback: self.modalHide, allowFor: self.modalIsActive }
            ]
        };
        menu.misc['modal-header'] = menu.misc.modal;
        menu.misc['modal-body'] = menu.misc.modal;
        menu.misc['modal-footer'] = menu.misc.modal;
    },

    _drop: function(target)
    {
        if ($(this).is('[data-uic="misc|breadcrumbs"]') && $(this).is(':empty')) {
            MBE.types.misc.buildBreadCrumbs.apply(this, []);
        }

        if ($(this).is('.modal') && $(this).is(':empty')) {
            MBE.types.misc.buildModal.apply(this, []);
        }
    },

    /*****************************************************/
    /* MODAL CONTEXT METHODS                             */
    /*****************************************************/
    buildModal: function() 
    {
        var self = MBE.types.misc;
        var modal = $(this);
        var dialog = $(self.templates['modal-dialog']);
        var content = $(self.templates['modal-content']);
        var body = $(self.templates['modal-body']);
        
        self.modalCreateHeader().appendTo(content);

        body.append('<p data-uic="text|paragraph">The content of your modal</p>');
        body.appendTo(content);

        self.modalCreateFooter().appendTo(content);

        content.appendTo(dialog);
        dialog.appendTo(modal);

        MBE.DnD.updateDOM();
    },

    modalCreateHeader: function() {
        var self = MBE.types.misc;
        var header = $(self.templates['modal-header']);
        header.append('<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>');
        header.append('<h4 class="modal-title" data-uic="text|heading">Modal title</h4>');

        return header;
    },

    modalCreateFooter: function () {
        var self = MBE.types.misc;
        var footer = $(self.templates['modal-footer']);
        footer.append('<button type="button" class="btn btn-default" data-dismiss="modal" data-uic="controls|button">Close</button>');
        footer.append('<button type="button" class="btn btn-primary" data-uic="controls|button">Save changes</button>');

        return footer;
    },

    modalGetTarget: function() {
        return $(this).is('.modal') ? $(this) : $(this).parents('.modal').eq(0);
    },

    modalIsActive: function () { return MBE.types.misc.modalGetTarget.apply(this, []).is('.in'); },
    modalIsInActive: function() { return MBE.types.misc.modalGetTarget.apply(this, []).is(':not(.in)'); },

    modalShow: function() {
        var m = MBE.types.misc.modalGetTarget.apply(this, []);
        m.addClass('in')
         .show()
         .after('<div class="modal-backdrop fade in" bs-hidden="" bs-system-element="" style="display: block;"></div>');
        
        MBE.selection.select.apply(m[0], []);
    },

    modalHide: function() {
        var m = MBE.types.misc.modalGetTarget.apply(this, []);
        m.removeClass('in')
         .hide()
         .next('.modal-backdrop').remove();

        MBE.selection.select.apply(m[0], []);
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