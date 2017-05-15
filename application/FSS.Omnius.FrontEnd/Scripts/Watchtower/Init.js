$(function () {
    $("#TimeSince,#TimeTo").datetimepicker({
        datepicker: true,
        timepicker: true,
        step: 5,
        format: "d.m.Y H:i"
    });
});
