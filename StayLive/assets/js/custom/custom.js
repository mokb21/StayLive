$(function () {
    $('.js-switch').each(function () {
        new Switchery($(this)[0], $(this).data());
    });

    $('.datepicker').datepicker({
        autoclose: true,
        todayHighlight: true
    });

    $('.number').mask('#');
    $('.datepicker').mask('9999/99/99');

    $(".dropify").dropify();
});

function BuildTree(elementId, url, callback, plugnis, dblClick, idsStorageField, dblClickFunc, openAll) {
    if (openAll == null)
        openAll = true;
    $.ajax({
        type: "GET",
        url: url,
        success: function (data) {
            if (data.Data != null && data.Data.length > 0) {
                $('#' + elementId).jstree({
                    'core': {
                        'strings': {
                            'Loading ...': 'Please wait ...'
                        },
                        "dblclick_toggle": false,
                        'themes': {
                            "dots": true,
                            "icons": false
                        },
                        'data': data.Data

                    },
                    "checkbox": {
                        "keep_selected_style": false,
                        "three_state": false
                    },
                    "plugins": plugnis || []
                });
            } else {
                $("#" + elementId).append('<p style="text-align: center;font-weight: 400;font-size: 20px;color: #40949F;">There is no available categories.</p >')
            }
        }
    })

    if (openAll) {
        $('#' + elementId).on('ready.jstree', function () {
            $("#" + elementId).jstree("open_all");
        });
    }

    $('#' + elementId).on('changed.jstree', callback)

    function SelectTreeChildren(node_info, select) {
        var childrens = $('#' + elementId).jstree("get_children_dom", node_info);
        if (childrens.length > 0) {
            $.each(childrens, function (idx, n) {
                if (select)
                    $('#' + elementId).jstree("select_node", n);
                else
                    $('#' + elementId).jstree("deselect_node", n);
                SelectTreeChildren(n, select)
            });
        }
    }

    if (dblClick) {
        $('#' + elementId).on("dblclick.jstree", dblClickFunc || function (event) {
            //get node list item
            var node = $(event.target).closest("li");
            //trigger click again to set node as selected or unselected
            $(event.target).trigger('click')
            //get selected nodes
            var selectedNodes = $('#' + elementId).jstree("get_selected");
            //determine if the node is selected or not
            var select = $(node).attr('id') == selectedNodes[selectedNodes.length - 1];
            //get selected node
            var node_info = $('#' + elementId).jstree("get_node", $(node).attr('id'));
            //select/unselect all children
            SelectTreeChildren(node_info, select)
            if (idsStorageField != null)
                $(idsStorageField).val($('#' + elementId).jstree("get_selected"));
        });
    }

}

function ShowDeleteAlert(url, name) {
    swal({
        title: "Are you sure?",
        text: "You will not be able to recover <strong>" + name + "</strong>",
        html: true,
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!",
        customClass: 'defaultSwal',
        cancelButtonText: "No, cancel please!",
        closeOnConfirm: false,
        closeOnCancel: false
    },
        function (isConfirm) {
            if (isConfirm) {
                window.location.href = url
            } else {
                swal({
                    title: "Cancelled",
                    text: "Your entity is safe :)",
                    type: "error",
                    customClass: 'defaultSwal',
                });
                //swal("Cancelled", "Your entity is safe :)", "error");
            }
        });
}

function ActionIcon(type, title, href, clickFun, icon, buttonClass, buttonId, html, newTab) {
    var button = $('<a class="tip btn btn-datatable m-l-10" title="' + title + '"></a>')

    $(button).attr('href', href || 'javascript:void(0)');

    if (clickFun != null && clickFun.length > 0) {
        $(button).attr('onclick', clickFun);
    }
    if (buttonId != null && buttonId != '') {
        $(button).attr('id', buttonId);
    }
    if (html != null && html != '') {
        $(button).html(html);
    }

    if (newTab) {
        $(button).attr('target', '_blank');
    }

    switch (type) {
        case "Delete":
            $(button).addClass('btn-danger');
            (button).append('<i class="mdi mdi-delete"></i>');
            break;
        case "Password":
            $(button).addClass('btn-warning');
            (button).append('<i class="mdi mdi-lock"></i>');
            break;
        default:
            $(button).addClass('btn-info');
            $(button).append('<i class="' + icon + '"></i>');
            break;
    }
    return button[0].outerHTML;
}

function ShowSaveModal(url, data, onHideFundtion) {
    $('#staylive-responsive-modal').modal({ keyboard: true }, 'show');
    $('#staylive-responsive-modal').on('hidden.bs.modal', onHideFundtion || function (e) { })
    $.ajax({
        type: "GET",
        url: url,
        data: data,
        cache: false,
        success: function (result) {
            $('#staylive-responsive-modal').html(result.data);
            $.validator.unobtrusive.parse($("#" + $('#staylive-responsive-modal .modal-content').find('form').attr("id")));

            $(".number").mask("#");

            $('.floating-labels .form-control').on('focus blur', function (e) {
                $(this).parents('.form-group').toggleClass('focused', (e.type === 'focus' || this.value.length > 0));
            }).trigger('blur');
        },
        error: function () {
            InfoMessage.error("", "Sorry, something went wrong");
        }
    });
}

function HideModal() {
    $("#fms-responsive-modal").modal('hide');
}

function GeneralAjaxFormComplete() {
    $('#fms-responsive-modal').modal('toggle');
}

function UpdateProfileImage() {
    var data = new FormData();
    var image = document.getElementById("ProfilePhoto").files.length;
    for (var i = 0; i < image; i++) {
        var file = document.getElementById("ProfilePhoto").files[i];
        data.append("ProfilePhoto", file);
    }
    $.ajax({
        type: "POST",
        url: GeneralHelper.Url("UpdateProfileImage", "Home", ""),
        data: data,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.success == true) {
                window.location.reload(true);
                //InfoMessage.success("Update profile image", "Image updated successfully");
            }
        },
        error: function () {
            //InfoMessage.error("Update profile image", "Sorry, something went wrong");
        }
    });
}

function BuildDoughnutChart(obj, data) {
    var DoughnutChart = new Chart(obj).Doughnut(data, {
        segmentShowStroke: true,
        segmentStrokeColor: "#fff",
        segmentStrokeWidth: 0,
        animationSteps: 100,
        tooltipCornerRadius: 2,
        animationEasing: "easeOutBounce",
        animateRotate: true,
        animateScale: false,
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>",
        responsive: true
    });
}

function BuildBarChart(obj, data) {
    var BarChart = new Chart(obj).Bar(data, {
        scaleBeginAtZero: true,
        scaleShowGridLines: true,
        scaleGridLineColor: "rgba(0,0,0,.005)",
        scaleGridLineWidth: 0,
        scaleShowHorizontalLines: true,
        scaleShowVerticalLines: true,
        barShowStroke: true,
        barStrokeWidth: 0,
        tooltipCornerRadius: 2,
        barDatasetSpacing: 3,
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].fillColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>",
        responsive: true
    });
}