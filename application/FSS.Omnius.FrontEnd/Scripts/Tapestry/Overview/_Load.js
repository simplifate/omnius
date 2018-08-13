TO.load = {

    init: function () {

    },

    _load: function () {
        var confirmed = true;

        if (TO.changedSinceLastSave)
            confirmed = confirm("You have unsaved changes. Do you really want to discard unsaved changes");
            
        if (confirmed) {
            LoadMetablock();
        }
    }

};

TO.onInit.push(TO.load.init);