using FileService.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Business
{
    public class Department : ModelBase<Data.Department>
    {
        public Department() : base(new Data.Department()) { }
        public bool UpdateDepartment(string id, BsonDocument document)
        {
            return mongoData.UpdateDepartment(ObjectId.Parse(id), document);
        }
        public bool ChangeOrder(string id, IEnumerable<BsonDocument> documents)
        {
            return mongoData.ChangeOrder(ObjectId.Parse(id), new BsonArray(documents));
        }
        public IEnumerable<BsonDocument> GetAllDepartment()
        {
            return mongoData.GetAllDepartment();
        }
        public BsonDocument GetByCode(string code)
        {
            return mongoData.GetByCode(code);
        }
        public void GetNamesByCodes(string companyCode, string[] departmentCodes, out string companyName, out List<string> departmentNames)
        {
            BsonDocument bson = mongoData.GetByCode(companyCode);
            companyName = "";
            departmentNames = new List<string> { };
            if (bson != null)
            {
                companyName = bson["DepartmentName"].ToString();
                Dictionary<string, string> dict = new Dictionary<string, string>();
                GetDictionary(bson["Department"].AsBsonArray, dict);
                foreach (var deptCode in departmentCodes)
                {
                    if (dict.ContainsKey(deptCode))
                        departmentNames.Add(dict[deptCode]);
                }
            }
        }
        private void GetDictionary(BsonArray department, Dictionary<string, string> dict)
        {
            foreach (BsonDocument dept in department)
            {
                dict.Add(dept["DepartmentCode"].ToString(), dept["DepartmentName"].ToString());
                if (dept["Department"].AsBsonArray.Count > 0)
                {
                    GetDictionary(dept["Department"].AsBsonArray, dict);
                }
            }
        }
        //public BsonDocument GetByDepartmentCode(string departmentCode)
        //{
        //    return mongoData.GetByDepartmentCode(departmentCode);
        //}
        //public int GetLayer(string departmentCode)
        //{
        //    return mongoData.GetByDepartmentCode(departmentCode)["Layer"].AsInt32;
        //}
        //public DepartmentSelect GetDepartmentSelect()
        //{
        //    IEnumerable<BsonDocument> departments = mongoData.FindAll();
        //    BsonDocument top = departments.Where(w => w["ParentCode"] == BsonNull.Value).FirstOrDefault();
        //    DepartmentSelect departmentSelect = AssembDepartment(top);
        //    GetSubDepartmentSelect(departmentSelect, departments, top["DepartmentCode"].AsString);
        //    return departmentSelect;
        //}
        //private void GetSubDepartmentSelect(DepartmentSelect departmentSelect, IEnumerable<BsonDocument> departments, string parentCode)
        //{
        //    List<DepartmentSelect> temp = new List<DepartmentSelect>();
        //    IEnumerable<BsonDocument> departs = departments.Where(w => w["ParentCode"] == parentCode).OrderBy(o => o["Order"].AsInt32);
        //    if (departs.Count() == 0) return;
        //    foreach (BsonDocument b in departs)
        //    {
        //        DepartmentSelect deptSelect = AssembDepartment(b);
        //        GetSubDepartmentSelect(deptSelect, departments, deptSelect.DepartmentCode);
        //        temp.Add(deptSelect);
        //    }
        //    departmentSelect.Departments = temp;
        //}
        //private DepartmentSelect AssembDepartment(BsonDocument b)
        //{
        //    return new DepartmentSelect()
        //    {
        //        Id = b["_id"].ToString(),
        //        DepartmentName = b["DepartmentName"].AsString,
        //        DepartmentCode = b["DepartmentCode"].AsString,
        //        Order = b["Order"].AsInt32,
        //        Layer = b["Layer"].AsInt32,
        //        ParentCode = b["ParentCode"] == BsonNull.Value ? null : b["ParentCode"].AsString,
        //        Departments = new List<DepartmentSelect>(),
        //        CreateTime = b["CreateTime"].ToUniversalTime()
        //    };
        //}

    }
}
