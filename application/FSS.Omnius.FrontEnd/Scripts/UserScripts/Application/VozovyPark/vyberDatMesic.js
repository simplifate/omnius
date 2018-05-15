$(function() {
      $(".notesu").each(function() {
        var modelId = $(this).attr('id');
        var item = $(this);
        $.ajax({
          type: 'GET',
          url: '/VozovyPark/DataDate?modelId=' + modelId,
          data: {},
          success: function (dataq) {
            var x = $(data)
            $(item).html(x.find("#accordion-3").html());
      }
    });
      });
			});