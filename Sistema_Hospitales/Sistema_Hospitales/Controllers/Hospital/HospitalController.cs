
using Sistema_Hospitales.Models;
using Sistema_Hospitales.Models.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace Sistema_Hospitales.Controllers
{
    public class HospitalController : Controller
    {

        // GET: Hospital

        public ActionResult mantHospitales()
        {
            

            using (var db = new SistemaHospitalesEntities1())
            {
                var listaHospitales = db.Hospital.AsNoTracking().ToList();

                // Mapear a ViewModel
                var viewModel = listaHospitales.Select(h => new mHospital
                {
                    IdHospital = h.IdHospital,
                    Nombre = h.Nombre,
                    Direccion = h.Direccion,
                    Telefono = h.Telefono
                }).ToList();

                return View(viewModel);
            }
        }

        [HttpGet]
        public ActionResult agregarHospital()
        {
           

            return View();
        }

        [HttpPost]
        public ActionResult AgregarHospital(mHospital hospital)
        {
            

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(hospital);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    Hospital h = new Hospital();
                    //h.IdHospital = hospital.IdHospital;
                    h.Nombre = hospital.Nombre;
                    h.Direccion = hospital.Direccion;
                    h.Telefono = hospital.Telefono;

                    db.Hospital.Add(h);

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Hospital agregado exitosamente!";
                }

                return View(hospital);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Hubo un error al agregar el Hospital deseado " + ex;
                return View(hospital);
            }

        }

        [HttpGet]
        public ActionResult actualizarHospital(int id)
        {
           

            mHospital hos = new mHospital();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var hospital = db.Hospital.Find(id);

                hos.IdHospital = hospital.IdHospital;
                hos.Nombre = hospital.Nombre;
                hos.Direccion = hospital.Direccion;
                hos.Telefono = hospital.Telefono;
            }

            return View(hos);
        }

        [HttpPost]
        public ActionResult actualizarHospital(mHospital hos)
        {
           

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(hos);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    var hospital = db.Hospital.Find(hos.IdHospital);

                    hospital.Nombre = hos.Nombre;
                    hospital.Direccion = hos.Direccion;
                    hospital.Telefono = hos.Telefono;

                    db.Entry(hospital).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Hospital actualizado correctamente";
                }

                return View(hos);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Falló al actualizar el hospital " + ex;
                return View(hos);
            }
        }

        [HttpGet]
        public ActionResult consultarHospital(int id)
        {
            

            mHospital hos = new mHospital();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {

                var hospital = db.Hospital.Find(id);

                hos.IdHospital = hospital.IdHospital;
                hos.Nombre = hospital.Nombre;
                hos.Direccion = hospital.Direccion;
                hos.Telefono = hospital.Telefono;

            }

            return View(hos);

        }

        [HttpGet]
        public ActionResult eliminarHospital(int id)
        {

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var hospital = db.Hospital.Find(id);

                db.Hospital.Remove(hospital);

                db.SaveChanges();
            }
            return RedirectToAction("mantHospitales", "Hospital");
        }

        [HttpGet]
        public ActionResult InventarioYPrescripciones(int? idHospital)
        {
            using (var db = new SistemaHospitalesEntities1())
            {
                var hospitales = db.Hospital.ToList();
                ViewBag.Hospitales = new SelectList(hospitales, "IdHospital", "Nombre");

                if (idHospital == null)
                {
                    ViewBag.Error = "Debe seleccionar un hospital para consultar.";
                    return View(new List<mInventarioPrescripcion>());
                }

                try
                {
                    var resultado = db.Database.SqlQuery<mInventarioPrescripcion>(
                        "EXEC InventarioYPrescripciones @p0", idHospital).ToList();

                    ViewBag.IdHospital = idHospital;
                    return View(resultado);
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error al obtener el inventario: " + ex.Message;
                    return View(new List<mInventarioPrescripcion>());
                }
            }
        }

    }
}