$(function () {
    $("#TimeSince,#TimeTo").datetimepicker({
        datepicker: true,
        timepicker: true,
        step: 5,
        format: "d.m.Y H:i"
    });

    $("body.watchtowerModule #lowerPanel .getJsonRow").click(function () {
        var row = $(this).parent();
        var id = row.attr('data-id');
        var varsCell = row.find('[data-data="Vars"]');
        var StackTraceCell = row.find('[data-data="StackTrace"]');

        $.getJSON("/Watchtower/Log/GetRow/" + id, function (data) {
            // Vars cell
            varsCell.removeClass('getJsonRow').addClass('mytooltip');
            varsCell.html(
                'Count: ' + (data.Vars.match(/<tr>/g) || []).length +
                '<div class="tooltiptext" >' + data.Vars + '</div>');

            // StackTrace cell
            StackTraceCell.removeClass('getJsonRow').addClass('mytooltip');
            StackTraceCell.html(
                'Show' +
                '<div class="tooltiptext" >' + data.StackTrace + '</div>');

            // Inner
            var inner = data.Inner;
            var parentRow = row;
            while (inner != null)
            {
                var htmlRow =
                    '<tr data-id="' + inner.Id + '"><td>Inner</td>' +
                    '<td>' + inner.LogLevelString + '</td>' +
                    '<td>' + inner.UserName + '</td>' +
                    '<td>' + inner.Server + '</td>' +
                    '<td>' + inner.LogSourceString + '</td>' +
                    '<td>' + inner.Application + '</td>' +
                    '<td>' + inner.BlockName + '</td>' +
                    '<td>' + inner.ActionName + '</td>' +
                    '<td>' + inner.Message + '</td>' +
                    '<td class="mytooltip">Show<div class="tooltiptext">' + inner.Vars + '</div></td>' +
                    '<td class="mytooltip">Show<div class="tooltiptext">' + inner.StackTrace + '</div></td></tr>';
                parentRow.after(htmlRow);

                inner = inner.ChildLogItems[0];
                parentRow = parentRow.next();
            }
        });
    });
});
