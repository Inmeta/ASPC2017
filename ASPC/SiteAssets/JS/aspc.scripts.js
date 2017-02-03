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
        self.title = data.Title;
        self.location = data.ASPC_Location;
        self.members = data.ASPC_Members;
        self.type = data.ASPC_RebelType;
        self.url = data.__metadata.uri;
    },

    RollupViewModel: function (menuItems) {
        var self = this;
        self.jsondata = ASPC.Rollup.RebelsData;
        self.menuItems = self.jsondata.d.results;

        self.load = function () {
            var mappedItems = $.map(self.jsondata.d.results, function (item) { return new ASPC.Rollup.MenuItem(item) });
            self.menuItems = mappedItems;
        }

        self.load();
    },

    GetItems: function (elId) {
        $.ajax({
            url: "/_api/web/lists/getbytitle('Rebels')/getitems",
            method: "POST",
            headers: { "accept": "application/json;odata=verbose", "content-type": "application/json;odata=verbose", "X-RequestDigest": $("#__REQUESTDIGEST").val() },
            data: JSON.stringify({ 'query': { '__metadata': { 'type': 'SP.CamlQuery' }, 'ViewXml': '<View><Query></Query></View>' } }),
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