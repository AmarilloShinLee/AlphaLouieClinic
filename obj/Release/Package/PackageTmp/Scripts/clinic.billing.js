var count = 1;
var total = 0;
var oldCode;

$(document).ready(function () {
    var rowLength = $("table > tbody tr").length;
    var rowCodeArr = new Array();

    oldCode = $("#IsUpdate").val();

    $("table > tbody tr:first-child").find("input[readonly='readonly']").val(1);

    //if update
    if (oldCode > 0) {
        count = rowLength;

        $('table > tbody tr').each(function (index, tr) {
            var rowCount = $(tr).find(".billDescription").val();

            $(tr).find(".billing_description").val(rowCount);
            rowCodeArr.push(rowCount);

            if (index < rowLength - 1)
                $(tr).find(".new_row").attr("class", "remove_row btn btn-danger").attr("title", "Delete").text("Delete").unbind("click").click(deleteRow);
        });

        $(rowCodeArr).each(function (index, i) {
            console.log(i)
        });
    }
});


var addRow = function () {
    var iniTotal = 0;

    addTableRow($("#billing_table"));

    $('table > tbody  > tr').each(function (index, tr) {
        iniTotal += parseFloat($(tr).find(".billing_amount").val());
    });

    total = iniTotal;

    $(".billing_total_amount").val(total.toFixed(2));

    return false;
};

var submitForm = function (e) {
    e.preventDefault();

    /*Total Amount*/
    var iniTotal = 0;

    $('table > tbody  > tr').each(function (index, tr) {
        iniTotal += parseFloat($(tr).find(".billing_amount").val());
    });

    total = iniTotal;

    $(".billing_total_amount").val(total.toFixed(2));

    /*Get Data*/
    var BillingHeaderFile = {}
    BillingHeaderFile.BillHNo = $("#BillHNo").val();
    BillingHeaderFile.BillHDate = $("#BillHDate").val();
    BillingHeaderFile.BillHPatCode = $("#BillHPatCode").val();
    BillingHeaderFile.OldCode = $("#IsUpdate").val();
    BillingHeaderFile.BillingDetails = new Array();

    $('table > tbody  > tr').each(function (index, tr) {
        var billDCount = $(tr).find(".row_num").val();
        var billDDesc = $(tr).find(".billing_description").val();
        var billDAmount = $(tr).find(".billing_amount").val();

        var BillingDetailFile = {};
        BillingDetailFile.BillDCount = billDCount;
        BillingDetailFile.BillDDesc = billDDesc;
        BillingDetailFile.BillDAmount = billDAmount;
        BillingHeaderFile.BillingDetails.push(BillingDetailFile);
    });

    BillingHeaderFile.BillHTotAmt = $("#BillHTotAmt").val();

    console.log(BillingHeaderFile.BillHNo);
    console.log(BillingHeaderFile.BillHDate);
    console.log(BillingHeaderFile.BillHPatCode);

    $(BillingHeaderFile.BillingDetails).each(function (index, row) {
        console.log(row.BillDCount);
        console.log(row.BillDDesc);
        console.log(row.BillDAmount);
    });

    console.log(BillingHeaderFile.BillHTotAmt);

    $.ajax({
        async: true,
        type: 'POST',
        dataType: 'JSON',
        contentType: 'application/json; charset=utf=8',
        url: '/Home/Billing',
        data: JSON.stringify(BillingHeaderFile),
        success: function (response) {
            if (response.success) {
                if (oldCode > 0)
                    alert(response.message);
                else {
                    alert(response.message);
                    $("#BillHNo").val("");
                    $("#BillHDate").val("");
                    $("#BillHPatCode").val("");
                    ClearRows();
                    $("#BillHTotAmt").val("");
                }
            }
            else {
                alert(response.message);
            }
        }
    });
}

function ClearRows() {
    var rows = $('table > tbody  > tr').length;

    count = 1;

    $('table > tbody  > tr').each(function (index, tr) {
        if (index != (rows - 1))
            $(tr).remove();
        else {
            $(tr).find(".row_num").val(count);
            $(tr).find(".billing_description").val("");
            $(tr).find(".billing_amount").val("");
        }
    });

    return false;
}

var deleteRow = function (event) {
    var rowIndex = $(event.target).closest("tr").index();
    var deleteAmount = $(event.target).closest("tr").find(".billing_amount").val();

    $(event.target).closest("tr").remove();

    $('table > tbody  > tr').each(function (index, tr) {
        if (index >= rowIndex) {
            var countNum = $(tr).find(".row_num").val();
            $(tr).find(".row_num").val(--countNum);
        }
    });

    total -= deleteAmount;
    $(".billing_total_amount").val(total.toFixed(2));

    --count;
    return false;
};

function addTableRow(table) {
    /* Sources:
    http://www.simonbingham.me.uk/index.cfm/main/post/uuid/adding-a-row-to-a-table-containing-form-fields-using-jquery-18
    http://stackoverflow.com/questions/5104288/adding-validation-with-mvc-3-jquery-validator-in-execution-time
    */

    /* Start of Cloning */
    var $ttc = $(table).find("tbody tr:last");
    var $tr = $ttc.clone();

    $tr.find("input,select").attr("name", function () { // find name in the cloned row
        var parts = this.id.match(/(\D+)_(\d+)__(\D+)$/); // extract parts from id, including index
        return parts[1] + "[" + ++parts[2] + "]." + parts[3]; // build new name
    }).attr("id", function () { // change id also
        var parts = this.id.match(/(\D+)_(\d+)__(\D+)$/); // extract parts
        return parts[1] + "_" + ++parts[2] + "__" + parts[3]; // build new id
    });

    $tr.find("span[data-valmsg-for]").attr("data-valmsg-for", function () { // find validation message
        var parts = $(this).attr("data-valmsg-for").match(/(\D+)\[(\d+)]\.(\D+)$/); // extract parts from the referring attribute
        return parts[1] + "[" + ++parts[2] + "]." + parts[3]; // build new value
    })
    $ttc.find(".new_row").attr("class", "remove_row btn btn-danger").attr("title", "Delete").text("Delete").unbind("click").click(deleteRow); // change buttin function
    $tr.find(".new_row").click(addRow); // add function to the cloned button

    // reset fields in the new row
    //$tr.find("select").val("");
    $tr.find(".row_num").val(++count);
    $tr.find(".billing_amount").val(0);

    // add cloned row as last row
    $(table).find("tbody tr:last").after($tr);
    /* End of Cloning*/

    // Find the affected form
    var $form = $tr.closest("FORM");

    // Unbind existing validation
    $form.unbind();
    $form.data("validator", null);

    // Check document for changes
    $.validator.unobtrusive.parse(document);

    // We could re-validate with changes
    // $form.validate($form.data("unobtrusiveValidation").options);
};

$(function () {
    $(".new_row").click(addRow);
    $(".remove_row").click(deleteRow);
    $("input[value='Clear']").click(function () {
        $("#BillHNo").val("");
        $("#BillHDate").val("");
        $("#BillHPatCode").val("");
        ClearRows();
        $("#BillHTotAmt").val("");
    });
    $("input[value='Submit']").click(submitForm);
})