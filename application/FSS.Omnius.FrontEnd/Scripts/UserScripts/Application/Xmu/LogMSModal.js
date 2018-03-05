$(function(){
  $("#Profiles_table").find(".fa-edit").attr("data-toggle","modal").attr("data-target","#modalProfile");
  
  $("#Profiles_table").on("click", ".fa-edit", function () {
            var userId = $(this).parent().parent().children("td:first").text();
            $("#modal_body_profile").html("");
  			$("#modal_name_profile").html("");
            $("#preloader_profile").css("display", "block");
            $.ajax({
                type: 'POST',
                url: "/Xmu/LogMSDetail?modelId="+userId,
                success: function (response) {
                    var x = $(response)
                    $("#preloader_profile").css("display", "none");
                    $("#modal_body_profile").html(x.find("#modal_body_profile").html());
                    $("#modal_name_profile").html(x.find("#modal_name_profile").html());
                }
            });
        }); 
});