var ASPC = ASPC || {};

ASPC.Rollup = {
    RebelsData: "",

    Init: function(elId){
        ASPC.Rollup.GetItems(elId);
    },

    InitRollup: function (rollID) {
        var roll = document.getElementById(rollID);
        ko.applyBindings(new ASPC.Rollup.RollupViewModel({ elementId: rollID }), roll);
    },

    MenuItem: function (data) {
        var self = this;
        self.title = data.title;
    },

    RollupViewModel: function (menuItems) {
        var self = this;
        self.jsondata = 
        self.menuItems = self.jsondata.children;

        self.load = function () {
            var mappedItems = $.map(self.jsondata.children, function (item) { return new ASPC.Rollup.MenuItem(item) });
            self.menuItems = mappedItems;
        }

        self.load();
    },

    GetItems: function (elId) {
        $.ajax({
            url: "/_api/web/lists/getbytitle('Rebels')/getitems",
            method: "GET",
            headers: { "accept": "application/json;odata=verbose", "content-type": "application/json;odata=verbose", "X-RequestDigest": $("#__REQUESTDIGEST").val() },
            success: function (data) {
                console.log(data);
                ASPC.Rollup.RebelsData = data;
                ASPC.Rollup.InitRollup(elId);
            },
            error: function (data) {
                console.log(data);
            }
        });
    },
};