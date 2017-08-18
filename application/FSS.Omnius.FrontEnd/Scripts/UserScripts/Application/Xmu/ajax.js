$(function(){
  $("#Support_request_table").find(".fa-edit").attr("data-toggle","modal").attr("data-target","#modalTicket");
  
  $("#Support_request_table").on("click", ".fa-edit", function () {
            var SupportID = $(this).parent().parent().children("td:first-child").html();
            $("#modal_body_ticket").html("");
            $("#modal_name_ticket").html("");
            $("#preloader").css("display", "block");
            $.ajax({
                dataType: "html",
                url: "/Xmu/Ticket?modelId=" + SupportID,
                success: function (response) {
                    var x = $(response)
                    $("#preloader").css("display", "none");
                    $("#modal_body_ticket").html(x.find("#modal_body_ticket").html());
                    $("#modal_name_ticket").html(x.find("#modal_name_ticket").html());
                }
            });
        }); 
	var reload = function () {
            var SupportID = $("#model_id").val();
            $("#modal_body_ticket").html("");
            $("#modal_name_ticket").html("");
            $("#preloader").css("display", "block");
            $.ajax({
                dataType: "html",
                url: "/Xmu/Ticket?modelId=" + SupportID,
                success: function (response) {
                    var x = $(response)
                    $("#preloader").css("display", "none");
                    $("#modal_body_ticket").html(x.find("#modal_body_ticket").html());
                    $("#modal_name_ticket").html(x.find("#modal_name_ticket").html());
                }
            });
        }
    $(document).on("click", "#saveSendButton", function () {
            $.ajax({
                type: 'POST',
                url: '/api/run/Xmu/' + $('#currentBlockName').val() + '/?button=saveSendButton',
                data: { 'new_comment': $("#new_comment").val(), "TicketID": $("#model_id").val()},
                success: function (data) {
                    reload();
                }
            });
        });
});