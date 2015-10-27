function BlockViewModel(configModel, viewModelData) {
    "use strict";
    var self = this;
    self.data = ko.observable(viewModelData);
    self.configModel = configModel;

    var handleAction = function (data, event) {
        var actionId = event.target.attributes["data-actionId"].value;
        var configAction = self.configModel.listConfigRoute.filter(function (configAction) {
            return configAction.id == actionId;
        })[0];
        $.post(configAction.route, {BlockId: 1, SourceObject: JSON.stringify(self.data())}, function (result) {
            result.forEach(function (resultAction) {
                alert(resultAction.ResultMessage);
            });
        });
    };
    var model = {
        data: self.data,
        onHandleAction: handleAction
    };
    return model;
}