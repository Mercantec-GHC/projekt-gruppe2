function changeTheme(theme) {
    const icon = document.getElementById("theme_icon");
    icon.classList.remove("bi-circle-half", "bi-sun-fill", "bi-moon-stars-fill", "bi-lightbulb-off-fill");

    setCookie("theme", theme, 400);
    updateTheme();

    if (theme == "light") icon.classList.add("bi-sun-fill");
    else if (theme == "dark") icon.classList.add("bi-moon-stars-fill");
    else if (theme == "oled") icon.classList.add("bi-lightbulb-off-fill");
    else icon.classList.add("bi-circle-half");
}

function updateTheme() {
    theme = getCookie("theme");
    if (theme == "light") {
        document.querySelector("html").setAttribute("data-bs-theme", "light")
    }
    else if (theme == "dark") {
        document.querySelector("html").setAttribute("data-bs-theme", "dark")
    }
    else if (theme == "oled") {
        document.querySelector("html").setAttribute("data-bs-theme", "oled")
    }
    else {
        document.querySelector("html").setAttribute("data-bs-theme",
            window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light")
    }
}

function setCookie(cname, cvalue, exdays) {
    const d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    let name = cname + "=";
    let ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

;(function () {
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', updateTheme);
    updateTheme();
})()
