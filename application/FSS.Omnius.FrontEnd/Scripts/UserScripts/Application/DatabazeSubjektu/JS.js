$(function(){
  var display = 'NOPE';
  $("#DataTables_Subjekt tbody tr").each(function(){
    var flag = $(this).children("td:nth-child(3)").text();
    if (flag == "Ano") {
      var display = 'YEP';
    }
  })
  if (display == 'YEP') {
    $("#yes_panel").css("display","block");
  }
  else {
    $("#no_panel").css("display","block");
  }
});