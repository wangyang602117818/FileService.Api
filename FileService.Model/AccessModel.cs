using System;
using System.Collections.Generic;
using System.Text;

namespace FileService.Model
{
    public class AccessModel
    {
        /// <summary>
        /// 权限属于哪个公司code
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// 用于前端显示
        /// </summary>
        public string CompanyDisplay { get; set; }
        /// <summary>
        /// 前端选择了那些部门code
        /// </summary>
        public string[] DepartmentCodes { get; set; }
        /// <summary>
        /// 前端选择好的部门存起来，修改的时候便于展示
        /// </summary>
        public string[] DepartmentDisplay { get; set; }
        /// <summary>
        /// 前端选择好的权限类型
        /// </summary>
        public string Authority { get; set; }
        /// <summary>
        /// 正真用于判断权限的部门code列表
        /// </summary>
        public string[] AccessCodes { get; set; }
        /// <summary>
        /// 正真用于判断权限的用户code列表
        /// </summary>
        public string[] AccessUsers { get; set; }
    }
}
