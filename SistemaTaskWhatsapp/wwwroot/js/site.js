// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const btn = document.getElementById("toggleDarkMode");

btn.addEventListener("click", () => {
    document.body.classList.toggle("dark-mode");

    // guardar preferencia
    localStorage.setItem(
        "darkMode",
        document.body.classList.contains("dark-mode")
    );
});

// cargar preferencia
if (localStorage.getItem("darkMode") === "true") {
    document.body.classList.add("dark-mode");
}
