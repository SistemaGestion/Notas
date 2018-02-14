using Notas.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Notas.Controllers
{
    public class NotaController : Controller
    {
               
        // GET: Nota
        public ActionResult Index()
        {
            using (var db = new ContextNota())
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                List<Nota> notas = db.Nota.Where(a => a.id_usuario == usuario.id).ToList();
                return View(notas);
            }
        }


        public ActionResult AgregarNota()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AgregarNota(Nota nota)
        {
            if (Session["Usuario"] != null)
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                if (!ModelState.IsValid)
                {
                    return View();
                }
                else
                {
                    string sqlCarga = @"INSERT INTO dbo.Nota (titulo, descripcion, id_usuario) VALUES(@titulo, @descripcion, @id_usuario)";

                    using (var db = new ContextNota())
                    {
                        using (SqlConnection sqlConnection1 = new System.Data.SqlClient.SqlConnection(db.Database.Connection.ConnectionString))
                        {
                            try
                            {
                                SqlCommand cmd = new SqlCommand(sqlCarga, sqlConnection1);
                                cmd.Parameters.AddWithValue("titulo", nota.titulo);
                                cmd.Parameters.AddWithValue("descripcion", nota.descripcion);
                                cmd.Parameters.AddWithValue("id_usuario", usuario.id);
                                sqlConnection1.Open();
                                cmd.Connection = sqlConnection1;
                                cmd.ExecuteNonQuery();
                                sqlConnection1.Close();
                                return RedirectToAction("Index");
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("", "No se pudo crear la nota.");
                                return View();
                            }
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Ocurrio un error. Intente nuevamente mas tarde.");
                return View();
            }
            
        }

        public Nota Buscar(int _id)
        {
            using (var db = new ContextNota())
            {
                using (SqlConnection sqlConnection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    String sql = "SELECT * FROM dbo.Nota WHERE id = " + _id;
                    if (sqlConnection != null)
                    {
                        SqlCommand cmd = new SqlCommand(sql, sqlConnection);
                        cmd.Parameters.AddWithValue("id", _id);
                        try
                        {
                            sqlConnection.Open();
                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.Read())
                            {
                                Nota nota = new Nota((int)dr["id"], (string)dr["titulo"], (string)dr["descripcion"], (int)dr["id_usuario"]);
                                return nota;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        catch (SqlException)
                        {
                            return null;
                        }
                        finally
                        {
                            sqlConnection.Close();
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public ActionResult Editar(int id)
        {
            Nota nota = Buscar(id);
            if (nota !=null)
            {
                return View(nota);
            }
            else
            {
                ModelState.AddModelError("", "No se encontro el usuario correspondiente.");
                return View();
            }
        }

        [HttpPost]
        public ActionResult Editar(Nota _nota)
        {

            using (var db = new ContextNota())
            {
                using (SqlConnection sqlConnection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    String sqlActualizar = "UPDATE dbo.Nota SET titulo = '" + _nota.titulo + "', descripcion = '" + _nota.descripcion + "' WHERE id = " + _nota.id;
                    if (sqlConnection != null)
                    {
                        SqlCommand cmd = new SqlCommand(sqlActualizar, sqlConnection);
                        try
                        {
                            sqlConnection.Open();
                            cmd.ExecuteNonQuery();
                            return RedirectToAction("Index");
                            
                        }
                        catch (SqlException)
                        {
                            ModelState.AddModelError("", "No se pudo actualizar.");
                            return View();
                        }
                        finally
                        {
                            sqlConnection.Close();
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public ActionResult Ver(int id)
        {
            Nota nota = Buscar(id);
            if (nota != null)
            {
                return View(nota);
            }
            else
            {
                ModelState.AddModelError("", "No se encuentra la nota que esta intendo ver.");
                return View();
            }
        }

        public ActionResult Borrar(int id)
        {
            using (var db = new ContextNota())
            {
                using (SqlConnection sqlConnection = new SqlConnection(db.Database.Connection.ConnectionString))
                    {
                    try
                    {
                        Nota nota = Buscar(id);
                        if (nota != null)
                        {
                            String sqlBorrar = "DELETE FROM dbo.Nota WHERE id = " + nota.id;
                            sqlConnection.Open();
                            SqlCommand cmd = new SqlCommand(sqlBorrar, sqlConnection);
                            cmd.ExecuteNonQuery();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    catch (Exception )
                    {
                        ModelState.AddModelError("", "No se pudo borrar.");
                        return RedirectToAction("Index");
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }
    }    
}
