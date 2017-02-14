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
    },

    callHooks(hooks, context, params) {
        for (var i = 0; i < hooks.length; i++) {
            hooks[i].apply(context, params);
        }
    }
};


$(function () {
    if (CurrentModuleIs("tapestryModule")) {
        TB.init();
    }
    $(".librarySearchToggler").on("click", function () {
        var search = $(this).parent().parent().find(".librarySearch");
        if (search.hasClass("shown")) {
            search.addClass("hidden");
            search.removeClass("shown");
        } else {
            search.addClass("shown");
            search.removeClass("hidden");
        }
    });
    
    $(".librarySearch").on("keyup", function () {
        var category = $(this).parent();
        var items = category.find(".libraryItem");
        var value = $(this).val();
        items.each(function () {
            $(this).detach();
        });
        items.sort(function (a, b) {
            a = $(a).text();
            b = $(b).text();
            if (value !== "") {
                var aSim = similar(a, value);
                var bSim = similar(b, value);
                return aSim === bSim ? CompareAlphabetical(a, b) : aSim < bSim ? 1 : -1;
            } else {
                return CompareAlphabetical(a, b);
            }
        });
        items.each(function () {
            category.append($(this));
        });
    });
    function CompareAlphabetical(a, b) {
        return a === b ? 0 : a < b ? -1 : 1;
    }
    function similar(a, b) {
        var lengthA = a.length;
        var lengthB = b.length;
        var equivalency = 0;
        var minLength = (a.length > b.length) ? b.length : a.length;
        var maxLength = (a.length < b.length) ? b.length : a.length;
        for (var i = 0; i < minLength; i++) {
            if (a[i] == b[i]) {
                equivalency++;
            }
        }


        var weight = equivalency / maxLength;
        return (weight * 100);
    }
});
