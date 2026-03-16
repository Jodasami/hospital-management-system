using Sistema_Hospitales.Models.viewModels;
using Sistema_Hospitales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema_Hospitales.Controllers.Tratamiento_Medicamento
{
    public class Tratamiento_MedicamentoController : Controller
    {
        // GET: Tratamiento_Medicamento
        public ActionResult mantTratamiento_Medicamento()
        {


            List<mTratamiento_Medicamento> listMedicamentos = null;

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                listMedicamentos = (from tm in db.Tratamiento_Medicamento
                                    join m in db.Medicamento on tm.IdMedicamento equals m.IdMedicamento
                                    select new mTratamiento_Medicamento
                                    {
                                        IdTratamiento = tm.IdTratamiento,
                                        IdMedicamento = tm.IdMedicamento,
                                        Cantidad = tm.Cantidad.Value,
                                        NombreMedicamento = m.Nombre
                                    }).ToList();
            }
            llenarListas();
            return View(listMedicamentos);

        }

        [HttpGet]
        public ActionResult agregarTratamiento_Medicamento()
        {

            llenarListas();
            return View();
        }

        [HttpPost]
        public ActionResult agregarTratamiento_Medicamento(mTratamiento_Medicamento medicamento)
        {


            try
            {
                if (!ModelState.IsValid)
                {
                    llenarListas();
                    return View(medicamento);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    Sistema_Hospitales.Models.Tratamiento_Medicamento m = new Sistema_Hospitales.Models.Tratamiento_Medicamento
                    {
                        IdTratamiento = medicamento.IdTratamiento,
                        IdMedicamento = medicamento.IdMedicamento,
                        Cantidad = medicamento.Cantidad
                    };

                    db.Tratamiento_Medicamento.Add(m);

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Medicamento agregado exitosamente!";
                }
                llenarListas();
                return View(medicamento);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Hubo un error al agregar el Medicamento deseado " + ex;
                llenarListas();
                return View(medicamento);
            }

        }

        [HttpGet]
        public ActionResult actualizarTratamiento_Medicamento(int id, int id2)
        {


            mTratamiento_Medicamento med = new mTratamiento_Medicamento();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var medicamento = db.Tratamiento_Medicamento.Find(id, id2);

                med.IdTratamiento = medicamento.IdTratamiento;
                med.IdMedicamento = medicamento.IdMedicamento;
                med.Cantidad = medicamento.Cantidad.Value;
            }
            llenarListas();
            return View(med);
        }

        [HttpPost]
        public ActionResult actualizarTratamiento_Medicamento(mTratamiento_Medicamento med)
        {



            try
            {
                if (!ModelState.IsValid)
                {
                    llenarListas();
                    return View(med);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    var paciente = db.Tratamiento_Medicamento.Find(med.IdTratamiento, med.IdMedicamento);

                    paciente.IdTratamiento = med.IdTratamiento;
                    paciente.IdMedicamento = med.IdMedicamento;
                    paciente.Cantidad = med.Cantidad;

                    db.Entry(paciente).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Medicamento actualizado correctamente";
                }
                llenarListas();
                return View(med);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Falló al actualizar el medicamento " + ex;
                llenarListas();
                return View(med);
            }
        }

        [HttpGet]
        public ActionResult consultarTratamiento_Medicamento(int id, int id2)
        {


            mTratamiento_Medicamento med = new mTratamiento_Medicamento();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {

                var medicamento = db.Tratamiento_Medicamento.Find(id, id2);

                med.IdTratamiento = medicamento.IdTratamiento;
                med.IdMedicamento = medicamento.IdMedicamento;
                med.Cantidad = medicamento.Cantidad.Value;

            }
            llenarListas();
            return View(med);

        }

        [HttpGet]
        public ActionResult eliminarTratamiento_Medicamento(int id, int id2)
        {

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var medicamento = db.Tratamiento_Medicamento.Find(id, id2);

                db.Tratamiento_Medicamento.Remove(medicamento);

                db.SaveChanges();
            }
            return RedirectToAction("mantTratamiento_Medicamento", "Tratamiento_Medicamento");
        }

        private void llenarListas()
        {
            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                ViewBag.Tratamientos = new SelectList(
                    db.Tratamiento.Select(t => new
                    {
                        Id = t.IdTratamiento,
                        Descripcion = "Tratamiento #" + t.IdTratamiento
                    }).ToList(),
                    "Id", "Descripcion"
                );

                ViewBag.Medicamentos = new SelectList(
                    db.Medicamento.Select(m => new
                    {
                        Id = m.IdMedicamento,
                        Descripcion = m.Nombre
                    }).ToList(),
                    "Id", "Descripcion"
                );
            }
        }
    }
}
