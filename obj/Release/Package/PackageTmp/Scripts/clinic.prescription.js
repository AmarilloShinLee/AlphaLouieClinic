var count = 1;
var oldCode;
var selectId;

var addRow = function () {
    addTableRow($("#prescriptionTable"));
    return false;
};

var deleteRow = function (event) {
    $(event.target).closest("tr").remove();
    return false;
};

var getSelectID = function (event) {
    return "#" + event.target.id;
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
    $tr.find("select").click(getSelectID);

    // reset fields in the new row
    $tr.find("select").val("");
    $tr.find("input").val("");
    //$tr.find(".row_num").val(++count);
    //$tr.find(".billing_amount").val(0);

    $(table).find("tbody tr:last").after($tr);

    console.log(selectId);

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

$(document).ready(function () {
    var rowCount = $("table > tbody tr").length;
    var rowCodeArr = new Array();

    oldCode = $("#IsUpdate").val();

    console.log(oldCode);

    //if update
    if (oldCode > 0) {
        $('table > tbody tr').each(function (index, tr) {
            var rowCode = $(tr).find(".detailCode").val();

            $(tr).find("select").val(rowCode);

            rowCodeArr.push(rowCode);

            if (index < rowCount - 1)
                $(tr).find(".new_row").attr("class", "remove_row btn btn-danger").attr("title", "Delete").text("Delete").unbind("click").click(deleteRow);
        });

        $(rowCodeArr).each(function (index, tr) {
            console.log(tr)
        });

        console.log(rowCount);
    }
});

$(function () {
    $(".new_row").click(addRow);
    $(".remove_row").click(deleteRow);
    $("input[value='Submit']").click(submitForm);
    $("input[value='Clear']").click(function () {
        $("#PresHCode").val("");
        $("#PresHConsNo").val("");
        $("#PresHPatCode").val("");
        $("#PresHDate").val("");
        ClearRows();
    });
})

function ClearRows() {
    var rows = $('table > tbody  > tr').length;

    $('table > tbody  > tr').each(function (index, tr) {
        if (index != (rows - 1))
            $(tr).remove();
        else {
            $(tr).find(".detail_code").val("");
            $(tr).find(".detail_quantity").val("");
            $(tr).find(".detail_remarks").val("");
        }
    });

    return false;
}

var submitForm = function (e) {
    e.preventDefault();

    /*Get Data*/
    var PrescriptionHeader = {}
    PrescriptionHeader.PresHCode = $("#PresHCode").val();
    PrescriptionHeader.PresHConsNo = $("#PresHConsNo").val();
    PrescriptionHeader.PresHPatCode = $("#PresHPatCode").val();
    PrescriptionHeader.PresHDate = $("#PresHDate").val();
    PrescriptionHeader.OldCode = $("#IsUpdate").val();
    PrescriptionHeader.PrescriptionDetails = new Array();

    $('table > tbody  > tr').each(function (index, tr) {
        var presDMedCode = $(tr).find(".detail_code").val();
        var presDQuantity = $(tr).find(".detail_quantity").val();
        var presDRemarks = $(tr).find(".detail_remarks").val();

        var PrescriptionDetailFile = {};
        PrescriptionDetailFile.PreDMedCode = presDMedCode;
        PrescriptionDetailFile.PreDQty = presDQuantity;
        PrescriptionDetailFile.PreDRemarks = presDRemarks;
        PrescriptionHeader.PrescriptionDetails.push(PrescriptionDetailFile);
    });

    console.log("Code: " + PrescriptionHeader.PresHCode);
    console.log("Consultation: " + PrescriptionHeader.PresHConsNo);
    console.log("Patient Code: " + PrescriptionHeader.PresHPatCode);
    console.log("Date: " + PrescriptionHeader.PresHDate);

    $(PrescriptionHeader.PrescriptionDetails).each(function (index, row) {
        console.log("Medicine Code: " + row.PreDMedCode);
        console.log("Medicine Qty: " + row.PreDQty);
        console.log("Medicine Remarks: " + row.PreDRemarks);
    });

    $.ajax({
        async: true,
        type: 'POST',
        dataType: 'JSON',
        contentType: 'application/json; charset=utf=8',
        url: '/Home/Prescription',
        data: JSON.stringify(PrescriptionHeader),
        success: function (response) {
            if (response.success) {

                if (oldCode > 0)
                    alert(response.message);
                else {
                    alert(response.message);
                    $("#PresHCode").val("");
                    $("#PresHConsNo").val("");
                    $("#PresHPatCode").val("");
                    $("#PresHDate").val("");
                    ClearRows();
                }
            }
            else {
                alert(response.message);
            }
        }
    });
}

                //To be implemented
                //$(document).ready(function () {
                //    $("select").on("change", function (event) {
                //        /*console.log($(this).val());*/
                //        $.ajax(
                //            {
                //                url: '/Home/GetMedicineName?medicineCode=' + $(this).val(),
                //                type: 'GET',
                //                data: "",
                //                contentType: 'application/json; charset=utf-8',
                //                success: function (data) {
                //                    console.log(data);
                //                    $("#prescription_name").html(data);
                //                },
                //                error: function () {
                //                    alert("error");
                //                }
                //            });
                //    });
                //});