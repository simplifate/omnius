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
    if ($("#currentBlockName").val() == "NovySubjekt" || $("#currentBlockName").val() == "EditaceSubjektu") {
        $("#Date_input").datetimepicker({
            datepicker: true,
            timepicker: false,
            format: "d.m.Y"
        });
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
                            var coordinator = data.Coordinators[i];
                            $("#uic_ia_coordinator_dropdown").append('<option value="' + coordinator.id + '">' + coordinator.name + '</option>');
                        }
                    }
                });
            });

    }

    $("td:not(.actionIcons)").on("click", function (event) {
        if (!$(event.target).is(".rowEditAction")) { //to stop event propagation resulting in a recursion
            $(this).parents("tr").find(".rowEditAction").trigger("click");
        }
    });
    if ($("#currentBlockName").val() == "FormulaceDoporuceni") {
        $("#uic_zobrazit").on("click",
            function () {
                $("#uic_panel20").show();
            });
    }
    if ($("#currentBlockName").val() == "FormulaceDoporuceni") {
        $("#uic_zavrit").on("click",
            function () {
                $("#uic_panel20").hide();
            });
    }
    if ($("#currentBlockName").val() == "FormulaceDoporuceni") {
        var btnVybratDoporuceniToggled = false;
        // Toggle panel with the table "Nadrazena doporuceni"
        $("#uic_btnVybratDoporuceni").on("click",
            function () {
                // Toggle one more time for the first time, because panel is initialy set to opacity:0
                if (btnVybratDoporuceniToggled == false) {
                    $("#uic_panel20").css({ "opacity": "100" });
                    $("#uic_panel20").toggle();
                    btnVybratDoporuceniToggled = true;
                }
                // Toggle for every click
                $("#uic_panel20").toggle();
            });

        // On click on row fill value of hiddenId to input "nadr_doporuc_label"
        $('#uic_nadraz_doporuceni_table').find('tr').click(function () {
            // For selected row return value of td for first column (hiddenId)
            var row = $(this).find('td:first').text();
            $("#uic_nadr_doporuc_label").val(row);
        });
    }
    if ($("#currentBlockName").val() == "NovyAudit") {
        $("#uic_info").mouseover(function() {
            $("#uic_panel40 > div").show();
        });
    }
    if ($("#currentBlockName").val() == "NovyAudit") {
        $("#uic_info").mouseleave(function() {
            $("#uic_panel40 > div").hide();
        });
    }
    if ($("#currentBlockName").val() == "NovyAuditBezWf") {
        $("#uic_info2").mouseover(function() {
            $("#uic_panel23 > div").show();
        });
    }
    if ($("#currentBlockName").val() == "NovyAuditBezWf") {
        $("#uic_info2").mouseleave(function() {
            $("#uic_panel23 > div").hide();
        });
    }
});
function ShowAppNotification(text, type) {
    var type = type.toLowerCase() || "info";
    switch (type) {
        case "success":
            icon = "fa-check";
            break;
        case "error":
            icon = "fa-exclamation-circle";
            break;
        case "warning":
            icon = "fa-exclamation";
            break;
        case "info":
        default:
            type = "info";
            icon = "fa-info-circle";
            break;
    }
    var newNotification = $('<div class="app-alert app-alert-' + type + '"><i class="fa ' + icon 
        + ' alertSymbol"></i><span></span><div class="fa fa-times closeAlertIcon"></div></div>');
    newNotification.find("span").text(text);
    $("#appNotificationArea").append(newNotification);
    $("#upperPanel, #lowerPanel, #minimizedUpperPanel, #userContentArea").css({ top: "+=" + newNotification.outerHeight() + "px" });

    if (CurrentModuleIs("tapestryModule")){
        RecalculateToolboxHeight();
    } else if (CurrentModuleIs("mozaicEditorModule")) {
        RecalculateMozaicToolboxHeight();
    }
    newNotification.find(".closeAlertIcon").on("click", function () {
        $("#upperPanel, #lowerPanel, #minimizedUpperPanel, #userContentArea").css({ top: "-=" + newNotification.outerHeight() + "px" });
        $(this).parents(".app-alert").remove();
        if (CurrentModuleIs("tapestryModule")) {
            RecalculateToolboxHeight();
        } else if (CurrentModuleIs("mozaicEditorModule")) {
            RecalculateMozaicToolboxHeight();
        }
    });
};
function dateTimePickerPreventWhenReadOnly(current_time, $input) {
    if ($input.attr("readonly") !== undefined) {
        return false;
    }
}
function HidePanel(paneName) {
    panel = $("#userContentArea").find('div[name="' + paneName + '"]');
    if (panel.is(":visible")) {
        panelBottom = parseInt(panel.css("top")) + panel.height();
        $(".mozaicForm > .uic").each(function (index, element) {
            currentComponent = $(element);
            if (parseInt(currentComponent.css("top")) > panelBottom) {
                currentComponent.css("top", parseInt(currentComponent.css("top")) - panel.height());
            }
        });
        panel.hide();
    }
}
function ShowPanel(paneName) {
    panel = $("#userContentArea").find('div[name="' + paneName + '"]');
    if (!panel.is(":visible")) {
        panel.show();
        panelTop = parseInt(panel.css("top"));
        $(".mozaicForm > .uic").each(function (index, element) {
            currentComponent = $(element);
            if (parseInt(currentComponent.css("top")) > panelTop) {
                currentComponent.css("top", parseInt(currentComponent.css("top")) + panel.height());
            }
        });
    }
}
function ClonePanel(paneName) {
    firstPanel = $("#userContentArea").find('div[name="' + paneName + '"]:first');
    lastPanel = $("#userContentArea").find('div[name="' + paneName + '"]:last');
    panelCount = $("#userContentArea").find('div[name="' + paneName + '"]').length;
    panelBottom = parseInt(lastPanel.css("top")) + lastPanel.height();
    $(".mozaicForm > .uic").each(function (index, element) {
        currentComponent = $(element);
        if (parseInt(currentComponent.css("top")) > panelBottom) {
            currentComponent.css("top", parseInt(currentComponent.css("top")) + lastPanel.height() + 10);
        }
    });
    newPanel = firstPanel.clone(true);
    newPanel.find('.uic, .uic checkbox, .uic input[type="radio"]').each(function (item, element) {
        currentComponent = $(element);
        componentName = currentComponent.attr("name");
        if (componentName)
            currentComponent.attr("name", "panelCopy" + panelCount + "_" + componentName);
        componentId = currentComponent.attr("id");
        if (componentId)
            currentComponent.attr("id", "panelCopy" + panelCount + "_" + componentId);
    });
    newPanel.append('<input type="hidden", name="panelCopy' + panelCount + 'Marker" value="true">');
    firstPanel.parents(".mozaicForm").append(newPanel);
    newPanel.css("top", panelBottom + 10);
    newPanel.find('input.input-single-line, textarea').each(function (item, element) {
        $(element).val("");
    });
    newPanel.find('.uic[originalId="uic_pieces_textbox"]').val(1);
    newPanel.find(".uic.input-with-datepicker").each(function (item, element) {
        datePicker = $(element);
        datePicker.off("focus");
        datePicker.off("keydown");
        datePicker.off("keyup");
        datePicker.off("keypress");
        datePicker.removeClass("hasDatepicker");
        datePicker.datepicker($.datepicker.regional['cs']);
    });
    removePanelIcon = $('<div class="fa fa-remove removePanelIcon"></div>');
    newPanel.append(removePanelIcon);
    removePanelIcon.on("click", function () {
        panel = $(this).parents(".panel-component");
        panelBottom = parseInt(panel.css("top")) + panel.height();
        $(".mozaicForm > .uic").each(function (index, element) {
            currentComponent = $(element);
            if (parseInt(currentComponent.css("top")) > panelBottom) {
                currentComponent.css("top", parseInt(currentComponent.css("top")) - panel.height() - 10);
            }
        });
        autosumTarget = null;
        panel.find(".uic.input-single-line").each(function (index, element) {
            autosumTargetName = $(element).attr("writeSumInto");
            if (autosumTargetName) {
                autosumTarget = $('.uic[name="' + autosumTargetName + '"]');
                sourceInputName = $(element).attr("name");
            }
        });
        panel.remove();
        if (autosumTarget) {
            if (sourceInputName.indexOf("_") == -1)
                sourceInputNameWithoutPrefix = sourceInputName;
            else
                sourceInputNameWithoutPrefix = sourceInputName.substring(sourceInputName.indexOf("_") + 1, sourceInputName.length);
            sum = 0;
            $(".uic.input-single-line").each(function (index, element) {
                inputName = $(element).attr("name");
                if (inputName.indexOf(sourceInputNameWithoutPrefix, inputName - sourceInputNameWithoutPrefix.length) !== -1) {
                    numericValue = parseInt($(element).val());
                    if (!isNaN(numericValue)) {
                        multiplierTextbox = $(element).parents(".panel-component").find('[originalId="uic_pieces_textbox"]');
                        if (multiplierTextbox && !isNaN(multiplierTextbox.val()) && multiplierTextbox.val() > 0)
                            sum += (numericValue * multiplierTextbox.val());
                        else
                            sum += numericValue;
                    }
                }
            });
            targetTemplate = autosumTarget.attr("contentTemplate");
            if (targetTemplate) {
                autosumTarget.text(targetTemplate.replace("{{var1}}", sum));
            }
            else
                autosumTarget.text(sum);
        }
    });
}
function RecalculateAutosum(panelDiv) {
    autosumTarget = null;
    panelDiv.find(".uic.input-single-line").each(function (index, element) {
        autosumTargetName = $(element).attr("writeSumInto");
        if (autosumTargetName) {
            autosumTarget = $('.uic[name="' + autosumTargetName + '"]');
            sourceInputName = $(element).attr("name");
        }
    });
    if (autosumTarget) {
        if (sourceInputName.indexOf("panelCopy") == -1)
            sourceInputNameWithoutPrefix = sourceInputName;
        else
            sourceInputNameWithoutPrefix = sourceInputName.substring(sourceInputName.indexOf("_") + 1, sourceInputName.length);
        sum = 0;
        $(".uic.input-single-line").each(function (index, element) {
            inputName = $(element).attr("name");
            if (inputName.indexOf(sourceInputNameWithoutPrefix, inputName - sourceInputNameWithoutPrefix.length) !== -1) {
                numericValue = parseInt($(element).val());
                if (!isNaN(numericValue)) {
                    multiplierTextbox = $(element).parents(".panel-component").find('[originalId="uic_pieces_textbox"]');
                    if (multiplierTextbox && !isNaN(multiplierTextbox.val()) && multiplierTextbox.val() > 0)
                        sum += (numericValue * multiplierTextbox.val());
                    else
                        sum += numericValue;
                }
            }
        });
        targetTemplate = autosumTarget.attr("contentTemplate");
        if (targetTemplate) {
            autosumTarget.text(targetTemplate.replace("{{var1}}", sum));
        }
        else
            autosumTarget.text(sum);
    }
}
function HideOption(option) {
    if (option.parent("span.hiddenOption").length == 0)
        option.wrap('<span class="hiddenOption" />');
}
function ShowOption(option) {
    if (option.parent("span.hiddenOption").length)
        option.unwrap();
}
function GetUrlParameter(sParam) {
    var sPageURL = decodeURIComponent(window.location.search.substring(1)),
        sURLVariables = sPageURL.split('&'), sParameterName, i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
};
function RecalculateMozaicFormHeight() {
    var mozaicForm = $("#userContentArea .mozaicForm");
    var mozaicFormHeight = mozaicForm.height();

    // For fixing panel overlapping when grown (sorting by dom-tree)
    // Updated: now sorting by pos
    // First, find every panel in mozaic form
    var panels = mozaicForm.find("> .panel-component");

    // Sort array of panels
    panels.sort(function (a, b) {
        // Key-value of sorting is top position of element
        return $(a).position().top - $(b).position().top;
    });

    // Variable for storing previous panel
    var lastUic = null;
    var test = 0;
    // Foreach every panel
    panels.each(function (index, element) {
        var currentUic = $(element);
        // If is not first one
        if (lastUic !== null) {
            // Calculate positions and save them to self-storytelling variables
            var topPos = currentUic.position().top;
            var bottomPos = currentUic.position().top + currentUic.height();
            var lastTopPos = lastUic.position().top;
            var lastBottomPos = parseInt(lastUic.position().top + lastUic.height());
            // If top of panel overlaps bottom of last one
            if (topPos < lastBottomPos + 10) {
                // Move it under it with 10 px space
                currentUic.css("top", lastBottomPos + 10);
            }
        }
        // Save current as previous
        lastUic = currentUic;
    });
    mozaicForm.find("> .uic").each(function (index, element) {
        currentUic = $(element);
        if (currentUic.position().top + currentUic.height() > mozaicFormHeight) {
            mozaicFormHeight = currentUic.position().top + currentUic.height();
        }
    });
    mozaicForm.height(mozaicFormHeight + 30);
}
function RecalculatePanelDimensions(panel) {
    var panelWidth = panel.width();
    var panelHeight = panel.hasClass("panelAutoHeight") ? 0 : panel.height();

    panel.find(".uic, .dataTables_wrapper").each(function (index, element) {
        currentUic = $(element);
        if (currentUic.position().left + currentUic.width() + 30 > panelWidth) {
            if (!panel.hasClass("named-panel") || currentUic.parent().css("overflow") == "visible") {
                panelWidth = currentUic.position().left + currentUic.width() + 30;
            }
        }
        if (currentUic.position().top + currentUic.height() + 30 > panelHeight) {
            panelHeight = currentUic.position().top + currentUic.height() + 30;
        }
    });
    panel.width(panelWidth);
    panel.height(panelHeight);
}
$(function () {
    $(document).on("click", ".mozaicForm [data-ajax='true']", function () {
        pageSpinner.show();
        $(document).one("ajaxComplete", function () {
            pageSpinner.hide();
        });
    });
});
function GetColumnSearchElementFor(title) {

    if ($("#currentAppName").val() == "Evidence Periodik") {

        if (title == "Forma periodika") {
            return '<select><option value="">--vyberte--</option><option>Elektronické</option><option>Papírové</option></select>';
        }
        else if (title == "Typ periodika") {
            return '<select><option value="">--vyberte--</option><option>tuzemské</option><option>zahraniční</option></select>';
        }
        else if (title == "Četnost periodika") {
            return '<select><option value="">--vyberte--</option><option>1x týdně</option><option>5x týdně</option><option>10x ročně</option><option>1x měsíčně</option><option>6x ročně</option><option>2x ročně</option><option>denně</option><option>4x ročně</option><option>2x měsíčně</option><option>nepravidelně</option><option>1x ročně</option><option>6x týdně</option><option>22x ročně</option><option>312x ročně</option><option>6x ročně + 4x ročně bulletin</option><option>12x ročně</option><option>2x týdně</option><option>254x ročně</option></select>';
        }
        else if (title == "Stav objednávky") {
            return '<select><option value="">--vyberte--</option><option>nový</option><option>rozpracováno</option><option>vyřízeno</option><option>zrušeno</option><option>nevyfakturováno</option><option>změna</option></select>';
        }
        else if (title == "Stav schválení") {
            return '<select><option value="">--vyberte--</option><option>schváleno</option><option>zamítnuto</option><option>čeká na schválení</option></select>';
        }
        else {
            return '<input type="text" placeholder="Hledat v &quot;' + title + '&quot;" />';
        }
      
    } else {
        return '<input type="text" placeholder="Hledat v &quot;' + title + '&quot;" />';
    }
}
$(document).ready(function () {
    //var logOutFlag = true;
    //$('a, input[type=submit], button').click(function () {
    //    logOutFlag = false;
    //});
    //$(window).unload(function () {
    //    if (logOutFlag)
    //        $.ajax({
    //            url: '/CORE/Info/LogOut',
    //            async: false
    //        })
    //        .success(function () {
    //            $.ajax({
    //                url: '/CORE/Info/Leave',
    //                async: false
    //            });
    //        });
    //});
    $('#logoutLink').click(function () {
        $('#logoutForm').submit();
        return false;
    });
});
var pageSpinner = (function () {
    var debug = false;
    var uses = 1;
    return {
        show: function (n) {
            if (!arguments.length) {
                n = 1;
            }
            if (n) {
                $(document.body).addClass("pageSpinnerShown");
            }
            uses += n;
            if (debug) {
                console.log("page spinner shown %d times, %d total", n, uses);
                console.trace();
            }
        },
        hide: function (n) {
            if (!arguments.length) {
                n = 1;
            }
            uses -= n;
            if (!uses) {
                $(document.body).removeClass("pageSpinnerShown");
            }
            if (debug) {
                console.log("page spinner hidden %d times, %d remaining", n, uses);
                console.trace();
            }
        }
    }
})();

$(function () {
    pageSpinner.hide();
    $(window).on("beforeunload", function () {
        if (typeof window.ignoreUnload == 'undefined' || window.ignoreUnload == false) {
            pageSpinner.show();
        }
        else {
            window.ignoreUnload = false;
        }
    });
});

var ModalDialogArray = [];
var mozaicFormValidator;

$(function () {
    $("#hideMenuIcon").on("click", function () {
        $(document.body).addClass("leftBarHidden");
    });
    $("#showMenuIcon").on("click", function () {
        $(document.body).removeClass("leftBarHidden");
    });
    $("#toggleMenuIcon").on("click", function () {
        $(".userBox").slideUp();
        $(".searchBox").slideUp(); 
        $(document.body).toggleClass("leftBarHidden");
    });

    if (CurrentModuleIs("appManagerModule")) {
        $(".appPanel").each(function () {
            while($(this).collision(".appPanel").length > 1) {
                $(this).css({top: "+=120px"});
            }
        });
        $(".appPanel").draggable({
            grid: [120, 120],
            revert: "invalid",
            stop: function () {
                $(this).draggable("option", "revert", "invalid");
                $.ajax({
                    type: "POST",
                    url: 'api/master/apps' + $(this).attr('data-target') + '/saveAppPosition',
                    data: {
                        'positionX': $(this).css('left'),
                        'positionY': $(this).css('top')
                    }
                });
            }
        });
        $(".appWorkspace").droppable({
            tolerance: "fit"
        });
        $(".appPanel").droppable({
            greedy: true,
            tolerance: "touch",
            drop: function (event, ui) {
                ui.draggable.draggable("option", "revert", true);
            }
        });
        $(".appPanel").bind("dragstart", function (event, ui) {
            ui.originalPosition.top = $(this).position().top;
            ui.originalPosition.left = $(this).position().left;
        });
        $(".appPanel").on("dblclick", function () {
            window.location.href = $(this).attr('data-target');
        });
    }
    else if (CurrentModuleIs("helpModule")) {
        $("#appManagerIcon").removeClass("activeIcon");
        $("#helpIcon").addClass("activeIcon");
    }
    else if (CurrentModuleIs("userDetailsModule")) {
        $("#appManagerIcon").removeClass("activeIcon");
    };
    if ($("#userLeftBar").length > 0) {
        $(".uic").each(function (index, element) {
            $(element).attr("originalId", $(element).attr("Id"));
        });
        $(".uic > checkbox").each(function (index, element) {
            $(element).prop("checked", false);
        });
        $(".userBoxMinimized").click(function () {
            $(document.body).addClass("leftBarHidden");
            $(".searchBox").slideUp();
            $(".userBox").slideToggle();
        });
        $(".searchBoxMinimized").click(function () {
            $(document.body).addClass("leftBarHidden");
            $(".searchBox").slideToggle();
            $(".userBox").slideUp();
        });
        try {
            var mozaicForm = $(".mozaicForm");

            var csrfTokenInput = null;

            if (mozaicForm.length > 0) {
                mozaicForm = mozaicForm.first();
                csrfTokenInput = mozaicForm.find("[name=__RequestVerificationToken]").clone();
            }

            function submitActionByForm(tableName, rowId, action, addons) {
                // Create
                var form = $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="modelId" value="' + rowId + '" /><input type="hidden" name="button" value="' + tableName + '_' + action + '" /></form>');
                
                // Add aditional inputs
                if (Array.isArray(addons)) {
                    $.each(addons, function (index, addon) {
                        form.append(addon);
                    });
                }

                // "protect"
                if (csrfTokenInput !== null) {
                    form.append(csrfTokenInput);
                }
                
                // Append
                form.appendTo('body').submit();
            }

            $(".uic.data-table").each(function (index, element) {
            var table = $(element);
            var tableWidth = parseInt(table.attr("uicWidth"));
            CreateCzechDataTable(table, table.hasClass("data-table-simple-mode"));
            wrapper = table.parents(".dataTables_wrapper");
            wrapper.css("position", "absolute");
            wrapper.css("left", table.css("left"));
            wrapper.css("top", table.css("top"));
            wrapper.css("width", tableWidth);
            table.css("max-width", tableWidth);
            table.css("position", "relative");
            table.css("left", "0px");
            table.css("top", "0px");
            if (table.attr("data-item-population-target") !== undefined) {
                var target = $(table.attr("data-item-population-target"));
                $(table.find("[data-item-populator]")).on("click", function (e) {
                    var id = $(this).parent().parent().find("td:first-child").text();
                    console.log("Selected row with id " + id);
                    target.val(id);
                    e.preventDefault();
                    target.get(0).scrollIntoView();
                });
            }
            table.wrap("<div class='inner_wrapper'>");
                table.on("click", ".rowEditAction", function () {
                    var rowId = parseInt($(this).parents("tr").find("td:first").text());
                    var tableName = table.attr("name");
                    if ($(this).hasClass("fa-download"))
                        window.ignoreUnload = true;

                    submitActionByForm(tableName, rowId, "EditAction");
            });
            table.on("click", ".rowDetailsAction", function () {
                var rowId = parseInt($(this).parents("tr").find("td:first").text());
                var tableName = table.attr("name");

                submitActionByForm(tableName, rowId, "DetailsAction");
            });
            table.on("click", ".rowDeleteAction", function () {
                if (confirm('Jste si jistí?')) {
                    rowId = parseInt($(this).parents("tr").find("td:first").text());
                    var modelId = GetUrlParameter("modelId");
                    var tableName = table.attr("name");
                    submitActionByForm(tableName, modelId, "DeleteAction", [$('<input type="hidden" name="deleteId" value="' + rowId + '" />')]);
                }
            });
            table.on("click", ".row_A_Action", function () {
                var rowId = parseInt($(this).parents("tr").find("td:first").text());
                var tableName = table.attr("name");

                submitActionByForm(tableName, rowId, "A_Action");
            });
            table.on("click", ".row_B_Action", function () {
                var rowId = parseInt($(this).parents("tr").find("td:first").text());
                var tableName = table.attr("name");

                submitActionByForm(tableName, rowId, "B_Action");
            });
            table.DataTable().on("draw", function () {
                var t = $(this);
                t.find("thead th").each(function (index, element) {
                    if ($(element).text() == "id" || $(element).text().indexOf == "hidden" || $(element).text().indexOf("hidden__") == 0) {
                        t.find("td:nth-child(" + (index + 1) + "), th:nth-child(" + (index + 1) + ")").hide();
                    }
                });
            });

            table.DataTable().draw();

            if(!table.hasClass("data-table-simple-mode")) {

                table.find("tfoot th").each(function () {
                    var title = $(this).text();
                    if (title != "Akce")
                        $(this).html(GetColumnSearchElementFor(title));
                    else
                        $(this).html("");
                });
                dataTable = table.DataTable();
                dataTable.columns().eq(0).each(function (colIdx) {
                    $("input, select", dataTable.column(colIdx).footer()).on("keyup change", function () {
                        dataTable
                            .column(colIdx)
                            .search(this.value)
                            .draw();
                    });
                });
                if ($("#currentBlockName").val() == "ZakladniReport") {
                    var currentUser = $("#currentUserName").val();
                    dataTable
                        .order([1, 'desc'])
                        .column(9)
                        .search(currentUser)
                        .draw();
                    table.find("tfoot th:nth-child(10) input").val(currentUser);
                }
                else if ($("#currentBlockName").val() == "ReportProAsistentky") {
                    var currentUser = $("#currentUserName").val();
                    dataTable
                        .order([1, 'desc'])
                        .column(4)
                        .search(currentUser)
                        .draw();
                    table.find("tfoot th:nth-child(5) input").val(currentUser);
                }
                else if ($("#currentBlockName").val() == "ReportProVedouciPracovniky") {
                    var currentUser = $("#currentUserName").val();
                    dataTable
                        .order([1, 'desc'])
                        .column(4)
                        .search(currentUser)
                        .draw();
                    table.find("tfoot th:nth-child(5) input").val(currentUser);
                }
                else if ($("#currentBlockName").val() == "NeaktivniUzivatele") {
                    dataTable
                        .order([3, 'desc'])
                        .draw();
                }
                table.find("thead th").each(function (index, element) {
                    if ($(element).text() == "id" || $(element).text().indexOf("hidden__") == 0) {
                        table.find("td:nth-child(" + (index + 1) + "), th:nth-child(" + (index + 1) + ")").hide();
                    }
                    else if ($(element).text() == "Barva") {
                        table.find("td:nth-child(" + (index + 1) + "), th:nth-child(" + (index + 1) + ")").hide();

                        table.find("td:nth-child(" + (index + 1) + ")").each(function (tdIndex, tdElement) {
                            var colorCode = $(tdElement).text();
                            $(tdElement).parents("tr").find("td:nth-child(" + (index + 2) + ")")
                                .prepend('<div class="colorRectangle" style="background-color:' + colorCode + '"></div>');
                        });
                    }
                });
            }
            });
        }
        catch (err) {
            console.log(err);
        }
        
        $.extend($.validator.methods, {
            auditNumber: function (value, element, attr) {
                return value.match(/^[0-9]{4} [PA] [0-9]{2,3}$/);
            }
        });
        $.extend($.validator.methods, {
            auditNumberNoWF: function (value, element, attr) {
                return value.match(/^[0-9]{4} [BCEFSZ] [0-9]{2,3}$/);
            }
        });
        $.extend($.validator.methods, {
            auditNumberNonWF: function (value, element, attr) {
                return value.match(/^[0-9]{4} C [0-9]{2,3}$/);
            }
        });
        $.extend($.validator.methods, {
            greaterThan: function (value, element, attr) {
                return this.optional(element) || +value > +attr;
            }
        });
        $.extend($.validator.methods, {
            greaterOrEqual: function (value, element, attr) {
                return this.optional(element) || +value >= +attr;
            }
        });
        $.extend($.validator.methods, {
            optionSelected: function (value, element, attr) {
                return $(element).attr("required") == undefined || +value != +attr;
            }
        });
        jQuery.validator.addClassRules("dropdown-select", {
            optionSelected: -1
        });

        mozaicFormValidator = $(".mozaicForm").validate({
            errorLabelContainer: $("<div>"), //put error messages into a detached element, AKA a trash bin; todo: find a better way to get rid of them
            ignore: "[readonly]",
            unhighlight: function (element) {
                $(".uic[ignoredonvalidation]").addClass("cancel");
                $(element).removeClass("has-error");
                
                // Element validator
                $('#' + element.id + '_validator').hide();

                if (this.numberOfInvalids() === 0) $(".uic.button-simple:not([ignoredonvalidation])").removeClass("looks-disabled");
            },
            highlight: function (element) {
                $(".uic[ignoredonvalidation]").addClass("cancel");
                $(element).addClass("has-error");
                
                // Element validator
                $('#' + element.id + '_validator').show();

                $(".uic.button-simple:not([ignoredonvalidation])").addClass("looks-disabled");
            }
        });
        if ($(".mozaicForm").length)
            mozaicFormValidator.form();

        $(".uic.button-simple, .uic.button-dropdown").on("click", function () {
            $(".uic.data-table").each(function (tableIndex, tableElement) {
                console.warn("Iterating over tables");
                var visibleRowList = "";
                var dataTable = $(tableElement).DataTable();
                dataTable.rows({ search: 'applied' }).nodes().each(function (row, index) {
                    // Export selection
                    var checkbox = $(row).find("th:first-child input[type=checkbox]");
                    if (checkbox.length > 0) {
                        if (checkbox.is(":checked")) {
                            if (visibleRowList !== "")
                                visibleRowList += ",";
                            visibleRowList += $(row).children()[1].innerText;
                        }
                    } else {
                        if (visibleRowList !== "")
                            visibleRowList += ",";
                        visibleRowList += $(row).children()[0].innerText;
                    }
                });
                tableName = $(tableElement).attr("name");
                $('input[name="' + tableName + '"').val(visibleRowList);
                var visibleColumnList = "";
                $(tableElement).find("thead th:visible").each(function () {
                    var header = $(this);
                    var checked = header.find("input[type=\'checkbox\']").is(":checked");
                    console.log(header.attr("data-column-name"), checked, header.find("input[type=\'checkbox\']"));
                    if (checked) {
                        if (visibleColumnList !== "")
                            visibleColumnList += ";";
                        visibleColumnList += header.attr("data-column-name");
                    }
                });
                $('input[name="' + tableName + '-column-filters').val(visibleColumnList);
            });
            if (this.value.indexOf('export') !== -1) {
                window.ignoreUnload = true;
            }
        });
        $(".uic.input-with-datepicker").datetimepicker({
            datepicker: true,
            timepicker: false,
            format: "d.m.Y"
        });
        $(".uic.input-with-timepicker").datetimepicker({
            datepicker: false,
            timepicker: true,
            step: 5,
            format: "H:i:00"
        });
        $(".uic.input-with-datetimepicker").datetimepicker({
            datepicker: true,
            timepicker: true,
            step: 5,
            format: "d.m.Y H:i:00"
        });
        $(".uic.color-picker").each(function (index, element) {
            newComponent = $(element);
            CreateColorPicker(newComponent);
            newReplacer = $("#userContentArea .sp-replacer:last");
            newReplacer.css("position", "absolute");
            newReplacer.css("left", newComponent.css("left"));
            newReplacer.css("top", newComponent.css("top"));
            newComponent.removeClass("uic");
            newReplacer.addClass("uic color-picker");
            newReplacer.attr("uicClasses", "color-picker");
            newReplacer.attr("uicName", newComponent.attr("uicName"));
        });
        $(".uic.countdown-component").each(function (index, element) {
            var newDateObj = new Date();
            newDateObj.setTime(newDateObj.getTime() + (5 * 60 * 1000));
            $(element).countdown({ until: newDateObj, format: 'HMS' });
        });
        $(".uic.input-single-line").each(function (index, element) {
            newComponent = $(element);
            autosumTargetName = newComponent.attr("writeSumInto");
            if (autosumTargetName) {
                newComponent.on("change", function () {
                    autosumTargetName = $(this).attr("writeSumInto");
                    autosumTarget = $('.uic[name="' + autosumTargetName + '"]');
                    sourceInputName = $(this).attr("name");
                    if (sourceInputName.indexOf("_") == -1)
                        sourceInputNameWithoutPrefix = sourceInputName;
                    else
                        sourceInputNameWithoutPrefix = sourceInputName.substring(sourceInputName.indexOf("_") + 1, sourceInputName.length);
                    sum = 0;
                    $(".uic.input-single-line").each(function (index, element) {
                        inputName = $(element).attr("name");
                        if (inputName.indexOf(sourceInputNameWithoutPrefix, inputName - sourceInputNameWithoutPrefix.length) !== -1) {
                            numericValue = parseInt($(element).val());
                            if (!isNaN(numericValue)) {
                                multiplierTextbox = $(element).parents(".panel-component").find('[originalId="uic_pieces_textbox"]');
                                if (multiplierTextbox && !isNaN(multiplierTextbox.val()) && multiplierTextbox.val() > 0)
                                    sum += (numericValue * multiplierTextbox.val());
                                else
                                    sum += numericValue;
                            }
                        }
                    });
                    autosumTarget = $('.uic[name="' + autosumTargetName + '"]');
                    targetTemplate = autosumTarget.attr("contentTemplate");
                    if (targetTemplate) {
                        autosumTarget.text(targetTemplate.replace("{{var1}}", sum));
                    }
                    else
                        autosumTarget.text(sum);
                });
            }
        });
        $(".uic.input-single-line").on("change", function () {
            if ($(this).attr("originalId") == "uic_pieces_textbox" && $(this).parents(".panel-component"))
                RecalculateAutosum($(this).parents(".panel-component"));
        });
        $("#uic_item_count_textbox, #uic_pieces_textbox").val(1);
        $(".uic.panel-component").each(function (index, element) {
            panel = $(element);
            hidingCheckboxName = panel.attr("panelHiddenBy");
            if (hidingCheckboxName) {
                hidingCheckbox = $('input[name="' + hidingCheckboxName + '"]');
                if (hidingCheckbox) {
                    hidingCheckbox.attr("panelToHide", panel.attr("name"));
                    hidingCheckbox.prop("checked", true);
                    hidingCheckbox.on("change", function () {
                        panelToHide = $(this).attr("panelToHide");
                        if ($(this).is(":checked")) {
                            ShowPanel(panelToHide);
                        }
                        else {
                            HidePanel(panelToHide);
                        }
                    });
                }
            }
            cloningButtonName = panel.attr("panelClonedBy");
            if (cloningButtonName) {
                cloningButton = $('button[buttonName="' + cloningButtonName + '"]');
                if (cloningButton) {
                    cloningButton.attr("type", "button");
                    cloningButton.attr("panelToClone", panel.attr("name"));
                    cloningButton.on("click", function () {
                        panelToClone = $(this).attr("panelToClone");
                        ClonePanel(panelToClone);
                    });
                }
            }
        });
        $(".uic.panel-component").each(function (index, element) {
            RecalculatePanelDimensions($(element));
        });
        RecalculateMozaicFormHeight();
        $("#modalRepository .modalRepositoryItem").each(function (index, element) {
            currentDialog = $(element);
            currentDialog.dialog({
                autoOpen: false,
                width: 370,
                height: 320,
                buttons: {
                    "OK": function () {
                        alert("TODO: Save");
                        $(this).dialog("close")
                    },
                    "Zrušit": function () {
                        $(this).dialog("close");
                    }
                }
            });
            ModalDialogArray.push(currentDialog);
        });
        notificationMessage = GetUrlParameter("message");
        if (notificationMessage) {
            notificationType = GetUrlParameter("messageType");
            ShowAppNotification(notificationMessage, notificationType);
        }
    }
});

var BootstrapUserInit = {

    context: null,

    init: function (bootstrapContext) {
        var self = BootstrapUserInit;
        self.context = bootstrapContext;

        $(self.context)
            .on('keyup change', '.data-table > tfoot input', self.DataTable.filter)
            .on('click', '.data-table i.fa[data-action]', self.DataTable.onAction)
            .on('search.dt', '.data-table', self.DataTable.onSearch)
            .on('select.dt deselect.dt', '.data-table', self.DataTable.onSearch);

        self.DataTable.init();
        self.loadValidators();
        $(".closeAlertIcon").on("click", function () {
            //$("#upperPanel, #lowerPanel, #minimizedUpperPanel, #userContentArea").css({ top: "-=" + newNotification.outerHeight() + "px" });
            $(this).parents(".app-alert").remove();
            if (CurrentModuleIs("tapestryModule")) {
                RecalculateToolboxHeight();
            } else if (CurrentModuleIs("mozaicEditorModule")) {
                RecalculateMozaicToolboxHeight();
            }
        });

    },

    confirm: function(message, callbackTrue, callbackFalse, context)
    {
        var modal = $('<div class="modal fade" id="modalConfirm" tabindex="-1" role="dialog"></div>');
        var modalDialog = $('<div class="modal-dialog" role="document"></div>');
        var modalContent = $('<div class="modal-content"></div>');
        var modalHeader = $('<div class="modal-header"></div>');
        var modalClose = $('<button type="button" class="close" data-dismiss="modal" aria-label="Close" title="Zavřít"><span aria-hidden="true">&times;</span></button>');
        var modalTitle = $('<h4 class="modal-title">Jste si jistí?</h4>');
        var modalBody = $('<div class="modal-body">' + message + '</div>');
        var modalFooter = $('<div class="modal-footer"></div>');
        var buttonYes = $('<button type="button" class="btn btn-danger">Ano</button>');
        var buttonNo = $('<button type="button" class="btn btn-default">Ne</button>');

        modal.append(modalDialog);
        modalDialog.append(modalContent);
        modalContent.append(modalHeader).append(modalBody).append(modalFooter);
        modalHeader.append(modalClose).append(modalTitle);
        modalFooter.append(buttonNo).append(buttonYes);

        buttonYes.click(function () {
            callbackTrue.apply(context, []);
            $('#modalConfirm').modal('hide');
        });
        buttonNo.click(function () {
            if (typeof callbackFalse == 'function') {
                callbackFalse.apply(context, []);
            }
            $('#modalConfirm').modal('hide');
        });

        modal.appendTo('body');
        $('#modalConfirm').modal();
        $('#modalConfirm').on('hidden.bs.modal', function () {
            $('#modalConfirm').remove();
        });
    },

    /******************************************************/
    /* DATA TABLES                                        */
    /******************************************************/
    DataTable:
    {
        init: function () {
            var self = BootstrapUserInit;

            $('.data-table', self.context).each(function () {
                var table = $(this);
                self.DataTable.initTable(table);
            });
        },
        initTable: function(table) {

                //Select extension init
                if (table.data('dtselect') == '1') {
                    table.find("thead tr").prepend("<th class='select-head'><input type='checkbox' id='selAll'></th>");
                    table.find("tfoot tr").prepend("<th>Select All</th>");
                    table.find("tbody tr").prepend("<td></td>");
                    $("th.select-head > input[type='checkbox']").on("change", function () {
                        var cb_checked = $("th.select-head > input[type='checkbox']").prop("checked");
                        if (cb_checked)
                            $(this).parents(".data-table").DataTable().rows().select();
                        else
                            $(this).parents(".data-table").DataTable().rows().deselect();
                    });

                }

                table.DataTable({
                    columnDefs: table.data('dtselect') ? [{
                        orderable: false,
                        className: 'select-checkbox',
                        targets: 'select-head'
                    }] : '0',
                    select: table.data('dtselect') ? {
                        style: 'multi',
                        selector: 'td:first-child'
                    } : false,
                    //order: [[ 1, 'asc' ]],
                    paging: table.data('dtpaging') == '1',
                    pageLength: 50,
                    lengthMenu: [[10, 20, 50, 100, 200, 500, 1000, -1], [10, 20, 50, 100, 200, 500, 1000, 'Vše']],
                    info: table.data('dtinfo') == '1',
                    filter: table.data('dtfilter') == '1' || table.data('dtcolumnfilter') == '1',
                    ordering: table.data('dtordering') == '1',
                    order: table.data('dtorder') ? eval(table.data('dtorder')) : [[0, 'desc']],
                    language: {
                        sEmptyTable: 'Tabulka neobsahuje žádná data',
                        sInfo: 'Zobrazuji _START_ až _END_ z celkem _TOTAL_ záznamů',
                        sInfoEmpty: 'Zobrazuji 0 až 0 z 0 záznamů',
                        sInfoFiltered: '(filtrováno z celkem _MAX_ záznamů)',
                        sInfoPostFix: '',
                        sInfoThousands: '',
                        sLengthMenu: 'Zobraz záznamů _MENU_',
                        sLoadingRecords: 'Načítám...',
                        sProcessing: 'Provádím...',
                        sSearch: 'Hledat:',
                        sZeroRecords: 'Žádné záznamy nebyly nalezeny',
                        oPaginate: {
                            sFirst: 'První',
                            sLast: 'Poslední',
                            sNext: 'Další',
                            sPrevious: 'Předchozí'
                        },
                        oAria: {
                            sSortAscending: ': aktivujte pro řazení sloupce vzestupně',
                            sSortDescending: ': aktivujte pro řazení sloupce sestupně'
                        }
                    },
                    drawCallback: function () {
                        var t = $(this);
                        t.find("thead th").each(function (i) {
                            if (/^(id|hiddenId|hidden__)/.test($(this).text())) {
                                t.find("td:nth-child(" + (i + 1) + "), th:nth-child(" + (i + 1) + ")").hide();
                            }
                        });
                    }
                });

                if (table.data('dtcolumnfilter') == '1') {
                    if (table.data('dtfilter') != '1') {
                        table.parent().find('.dataTables_filter').remove();
                    }

                    table.find('tfoot th').each(function () {
                        var title = $(this).text();
                        if (title == "Akce" || title == "Select All")
                            $(this).html("");
                        else
                            $(this).html('<input type="text" placeholder="" />');
                    });
                }
                else {
                    table.find('> tfoot').remove();
                }
                
                table.css("background-image", "initial");
                table.children("thead").css("visibility", "visible");
                table.children("tbody").css("visibility", "visible");
                table.children("tfoot").css("visibility", "visible");
        },

        filter: function () {
            var field = $(this);
            var dataTable = field.parents('.data-table').DataTable();
            var colIndex = field.parent().prevAll().length;

            dataTable.column(colIndex).search(this.value).draw();
        },

        onSearch: function () {
            var visibleRowList = "";
            var i = $(this).data('dtselect') == '1' ? 1 : 0;
            var dataTable = $(this).DataTable();
            dataTable.rows({ search: 'applied', selected: true }).data().each(function (value, index) {
                if (index > 0)
                    visibleRowList += ",";
                visibleRowList += value[i];
            });
            var tableName = $(this).attr("id");
            $('input[name="' + tableName + '"]').val(visibleRowList);
        },

        onAction: function () {
            var button = $(this);
            var confirm = button.data('confirm');

            if (button.attr("title") == "modal")
                return;
            if (confirm && confirm.length) {
                while (match = /(\{col_(\d)\})/.exec(confirm)) {
                    var colIndex = match[2];
                    var text = button.parents('tr').eq(0).find('td').eq(colIndex).text();

                    confirm = confirm.replace(match[1], text);
                }

                BootstrapUserInit.confirm(confirm, BootstrapUserInit.DataTable.doAction, null, this);
            }
            else {
                BootstrapUserInit.DataTable.doAction.apply(this, []);
            }
        },

        doAction: function () {
            var button = $(this);
            var rowId = parseInt(button.parents('tr').find('td:first').text());
            var tableName = button.parents('table').eq(0).attr('id');

            $.ajax({
                url: '/Persona/Account/GetAntiForgeryToken',
                type: 'GET',
                success: function (token) {
                    if (button.hasClass('fa-download')) {
                        window.ignoreUnload = true;
                    }

                    var form = $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="' + button.data('idparam') + '" value="' + rowId + '" /><input type="hidden" name="button" value="' + tableName + '_' + button.data('action') + '" /></form>');
                    form.append('<input type="hidden" name="__RequestVerificationToken" value="' + token + '" />');
                    form.appendTo('body').submit();
                }
            })
        },

    },

    loadValidators: function () {
        $.extend($.validator.methods, {
            auditNumber: function (value, element, attr) {
                return value.match(/^[0-9]{4} [PA] [0-9]{2,3}$/);
            }
        });
        $.extend($.validator.methods, {
            auditNumberNoWF: function (value, element, attr) {
                return value.match(/^[0-9]{4} [BCEFSZ] [0-9]{2,3}$/);
            }
        });
        $.extend($.validator.methods, {
            auditNumberNonWF: function (value, element, attr) {
                return value.match(/^[0-9]{4} C [0-9]{2,3}$/);
            }
        });
        $.extend($.validator.methods, {
            greaterThan: function (value, element, attr) {
                return this.optional(element) || +value > +attr;
            }
        });
        $.extend($.validator.methods, {
            greaterOrEqual: function (value, element, attr) {
                return this.optional(element) || +value >= +attr;
            }
        });
        $.extend($.validator.methods, {
            optionSelected: function (value, element, attr) {
                return $(element).attr("required") == undefined || +value != +attr;
            }
        });
        jQuery.validator.addClassRules("dropdown-select", {
            optionSelected: -1
        });

        mozaicFormValidator = $(".mozaicBootstrapPage form").validate({
            errorLabelContainer: $("<div>"), //put error messages into a detached element, AKA a trash bin; todo: find a better way to get rid of them
            ignore: "[readonly]",
            unhighlight: function (element) {
                $("button[ignoredonvalidation]").addClass("cancel");
                $(element).removeClass("has-error");

                // Element validator
                $('#' + element.id + '_validator').hide();

                if (this.numberOfInvalids() === 0) $("button:not([ignoredonvalidation])").removeClass("looks-disabled");
            },
            highlight: function (element) {
                $("button[ignoredonvalidation]").addClass("cancel");
                $(element).addClass("has-error");

                // Element validator
                $('#' + element.id + '_validator').show();

                $("button:not([ignoredonvalidation])").addClass("looks-disabled");
            }
        });
        if ($(".mozaicBootstrapPage from").length)
            mozaicFormValidator.form();
    }
};

$(function () {
    var bc = $('.mozaicBootstrapPage');
    if(bc.length)
    {
        BootstrapUserInit.init(bc);

        $(".input-with-datepicker").datetimepicker({
            datepicker: true,
            timepicker: false,
            format: "d.m.Y"
        }).off('mousewheel');
        $(".input-with-timepicker").datetimepicker({
            datepicker: false,
            timepicker: true,
            step: 5,
            format: "H:i:00"
        }).off('mousewheel');
        $(".input-with-datetimepicker").datetimepicker({
            datepicker: true,
            timepicker: true,
            step: 5,
            format: "d.m.Y H:i:00"
        }).off('mousewheel');
    }
});
// IE Buster

if (!window.jQuery) {
    var message;
    if (/^Mozilla\/4\.0.*\bMSIE\b/.test(navigator.userAgent)) {
        // (emulované) IE5 .. IE8 se hlásí jako Mozilla/4.0, novější prohlížeče jako Mozilla/5.0 a fungují
        message = "Omlouváme se, ale Vaše verze Internet Exploreru nepodporuje základní funkce jazyka Javascript, " +
        "které jsou pro chod aplikace nezbytné.  Kontaktujte helpdesk nebo administrátory platformy. ";
    } else {
        message = "Omlouváme se, ale verze Vašeho prohlížeče nepodporuje základní funkce jazyka Javascript, " +
            "které jsou pro chod aplikace nezbytné.  Kontaktujte helpdesk nebo administrátory platformy. ";
    }
    var style = "body {background: white !important} body > * {display:none !important} div:first-child {display:block !important; margin: 25px; border: 5px solid red; padding: 25px; font-weight: bold}";

    document.body.innerHTML = "<div>" + message + "</div><style> " + style + "</style>";
}
// Gets collisions of spec. element with elements meeting selector
function getCollisions(jqueryEl, selector) { //jqueryEl => target el, selector => list of colliders
    // To collect elements
    var collisions = [];
    // Search all the elements (collider)
    jqueryEl.parent().find(selector).each(function () {
        // If collider isn't target & they're colliding
        if (!$(this).is(jqueryEl) && areColliding($(this), jqueryEl)) {
            // Push it into array
            collisions.push($(this)[0]);
        }
    });
    // Return all colliders
    return collisions;
}

// Checks if two elements are colliding or not (boundary)
function areColliding(el1, el2) {
    // EL1 Props
    var el1top = el1.position().top;
    var el1left = el1.position().left;
    var el1width = el1.width();
    var el1height = el1.height();

    // EL2 Props
    var el2top = el2.position().top;
    var el2left = el2.position().left;
    var el2width = el2.width();
    var el2height = el2.height();

    // If left of element is inside other one
    var isInBoundX = Math.abs(el2left - el1left) <= el1width;
    // If top of element is inside other one
    var isInBoundY = Math.abs(el2top - el1top) <= el1height;

    // Checkout both of them (so it's inside it)
    return isInBoundX && isInBoundY;
}

function handleCollisions() {
    // Checkout all the panels
    $(".appPanel").each(function () {
        // Get panels colliding with the one that we are checking
        var collision = getCollisions($(this), ".appPanel");

        // Save info about what element we are actually checking
        var currentlyCheckedElement = this;

        // Search all elements that are colliding with our el.
        $.each(collision, function (i, value) {
            // Save info about element that collides with our el.
            var collidenElement = value;

            // If it's still colliding (it's changing while foreaching the loop)
            if (areColliding($(collidenElement), $(currentlyCheckedElement))) {

                // If they are at same high then move second one right
                if ($(collidenElement).position().top == $(currentlyCheckedElement).position().top) {
                    $(collidenElement).css("left", parseInt($(currentlyCheckedElement).css("left")) + $(currentlyCheckedElement).width() + 40);
                }

                // If they are at same "left" then move second one down
                if ($(collidenElement).position().left == $(currentlyCheckedElement).position().left) {
                    $(collidenElement).css("top", parseInt($(currentlyCheckedElement).css("top")) + $(currentlyCheckedElement).height() + 40);
                }
            }
        });
    });
}
function CurrentModuleIs(moduleClass) {
    return $("body").hasClass(moduleClass) ? true : false;
}
function CreateCzechDataTable(element, simpleMode) {
    featureSwitch = !simpleMode;
    var config = {};
    var locale = $('input#currentLocale').val();
    if (locale == "") {
        locale = "cs";
    }
    if (locale == "en") {
        config = {
            "paging": featureSwitch,
            "pageLength": 50,
            "lengthMenu": [[10, 20, 50, 100, 200, 500, 1000, -1], [10, 20, 50, 100, 200, 500, 1000, "Vše"]],
            "info": featureSwitch,
            "filter": featureSwitch,
            "order": [[0, "desc"]],
            "language": {
                "decimal": "",
                "emptyTable": "No data available in table",
                "info": "Showing _START_ to _END_ of _TOTAL_ entries",
                "infoEmpty": "Showing 0 to 0 of 0 entries",
                "infoFiltered": "(filtered from _MAX_ total entries)",
                "infoPostFix": "",
                "thousands": ",",
                "lengthMenu": "Show _MENU_ entries",
                "loadingRecords": "Loading...",
                "processing": "Processing...",
                "search": "Search:",
                "zeroRecords": "No matching records found",
                "paginate": {
                    "first": "First",
                    "last": "Last",
                    "next": "Next",
                    "previous": "Previous"
                },
                "aria": {
                    "sortAscending": ": activate to sort column ascending",
                    "sortDescending": ": activate to sort column descending"
                }
            }
        };
    } else {
        config = {
            "paging": featureSwitch,
            "pageLength": 50,
            "lengthMenu": [[10, 20, 50, 100, 200, 500, 1000, -1], [10, 20, 50, 100, 200, 500, 1000, "Vše"]],
            "info": featureSwitch,
            "filter": featureSwitch,
            "order": [[0, "desc"]],
            "language": {
                "sEmptyTable": "Tabulka neobsahuje žádná data",
                "sInfo": "Zobrazuji _START_ až _END_ z celkem _TOTAL_ záznamů",
                "sInfoEmpty": "Zobrazuji 0 až 0 z 0 záznamů",
                "sInfoFiltered": "(filtrováno z celkem _MAX_ záznamů)",
                "sInfoPostFix": "",
                "sInfoThousands": " ",
                "sLengthMenu": "Zobraz záznamů _MENU_",
                "sLoadingRecords": "Načítám...",
                "sProcessing": "Provádím...",
                "sSearch": "Hledat:",
                "sZeroRecords": "Žádné záznamy nebyly nalezeny",
                "oPaginate": {
                    "sFirst": "První",
                    "sLast": "Poslední",
                    "sNext": "Další",
                    "sPrevious": "Předchozí"
                },
                "oAria": {
                    "sSortAscending": ": aktivujte pro řazení sloupce vzestupně",
                    "sSortDescending": ": aktivujte pro řazení sloupce sestupně"
                }
            }
        };
    }

    if (element.attr("data-column-filter") !== undefined) {
        element.find("thead th:visible").each(function () {
            var wrapper = $("<span class='column-filter-check'></span>");
            var check = $("<input type='checkbox' data-select-column=\'this\' checked>");
            $(this).prepend(wrapper);
            wrapper.append(check);
            check.on("click", function (e) {
                e.stopPropagation();
            });
        });
    }

    // Main checkbox (all)
    var selection = element.find("[data-item-selection='\*']");

    // If checkbox found
    if (selection.length == 1) {

        var main = $(selection[0]);

        // Child checkboxes
        var checkes = element.find("[data-item-selection=\'row\']");

        // When clicked on main
        main.on("change", function () {
            // Titles depending on state
            if (main.is(":checked")) {
                main.attr("title", "Odoznačit vše");
            } else {
                main.attr("title", "Označit vše");
            }

            // Iterate all row checkboxes
            checkes.each(function () {
                // Change them to value of main
                $(this).prop("checked", main.is(":checked"));

                // Trigger change event
                $(this).trigger("change");
            });
        });

        // When any of row checks changes
        checkes.on("change", function () {
            // If it's checked
            if ($(this).is(":checked")) {
                // Add attr to row
                $(this).parent().parent().attr("data-row-selected", true);
            } else {
                // Remove attr from row
                $(this).parent().parent().removeAttr("data-row-selected");
            }
            // If main isn't checked but row is, then check main !WITHOUT triggering EVENT!
            if (!main.is(":checked") && $(this).is(":checked")) {
                main.prop("checked", true);
                main.attr("title", "Odoznačit vše");
            }
        });

        // If row select mode is enabled (selecting by clicking on rows)
        if (element.attr("data-select-mode") === "row") {

            // Find all rows and bind event to them
            element.find("tbody tr").on("click", function (e) {
                if (!$(e.target).is("[data-item-selection=\'row\']")) {
                    // Get coresp. checkbox
                    var check = $(this).find("[data-item-selection=\'row\']");

                    // Change it & trigger event
                    check.prop("checked", !check.is(":checked"));
                    check.trigger("change");
                }
            });
        }

        // Find column of checkboxes
        var selectionColumnId = selection.parent().index();

        // Disable sorting for them
        config.columnDefs = [{
            orderable: false,
            targets: selectionColumnId
        }];

        // Change default order to another column
        config.order = [[selectionColumnId + 1, "desc"]];
    }

    // Create instance of datatable
    var dtb = element.DataTable(config);
}
jQuery(function ($) {
    var locale = $('input#currentLocale').val();
    if (locale == "") {
        locale = "cs";
    }
    if (locale == "cs") {
        jQuery.datetimepicker.setLocale('cs');
    } else {
        jQuery.datetimepicker.setLocale('en');

    }
   
});
$.countdown.setDefaults($.countdown.regionalOptions['cs']);
function CreateColorPicker(target) {
    target.spectrum({
        showPaletteOnly: true,
        togglePaletteOnly: true,
        togglePaletteMoreText: 'more',
        togglePaletteLessText: 'less',
        color: '#f00',
        palette: [
            ["#000", "#444", "#666", "#999", "#ccc", "#eee", "#f3f3f3", "#fff"],
            ["#f00", "#f90", "#ff0", "#0f0", "#0ff", "#00f", "#90f", "#f0f"],
            ["#f4cccc", "#fce5cd", "#fff2cc", "#d9ead3", "#d0e0e3", "#cfe2f3", "#d9d2e9", "#ead1dc"],
            ["#ea9999", "#f9cb9c", "#ffe599", "#b6d7a8", "#a2c4c9", "#9fc5e8", "#b4a7d6", "#d5a6bd"],
            ["#e06666", "#f6b26b", "#ffd966", "#93c47d", "#76a5af", "#6fa8dc", "#8e7cc3", "#c27ba0"],
            ["#c00", "#e69138", "#f1c232", "#6aa84f", "#45818e", "#3d85c6", "#674ea7", "#a64d79"],
            ["#900", "#b45f06", "#bf9000", "#38761d", "#134f5c", "#0b5394", "#351c75", "#741b47"],
            ["#600", "#783f04", "#7f6000", "#274e13", "#0c343d", "#073763", "#20124d", "#4c1130"]
        ]
    });
}

(function ($) {
    $.fn.trackInputDone = function (selector, cb) {
        if (typeof selector === "function") {
            cb = selector;
            selector = null;
        }
        if (cb) this.on("inputDone", selector, cb);

        this.on("input keypress", function (e) {
            if (e.type === "keypress" && e.which === 13) {
                e.preventDefault();
                recheckInput.call(this);
            } else {
                clearTimeout($(this).data("inputTrackerTimeout"));
                $(this).data("inputTrackerTimeout", setTimeout(recheckInput.bind(this), 1000));
            }
        }).on("change", recheckInput);

        function recheckInput() {
            if ($(this).data("inputTrackerPrevValue") !== this.value) {
                $(this).trigger("inputDone");
                $(this).data("inputTrackerPrevValue", this.value);
            }
        }
    }
})(jQuery);

$(document).on("resize", ".ui-dialog", function (e, ui) {
    setTimeout(function () {
        ui.element.find(".ui-dialog-content").css({
            width: "auto",
            height: "calc(100% - 6em)"
        });
    }, 0);
})

var defaultDiacriticsRemovalMap = [
    { 'base': 'A', 'letters': '\u0041\u24B6\uFF21\u00C0\u00C1\u00C2\u1EA6\u1EA4\u1EAA\u1EA8\u00C3\u0100\u0102\u1EB0\u1EAE\u1EB4\u1EB2\u0226\u01E0\u00C4\u01DE\u1EA2\u00C5\u01FA\u01CD\u0200\u0202\u1EA0\u1EAC\u1EB6\u1E00\u0104\u023A\u2C6F' },
    { 'base': 'AA', 'letters': '\uA732' },
    { 'base': 'AE', 'letters': '\u00C6\u01FC\u01E2' },
    { 'base': 'AO', 'letters': '\uA734' },
    { 'base': 'AU', 'letters': '\uA736' },
    { 'base': 'AV', 'letters': '\uA738\uA73A' },
    { 'base': 'AY', 'letters': '\uA73C' },
    { 'base': 'B', 'letters': '\u0042\u24B7\uFF22\u1E02\u1E04\u1E06\u0243\u0182\u0181' },
    { 'base': 'C', 'letters': '\u0043\u24B8\uFF23\u0106\u0108\u010A\u010C\u00C7\u1E08\u0187\u023B\uA73E' },
    { 'base': 'D', 'letters': '\u0044\u24B9\uFF24\u1E0A\u010E\u1E0C\u1E10\u1E12\u1E0E\u0110\u018B\u018A\u0189\uA779\u00D0' },
    { 'base': 'DZ', 'letters': '\u01F1\u01C4' },
    { 'base': 'Dz', 'letters': '\u01F2\u01C5' },
    { 'base': 'E', 'letters': '\u0045\u24BA\uFF25\u00C8\u00C9\u00CA\u1EC0\u1EBE\u1EC4\u1EC2\u1EBC\u0112\u1E14\u1E16\u0114\u0116\u00CB\u1EBA\u011A\u0204\u0206\u1EB8\u1EC6\u0228\u1E1C\u0118\u1E18\u1E1A\u0190\u018E' },
    { 'base': 'F', 'letters': '\u0046\u24BB\uFF26\u1E1E\u0191\uA77B' },
    { 'base': 'G', 'letters': '\u0047\u24BC\uFF27\u01F4\u011C\u1E20\u011E\u0120\u01E6\u0122\u01E4\u0193\uA7A0\uA77D\uA77E' },
    { 'base': 'H', 'letters': '\u0048\u24BD\uFF28\u0124\u1E22\u1E26\u021E\u1E24\u1E28\u1E2A\u0126\u2C67\u2C75\uA78D' },
    { 'base': 'I', 'letters': '\u0049\u24BE\uFF29\u00CC\u00CD\u00CE\u0128\u012A\u012C\u0130\u00CF\u1E2E\u1EC8\u01CF\u0208\u020A\u1ECA\u012E\u1E2C\u0197' },
    { 'base': 'J', 'letters': '\u004A\u24BF\uFF2A\u0134\u0248' },
    { 'base': 'K', 'letters': '\u004B\u24C0\uFF2B\u1E30\u01E8\u1E32\u0136\u1E34\u0198\u2C69\uA740\uA742\uA744\uA7A2' },
    { 'base': 'L', 'letters': '\u004C\u24C1\uFF2C\u013F\u0139\u013D\u1E36\u1E38\u013B\u1E3C\u1E3A\u0141\u023D\u2C62\u2C60\uA748\uA746\uA780' },
    { 'base': 'LJ', 'letters': '\u01C7' },
    { 'base': 'Lj', 'letters': '\u01C8' },
    { 'base': 'M', 'letters': '\u004D\u24C2\uFF2D\u1E3E\u1E40\u1E42\u2C6E\u019C' },
    { 'base': 'N', 'letters': '\u004E\u24C3\uFF2E\u01F8\u0143\u00D1\u1E44\u0147\u1E46\u0145\u1E4A\u1E48\u0220\u019D\uA790\uA7A4' },
    { 'base': 'NJ', 'letters': '\u01CA' },
    { 'base': 'Nj', 'letters': '\u01CB' },
    { 'base': 'O', 'letters': '\u004F\u24C4\uFF2F\u00D2\u00D3\u00D4\u1ED2\u1ED0\u1ED6\u1ED4\u00D5\u1E4C\u022C\u1E4E\u014C\u1E50\u1E52\u014E\u022E\u0230\u00D6\u022A\u1ECE\u0150\u01D1\u020C\u020E\u01A0\u1EDC\u1EDA\u1EE0\u1EDE\u1EE2\u1ECC\u1ED8\u01EA\u01EC\u00D8\u01FE\u0186\u019F\uA74A\uA74C' },
    { 'base': 'OI', 'letters': '\u01A2' },
    { 'base': 'OO', 'letters': '\uA74E' },
    { 'base': 'OU', 'letters': '\u0222' },
    { 'base': 'OE', 'letters': '\u008C\u0152' },
    { 'base': 'oe', 'letters': '\u009C\u0153' },
    { 'base': 'P', 'letters': '\u0050\u24C5\uFF30\u1E54\u1E56\u01A4\u2C63\uA750\uA752\uA754' },
    { 'base': 'Q', 'letters': '\u0051\u24C6\uFF31\uA756\uA758\u024A' },
    { 'base': 'R', 'letters': '\u0052\u24C7\uFF32\u0154\u1E58\u0158\u0210\u0212\u1E5A\u1E5C\u0156\u1E5E\u024C\u2C64\uA75A\uA7A6\uA782' },
    { 'base': 'S', 'letters': '\u0053\u24C8\uFF33\u1E9E\u015A\u1E64\u015C\u1E60\u0160\u1E66\u1E62\u1E68\u0218\u015E\u2C7E\uA7A8\uA784' },
    { 'base': 'T', 'letters': '\u0054\u24C9\uFF34\u1E6A\u0164\u1E6C\u021A\u0162\u1E70\u1E6E\u0166\u01AC\u01AE\u023E\uA786' },
    { 'base': 'TZ', 'letters': '\uA728' },
    { 'base': 'U', 'letters': '\u0055\u24CA\uFF35\u00D9\u00DA\u00DB\u0168\u1E78\u016A\u1E7A\u016C\u00DC\u01DB\u01D7\u01D5\u01D9\u1EE6\u016E\u0170\u01D3\u0214\u0216\u01AF\u1EEA\u1EE8\u1EEE\u1EEC\u1EF0\u1EE4\u1E72\u0172\u1E76\u1E74\u0244' },
    { 'base': 'V', 'letters': '\u0056\u24CB\uFF36\u1E7C\u1E7E\u01B2\uA75E\u0245' },
    { 'base': 'VY', 'letters': '\uA760' },
    { 'base': 'W', 'letters': '\u0057\u24CC\uFF37\u1E80\u1E82\u0174\u1E86\u1E84\u1E88\u2C72' },
    { 'base': 'X', 'letters': '\u0058\u24CD\uFF38\u1E8A\u1E8C' },
    { 'base': 'Y', 'letters': '\u0059\u24CE\uFF39\u1EF2\u00DD\u0176\u1EF8\u0232\u1E8E\u0178\u1EF6\u1EF4\u01B3\u024E\u1EFE' },
    { 'base': 'Z', 'letters': '\u005A\u24CF\uFF3A\u0179\u1E90\u017B\u017D\u1E92\u1E94\u01B5\u0224\u2C7F\u2C6B\uA762' },
    { 'base': 'a', 'letters': '\u0061\u24D0\uFF41\u1E9A\u00E0\u00E1\u00E2\u1EA7\u1EA5\u1EAB\u1EA9\u00E3\u0101\u0103\u1EB1\u1EAF\u1EB5\u1EB3\u0227\u01E1\u00E4\u01DF\u1EA3\u00E5\u01FB\u01CE\u0201\u0203\u1EA1\u1EAD\u1EB7\u1E01\u0105\u2C65\u0250' },
    { 'base': 'aa', 'letters': '\uA733' },
    { 'base': 'ae', 'letters': '\u00E6\u01FD\u01E3' },
    { 'base': 'ao', 'letters': '\uA735' },
    { 'base': 'au', 'letters': '\uA737' },
    { 'base': 'av', 'letters': '\uA739\uA73B' },
    { 'base': 'ay', 'letters': '\uA73D' },
    { 'base': 'b', 'letters': '\u0062\u24D1\uFF42\u1E03\u1E05\u1E07\u0180\u0183\u0253' },
    { 'base': 'c', 'letters': '\u0063\u24D2\uFF43\u0107\u0109\u010B\u010D\u00E7\u1E09\u0188\u023C\uA73F\u2184' },
    { 'base': 'd', 'letters': '\u0064\u24D3\uFF44\u1E0B\u010F\u1E0D\u1E11\u1E13\u1E0F\u0111\u018C\u0256\u0257\uA77A' },
    { 'base': 'dz', 'letters': '\u01F3\u01C6' },
    { 'base': 'e', 'letters': '\u0065\u24D4\uFF45\u00E8\u00E9\u00EA\u1EC1\u1EBF\u1EC5\u1EC3\u1EBD\u0113\u1E15\u1E17\u0115\u0117\u00EB\u1EBB\u011B\u0205\u0207\u1EB9\u1EC7\u0229\u1E1D\u0119\u1E19\u1E1B\u0247\u025B\u01DD' },
    { 'base': 'f', 'letters': '\u0066\u24D5\uFF46\u1E1F\u0192\uA77C' },
    { 'base': 'g', 'letters': '\u0067\u24D6\uFF47\u01F5\u011D\u1E21\u011F\u0121\u01E7\u0123\u01E5\u0260\uA7A1\u1D79\uA77F' },
    { 'base': 'h', 'letters': '\u0068\u24D7\uFF48\u0125\u1E23\u1E27\u021F\u1E25\u1E29\u1E2B\u1E96\u0127\u2C68\u2C76\u0265' },
    { 'base': 'hv', 'letters': '\u0195' },
    { 'base': 'i', 'letters': '\u0069\u24D8\uFF49\u00EC\u00ED\u00EE\u0129\u012B\u012D\u00EF\u1E2F\u1EC9\u01D0\u0209\u020B\u1ECB\u012F\u1E2D\u0268\u0131' },
    { 'base': 'j', 'letters': '\u006A\u24D9\uFF4A\u0135\u01F0\u0249' },
    { 'base': 'k', 'letters': '\u006B\u24DA\uFF4B\u1E31\u01E9\u1E33\u0137\u1E35\u0199\u2C6A\uA741\uA743\uA745\uA7A3' },
    { 'base': 'l', 'letters': '\u006C\u24DB\uFF4C\u0140\u013A\u013E\u1E37\u1E39\u013C\u1E3D\u1E3B\u017F\u0142\u019A\u026B\u2C61\uA749\uA781\uA747' },
    { 'base': 'lj', 'letters': '\u01C9' },
    { 'base': 'm', 'letters': '\u006D\u24DC\uFF4D\u1E3F\u1E41\u1E43\u0271\u026F' },
    { 'base': 'n', 'letters': '\u006E\u24DD\uFF4E\u01F9\u0144\u00F1\u1E45\u0148\u1E47\u0146\u1E4B\u1E49\u019E\u0272\u0149\uA791\uA7A5' },
    { 'base': 'nj', 'letters': '\u01CC' },
    { 'base': 'o', 'letters': '\u006F\u24DE\uFF4F\u00F2\u00F3\u00F4\u1ED3\u1ED1\u1ED7\u1ED5\u00F5\u1E4D\u022D\u1E4F\u014D\u1E51\u1E53\u014F\u022F\u0231\u00F6\u022B\u1ECF\u0151\u01D2\u020D\u020F\u01A1\u1EDD\u1EDB\u1EE1\u1EDF\u1EE3\u1ECD\u1ED9\u01EB\u01ED\u00F8\u01FF\u0254\uA74B\uA74D\u0275' },
    { 'base': 'oi', 'letters': '\u01A3' },
    { 'base': 'ou', 'letters': '\u0223' },
    { 'base': 'oo', 'letters': '\uA74F' },
    { 'base': 'p', 'letters': '\u0070\u24DF\uFF50\u1E55\u1E57\u01A5\u1D7D\uA751\uA753\uA755' },
    { 'base': 'q', 'letters': '\u0071\u24E0\uFF51\u024B\uA757\uA759' },
    { 'base': 'r', 'letters': '\u0072\u24E1\uFF52\u0155\u1E59\u0159\u0211\u0213\u1E5B\u1E5D\u0157\u1E5F\u024D\u027D\uA75B\uA7A7\uA783' },
    { 'base': 's', 'letters': '\u0073\u24E2\uFF53\u00DF\u015B\u1E65\u015D\u1E61\u0161\u1E67\u1E63\u1E69\u0219\u015F\u023F\uA7A9\uA785\u1E9B' },
    { 'base': 't', 'letters': '\u0074\u24E3\uFF54\u1E6B\u1E97\u0165\u1E6D\u021B\u0163\u1E71\u1E6F\u0167\u01AD\u0288\u2C66\uA787' },
    { 'base': 'tz', 'letters': '\uA729' },
    { 'base': 'u', 'letters': '\u0075\u24E4\uFF55\u00F9\u00FA\u00FB\u0169\u1E79\u016B\u1E7B\u016D\u00FC\u01DC\u01D8\u01D6\u01DA\u1EE7\u016F\u0171\u01D4\u0215\u0217\u01B0\u1EEB\u1EE9\u1EEF\u1EED\u1EF1\u1EE5\u1E73\u0173\u1E77\u1E75\u0289' },
    { 'base': 'v', 'letters': '\u0076\u24E5\uFF56\u1E7D\u1E7F\u028B\uA75F\u028C' },
    { 'base': 'vy', 'letters': '\uA761' },
    { 'base': 'w', 'letters': '\u0077\u24E6\uFF57\u1E81\u1E83\u0175\u1E87\u1E85\u1E98\u1E89\u2C73' },
    { 'base': 'x', 'letters': '\u0078\u24E7\uFF58\u1E8B\u1E8D' },
    { 'base': 'y', 'letters': '\u0079\u24E8\uFF59\u1EF3\u00FD\u0177\u1EF9\u0233\u1E8F\u00FF\u1EF7\u1E99\u1EF5\u01B4\u024F\u1EFF' },
    { 'base': 'z', 'letters': '\u007A\u24E9\uFF5A\u017A\u1E91\u017C\u017E\u1E93\u1E95\u01B6\u0225\u0240\u2C6C\uA763' }
];

var diacriticsMap = {};
for (var i = 0; i < defaultDiacriticsRemovalMap.length; i++) {
    var letters = defaultDiacriticsRemovalMap[i].letters;
    for (var j = 0; j < letters.length; j++) {
        diacriticsMap[letters[j]] = defaultDiacriticsRemovalMap[i].base;
    }
}

function RemoveDiacritics(str) {
    return str.replace(/[^\u0000-\u007E]/g, function (a) {
        return diacriticsMap[a] || a;
    });
}