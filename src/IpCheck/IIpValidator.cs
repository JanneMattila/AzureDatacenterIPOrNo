namespace IpCheck
{
    public interface IIpValidator
    {
        bool TryParse(string ip, out IpValidationResult result);
    }
}