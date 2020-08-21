var GeneralHelper = {
    Url: function (Action, Controller, Area, Parameters) {
        var url = "/";
        var virtualDirectory = window.location.href.split('/')[3];
        url += virtualDirectory + "/" + (Area.length > 0 ? (Area + "/") : "") + (Controller.length > 0 ? (Controller + "/") : "") + Action
        if (Parameters != null)
            url += "?";
        $.each(Parameters, function (key, val) {
            url += key + "=" + val + "&"
        });
        if (Parameters != null)
            url = url.slice(0, -1)

        return url;
    },

    DrawImage: function (Parameters) {
        var url = "/";
        var virtualDirectory = window.location.href.split('/')[3];
        url += virtualDirectory + "/Helpers/DrawImage.ashx";
        if (Parameters != null)
            url += "?";
        $.each(Parameters, function (key, val) {
            url += key + "=" + val + "&"
        });
        if (Parameters != null)
            url = url.slice(0, -1)

        return url;
    }
}