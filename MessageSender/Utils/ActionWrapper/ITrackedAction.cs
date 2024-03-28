using System.Threading.Tasks;

namespace MessageSender.Utils.ActionWrapper
{
    public interface ITrackedAction
    {
        Task Run();
    }
}
