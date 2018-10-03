using System.Threading;
using System.Threading.Tasks;

namespace DemoClient.Services
{
    public interface IDemoService
    {
        Task<bool> DoWork(CancellationTokenSource cancellationTokenSource);
    }  
}
