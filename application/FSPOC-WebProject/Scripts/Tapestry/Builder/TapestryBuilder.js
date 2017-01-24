var TB = {

    onInit: [],

    changedSinceLastSave: false,

    init: function () {
        var self = TB;

        for (var i = 0; i < self.onInit.length; i++) {
            self.onInit[i]();
        }
    },

    checkRuleResizeLimits: function (rule, resourceRuleMode)
    {
        var horizontalLimit = 1000000;
        var verticalLimit = 1000000;

        var ruleLeft = rule.position().left;
        var ruleRight = ruleLeft + rule.width();
        var ruleTop = rule.position().top;
        var ruleBottom = rule.position().top + rule.height();

        $(resourceRuleMode ? "#resourceRulesPanel .resourceRule" : "#workflowRulesPanel .workflowRule").each(function (index, element) {
            var otherRule = $(element);
            if (otherRule.attr("id") != rule.attr("id")) {
                var otherRuleLeft = otherRule.position().left;
                var otherRuleRight = otherRuleLeft + otherRule.width();
                var otherRuleTop = otherRule.position().top;
                var otherRuleBottom = otherRule.position().top + otherRule.height();

                if (otherRuleTop < ruleBottom && otherRuleBottom > ruleTop
                    && otherRuleLeft + 30 > ruleRight && otherRuleLeft - ruleLeft < horizontalLimit)
                    horizontalLimit = otherRuleLeft - ruleLeft;
                if (otherRuleLeft < ruleRight && otherRuleRight > ruleLeft
                    && otherRuleTop  + 20 > ruleBottom && otherRuleTop - ruleTop < verticalLimit)
                    verticalLimit = otherRuleTop - ruleTop;
            }
        });
        return { horizontal: horizontalLimit, vertical: verticalLimit };
    }
};


$(function () {
    if (CurrentModuleIs("tapestryModule")) {
        TB.init();
    }
});
