var maintenanceModeActive = false;

var pageSpinner = (function () {
    var uses = 1;
    return {
        show: function (n) {
            if (!arguments.length) {
                n = 1;
            }
            if (n) {
                $(document.body).addClass("pageSpinnerShown");
            }
            uses += n;
        },
        hide: function (n) {
            if (!arguments.length) {
                n = 1;
            }
            uses -= n;
            if (!uses) {
                $(document.body).removeClass("pageSpinnerShown");
            }
        }
    }
})()

$(function () {
    pageSpinner.hide();
    $(window).on("beforeunload", function () {
        pageSpinner.show();
    });

    $("#identitySuperMenu").on("click", function () {
        $("#leftBar .leftMenu li.identitySubMenu").slideToggle();
    });
    $("#appSuperMenu").on("click", function () {
        $("#leftBar .leftMenu li.appSubMenu").slideToggle();
    });

    if (CurrentModuleIs("portalModule")) {
        $("#adminMenuPortal").addClass("active");
    }
    else if (CurrentModuleIs("adminAppModule")) {
        $("#adminMenuApps").addClass("active");
    }
    else if (CurrentModuleIs("nexusModule")) {
        $("#adminMenuNexus").addClass("active");
    }
    else if (CurrentModuleIs("tapestryModule") || CurrentModuleIs("overviewModule")) {
        $("#adminMenuTapestry").addClass("active");
        $("#leftBar .leftMenu li.appSubMenu").show();
    }
    else if (CurrentModuleIs("mozaicModule") || CurrentModuleIs("mozaicEditorModule")) {
        $("#adminMenuMozaic").addClass("active");
        $("#leftBar .leftMenu li.appSubMenu").show();
    }
    else if (CurrentModuleIs("dbDesignerModule")) {
        $("#adminMenuDbDesigner").addClass("active");
        $("#leftBar .leftMenu li.appSubMenu").show();
    }
    else if (CurrentModuleIs("personaModule") || CurrentModuleIs("personaRolesModule")) {
        $("#adminMenuPersona").addClass("active");
        $("#leftBar .leftMenu li.identitySubMenu").show();
    }
    else if (CurrentModuleIs("personaModulesModule")) {
        $("#adminMenuPersonaModules").addClass("active");
        $("#leftBar .leftMenu li.identitySubMenu").show();
    }
    else if (CurrentModuleIs("watchtowerModule")) {
        $("#adminMenuWatchtower").addClass("active");
    }
    else if (CurrentModuleIs("hermesModule")) {
        $("#adminMenuHermes").addClass("active");
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
    $("#hideUpperPanelIcon").on("click", function () {
        $("#upperPanel").hide();
        $("#minimizedUpperPanel").show();
        $("#lowerPanel").addClass("minimizedPanelMode");
        if (CurrentModuleIs("tapestryModule"))
            RecalculateToolboxHeight();
        else if (CurrentModuleIs("mozaicEditorModule"))
            RecalculateMozaicToolboxHeight();
    });
    $("#showUpperPanelIcon").on("click", function () {
        $("#minimizedUpperPanel").hide();
        $("#upperPanel").show();
        $("#lowerPanel").removeClass("minimizedPanelMode");
        if (CurrentModuleIs("tapestryModule"))
            RecalculateToolboxHeight();
        else if (CurrentModuleIs("mozaicEditorModule"))
            RecalculateMozaicToolboxHeight();
    });
    $("#topBar").width($(window).width());
    $("#upperPanel").width($(window).width() - 225);
    $("#minimizedUpperPanel").width($(window).width() - 225);
    $(window).on("resize", function () {
        $("#topBar").width($(window).width());
        $("#upperPanel").width($(window).width() - 225);
        $("#minimizedUpperPanel").width($(window).width() - 225);
    });
});
