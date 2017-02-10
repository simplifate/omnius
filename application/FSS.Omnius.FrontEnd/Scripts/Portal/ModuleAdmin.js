function LoadModuleAdminScript() {
    $("#moduleAdminPanel .moduleSquare").on("click", function () {
        $("#moduleAdminPanel .moduleSquare").removeClass("selectedSquare");
        $(this).addClass("selectedSquare");
        $("#moduleConfigPanel .currentModuleIcon").css("background-image", $(this).css("background-image"));
        $("#moduleConfigPanel .currentModuleName").text($(this).attr("moduleName"));
    });
}
$(function () {
    if ($("#moduleAdminPanel").length) {
        LoadModuleAdminScript();
    }
});