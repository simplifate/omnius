function RecalculateMozaicToolboxHeight() {
    leftBar = $("#mozaicLeftBar");
    leftBarMinimized = $("#mozaicLeftBarMinimized");
    scrollTop = $(window).scrollTop();
    lowerPanelTop = $("#lowerPanel").offset().top;
    leftBar.height($(window).height() + scrollTop - lowerPanelTop - leftBar.position().top);
    leftBarMinimized.height($(window).height() + scrollTop - lowerPanelTop - leftBarMinimized.position().top);
}
