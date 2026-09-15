# Guía de contribución — Voltaje Moda

## Ramas

- **`main`** — rama estable, lo que está en producción. Nadie hace push directo aquí.
- **`desarrollo`** — rama de integración. Todas las Pull Requests apuntan aquí. Cuando `desarrollo` está estable, se promueve a `main` para desplegar (lo coordina el dueño del proyecto).
- **Ramas de feature** — una por tarea, creadas desde `desarrollo`:
  - `feature/nombre-corto` para funcionalidad nueva (ej. `feature/cupones-descuento`)
  - `fix/nombre-corto` para corrección de bugs (ej. `fix/stock-negativo-checkout`)

Flujo para una tarea nueva:

```powershell
git checkout desarrollo
git pull
git checkout -b feature/nombre-de-tu-tarea
# ... trabajas, commits ...
git push -u origin feature/nombre-de-tu-tarea
```

Luego abres una **Pull Request hacia `desarrollo`** (no hacia `main`) en GitHub, para que se revise antes de integrarse.

## Pull Requests

- Describe brevemente qué cambia y por qué.
- Si tu cambio requiere una migración de base de datos, dilo explícitamente en la descripción del PR (ver siguiente sección — tú no generas la migración).
- Espera al menos una revisión antes de mergear, incluso si técnicamente podrías hacerlo tú mismo.

## Migraciones de base de datos

**Las migraciones (`dotnet ef migrations add` / `dotnet ef database update`) las genera y aplica únicamente el dueño del proyecto.** No porque el resto del equipo no sepa hacerlo, sino porque varias personas generando migraciones en paralelo sobre el mismo modelo en evolución termina en conflictos y una base de datos con historial inconsistente entre máquinas.

**Mecanismo que lo hace cumplir (no solo una convención):** la carpeta `Migrations/` está protegida por `.github/CODEOWNERS`, apuntando al dueño del proyecto. Con la regla de protección de rama activada en GitHub (ver abajo), **ningún PR que toque esa carpeta se puede mergear sin su aprobación explícita** — es un control real, no solo una nota en este documento.

**Cómo trabajar si tu tarea necesita cambiar el modelo de datos:**

1. Modifica las clases en `Models/` normalmente, como parte de tu feature.
2. **No corras `dotnet ef migrations add`.** Deja el PR sin migración, y menciona en la descripción qué cambió en el modelo y por qué.
3. El dueño del proyecto revisa, genera la migración (`dotnet ef migrations add NombreDescriptivo`), la prueba contra su base local, y la agrega al PR (o a un commit de seguimiento) antes de aprobar el merge.

### Configuración pendiente en GitHub (una sola vez, la hace el dueño del repo)

En `Settings → Branches → Add branch protection rule` para `main` (y opcionalmente `desarrollo`):

- Branch name pattern: `main`
- Activar **"Require a pull request before merging"**
- Activar **"Require review from Code Owners"**
- Guardar

Esto es lo que convierte el `CODEOWNERS` en una regla real y no solo documentación.

## Convenciones del código

Ver la sección "Convenciones a tener en cuenta" en el [README](./README.md).
