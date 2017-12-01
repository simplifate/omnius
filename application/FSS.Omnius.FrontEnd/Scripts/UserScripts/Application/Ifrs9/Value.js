$("input").each(function (index, element) {
    $(element).val($(element).attr("value-source"));
});

$("#saveAll_button").click(function(){
  $(".container form").each(function() {
    if ($(this).parent().is("#profileRow_foreach")) {
      $.ajax({
         type: 'POST',
         url: '/Ifrs9/' + $('#currentBlockName').val() + '?button=saveRowProfile_button',
         data: $(this).serialize()
      });
    }
    else {
      $.ajax({
         type: 'POST',
         url: '/Ifrs9/' + $('#currentBlockName').val() + '?button=saveRowIbis_button',
         data: $(this).serialize()
      });
    }
  })
  window.setTimeout('location.reload()', 1500);
});