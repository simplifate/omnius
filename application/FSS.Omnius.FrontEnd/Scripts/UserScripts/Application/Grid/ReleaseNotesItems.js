$(document).on('click', '#deleteItem_button', function () {
	var that = $(this);
    var id = $(this).parent().children("#hiddenItemID").html();
	$.ajax({
		type: 'POST',
		url: '/Grid/NotesItems/?button=deleteNote_button',
		data: {posranyId:id},
		success: function (data) {			
			that.parent().parent().parent().parent().remove();
		}
 	});
});