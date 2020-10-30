
namespace DataAccess.Repositories
{
    public interface IRepositoryWrapper
    {
        ISettingRepository Setting { get; }
        IPlaceRepository Place { get; }
        IQueueRepository Queue { get; }
        IOptionRepository Option { get; }
        IProtocolRepository Protocol { get; }
        IUserRepository User { get; }
        IPlaceOptionRepository PlaceOption { get; }
        IRoleRepository Role { get; }
        IOptionQueueRepository OptionQueue { get; }
    }
}

