using Cnf.Finance.Entity;
using Cnf.Finance.Web.Models;
using Cnf.Finance.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cnf.Finance.Web
{
    public static class Helper
    {
        const string COOKIE_USERID = "userid";
        const string COOKIE_USERNAME = "name";
        const string COOKIE_ROLE = "role";
        const string COOKIE_ORGANIZATIONID = "organizationid";

        static UserRole GetRole(HttpContext context) => int.TryParse(context.User?.FindFirstValue(COOKIE_ROLE), out var role) ? (UserRole)role : UserRole.None;

        public static int GetUserID(HttpContext context) => int.TryParse(context.User?.FindFirstValue(COOKIE_USERID), out var userId) ? userId : 0;

        public static string GetUserName(HttpContext context) => context.User?.FindFirstValue(COOKIE_USERNAME);

        public static int? GetUserOrgId(HttpContext context)
        {
            var orgIdStr = context.User?.FindFirstValue(COOKIE_ORGANIZATIONID);
            if (string.IsNullOrWhiteSpace(orgIdStr))
                return null;
            else
            {
                int orgId = Convert.ToInt32(orgIdStr);
                if (orgId <= 0)
                    return null;
                else
                    return orgId;
            }
        }

        public static bool AllowAllOrgs(HttpContext context, out int? allowedOrgId) =>
            (allowedOrgId = GetUserOrgId(context)) == null;

        public static bool IsSystemAdmin(UserRole role) =>
            (role & UserRole.SystemAdmin) == UserRole.SystemAdmin;
        public static bool IsSystemAdmin(HttpContext context) => IsSystemAdmin(GetRole(context));

        public static bool IsPlanner(UserRole role) =>
            (role & UserRole.Planner) == UserRole.Planner;
        public static bool IsPlanner(HttpContext context) => IsPlanner(GetRole(context));

        public static bool IsReporter(UserRole role) =>
            (role & UserRole.Reporter) == UserRole.Reporter;
        public static bool IsReporter(HttpContext context) => IsReporter(GetRole(context));

        public static bool IsSupervisor(UserRole role) =>
            (role & UserRole.Supervisor) == UserRole.Supervisor;
        public static bool IsSupervisor(HttpContext context) => IsSupervisor(GetRole(context));

        internal static void Signout(HttpContext context)
        {
            context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        internal static void Signin(Users user, HttpContext context)
        {
            var claims = new List<Claim>
                {
                    new Claim(COOKIE_USERID, user.UserId.ToString()),
                    new Claim(COOKIE_USERNAME, user.UserName),
                    new Claim(COOKIE_ROLE, user.Role.ToString()),
                    new Claim(COOKIE_ORGANIZATIONID, $"{user.OrganizationId}"),
                };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var properties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
            };

            var principal = new ClaimsPrincipal(identity);

            context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        }
    }

    public static class Extension
    {
        enum PropertyType
        {
            NotSupported,
            Decimal,
            Sigle,
            Double,
            Int
        }

        static void ClearZeroProperty(object obj, PropertyInfo property)
        {
            var value = property.GetValue(obj);
            if (value != null)
            {
                if (property.IsNullableNumeric(out _))
                {
                    if (Convert.ToDouble(value) == 0D)
                        property.SetValue(obj, null);
                }
            }
        }

        static bool IsNullableNumeric(this PropertyInfo property, out PropertyType baseType)
        {
            Type type = property.PropertyType;
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var name = type.GetGenericArguments()[0].FullName;
                    baseType = name switch
                    {
                        "System.Int16" => PropertyType.Int,
                        "System.Int32" => PropertyType.Int,
                        "System.Int64" => PropertyType.Int,
                        "System.Single" => PropertyType.Sigle,
                        "System.Decimal" => PropertyType.Decimal,
                        "System.Double" => PropertyType.Double,
                        _ => PropertyType.NotSupported,
                    };
                    return true;
                }
            }
            baseType = PropertyType.NotSupported;
            return false;
        }

        /// <summary>
        /// 将对象所有为0的可为空数值类型属性设置为null
        /// </summary>
        /// <param name="obj"></param>
        public static void ClearZeroProperties(this object obj)
        {
            Type type = obj.GetType();
            foreach (var prop in type.GetProperties())
            {
                ClearZeroProperty(obj, prop);
            }
        }

        /// <summary>
        /// 将record中的所有数值累加到当前对象的对应数值上。
        /// </summary>
        /// <param name="record"></param>
        public static void Add(this YearGroupRecord result, YearGroupRecord record)
        {
            result.AnnualBalance.Incoming += record.AnnualBalance?.Incoming ?? 0M;
            result.AnnualBalance.Settlement += record.AnnualBalance?.Settlement ?? 0M;
            result.AnnualBalance.Retrievable += record.AnnualBalance?.Retrievable ?? 0M;
            result.AnnualBalance.Tax += record.AnnualBalance?.Tax ?? 0M;

            result.Accumulation.Plan.Incoming = result.Accumulation.Plan.Incoming.AddNullableDecimal(record.Accumulation.Plan.Incoming);
            result.Accumulation.Plan.Settlement = result.Accumulation.Plan.Settlement.AddNullableDecimal(record.Accumulation.Plan.Settlement);
            result.Accumulation.Plan.Retrievable = result.Accumulation.Plan.Retrievable.AddNullableDecimal(record.Accumulation.Plan.Retrievable);
            result.Accumulation.Plan.Tax = result.Accumulation.Plan.Tax.AddNullableDecimal(record.Accumulation.Plan.Tax);
            result.Accumulation.Perform.Incoming = result.Accumulation.Perform.Incoming.AddNullableDecimal(record.Accumulation.Perform.Incoming);
            result.Accumulation.Perform.Settlement = result.Accumulation.Perform.Settlement.AddNullableDecimal(record.Accumulation.Perform.Settlement);
            result.Accumulation.Perform.Retrievable = result.Accumulation.Perform.Retrievable.AddNullableDecimal(record.Accumulation.Perform.Retrievable);
            result.Accumulation.Perform.Tax = result.Accumulation.Perform.Tax.AddNullableDecimal(record.Accumulation.Perform.Tax);

            result.CurrentMonth.Plan.Incoming = result.CurrentMonth.Plan.Incoming.AddNullableDecimal(record.CurrentMonth.Plan.Incoming);
            result.CurrentMonth.Plan.Settlement = result.CurrentMonth.Plan.Settlement.AddNullableDecimal(record.CurrentMonth.Plan.Settlement);
            result.CurrentMonth.Plan.Retrievable = result.CurrentMonth.Plan.Retrievable.AddNullableDecimal(record.CurrentMonth.Plan.Retrievable);
            result.CurrentMonth.Plan.Tax = result.CurrentMonth.Plan.Tax.AddNullableDecimal(record.CurrentMonth.Plan.Tax);
            result.CurrentMonth.Perform.Incoming = result.CurrentMonth.Perform.Incoming.AddNullableDecimal(record.CurrentMonth.Perform.Incoming);
            result.CurrentMonth.Perform.Settlement = result.CurrentMonth.Perform.Settlement.AddNullableDecimal(record.CurrentMonth.Perform.Settlement);
            result.CurrentMonth.Perform.Retrievable = result.CurrentMonth.Perform.Retrievable.AddNullableDecimal(record.CurrentMonth.Perform.Retrievable);
            result.CurrentMonth.Perform.Tax = result.CurrentMonth.Perform.Tax.AddNullableDecimal(record.CurrentMonth.Perform.Tax);
        }

        /// <summary>
        /// 在based上加上一个nullable的数，返回和
        /// </summary>
        /// <param name="based"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static decimal? AddNullableDecimal(this decimal? based, decimal? d) =>
            d == null ? based : based == null ? d : based + d;

        /// <summary>
        /// 在based上加上一个nullable的数，返回和
        /// </summary>
        /// <param name="based"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static decimal? AddNullableDecimal(this decimal based, decimal? d) =>
            d == null ? based : based + d;

        /// <summary>
        /// 从based减去一个nullable的数，返回差
        /// </summary>
        /// <param name="based"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static decimal? SubstractNullableDecimal(this decimal? based, decimal? d) =>
            d == null ? based : based == null ? -1 * d : based - d;
    }
}
