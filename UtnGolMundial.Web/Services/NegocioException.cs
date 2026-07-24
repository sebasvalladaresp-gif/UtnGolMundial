namespace UtnGolMundial.Web.Services;

public class NegocioException : Exception
{
    public NegocioException(string mensaje) : base(mensaje) { }
}
