$(document).ready(function () {
    var logOutFlag = true;
    $('a, input[type=submit], button').click(function () {
        logOutFlag = false;
    });
    $(window).unload(function () {
        if (logOutFlag)
            $.ajax({
                url: '/CORE/Info/LogOut',
                async: false
            })
            .success(function () {
                $.ajax({
                    url: '/CORE/Info/Leave',
                    async: false
                });
            });
    });
});