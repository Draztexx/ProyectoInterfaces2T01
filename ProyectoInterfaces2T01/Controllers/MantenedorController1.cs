using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;



using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

using Newtonsoft.Json;

using ProyectoInterfaces2T01.Models;
using System.Text;
using System.Net;
using System.Diagnostics.Contracts;

namespace ProyectoInterfaces2T01.Controllers
{
    public class MantenedorController1 : Controller
    {
        IFirebaseClient cliente;

        public MantenedorController1()
        {

            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "tjoUKzI1g0rgopMWtq1seZIgIowory23wG70DJ3U",
                BasePath = "https://proyectointerfaces2t01-default-rtdb.firebaseio.com/"

            };

            cliente = new FirebaseClient(config);

        }

        //GET: Mantenedor
        public IActionResult Inicio()
        {
            return View();
        }

        public IActionResult Registrar()
        {
            return View();
        }

        public IActionResult Editar()
        {
            return View();
        }

 

        public IActionResult Login()
        {
            return View();
        }
		public IActionResult CrearTema()
        {
			return View();
		}

		public IActionResult Eliminar(String Idtema)
		{
            FirebaseResponse response = cliente.Delete("Temas/" + Idtema);

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
				return RedirectToAction("Temas", "MantenedorController1");
			}else
			{
				ViewBag.ErrorMessage = "No se pudo eliminar el contacto. Inténtalo nuevamente.";
				return RedirectToAction("Temas", "MantenedorController1");
            }
		}


		public IActionResult Comentar(String Idtema)
        {
            FirebaseResponse response = cliente.Get("Temas/"+Idtema);
            Tema tema = response.ResultAs<Tema>();
            tema.IdTema = Idtema;

            return View(tema);
        }

        [HttpPost]
        public IActionResult Comentar(String Idtema,Comentario comentario) {
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(HttpContext.Request.Cookies["Usuario"]);
            string nombreUsuario = usuario?.Name;
            string IdGenerado = Guid.NewGuid().ToString("N");
            comentario.IdComentario = IdGenerado;
            comentario.NombreUsu = nombreUsuario;

            FirebaseResponse Response = cliente.Get("Temas/" + Idtema);
            if (Response.StatusCode != System.Net.HttpStatusCode.OK)
            {

                return View();
            }


            Tema tema = Response.ResultAs<Tema>();
            
            if (tema.Comentarios == null)
            {
                tema.Comentarios = new List<Comentario>();
            }
            tema.Comentarios.Add(comentario);

            FirebaseResponse res = cliente.Update("Temas/" + Idtema, tema);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Temas", "MantenedorController1");
            }
            else
            {
                return View();
            }
        }



		[HttpPost]
        public IActionResult CrearTema(Tema tema) {

			string IdGenerado = Guid.NewGuid().ToString("N");
            
			Usuario usuario = JsonConvert.DeserializeObject<Usuario>(HttpContext.Request.Cookies["Usuario"]);
			string nombreUsuario = usuario?.Name;
            tema.Autor = nombreUsuario;
			tema.Comentarios = new List<Comentario>();
            FirebaseResponse response = cliente.Update("Temas/" + IdGenerado, tema);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{

				return RedirectToAction("Temas", "MantenedorController1");
			}
			else
			{
				return View();
			}

			return View();
        }

        
        public IActionResult Temas() 
        {
			
			Dictionary<string, Tema> lista = new Dictionary<string, Tema>();
			FirebaseResponse response = cliente.Get("Temas");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                lista = JsonConvert.DeserializeObject<Dictionary<string, Tema>>(response.Body);


				List<Tema> listaContacto = new List<Tema>();
                
				foreach (KeyValuePair<string, Tema> elemento in lista)
				{
					listaContacto.Add(new Tema
					{
						IdTema = elemento.Key,
						Titulo = elemento.Value.Titulo,
						Cuerpo = elemento.Value.Cuerpo,
                        Autor = elemento.Value.Autor,
						Comentarios = elemento.Value.Comentarios
					});
					
				}
				Usuario usuario = JsonConvert.DeserializeObject<Usuario>(HttpContext.Request.Cookies["Usuario"]);
				string nombreUsuario = usuario?.Name;
				ViewBag.NombreUsuario = nombreUsuario;
				return View(listaContacto);

			}

			return View();

		}



	
        

        [HttpPost]
        public IActionResult Registrar(Usuario usuario)
        {
            string IdGenerado = Guid.NewGuid().ToString("N");
            usuario.Password = ConvertirSha256(usuario.Password);


            SetResponse response = cliente.Set("Usuarios/" + IdGenerado, usuario);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {

                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }


        }

        [HttpPost]
        public IActionResult Login(Usuario usuario)
        {
            usuario.Password = ConvertirSha256(usuario.Password);

            FirebaseResponse response = cliente.Get("Usuarios");

            if (response.Body != "null")
            {
                Dictionary<string, Usuario> usuarios = response.ResultAs<Dictionary<string, Usuario>>();
                var usuarioExistente = usuarios.Values.FirstOrDefault(u => u.Name == usuario.Name && u.Password == usuario.Password);

                if (usuarioExistente != null)
                {
					string usuarioJson = JsonConvert.SerializeObject(usuarioExistente); // Usuario usuario = JsonConvert.DeserializeObject<Usuario>(json); siendo json el string que obtines del cookie
					Response.Cookies.Append("Usuario", usuarioJson, new CookieOptions
					{
						Expires = DateTime.Now.AddMinutes(30) // Establece el tiempo de expiración
					});

					//string usuarioName = Request.Cookies["Usuario"];

					return RedirectToAction("Temas", "MantenedorController1");
                }
                else
                {
                    Console.WriteLine("1");
                    ModelState.AddModelError("Password", "Contraseña incorrecta");
                    return View();
                }
            }
            else
            {
                Console.WriteLine("2");
                ModelState.AddModelError("Email", "Usuario no encontrado");
                return View();
            }


        }

        public IActionResult Index()
        {
            return View();
        }

        public static string ConvertirSha256(string texto)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

		

		public IActionResult CerrarSesion()
		{
            Response.Cookies.Delete("Usuario");

            return RedirectToAction("Login","MantenedorController1");
		}





	}
}
	
