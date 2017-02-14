TB.wizard = {

    actions: {},
    dataTypeList: {
        's$': 'string',
        'b$': 'boolean',
        'i$': 'integer',
        'f$': 'float',
        'd$': 'datetime',
    },

    init: function () 
    {
        var self = TB.wizard;

        TB.library.onClean.push(self._onLibraryClean);
        TB.library.onCreate.push(self._onLibraryCreateItem);

    },

    parseVars: function(source) {
        var vars = [];
        for (var i = 0; i < source.length; i++) {
            var m = source[i].match(/^(\?)?([a-z]\$)?([a-z]+)(\[([^\]]*)\])?$/i);
            // 1 = ?    = volitelná?
            // 2 = s$   = typ
            // 3 = název
            // 4 = pole nebo enum
            // 5 = enum items

            vars.push({
                required: m[1] == '?' ? false : true,
                type: m[2],
                name: m[3],
                isArray: m[4] && !m[5],
                isEnum: m[4] && m[5],
                enumItems: m[5] ? m[5].split('|') : []
            });
        }
        return vars;
    },

    _onLibraryClean: function () {
        TB.wizard.actions = {};
    },

    _onLibraryCreateItem: function (type) {
        if (type == 'action') {
            var self = TB.wizard;
            self.actions[this.Id] = {
                name: this.Name,
                inputVars: self.parseVars(this.InputVars),
                outputVars: self.parseVars(this.OutputVars)
            }
        }
    }

};

TB.onInit.push(TB.wizard.init);