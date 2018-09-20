var RigPlacementView = {
    
    abc: ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'],
    
    init: function () {
        $('.rig-placement').each($.proxy(this.makeGrid, this));
        this.loadData();
    },

    makeGrid: function (index, element) {
        var target = $(element);
        var cols = parseInt(target.data('cols')) + 1;
        var rows = parseInt(target.data('rows'));

        var grid = $('<table class="table table-bordered table-condensed"></table>');
        var colgroup = $('<colgroup></colgroup>');
        var cellWidth = 98 / cols;
    
        var row = $('<tr></tr>');
      
      	var indexOffset = index * 17;
      
        for (var j = 0; j < cols; j++) {
            var cell = $('<th class="text-center bg-primary"></th>');
            cell.html(j == 0 ? '' : indexOffset+j).appendTo(row);
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

            // Existuje stále rig?
            var cell = $('.rig-placement[data-side="' + side + '"] table tr').eq(y+1).find('td').eq(x);
            cell.html(rigName).data('rigip', rigIp);
            
            var info = null;
          	$.each(data['info'], function() {
              if(this.host == rigIp) {
                info = this;
                return false;
              }
            })

            if (info != null) {
                switch (info.status) {
                    case 'up': cell.addClass('success'); break;
                    case 'down': cell.addClass('danger'); break;
                    case 'warning': cell.addClass('warning'); break;
                }
            }
            else {
                cell.addClass('active');
            }
        }
    },

    getUrl: function () {
        return '/api/run/' + $('#currentAppName').val() + '/' + $('#currentBlockName').val();
    }
}

$($.proxy(RigPlacementView.init, RigPlacementView));