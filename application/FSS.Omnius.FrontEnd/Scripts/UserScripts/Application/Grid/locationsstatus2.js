       $(function() { $("#LocationsDropdown").on('change', function () {
         	$("#chooselocation").click();
            var val = this.val;
         	var form = $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="modelId" value="this.value" /><input type="hidden" name="button" value="#chooselocation" /></form>');
        });
                    });