let login = sessionStorage.getItem("login");
let password = sessionStorage.getItem("password");

let taskList = document.getElementById("task-list");

onLoad();

async function onLoad() {
    let response = await fetch("/task-list", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            login: login,
            password: password
        })
    });
    
    if (response.status === 200) {
        taskList.innerHTML = await response.text();
    }
    else {
        alert("Не удалось вывести список задач");
    }
}

async function addTask() {
    let description = document.getElementById("description-input").value;

    let response = await fetch("/add-task", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            login: login,
            password: password,
            description: description
        })
    });

    if (response.status === 200) {
        taskList.innerHTML = await response.text();
    }
    else {
        alert("Не удалось добавить задачу");
    }
}

async function changeStatus(taskId) {
    let response = await fetch("/change-status", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            login: login,
            password: password,
            taskId: taskId
        })
    });

    if (response.status === 200) {
        taskList.innerHTML = await response.text();
    }
    else {
        alert("Не удалось изменить статус");
    }
}

async function deleteTask(taskId) {
    let response = await fetch("/delete-task", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            login: login,
            password: password,
            taskId: taskId
        })
    });

    if (response.status === 200) {
        taskList.innerHTML = await response.text();
    }
    else {
        alert("Не удалось удалить задачу");
    }
}