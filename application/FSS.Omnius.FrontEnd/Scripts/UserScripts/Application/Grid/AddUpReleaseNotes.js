lastpanelid=1;

$(document).on('click', '#add_button', function (e) {
    e.preventDefault();
  	var panelTemplate=$($('#parent').children('div:first').html());
  
  	panelTemplate.find('input, select').each(function(index,element){

    	$(element).attr('name','panelCopy'+lastpanelid+'_'+$(element).attr('originalName'));
    });
  	panelTemplate.find('input[type=hidden]').attr('name','panelCopy'+lastpanelid+'Marker');
    $('#repeat').parent('#parent').append(panelTemplate);
  	lastpanelid++;
});

$(document).on('click', '#deleteNote_button', function() {
    $(this).parent().parent().parent().parent().remove();
  	lastpanelid=1;
  	$('#parent .panel:not(:first)').each(function(index,element){
        console.log('blabla');
    	var panel=$(element);
    	panel.find('input, select').each(function(index,element){
        	$(element).attr('name','panelCopy'+lastpanelid+'_'+$(element).attr('originalName'));	
    	});
      	panel.find('input[type=hidden]').attr('name','panelCopy'+lastpanelid+'Marker');
        lastpanelid++;
    });

});
