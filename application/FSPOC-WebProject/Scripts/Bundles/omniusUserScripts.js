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
    mozaicForm.find(".uic").each(function (index, element) {
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

    panel.find(".uic").each(function (index, element) {
        currentUic = $(element);
        if (currentUic.position().left + currentUic.width() + 30 > panelWidth) {
            panelWidth = currentUic.position().left + currentUic.width() + 30;
        }
        if (currentUic.position().top + currentUic.height() + 30 > panelHeight) {
            if (currentUic.hasClass("data-table")) {
                panelHeight = currentUic.position().top + currentUic.height() + 60;
            }
            else {
                panelHeight = currentUic.position().top + currentUic.height() + 30;
            }
        }
    });
    panel.width(panelWidth);
    panel.height(panelHeight);
}
$(function () {
    $(document).on("click", "[data-ajax='true']", function () {
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
        $("#userLeftBar").css("height", $(window).height() + $(window).scrollTop() - 50);
        $(window).scroll(function () {
            $("#userLeftBar").css("height", $(window).height() + $(window).scrollTop() - 50);
        });
        $(window).resize(function () {
            $("#userLeftBar").css("height", $(window).height() + $(window).scrollTop() - 50);
        });
        try {
            $(".uic.data-table").each(function (index, element) {
            var table = $(element);
            var tableWidth = parseInt(table.attr("uicWidth"));
            CreateCzechDataTable(table, table.hasClass("data-table-simple-mode"));
            wrapper = table.parents(".dataTables_wrapper");
            wrapper.css("position", "absolute");
            wrapper.css("left", table.css("left"));
            wrapper.css("top", table.css("top"));
            wrapper.css("width", tableWidth + 8);
            table.css("position", "relative");
            table.css("left", "0px");
            table.css("top", "0px");
            table.wrap("<div class='inner_wrapper'>");
            table.on("click", ".rowEditAction", function () {
                rowId = parseInt($(this).parents("tr").find("td:first").text());
                $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="modelId" value="' + rowId + '" /><input type="hidden" name="button" value="datatable_edit" /></form>').appendTo('body').submit();
            });
            table.on("click", ".rowDetailsAction", function () {
                rowId = parseInt($(this).parents("tr").find("td:first").text());
                $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="modelId" value="' + rowId + '" /><input type="hidden" name="button" value="datatable_detail" /></form>').appendTo('body').submit();
            });
            table.on("click", ".rowDeleteAction", function () {
                if (confirm('Jste si jistí?')) {
                    rowId = parseInt($(this).parents("tr").find("td:first").text());
                    $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="modelId" value="' + rowId + '" /><input type="hidden" name="button" value="datatable_delete" /></form>').appendTo('body').submit();
                }
            });
            table.on("click", ".row_A_Action", function () {
                    rowId = parseInt($(this).parents("tr").find("td:first").text());
                    $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="modelId" value="' + rowId + '" /><input type="hidden" name="button" value="datatable_actionA" /></form>').appendTo('body').submit();
                });
            table.on("click", ".row_B_Action", function () {
                    rowId = parseInt($(this).parents("tr").find("td:first").text());
                    $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="modelId" value="' + rowId + '" /><input type="hidden" name="button" value="datatable_actionB" /></form>').appendTo('body').submit();
            });
            table.DataTable().on("draw", function () {
                var t = $(this);
                t.find("thead th").each(function (index, element) {
                    if ($(element).text() == "id" || $(element).text().indexOf("hidden__") == 0) {
                        t.find("td:nth-child(" + (index + 1) + "), th:nth-child(" + (index + 1) + ")").hide();
                    }
                });
            });
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
                if (this.numberOfInvalids() === 0) $("[disableOnError]").removeClass("looks-disabled");
            },
            highlight: function (element) {
                $(".uic[ignoredonvalidation]").addClass("cancel");
                $(element).addClass("has-error");
                $("[disableOnError]").addClass("looks-disabled");
            }
        });
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
        $(".uic.input-with-datepicker").datepicker($.datepicker.regional['cs']);
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