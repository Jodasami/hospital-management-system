using Sistema_Hospitales.Models.viewModels;
using Sistema_Hospitales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema_Hospitales.Controllers.Paciente
{
    public class PacienteController : Controller
    {
        // GET: Hospital

        public ActionResult mantPacientes()
        {
          

            List<mPaciente> listPaciente = null;

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    listPaciente = (from paciente in db.Paciente
                                    join hp in db.Hospital on paciente.IdHospital equals hp.IdHospital 
                               select new mPaciente
                               {
                                   IdPaciente = paciente.IdPaciente,
                                   Nombre = paciente.Nombre,
                                   Apellido = paciente.Apellido,
                                   FechaNacimiento = paciente.FechaNacimiento.Value,
                                   Genero = paciente.Genero,
                                   Direccion = paciente.Direccion,
                                   Telefono = paciente.Telefono,
                                   IdHospital = paciente.IdHospital.Value,
                                   NombreHospital = hp.Nombre
                               }).ToList();
                }
            CargarHospitales();
            return View(listPaciente);
            
        }

        [HttpGet]
        public ActionResult agregarPacientes()
        {
            CargarHospitales();

            return View();
        }

        [HttpPost]
        public ActionResult AgregarPacientes(mPaciente paciente)
        {

          

            try
            {
                if (!ModelState.IsValid)
                {
                    CargarHospitales();
                    return View(paciente);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    Sistema_Hospitales.Models.Paciente p = new Sistema_Hospitales.Models.Paciente
                    {
                        IdPaciente = paciente.IdPaciente,
                        Nombre = paciente.Nombre,
                        Apellido = paciente.Apellido,
                        FechaNacimiento = paciente.FechaNacimiento,
                        Genero = paciente.Genero,
                        Direccion = paciente.Direccion,
                        Telefono = paciente.Telefono,
                        IdHospital = paciente.IdHospital
                    };

                    db.Paciente.Add(p);

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Paciente agregado exitosamente!";
                }
                CargarHospitales();
                return View(paciente);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Hubo un error al agregar el Paciente deseado " + ex;
                CargarHospitales();
                return View(paciente);
            }

        }

        [HttpGet]
        public ActionResult actualizarPacientes(string id)
        {


            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("mantPacientes");
            }

            mPaciente pac = new mPaciente();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var paciente = db.Paciente.Find(id);

                pac.IdPaciente = paciente.IdPaciente;
                pac.Nombre = paciente.Nombre;
                pac.Apellido = paciente.Apellido;
                pac.FechaNacimiento = paciente.FechaNacimiento.Value;
                pac.Genero = paciente.Genero;
                pac.Direccion = paciente.Direccion;
                pac.Telefono = paciente.Telefono;
                pac.IdHospital = paciente.IdHospital.Value;
            }
            CargarHospitales();
            return View(pac);
        }

        [HttpPost]
        public ActionResult actualizarPacientes(mPaciente pac)
        {
            

            try
            {
                if (!ModelState.IsValid)
                {
                    CargarHospitales();
                    return View(pac);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    var paciente = db.Paciente.Find(pac.IdPaciente);

                    paciente.Nombre = pac.Nombre;
                    paciente.Apellido = pac.Apellido;
                    paciente.FechaNacimiento = pac.FechaNacimiento;
                    paciente.Genero = pac.Genero;
                    paciente.Direccion = pac.Direccion;
                    paciente.Telefono = pac.Telefono;
                    paciente.IdHospital = pac.IdHospital;

                    db.Entry(paciente).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Paciente actualizado correctamente";
                }
                CargarHospitales();
                return View(pac);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Falló al actualizar el Paciente " + ex;
                CargarHospitales();
                return View(pac);
            }
        }


        [HttpGet]
        public ActionResult actualizarDatosContacto()
        {
            string id = Session["IdPaciente"].ToString();


            mPaciente pac = new mPaciente();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var paciente = db.Paciente.Find(id);

                pac.IdPaciente = paciente.IdPaciente;
                pac.Nombre = paciente.Nombre;
                pac.Apellido = paciente.Apellido;
                pac.FechaNacimiento = paciente.FechaNacimiento.Value;
                pac.Genero = paciente.Genero;
                pac.Direccion = paciente.Direccion;
                pac.Telefono = paciente.Telefono;
                pac.IdHospital = paciente.IdHospital.Value;
            }
            CargarHospitales();
            return View(pac);
        }

        [HttpPost]
        public ActionResult actualizarDatosContacto(mPaciente pac)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    CargarHospitales();
                    return View(pac);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    var paciente = db.Paciente.Find(pac.IdPaciente);

                    paciente.Direccion = pac.Direccion;
                    paciente.Telefono = pac.Telefono;

                    db.Entry(paciente).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Datos actualizados correctamente";
                }
                CargarHospitales();
                return View(pac);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Falló al actualizar los datos" + ex;
                CargarHospitales();
                return View(pac);
            }
        }

        [HttpGet]
        public ActionResult consultarPacientes(string id)
        {

           

            mPaciente pac = new mPaciente();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {

                var paciente = db.Paciente.Find(id);

                pac.IdPaciente = paciente.IdPaciente;
                pac.Nombre = paciente.Nombre;
                pac.Apellido = paciente.Apellido;
                pac.FechaNacimiento = paciente.FechaNacimiento.Value;
                pac.Genero = paciente.Genero;
                pac.Direccion = paciente.Direccion;
                pac.Telefono = paciente.Telefono;
                pac.IdHospital = paciente.IdHospital.Value;

            }
            CargarHospitales();
            return View(pac);

        }

        [HttpGet]
        public ActionResult eliminarPacientes(string id)
        {
            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var paciente = db.Paciente.Find(id);

                db.Paciente.Remove(paciente);

                db.SaveChanges();
            }
            return RedirectToAction("mantPacientes", "Paciente");
        }

        private void CargarHospitales()
        {
            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                ViewBag.Hospitales = new SelectList(db.Hospital.ToList(), "IdHospital", "Nombre");
            }
        }

    }
}