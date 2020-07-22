
using Microsoft.AspNetCore.Builder;

namespace Epm.Cec.Api.Middleware
{
    public  static class MiddlewareExcepciones
    {

        /// <summary>
        /// Agrega el middleware GestionExcepciones
        /// </summary>
        /// <param name="app"></param>
        public static void UseGestorExepciones(this IApplicationBuilder app)
        {
            app.UseMiddleware<GestionExcepciones>();
        }

    }
}
