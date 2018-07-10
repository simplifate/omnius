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
        self.EventCalendar.init();
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
                var columns = [];
                table.find('tr:eq(0) th').each(function () {
                    columns.push({ data: $(this).text() });
                });

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
                    processing: table.data('dtserverside') == '1',
                    serverSide: table.data('dtserverside') == '1',
                    ajax: table.data('dtserverside') == '1' ? { url: "/api/run" + location.pathname + '?button=' + table.attr('id'), type: 'POST' } : null,
                    columns: columns,
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

    /******************************************************/
    /* Event calendar                                     */
    /******************************************************/
    EventCalendar: {

        DEFAULT_HEADER_COMPONENTS: {
            DatePickerIcon: "<span class='cContHeaderButton cContHeaderDatePickerIcon clickableLink cs-icon-Calendar'></span>",
            PreviousButton: "<span class='cContHeaderButton cContHeaderNavButton cContHeaderPrevButton clickableLink cs-icon-Prev'></span>",
            NextButton: "<span class='cContHeaderButton cContHeaderNavButton cContHeaderNextButton clickableLink cs-icon-Next'></span>",
            TodayButton: "<span class='cContHeaderButton cContHeaderToday clickableLink'></span>",
            HeaderLabel: "<span class='cContHeaderLabelOuter'><span class='cContHeaderLabel'></span></span>",
            HeaderLabelWithDropdownMenuArrow: "<span class='cContHeaderLabelOuter clickableLink'><span class='cContHeaderLabel'></span><span class='cContHeaderButton cContHeaderDropdownMenuArrow'></span></span>",
            MenuSegmentedTab: "<span class='cContHeaderMenuSegmentedTab'></span>",
            MenuDropdownIcon: "<span class='cContHeaderButton cContHeaderMenuButton clickableLink'>&#9776;</span>",
            FullscreenButton: function (isFullscreen) {
                var sIconClass = (isFullscreen) ? "cs-icon-Contract" : "cs-icon-Expand";
                return "<span class='cContHeaderButton cContHeaderFullscreen clickableLink " + sIconClass + "'></span>";
            }
        },

        DEFAULT_DURATION_STRINGS: {
            y: ["year ", "years "],
            M: ["month ", "months "],
            w: ["w ", "w "],
            d: ["d ", "d "],
            h: ["h ", "h "],
            m: ["m ", "m "],
            s: ["s ", "s "]
        },

        init: function () {
            $('.event-calendar').each(function () {
                var elm = $(this);
                var cal = elm.CalenStyle(window[this.id + "Options"]);
                
                setTimeout(function () {
                    BootstrapUserInit.EventCalendar.adjustList.apply(cal);
                }, 0);
                $(window).resize(function () {
                    BootstrapUserInit.EventCalendar.adjustList.apply(cal);
                });
            });
        },

        renderFilters: function (filterBarElement, eventFilterCriteria, eventFilterCount)
        {
            var self = this,
                tempFC = JSON.stringify(eventFilterCriteria),
                tempFCJ = $.parseJSON(tempFC),
                hasCount = (eventFilterCount != null) ? ((eventFilterCount.length > 0) ? true : false) : false;

            var col = $('<div class="col-xs-12 event-calendar-filters"></div>');
            col.append('<h4>Filters</h4>');

            $(filterBarElement).html('').append(col);

            for (var i = 0; i < eventFilterCriteria.length; i++) {
                var filter = eventFilterCriteria[i],
                    keyName = filter['keyName'],
                    displayName = filter['keyDisplayName'] || keyName,
                    dataType = filter['dataType'],
                    values = filter['values'],
                    selectedValues = filter['selectedValues'];

                var filterCountList;
                for (var j = 0; j < eventFilterCount.length; j++) {
                    var filterCount = eventFilterCount[j];
                    if (filterCount['keyName'] == keyName) {
                        filterCountList = filterCount;
                        break;
                    }
                }

                var panel = $('<div class="panel panel-default"></div>');
                var header = $('<div class="panel-heading" data-filterkey="' + keyName + '">' + displayName + '</div>');
                var body = $('<div class="panel-body"></div>');
                
                for (var j = 0; j < values.length; j++) {
                    var value = values[j];
                    var valueCount = hasCount ? filterCountList[value] : 0;

                    var isChecked = "";
                    for (var k = 0; k < selectedValues.length; k++) {
                        if (value == selectedValues[k]) {
                            isChecked = "checked";
                            break;
                        }
                    }

                    var div = $('<div class="checkbox"></div>');
                    var label = $('<label></label>');
                    var chb = $('<input type="checkbox" value="' + value + '" ' + isChecked + '>');

                    label.append(chb).append(value);
                    div.append(label);

                    if (hasCount) {
                        label.append(' (' + valueCount + ')');
                    }
                    body.append(div);
                }
                panel.append(header);
                panel.append(body);

                col.append(panel);
            }

            $('input[type="checkbox"]', col).change(function () {
                var $this = $(this),
                    isChecked = $this.is(':checked'),
                    value = $this.val(),
                    $parent = $this.closest('.panel-body'),
                    childCheckboxList = $parent.find("input[type='checkbox']"),
                    keyName = $parent.prev().data('filterkey');

                for (var i = 0; i < eventFilterCriteria.length; i++) {
                    var filter = eventFilterCriteria[i];
                    if ($.cf.compareStrings(filter["keyName"], keyName)) {
                        var selectedList = new Array();
                        for (var j = 0; j < childCheckboxList.length; j++) {
                            var $checkbox = childCheckboxList.eq(j);
                            if ($checkbox.is(':checked'))
                                selectedList.push($checkbox.val());
                        }

                        eventFilterCriteria[i]["selectedValues"] = selectedList;
                        self.applyFilter(eventFilterCriteria, []);
                        break;
                    }
                }
            });
        },

        getSlotTooltipContent: function(slotAvailability) {
            if (slotAvailability.status === "Busy")
                return "";
            else if (slotAvailability.status === "Free") {
                if (slotAvailability.count === undefined || slotAvailability.count === null)
                    return "<div class=cavTooltipBookNow>Book Now</div>";
                else
                    return "<div class=cavTooltipSlotCount>" + slotAvailability.count + " slots available</div><div class=cavTooltipBookNow>Book Now</div>";
            }
        },

        defaultEventDetail: function (visibleView, selector, event) {
            var calendar = this;
            console.log(calendar);
            $(calendar.elem).find(selector).popover({
                placement: 'top',
                trigger: 'manual',
                html: true,
                container: 'body',
                content: function () {
                    var tooltipContentData = $(this).data('tooltipcontent'),
                        title = tooltipContentData.title || '',
                        startTime = tooltipContentData.startDateTime || '',
                        endTime = tooltipContentData.endDateTime || '',
                        description = event.desc || '',
                        url = event.url || '',
                        time = startTime + ((endTime !== '') ? (' - ' + endTime) : "");

                    var body = $('<div></div>');
                    body.append('<div class="cTooltipTitle">' + title + '</div>');
                    body.append('<p><time class="cTooltipTime">' + time + '</time></p>');
                    if (description) {
                        body.append('<div class="cTooltipDescription">' + description + '</div>');
                    }
                    if (url) {
                        body.append('<hr />');
                        body.append('<div class="text-center"><a href="' + url + '" class="btn btn-default text-white">Detail</a></div>');
                    }
                    return body[0];
                }
            });

            $(calendar.elem).find(selector).popover('show');

            $(document).one('click', function () {
                $('.popover').remove();
            });
        },

        adjustList: function () {
            $('.elem-CalenStyle').each(function () {
                var eventWidth = $('.cListOuterCont', this).width(),
                    eventColorWidth = $('.cListEventColor', this).outerWidth(true),
                    eventIconWidth = $('.cListEventIcon span', this).outerWidth(true),
                    timeItems = $('.cListEventTime span', this),
                    actionItems = $('.cListEventActions a', this);
                console.log(eventWidth);
                var timeMaxWidth = timeItems.length ? Math.max.apply(null, timeItems.map(function () {
                    return Math.ceil($(this).outerWidth(true));
                }).get()) : 0;
                timeMaxWidth += 10;

                var actionMaxWidth = actionItems.length ? Math.max.apply(null, actionItems.map(function () {
                    return Math.ceil($(this).outerWidth(true));
                }).get()) : 0;
                actionMaxWidth += 10;

                $('.cListEventTime', this).css({ 'width': timeMaxWidth });
                $('.cListEventActions', this).css({ 'width': actionMaxWidth });

                console.log(eventColorWidth, eventIconWidth, timeMaxWidth, actionMaxWidth);

                var eventTitleWidth = eventWidth - (eventColorWidth + eventIconWidth + timeMaxWidth + actionMaxWidth) - 22;
                $('.cListEventTitle').css({ 'width': eventTitleWidth });
            });
        },

        displayEventsForPeriodInList: function (listStartDate, listEndDate) {
            var eventId = 0, eventCount = 0;
            var viewDetails = new Array();

            if (this.compareDates(listStartDate, listEndDate) == 0) {
                var viewDetail = new Object();
                viewDetail.date = new Date(listStartDate);
                var tempVSDate = this.setDateInFormat({ "date": listStartDate }, "START"),
                    tempVEDate = this.setDateInFormat({ "date": listStartDate }, "END");
                viewDetail.events = this.getArrayOfEventsForView(tempVSDate, tempVEDate);
                viewDetails.push(viewDetail);
                eventCount += viewDetail.events.length;
            }
            else {
                var tempDate = new Date(listStartDate);
                while (this.compareDates(tempDate, listEndDate) != 0) {
                    var viewDetail = new Object();
                    viewDetail.date = new Date(tempDate);
                    var tempVSDate = this.setDateInFormat({ "date": tempDate }, "START"),
                        tempVEDate = this.setDateInFormat({ "date": tempDate }, "END");
                    viewDetail.events = this.getArrayOfEventsForView(tempVSDate, tempVEDate);
                    viewDetails.push(viewDetail);
                    eventCount += viewDetail.events.length;

                    tempDate.setDate(tempDate.getDate() + 1);
                }
            }

            var table = $('<table class="table table-condensed table-striped"></table>');
            var body = $('<tbody></tbody>');
            
            if (eventCount > 0) {
                for (var i = 0; i < viewDetails.length; i++) {
                    var viewDetail = viewDetails[i],
                        viewDate = viewDetail.date,
                        eventList = viewDetail.events,
                        colspan = this.setting.hideEventIcon[this.setting.visibleView] ? 3 : 4;

                    if (viewDetails.length > 1) {
                        var fullDate = this.getDateInFormat({ "date": viewDate }, "DDD MMM dd, yyyy", false, true);
                        var dateId = "Date-" + fullDate;
                        body.append("<tr><th colspan='" + colspan + "'><div id='" + dateId + "' class='cListDate'>" + fullDate + "</div></th></tr>");
                    }

                    if (eventList.length > 0) {
                        for (var j = 0; j < eventList.length; j++) {
                            var event = eventList[j],
                                startDateTime = null, endDateTime = null,
                                isAllDay = 0, title = "", url = "", eventColor = "", desc = "",
                                isMarked = false;

                            if (event.start != null) startDateTime = event.start;
                            if (event.end != null) endDateTime = event.end;
                            if (event.isAllDay != null) isAllDay = event.isAllDay;
                            if (event.title != null) title = event.title;
                            if (event.desc !== null) desc = event.desc;
                            if (event.url != null) url = event.url;
                            if (event.isMarked !== null) isMarked = event.isMarked;
                            if (isMarked) isAllDay = true;

                            var sArrEventDateTime = this.getEventDateTimeDataForAgendaView(startDateTime, endDateTime, isAllDay, viewDate, "cListEventTime"),
                                sEventDateTime = sArrEventDateTime[0];
                            if (sEventDateTime === "")
                                sEventDateTime = "All Day";

                            eventColor = event.fromSingleColor ? event.textColor : event.backgroundColor;
                            eventColor = ($.cf.compareStrings(eventColor, "") || $.cf.compareStrings(eventColor, "transparent")) ? "transparent" : eventColor;
                            var idAttr = "Event" + (++eventId),
                                sStyleColorHeight = sArrEventDateTime[1],
                                icon, iconStyle,
                                eventClass = "cListEvent",
                                fontIconClass;

                            if (isMarked) {
                                eventClass += " cMarkedDayEvent";
                                icon = ($.cf.isValid(event.icon) && event.icon !== "Dot") ? event.icon : "cs-icon-Mark";
                                iconStyle = "background: " + eventColor + ";";
                                fontIconClass = "cListEventIconFont " + icon;
                            }
                            else {
                                icon = $.cf.isValid(event.icon) ? event.icon : this.setting.eventIcon;
                                iconStyle = "background: " + eventColor + "; ";
                                fontIconClass = $.cf.compareStrings(icon, "Dot") ? "cListEventIconDot" : "cListEventIconFont " + icon;
                            }

                            if ($.cf.compareStrings(this.setting.visibleView, "DayEventListView") && $.cf.isValid(event.status)) {
                                fontIconClass += $.cf.compareStrings(icon, "Dot") ? " cListEventIconDotStatus" : " cListEventIconFontStatus";
                                iconStyle += "border-color: " + event.statusColor + ";";
                            }


                            var row = $('<tr></tr>');
                            row.attr('id', idAttr).addClass(eventClass);

                            row.append('<td class="cListEventColor"><span style="background:' + eventColor + '; height:' + sStyleColorHeight + ';"></span></td>');
                            row.append('<td class="cListEventTime">' + sEventDateTime + '</td>');

                            if (isMarked) {
                                row.append('<td class="cListEventIcon"><span class="' + fontIconClass + '" style="' + iconStyle + '"></span></td>');
                            }
                            else {
                                if (!this.setting.hideEventIcon[this.setting.visibleView]) {
                                    if ($.cf.compareStrings(icon, "Dot"))
                                        row.append('<td class="cListEventIcon"><span class="' + fontIconClass + '" style="' + iconStyle + '"></span></td>');
                                    else {
                                        fontIconClass = "cListEventIconFont " + ($.cf.compareStrings(this.setting.visibleView, "DayEventListView") && $.cf.isValid(event.status) ? "cListEventIconFontStatus " : "") + icon;
                                        row.append('<td class="cListEventIcon"><span class="' + fontIconClass + '" style="' + iconStyle + '"></span></td>');
                                    }
                                }
                            }

                            var cell = $('<td class="cListEventContent"></td>');
                            cell.append('<div class="cListEventTitle">' + title + '</div>');
                            cell.append('<div class="cListEventDesc">' + desc + '</div>');

                            cell.appendTo(row);

                            var action = $('<td class="cListEventActions"></td>');
                            if (url) {
                                action.append('<a href="' + url + '" title="Detail" class="btn btn-default">Detail</a>');
                            }
                            action.appendTo(row);

                            row.appendTo(body);
                        }
                    }
                    else {
                        body.append('<tr><td colspan="' + colspan + '" class="cEmptyList">Žádné události</td></tr>');
                    }
                }
            }
            else {
                body.append('<tr><td>Žádné události</td></tr>');
            }
            body.appendTo(table);

            return table[0];
        }
    },

    /******************************************************/
    /* Validators                                         */
    /******************************************************/

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
        });
        $(".input-with-timepicker").datetimepicker({
            datepicker: false,
            timepicker: true,
            step: 5,
            format: "H:i:00"
        });
        $(".input-with-datetimepicker").datetimepicker({
            datepicker: true,
            timepicker: true,
            step: 5,
            format: "d.m.Y H:i:00"
        });

        $("input").on('wheel', function(e){ e.preventDefault(e) });
        $("input").off('mousewheel');
    }
});