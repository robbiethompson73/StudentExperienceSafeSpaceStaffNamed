document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("sidebar");
    const toggleSidebar = document.getElementById("toggleSidebar");
    const closeSidebar = document.getElementById("closeSidebar");

    toggleSidebar.addEventListener("click", () => {
        sidebar.classList.toggle("active");
    });

    closeSidebar.addEventListener("click", () => {
        sidebar.classList.remove("active");
    });

    document.addEventListener("click", function (e) {
        if (!sidebar.contains(e.target) && !toggleSidebar.contains(e.target)) {
            sidebar.classList.remove("active");
        }
    });
});
