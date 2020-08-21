/**
 * Automatically detect British (`dd/mm/yyyy`) date types. Goes with the UK 
 * date sorting plug-in.
 *
 *  @name Date (`dd/mm/yyyy`)
 *  @summary Detect data which is in the date format `dd/mm/yyyy`
 *  @author Andy McMaster
 */



jQuery.extend(jQuery.fn.dataTableExt.oSort, {
    "uk-date-pre": function (value) {
        var date = $(value, 'span')[0].innerHTML;
        date = date.split('/');
        return Date.parse(date[1] + '/' + date[0] + '/' + date[2])
    },
    "uk-date-asc": function (a, b) {
        return ((a < b) ? -1 : ((a > b) ? 1 : 0));
    },
    "uk-date-desc": function (a, b) {
        return ((a < b) ? 1 : ((a > b) ? -1 : 0));
    }
});

jQuery.extend(jQuery.fn.dataTableExt.oSort, {
    "numeric-comma-pre": function (a) {
        var x = (a == "-") ? 0 : a.replace(/,/, ".");
        return parseFloat(x);
    },

    "numeric-comma-asc": function (a, b) {
        return ((a < b) ? -1 : ((a > b) ? 1 : 0));
    },

    "numeric-comma-desc": function (a, b) {
        return ((a < b) ? 1 : ((a > b) ? -1 : 0));
    }
});

$.fn.loadDataTable = function (url, sortColumn, dataPost, dataHandler, selectEvent, drawCallback, dataSrcFun, pageLength, options) {
    dataSrcFun = dataSrcFun || function (json) {
        return json.data;
    };

    drawCallback = drawCallback || function (settings) {
        $('.tip').qtip({
            content: false,
            position: {
                my: 'bottom center',
                at: 'top center',
            },
            style: {
                classes: 'qtip-tipsy'
            }
        });
    }

    selectEvent = selectEvent || function (e, dt, type, indexes) {
        //var rowData = dt.rows(indexes).data().toArray()[0];
    }

    var settings = $.extend({
        "order": [[sortColumn || 1, "asc"]],
        "pageLength": pageLength || 10,
        "pagingType": "simple_numbers",
        "pagingType": "simple_numbers",
        "searchDelay": 500,
        "paging": true,
        "ordering": true,
        "processing": true,
        "stateSave": true,
        "info": true,
        "serverSide": true,
        "bSortCellsTop": true,
        "filter": true,
        "orderMulti": false,
        "select": {
            'style': 'os',
            'selector': 'td:not(:last-child)'
        },
        "language": {
            "url": dataTablesLang,
        },
        lengthMenu: [
            [10, 25, 50, 100],
            ['10 ', '25 ', '50 ', '100 ']
        ],
        "drawCallback": drawCallback,
        "ajax": {
            "url": url,
            "type": "POST",
            "datatype": "json",
            "data": dataPost,
            "dataSrc": dataSrcFun,
            "deferRender": true
        },
        "columns": dataHandler
    }, options);

    var oTable = this.DataTable(settings);

    oTable.on('select', selectEvent);
    return oTable;
};

$.fn.loadClientDataTable = function (url, sortColumn, columnsDefs, dataHandler, options) {

    var settings = $.extend({
        "order": [[sortColumn || 1, "asc"]],
        "autoWidth": false,
        "language": {
            "url": dataTablesLang,
        },
        lengthMenu: [
            [10, 25, 50, 100, -1],
            ['10 ', '25 ', '50 ', '100 ', 'All']
        ],
        "ajax": {
            "url": url,
            "type": "GET",
            "datatype": "json"
        },
        "columnDefs": columnsDefs,
        "columns": dataHandler
    }, options);

    var oTable = this.DataTable(settings);
    return oTable;
};

function loadReportDataTable(tableId, url, sortColumn, columnsDefs, dataHandler, tableOptions, array, checkboxName, selectAllName, rowValueId) {
    //Create DataTable
    var oTable = $("#" + tableId).loadClientDataTable(url, sortColumn,
        columnsDefs,
        dataHandler,
        tableOptions);

    $('#' + tableId + ' tbody').on('click', 'input[name="' + checkboxName + '"]', function (e) {
        var $row = $(this).closest('tr');
        // Get row data
        var data = oTable.row($row).data();

        // Get row ID
        var rowId = data[rowValueId];

        // Determine whether row ID is in the list of selected row IDs 
        var index = $.inArray(rowId, array);

        // If checkbox is checked and row ID is not in list of selected row IDs
        if (this.checked && index === -1) {
            array.push(rowId);

            // Otherwise, if checkbox is not checked and row ID is in list of selected row IDs
        } else if (!this.checked && index !== -1) {
            array.splice(index, 1);
        }

        //Handle select one one and change select all
        if (array.length == oTable.data().count()) {
            $('input[name="' + selectAllName + '"]').prop('checked', true);
        }
        else {
            $('input[name="' + selectAllName + '"]').prop('checked', false);
        }

        // Prevent click event from propagating to parent
        e.stopPropagation();
    });

    $('#' + tableId).on('click', 'tbody td, thead th:first-child', function (e) {
        $(this).parent().find('input[name="' + checkboxName + '"]').trigger('click');
    });

    // Handle click on "Select all" control
    $('thead input[name="' + selectAllName + '"]', oTable.table().container()).on('click', function (e) {
        var rows = oTable.rows({ page: 'current', filter: 'applied' }).nodes();
        $('input[name="' + checkboxName + '"]', rows).prop('checked', this.checked);
        if (!this.checked) {
            array.splice(0, array.length)
        }
        else {
            var allrows = oTable.rows({ filter: 'applied' }).data();
            for (var i = 0; i < allrows.length; i++) {
                var temprow = allrows[i];
                var rowId = temprow[rowValueId];

                var index = $.inArray(rowId, array);

                if (this.checked && index === -1) {
                    array.push(rowId);

                } else if (!this.checked && index !== -1) {
                    array.splice(index, 1);
                }
            }
        }
        // Prevent click event from propagating to parent
        e.stopPropagation();
    });

    //on page change check if checkbox checked and uniform check boxes in this page
    $('#' + tableId).on('draw.dt', function () {
        var rows = oTable.rows({ page: 'current' }).every(function (rowIdx, tableLoop, rowLoop) {
            var rowdata = this.data();
            var rowId = rowdata[rowValueId];
            var index = $.inArray(rowId, array);
            if (index === -1) {
                $('input[name="' + checkboxName + '"]', this.nodes()).prop('checked', false);

            } else if (index !== -1) {
                $('input[name="' + checkboxName + '"]', this.nodes()).prop('checked', true);
            }
        })
    });
};