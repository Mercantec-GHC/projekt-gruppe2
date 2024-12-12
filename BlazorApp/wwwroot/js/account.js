export function Load(firstNameVal, lastNameVal, emailVal) {
    const firstName = document.getElementById("floatingFirstName");
    const lastName = document.getElementById("floatingLastName");
    const email = document.getElementById("floatingEmail");
    const pswd1 = document.getElementById("floatingPassword1");
    const pswd2 = document.getElementById("floatingPassword2");
    const saveButton = document.getElementById("saveButton");

    firstName.addEventListener('input', function () { updateField(firstName, firstNameVal) });
    lastName.addEventListener('input', function () { updateField(lastName, lastNameVal) });
    email.addEventListener('input', updateEmailField);
    pswd1.addEventListener('input', updatePasswordFields);
    pswd2.addEventListener('input', updatePasswordFields);

    function updateField(element, original) {
        if (element.value.length > 0) {
            if (element.value == original) {
                element.classList.remove("is-invalid");
                element.classList.remove("is-valid");
            }
            else {
                element.classList.remove("is-invalid");
                element.classList.add("is-valid");
            }
        }
        else {
            element.classList.remove("is-valid");
            element.classList.add("is-invalid");
        }
        updateSaveButton();
    }

    function updateEmailField() {
        if (email.value == emailVal) {
            email.classList.remove("is-invalid");
            email.classList.remove("is-valid");
        }
        else if (isValidEmail(email.value)) {
            email.classList.remove("is-invalid");
            email.classList.add("is-valid");
        }
        else {
            email.classList.remove("is-valid");
            email.classList.add("is-invalid");
        }
        updateSaveButton();
    }

    function updatePasswordFields() {
        if (pswd1.value.length == 0 && pswd2.value.length == 0) {
            pswd1.classList.remove("is-invalid");
            pswd2.classList.remove("is-invalid");
            pswd1.classList.remove("is-valid");
            pswd2.classList.remove("is-valid");
        }
        else if (pswd1.value == pswd2.value) {
            pswd1.classList.remove("is-invalid");
            pswd2.classList.remove("is-invalid");
            pswd1.classList.add("is-valid");
            pswd2.classList.add("is-valid");
        }
        else {
            pswd1.classList.remove("is-valid");
            pswd2.classList.remove("is-valid");
            pswd1.classList.add("is-invalid");
            pswd2.classList.add("is-invalid");
        }
        updateSaveButton();
    }

    function isValidEmail(email) {
        return String(email)
            .toLowerCase()
            .match(/^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);
    }

    function updateSaveButton() {
        if (pswd1.value == pswd2.value && firstName.value.length > 0 && lastName.value.length > 0 && isValidEmail(email.value) &&
            ((pswd1.value.length > 0) || firstName.value != firstNameVal || lastName.value != lastNameVal || email.value != emailVal)) {
            saveButton.disabled = false;
            saveButton.classList.remove("btn-secondary");
            saveButton.classList.add("btn-primary");
        }
        else {
            saveButton.disabled = true;
            saveButton.classList.remove("btn-primary");
            saveButton.classList.add("btn-secondary");
        }
    }
}

export function DisableSubmit() {
    const saveButton = document.getElementById("saveButton");
    saveButton.disabled = true;
}