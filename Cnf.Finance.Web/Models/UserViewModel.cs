using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Cnf.Finance.Entity;

namespace Cnf.Finance.Web.Models
{
    [Flags]
    public enum UserRole
    {
        [Display(Name ="无职能")]
        None = 0b0000,
        [Display(Name ="系统管理员")]
        SystemAdmin = 0b0001,
        [Display(Name ="计划员")]
        Planner=0b0010,
        [Display(Name ="统计员")]
        Reporter=0b0100,
        [Display(Name ="督导员")]
        Supervisor=0b1000,
    }

    public class UserViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "登录账号")]
        [Required(ErrorMessage = "必须输入登录账号")]
        [MaxLength(20, ErrorMessage = "登录账号不能超过20位")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "登录账号只能是字母和数字组合")]
        public string Login { get; set; }

        [Display(Name = "初始口令")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "必须设置初始口令")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,20}$", ErrorMessage = "口令必须包含大小写字母和数字，在8~20位之间")]
        public string Password { get; set; }

        [Display(Name = "姓名")]
        [Required(ErrorMessage = "必须输入姓名")]
        [MaxLength(20, ErrorMessage = "姓名最长不能操作20位")]
        public string UserName { get; set; }

        [Display(Name = "角色")]
        public UserRole Role { get; set; }

        public bool IsSystemAdmin { get; set; }

        public bool IsPlanner { get; set; }

        public bool IsReporter { get; set; }

        public bool IsSupervisor { get; set; }


        [Display(Name ="是否有效")]
        public bool ActiveStatus { get; set; }

        [Display(Name ="管理单位")]
        public int? OrganizationId { get; set; }

        public static implicit operator UserViewModel(Users user)=>
            new UserViewModel
            {
                ActiveStatus = user.ActiveStatus,
                Login = user.Login,
                Password = user.Password,
                UserName = user.UserName,
                Role = (UserRole)user.Role,
                UserId = user.UserId,
                OrganizationId = user.OrganizationId,
                IsSystemAdmin = UserRole.SystemAdmin == (UserRole.SystemAdmin & (UserRole)user.Role),
                IsPlanner = UserRole.Planner == (UserRole.Planner & (UserRole)user.Role),
                IsReporter = UserRole.Reporter == (UserRole.Reporter & (UserRole)user.Role),
                IsSupervisor = UserRole.Supervisor == (UserRole.Supervisor & (UserRole)user.Role),
            };
    }

    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "登录名")]
        public string Login { get; set; }

        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Display(Name = "旧口令")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "新口令"), DataType(DataType.Password)]
        [Required(ErrorMessage = "必须输入符合强密码规则的口令")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,20}$", ErrorMessage = "口令必须包含大小写字母和数字，在8~20位之间")]
        public string NewPassword { get; set; }

        [Display(Name = "确认口令"), DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "确认口令和新口令不一致")]
        public string ConfirmPassword { get; set; }

        public static implicit operator ChangePasswordViewModel(Users user) =>
            new ChangePasswordViewModel
            {
                UserId = user.UserId,
                Login = user.Login,
                Name = user.UserName
            };
    }
}
