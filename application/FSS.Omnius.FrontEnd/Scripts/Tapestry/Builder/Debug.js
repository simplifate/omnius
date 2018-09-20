DEBUG = {
    panelTemplate: '<div id="debug" class="debug"><div class="debug-control"><span class="fa fa-play debug-run" title="run all; watch trace"></span><span class="fa fa-forward debug-forward" title="run all; watch trace & vars"></span><span class="fa fa-step-forward debug-step" title="run single step"></span><span class="fa fa-stop debug-stop" title="terminate job"></span></div><div class="debug-status"></div><div class="debug-input"><button value="add">Add var</button><button value="apply">Apply</button></div><div class="debug-vars"></div></div>',
    newVarInputTemplate: '<div><input class="var-name" type="text" placeholder="var name" /><input class="var-value" type="text" placeholder="var value" /></div>',

    ws: null,
    rule: null,
    panel: null,
    variables: null,
    
    alive: function (rule) {
        $(rule).find('.debug-control span').tooltip();

        $(rule).find('.debug-run').on('click', function () {
            if (DEBUG.ws == null)
                DEBUG.startWs(this);
            else
                DEBUG.ws.send('{action:"runAll"}');
        });

        $(rule).find('.debug-forward').on('click', function () {
            if (DEBUG.ws == null)
                DEBUG.startWs(this);
            else
                DEBUG.ws.send('{action:"fastForward"}');
        });

        $(rule).find('.debug-step').on('click', function () {
            if (DEBUG.ws == null)
                DEBUG.startWs(this);
            else
                DEBUG.ws.send('{action:"step"}');
        });

        $(rule).find('.debug-stop').on('click', function () {
            if (DEBUG.ws != null)
                DEBUG.ws.send('{action:"stop"}');
        });

        $(rule).find('.debug-input button[value=add]').on('click', function () {
            $(rule).find('.debug-input').append(DEBUG.newVarInputTemplate);
        });

        $(rule).find('.debug-input button[value=apply]').on('click', function () {
            var message = { action: 'addVars', data: {} };

            $('.debug-input .var-name').each(function (index) {
                message.data[$(this).val()] = $(this).parent().find('.var-value').val();
            });

            DEBUG.ws.send(JSON.stringify(message));
        });
    },

    startWs: function (button) {
        var clearOldStuff = function () {
            DEBUG.rule.find('.debug-active').removeClass('debug-active');
            DEBUG.rule.find('.debug-passed').removeClass('debug-passed');
            DEBUG.rule.find('.debug-error').removeClass('debug-error').removeAttr('title').removeAttr('data-original-title');
            DEBUG.panel.find('.error-messages').text('');
            DEBUG.panel.find('.debug-status').removeClass('building waiting running finished error').text('');
        }

        if (typeof WebSocket === "undefined") {
            ShowAppNotification("Váš prohlížeč nepodporuje webSockety, a nemůže být využit k aktualizaci aplikací", "error");
            return;
        }

        DEBUG.rule = $(button).parent().parent();
        DEBUG.createPanel();
        clearOldStuff();

        // server.com:80/Tapestry/Builder/AppId?blockName=INIT;executor=button
        var url = 'ws://' + window.location.hostname + ':' + window.location.port + '/Tapestry/Builder/RunDebug/' + $('#currentAppId').val() + '?blockName=' + $('#blockHeaderBlockName').html();
        if (DEBUG.rule.find('.uiItem .itemLabel').length > 0)
            url += '&executor=' + DEBUG.rule.find('.uiItem .itemLabel').html();
        DEBUG.ws = new WebSocket(url);

        DEBUG.ws.onerror = function () {
            $(document).trigger("ajaxError", {})
        }
        DEBUG.ws.onmessage = function (event) {
            var response = JSON.parse(event.data);

            switch (response.action) {
                case "building":
                    DEBUG.panel.find('.debug-status').removeClass('waiting running finished error').addClass('building').text('Building...');
                    break;
                case "wait":
                    var wfItem = DEBUG.rule.find('#wfItem' + response.wfItemId)
                        .removeClass('debug-passed').addClass('debug-active');
                    DEBUG.panel.find('.debug-status').removeClass('building running finished error').addClass('waiting').text('Waiting for user');
                    DEBUG.setData(response.data, wfItem);
                    break;
                case "running":
                    DEBUG.rule.find('#wfItem' + response.wfItemId)
                        .removeClass('debug-active').addClass('debug-passed');
                    if (DEBUG.rule.find('.debug-active').length <= 0) // if there is no waiting thread
                        DEBUG.panel.find('.debug-status').removeClass('building waiting finished error').addClass('running').text('Running...');
                    break;
                case "end":
                    DEBUG.panel.find('.debug-status').removeClass('building waiting running error').addClass('finished').text('Finished! Redirect to: ' + response.target);
                    break;
                case "error":
                    DEBUG.rule.find('#wfItem' + response.wfItemId).removeClass('debug-active').removeClass('debug-passed').addClass('debug-error');
                    DEBUG.panel.find('.debug-status').removeClass('building waiting running finished').addClass('error').text('Error: ' + response.message);
                    break;
            }
        };
        DEBUG.ws.onclose = function (event) {
            if (event.code === 1006) {
                alert('unknown error');
            }

            DEBUG.ws = null;
        }
    },

    createPanel: function () {
        if ($('#debug').length == 0) {
            $('#tapestryLeftBar .leftBarHeader').after(DEBUG.panelTemplate);
            DEBUG.panel = $('#debug');

            DEBUG.alive(DEBUG.panel);
        }
    },

    setData: function (data, wfItem) {
        // wfItem
        var result = '';
        $.each(data, function (key, value) {
            result += key + ': ' + value + '\n';
        });
        wfItem.attr('title', result).tooltip();

        // update
        if (DEBUG.variables == null)
            DEBUG.variables = data;
        else {
            $.each(data, function (key, value) {
                DEBUG.variables[key] = value;
            });
        }
        
        // edit vars
        var result = '';
        $.each(DEBUG.variables, function (key, value) {
            result += '<div class="debug-var">' + key + ': ' + value + '</div>';
        });
        DEBUG.panel.find('.debug-vars').html(result);
    }
}