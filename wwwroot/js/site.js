// Alterna la visibilidad de campos de contraseña marcados con data-toggle-password="idDelInput".
document.addEventListener('click', function (event) {
    const toggle = event.target.closest('[data-toggle-password]');
    if (!toggle) {
        return;
    }

    const input = document.getElementById(toggle.getAttribute('data-toggle-password'));
    if (!input) {
        return;
    }

    const isShowing = input.type === 'text';
    input.type = isShowing ? 'password' : 'text';
    toggle.setAttribute('aria-pressed', String(!isShowing));
    toggle.textContent = isShowing ? 'Mostrar' : 'Ocultar';
});
