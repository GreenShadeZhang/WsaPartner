using System.Threading.Tasks;

namespace WsaPartner.Contracts.Services
{
    public interface IActivationService
    {
        Task ActivateAsync(object activationArgs);
    }
}
