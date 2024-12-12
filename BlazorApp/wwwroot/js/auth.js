export function SignIn(email, password, remember, redirect) {
    let loginSpinner = document.getElementById("loginSpinner");
    let loginText = document.getElementById("loginText");

    loginText.innerHTML = "Login in...";
    loginSpinner.classList.remove("d-none");

    let url = "/api/auth/signin";
    let xhr = new XMLHttpRequest();

    // Initialization
    xhr.open("POST", url);
    xhr.setRequestHeader("Accept", "application/json");
    xhr.setRequestHeader("Content-Type", "application/json");

    // Catch response
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) // 4=DONE 
        {
            console.log("Call '" + url + "'. Status " + xhr.status);
            if (xhr.status == 200) {
                if (redirect)
                    location.replace(redirect);
            }
            else if (xhr.status == 400) {
                ShowLoginFlash();
            }
            loginText.innerHTML = "Log in";
            loginSpinner.classList.add("d-none");
        }
    };

    // Data to send
    let data = {
        email: email,
        password: password,
        remember: remember
    };

    // Call API
    xhr.send(JSON.stringify(data));
};

export function SignOut(redirect) {
    let url = "/api/auth/signout";
    let xhr = new XMLHttpRequest();

    // Initialization
    xhr.open("POST", url);
    xhr.setRequestHeader("Accept", "application/json");
    xhr.setRequestHeader("Content-Type", "application/json");

    // Catch response
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) // 4=DONE 
        {
            console.log("Call '" + url + "'. Status " + xhr.status);
            if (redirect)
                location.replace(redirect);
        }
    };

    // Call API
    xhr.send();
};

export function Register(email, firstName, lastName, username, password) {
    let registerSpinner = document.getElementById("registerSpinner");
    let registerText = document.getElementById("registerText");

    registerText.innerHTML = "Signing up...";
    registerSpinner.classList.remove("d-none");

    let url = "/api/auth/register";
    let xhr = new XMLHttpRequest();

    // Initialization
    xhr.open("POST", url);
    xhr.setRequestHeader("Accept", "application/json");
    xhr.setRequestHeader("Content-Type", "application/json");

    // Catch response
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) // 4=DONE 
        {
            console.log("Call '" + url + "'. Status " + xhr.status);
            if (xhr.status == 200) {
                location.replace("/login");
            }
            else if (xhr.status == 400) {
                ShowRegisterFlash();
            }
            registerText.innerHTML = "Sign up";
            registerSpinner.classList.add("d-none");
        }
    };

    // Data to send
    let data = {
        email: email,
        firstName: firstName,
        lastName: lastName,
        username: username,
        password: password
    };

    // Call API
    xhr.send(JSON.stringify(data));
}

export function GetInputValue(id) {
    const input = document.getElementById(id)
    return input.value;
};

export function IsChecked(id) {
    const input = document.getElementById(id)
    return input.checked;
};

export function ShowLoginFlash() {
    const loginFlashMessage = document.getElementById('loginFlashMessage')
    loginFlashMessage.style.display = "block";
};

export function ShowRegisterFlash() {
    const loginFlashMessage = document.getElementById('registerFlashMessage')
    loginFlashMessage.style.display = "block";
};