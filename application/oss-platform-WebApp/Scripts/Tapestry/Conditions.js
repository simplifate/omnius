ConditionData = [];

ConditionsRelationList = ["AND", "OR", "XOR"];

ConditionsOperatorList = ["=", "!=", ">", ">=", "<", "<="];

FakeInputsForTesting = [
    "Username.Value",
    "Password.Value",
    "LogIn.Success",
    "LogIn.AttemptCount",
    "LogIn.UserGroup",
    "Global.BlockReferal",
    "Global.BlockStartTimestamp"
]

function FillConditionsForLogicTableRow(row) {
    row.find(".selectRelation option").remove();
    for (i = 0; i < ConditionsRelationList.length; i++) {
        row.find(".selectRelation").append(
            $('<option value="' + ConditionsRelationList[i] + '">' + ConditionsRelationList[i] + '</option>'));
    }
    row.find(".selectField option").remove();
    for (i = 0; i < FakeInputsForTesting.length; i++) {
        row.find(".selectField").append(
            $('<option value="' + FakeInputsForTesting[i] + '">' + FakeInputsForTesting[i] + '</option>'));
    }
    row.find(".selectOperator option").remove();
    for (i = 0; i < ConditionsOperatorList.length; i++) {
        row.find(".selectOperator").append(
            $('<option value="' + ConditionsOperatorList[i] + '">' + ConditionsOperatorList[i] + '</option>'));
    }
}
