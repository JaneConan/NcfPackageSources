﻿using Senparc.Ncf.XncfBase.FunctionRenders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senparc.Ncf.XncfBase.Functions;
using Senparc.Xncf.AgentsManager.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Senparc.Ncf.Service;
using Senparc.Xncf.AgentsManager.Models.DatabaseModel.Models;
using Senparc.Xncf.XncfBuilder.OHS.PL;
using System.Web.Mvc;
using Senparc.Xncf.AgentsManager.Domain.Models.DatabaseModel;

namespace Senparc.Xncf.AgentsManager.OHS.Local.PL
{
    public class ChatGroup_ManageChatGroupRequest : FunctionAppRequestBase
    {
        [Description("选择组||选择需要操作的组，或新增")]
        public SelectionList ChatGroup { get; set; } = new SelectionList(SelectionType.DropDownList, new List<SelectionItem> {
                 new SelectionItem("New","新建组","新建",true)
            });

        [Required]
        [MaxLength(30)]
        [Description("群名称||群名称")]
        public string Name { get; set; }

        [Required]
        [Description("群成员||群成员")]
        public SelectionList Members { get; set; } = new SelectionList(SelectionType.CheckBoxList, new List<SelectionItem>());

        [Required]
        [Description("群主||群管理员，群管理员不会被合并到“群成员”中，通常不参与显式的发言。")]
        public SelectionList Admin { get; set; } = new SelectionList(SelectionType.DropDownList, new List<SelectionItem>());

        [Required]
        [Description("对接人||对接人，即接受命令的人，通常也是期待返回期望结果的人。对接人也会被合并到“群成员”中")]
        public SelectionList EnterAgent { get; set; } = new SelectionList(SelectionType.DropDownList, new List<SelectionItem>());


        [MaxLength(200)]
        [Description("说明||说明")]
        public string Description { get; set; }

        public override async Task LoadData(IServiceProvider serviceProvider)
        {
            //ChatGroup
            var chatGroupService = serviceProvider.GetService<ChatGroupService>();
            var chatGroups = await chatGroupService.GetFullListAsync(z => true, z => z.Id, Ncf.Core.Enums.OrderingType.Ascending);

            chatGroups.Select(z => new SelectionItem(z.Id.ToString(), z.Name, z.Description))
                .ToList().ForEach(z => ChatGroup.Items.Add(z));

            //Agent
            var agentTemplateService = serviceProvider.GetService<AgentsTemplateService>();
            var agentsTemplates = await agentTemplateService.GetFullListAsync(z => z.Enable, z => z.Name, Ncf.Core.Enums.OrderingType.Ascending);

            Members.Items = agentsTemplates.Select(z => new SelectionItem(z.Id.ToString(), z.Name, z.Description)).ToList();
            Admin.Items = agentsTemplates.Select(z => new SelectionItem(z.Id.ToString(), z.Name, z.Description)).ToList();
            EnterAgent.Items = agentsTemplates.Select(z => new SelectionItem(z.Id.ToString(), z.Name, z.Description)).ToList();

            var admin = Admin.Items.FirstOrDefault(z => z.Text == "群主");
            if (admin != null)
            {
                admin.DefaultSelected = true;
            }

            await base.LoadData(serviceProvider);
        }
    }

    public class ChatGroup_RunChatGroupRequest : FunctionAppRequestBase
    {
        [Description("选择组||选择需要运行的组")]
        public SelectionList ChatGroups { get; set; } = new SelectionList(SelectionType.CheckBoxList, new List<SelectionItem>());

        [Description("AI 模型||请选择运行此程序的外围 AI 模型")]
        public SelectionList AIModel { get; set; } = new SelectionList(SelectionType.DropDownList, new List<SelectionItem>
        {
            //new SelectionItem("Default","系统默认","通过系统默认配置的固定 AI 模型信息",true)
        });

        [Description("个性化智能体||")]
        public SelectionList Individuation { get; set; } = new SelectionList(SelectionType.CheckBoxList, new List<SelectionItem>
        {
            new SelectionItem("1","是","采用个性化 AI 参数运行 Agent",true)
        });

        [Required]
        [MaxLength(500)]
        [Description("我能帮你做什么||说明需要 Agents 协助你完成的工作内容")]
        public string Command { get; set; }

        public override async Task LoadData(IServiceProvider serviceProvider)
        {
            //ChatGroup
            var chatGroupService = serviceProvider.GetService<ServiceBase<ChatGroup>>();
            var chatGroups = await chatGroupService.GetFullListAsync(z => true, z => z.Id, Ncf.Core.Enums.OrderingType.Ascending);

            ChatGroups.Items = chatGroups.Select(z => new SelectionItem(z.Id.ToString(), z.Name, z.Description)).ToList();

            //载入 AI 模型
            await BuildXncfRequestHelper.LoadAiModelData(serviceProvider, AIModel);

            await base.LoadData(serviceProvider);
        }
    }

    public class ChatGroup_RunGroupRequest 
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ChatGroup ID
        /// </summary>
        public int ChatGroupId { get; set; }

        /// <summary>
        /// 如果是 0 ，则使用系统默认配置
        /// </summary>
        public int AiModelId { get; set; }

        /// <summary>
        /// 发起对话的要求
        /// </summary>
        public string PromptCommand { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 使用个性化智能体
        /// </summary>
        public bool Personality { get; set; }

        /// <summary>
        /// 消息平台
        /// </summary>
        public HookPlatform HookPlatform { get; set; }
        /// <summary>
        /// 消息平台参数
        /// </summary>
        public string HookParameter { get; set; }

        /// <summary>
        /// 最大对话轮数
        /// </summary>
        public int ChatMaxRound { get; set; } = ChatGroupService.ChatMaxRound;
    }
}
