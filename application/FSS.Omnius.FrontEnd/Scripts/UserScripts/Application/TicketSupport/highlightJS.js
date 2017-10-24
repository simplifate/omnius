$(function(){
  $("#dataTableMyProgress tbody tr").each(function(){
    var wait = $(this).children("td:nth-last-child(3)").text();
    var user = $("#currentUserId").val();
    if (wait == user) {
      $(this).css("background-color","#f2e883 ")
    }
    var flag = $(this).children("td:nth-last-child(2)").text();
    if (flag == "True") {
      $(this).css("background-color","#f28383 ")
    }
  })
});