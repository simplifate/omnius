$(function () {
    if ($("#currentBlockName").val() == "HlavniStranka") {
        if (UserHasOneOfTheseRoles(["Admin", "Coordinator"]))
            $("#uic_auction_management_button").show();
    }
    else if ($("#currentBlockName").val() == "OvereniOrganizace") {
        var deposit_done = false;
        $("#uic_next_button").show().prop({ disabled: true });
        $("#uic_organizations_dropdown").on("change", function (e) {
            var orgId = $(this).val();
            if (orgId == -1)
            {
                $("#uic_deposit_panel, #uic_give_up_panel, #uic_conditions_panel, #uic_next_button").prop({ disabled: true });
            }
            else
            {
                $.ajax({
                    type: "POST",
                    url: "/api/run/Aukcnisystem/" + $("#currentBlockName").val() + "/?button=" + $(this).attr("name"),
                    data: { "targetId": $(this).val(), "auctionId": GetUrlParameter("modelId") },
                    success: function (data) {
                        deposit_done = data.deposit;
                        $("#uic_next_button").prop({ disabled: !data.accepted || !data.deposit });
                        $("#uic_conditions_panel").toggle( !data.accepted );
                        $("#uic_agree_checkbox input").prop({ checked: data.accepted });
                        $("#uic_deposit_panel").toggle(!data.deposit);
                        $("#uic_give_up_panel").toggle( !data.gaveUp );
                        $("#uic_lblGaveUp").prop({ visible: data.gaveUp });
                    }
                });
            }
        });
        $("#uic_agree_checkbox input").on("click", function (e) {
            if($("#uic_agree_checkbox input").prop("checked"))
                $("#uic_next_button").prop({ disabled: !deposit_done });
            else
                $("#uic_next_button").prop({ disabled: true });
        });
    }
    else if ($("#currentBlockName").val() == "VlozeniPoptavky") {
        var registry = JSON.parse(GetUrlParameter("registry"));
        $('input[name=agreeWithSpeedingUpProcess]').prop('checked', true);

        var periodDD = $('#uic_obdobi_dropdown');
        var periodFromDD = $('#uic_obdobi_od_dropdown');

        $('#uic_obdobi_dropdown, #uic_obdobi_od_dropdown').change(function () {
            if (Number(periodFromDD.val()) > Number(periodDD.val())) {
                $('#uic_obdobi_od_dropdown_error').show();
            }
            else {
                $('#uic_obdobi_od_dropdown_error').hide();
            }
            periodFromDD.attr('max', periodDD.val());
            mozaicFormValidator.form();
        });
        

        function CheckPercentageValue()
        {
            $("#uic_provozni_objem_procenta_textbox_error").hide();
            $("#uic_tezebni_vykon_procenta_textbox_error").hide();
            $("#uic_vtlaceci_vykon_procenta_textbox_error").hide();

            if (Number($("#uic_provozni_objem_procenta_textbox").val()) > 100) {
                if (!$('#uic_provozni_objem_procenta_textbox').is('[readonly]') || !$('#uic_provozni_objem_jednotky_textbox').is('[readonly]')) {
                    $("#uic_provozni_objem_procenta_textbox_error").show();
                }
            }
            if (Number($("#uic_tezebni_vykon_procenta_textbox").val()) > 100) {
                if (!$('#uic_tezebni_vykon_procenta_textbox').is('[readonly]') || !$('#uic_tezebni_vykon_jednotky_textbox').is('[readonly]')) {
                    $("#uic_tezebni_vykon_procenta_textbox_error").show();
                }
            }
            if (Number($("#uic_vtlaceci_vykon_procenta_textbox").val()) > 100) {
                if (!$('#uic_vtlaceci_vykon_procenta_textbox').is('[readonly]') || !$('#uic_vtlaceci_vykon_jednotky_textbox').is('[readonly]')) {
                    $("#uic_vtlaceci_vykon_procenta_textbox_error").show();
                }
            }
        }

        $.ajax({
            type: "POST",
            url: "/api/run/Aukcnisystem/" + $("#currentBlockName").val() + "/?button=amount_label",
            data: { "AuctionId": registry.Auction },
            success: function (data) {
                var enabledInput, currentValue, currentFraction;
                var periodDD = $('#uic_obdobi_dropdown');
                var periodFromDD = $('#uic_obdobi_od_dropdown');
                for (var i = data.Auction.minimal_offer_period; i <= data.Auction.maximal_period; i++) {
                    periodDD.append('<option value="' + i + '">' + i + '</option>');
                    periodFromDD.append('<option value="' + i + '">' + i + '</option>');
                }

                var periodType = $('#uic_period_type_label').html();
                $('#uic_period_type_label').html(periodType.replace('{var1}', data.Auction.id_period_type == 5 ? 'v měsících' : 'v letech'));

                var injectionPerformance;
                var miningPerformance;
                var operatingVolume;

                if(data.Auction.id_type == 10)
                {
                    injectionPerformance = data.Auction.offered_injection_performance;
                    miningPerformance = data.Auction.mining_performace;
                    operatingVolume = data.Auction.offered_operating_volume;
                }
                else {
                    injectionPerformance = data.Auction.offered_injection_performance * (1 - data.DemandedFraction / 100);
                    miningPerformance = data.Auction.mining_performace * (1 - data.DemandedFraction / 100);
                    operatingVolume = data.Auction.offered_operating_volume * (1 - data.DemandedFraction / 100);
                }

                if (data.Auction.id_item_value_type == 22) {
                    switch (data.Auction.id_item_type)
                    {
                        case 14:
                            enabledInput = $("#uic_provozni_objem_procenta_textbox");
                            enabledInput.on("input keypress change", function () {
                                currentValue = +$(this).val();
                                $("#uic_tezebni_vykon_procenta_textbox, #uic_vtlaceci_vykon_procenta_textbox").val(currentValue);
                                if (data.Auction.offered_operating_volume > 0)
                                    $("#uic_provozni_objem_jednotky_textbox").val((currentValue * operatingVolume) / 100);
                                if (data.Auction.mining_performace > 0)
                                    $("#uic_tezebni_vykon_jednotky_textbox").val((currentValue * miningPerformance) / 100);
                                if (data.Auction.offered_injection_performance > 0)
                                    $("#uic_vtlaceci_vykon_jednotky_textbox").val((currentValue * injectionPerformance) / 100);
                                CheckPercentageValue();
                            });
                            break;
                        case 15:
                            enabledInput = $("#uic_tezebni_vykon_procenta_textbox");
                            enabledInput.on("input keypress change", function () {
                                currentValue = +$(this).val();
                                $("#uic_provozni_objem_procenta_textbox, #uic_vtlaceci_vykon_procenta_textbox").val($(this).val());
                                if (data.Auction.offered_operating_volume > 0)
                                    $("#uic_provozni_objem_jednotky_textbox").val((currentValue * operatingVolume) / 100);
                                if (data.Auction.mining_performace > 0)
                                    $("#uic_tezebni_vykon_jednotky_textbox").val((currentValue * miningPerformance) / 100);
                                if (data.Auction.offered_injection_performance > 0)
                                    $("#uic_vtlaceci_vykon_jednotky_textbox").val((currentValue * injectionPerformance) / 100);
                                CheckPercentageValue();
                            });
                            break;
                        case 16:
                            enabledInput = $("#uic_vtlaceci_vykon_procenta_textbox");
                            enabledInput.on("input keypress change", function () {
                                currentValue = +$(this).val();
                                $("#uic_provozni_objem_procenta_textbox, #uic_tezebni_vykon_procenta_textbox").val($(this).val());
                                if (data.Auction.offered_operating_volume > 0)
                                    $("#uic_provozni_objem_jednotky_textbox").val((currentValue * operatingVolume) / 100);
                                if (data.Auction.mining_performace > 0)
                                    $("#uic_tezebni_vykon_jednotky_textbox").val((currentValue * miningPerformance) / 100);
                                if (data.Auction.offered_injection_performance > 0)
                                    $("#uic_vtlaceci_vykon_jednotky_textbox").val((currentValue * injectionPerformance) / 100);
                                CheckPercentageValue();
                            });
                            break;
                    }
                }
                else {
                    switch (data.Auction.id_item_type) {
                        case 14:
                            enabledInput = $("#uic_provozni_objem_jednotky_textbox");
                            enabledInput.prop({ max: data.Auction.offered_operating_volume });
                            enabledInput.on("input keypress change", function () {
                                currentFraction = +$(this).val() / operatingVolume;
                                if (data.Auction.mining_performace > 0)
                                    $("#uic_tezebni_vykon_jednotky_textbox").val(currentFraction * miningPerformance);
                                if (data.Auction.offered_injection_performance > 0)
                                    $("#uic_vtlaceci_vykon_jednotky_textbox").val(currentFraction * injectionPerformance);
                                $("#uic_provozni_objem_procenta_textbox, #uic_tezebni_vykon_procenta_textbox, #uic_vtlaceci_vykon_procenta_textbox").val(currentFraction * 100);
                                CheckPercentageValue();
                            });
                            break;
                        case 15:
                            enabledInput = $("#uic_tezebni_vykon_jednotky_textbox");
                            enabledInput.prop({ max: data.Auction.mining_performace });
                            enabledInput.on("input keypress change", function () {
                                currentFraction = +$(this).val() / miningPerformance;
                                if (data.Auction.offered_operating_volume > 0)
                                    $("#uic_provozni_objem_jednotky_textbox").val(currentFraction * operatingVolume);
                                if (data.Auction.offered_injection_performance > 0)
                                    $("#uic_vtlaceci_vykon_jednotky_textbox").val(currentFraction * injectionPerformance);
                                $("#uic_provozni_objem_procenta_textbox, #uic_tezebni_vykon_procenta_textbox, #uic_vtlaceci_vykon_procenta_textbox").val(currentFraction * 100);
                                CheckPercentageValue();
                            });
                            break;
                        case 16:
                            enabledInput = $("#uic_vtlaceci_vykon_jednotky_textbox");
                            enabledInput.prop({ max: data.Auction.offered_injection_performance });
                            enabledInput.on("input keypress change", function () {
                                currentFraction = +$(this).val() / injectionPerformance;
                                if (data.Auction.mining_performace > 0)
                                    $("#uic_tezebni_vykon_jednotky_textbox").val(currentFraction * miningPerformance);
                                if (data.Auction.offered_operating_volume > 0)
                                    $("#uic_provozni_objem_jednotky_textbox").val(currentFraction * operatingVolume);
                                $("#uic_provozni_objem_procenta_textbox, #uic_tezebni_vykon_procenta_textbox, #uic_vtlaceci_vykon_procenta_textbox").val(currentFraction * 100);
                                CheckPercentageValue();
                            });
                            break;
                    }
                }
                enabledInput.removeClass("input-read-only").prop("readonly", false);
                mozaicFormValidator.form();
            }
        });
    }
    else if ($("#currentBlockName").val() == "NovaAukce" || $("#currentBlockName").val() == "EditaceAukce") {
        if ($("#uic_druh_aukce_dropdown").val() == 11)
        {
            $("#uic_konecna_cena_textbox").removeClass("input-read-only").prop("readonly", false);
            $("#uic_umoznit_vyssi_poptavku_checkbox input").prop("checked", true).prop("disabled", true);
        }
        else
        {
            $("#uic_konecna_cena_textbox").addClass("input-read-only").prop("readonly", true).val("");
            $("#uic_umoznit_vyssi_poptavku_checkbox input").prop("disabled", false);
        }
            
        $("#uic_druh_aukce_dropdown").on("change", function () {
            if ($(this).val() == 11) {
                $("#uic_konecna_cena_textbox").removeClass("input-read-only").prop("readonly", false);
                $("#uic_umoznit_vyssi_poptavku_checkbox input").prop("checked", true).prop("disabled", true);
            }
            else
            {
                $("#uic_konecna_cena_textbox").addClass("input-read-only").prop("readonly", true).val("");
                $("#uic_umoznit_vyssi_poptavku_checkbox input").prop("disabled", false);
            }                
        });

        if ($("#uic_zpusob_jistoty_dropdown").val() == 25) {
            $("#uic_label11").text("Koeficient jistoty (%)");
        }
        else {
            $("#uic_label11").text("Minimální výše jistoty");
        }

        $("#uic_zpusob_jistoty_dropdown").on("change", function () {
            if ($(this).val() == 25) {
                $("#uic_label11").text("Koeficient jistoty (%)");
            }
            else {
                $("#uic_label11").text("Minimální výše jistoty");
            }
        });

        // Textboxes nabizeny_provozni_objem & nabizeny_vtlaceci_vykon & nabizeny_tezebni_vykon can't equal <= 0 at the same time.
        $("#uic_nabizeny_provozni_objem_textbox, #uic_nabizeny_vtlaceci_vykon_textbox, #uic_nabizeny_tezebni_vykon_textbox").on("change", function () {
            if ($("#uic_nabizeny_provozni_objem_textbox").val() != "" &&
                $("#uic_nabizeny_vtlaceci_vykon_textbox").val() != "" &&
                $("#uic_nabizeny_tezebni_vykon_textbox").val() != "") {
                    if (($("#uic_nabizeny_provozni_objem_textbox").val() > 0 &&
                        $("#uic_nabizeny_vtlaceci_vykon_textbox").val() >= 0 &&
                        $("#uic_nabizeny_tezebni_vykon_textbox").val() >= 0) ||
                        ($("#uic_nabizeny_vtlaceci_vykon_textbox").val() > 0 &&
                        $("#uic_nabizeny_provozni_objem_textbox").val() >= 0 &&
                        $("#uic_nabizeny_tezebni_vykon_textbox").val() >= 0) ||
                        ($("#uic_nabizeny_tezebni_vykon_textbox").val() > 0 &&
                        $("#uic_nabizeny_provozni_objem_textbox").val() >= 0 &&
                        $("#uic_nabizeny_vtlaceci_vykon_textbox").val() >= 0)) {
                            $("#uic_nabizeny_provozni_objem_textbox").removeClass("has-error");
                            $("#uic_nabizeny_vtlaceci_vykon_textbox").removeClass("has-error");
                            $("#uic_nabizeny_tezebni_vykon_textbox").removeClass("has-error");
                    }
            }

            if ($("#uic_nabizeny_provozni_objem_textbox").val() <= 0 &&
                $("#uic_nabizeny_vtlaceci_vykon_textbox").val() <= 0 &&
                $("#uic_nabizeny_tezebni_vykon_textbox").val() <= 0) {
                    $("#uic_nabizeny_provozni_objem_textbox").addClass("has-error");
                    $("#uic_nabizeny_vtlaceci_vykon_textbox").addClass("has-error");
                    $("#uic_nabizeny_tezebni_vykon_textbox").addClass("has-error");
            }         
        });

        $("#uic_nabizeny_provozni_objem_textbox, #uic_nabizeny_vtlaceci_vykon_textbox, #uic_nabizeny_tezebni_vykon_textbox").on("blur", function () {
            if ($("#uic_nabizeny_provozni_objem_textbox").val() != "" &&
                $("#uic_nabizeny_vtlaceci_vykon_textbox").val() != "" &&
                $("#uic_nabizeny_tezebni_vykon_textbox").val() != "") {
                    if (($("#uic_nabizeny_provozni_objem_textbox").val() > 0 &&
                        $("#uic_nabizeny_vtlaceci_vykon_textbox").val() >= 0 &&
                        $("#uic_nabizeny_tezebni_vykon_textbox").val() >= 0) ||
                        ($("#uic_nabizeny_vtlaceci_vykon_textbox").val() > 0 &&
                        $("#uic_nabizeny_provozni_objem_textbox").val() >= 0 &&
                        $("#uic_nabizeny_tezebni_vykon_textbox").val() >= 0) ||
                        ($("#uic_nabizeny_tezebni_vykon_textbox").val() > 0 &&
                        $("#uic_nabizeny_provozni_objem_textbox").val() >= 0 &&
                        $("#uic_nabizeny_vtlaceci_vykon_textbox").val() >= 0)) {
                            $("#uic_nabizeny_provozni_objem_textbox").removeClass("has-error");
                            $("#uic_nabizeny_vtlaceci_vykon_textbox").removeClass("has-error");
                            $("#uic_nabizeny_tezebni_vykon_textbox").removeClass("has-error");
                    }
            }
            
            if ($("#uic_nabizeny_provozni_objem_textbox").val() <= 0 &&
                $("#uic_nabizeny_vtlaceci_vykon_textbox").val() <= 0 &&
                $("#uic_nabizeny_tezebni_vykon_textbox").val() <= 0) {
                    $("#uic_nabizeny_provozni_objem_textbox").addClass("has-error");
                    $("#uic_nabizeny_vtlaceci_vykon_textbox").addClass("has-error");
                    $("#uic_nabizeny_tezebni_vykon_textbox").addClass("has-error");
            }
        });
    }
    else if ($("#currentBlockName").val() == "PrehledPoptavekKola") {
        setTimeout(function () {
            location.reload();
        }, 8000);
    }
    if ($("#uic_auction_countdown").length) {
        if ($("#uic_countdown_label").text() == "Aukce byla pozastavena" || $("#uic_countdown_label").text() == "Aukce již skončila" || $("#uic_countdown_label").text() == "Aukce byla zrušena") {
            $("#uic_auction_countdown").hide();
        }
    }
    if (moment($("#uic_zacatek_skladovaciho_obdobi_datepicker").val(), "D.M.YYYY", "cs") >= moment($("#uic_zacatek_aukce_datepicker").val(), "D.M.YYYY H:mm:ss", "cs") &&
            $("#uic_zacatek_skladovaciho_obdobi_datepicker").val() != "" &&
            $("#uic_zacatek_aukce_datepicker").val() != "") {
        $("#uic_zacatek_skladovaciho_obdobi_datepicker").removeClass("has-error");
    }
    else {
        $("#uic_zacatek_skladovaciho_obdobi_datepicker").addClass("has-error");
    }
    $("#uic_zacatek_skladovaciho_obdobi_datepicker, #uic_zacatek_aukce_datepicker").on("change", function () {
        if (moment($("#uic_zacatek_skladovaciho_obdobi_datepicker").val(), "D.M.YYYY", "cs") >= moment($("#uic_zacatek_aukce_datepicker").val(), "D.M.YYYY H:mm:ss", "cs") &&
            $("#uic_zacatek_skladovaciho_obdobi_datepicker").val() != "" &&
            $("#uic_zacatek_aukce_datepicker").val() != "") {
            $("#uic_zacatek_skladovaciho_obdobi_datepicker").removeClass("has-error");
        }
        else {
            $("#uic_zacatek_skladovaciho_obdobi_datepicker").addClass("has-error");
        }
    });
});

function ShowAppNotification(text, type) {
    type = type.toLowerCase() || "info";
    $("#appNotificationArea .app-alert").remove();
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
            icon = "fa-info-circle";
            break;
    }
    newNotification = $('<div class="app-alert app-alert-' + type + '"><i class="fa ' + icon + ' alertSymbol"></i>'
        + text + '<div class="fa fa-times closeAlertIcon"></div></div>');
    $("#appNotificationArea").append(newNotification);
    newNotification.find(".closeAlertIcon").on("click", function () {
        $(this).parents(".app-alert").remove();
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
            panelHeight = currentUic.position().top + currentUic.height() + 30;
        }
    });
    panel.width(panelWidth);
    panel.height(panelHeight);
}
function GetCurrentUserRoleArray() {
    return JSON.parse($("#currentUserRoleArray").val());
}
function UserHasRole(role) {
    return GetCurrentUserRoleArray().indexOf(role) != -1;
}
function UserHasOneOfTheseRoles(inputArray) {
    return GetCurrentUserRoleArray().some(function (r) {
        return inputArray.indexOf(r) != -1;
    });
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
})()

$(function () {
    pageSpinner.hide();
    $(window).on("beforeunload", function () {
        pageSpinner.show();
    });
});

var ModalDialogArray = [];
var mozaicFormValidator;
$(function () {
    $("#hideMenuIcon").on("click", function () {
        $("#userLeftBar").hide();
        $("#userLeftBarMinimized").show();
        $("#userContentArea").css("left", 60);
        $("#helpContainer").css("left", 60);
    });
    $("#showMenuIcon").on("click", function () {
        $("#userLeftBarMinimized").hide();
        $("#userLeftBar").show();
        $("#userContentArea").css("left", 310);
        $("#helpContainer").css("left", 310);
    });
    if (CurrentModuleIs("appManagerModule")) {
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
    if ($("#asPageContainer").length > 0) {
        $("#appMenu .menu-link").on("click", function () {
            $(".asMenuArea .submenu").hide();
            $("#" + $(this).data("menu-link-for")).show();
        });
        $(".uic").each(function (index, element) {
            $(element).attr("originalId", $(element).attr("Id"));
        });
        $(".uic > checkbox").each(function (index, element) {
            $(element).prop("checked", false);
        });
        $.fn.dataTable.moment("D. M. YYYY H:mm:ss", "cs");
        var currentBlockName = $("#currentBlockName").val();
        try {
            $(".uic.data-table").each(function(index, element) {
                var table = $(element);
                var tableWidth = parseInt(table.attr("uicWidth"));
                var locale = $("html").attr("lang");
                if (locale == null) {
                    locale = "cs";
                }

                if (!table.find('tbody tr').length) {
                    var empty = $('#' + table.attr('id') + '_IsEmpty');
                    if (empty.length) {
                        var message = empty.html();
                        empty.html('<div class="alert alert-info">' + message + '</div>');
                        empty.removeClass('hidden');
                    }
                    table.hide();
                    table.parents('.dataTables_wrapper').eq(0).hide();
                }
                else {
                    CreateDataTable(table, locale, table.hasClass("data-table-simple-mode"));
                    wrapper = table.parents(".dataTables_wrapper");
                    wrapper.css("position", "absolute");
                    wrapper.css("left", table.css("left"));
                    wrapper.css("top", table.css("top"));
                    wrapper.css("width", tableWidth + 5);
                    table.css("width", tableWidth);
                    table.removeClass("uic");
                    wrapper.addClass("uic");
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
                            var modelId = GetUrlParameter("modelId");
                            $('<form class="hiddenForm" method="POST" action="' + window.location.href + '"><input type="hidden" name="modelId" value="' + modelId + '" /><input type="hidden" name="deleteId" value="' + rowId + '" /><input type="hidden" name="button" value="datatable_delete" /></form>').appendTo('body').submit();
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



                    dataTable = table.DataTable();
                    if (!table.hasClass("data-table-simple-mode")) {
                        table.find("tfoot th").each(function () {
                            var title = $(this).text();
                            if (title != "Akce")
                                $(this).html('<input type="text" placeholder="Hledat v &quot;' + title + '&quot;" />');
                            else
                                $(this).html("");
                        });
                        dataTable.columns().eq(0).each(function (colIdx) {
                            $("input", dataTable.column(colIdx).footer()).on("keyup change", function () {
                                dataTable
                                    .column(colIdx)
                                    .search(this.value)
                                    .draw();
                            });
                        });
                    }

                    var tableId = table.attr("id");
                    if (currentBlockName == "HlavniStranka" && tableId == "uic_auction_table")
                        dataTable.order([[4, "asc"]]).draw();
                    if (currentBlockName == "HlavniStranka" && tableId == "uic_history_table")
                        dataTable.order([[4, "desc"]]).draw();
                    if (currentBlockName == "HistorieAukci" && tableId == "uic_auction_table")
                        dataTable.order([[4, "asc"]]).draw();
                    if (currentBlockName == "PrehledAktualnichAukci" && tableId == "uic_auction_table")
                        dataTable.order([[4, "asc"]]).draw();
                    if (currentBlockName == "SeznamOrganizaci" && tableId == "uic_org_table")
                        dataTable.order([[2, "asc"]]).draw();
                    if (currentBlockName == "SpravaOrganizaci" && tableId == "uic_org_table")
                        dataTable.order([[9, "desc"], [2, "asc"]]).draw();
                    if (currentBlockName == "DetailAukce" && tableId == "uic_demands_table")
                        dataTable.order([[2, "asc"]]).draw();
                    if (currentBlockName == "SpravaFinancnichJistotOrganizace" && tableId == "uic_deposit_table") {
                        dataTable.order([[3, "asc"]]).draw();
                    }
                    if (currentBlockName == "SpravaFinancnichJistot" && tableId == "uic_org_table") {
                        dataTable.order([[2, "asc"]]).draw();
                    }
                }
                if (table.parents(".panel-component").length > 0) {
                    RecalculatePanelDimensions(table.parents(".panel-component"));
                    RecalculateMozaicFormHeight();
                }
            });
        }
        catch (err) {
            console.log(err);
        }

        $.extend($.validator.methods, {
            greaterThan: function (value, element, attr) {
                return this.optional(element) || +value > +attr;;
            }
        });
        $.extend($.validator.methods, {
            greaterOrEqual: function (value, element, attr) {
                return this.optional(element) || +value >= +attr;
            }
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
            var countdownTarget = $(element).attr("countdownTarget");
            if (countdownTarget) {
                var newDateObj = moment(countdownTarget, moment.ISO_8601).toDate();
            }
            else {
                var newDateObj = new Date("1970-1-1");
            }
            $(element).countdown({
                until: newDateObj, format: 'HMS',
                onExpiry: function(){
                    setTimeout( function(){
                        location.reload();
                    }, 8000);
                }
            });

            if ($('#uic_auction_countdown').length > 0) {
                if ($("#uic_countdown_label").text() != "Aukce byla pozastavena" && $("#uic_countdown_label").text() != "Aukce již skončila" && $("#uic_countdown_label").text() != "Aukce byla zrušena") {
                    var isNull = true;
                    $('.countdown-amount', '#uic_auction_countdown').each(function () {
                        if ($(this).text() != '0') {
                            isNull = false;
                        }
                    });
                    if (isNull) {
                        setTimeout(function () {
                            location.reload();
                        }, 8000);
                    }
                }
            }
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
        $(".uic.panel-component").each(function (index, element) {
            var panel = $(element);
            if (panel.hasClass("visible-panel-pale-background"));
            var collidingPanels = panel.collision(".uic.visible-panel-pale-background");
            if (collidingPanels.length > 1) {
                collidingPanels.each(function(collidingPanelIndex, collidingPanelElement) {
                    var collidingPanel = $(collidingPanelElement);
                    if (collidingPanel.attr("id") != panel.attr("id")) {
                        if(collidingPanel.position().top > panel.position().top) {
                            collidingPanel.css("top", panel.position().top + panel.height() + 10);
                        }
                        else {
                            panel.css("top", collidingPanel.position().top + collidingPanel.height() + 10);
                        }
                    }
                });
            }
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
        currentBlockName = $("#currentBlockDisplayName").val();
        $("#appMenu li").each(function (index, element) {
            menuLi = $(element);
            if (menuLi.text().trim() == currentBlockName)
                menuLi.addClass("active");
        });
        if (currentBlockName == "Hlavní stránka") {
            $("#appMenu li:first").addClass("active");
        }
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

    $('.name-value-list td').each(function () {
        if ($(this).text() == '__hide__') {
            $(this).parents('tr').eq(0).hide();
        }
    });
});

function InitDataTable(table) {
    table.find("thead th").each(function (index, element) {
        if ($(element).text() == "id" || $(element).text().indexOf("hidden__") == 0) {
            table.find("td:nth-child(" + (index + 1) + "), th:nth-child(" + (index + 1) + ")").hide();
        }
        else if ($(element).text() == "Barva" || $(element).text() == "special__color_code") {
            table.find("td:nth-child(" + (index + 1) + "), th:nth-child(" + (index + 1) + ")").hide();

            table.find("td:nth-child(" + (index + 1) + ")").each(function (tdIndex, tdElement) {
                if (!$(tdElement).parents("tr").find("td:nth-child(" + (index + 2) + ") .colorRectangle").length) {

                    var colorCode = $(tdElement).text();
                    $(tdElement).parents("tr").find("td:nth-child(" + (index + 2) + ")")
                        .prepend('<div class="colorRectangle" style="background-color:' + colorCode + '"></div>');
                }
            });
        }
    });
}