$(function() {
      $(".notesu").each(function() {
        var modelId = $(this).attr('month');
        var item = $(this);
        $.ajax({
          type: 'GET',
          url: '/VozovyPark/DataDate?modelId=' + modelId,
          data: {},
          success: function (data) {
            var x = $(data)
            $(item).html(x.find("#accordion-2").html());
      }
    });
      });
			});