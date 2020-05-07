using System.Threading.Tasks;

namespace CoAPnet.Client
{
    public interface ICoapResponseHandler
    {
        Task HandleResponseAsync(HandleResponseContext context);
    }
}
