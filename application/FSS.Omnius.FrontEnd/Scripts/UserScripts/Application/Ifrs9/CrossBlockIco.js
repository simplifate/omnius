$(function() {
    $("table.klient-table").on("click", "tbody tr i", function() {
      	var rowIco = $(this).parent().parent().find("td:nth-child(2)").text();
      	$('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="IcoVar" value="' + rowIco + '"/><input type="hidden" name="button" value="klient_table_modal" /></form>').appendTo('body').submit();
    })
});