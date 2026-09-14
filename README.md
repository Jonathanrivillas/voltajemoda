# Voltaje Moda

Ecommerce de ropa construido en **Blazor Web App** (.NET 10), full-stack en C#: un mismo proyecto sirve tanto la lógica de servidor como la interfaz. No hay una separación de repos/deploys entre "frontend" y "backend" — es un solo codebase, y se espera que **todo el equipo trabaje en ambas partes** según lo que necesite la funcionalidad que esté construyendo.

## Alcance actual del proyecto

Funcionalidades ya implementadas y operativas de punta a punta:

- **Catálogo**: productos con variantes (talla/color) y stock por variante, categorías, filtro por categoría, productos en oferta (precio original + precio de oferta).
- **Cuentas**: registro/login (ASP.NET Core Identity), roles `Cliente` y `Administrador`, perfil editable, gestión de direcciones de envío. Registro con nombre, teléfono, correo, contraseña y dirección de envío en un solo paso, más aceptación de términos y condiciones. Login también disponible con **Google** (vincula automáticamente si ya existe una cuenta local con el mismo correo). Interfaz de login/registro con diseño propio (`auth-page`/`auth-card` en `wwwroot/app.css`).
- **Carrito**: funciona con o sin cuenta (invitado). El carrito de un invitado se fusiona automáticamente con su cuenta al iniciar sesión.
- **Checkout**: con o sin cuenta, valida stock disponible, descuenta stock al confirmar, respeta el precio de oferta vigente.
- **Historial de pedidos** del cliente, y **detalle de pedido** (accesible tanto por el dueño registrado como por el invitado que lo generó).
- **Panel de administración** (`/admin`, solo rol `Administrador`): CRUD de productos (incluyendo variantes) y categorías, gestión de pedidos y cambio de estado. Con protecciones para no borrar en cascada datos con historial real (ver "Convenciones" más abajo).
- **Diseño base**: navbar superior, footer con contacto/redes (placeholders, ver más abajo), paleta blanco/negro aplicada globalmente.

### Backlog / próximos pasos (no implementado todavía)

- Pasarela de pago real (hoy el checkout es simulado).
- Búsqueda de productos, cupones/descuentos por código, subida de imágenes (hoy las imágenes son por URL externa).
- Reemplazar los placeholders del footer (redes sociales, teléfono) por los datos reales.
- Rehacer todo el diseño visual de las interfaces.
- Pruebas automatizadas (unitarias/integración).

## Stack

- **.NET 10** (Blazor Web App, interactividad Server)
- **Entity Framework Core 10** + **SQL Server Express** en desarrollo / **Azure SQL Database** en producción
- **ASP.NET Core Identity** (registro/login, roles `Cliente` / `Administrador`), con login externo por **Google OAuth**
- **Azure Communication Services** para el envío real de emails (confirmación de cuenta, reseteo de contraseña) en producción
- **Bootstrap** (incluido por la plantilla)

## Requisitos previos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- **SQL Server Express** instalado localmente (instalación "Basic"), con la instancia por defecto `SQLEXPRESS`. Si tu instancia se llama distinto, ajusta la cadena de conexión en `appsettings.json`.
- Editor: VS Code + extensión **C# Dev Kit** (o Visual Studio 2022).

## Cómo levantar el proyecto

```powershell
git clone https://github.com/Jonathanrivillas/voltajemoda.git
cd voltajemoda
git checkout desarrollo
dotnet restore
dotnet ef database update
dotnet run
```

- Trabaja siempre a partir de la rama `desarrollo` (ver [CONTRIBUTING.md](./CONTRIBUTING.md) para el flujo completo de ramas y Pull Requests).
- `dotnet ef database update` crea la base de datos `VoltajeModaDb` y aplica todas las migraciones existentes.
- Al arrancar **en desarrollo local únicamente**, la app siembra automáticamente datos de prueba (categorías, productos con variantes, una oferta) y roles (`Cliente`, `Administrador`), incluyendo un usuario administrador de prueba:
  - **Email:** `admin@voltajemoda.com`
  - **Contraseña:** `Admin123!`
  
  Este usuario **no existe en producción** (el seeding de datos demo y del admin de prueba está condicionado a `IsDevelopment()` en `Program.cs`). En producción, el primer administrador se asigna manualmente en la base de datos — ver la sección "Despliegue" más abajo.

Si no tienes la herramienta `dotnet-ef` instalada:

```powershell
dotnet tool install --global dotnet-ef
```

> **Importante:** no corras `dotnet ef migrations add` tú mismo salvo que seas el dueño del proyecto. Ver la sección de migraciones en [CONTRIBUTING.md](./CONTRIBUTING.md).

## Estructura del proyecto

```
Models/           Entidades de dominio (Producto, Categoria, Carrito, Pedido, etc.) — el "backend" de datos
Data/             ApplicationDbContext, ApplicationUser, sembradores de datos, Migrations/
Services/         Servicios de negocio compartidos entre varias páginas (ej. CarritoService)
Components/
  Pages/          Páginas públicas (Home, Productos, Carrito, Checkout, MisPedidos...)
  Pages/Admin/    Panel de administración (protegido por rol "Administrador")
  Account/        Login/Registro/gestión de cuenta (scaffolding de Identity)
  Layout/         MainLayout, NavMenu, Footer, AdminLayout
  Shared/         Componentes reutilizables sin ruta propia (ej. ProductoCard)
```

Como el proyecto es un solo Blazor Web App, una misma página `.razor` normalmente mezcla ambas capas: la parte "backend" (consultas EF Core, validaciones, reglas de negocio en el bloque `@code`) y la parte "frontend" (el marcado HTML/Razor de arriba). No hay una API separada que consumir — Blazor Server llama directo a `ApplicationDbContext`/servicios desde el propio componente.

## Cómo agregar una nueva vista (página)

1. Crea un archivo `.razor` en `Components/Pages/` (o en `Components/Pages/Admin/` si es del panel de administración).
2. Agrega la directiva de ruta arriba del todo: `@page "/mi-ruta"`.
3. Decide el modo de renderizado:
   - Si la página **no tiene eventos** (clics, formularios que reaccionan en vivo, `@bind`), déjala sin nada — se sirve como HTML estático (SSR), más liviano.
   - Si la página **necesita interactividad** (`@onclick`, `@bind` con actualización en vivo), agrega `@rendermode InteractiveServer` justo debajo de `@page`. Sin esto, los eventos simplemente no disparan nada (ver "Convenciones" abajo).
4. Si necesitas acceso a datos: `@inject ApplicationDbContext Db` (y `@using Microsoft.EntityFrameworkCore` para los métodos async como `ToListAsync()`).
5. Si la página es solo para cierto rol, agrega `@attribute [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrador")]` (o sin `Roles` para exigir solo estar logueado).
6. Agrega el link correspondiente en `Components/Layout/NavMenu.razor` si debe aparecer en el menú.

## Cómo crear y usar un componente reutilizable

Un componente reutilizable es un `.razor` **sin** `@page` — no es una ruta navegable, es una pieza de UI que insertas dentro de otras páginas con su propio tag. Ejemplo real ya en el proyecto: `Components/Shared/ProductoCard.razor`, usado tanto en `Productos.razor` como en `Home.razor` para no repetir el HTML de la tarjeta de producto.

```razor
@* Components/Shared/ProductoCard.razor *@
<div class="card h-100">
    <h5 class="card-title">@Producto.Nombre</h5>
    ...
</div>

@code {
    [Parameter, EditorRequired]
    public Producto Producto { get; set; } = default!;

    [Parameter]
    public bool MostrarCategoria { get; set; } = true;
}
```

Y se usa así en cualquier página (no necesita `@using` adicional — `Components/Shared` ya está importado globalmente en `_Imports.razor`):

```razor
<ProductoCard Producto="producto" MostrarCategoria="false" />
```

Cuando veas HTML repetido entre dos o más páginas, es buena señal de que debería ser un componente en `Components/Shared/`. Componentes ya disponibles para reutilizar (en vez de repetir el HTML a mano):

- **`<Cargando />`** — reemplaza cualquier `<p>Cargando...</p>` mientras una página espera datos async.
- **`<EstadoVacio Mensaje="..." EnlaceTexto="..." EnlaceHref="..." />`** — reemplaza los mensajes de "no hay nada que mostrar" (carrito vacío, sin pedidos, sin productos, etc.). `EnlaceTexto`/`EnlaceHref` son opcionales, solo agrégalos si el mensaje debe incluir un link de acción.
- **`<ProductoCard />`** — tarjeta de producto (catálogo/inicio).

Antes de escribir un nuevo bloque de "cargando" o "no hay resultados", usa estos componentes en vez de duplicar el markup.

## Convenciones a tener en cuenta

- **Todo el texto visible al usuario debe estar en español** — títulos de página, botones, labels, placeholders, mensajes de error/éxito, todo. No mezclar inglés y español en una misma vista. Esto aplica también a las páginas de `Components/Account/` (login, registro, gestión de cuenta): aunque nacieron del scaffolding de Identity en inglés, ya están traducidas — si tocas alguna, mantenla en español. La única excepción es **nombres en código** (clases, propiedades, métodos, rutas de C#) — ahí sí se sigue la convención de nombres en español para el dominio del ecommerce, pero el código de infraestructura/Identity que viene de la plantilla de Microsoft se dejó en inglés tal como se genera (nombres de propiedades de `ApplicationUser`, servicios de Identity, etc.).
- **Modos de renderizado de Blazor:** las páginas son estáticas (SSR) por defecto. Si tu página necesita reaccionar a eventos (`@onclick`, `@bind` con cambios en vivo), agrégale `@rendermode InteractiveServer` explícitamente — si no, los eventos simplemente no van a disparar nada.
- **Carrito y checkout funcionan sin cuenta** (invitados): el visitante anónimo se identifica con un id guardado vía `ProtectedLocalStorage` en su navegador (`Services/CarritoService.cs`). Por eso `Carrito`, `Direccion` y `Pedido` tienen `UsuarioId` **y** `AnonimoId`, ambos opcionales — un registro pertenece a uno u otro. Al iniciar sesión, todo lo anónimo se reasigna automáticamente a la cuenta.
- **`ProtectedLocalStorage` necesita JS interop**, que no está disponible durante el prerenderizado — por eso las páginas que lo usan (`Carrito.razor`, `Checkout.razor`) cargan sus datos en `OnAfterRenderAsync(firstRender)`, no en `OnInitializedAsync`.
- **Cuidado con `ON DELETE CASCADE`:** borrar una `Categoria` con productos, o un `Producto`/`ProductoVariante` con pedidos asociados, borraría en cascada historial real. Las páginas de administración ya validan esto — si agregas nuevas formas de borrar datos, replica esa validación.
- **Precio de un producto:** usa siempre `producto.PrecioEfectivo` (no `producto.Precio` directamente) para cualquier cálculo de dinero real (carrito, checkout, totales) — respeta automáticamente si el producto está en oferta. `Precio` es el precio de lista, útil solo para mostrarlo tachado.
- **Colores:** paleta blanco/negro aplicada globalmente en `wwwroot/app.css` (sobreescribe `.btn-primary`, `.text-primary`, `.bg-primary` de Bootstrap). Usa clases `btn-dark` / `btn-outline-dark` en vistas nuevas en vez de `btn-primary`, para consistencia visual.
- Después de cambiar cualquier clase en `Models/`, hace falta una migración — pero **no la generes tú mismo**, ver [CONTRIBUTING.md](./CONTRIBUTING.md).

## Autenticación

- **Registro por correo**: pide nombre completo, correo, teléfono, contraseña y una dirección de envío en el mismo formulario, además de aceptar los [Términos y Condiciones](./Components/Pages/Terminos.razor) (placeholder, ver backlog). Requiere confirmar el correo antes de poder iniciar sesión (`RequireConfirmedAccount = true`).
- **Login con Google**: opcional — solo aparece si están configuradas las credenciales (`Authentication:Google:ClientId` / `Authentication:Google:ClientSecret`, vía `dotnet user-secrets` en local o variables de entorno `Authentication__Google__ClientId` / `Authentication__Google__ClientSecret` en Azure App Service). Si el correo de la cuenta de Google ya tiene una cuenta local registrada por contraseña, se **vincula automáticamente** a esa cuenta en vez de fallar por correo duplicado (`Components/Account/Pages/ExternalLogin.razor`). Fuerza el selector de cuenta de Google en cada intento (`prompt=select_account` en `Program.cs`).
- **Política de contraseña**: mínimo 10 caracteres, con mayúscula, minúscula, número y símbolo. Bloqueo de cuenta tras 5 intentos fallidos por 10 minutos.
- **Rate limiting**: máximo 10 solicitudes por minuto por IP en cualquier ruta `/Account/*`, para mitigar fuerza bruta/credential stuffing.
- **Passkeys**: el código de gestión sigue en `Cuenta > Seguridad`, pero **no** hay entrada de login por passkey en `/Account/Login` (se quitó por UX — Google la reemplaza).

## Despliegue

La app corre en producción sobre **Azure App Service** (Linux, .NET 10) + **Azure SQL Database**, con **Azure Communication Services** para el envío real de emails (confirmación de cuenta, reseteo de contraseña).

- El despliegue es **automático**: cada push a `main` dispara un workflow de GitHub Actions (`.github/workflows/main_voltajemoda.yml`) que compila y publica la app.
- Las credenciales (connection string de SQL, connection string de Azure Communication Services) se configuran como variables de entorno/cadenas de conexión directamente en el App Service — **nunca** en `appsettings.json` ni en el repo.
- El primer administrador en producción se crea registrándose normalmente en el sitio y luego asignándole el rol `Administrador` manualmente vía SQL (no hay todavía una pantalla de gestión de roles en el panel admin).
- Esto lo gestiona exclusivamente el dueño del proyecto, igual que las migraciones (ver [CONTRIBUTING.md](./CONTRIBUTING.md)).

## Cómo contribuir

Ver [CONTRIBUTING.md](./CONTRIBUTING.md) para el flujo de ramas, Pull Requests, y el mecanismo de migraciones de base de datos.

