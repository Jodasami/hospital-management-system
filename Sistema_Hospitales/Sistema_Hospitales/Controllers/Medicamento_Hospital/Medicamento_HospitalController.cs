using Sistema_Hospitales.Models.viewModels;
using Sistema_Hospitales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema_Hospitales.Controllers.Medicamento_Hospital
{
    public class Medicamento_HospitalController : Controller
    {
        // GET: Medicamento_Hospital
        public ActionResult mantMedicamento_Hospitales()
        {
         

            List<mMedicamento_Hospital> listMedicamentos = null;

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                listMedicamentos = (from medicamento in db.Medicamento_Hospital
                                    join hosp in db.Hospital on medicamento.IdHospital equals hosp.IdHospital
                                    join med in db.Medicamento on medicamento.IdMedicamento equals med.IdMedicamento
                                    select new mMedicamento_Hospital
                                    {
                                        IdHospital = medicamento.IdHospital,
                                        IdMedicamento = medicamento.IdMedicamento,
                                        CantidadDisponible = medicamento.CantidadDisponible.Value,
                                        Hospital = hosp.Nombre,
                                        Medicamento = med.Nombre
                                    }).ToList();
            }
            LlenarViewBags();
            return View(listMedicamentos);

        }

        [HttpGet]
        public ActionResult agregarMedicamentos_Hospitales()
        {

            LlenarViewBags();
            return View();
        }

        [HttpPost]
        public ActionResult AgregarMedicamentos_Hospitales(mMedicamento_Hospital medicamento)
        {
            LlenarViewBags();

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(medicamento);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    Sistema_Hospitales.Models.Medicamento_Hospital m = new Sistema_Hospitales.Models.Medicamento_Hospital
                    {
                        IdHospital = medicamento.IdHospital,
                        IdMedicamento = medicamento.IdMedicamento,
                        CantidadDisponible = medicamento.CantidadDisponible
                    };

                    db.Medicamento_Hospital.Add(m);

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Medicamento agregado exitosamente!";
                }
                
                return View(medicamento);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Hubo un error al agregar el Medicamento deseado " + ex;
                LlenarViewBags();
                return View(medicamento);
            }

        }

        [HttpGet]
        public ActionResult actualizarMedicamentos_Hospitales(int id, int id2)
        {
           

            mMedicamento_Hospital med = new mMedicamento_Hospital();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var medicamento = db.Medicamento_Hospital.Find(id, id2);

                med.IdHospital = medicamento.IdHospital;
                med.IdMedicamento = medicamento.IdMedicamento;
                med.CantidadDisponible = medicamento.CantidadDisponible.Value;
            }
            LlenarViewBags();
            return View(med);
        }

        [HttpPost]
        public ActionResult actualizarMedicamentos_Hospitales(mMedicamento_Hospital med)
        {
            LlenarViewBags();

            try
            {
                if (!ModelState.IsValid)
                {
                    LlenarViewBags();
                    return View(med);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    var paciente = db.Medicamento_Hospital.Find(med.IdHospital, med.IdMedicamento);

                    paciente.IdHospital = med.IdHospital;
                    paciente.IdMedicamento = med.IdMedicamento;
                    paciente.CantidadDisponible = med.CantidadDisponible;

                    db.Entry(paciente).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Medicamento actualizado correctamente";
                }
                
                return View(med);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Falló al actualizar el medicamento " + ex;
                LlenarViewBags();
                return View(med);
            }
        }

        [HttpGet]
        public ActionResult consultarMedicamentos_Hospitales(int id, int id2)
        {
           

            mMedicamento_Hospital med = new mMedicamento_Hospital();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {

                var medicamento = db.Medicamento_Hospital.Find(id, id2);

                med.IdHospital = medicamento.IdHospital;
                med.IdMedicamento = medicamento.IdMedicamento;
                med.CantidadDisponible = medicamento.CantidadDisponible.Value;

            }
            return View(med);

        }

        [HttpGet]
        public ActionResult eliminarMedicamentos_Hospitales(int id, int id2)
        {
            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var medicamento = db.Medicamento_Hospital.Find(id, id2);

                db.Medicamento_Hospital.Remove(medicamento);

                db.SaveChanges();
            }
            return RedirectToAction("mantMedicamento_Hospitales", "Medicamento_Hospital");
        }

        private void LlenarViewBags()
        {
            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var hospitales = db.Hospital.Select(h => new
                {
                    Id = h.IdHospital,
                    Nombre = h.Nombre
                }).ToList();

                var medicamentos = db.Medicamento.Select(m => new
                {
                    Id = m.IdMedicamento,
                    Nombre = m.Nombre
                }).ToList();

                ViewBag.ListaHospitales = new SelectList(hospitales, "Id", "Nombre");
                ViewBag.ListaMedicamentos = new SelectList(medicamentos, "Id", "Nombre");
            }
        }

    }
}