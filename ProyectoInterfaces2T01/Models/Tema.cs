namespace ProyectoInterfaces2T01.Models
{
    public class Tema
	{
		public string IdTema { get; set; }
		public string Titulo { get; set; }
		public string Cuerpo { get; set; }
		public string Autor { get; set; }
		public List<Comentario> Comentarios { get; set; }
        public Tema()
        {
            // Inicializa la lista de comentarios en el constructor
            Comentarios = new List<Comentario>();
        }
    }
}
