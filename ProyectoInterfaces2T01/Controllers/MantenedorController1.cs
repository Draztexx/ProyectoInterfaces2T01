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

        public IActionResult Eliminar()
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



		[HttpPost]
        public IActionResult CrearTema(Tema tema) {

			string IdGenerado = Guid.NewGuid().ToString("N");
            
			Usuario usuario = JsonConvert.DeserializeObject<Usuario>(HttpContext.Request.Cookies["Usuario"]);
			string nombreUsuario = usuario?.Name;
            tema.Autor = nombreUsuario;
			tema.Comentarios = new List<Comentario>();
			SetResponse response = cliente.Set("Temas/" + IdGenerado, tema);

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{

				return RedirectToAction("register");
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

					return RedirectToAction("CrearTema");
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
            //using System.Text;
            //USAR LA REFERENCIA DE "System.Security.Cryptography"

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

		public IActionResult verificar()
		{
			if (HttpContext.Session.GetString("Nombre") != null && HttpContext.Session.GetString("Email") != null)
			{

				return View();
			}
			else
			{

				return RedirectToAction("Login");
			}
		}

		public IActionResult CerrarSesion()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login");
		}





	}
}
	
