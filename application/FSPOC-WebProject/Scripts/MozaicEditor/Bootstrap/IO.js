MBE.io = {

    allComponents: null,

    convert: function(data)
    {
        if (!data.Id) {
            return;
        }
        MBE.io.allComponents = new Array();
        MBE.workspace.html('');
        
        var container = $(MBE.types.containers.templates['container']);
        container.attr('data-uic', 'containers|container').appendTo(MBE.workspace);

        var form = $(MBE.types.form.templates.form);
        form.attr('data-uic', 'form|form').appendTo(container);

        

        for (var i = 0; i < data.Components.length; i++) {
            MBE.io.convertComponent(form, data.Components[i]);
        }

        // Pokusíme se naskládat inputy ke správným labelům
        console.log(MBE.io.allComponents);

        for (var i = 0; i < MBE.io.allComponents.length; i++) {
            var c1 = MBE.io.allComponents[i];
            if (c1.item.is('[data-uic="form|label"]')) {
                var mostLikelyInput = null;

                c1.PositionX = parseInt(c1.PositionX);
                c1.PositionY = parseInt(c1.PositionY);
                c1.Width = parseInt(c1.Width);

                for (var j = 0; j < MBE.io.allComponents.length; j++) 
                {
                    var c2 = MBE.io.allComponents[j];
                    
                    if (c2.item.is('input, textarea, select')) 
                    {   
                        c2.PositionX = parseInt(c2.PositionX);
                        c2.PositionY = parseInt(c2.PositionY);

                        if (c1.PositionY + 5 >= c2.PositionY && c1.PositionY - 5 <= c2.PositionY) {
                            if (mostLikelyInput == null) {
                                if (c2.PositionX > (c1.PositionX + c1.Width)) {
                                    mostLikelyInput = c2;
                                }
                            }
                            else {
                                if (c2.PositionX < mostLikelyInput.PositionX && c2.PositionX > (c1.PositionX + c1.Width)) {
                                    mostLikelyInput = c2;
                                }
                            }
                        }
                    }
                }

                console.log(c1);
                console.log(mostLikelyInput);

                if(mostLikelyInput != null) 
                {
                    c1.item.next().append(mostLikelyInput.item);
                    c1.item.attr('for', mostLikelyInput.Name);
                }
            }
        }
        
        $("#currentPageId").val(data.Id);
        $("#headerPageName").text(data.Name);

        MBE.DnD.updateDOM();
    },

    convertComponent: function (targetContainer, c)
    {
        var item;
        var newTarget;
        var systemClasses = [
            'panel-component', 'named-panel', 'control-label', 'dropdown-select', 'input-single-line', 'button-browse',
            'input-multiline', 'uic', 'button-simple', 'button-large', 'button-small', 'button-extra-small'
        ];

        switch(c.Tag.toLowerCase())
        {
            case 'div':
                // PANEL
                if (c.Classes.indexOf('panel-component') !== -1) {
                    item = $(MBE.types.containers.templates.panel);
                    item
                        .find('.panel-body').html('').end()
                        .find('.panel-footer').remove().end()
                        .attr('data-uic', 'containers|panel');

                    if (c.Name) {
                        item.attr('id', c.Name);
                    }
                    if (c.Classes.indexOf('named-panel') === -1) {
                        item.find('.panel-heading').remove();
                    }
                    else {
                        item.find('.panel-title').html(c.Label);
                    }
                    newTarget = item.find('.panel-body');
                }
                if (c.Classes.indexOf('control-label') !== -1) {
                    item = $(MBE.types.form.templates.label);
                    item
                        .html(c.Label)
                        .attr('data-uic', 'form|label')
                        .addClass('col-sm-2');
                    item.wrap('<div class="form-group" data-uic="form|form-group"></div>');
                    item.after('<div class="col-sm-10" data-uic="grid|column"></div>');
                }
                break;
            case 'select':
                item = $(MBE.types.form.templates.select);
                item.attr({
                    'data-uic': 'form|select',
                    'name': c.Name
                });

                if (c.Properties.indexOf('defaultoption') !== -1)
                {
                    var defaultOption = MBE.io.getProperty('defaultoption', c.Properties);
                    item.append('<option value="">' + defaultOption + '</option>');
                }

                break;
            case 'input':
                if (c.Classes.indexOf('input-single-line') !== -1)
                {
                    item = $(MBE.types.form.templates['input-text']);
                    item.attr('data-uic', 'form|input-text');
                }
                if (c.Classes.indexOf('button-browse') !== -1) {
                    item = $(MBE.types.form.templates['input-file']);
                    item.attr('data-uic', 'form|input-file');
                }
                item.attr({
                    'name': c.Name
                });
                break;
            case 'textarea':
                item = $(MBE.types.form.templates.textarea);
                item.attr({
                    'data-uic': 'form|textarea',
                    'name': c.Name,
                    'rows': 5
                });
                break;
            case 'button':
                item = $(MBE.types.controls.templates.button);
                item.attr({
                    'data-uic': 'controls|button',
                    'type': 'submit',
                    'name': c.Name
                }).html(c.Label).addClass('btn-primary');

                if (c.Classes.indexOf('button-large') !== -1) {
                    item.addClass('btn-lg');
                }
                if (c.Classes.indexOf('button-small') !== -1) {
                    item.addClass('btn-sm');
                }
                if (c.Classes.indexOf('button-extra-small') !== -1) {
                    item.addClass('btn-xs');
                }
                break;
            case 'table':

                break;
        }

        if (item)
        {
            var systemClassesRegExp = new RegExp(systemClasses.join('|'), 'g');
            var customClass = c.Classes.replace(systemClassesRegExp, '');
            customClass = customClass.replace(/ {2,}/, ' ').replace(/(^ )|( $)/g, '');

            if (customClass.length) {
                item.attr('data-custom-classes', customClass).addClass(customClass);
            }
            if (c.Placeholder) {
                item.attr("placeholder", c.Placeholder);
            }
            if (c.TabIndex) {
                item.attr("tabindex", c.TabIndex);
            }
            if (c.Attributes) {
                var customAttributes = [];
                var fake = $('<span ' + c.Attributes + '></span>')[0];

                for (var i = 0; i < fake.attributes.length; i++) {
                    var attr = fake.attributes[i];
                    customAttributes.push(attr.name);

                    item.attr(attr.name, attr.value);
                }

                item.attr('data-custom-attributes', customAttributes.join(','));
            }

            item.attr('id', c.Name);

            if (c.Classes.indexOf('control-label') != -1) {
                item.parent().appendTo(targetContainer);
            }
            else {
                item.appendTo(targetContainer);
            }
        }

        if (c.ChildComponents) {
            for (j = 0; j < c.ChildComponents.length; j++) {
                MBE.io.convertComponent(newTarget ? newTarget : item, c.ChildComponents[j]);
            }
        }

        c.item = item;
        MBE.io.allComponents.push(c);


        /*
        
        newComponent = $('<' + cData.Tag + ' id="' + cData.Id + '" uicName="' + cData.Name + /*'" uicAttributes="' + (cData.Attributes || "") + /'" class="uic ' + cData.Classes
                    + '" uicClasses="' + cData.Classes + '" uicStyles="' + cData.Styles + '" style="left: ' + cData.PositionX + '; top: ' + cData.PositionY + '; width: '
                    + cData.Width + '; height: ' + cData.Height + '; ' + cData.Styles + '"></' + cData.Tag + '>');
    newComponent.data("uicAttributes", cData.Attributes);

    targetContainer.append(newComponent);
    
   
    if (cData.Properties)
        newComponent.attr("uicProperties", cData.Properties);
    else if (newComponent.hasClass("button-dropdown"))
        newComponent.html(cData.Label + '<i class="fa fa-caret-down"></i>');
    else if (newComponent.hasClass("info-container")) {
        newComponent.append($('<div class="fa fa-info-circle info-container-icon"></div>'
            + '<div class="info-container-header"></div>'
            + '<div class="info-container-body"></div>'));
        newComponent.find(".info-container-header").text(cData.Label);
        newComponent.find(".info-container-body").text(cData.Content);
    }
    else if (newComponent.hasClass("multiple-select")) {
        newComponent.append($('<option value="1">Multiple</option><option value="2">Choice</option><option value="3">Select</option>'));
        newComponent.attr("multiple", "");
    }
    else if (newComponent.hasClass("form-heading") || newComponent.hasClass("control-label")) {
        newComponent.html(cData.Label);
        newComponent.attr("contentTemplate", cData.Content);
    }
    else if (newComponent.hasClass("checkbox-control")) {
        newComponent.append($('<input type="checkbox" /><span class="checkbox-label">' + cData.Label + '</span>'));
    }
    else if (newComponent.hasClass("radio-control")) {
        newComponent.append($('<input type="radio" name="' + cData.Name + '" /><span class="radio-label">' + cData.Label + '</span>'));
    }
    else if (newComponent.hasClass("breadcrumb-navigation")) {
        newComponent.append($('<div class="app-icon fa fa-question"></div><div class="nav-text">APP NAME &gt; Nav</div>'));
    }
    else if (newComponent.hasClass("data-table")) {
        newComponent.append($('<thead><tr><th>Column 1</th><th>Column 2</th><th>Column 3</th></tr></thead>'
            + '<tbody><tr><td>Value1</td><td>Value2</td><td>Value3</td></tr><tr><td>Value4</td><td>Value5</td><td>Value6</td></tr>'
            + '<tr><td>Value7</td><td>Value8</td><td>Value9</td></tr></tbody>'));
        CreateCzechDataTable(newComponent, newComponent.hasClass("data-table-simple-mode"));
        newComponent.css("width", cData.Width);
        wrapper = newComponent.parents(".dataTables_wrapper");
        wrapper.css("position", "absolute");
        wrapper.css("left", cData.PositionX);
        wrapper.css("top", cData.PositionY);
        newComponent.css("position", "relative");
        newComponent.css("left", "0px");
        newComponent.css("top", "0px");
    }
    else if (newComponent.hasClass("name-value-list")) {
        newComponent.append($('<tr><td class="name-cell">Platform</td><td class="value-cell">Omnius</td></tr><tr><td class="name-cell">Country</td>'
            + '<td class="value-cell">Czech Republic</td></tr><tr><td class="name-cell">Year</td><td class="value-cell">2016</td></tr>'));
    }
    else if (newComponent.hasClass("tab-navigation")) {
        tabLabelArray = cData.Content.split(";");
        newComponent.append($('<li class="active"><a class="fa fa-home"></a></li>'));
        for (k = 0; k < tabLabelArray.length; k++) {
            if (tabLabelArray[k].length > 0)
                newComponent.append($("<li><a>" + tabLabelArray[k] + "</a></li>"));
        }
        newComponent.css("width", "auto");
    }
    else if (newComponent.hasClass("color-picker")) {
        CreateColorPicker(newComponent);
        newReplacer = targetContainer.find(".sp-replacer:last");
        newReplacer.css("position", "absolute");
        newReplacer.css("left", newComponent.css("left"));
        newReplacer.css("top", newComponent.css("top"));
        newComponent.removeClass("uic");
        newReplacer.addClass("uic color-picker");
        newReplacer.attr("uicClasses", "color-picker");
        newReplacer.attr("uicName", newComponent.attr("uicName"));
    }
    else if (newComponent.hasClass("countdown-component")) {
        newComponent.html('<span class="countdown-row countdown-show3"><span class="countdown-section"><span class="countdown-amount">0</span>'
            + '<span class="countdown-period">Hodin</span></span><span class="countdown-section"><span class="countdown-amount">29</span>'
            + '<span class="countdown-period">Minut</span></span><span class="countdown-section"><span class="countdown-amount">59</span>'
            + '<span class="countdown-period">Sekund</span></span></span>');
    }
    else if (newComponent.hasClass("wizard-phases")) {
        newComponent.html(WizardPhasesContentTemplate);
        var phaseLabelArray = cData.Content.split(";");
        newComponent.find(".phase1 .phase-label").text(phaseLabelArray[0] ? phaseLabelArray[0] : "Fáze 1");
        newComponent.find(".phase2 .phase-label").text(phaseLabelArray[1] ? phaseLabelArray[1] : "Fáze 2");
        newComponent.find(".phase3 .phase-label").text(phaseLabelArray[2] ? phaseLabelArray[2] : "Fáze 3");
    }

    if (newComponent.hasClass("panel-component")) { //mšebela: odstraněno else před if (kvůli named-component)
        CreateDroppableMozaicContainer(newComponent, false);
    }
    if (newComponent.hasClass("data-table"))
        draggableElement = wrapper;
    else if (newComponent.hasClass("color-picker"))
        draggableElement = newReplacer;
    else
        draggableElement = newComponent;
    draggableElement.draggable({
        cancel: false,
        containment: "parent",
        drag: function (event, ui) {
            if (GridResolution > 0) {
                ui.position.left -= (ui.position.left % GridResolution);
                ui.position.top -= (ui.position.top % GridResolution);
            }
        }
    });
    */
    },

    getProperty: function(name, properties)
    {
        var propList = properties.split(/; */g);
        for (var i = 0; i < propList.length; i++) {
            var pair = propList[i].split(/=/);
            if (pair[0] == name) {
                return pair[1];
            }
        }
    }
}