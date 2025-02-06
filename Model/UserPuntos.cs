namespace BackReciclaje.Model
{
    public class UserPuntos : User
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public int? CantidadBasura { get; set; }
        public int? PuntosObtenidos { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}
