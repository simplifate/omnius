var ModalDialogArray = [];
$(function () {
    $("#hideMenuIcon").on("click", function () {
        $("#userLeftBar").hide();
        $("#userLeftBarMinimized").show();
        $("#userContentArea").css("left", 60);
        $("#helpContainer").css("left", 60);
    });
    $("#showMenuIcon").on("click", function () {
        $("#userLeftBarMinimized").hide();
        $("#userLeftBar").show();
        $("#userContentArea").css("left", 310);
        $("#helpContainer").css("left", 310);
    });
    if (CurrentModuleIs("appManagerModule")) {
        $(".appPanel").draggable({
            grid: [120, 120],
            revert: "invalid",
            stop: function () {
                $(this).draggable("option", "revert", "invalid");
                $.ajax({
                    type: "POST",
                    url: 'api/master/apps' + $(this).attr('data-target') + '/saveAppPosition',
                    data: {
                        'positionX': $(this).css('left'),
                        'positionY': $(this).css('top')
                    }
                });
            }
        });
        $(".appWorkspace").droppable({
            tolerance: "fit"
        });
        $(".appPanel").droppable({
            greedy: true,
            tolerance: "touch",
            drop: function (event, ui) {
                ui.draggable.draggable("option", "revert", true);
            }
        });
        $(".appPanel").bind("dragstart", function (event, ui) {
            ui.originalPosition.top = $(this).position().top;
            ui.originalPosition.left = $(this).position().left;
        });
        $(".appPanel").on("dblclick", function () {
            window.location.href = $(this).attr('data-target');
        });
    }
    else if (CurrentModuleIs("helpModule")) {
        $("#appManagerIcon").removeClass("activeIcon");
        $("#helpIcon").addClass("activeIcon");
    }
    else if (CurrentModuleIs("userDetailsModule")) {
        $("#appManagerIcon").removeClass("activeIcon");
    };
    if ($("#userLeftBar").length > 0) {
        $("#userLeftBar").css("height", $(window).height() + $(window).scrollTop() - 50);
        $(window).scroll(function () {
            $("#userLeftBar").css("height", $(window).height() + $(window).scrollTop() - 50);
        });
        $(window).resize(function () {
            $("#userLeftBar").css("height", $(window).height() + $(window).scrollTop() - 50);
        });
        $(".uic.data-table").each(function(index, element) {
            table = $(element);
            CreateCzechDataTable(table);
            wrapper = table.parents(".dataTables_wrapper");
            wrapper.css("position", "absolute");
            wrapper.css("left", table.css("left"));
            wrapper.css("top", table.css("top"));
            table.css("position", "relative");
            table.css("left", "0px");
            table.css("top", "0px");
            table.on("click", ".rowEditAction", function () {
                rowId = parseInt($(this).parents("tr").find("td:first").text());
                // TODO: call API with this rowId
            });
            table.on("click", ".rowDetailsAction", function () {
                rowId = parseInt($(this).parents("tr").find("td:first").text());
                // TODO: call API with this rowId
            });
            table.on("click", ".rowDeleteAction", function () {
                rowId = parseInt($(this).parents("tr").find("td:first").text());
                // TODO: call API with this rowId
            });
        });
        $(".uic.input-with-datepicker").datepicker($.datepicker.regional['cs']);
        $(".uic.color-picker").each(function (index, element) {
            CreateColorPicker($(element));
            newReplacer = $("#userContentArea .sp-replacer:last");
            newReplacer.css("position", "absolute");
            newReplacer.css("left", droppedElement.css("left"));
            newReplacer.css("top", droppedElement.css("top"));
        });
        $("#modalRepository .modalRepositoryItem").each(function (index, element) {
            currentDialog = $(element);
            currentDialog.dialog({
                autoOpen: false,
                width: 370,
                height: 320,
                buttons: {
                    "OK": function () {
                        alert("TODO: Save");
                        $(this).dialog("close")
                    },
                    "Zrušit": function () {
                        $(this).dialog("close");
                    }
                }
            });
            ModalDialogArray.push(currentDialog);
        });
    }
});
