$(function() {
      $(".notes").each(function() {
        var modelId = $(this).attr('day');
        var item = $(this);
        $.ajax({
          type: 'GET',
          url: '/VozovyPark/DataDate?modelId=' + modelId,
          data: {},
          success: function (data) {
            var x = $(data)
            $(item).html(x.find("#accordion-1").html());
      }
    });
      });
			});