$(function () {
    $("#Profiles_table").find(".fa-edit").attr("data-toggle", "modal").attr("data-target", "#modalProfile");

    $("#Profiles_table").on("click", ".fa-edit", function () {
        var userId = $(this).parent().parent().children("td:nth-child(2)").text();
        $("#modal_body_profile").html("");
        $("#modal_name_profile").html("");
        $("#preloader_profile").css("display", "block");
        $.ajax({
            type: 'GET',
            url: "/VozovyPark,
            success: function (response) {
                var x = $(response)
                $("#preloader_profile").css("display", "none");
                $("#modal_body_profile").html(x.find("#modal_body_profile").html());
                $("#modal_name_profile").html(x.find("#modal_name_profile").html());
            }
        });
    });
});
$(function () {
    var reload = function () {
        var userId = $("#hiddenUserId").val();
        $("#modal_body_profile").html("");
        $("#modal_name_profile").html("");
        $("#preloader_profile").css("display", "block");
        $.ajax({
            dataType: "html",
            url: "/VozovyPark,
            success: function (response) {
                var x = $(response)
                $("#preloader_profile").css("display", "none");
                $("#modal_body_profile").html(x.find("#modal_body_profile").html());
                $("#modal_name_profile").html(x.find("#modal_name_profile").html());
            }
        });
    };
});