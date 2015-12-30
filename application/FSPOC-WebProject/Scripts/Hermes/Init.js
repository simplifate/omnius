$(function () {
    if (CurrentModuleIs("hermesModule")) {
        if ($("#smtpMenuArea").length) {
            $("#hermesMenuSMTP").addClass("highlighted");
        }
        if ($("#templateMenuArea").length) {
            $("#hermesMenuTemplate").addClass("highlighted");
        }
        $("#hermesMenuSMTP").on("click", function () {
            if (!$("#hermesMenuSMTP").hasClass("highlighted"))
                window.location.href = "/Hermes/SMTP";
        });
        $("#hermesMenuTemplate").on("click", function () {
            if (!$("#hermesMenuTemplate").hasClass("highlighted"))
                window.location.href = "/Hermes/Template";
        });
    }
});
