var ASPC = ASPC || {};

ASPC.Rollup = {
    InitRollup: function (rollID) {
        var roll = document.getElementById(rollID);
        ko.applyBindings(new Coop.TopMenu.TopMenuViewModel({ elementId: rollID }), roll);
    },

    MenuItem: function (data) {
        var self = this;
        self.title = data.title;
    },

    RollupViewModel: function (menuItems) {
        var self = this;
        self.jsondata = //GET JSON
        self.menuItems = self.jsondata.children;

        self.load = function () {
            var mappedItems = $.map(self.jsondata.children, function (item) { return new ASPC.Rollup.MenuItem(item) });
            self.menuItems = mappedItems;
        }

        self.load();
    },
};