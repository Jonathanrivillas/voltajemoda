# Ejemplos de JSON para probar cada endpoint manualmente en Postman

Este archivo es una referencia rápida: por cada endpoint tenés el método, la URL, el body de ejemplo (si aplica) y qué esperar de respuesta. Pegá la URL y el JSON directo en Postman (`Body` → `raw` → `JSON`) como vimos.

`{{baseUrl}}` = `http://localhost:5123` (o `https://localhost:7084` si corrés con `--launch-profile https`). Reemplazalo por la URL real, Postman no expande esa variable a menos que la definas en el entorno.

Los enums (`NuevoEstado`, `Tipo`) se mandan como **texto** (`"Enviado"`, `"Salida"`), no como número — esto acaba de arreglarse en el proyecto, así que si te da error de deserialización, reiniciá `dotnet run`.

---

## 🟢 Productos (`/api/productos`) — lectura pública, escritura solo Administrador

### Listar productos
```
GET {{baseUrl}}/api/productos
GET {{baseUrl}}/api/productos?categoriaId=2&busqueda=camisa&destacado=true&enOferta=false&pagina=1&tamanoPagina=20
```
Sin body. Respuesta esperada: `200` con `{ "items": [...], "total": N, "pagina": 1, "tamanoPagina": 20 }`.

### Ver un producto puntual
```
GET {{baseUrl}}/api/productos/4
```
`200` con el detalle (incluye `variantes` e `imagenes`), o `404` si no existe.

### Crear producto — **requiere Administrador**
```
POST {{baseUrl}}/api/productos
```
```json
{
  "nombre": "Chaqueta de cuero",
  "descripcion": "Chaqueta de cuero sintético, corte moderno",
  "precio": 250000,
  "imagenUrl": "/uploads/productos/chaqueta.jpg",
  "categoriaId": 1,
  "destacado": true,
  "esNuevo": true
}
```
`201 Created` con el producto creado (incluye su `id`).

### Editar producto — **Administrador**
```
PUT {{baseUrl}}/api/productos/4
```
```json
{
  "nombre": "Chaqueta de cuero (editada)",
  "descripcion": "Descripción actualizada",
  "precio": 260000,
  "imagenUrl": "/uploads/productos/chaqueta.jpg",
  "categoriaId": 1,
  "destacado": false,
  "esNuevo": false
}
```
`204 No Content` si salió bien.

### Borrar producto — **Administrador**
```
DELETE {{baseUrl}}/api/productos/4
```
Sin body. `204 No Content`.

### Poner en oferta — **Administrador**
```
POST {{baseUrl}}/api/productos/4/oferta
```
```json
{ "precioOferta": 199900 }
```
`200` con el producto actualizado. Da `400` si `precioOferta` es mayor o igual al precio de lista.

### Quitar oferta — **Administrador**
```
DELETE {{baseUrl}}/api/productos/4/oferta
```
Sin body. `204 No Content`.

### Agregar variante (talla/color) — **Administrador**
```
POST {{baseUrl}}/api/productos/4/variantes
```
```json
{
  "talla": "XL",
  "color": "Negro",
  "stockInicial": 15
}
```
`201 Created` con la variante creada (incluye su `id`). Da `400` si ya existe una variante con esa misma combinación talla+color.

### Eliminar variante — **Administrador**
```
DELETE {{baseUrl}}/api/productos/4/variantes/15
```
Sin body. `204 No Content`. Da `409` si la variante está en un carrito, tiene pedidos activos (no cancelados) o movimientos de stock registrados — mismas reglas que ya validaba el panel de administración.

### Agregar imagen (por URL) — **Administrador**
```
POST {{baseUrl}}/api/productos/4/imagenes
```
```json
{
  "url": "/uploads/productos/chaqueta-2.jpg",
  "orden": 1
}
```
`201 Created`.

### Eliminar imagen — **Administrador**
```
DELETE {{baseUrl}}/api/productos/4/imagenes/2
```
Sin body. `204 No Content`. Sin restricciones adicionales.

### Subir el archivo de la imagen principal — **Administrador**
```
POST {{baseUrl}}/api/productos/4/imagen-principal
```
No es JSON: en Postman, pestaña **Body** → **form-data** → agregá una fila con key `archivo`, cambiá el tipo de esa fila de "Text" a **"File"** (aparece un desplegable a la derecha del campo) y elegí el archivo de imagen desde tu disco.
`200` con el producto actualizado (`imagenUrl` apunta al archivo recién guardado). `400` si el formato no es JPG/PNG/WEBP/GIF o pesa más de 5 MB.

### Subir el archivo de una imagen adicional — **Administrador**
```
POST {{baseUrl}}/api/productos/4/imagenes/archivo
```
En **Body** → **form-data**: una fila `archivo` (tipo **File**, elegí el archivo) y otra fila `orden` (tipo Text, ej. `2`).
`201 Created` con la imagen creada. Mismas validaciones de formato/tamaño que la imagen principal.

---

## 🟢 Categorías (`/api/categorias`)

### Listar
```
GET {{baseUrl}}/api/categorias
```
`200` con un arreglo `[{ "id": 1, "nombre": "Camisas" }, ...]`.

### Crear — **Administrador**
```
POST {{baseUrl}}/api/categorias
```
```json
{ "nombre": "Accesorios" }
```
`201 Created`.

### Editar — **Administrador**
```
PUT {{baseUrl}}/api/categorias/5
```
```json
{ "nombre": "Accesorios y complementos" }
```
`200` con la categoría actualizada.

### Eliminar — **Administrador**
```
DELETE {{baseUrl}}/api/categorias/5
```
Sin body. `204 No Content`. Da `409` si la categoría todavía tiene productos asociados.

---

## 🟡 Carrito (`/api/carrito`) — público, funciona con o sin cuenta

### Ver mi carrito
```
GET {{baseUrl}}/api/carrito
```
Sin body. `200` con `{ "id": N, "items": [...] }`.

### Agregar un producto
```
POST {{baseUrl}}/api/carrito/items
```
```json
{
  "productoVarianteId": 11,
  "cantidad": 2
}
```
`200` con el carrito actualizado. `404` si la variante no existe, `400` si `cantidad` es 0 o negativa.

### Cambiar la cantidad de un ítem
```
PUT {{baseUrl}}/api/carrito/items/11
```
```json
{ "cantidad": 5 }
```
`200`. Si mandás `cantidad: 0` el ítem se elimina del carrito. `404` si ese ítem no está en el carrito.

### Quitar un ítem
```
DELETE {{baseUrl}}/api/carrito/items/11
```
Sin body. `204 No Content`. `404` si no estaba en el carrito.

### Vaciar el carrito
```
DELETE {{baseUrl}}/api/carrito
```
Sin body. `204 No Content`.

### Fusionar carrito de invitado con mi cuenta — **requiere estar logueado** (cualquier rol)
```
POST {{baseUrl}}/api/carrito/fusionar
```
Sin body. `204 No Content`. Ver la sección de administrador más abajo para saber cómo mandar la cookie de sesión.

---

## 🟡 Pedidos (`/api/pedidos`) — crear/ver propios es público, listar todos y cambiar estado es Administrador

### Crear un pedido desde mi carrito
```
POST {{baseUrl}}/api/pedidos
```
```json
{ "direccionId": 6 }
```
`201 Created` con el pedido. `409` si el carrito está vacío o si el stock no alcanza, `404` si la dirección no existe o no es tuya.

### Ver mis pedidos
```
GET {{baseUrl}}/api/pedidos/mios
```
Sin body. `200` con un arreglo de pedidos.

### Ver el detalle de un pedido
```
GET {{baseUrl}}/api/pedidos/5
```
Sin body. `200` si el pedido es tuyo (o sos Administrador), `404` si no existe o es de otro.

### Listar todos los pedidos — **Administrador**
```
GET {{baseUrl}}/api/pedidos
GET {{baseUrl}}/api/pedidos?estado=Pendiente&desde=2026-01-01&hasta=2026-12-31&pagina=1&tamanoPagina=20
```
Sin body. `200` con `{ "items": [...], "total": N, ... }`.

### Cambiar el estado de un pedido — **Administrador**
```
PATCH {{baseUrl}}/api/pedidos/5/estado
```
```json
{ "nuevoEstado": "Enviado" }
```
Valores válidos: `"Pendiente"`, `"Enviado"`, `"Entregado"`, `"Cancelado"`. `200` con el pedido actualizado. `409` si la transición no es válida (ej. pasar de `Entregado` a `Pendiente`).

### Eliminar un pedido — **Administrador**
```
DELETE {{baseUrl}}/api/pedidos/5
```
Sin body. `204 No Content` **solo si el pedido ya está en estado `Cancelado`**. Si intentás borrar uno `Pendiente`, `Enviado` o `Entregado`, da `409` — primero hay que cambiarle el estado a `Cancelado` con el endpoint de arriba.

---

## 🟡 Direcciones (`/api/direcciones`) — público

### Listar mis direcciones
```
GET {{baseUrl}}/api/direcciones
```
Sin body. `200` con un arreglo.

### Crear una dirección
```
POST {{baseUrl}}/api/direcciones
```
```json
{
  "etiqueta": "Casa",
  "nombreDestinatario": "Jonathan Peña",
  "telefono": "3001234567",
  "calle": "Cra 10 #20-30",
  "ciudad": "Bogota",
  "codigoPostal": "110111",
  "esPredeterminada": true
}
```
`201 Created`. `400` si `calle`, `ciudad` o `codigoPostal` van vacíos.

### Marcar como predeterminada
```
PATCH {{baseUrl}}/api/direcciones/6/predeterminada
```
Sin body. `204 No Content`. `404` si no existe o no es tuya.

### Eliminar una dirección
```
DELETE {{baseUrl}}/api/direcciones/6
```
Sin body. `204 No Content`. Da `409` si la dirección tiene algún pedido asociado (ni siquiera cancelado la libera — mismo criterio que ya usa `Account/Manage/Direcciones.razor`).

---

## 🔴 Inventario (`/api/inventario`) — **TODO el módulo requiere Administrador**

### Ver estado de stock de las variantes
```
GET {{baseUrl}}/api/inventario/variantes
GET {{baseUrl}}/api/inventario/variantes?categoriaId=2&busqueda=camisa&estado=StockBajo
```
Sin body. `200` con un arreglo. `estado` acepta `SinStock`, `StockBajo`, `EnStock`.

### Registrar un movimiento de stock manual
```
POST {{baseUrl}}/api/inventario/movimientos
```
```json
{
  "productoVarianteId": 11,
  "tipo": "Entrada",
  "cantidad": 20,
  "motivo": "Reposición de proveedor"
}
```
Valores válidos de `tipo`: `"Entrada"`, `"Salida"`. `201 Created` con el movimiento registrado (incluye `stockResultante`). `400` si `cantidad` es 0 o negativa, `409` si es una `"Salida"` y no hay stock suficiente.

### Ver historial de movimientos
```
GET {{baseUrl}}/api/inventario/movimientos
GET {{baseUrl}}/api/inventario/movimientos?productoVarianteId=11&desde=2026-01-01&hasta=2026-12-31&tipo=Salida&pagina=1&tamanoPagina=20
```
Sin body. `200` con `{ "items": [...], "total": N, ... }`.

---

## 🔑 Cómo probar los endpoints de Administrador (🔴 y los marcados arriba)

Todos los que dicen **"Administrador"** te van a devolver `401 No autorizado` si los mandás tal cual — les falta la cookie de sesión de un usuario con ese rol. Pasos para conseguirla:

1. **Andá al navegador** (Chrome/Edge) y entrá a `http://localhost:5123/Account/Login`.
2. Logueate con el usuario administrador de prueba que ya viene sembrado en desarrollo:
   - **Email:** `admin@voltajemoda.com`
   - **Contraseña:** `Admin123!` (la del README/seed original es distinta; esta es la vigente en la base de desarrollo actual — si te vuelve a fallar, confirmala con el equipo)
3. Una vez logueado (te va a redirigir al home), abrí las **herramientas de desarrollador** con `F12`.
4. Andá a la pestaña **Application** (en Chrome/Edge; en Firefox se llama **Storage**) → en el panel izquierdo, **Cookies** → hacé clic en `http://localhost:5123`.
5. Buscá la fila cuyo **Name** es `.AspNetCore.Identity.Application` y copiá todo su **Value** (es un texto largo, tipo `CfDJ8...`).
6. Volvé a Postman. Abrí (o creá) la request que querés probar como admin — por ejemplo `POST {{baseUrl}}/api/productos`.
7. Debajo de la URL, hacé clic en la pestaña **"Cookies"**.
8. Hacé clic en **"Add Cookie"** (o el link que diga algo como "Manage Cookies" → **Add Cookie**).
9. Completá:
   - **Domain:** `localhost`
   - **Name:** `.AspNetCore.Identity.Application`
   - **Value:** el valor que copiaste del navegador
   - **Path:** `/`
10. Guardá y volvé a mandar la request (**Send**). Ahora, en vez de `401`, deberías recibir el resultado real (`201`, `200`, `400`, `409` según el caso).

**Importante:** esa cookie tiene fecha de vencimiento y se invalida si cerrás sesión en el navegador o el servidor se reinicia y regenera claves — si en algún momento vuelve a darte `401` después de que funcionaba, repetí los pasos 1 a 6 para conseguir una cookie nueva.

**Atajo para no repetir esto a mano en cada request:** una vez que agregaste la cookie en una request, Postman la guarda en su "Cookie Jar" para ese dominio (`localhost`) — las demás requests que apunten a `{{baseUrl}}` (mismo `localhost`) ya la van a mandar solas automáticamente, no hace falta repetir el paso 6-10 en cada una.
