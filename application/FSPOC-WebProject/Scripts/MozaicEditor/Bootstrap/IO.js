MBE.io = {

    allComponents: null,

    loadPageList: function () {
        $('#page-table tbody tr').remove();
        $('#choose-page-dialog .spinner-2').show();

        var appId = $('#currentAppId').val();
        $.ajax({
            type: "GET",
            url: "/api/mozaic-bootstrap/apps/" + appId + "/pages",
            dataType: "json",
            error: MBE.ajaxError,
            success: function (data) {
                var tbody = $('#page-table tbody');
                for (i = 0; i < data.length; i++) {
                    var row = $('<tr class="pageRow">');
                    row.attr({
                        'data-id': data[i].Id,
                        'data-isbootstrap': data[i].IsBootstrap
                    });
                    row.append('<td width="95%">' + data[i].Name + '</td>');
                    row.append('<td width="5%" align="center">' + (data[i].IsBootstrap ? 'yes' : 'no') + '</td>');
                    row.appendTo(tbody);
                }

                $('#choose-page-dialog .spinner-2').hide();
            }
        });
    },

    loadDeletedPageList: function () {
        $('#trash-page-table tbody tr').remove();
        $('#trash-page-dialog .spinner-2').show();

        var appId = $("#currentAppId").val();
        $.ajax({
            type: "GET",
            url: "/api/mozaic-bootstrap/apps/" + appId + "/deletedPages",
            dataType: "json",
            error: MBE.ajaxError,
            success: function (data) {
                var tbody = $('#trash-page-table tbody');
                for (i = 0; i < data.length; i++) {
                    var row = $('<tr class="pageRow"></tr>');
                    row.attr({
                        'data-id': data[i].Id,
                        'data-isbootstrap': data[i].IsBootstrap
                    });
                    row.append('<td width="95%">' + data[i].Name + '</td>');
                    row.append('<td width="5%" align="center">' + (data[i].IsBootstrap ? 'yes' : 'no') + '</td>');
                    row.appendTo(tbody);
                }
                $("#trash-page-dialog .spinner-2").hide();
            }
        });
    },

    loadPage: function () {
        var selected = $('#page-table tr.highlightedRow');
        if (selected.length) {
            $('#choose-page-dialog').dialog('close');
            var pageId = selected.attr('data-id');
            if (selected.attr('data-isbootstrap') == 'true') {
                MBE.io.loadBootstrapPage(pageId);
            }
            else {
                MBE.io.loadLegacyPage(pageId);
            }
        }
        else {
            alert('Please select a page');
        }
    },

    loadDeletedPage: function () {
        var selected = $('#trash-page-table tr.highlightedRow');
        if (selected.length) {
            $('#trash-page-dialog').dialog('close');
            var pageId = selected.attr('data-id');
            if (selected.attr('data-isbootstrap') == 'true') {
                MBE.io.loadBootstrapPage(pageId);
            }
            else {
                MBE.io.loadLegacyPage(pageId);
            }
        }
        else {
            alert('Please select a page');
        }
    },

    reloadPage: function () {
        if ($('#currentPageVersion').val() == 'Legacy') {
            MBE.io.loadLegacyPage('current');
        }
        else {
            MBE.io.loadBootstrapPage('current');
        }
    },

    loadLegacyPage: function (pageId) {
        pageSpinner.show();
        pageId = pageId == "current" ? $('#currentPageId').val() : pageId;

        var appId = $('#currentAppId').val();
        var url = "/api/mozaic-editor/apps/" + appId + "/pages/" + pageId;

        $.ajax({
            type: "GET",
            url: url,
            dataType: "json",
            complete: function () { pageSpinner.hide() },
            error: MBE.ajaxError,
            success: function (data) { MBE.io.convert(data); }
        });
    },

    createPage: function () {
        pageSpinner.show();
        $('#new-page-dialog').dialog('close');

        var appId = $('#currentAppId').val();
        var newPageName = $('#new-page-name').val();
        var postData = {
            Name: newPageName,
            Content: '',
            IsDeleted: false,
            Components: []
        };

        $.ajax({
            type: "POST",
            url: "/api/mozaic-bootstrap/apps/" + appId + "/pages",
            data: postData,
            error: MBE.ajaxError,
            success: function (data) {
                $('#currentPageId').val(data);
                $('#currentPageVersion').val('Bootstrap');
                $('#headerPageName').text(newPageName == '' ? 'Nepojmenovaná stránka' : newPageName);

                if (MBE.saveRequested) {
                    MBE.io.doSave();
                }
                else {
                    pageSpinner.hide();
                }
            }
        });
    },

    deletePage: function () {
        var selected = $('#page-table tr.highlightedRow');
        if (selected.length) {
            if (confirm('Are you sure you want to delete this page?')) {
                pageSpinner.show();
                var appId = $('#currentAppId').val();
                var pageId = selected.attr('data-id');
                var api = selected.attr('data-isbootstrap') == 'true' ? 'mozaic-bootstrap' : 'mozaic-editor';

                $.ajax({
                    type: "POST",
                    url: '/api/' + api + '/apps/' + appId + '/pages/' + pageId + '/delete',
                    complete: function () {
                        pageSpinner.hide();
                    },
                    success: function () {
                        alert("OK. Page deleted.");
                        $('#choose-page-dialog').dialog('close');
                        // Clear workspace, but only when deleted page is currently opened
                        if ($('#currentPageId').val() == pageId) {
                            MBE.clearWorkspace();
                            $('#headerPageName').html('');
                            $('#currentPageId').val('');
                        }
                    },
                    error: MBE.ajaxError
                });
            }
        }
        else {
            alert('Please select a page to delete.');
        }
    },

    rename: function () {
        $('#headerPageName').text($(this).find('#page-name').val());
        $('#rename-page-dialog').dialog('close');
        MBE.changedSinceLastSave = true;
    },

    savePage: function () {
        var pageId = Number($('#currentPageId').val());
        if (!pageId || isNaN(pageId)) {
            MBE.saveRequested = true;
            MBE.dialogs.createPage();
        }
        else {
            MBE.io.doSave();
        }
    },

    doSave: function () {
        pageSpinner.show();
        MBE.saveRequested = false;

        var postData = {
            Name: $('#headerPageName').text(),
            Content: MBE.io.getPageDOM(),
            IsDeleted: false,
            Components: MBE.io.getComponentsArray(MBE.workspace)
        };

        var appId = $('#currentAppId').val();
        var pageId = $('#currentPageId').val();

        $.ajax({
            type: "POST",
            url: "/api/mozaic-bootstrap/apps/" + appId + "/pages/" + pageId,
            data: postData,
            complete: function () {
                pageSpinner.hide();
            },
            success: function () { alert("OK") },
            error: MBE.ajaxError
        });
    },

    getPageDOM: function () {
        var dom = $(MBE.workspace).clone(true);
        return dom.html();
    },

    getComponentsArray: function (parent) {
        var components = new Array();
        var uicList = $(parent).find('> [data-uic]');

        for (var i = 0; i < uicList.length; i++) {
            var node = uicList.eq(i);
            var component = {
                ElmId: node.attr('id') ? node.attr('id') : '',
                Tag: node[0].tagName.toLowerCase(),
                UIC: node.attr('[data-uic]'),
                Attributes: MBE.io.filterAttrs(MBE.io.getAttrs(node[0])),
                Properties: node.attr('data-properties') ? node.attr('data-properties') : '',
                Content: MBE.io.filterUICContent(node),
                ChildComponents: MBE.io.getComponentsArray(node)
            };
            components.push(component);
        }

        return components;
    },

    /*************************************************************/
    /* CONVERSION METHODS                                        */
    /*************************************************************/
    convert: function (data) {
        if (!data.Id) {
            return;
        }
        MBE.io.allComponents = new Array();
        MBE.clearWorkspace();

        var container = $(MBE.types.containers.templates['container']);
        container.attr('data-uic', 'containers|container').appendTo(MBE.workspace);

        var form = $(MBE.types.form.templates.form);
        form.attr('data-uic', 'form|form').appendTo(container);

        for (var i = 0; i < data.Components.length; i++) {
            MBE.io.convertComponent(form, data.Components[i]);
        }

        // Pokusíme se naskládat inputy ke správným labelům

        var boundInputs = [];
        for (var i = 0; i < MBE.io.allComponents.length; i++) {
            var c1 = MBE.io.allComponents[i];
            if (c1.item && c1.item.is('[data-uic="form|label"]')) {
                var mostLikelyInput = null;

                c1.PositionX = parseInt(c1.PositionX);
                c1.PositionY = parseInt(c1.PositionY);
                c1.Width = parseInt(c1.Width);

                for (var j = 0; j < MBE.io.allComponents.length; j++) {
                    var c2 = MBE.io.allComponents[j];

                    if (c2.item.is('input:not([type=radio],[type=checkbox]), textarea, select, .form-control-static') && $.inArray(c2.Id, boundInputs) === -1) {
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

                if (mostLikelyInput != null) {
                    c1.item.next().append(mostLikelyInput.item);
                    c1.item.attr('for', mostLikelyInput.Name);
                    boundInputs.push(mostLikelyInput.Id);
                }
            }
        }

        $('#currentPageId').val(data.Id);
        $('#currentPageVersion').val('Legacy');
        $('#headerPageName').text(data.Name);

        MBE.DnD.updateDOM();
    },

    convertDiv: function (nc, c) {
        if (c.Classes.indexOf('panel-component') !== -1) {
            nc.item = $(MBE.types.containers.templates.panel);
            nc.item
                .find('.panel-body').html('').end()
                .find('.panel-footer').remove().end()
                .attr('data-uic', 'containers|panel');

            if (c.Name) {
                nc.item.attr('id', c.Name);
            }
            if (c.Classes.indexOf('named-panel') === -1) {
                nc.item.find('.panel-heading').remove();
            }
            else {
                nc.item.find('.panel-title').html(c.Label);
            }
            nc.newTarget = nc.item.find('.panel-body');
        }
        if (c.Classes.indexOf('control-label') !== -1) {
            if (c.Label == '{var1}') { // Pravděpodobně je to statický text ve formuláři
                nc.item = $(MBE.types.form.templates['static-control']);
                nc.item
                    .html(c.Label)
                    .attr('data-uic', 'form|static-control');
            }
            else {
                nc.item = $(MBE.types.form.templates.label);
                nc.item
                    .html(c.Label)
                    .attr('data-uic', 'form|label')
                    .addClass('col-sm-2');
                nc.item.wrap('<div class="form-group" data-uic="form|form-group"></div>');
                nc.item.after('<div class="col-sm-10" data-uic="grid|column"></div>');
            }
        }
        if (c.Classes.indexOf('form-heading') !== -1) {
            nc.item = $('<h2 data-uic="text|heading">' + c.Label + '</h2>');
        }
        if (c.Classes.indexOf('checkbox-control') !== -1) {
            nc.item = $(MBE.types.form.templates['checkbox']);
            nc.item.attr({ name: c.Name, 'data-uic': 'form|checkbox' });
            nc.item.wrap('<label for="' + c.Name + '" data-uic="form|label"></label>');
            nc.item.parent().append(' ' + c.Label);
            nc.item.parent().wrap('<div class="checkbox" data-uic="form|checkbox-group"></div>');
            nc.item.parent().parent().wrap('<div class="col-sm-10 col-sm-push-2" data-uic="grid|column"></div>');
            nc.item.parent().parent().parent().wrap('<div class="form-group" data-uic="form|form-group"></div>');
        }
        if (c.Classes.indexOf('radio-control') !== -1) {
            nc.item = $(MBE.types.form.templates['radio']);
            nc.item.attr({ name: c.Name, 'data-uic': 'form|radio' });
            nc.item.wrap('<label for="' + c.Name + '" data-uic="form|label"></label>');
            nc.item.parent().append(' ' + c.Label);
            nc.item.parent().wrap('<div class="radio" data-uic="form|radio-group"></div>');
            nc.item.parent().parent().wrap('<div class="col-sm-10 col-sm-push-2" data-uic="grid|column"></div>');
            nc.item.parent().parent().parent().wrap('<div class="form-group" data-uic="form|form-group"></div>');
        }
        if (c.Classes.indexOf('info-container') !== -1) {
            nc.item = $(MBE.types.containers.templates.div);
            nc.item.attr('data-uic', 'containers|div');
            nc.item.append('<div class="info-container-header" data-uic="containers|div"><span class="fa fa-info-circle" data-uic="image|icon"></span>' + c.Label + '</div>');
            nc.item.append('<div class="info-container-body" data-uic="containers|div">' + c.Content + '</div>');
        }
        if (c.Classes.indexOf("breadcrumb-navigation") !== -1) {

            nc.item = $(MBE.types.misc.templates.breadcrumbs);
            nc.item.attr('data-uic', 'misc|breadcrumbs');

            var item = $(MBE.types.misc.templates['breadcrumbs-item']);
            var itemActive = $(MBE.types.misc.templates['breadcrumbs-active']);
            var itemInactive = $(MBE.types.misc.templates['breadcrumbs-inactive']);

            var i1 = item.clone();
            i1.append(itemActive.clone());
            i1.find('> span').addClass('fa fa-question app-icon').end().appendTo(nc.item);

            var i2 = item.clone();
            i2.append(itemInactive.clone());
            i2.find('> a').html('APP NAME').end().appendTo(nc.item);

            var i3 = item.clone();
            i3.append(itemActive.clone());
            i3.find('> span').html('Nav').end().appendTo(nc.item);
        }
        if (c.Classes.indexOf('countdown-component') !== -1) {
            nc.item = $(MBE.types.ui.templates.countdown);
        }
        if (c.Classes.indexOf('wizard-phases') !== -1) {
            nc.item = $(MBE.types.ui.templates.wizzard);
            nc.item.attr({
                'data-phases': c.Content,
                'data-activephase': MBE.io.getProperty('ActivePhase', c.Properties)
            });
            nc.afterAppend = MBE.types.ui.buildWizzard;
        }
    },

    convertSelect: function (nc, c) {
        nc.item = $(MBE.types.form.templates.select);
        nc.item.attr({
            'data-uic': 'form|select',
            'name': c.Name
        });
        if (c.Classes.indexOf('multiple-select') !== -1) {
            nc.item.attr('multiple', 'multiple');
        }

        if (c.Properties && c.Properties.indexOf('defaultoption') !== -1) {
            var defaultOption = MBE.io.getProperty('defaultoption', c.Properties);
            nc.item.append('<option value="">' + defaultOption + '</option>');
        }
    },

    convertInput: function (nc, c) {
        if (c.Classes.indexOf('input-single-line') !== -1) {
            nc.item = $(MBE.types.form.templates['input-text']);
            nc.item.attr('data-uic', 'form|input-text');
        }
        if (c.Classes.indexOf('button-browse') !== -1) {
            nc.item = $(MBE.types.form.templates['input-file']);
            nc.item.attr('data-uic', 'form|input-file');
        }
        if (c.Classes.indexOf('color-picker') !== -1) {
            nc.item = $(MBE.types.form.templates['input-color']);
            nc.item.attr({
                'name': c.Name,
                'data-uic': 'form|input-color'
            });
        }

        nc.item.attr({ name: c.Name });
    },

    convertTextArea: function (nc, c) {
        nc.item = $(MBE.types.form.templates.textarea);
        nc.item.attr({
            'data-uic': 'form|textarea',
            'name': c.Name,
            'rows': 5
        });
    },

    convertButton: function (nc, c) {
        nc.item = $(MBE.types.controls.templates.button);
        nc.item.attr({
            'data-uic': 'controls|button',
            'type': 'submit',
            'name': c.Name
        }).html(c.Label).addClass('btn-primary');

        if (c.Classes.indexOf('button-large') !== -1) {
            nc.item.addClass('btn-lg');
        }
        if (c.Classes.indexOf('button-small') !== -1) {
            nc.item.addClass('btn-sm');
        }
        if (c.Classes.indexOf('button-extra-small') !== -1) {
            nc.item.addClass('btn-xs');
        }

        if (c.Classes.indexOf('button-dropdown') !== -1) {
            nc.item.addClass('dropdown-toggle').append(' <span class="caret"></span>');
            nc.item.wrap('<div class="btn-group" data-uic="controls|button-dropdown"></div>');
            nc.item.parent().append('<ul class="dropdown-menu" data-uic="controls|dropdown-menu" locked></ul>');
        }
    },

    convertTable: function (nc, c) {
        if (c.Classes.indexOf('data-table') !== -1) {
            nc.item = $(MBE.types.ui.templates['data-table']);
            nc.afterAppend = MBE.types.ui.buildDataTable;

            nc.item.attr('data-uic', 'ui|data-table');
            if (c.Classes.indexOf('data-table-simple-mode') !== -1) {
                nc.item.attr({
                    'data-dtpaging': '0',
                    'data-dtinfo': '0',
                    'data-dtfilter': '0',
                    'data-dtordering': '0'
                });
            }
            if (c.Classes.indexOf('data-table-with-actions') !== -1) {
                var actions = MBE.io.getProperty('actions', c.Properties);
                actions = actions && actions.length > 0 ? actions.split(/-/) : ['edit', 'detail', 'delete'];

                var actionList = [];
                for (var i = 0; i < actions.length; i++) {
                    var a = null;
                    switch (actions[i]) {
                        case 'edit': a = { icon: 'fa fa-edit', action: 'edit', title: 'upravit' }; break;
                        case 'details': a = { icon: 'fa fa-search', action: 'details', title: 'detail' }; break;
                        case 'delete': a = { icon: 'fa fa-remove', action: 'delete', title: 'smazat' }; break;
                        case 'download': a = { icon: 'fa fa-download', action: 'download', title: 'stáhnout' }; break;
                        case 'enter': a = { icon: 'fa fa-sign-in', action: 'enter', title: 'vstoupit' }; break;
                    }
                    if (a !== null) {
                        actionList.push(a);
                    }
                }
                nc.item.attr('data-actions', JSON.stringify(actionList).replace(/"/g, "'"));
            }
        }
        if (c.Classes.indexOf('name-value-list') !== -1) {
            nc.item = $(MBE.types.ui.templates['nv-list']);
            nc.afterAppend = MBE.types.ui.buildNVList;
        }
    },

    convertUL: function (nc, c) {
        if (c.Classes.indexOf('tab-navigation') !== -1) {
            var tabLabelArray = c.Content.split(";");
            tabLabelArray.unshift('<span class="fa fa-home" data-uic="image|icon"></span>');

            nc.item = $(MBE.types.containers.templates.tabs);
            nc.item.attr('data-uic', 'containers|tabs');

            MBE.types.containers.buildTabs.apply(nc.item[0], tabLabelArray);
        }
    },

    convertComponent: function (targetContainer, c) {
        var self = MBE.io;
        var nc = {
            item: null,
            newTarget: null,
            afterAppend: null
        };
        var systemClasses = [
            'panel-component', 'named-panel', 'control-label', 'dropdown-select', 'input-single-line', 'button-browse',
            'input-multiline', 'uic', 'button-simple', 'button-large', 'button-small', 'button-extra-small', 'data-table'
        ];

        switch (c.Tag.toLowerCase()) {
            case 'div':
                self.convertDiv(nc, c);
                break;
            case 'select':
                self.convertSelect(nc, c);
                break;
            case 'input':
                self.convertInput(nc, c);
                break;
            case 'textarea':
                self.convertTextArea(nc, c);
                break;
            case 'button':
                self.convertButton(nc, c);
                break;
            case 'table':
                self.convertTable(nc, c);
                break;
            case 'ul':
                self.convertUL(nc, c);
                break;
        }

        var item = nc.item;
        var newTarget = nc.newTarget;

        if (item) {
            var systemClassesRegExp = new RegExp('(' + systemClasses.join('( |$))|(') + '( |$))', 'g');
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
            if (c.Properties) {

                var propList = c.Properties.split(/; */g);
                var newPropList = [];
                for (var i = 0; i < propList.length; i++) {
                    var pair = propList[i].split(/=/);
                    if (pair[0] == 'defaultoption') {
                        continue;
                    }
                    newPropList.push(pair[0] + '=' + pair[1]);
                }

                item.attr('data-properties', newPropList.join(';'));
            }

            item.attr('id', c.Name);

            if (item.parents('.form-group').length) {
                item.parents('.form-group').appendTo(targetContainer);
            }
            else if (item.parents('.btn-group').length) {
                item.parents('.btn-group').appendTo(targetContainer);
            }
            else {
                item.appendTo(targetContainer);
            }

            if (typeof nc.afterAppend == 'function') {
                nc.afterAppend.apply(item[0], []);
            }
        }

        if (c.ChildComponents) {
            for (j = 0; j < c.ChildComponents.length; j++) {
                self.convertComponent(newTarget ? newTarget : item, c.ChildComponents[j]);
            }
        }

        c.item = item;
        self.allComponents.push(c);
    },

    /************************************************/
    /* TOOLS                                        */
    /************************************************/
    getProperty: function (name, properties) {
        var propList = properties ? properties.split(/; */g) : [];
        for (var i = 0; i < propList.length; i++) {
            var pair = propList[i].split(/=/);
            if (pair[0] == name) {
                return pair[1];
            }
        }
        return '';
    },

    getAttrs: function (el) {
        return [].slice.call(el.attributes).map(function(attr) {
            var attrs = {
                name: attr.name,
                value: attr.value
            };
            return attrs;
        });
    },

    filterAttrs: function (attrs) {
        var finalAttrs = new Array();

        for (var i = 0; i < attrs.length; i++) {
            if ($.inArray(attrs[i].name, ['data-uic', 'data-custom-classes', 'data-custom-attributes', 'data-properties']) != -1) {
                continue;
            }
            finalAttrs.push(attr[i]);
        }

        return JSON.stringify(finalAttrs);
    }
};
