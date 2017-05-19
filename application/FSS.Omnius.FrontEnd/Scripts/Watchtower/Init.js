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
        });
    });
});
