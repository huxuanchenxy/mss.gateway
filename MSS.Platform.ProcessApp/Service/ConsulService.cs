using Consul;
using Microsoft.Extensions.Configuration;
using MSS.API.Common.Utility;
using MSS.Platform.ProcessApp.Data;
using MSS.Platform.ProcessApp.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace MSS.Platform.ProcessApp.Service
{
    public class ConsulService : IConsulService
    {
        public IConfiguration _configuration { get; }
        private readonly IConsulRepo<ConsulServiceEntity> _repo;
        public ConsulService(IConfiguration configuration, IConsulRepo<ConsulServiceEntity> repo)
        {
            _configuration = configuration;
            _repo = repo;
        }


        public async Task<ApiResult> GetPageByParm(ConsulServiceEntityParm parm)
        {
            ApiResult ret = new ApiResult();

            try
            {
                string consulurl = "http://" + _configuration["ConsulServiceEntity:ConsulIP"] + ":" + _configuration["ConsulServiceEntity:ConsulPort"];


                var consuleClient = new ConsulClient(consulConfig =>
                {
                    consulConfig.Address = new Uri(consulurl);
                });

                var consulhealthdata = await consuleClient.Agent.Services();
                var consulhealthresponse = consulhealthdata.Response;
                List<ConsulObj> consulhealthlist = new List<ConsulObj>();
                foreach (var c in consulhealthresponse)
                {
                    consulhealthlist.Add(new ConsulObj() { ID = c.Value.ID, Service = c.Value.Service });
                }

                //var registedservice = _configuration.GetSection("ConsulServiceDB").Get<List<ConsulServiceEntity>>();
                ConsulServiceEntityView data = new ConsulServiceEntityView();
                var registedservice = await _repo.GetPageList(parm);
                data.total = registedservice.total;
                var dbdatalist = registedservice.rows;
                var query = from c in dbdatalist
                            join o in consulhealthlist on c.ServiceName equals o.Service
                             into g
                            from tt in g.Where(o => o.ID == c.ServiceAddr + ":" + c.ServicePort).DefaultIfEmpty()
                            select new ConsulServiceEntity
                            {
                                ServiceName = c.ServiceName,
                                ServiceAddr = c.ServiceAddr,
                                ServicePort = c.ServicePort
                            ,
                                HealthStatus = tt != null ? ConsulServiceStatus.Running : ConsulServiceStatus.Closed,
                                CreatedTime = c.CreatedTime,
                                UpdatedTime = c.UpdatedTime,
                                ID = c.ID,
                                ServicePID = c.ServicePID
                            };

                data.rows = query.Cast<ConsulServiceEntity>().ToList<ConsulServiceEntity>();
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }


        public async Task<int> StartProcess(int id)
        {
            //Console.WriteLine("请输入要执行的命令:");
            //string strInput = Console.ReadLine();

            var data = await _repo.GetById(id);
            //string strInput = _configuration["BatPath"] + "\\" + data.ServiceName + ".bat";
            string strInput = data.ServiceDll + " " + data.ServicePort;
            int pid = DOCMD2(strInput);
            await RegisterConsul(id);
            await _repo.UpdById(new ConsulServiceEntity() { ID = id, ServicePID = pid });
            return pid;
        }

        private int DOCMD(string strInput)
        {
            Process p = new Process();
            //设置要启动的应用程序
            p.StartInfo.FileName = "cmd.exe";
            //是否使用操作系统shell启动
            p.StartInfo.UseShellExecute = false;
            // 接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardInput = true;
            //输出信息
            p.StartInfo.RedirectStandardOutput = true;
            // 输出错误
            p.StartInfo.RedirectStandardError = true;
            //不显示程序窗口
            p.StartInfo.CreateNoWindow = false;

            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

            //p.StartInfo.Arguments = strInput;
            //启动程序
            p.Start();

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(strInput);

            p.StandardInput.AutoFlush = true;
            p.WaitForExit(2000);
            p.Kill();
            return p.Id;
            //获取输出信息
            //string strOuput = p.StandardOutput.ReadToEnd();
            //等待程序执行完退出进程
            //p.WaitForExit();
            //p.Close();

            //Console.WriteLine(strOuput);
            //Console.ReadKey();
            ////创建一个ProcessStartInfo对象 使用系统shell 指定命令和参数 设置标准输出
            //var psi = new ProcessStartInfo("dotnet", "--info") { RedirectStandardOutput = true };
            ////启动
            //var proc = Process.Start(psi);
        }

        private int DOCMD2(string strInput)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfo.FileName = "dotnet.exe";
            startInfo.Arguments = strInput;
            //startInfo.RedirectStandardInput = false;
            //startInfo.RedirectStandardOutput = false;
            //startInfo.CreateNoWindow = false;
            process.StartInfo = startInfo;
            process.Start();
            return process.Id;
        }

        public async Task<int> StopProcess(int id)
        {
            var data = await _repo.GetById(id);
            string deregistconsul = "http://" + _configuration["ConsulServiceEntity:ConsulIP"] + ":" + _configuration["ConsulServiceEntity:ConsulPort"] + "/v1/agent/service/deregister/" + data.ServiceAddr + ":" + data.ServicePort;
            HttpClientHelper.PutResponse(deregistconsul, new object());
            //string strInput = "cmd /c TASKKILL /T /F /PID " + data.ServicePID;
            string strInput = "TASKKILL /T /F /PID " + data.ServicePID;
            //string strInput = "/c taskkill /fi \"WINDOWTITLE eq "+data.ServiceName+"*\" /t /f ";
            return DOCMD(strInput);
        }

        private async Task RegisterConsul(int id)
        {
            var data = await _repo.GetById(id);
            var postdata = new AgentServiceRegistration()
            {
                ID = data.ServiceAddr + ":" + data.ServicePort,
                Name = data.ServiceName,
                Address = data.ServiceAddr,
                Port = data.ServicePort,
                Tags = new[] { $"urlprefix-/{data.ServiceName}" },
                Checks = new[] { new  AgentServiceCheck()
                                {
                                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                                    Interval = TimeSpan.FromSeconds(10),
                                    HTTP = $"http://{data.ServiceAddr}:{data.ServicePort}/health",
                                    Timeout = TimeSpan.FromSeconds(5),
                                } },
            };
            string url = $@"http://{_configuration["ConsulServiceEntity:ConsulIP"]}:{_configuration["ConsulServiceEntity:ConsulPort"]}/v1/agent/service/register";
            HttpClientHelper.PutResponse(url, postdata);

        }


        public static void KillProcessAndChildren(int pid)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                Console.WriteLine(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                /* process already exited */
            }
        }

        public async Task<ConsulServiceEntity> GetById(int id)
        {
            var data = await _repo.GetById(id);
            return data;
        }

        public async Task<ApiResult> RemoteStartProcess(int id)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            var data = await _repo.GetById(id);
            string url = "http://" + data.ServiceAddr.Trim() + ":" + _configuration["ConsulServiceEntity:Port"] + "/api/v1/Consul/start/" + id;
            try
            {
                HttpClientHelper.GetResponse(url);
                ret.code = Code.Success;
            }
            catch (Exception ex)
            {
                ret.msg = ex.ToString();
            }
            return ret;
        }

        public async Task<ApiResult> RemoteEndProcess(int id)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            var data = await _repo.GetById(id);
            string url = "http://" + data.ServiceAddr.Trim() + ":" + _configuration["ConsulServiceEntity:Port"] + "/api/v1/Consul/stop/" + id;
            try
            {
                HttpClientHelper.GetResponse(url);
                ret.code = Code.Success;
            }
            catch (Exception ex)
            {
                ret.msg = ex.ToString();
            }
            return ret;
        }


    }

    public interface IConsulService
    {
        Task<ApiResult> GetPageByParm(ConsulServiceEntityParm parm);
        Task<int> StartProcess(int id);

        Task<int> StopProcess(int id);
        Task<ConsulServiceEntity> GetById(int id);
        Task<ApiResult> RemoteStartProcess(int id);
        Task<ApiResult> RemoteEndProcess(int id);
    }


}
