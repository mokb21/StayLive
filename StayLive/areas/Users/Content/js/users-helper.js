UsersHelper = {
    IndexLoad: function (isSysAmin) {
        this.LoadUsersTable(isSysAmin);
    },

    LoadUsersTable: function (isSysAmin) {
        var currentAccount = $('#hfCurrentAccount').val();
        var usersTbl = $("#UsersTbl").loadDataTable(GeneralHelper.Url("getUsersData", "User", "Users"), 0,
            function (d) { },
            [{
                "data": "Id", "width": "10%", "orderable": false, "searchable": false, "render": function (data) {
                    return '<img src="' + GeneralHelper.DrawImage({ "T": "u", "Id": data }) + '" class="img-circle datatable-image" width="32px" height="32px"/>';
                }
            },
            { "data": "Name" },
            { "data": "Email" },
            { "data": "Mobile" },
            {
                "data": "Role", "render": function (data) {
                    switch (data) {
                        case "SystemAdmin":
                            return '<span class="badge badge-purple">' + 'System Admin' + '</span>';
                        case "Supervisor":
                            return '<span class="badge badge-warning">' + data + '</span>';
                        case "Agent":
                            return '<span class="badge badge-success">' + data + '</span>';
                        case "Admin":
                            return '<span class="badge badge-info">' + data + '</span>';
                        default:
                            return '';
                    }

                }
            },
            { "data": "Company" },
            {
                "data": "Id", 'className': 'dt-body-right', "width": "10%", "orderable": false, 'render': function (data, type, full, meta) {
                    if (currentAccount == data)
                        return "";
                    else
                        return ActionIcon("Delete", "Delete", null, "ShowDeleteAlert('" + GeneralHelper.Url("Delete", "User", "Users", { "id": data }) + "','" + full.Name + "')") +
                            ActionIcon("Password", "Reset Password", null, "ShowSaveModal('" + GeneralHelper.Url("ResetPassword", "User", "Users", { "id": data }) + "')");
                }
            }],
            function (e, dt, type, indexes) {
                var rowData = dt.rows(indexes).data().toArray()[0];
                window.location.href = GeneralHelper.Url("UserProfile", "User", "Users", { "id": rowData.Id });
            }
        );
        usersTbl.column(5).visible($('#hfCurrentRole').val() == '1');
    },

    UserProfileLoad: function () {
        this.notSuperAdminFields();
        this.notSuperAdminOrAdmin();
        var id = $('#hdId').val();
        if (id > 0)
            $('.notNewUser').hide();
        else
            $('.notNewUser').show();
    },

    notSuperAdminFields: function () {
        var ddl = $('#ddlRoles').val();
        if (ddl == 1)
            $('.notSuperAdmin').hide();
        else
            $('.notSuperAdmin').show();
    },

    notSuperAdminOrAdmin: function () {
        var ddl = $('#ddlRoles').val();
        if (ddl == 1 || ddl == 2)
            $('.notSuperAdminOrAdmin').hide();
        else
            $('.notSuperAdminOrAdmin').show();
    }
}