using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Services;
using System.Web.UI.WebControls;
using ws_Mantenimiento.Models;

namespace ws_Mantenimiento
{
    /// <summary>
    /// Descripción breve de wsMantenimiento
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class wsMantenimiento : System.Web.Services.WebService
    {
        public class ErrorDetails
        {
            public HttpStatusCode StatusCode { get; set; }
            public string Message { get; set; }
            public string ExceptionMessage { get; set; }
            public string StackTrace { get; set; }
        }

        public class CustomSoapException : Exception
        {
            public ErrorDetails ErrorDetails { get; }

            public CustomSoapException(string message, ErrorDetails errorDetails) : base(message)
            {
                ErrorDetails = errorDetails;
            }
        }

        [WebMethod]
        public List<MANTENIMIENTO> GetRequests()
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    List<MANTENIMIENTO> mantenimientos = dbContext.MANTENIMIENTO.ToList();
                    return mantenimientos;
                }
            }
            catch (Exception ex)
            {
                ErrorDetails errorResponse = new ErrorDetails
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Se produjo un error al obtener los mantenimientos.",
                    ExceptionMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new CustomSoapException(errorResponse.Message, errorResponse);
            }
        }

        [WebMethod]
        public int AddRequest(string fecha_solicitud, string tipo_mantenimiento, string estado_mantenimiento, long cliente_id, long libro_id, string comentario)
        {
            try
            {

                DateTime solicitudDateTime;

                // Verificar si la cadena puede convertirse en un DateTime
                if (!DateTime.TryParse(fecha_solicitud, out solicitudDateTime))
                {
                    // La cadena no se puede convertir a DateTime, intentar conversión adicional
                    if (!DateTime.TryParseExact(fecha_solicitud, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out solicitudDateTime))
                    {
                        // No se pudo convertir la cadena en un DateTime válido
                        throw new ArgumentException("El formato de la fecha de solicitud es incorrecto.");
                    }
                }


                using (var dbContext = new Entities())
                {
                    var mantenimineto = new MANTENIMIENTO()
                    {
                        FECHA_SOLICITUD = solicitudDateTime,
                        TIPO_MANTENIMIENTO = tipo_mantenimiento,
                        ESTADO_MANTENIMIENTO = estado_mantenimiento,
                        CLIENTE_ID = cliente_id,
                        LIBRO_ID = libro_id,
                        COMENTARIO = comentario
                    };

                    dbContext.MANTENIMIENTO.Add(mantenimineto);
                    dbContext.SaveChanges();

                    if (mantenimineto.ID_MANTENIMIENTO > 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorDetails errorResponse = new ErrorDetails
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Se produjo un error al crear una solicitud.",
                    ExceptionMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new CustomSoapException(errorResponse.Message, errorResponse);
            }
        }

        [WebMethod]
        public int ModifyRequest(int mantenimientoId, string nuevoEstadoMantenimiento, string respuesta)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    // Buscar el mantenimiento por su ID
                    var mantenimiento = dbContext.MANTENIMIENTO.FirstOrDefault(m => m.ID_MANTENIMIENTO == mantenimientoId);

                    if (mantenimiento != null)
                    {
                        // Modificar los campos deseados
                        mantenimiento.ESTADO_MANTENIMIENTO = nuevoEstadoMantenimiento;
                        mantenimiento.RESPUESTA = respuesta;

                        dbContext.SaveChanges();

                        return 1;
                    }
                    else
                    {
                        return 0; // No se encontró el mantenimiento con el ID especificado
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDetails errorResponse = new ErrorDetails
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Se produjo un error al modificar la solicitud.",
                    ExceptionMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new CustomSoapException(errorResponse.Message, errorResponse);
            }
        }





























    }
}