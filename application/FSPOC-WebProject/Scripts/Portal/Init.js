$(function () {
    $("#moduleAdminPanel .moduleSquare").on("click", function () {
        $("#moduleAdminPanel .moduleSquare").removeClass("selectedSquare");
        $(this).addClass("selectedSquare");
    });
    $("#leftBar .leftMenu li.expanded").on("click", function () {
        $("#leftBar .leftMenu li.subMenu").slideToggle();
    });
});