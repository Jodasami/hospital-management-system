using Sistema_Hospitales.Models.viewModels;
using Sistema_Hospitales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema_Hospitales.Controllers.Medico
{
    public class MedicoController : Controller
    {
        // GET: Medico
        public ActionResult mantMedicos()
        {
          

            List<mMedico> listMedicos = null;

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                listMedicos = (from medico in db.Medico
                               join hosp in db.Hospital on medico.IdHospital equals hosp.IdHospital
                               select new mMedico
                                {
                                    IdMedico = medico.IdMedico,
                                    Nombre = medico.Nombre,
                                    Especialidad = medico.Especialidad,
                                    Telefono = medico.Telefono,
                                    Email = medico.Email,
                                    IdHospital = medico.IdHospital.Value,
                                    NombreHospital = hosp.Nombre
                                }).ToList();
            }
            llenarListaHospitales();
            return View(listMedicos);

        }


        public ActionResult verPacientes()
        {
            int idMedico = Convert.ToInt32(Session["IdMedico"]);

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var pacientes = (from cita in db.Cita
                                 where cita.IdMedico == idMedico
                                 join paciente in db.Paciente on cita.IdPaciente equals paciente.IdPaciente
                                 select new mPaciente
                                 {
                                     IdPaciente = paciente.IdPaciente,
                                     Nombre = paciente.Nombre,
                                     Apellido = paciente.Apellido,
                                     FechaNacimiento = paciente.FechaNacimiento.Value,
                                     Genero = paciente.Genero,
                                     Direccion = paciente.Direccion,
                                     Telefono = paciente.Telefono,
                                     IdHospital = paciente.IdHospital.Value
                                 })
                         .Distinct() //evita que un paciente aparezca más de una vez si tuvo varias citas con el médico.
                         .ToList();
                llenarListaHospitales();
                return View(pacientes);
            }

        }

        [HttpGet]
        public ActionResult agregarMedicos()
        {

            llenarListaHospitales();
            return View();
        }

        [HttpPost]
        public ActionResult AgregarMedicos(mMedico medico)
        {
          

            try
            {
                if (!ModelState.IsValid)
                {
                    llenarListaHospitales();
                    return View(medico);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    Sistema_Hospitales.Models.Medico m = new Sistema_Hospitales.Models.Medico
                    {
                        IdMedico = medico.IdMedico,
                        Nombre = medico.Nombre,
                        Especialidad = medico.Especialidad,
                        Telefono = medico.Telefono,
                        Email = medico.Email,
                        IdHospital = medico.IdHospital
                    };
                    

                    db.Medico.Add(m);

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Medico agregado exitosamente!";
                }
                llenarListaHospitales();
                return View(medico);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Hubo un error al agregar el Medico deseado " + ex;
                llenarListaHospitales();
                return View(medico);
            }

        }

        [HttpGet]
        public ActionResult actualizarMedicos(int id)
        {

         
            mMedico med = new mMedico();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var medico = db.Medico.Find(id);

                med.IdMedico = medico.IdMedico;
                med.Nombre = medico.Nombre;
                med.Especialidad = medico.Especialidad;
                med.Telefono = medico.Telefono;
                med.Email = medico.Email;
                med.IdHospital = medico.IdHospital.Value;
            }
            llenarListaHospitales();
            return View(med);
        }

        [HttpPost]
        public ActionResult actualizarMedicos(mMedico med)
        {

            
            try
            {
                if (!ModelState.IsValid)
                {
                    llenarListaHospitales();
                    return View(med);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    var medico = db.Medico.Find(med.IdMedico);

                    medico.Nombre = med.Nombre;
                    medico.Especialidad = med.Especialidad;
                    medico.Telefono = med.Telefono;
                    medico.Email = med.Email;
                    medico.IdHospital = med.IdHospital;

                    db.Entry(medico).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Medico actualizado correctamente";
                }
                llenarListaHospitales();
                return View(med);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Falló al actualizar el Medico " + ex;
                llenarListaHospitales();
                return View(med);
            }
        }

        [HttpGet]
        public ActionResult actualizarDatosContacto()
        {
          

            int id = Convert.ToInt32(Session["IdMedico"]);


            mMedico med = new mMedico();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var medico = db.Medico.Find(id);

                med.IdMedico = medico.IdMedico;
                med.Nombre = medico.Nombre;
                med.Especialidad = medico.Especialidad;
                med.Telefono = medico.Telefono;
                med.Email = medico.Email;
                med.IdHospital = medico.IdHospital.Value;
            }
            llenarListaHospitales();
            return View(med);
        }

        [HttpPost]
        public ActionResult actualizarDatosContacto(mMedico med)
        {
           

            try
            {
                if (!ModelState.IsValid)
                {
                    llenarListaHospitales();
                    return View(med);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    var medico = db.Medico.Find(med.IdMedico);

                    medico.Telefono = med.Telefono;
                    medico.Email = med.Email;

                    db.Entry(medico).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Datos actualizados correctamente";
                }
                llenarListaHospitales();
                return View(med);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Falló al actualizar los datos" + ex;
                llenarListaHospitales();
                return View(med);
            }
        }

        [HttpGet]
        public ActionResult consultarMedicos(int id)
        {
            mMedico medicoVM = new mMedico();

            using (var db = new SistemaHospitalesEntities1())
            {
                var medico = db.Medico.Find(id);
                if (medico != null)
                {
                    medicoVM.IdMedico = medico.IdMedico;
                    medicoVM.Nombre = medico.Nombre;
                    medicoVM.Especialidad = medico.Especialidad;
                    medicoVM.Telefono = medico.Telefono;
                    medicoVM.Email = medico.Email;
                    medicoVM.IdHospital = medico.IdHospital.Value;

                    // Obtener nombre del hospital y enviarlo por ViewBag
                    var hospital = db.Hospital.Find(medico.IdHospital);
                    if (hospital != null)
                    {
                        ViewBag.NombreHospital = hospital.Nombre;
                    }
                }
            }
            llenarListaHospitales();
            return View(medicoVM);
        }

        [HttpGet]
        public ActionResult eliminarMedicos(int id)
        {
            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var medico = db.Medico.Find(id);


                bool tieneUsuarios = db.Usuario.Any(u => u.IdMedico == id);

                if (tieneUsuarios)
                {
                    TempData["MensajeError"] = "No se puede eliminar el médico porque está asignado a un usuario, si desea eliminarlo, elimine primero al usuario";
                    return RedirectToAction("mantMedicos", "Medico");
                }


                db.Medico.Remove(medico);

                db.SaveChanges();
            }
            return RedirectToAction("mantMedicos", "Medico");
        }


        private void llenarListaHospitales()
        {
            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                
                ViewBag.Hospitales = db.Hospital
                    .Select(h => new SelectListItem
                    {
                        Value = h.IdHospital.ToString(),
                        Text = h.Nombre + " - " + h.Direccion
                    }).ToList();

            }
        }

    }
}