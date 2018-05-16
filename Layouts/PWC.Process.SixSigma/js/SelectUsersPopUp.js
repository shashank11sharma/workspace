function filterUserList() {
    $("[id*='LB_MainSelection']")[0].length = 0;
    var length = $("[id*='LB_CentralData']")[0].length;
    for (var i = 0; i < length; i++) {
        if ($("[id*='LB_CentralData']")[0][i].text.toLowerCase().indexOf($("[id*='search_TB']")[0].value.toLowerCase()) > -1 || $("[id*='LB_CentralData']")[0][i].value.toLowerCase().indexOf($("[id*='search_TB']")[0].value.toLowerCase()) > -1) {
            var option = document.createElement("option");
            option.text = $("[id*='LB_CentralData']")[0][i].text;
            option.value = $("[id*='LB_CentralData']")[0][i].value;
            $("[id*='LB_MainSelection']")[0].add(option);
        }
    }
    filterMainSelection();
}

function addUsers(action, culture) {
    $("[id*='ErrorLabel']").text("");
    var length = $("[id*='LB_SelectedUserList']")[0].length;


    var value = $("[id*='LB_MainSelection']").val();


    //var value = $("[id*='LB_MainSelection']").val();
    if (value == null) {
        // alert("Please select user.")
        if (action != "yes") {
            if (culture == 1) {
                $("[id*='ErrorLabel']").text("Error: Please select user.");
            }
            else if (culture == 2) {
                $("[id*='ErrorLabel']").text("Erreur: sélectionnez l'utilisateur.");
            }


            return false;
        }
    }
    else if (length > 0) {
        // alert("You can not select more than one user.")
        if (culture == 1) {
            $("[id*='ErrorLabel']").text("Error: You can not select more than one user.");
        }
        else if (culture == 2) {
            $("[id*='ErrorLabel']").text("Erreur: Vous ne pouvez pas sélectionner plus d'un utilisateur.");
        }

        return false;
    }

    var selectedData = $("[id*=LB_MainSelection] option:selected");
    selectedData.each(function () {
        var option = document.createElement("option");
        option.text = $(this).html();
        option.value = $(this).val();
        $("[id*='LB_SelectedUserList']")[0].add(option);
    });

    filterMainSelection();
    $("[id*='search_TB']")[0].value = "";
    filterUserList();
    return false;
}


function addExternalUsers() {
    if ($("[id*='TB_External']")[0].value !== "") {
        var option = document.createElement("option");
        option.text = "[External] " + $("[id*='TB_External']")[0].value;
        option.value = $("[id*='TB_External']")[0].value;
        $("[id*='LB_SelectedUserList']")[0].add(option);
        $("[id*='TB_External']")[0].value = "";
    }
    return false;
}
function removeUsers(action, culture) {
    $("[id*='ErrorLabel']").text("");
    var value = $("[id*='LB_SelectedUserList']").val();
    if (value == null) {
        if (culture == 1) {
            $("[id*='ErrorLabel']").text("Error: Please select user.");
        }
        else if (culture == 2) {
            $("[id*='ErrorLabel']").text("Erreur: sélectionnez l'utilisateur.");
        }
        return false;
    }
    else {
        var selectedData = $("[id*=LB_SelectedUserList] option:selected");
        selectedData.each(function () {
            $("[id*=LB_SelectedUserList]")[0].remove($(this).index());
        });
        var remove = "yes";
        addUsers(remove);
        return false;
    }
}

function filterMainSelection() {
    var length = $("[id*='LB_SelectedUserList']")[0].length;
    for (var i = 0; i < length; i++) {
        var mainListLength = $("[id*='LB_MainSelection']")[0].length;
        for (var j = 0; j < mainListLength; j++) {
            if ($("[id*='LB_MainSelection']")[0][j].value === $("[id*='LB_SelectedUserList']")[0][i].value) {
                $("[id*='LB_MainSelection']")[0].remove(j);
                break;
            }
        }
    }
}

function setUserValues() {
    var qrStr = window.location.search;

    var accountNameControl = $("[id*='HiddenField1']");
    var displayNameControl = $("[id*='HiddenField2']");

    var displayNameList = "";
    var accountNameList = "";
    var length = $("[id*='LB_SelectedUserList']")[0].length;
    if (length > 1) {
        $("[id*='ErrorLabel']").text("Error: You can not select more than one user.");
        //  alert("You can not select more than one user.")
        return false;
    }
    else if (length == 0) {

        $("[id*='ErrorLabel']").text("Error: Please select user.");
        //alert("Please select user.")
        return false;
    }
    else {

        for (var i = 0; i < length; i++) {
            displayNameList += $("[id*='LB_SelectedUserList']")[0][i].text + "=";
            accountNameList += $("[id*='LB_SelectedUserList']")[0][i].value + "=";

        }

        document.getElementById(hdn1).value = accountNameList.substring(0, accountNameList.length - 1);
        document.getElementById(hdn2).value = displayNameList.substring(0, displayNameList.length - 1);

        SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.OK, accountNameList.substring(0, accountNameList.length - 1));


    }

}

function prePopulateUsers() {

    filterMainSelection();
}