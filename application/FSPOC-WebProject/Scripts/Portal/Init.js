var maintenanceModeActive = false;

$(function () {
    $("#leftBar .leftMenu li.expanded").on("click", function () {
        $("#leftBar .leftMenu li.subMenu").slideToggle();
    });

    if (CurrentModuleIs("portalModule"))
        $("#adminMenuPortal").addClass("active");
    else if (CurrentModuleIs("adminAppModule"))
        $("#adminMenuApps").addClass("active");
    else if (CurrentModuleIs("portalModule"))
        $("#adminMenuApps").addClass("active");
    else if (CurrentModuleIs("nexusModule"))
        $("#adminMenuNexus").addClass("active");
    else if (CurrentModuleIs("tapestryModule")) {
        $("#adminMenuTapestry").addClass("active");
        $("#leftBar .leftMenu li.subMenu").show();
    }
    else if (CurrentModuleIs("mozaicModule")) {
        $("#adminMenuMozaic").addClass("active");
        $("#leftBar .leftMenu li.subMenu").show();
    }
    else if (CurrentModuleIs("dbDesignerModule")) {
        $("#adminMenuDbDesigner").addClass("active");
        $("#leftBar .leftMenu li.subMenu").show();
    }

    $("#usersOnlineIndicator").on("click", function () {
        $(".clickableIndicatorRectangle").removeClass("highlighted");
        $("#usersOnlineIndicator").addClass("highlighted");
        $.get("/Portal/UsersOnline").success(function (result) {
            $("#lowerPanelDynamicContainer").html(result);
        });
    });
    $("#activeProfileIndicator").on("click", function () {
        $(".clickableIndicatorRectangle").removeClass("highlighted");
        $("#activeProfileIndicator").addClass("highlighted");
        $.get("/Portal/ActiveProfile").success(function (result) {
            $("#lowerPanelDynamicContainer").html(result);
        });
    });
    $("#activeModulesIndicator").on("click", function () {
        $(".clickableIndicatorRectangle").removeClass("highlighted");
        $("#activeModulesIndicator").addClass("highlighted");
        $.get("/Portal/ModuleAdmin").success(function (result) {
            $("#lowerPanelDynamicContainer").html(result);
            LoadModuleAdminScript();
        });
    });
    $("#maintenanceIndicator").on("click", function () {
        if (maintenanceModeActive) {
            $("#maintenanceIndicator").removeClass("maintenanceActive");
            $("#maintenanceIndicator .indicatorLabel").text("vypnuta");
            maintenanceModeActive = false;
        }
        else {
            $("#maintenanceIndicator").addClass("maintenanceActive");
            $("#maintenanceIndicator .indicatorLabel").text("zapnuta");
            maintenanceModeActive = true;
        }
    });
    $("#notificationArea .indicatorBar").on("click", function () {
        $(this).remove();
    });
    $("#hidePortalPanelIcon").on("click", function () {
        $("#centralAdminPanel").hide();
        $("#minimizedCentralAdminPanel").show();
        $("#lowerPanel").css("top", 115);
    });
    $("#showPortalPanelIcon").on("click", function () {
        $("#minimizedCentralAdminPanel").hide();
        $("#centralAdminPanel").show();
        $("#lowerPanel").css("top", 432);
    });
});
