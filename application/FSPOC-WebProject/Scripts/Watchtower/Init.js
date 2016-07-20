$(function () {
    $("#filterSelectType").val($("#previousSearchType").val());
    $("#filterSelectLevel").val($("#previousSearchLevel").val());
    $("#filterSelectSource").val($("#previousSearchSource").val());
    $("#filterSelectUser").val($("#previousSearchUser").val());
    $("#filterSearchMessage").val($("#previousSearchMessage").val());
    $("#filterSearchTimeFrom").val($("#previousSearchTimeFrom").val());
    $("#filterSearchTimeTo").val($("#previousSearchTimeTo").val());

    $("#filterSearchTimeFrom,#filterSearchTimeTo").datetimepicker({
        datepicker: true, 
        timepicker: true, 
        step: 5, 
        format: "d-m-Y H:i"
       
    }); 

});
