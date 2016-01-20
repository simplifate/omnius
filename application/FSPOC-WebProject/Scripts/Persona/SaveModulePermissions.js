function SaveModulePermissions() {
    permissionArray = [];
    moduleAccessTable = $("#moduleAccessTable");

    moduleAccessTable.find("tr.hiddenIdRow td").each(function (index, element) {
        if(index > 0)
            permissionArray.push({ Id: parseInt($(element).text()) })
    });
    moduleAccessTable.find("thead th").each(function (index, element) {
        if (index > 0)
            permissionArray[index - 1].UserName = $(element).text();
    });

    moduleAccessTable.find("tr[moduleId='Core'] td").each(function (index, element) {
        if (index > 0)
            permissionArray[index - 1].Core = $(element).hasClass("yesCell");
    });
    moduleAccessTable.find("tr[moduleId='Master'] td").each(function (index, element) {
        if (index > 0)
            permissionArray[index - 1].Master = $(element).hasClass("yesCell");
    });
    moduleAccessTable.find("tr[moduleId='Tapestry'] td").each(function (index, element) {
        if (index > 0)
            permissionArray[index - 1].Tapestry = $(element).hasClass("yesCell");
    });
    moduleAccessTable.find("tr[moduleId='Entitron'] td").each(function (index, element) {
        if (index > 0)
            permissionArray[index - 1].Entitron = $(element).hasClass("yesCell");
    });
    moduleAccessTable.find("tr[moduleId='Mozaic'] td").each(function (index, element) {
        if (index > 0)
            permissionArray[index - 1].Mozaic = $(element).hasClass("yesCell");
    });
    moduleAccessTable.find("tr[moduleId='Persona'] td").each(function (index, element) {
        if (index > 0)
            permissionArray[index - 1].Persona = $(element).hasClass("yesCell");
    });
    moduleAccessTable.find("tr[moduleId='Nexus'] td").each(function (index, element) {
        if (index > 0)
            permissionArray[index - 1].Nexus = $(element).hasClass("yesCell");
    });
    moduleAccessTable.find("tr[moduleId='Sentry'] td").each(function (index, element) {
        if (index > 0)
            permissionArray[index - 1].Sentry = $(element).hasClass("yesCell");
    });
    moduleAccessTable.find("tr[moduleId='Hermes'] td").each(function (index, element) {
        if (index > 0)
            permissionArray[index - 1].Hermes = $(element).hasClass("yesCell");
    });
    moduleAccessTable.find("tr[moduleId='Athena'] td").each(function (index, element) {
        if (index > 0)
            permissionArray[index - 1].Athena = $(element).hasClass("yesCell");
    });
    moduleAccessTable.find("tr[moduleId='Watchtower'] td").each(function (index, element) {
        if (index > 0)
            permissionArray[index - 1].Watchtower = $(element).hasClass("yesCell");
    });
    moduleAccessTable.find("tr[moduleId='Cortex'] td").each(function (index, element) {
        if (index > 0)
            permissionArray[index - 1].Cortex = $(element).hasClass("yesCell");
    });
    postData = {
        PermissionList: permissionArray
    };
    $.ajax({
        type: "POST",
        url: "/api/persona/module-permissions",
        data: postData,
        success: function () { alert("OK") },
        error: function () { alert("ERROR") }
    });
}
