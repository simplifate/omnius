$(function () {
    $("#overviewPanel").resizable();
    $("#btnAddBlock").on("click", function () {
        alert("TODO: Add new block");
    });
    $(".block").on("dblclick", function () {
        name = $(this).find(".blockName").text();
        alert('Double clicked "' + name + '"');
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
});
