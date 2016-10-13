MBE.types.containers = {

    templates: {
        'container': '<div class="container-fluid"></div>'
    },

    options: {
        'container': {
            'containerOptions': {
                name: 'Container options',
                type: 'boolean',
                options: {
                    'container container-fluid': 'Fluid'
                },
                set: MBE.options.toggleClass,
                get: MBE.options.hasClass
            }
        }
    }
};