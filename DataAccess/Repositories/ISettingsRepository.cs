using Models;

namespace DataAccess.Repositories
{
    public interface ISettingRepository : IRepositoryBase<Setting>
    {
        Setting Setting { get; }
    }
}