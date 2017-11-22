$(function() {
    $("table.klient-table").on("click", "tbody tr", function() {
      alert("bagr");
      	var rowIco = $(this).find("td:nth-child(2)").text();
      	$('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="Ico" value="' + rowIco + '"/><input type="hidden" name="button" value="klient_table_edit" /></form>').appendTo('body').submit();
    })
});