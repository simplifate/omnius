$(function(){
  $("#Support_request_table").find(".fa-edit").attr("data-toggle","modal").attr("data-target","#modalTicket");
  
  $("#Support_request_table").on("click", ".fa-edit", function () {
            var SupportID = $(this).parent().parent().children("td:first-child").html();
            $("#modal_body_ticket").html("");
            $("#modal_name_ticket").html("");
            $("#preloader_ticket").css("display", "block");
            $.ajax({
                dataType: "html",
                url: "/Xmu/Ticket?modelId=" + SupportID,
                success: function (response) {
                    var x = $(response)
                    $("#preloader_ticket").css("display", "none");
                    $("#modal_body_ticket").html(x.find("#modal_body_ticket").html());
                    $("#modal_name_ticket").html(x.find("#modal_name_ticket").html());
                }
            });
        }); 
	var reload = function () {
            var SupportID = $("#model_id").val();
            $("#modal_body_ticket").html("");
            $("#modal_name_ticket").html("");
            $("#preloader_ticket").css("display", "block");
            $.ajax({
                dataType: "html",
                url: "/Xmu/Ticket?modelId=" + SupportID,
                success: function (response) {
                    var x = $(response)
                    $("#preloader_ticket").css("display", "none");
                    $("#modal_body_ticket").html(x.find("#modal_body_ticket").html());
                    $("#modal_name_ticket").html(x.find("#modal_name_ticket").html());
                }
            });
        }
    $(document).on("click", "#saveSendButton", function () {
            $.ajax({
                type: 'POST',
                url: '/api/run/Xmu/' + $('#currentBlockName').val() + '/?button=saveSendButton',
                data: { 'new_comment': $("#new_comment").val(), "TicketID": $("#model_id").val(), "ticketTag_input": $("#ticketTag_input").val()},
                success: function (data) {
                    reload();
                }
            });
        });
  $(document).on("click", ".fa-remove", function (e) {
    alert("bagr");
    var Tag = e.target.parent().parent().parent().child("#tag_hiddenSpan").html();
            $.ajax({
                type: 'POST',
                url: '/api/run/Xmu/' + $('#currentBlockName').val() + '/?button=total_balance',
                data: { 'deleteTag': Tag},
                success: function (data) {
                    reload();
                }
            });
        });
});


$(function(){
  $("#Profiles_table").find(".fa-edit").attr("data-toggle","modal").attr("data-target","#modalProfile");
  
  $("#Profiles_table").on("click", ".fa-edit", function () {
            var UserHash = $(this).parent().parent().children("td:nth-child(3)").html();
            $("#modal_body_profile").html("");
            $("#modal_body_cash").html("");
  			$("#modal_name_profile").html("");
    		$("#modal_modal_profile").html("");
            $("#preloader_profile").css("display", "block");
            $.ajax({
                type: 'POST',
                url: "/Xmu/Profile?button=headingHash",
                data: { 'UserHash': UserHash},
                success: function (response) {
                    var x = $(response)
                    $("#preloader_profile").css("display", "none");
                    $("#modal_body_profile").html(x.find("#modal_body_profile").html());
                    $("#modal_modal_profile").html(x.find("#modal_modal_profile").html());
                    $("#modal_body_cash").html(x.find("#modal_body_cash").html());
                    $("#modal_name_profile").html(x.find("#modal_name_profile").html());
                  	var tables = $("#datatableCrypto").add("#datatableFiat").add("#datatableTrades").add("#datatableWithdrawals").add("#datatableDeposits").add("#datatablePending").add("#datatableSupport");
                    tables.each(function() {
                      var table = $(this);
                      BootstrapUserInit.DataTable.initTable(table);
                    });
                }
            });
        }); 
});
