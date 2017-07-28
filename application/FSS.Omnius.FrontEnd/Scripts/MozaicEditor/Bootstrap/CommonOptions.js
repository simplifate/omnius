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
        if(event.stopImmediatePropagation)
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
            width: '70%',
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

    createCheckBoxList: function(opt, isCollapsed) {
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
                MBE.selection._update();
            });

            lb.append(ch)
            lb.append(' ' + opt.options[k]);
            group.append(lb);
            group.append('<br>');
        }

        if (isCollapsed) {
            group.css('display', 'none');
        }

        return group;
    },

    createSelect: function(opt, isCollapsed)
    {
        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var group = $('<div class="option-group form-group"' + (opt.id ? ' id="' + opt.id + '"' : '') + '></div>');
        var set = opt.set;

        var lb = $('<label class="control-label col-xs-2">' + opt.label + '</label>');
        var sel = $('<select class="form-control input-sm"></select>');
        for (var k in opt.options) {
            sel.append('<option value="' + k + '"' + (opt.get.apply(MBE.options.target, [k, opt]) ? ' selected' : '') + '>' + opt.options[k] + '</option>');
        };

        if(typeof opt.attr != 'undefined') {
            sel.data('attr', opt.attr);
        }

        sel.on('change', function () {
            set.apply(MBE.options.target, [this]);
            MBE.selection._update();
        });

        group.append(lb);
        group.append(sel);

        sel.wrap('<div class="col-xs-10"></div>');

        if (isCollapsed) {
            group.css('display', 'none');
        }

        return group;
    },

    createText: function(opt, isCollapsed)
    {
        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var group = $('<div class="option-group form-group"' + (opt.id ? ' id="' + opt.id + '"' : '') + '></div>');
        var set = opt.set;

        var lb = $('<label class="control-label col-xs-2">' + opt.label + '</label>');
        var inp = $('<input type="'+opt.type+'" value="' + opt.get(false, opt) + '" class="form-control input-sm">');
        
        if(typeof opt.attr != 'undefined') {
            inp.data('attr', opt.attr);
        }

        inp.on('change', function () {
            set.apply(MBE.options.target, [this]);
            MBE.selection._update();
        });
        if (typeof opt.change == 'function') {
            var onChange = opt.change;
            inp.on('change', function () {
                onChange.apply(MBE.options.target, [this]);
            });
        }

        group.append(lb);
        group.append(inp);

        inp.wrap('<div class="col-xs-10"></div>');

        if (isCollapsed) {
            group.css('display', 'none');
        }

        return group;
    },

    createCM: function (opt, isCollapsed) {
        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var group = $('<div class="option-group form-group"' + (opt.id ? ' id="' + opt.id + '"' : '') + '></div>');
        var set = opt.set;

        var inp = $('<textarea class="form-control" rows="15">' + opt.get.apply(MBE.options.target, [opt]) + '</textarea>');

        inp.on('change', function () {
            set.apply(MBE.options.target, [this]);
        });
        if (typeof opt.change == 'function') {
            var onChange = opt.change;
            inp.on('change', function () {
                onChange.apply(MBE.options.target, [this]);
                MBE.selection._update();
            });
        }

        group.append(inp);

        if (isCollapsed) {
            group.css('display', 'none');
        }

        setTimeout(function () {
            var cm = CodeMirror.fromTextArea(inp[0], {
                lineNumbers: true,
                lineWrapping: true,
                mode: "htmlmixed",
                autoCloseBrackets: true,
                autoCloseTags: true,
                matchBrackets: true,
                matchTags: true,
                extraKeys: {
                    "Ctrl-Space": "autocomplete"
                },
            });
            cm.on('blur', function (instance) {
                cm.save();
                inp.change();
            });
        }, 10);

        return group;
    },

    createIcon: function (opt, isCollapsed)
    {
        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var group = $('<div class="option-group form-group"' + (opt.id ? ' id="' + opt.id + '"' : '') + '></div>');
        var set = opt.set;

        var lb = $('<label class="col-xs-2 control-label">' + opt.label + '</label>');
       
        var active = '';
        var iconList = {};
        var sSheetList = document.styleSheets;

        for (var f in opt.fontSets)
        {
            var list = $('<div class="icon-list col-xs-12 font-set-' + f + '"></div>');

            for (var sSheet = 0; sSheet < sSheetList.length; sSheet++) {
                var ruleList = document.styleSheets[sSheet].cssRules;
                for (var rule = 0; rule < ruleList.length; rule++) {
                    var text = ruleList[rule].selectorText;
                    var selectors = text ? text.split(/, ?/) : [];
                    for (var si = 0; si < selectors.length; si++)
                    {
                        var selector = selectors[si];
                        var rx = new RegExp('^\.(' + f + '-[^ ]+)::before$');
                        if (m = rx.exec(selector)) {
                            var btn = $('<a href="#"></a>');
                            btn.append('<span class="' + f + ' ' + m[1] + '"></span>');
                            btn.append('<span class="icon-name">' + f + ' ' + m[1] + '</span>');
                            list.append(btn);

                            var test = '.' + f + '.' + m[1];
                            if ($(MBE.options.target).is(test)) {
                                btn.addClass('active');
                                active = f;
                            }
                        }
                    }
                }
            }
            list.on('click', 'a', function () {
                set.apply(MBE.options.target, [this]);
                MBE.selection._update();
                return false;
            });
            iconList[f] = list;
        }

        var search = $('<input type="text" value="" placeholder="Find icon..." class="form-control input-sm">');

        search.on('input', function () {
            var text = this.value;
            if (text.length) {
                $('.icon-list').find('a').hide().each(function () {
                    if ($('.icon-name', this).text().indexOf(text) != -1) {
                        $(this).show();
                    }
                });
            }
            else {
                $('.icon-list').find('a').show();
            }
        });

        var sets = $('<select class="form-control input-sm"></select>');

        for (f in opt.fontSets) {
            var option = $('<option></option>');
            option.val(f).html(opt.fontSets[f]).appendTo(sets);

            if (active == f) {
                option.attr('selected', true);
                iconList[f].show();
            }
            else {
                iconList[f].hide();
            }
        }

        sets.change(function () {
            $('[class*="font-set-"]').hide();
            $('.font-set-' + this.value).show();
        });
        
        group.append(lb);
        group.append(sets);
        group.append(search);
        group.append('<div class="clearfix"></div>');
        for (var f in iconList) {
            group.append(iconList[f]);
        }

        sets.wrap('<div class="col-xs-4"></div>');
        search.wrap('<div class="col-xs-6"></div>');

        if (isCollapsed) {
            group.css('display', 'none');
        }
        
        return group;
    },

    createOptions: function(opt)
    {
        var self = MBE.options;
        var isCollapsed = typeof opt.state != 'undefined' && opt.state == 'collapsed';

        if (!MBE.options.isAllowed(opt)) {
            return '';
        }

        var f = $('<fieldset />');
        f.append('<legend><span class="fa fa-caret-'+(isCollapsed ? 'right' : 'down')+' fa-fw"></span>' + opt.name + '</legend>');

        switch (opt.type) {
            case 'boolean': {
                f.append(self.createCheckBoxList(opt, isCollapsed));        
                break;
            }
            case 'select': {
                f.append(self.createSelect(opt, isCollapsed));
                break;
            }
            case 'icon': {
                f.append(self.createIcon(opt, isCollapsed));
                break;
            }
            case 'cm': {
                f.append(self.createCM(opt, isCollapsed));
                break;
            }
            case 'group': {
                for(var i = 0; i < opt.groupItems.length; i++) {
                    var item = opt.groupItems[i];
                    switch(item.type) {
                        case 'boolean': {
                            f.append(self.createCheckBoxList(item, isCollapsed));
                            break;
                        }
                        case 'select': {
                            f.append(self.createSelect(item, isCollapsed));
                            break;
                        }
                        case 'number':
                        case 'text': {
                            f.append(self.createText(item, isCollapsed));
                            break;
                        }
                        case 'icon': {
                            f.append(self.createIcon(item, isCollapsed));
                            break;
                        }
                        case 'cm': {
                            f.append(self.createCM(item, isCollapsed));
                            break;
                        }
                        case 'builder':
                        {
                            f.append(item.builder(item, isCollapsed));
                            break;
                        }
                    }
                }
                break;
            }
            case 'builder':
            {
                f.append(opt.builder(opt, isCollapsed));
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
            if (!opt.value.length) {
                t.removeAttr(attr);
            }
            else  {
                t.attr(attr, opt.value == 'null' ? '' : opt.value);
            }
            
            if (attr == 'id') {
                MBE.navigator.rebuild();
            }
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
        MBE.selection.select.apply(this, []);
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
    },

    setIcon: function (icon) {
        var elm = $(this);
        var currentClass = $('.icon-list a.active .icon-name').text();
        var newClass = $('.icon-name', icon).text();

        elm.removeClass(currentClass).addClass(newClass);
        $('.icon-list .active').removeClass('active');
        $(icon).addClass('active');
    },

    getCustomClasses: function (value, opt) {
        var c = $(MBE.options.target).attr('data-custom-classes');
        return c ? c : '';
    },

    setCustomClasses: function (opt) {
        var currentClasses = $(this).attr('data-custom-classes');
        if (currentClasses) {
            $(this).removeClass(currentClasses);
        }

        $(this).addClass(opt.value).attr('data-custom-classes', opt.value)
    },

    getCustomAttributes: function (value, opt) {
        var t = $(MBE.options.target);
        var customAttributes = t.attr('data-custom-attributes');
        if (customAttributes && customAttributes.length) {
            var data = new Array();
            var attrList = customAttributes.split(/,/g);
            for (var i = 0; i < attrList.length; i++) {
                var attrName = attrList[i];
                var attrValue = t.attr(attrName);
                
                data.push(attrName + '=' + attrValue + '');
            }
            return data.join(';');
        }
        else {
            return '';
        }
    },

    setCustomAttributes: function (opt) {
        var customAttributes = new Array();
        if (opt.value.length) {
            var data = opt.value.split(/; */g);
            for (var i = 0; i < data.length; i++) {
                var pair = data[i].split(/=/);
                $(this).attr(pair[0], pair.length > 1 ? pair[1] : pair[0]);

                customAttributes.push(pair[0]);
            }
        }
        $(this).attr('data-custom-attributes', customAttributes.join(','));
    }
};

// Obecné vlastnosti - musí být na konci kvůli referencím na set metody
MBE.options.common = {
    common: {
        name: 'Common',
        type: 'group',
        groupItems: [{
            label: 'ID',
            type: 'text',
            attr: 'id',
            id: 'AttrID',
            get: MBE.options.hasAttr,
            set: MBE.options.setAttr
        }, {
            label: 'Style',
            type: 'text',
            attr: 'style',
            get: MBE.options.hasAttr,
            set: MBE.options.setAttr
        }, {
            label: 'Tab index',
            type: 'number',
            attr: 'tabindex',
            get: MBE.options.hasAttr,
            set: MBE.options.setAttr
        }, {
            label: 'Custom classes',
            type: 'text',
            get: MBE.options.getCustomClasses,
            set: MBE.options.setCustomClasses
        }, {
            label: 'Custom attributes',
            type: 'text',
            get: MBE.options.getCustomAttributes,
            set: MBE.options.setCustomAttributes
        }, {
            label: 'Build properties',
            type: 'text',
            attr: 'data-properties',
            get: MBE.options.hasAttr,
            set: MBE.options.setAttr
        }]
    },
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
        get: MBE.options.hasClass,
        state: 'collapsed',
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
        get: MBE.options.hasClass,
        state: 'collapsed'
    }
};

MBE.onInit.push(MBE.options.init);