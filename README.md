# Voltaje Moda

Ecommerce de ropa construido en **Blazor Web App** (.NET 10), full-stack en C#: un mismo proyecto sirve tanto la lógica de servidor como la interfaz. No hay una separación de repos/deploys entre "frontend" y "backend" — es un solo codebase, y se espera que **todo el equipo trabaje en ambas partes** según lo que necesite la funcionalidad que esté construyendo.

## Alcance actual del proyecto

Funcionalidades ya implementadas y operativas de punta a punta:

- **Catálogo**: productos con variantes (talla/color) y stock por variante, categorías, filtro por categoría, productos en oferta (precio original + precio de oferta).
- **Cuentas**: registro/login (ASP.NET Core Identity), roles `Cliente` y `Administrador`, perfil editable, gestión de direcciones de envío. Registro con nombre, teléfono, correo, contraseña y dirección de envío en un solo paso, más aceptación de términos y condiciones. Login también disponible con **Google** (vincula automáticamente si ya existe una cuenta local con el mismo correo). Interfaz de login/registro con diseño propio (`auth-page`/`auth-card` en `wwwroot/app.css`).
- **Carrito**: funciona con o sin cuenta (invitado). El carrito de un invitado se fusiona automáticamente con su cuenta al iniciar sesión.
- **Checkout**: con o sin cuenta, valida stock disponible, descuenta stock al confirmar, respeta el precio de oferta vigente.
- **Historial de pedidos** del cliente, y **detalle de pedido** (accesible tanto por el dueño registrado como por el invitado que lo generó).
- **Panel de administración** (`/admin`, solo rol `Administrador`): shell propio con sidebar oscura y diseño independiente del sitio público. Incluye dashboard con KPIs y gráficos (ventas, top productos), gestión del banner/carrusel del inicio (`/admin/banner`, imagen por URL o subida desde el equipo, con enlace opcional al hacer click), CRUD de productos (incluyendo variantes, imagen y marca de "destacado") y categorías, control de inventario con historial de movimientos de stock (`/admin/inventario`), gestión de pedidos y cambio de estado, estadísticas con rango de fechas configurable (`/admin/estadisticas`) y gestión de usuarios/roles (`/admin/usuarios`). Con protecciones para no borrar en cascada datos con historial real (ver "Convenciones" más abajo).
- **Inicio (`/`)**: carrusel configurable desde el admin (con fallback a una imagen genérica si no hay slides activos) y sección "Ofertas destacadas" con los productos que el admin marcó como destacados (no es automático por precio/oferta).
- **Diseño base**: navbar superior, footer con contacto/redes (placeholders, ver más abajo), paleta blanco/negro aplicada globalmente en el sitio público; el panel admin tiene su propia paleta (sidebar oscura + acento coral), ver `wwwroot/admin.css`.

### Backlog / próximos pasos (no implementado todavía)

- Pasarela de pago real (hoy el checkout es simulado).
- Búsqueda de productos, cupones/descuentos por código.
- Reemplazar los placeholders del footer (redes sociales, teléfono) por los datos reales.
- Rehacer el diseño visual del sitio público (el panel de administración ya tiene diseño propio, ver arriba).
- Pruebas automatizadas (unitarias/integración).

## Stack

- **.NET 10** (Blazor Web App, interactividad Server)
- **Entity Framework Core 10** + **SQL Server Express** en desarrollo / **Azure SQL Database** en producción
- **ASP.NET Core Identity** (registro/login, roles `Cliente` / `Administrador`), con login externo por **Google OAuth**
- **Azure Communication Services** para el envío real de emails (confirmación de cuenta, reseteo de contraseña) en producción
- **Bootstrap** (incluido por la plantilla, vendored en `wwwroot/lib/bootstrap`)
- **Chart.js**, cargado por CDN desde `Components/App.razor` (se probó cargarlo solo desde el layout del admin vía `<HeadContent>`, pero con la navegación "enhanced" de Blazor el script no siempre llegaba a estar listo a tiempo y tumbaba el circuito — por eso vive en el shell raíz, que sí se carga de forma determinística siempre). **Bootstrap Icons**, cargado por CDN solo en el panel de administración (`Components/Layout/AdminLayout.razor`, es únicamente CSS, no tiene este problema de timing). No hay pipeline de npm/build de JS en el proyecto, por eso se usan así en vez de instalarlos como paquete.

## Requisitos previos

- [.NET SDK 10](https://dotnet.microsoft.com/download) — mismo instalador en Windows, Linux y macOS.
- **Base de datos** (SQL Server): la forma de tenerla corriendo localmente cambia según tu sistema operativo — ver [Base de datos según tu sistema operativo](#base-de-datos-según-tu-sistema-operativo) más abajo.
- **Editor + extensiones**:
  - **VS Code** (Windows, Linux, macOS):
    - [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) (`ms-dotnettools.csdevkit`) — soporte de C#/Razor/Blazor y depurador. Es la única extensión estrictamente necesaria para compilar y correr el proyecto.
    - [SQL Server (mssql)](https://marketplace.visualstudio.com/items?itemName=ms-mssql.mssql) (`ms-mssql.mssql`) — opcional, para conectarte y explorar la base de datos desde el propio editor.
  - **Visual Studio 2022** (Windows; también hay una vista previa para Mac, pero el equipo usa mayormente VS Code + Visual Studio en Windows): versión **17.12 o superior** (la que soporta .NET 10), con el workload **"Desarrollo web y de ASP.NET"** marcado en el instalador — incluye las herramientas de Blazor/Razor. El workload **"Almacenamiento y procesamiento de datos"** (SQL Server Data Tools) es opcional, solo si preferís gestionar la base desde ahí en vez de con `mssql`/`sqlcmd`.
  - **Docker Desktop** (Linux: Docker Engine) — necesario únicamente si vas a correr SQL Server en contenedor (Linux y macOS, ver abajo).

### Base de datos según tu sistema operativo

- **Windows**: SQL Server Express local (instalación "Basic"), con la instancia por defecto `SQLEXPRESS`. Si tu instancia se llama distinto, ajusta la cadena de conexión en `appsettings.json`. Es la opción que usa hoy el resto del equipo.
- **Linux**: no existe SQL Server Express nativo fuera de contenedor — corré la imagen oficial de Microsoft con Docker:

  ```bash
  docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=TuPassword123!" \
    -p 1433:1433 --name voltaje-sql -d mcr.microsoft.com/mssql/server:2022-latest
  ```

- **macOS**:
  - **Intel**: la misma imagen y comando que en Linux (`mcr.microsoft.com/mssql/server:2022-latest`) funciona sin cambios.
  - **Apple Silicon (M1/M2/M3)**: la imagen estándar de SQL Server no tiene build ARM64 nativo. Usá **Azure SQL Edge** en su lugar (sí soporta ARM64):

    ```bash
    docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=TuPassword123!" \
      -p 1433:1433 --name voltaje-sql -d mcr.microsoft.com/azure-sql-edge
    ```

    Alternativa si ya tenés una base Azure SQL de desarrollo compartida por el equipo: apuntá directo a esa cadena de conexión en vez de correr un contenedor local.

- **Nota común para Linux/macOS**: quien use Docker debe ajustar la cadena de conexión en `appsettings.Development.json` (o vía `dotnet user-secrets`) para usar autenticación SQL en vez de la integrada de Windows, por ejemplo:

  ```
  Server=localhost,1433;Database=VoltajeModaDb;User Id=sa;Password=TuPassword123!;TrustServerCertificate=True;
  ```

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
  
  Este usuario **no existe en producción** (el seeding de datos demo y del admin de prueba está condicionado a `IsDevelopment()` en `Program.cs`). En producción, el primer administrador se asigna manualmente en la base de datos — ver la sección "Permisos" más abajo.

Si no tienes la herramienta `dotnet-ef` instalada:

```powershell
dotnet tool install --global dotnet-ef
```

> **Importante:** no corras `dotnet ef migrations add` tú mismo salvo que seas el dueño del proyecto. Ver la sección de migraciones en [CONTRIBUTING.md](./CONTRIBUTING.md).

## Estructura del proyecto

```
Models/           Entidades de dominio (Producto, Categoria, Carrito, Pedido, MovimientoStock, etc.) — el "backend" de datos
Data/             ApplicationDbContext, ApplicationUser, sembradores de datos
Migrations/       Migraciones de EF Core (protegida por .github/CODEOWNERS, ver CONTRIBUTING.md)
Services/         Servicios de negocio compartidos entre varias páginas (ej. CarritoService, DashboardService, InventarioService, EstadisticasService)
Services/Dtos/    Records de solo lectura usados por los servicios del panel admin (dashboard, inventario, estadísticas)
Components/
  Pages/          Páginas públicas (Home, Productos, Carrito, Checkout, MisPedidos...)
  Pages/Admin/    Panel de administración (protegido por rol "Administrador"): dashboard, productos, categorías, inventario, pedidos, estadísticas, usuarios
  Account/        Login/Registro/gestión de cuenta (scaffolding de Identity)
  Layout/         MainLayout, NavMenu, Footer, AdminLayout
  Shared/         Componentes reutilizables sin ruta propia (ej. ProductoCard)
  Shared/Admin/   Componentes reutilizables solo del panel admin (AdminCard, AdminStatCard, ConfirmDialog, StockBadge, EstadoPedidoBadge)
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

- El despliegue es **automático**: cada push a `main` dispara un workflow de GitHub Actions (`.github/workflows/main_voltajemoda.yml`) que compila y publica la app al App Service llamado `voltajemoda`.
- **Cómo ver el sitio en producción**: por defecto en `https://voltajemoda.azurewebsites.net` (verificá el dominio real vigente en Azure Portal → App Service `voltajemoda` → **Overview**, por si hay un dominio personalizado configurado).
- **Cómo revisar logs/estado en producción**: Azure Portal → App Service `voltajemoda` → **Log stream** (logs en vivo) o **Diagnosticar y solucionar problemas**.
- **Migraciones en producción**: el workflow de GitHub Actions (`.github/workflows/main_voltajemoda.yml`) hoy solo compila y publica — no corre `dotnet ef database update` contra la base de producción. Confirmá con el dueño del proyecto si aplica las migraciones manualmente antes/después de cada deploy, o si eso todavía está pendiente de automatizar.
- **Imágenes de productos subidas desde el panel admin** (`/admin/productos/*/editar`, guardadas en `wwwroot/uploads/productos/`) se escriben en el disco local del App Service. Azure App Service **no garantiza que ese disco persista entre deploys** (cada deploy puede reemplazar el contenido de la app) ni entre instancias si el plan escala a más de una — funciona bien en desarrollo local y para volúmenes chicos, pero si el catálogo crece o el sitio escala, conviene migrar el guardado a Azure Blob Storage en vez de disco local. Queda como ítem de backlog, no es necesario para desarrollar/probar localmente.
- Las credenciales (connection string de SQL, connection string de Azure Communication Services) se configuran como variables de entorno/cadenas de conexión directamente en el App Service — **nunca** en `appsettings.json` ni en el repo.
- Esto lo gestiona exclusivamente el dueño del proyecto, igual que las migraciones (ver [CONTRIBUTING.md](./CONTRIBUTING.md)).

## Permisos

Hay dos niveles de permisos distintos en este proyecto: los roles dentro de la aplicación, y los accesos a la infraestructura donde corre.

### Roles de la aplicación

- La app tiene dos roles de Identity: `Cliente` y `Administrador` (sembrados en `Data/IdentitySeeder.cs`). El panel `/admin` completo exige `Administrador`.
- **Primer administrador en producción**: siempre requiere un paso manual — registrate normalmente en el sitio y asignate el rol `Administrador` por SQL directo contra la base de producción. Es inevitable: para usar la pantalla de gestión de usuarios primero hace falta ya ser administrador.
- **A partir de ahí**, otorgar o quitar el rol `Administrador` a otras cuentas se hace desde el propio panel, en **`/admin/usuarios`** — ya no hace falta tocar la base de datos a mano para altas posteriores.
- En desarrollo local, el seeding automático (`Data/IdentitySeeder.cs`, solo corre si `IsDevelopment()`) ya crea un usuario administrador de prueba (`admin@voltajemoda.com` / `Admin123!`), así que este paso manual no aplica localmente.

### Accesos de infraestructura (Azure / GitHub)

- **Quién puede desplegar**: cualquiera con permiso de push/merge a `main` dispara el deploy automáticamente (ver workflow arriba); el acceso a `main` está limitado por la regla de protección de rama (ver [CONTRIBUTING.md](./CONTRIBUTING.md)).
- **Dónde viven los secretos**: el publish profile del App Service se guarda como GitHub Secret (`AZUREAPPSERVICE_PUBLISHPROFILE_...`, ver `.github/workflows/main_voltajemoda.yml`); las connection strings de producción (SQL, Azure Communication Services) viven como configuración del App Service en Azure, no en GitHub ni en el repo.
- **Quién tiene acceso al propio recurso de Azure** (App Service, Azure SQL Database, Azure Communication Services) lo administra el dueño del proyecto — pedile acceso directamente si lo necesitás.
- El control de qué se puede mergear a `main` y a la carpeta `Migrations/` pasa por `.github/CODEOWNERS` + la regla de protección de rama en GitHub (ver [CONTRIBUTING.md](./CONTRIBUTING.md)).

## Cómo contribuir

Ver [CONTRIBUTING.md](./CONTRIBUTING.md) para el flujo de ramas, Pull Requests, y el mecanismo de migraciones de base de datos.

