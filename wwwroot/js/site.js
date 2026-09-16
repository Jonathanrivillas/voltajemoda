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

// Rota los mensajes de la barra de promoción sobre la navbar cada 7 segundos.
(function () {
    const mensajes = [
        'Suscríbete y obtén el 15%DTO. en tu primera compra',
        'ENVIO GRATIS por compras superiores a $100.000'
    ];
    let indice = 0;

    setInterval(function () {
        const promoBar = document.getElementById('promoBar');
        if (!promoBar) {
            return;
        }

        promoBar.classList.add('promo-bar-fade');

        setTimeout(function () {
            indice = (indice + 1) % mensajes.length;
            promoBar.textContent = mensajes[indice];
            promoBar.classList.remove('promo-bar-fade');
        }, 400);
    }, 7000);
})();

// Navega entre las fotos de una tarjeta de producto con las flechas chicas sobre la imagen.
document.addEventListener('click', function (event) {
    const boton = event.target.closest('.producto-img-nav');
    if (!boton) {
        return;
    }

    const media = boton.closest('.producto-card-media');
    if (!media) {
        return;
    }

    const imagenes = media.querySelectorAll('.producto-card-img');
    if (imagenes.length < 2) {
        return;
    }

    let indiceActual = Array.prototype.findIndex.call(imagenes, function (img) {
        return img.classList.contains('activa');
    });
    if (indiceActual === -1) {
        indiceActual = 0;
    }

    const direccion = boton.classList.contains('producto-img-nav-prev') ? -1 : 1;
    const siguiente = (indiceActual + direccion + imagenes.length) % imagenes.length;

    imagenes[indiceActual].classList.remove('activa');
    imagenes[siguiente].classList.add('activa');
});

// Desliza el carrusel de "Ofertas destacadas" con las flechas grandes a los costados.
document.addEventListener('click', function (event) {
    const boton = event.target.closest('.destacados-arrow');
    if (!boton) {
        return;
    }

    const wrapper = boton.closest('.destacados-wrapper');
    const carrusel = wrapper?.querySelector('.destacados-carousel');
    if (!carrusel) {
        return;
    }

    const direccion = boton.classList.contains('destacados-arrow-prev') ? -1 : 1;
    carrusel.scrollBy({ left: direccion * carrusel.clientWidth * 0.9, behavior: 'smooth' });
});
