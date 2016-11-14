MBE.types.ui = {

    templates: {
        'horizontal-form-row': '<div></div>',
        'nv-list': '<table class="table name-value-list"></table>',
    },

    templatesName: {
        'horizontal-form-row': 'Horizontal form row',
        'nv-list': 'Name - Value list',
    },

    options: {

    },
    
    init: function()
    {
        var self = MBE.types.ui;

        var group = $('<li></li>');
        group.html('<span class="fa fa-caret-right fa-fw"></span>UI').prependTo('ul.category');
    
        var items = $('<ul data-type="ui" style="display: none"></ul>');
        for(template in self.templatesName) {
            var item = $('<li></li>');
            item
                .html('<span class="fa fa-square fa-fw"></span>' + self.templatesName[template])
                .attr({ 'data-template': template, 'draggable': true })
                .data('type', 'ui')
                .appendTo(items);
        }
        items.appendTo(group);

        MBE.DnD.onDrop.push(MBE.types.ui._drop);
    },

    _drop: function(target)
    {
        if ($(this).is('[data-uic="ui|horizontal-form-row"]')) {
            MBE.types.ui.buildHorizontalFormRow.apply(this, []);
        }
        if ($(this).is('[data-uic="ui|nv-list"]')) {
            MBE.types.ui.buildNVList.apply(this, []);
        }
    },

    buildHorizontalFormRow: function()
    {
        var g = MBE.types.grid;
        var f = MBE.types.form;

        var row = $(f.templates['form-group']);
        var label = $(f.templates['label']);
        var input = $(f.templates['input-text']);
        var column = $(g.templates['column']);
        
        row.attr('data-uic', 'form|form-group');
        label.attr('data-uic', 'form|label').addClass('col-sm-2').appendTo(row);
        column.attr('data-uic', 'grid|column').addClass('col-sm-10').appendTo(row);
        input.attr('data-uic', 'form|input-text').appendTo(column);
        
        $(this).replaceWith(row);
    },

    buildNVList: function()
    {
        var body = $(MBE.types.table.templates.tbody);
        
        var names = ["Platform", "Country", "Year"];
        var values = ["Omnius", "Czech Republic", "2006"];

        for (var i = 0; i < names.length; i++) {
            var row = $(MBE.types.table.templates.tr);
            row.attr('locked', true);

            var nameCell = $(MBE.types.table.templates.td);
            nameCell.attr({ 'data-uic': 'table|td', 'locked': true });
        
            var valueCell = nameCell.clone();
            
            nameCell.addClass('name-cell').html(names[i]).appendTo(row);
            valueCell.addClass('value-cell').html(values[i]).appendTo(row);

            row.appendTo(body);
        }

        body.appendTo(this);
    }
};

MBE.onInit.push(MBE.types.ui.init);
