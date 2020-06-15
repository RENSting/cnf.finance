﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Cnf.Finance.Entity;

namespace Cnf.Finance.Web.Models
{
    public class MonthDataViewModel
    {
        /// <summary>
        /// 月计划的ID，或者月完成的ID
        /// </summary>
        public int Id { get; set; }

        [Display(Name ="月份")]
        [Required(ErrorMessage ="必须输入月份")]
        [Range(1, 12, ErrorMessage ="月份必须在1~12月之间")]
        public int Month { get; set; }

        [Display(Name ="收入(万)")]
        [Required(ErrorMessage ="必须输入收入，如无请输入0")]
        [RegularExpression(@"^(\-|\+)?\d+(\.\d+)?$", ErrorMessage = "必须输入数字")]
        [DisplayFormat(DataFormatString = "{0:#.###}")]
        public decimal? Incoming { get; set; }

        [Display(Name = "结算(万)")]
        [Required(ErrorMessage = "必须输入结算，如无请输入0")]
        [RegularExpression(@"^(\-|\+)?\d+(\.\d+)?$", ErrorMessage = "必须输入数字")]
        [DisplayFormat(DataFormatString = "{0:#.###}")]
        public decimal? Settlement { get; set; }

        [Display(Name = "回款(万)")]
        [Required(ErrorMessage = "必须输入回款，如无请输入0")]
        [RegularExpression(@"^(\-|\+)?\d+(\.\d+)?$", ErrorMessage = "必须输入数字")]
        [DisplayFormat(DataFormatString = "{0:#.###}")]
        public decimal? Retrievable { get; set; }

        public static implicit operator MonthDataViewModel(Perform perform)
            => perform==null? new MonthDataViewModel()
            :new MonthDataViewModel
            {
                Id = perform.Id,
                Month = perform.Month,
                Incoming = perform.Incoming, // == null ? default : perform.Incoming.Value,
                Settlement = perform.Settlement, // == null ? default : perform.Settlement.Value,
                Retrievable = perform.Retrieve, // == null ? default : perform.Retrieve.Value,
            };

        public static implicit operator MonthDataViewModel(Plan plan)
            => plan==null? new MonthDataViewModel()
            : new MonthDataViewModel
            {
                Id = plan.Id,
                Month = plan.Month,
                Incoming = plan.Incoming, // == null ? default : plan.Incoming.Value,
                Settlement = plan.Settlement, // == null ? default : plan.Settlement.Value,
                Retrievable = plan.Retrieve, // == null ? default : plan.Retrieve.Value,
            };
    }
}
