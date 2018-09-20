$(function() {
      $(".notes").each(function() {
        var modelId = $(this).attr('id');
        var item = $(this);
        $.ajax({
          type: 'GET',
          url: '/Grid/Notesitems?modelId=' + modelId,
          data: {},
          success: function (data) {
            var x = $(data)
            $(item).html(x.find("#NoteList").html());
      }
    });
      });
			});
$(document).on('click', '#deleteItem_button', function () {
	var that = $(this);
    var id = $(this).parent().parent().children("#hiddenItemID").html();
	$.ajax({
		type: 'POST',
		url: '/Grid/NotesItems/?button=deleteNote_button',
		data: {posranyId:id},
		success: function (data) {			
			that.parent().parent().parent().parent().parent().remove();
		}
 	});
});

$(document).on('click', '#deleteVersion_button', function () {
	var that = $(this);
    var id = $(this).parent().parent().children("#hiddenVersionId").html();
	$.ajax({
		type: 'POST',
		url: '/Grid/ReleaseNotes/?button=deleteVersion_button',
		data: {versionId:id},
		success: function (data) {			
			that.parent().parent().parent().remove();
		}
 	});
});

$(document).on('click', '#addUp_button', function () {
	var that = $(this);
    var id = $(this).parent().parent().children("#hiddenVersionId").html();
	window.location.href = '/Grid/AddUpToReleaseNotes?modelId=' + id;
});
