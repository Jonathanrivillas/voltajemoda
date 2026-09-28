# API REST de VoltajeModa — guía completa

Este documento explica, sin dar por hecho que ya sabés arquitectura hexagonal ni DDD, **qué se construyó, para qué sirve, cómo está armado por dentro y cómo lo usás**. Está pensado para que cualquiera del equipo pueda leerlo de arriba a abajo y entender la API completa, aunque nunca la haya tocado.

## 1. ¿Qué es esto y para qué sirve?

Hasta ahora, VoltajeModa era **solo** una aplicación Blazor Server: las páginas `.razor` (lo que ves en el navegador) hablaban directo con la base de datos. Eso funciona bien mientras la única puerta de entrada al sistema sea el propio sitio web.

Lo que se agregó es **una puerta de entrada nueva y separada**: una API REST. Una API REST es, en el fondo, un conjunto de URLs a las que cualquier programa (no solo un navegador mostrando el sitio) le puede pedir cosas usando HTTP: "dame la lista de productos", "agregá esto al carrito", "creá este pedido". El programa que pregunta le manda datos en formato JSON y recibe la respuesta también en JSON.

**¿Para qué sirve tener esto además del sitio web?** Porque abre la puerta a que en el futuro otras cosas usen los mismos datos y las mismas reglas de negocio sin duplicar código: una app móvil, un panel externo, una integración con otro sistema, pruebas automatizadas, etc. Todo eso podría hablar con `https://tuservidor/api/...` sin necesitar saber nada de Blazor ni de cómo está armada la base de datos.

**Importante:** el sitio web (`Components/`, `Services/`) **sigue funcionando exactamente igual que antes**. No se tocó nada de eso. La API es un agregado, no un reemplazo — conviven las dos cosas en el mismo proyecto.

## 2. ¿Qué significa "arquitectura hexagonal"?

Imaginate que las **reglas del negocio** (qué es un pedido, cuándo se puede cancelar, que no se puede vender más stock del que hay) son lo más importante e independiente que existe en el sistema. Todo lo demás — la base de datos, el formato JSON, el framework web — son detalles que podrían cambiar sin que esas reglas cambien.

La arquitectura hexagonal organiza el código en capas para proteger esa idea:

1. **Dominio** (`Core/Dominio`): las reglas de negocio puras, en clases de C# normales que no saben nada de bases de datos ni de HTTP. Por ejemplo, la clase `Pedido` sabe que "un pedido no puede estar vacío" y que "no se puede pasar de Entregado a Pendiente" — eso es una regla de negocio, vive acá, y no depende de Entity Framework ni de nada externo.
2. **Aplicación** (`Core/Aplicacion`): los "casos de uso" — pasos concretos como "crear un pedido a partir del carrito" o "registrar un movimiento de stock". Cada caso de uso orquesta al dominio y habla con el resto del mundo únicamente a través de **interfaces** (a esto se le llama "puertos"), nunca conociendo el detalle de cómo se guarda algo en la base de datos.
3. **Infraestructura** (`Infraestructura/`): la implementación real de esos puertos usando Entity Framework Core — acá sí se habla con la base de datos SQL Server.
4. **Presentación** (`Presentacion/`): los controladores HTTP — reciben la petición JSON, la traducen a lo que entiende un caso de uso, y devuelven la respuesta.

La ventaja práctica: si mañana quisiéramos cambiar de SQL Server a otra base de datos, o agregar una interfaz de línea de comandos además de la API REST, **el dominio y los casos de uso no se tocan** — solo se reemplazaría la capa de Infraestructura o se agregaría una nueva capa de Presentación.

## 3. Cómo está organizado el código

```
Core/
  Dominio/        Las reglas de negocio puras (Producto, Carrito, Pedido, Inventario...)
  Aplicacion/      Los casos de uso (CrearPedidoDesdeCarritoCasoDeUso, AgregarItemCarritoCasoDeUso...)
Infraestructura/
  Persistencia/    Los repositorios que hablan con la base de datos (reutilizando el ApplicationDbContext existente)
  Extensiones/     Configuración de inyección de dependencias
Presentacion/
  Controladores/   Los controladores REST (ProductosController, CarritoController...)
  Dtos/            Las formas de los JSON que entran y salen
  Filtros/          Traduce errores de negocio a respuestas HTTP
  Servicios/        Resuelve si quien pregunta es un usuario logueado o un invitado
```

Cada uno de los cuatro módulos de negocio (Productos, Carrito, Pedidos, Inventario) repite este mismo patrón de cuatro capas.

## 4. Los cuatro módulos de la API

### 4.1 Productos (`/api/productos`, `/api/categorias`)

Maneja el catálogo: productos, sus variantes (talla/color con stock), sus imágenes, y las categorías.

| Acción | Método y ruta | ¿Quién puede usarlo? |
|---|---|---|
| Ver el catálogo (con filtros y paginado) | `GET /api/productos` | Cualquiera |
| Ver el detalle de un producto | `GET /api/productos/{id}` | Cualquiera |
| Crear un producto | `POST /api/productos` | Solo Administrador |
| Editar un producto | `PUT /api/productos/{id}` | Solo Administrador |
| Borrar un producto | `DELETE /api/productos/{id}` | Solo Administrador |
| Poner/quitar oferta | `POST` / `DELETE /api/productos/{id}/oferta` | Solo Administrador |
| Agregar una variante (talla/color) | `POST /api/productos/{id}/variantes` | Solo Administrador |
| Agregar una imagen | `POST /api/productos/{id}/imagenes` | Solo Administrador |
| Ver categorías | `GET /api/categorias` | Cualquiera |
| Crear categoría | `POST /api/categorias` | Solo Administrador |

Reglas de negocio que la API hace cumplir sola (sin que el que la usa tenga que acordarse): el precio tiene que ser mayor a 0, el precio de oferta tiene que ser menor al precio de lista, no puede haber dos variantes con la misma combinación de talla y color.

### 4.2 Carrito (`/api/carrito`)

| Acción | Método y ruta |
|---|---|
| Ver mi carrito | `GET /api/carrito` |
| Agregar un producto | `POST /api/carrito/items` |
| Cambiar la cantidad de un ítem | `PUT /api/carrito/items/{productoVarianteId}` |
| Quitar un ítem | `DELETE /api/carrito/items/{productoVarianteId}` |
| Vaciar el carrito | `DELETE /api/carrito` |
| Fusionar el carrito de invitado con mi cuenta | `POST /api/carrito/fusionar` (requiere estar logueado) |

Todos estos endpoints son públicos porque **el carrito funciona con o sin cuenta** (ver punto 5 más abajo sobre cómo se identifica a un invitado).

### 4.3 Pedidos y direcciones (`/api/pedidos`, `/api/direcciones`)

| Acción | Método y ruta | ¿Quién puede usarlo? |
|---|---|---|
| Crear un pedido a partir de mi carrito | `POST /api/pedidos` | Cualquiera (con o sin cuenta) |
| Ver mis pedidos | `GET /api/pedidos/mios` | Cualquiera |
| Ver el detalle de un pedido puntual | `GET /api/pedidos/{id}` | El dueño del pedido, o un Administrador |
| Ver todos los pedidos (con filtros) | `GET /api/pedidos` | Solo Administrador |
| Cambiar el estado de un pedido | `PATCH /api/pedidos/{id}/estado` | Solo Administrador |
| Ver mis direcciones | `GET /api/direcciones` | Cualquiera |
| Agregar una dirección | `POST /api/direcciones` | Cualquiera |
| Marcar una dirección como predeterminada | `PATCH /api/direcciones/{id}/predeterminada` | Cualquiera |

**Qué pasa exactamente cuando creás un pedido (`POST /api/pedidos`)** — este es el flujo más importante de toda la API:

1. Mira qué hay en tu carrito. Si está vacío, rechaza el pedido.
2. Verifica que la dirección que mandaste exista y sea tuya.
3. Para cada producto del carrito, toma una "foto" del precio vigente en ese momento (si está en oferta, usa el precio de oferta).
4. Descuenta el stock de cada variante comprada.
5. Si en algún momento no hay stock suficiente para alguna línea, **se cancela todo el pedido completo** — no queda nada a medias en la base de datos (ni el pedido, ni el stock descontado de las líneas anteriores). A esto se le llama que la operación es **atómica**: o pasa todo, o no pasa nada.
6. Si todo salió bien, guarda el pedido y vacía el carrito.

Los estados de un pedido solo pueden cambiar en un orden válido: `Pendiente → Enviado → Entregado`, o `Pendiente/Enviado → Cancelado`. No se puede, por ejemplo, "des-entregar" un pedido — la API lo rechaza si se intenta.

### 4.4 Inventario (`/api/inventario`)

Todo este módulo es exclusivo de Administrador.

| Acción | Método y ruta |
|---|---|
| Ver el estado de stock de todas las variantes (con filtros) | `GET /api/inventario/variantes` |
| Registrar una entrada o salida de stock manual | `POST /api/inventario/movimientos` |
| Ver el historial de movimientos | `GET /api/inventario/movimientos` |

Cada variante se clasifica automáticamente como **Sin stock** (0), **Stock bajo** (1 a 5 unidades) o **En stock** (más de 5) — el mismo umbral que ya usaba el panel de administración.

## 5. ¿Cómo sabe la API quién sos? (usuarios y también invitados)

Esta es la parte más particular de este proyecto: en VoltajeModa **podés comprar sin crear una cuenta**. La API respeta esa misma idea.

- Si estás **logueado**, la API te identifica con tu usuario de siempre (la misma cookie de sesión que ya usa el sitio).
- Si sos **invitado**, la primera vez que tocás el carrito la API te asigna automáticamente una cookie propia (`voltajemoda.carrito.anonimo`) con un identificador al azar. Esa cookie viaja sola en cada pedido siguiente — no tenés que hacer nada manualmente, el navegador la maneja solo.
- Cuando un invitado **se registra o inicia sesión**, hay que llamar una sola vez a `POST /api/carrito/fusionar` (ya logueado). Eso toma todo lo que tenías como invitado — carrito, direcciones, pedidos — y lo pasa a tu cuenta nueva, sumando cantidades si ya tenías algo en el carrito de tu cuenta.

## 6. Seguridad: ¿qué es público y qué requiere permisos?

La API **no inventó un sistema de login propio** — reutiliza el mismo que ya tenía el sitio (ASP.NET Core Identity, con los roles `Cliente` y `Administrador`). En la práctica:

- **Sin restricciones**: consultar el catálogo, categorías, tu propio carrito, tus propios pedidos y direcciones.
- **Requiere estar logueado**: fusionar el carrito de invitado.
- **Requiere ser Administrador**: crear/editar/borrar productos y categorías, todo el módulo de inventario, ver todos los pedidos y cambiarles el estado.

Si probás un endpoint de Administrador sin estar logueado como tal, la API responde `401 No autorizado` (o `403` si estás logueado pero con otro rol) en vez de dejarte pasar.

## 7. Cómo probar la API vos mismo

### Opción 1: Swagger (la más fácil)

1. Corré el proyecto normalmente: `dotnet run`.
2. Abrí `https://localhost:7084/swagger` en el navegador (**solo existe en modo Desarrollo**, en producción no está disponible por seguridad).
3. Ahí vas a ver los 6 controladores con todos sus endpoints, y podés probarlos directo desde el navegador con el botón "Try it out".
4. Si primero te logueaste normalmente en el sitio (`/Account/Login`) en la misma pestaña, Swagger va a poder probar también los endpoints de Administrador, porque comparte la misma cookie de sesión del navegador.

### Opción 2: `curl` desde la terminal

Ejemplo real, el flujo completo de un invitado comprando algo:

```powershell
# 1. Agregar un producto al carrito (guarda la cookie de invitado en cookies.txt)
curl.exe -k -c cookies.txt -b cookies.txt -X POST https://localhost:7084/api/carrito/items `
  -H "Content-Type: application/json" `
  -d '{"productoVarianteId":11,"cantidad":2}'

# 2. Confirmar qué quedó en el carrito
curl.exe -k -b cookies.txt https://localhost:7084/api/carrito

# 3. Crear una dirección de envío
curl.exe -k -c cookies.txt -b cookies.txt -X POST https://localhost:7084/api/direcciones `
  -H "Content-Type: application/json" `
  -d '{"calle":"Cra 10 #20-30","ciudad":"Bogota","codigoPostal":"110111"}'
# anotá el "id" que devuelve, por ejemplo 6

# 4. Crear el pedido usando esa dirección
curl.exe -k -c cookies.txt -b cookies.txt -X POST https://localhost:7084/api/pedidos `
  -H "Content-Type: application/json" `
  -d '{"direccionId":6}'

# 5. Confirmar que el carrito quedó vacío
curl.exe -k -b cookies.txt https://localhost:7084/api/carrito
```

El truco de `-c cookies.txt -b cookies.txt` es que `curl` guarda y reenvía la cookie de invitado automáticamente entre llamadas, simulando lo que hace un navegador solo.

## 8. Cómo se ven los errores

Si algo sale mal (un dato inválido, algo que no existe, una regla de negocio que no se cumple), la API **no devuelve una página de error HTML** ni un mensaje críptico: devuelve un JSON estándar con el problema, por ejemplo:

```json
{
  "title": "StockInsuficienteException",
  "detail": "Stock insuficiente para la variante 11: disponible 1, solicitado 5.",
  "status": 409
}
```

El campo `status` es el mismo código HTTP de la respuesta (`400` = dato inválido, `404` = no existe, `409` = conflicto con una regla de negocio, `401`/`403` = falta permiso). Esto hace que sea fácil para cualquier programa que consuma la API mostrar un mensaje de error entendible sin tener que adivinar qué pasó.

## 9. Preguntas frecuentes

**¿Esto reemplaza al sitio web?** No. El sitio Blazor sigue usando sus propios `Services/` tal como siempre. La API es un canal nuevo y adicional, pensado para consumidores que no son el navegador mostrando las páginas `.razor`.

**¿Necesito tocar la base de datos o crear tablas nuevas?** No, la API usa exactamente las mismas tablas que ya existían (`Productos`, `Carritos`, `Pedidos`, etc.) — no se agregó ni duplicó ninguna tabla.

**¿Por qué el carrito y los pedidos son "AllowAnonymous" si hay cosas protegidas por rol en el mismo controlador?** Porque el permiso se decide **endpoint por endpoint**, no por controlador entero — así un mismo controlador puede tener acciones públicas y acciones solo para Administrador sin contradecirse.

**¿Dónde busco si quiero entender una regla de negocio puntual?** En `Core/Dominio/`, en la clase del concepto que te interese (`Producto.cs`, `Pedido.cs`, `Carrito.cs`, `MovimientoStock.cs`). Ahí están escritas en código C# simple, sin nada de base de datos ni de HTTP de por medio.
