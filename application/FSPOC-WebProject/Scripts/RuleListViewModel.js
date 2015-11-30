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