MBE.options = {

    target: null,
    targetType: null,
    targetTemplate: null,

    init: function() {
        $(document).on('click', '.dialog-options legend', function () {
            $('.fa', this).toggleClass('fa-caret-down fa-caret-right');
            $(this).nextAll().toggle();
        });
    },
    
    openDialog: function(event)
    {
        event.stopImmediatePropagation();

        var self = MBE.options;
        self.target = this;

        var uic = $(this).data('uic').split(/\|/);
        self.targetType = uic[0];
        self.targetTemplate = uic[1];

        var d = $('<div />');
        var onBuild = [];

        if (typeof MBE.types[self.targetType].options[self.targetTemplate] != 'undefined') {
            var optSet = MBE.types[self.targetType].options[self.targetTemplate];
            for (var opt in optSet) {
                d.append(self.createOptions(optSet[opt]));
                if (typeof optSet[opt].onBuild == 'function') {
                    onBuild.push(optSet[opt].onBuild);
                }
            }
        }

        if (typeof MBE.types[self.targetType].options.common != 'undefined') {
            var optSet = MBE.types[self.targetType].options.common;
            for (var opt in optSet) {
                d.append(self.createOptions(optSet[opt]));
            }
        }
        
        for(var opt in self.common) {
            d.append(self.createOptions(self.common[opt]));
        }
        
        d.dialog({
            appendTo: 'body',
            modal: true,
            closeOnEscape: true,
            draggable: false,
            resizable: false,
            width: '40%',
            maxHeight: '90%',
            title: 'Options',
            dialogClass: 'dialog-options',
            close: function () { $(this).remove(); }
        });

        for(var i = 0; i < onBuild.length; i++) {
            onBuild[i]();
        }
    },

    isAllowed: function(opt) {
        if (typeof opt.allowFor != 'undefined' && $.inArray(MBE.options.targetTemplate, opt.allowFor) == -1) {
            return false;
        }
        if (typeof opt.disallowFor != 'undefined') {
            if (typeof opt.disallowFor == 'function') {
                return opt.disallowFor.apply(MBE.options.target, []);
            }
            else if ($.inArray(MBE.options.targetTemplate, opt.disallowFor) != -1) {
                return false;
            }
        }
        return true;
    },

    createCheckBoxList: function(opt) {
        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var group = $('<div class="option-group"' + (opt.id ? ' id="' + opt.id + '"' : '') + '></div>');
        var set = opt.set;

        for (var k in opt.options) {
            var ch = $('<input type="checkbox" value="' + k + '"' + (opt.get.apply(MBE.options.target, [k, opt]) ? ' checked' : '' ) + '>');
            var lb = $('<label />');

            ch.on('click', function () {
                set.apply(MBE.options.target, [this]);
            });

            lb.append(ch)
            lb.append(' ' + opt.options[k]);
            group.append(lb);
            group.append('<br>');
        }
        return group;
    },

    createSelect: function(opt)
    {
        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var group = $('<div class="option-group"' + (opt.id ? ' id="' + opt.id + '"' : '') + '></div>');
        var set = opt.set;

        var lb = $('<label>' + opt.label + '</label>');
        var sel = $('<select></select>');
        for (var k in opt.options) {
            sel.append('<option value="' + k + '"' + (opt.get(k, opt) ? ' selected' : '') + '>' + opt.options[k] + '</option>');
        };

        if(typeof opt.attr != 'undefined') {
            sel.data('attr', opt.attr);
        }

        sel.on('change', function () {
            set.apply(MBE.options.target, [this]);
        });

        group.append(lb);
        group.append(sel);
        return group;
    },

    createText: function(opt)
    {
        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var group = $('<div class="option-group"' + (opt.id ? ' id="' + opt.id + '"' : '') + '></div>');
        var set = opt.set;

        var lb = $('<label>' + opt.label + '</label>');
        var inp = $('<input type="'+opt.type+'" value="' + opt.get(false, opt) + '">');
        
        if(typeof opt.attr != 'undefined') {
            inp.data('attr', opt.attr);
        }

        inp.on('change', function () {
            set.apply(MBE.options.target, [this]);
        });
        if (typeof opt.change == 'function') {
            var onChange = opt.change;
            inp.on('change', function () {
                onChange.apply(MBE.options.target, [this]);
            });
        }

        group.append(lb);
        group.append(inp);
        return group;
    },

    createOptions: function(opt)
    {
        var self = MBE.options;

        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var f = $('<fieldset />');
        f.append('<legend><span class="fa fa-caret-down fa-fw"></span>' + opt.name + '</legend>');

        switch (opt.type) {
            case 'boolean': {
                f.append(self.createCheckBoxList(opt));        
                break;
            }
            case 'select': {
                f.append(self.createSelect(opt));
                break;
            }
            case 'group': {
                for(var i = 0; i < opt.groupItems.length; i++) {
                    var item = opt.groupItems[i];
                    switch(item.type) {
                        case 'boolean': {
                            f.append(self.createCheckBoxList(item));
                            break;
                        }
                        case 'select': {
                            f.append(self.createSelect(item));
                            break;
                        }
                        case 'number':
                        case 'text': {
                            f.append(self.createText(item));
                            break;
                        }
                    }
                }
                break;
            }
        }
        return f;
    },

    hasClass: function(value) {
        return $(MBE.options.target).hasClass(value);
    },

    is: function(tagName) {
        return $(MBE.options.target).is(tagName);
    },

    hasAttr: function (value, opt) {
        var t = $(MBE.options.target);
        if (value !== false) {
            return opt.attr ? t.attr(opt.attr) == value : t.is('[' + value + ']');
        }
        else {
            return t.is('[' + opt.attr + ']') ? t.attr(opt.attr) : '';
        }
    },

    setAttr: function(opt) {
        var attr = $(opt).data('attr');
        var t = $(MBE.options.target);
        if (attr) {
            t.attr(attr, opt.value == 'null' ? '' : opt.value);
        }
        else { // Je to checkbox
            if(opt.checked) {
                t.attr(opt.value, opt.value);
            }
            else {
                t.removeAttr(opt.value);
            }
        }
    },

    hasProp: function (value, opt) {
        var t = $(MBE.options.target);
        if (value !== false) {
            return t.is(':' + value);
        }
        else {
            return t.is(':' + opt.attr);
        }
    },

    setProp: function (opt) {
        var attr = $(opt).data('attr');
        var t = $(MBE.options.target);
        if (attr) {
            t.prop(attr, opt.value == 'null' ? '' : opt.value);
        }
        else { // Je to checkbox
            t.prop(opt.value, opt.checked);
        }
    },

    toggleClass: function (opt) {
        if (opt.tagName.toLowerCase() == 'select') {
            var classes = new Array();
            $(opt).find('option').each(function() {
                classes.push(this.value);
            });
            $(this).removeClass(classes.join(' '));
            if(opt.value == 'null') {
                return;
            }
        }

        $(this).toggleClass(opt.value);
    },

    toggleTagName: function (opt) {
        var newTag = $('<' + opt.value + '></' + opt.value + '>');
        var l = this.attributes.length;

        for (var i = 0; i < l; i++) {
            var nodeName = this.attributes.item(i).nodeName;
            var nodeValue = this.attributes.item(i).nodeValue;

            newTag[0].setAttribute(nodeName, nodeValue);
        }
        newTag.html(this.innerHTML);

        $(this).replaceWith(newTag);
        MBE.options.target = newTag[0];
        MBE.DnD.updateDOM();
    }
};

// Obecné vlastnosti - musí být na konci kvůli referencím na set metody
MBE.options.common = {
    visibility: {
        name: 'Responsive visibility',
        disallowFor: ['input-hidden'],
        type: 'boolean',
        options: {
            'visible-xs-block': 'visible-xs-block',
            'visible-sm-block': 'visible-sm-block',
            'visible-md-block': 'visible-md-block',
            'visible-lg-block': 'visible-lg-block',
            'visible-xs-inline': 'visible-xs-inline',
            'visible-sm-inline': 'visible-sm-inline',
            'visible-md-inline': 'visible-md-inline',
            'visible-lg-inline': 'visible-lg-inline',
            'visible-xs-inline-block': 'visible-xs-inline-block',
            'visible-sm-inline-block': 'visible-sm-inline-block',
            'visible-md-inline-block': 'visible-md-inline-block',
            'visible-lg-inline-block': 'visible-lg-inline-block',
            'hidden-xs': 'hidden-xs',
            'hidden-sm': 'hidden-sm',
            'hidden-md': 'hidden-md',
            'hidden-lg': 'hidden-lg'
        },
        set: MBE.options.toggleClass,
        get: MBE.options.hasClass
    },

    accessibility: {
        name: 'Accessibility',
        disallowFor: ['input-hidden'],
        type: 'boolean',
        options: {
            'show': 'show',
            'hidden': 'hidden',
            'sr-only': 'sr-only'
        },
        set: MBE.options.toggleClass,
        get: MBE.options.hasClass
    }
};

MBE.onInit.push(MBE.options.init);