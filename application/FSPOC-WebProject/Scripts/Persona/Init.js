$(function () {
    if (CurrentModuleIs("personaModulesModule")) {
        $("#moduleAccessTable .checkboxCell").on("click", function () {
            checkboxCell = $(this);
            if (checkboxCell.hasClass("yesCell")) {
                checkboxCell.removeClass("yesCell");
                checkboxCell.addClass("noCell");
                checkboxCell.text("Ne");
            }
            else {
                checkboxCell.removeClass("noCell");
                checkboxCell.addClass("yesCell");
                checkboxCell.text("Ano");
            }
        });
        $("#btnSaveModuleAccessTable").on("click", function () {
            SaveModulePermissions();
        });
        $("#btnReloadModuleAccessTable").on("click", function () {
            location.reload();
        });
    }
});
