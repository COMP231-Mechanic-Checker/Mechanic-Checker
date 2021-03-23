// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function filterRecords(e) {
   // alert('yes');
    var catVal = $('#ddl_categories').val();
    $.ajax({
        url: '/home/SearchCategories',
        type: 'POST',
        data: {
            'query': catVal,

        },
        //contentType: 'application/json; charset=utf-8',
        success: function (data) {
            alert(data.success);
        },
        error: function () {
            //alert("error");
        }
    });

}
// Write your JavaScript code.
