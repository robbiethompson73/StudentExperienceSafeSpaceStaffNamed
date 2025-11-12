document.addEventListener('DOMContentLoaded', () => {
    const sidebar = document.querySelector('.sidebar');
    const toggleButton = document.getElementById('toggleSidebar');
    const closeButton = document.getElementById('closeSidebar');

    toggleButton.addEventListener('click', () => {
        sidebar.classList.add('active');
    });

    closeButton.addEventListener('click', () => {
        sidebar.classList.remove('active');
    });

    // Optional: Close sidebar on ESC key
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape' && sidebar.classList.contains('active')) {
            sidebar.classList.remove('active');
        }
    });
});
