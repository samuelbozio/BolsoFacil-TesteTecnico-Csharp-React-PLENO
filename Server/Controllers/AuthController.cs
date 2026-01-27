using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Services;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthController> _logger;

        // Demo users (em produção, usar banco de dados com senhas criptografadas)
        private static readonly Dictionary<string, string> DemoUsers = new()
        {
            { "usuario1", "senha123" },
            { "usuario2", "senha456" },
            { "admin", "admin123" }
        };

        public AuthController(IJwtTokenService jwtTokenService, ILogger<AuthController> logger)
        {
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        /// <summary>
        /// Autentica usuário e retorna JWT token
        /// </summary>
        [HttpPost("login")]
        public ActionResult<LoginResponseDTO> Login([FromBody] LoginDTO login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
            {
                return BadRequest(new LoginResponseDTO
                {
                    Success = false,
                    Message = "Username e password são obrigatórios"
                });
            }

            // Validar credenciais (demo)
            if (!DemoUsers.TryGetValue(login.Username, out var password) || password != login.Password)
            {
                _logger.LogWarning($"Falha de login para usuário: {login.Username}");
                return Unauthorized(new LoginResponseDTO
                {
                    Success = false,
                    Message = "Credenciais inválidas"
                });
            }

            try
            {
                // Gerar token
                var userId = login.Username.GetHashCode(); // Demo: usar hash como ID
                var token = _jwtTokenService.GenerateToken(userId, login.Username, $"{login.Username}@example.com");

                _logger.LogInformation($"Login bem-sucedido para usuário: {login.Username}");

                return Ok(new LoginResponseDTO
                {
                    Success = true,
                    Token = token,
                    User = new UserDTO
                    {
                        Id = userId,
                        Username = login.Username,
                        Email = $"{login.Username}@example.com"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao gerar token: {ex.Message}");
                return StatusCode(500, new LoginResponseDTO
                {
                    Success = false,
                    Message = "Erro ao processar login"
                });
            }
        }

        /// <summary>
        /// Valida um token JWT
        /// </summary>
        [HttpPost("validate")]
        public ActionResult<object> ValidateToken([FromBody] TokenValidationDTO request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Token))
                {
                    return BadRequest(new { valid = false, message = "Token é obrigatório" });
                }

                var isValid = _jwtTokenService.ValidateToken(request.Token);
                return Ok(new { valid = isValid, message = isValid ? "Token válido" : "Token inválido" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao validar token: {ex.Message}");
                return StatusCode(500, new { valid = false, message = "Erro ao validar token" });
            }
        }
    }
}
