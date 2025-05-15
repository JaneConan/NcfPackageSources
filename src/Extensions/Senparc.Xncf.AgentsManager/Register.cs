using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Senparc.CO2NET.RegisterServices;
using Senparc.Ncf.Core;
using Senparc.Ncf.Core.Enums;
using Senparc.Ncf.Core.Models;
using Senparc.Ncf.XncfBase;
using Senparc.Ncf.XncfBase.Database;
using Senparc.Xncf.AgentsManager.Domain.Models.DatabaseModel;
using Senparc.Xncf.AgentsManager.Domain.Models.DatabaseModel.Dto;
using Senparc.Xncf.AgentsManager.Domain.Services;
using Senparc.Xncf.AgentsManager.Domain.Services.AIPlugins;
using Senparc.Xncf.AgentsManager.Models;
using Senparc.Xncf.AgentsManager.Models.DatabaseModel;
using Senparc.Xncf.AgentsManager.Models.DatabaseModel.Models;
using Senparc.Xncf.AgentsManager.Models.DatabaseModel.Models.Dto;
using Senparc.Xncf.XncfBuilder.OHS.Local;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Senparc.Xncf.AgentsManager
{
    [XncfRegister]
    public partial class Register : XncfRegisterBase, IXncfRegister
    {
        #region IXncfRegister 接口

        public override string Name => "Senparc.Xncf.AgentsManager";

        public override string Uid => "D858D7FA-775A-4690-9023-CFB0B3B84994";//必须确保全局唯一，生成后必须固定，已自动生成，也可自行修改

        public override string Version => "0.3.18.9";//必须填写版本号

        public override string MenuName => "Agents 管理模块";

        public override string Icon => "fa fa-star";

        public override string Description => "Agents 管理模块";

        public override async Task InstallOrUpdateAsync(IServiceProvider serviceProvider, InstallOrUpdate installOrUpdate)
        {
            //安装或升级版本时更新数据库
            await XncfDatabaseDbContext.MigrateOnInstallAsync(serviceProvider, this);

            //根据安装或更新不同条件执行逻辑
            switch (installOrUpdate)
            {
                case InstallOrUpdate.Install:
                    //新安装
                    #region 初始化数据库数据
                    //var colorService = serviceProvider.GetService<ColorAppService>();
                    //var colorResult = await colorService.GetOrInitColorAsync();
                    #endregion
                    break;
                case InstallOrUpdate.Update:
                    //更新
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override async Task UninstallAsync(IServiceProvider serviceProvider, Func<Task> unsinstallFunc)
        {
            #region 删除数据库（演示）

            var mySenparcEntitiesType = this.TryGetXncfDatabaseDbContextType;
            AgentsManagerSenparcEntities mySenparcEntities = serviceProvider.GetService(mySenparcEntitiesType) as AgentsManagerSenparcEntities;

            //指定需要删除的数据实体

            //注意：这里作为演示，在卸载模块的时候删除了所有本模块创建的表，实际操作过程中，请谨慎操作，并且按照删除顺序对实体进行排序！
            var dropTableKeys = EntitySetKeys.GetEntitySetInfo(this.TryGetXncfDatabaseDbContextType).Keys.ToArray();
            await base.DropTablesAsync(serviceProvider, mySenparcEntities, dropTableKeys);

            #endregion
            await unsinstallFunc().ConfigureAwait(false);
        }
        #endregion

        public override IServiceCollection AddXncfModule(IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            //AutoMap映射
            base.AddAutoMapMapping(profile =>
            {
                profile.CreateMap<AgentTemplate, AgentTemplateDto>().ReverseMap();
                profile.CreateMap<AgentTemplate, AgentTemplateSimpleStatusDto>().ReverseMap();
                profile.CreateMap<ChatGroup, ChatGroupDto>().ReverseMap();
                profile.CreateMap<ChatGroupMember, ChatGroupMemberDto>().ReverseMap();
                profile.CreateMap<ChatGroupHistory, ChatGroupHistoryDto>().ReverseMap();
                profile.CreateMap<ChatTask, ChatTaskDto>().ReverseMap();
            });

            //Service DI
            services.AddScoped<AgentsTemplateService>();
            services.AddScoped<ChatGroupService>();
            services.AddScoped<ChatGroupHistoryService>();
            services.AddScoped<ChatTaskService>();
            services.AddScoped<ChatGroupMemberService>();

            //AI Plugins DI
            services.AddScoped<CrawlPlugin>();
            services.AddScoped<FormatorPlugin>();
            services.AddScoped<TranslatorPlugin>();

            //测试
            services.AddScoped<BuildXncfAppService>();

            return base.AddXncfModule(services, configuration, env);
        }
        public override IApplicationBuilder UseXncfModule(IApplicationBuilder app, IRegisterService registerService)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new ManifestEmbeddedFileProvider(Assembly.GetExecutingAssembly(), "wwwroot")
            });

            var aiPlugins = AIPluginHub.Instance;
            aiPlugins.Add(typeof(CrawlPlugin));
            aiPlugins.Add(typeof(FormatorPlugin));
            aiPlugins.Add(typeof(TranslatorPlugin));

            return base.UseXncfModule(app, registerService);
        }
    }
}
















