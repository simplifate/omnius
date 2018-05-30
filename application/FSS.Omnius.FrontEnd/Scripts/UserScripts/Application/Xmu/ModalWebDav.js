$(function(){
  $("#Profiles_table_nove").find(".fa-edit").attr("data-toggle","modal_nove").attr("data-target","#modalProfile_nove");
  
  $("#Profiles_table_nove").on("click", ".fa-edit", function () {
			var userId = $(this).parent().parent().children("td:nth-child(2)").text();
            $("#modal_body_profile_nove").html("");
  			$("#modal_name_profile_nove").html("");
            $("#preloader_profile_nove").css("display", "block");
            $.ajax({
                type: 'GET',
                url: "/Xmu/WebDavUsersEdit?modelId="+userId,
                success: function (response) {
                    var x = $(response)
                    $("#preloader_profile_nove").css("display", "none");
                    $("#modal_body_profile_nove").html(x.find("#modal_body_profile_nove").html());
                    $("#modal_name_profile_nove").html(x.find("#modal_name_profile_nove").html());
                    alert('sracka');
    				var table = $("#TableForLogs");
              		BootstrapUserInit.DataTable.initTable(table);
    				alert('hovno');
                }
            });
        }); 
});

$(function(){
  $("#Profiles_table_schvalene").find(".fa-edit").attr("data-toggle","modal_schvalene").attr("data-target","#modalProfile_schvalene");
  
  $("#Profiles_table_schvalene").on("click", ".fa-edit", function () {
			var userId = $(this).parent().parent().children("td:nth-child(2)").text();
            $("#modal_body_profile_schvalene").html("");
  			$("#modal_name_profile_schvalene").html("");
            $("#preloader_profile_schvalene").css("display", "block");
            $.ajax({
                type: 'GET',
                url: "/Xmu/WebDavUsersEdit?modelId="+userId,
                success: function (response) {
                    var x = $(response)
                    $("#preloader_profile_schvalene").css("display", "none");
                    $("#modal_body_profile_schvalene").html(x.find("#modal_body_profile_schvalene").html());
                    $("#modal_name_profile_schvalene").html(x.find("#modal_name_profile_schvalene").html());
                    alert('sracka');
    				var table = $("#TableForLogs");
              		BootstrapUserInit.DataTable.initTable(table);
    				alert('hovno');
                }
            });
        }); 
});

$(function(){
  $("#Profiles_table_zamitnute").find(".fa-edit").attr("data-toggle","modal_zamitnute").attr("data-target","#modalProfile_zamitnute");
  
  $("#Profiles_table_zamitnute").on("click", ".fa-edit", function () {
			var userId = $(this).parent().parent().children("td:nth-child(2)").text();
            $("#modal_body_profile_zamitnute").html("");
  			$("#modal_name_profile_zamitnute").html("");
            $("#preloader_profile_zamitnute").css("display", "block");
            $.ajax({
                type: 'GET',
                url: "/Xmu/WebDavUsersEdit?modelId="+userId,
                success: function (response) {
                    var x = $(response)
                    $("#preloader_profile_zamitnute").css("display", "none");
                    $("#modal_body_profile_zamitnute").html(x.find("#modal_body_profile_zamitnute").html());
                    $("#modal_name_profile_zamitnute").html(x.find("#modal_name_profile_zamitnute").html());
                    alert('sracka');
    				var table = $("#TableForLogs");
              		BootstrapUserInit.DataTable.initTable(table);
    				alert('hovno');
                }
            });
        }); 
});

$(function(){
    var reload = function () {
                var userId = $("#hiddenUserId").val();
                $("#modal_body_profile").html("");
                $("#modal_name_profile").html("");
                $("#preloader_profile").css("display", "block");
                $.ajax({
                    dataType: "html",
                	url: "/Xmu/WebDavUsersEdit?modelId="+userId,
                    success: function (response) {
                        var x = $(response)
                        $("#preloader_profile").css("display", "none");
                        $("#modal_body_profile").html(x.find("#modal_body_profile").html());
                        $("#modal_name_profile").html(x.find("#modal_name_profile").html());
                    }
                });
            };

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
        			$.ajax({
            			type: 'POST',
            			url: '/Xmu/' + $('#currentBlockName').val() + '/?button=deleteImage',
            			data: { 'deleteUrl': Image, '__RequestVerificationToken': token },
            			success: function (data) {
                			reload();
            			}
        			});
    			}
			}) 
          },
          null, 
          this
        );
        document.getElementById("modalProfile").style.overflowY = "auto";
    });
});