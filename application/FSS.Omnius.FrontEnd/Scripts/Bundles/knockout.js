function RuleDetailViewModel(ruleId) {
    "use strict";
    var self = this;
    self.ruleId = ruleId;
    self.name = ko.observable("User");
    self.actionList = ko.observableArray([
        {id: 1, name: "Send email", enabled: ko.observable(true)},
        { id: 2, name: "Save view", enabled: ko.observable(false)},
        { id: 3, name: "Login", enabled: ko.observable(false)}
    ]);

    var save = function () {
        var list = [];
        self.actionList().forEach(function (action) {
            list.push({id: action.id, enabled: action.enabled()});
        });
        var updateModel = {
            name: self.name(),
            list: list
        };
        alert(JSON.stringify(updateModel));

    };
    var countSelected = ko.computed(function () {
        var total = 0;
        self.actionList().forEach(function (action) {
            if (action.enabled()) {
                total++;
            }
        });
        return "Total selected: " + total;
    });
    var model = {
        name: self.name,
        actionList: self.actionList,
        onSave: save,
        countSelected: countSelected
    };

    return model;

}
function RuleListViewModel(urlForDetail) {
    "use strict";
    var self = this;
    self.urlDetail = urlForDetail;

    self.rules = ko.observableArray([
        {name: "Admin", id: 1},
        {name: "User", id: 2},
        {name: "Managment", id: 3}
    ]);

    var removeRule = function () {
        self.rules.remove(this);
    };
    var updateRule = function () {
        window.location.href = self.urlDetail + "/" + this.id;
    };
    var countRule = ko.computed(function () {
        return "Total :" + self.rules().length;
    });

    var model = {
        rules: self.rules,
        removeRule: removeRule,
        updateRule: updateRule,
        countRule: countRule

    };
    return model;
}
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