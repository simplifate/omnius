$(function () {
    $("#hideMenuIcon").on("click", function () {
        $("#userLeftBar").hide();
        $("#userLeftBarMinimized").show();
        $("#userContentArea").css("left", 60);
        $("#helpContainer").css("left", 60);
    });
    $("#showMenuIcon").on("click", function () {
        $("#userLeftBarMinimized").hide();
        $("#userLeftBar").show();
        $("#userContentArea").css("left", 310);
        $("#helpContainer").css("left", 310);
    });
    if (CurrentModuleIs("appManagerModule")) {
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
        $(".appPanel").on("dblclick", function () {
            window.location.href = $("#fakeAppPath").val();
        });
    }
    else if (CurrentModuleIs("helpModule")) {
        $("#appManagerIcon").removeClass("activeIcon");
        $("#helpIcon").addClass("activeIcon");
    }
    else if (CurrentModuleIs("userDetailsModule")) {
        $("#appManagerIcon").removeClass("activeIcon");
    };
    if ($("#userLeftBar").length > 0) {
        $("#userLeftBar").css("height", $(window).height() + $(window).scrollTop() - 50);
        $(window).scroll(function () {
            $("#userLeftBar").css("height", $(window).height() + $(window).scrollTop() - 50);
        });
        $(window).resize(function () {
            $("#userLeftBar").css("height", $(window).height() + $(window).scrollTop() - 50);
        });
    }
});
