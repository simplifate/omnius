$(function () {
    $("#appManagerIcon").on("click", function () {
        if(!$("#appManagerIcon").hasClass("activeIcon"))
            window.location.href = "/Master";
    });
    $("#adminSectionIcon").on("click", function () {
        if (!$("#adminSectionIcon").hasClass("activeIcon"))
            window.location.href = "/CORE";
    });
    $("#helpIcon").on("click", function () {
        if (!$("#helpIcon").hasClass("activeIcon"))
            window.location.href = "/Master/Home/Help";
    });
    $("#hideMenuIcon").on("click", function () {
        $("#userLeftBar").hide();
        $("#userLeftBarMinimized").show();
        $(".appWorkspace").css("left", 60);
        $("#helpContainer").css("left", 60);
    });
    $("#showMenuIcon").on("click", function () {
        $("#userLeftBarMinimized").hide();
        $("#userLeftBar").show();
        $(".appWorkspace").css("left", 310);
        $("#helpContainer").css("left", 310);
    });
    if ($("body.appManagerModule").length) {
        $(".appPanel").draggable({
            grid: [120, 120],
            revert: "invalid",
            stop: function () {
                $(this).draggable("option", "revert", "invalid");
            }
        });
        $(".appWorkspace").droppable({
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
    else if ($("body.adminAppTableModule").length) {
        $(".adminAppTable .actions .btn").on("click", function () {
            window.location.href = "/Portal/AppValidation";
        });
    }
    else if ($("body.helpModule").length) {
        $("#appManagerIcon").removeClass("activeIcon");
        $("#helpIcon").addClass("activeIcon");
    };
});
