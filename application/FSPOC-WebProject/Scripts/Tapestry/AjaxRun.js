function AjaxRunAndReplace(url, uic_name, modelId)
{
    $.ajax({
        type: "POST",
        url: "/api/run" + url + '?button=' + uic_name,
        data: { 'modelId': modelId },
        success: function (data) {
            $.each(data, function (name, value) {
                if ($('select#uic_' + name).size() > 0)
                {
                    var html = '';
                    $.each(value, function (i, item) {
                        html += '<option value="' + item['id'] + '">' + item['Name'] + '</option>';
                    });

                    $('select#uic_' + name).html(html);
                }
                else if ($('input#uic_' + name).size() > 0)
                {
                    $('input#uic_' + name).val(value);
                }
            });
        }
    });
}
$('body').on('click', '.runAjax', function (e) {
    e.preventDefault();
    AjaxRunAndReplace(window.location.pathname, $(this).val());
});
$("body").on("change", "#uic_periodical_dropdown", function (e) {
    $.ajax({
        type: "POST",
        url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockId").val() + "/?button=" + $(this).attr("name"),
        data: { 'targetId': $(this).val() },
        success: function (data) {
            $("#uic_interval_dropdown option").each(function (index, element) {
                option = $(element);
                if (option.attr("value") == data.periodical.id_periodical_interval) {
                    ShowOption(option);
                }
                else
                    HideOption(option);
            });
            $("#uic_form_dropdown option").each(function (index, element) {
                option = $(element);
                if (option.attr("value") == data.periodical.id_periodical_form) {
                    ShowOption(option);
                }
                else
                    HideOption(option);
            });
            $("#uic_type_dropdown option").each(function (index, element) {
                option = $(element);
                if (option.attr("value") == data.periodical.id_periodical_types) {
                    ShowOption(option);
                }
                else
                    HideOption(option);
            });
        }
    });
});
