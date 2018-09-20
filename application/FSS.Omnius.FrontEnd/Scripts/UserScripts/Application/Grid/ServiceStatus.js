//Get number of GPUs per rig
//var url = new URL(window.location.href);
//var container_id = ;
var rig_gpus = {};
rig_gpus = $.ajax({
    method:"GET",
    url: "/api/grid/rig_configs/" + new URL(window.location.href).searchParams.get("modelId"),
}).done(function (response) {
    rig_gpus = response;
});

var RigPlacement = {

    abc: ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'],

    shelfCount: 0,

    afterSetDate: null,
    isEditMode: false,
    isBothSides: false,
    isMultiple: false,
    gridWidth: null,
    gridHeight: null,

    init: function () {
        if (!$('#panelRigPlacement').length && !$('#panelRigPlacementEdit').length)
            return;

        var settings = $('#Shelf');
        this.gridWidth = settings.data('x');
        this.gridHeight = settings.data('y');
        this.isBothSides = settings.data('bothsides') == 'True';
        this.isMultiple = settings.data('multiple') == 'True';

        this.View.init();
        this.Edit.init();

        this.loadData();
    },

    buildTemplate: function (config) {
        var width = config && config.width ? config.width * 1 : this.gridWidth,
            height = config && config.height ? config.height * 1 : this.gridHeight,
            isBothSides = config && typeof config.both_sides != 'undefined' ? config.both_sides : this.isBothSides,
            colsNames = config && config.cols_names ? JSON.parse(config.cols_names) : null,
            rowsNames = config && config.rows_names ? JSON.parse(config.rows_names) : null;

        var cellWidth = 98 / width,
            sides = ['Left'];

        if (isBothSides) {
            sides.push('Right');
        }

        var $shelf = $('<div class="shelf"></div>'),
            $btnRemove = $('<button type="button" class="btn btn-danger btn-sm delete-shelf pull-right" name="delete" value="X"><span class="fa fa-times"></span></button>'),
            $btnRename = $('<sup style="margin-left: 5px; font-size: 0.7em; display: inline-block"><i class="btn-link btn-rename-shelf fa fa-pencil" title="Rename" style="text-decoration:none;cursor:pointer;"></i></sup>');
        $h3 = $('<h3><span class="shelf-name"></span></h3>'),
            $h4 = $('<h4></h4>'),
            $placement = $('<div class="rig-placement"></div>'),
            $table = $('<table class="table table-bordered table-condensed"></table>'),
            $colgroup = $('<colgroup></colgroup>'),
            $tbody = $('<tbody></tbody>');

        if (this.isEditMode && this.isMultiple) {
            $shelf.append($btnRemove);
            $h3.append($btnRename);
        }
        if (this.isMultiple) {
            $shelf.append($h3);
        }

        // Create only left or both sides
        for (var s = 0; s < sides.length; s++) {
            var indexOffset = s * width;
            var sideKey = sides[s].toLowerCase();

            var $heading = $h4.clone();
            $heading.html(sides[s] + ' side').appendTo($shelf);

            var $sidePlacement = $placement.clone();
            $sidePlacement.attr({
                'data-side': sideKey,
                'data-cols': this.gridWidth,
                'data-rows': this.gridHeight
            });

            var $sideTable = $table.clone(),
                $sideColgroup = $colgroup.clone(),
                $sideTBody = $tbody.clone(),
                $sideTHead = $('<thead>'),
                $rowHead = $('<tr></tr>');

            $sideTable.append($sideColgroup).append($sideTHead).append($sideTBody);
            $sideTHead.append($rowHead);

            // Create table header		 
            for (var j = 0; j <= width; j++) {
                var $cell = $('<th class="text-center bg-primary"></th>'),
                    text = j == 0 ? '' : (colsNames != null && colsNames[sideKey][j - 1] ? colsNames[sideKey][j - 1] : indexOffset + j);

                $cell.html(text).appendTo($rowHead);

                var $col = $('<col width="' + (j == 0 ? 2 : cellWidth) + '%" />');
                $col.appendTo($sideColgroup);
            }

            // Create table body
            for (var i = 0; i < height; i++) {
                var $row = $('<tr></tr>');
                for (var j = 0; j <= width; j++) {

                    var tag = j == 0 ? 'th' : 'td',
                        $cell = $('<' + tag + '></' + tag + '>');

                    $cell.html(j == 0 ? (rowsNames != null && rowsNames[sideKey][i] ? rowsNames[sideKey][i] : this.abc[i]) : '&nbsp;').appendTo($row);
                    $cell.addClass(j == 0 ? 'text-right bg-primary' : '');

                }
                $row.appendTo($sideTBody);
            }
            $sideTable.appendTo($sidePlacement);
            $sidePlacement.appendTo($shelf);
        }

        return $shelf;
    },

    loadData: function () {
        $.ajax({
            url: this.getUrl() + '?button=btnLoad' + '&modelId=' + GetUrlParameter("modelId"),
            type: 'post',
            dataType: 'json',
            data: {},
            success: $.proxy(this.setData, this)
        });
    },

    setData: function (data) {
        // count shelfs and create them
        $('.shelf').remove();

        this.shelfCount = 0;

        if (data.shelfs && data.shelfs.length) {
            for (var i = 0; i < data.shelfs.length; i++) {
                this.addShelf(data.shelfs[i]);
            }
        }
        else {
            var sc = 0;
            for (var i = 0; i < data['data'].length; i++) {
                sc = sc < data.data[i].Shelf_id && data.data[i].X > -1 && data.data[i].Y > -1 ? data.data[i].Shelf_id : sc;
            }

            sc += sc == 0 ? 1 : 0;
            for (var i = 0; i < sc; i++) {
                this.addShelf();
            }
        }

        // load data
        for (var i = 0; i < data['data'].length; i++) {
            var rigIp = data.data[i].Rig_ip.split(':')[0],
                rigName = data.data[i].Rig_name,
                shelf = data.data[i].Shelf_id,
                side = data.data[i].Side,
                x = data.data[i].X,
                y = data.data[i].Y,
                RigId = data.data[i].id,
                testing = data.data[i].Testing;

            // Existuje stále rig?
            var $cell = $('.shelf[data-shelfid=' + shelf + '] .rig-placement[data-side="' + side + '"] table tbody tr').eq(y).find('td').eq(x);
            $cell.html(rigName).data('rigip', rigIp);

            var info = null;
            $.each(data['info'], function () {
                if (this.host.substring(0, this.host.indexOf(":")) == rigIp) {
                    info = this;
                    return false;
                }
            });

            // Set rig data in rig list on Edit mode
            var rig = $('ul.list-group li[data-rigip="' + rigIp + '"]');
            if (rig.length) {
                rig.data('x', x)
                    .data('y', y)
                    .data('side', side)
                    .data('shelf', shelf)
                    .data('reason', info != null ? info.reason : '')
                    .data('gpus', info != null ? info.gpu_temp.length : '');
            }

            // Set cell status class
            if (info != null) {
                this.setCellStatus($cell, info.status, info.reason, info.gpu_temp.length, rigIp);
            }
            else {
                $cell.addClass('rig-not-found');
            }

            // Set cell data
            $cell.data('RigId', RigId);
            $cell.attr('data-toggle', "modal");
            $cell.attr('data-target', "#RigEdit");
        }

        if (typeof this.afterSetData == 'function')
            this.afterSetData();
    },

    getUrl: function () {
        return '/api/run/' + $('#currentAppName').val() + '/' + $('#currentBlockName').val();
    },

    addShelf: function (config) {
        this.shelfCount++;

        var $shelf = this.buildTemplate(config);
        $shelf.attr('data-shelfid', this.shelfCount);
        $shelf.find('h3 .shelf-name').html(config && config.name ? config.name : 'Shelf ' + this.shelfCount);

        if (config && typeof config.id != 'undefined') {
            $shelf.attr('data-shelfrealid', config.id);
        }

        if ($('#Shelf').nextAll('.shelf').length) {
            $('#Shelf').nextAll('.shelf').last().after($shelf);
        }
        else {
            $('#Shelf').after($shelf);
        }
        return $shelf;
    },

    setCellStatus: function (cell, status, reason, gpus, ip) {
        if (status != "down" && typeof rig_gpus[ip] !== 'undefined' && rig_gpus[ip] > gpus)
        {
            status = "warning";
            reason = "bad_riser";
        }
        if ($("#panelRigPlacementEdit")[0]) {
            switch (status) {
                case 'up':
                    cell.addClass('rig-ok');
                    break;
                case 'down':
                    switch (reason) {
                        case 'hw_down': cell.addClass('hardware-down'); break;
                        case 'miner_down': cell.addClass('miner-down'); break;
                        case 'rig_not_mining': cell.addClass('rig-not-mining'); break;
                        case 'response_timeout': cell.addClass('response-timeout'); break;
                    }
                    break;
                case 'warning':
                    switch (reason) {
                        case 'bad_riser': cell.addClass('bad-riser'); break;
                        case 'bad-gpu': cell.addClass('bad-gpu'); break;
                        case 'gpu_stopped': cell.addClass('gpu-stopped'); break;
                        case 'gpu_not_mining': cell.addClass('gpu-not-mining'); break;
                        case 'cpu_not_mining': cell.addClass('cpu-not-mining'); break;
                        case 'gpu_overheating': cell.addClass('gpu-overheating'); break;
                        case 'rig_partially_down': cell.addClass('miner-patially-down'); break;
                    }
                    break;
            }
        } else {
            switch (status) {
                case 'up':
                    cell.addClass('success');
                    break;
                case 'down':
                    cell.addClass('danger');
                    break;
                case 'warning':
                    cell.addClass('warning');
                    break;
            }
        }
    }
};

RigPlacement.View = {

    init: function () {
        if (!$('#panelRigPlacement').length)
            return;

        setInterval($.proxy(RigPlacement.loadData, RigPlacement), 20000);
    }
};

RigPlacement.Edit = {

    modal: null,
    modalHeading: null,
    modalButton: null,

    contextItems: {
        'remove': { name: 'Remove', icon: 'fa-trash' }
    },
    headContextItems: {
        'rename': { name: 'Rename', icon: 'fa-pencil' }
    },

    removedShelfs: [],

    rowNewShelf: null,
    rowRenameShelf: null,

    target: null,

    init: function () {
        if (!$('#panelRigPlacementEdit').length)
            return;

        $(document)
            .on('click', '#btnSave', $.proxy(this.save, this))
            .on('click', '#btnAssign', $.proxy(this.autoAssign, this))
            .on('click', '.btn-rename-shelf', this.renameShelfDialog);

        this.contextItems.remove.disabled = this.isEmpty;
        this.headContextItems.rename.disabled = this.isCorner;

        this.modal = $('#RigPlacementModal');
        this.modalHeading = this.modal.find('.modal-title');
        this.modalButton = this.modal.find('#btnRigPlacementConfirm');

        this.rowNewShelf = this.modal.find('#NewShelfRow');
        this.rowRenameShelf = this.modal.find('#RenameShelfRow');
        var self = this;
        $.contextMenu({
            trigger: 'right',
            zIndex: 300,
            selector: '.rig-placement td',
            callback: $.proxy(this._onAction, this),
            items: this.contextItems,
        });
        $.contextMenu({
            trigger: 'right',
            zIndex: 300,
            selector: '.rig-placement th',
            callback: $.proxy(this._onAction, this),
            items: this.headContextItems
        });

        RigPlacement.afterSetData = $.proxy(this.afterSetData, this);
        RigPlacement.isEditMode = true;

        if (!RigPlacement.isMultiple) {
            $('#addShelf').hide();
        }
        else {
            $(document)
                .on('click', '#addShelf', $.proxy(this.addShelfDialog, this))
                .on('click', '.delete-shelf', this.deleteShelf);
        }
    },

    /******************************************************/
    /* RIG related                                        */
    /******************************************************/
    isEmpty: function (key, options) {
        var rigIp = options.$trigger.data('rigip') ? options.$trigger.data('rigip') : options.$trigger.attr('data-rigip');
        return typeof rigIp == 'undefined' || !rigIp.length;
    },

    removePlacement: function (cell) {
        var rigIp = cell.data('rigip') ? cell.data('rigip') : cell.attr('data-rigip');

        cell.data('rigip', '').html('').removeClass();
        cell.attr('data-rigip', '');
        cell.addClass('ui-droppable');
        cell.data('RigId', '').attr('data-toggle', '').attr('data-target', '');

        var rig = $('ul.list-group li[data-rigip="' + rigIp + '"]');
        if (rig.length) {
            rig.data('x', '').data('y', '').data('side', '');
            rig.prepend('<span class="badge">unassigned</span>');
        }
    },

    afterSetData: function () {
        var unassigned = [];

        $('ul.list-group li').each(function () {
            var elm = $(this),
                x = elm.data('x'),
                y = elm.data('y'),
                side = elm.data('side')
            shelf = elm.data('shelf');

            if ((x == null || !x.toString().length || x == -1) || (y == null || !y.toString().length || y == -1) || (side == null || !side.length) || (shelf == null || !shelf.toString().length || shelf == -1)) {
                elm.prepend('<span class="badge">unassigned</span>');
            }
        });

        $('.list-group-item span.badge').parent().prependTo(".list-group");
        this.alive();
    },

    autoAssign: function () {
        var self = this,
            leftRigList = {},
            rightRigList = {},
            isBothSides = RigPlacement.isBothSides,
            isMultiple = RigPlacement.isMultiple,
            gridSize = RigPlacement.gridWidth * RigPlacement.gridHeight;

        $('#divRigEdit .badge').each(function () {
            var rig = $(this).parent();
            var ip = rig.data('rigip').replace(/:\d+$/, '');
            var segments = ip.split(/\./);

            var side = segments[2] % 2 == 0 ? 'left' : 'right';
            var num = segments[3] * 1;

            if (!isBothSides || side == 'left') {
                leftRigList[num] = rig;
            }
            else {
                rightRigList[num] = rig;
            }
        });

        var lists = { 'left': leftRigList, 'right': rightRigList };

        // projdeme obě strany
        for (var s in lists) {
            if (lists.hasOwnProperty(s)) // Nezajímají nás zdězěné property
            {
                var keys = [];
                for (var k in lists[s]) {
                    if (lists[s].hasOwnProperty(k)) {
                        keys.push(k);
                    }
                }

                // seřadíme klíče / rigy podle posledního segmentu IP adresy
                keys.sort(function (a, b) {
                    return a * 1 < b * 1 ? -1 : (a * 1 > b * 1 ? 1 : 0);
                });

                // Máme něco k přiřazení?
                if (keys.length) {
                    // Spočítáme počet polic a případně je doplníme
                    if (isMultiple) {
                        var sc = Math.ceil(keys.length / gridSize);
                        while (RigPlacement.shelfCount < sc) {
                            var shelf = RigPlacement.addShelf();
                            this.alive(shelf);
                        }
                    }
                    else {
                        sc = 1;
                    }

                    var assigned = -1;
                    for (var sh = 1; sh <= sc; sh++) // Projdeme police
                    {
                        var $t = $('.shelf[data-shelfid=' + sh + '] .rig-placement[data-side=' + s + '] table tbody');

                        // Projdeme tabulku po sloupcích
                        for (var ci = 0; ci < RigPlacement.gridWidth; ci++) {
                            for (var ri = 0; ri < RigPlacement.gridHeight; ri++) {
                                var cell = $t.find('tr').eq(ri).find('td').eq(ci);

                                // Je pozice volná?
                                if (!cell.data('rigip')) {
                                    assigned++;
                                    var rigKey = keys[assigned];
                                    var rig = lists[s][rigKey];

                                    // Je ještě nepřiřazený rig pro tuto stranu?
                                    if (typeof rig != 'undefined') {
                                        rig.attr({
                                            'data-side': s,
                                            'data-x': ci,
                                            'data-y': ri
                                        }).find('.badge').remove();

                                        RigPlacement.setCellStatus(cell, rig.data('status'), rig.data('reason'), rig.data('gpus'), rig.data('rigip'));
                                        cell.html(rig.text()).attr('data-rigip', rig.data('rigip'));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    },

    /******************************************************/
    /* Shelf related                                      */
    /******************************************************/
    addShelfDialog: function () {
        this.rowNewShelf.removeClass('hidden');
        this.rowRenameShelf.addClass('hidden');

        this.rowNewShelf.find('#shelf_name').val('Shelf ' + (RigPlacement.shelfCount + 1));
        this.rowNewShelf.find('#shelf_cols').val(RigPlacement.gridWidth);
        this.rowNewShelf.find('#shelf_rows').val(RigPlacement.gridHeight);
        this.rowNewShelf.find('#both_sides').prop('checked', RigPlacement.isBothSides);

        this.modalButton.one('click', $.proxy(this.addShelf, this));
        this.modal.modal('show');
    },

    addShelf: function () {
        var n = this.rowNewShelf.find('#shelf_name').val(),
            w = this.rowNewShelf.find('#shelf_cols').val(),
            h = this.rowNewShelf.find('#shelf_rows').val(),
            b = this.rowNewShelf.find('#both_sides').is(':checked');

        var $shelf = RigPlacement.addShelf({
            width: w,
            height: h,
            name: n,
            both_sides: b
        });
        this.alive($shelf);

        this.modal.modal('hide');
    },

    renameShelfDialog: function () {
        var self = RigPlacement.Edit;

        self.rowNewShelf.addClass('hidden');
        self.rowRenameShelf.removeClass('hidden');
        self.target = $(this).parents('h3').find('.shelf-name');

        self.rowRenameShelf.find('#new_shelf_name').val(self.target.text());

        self.modalButton.one('click', $.proxy(self.renameShelf, self));
        self.modal.modal('show');
    },

    renameShelf: function () {
        var name = this.rowRenameShelf.find('#new_shelf_name').val();
        if (!name.length) {
            name = 'Shelf ' + this.target.parents('.shelf').data('shelfid');
        }
        this.target.html(name);
        this.target = null;

        this.modal.modal('hide');
    },

    deleteShelf: function () {
        var self = RigPlacement;
        if (self.shelfCount < 2)
            return;

        var id = $(this).parent().data('shelfid') * 1;
        $(this).parent().find('td:not(:empty)').each(function () {
            self.Edit.removePlacement($(this));
        });

        if ($(this).parent().data('shelfrealid')) {
            self.Edit.removedShelfs.push($(this).parent().data('shelfrealid') * 1);
        }

        $(this).parent().remove();
        self.shelfCount--;

        var next = $('.shelf[data-shelfid=' + (id + 1) + ']');
        while (next.length > 0) {
            var currentName = $('h3 .shelf-name', next).text();
            if (/^Shelf \d+$/.test(currentName)) {
                $('h3 .shelf-name', next).html('Shelf ' + id);
            }
            next.attr('data-shelfid', id);

            id++;
            next = $('.shelf[data-shelfid=' + (id + 1) + ']');
        }
    },

    alive: function (newShelf) {
        var target = newShelf ? $('td', newShelf) : $('.rig-placement td');

        target.droppable({
            hoverClass: 'info',
            drop: function (event, ui) {
                var rig = ui.draggable,
                    currentX = rig.data('x'),
                    currentY = rig.data('y'),
                    side = rig.data('side'),
                    target = $(this),
                    targetRigIp = target.data('rigip'),
                    reason = rig.data('reason');

                if (targetRigIp && targetRigIp.length) {
                    return false;
                }

                if (typeof currentX != 'undefined' && typeof currentY != 'undefined') {
                    var currentCell = $('td[data-rigip="' + rig.data('rigip') + '"]');
                    currentCell.text('').removeClass().attr('data-rigip', '');
                }

                rig.attr({
                    'data-side': target.parents('.rig-placement').data('side'),
                    'data-x': target.index() - 1,
                    'data-y': target.parent().index()
                }).find('.badge').remove();

                RigPlacement.setCellStatus(target, rig.data('status'), reason, rig.data('gpus'), rig.data('rigip'));
                target.html(rig.text()).attr('data-rigip', rig.data('rigip'));
            }
        });

        if (!newShelf) {
            $('ul.list-group li').draggable({
                revert: true,
                revertDuration: 0,
                cursorAt: { left: 3, top: 3 },
                zIndex: 100,
                helper: function (event) {
                    var clone = $(this).clone();
                    clone.find('.badge').remove();
                    return $('<span class="label label-default">' + clone.text() + '</span>');
                }
            });
        }
    },

    renameHead: function (target) {
        var field = $('<input type="text" class="form-control input-sm" value="" style="padding:1px 3px; min-width:50px; border-radius:2px; height:20px">');
        field.val(target.text());

        this.target = target;
        target.data('currentName', target.text());
        target.html('').append(field);

        field.one('blur', $.proxy(this.setHeadName, this));
        field.select().focus();
    },

    setHeadName: function () {
        var newName = this.target.find('input').val();
        if (!newName.length) {
            newName = this.target.data('currentName');
        }
        this.target.html(newName);
        this.target.data('currentName', null);
    },

    isCorner: function (key, options) {
        return options.$trigger.html().length == 0;
    },

    /******************************************************/
    /* SAVE                                               */
    /******************************************************/
    save: function () {
        var data = [];
        var unassigned = [];
        var shelfs = [];
        $("[data-rigip]").has("span.badge:contains('unassigned')").each(
            function () { unassigned.push({ Rig_ip: $(this).data("rigip") }); }
        );

        $('#btnSave .fa').removeClass('fa-floppy-o').addClass('fa-circle-o-notch fa-spin');

        // Sestavíme data
        $('.shelf').each(function () {
            var shelfIndex = $(this).data('shelfid'),
                shelfRealId = $(this).data('shelfrealid'),
                shelf = {
                    id: shelfRealId ? shelfRealId * 1 : null,
                    height: $('.rig-placement', this).eq(0).find('tbody tr').length,
                    width: $('.rig-placement', this).eq(0).find('tbody tr:first-child td').length,
                    both_sides: $('.rig-placement', this).length == 2,
                    container_id: null,
                    name: $(this).find('.shelf-name').text(),
                    cols_names: { left: [], right: [] },
                    rows_names: { left: [], right: [] },
                    shelf_index: shelfIndex
                };

            $('.rig-placement', this).each(function () {

                var side = $(this).data('side');

                $('table thead th', this).each(function (i) {
                    if (i > 0) {
                        shelf.cols_names[side].push($(this).text());
                    }
                });

                $('table tbody tr', this).each(function () {
                    var y = $(this).index();
                    shelf.rows_names[side].push($(this).find('th').text());

                    $('td', this).each(function () {
                        var cell = $(this),
                            x = cell.index() - 1;/*,
                            rigIp = (cell.data('rigip') != "") ? cell.data('rigip') : cell.attr('data-rigip');*/
                        

                        //if (rigIp && rigIp.length) { // Buňka obsahuje referenci na rig
                            data.push({
                                //Rig_ip: rigIp,
                                Rig_name: cell.text().trim(),
                                X: x,
                                Y: y,
                                Side: side,
                                Shelf_id: shelfIndex
                            });
                        //}
                    });
                });
            });

            shelfs.push(shelf);
        });

        // Odešleme
        $.ajax({
            url: RigPlacement.getUrl() + '?button=btnSave' + '&modelId=' + GetUrlParameter("modelId"),
            type: 'post',
            dataType: 'json',
            data: {
                RigPlacementData: JSON.stringify({
                    data: { item: data },
                    unassignedRigs: { item: unassigned },
                    shelfs: { item: shelfs },
                    removedShelfs: RigPlacement.Edit.removedShelfs
                }),
                ShelfCount: RigPlacement.shelfCount
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

    /******************************************************/
    /* ACTIONS                                            */
    /******************************************************/
    _onAction: function (key, options) {
        if (key == 'remove') {
            this.removePlacement(options.$trigger);
        }
        if (key == 'rename') {
            this.renameHead(options.$trigger);
        }
    }
};

$($.proxy(RigPlacement.init, RigPlacement));


var CamSnapshots = {

    init: function () {
        $(document).on('click', '.btn-snapshot', this.takeSnapshot);
    },

    takeSnapshot: function () {
        var self = CamSnapshots;

        $(this).find('.fa').toggleClass('hide');

        $.ajax({
            url: self.getUrl() + '?button=btnTakeSnapshot' + '&modelId=' + GetUrlParameter("modelId"),
            type: 'post',
            dataType: 'json',
            data: { camNumber: this.value },
            success: $.proxy(self.showResult, self),
            complete: $.proxy(self.takeComplete, this)
        });
    },

    showResult: function (data) {
        var info = '';
        var type = '';
        if (data && data.message && data.message.length) {
            info = data.message;
            type = 'alert-success';

            $.ajax({
                url: this.getUrl() + '?button=Snapshots' + '&modelId=' + GetUrlParameter("modelId"),
                type: 'post',
                dataType: 'json',
                success: $.proxy(this.updateGallery, this)
            });
        }
        else {
            info = data.error && data.error.length ? data.error : 'An unexpected error occured';
            type = 'alert-danger';
        }

        var a = $('<p class="alert alert-dismissable ' + type + '"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>' + info + '</p>');
        $('#LiveStream .modal-body').prepend(a);
    },

    updateGallery: function (data) {
        var b = $('#Snapshots .modal-body');
        b.html('');

        for (var i = 0; i < data.rows.length; i++) {
            var row = data.rows[i];
            var t = '<div>' +
                '<div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">' +
                '<div>_label_</div>' +
                '<a href="_url_" target="_blank" class="thumbnail"><img src="_urlSmall_" alt="" class="img-responsive"></a>' +
                '</div>' +
                '</div>';

            b.append(t.replace(/_label_/, row.Label).replace(/_url_/, row.FileUrl).replace(/_urlSmall_/, row.ThumbUrl));
        }
        b.append('<div class="clearfix"></div>');
    },

    takeComplete: function () {
        $(this).find('.fa').toggleClass('hide');
    },

    getUrl: function () {
        return '/api/run/' + $('#currentAppName').val() + '/' + $('#currentBlockName').val();
    },
}

$($.proxy(CamSnapshots.init, CamSnapshots));