$(function ()
{
    if($('body').hasClass('menuOrderModule'))
    {
        $('ul.sortable').sortable();

        $('#btnSave').click(function () {
            $('#menuOrderForm').submit();
        });
    }
});