namespace DataAccess.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly RepositoryContext _repoContext;
        private IPlaceRepository _place;
        private IQueueRepository _queue;
        private IProtocolRepository _protocol;
        private ISettingRepository _setting;
        private IOptionRepository _option;
        private IUserRepository _user;
        private IPlaceOptionRepository _placeOption;
        private IRoleRepository _role;
        private IOptionQueueRepository _optionQueue;

        public IPlaceRepository Place
        {
            get
            {
                return _place ?? new PlaceRepository(_repoContext);
            }
        }

        public ISettingRepository Setting
        {
            get
            {
                return _setting ?? new SettingRepository(_repoContext);
            }
        }

        public IOptionRepository Option
        {
            get
            {
                return _option ?? new OptionRepository(_repoContext);
            }
        }
        public IQueueRepository Queue
        {
            get
            {
                return _queue ?? new QueueRepository(_repoContext);
            }
        }
        public IProtocolRepository Protocol
        {
            get
            {
                return _protocol ?? new ProtocolRepository(_repoContext);
            }
        }
        public IUserRepository User
        {
            get
            {
                return _user ?? new UserRepository(_repoContext);
            }
        }
        public IPlaceOptionRepository PlaceOption
        {
            get
            {
                return _placeOption ?? new PlaceOptionRepository(_repoContext);
            }
        }
        public IRoleRepository Role
        {
            get
            {
                return _role ?? new RoleRepository(_repoContext);
            }
        }
        public IOptionQueueRepository OptionQueue
        {
            get
            {
                return _optionQueue ?? new OptionQueueRepository(_repoContext);
            }
        }
        public RepositoryWrapper(RepositoryContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
    }
}
