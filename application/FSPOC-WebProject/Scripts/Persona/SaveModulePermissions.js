function SaveModulePermissions() {
    permissionArray = [];

    var rows = $("#moduleAccessTable").dataTable().fnGetNodes();
    for (var i = 0; i < rows.length; i++) {
        var userId = parseInt($(rows[i]).find("td:eq(0)").text());
      
        permissionArray.push({
            UserId: userId,
            Core: ($(rows[i]).find("td[moduleId=Core]").hasClass("yesCell")),
            Master: ($(rows[i]).find("td[moduleId=Master]").hasClass("yesCell")),
            Tapestry: ($(rows[i]).find("td[moduleId=Tapestry]").hasClass("yesCell")),
            Entitron: ($(rows[i]).find("td[moduleId=Entitron]").hasClass("yesCell")),
            Persona: ($(rows[i]).find("td[moduleId=Persona]").hasClass("yesCell")),
            Nexus: ($(rows[i]).find("td[moduleId=Nexus]").hasClass("yesCell")),
            Sentry: ($(rows[i]).find("td[moduleId=Sentry]").hasClass("yesCell")),
            Hermes: ($(rows[i]).find("td[moduleId=Hermes]").hasClass("yesCell")),
            Athena: ($(rows[i]).find("td[moduleId=Athena]").hasClass("yesCell")),
            Watchtower: ($(rows[i]).find("td[moduleId=Watchtower]").hasClass("yesCell")),
            Cortex: ($(rows[i]).find("td[moduleId=Cortex]").hasClass("yesCell")),

        });
    }
   



    
    
    postData = {
        PermissionList: permissionArray
    };
    $.ajax({
        type: "POST",
        url: "/api/persona/module-permissions",
        data: postData,
        success: function () { alert("Module permissions has been updated!") }
    });
}
