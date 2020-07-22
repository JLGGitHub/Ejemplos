
using Epm.Cec.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;


namespace Epm.Cec.Api.Middleware
{
    public class GestionExcepciones
    {
        /// <summary>
        /// Objeto con la información del request
        /// </summary>
        private readonly RequestDelegate request;

        /// <summary>
        /// Objeto para gestionar los logs de la aplicación
        /// </summary>
        private readonly ILogger<GestionExcepciones> logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="logger">Objeto para gestionar los logs de la aplicación</param>
        public GestionExcepciones(RequestDelegate request, ILogger<GestionExcepciones> logger)
        {

            this.logger = logger;
            this.request = request;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await request(httpContext);
            }
            catch (CusException excepcionCEC)
            {
                await ManejarExcepcionCEC(httpContext, excepcionCEC);
            }
            catch (Exception ex)
            {
                await ManejarExcepcionAsync(httpContext, ex);
            }
        }

        /// <summary>
        /// Maneja excepciones genericas
        /// </summary>
        /// <param name="context">Información del request</param>
        /// <param name="excepcion">Excepción</param>
        /// <returns></returns>
        private Task ManejarExcepcionAsync(HttpContext context, Exception excepcion)
        {
            string recurso = context.Request.Path;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            RespuestaError respuesta = GestionErrores.ConstruirRespuestaError(excepcion);
            logger.LogError(excepcion, respuesta.ToJson());
            return context.Response.WriteAsync(respuesta.ToJson());
        }

        /// <summary>
        /// Maneja excepciones de tipo ExcepcionCEC
        /// </summary>
        /// <param name="context">Información del request</param>
        /// <param name="excepcionCEC">Excepcion</param>
        /// <returns></returns>
        private Task ManejarExcepcionCEC(HttpContext context, CusException excepcionCEC)
        {
            string recurso = context.Request.Path;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            RespuestaError respuesta = GestionErrores.ConstruirRespuestaError(excepcionCEC);
            logger.LogError(excepcionCEC, respuesta.ToJson());
            return context.Response.WriteAsync(respuesta.ToJson());
        }
    }
}
