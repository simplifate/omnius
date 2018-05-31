$(function(){
$(".modalType").find(".fa-edit").attr("data-toggle","modal").attr("data-target","#modalProfile");
  
  var modal = function (type) {
                      alert($(this));

			var userId = $(this).parent().parent().children("td:nth-child(3)").text();
            $("#modal_body_profile").html("");
  			$("#modal_name_profile").html("");
            $("#preloader_profile").css("display", "block");
        $.ajax({
        url: '/Persona/Account/GetAntiForgeryToken',
        type: 'GET',
        success: function (token) {
            $.ajax({
                type: 'POST',
                url: "/Xmu/WebDavUsersEdit?modelId="+userId,
                data: { 'type': type, '__RequestVerificationToken': token},
                success: function (response) {
                    var x = $(response)
                    $("#preloader_profile").css("display", "none");
                    $("#modal_body_profile").html(x.find("#modal_body_profile").html());
                    $("#modal_name_profile").html(x.find("#modal_name_profile").html());
    				var table = $("#TableForLogs");
              		BootstrapUserInit.DataTable.initTable(table);
                	}
            });
          }
        })
     };
  $("#Profiles_table_nove").on("click", ".fa-edit", function () {modal("new")});
  $("#Profiles_table_schvalene").on("click", ".fa-edit", function () {modal("approved")});
  $("#Profiles_table_zamitnute").on("click", ".fa-edit", function () {modal("rejected")});
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