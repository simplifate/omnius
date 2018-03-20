$(function(){
  $("#Support_request_table").find(".fa-edit").attr("data-toggle","modal").attr("data-target","#modalTicket");
  
  $(document).on("click", "#Support_request_table .fa-edit", function (e) {
            var SupportID = $(e.target).parent().parent().children("td:first-child").html();
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
    var Tag = $(e.target).parents(".tag").children("#tag_hiddenSpan").html();
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
                url: '/Persona/Account/GetAntiForgeryToken',
                type: 'GET',
                success: function (token) {
                    $.ajax({
                        type: 'POST',
                        url: "/Xmu/Profile?button=headingHash",
                        data: { 'UserHash': UserHash, '__RequestVerificationToken': token },
                        success: function (response) {
                            var x = $(response)
                            $("#preloader_profile").css("display", "none");
                            $("#modal_body_profile").html(x.find("#modal_body_profile").html());
                            $("#modal_modal_profile").html(x.find("#modal_modal_profile").html());
                            $("#modal_body_cash").html(x.find("#modal_body_cash").html());
                            $("#modal_name_profile").html(x.find("#modal_name_profile").html());
                          	$("#inputDate").datetimepicker({
                                datepicker: true,
                                timepicker: false,
                                format: "d.m.Y"
                            }).off('mousewheel');
                            $("#modalProfile #Support_request_table").find(".fa-edit").attr("data-toggle", "modal").attr("data-target", "#modalTicket");
                            var tables = $("#datatableCrypto").add("#datatableFiat").add("#datatableTrades").add("#datatableWithdrawals").add("#datatableDeposits").add("#modalProfile #pendingReuquest").add("#modalProfile #transactionerrors").add("#modalProfile #withdrawErrors").add("#modalProfile #LogTable");
                            tables.each(function () {
                                var table = $(this);
                                BootstrapUserInit.DataTable.initTable(table);
                            });
                        }
                    });
                }
            })
        }); 
});

$(function () {
    setTimeout(function () {
        $("#Transactions_table tbody tr").each(function () {
            var flag = $(this).children("td:nth-child(3)").text();
            if (flag == "True") {
                $(this).css("background-color", "#ffdede")
            }
        })
    }, 1000);
});

/*
var transactionColors = function () {
    setTimeout(function () {
        $("#Transactions_table tbody tr").each(function () {
            var flag = $(this).children("td:nth-child(3)").text();
            if (flag == "True") {
                $(this).css("background-color", "#ffdede")
            }
        })
    }, 500);
}

$(function () {
    $("tfoot input").on("blur", function () { transactionColors() });
});

$(function () {
    $("#Transactions_table").parent().on("click", ".paginate_button", function () { transactionColors() });
    $("#Transactions_table").parent().on("click", ".sorting", function () { transactionColors() });
    $("#Transactions_table").parent().on("click", ".sorting_asc", function () { transactionColors() });
    $("#Transactions_table").parent().on("click", ".sorting_desc", function () { transactionColors() });
    $("#Transactions_table").parent().keyup("tfoot input", function () { transactionColors() });
});
*/

$(function(){
  $("#ExchangeVOther_table").find(".fa-sign-in").attr("data-toggle","modal").attr("data-target","#modalPair");
  
  $("#ExchangeVOther_table").on("click", ".fa-sign-in", function () {
            var Pair = $(this).parent().parent().children("td:nth-child(3)").html();
            $("#exchange_heading").html("");
            $("#exchange_body").html("");
            $("#preloader_pair").css("display", "block");
            $.ajax({
                type: 'POST',
                url: "/Xmu/Exchange?button=headingExchange",
                data: { 'pairCurr': Pair},
                success: function (response) {
                    var x = $(response)
                    $("#preloader_pair").css("display", "none");
                    $("#exchange_body").html(x.find("#exchange_body").html());
                    $("#exchange_heading").html(x.find("#exchange_heading").html());
                }
            });
        }); 
});

