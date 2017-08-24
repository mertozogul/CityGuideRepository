function displayErrorMessage(message) {
    // Display an error toast, with a title
    toastr.error(message)
}

function displayWarningMessage(message) {
    // Display a warning toast, with no title
    toastr.warning(message)
}

function displayInfoMessage(message) {
    //alert('yes');
    // Display a info toast, with no title
    toastr.info(message)
}

function displaySuccessMessage(message) {
    // Display a success toast, with a title
    toastr.success(message)
}