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