let gotoRegistrationButton = document.getElementById("goto-registration-button");
gotoRegistrationButton.onclick = () => {
    let authorizationForm = document.getElementById("authorization-form");
    authorizationForm.classList.add("flipped");
}

let gotoAuthorizationButton = document.getElementById("goto-authorization-button");
gotoAuthorizationButton.onclick = () => {
    let authorizationForm = document.getElementById("authorization-form");
    authorizationForm.classList.remove("flipped");
}

let authorizationButton = document.getElementById("authorization-button");
authorizationButton.onclick = async () => {
    let login = document.getElementById("af-login-input").value;
    let password = document.getElementById("af-password-input").value;

    let response = await fetch("/authorization", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            login: login,
            password: password
        })
    });

    if (response.status === 200) {
        sessionStorage.setItem("login", login);
        sessionStorage.setItem("password", password);
    
        window.location.href = "/task-list.html";
    }
    else {
        alert("Неверное имя пользователя или пароль");
    }
}

let registrationButton = document.getElementById("registration-button");
registrationButton.onclick = async () => {
    let login = document.getElementById("rf-login-input").value;
    let password = document.getElementById("rf-password-input").value;

    let response = await fetch("/registration", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            login: login,
            password: password
        })
    });

    if (response.status === 200) {
        sessionStorage.setItem("login", login);
        sessionStorage.setItem("password", password);
    
        window.location.href = "/task-list.html";
    }

    if (response.status === 400) {
        alert("Некорректное имя пользователя или пароль");
    }

    if (response.status === 409) {
        alert(`Логин ${login} уже занят`);
    }
}