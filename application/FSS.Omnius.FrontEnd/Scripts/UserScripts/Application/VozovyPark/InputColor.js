$(dpcument).ready(function(){
  var datum = new Date()
  var date = datum.getDate()
  if($("input:text#datumstk").val() >=  date){
    $("input:text#datumstk").css("background" , "FF0000");
  }
});