using Dapper;
using MSS.Platform.ProcessApp.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS.Platform.ProcessApp.Data
{

    public interface IConsulRepo<T> where T : BaseEntity
    {
        Task<ConsulServiceEntityView> GetPageList(ConsulServiceEntityParm param);
        Task<ConsulServiceEntity> GetById(int id);
        Task<bool> UpdById(ConsulServiceEntity obj);
    }

    public class ConsulRepo : BaseRepo, IConsulRepo<ConsulServiceEntity>
    {
        public ConsulRepo(DapperOptions options) : base(options) { }

        public async Task<ConsulServiceEntityView> GetPageList(ConsulServiceEntityParm param)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                StringBuilder sqlCount = new StringBuilder();

                sql.Append($@"SELECT a.* ");
                sqlCount.Append("SELECT COUNT(1) ");

                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" FROM consul_services a WHERE 1 = 1 ");



                if (!string.IsNullOrEmpty(param.ServiceName))
                {
                    whereSql.Append(" AND  a.service_name LIKE '%" + param.ServiceName + "%' ");
                }
                sql.Append(whereSql)
                   .Append(" order by a." + param.sort + " " + param.order)
                   .Append(" limit " + (param.page - 1) * param.rows + "," + param.rows);
                sqlCount.Append(whereSql);
                var data = await c.QueryAsync<ConsulServiceEntity>(sql.ToString());
                int total = await c.QueryFirstOrDefaultAsync<int>(sqlCount.ToString());

                ConsulServiceEntityView ret = new ConsulServiceEntityView();
                ret.rows = data.ToList();
                ret.total = total;

                return ret;
            });
        }

        public async Task<ConsulServiceEntity> GetById(int id)
        {
            return await WithConnection(async c =>
            {
                string sql = $@" SELECT * FROM consul_services WHERE id = '{id}' ";
                var data = await c.QueryFirstOrDefaultAsync<ConsulServiceEntity>(sql);
                return data;
            });
        }

        public async Task<bool> UpdById(ConsulServiceEntity obj)
        {
            return await WithConnection(async c =>
            {
                string sql = $@" UPDATE consul_services SET service_pid= '{obj.ServicePID}' WHERE id = '{obj.ID}' ";
                await c.ExecuteAsync(sql);
                return true;
            });
        }
    }

}
