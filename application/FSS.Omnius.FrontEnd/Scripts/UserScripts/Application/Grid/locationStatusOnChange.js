       $(function() { $("#LocationsDropdown").on('change', function () {
            var val = this.val;
         	var form = $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="modelId" value="'+val+'" /><input type="hidden" name="button" value="#chooselocation" /></form>');
        });
                    });