$(function () {
    $("#moduleSwitcher").on("change", function () {
        option = $("#moduleSwitcher").val();
        switch (option) {
            case "apps":
                window.location.href = "/";
                break;
            case "workflow":
                window.location.href = $("#wfdesigner").attr("href");
                break;
            case "database":
                window.location.href = "/DbDesigner";
                break;
            case "block":
                window.location.href = "/Block/Index/1";
                break;
            case "rules":
                window.location.href = "/rules";
                break;
            case "tapestry":
                window.location.href = $("#tapestry").attr("href");
                break;
            case "overview":
                window.location.href = $("#overview").attr("href");
                break;
        }
    });
});