using Sistema_Hospitales.Models.viewModels;
using Sistema_Hospitales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema_Hospitales.Controllers.Cita
{
    public class CitaController : Controller
    {
        // GET: Cita
        public ActionResult mantCitas()
        {
            List<mCita> listCitas = null;

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                listCitas = (from citas in db.Cita
                             join med in db.Medico on citas.IdMedico equals med.IdMedico
                             join hosp in db.Hospital on citas.IdHospital equals hosp.IdHospital
                             join pac in db.Paciente on citas.IdPaciente equals pac.IdPaciente
                             select new mCita
                             {
                                 IdCita = citas.IdCita,
                                 Fecha = citas.Fecha,
                                 Hora = citas.Hora.Value,
                                 IdMedico = citas.IdMedico.Value,
                                 IdPaciente = citas.IdPaciente,
                                 IdHospital = citas.IdHospital.Value,
                                 Diagnostico = citas.Diagnostico,
                                 Hospital = hosp.Nombre,
                                 Medico = med.Nombre + " - " + med.Especialidad,
                                 Paciente = pac.Nombre + " " + pac.Apellido
                             }).ToList();
            }
            llenarListas();
            return View(listCitas);

        }

        public ActionResult verCitas()
        {
            string idPaciente = Session["IdPaciente"].ToString();

            List<mCita> listCitas = null;

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                listCitas = (from citas in db.Cita
                             where citas.IdPaciente == idPaciente
                             join med in db.Medico on citas.IdMedico equals med.IdMedico
                             join hosp in db.Hospital on citas.IdHospital equals hosp.IdHospital
                             select new mCita
                             {
                                 IdCita = citas.IdCita,
                                 Fecha = citas.Fecha,
                                 Hora = citas.Hora.Value,
                                 IdMedico = citas.IdMedico.Value,
                                 IdPaciente = citas.IdPaciente,
                                 IdHospital = citas.IdHospital.Value,
                                 Diagnostico = citas.Diagnostico,
                                 Hospital = hosp.Nombre,
                                 Medico = med.Nombre + " - " + med.Especialidad
                             }).ToList();
            }
            llenarListas();
            return View(listCitas);

        }


        [HttpGet]
        public ActionResult agregarCitas()
        {
            ViewBag.IdMedico = Session["IdMedico"];
            llenarListas();
            return View();
        }

        [HttpPost]
        public ActionResult AgregarCitas(mCita cita)
        {
            try
            {
                if (Session["IdMedico"] != null)
                {
                    if (!ModelState.IsValid)
                    {
                        llenarListas();
                        return View(cita);
                    }

                    using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                    {
                        var result = db.Database.ExecuteSqlCommand(
                            "EXEC RegistrarNuevaCita @p0, @p1, @p2, @p3, @p4, @p5",
                            cita.Fecha,
                            cita.Hora,
                            Session["IdMedico"],
                            cita.IdPaciente,
                            cita.IdHospital,
                            cita.Diagnostico
                        );

                        ViewBag.ValorMensaje = 1;
                        ViewBag.MensajeProceso = "Cita registrada exitosamente!";
                    }
                }
                else
                {
                    if (!ModelState.IsValid)
                    {
                        llenarListas();
                        return View(cita);
                    }

                    using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                    {
                        var result = db.Database.ExecuteSqlCommand(
                            "EXEC RegistrarNuevaCita @p0, @p1, @p2, @p3, @p4, @p5",
                            cita.Fecha,
                            cita.Hora,
                            cita.IdMedico,
                            cita.IdPaciente,
                            cita.IdHospital,
                            cita.Diagnostico
                        );

                        ViewBag.ValorMensaje = 1;
                        ViewBag.MensajeProceso = "Cita registrada exitosamente!";
                    }
                }

                llenarListas();
                return View(cita);
            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Hubo un error al registrar la Cita: " + ex.Message;
                llenarListas();
                return View(cita);
            }
        }

        [HttpGet]
        public ActionResult actualizarCita(int id)
        {


            mCita cit = new mCita();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var cita = db.Cita.Find(id);

                cit.IdCita = cita.IdCita;
                cit.Fecha = cita.Fecha.Value;
                cit.Hora = cita.Hora.Value;
                cit.IdMedico = cita.IdMedico.Value;
                cit.IdPaciente = cita.IdPaciente;
                cit.IdHospital = cita.IdHospital.Value;
                cit.Diagnostico = cita.Diagnostico;
            }
            llenarListas();
            return View(cit);
        }

        [HttpPost]
        public ActionResult actualizarCita(mCita cit)
        {


            try
            {
                if (!ModelState.IsValid)
                {
                    llenarListas();
                    return View(cit);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    var cita = db.Cita.Find(cit.IdCita);

                    cita.Fecha = cit.Fecha;
                    cita.Hora = cit.Hora;
                    cita.IdMedico = cit.IdMedico;
                    cita.IdPaciente = cit.IdPaciente;
                    cita.IdHospital = cit.IdHospital;
                    cita.Diagnostico = cit.Diagnostico;

                    db.Entry(cita).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Cita actualizada correctamente";
                }
                llenarListas();
                return View(cit);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Falló al actualizar la Cita " + ex;
                llenarListas();
                return View(cit);
            }
        }

        [HttpGet]
        public ActionResult consultarCitas(int id)
        {


            mCita cit = new mCita();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {

                var cita = db.Cita.Find(id);

                cit.IdCita = cita.IdCita;
                cit.Fecha = cita.Fecha.Value;
                cit.Hora = cita.Hora.Value;
                cit.IdMedico = cita.IdMedico.Value;
                cit.IdPaciente = cita.IdPaciente;
                cit.IdHospital = cita.IdHospital.Value;
                cit.Diagnostico = cita.Diagnostico;

            }
            llenarListas();
            return View(cit);

        }

        [HttpGet]
        public ActionResult eliminarCitas(int id)
        {
            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var cita = db.Cita.Find(id);

                db.Cita.Remove(cita);

                db.SaveChanges();
            }
            return RedirectToAction("mantCitas", "Cita");
        }

        [HttpGet]
        public ActionResult ObtenerPacientesPorMedico(int? idMedico, int? idHospital, DateTime? fechaInicio, DateTime? fechaFin)
        {
            llenarListas(); // Llena los dropdowns primero

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                // Si hay datos enviados, ejecutamos el SP
                if (idMedico.HasValue && idHospital.HasValue && fechaInicio.HasValue && fechaFin.HasValue)
                {
                    var pacientes = db.Database.SqlQuery<mPaciente>(
                        "EXEC ObtenerPacientesPorMedico @p0, @p1, @p2, @p3",
                        idMedico, idHospital, fechaInicio, fechaFin).ToList();

                    ViewBag.ResultadosPacientes = pacientes; // Cambiamos el nombre para evitar conflicto
                }
            }

            return View();
        }

        private void llenarListas()
        {
            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                // Dropdown de pacientes (Id + Nombre Completo)
                ViewBag.Pacientes = db.Paciente.Select(p => new SelectListItem
                {
                    Value = p.IdPaciente,
                    Text = p.IdPaciente + " - " + p.Nombre + " " + p.Apellido
                }).ToList();

                // Dropdown de medicos (Id + Nombre + Especialidad)
                ViewBag.Medicos = db.Medico.Select(m => new SelectListItem
                {
                    Value = m.IdMedico.ToString(),
                    Text = m.IdMedico + " - " + m.Nombre + " " + m.Especialidad
                }).ToList();

                // Dropdown de hospitales
                ViewBag.Hospitales = db.Hospital.Select(h => new SelectListItem
                {
                    Value = h.IdHospital.ToString(),
                    Text = h.IdHospital + " - " + h.Nombre
                }).ToList();
            }

        }
    }
}