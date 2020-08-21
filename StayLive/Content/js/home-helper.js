HomeHelper = {
    IndexLoad: function () {
        this.LoadAssignedTicketsChart();
        this.LoadLevelsStatusChart();
        this.LoadUsersStatusChart();
    },

    LoadAssignedTicketsChart: function () {
        var Chart = document.getElementById("AssignedTicketStatusChart").getContext("2d");
        $.ajax({
            type: "POST",
            url: GeneralHelper.Url("GetAssignedTickets", "Home", ""),
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.success == true) {
                    BuildDoughnutChart(Chart, data.data);
                }
                else {
                    InfoMessage.error("Load Assigned Tickets", "Sorry somthing went wrog");
                }
            },
            error: function () {
                InfoMessage.error("Load Assigned Tickets", "Sorry somthing went wrog");
            }
        });
    },

    LoadLevelsStatusChart: function () {
        var Chart = document.getElementById("LevelsStatusChart").getContext("2d");
        $.ajax({
            type: "POST",
            url: GeneralHelper.Url("GetLevelsStatus", "Home", ""),
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.success == true) {
                    BuildDoughnutChart(Chart, data.data);
                }
                else {
                    InfoMessage.error("Load Levels Status", "Sorry somthing went wrog");
                }
            },
            error: function () {
                InfoMessage.error("Load Levels Status", "Sorry somthing went wrog");
            }
        });

    },

    LoadUsersStatusChart: function () {
        var Chart = document.getElementById("UsersStatusChart").getContext("2d");
        $.ajax({
            type: "POST",
            url: GeneralHelper.Url("GetTopUsers", "Home", ""),
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.success == true) {
                    BuildBarChart(Chart, data.data);
                }
                else {
                    InfoMessage.error("Load Users Status", "Sorry somthing went wrog");
                }
            },
            error: function () {
                InfoMessage.error("Load Users Status", "Sorry somthing went wrog");
            }
        });
    }
}