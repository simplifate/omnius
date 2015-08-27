function CreateWorkflow(name) {
    postData = {
        Name: name
    }

    $.ajax({
        type: "POST",
        url: "/api/workflows",
        data: postData,
        success: function (data) {
            CurrentWorkflowId = data.Id;
            CurrentWorkflowName = data.Name;
            $(".top-bar-container").text(CurrentWorkflowName);
            ClearWorkflow();
            alert("OK");
        },
        error: function () { alert("ERROR") }
    });
}