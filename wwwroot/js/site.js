// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function isNew() {
    if (document.getElementById("Id").value && document.getElementById("Id").value != 0)
        return false;

    return true;
}

function isNameValid() {
    if (document.getElementById("Name").value.length > 0) {
        this.unflagElement("NameWrapper");
        return true;
    }
    
    this.flagElement("NameWrapper");
    return false;
}

function hasUpload() {
    if (document.getElementById("Image").value) {
        this.unflagElement("ImageWrapper");
        return true;
    }

    this.flagElement("ImageWrapper");
    return false;
}

function isEditFormValid() {
    if (!this.isNameValid())
        return false;

    if (this.isNew() && !this.hasUpload())
        return false;

    return true;
}

function disableSave() {
    document.getElementById("saveButton").setAttributeNode(document.createAttribute("disabled"));
}
function enableSave() {
    document.getElementById("saveButton").removeAttribute("disabled");
}

function flagElement(id) {
    document.getElementById(id).classList.add("invalid");
}
function unflagElement(id) {
    document.getElementById(id).classList.remove("invalid");
}

function validateEdit() {
    if (this.isEditFormValid())
        this.enableSave();
    else {
        this.setTimeout(this.validateEdit, 2000);
        this.disableSave();
    }
}

function isUserNameValid() {
    if (document.getElementById("Username").value.length > 0) {
        this.unflagElement("UsernameWrapper");
        return true;
    }

    this.flagElement("UsernameWrapper");
    return false;
}

function passwordsMatch() {
    if (document.getElementById("Password").value == document.getElementById("ConfirmPassword").value) {
        this.unflagElement("ConfirmPasswordWrapper");
        return true;
    }

    this.flagElement("ConfirmPasswordWrapper");
    return false;
}

function isPasswordValid() {
    if (document.getElementById("Password").value.length > 0) {
        this.unflagElement("PasswordWrapper");
        return true;
    }

    this.flagElement("PasswordWrapper");
    return false;
}

function isRegistrationFormValid() {
    if (this.isUserNameValid() && this.isPasswordValid() && this.passwordsMatch())
        return true;

    return false;
}

function validateRegistration() {
    if (this.isRegistrationFormValid())
        this.enableSave();
    else {
        this.setTimeout(this.validateRegistration, 2000);
        this.disableSave();
    }
}