var RigPlacement = {
    
  
    abc: ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'],
    
    contextItems: {
        'remove': { name: 'Remove', icon: 'fa-trash' }
    },

    init: function () {
        $(document).on('click', '#btnSave', $.proxy(this.save, this));

        $('.rig-placement').each($.proxy(this.makeGrid, this));

        this.contextItems.remove.disabled = this.isEmpty;

        $.contextMenu({
            trigger: 'right',
            zIndex: 300,
            selector: '.rig-placement td',
            callback: $.proxy(this._onAction, this),
            items: this.contextItems
        });

        this.loadData();
        this.alive();
    },

    makeGrid: function (index, element) {
        var target = $(element);
        var cols = parseInt(target.data('cols')) + 1;
        var rows = parseInt(target.data('rows'));

        var grid = $('<table class="table table-bordered table-condensed"></table>');
        var colgroup = $('<colgroup></colgroup>');
        var cellWidth = 98 / cols;
      
      	var indexOffset = index * 17;
        
        var row = $('<tr></tr>');
        for (var j = 0; j < cols; j++) {
            var cell = $('<th class="text-center bg-primary"></th>');
            cell.html(j == 0 ? '' : indexOffset + j).appendTo(row);
        }
     
        row.appendTo(grid);

        for (var i = 0; i < rows; i++) {
            var row = $('<tr></tr>');
            for (var j = 0; j < cols; j++) {
                if (i == 0) {
                    var col = $('<col width="' + (j == 0 ? 2 : cellWidth) + '%" />');
                    col.appendTo(colgroup);
                }
                var tag = j == 0 ? 'th' : 'td';
                var cell = $('<'+tag+'></'+tag+'>');
                cell.html(j == 0 ? this.abc[i] : '&nbsp;').appendTo(row);
                
                if(j == 0) { cell.addClass('text-right bg-primary'); }
            }
            row.appendTo(grid);
        }

        grid.prepend(colgroup);
        grid.appendTo(target);
    },

    loadData: function ()
    {
        $.ajax({
            url: this.getUrl() + '?button=btnLoad',
            type: 'post',
            dataType: 'json',
            data: {
                button: 'btnLoad'   
            },
            success: $.proxy(this.setData, this)
        });
    },

    setData: function (data) {
        for (var i = 0; i < data['data'].length; i++) {
            var rigIp = data.data[i].Rig_ip,
                rigName = data.data[i].Rig_name,
                side = data.data[i].Side,
                x = data.data[i].X,
                y = data.data[i].Y;
          		
            alert(x);
            	var testing = data.data[i].Testing;

            // Existuje stále rig?
            var cell = $('.rig-placement[data-side="' + side + '"] table tr').eq(y+1).find('td').eq(x);
            cell.html(rigName).data('rigip', rigIp);

            var rig = $('ul.list-group li[data-rigip="' + rigIp + '"]');
            if (rig.length) {
                rig.data('x', x).data('y', y).data('side', side);
                switch (rig.data('status')) {
                    case 'up': cell.addClass('success'); break;
               	    case 'down': testing ? cell.addClass('success') : cell.addClass('danger'); break;
                    case 'warning': cell.addClass('warning'); break;
                }
            }
            else {
                cell.addClass('active');
            }
        }
      unassigned = []

        $('ul.list-group li').each(function () {
            var elm = $(this),
                x = elm.data('x'),
                y = elm.data('y'),
                side = elm.data('side');

            if ((typeof x == 'undefined' || !x.toString().length) || (typeof y == 'undefined' || !y.toString().length) || (typeof side == 'undefined' || !side.length)) {
                elm.prepend('<span class="badge">unassigned</span>');
            }
        });
      
      $(".list-group-item span.badge").parent().prependTo(".list-group");
    },

    save: function () {
        var data = [];

        $('#btnSave .fa').removeClass('fa-floppy-o').addClass('fa-circle-o-notch fa-spin');

        // Sestavíme data
        $('.rig-placement').each(function () {
            var side = $(this).data('side');

            $('table tr', this).each(function () {
                var y = $(this).index() - 1;

                $('td', this).each(function () {
                    var cell = $(this),
                        x = cell.index() - 1,
                        rigIp = cell.data('rigip');

                    if (rigIp && rigIp.length) { // Buňka obsahuje referenci na rig
                        data.push({
                            Rig_ip: rigIp,
                            Rig_name: cell.text().trim(),
                            X: x,
                            Y: y,
                            Side: side
                        });
                    }
                });
            });
        });

        // Odešleme
        $.ajax({
            url: this.getUrl() + '?button=btnSave',
            type: 'post',
            dataType: 'json',
            data: {
                RigPlacementData: JSON.stringify({
                    data: { item: data }
                })
            },
            success: $.proxy(this.showResult, this),
            complete: this.saveDone
        });
    },

    showResult: function (data) {
        if (data.message) {
            var msg = $('<div class="alert alert-dismissible" role="alert"></div>');
            var btn = $('<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>');

            msg.addClass('alert-' + data.status).html(data.message).prepend(btn).appendTo('#messageContainer');
        }
    },

    saveDone: function () {
        $('#btnSave .fa').addClass('fa-floppy-o').removeClass('fa-circle-o-notch fa-spin');
    },

    removePlacement: function (cell) {
        var rigIp = cell.data('rigip');
        cell.data('rigip', '').html('').removeClass();

        var rig = $('ul.list-group li[data-rigip="' + rigIp + '"]');
        if (rig.length) {
            rig.data('x', '').data('y', '').data('side', '');
            rig.prepend('<span class="badge">unassigned</span>');
        }
    },

    getUrl: function () {
        return '/api/run/' + $('#currentAppName').val() + '/' + $('#currentBlockName').val();
    },

    isEmpty: function (key, options) {
        var rigIp = options.$trigger.data('rigip');
        return typeof rigIp == 'undefined' || !rigIp.length;
    },

    alive: function ()
    {
        $('.rig-placement td').droppable({
            hoverClass: 'info',
            drop: function (event, ui) {
                var rig = ui.draggable,
                    currentX = rig.data('x'),
                    currentY = rig.data('y'),
                    side = rig.data('side'),
                    target = $(this),
                    targetRigIp = target.data('rigip');
              
                if (targetRigIp && targetRigIp.length) {
                  return false;
                }

                if (typeof currentX != 'undefined' && typeof currentY != 'undefined') {
                    var currentCell = $('.rig-placement[data-side="' + side + '"] table tr').eq(currentY+1).find('td').eq(currentX);
                    currentCell.text('').removeClass().data('rigip', '');
                }
                
                rig.data('side', target.parents('.rig-placement').data('side'))
                    .data('x', target.index() - 1)
                    .data('y', target.parent().index() - 1)
                    .find('.badge').remove();

                switch (rig.data('status')) {
                    case 'up': target.addClass('success'); break;
                    case 'down': target.addClass('danger'); break;
                    case 'warning': target.addClass('warning'); break;
                }

                target.html(rig.text()).data('rigip', rig.data('rigip'));
            }
        });
        $('ul.list-group li').draggable({
            revert: true,
            revertDuration: 0,
            cursorAt: { left: 3, top: 3 },
            zIndex: 100,
            helper: function (event) {
                var clone = $(this).clone();
                clone.find('.badge').remove();
                return $('<span class="label label-default">'+ clone.text() + '</span>');
            }
        });
    },

    _onAction: function (key, options) {
        if (key == 'remove') {
            this.removePlacement(options.$trigger);
        }
    }  
}

$($.proxy(RigPlacement.init, RigPlacement));