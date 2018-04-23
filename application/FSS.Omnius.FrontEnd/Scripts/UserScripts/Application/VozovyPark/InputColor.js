$(dpcument).ready(function(){
  var datum = new Date()
  var date = datum.getDate()
  if($("input:text#STKWarning").val() >=  date){
    $("input:text#STKWarning").css("background" , "FF0000");
  }
});