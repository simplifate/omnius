var CurrentWsdlFile;
$(function () {
    if (CurrentModuleIs("nexusModule")) {
        showWsdlDialog = $("#show-wsdl-dialog").dialog({
            autoOpen: false,
            resizable: false,
            width: 800,
            height: 600,
            buttons: {
                "Close": function () {
                    showWsdlDialog.dialog("close");
                }
            },
            open: function () {
                $(this).find("#wsdlFileText").text(CurrentWsdlFile);
            }
        });
    }
});
