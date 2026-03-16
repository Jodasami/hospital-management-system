using Sistema_Hospitales.Models.viewModels;
using Sistema_Hospitales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema_Hospitales.Controllers.Tratamiento
{
    public class TratamientoController : Controller
    {
        // GET: Tratamiento
        public ActionResult mantTratamientos()
        {

            List<mTratamiento> listTratamiento = null;

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {

                listTratamiento = (from t in db.Tratamiento
                                join c in db.Cita on t.IdCita equals c.IdCita
                                join p in db.Paciente on c.IdPaciente equals p.IdPaciente
                                select new mTratamiento
                                {
                                    IdTratamiento = t.IdTratamiento,
                                    IdCita = t.IdCita.Value,
                                    CostoTotal = t.CostoTotal.Value,
                                    NombrePaciente = p.Nombre + " " + p.Apellido + " - " + p.IdPaciente
                                }).ToList();
            }
            llenarCitas();
            return View(listTratamiento);

        }
        public ActionResult verTratamientos()
        {
            string pacienteId = Session["IdPaciente"].ToString();

            List<mTratamiento> listTratamiento = null;

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {

                listTratamiento = (from t in db.Tratamiento
                                join c in db.Cita on t.IdCita equals c.IdCita
                                join m in db.Medico on c.IdMedico equals m.IdMedico
                                join tm in db.Tratamiento_Medicamento on t.IdTratamiento equals tm.IdTratamiento
                                join med in db.Medicamento on tm.IdMedicamento equals med.IdMedicamento
                                select new mTratamiento
                                {
                                    IdTratamiento = t.IdTratamiento,
                                    IdCita = c.IdCita,
                                    CostoTotal = t.CostoTotal.Value,
                                    Medico = m.Nombre + " - " + m.Especialidad,
                                    Medicamento = med.Nombre + " x" +tm.Cantidad
                                }).ToList();
            }
            llenarCitas();
            return View(listTratamiento);

        }
        



        [HttpGet]
        public ActionResult agregarTratamientos()
        {
            llenarCitas();
            return View();
        }

        [HttpPost]
        public ActionResult AgregarTratamientos(mTratamiento tratamiento)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    llenarCitas();
                    return View(tratamiento);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    Sistema_Hospitales.Models.Tratamiento t = new Sistema_Hospitales.Models.Tratamiento
                    {
                        IdTratamiento = tratamiento.IdTratamiento,
                        IdCita = tratamiento.IdCita,
                        CostoTotal = tratamiento.CostoTotal
                    };

                    db.Tratamiento.Add(t);

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Tratamiento agregado exitosamente!";
                }
                llenarCitas();
                return View(tratamiento);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Hubo un error al agregar el Tratamiento deseado " + ex;
                llenarCitas();
                return View(tratamiento);
            }

        }

        [HttpGet]
        public ActionResult actualizarTratamientos(int id)
        {
           

            mTratamiento tra = new mTratamiento();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var tratamiento = db.Tratamiento.Find(id);

                tra.IdTratamiento = tratamiento.IdTratamiento;
                tra.IdCita = tratamiento.IdCita.Value;
                tra.CostoTotal = tratamiento.CostoTotal.Value;
            }
            llenarCitas();
            return View(tra);
        }

        [HttpPost]
        public ActionResult actualizarTratamientos(mTratamiento tra)
        {
           

            try
            {
                if (!ModelState.IsValid)
                {
                    llenarCitas();
                    return View(tra);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    var tratamiento = db.Tratamiento.Find(tra.IdTratamiento);

                    tratamiento.IdCita = tra.IdCita;
                    tratamiento.CostoTotal = tra.CostoTotal;

                    db.Entry(tratamiento).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Tratamiento actualizado correctamente";
                }
                llenarCitas();
                return View(tra);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Falló al actualizar el Tratamiento " + ex;
                llenarCitas();
                return View(tra);
            }
        }

        [HttpGet]
        public ActionResult consultarTratamientos(int id)
        {
            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1()) { 
            var tratamiento = db.Tratamiento.Find(id);

            var cita = db.Cita.FirstOrDefault(c => c.IdCita == tratamiento.IdCita);
            var paciente = db.Paciente.FirstOrDefault(p => p.IdPaciente == cita.IdPaciente);

            ViewBag.NombrePaciente = paciente.Nombre + " " + paciente.Apellido;

            var modelo = new mTratamiento
            {
                IdTratamiento = tratamiento.IdTratamiento,
                IdCita = tratamiento.IdCita.Value,
                CostoTotal = tratamiento.CostoTotal.Value
            };
                llenarCitas();
                return View(modelo); 
            }

        }

        [HttpGet]
        public ActionResult eliminarTratamientos(int id)
        {
            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var tratamiento = db.Tratamiento.Find(id);

                db.Tratamiento.Remove(tratamiento);

                db.SaveChanges();
            }
            return RedirectToAction("mantTratamientos", "Tratamiento");
        }

        private void llenarCitas()
        {
            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var listaCitas = db.Cita
                    .Select(c => new
                    {
                        IdCita = c.IdCita,
                        Descripcion = c.IdCita +" "+ c.Fecha + " " + c.Hora + " - " + c.IdPaciente
                    }).ToList();

                ViewBag.Citas = new SelectList(listaCitas, "IdCita", "Descripcion");
            }
        }
    }
}