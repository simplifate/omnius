$(function () {
    $("#TimeSince,#TimeTo").datetimepicker({
        datepicker: true,
        timepicker: true,
        step: 5,
        format: "d.m.Y H:i"
    });

    function ShowDetail(id, width) {
        $.getJSON("/Watchtower/Log/GetRow/" + id, function (data) {
            var d = $('<div title="Record details..."></div>');
            var template = $('#detail').html()
                .replace('{time}', data.Time)
                .replace('{level}', data.Level)
                .replace('{user}', data.User)
                .replace('{server}', data.Server)
                .replace('{source}', data.Source)
                .replace('{application}', data.Application)
                .replace('{block}', data.Block)
                .replace('{action}', data.Action)
                .replace('{message}', data.Message)
                .replace('{vars}', data.Vars)
                .replace('{st}', data.StackTrace)
                .replace('{innerId}', data.Inner && data.Inner.Id ? data.Inner.Id : "");

            d.html(template);

            if (data.Inner == null) {
                d.find('.row-innerex').remove();
            }

            d.dialog({
                modal: true,
                draggable: false,
                resizable: false,
                width: width,
                maxHeight: $(window).height() * 0.9,
                close: function () {
                    $(this).dialog('destroy').remove();
                }
            });
        });
    }

    $(document).on('click', "body.watchtowerModule #lowerPanel .fa-search", function () {
        var row = $(this).parents('tr').eq(0);
        var id = row.attr('data-id');

        ShowDetail(id, '80%');
        return false;
    })
        .on('click', "a[data-innerid]", function () {
            var id = $(this).attr('data-innerid');

            ShowDetail(id, '70%');
            return false;
        })
        .on('click', '.pagination a[data-page]', function () {
            $('#PageNumber').val($(this).attr('data-page'));
            $('#filterLogForm').submit();
            return false;
        })
        .on('click', '#filterLogForm #resetSearchForm', function () {
            $('#filterLogForm').find('select').each(function () { this.selectedIndex = 0; }).end()
                .find('input[type=text]').val('').end()
                .submit();
        });
});
