TO.save = {

    init: function () {

    },

    _save: function () {
        SaveMetablock();
    }

};

TO.onInit.push(TO.save.init);