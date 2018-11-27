using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiraEntity
{
    public class Pagination
    {
        public string TableName { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int StartRow { get { return (PageIndex - 1) * PageSize; } }
        public int TotalCount { get; set; }
        public int TotalPage => (int)Math.Ceiling(TotalCount * 1.00 / PageSize);
        public bool HasParameter => _dyparam.ParameterNames.Count() > 0;
        public DynamicParameters DyParameters => _dyparam;
        /// <summary>
        /// 此字段存在注入风险，推荐在后台"硬编码"赋值
        /// </summary>
        public string OrderField { get; set; }
        private string _orderType;

        private DynamicParameters _dyparam = new DynamicParameters();
        public DynamicParameters PageDyParameters
        {
            get
            {
                DynamicParameters pd = new DynamicParameters(_dyparam);
                pd.Add("@StartRow", StartRow);
                pd.Add("@PageSize", PageSize);
                pd.Add("@OrderField", OrderField);
                return pd;
            }
        }
        public string OrderType
        {
            get { return _orderType; }
            set
            {
                if (!(value.Equals("asc", StringComparison.CurrentCultureIgnoreCase) || value.Equals("desc", StringComparison.CurrentCultureIgnoreCase)))
                    throw new ArgumentException("sortType被注入，" + value + "不符合规范：asc、desc");
                _orderType = value;
            }
        }
        public Pagination(string field, string type)
        {
            this.PageIndex = 1;
            this.PageSize = 10;
            this.OrderField = field;
            this.OrderType = type;
        }

        public Pagination(int pi, int ps, string field, string type)
        {
            this.OrderType = type;
            this.OrderField = field;
            this.PageSize = ps;
            this.PageIndex = pi < 1 ? 1 : pi;
        }

        public Pagination()
        {
            this.PageIndex = 1;
            this.PageSize = 10;
        }

        public void AddParameter(string key, object value)
        {
            _dyparam.Add(key, value);
        }
       
      


    }

}
