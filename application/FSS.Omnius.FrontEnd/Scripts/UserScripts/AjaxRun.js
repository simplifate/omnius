function AjaxRunAndReplace(url, uic_name, modelId)
{
     $.ajax({
        type: "POST",
        url: "/api/run" + url + '?button=' + uic_name,
        data: { 'modelId': modelId },
        error: console.error.bind(console),
        success: function (data) {
            $.each(data, function (name, value) {
                if ($('select#uic_' + name).size() > 0) {
                    var html = '';
                    $.each(value, function (i, item) {
                        html += '<option value="' + item['id'] + '">' + item['Name'] + '</option>';
                    });

                    $('select#uic_' + name).html(html);
                }
                else if ($('input#uic_' + name).size() > 0) {
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
    if ($("#currentBlockName").val() == "Home") {
        function refresh() {
            $.ajax({
                dataType: "html",
                method: "POST",
                url: window.location.protocol + '//' + window.location.hostname + ':' + window.location.port + '/api/run/Grid/Home?button=refresh', //kvůli ignorování selectedProfile v url při autorefreshi
                success: function (result) {
                    var data = JSON.parse(result).Data;
                    // Miner status
                    var total = data.MinerStatus[0].y + data.MinerStatus[1].y + data.MinerStatus[2].y;
                    var totalHash = Math.round((data.MinerStatus[0].x + data.MinerStatus[1].x) * 100) / 100;
                    var totalHash_XMR = Math.round((data.MinerStatus[0].x2 + data.MinerStatus[1].x2) * 100) / 100;

                    rigstatusdonut_graph_chart.series[0].setData(data.MinerStatus, true);
                    $('#numberOfWarnings').html('You have ' + data.MinerStatus[1].y + ' warnings to explore.');
                    $('#numberOfRigs').html('Rigs in total ' + total);
                    $('#sys_hash').html(totalHash + ' GH/s');
                    $('#sys_hash_xmr').html(totalHash_XMR + ' kH/s');

                    $('#online_percent').html(Math.round((data.MinerStatus[0].y / total) * 10000) / 100 + '%');
                    $('#online_rigs').html(data.MinerStatus[0].y + ' rigs');
                    $('#online_ghs').html(data.MinerStatus[0].x + ' GH/s');
                    $('#online_ghs_xmr').html(data.MinerStatus[0].x2 + ' kH/s');

                    $('#warning_percent').html(Math.round((data.MinerStatus[1].y / total) * 10000) / 100 + '%');
                    $('#warning_rigs').html(data.MinerStatus[1].y + ' rigs');
                    $('#warning_ghs').html(data.MinerStatus[1].x + ' GH/s');
                    $('#warning_ghs_xmr').html(data.MinerStatus[1].x2 + ' kH/s');

                    $('#offline_percent').html(Math.round((data.MinerStatus[2].y / total) * 10000) / 100 + '%');
                    $('#offline_rigs').html(data.MinerStatus[2].y + ' rigs');
                    //$('#offline_ghs').html(data.MinerStatus[2].x + ' MH/s');

                    var warningModal = $('#modalWarnings div.modal-dialog div.modal-content div.modal-body ul');
                    warningModal.html('');
                    $.each(data.RigsWarningMessages, function (index, item) {
                        warningModal.append('<li><span>' + item + '</span></li>');
                    });

                    // Rig Status history
                    var id = $('body').data('LastId-RigStatusHistory');
                    if (typeof id === 'undefined' || id != data.RigStatusHistory.id) {
                        $('body').data('LastId-RigStatusHistory', data.RigStatusHistory.id);

                        testrigstatus_graph_chart.series[0].addPoint([data.RigStatusHistory.id, data.RigStatusHistory.value[0]], true, true);
                        testrigstatus_graph_chart.series[1].addPoint([data.RigStatusHistory.id, data.RigStatusHistory.value[1]], true, true);
                    }

                    // Exp vs Real
                    id = $('body').data('LastId-ExpVsReal');
                    if (typeof id === 'undefined' || id != data.ExpVsReal.id) {
                        $('body').data('LastId-ExpVsReal', data.ExpVsReal.id);

                        hc_actual_yield_graph_chart.series[0].addPoint([data.ExpVsReal.id, data.ExpVsReal.value[0]], true, true);
                        hc_actual_yield_graph_chart.series[2].addPoint([data.ExpVsReal.id, data.ExpVsReal.value[1]], true, true);
                    }

                    // consum
                    hc_rigs_consumption_graph_gauge.series[0].setData([data.Consum.rigs], true);
                    hc_aircondition_consumption_graph_gauge.series[0].setData([data.Consum.air], true);
                    id = $('body').data('LastId-consum');
                    if (typeof id === 'undefined' || id != data.Consum.history.id) {
                        $('body').data('LastId-consum', data.Consum.history.id);

                        hc_modbus_history_graph_chart.series[0].addPoint([data.Consum.history.id, data.Consum.history.value[0]], true, true);
                        hc_modbus_history_graph_chart.series[1].addPoint([data.Consum.history.id, data.Consum.history.value[1]], true, true);
                    }

                    // earnings
                    $('#earnings_amount').html(data.Earnings.day[0]);
                    $('#yield_amount').html(data.Earnings.day[1]);
                    $('#cost_amount').html(data.Earnings.day[2]);
                    $('#elec_amount').html(data.Earnings.day[3]);

                    $('#earnings_amount_weekly').html(data.Earnings.week[0]);
                    $('#yield_amount_weekly').html(data.Earnings.week[1]);
                    $('#cost_amount_weekly').html(data.Earnings.week[2]);
                    $('#elec_amount_weekly').html(data.Earnings.week[3]);

                    $('#earnings_amount_monthly').html(data.Earnings.month[0]);
                    $('#yield_amount_monthly').html(data.Earnings.month[1]);
                    $('#cost_amount_monthly').html(data.Earnings.month[2]);
                    $('#elec_amount_monthly').html(data.Earnings.month[3]);

                    //// last transaction
                    transactionLine = function (item) {
                        return '<tr><td style="display:none">' + item.hidden_id + '</td><td style="display:none">' + item.hidden_add + '</td><td style="display:none">' + item.hidden_date + '</td><td>' + item.currency + '</td><td>' + item.amount + '</td><td>' + item.wallet + '</td><td>' + item.date + '</td><td>' + item.time + '</td></tr>';
                    };
                    var ltTable = $('#transaction_history_table tbody');
                    ltTable.html('');
                    $.each(data.LastTransaction, function (index, item) {
                        ltTable.append(transactionLine(item));
                    });

                    //modalHistory = $('#transaction_history_table_more tbody');
                    //lastModalAdd = $('tr:first td:nth-child(2)', modalHistory).html();
                    //itemFound = false;
                    //$.each(data.LastTransaction.reverse(), function (index, item) {
                    //    if (itemFound) {
                    //        modalHistory.prepend(transactionLine(item));
                    //    }
                    //    else {
                    //        if (item.hidden_add == lastModalAdd) { // chybná detekce!!!
                    //            itemFound = true;
                    //        }
                    //    }
                    //});
                    //// if !itemFound -> list needs to refresh

                    // Total income
                    id = $('body').data('LastId-totalIncome');
                    if (typeof id === 'undefined' || id != data.TotalIncome.id) {
                        $('body').data('LastId-totalIncome', data.TotalIncome.id);

                        totalbalancehistory_graph_chart.series[0].addPoint([data.TotalIncome.id, data.TotalIncome.value], true, true);
                    }

                    //// exchange rate
                    $('#bitcoin_value').html('$' + data.ExchangeRates.btc.value);
                    $('#bitcoin_change').html('$' + data.ExchangeRates.btc.change);
                    $('#bitcoin_change_percent').html('&nbsp; (' + data.ExchangeRates.btc.percent + '%)');
                    if (data.ExchangeRates.btc.change < 0)
                        $('#bitcoin_change').css('color', 'red');
                    else
                        $('#bitcoin_change').css('color', 'green');

                    $('#ethereum_value').html('$' + data.ExchangeRates.eth.value);
                    $('#ethereum_change').html('$' + data.ExchangeRates.eth.change);
                    $('#ethereum_change_percent').html('&nbsp; (' + data.ExchangeRates.eth.percent + '%)');
                    if (data.ExchangeRates.eth.change < 0)
                        $('#ethereum_change').css('color', 'red');
                    else
                        $('#ethereum_change').css('color', 'green');

                    $('#zcash_value').html('$' + data.ExchangeRates.zec.value);
                    $('#zcash_change').html('$' + data.ExchangeRates.zec.change);
                    $('#zcash_change_percent').html('&nbsp; (' + data.ExchangeRates.zec.percent + '%)');
                    if (data.ExchangeRates.zec.change < 0)
                        $('#zcash_change').css('color', 'red');
                    else
                        $('#zcash_change').css('color', 'green');

                    $('#dcr_value').html('$' + data.ExchangeRates.dcr.value);
                    $('#dcr_change').html('$' + data.ExchangeRates.dcr.change);
                    $('#dcr_change_percent').html('&nbsp; (' + data.ExchangeRates.dcr.percent + '%)');
                    if (data.ExchangeRates.dcr.change < 0)
                        $('#dcr_change').css('color', 'red');
                    else
                        $('#dcr_change').css('color', 'green');

                    $('#ltc_value').html('$' + data.ExchangeRates.ltc.value);
                    $('#ltc_change').html('$' + data.ExchangeRates.ltc.change);
                    $('#ltc_change_percent').html('&nbsp; (' + data.ExchangeRates.ltc.percent + '%)');
                    if (data.ExchangeRates.ltc.change < 0)
                        $('#ltc_change').css('color', 'red');
                    else
                        $('#ltc_change').css('color', 'green');

                    $('#xmr_value').html('$' + data.ExchangeRates.xmr.value);
                    $('#xmr_change').html('$' + data.ExchangeRates.xmr.change);
                    $('#xmr_change_percent').html('&nbsp; (' + data.ExchangeRates.xmr.percent + '%)');
                    if (data.ExchangeRates.xmr.change < 0)
                        $('#xmr_change').css('color', 'red');
                    else
                        $('#xmr_change').css('color', 'green');

                    $('#music_value').html('$' + data.ExchangeRates.music.value);
                    $('#music_change').html('$' + data.ExchangeRates.music.change);
                    $('#music_change_percent').html('&nbsp; (' + data.ExchangeRates.music.percent + '%)');
                    if (data.ExchangeRates.music.change < 0)
                        $('#music_change').css('color', 'red');
                    else
                        $('#music_change').css('color', 'green');
                }
            });
        }
        setInterval(refresh, 20000);


        //panel state saving
        $("body").on("click", "div.panel-heading", function () {
            //get the id of clicked 

            var panelId = $(this).parent().find(".panel-body").attr("id");
            var panelCollapsed = false;
            if($(this).hasClass('collapsed')){
                panelCollapsed = true;
            }
            //on each panel header click we show or hide the panel
            //and then call AJAX to the server to save state of panels

            //AJAX
            $.ajax({
                type: "POST",
                url: "/api/run/" + $("#currentAppName").val() + "/" + $("#currentBlockName").val() + "/?button=total_yield",
                data: {"PanelName" : panelId,"TargetState": panelCollapsed},
                error: console.error.bind(console),
                complete: function () {
                },
                success: function (data) {
                    
                }
            });
            //End AJAX
        });


        //end 
        if ($('#ethereum_change').text()[1] == "-") {
            $('#ethereum_change').css('color', 'red');
        } else {
            $('#ethereum_change').css('color', 'green');

        }
        if ($('#bitcoin_change').text()[1] == "-") {
            $('#bitcoin_change').css('color', 'red');
        } else {
            $('#bitcoin_change').css('color', 'green');

        }
        if ($('#zcash_change').text()[1] == "-") {
            $('#zcash_change').css('color', 'red');
        } else {
            $('#zcash_change').css('color', 'green');

        }
        if ($('#dcr_change').text()[1] == "-") {
            $('#dcr_change').css('color', 'red');
        } else {
            $('#dcr_change').css('color', 'green');

        }
        if ($('#ltc_change').text()[1] == "-") {
            $('#ltc_change').css('color', 'red');
        } else {
            $('#ltc_change').css('color', 'green');

        }
        if ($('#xmr_change').text()[1] == "-") {
            $('#xmr_change').css('color', 'red');
        } else {
            $('#xmr_change').css('color', 'green');

        }
        if ($('#music_change').text()[1] == "-") {
            $('#music_change').css('color', 'red');
        } else {
            $('#music_change').css('color', 'green');

        }
    }

    if ($("#currentBlockName").val() == "ServiceStatus") {
        //Reload panels
        function refresh() {
            $.ajax({
                dataType: "html",
                url: window.location.href, //kvůli ignorování selectedProfile v url při autorefreshi
                success: function (response) {
                    var x = $(response)
                    //$("#bodyMiningHistory").html(x.find("#bodyMiningHistory").html());
                    $("#performance_div").html(x.find("#performance_div").html());
                    $("#divCam").html(x.find("#divCam").html());
                    $("#containerConsump").html(x.find("#containerConsump").html());
                    $("#containerWarnings").html(x.find("#containerWarnings").html());
                    //$("#divRigPlace").html(x.find("#divRigPlace").html());
                    //$("#divRigEdit").html(x.find("#divRigEdit").html()); 
                    setTimeout(
                        function () {
                            refresh()
                        }, 10000);
                }
            });
        }

        refresh();
        //setInterval(refresh, 20000);
        //
        $(document).on('click', '.rig-placement td', function () {
            var RigId = $(this).data('RigId');
            if (!RigId) {
                return false;
            }

            $("#header").html("");
            $("#gpuTempGraphModal").html("");
            $("#gpu-table").html("");
            $("#messages").html("");
            $("#MessageWindow").html("");
            $("#preloader").css("display", "block");
            $.ajax({
                dataType: "html",
                url: "/Grid/RigHistoryMessages?modelId=" + RigId,
                success: function (response) {
                    var x = $(response)
                    $("#preloader").css("display", "none");
                    $("#header").html(x.find("#HeadingText").html());
                    $("#gpuTempGraphModal").html(x.find("#gpuTempGraph").html());
                    $("#gpu-table").html(x.find("#gpuStatus").html());
                    $("#messages").html(x.find("#Messages").html());
                    $("#MessageWindow").html(x.find("#MessageWindow").html());
                    $("#RigIpForm").val($("#header").find("#Rig").text());
                    $("#RigIpForm").data('RigId', RigId);
                    $(".Gpu_table .status_column #status_span").each(function () {
                        var status = $(this).text();
                        if (status == "UP") {
                            $(this).hide().parent().children("div.empty-element").css({ "background-color": "green", "display": "block" });
                        }
                        else if (status == "DOWN") {
                            $(this).hide().parent().children("div.empty-element").css({ "background-color": "red", "display": "block" });
                        }
                    });
                }
            });
        });

        var reload = function () {
            var RigId = $("#MessageWindow").find("#RigIpForm").data('RigId');
            $("#header").html("");
            $("#gpuTempGraphModal").html("");
            $("#gpu-table").html("");
            $("#messages").html("");
            $("#MessageWindow").html("");
            $("#preloader").css("display", "block");
            $.ajax({
                dataType: "html",
                url: "/Grid/RigHistoryMessages?modelId=" + RigId,
                success: function (response) {
                    var x = $(response)
                    $("#preloader").css("display", "none");
                    $("#header").html(x.find("#HeadingText").html());
                    $("#gpuTempGraphModal").html(x.find("#gpuTempGraph").html());
                    $("#gpu-table").html(x.find("#gpuStatus").html());
                    $("#messages").html(x.find("#Messages").html());
                    $("#MessageWindow").html(x.find("#MessageWindow").html());
                    $("#RigIpForm").val($("#header").find("#Rig").text());
                    $("#RigIpForm").data('RigId', RigId);
                }
            });
        }

        $(document).on("click", "#NewMessage", function () {
            var rigHistoryMessage = $("#rigHistoryMessage").val();
            $.ajax({
                type: 'POST',
                url: '/api/run/Grid/' + $('#currentBlockName').val() + '/?button=NewMessage',
                data: { 'rigHistoryMessage': rigHistoryMessage, "RigIpForm": $("#RigIpForm").val(), "MessageId": $("#MessageId").val()},
                success: function (data) {
                    reload();
                }
            });
        });

        $(document).on("click", "#btnStopRig", function () {
            var rigHistoryMessage = $("#rigHistoryMessage").val();
            $.ajax({
                type: 'POST',
                url: '/api/run/Grid/' + $('#currentBlockName').val() + '/?button=btnStopRig',
                data: { 'rigHistoryMessage': rigHistoryMessage, "RigIpForm": $("#RigIpForm").val() },
                success: function (data) {
                    reload();
                }
            });
        });
        $(document).on("click", "#btnStartRig", function () {
            var rigHistoryMessage = $("#rigHistoryMessage").val();
            $.ajax({
                type: 'POST',
                url: '/api/run/Grid/' + $('#currentBlockName').val() + '/?button=btnStartRig',
                data: { 'rigHistoryMessage': rigHistoryMessage, "RigIpForm": $("#RigIpForm").val() },
                success: function (data) {
                    reload();
                }
            });
        }); $(document).on("click", "#btnRestartRig", function () {
            var rigHistoryMessage = $("#rigHistoryMessage").val();
            var modelId = new URL(window.location.href).searchParams.get("modelId");
            $.ajax({
                type: 'POST',
                url: '/api/run/Grid/' + $('#currentBlockName').val() + '/?button=btnRestartRig',
                data: { 'rigHistoryMessage': rigHistoryMessage, "RigIpForm": $("#RigIpForm").val(), "ContainerId": modelId },
                success: function (data) {
                    reload();
                    alert("Rig has been restarted");
                }
            });
        });

        $(document).on("click", ".fa-pencil", function () {
            var MsgId = $(this).attr('msgid');
            $("#MessageId").val(MsgId);
            var MsgTxt = $(this).parent().find(".reason-txt").html();
            $("#HiddenMessageTxt").html(MsgTxt);
            $("#CancelMsg").css("display", "block");
            $("#HiddenMessage").css("display", "block");
        });
        $(document).on("click", "#CancelMsg", function () {
            $("#MessageId").val("");
            $("#rigHistoryMessage").val("");
            $("#HiddenMessageTxt").html("");
            $("#HiddenMessage").css("display", "none");
            $("#CancelMsg").css("display", "none");
        });
    }


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
