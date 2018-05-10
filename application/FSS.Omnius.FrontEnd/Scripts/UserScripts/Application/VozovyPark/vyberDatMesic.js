$(function() {
      $(".notes").each(function() {
        var modelId = $(this).attr('month');
        var item = $(this);
        $.ajax({
          type: 'GET',
          url: '/VozovyPark/DataMesic?modelId=' + modelId,
          data: {},
          success: function (data) {
            var x = $(data)
            $(item).html(x.find("#accordion-1").html());
      }
    });
      });
			});