MBE.types.image = {

    templates: {
        'image': '<img src="" width="80" height="80" alt="">',
        'icon': '<span class="fa fa-star"></span>',
        'figure': '<figure></figure>',
        'figcaption': '<figcaption>Caption</figcaption>'
    },

    options: {
        'image': {
            'imageOptions': {
                name: 'Image options',
                type: 'group',
                groupItems: [{
                    label: 'Source URL',
                    type: 'text',
                    attr: 'src',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Width',
                    type: 'text',
                    attr: 'width',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Height',
                    type: 'text',
                    attr: 'height',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Alt',
                    type: 'text',
                    attr: 'alt',
                    get: MBE.options.hasAttr,
                    set: MBE.options.setAttr
                }, {
                    label: 'Style',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'img-rounded': 'Rounded',
                        'img-circle': 'Circle',
                        'img-thumbnail': 'Thumbnail'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }, {
                    label: 'Responsive',
                    type: 'boolean',
                    options: {
                        'img-responsive': 'Responsive'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'icon': {
            'iconOptions': {
                name: 'Icon options',
                type: 'group',
                groupItems: [{
                    label: 'Icon',
                    type: 'icon',
                    fontSets: { 'fa': 'Font Awesome', 'glyphicon': 'Glyphicons' },
                    set: MBE.options.setIcon
                }]
            }
        }
    }
}