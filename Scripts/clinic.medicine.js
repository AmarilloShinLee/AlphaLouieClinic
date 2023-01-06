var oldCode;

$(document).ready(function () {
    oldCode = $("#IsUpdate").val();

    console.log(oldCode);
});

$(function () {
    $("#clearForm").click(function () {

        $("input[type='text']").val("");
        $("input[type='number']").val(0);

        console.log("Cleared!");
    });

    $("#submitForm").click(function (e) {
        e.preventDefault();

        /*Get Data*/
        var MedicineFile = {}
        MedicineFile.MedCode = $("#MedCode").val();
        MedicineFile.MedName = $("#MedName").val();
        MedicineFile.MedDose = $("#MedDose").val();
        MedicineFile.MedDesc = $("#MedDesc").val();
        MedicineFile.OldCode = oldCode;

        console.log(MedicineFile.MedCode);
        console.log(MedicineFile.MedName);
        console.log(MedicineFile.MedDose);
        console.log(MedicineFile.MedDesc);

        $.ajax({
            async: true,
            type: 'POST',
            dataType: 'JSON',
            contentType: 'application/json; charset=utf=8',
            url: '/Home/Medicine',
            data: JSON.stringify(MedicineFile),
            success: function (response) {
                if (response.success) {

                    if (oldCode > 0)
                        alert(response.message);
                    else {
                        alert(response.message);

                        $("input[type='text']").val("");
                        $("input[type='number']").val(0);
                    }
                }
                else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("Error. Something is wrong. Unable to add/update data");
            }
        });

    });
})