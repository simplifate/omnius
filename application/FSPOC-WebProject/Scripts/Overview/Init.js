$(function () {
    if (CurrentModuleIs("overviewModule")) {
        $("#overviewPanel").resizable();
        $("#btnAddBlock").on("click", function () {
            addBlockDialog.dialog("open");
        });
        $(".block").on("dblclick", function () {
            // TODO: Pass block's ID to Tapestry to edit this block
            window.location.href = "/tapestry";
        });
        $.contextMenu({
            selector: '.block',
            trigger: 'right',
            zIndex: 300,
            callback: function (key, options) {
                if (key == "delete") {
                    instance.removeAllEndpoints(options.$trigger, true);
                    options.$trigger.remove();
                }
            },
            items: {
                "delete": { name: "Delete", icon: "delete" }
            }
        });
    }
});
