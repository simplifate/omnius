function SaveModulePermissions() {
    permissionArray = [];

    moduleAccessTable = $("#moduleAccessTable");


        alert(moduleAccessTable.html());
    

   
    moduleAccessTable.find("td.userIds").each(function (index, element) {
        if(index > 0)
            permissionArray.push({ UserId: parseInt($(element).text()) })
    });
    moduleAccessTable.find("thead th").each(function (index, element) {
        if (index > 0)
            permissionArray[index - 1].UserName = $(element).text();
    });

    var modules = ["Core", "Master","Tapestry","Entitron","Mozaic","Persona","Nexus","Sentry","Hermes","Athena","Watchtower","Cortex"];

    $.each(modules, function (index, object) {
        moduleAccessTable.find("tr[moduleId='" + object + "'] td").each(function (index, element) {
            if (index > 0)
                permissionArray[index - 1][object] = $(element).hasClass("yesCell");
        });
    });
    
    postData = {
        PermissionList: permissionArray
    };
    $.ajax({
        type: "POST",
        url: "/api/persona/module-permissions",
        data: postData,
        success: function () { alert("OK") }
    });
}
