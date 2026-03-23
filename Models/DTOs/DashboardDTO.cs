namespace RecetasAPINet.DTOs
{
    public class DashboardDTO
{
    public int TotalUsuarios { get; set; }
    public double UsuariosCrecimiento { get; set; }

    public int TotalRecetas { get; set; }
    public double RecetasCrecimiento { get; set; }

    public int TotalSesiones { get; set; }
    public double SesionesCrecimiento { get; set; }

    public List<DailyActivityDTO> ActividadDiaria {get; set;} = new();

    public List<ActiveUserDTO> UsuariosMasActivos {get; set;} = new();
}

public class DailyActivityDTO
    {
        public DateTime Fecha {get; set;}
        public int Valor{get; set;}
    }
public class ActiveUserDTO
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public int RecetasCreadas { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}


}