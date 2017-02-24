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
        if ($("#currentBlockName").val() == "VracenoKPrepracovaniNadrizenym" || $("#currentBlockName").val() == "VracenoKPrepracovaniAuditorem")
        {
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
        if ($("#currentBlockName").val() == "EditaceOpatreniVReseni") {
            $("[name=STATUS_radio]").on("change", function () {
                if ($(this).val() === "realizovano") {
                    $("[name=DUVOD_POSUNUTI_textbox]").prop("readonly", true);
                    $("[name=NT_REALIZACE_date]").prop("readonly", true);
                } else {
                    $("[name=DUVOD_POSUNUTI_textbox]").prop("readonly", false);
                    $("[name=NT_REALIZACE_date").prop("readonly", false);
                }
            });
        }
       
    
    else if ($("#currentBlockName").val() == "ZadaniObjednavkyPeriodika") {
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
                complete: function(){spinner.remove()},
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
                complete: function(){spinner.remove()},
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
                complete: function(){spinner.remove()},
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
                complete: function(){spinner.remove()},
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
                    complete: function(){spinner.remove()},
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
                    complete: function(){spinner.remove()},
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
                    complete: function(){spinner.remove()},
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
                complete: function(){spinner.remove()},
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
                complete: function(){spinner.remove()},
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

            function submitActionByForm(tableName, rowId, action) {
                // Create
                var form = $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="modelId" value="' + rowId + '" /><input type="hidden" name="button" value="' + tableName + '_' + action + '" /></form>');
                
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
                    var rowId = parseInt($(this).parents("tr").find("td:first").text());
                    var tableName = table.attr("name");

                    submitActionByForm(tableName, rowId, "DeleteAction");
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
                    if ($(element).text() == "id" || $(element).text().indexOf("hidden__") == 0) {
                        t.find("td:nth-child(" + (index + 1) + "), th:nth-child(" + (index + 1) + ")").hide();
                    }
                });
            });
            table.DataTable().draw();
            if (!table.hasClass("data-table-simple-mode")) {
                table.find("tfoot th").each(function () {
                    var title = $(this).text();
                    if (title != "Akce")
                        $(this).html('<input type="text" placeholder="Hledat v &quot;' + title + '&quot;" />');
                    else
                        $(this).html("");
                });
                dataTable = table.DataTable();
                dataTable.columns().eq(0).each(function (colIdx) {
                    $("input", dataTable.column(colIdx).footer()).on("keyup change", function () {
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
                return value.match(/^[0-9]{4} [BCEQ] [0-9]{2,3}$/);
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
                if (this.numberOfInvalids() === 0) $(".uic.button-simple:not([ignoredonvalidation])").removeClass("looks-disabled");
            },
            highlight: function (element) {
                $(".uic[ignoredonvalidation]").addClass("cancel");
                $(element).addClass("has-error");
                $(".uic.button-simple:not([ignoredonvalidation])").addClass("looks-disabled");
            }
        });
        if ($(".mozaicForm").length)
            mozaicFormValidator.form();

        $(".uic.button-simple, .uic.button-dropdown").on("click", function () {
            $(".uic.data-table").each(function (tableIndex, tableElement) {
                var visibleRowList = "";
                var dataTable = $(tableElement).DataTable();
                dataTable.rows({ search: 'applied' }).data().each(function (value, index) {
                    if (index > 0)
                        visibleRowList += ",";
                    visibleRowList += value[0];
                });
                tableName = $(tableElement).attr("name");
                $('input[name="' + tableName + '"').val(visibleRowList);
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

    init: function(bootstrapContext)
    {
        var self = BootstrapUserInit;
        self.context = bootstrapContext;

        $(self.context)
            .on('keyup change', '.data-table > tfoot input', self.DataTable.filter)
            .on('click', '.data-table .actionIcons i.fa', self.DataTable.onAction)
        ;

        self.DataTable.init();
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
        init: function ()
        {
            var self = BootstrapUserInit;

            $('.data-table', self.context).each(function () {
                var table = $(this);

                table.DataTable({
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
                        if (title != "Akce")
                            $(this).html('<input type="text" placeholder="Hledat v &quot;' + title + '&quot;" />');
                        else
                            $(this).html("");
                    });
                }
                else {
                    table.find('> tfoot').remove();
                }

                //table.DataTable().draw();

                /*
                CreateCzechDataTable(table, table.hasClass("data-table-simple-mode"));
                
                table.on("click", ".rowEditAction", function () {
                    var rowId = parseInt($(this).parents("tr").find("td:first").text());
                    var tableName = table.attr("name");
                    if ($(this).hasClass("fa-download"))
                        window.ignoreUnload = true;
                    $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="modelId" value="' + rowId + '" /><input type="hidden" name="button" value="' + tableName + '_EditAction" /></form>').appendTo('body').submit();
                });
                table.on("click", ".rowDetailsAction", function () {
                    var rowId = parseInt($(this).parents("tr").find("td:first").text());
                    var tableName = table.attr("name");
                    $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="modelId" value="' + rowId + '" /><input type="hidden" name="button" value="' + tableName + '_DetailsAction" /></form>').appendTo('body').submit();
                });
                table.on("click", ".rowDeleteAction", function () {
                    if (confirm('Jste si jistí?')) {
                        var rowId = parseInt($(this).parents("tr").find("td:first").text());
                        var tableName = table.attr("name");
                        $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="deleteId" value="' + rowId + '" /><input type="hidden" name="button" value="' + tableName + '_DeleteAction" /></form>').appendTo('body').submit();
                    }
                });
                table.on("click", ".row_A_Action", function () {
                    var rowId = parseInt($(this).parents("tr").find("td:first").text());
                    var tableName = table.attr("name");
                    $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="modelId" value="' + rowId + '" /><input type="hidden" name="button" value="' + tableName + '_A_Action" /></form>').appendTo('body').submit();
                });
                table.on("click", ".row_B_Action", function () {
                    var rowId = parseInt($(this).parents("tr").find("td:first").text());
                    var tableName = table.attr("name");
                    $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="modelId" value="' + rowId + '" /><input type="hidden" name="button" value="' + tableName + '_B_Action" /></form>').appendTo('body').submit();
                });
                
                
                if (!table.hasClass("data-table-simple-mode")) {
                    
                    dataTable = table.DataTable();
                 
                    if ($("#currentBlockName").val() == "ZakladniReport") {
                        var currentUser = $("#currentUserName").val();
                        dataTable
                            .order([1, 'desc'])
                            .column(9)
                            .search(currentUser)
                            .draw();
                        table.find("tfoot th:nth-child(10) input").val(currentUser);
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
                }*/
            });
        },

        filter: function ()
        {
            var field = $(this);
            var dataTable = field.parents('.data-table').DataTable();
            var colIndex = field.parent().prevAll().length;

            dataTable.column(colIndex).search(this.value).draw();
        },

        onAction: function()
        {
            var button = $(this);
            var confirm = button.data('confirm');

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

        doAction: function()
        {
            var button = $(this);
            var rowId = parseInt(button.parents('tr').find('td:first').text());
            var tableName = button.parents('table').eq(0).attr('id');
            
            if (button.hasClass('fa-download')) {
                window.ignoreUnload = true; 
            }

            $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="' + button.data('idparam') + '" value="' + rowId + '" /><input type="hidden" name="button" value="' + tableName + '_' + button.data('action') + '" /></form>').appendTo('body').submit();
        }
    }
};

$(function () {

    var bc = $('.mozaicBootstrapPage');
    if(bc.length)
    {
        BootstrapUserInit.init(bc);
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

function CurrentModuleIs(moduleClass) {
    return $("body").hasClass(moduleClass) ? true : false;
}
function CreateCzechDataTable(element, simpleMode) {
    featureSwitch = !simpleMode;
    element.DataTable({
        "paging": featureSwitch,
        "pageLength": 50,
        "lengthMenu": [[10, 20, 50, 100, 200, 500, 1000, -1], [10, 20, 50, 100, 200, 500, 1000, "Vše"]],
        "info": featureSwitch,
        "filter": featureSwitch,
        "order": [[0, "desc"]],
        "language": {
            "sEmptyTable":     "Tabulka neobsahuje žádná data",
            "sInfo":           "Zobrazuji _START_ až _END_ z celkem _TOTAL_ záznamů",
            "sInfoEmpty":      "Zobrazuji 0 až 0 z 0 záznamů",
            "sInfoFiltered":   "(filtrováno z celkem _MAX_ záznamů)",
            "sInfoPostFix":    "",
            "sInfoThousands":  " ",
            "sLengthMenu":     "Zobraz záznamů _MENU_",
            "sLoadingRecords": "Načítám...",
            "sProcessing":     "Provádím...",
            "sSearch":         "Hledat:",
            "sZeroRecords":    "Žádné záznamy nebyly nalezeny",
            "oPaginate": {
                "sFirst":    "První",
                "sLast":     "Poslední",
                "sNext":     "Další",
                "sPrevious": "Předchozí"
            },
            "oAria": {
                "sSortAscending":  ": aktivujte pro řazení sloupce vzestupně",
                "sSortDescending": ": aktivujte pro řazení sloupce sestupně"
            }
        }
    });
}
jQuery(function ($) {
    $.datepicker.regional['cs'] = {
        closeText: 'Zavřít',
        prevText: 'Předchozí',
        nextText: 'Další',
        currentText: 'Dnes',
        monthNames: ['Leden', 'Únor', 'Březen', 'Duben', 'Květen', 'Červen', 'Červenec', 'Srpen', 'Září', 'Říjen', 'Listopad', 'Prosinec'],
        monthNamesShort: ['Led', 'Úno', 'Bře', 'Dub', 'Kvě', 'Čer', 'Čvc', 'Srp', 'Zář', 'Říj', 'Lis', 'Pro'],
        dayNames: ['Neděle', 'Pondělí', 'Úterý', 'Středa', 'Čtvrtek', 'Pátek', 'Sobota'],
        dayNamesShort: ['Ne', 'Po', 'Út', 'St', 'Čt', 'Pá', 'So', ],
        dayNamesMin: ['Ne', 'Po', 'Út', 'St', 'Čt', 'Pá', 'So'],
        weekHeader: 'Týd',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $.datepicker.setDefaults($.datepicker.regional['cs']);
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