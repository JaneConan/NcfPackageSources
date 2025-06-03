﻿using Microsoft.Extensions.DependencyInjection;
using Senparc.CO2NET;
using Senparc.Ncf.Core.AppServices;
using Senparc.Ncf.Core.Models;
using Senparc.Ncf.Service;
using Senparc.Ncf.XncfBase.FunctionRenders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.Xncf.DatabaseToolkit.OHS.Local.AppService
{
    public class DatabaseConfigAppService : AppServiceBase
    {
        public DatabaseConfigAppService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }


        #region SetConfig

        public class SetConfigFunctionAppRequest : FunctionAppRequestBase
        {
            [Required]
            [MaxLength(300)]
            [Description("自动备份周期（分钟）||0 则为不自动备份")]
            public int BackupCycleMinutes { get; set; }

            [Required]
            [MaxLength(300)]
            [Description("备份路径||本地物理路径，如：E:\\Senparc\\Ncf\\NCF.bak")]
            public string BackupPath { get; set; }

            public override async Task LoadData(IServiceProvider serviceProvider)
            {
                var configService = serviceProvider.GetService<ServiceBase<DbConfig>>();
                var config = await configService.GetObjectAsync(z => true);
                if (config != null)
                {
                    BackupCycleMinutes = config.BackupCycleMinutes;
                    BackupPath = config.BackupPath;
                }
            }
        }

        [FunctionRender("设置参数", "设置备份间隔时间、备份文件路径等参数", typeof(Register))]
        public async Task<StringAppResponse> SetConfig(SetConfigFunctionAppRequest request)
        {
            return await this.GetStringResponseAsync(async (response, logger) =>
            {
                var configService = base.ServiceProvider.GetService<ServiceBase<DbConfig>>();
                var config = configService.GetObject(z => true);
                if (config == null)
                {
                    config = new DbConfig(request.BackupCycleMinutes, request.BackupPath);
                }
                else
                {
                    //configService.Mapper.Map(request, config);
                    config.SetConfig(request.BackupCycleMinutes, request.BackupPath);
                }
                configService.SaveObject(config);

                var msg = $"设置间隔分钟：{request.BackupCycleMinutes}，路径：{request.BackupPath}";
                logger.Append(msg);
                return msg;
            }, afterFunc: (response, logger) =>
            {
                logger.Append($"设置已保存！");
            },
            saveLogAfterFinished: true,
            saveLogName:"设置数据库信息", 
            exceptionHandler: async (ex,response, logger) => { 
            
            
            }
            );
        }

        #endregion


        [ApiBind]
        [FunctionRender("查看数据库配置类型", "查看实现 IDatabaseConfiguration 接口的数据库配置类型", typeof(Register))]
        public async Task<StringAppResponse> ShowDatabaseConfiguration()
        {
            return await this.GetStringResponseAsync(async (response, logger) =>
            {
                var databaseConfigurationFactory = DatabaseConfigurationFactory.Instance;
                var currentDatabaseConfiguration = databaseConfigurationFactory.Current;
                return logger.Append($"当前 DatabaseConfiguration：{currentDatabaseConfiguration.GetType().Name}，数据库类型：{currentDatabaseConfiguration.MultipleDatabaseType}");
            });
        }
    }
}
