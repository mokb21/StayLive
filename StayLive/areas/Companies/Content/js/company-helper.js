CompanyHelper = {
    IndexLoad: function () {
        this.LoadCompaniesTable();
    },

    LoadCompaniesTable: function () {
        $("#CompaniesTbl").loadDataTable(GeneralHelper.Url("getCompaniesData", "Company", "Companies"), 1,
            function (d) { },
            [{
                "data": "Id", "width": "10%", "orderable": false, "searchable": false, "render": function (data) {
                    return '<img src="' + GeneralHelper.DrawImage({ "T": "c", "Id": data }) + '" class="img-circle datatable-image" width="32px" height="32px"/>';
                }
            },
            { "data": "Name" },
            { "data": "Email" },
            { "data": "Region" },
            {
                "data": "Id", 'className': 'dt-body-right', "orderable": false, 'render': function (data, type, full, meta) {
                    return ActionIcon("Delete", "Delete", null, "ShowDeleteAlert('" + GeneralHelper.Url("Delete", "Company", "Companies", { "id": data }) + "','" + full.Name + "')") +
                        ActionIcon("Password", "Reset Password", null, "ShowSaveModal('" + GeneralHelper.Url("ResetPassword", "Company", "Companies", { "id": data }) + "')");
                }
            }],
            function (e, dt, type, indexes) {
                var rowData = dt.rows(indexes).data().toArray()[0];
                window.location.href = GeneralHelper.Url("Company", "Company", "Companies", { "id": rowData.Id });
            },
            function (settings) { },
            function (json) { return json.data; },
            10,
            {
                "serverSide": false,
            }
        );
    },

    CompanyLoaded: function () {
        var Id = $('#hfId').val();
        if (Id > 0)
            $('.notNewCompany').hide();
        else
            $('.notNewCompany').show();
    }
}