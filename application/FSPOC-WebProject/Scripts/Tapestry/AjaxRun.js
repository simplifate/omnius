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

$(function () {
    var inlineSpinnerTemplate = '<div class="spinner-3"> <div class="rect1"></div> <div class="rect2"></div> <div class="rect3"></div> <div class="rect4"></div> <div class="rect5"></div> </div>';
    if ($("#currentBlockName").val() == "ZadaniObjednavkyPeriodika") {
        $("body").on("change", "#uic_periodical_dropdown", function (e) {
            var spinner = $(inlineSpinnerTemplate)
                .attr({ id: "approver_select_dropdown_spinner" })
                .css({
                    position: "absolute",
                    top: $(this).position().top,
                    left: $(this).position().left + $(this).outerWidth()
                })
                .insertAfter(this);

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
                    if (data.periodical.id_periodical_form == 1)
                        $("#uic_ship_to_textbox").val(data.user.Email);
                    else
                        $("#uic_ship_to_textbox").val(data.user.Address);

                    spinner.remove();
                }
            });
        });
    }
    else if ($("#currentBlockName").val() == "HromadnaObjednavkaProAsistentky") {
        $("#uic_approver_textbox").on("change", function (e) {
            var spinner = $(inlineSpinnerTemplate)
                .attr({ id: "approver_select_dropdown_spinner" })
                .css({
                    position: "absolute",
                    top: $(this).position().top,
                    left: $(this).position().left + $(this).outerWidth()
                })
                .insertAfter(this);

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
                    spinner.remove();
                }
            });
        });
        $("#uic_approver_select_dropdown").on("change", function (e) {
            var spinner = $(inlineSpinnerTemplate)
                .attr({ id: "approver_select_dropdown_spinner" })
                .css({
                    position: "absolute",
                    top: $(this).position().top,
                    left: $(this).position().left + $(this).outerWidth()
                })
                .insertAfter(this);

            $.ajax({
                type: "POST",
                url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=" + $(this).attr("name"),
                data: { "targetId": $(this).val() },
                success: function (data) {
                    $("#uic_occupation_select_dropdown option").remove();
                    $("#uic_occupation_select_dropdown").append('<option value="' + data.job[0].objid + '">' + data.job[0].stext + '</option>');
                    spinner.remove();
                }
            });
        });
        $(".dropdown-select").on("change", function (e) {
            if ($(this).attr("originalId") == "uic_periodical_dropdown") {
                var spinner = $(inlineSpinnerTemplate)
                    .attr({ id: "approver_select_dropdown_spinner" })
                    .css({
                        position: "absolute",
                        top: $(this).position().top,
                        left: $(this).position().left + $(this).outerWidth()
                    })
                    .insertAfter(this);

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

                        spinner.remove();
                    }
                });
            }
            else if ($(this).attr("originalId") == "uic_subscriber_name_select_dropdown") {
                var spinner = $(inlineSpinnerTemplate)
                    .attr({ id: "approver_select_dropdown_spinner" })
                    .css({
                        position: "absolute",
                        top: $(this).position().top,
                        left: $(this).position().left + $(this).outerWidth()
                    })
                    .insertAfter(this);

                panel = $(this).parents(".uic.panel-component");
                dropdownName = $(this).attr("name");
                if (dropdownName.startsWith("panelCopy"))
                    dropdownName = dropdownName.substring(dropdownName.indexOf("_") + 1, dropdownName.length);
                $.ajax({
                    type: "POST",
                    url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=approver_select_dropdown",
                    data: { "targetId": $(this).val() },
                    success: function (data) {
                        panel.find('.uic[originalId="uic_subscriber_occupation_select_dropdown"] option').remove();
                        panel.find('.uic[originalId="uic_subscriber_occupation_select_dropdown"]').append('<option value="' + data.job[0].objid + '">' + data.job[0].stext + '</option>');
                        panel.find('.uic[originalId="uic_function_textbox"]').val(data.job[0].stext);
                        panel.find('.uic[originalId="uic_address_textbox"]').val(data.user.stras + " " + data.user.hsnmr + ", " + data.user.pstlz + " " + data.user.ort01);
                        panel.find('.uic[originalId="uic_company_textbox"]').val("RWE");
                        panel.find('.uic[originalId="uic_ns_textbox"]').val(data.user.kostl);
                        spinner.remove();
                    }
                });
            }
        });
        var recieverTextbox = $("#uic_reciever_textbox");
        recieverTextbox.on("change", updateSubscriberList);
        recieverTextbox.on("input", $.debounce(1000, updateSubscriberList));
        recieverTextbox.on("keypress", function(e){
            if(e.which == 9 || e.which == 13){
                updateSubscriberList();
                e.preventDefault();
            }
        });

        function updateSubscriberList(e) {
            if ($(this).attr("originalId") == "uic_reciever_textbox") {
                var spinner = $(inlineSpinnerTemplate)
                    .attr({ id: "approver_select_dropdown_spinner" })
                    .css({
                        position: "absolute",
                        top: $(this).position().top,
                        left: $(this).position().left + $(this).outerWidth()
                    })
                    .insertAfter(this);

                panel = $(this).parents(".uic.panel-component");
                dropdownName = $(this).attr("name");
                if (dropdownName.startsWith("panelCopy"))
                    dropdownName = dropdownName.substring(dropdownName.indexOf("_") + 1, dropdownName.length);
                $.ajax({
                    type: "POST",
                    url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=approver_textbox",
                    data: { "SearchQuery": $(this).val() },
                    success: function (data) {
                        targetDropdown = panel.find('.uic[originalId="uic_subscriber_name_select_dropdown"]');
                        targetDropdown.find("option").remove();
                        targetDropdown.append("<option selected> --- Prosím vyberte Uživatele --- </option>");
                        for (i = 0; i < data.UserList.length; i++) {
                            currentUser = data.UserList[i];
                            targetDropdown.append('<option value="' + currentUser.id + '">' + currentUser.vorna + ' ' + currentUser.nachn + '</option>');
                        }
                        spinner.remove();
                    }
                });
            }
        };
    }
    else if ($("#currentBlockName").val() == "SchvaleniObjednavkyPeriodika") {
        $.ajax({
            type: "POST",
            url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=periodical_textbox",
            data: { "targetId": GetUrlParameter("modelId") },
            success: function (data) {
                $("#uic_periodical_textbox").val(data.PeriodicalName);
                $("#uic_interval_textbox").val(data.PeriodicalInterval);
                $("#uic_form_textbox").val(data.PeriodicalForm);
                $("#uic_type_textbox").val(data.PeriodicalType);
            }
        });
    }
    else if ($("#currentBlockName").val() == "SchvaleniHromadneObjednavky") {
        var heapOrderId = GetUrlParameter("modelId");
        var orderTable = $("#uic_order_table");
        orderTable.find("tbody tr").each(function (trIndex, trElement) {
            if ($(trElement).find("td:nth-child(2)").text() != heapOrderId)
                $(trElement).remove();
        });
        orderTable.find("thead tr th:nth-child(2), tbody tr td:nth-child(2), tfoot tr th:nth-child(2)").hide();
    }
});
