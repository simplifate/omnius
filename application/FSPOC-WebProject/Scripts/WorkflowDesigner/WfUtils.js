$(function () {
    $("#mode-normal input[type=radio]").change(function () {
        DeleteModeActive = false;
    });
    $("#mode-delete input[type=radio]").change(function () {
        DeleteModeActive = true;
    });
})

function ClearWorkflow() {
    jQuery.each($("#workflow-container .activity"), function (i, val) {
        instance.removeAllEndpoints(val, true);
        val.remove();
    });
};
