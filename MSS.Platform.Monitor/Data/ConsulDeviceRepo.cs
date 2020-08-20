using Dapper;
using MSS.Platform.Monitor.Model;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Coded By admin 2020/8/20 10:41:08
namespace MSS.Platform.Monitor.Data
{
    public interface IConsulDeviceRepo<T> where T : BaseEntity
    {
        Task<ConsulDevicePageView> GetPageList(ConsulDeviceParm param);
        Task<ConsulDevice> Save(ConsulDevice obj);
        Task<ConsulDevice> GetByID(long id);
        Task<int> Update(ConsulDevice obj);
        Task<int> Delete(string[] ids, int userID);
    }

    public class ConsulDeviceRepo : BaseRepo, IConsulDeviceRepo<ConsulDevice>
    {
        public ConsulDeviceRepo(DapperOptions options) : base(options) { }

        public async Task<ConsulDevicePageView> GetPageList(ConsulDeviceParm parm)
        {
            return await WithConnection(async c =>
            {

                StringBuilder sql = new StringBuilder();
                sql.Append($@"  SELECT 
                id,
                ip,
                pretty_name,
                cpu_load,
                cpu_up,
                pretty_memory_used,
                pretty_total_memory,
                percent_memory_used,
                memory_up,
                pretty_total_network,
                device_type_id,
                temperature,
                temperature_up,
                disk_used,
                disk_up,
                created_time,
                created_by,
                updated_time,updated_by FROM consul_device
                 ");
                StringBuilder whereSql = new StringBuilder();
                //whereSql.Append(" WHERE ai.ProcessInstanceID = '" + parm.ProcessInstanceID + "'");

                //if (parm.AppName != null)
                //{
                //    whereSql.Append(" and ai.AppName like '%" + parm.AppName.Trim() + "%'");
                //}

                sql.Append(whereSql);
                //验证是否有参与到流程中
                //string sqlcheck = sql.ToString();
                //sqlcheck += ("AND ai.CreatedByUserID = '" + parm.UserID + "'");
                //var checkdata = await c.QueryFirstOrDefaultAsync<TaskViewModel>(sqlcheck);
                //if (checkdata == null)
                //{
                //    return null;
                //}

                var data = await c.QueryAsync<ConsulDevice>(sql.ToString());
                var total = data.ToList().Count;
                sql.Append(" order by " + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                var ets = await c.QueryAsync<ConsulDevice>(sql.ToString());

                ConsulDevicePageView ret = new ConsulDevicePageView();
                ret.rows = ets.ToList();
                ret.total = total;
                return ret;
            });
        }

        public async Task<ConsulDevice> Save(ConsulDevice obj)
        {
            return await WithConnection(async c =>
            {
                string sql = $@" INSERT INTO `consul_device`(
                    
                    ip,
                    pretty_name,
                    cpu_load,
                    cpu_up,
                    pretty_memory_used,
                    pretty_total_memory,
                    percent_memory_used,
                    memory_up,
                    pretty_total_network,
                    device_type_id,
                    temperature,
                    temperature_up,
                    disk_used,
                    disk_up,
                    created_time,
                    created_by,
                    updated_time,
                    updated_by
                ) VALUES 
                (
                    @Ip,
                    @PrettyName,
                    @CpuLoad,
                    @CpuUp,
                    @PrettyMemoryUsed,
                    @PrettyTotalMemory,
                    @PercentMemoryUsed,
                    @MemoryUp,
                    @PrettyTotalNetwork,
                    @DeviceTypeId,
                    @Temperature,
                    @TemperatureUp,
                    @DiskUsed,
                    @DiskUp,
                    @CreatedTime,
                    @CreatedBy,
                    @UpdatedTime,
                    @UpdatedBy
                    );
                    ";
                sql += "SELECT LAST_INSERT_ID() ";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, obj);
                obj.Id = newid;
                return obj;
            });
        }

        public async Task<ConsulDevice> GetByID(long id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<ConsulDevice>(
                    "SELECT * FROM consul_device WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<int> Update(ConsulDevice obj)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync($@" UPDATE consul_device set 
                    
                    ip=@Ip,
                    pretty_name=@PrettyName,
                    cpu_load=@CpuLoad,
                    cpu_up=@CpuUp,
                    pretty_memory_used=@PrettyMemoryUsed,
                    pretty_total_memory=@PrettyTotalMemory,
                    percent_memory_used=@PercentMemoryUsed,
                    memory_up=@MemoryUp,
                    pretty_total_network=@PrettyTotalNetwork,
                    device_type_id=@DeviceTypeId,
                    temperature=@Temperature,
                    temperature_up=@TemperatureUp,
                    disk_used=@DiskUsed,
                    disk_up=@DiskUp,
                    created_time=@CreatedTime,
                    created_by=@CreatedBy,
                    updated_time=@UpdatedTime,
                    updated_by=@UpdatedBy
                 where id=@Id", obj);
                return result;
            });
        }

        public async Task<int> Delete(string[] ids, int userID)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Update consul_device set is_del=1" +
                ",updated_time=@updated_time,updated_by=@updated_by" +
                " WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                return result;
            });
        }
    }
}



