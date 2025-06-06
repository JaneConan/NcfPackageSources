﻿using Senparc.Ncf.Core.Enums;
using Senparc.Ncf.XncfBase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

#if (Database || Sample)
using Template_OrgName.Xncf.Template_XncfName.Models;
using Template_OrgName.Xncf.Template_XncfName.OHS.Local.AppService;
using Senparc.Ncf.Core.Models;
using Senparc.Ncf.Database;
using Senparc.Ncf.XncfBase.Database;
#endif
#if (Sample)
using Template_OrgName.Xncf.Template_XncfName.Domain.Models.DatabaseModel.Dto;
#endif

namespace Template_OrgName.Xncf.Template_XncfName
{
    [XncfRegister]
    public partial class Register : XncfRegisterBase, IXncfRegister
    {
        #region IXncfRegister 接口

        public override string Name => "Template_OrgName.Xncf.Template_XncfName";

        public override string Uid => "Template_Guid";//必须确保全局唯一，生成后必须固定，已自动生成，也可自行修改

        public override string Version => "Template_Version";//必须填写版本号

        public override string MenuName => "Template_MenuName";

        public override string Icon => "Template_Icon";

        public override string Description => "Template_Description";

        public override async Task InstallOrUpdateAsync(IServiceProvider serviceProvider, InstallOrUpdate installOrUpdate)
        {
#if (Database || Sample)
            //安装或升级版本时更新数据库
            await XncfDatabaseDbContext.MigrateOnInstallAsync(serviceProvider, this);

            //根据安装或更新不同条件执行逻辑
            switch (installOrUpdate)
            {
                case InstallOrUpdate.Install:
                    //新安装
#if (Sample)
                    #region 初始化数据库数据
                    var colorService = serviceProvider.GetService<ColorAppService>();
                    var colorResult = await colorService.GetOrInitColorAsync();
                    #endregion
#endif
                    break;
                case InstallOrUpdate.Update:
                    //更新
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#endif
        }

        public override async Task UninstallAsync(IServiceProvider serviceProvider, Func<Task> unsinstallFunc)
        {
#if (Database || Sample)
            #region 删除数据库（演示）

            var mySenparcEntitiesType = this.TryGetXncfDatabaseDbContextType;
            Template_XncfNameSenparcEntities mySenparcEntities = serviceProvider.GetService(mySenparcEntitiesType) as Template_XncfNameSenparcEntities;

            //指定需要删除的数据实体

            //注意：这里作为演示，在卸载模块的时候删除了所有本模块创建的表，实际操作过程中，请谨慎操作，并且按照删除顺序对实体进行排序！
            var dropTableKeys = EntitySetKeys.GetEntitySetInfo(this.TryGetXncfDatabaseDbContextType).Keys.ToArray();
            await base.DropTablesAsync(serviceProvider, mySenparcEntities, dropTableKeys);

            #endregion
#endif
            await unsinstallFunc().ConfigureAwait(false);
        }
        #endregion

        public override IServiceCollection AddXncfModule(IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
#if (Database || Sample)
            services.AddScoped<ColorAppService>();
            
            services.AddAutoMapper(z =>
            {
                z.CreateMap<Color, ColorDto>().ReverseMap();
            });
#endif
            return base.AddXncfModule(services, configuration, env);
        }
    }
}
