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
                console.warn("Iterating over tables");
                var visibleRowList = "";
                var dataTable = $(tableElement).DataTable();
                dataTable.rows({ search: 'applied' }).nodes().each(function (row, index) {
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
