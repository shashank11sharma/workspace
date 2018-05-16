//  When invoked open the modal dialog.
//  We will call this function from the Web Part's page.
 


 

 

function popupmodaluiNewForm(url, Title) {
    //document.getElementById("<%=LinkButton1.ClientID %>")
    //document.getElementById('<%= LinkButton1.ClientID %>').click();
   
    // Set the required properties.
    var options = { autoSize: true,
        title: Title,
        showClose: true,
        allowMaximize: false
    };
    // Pop up the application page in the modal window, 
    // and pass the site url as a query string to the application page.
    SP.UI.ModalDialog.commonModalDialogOpen(url, options, closecallbackAttach, null); // Updated By Ujjwal 
}


 

 


//for edit properties winow//
function popupmodaluiNew(url) {
  
    // Set the required properties.
    var options = { autoSize: true,
        title: "Edit Properties",
        showClose: true,
        allowMaximize: false
    };
    // Pop up the application page in the modal window, 
    // and pass the site url as a query string to the application page.
    SP.UI.ModalDialog.commonModalDialogOpen(url, options, closecallback, null);
}


//for edit properties winow//
function popupmodaluiNewAttach(url) {

    // Set the required properties.
    var options = { autoSize: true,
        title: "Edit Properties",
        showClose: true,
        allowMaximize: false
    };
    // Pop up the application page in the modal window, 
    // and pass the site url as a query string to the application page.
    SP.UI.ModalDialog.commonModalDialogOpen(url, options, closecallbackAttachment, null);
}

 



function popupmodaluiReturnValue1(url, reportTitle) {

    // Set the required properties.
    var options = { autoSize: true,
        title: reportTitle,
        showClose: true,
        allowMaximize: false,
        dialogReturnValueCallback: portal_modalDialogClosedCallback1
    };
    // Pop up the application page in the modal window, 
    // and pass the site url as a query string to the application page.

    SP.UI.ModalDialog.commonModalDialogOpen(url, options, portal_modalDialogClosedCallback1, null);
    return false;
}
function popupmodaluiReturnValue(url, reportTitle) {

    // Set the required properties.
    var options = { autoSize: true,
        title: reportTitle,
        showClose: true,
        allowMaximize: false,
        dialogReturnValueCallback: portal_modalDialogClosedCallback
    };
    // Pop up the application page in the modal window, 
    // and pass the site url as a query string to the application page.

    SP.UI.ModalDialog.commonModalDialogOpen(url, options, portal_modalDialogClosedCallback, null);
    return false;
}


function popupmodaluiNewReports(url, reportTitle) {
   
    // Set the required properties.
    var options = { autoSize: true,
        title: reportTitle,
        showClose: true,
        allowMaximize: false
    };
    // Pop up the application page in the modal window, 
    // and pass the site url as a query string to the application page.
    SP.UI.ModalDialog.commonModalDialogOpen(url, options, closecallback, null);
    return false;
}

// Handles the click event for OK button on the modal dialog.
// This function runs in the context of the application page.
function ModalOk_click() {
    // Get the value of the hidden textbox on the modal dialog.
    var value = getValueByClass('.modalhiddenfield');
    // Pass the hidden textbox value to the callback and close the modal dialog.
    SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.OK, value);
}

// Handles the click event for the Cancel button on the modal dialog.
function ModalCancel_click() {
    // Set the dialog result property to Cancel and close the modal dialog.
    SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.cancel, 'Cancel clicked');
}

// Executes when the modal dialog is closed.
// This function runs in the context of the Web Part's page.
function closecallback(result, value) {
    // Check if OK button was clicked.
    if (result === SP.UI.DialogResult.OK) {
        // Set the value of the hidden textbox on the web part 
        // with the value passed by the OK button event.
        var ispostback = setValue('.webparthiddenfield', value);
        if (ispostback == true) {
            // Postback the page so the web part lifecycle is reinitiated.
            postpage();
            //document.getElementById('<%=lnkResolve.ClientID%>').click();
        }
    }
}

// Finds a control by css class name and retrieves its value.
function getValueByClass(className) {
    formtextBox = $(className);
    if (formtextBox != null) {
        return formtextBox.val();
    }
}

// Finds a control by css class name and sets its value.
// This function runs in the context of the Web Part's page.
function setValue(className, value) {
    hiddenfieldid = $(className);
    if (hiddenfieldid != null) {
        hiddenfieldid.val(value);
        /*For testing the modal dialog with the CEWP. Can be removed.*/
        if (hiddenfieldid.css('visibility') == "visible") {
            return false;
        }
        /*--*/
        return true;
    }
}

// Check if the hidden text box on the modal dialog is empty.
function checkTextChange() {
    value = jQuery.trim(getValueByClass('.modalhiddenfield'));
    // Enable the OK button on the modal window if the hidden textbox has a value.
    if (value) {
        $('#btnModalOK').removeAttr('disabled');
    }
    // Disable the OK button on the modal window if the hidden textbox does not have a value.
    else {
        $('#btnModalOK').attr("disabled", "true");
    }
}

// Postback the page to reinitiate the web part life cycle.
function postpage() {
    document.forms[0].submit();
}

// Look for a change every time the page is loaded.
$(document).ready(function () {
    checkTextChange();
});