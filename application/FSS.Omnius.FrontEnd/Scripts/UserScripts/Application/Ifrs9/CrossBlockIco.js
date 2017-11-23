$(function() {
    $("table.klient-table").on("click", "tbody tr", function() {
      	var rowIco = $(this).find("td:nth-child(2)").text();
      	alert(rowIco)
      	$('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="IcoVar" value="' + rowIco + '"/><input type="hidden" name="button" value="klient_table_edit" /></form>').appendTo('body').submit();
    })
});