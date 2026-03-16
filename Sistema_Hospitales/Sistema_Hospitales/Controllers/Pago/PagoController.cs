using Sistema_Hospitales.Models.viewModels;
using Sistema_Hospitales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace Sistema_Hospitales.Controllers.Pago
{
    public class PagoController : Controller
    {
        // GET: Pago
        public ActionResult mantPagos()
        {

          
            List<mPago> listaPagos = null;

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                listaPagos = (from pagos in db.Pago
                              join tm in db.Tratamiento_Medicamento on pagos.IdTratamiento equals tm.IdTratamiento
                              join med in db.Medicamento on tm.IdMedicamento equals med.IdMedicamento
                              join pc in db.Paciente on pagos.IdPaciente equals pc.IdPaciente
                              select new mPago
                                {
                                    IdPago = pagos.IdPago,
                                    IdPaciente = pagos.IdPaciente,
                                    IdTratamiento = pagos.IdTratamiento.Value,
                                    Fecha = pagos.Fecha.Value,
                                    Monto = pagos.Monto.Value,
                                    MetodoPago = pagos.MetodoPago,
                                    Pagado = pagos.Pagado.Value,
                                    Medicamento = med.Descripcion + " x" +tm.Cantidad.Value,
                                    Paciente = pc.Nombre + " "+pc.Apellido
                                }).ToList();
            }

            return View(listaPagos);

        }


        public ActionResult verPagosPendientes()
        {
            string idPaciente = Session["IdPaciente"].ToString();

            if (string.IsNullOrEmpty(idPaciente))
            {
                return RedirectToAction("Login", "Usuario");
            }

            List<mPago> lista = new List<mPago>();

            using (var db = new SistemaHospitalesEntities1())
            {
                lista = db.Database.SqlQuery<mPago>(
                    "EXEC PagosPendientesPorPaciente @p0", idPaciente).ToList();
            }

            return View(lista);
        }

        [HttpPost]
        public ActionResult pagar(int idPago)
        {
            using (var db = new SistemaHospitalesEntities1())
            {
                var pago = db.Pago.Find(idPago);

                if (pago != null)
                {
                    pago.Pagado = true;
                    db.Entry(pago).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("verPagosPendientes");
        }


        public ActionResult verPagosPaciente(string IdPaciente)
        {
            using (var db = new SistemaHospitalesEntities1())
            {
                List<SelectListItem> pacientes;

                if (Session["IdMedico"] != null)
                {
                    int idMedico = Convert.ToInt32(Session["IdMedico"]);
                    var idHospital = db.Medico
                        .Where(m => m.IdMedico == idMedico)
                        .Select(m => m.IdHospital)
                        .FirstOrDefault();

                    pacientes = db.Paciente
                        .Where(p => p.IdHospital == idHospital)
                        .Select(p => new SelectListItem
                        {
                            Value = p.IdPaciente,
                            Text = p.Nombre + " " + p.Apellido
                        }).ToList();
                }
                else
                {
                    pacientes = db.Paciente
                        .Select(p => new SelectListItem
                        {
                            Value = p.IdPaciente,
                            Text = p.Nombre + " " + p.Apellido
                        }).ToList();
                }

                ViewBag.Pacientes = pacientes;

                if (!string.IsNullOrEmpty(IdPaciente))
                {
                    // Traer los pagos pendientes del paciente seleccionado
                    var pagos = db.Pago
                        .Where(p => p.IdPaciente == IdPaciente && !p.Pagado.Value)
                        .Select(p => new Sistema_Hospitales.Models.viewModels.mPago
                        {
                            Fecha = p.Fecha.Value,
                            Monto = p.Monto.Value,
                            MetodoPago = p.MetodoPago,
                            Pagado = p.Pagado.Value
                        }).ToList();

                    return View(pagos); // Devolvés los pagos como modelo
                }

                return View(); // Si no hay paciente seleccionado, devolvés vista sin modelo
            }
        }

        [HttpPost]
        public ActionResult VerPagosPaciente(string IdPaciente)
        {
            int idMedico = Convert.ToInt32(Session["IdMedico"]);

            using (var db = new SistemaHospitalesEntities1())
            {
                var hospital = db.Medico.Where(m => m.IdMedico == idMedico).Select(m => m.IdHospital).FirstOrDefault();

                // Llenar el dropdown nuevamente
                var pacientes = db.Paciente
                    .Where(p => p.IdHospital == hospital)
                    .Select(p => new SelectListItem
                    {
                        Value = p.IdPaciente,
                        Text = p.Nombre + " " + p.Apellido
                    }).ToList();
                ViewBag.Pacientes = pacientes;

                // Validación: que el paciente sea del mismo hospital
                var pacienteHospital = db.Paciente.Where(p => p.IdPaciente == IdPaciente).Select(p => p.IdHospital).FirstOrDefault();
                if (pacienteHospital != hospital)
                {
                    ModelState.AddModelError("", "El paciente no pertenece al mismo hospital que el médico.");
                    return View(new List<mPago>());
                }

                List<mPago> pagos = db.Database.SqlQuery<mPago>(
                    "EXEC PagosPendientesPorPaciente @p0", IdPaciente).ToList();

                return View(pagos);
            }
        }

        public ActionResult TotalPagos(string idPaciente, DateTime? fechaInicio, DateTime? fechaFin)
        {
            using (var db = new SistemaHospitalesEntities1())
            {
                // Siempre cargar la lista de pacientes para evitar el error en la vista
                ViewBag.Pacientes = db.Paciente.Select(p => new SelectListItem
                {
                    Value = p.IdPaciente,
                    Text = p.Nombre + " " + p.Apellido + " - " + p.IdPaciente // Formato a mostrar
                }).ToList();


                if (string.IsNullOrEmpty(idPaciente) || !fechaInicio.HasValue || !fechaFin.HasValue)
                {
                    ViewBag.Mensaje = "Debe seleccionar un paciente y un rango de fechas.";
                    return View();
                }

                try
                {
                    var resultado = db.Database.SqlQuery<decimal?>(
                        "EXEC TotalPagosRealizadosPorPaciente @IdPaciente, @FechaInicio, @FechaFin",
                        new SqlParameter("@IdPaciente", idPaciente),
                        new SqlParameter("@FechaInicio", fechaInicio),
                        new SqlParameter("@FechaFin", fechaFin)
                    ).FirstOrDefault();

                    ViewBag.TotalPagado = resultado ?? 0;
                }
                catch (Exception ex)
                {
                    ViewBag.Mensaje = "Error al ejecutar el procedimiento: " + ex.Message;
                }
            }

            return View();
        }



        [HttpGet]
        public ActionResult agregarPagos()
        {
        
            return View();
        }

        [HttpPost]
        public ActionResult AgregarPagos(mPago pago)
        {
            

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(pago);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    Sistema_Hospitales.Models.Pago p = new Sistema_Hospitales.Models.Pago
                    {
                        IdPago = pago.IdPago,
                        IdPaciente = pago.IdPaciente,
                        IdTratamiento = pago.IdTratamiento,
                        Fecha = pago.Fecha,
                        Monto = pago.Monto,
                        MetodoPago = pago.MetodoPago,
                        Pagado = pago.Pagado
                    };

                    db.Pago.Add(p);

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Pago agregado exitosamente!";
                }

                return View(pago);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Hubo un error al agregar el pago deseado " + ex;
                return View(pago);
            }

        }

        [HttpGet]
        public ActionResult actualizarPagos(int id)
        {
       

            mPago pag = new mPago();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var pago = db.Pago.Find(id);

                pag.IdPago = pago.IdPago;
                pag.IdPaciente = pago.IdPaciente;
                pag.IdTratamiento = pago.IdTratamiento.Value;
                pag.Fecha = pago.Fecha.Value;
                pag.Monto = pago.Monto.Value;
                pag.MetodoPago = pago.MetodoPago;
                pag.Pagado = pago.Pagado.Value;
            }

            return View(pag);
        }

        [HttpPost]
        public ActionResult actualizarPagos(mPago pag)
        {
           

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(pag);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    var pago = db.Pago.Find(pag.IdPago);

                    pago.IdPaciente = pag.IdPaciente;
                    pago.IdTratamiento = pag.IdTratamiento;
                    pago.Fecha = pag.Fecha;
                    pago.Monto = pag.Monto;
                    pago.MetodoPago = pago.MetodoPago;
                    pago.Pagado = pag.Pagado;

                    db.Entry(pago).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Pago actualizado correctamente";
                }

                return View(pag);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Falló al actualizar el pago " + ex;
                return View(pag);
            }
        }

        

        [HttpGet]
        public ActionResult consultarPagos(int id)
        {

         

            mPago pag = new mPago();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {

                var pago = db.Pago.Find(id);

                pag.IdPago = pago.IdPago;
                pag.IdPaciente = pago.IdPaciente;
                pag.IdTratamiento = pago.IdTratamiento.Value;
                pag.Fecha = pago.Fecha.Value;
                pag.Monto = pago.Monto.Value;
                pag.MetodoPago = pago.MetodoPago;
                pag.Pagado = pago.Pagado.Value;

            }

            return View(pag);

        }

        [HttpGet]
        public ActionResult eliminarPagos(int id)
        {
            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var pago = db.Pago.Find(id);

                db.Pago.Remove(pago);

                db.SaveChanges();
            }
            return RedirectToAction("mantPagos", "Pago");
        }
    }
}