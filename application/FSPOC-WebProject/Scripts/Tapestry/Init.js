var ZoomFactor = 1.0;
var ChangedSinceLastSave = false, dragModeActive = false;
var lastLibId = 1000;
$(function () {
    if (CurrentModuleIs("tapestryModule")) {
        RecalculateToolboxHeight();
        LoadBlock();

        // Buttons and UI effects
        $("#btnClear").on("click", function () {
            $("#resourceRulesPanel .resourceRule").remove();
            $("#workflowRulesPanel .workflowRule").remove();
            ChangedSinceLastSave = true;
        });
        $("#btnSave").on("click", function () {
            saveDialog.dialog("open");
        });
        $("#btnLoad").on("click", function () {
            if (ChangedSinceLastSave)
                confirmed = confirm("Máte neuložené změny, opravdu si přejete tyto změny zahodit?");
            else
                confirmed = true;
            if (confirmed) {
                LoadBlock();
            }
        });
        $("#btnHistory").on("click", function () {
            historyDialog.dialog("open");
        });
        $("#btnOverview").on("click", function () {
            if (ChangedSinceLastSave)
                confirmed = confirm("Máte neuložené změny, opravdu si přejete opustit blok?");
            else
                confirmed = true;
            if (confirmed) {
                ChangedSinceLastSave = false;
                openMetablockForm = $("#openMetablockForm");
                openMetablockForm.find("input[name='metablockId']").val($("#parentMetablockId").val());
                openMetablockForm.submit();
            }
        });
        window.onbeforeunload = function () {
            if (ChangedSinceLastSave)
                return "Máte neuložené změny, opravdu si přejete opustit blok?";
            else
                return;
        };
        $("#btnOpenTableConditions").on("click", function () {
            $("#conditions-dialog").dialog("open");
        });
        $(".toolboxCategoryHeader_Symbols").on("click", function () {
            $(this).toggleClass("hiddenCategory");
            $(".symbolToolboxSpace").slideToggle();
        });
        $(".toolboxCategoryHeader_Actions").on("click", function () {
            if($(".toolboxLi_Actions").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_Actions").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $(".toolboxCategoryHeader_Attributes").on("click", function () {
            if ($(".toolboxLi_Attributes").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_Attributes").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $(".toolboxCategoryHeader_UI").on("click", function () {
            if ($(".toolboxLi_UI").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_UI").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $(".toolboxCategoryHeader_Roles").on("click", function () {
            if ($(".toolboxLi_Roles").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_Roles").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $(".toolboxCategoryHeader_States").on("click", function () {
            if ($(".toolboxLi_States").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_States").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $(".toolboxCategoryHeader_Targets").on("click", function () {
            if ($(".toolboxLi_Targets").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_Targets").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $(".toolboxCategoryHeader_Templates").on("click", function () {
            if ($(".toolboxLi_Templates").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_Templates").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $(".toolboxCategoryHeader_Integrations").on("click", function () {
            if ($(".toolboxLi_Integrations").length > 0) {
                $(this).toggleClass("hiddenCategory");
                $(".toolboxLi_Integrations").slideToggle();
            }
            else
                $(this).removeClass("hiddenCategory");
        });
        $("#blockHeaderBlockName").on("click", function () {
            renameBlockDialog.dialog("open");
        });
        $("#blockHeaderDbResCount").on("click", function () {
            chooseTablesDialog.dialog("open");
        });
        $("#blockHeaderScreenCount").on("click", function () {
            chooseScreensDialog.dialog("open");
        });
        $("#blockHeaderRolesCount").on("click", function () {
            chooseWhitelistRolesDialog.dialog("open");
        });
        $(window).scroll(function () {
            var leftBar = $("#tapestryLeftBar");
            var scrollTop = $(window).scrollTop();
            var lowerPanelTop = $("#lowerPanel").offset().top;
            var overlay = $("#lowerPanelSpinnerOverlay");
            if (scrollTop > lowerPanelTop) {
                leftBar.css({ top: 0, position: "fixed" });
                overlay.css({ top: 0, position: "fixed" });
            } else {
                leftBar.css({ top: 0, position: "absolute" });
                overlay.css({ top: 0, position: "absolute" });
            }
            RecalculateToolboxHeight();
        });
        $(window).resize(function () {
            RecalculateToolboxHeight();
        });
        $(".toolboxItem, .toolboxSymbol").draggable({
            helper: "clone",
            appendTo: '#tapestryWorkspace',
            containment: 'window',
            tolerance: "fit",
            revert: true,
            scroll: true,
            start: function () {
                dragModeActive = true;
            }
        });
        $("#hideTapestryTooboxIcon").on("click", function () {
            $("#tapestryLeftBar").hide();
            $("#tapestryLeftBarMinimized").show();
            $("#tapestryWorkspace").css("left", 32);
            RecalculateToolboxHeight();
        });
        $("#showTapestryTooboxIcon").on("click", function () {
            $("#tapestryLeftBar").show();
            $("#tapestryLeftBarMinimized").hide();
            $("#tapestryWorkspace").css("left", 236);
            RecalculateToolboxHeight();
        });

        // Add rules
        $("#btnAddResRule").on("click", function () {
            ChangedSinceLastSave = true;
            rightmostRuleEdge = 0;
            $("#resourceRulesPanel .resourceRule").each(function (index, element) {
                edge = $(element).position().left + $(element).width() + $("#resourceRulesPanel .scrollContainer").scrollLeft();
                if (edge > rightmostRuleEdge)
                    rightmostRuleEdge = edge;
            });
            newRule = $('<div class="rule resourceRule" style="width: 350px; height: 60px; left: ' + (rightmostRuleEdge + 10) + 'px; top: 10px;"></div>');
            $("#resourceRulesPanel .scrollArea").append(newRule);
            newRule.draggable({
                containment: "parent",
                revert: function (event, ui) {
                    return ($(this).collision("#resourceRulesPanel .resourceRule").length > 1);
                },
                stop: function (event, ui) {
                    ChangedSinceLastSave = true;
                }
            });
            newRule.resizable({
                start: function (event, ui) {
                    rule = $(this);
                    contentsWidth = 120;
                    contentsHeight = 40;
                    rule.find(".item").each(function (index, element) {
                        rightEdge = $(element).position().left + $(element).width();
                        if (rightEdge > contentsWidth)
                            contentsWidth = rightEdge;
                        bottomEdge = $(element).position().top + $(element).height();
                        if (bottomEdge > contentsHeight)
                            contentsHeight = bottomEdge;
                    });
                    rule.css("min-width", contentsWidth + 40);
                    rule.css("min-height", contentsHeight + 20);

                    limits = CheckRuleResizeLimits(rule, true);
                    rule.css("max-width", limits.horizontal - 10);
                    rule.css("max-height", limits.vertical - 10);
                },
                resize: function (event, ui) {
                    rule = $(this);
                    limits = CheckRuleResizeLimits(rule, true);
                    rule.css("max-width", limits.horizontal - 10);
                    rule.css("max-height", limits.vertical - 10);
                },
                stop: function (event, ui) {
                    instance = $(this).data("jsPlumbInstance");
                    instance.recalculateOffsets();
                    instance.repaintEverything();
                    ChangedSinceLastSave = true;
                }
            });
            CreateJsPlumbInstanceForRule(newRule);
            newRule.droppable({
                containment: ".resourceRule",
                tolerance: "touch",
                accept: ".toolboxItem",
                greedy: true,
                drop: function (e, ui) {
                    if (dragModeActive) {
                        dragModeActive = false;
                        droppedElement = ui.helper.clone();
                        droppedElement.removeClass("toolboxItem");
                        droppedElement.addClass("item");
                        $(this).append(droppedElement);
                        ruleContent = $(this);
                        leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left + 20;
                        topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top;
                        droppedElement.offset({ left: droppedElement.offset().left + leftOffset, top: droppedElement.offset().top + topOffset });
                        ui.helper.remove();
                        AddToJsPlumb(droppedElement);
                        if (droppedElement.position().left + droppedElement.width() + 35 > ruleContent.width()) {
                            droppedElement.css("left", ruleContent.width() - droppedElement.width() - 40);
                            instance = ruleContent.data("jsPlumbInstance");
                            instance.repaintEverything();
                        }
                        if (droppedElement.position().top + droppedElement.height() + 5 > ruleContent.height()) {
                            console.log("matched");
                            droppedElement.css("top", ruleContent.height() - droppedElement.height() - 15);
                            instance = ruleContent.data("jsPlumbInstance");
                            instance.repaintEverything();
                        }
                        ChangedSinceLastSave = true;
                    }
                }
            });
        });
        $("#btnAddWfRule").on("click", function () {
            ChangedSinceLastSave = true;
            lowestRuleBottom = 0;
            highestRuleNumber = 0;
            $("#workflowRulesPanel .workflowRule").each(function (index, element) {
                bottom = $(element).position().top + $(element).height() + $("#workflowRulesPanel .scrollContainer").scrollTop();
                if (bottom > lowestRuleBottom)
                    lowestRuleBottom = bottom;
                name = $(element).find(".workflowRuleHeader .verticalLabel").text();
                if (name.startsWith("Pravidlo ") && !isNaN(name.substring(9, name.length))) {
                    ruleNumber = parseInt(name.substring(9, name.length));
                    if (ruleNumber > highestRuleNumber)
                        highestRuleNumber = ruleNumber;
                }
            });
            newRule = $('<div class="rule workflowRule" style="width: 766px; height: 180px; left: 40px; top: ' + (lowestRuleBottom + 20) + 'px;"><div class="workflowRuleHeader"><div class="verticalLabel" style="margin-top: 0px;">Pravidlo ' + (highestRuleNumber + 1) + '</div>'
                + '</div><div class="swimlaneArea"><div class="swimlane" style="height: 100%;"><div class="swimlaneRolesArea"><div class="roleItemContainer"></div><div class="rolePlaceholder"><div class="rolePlaceholderLabel">Pokud chcete specifikovat roli<br />'
                + 'přetáhněte ji do této oblasti</div></div></div><div class="swimlaneContentArea"></div></div>'
                + '</div></div>');
            $("#workflowRulesPanel .scrollArea").append(newRule);
            newRule.draggable({
                containment: "parent",
                handle: ".workflowRuleHeader",
                revert: function (event, ui) {
                    return ($(this).collision("#workflowRulesPanel .workflowRule").length > 1);
                },
                stop: function (event, ui) {
                    ChangedSinceLastSave = true;
                }
            });
            newRule.resizable({
                start: function (event, ui) {
                    rule = $(this);
                    contentsWidth = 120;
                    contentsHeight = 40;
                    rule.find(".item").each(function (index, element) {
                        rightEdge = $(element).position().left + $(element).width();
                        if (rightEdge > contentsWidth)
                            contentsWidth = rightEdge;
                        bottomEdge = $(element).position().top + $(element).height();
                        if (bottomEdge > contentsHeight)
                            contentsHeight = bottomEdge;
                    });
                    rule.css("min-width", contentsWidth + 40);
                    rule.css("min-height", contentsHeight + 20);

                    limits = CheckRuleResizeLimits(rule, false);
                    rule.css("max-width", limits.horizontal - 10);
                    rule.css("max-height", limits.vertical - 10);
                },
                resize: function (event, ui) {
                    rule = $(this);
                    instance = rule.data("jsPlumbInstance");
                    instance.recalculateOffsets();
                    instance.repaintEverything();
                    limits = CheckRuleResizeLimits(rule, false);
                    rule.css("max-width", limits.horizontal - 10);
                    rule.css("max-height", limits.vertical - 10);
                },
                stop: function (event, ui) {
                    ChangedSinceLastSave = true;
                }
            });
            CreateJsPlumbInstanceForRule(newRule);
            newRule.find(".swimlaneRolesArea").droppable({
                containment: ".swimlaneContentArea",
                tolerance: "touch",
                accept: ".toolboxItem.roleItem",
                greedy: true,
                drop: function (e, ui) {
                    if (dragModeActive) {
                        dragModeActive = false;
                        roleExists = false;
                        $(this).find(".roleItem").each(function (index, element) {
                            if ($(element).text() == ui.helper.text())
                                roleExists = true;
                        });
                        if (!roleExists) {
                            droppedElement = ui.helper.clone();
                            $(this).find(".rolePlaceholder").remove();
                            $(this).find(".roleItemContainer").append($('<div class="roleItem">' + droppedElement.text() + '</div>'));
                            ui.helper.remove();
                            ChangedSinceLastSave = true;
                        }
                    }
                }
            });
            newRule.find(".swimlaneContentArea").droppable({
                containment: ".swimlaneContentArea",
                tolerance: "touch",
                accept: ".toolboxSymbol, .toolboxItem",
                greedy: false,
                drop: function (e, ui) {
                    if (dragModeActive) {
                        dragModeActive = false;
                        droppedElement = ui.helper.clone();
                        if (droppedElement.hasClass("roleItem")) {
                            ui.draggable.draggable("option", "revert", true);
                            return false;
                        }
                        ruleContent = $(this);
                        ruleContent.append(droppedElement);
                        if (droppedElement.hasClass("toolboxSymbol")) {
                            droppedElement.removeClass("toolboxSymbol ui-draggable ui-draggable-dragging");
                            droppedElement.addClass("symbol");
                            leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left;
                            topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top;
                        }
                        else {
                            droppedElement.removeClass("toolboxItem");
                            droppedElement.addClass("item");
                            leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left + 38;
                            topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top - 18;
                        }
                        droppedElement.offset({ left: droppedElement.offset().left + leftOffset, top: droppedElement.offset().top + topOffset });
                        ui.helper.remove();
                        AddToJsPlumb(droppedElement);
                        if (droppedElement.position().top + droppedElement.height() + 10 > ruleContent.height()) {
                            console.log("matched");
                            droppedElement.css("top", ruleContent.height() - droppedElement.height() - 20);
                            instance = ruleContent.parents(".workflowRule").data("jsPlumbInstance");
                            instance.repaintEverything();
                        }
                        ChangedSinceLastSave = true;
                    }
                }
            });
        });
        $(document).on("click", ".libraryItem", function () {
            currentLibraryItem = $(this);
            libId = currentLibraryItem.attr("libId");
            libType = currentLibraryItem.attr("libType");
            if (libId) {
                if (currentLibraryItem.hasClass("highlighted")) {
                    $('.tapestryToolbox .toolboxLi[libId="' + libId + '"]').remove();
                }
                else {
                    newToolboxLi = null;
                    if (libType == "action") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Actions"><div class="toolboxItem actionItem" actionId="' + currentLibraryItem.attr("actionId") + '"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_Attributes").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Actions").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "column-attribute") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Attributes"><div class="toolboxItem attributeItem tableAttribute" tableName="' + currentLibraryItem.attr("tableName") + '" columnName="' + currentLibraryItem.attr("columnName") + '"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_UI").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Attributes").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "table-attribute") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Attributes"><div class="toolboxItem attributeItem tableAttribute" tableName="' + currentLibraryItem.attr("tableName") + '"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_UI").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Attributes").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "view-attribute") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Attributes"><div class="toolboxItem attributeItem viewAttribute" tableName="' + currentLibraryItem.attr("tableName") + '"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_UI").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Attributes").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "ui") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_UI"><div class="toolboxItem uiItem" pageId="' + currentLibraryItem.attr("pageId") + '" componentName="' + currentLibraryItem.attr("componentName") + '"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_Roles").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_UI").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "page-ui") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_UI"><div class="toolboxItem uiItem pageUi" pageId="' + currentLibraryItem.attr("pageId") + '"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_Roles").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_UI").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "role") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Roles"><div class="toolboxItem roleItem"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_States").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Roles").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "state") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_States"><div class="toolboxItem stateItem" stateId="' + currentLibraryItem.attr("stateId") + '"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_Targets").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_States").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "target") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Targets"><div class="toolboxItem targetItem" targetId="' + currentLibraryItem.attr("targetId") + '"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>');
                        $(".tapestryToolbox .toolboxCategoryHeader_Templates").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Targets").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (libType == "template") {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Templates"><div class="toolboxItem templateItem"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>')
                        $(".tapestryToolbox .toolboxCategoryHeader_Integrations").before(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Templates").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    else if (["ldap", "ws", "smtp", "webdav"].indexOf(libType) != -1) {
                        newToolboxLi = $('<li libId="' + libId + '" class="toolboxLi toolboxLi_Integrations"><div class="toolboxItem integrationItem"><span class="itemLabel">'
                            + currentLibraryItem.text() + '</span></div></li>')
                        $(".tapestryToolbox").append(newToolboxLi);
                        if ($(".tapestryToolbox .toolboxCategoryHeader_Integrations").hasClass("hiddenCategory"))
                            newToolboxLi.hide();
                    }
                    if (newToolboxLi)
                        newToolboxLi.find(".toolboxItem").draggable({
                            helper: "clone",
                            appendTo: '#tapestryWorkspace',
                            containment: 'window',
                            tolerance: "fit",
                            revert: true,
                            scroll: true,
                            start: function () {
                                dragModeActive = true;
                            }
                        });
                }
            }
            $(this).toggleClass("highlighted");
        });
    }
});
