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
            panelHeight = currentUic.position().top + currentUic.height() + 30;
        }
    });
    panel.width(panelWidth);
    panel.height(panelHeight);
}
$(function () {
    $(document, "[data-ajax='true']").on("click", function () {
        pageSpinner.show();
        $(document).one("ajaxComplete", function () {
            pageSpinner.hide();
        });
    });
});
