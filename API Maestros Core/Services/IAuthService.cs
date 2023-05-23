namespace API_Maestros_Core.Services
{
    public interface IAuthService
    {
        public bool ValidateLogin(string username, string password);
        string GenerateToken(DateTime fechaActual, string username, TimeSpan tiempoValidez);
    }
}
