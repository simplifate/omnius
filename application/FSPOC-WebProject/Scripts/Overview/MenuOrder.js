$(function ()
{
    if($('body').hasClass('menuOrderModule'))
    {
        $('ul.sortable').sortable();

        $('#menuOrderForm').on('submit', function () {
            return false;
        });

        $('#btnOverview').click(function () {
            $('#openMetablockForm').submit();
        });

        $('#btnSave').click(function () {
            
            var metablockOrder = {};
            var blockOrder = {};

            var i = 1;
            $('.sortable input[type=hidden]').each(function () {
                if ($(this).is('.metablock')) {
                    metablockOrder[this.value] = i;
                }
                else {
                    blockOrder[this.value] = i;
                }
                i++;
            });

            var postData = {
                Blocks: blockOrder,
                Metablocks: metablockOrder
            };

            $.ajax({
                type: "POST",
                url: "/api/tapestry/saveMenuOrder",
                contentType: 'application/json; charset=UTF-8',
                data: JSON.stringify(postData),
                success: function () {
                    alert('ok');
                },
                error: function (request, status, error) {
                    alert(request.responseText);
                }
            });
        });
    }
});