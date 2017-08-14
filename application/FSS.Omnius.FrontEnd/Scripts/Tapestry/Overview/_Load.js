TO.load = {

    init: function () {

    },

    _load: function () {
        var confirmed = true;

        if (TO.changedSinceLastSave)
            confirmed = confirm("Máte neuložené změny, opravdu si přejete tyto změny zahodit?");
            
        if (confirmed) {
            LoadMetablock();
        }
    }

};

TO.onInit.push(TO.load.init);