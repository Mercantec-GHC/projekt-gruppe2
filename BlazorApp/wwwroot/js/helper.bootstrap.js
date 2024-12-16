export function OpenModal(modalId) {
    console.log(modalId);

    const modal = new bootstrap.Modal(document.getElementById(modalId));
    modal.show();
}