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
        var value = $(this).val().toLowerCase();
        var words = value.split(" ");
        items.each(function () {
            $(this).detach();
        });
        items.sort(CompareAlphabetical);
        items.sort(function (a, b) {
            a = $(a).text().toLowerCase();
            b = $(b).text().toLowerCase();
            if (value !== "") {
                /*if (a === value) {
                    return 1;
                } else if(b === value) {
                    return -1;
                }*/
                var aContains = contained(a, value);
                var bContains = contained(b, value);
                var aSplit = SplitedContainedCompare(a, words);
                var bSplit = SplitedContainedCompare(b, words);
                var containsScore = aContains > bContains ? -1 : 1;
                var splittedContainsScore = aSplit > bSplit ? -1 : 1;
                var alphabeticalScore = CompareAlphabetical(a, b);
                var result = aContains === bContains ? splittedContainsScore : containsScore;
                return aContains === bContains && aSplit === bSplit ? alphabeticalScore : result;
            }
        });
        items.each(function () {
            category.append($(this));
        });
    });
    function SplitedContainedCompare(text, words) {
        var funds = 0;
        $.each(words, function (i) {
            if (words[i] !== "") {
                if (contained(text, words[i])) {
                    funds++;
                }
            }
        });
        return funds;
    }
    function CompareAlphabetical(a, b) {
        return a === b ? 0 : a < b ? -1 : 1;
    }
    function contained(a, b) {
        var matched = a.indexOf(b) !== -1;
        return matched ? 1 : 0;
    }
});
