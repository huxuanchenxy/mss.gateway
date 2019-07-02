using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IUserRepo<T> where T : BaseEntity
    {
        Task<User> IsValid(User user);
    }
}
