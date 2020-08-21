LevelsHelper = {
    IndexLoad: function () {
        this.LoadLevelsTree();
    },

    LoadLevelsTree: function () {
        BuildTree("LevelsTree", GeneralHelper.Url("LevelsTree", "Level", "Levels"), function (e, data) {
            return undefined;
        });
    }
}