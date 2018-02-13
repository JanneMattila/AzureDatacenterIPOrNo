using System.Threading.Tasks;

namespace IpCheck
{
    public interface IIpValidator
    {
        void Initialize();
        bool TryParse(string ip, out IpValidationResult result);
    }
}