$(document).ready(function(){
  var datum = new Date()
  var date = datum.getDate()
  if($("input:date#datumstk").val() >=  date){
    $("input:date#datumstk").css("background" , "FF0000");
  }
});