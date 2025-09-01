document.addEventListener("DOMContentLoaded", function () {
    const toggle = document.getElementById('themeToggle');
    const icon = document.getElementById('themeIcon');
    const root = document.documentElement;

    const sidebarToggleBtn = document.getElementById("sidebar-toggle-btn");
    const wrapper = document.getElementById("wrapper");

    // ⬇️ Restaurar estado del sidebar desde localStorage
    if (localStorage.getItem("sidebar") === "hidden") {
        wrapper.classList.add("toggled");
    }

    // ⬇️ Restaurar modo oscuro desde localStorage
    if (localStorage.getItem("theme") === "dark") {
        root.setAttribute("data-bs-theme", "dark");
        icon.classList.replace("bi-sun", "bi-moon");
    }

    // ⬇️ Toggle sidebar
    sidebarToggleBtn.addEventListener("click", () => {
        wrapper.classList.toggle("toggled");

        // Guardar estado en localStorage
        const isHidden = wrapper.classList.contains("toggled");
        localStorage.setItem("sidebar", isHidden ? "hidden" : "visible");
    });

    // ⬇️ Toggle modo oscuro
    toggle.addEventListener("click", () => {
        const isDark = root.getAttribute("data-bs-theme") === "dark";
        root.setAttribute("data-bs-theme", isDark ? "light" : "dark");
        localStorage.setItem("theme", isDark ? "light" : "dark");
        icon.classList.toggle("bi-sun");
        icon.classList.toggle("bi-moon");
    });
});
