$( document ).ready(function(){
  $("#Support_request_table_wrapper").find(".fa-edit").attr("data-toggle","modal").attr("data-target","modalTicket");
});

$("#Support_request_table_wrapper").find(".fa-edit").click(function () {
            var SupportID = $(this).parent().parent().children("td:first-child").html();
            $("#modal_name_ticket").html("");
            $("#modal_body_ticket").html("");
            $("#preloader").css("display", "block");
            $.ajax({
                dataType: "html",
                url: "/Xmu/Ticket?modelId=" + SupportID,
                success: function (response) {
                    var x = $(response)
                    $("#preloader").css("display", "none");
                    $("#modal_name_ticket").html(x.find("#modal_name_ticket").html());
                    $("#modal_body_ticket").html(x.find("#modal_body_ticket").html());
                }
            });
        });

        var reload = function () {
            var RigId = $("#MessageWindow").find("#RigIpForm").data('RigId');
            $("#header").html("");
            $("#gpuTempGraphModal").html("");
            $("#gpu-table").html("");
            $("#messages").html("");
            $("#MessageWindow").html("");
            $("#preloader").css("display", "block");
            $.ajax({
                dataType: "html",
                url: "/Grid/RigHistoryMessages?modelId=" + RigId,
                success: function (response) {
                    var x = $(response)
                    $("#preloader").css("display", "none");
                    $("#header").html(x.find("#HeadingText").html());
                    $("#gpuTempGraphModal").html(x.find("#gpuTempGraph").html());
                    $("#gpu-table").html(x.find("#gpuStatus").html());
                    $("#messages").html(x.find("#Messages").html());
                    $("#MessageWindow").html(x.find("#MessageWindow").html());
                    $("#RigIpForm").val($("#header").find("#Rig").text());
                    $("#RigIpForm").data('RigId', RigId);
                }
            });
        }
          $(document).on("click", "#NewMessage", function () {
            var rigHistoryMessage = $("#rigHistoryMessage").val();
            $.ajax({
                type: 'POST',
                url: '/api/run/Grid/' + $('#currentBlockName').val() + '/?button=NewMessage',
                data: { 'rigHistoryMessage': rigHistoryMessage, "RigIpForm": $("#RigIpForm").val(), "MessageId": $("#MessageId").val() },
                success: function (data) {
                    reload();
                }
            });
        });