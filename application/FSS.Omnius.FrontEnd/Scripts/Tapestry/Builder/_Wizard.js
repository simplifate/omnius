TB.wizard = {

    actions: {},
    dataTypeList: {
        's$': 'string',
        'b$': 'boolean',
        'i$': 'integer',
        'f$': 'float',
        'd$': 'datetime',
        '_var_': 'variable'
    },

    init: function () 
    {
        var self = TB.wizard;

        TB.library.onClean.push(self._libraryClean);
        TB.library.onCreate.push(self._libraryCreateItem);
    },

    open: function()
    {
        var self = TB.wizard;
        var target = $(this);
        var action = self.actions[target.attr('actionId')];
        var form = $('<form class="form-horizontal" onsubmit="return false"></form>');

        if (action.inputVars.length || action.outputVars.length) 
        {
            var iSet = $('<fieldset><legend>Input vars</legend></fieldset>');
            var oSet = $('<fieldset><legend>Output vars</legend></fieldset>');

            if (action.inputVars.length) {
                for (var i = 0; i < action.inputVars.length; i++) {
                    self.createInputVar(action.inputVars[i], iSet);
                }
            }
            else {
                iSet.append('<div class="form-group"><div class="col-xs-12"><p class="alert alert-info">Tato akce nemá žádné vstupní parametry</p></div></div>');
            }

            if (action.outputVars.length) {
                for (var i = 0; i < action.outputVars.length; i++) {
                    self.createOutputVar(action.outputVars[i], oSet);
                }
            }
            else {
                oSet.append('<div class="form-group"><div class="col-xs-12"><p class="alert alert-info">Tato akce nemá žádné výstupní parametry</p></div></div>');
            }

            iSet.appendTo(form);
            oSet.appendTo(form);
        }
        else {
            form.html('<div class="form-group"><div class="col-xs-12"><p class="alert alert-info">Tato akce nemá žádné vstupní ani výstupní parametry</p></div></div>');
        }

        var d = $('<div title="Průvodce parametry akce \'{action_name}\'"></div>');
        d.attr('title', d.attr('title').replace(/\{action_name\}/, action.name));
        d.append(form);

        d.dialog({
            autoOpen: true,
            width: '50%',
            draggable: true,
            resizable: true,
            appendTo: 'body',
            dialogClass: 'dialog-wizard',
            buttons: [{
                text: 'Použít',
                click: TB.wizard.apply
            }], 
            create: function() {
                var btn = $(this).parents('.ui-dialog').find('.ui-dialog-buttonset button');
                btn.addClass('btn btn-primary').prepend('<span class="fa fa-check"></span> ');
            }
        });
    },

    /******************************************************/
    /* TOOLS                                              */
    /******************************************************/
    parseVars: function(source) {
        var vars = [];
        if (source) {
            for (var i = 0; i < source.length; i++) {
                var m = source[i].match(/^(\?)?([a-z]\$)?([a-z0-9]+)(\[([^\]]*)\])?$/i);
                // 1 = ?    = volitelná?
                // 2 = s$   = typ
                // 3 = název
                // 4 = pole nebo enum
                // 5 = enum items

                vars.push({
                    required: m[1] == '?' ? false : true,
                    type: m[2],
                    name: m[3],
                    isArray: m[4] && !m[5] ? true : false,
                    isEnum: m[4] && m[5] ? true : false,
                    enumItems: m[5] ? m[5].split('|') : []
                });
            }
        }
        return vars;
    },

    createInputVar: function(inputVar, target) {
        var group = $('<div class="form-group form-group-sm" />');
        var typeWrapper = $('<div class="col-md-2 col-sm-6" />');
        var valueWrapper = $('<div class="col-md-7 col-sm-6" />');
        var label = $('<label class="control-label col-md-3" data-invar="' + inputVar.name + '">' + inputVar.name + ' =</label>');
        var type = $('<select name="var_type" class="form-control"></select>');
        var value = $('<input type="text" name="var_value" class="form-control" />');
        var enumValue = $('<select name="var_value" class="form-control"></select>');

        label.appendTo(group);
        typeWrapper.appendTo(group);
        valueWrapper.appendTo(group);

        type.appendTo(typeWrapper);
        value.appendTo(valueWrapper);

        group.appendTo(target);

        if (inputVar.isEnum) {
            valueWrapper.append(enumValue);
            for (var i = 0; i < inputVar.enumItems.length; i++) {
                enumValue.append('<option value="' + inputVar.enumItems[i] + '">' + inputVar.enumItems[i] + '</option>');
            }

            value.attr('disabled', true).hide();
            type.change(function () {
                $(this).parents('.form-group')
                    .find('input[name=var_value]').attr('disabled', this.value != '_var_').toggle(this.value == '_var_').end()
                    .find('select[name=var_value]').attr('disabled', this.value == '_var_').toggle(this.value != '_var_');
            });
        }

        for (var k in TB.wizard.dataTypeList) {
            var opt = $('<option value="' + k + '">' + TB.wizard.dataTypeList[k] + '</option>');
            opt.attr('disabled', inputVar.type && k != inputVar.type && k != '_var_');
            
            type.append(opt);
        }
    },

    createOutputVar: function (outputVar, target) {
        var group = $('<div class="form-group form-group-sm" />');
        var wrapper = $('<div class="col-xs-12"></div>');
        var valueWrapper = $('<div class="input-group" />');
        var value = $('<input type="text" name="out_value" class="form-control text-right" />');
        var addOn = $('<div class="input-group-addon" data-outvar="' + outputVar.name + '">= ' + outputVar.name + '</div>');

        wrapper.appendTo(group);
        valueWrapper.appendTo(wrapper);
        value.appendTo(valueWrapper);
        addOn.appendTo(valueWrapper);

        group.appendTo(target);
    },


    /******************************************************/
    /* EVENT CALLBACKS                                    */
    /******************************************************/
    _libraryClean: function () {
        TB.wizard.actions = {};
    },

    _libraryCreateItem: function (type) {
        if (type == 'action') {
            var self = TB.wizard;
            self.actions[this.Id] = {
                name: this.Name,
                inputVars: self.parseVars(this.InputVars),
                outputVars: self.parseVars(this.OutputVars)
            }
        }
    }

};

TB.onInit.push(TB.wizard.init);