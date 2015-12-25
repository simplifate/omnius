﻿$(function () {
    if (CurrentModuleIs("nexusModule")) {
        if ($("#ldapMenuArea").length) {
            $("#nexusMenuLDAP").addClass("highlighted");
        }
        else if ($("#wsMenuArea").length) {
            $("#nexusMenuWebServices").addClass("highlighted");
        }
        else if ($("#extDbMenuArea").length) {
            $("#nexusMenuExtDB").addClass("highlighted");
        }
        else if ($("#webDavMenuArea").length) {
            $("#nexusMenuWebDav").addClass("highlighted");
        }
        $("#nexusMenuLDAP").on("click", function () {
            if(!$("#nexusMenuLDAP").hasClass("highlighted"))
                window.location.href = "/Nexus/LDAP";
        });
        $("#nexusMenuWebServices").on("click", function () {
            if (!$("#nexusMenuWebServices").hasClass("highlighted"))
                window.location.href = "/Nexus/WS";
        });
        $("#nexusMenuExtDB").on("click", function () {
            if (!$("#nexusMenuExtDB").hasClass("highlighted"))
                window.location.href = "/Nexus/ExtDB";
        });
        $("#nexusMenuWebDav").on("click", function () {
            if (!$("#nexusMenuWebDav").hasClass("highlighted"))
                window.location.href = "/Nexus/WebDAV";
        });
    }
});
function ShowWsdlButtonClick(button) {
    encodedString = $(button).parents("td").find(".wsdlFileString").text();
    CurrentWsdlFile = $("<div/>").html(encodedString).text();
    showWsdlDialog.dialog("open");
};
