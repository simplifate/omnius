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
    if ($("#currentBlockName").val() == "VyjadreniKAuditu" || $("#currentBlockName").val() == "VracenoKPrepracovaniNadrizenym" || $("#currentBlockName").val() == "EditaceOpatreniVReseni" || $("#currentBlockName").val() == "VracenoKPrepracovaniAuditorem") {
        setTimeout(function () {
            $('input.input-with-datepicker').datetimepicker('setOptions', {
                format: 'd.m.Y',
                formatDate: 'Y-m-d',
                allowDateRe: '^[0-9]{4}-(01|02|04|05|07|08|10|11)'
            });
        }, 100);
    }
    if ($("#currentBlockName").val() == "VracenoKPrepracovaniNadrizenym" || $("#currentBlockName").val() == "VracenoKPrepracovaniAuditorem") {
        $("[name=radio_agree]").on("change", function () {
            if ($(this).val() === "true") {
                $("[name=DUVOD_NESOUHLASU_textbox]").prop("readonly", true);
                $("[name=NAPRAVNE_OPATRENI_textbox]").prop("readonly", true);
                $("[name=TERMIN_REALIZACE_date]").prop("readonly", true);
            } else {
                $("[name=DUVOD_NESOUHLASU_textbox]").prop("readonly", false);
                $("[name=NAPRAVNE_OPATRENI_textbox]").prop("readonly", false);
                $("[name=TERMIN_REALIZACE_date]").prop("readonly", false);
            }
        });
    }
    if ($("#currentBlockName").val() == "VyjadreniKAuditu") {
        $("[name=radio_agree]").on("change", function () {
            if ($(this).val() === "true") {
                $("[name=DUVOD_NESOUHLASU_textbox]").prop("readonly", true);
                $("[name=NAPRAVNE_OPATRENI_textbox]").prop("readonly", false);
                $("[name=TERMIN_REALIZACE_date]").prop("readonly", false);
            } else {
                $("[name=DUVOD_NESOUHLASU_textbox]").prop("readonly", false);
                $("[name=NAPRAVNE_OPATRENI_textbox]").prop("readonly", true);
                $("[name=TERMIN_REALIZACE_date]").prop("readonly", true);
            }
        });
    }

    // Implement datePicker by jQuery because of IE 11
    if ($("#currentBlockName").val() == "NovySubjekt") {
        $("input[type=date]").datepicker();
    }
    
    if ($("#currentBlockName").val() == "EditaceAuditu" ||
        $("#currentBlockName").val() == "EditaceAudituBezWf" ||
        $("#currentBlockName").val() == "FollowUp" ||
        $("#currentBlockName").val() == "NapravnaOpatreni") {

        // Implementation for filtering by months in DataTables
        var selMonthFrom = $("#uic_dropMonthFrom");
        var selMonthTo = $("#uic_dropMonthTo");

        function getMonthName(month) {
            switch (month) {
                case 1:
                    return "leden";
                case 2:
                    return "únor";
                case 3:
                    return "březen";
                case 4:
                    return "duben";
                case 5:
                    return "květen";
                case 6:
                    return "červen";
                case 7:
                    return "červenec";
                case 8:
                    return "srpen";
                case 9:
                    return "září";
                case 10:
                    return "říjen";
                case 11:
                    return "listopad";
                case 12:
                    return "prosinec";
                default:
                    return "nedefinováno";
            }
        }

        // Set value of undefined date to -1
        selMonthFrom.append('<option value="' + "-1" + '" selected></option>');
        selMonthTo.append('<option value="' + "-1" + '" selected></option>');

        // Generate months to select lists
        for (var y = 2017; y >= 2010; y--) {
            for (var m = 12; m >= 1; m--) {
                var customDateFrom = 1 + "." + m + "." + y;
                selMonthFrom.append('<option value="' + customDateFrom + '">' + y + " " + getMonthName(m) + '</option>');

                // For 1.12.2017 compare with 1.1.2018 and so on
                if (m == 12)
                    var customDateTo = 1 + ".1." + (y + 1);
                else
                    var customDateTo = 1 + "." + (m + 1) + "." + y;

                selMonthTo.append('<option value="' + customDateTo + '">' + y + " " + getMonthName(m) + '</option>');
            }
        }

        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {

                // Do not filter dates on page load (value is not setted to -1 at first)
                if (selMonthFrom.val() == null && selMonthTo.val() == null)
                    return true;

                // Do not filter dates
                if (selMonthFrom.val() == -1 && selMonthTo.val() == -1) {
                    return true;
                }

                var parsedDate;

                // Get dates from 7th column (with hidden columns -1)
                if ($("#currentBlockName").val() == "EditaceAuditu" ||
                    $("#currentBlockName").val() == "EditaceAudituBezWf")
                    parsedDate = moment(data[7], 'D.M.YYYY');

                // Get dates from 11th column (with hidden columns -1)
                if ($("#currentBlockName").val() == "FollowUp")
                    parsedDate = moment(data[11], 'D. M. YYYY H:mm:ss');

                // Get dates from 12th column (with hidden columns -1)
                if ($("#currentBlockName").val() == "NapravnaOpatreni")
                    parsedDate = moment(data[12], 'D. M. YYYY H:mm:ss');

                // Allow dates between 2 chosen months
                if (selMonthFrom.val() != -1 && selMonthTo.val() != -1) {
                    return moment(selMonthFrom.val(), 'D.M.YYYY') < parsedDate
                        && parsedDate < moment(selMonthTo.val(), 'D.M.YYYY');
                }

                // Allow dates from chosen month
                if (selMonthFrom.val() != -1 && selMonthTo.val() == -1) {
                    return moment(selMonthFrom.val(), 'D.M.YYYY') < parsedDate;
                }

                // Allow dates before chosen month
                if (selMonthFrom.val() == -1 && selMonthTo.val() != -1) {
                    return parsedDate < moment(selMonthTo.val(), 'D.M.YYYY');
                }

                return false;
            }
        );

        $(document).on("change", "#uic_dropMonthFrom", function () {
            $(".data-table").DataTable().draw();
        });

        $(document).on("change", "#uic_dropMonthTo", function () {
            $(".data-table").DataTable().draw();
        });
    }

    if ($("#currentBlockName").val() == "EditaceOpatreniVReseni") {
        $("[name=STATUS_radio]").on("change", function () {
            if ($(this).val() === "realizovano") {
                $("[name=DUVOD_POSUNUTI_textbox]").prop("readonly", true);
                $("[name=NT_REALIZACE_date]").prop("readonly", true);
                $("[name=NT_REALIZACE_date]").attr("required", false);
            } else if ($(this).val() === "posunout_termin") {
                $("[name=DUVOD_POSUNUTI_textbox]").prop("readonly", false);
                $("[name=NT_REALIZACE_date").prop("readonly", false);
                $("[name=NT_REALIZACE_date]").attr("required", true);
            }
            else {
                $("[name=DUVOD_POSUNUTI_textbox]").prop("readonly", false);
                $("[name=NT_REALIZACE_date]").prop("readonly", true);
                $("[name=NT_REALIZACE_date]").attr("required", false);
            }
        });
    }
    if ($("#currentBlockName").val() == "FormulaceDoporuceni") {
        $("#uic_doporuc_button").click(function () {
            $("#uic_panel20").toggle('disabled');
        });
    }
    else if ($("#currentBlockName").val() == "ZadaniObjednavkyPeriodika") {

        $("#uic_address_dropdown").change(function () {
            var dropdownText = $("#uic_address_dropdown :selected");
            $("#uic_ship_to_textbox").val(dropdownText.text());
        });



        $("#uic_begin_dtpicker").val("01.01.2017");
        $("#uic_end_dtpicker").val("31.12.2017");
        var userSelectDropdown = $("#uic_user_select_dropdown");
        pageSpinner.show();
        $.ajax({
            type: "POST",
            url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=user_select_label",
            data: {},
            error: console.error.bind(console),
            complete: function () {
                pageSpinner.hide()
            },
            success: function (data) {
                $("#uic_user_select_dropdown option[value != '-1']").remove();
                $("#uic_user_select_dropdown").append('<option value="' + data.user.RweId + '">Za sebe</option>');
                if (data.managers) {
                    for (i = 0; i < data.managers.length; i++) {
                        $("#uic_user_select_dropdown").append('<option value="' + data.managers[i].manager_id + '">' + data.managers[i].manager_full_name + '</option>');
                    }
                }
                $("#uic_user_select_dropdown").val(data.user.RweId);
                if (data.superior)
                    $("#uic_supperior_textbox").val(data.superior.DisplayName);
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
                complete: function () { spinner.remove() },
                success: function (data) {
                    $("#uic_subscriber_textbox").val(data.user[0].full_name);
                    $("#uic_ns_textbox").val(data.user[0].kostl);
                    $("#uic_company_textbox").val(data.user[0].Company);
                    if (data.superior)
                        $("#uic_supperior_textbox").val(data.superior.DisplayName);
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
                complete: function () { spinner.remove() },
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
                complete: function () { spinner.remove() },
                success: function (data) {
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
                complete: function () { spinner.remove() },
                success: function (data) {
                    $("#uic_occupation_select_dropdown option[value != '-1']").remove();
                    if (data.user.length > 0) {
                        $("#uic_occupation_select_dropdown").append('<option value="' + data.user[0].JobObjid + '">' + data.user[0].Job + '</option>');
                    }
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
                    complete: function () { spinner.remove() },
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
                    complete: function () { spinner.remove() },
                    success: function (data) {
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

        $("#uic_reciever_textbox").trackInputDone(function () {
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
                    complete: function () { spinner.remove() },
                    success: function (data) {
                        targetDropdown = panel.find('.uic[originalId="uic_subscriber_name_select_dropdown"]');
                        targetDropdown.find("option[value != '-1']").remove();
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
                complete: function () { spinner.remove() },
                success: function (data) {
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
                complete: function () { spinner.remove() },
                success: function (data) {
                    $("#uic_select_deputy_dropdown option[value != '-1']").remove();
                    for (i = 0; i < data.UserList.length; i++) {
                        currentUser = data.UserList[i];
                        $("#uic_select_deputy_dropdown").append('<option value="' + currentUser.pernr + '">' + currentUser.vorna + ' ' + currentUser.nachn + '</option>');
                    }
                }
            });
        });
    }
    else if ($("#currentBlockName").val() == "NovyAudit") {
        // Selection of a company will fill coordinator dropdown by data wich it receives from server.
        
        $('#uic_company_dropdown').on("change",
            function () {
                $.ajax({
                    type: "POST",
                    url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=company_dropdown",
                    data: { "id_company": $("#uic_company_dropdown option:selected").val() },
                    error: console.error.bind(console),
                    success: function (data) {
                        $("#uic_ia_coordinator_dropdown option[value != '-1']").remove();
                        for (i = 0; i < data.Coordinators.length; i++) {
                            coordinator = data.Coordinators[i];
                            $("#uic_ia_coordinator_dropdown").append('<option value="' + coordinator.id + '">' + coordinator.name + '</option>');
                        }
                    }
                });
            });

    }

    $("tr").on("click", function (event) {
        if (!$(event.target).is(".rowEditAction")) { //to stop event propagation resulting in a recursion
            $(this).find(".rowEditAction").trigger("click");
        }
    });
    if ($("#currentBlockName").val() == "FormulaceDoporuceni") {
        $("#uic_zobrazit").on("click", function () {
            $("#uic_panel20").show();
        })
    }
    if ($("#currentBlockName").val() == "FormulaceDoporuceni") {
        $("#uic_zavrit").on("click", function () {
            $("#uic_panel20").hide();
        })
    }
});