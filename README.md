# Voltaje Moda

Frontend de ecommerce en Blazor Web App y .NET 10.

## Arquitectura

`voltajemoda` es la interfaz Blazor y consume por HTTP la API REST del proyecto `voltajemoda_backend`, cuya URL base local es `http://localhost:5181/api/`. El frontend no abre conexiones directas a SQL Server.

La base de datos oficial y activa es `VoltajeModa` en SQL Server, administrada por el backend. `VoltajeModaDb`, usada anteriormente por Identity, está deprecada y desconectada.

## Autenticación

El inicio de sesión usa `POST /api/Auth/login`. El JWT y los datos de sesión se mantienen en memoria por circuito en `SesionUsuarioService`. `ProductoApiClient` añade `Authorization: Bearer <token>` a las solicitudes protegidas. Al reiniciar o recargar el circuito, el usuario debe iniciar sesión nuevamente.

## Ejecución local

El carrito se guarda mediante los endpoints protegidos de la API. Las selecciones de productos destacados del panel de banners se mantienen en memoria por circuito y no son persistentes en la base de datos.

En una terminal, desde la carpeta del backend:

```powershell
cd <ruta>\voltajemoda_backend
dotnet run
```

En otra terminal, desde la carpeta del frontend:

```powershell
cd <ruta>\voltajemoda
dotnet run
```

Verifica que `ApiSettings:BaseUrl` en `appsettings.json` apunte al backend. No hay credenciales iniciales universales ni contraseñas de prueba configuradas en este repositorio; solicita al responsable de la base `VoltajeModa` una cuenta Administrador o crea una mediante el proceso autorizado del backend. No guardar contraseñas reales en este README.

## Rutas principales

| API | Uso |
|---|---|
| `POST /api/Auth/login`, `POST /api/Auth/registro` | Inicio de sesión y registro |
| `GET /api/Productos`, `GET /api/Categorias` | Catálogo |
| `POST /api/Productos`, `POST /api/Variantes` | Crear productos y variantes |
| `POST /api/Variantes/{id}/inventario` | Ajustes de stock y Kardex |
| `GET /api/AdminCupones`, `POST /api/AdminCupones` | Consultar y crear cupones |
| `PUT /api/Variantes/{id}/estado` | Activar o desactivar una variante |

Rutas de la interfaz: `/productos`, `/productos/{id}`, `/Account/Login` y panel administrativo bajo `/admin/*` (productos, categorías, inventario, usuarios y cupones).
