using BackReciclaje.Model;
using BackReciclaje.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackReciclaje.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository repo)
        {
            _userRepository = repo;
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser(User user)
        {
            bool userExist = _userRepository.UserExist(user.Cedula);
            if (userExist) return StatusCode(StatusCodes.Status400BadRequest, new { message = $"El usuario {user.NombreUsuario} ya existe."});
            bool resp = _userRepository.CreateUser(user);
            if (resp) return StatusCode(StatusCodes.Status200OK, new { message = "Usuario creado con éxito." });
            else return StatusCode(StatusCodes.Status400BadRequest, new { message = "Ocurrió un error desconocido, intentalo de nuevo más tarde." });
        }

        [HttpPost("Login")]
        public IActionResult Login(UserLogin user)
        {
            UserPuntos userLog = _userRepository.Login(user);
            if (userLog == null) return StatusCode(StatusCodes.Status400BadRequest, new { result = $"Credenciales incorrectas, intentalo de nuevo más tarde." });
            else return StatusCode(StatusCodes.Status200OK, new { result = userLog });
        }

        [HttpGet("GetRanking")]
        public IActionResult GetRanking()
        {
            List<Ranking> rankings = _userRepository.GetTopToRanking();
            if (rankings.Count < 3) return StatusCode(StatusCodes.Status400BadRequest, new { result = $"No existen suficientes usuarios para mostrar el top 3 con más puntos." });
            else return StatusCode(StatusCodes.Status200OK, new { result = rankings });
        }

        [HttpPost("SavePoints")]
        public IActionResult SavePoints(Puntos points)
        {
            bool resp = _userRepository.SavePoints(points);
            if (resp) return StatusCode(StatusCodes.Status200OK, new { message = "Puntos registrados con éxito." });
            else return StatusCode(StatusCodes.Status400BadRequest, new { message = "Error intentalo de nuevo después de 1 minuto." });
        }
    }
}
