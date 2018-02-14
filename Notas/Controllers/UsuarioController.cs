using Notas.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notas.Controllers
{
    public class UsuarioController : Controller
    {


        // GET: Usuario
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Usuario user)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            else
            {
                String sql = @"SELECT * FROM dbo.Usuario WHERE nombre_usuario = '" + user.nombre_usuario + "' AND contraseña = '" + user.contraseña + "'";

                using (var db = new ContextNota())
                {
                    using (SqlConnection cn = new System.Data.SqlClient.SqlConnection(db.Database.Connection.ConnectionString))
                    {
                        if (cn != null)
                        {
                            cn.Open();
                            SqlCommand cmd = new SqlCommand(sql, cn);
                            try
                            {
                                SqlDataReader dr = cmd.ExecuteReader();
                                if (dr.Read())
                                {
                                    Usuario _user = new Usuario((int)dr["id"], (string)dr["nombre"], (string)dr["apellido"], (Nullable<System.DateTime>)dr["fecha_nacimiento"], (string)dr["email"], (string)dr["nombre_usuario"], (string)dr["contraseña"]);
                                    Session["Usuario"] = _user;
                                    return RedirectToAction("../Nota/Index");
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Usuario y/o contraseña invalido.");
                                    return View();
                                }
                               
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("", "Usuario y/o contraseña invalido.");
                                return View();
                            }
                            finally
                            {
                                cn.Close();
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Problemas con la conexion.");
                            return View();
                        }
                    }
                }
            }       
        }  

        public ActionResult Registro()
        {
            return View();
        }

        public void sugerencia(Usuario _usuario)
        {
            List<string> list = new List<string>();
            list.Add(_usuario.apellido);
            list.Add(_usuario.nombre);
            list.Add(_usuario.fecha_nacimiento.Value.Month.ToString());
            list.Add(_usuario.fecha_nacimiento.Value.Day.ToString());
            list.Add(_usuario.fecha_nacimiento.Value.DayOfYear.ToString());
            list.Add(_usuario.fecha_nacimiento.Value.Year.ToString());

            Random random = new Random();

            
            using (var db = new ContextNota())
            {
                using (SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(db.Database.Connection.ConnectionString))
                {
                    sqlConnection.Open();
                    for (int i = 0; i < 5; i++)
                    {
                        string ne = "";
                        for (int j = 0; j < 3; j++)
                        {
                            int pos = random.Next(list.Count);
                            ne += list[pos];
                        }
                        string sqlExiste = @"SELECT COUNT(*) FROM dbo.Usuario WHERE nombre_usuario = '" + ne + "' OR email = '" + _usuario.email+ "'";
                             SqlCommand cmd = new SqlCommand(sqlExiste, sqlConnection);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 0)
                        {
                            ModelState.AddModelError("", ne);
                        }
                        

                    }
                    sqlConnection.Close();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            else
            {
                string sqlExiste = @"SELECT COUNT(*) FROM dbo.Usuario WHERE nombre_usuario = @nombre_usuario OR email = @email";

                string sqlCarga = @"INSERT INTO dbo.Usuario (nombre, apellido, fecha_nacimiento, email, nombre_usuario, contraseña) VALUES(@nombre, @apellido, @fecha_nacimiento, @email, @nombre_usuario, @contraseña)";

                using (var db = new ContextNota())
                {
                    using (SqlConnection sqlConnection1 = new System.Data.SqlClient.SqlConnection(db.Database.Connection.ConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand(sqlExiste, sqlConnection1);
                        cmd.Parameters.AddWithValue("nombre_usuario", usuario.nombre_usuario);
                        cmd.Parameters.AddWithValue("email", usuario.email);
                        sqlConnection1.Open();
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 0)
                        {
                            try
                            {
                                SqlCommand command = new SqlCommand(sqlCarga,sqlConnection1);
                                command.Parameters.Add("@nombre", SqlDbType.VarChar).Value = usuario.nombre;
                                command.Parameters.Add("@apellido", SqlDbType.VarChar).Value = usuario.apellido;
                                command.Parameters.Add("@fecha_nacimiento", SqlDbType.DateTime).Value = usuario.fecha_nacimiento;
                                command.Parameters.Add("@email", SqlDbType.VarChar).Value = usuario.email;
                                command.Parameters.Add("@nombre_usuario", SqlDbType.VarChar).Value = usuario.nombre_usuario;
                                command.Parameters.Add("@contraseña", SqlDbType.VarChar).Value = usuario.contraseña;
                                command.Connection = sqlConnection1;
                                command.ExecuteNonQuery();
                                sqlConnection1.Close();
                                return RedirectToAction("Login");
                            }
                            catch (Exception )
                            {
                                ModelState.AddModelError("", "No se pudo crear el usuario. Por favor intente mas tarde.");
                                return View();
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("","El usuario que esta intentando creas ya se encuentra registrado. Intente acceder o con otro usuario.");
                            sugerencia(usuario);
                            sqlConnection1.Close();
                            return View();
                        }

                    }
                }
            }
        }
    }
}