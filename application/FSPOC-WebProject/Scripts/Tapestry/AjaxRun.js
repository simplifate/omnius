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
if ($("#currentBlockName").val() == "Zadaniobjednavkyperiodika") {
    $("body").on("change", "#uic_periodical_dropdown", function (e) {
        $.ajax({
            type: "POST",
            url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=" + $(this).attr("name"),
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
}
else if ($("#currentBlockName").val() == "Hromadnaobjednavkaproasistentky") {
    $("#uic_approver_textbox").on("change", function (e) {
        $.ajax({
            type: "POST",
            url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=" + $(this).attr("name"),
            data: { "SearchQuery": $(this).val() },
            success: function (data) {
                $("#uic_approver_select_dropdown option").remove();
                for (i = 0; i < data.UserList.length; i++) {
                    currentUser = data.UserList[i];
                    $("#uic_approver_select_dropdown").append('<option value="' + currentUser.id + '">' + currentUser.vorna + ' ' + currentUser.nachn + '</option>');
                }
            }
        });
    });
    $("#uic_approver_select_dropdown").on("change", function (e) {
        $.ajax({
            type: "POST",
            url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=" + $(this).attr("name"),
            data: { "targetId": $(this).val() },
            success: function (data) {
                $("#uic_occupation_select_dropdown option").remove();
                $("#uic_occupation_select_dropdown").append('<option value="' + data.job[0].objid + '">' + data.job[0].stext + '</option>');
            }
        });
    });
    $(".dropdown-select").on("change", function (e) {
        if ($(this).attr("originalId") == "uic_periodical_dropdown") {
            panel = $(this).parents(".uic.panel-component");
            dropdownName = $(this).attr("name");
            if (dropdownName.startsWith("panelCopy"))
                dropdownName = dropdownName.substring(dropdownName.indexOf("_") + 1, dropdownName.length);
            $.ajax({
                type: "POST",
                url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=" + dropdownName,
                data: { 'targetId': $(this).val() },
                success: function (data) {
                    panel.find('[originalId="uic_interval_dropdown"] option').each(function (index, element) {
                        option = $(element);
                        if (option.attr("value") == data.periodical.id_periodical_interval) {
                            ShowOption(option);
                        }
                        else
                            HideOption(option);
                    });
                    panel.find('[originalId="uic_form_dropdown"] option').each(function (index, element) {
                        option = $(element);
                        if (option.attr("value") == data.periodical.id_periodical_form) {
                            ShowOption(option);
                        }
                        else
                            HideOption(option);
                    });
                    panel.find('[originalId="uic_type_dropdown"] option').each(function (index, element) {
                        option = $(element);
                        if (option.attr("value") == data.periodical.id_periodical_types) {
                            ShowOption(option);
                        }
                        else
                            HideOption(option);
                    });
                    panel.find('[originalId="uic_prince_vat_10_textbox"]').val(data.periodical.tentatively_net_of_VAT10);
                    RecalculateAutosum(panel);
                }
            });
        }
    });
}