$(function () {
    $("#moduleSwitcher").on("change", function () {
        option = $("#moduleSwitcher").val();
        switch (option) {
            case "apps":
                window.location.href = "/apps";
                break;
            case "workflow":
                window.location.href = "/workflow";
                break;
            case "database":
                window.location.href = "/database";
                break;
            case "tapestry":
                window.location.href = "/tapestry";
                break;
        }
    });
});