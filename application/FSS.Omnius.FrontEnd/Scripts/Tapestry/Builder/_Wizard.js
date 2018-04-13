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

    variableList: [
        { label: '__Model__', category: 'System' },
        { label: '__ModelId__', category: 'System' },
        { label: '__TableName__', category: 'System' },
        { label: '__CORE__', category: 'System' },
        { label: '__User__', category: 'System' },
    ],

    dataPrefixList: {
        // BOOTSTRAP COMPONENTS
        'ui|nv-list':           ['tableData'],
        'ui|data-table':        ['tableData'],
        'ui|countdown':         ['countdownTargetData'],
        'form|label':           ['inputData'],
        'form|input-text':      ['inputData'],
        'form|input-email':     ['inputData'],
        'form|input-color':     ['inputData'],
        'form|input-tel':       ['inputData'],
        'form|input-date':      ['inputData'],
        'form|input-number':    ['inputData'],
        'form|input-range':     ['inputData'],
        'form|input-hidden':    ['inputData'],
        'form|input-url':       ['inputData'],
        'form|input-search':    ['inputData'],
        'form|input-password':  ['inputData'],
        'form|input-file':      ['inputData'],
        'form|textarea':        ['inputData'],
        'form|checkbox':        ['checkboxData'],
        'form|radio':           ['checkboxData'],
        'form|select':          ['dropdownData', 'dropdownSelection'],
        // LEGACY COMPONENTS
        'checkbox':                 ['checkboxData'],
        'countdown':                ['countdownTargetData'],
        'data-table-read-only':     ['tableData'],
        'data-table-with-actions':  ['tableData'],
        'dropdown-select':          ['dropdownData', 'dropdownSelection'],
        'input-single-line':        ['inputData'],
        'input-multiline':          ['inputData'],
        'label':                    ['inputData'],
        'multiple-select':          ['dropdownData', 'dropdownSelection'],
        'name-value-list':          ['tableData']
    },

    uiTypeList: {},

    target: null,

    init: function () 
    {
        var self = TB.wizard;

        TB.load.onAttributesLoad.push(self._attributesLoad);
        TB.library.onClean.push(self._libraryClean);
        TB.library.onCreate.push(self._libraryCreateItem);
        TB.wfr.onCreateItem.push(self._workflowCreateItem);

        $.widget('custom.catcomplete', $.ui.autocomplete, {
            _create: function () {
                this._super();
                this.widget().menu('option', 'items', '> :not(.ui-autocomplete-category)');
            },
            _renderMenu: function (ul, items) {
                var self = this;
                var w = TB.wizard;
                var currentCategory = '';
                var requestedCategory;
                var requestedTable;
                var requestedView;
                var requestedWorkflow = w.getRequestedWorkflow();
                
                var type = $(this.element).parents('.form-group').find('select[name=var_type]').val();

                if (type) {
                    switch (type) {
                        case '_var_': requestedCategory = '*'; break;
                        case 's$': requestedCategory = w.getRequestedCategory.apply(this.element, []); break;
                        default: requestedCategory = ''; break;
                    }
                }
                else {
                    requestedCategory = w.getRequestedCategory.apply(this.element, []);
                }

                if (requestedCategory == 'Column') {
                    requestedTable = w.getRequestedTable.apply(this.element, []);
                    requestedView = w.getRequestedView.apply(this.element, []);
                }

                $.each(items, function () {
                    var li;
                    if (requestedCategory == '*' ||
                        requestedCategory == this.category ||
                        requestedCategory.indexOf(this.category) !== -1 ||
                        (requestedCategory.indexOf('Workflow') !== -1 && this.category.indexOf('Workflow') !== -1) ||
                        (requestedCategory == 'Column' && ((this.category == 'Tables' && requestedTable) || (this.category == 'Views' && requestedView)))
                       ) {

                        if (this.category != currentCategory &&
                            (this.category.indexOf('Workflow') === -1 || this.category == 'Workflow: ' + requestedWorkflow) &&
                            (requestedCategory != 'Column' || (this.category == 'Tables' && requestedTable) || (this.category == 'Views' && requestedView))
                           ) {
                            ul.append('<li class="ui-autocomplete-category bg-info">' + this.category + '</li>');
                            currentCategory = this.category;
                        }

                        if (requestedCategory == 'Tables' && this.label.indexOf('.') !== -1) return;
                        if (requestedCategory == 'Column' && this.category == 'Tables' && (!requestedTable || this.label.indexOf(requestedTable + '.') === -1)) return;
                        if (requestedCategory == 'Column' && this.category == 'Views' && (!requestedView || this.label.indexOf(requestedView + '.') === -1)) return;
                        if (this.category.indexOf('Workflow') !== -1 && this.category != 'Workflow: ' + requestedWorkflow) return;

                        li = self._renderItemData(ul, this);
                    }
                });
            }
        });
    },

    open: function()
    {
        var self = TB.wizard;
        var target = $(this);
        var action = self.actions[target.attr('actionId')];
        var form = $('<form class="form-horizontal" onsubmit="return false"></form>');
        var inputVarsValues = self.parseInputValue(target);
        var outputVarsValues = self.parseOutputValue(target);
        var knownInputVars = [];
        var knownOutputVars = [];

        self.target = target;

        var d = $('<div title="Průvodce parametry akce \'{action_name}\'"></div>');
        d.attr('title', d.attr('title').replace(/\{action_name\}/, action.name));
        d.append(form);

        d.dialog({
            autoOpen: false,
            width: '50%',
            draggable: true,
            resizable: true,
            appendTo: 'body',
            dialogClass: 'dialog-wizard',
            buttons: [{
                text: 'Použít',
                click: TB.wizard.validate
            }],
            create: function () {
                var btn = $(this).parents('.ui-dialog').find('.ui-dialog-buttonset button');
                btn.addClass('btn btn-primary').prepend('<span class="fa fa-check"></span> ');
            },
            close: function () {
                $(this).remove();
            }
        });

        if (action.inputVars.length || action.outputVars.length || inputVarsValues.length || outputVarsValues.length) 
        {
            var iSet = $('<fieldset class="inputVars"><legend>Input vars</legend></fieldset>');
            var oSet = $('<fieldset class="outputVars"><legend>Output vars</legend></fieldset>');

            iSet.appendTo(form);
            oSet.appendTo(form);

            if (action.inputVars.length || inputVarsValues.length) {
                for (var i = 0; i < action.inputVars.length; i++) {
                    var inputVar = action.inputVars[i];
                    if (!inputVar.isArray) {
                        self.createInputVar(inputVar, iSet, inputVarsValues.items);
                        knownInputVars.push(inputVar.name);
                    }
                    else {
                        var matchFound = false;
                        var rx = new RegExp('^' + inputVar.name + '\\[(\\d+)\\]$');
                        for (k in inputVarsValues.items) {
                            if (m = k.match(rx)) {
                                self.createInputVar(inputVar, iSet, inputVarsValues.items, m[1]);
                                knownInputVars.push(m[0]);
                                matchFound = true;
                            }
                        }

                        if (!matchFound) {
                            self.createInputVar(inputVar, iSet, inputVarsValues.items, 0);
                            knownInputVars.push(inputVar.name + '[0]');
                        }
                    }
                }

                for (k in inputVarsValues.items) {
                    if ($.inArray(k, knownInputVars) == -1) {
                        self.createInputVar({
                            required: false,
                            type: null,
                            name: k,
                            isArray: false,
                            isEnum: false,
                            enumItems: [],
                            unknown: true
                        }, iSet, inputVarsValues.items);
                    }
                }
            }
            else {
                iSet.append('<div class="form-group no-vars"><div class="col-xs-12"><p class="alert alert-info">Tato akce nemá žádné vstupní parametry</p></div></div>');
            }

            if (action.outputVars.length || outputVarsValues.length) {
                for (var i = 0; i < action.outputVars.length; i++) {
                    self.createOutputVar(action.outputVars[i], oSet, outputVarsValues.items);
                    knownOutputVars.push(action.outputVars[i].name);
                }

                for (k in outputVarsValues.items) {
                    if ($.inArray(k, knownOutputVars) == -1) {
                        self.createOutputVar({
                            required: false,
                            type: null,
                            name: k,
                            isArray: false,
                            isEnum: false,
                            enumItems: [],
                            unknown: true
                        }, oSet, outputVarsValues.items);
                    }
                }
            }
            else {
                oSet.append('<div class="form-group no-vars"><div class="col-xs-12"><p class="alert alert-info">Tato akce nemá žádné výstupní parametry</p></div></div>');
            }
        }
        else {
            form.html('<div class="form-group no-vars"><div class="col-xs-12"><p class="alert alert-info">Tato akce nemá žádné vstupní ani výstupní parametry</p></div></div>');
        }

        d.dialog('open');
    },

    validate: function()
    {
        var d = $(this);
        var isValid = true;

        d.find('fieldset.inputVars > .form-group:not(.no-vars)').each(function () {
            if ($(this).data('required') == 'true') {
                var value = $(this).find('input[name=var_value], select[name=var_value]').not(':disabled').val();
                isValid = isValid && value.length > 0;
            }

            /*if ($(this).find('select[name=var_type]').val() == '_var_') {
                var value = $(this).find('input[name=var_value]').val();
                if (value.length) {
                    var exists = false;
                    var requestedWorkflow = TB.wizard.getRequestedWorkflow();

                    $.each(TB.wizard.variableList, function () {

                    });
                }
            }*/
        });

        if (isValid) {
            TB.wizard.build.apply(this, []);
        }
        else {
            var confirm = $('<div title="Jste si jistí?"><p class="text-nowrap">Nemáte vyplněné všechny povinné proměnné.<br><b>Opravdu chcete parametry uložit?</b></p></div>');
            var context = this;

            confirm.dialog({
                autoOpen: true,
                width: 450,
                draggable: false,
                resizable: false,
                modal: true,
                appendTo: 'body',
                dialogClass: 'dialog-wizard-confirm',
                buttons: [{
                    text: 'Ano',
                    click: function () {
                        TB.wizard.build.apply(context);
                        $(this).dialog('close');
                    }
                }, {
                    text: 'Ne',
                    click: function() {
                        $(this).dialog('close');
                    }
                }],
                create: function () {
                    var buttons = $(this).parents('.ui-dialog').find('.ui-dialog-buttonset button');
                    buttons.eq(0).addClass('btn btn-success pull-right').prepend('<span class="fa fa-check"></span> ');
                    buttons.eq(1).addClass('btn btn-default').prepend('<span class="fa fa-times"></span> ');

                    $(this).parents('.ui-dialog').find('.ui-dialog-buttonset').css('float', 'none');
                },
                close: function () {
                    $(this).remove();
                }
            });
        }
    },

    build: function()
    {
        var d = $(this);

        var inputVars = [];
        var outputVars = [];

        d.find('fieldset.inputVars > .form-group:not(.no-vars)').each(function () {
            var variable = $(this).find('.control-label').data('invar');
            var index = $(this).find('.control-label').data('index');
            var dataType = $(this).find('select[name=var_type]').val();
            var value = $(this).find('input[name=var_value], select[name=var_value]').not(':disabled').val();
            if (value.length) {
                inputVars.push(variable + (typeof index != 'undefined' ? '[' + index + ']' : '') + '=' + (dataType == '_var_' ? '' : dataType) + value);
            }
        });
        d.find('fieldset.outputVars > .form-group:not(.no-vars)').each(function () {
            var variable = $(this).find('.input-group-addon:last-child').data('outvar');
            var value = $(this).find('input[name=out_value]').val();

            if (value.length) {
                outputVars.push(value + '=' + variable);
            }
        });

        TB.wizard.target.data('inputVariables', inputVars.join(';'));
        TB.wizard.target.data('outputVariables', outputVars.join(';'));
        TB.wizard.rebuildWorkflowVars();
        TB.wizard.target = null;

        d.dialog('close');
    },

    rebuildWorkflowVars: function()
    {
        var self = TB.wizard;
        var workflowName = self.getRequestedWorkflow();

        var newVars = [];
        $.each(self.variableList, function () {
            if (this.category != 'Workflow: ' + workflowName) {
                newVars.push(this);
            }
        });

        self.target.parents('.swimlaneArea').find('.actionItem').each(function () {
            var outputVars = self.parseOutputValue($(this));
            if (outputVars.length > 0) {
                for(var k in outputVars.items) {
                    newVars.push({
                        label: outputVars.items[k],
                        category: 'Workflow: ' + workflowName
                    });
                }
            }

        });

        self.variableList = newVars;
        self.variableList.sort(self.sort);
    },

    /******************************************************/
    /* TOOLS                                              */
    /******************************************************/
    parseVars: function(source) {
        var vars = [];
        if (source) {
            for (var i = 0; i < source.length; i++) {
                var m = source[i].match(/^(\?)?([a-z]\$)?([a-zA-Z0-9_]+)(\[(index)?\])?(\[([^\]]*)\])?$/i);
                // 1 = ?    = volitelná?
                // 2 = s$   = typ
                // 3 = název
                // 4 = pole
                // 6 = enum
                // 7 = enum items

                vars.push({
                    required: m[1] == '?' ? false : true,
                    type: m[2],
                    name: m[3],
                    isArray: m[4] ? true : false,
                    isEnum: m[6] && (m[7] && m[7] != 'index') ? true : false,
                    enumItems: (m[7] && m[7] != 'index') ? m[7].split('|') : [],
                    unknown: false
                });
            }
        }
        return vars;
    },

    parseInputValue: function(target) {
        var data = target.data('inputVariables');
        var values = {
            length: 0,
            items: {}
        };

        if (data) {
            var list = data.split(/;/);
            for (var i = 0; i < list.length; i++) {
                var pair = list[i].split(/=/);
                var m = pair[1].match(/^([a-z]\$)?(.+)/);

                values.length++;
                values.items[pair[0]] = {
                    dataType: m[1],
                    value: m[2]
                };
            }
        }

        return values;
    },

    parseOutputValue: function(target) {
        var data = target.data('outputVariables');
        var values = {
            length: 0,
            items: {}
        };
        
        if (data) {
            var list = data.split(/;/);
            for (var i = 0; i < list.length; i++) {
                var pair = list[i].split(/=/);
                values.length++;
                values.items[pair[1]] = pair[0];
            }
        }

        return values;
    },

    createInputVar: function(inputVar, target, values, index) {
        var group = $('<div class="form-group form-group-sm" />');
        var typeWrapper = $('<div class="col-md-2 col-sm-6" />');
        var valueWrapper = $('<div class="col-md-7 col-sm-6" />');
        var valueInputGroup = $('<div class="input-group" />');
        var label = $('<label class="control-label col-md-3" data-invar="' + inputVar.name + '">' + inputVar.name + (inputVar.isArray ? '[' + index + ']' : '') + ' =</label>');
        var type = $('<select name="var_type" class="form-control"></select>');
        var value = $('<input type="text" name="var_value" class="form-control" />');
        var enumValue = $('<select name="var_value" class="form-control enum-value"></select>');
        var booleanValue = $('<select name="var_value" class="form-control boolean-value"></select>');
        var addOn = $('<div class="input-group-addon"></div>');

        group.data('required', inputVar.required ? 'true' : 'false');

        label.appendTo(group);
        typeWrapper.appendTo(group);
        valueWrapper.appendTo(group);

        type.appendTo(typeWrapper);

        valueInputGroup.appendTo(valueWrapper);
        addOn.appendTo(valueInputGroup);
        value.appendTo(valueInputGroup);
        booleanValue.appendTo(valueInputGroup);

        group.appendTo(target);

        booleanValue.append('<option value="True">True</option>');
        booleanValue.append('<option value="False">False</option>');

        if (inputVar.isEnum) {
            valueInputGroup.append(enumValue);
            for (var i = 0; i < inputVar.enumItems.length; i++) {
                enumValue.append('<option value="' + inputVar.enumItems[i] + '">' + inputVar.enumItems[i] + '</option>');
            }

            value.attr('disabled', true).hide();
        }

        if (inputVar.isArray) {
            var addOn2 = $('<div class="input-group-addon"></div>');
            var add = $('<span class="fa fa-plus fa-fw"></span>');
            var del = $('<span class="fa fa-times fa-fw"></span>');

            addOn2.appendTo(valueInputGroup);
            add.appendTo(addOn2);
            del.appendTo(addOn2);

            add.click(TB.wizard._addArrayItem);
            del.click(TB.wizard._deleteArrayItem);

            label.data('index', index);
        }

        if (inputVar.required) {
            addOn.append('<span class="fa fa-asterisk fa-fw"></span>');
        }
        else {
            if (inputVar.unknown) {
                addOn.append('<span class="fa fa-warning fa-fw" title="Neočekávaná vstupní proměnná"></span>');

                var addOn2 = $('<div class="input-group-addon"></div>');
                var del = $('<span class="fa fa-times fa-fw"></span>');

                addOn2.appendTo(valueInputGroup);
                del.appendTo(addOn2);

                del.click(TB.wizard._deleteUnexpectedVar);
            }
            else {
                addOn.append('<span class="fa fa-question fa-fw"></span>');
            }
        }

        type.data('hasEnum', inputVar.isEnum);
        type.change(TB.wizard._changeType); 

        for (var k in TB.wizard.dataTypeList) {
            var opt = $('<option value="' + k + '">' + TB.wizard.dataTypeList[k] + '</option>');
            opt.attr('disabled', inputVar.type && k != inputVar.type && k != '_var_');
            
            type.append(opt);
        }

        var key = inputVar.isArray ? inputVar.name + '[' + index + ']' : inputVar.name;
        if (values[key]) {
            var v = values[key];

            type.val(v.dataType ? v.dataType : '_var_');
            if (inputVar.isEnum && type.val() != '_var_') {
                enumValue.val(v.value);
            }
            else {
                if (type.val() == 'b$') {
                    var b = v.value.toLowerCase() == 'true' ? 'True' : 'False';
                    booleanValue.val(b);
                    value.attr('disabled', true).hide();
                }
                else {
                    value.val(v.value);
                }
            }
        }

        booleanValue.attr('disabled', type.val() != 'b$').toggle(type.val() == 'b$');

        TB.wizard.setInputVarAutocomplete(value);

        type.change();
    },

    createOutputVar: function (outputVar, target, values) {
        var group = $('<div class="form-group form-group-sm" />');
        var wrapper = $('<div class="col-xs-12"></div>');
        var valueWrapper = $('<div class="input-group" />');
        var value = $('<input type="text" name="out_value" class="form-control text-right" />');
        var addOn = $('<div class="input-group-addon" data-outvar="' + outputVar.name + '">= ' + outputVar.name + '</div>');

        wrapper.appendTo(group);
        valueWrapper.appendTo(wrapper);
        value.appendTo(valueWrapper);
        addOn.appendTo(valueWrapper);

        if (values[outputVar.name]) {
            value.val(values[outputVar.name]);
        }

        if (outputVar.unknown) {
            var del = $('<span class="fa fa-times fa-fw" style="margin-left: 7px" title="Smazat"></span>');

            valueWrapper.prepend('<div class="input-group-addon"><span class="fa fa-warning fa-fw" title="Neočekávaná výstupní proměnná"></span></div>');
            addOn.append(del);
            del.click(TB.wizard._deleteUnexpectedVar);
        }

        group.appendTo(target);

        value.catcomplete({
            delay: 0,
            source: TB.wizard.variableList
        });
    },

    loadViewColumns: function(viewName) {
        $.ajax({
            url: '/api/database/apps/' + $('#currentAppId').val() + '/viewscheme/' + viewName,
            type: 'post',
            dataType: 'json',
            success: function (data) {
                var self = TB.wizard;

                for (var i = 0; i < data.Columns.length; i++) {
                    self.variableList.push({
                        label: viewName + '.[' + data.Columns[i] + ']',
                        category: 'Views'
                    });
                }
            }
        });
    }, 

    sort: function (a, b) {
        var sortKeyA = a.category + '_' + a.label;
        var sortKeyB = b.category + '_' + b.label;
        return sortKeyA.localeCompare(sortKeyB);
    },

    getRequestedCategory: function()
    {
        var inVar = $(this).parents('.form-group').find('.control-label').data('invar');
        var outVar = $(this).parents('.form-group').find('.input-group-addon:last-child').data('outvar');

        if (inVar) {
            switch (inVar) {
                case 'TableName': return 'Tables';
                case 'ViewName': return 'Views';
            }
            if (inVar.indexOf('Column') !== -1) return 'Column';
        }
        if (outVar) {
            return 'System|Workflow|UI input data';
        }
        
        return '';
    },

    getRequestedWorkflow: function()
    {
        return TB.wizard.target.parents('.workflowRule').find('.workflowRuleHeader .verticalLabel').text();
    },

    getRequestedTable: function() {
        var tableRow = $(this).parents('.inputVars').find('.control-label[data-invar=TableName]').parent();
        if(tableRow.find('select[name=var_type]').val() == 's$') {
            return tableRow.find('input[name=var_value]').val();
        }
        return '';
    },

    getRequestedView: function() {
        var viewRow = $(this).parents('.inputVars').find('.control-label[data-invar=ViewName]').parent();
        if(viewRow.find('select[name=var_type]').val() == 's$') {
            return viewRow.find('input[name=var_value]').val();
        }
        return '';
    },

    setInputVarAutocomplete: function(target) {
        target.catcomplete({
            delay: 0,
            source: TB.wizard.variableList,
            search: TB.wizard._autocompleteSearch,
            select: TB.wizard._autocompleteSelect
        });
    },

    /******************************************************/
    /* EVENT CALLBACKS                                    */
    /******************************************************/
    _libraryClean: function () {
        TB.wizard.actions = {};
    },

    _libraryCreateItem: function (type) {
        var self = TB.wizard;

        switch(type) {
            case 'action':
                self.actions[this.Id] = {
                    name: this.Name,
                    inputVars: self.parseVars(this.InputVars),
                    outputVars: self.parseVars(this.OutputVars)
                }
                break;
            case 'ui':
                if(typeof this.ElmId != 'undefined' || typeof this.ComponentName != 'undefined') {
                    self.variableList.push({
                        label: typeof this.ElmId != 'undefined' ? this.ElmId : this.ComponentName,
                        category: 'UI elements'
                    });

                    if (typeof this.UIC != 'undefined' && typeof self.dataPrefixList[this.UIC] != 'undefined') {
                        for (var i = 0; i < self.dataPrefixList[this.UIC].length; i++) {
                            self.variableList.push({
                                label: '_uic_' + self.dataPrefixList[this.UIC][i] + '_' + this.ElmId,
                                category: 'UI input data'
                            });
                        }
                    }
                    if (typeof this.ComponentName != 'undefined' && typeof self.dataPrefixList[this.Type] != 'undefined') {
                        for (var i = 0; i < self.dataPrefixList[this.Type].length; i++) {
                            self.variableList.push({
                                label: '_uic_' + self.dataPrefixList[this.Type][i] + '_' + this.ComponentName,
                                category: 'UI input data'
                            });
                        }
                    }

                    self.variableList.sort(self.sort);
                }
                break;
        }
    },

    _attributesLoad: function(data) {
        var self = TB.wizard;

        for (var ti = 0; ti < data.Tables.length; ti++) {
            self.variableList.push({
                label: data.Tables[ti].Name,
                category: 'Tables'
            });
            self.variableList.push({
                label: '__Model.' + data.Tables[ti].Name,
                category: 'System'
            });

            if (data.Tables[ti].Columns.length) {
                for (var ci = 0; ci < data.Tables[ti].Columns.length; ci++) {
                    self.variableList.push({
                        label: data.Tables[ti].Name + '.' + data.Tables[ti].Columns[ci].Name,
                        category: 'Tables'
                    });
                    self.variableList.push({
                        label: '__Model.' + data.Tables[ti].Name + '.' + data.Tables[ti].Columns[ci].Name,
                        category: 'System'
                    });
                }
            }
        }
        for (var vi = 0; vi < data.Views.length; vi++) {
            self.variableList.push({
                label: data.Views[vi].Name,
                category: 'Views'
            });
            self.loadViewColumns(data.Views[vi].Name);
        }
        
        // Shared tables & views
        if (data.Shared != null) {
            for (var ti = 0; ti < data.Shared.Tables.length; ti++) {
                self.variableList.push({
                    label: data.Shared.Tables[ti].Name,
                    category: 'Tables'
                });
                self.variableList.push({
                    label: '__Model.' + data.Shared.Tables[ti].Name,
                    category: 'System'
                });

                if (data.Shared.Tables[ti].Columns.length) {
                    for (var ci = 0; ci < data.Shared.Tables[ti].Columns.length; ci++) {
                        self.variableList.push({
                            label: data.Shared.Tables[ti].Name + '.' + data.Shared.Tables[ti].Columns[ci].Name,
                            category: 'Tables'
                        });
                        self.variableList.push({
                            label: '__Model.' + data.Shared.Tables[ti].Name + '.' + data.Shared.Tables[ti].Columns[ci].Name,
                            category: 'System'
                        });
                    }
                }
            }

            for (var vi = 0; vi < data.Shared.Views.length; vi++) {
                self.variableList.push({
                    label: data.Shared.Views[vi].Name,
                    category: 'Views'
                });
            }
        }

        self.variableList.sort(self.sort);
    },

    _workflowCreateItem: function() {
        var self = TB.wizard;
        if (this.attr('actionId') && this.attr('actionId').length && this.data('outputVariables')) {
            var values = TB.wizard.parseOutputValue(this);
            if (values.length) {
                for (var k in values.items) {
                    self.variableList.push({
                        label: values.items[k],
                        category: 'Workflow: ' + this.parents('.workflowRule').find('.workflowRuleHeader .verticalLabel').text()
                    });
                }
                self.variableList.sort(self.sort);
            }
        }
    },

    _changeType: function () {
        var hasEnum = $(this).data('hasEnum');

        $(this).parents('.form-group')
            .find('input[name=var_value]').attr('disabled', (this.value != '_var_' && hasEnum) || this.value == 'b$').toggle((this.value == '_var_' || !hasEnum) && this.value != 'b$').end()
            .find('select.enum-value').attr('disabled', this.value == '_var_' || !hasEnum).toggle(this.value != '_var_' && hasEnum).end()
            .find('select.boolean-value').attr('disabled', this.value != 'b$').toggle(this.value == 'b$');

        if (this.value == 'f$' || this.value == 'i$') {
            $(this).parents('.form-group').find('input[name=var_value]').attr('type', 'number');
        }
        else {
            $(this).parents('.form-group').find('input[name=var_value]').attr('type', 'text');
        }
    },

    _deleteUnexpectedVar: function () {
        $(this).parents('.form-group').remove();
    },

    _autocompleteSearch: function (event, ui) {
        var requestedCategory;
        var type = $(this).parents('.form-group').find('select[name=var_type]').val();

        switch (type) {
            case '_var_': requestedCategory = '*'; break;
            case 's$': requestedCategory = TB.wizard.getRequestedCategory.apply(this, []); break;
            default: requestedCategory = ''; break;
        }

        if (requestedCategory == '') {
            return false;
        }
    },

    _autocompleteSelect: function (event, ui) {
        var requestedCategory = TB.wizard.getRequestedCategory.apply(this, []);
        
        if (requestedCategory == 'Column') {
            this.value = ui.item.value.replace(/^[^\.]+\./, '');
            return false;
        }
    },

    _addArrayItem: function () {
        var group = $(this).parents('.form-group');
        var label = group.find('.control-label');
        var inputVar = label.data('invar');

        var lastGroup = $('.control-label[data-invar=' + inputVar + ']').last().parents('.form-group');
        var lastIndex = Number(lastGroup.find('.control-label').data('index'));

        lastGroup.find('input[name=var_value]').catcomplete('destroy');

        var newIndex = lastIndex + 1;
        var newGroup = lastGroup.clone(true);
        var newLabel = newGroup.find('.control-label')
        
        newLabel.data('index', newIndex).html(newLabel.html().replace(/\[\d+\]/, '[' + newIndex + ']'));
        newGroup.find('[name=var_value]').val('');
        newGroup.insertAfter(lastGroup);

        TB.wizard.setInputVarAutocomplete(newGroup.find('input[name=var_value]'));
        TB.wizard.setInputVarAutocomplete(lastGroup.find('input[name=var_value]'));
    },

    _deleteArrayItem: function () {
        var group = $(this).parents('.form-group');
        var label = group.find('.control-label');
        var inputVar = label.data('invar');

        group.remove();

        $('.control-label[data-invar=' + inputVar + ']').each(function (index) {
            $(this).data('index', index).html(this.innerHTML.replace(/\[\d+\]/, '[' + index + ']'));
        });
    }
};

TB.onInit.push(TB.wizard.init);