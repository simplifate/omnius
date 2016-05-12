function AjaxRunAndReplace(url, uic_name, modelId)
{
    $.ajax({
        type: "POST",
        url: "/api/run" + url + '?button=' + uic_name,
        data: { 'modelId': modelId },
        error: console.error.bind(console),
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
        $("#uic_begin_dtpicker").val("01.01.2017");
        $("#uic_end_dtpicker").val("31.12.2017");
        var userSelectDropdown = $("#uic_user_select_dropdown");
        pageSpinner.show();
        $.ajax({
            type: "POST",
            url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=user_select_label",
            data: {},
            error: console.error.bind(console),
            success: function (data) {
                pageSpinner.hide();
                $("#uic_user_select_dropdown option[value != '-1']").remove();
                $("#uic_user_select_dropdown").append('<option value="' + data.user.rwe_id + '">Za sebe</option>');
                if (data.managers) {
                    for (i = 0; i < data.managers.length; i++) {
                        $("#uic_user_select_dropdown").append('<option value="' + data.managers[i].manager_id + '">' + data.managers[i].manager_full_name + '</option>');
                    }
                }
                $("#uic_user_select_dropdown").val(data.user.rwe_id);
            }
        });
        $("body").on("change", "#uic_user_select_dropdown", function (e) {
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
                error: console.error.bind(console),
                success: function (data) {
                    spinner.remove();
                    $("#uic_subscriber_textbox").val(data.user[0].full_name);
                    $("#uic_ns_textbox").val(data.user[0].kostl);
                    $("#uic_company_textbox").val(data.user[0].Company);
                }
            });
        });
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
                data: { 'targetId': $(this).val(), 'userId': $("#uic_user_select_dropdown").val() ? $("#uic_user_select_dropdown").val() : 0 },
                error: console.error.bind(console),
                success: function (data) {
                    spinner.remove();
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
                    if (data.user[0]) {
                        if (data.periodical.id_periodical_form == 1)
                            $("#uic_ship_to_textbox").val(data.user[0].email);
                        else
                            $("#uic_ship_to_textbox").val(data.user[0].stras + " " + data.user[0].hsnmr + ", " + data.user[0].pstlz + " " + data.user[0].ort01);
                    }
                    $('.uic[uicRole="price"]').val(data.periodical.tentatively_net_of_VAT10);
                    $('.uic[uicRole="output"]').val(data.periodical.tentatively_net_of_VAT10 * parseInt($('.uic[uicRole="count"]').val()));
                }
            });
        });
        $("body").on("change", '.uic[uicRole="count"]', function (e) {
            $('.uic[uicRole="output"]').val($('.uic[uicRole="price"]').val() * parseInt($('.uic[uicRole="count"]').val()));
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
                error: console.error.bind(console),
                success: function (data) {
                    spinner.remove();
                    $("#uic_approver_select_dropdown option[value != '-1']").remove();
                    for (i = 0; i < data.UserList.length; i++) {
                        currentUser = data.UserList[i];
                        $("#uic_approver_select_dropdown").append('<option value="' + currentUser.id + '">' + currentUser.vorna + ' ' + currentUser.nachn + '</option>');
                    }
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
                error: console.error.bind(console),
                success: function (data) {
                    spinner.remove();
                    $("#uic_occupation_select_dropdown option[value != '-1']").remove();
                    $("#uic_occupation_select_dropdown").append('<option value="' + data.job[0].objid + '">' + data.job[0].stext + '</option>');
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
                    data: {
                        'targetId': $(this).val(), 'userId': panel.find('[originalId="uic_subscriber_name_select_dropdown"]').val()
                       ? panel.find('[originalId="uic_subscriber_name_select_dropdown"]').val() : 0
                    },
                    error: console.error.bind(console),
                    success: function (data) {
                        spinner.remove();
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
                        if (data.user[0]) {
                            if (data.periodical.id_periodical_form == 1)
                                panel.find('[originalId="uic_address_textbox"]').val(data.user[0].email);
                            else
                                panel.find('[originalId="uic_address_textbox"]').val(data.user[0].stras + " " + data.user[0].hsnmr + ", " + data.user[0].pstlz + " " + data.user[0].ort01);
                        }
                    }
                });
            }
            else if ($(this).attr("originalId") == "uic_subscriber_name_select_dropdown" && this.value) {
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
                    error: console.error.bind(console),
                    success: function (data) {
                        spinner.remove();
                        var currentUser = data.user[0];
                        panel.find('.uic[originalId="uic_subscriber_occupation_select_dropdown"]').append('<option value="' + currentUser.Job + '">' + currentUser.Job + '</option>');
                        panel.find('.uic[originalId="uic_function_textbox"]').val(currentUser.Job);
                        panel.find('.uic[originalId="uic_address_textbox"]').val(currentUser.stras + " " + currentUser.hsnmr + ", " + currentUser.pstlz + " " + currentUser.ort01);
                        panel.find('.uic[originalId="uic_company_textbox"]').val(currentUser.Company);
                        panel.find('.uic[originalId="uic_ns_textbox"]').val(currentUser.kostl);
                        panel.find('.uic[originalId="uic_subscriber_full_name_textbox"]').val(currentUser.full_name);
                    }
                });
            }
        });

        $("#uic_reciever_textbox").trackInputDone(function() {
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
                    error: console.error.bind(console),
                    success: function (data) {
                        spinner.remove();
                        targetDropdown = panel.find('.uic[originalId="uic_subscriber_name_select_dropdown"]');
                        targetDropdown.find("option").remove(); //todo: add [value != '-1']
                        targetDropdown.append("<option selected value = '-1'> --- Prosím vyberte Uživatele --- </option>");
                        for (i = 0; i < data.UserList.length; i++) {
                            currentUser = data.UserList[i];
                            targetDropdown.append('<option value="' + currentUser.id + '">' + currentUser.vorna + ' ' + currentUser.nachn + '</option>');
                        }
                    }
                });
            }
        });

    }
    else if ($("#currentBlockName").val() == "SchvaleniObjednavkyPeriodika") {
        $.ajax({
            type: "POST",
            url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=periodical_textbox",
            data: { "targetId": GetUrlParameter("modelId") },
            error: console.error.bind(console),
            success: function (data) {
                $("#uic_periodical_textbox").val(data.PeriodicalName);
                $("#uic_interval_textbox").val(data.PeriodicalInterval);
                $("#uic_form_textbox").val(data.PeriodicalForm);
                $("#uic_type_textbox").val(data.PeriodicalType);
            }
        });
    }
    else if ($("#currentBlockName").val() == "EditaceObjednavky") {
        $.ajax({
            type: "POST",
            url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=periodical_textbox",
            data: { "targetId": GetUrlParameter("modelId") },
            error: console.error.bind(console),
            success: function (data) {
                dataRow = data.ViewData[0];
                $("#uic_periodical_textbox").val(dataRow.Periodikum);
                $("#uic_interval_textbox").val(dataRow["Četnost"]);
                $("#uic_form_textbox").val(dataRow.Forma);
                $("#uic_type_textbox").val(dataRow.Typ);
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
    else if ($("#currentBlockName").val() == "NovyZastupce") {
        $("#uic_user_textbox").on("change", function (e) {
            var spinner = $(inlineSpinnerTemplate)
                .attr({ id: "user_textbox_spinner" })
                .css({
                    position: "absolute",
                    top: $(this).position().top,
                    left: $(this).position().left + $(this).outerWidth()
                })
                .insertAfter(this);

            $.ajax({
                type: "POST",
                url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=user_textbox",
                data: { "SearchQuery": $(this).val() },
                error: console.error.bind(console),
                success: function (data) {
                    spinner.remove();
                    $("#uic_select_user_dropdown option[value != '-1']").remove();
                    for (i = 0; i < data.UserList.length; i++) {
                        currentUser = data.UserList[i];
                        $("#uic_select_user_dropdown").append('<option value="' + currentUser.pernr + '">' + currentUser.vorna + ' ' + currentUser.nachn + '</option>');
                    }
                }
            });
        });
        $("#uic_deputy_textbox").on("change", function (e) {
            var spinner = $(inlineSpinnerTemplate)
                .attr({ id: "deputy_textbox_spinner" })
                .css({
                    position: "absolute",
                    top: $(this).position().top,
                    left: $(this).position().left + $(this).outerWidth()
                })
                .insertAfter(this);

            $.ajax({
                type: "POST",
                url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=user_textbox",
                data: { "SearchQuery": $(this).val() },
                error: console.error.bind(console),
                success: function (data) {
                    spinner.remove();
                    $("#uic_select_deputy_dropdown option[value != '-1']").remove();
                    for (i = 0; i < data.UserList.length; i++) {
                        currentUser = data.UserList[i];
                        $("#uic_select_deputy_dropdown").append('<option value="' + currentUser.pernr + '">' + currentUser.vorna + ' ' + currentUser.nachn + '</option>');
                    }
                }
            });
        });
    }
});
