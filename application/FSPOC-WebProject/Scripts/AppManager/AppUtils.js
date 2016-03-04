function ShowAppNotification(text, type) {
    type = type || "info";
    $("#appNotificationArea .app-alert").remove();
    switch (type) {
        case "success":
            icon = "fa-check";
            break;
        case "error":
            icon = "fa-exclamation-circle";
            break;
        case "warning":
            icon = "fa-exclamation";
            break;
        case "info":
            icon = "fa-info-circle";
            break;
    }
    newNotification = $('<div class="app-alert app-alert-' + type + '"><i class="fa ' + icon + ' alertSymbol"></i>'
        + text + '<div class="fa fa-times closeAlertIcon"></div></div>');
    $("#appNotificationArea").append(newNotification);
    newNotification.find(".closeAlertIcon").on("click", function () {
        $(this).parents(".app-alert").remove();
    });
};
