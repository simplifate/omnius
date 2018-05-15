$(function() {
      $(".notes").each(function() {
        var modelId = $(this).attr('year');
        var item = $(this);
        $.ajax({
          type: 'GET',
          url: '/VozovyPark/DataMesic?modelId=' + modelId,
          data: {},
          success: function (data) {
            var x = $(data)
            $(item).html(x.find("#accordion-2").html());
      }
    });
      });
			});
$(document).ready(function(){
  var datum = new Date()
  var date = datum.getDate()
  if($("input:date#datumstk").val() >=  date){
    $("input:date#datumstk").css("background" , "FF0000");
  }
});