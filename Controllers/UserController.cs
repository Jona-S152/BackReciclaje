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
            if (userLog == null) return StatusCode(StatusCodes.Status400BadRequest, new { message = $"Credenciales incorrectas, intentalo de nuevo más tarde." });
            else return StatusCode(StatusCodes.Status200OK, new { message = "Usuario logueado.", result = userLog });
        }
    }
}
