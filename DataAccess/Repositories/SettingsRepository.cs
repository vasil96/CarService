using System.Linq;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess.Repositories
{
    public class SettingRepository : RepositoryBase<Setting>, ISettingRepository
    {
        private Setting _setting;

        public Setting Setting
        {
            get
            {
                return _setting;
            }
        }
        public SettingRepository(RepositoryContext context) : base(context)
        {
            _setting = context.Setting.AsNoTracking().FirstOrDefault();
        }

    }
}
