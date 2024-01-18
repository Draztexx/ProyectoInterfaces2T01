using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

using Newtonsoft.Json;

using ProyectoInterfaces2T01.Models;

namespace ProyectoInterfaces2T01.Controllers
{
    public class MantenedorController1 : Controller
    {
        IFirebaseClient cliente;

        public  MantenedorController1() { 
        
            IFirebaseConfig config = new FirebaseConfig{
                AuthSecret= "tjoUKzI1g0rgopMWtq1seZIgIowory23wG70DJ3U",
                BasePath= "https://proyectointerfaces2t01-default-rtdb.firebaseio.com/"

            };

            cliente = new FirebaseClient(config);

        }

        //GET: Mantenedor
        public IActionResult Inicio()
        {
            return View();
        }

        public IActionResult Crear()
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

        [HttpPost]
        public IActionResult Crear(Usuario usuario)
        {
            string IdGenerado=Guid.NewGuid().ToString("N");

            SetResponse response = cliente.Set("Usuarios/" + IdGenerado, usuario);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return View();
            }
            else
            {
                return View();
            }

            
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
