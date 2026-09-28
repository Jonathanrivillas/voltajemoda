using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using VoltajeModa.Core.Dominio.Comun;

namespace VoltajeModa.Presentacion.Filtros;

// Registrado solo en AddControllers(): no interfiere con app.UseExceptionHandler("/Error") ni con
// UseStatusCodePagesWithReExecute, que siguen manejando los errores de las páginas Razor de Blazor.
public class ManejadorExcepcionesFiltro : IExceptionFilter
{
    private readonly ILogger<ManejadorExcepcionesFiltro> _logger;

    public ManejadorExcepcionesFiltro(ILogger<ManejadorExcepcionesFiltro> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ExcepcionDominio excepcionDominio)
        {
            context.Result = new ObjectResult(new ProblemDetails
            {
                Title = excepcionDominio.GetType().Name,
                Detail = excepcionDominio.Message,
                Status = excepcionDominio.CodigoHttp
            })
            {
                StatusCode = excepcionDominio.CodigoHttp
            };
            context.ExceptionHandled = true;
            return;
        }

        if (context.Exception is DbUpdateConcurrencyException)
        {
            context.Result = new ObjectResult(new ProblemDetails
            {
                Title = "ConflictoDeConcurrencia",
                Detail = "El recurso fue modificado por otra solicitud. Intenta de nuevo.",
                Status = StatusCodes.Status409Conflict
            })
            {
                StatusCode = StatusCodes.Status409Conflict
            };
            context.ExceptionHandled = true;
            return;
        }

        _logger.LogError(context.Exception, "Error no controlado en la API de VoltajeModa.");
        context.Result = new ObjectResult(new ProblemDetails
        {
            Title = "ErrorInterno",
            Detail = "Ocurrió un error inesperado procesando la solicitud.",
            Status = StatusCodes.Status500InternalServerError
        })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
        context.ExceptionHandled = true;
    }
}
