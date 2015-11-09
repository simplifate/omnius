$(function () {
    if ($("body.appManagerModule").length) {
        $(".appPanel").draggable({
            grid: [120, 120],
            revert: "invalid",
            stop: function(){
                $(this).draggable("option", "revert", "invalid");
            }
        });
        $(".appManagerModule").droppable({
            tolerance: "fit"
        });
        $(".appPanel").droppable({
            greedy: true,
            tolerance: "touch",
            drop: function (event, ui) {
                ui.draggable.draggable("option", "revert", true);
            }
        });
        $(".appPanel").bind("dragstart", function (event, ui) {
            ui.originalPosition.top = $(this).position().top;
            ui.originalPosition.left = $(this).position().left;
        });
    }
});
