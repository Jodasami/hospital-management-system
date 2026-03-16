using Sistema_Hospitales.Models.viewModels;
using Sistema_Hospitales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Sistema_Hospitales.Controllers.Usuario
{
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult mantUsuarios()
        {
           

            List<mUsuario> listPaciente = null;

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                listPaciente = (from usuario in db.Usuario
                                select new mUsuario
                                {
                                    IdUsuario = usuario.IdUsuario,
                                    UserName = usuario.Username,
                                    Contrasena = usuario.Contrasena,
                                    IdRol = usuario.IdRol,
                                    IdPaciente = usuario.IdPaciente,
                                    IdMedico = usuario.IdMedico
                                }).ToList();
            }
            llenarLista();
            return View(listPaciente);

        }

        [HttpGet]
        public ActionResult agregarUsuarios()
        {
            llenarLista();
            return View();
        }

        [HttpPost]
        public ActionResult AgregarUsuarios(mUsuario usuario)
        {
          

            try
            {
                if (!ModelState.IsValid)
                {
                    llenarLista();
                    return View(usuario);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    Sistema_Hospitales.Models.Usuario p = new Sistema_Hospitales.Models.Usuario
                    {
                        IdUsuario = usuario.IdUsuario,
                        Username = usuario.UserName,
                        Contrasena = usuario.Contrasena,
                        IdRol = usuario.IdRol,
                        IdPaciente = usuario.IdPaciente,
                        IdMedico = usuario.IdMedico
                    };

                    db.Usuario.Add(p);

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Usuario agregado exitosamente!";
                }
                llenarLista();
                return View(usuario);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Hubo un error al agregar el Usuario deseado " + ex;
                llenarLista();
                return View(usuario);
            }

        }

        [HttpGet]
        public ActionResult actualizarUsuarios(int id)
        {

            

            mUsuario usu = new mUsuario();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var usuario = db.Usuario.Find(id);

                usu.IdUsuario = usuario.IdUsuario;
                usu.UserName = usuario.Username;
                usu.Contrasena = usuario.Contrasena;
                usu.IdRol = usuario.IdRol;
                usu.IdPaciente = usuario.IdPaciente;
                usu.IdMedico = usuario.IdMedico;
            }
            llenarLista();
            return View(usu);
        }

        [HttpPost]
        public ActionResult actualizarUsuarios(mUsuario usu)
        {
            

            try
            {
                if (!ModelState.IsValid)
                {
                    llenarLista();
                    return View(usu);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    var usuario = db.Usuario.Find(usu.IdUsuario);

                    usuario.Username = usu.UserName;
                    usuario.Contrasena = usu.Contrasena;
                    usuario.IdPaciente = usu.IdPaciente;
                    usuario.IdMedico = usu.IdMedico;

                    db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Usuario actualizado correctamente";
                }
                llenarLista();
                return View(usu);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Falló al actualizar el Usuario " + ex;
                llenarLista();
                return View(usu);
            }
        }


        [HttpGet]
        public ActionResult cambiarContrasena()
        {
            

            int id = Convert.ToInt32(Session["UsuarioId"]);

            mUsuario usu = new mUsuario();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var usuario = db.Usuario.Find(id);

                usu.IdUsuario = usuario.IdUsuario;
                usu.UserName = usuario.Username;
                usu.Contrasena = usuario.Contrasena;
                usu.IdRol = usuario.IdRol;
                usu.IdPaciente = usuario.IdPaciente;
                usu.IdMedico = usuario.IdMedico;
            }
            llenarLista();
            return View(usu);
        }

        [HttpPost]
        public ActionResult cambiarContrasena(mUsuario usu)
        {


            try
            {
                if (!ModelState.IsValid)
                {
                    llenarLista();
                    return View(usu);
                }

                using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
                {
                    var usuario = db.Usuario.Find(usu.IdUsuario);

                    usuario.Contrasena = usu.Contrasena;

                    db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    ViewBag.ValorMensaje = 1;
                    ViewBag.MensajeProceso = "Contraseña actualizado correctamente";
                }
                llenarLista();
                return View(usu);

            }
            catch (Exception ex)
            {
                ViewBag.ValorMensaje = 0;
                ViewBag.MensajeProceso = "Falló al actualizar la contraseña " + ex;
                llenarLista();
                return View(usu);
            }
        }

        [HttpGet]
        public ActionResult consultarUsuarios(int id)
        {
            

            mUsuario usu = new mUsuario();

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {

                var usuario = db.Usuario.Find(id);

                usu.IdUsuario = usuario.IdUsuario;
                usu.UserName = usuario.Username;
                usu.Contrasena = usuario.Contrasena;
                usu.IdRol = usuario.IdRol;
                usu.IdPaciente = usuario.IdPaciente;
                usu.IdMedico = usuario.IdMedico;

            }
            llenarLista();
            return View(usu);

        }

        [HttpGet]
        public ActionResult eliminarUsuarios(int id)
        {

            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                var usuario = db.Usuario.Find(id);

                db.Usuario.Remove(usuario);

                db.SaveChanges();
            }
            return RedirectToAction("mantUsuarios", "Usuario");
        }

        [HttpPost]
        public ActionResult Login(mUsuario user)
        {
            using (var db = new SistemaHospitalesEntities1())
            {
                var usuario = db.Usuario.FirstOrDefault(u => u.Username == user.UserName && u.Contrasena == user.Contrasena);

                if (usuario != null)
                {
                    Session["Usuario"] = usuario.Username;
                    Session["UsuarioId"] = usuario.IdUsuario;
                    Session["Rol"] = usuario.IdRol;
                    Session["IdMedico"] = usuario.IdMedico;
                    Session["IdPaciente"] = usuario.IdPaciente;

                    return RedirectToAction("Dashboard", "Home");
                }

                TempData["LoginError"] = "Credenciales incorrectas.";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Logout()
        {
            Session.Clear(); // Elimina todos los datos de sesión
            return RedirectToAction("Index", "Home"); // Vuelve al inicio o login
        }

        private void llenarLista()
        {
            using (SistemaHospitalesEntities1 db = new SistemaHospitalesEntities1())
            {
                ViewBag.ListaPacientes = db.Paciente.Select(p => new SelectListItem
                {
                    Value = p.IdPaciente,
                    Text = p.IdPaciente + " - " + p.Nombre + " " + p.Apellido
                }).ToList();

                ViewBag.ListaMedicos = db.Medico.Select(m => new SelectListItem
                {
                    Value = m.IdMedico.ToString(),
                    Text = m.Nombre + " - " + m.Especialidad
                }).ToList();
            }
              
        }

    }
}