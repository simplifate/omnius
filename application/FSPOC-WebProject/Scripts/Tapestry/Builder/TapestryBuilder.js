var TB = {

    onInit: [],

    init: function () {
        var self = TB;

        for (var i = 0; i < self.onInit.length; i++) {
            self.onInit[i]();
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
