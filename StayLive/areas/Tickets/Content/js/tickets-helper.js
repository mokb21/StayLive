TicketsHelper = {
    IndexLoad: function () {
        this.LoadTicketsTable();
        $('.filter-side-toggle').click(function () {
            $("#TicketsFilter").slideDown(50);
            $("#TicketsFilter").toggleClass("shw-rside");
        });
    },

    LoadTicketsTable: function () {
        $("#TicketsTbl").loadDataTable(GeneralHelper.Url("getTicketsData", "Ticket", "Tickets"), 0,
            function (d) { },
            [{ "data": "Subject" },
            { "data": "ClientName" },
            { "data": "CreateDate" },
            { "data": "Level" },
            {
                "data": "Status", "render": function (data, type, row, meta) {
                    return '<span class="badge badge-info ' + data + '">' + data + '</span>';
                }
            },
            {
                "data": "Id", 'className': 'dt-body-right', "orderable": false, 'render': function (data, type, full, meta) {
                    return ActionIcon("Delete", "Delete", null, "ShowDeleteAlert('" + GeneralHelper.Url("Delete", "Ticket", "Tickets", { "id": data }) + "','" + full.Subject + "')");
                }
            }
            ],
            function (e, dt, type, indexes) {
                var rowData = dt.rows(indexes).data().toArray()[0];
                window.location.href = GeneralHelper.Url("Ticket", "Ticket", "Tickets", { "Key": rowData.Key });
            },
            function (setting) {
                TicketsHelper.AddTicketsFilterButton();
            }
        );
    },

    AddTicketsFilterButton: function () {
        if (!$('#btnTicketsFilter').length) {
            $('<span>', {
                'id': "ctrlTicketsFilter"
            }).appendTo($('#TicketsTbl_filter'));

            $('<a>', {
                'id': 'btnTicketsFilter',
                'class': 'btn btn-primary filter-side-toggle m-l-10'
            }).appendTo($('#ctrlTicketsFilter'));

            $('<i>', {
                'class': 'mdi mdi-filter text-white'
            }).appendTo($('#btnTicketsFilter'));

            $("#btnTicketsFilter").on("click", function () {
                $(".right-sidebar").slideDown(50);
                $(".right-sidebar").toggleClass("shw-rside");
            });
        }
    },

    CheckAttachementInput: function () {
        if ($('#Attachment').get(0).files.length === 0) {
            $('#AttachementName').html('');
        }
        else {
            $('#AttachementName').html($('#Attachment').get(0).files[0].name);
        }
    }

}