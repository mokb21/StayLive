
var InfoMessage = {
    MessageType: {
        success: 1,
        error: 2,
        info: 3,
        warning: 4
    },

    DefaultSettings: {
        showHideTransition: 'fade',  // It can be plain, fade or slide
        bgColor: 'blue',
        textColor: '#fff',
        allowToastClose: true,
        hideAfter: 5000,
        stack: 5,
        icon: 'info',
        textAlign: 'left',
        position: 'top-right',
        loader: true,        // Change it to false to disable loader
        loaderBg: '#fff'
    },

    show: function (settings) {
        $.extend(settings);
        $.toast(this.DefaultSettings)
    },

    success: function (title, text) {
        $.extend(this.DefaultSettings, {
            heading: title,
            text: text,
            icon: 'success',
            bgColor: "#5CB57C"
        });

        $.toast(this.DefaultSettings)
    },

    error: function (title, text) {
        $.extend(this.DefaultSettings, {
            heading: title,
            text: text,
            icon: 'error',
            bgColor: "#ff2b52"
        });

        $.toast(this.DefaultSettings)
    },

    info: function (title, text) {
        $.extend(this.DefaultSettings, {
            heading: title,
            text: text,
            icon: 'info',
            bgColor: "#1E88E5"
        });

        $.toast(this.DefaultSettings)
    },

    warning: function (title, text) {
        $.extend(this.DefaultSettings, {
            heading: title,
            text: text,
            icon: 'warning',
            bgColor: "#FFB62B"
        });

        $.toast(this.DefaultSettings)
    },

    savedSuccessfuly: function () {

    },

    notValid: function () {

    },

    notFound: function () {

    },

    emailAlreadyExist: function () {

    },



};