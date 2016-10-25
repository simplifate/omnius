MBE.options.getProgressBarClass = function (value) {
    return $('.progress-bar', this).hasClass(value);
};
MBE.options.setProgressBarClass = function (opt) {
    if (opt.type == 'select') {
        var classes = new Array();
        $(opt).find('option').each(function () {
            classes.push(this.value);
        });
        $('.progress-bar', this).removeClass(classes.join(' '));
        if (opt.value == 'null') {
            return;
        }
    }
    $('.progress-bar', this).toggleClass(opt.value);
};

MBE.types.misc = {

    templates: {
        'badge': '<span class="badge">4</span>',
        'tag': '<span class="label label-default">label</span>',
        'caret': '<span class="caret"></span>',
        'close': '<button type="button" class="close" aria-label="Close"><span aria-hidden="true">&times;</span></button>',
        'hr': '<hr />',
        'responsive-embed': '<div class="embed-responsive embed-responsive-16by9"><iframe class="embed-responsive-item" src=""></iframe></div>',
        'progressBar': '<div class="progress">' + 
                            '<div class="progress-bar" role="progressbar" style="min-width:2em; width:0%"></div>' +
                        '</div>'
    },

    options: {
        'tag': {
            'tagOptions': {
                name: 'Label options',
                type: 'group',
                groupItems: [{
                    label: 'Style',
                    type: 'select',
                    options: {
                        'label-default': 'Default',
                        'label-primary': 'Primary',
                        'label-success': 'Success',
                        'label-info': 'Info',
                        'label-warning': 'Warning',
                        'label-danger': 'Danger',
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        },
        'progressBar': {
            'progressBarOptions': {
                name: 'Progress bar options',
                type: 'group',
                groupItems: [{
                    label: 'Percentage',
                    type: 'number',
                    get: function () {
                        return parseInt($('.progress-bar', this).css('width'));
                    },
                    set: function (opt) {
                        $('.progress-bar', this).css('width', opt.value + '%');
                        if ($('.progress-bar', this).text().length > 0) {
                            $('.progress-bar', this).html(opt.value + '%');
                        }
                    }
                }, {
                    label: 'Style',
                    type: 'select',
                    options: {
                        'null': 'Default',
                        'progress-bar-success': 'Success',
                        'progress-bar-info': 'Info',
                        'progress-bar-warning': 'Warning',
                        'progress-bar-danger': 'Danger'
                    },
                    get: MBE.options.getProgressBarClass,
                    set: MBE.options.setProgressBarClass
                }, {
                    label: 'Options',
                    type: 'boolean',
                    options: {
                        'progress-bar-striped': 'Striped',
                        'active': 'Animated'
                    },
                    get: MBE.options.getProgressBarClass,
                    set: MBE.options.setProgressBarClass
                }, {
                    label: 'Show label',
                    type: 'boolean',
                    options: {
                        'show-label': 'Show label'
                    },
                    get: function () {
                        return $('.progress-bar', this).text().length > 0;
                    },
                    set: function (opt) {
                        if (opt.checked) {
                            $('.progress-bar', this).html($('.progress-bar', this)[0].style.width);
                        }
                        else {
                            $('.progress-bar', this).html('');
                        }
                    }
                }]
            }
        },
        'responsive-embed': {
            'responsiveEmbedOptions': {
                name: 'Responsive embed options',
                type: 'group',
                groupItems: [{
                    label: 'Source URL',
                    type: 'text',
                    get: function () {
                        return $('> iframe', MBE.options.target).attr('src').length ? $('> iframe', MBE.options.target).attr('src') : '';
                    },
                    set: function (opt) {
                        $('> iframe', this).attr('src', opt.value);
                    }
                }, {
                    label: 'Aspect ratio',
                    type: 'select',
                    options: {
                        'embed-responsive-16by9': '16 by 9',
                        'embed-responsive-4by3': '4 by 3'
                    },
                    get: MBE.options.hasClass,
                    set: MBE.options.toggleClass
                }]
            }
        }
    }
}