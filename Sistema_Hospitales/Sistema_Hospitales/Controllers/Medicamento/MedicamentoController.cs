using Sistema_Hospitales.Models.viewModels;
using Sistema_Hospitales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema_Hospitales.Controllers.Medicamento
{
    public class MedicamentoController : Controller
    {
        // GET: Medicamento
        public ActionResult mantMedicamentos()
        {

           

            List<mMedicamento> listMedicamentos = null;

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                listMedicamentos = (from medicamento in db.Medicamento
                                select new mMedicamento
                                {
                                    IdMedicamento = medicamento.IdMedicamento,
                                    Nombre = medicamento.Nombre,
                                    Descripcion = medicamento.Descripcion,
                                    CostoUnidad = medicamento.CostoUnidad.Value
                                }).ToList();
            }

            return View(listMedicamentos);

        }

        [HttpGet]
        public ActionResult agregarMedicamentos()
        {
           

            return View();
        }

        [HttpPost]
        public ActionResult AgregarMedicamentos(mMedicamento medicamento)
        {
          

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(medicamento);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    Sistema_Hospitales.Models.Medicamento m = new Sistema_Hospitales.Models.Medicamento
                    {
                        IdMedicamento = medicamento.IdMedicamento,
                        Nombre = medicamento.Nombre,
                        Descripcion = medicamento.Descripcion,
                        CostoUnidad = medicamento.CostoUnidad
                    };

                    db.Medicamento.Add(m);

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
                return View(medicamento);
            }

        }

        [HttpGet]
        public ActionResult actualizarMedicamentos(int id)
        {
          

            mMedicamento med = new mMedicamento();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var medicamento = db.Medicamento.Find(id);

                med.IdMedicamento = medicamento.IdMedicamento;
                med.Nombre = medicamento.Nombre;
                med.Descripcion = medicamento.Descripcion;
                med.CostoUnidad = medicamento.CostoUnidad.Value;
            }

            return View(med);
        }

        [HttpPost]
        public ActionResult actualizarMedicamentos(mMedicamento med)
        {
          

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(med);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    var paciente = db.Medicamento.Find(med.IdMedicamento);

                    paciente.Nombre = med.Nombre;
                    paciente.Descripcion = med.Descripcion;
                    paciente.CostoUnidad = med.CostoUnidad;

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
                return View(med);
            }
        }

        [HttpGet]
        public ActionResult consultarMedicamentos(int id)
        {


            mMedicamento med = new mMedicamento();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {

                var medicamento = db.Medicamento.Find(id);

                med.IdMedicamento = medicamento.IdMedicamento;
                med.Nombre = medicamento.Nombre;
                med.Descripcion = medicamento.Descripcion;
                med.CostoUnidad = medicamento.CostoUnidad.Value;

            }

            return View(med);

        }

        [HttpGet]
        public ActionResult eliminarMedicamentos(int id)
        {

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var medicamento = db.Medicamento.Find(id);

                db.Medicamento.Remove(medicamento);

                db.SaveChanges();
            }
            return RedirectToAction("mantMedicamentos", "Medicamento");
        }
    }
}