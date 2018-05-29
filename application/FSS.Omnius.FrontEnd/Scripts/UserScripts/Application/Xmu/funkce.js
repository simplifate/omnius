$(function(){
  alert("bagr");
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
                            var tables = $("#actualTransactions table");                   
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

$(function(){
  $("#ExchangeVOther_table").find(".fa-sign-in").attr("data-toggle","modal").attr("data-target","#modalPair");
  
  $("#ExchangeVOther_table").on("click", ".fa-sign-in", function () {
            var Pair = $(this).parent().parent().children("td:nth-child(3)").html();
            $("#exchange_heading").html("");
            $("#exchange_body").html("");
            $("#preloader_pair").css("display", "block");            
    		$.ajax({
                url: '/Persona/Account/GetAntiForgeryToken',
                type: 'GET',
                success: function (token) {
                  $.ajax({
                    type: 'POST',
                    url: "/Xmu/Exchange?button=headingExchange",
                    data: { 'pairCurr': Pair,'__RequestVerificationToken': token},
                    success: function (response) {
                        var x = $(response)
                        $("#preloader_pair").css("display", "none");
                        $("#exchange_body").html(x.find("#exchange_body").html());
                        $("#exchange_heading").html(x.find("#exchange_heading").html());
                    }
                  });
            }
        });
  });
});

    $(document).on("click", "#deleteImage", function (e) {
      	var bui = BootstrapUserInit;
        var btn = $(this);
        var message = btn.data('confirm');


        bui.confirm(
          message, 
          function () {

            var Image = $(this).parent().children("#imageUrl").html();
			$.ajax({
    			url: '/Persona/Account/GetAntiForgeryToken',
    			type: 'GET',
    			success: function (token) {
                  setTimeout(reloadImage,1000);
                  
        			$.ajax({	
            			type: 'POST',
            			url: '/Xmu/' + '/userskyc' + '/?button=deleteImage',
            			data: { 'deleteUrl': Image, '__RequestVerificationToken': token }
                     
        			});
    			}
			}); 
          },
          null, 
          this
        );
		document.getElementById("modalProfile").style.overflowY = "auto";
    });
});