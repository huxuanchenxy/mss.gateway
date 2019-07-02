using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using Dapper;
using MSS.API.Model.DTO;
using System.Linq;

namespace MSS.API.Dao.Implement
{
    public class UserRepo : BaseRepo, IUserRepo<User>
    {
        public UserRepo(DapperOptions options) : base(options) { }

       

        public async Task<User> IsValid(User user)
        {
            return await WithConnection(async c =>
            {
                try
                {
                    var ret = await c.QueryFirstOrDefaultAsync<User>(
                      "select * from user where is_del=0 and acc_name=@acc_name ;", user);
                    return ret;
                }
                catch (Exception ex)
                {
                    return null;
                }

                
            });
        }
    }
}
