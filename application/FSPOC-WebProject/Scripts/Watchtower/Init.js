$(function () {
    $("#filterSelectType").val($("#previousSearchType").val());
    $("#filterSelectLevel").val($("#previousSearchLevel").val());
    $("#filterSelectSource").val($("#previousSearchSource").val());
    $("#filterSelectUser").val($("#previousSearchUser").val());
    $("#filterSearchMessage").val($("#previousSearchMessage").val());
    $("#filterSearchTimeFrom").val($("#previousSearchTimeFrom").val());
    $("#filterSearchTimeTo").val($("#previousSearchTimeTo").val());
    $("#showFilterIcon").on("click", function () {
        $("#filterFormPanelMinimized").hide();
        $("#filterFormPanel").show();
    });
    $("#hideFilterIcon").on("click", function () {
        $("#filterFormPanel").hide();
        $("#filterFormPanelMinimized").show();
    });
});
