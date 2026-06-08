// Confirm delete dialogs
document.querySelectorAll('[data-confirm]').forEach(btn => {
    btn.addEventListener('click', function (e) {
        if (!confirm(this.dataset.confirm || 'Bạn có chắc chắn muốn thực hiện thao tác này?')) {
            e.preventDefault();
        }
    });
});

// Auto dismiss alerts after 4s
document.querySelectorAll('.alert-dismissible').forEach(el => {
    setTimeout(() => { el.style.opacity = 0; setTimeout(() => el.remove(), 400); }, 4000);
});
