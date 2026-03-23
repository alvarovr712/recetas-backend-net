using System.Security.Cryptography.X509Certificates;
using RecetasAPINet.Data;
using RecetasAPINet.DTOs;
using RecetasAPINet.Repositories;

namespace RecetasAPINet.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly RecetasDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;

        private readonly ISessionRepository _sessionRepository;

        private readonly ILogRepository _logRepository;

        public DashboardService (RecetasDbContext context, IUserRepository userRepository,IRecipeRepository recipeRepository,
        ISessionRepository sessionRepository, ILogRepository logRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _sessionRepository = sessionRepository;
            _logRepository = logRepository;

        }

        public async Task<DashboardDTO> GetDashboardAsync(int? month = null, int? year = null)
        {
            var totalUsuarios = await _userRepository.CountAllAsync();
            var totalRecetas = await _recipeRepository.CountAllAsync();
            var totalSesionesActivas = await _sessionRepository.CountAllAsync();

            var now = DateTime.UtcNow;
            var targetMonth = month ?? now.Month;
            var targetYear = year ?? now.Year;

            var start = new DateTime(targetYear, targetMonth, 1);
            var end = start.AddMonths(1);
            var startPrev = start.AddMonths(-1);
            var endPrev = start;

            //Crecimiento de Usuarios
            //Usuarios creados este mes
            var usuariosMes = await _userRepository.CountCreatedBetweenAsync(start,end);

            //Usuarios creados el mes anterior
            var usuariosMesAnterior = await _userRepository.CountCreatedBetweenAsync(startPrev,endPrev);

            //Crecimiento Usuarios

            double usuariosCrecimiento = usuariosMesAnterior == 0
                ? 100
                : (double)usuariosMes/ usuariosMesAnterior * 100;

            //Crecimiento Recetas

            var recetasMes = await _recipeRepository.CountCreatedBetweenAsync(start,end);
            var recetasMesAnterior = await _recipeRepository.CountCreatedBetweenAsync(startPrev,endPrev);

            double recetasCrecimiento = recetasMesAnterior == 0
                ? 100
                : (double) recetasMes/recetasMesAnterior * 100;

            //Crecimiento Sesiones

            var sesionesMes = await _sessionRepository.CountCreatedBetweenAsync(start,end);
            var sesionesMesAnterior = await _sessionRepository.CountCreatedBetweenAsync(startPrev,endPrev);

            double sesionesCrecimiento = sesionesMesAnterior == 0
                ? 100
                : (double) sesionesMes/sesionesMesAnterior * 100;

            // Activity for the selected month
            var actividadData = await _logRepository.CountByDayAsync(start, end);
            var daysInMonth = DateTime.DaysInMonth(targetYear, targetMonth);

            var actividadDiaria = Enumerable.Range(0, daysInMonth)
                .Select(offset => start.AddDays(offset))
                .Select(date => new DailyActivityDTO
                {
                    Fecha = date,
                    Valor = actividadData.FirstOrDefault(a => a.Fecha.Date == date.Date)?.Valor ?? 0
                })
                .ToList();

            //Top usuarios por recetas creadas

            var topUsuarios = await _recipeRepository.GetTopUsersByRecipesAsync(3);
            
            return new DashboardDTO
            {
                TotalUsuarios = totalUsuarios,
                TotalRecetas = totalRecetas,
                TotalSesiones = totalSesionesActivas,
                UsuariosCrecimiento = usuariosCrecimiento,
                RecetasCrecimiento = recetasCrecimiento,
                SesionesCrecimiento = sesionesCrecimiento,
                ActividadDiaria = actividadDiaria,
                UsuariosMasActivos = topUsuarios
                
            };
        }
    }
}