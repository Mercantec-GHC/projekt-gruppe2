export function attachHandlers() {
    const bsCollapse = new bootstrap.Collapse('#navbarNav', {
        toggle: false
    });
    bsCollapse.hide();

    const loginModal = document.getElementById('loginModal')
    if (loginModal) {
        loginModal.addEventListener('hidden.bs.modal', event => {
            document.getElementById("loginForm").reset();
            document.getElementById('loginFlashMessage').style.display = "none";
        });

        loginModal.addEventListener('show.bs.modal', event => {
            bsCollapse.hide();
        });
    }
};