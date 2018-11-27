using KiraEntity.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace KiraEntity.Base
{

	/// <summary>
	/// sql语句操作类型
	/// </summary>
	public enum SqlClause
	{
		Like,
		NotIn,
		In,
		Equal,
	}
	public enum SqlOperate
	{
		Update,
		Insert,
		Select,
		Delete
	}


	/// <summary>
	/// 表字段属性枚举
	/// </summary>
	public enum ColmunProperty
	{
		Normal,
		Identity,
		PrimaryKey,
		PrimaryIdentityKey,
	}

	/// <summary>
	/// Sql语句生成类
	/// </summary>
	public class SqlStatement
	{
		private IEnumerable<PropertyInfo> GetKeys(Type dytype, SqlOperate symbol = SqlOperate.Select)
		{
			IEnumerable<PropertyInfo> result = null;
			switch (symbol)
			{
				case SqlOperate.Select:
					result = dytype.GetProperties();
					break;
				case SqlOperate.Update:
					// 不获取主键
					result = dytype.GetProperties().Where(m => m.GetCustomAttribute<IdentityKeyAttribute>() == null);
					break;
				case SqlOperate.Insert:
					result = dytype.GetProperties().Where(m => m.GetCustomAttribute<IdentityKeyAttribute>() == null);
					break;
				case SqlOperate.Delete:
					result = dytype.GetProperties();
					break;
			}

			return result;
		}

		/// <summary>
		/// 构建获取自定义数据sql方法
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="clause"></param>
		/// <param name="cloumns"></param>
		/// <returns></returns>
		public string CustomSelect(string tableName, string clause, Type param, string columns = null)
		{
			if (string.IsNullOrEmpty(columns))
			{
				columns = "*";
			}
			if (string.IsNullOrEmpty(clause))
			{
				IEnumerable<PropertyInfo> fields = param.GetProperties();
				clause = fields.Join(" and ", m => m.Name + "=@" + m.Name);

			}
			StringBuilder sqlmem = new StringBuilder($"select {columns} from {tableName}");
			if (!string.IsNullOrEmpty(clause))
			{
				sqlmem.Append(" where ").Append(clause);
			}
			return sqlmem.ToString();
		}

		public static string Where(string key, string value, SqlClause symbol)
		{
			if (value == null)
			{
				return "";
			}
			switch (symbol)
			{
				case SqlClause.In:
					return key + " in @" + key;
				case SqlClause.Like:
					return key + " like '%' +@" + key + "'%'";
				case SqlClause.NotIn:
					return key + " not in @" + key;
				default:
				case SqlClause.Equal:
					return key + "=@" + key;
			}

		}
		public string ToSelect(string tableName, string clause)
		{
			StringBuilder sqlmem = new StringBuilder("select * from " + tableName);
			if (!string.IsNullOrEmpty(clause))
			{
				sqlmem.Append(" where ").Append(clause);
			}
			return sqlmem.ToString();
		}

		public string ToSelect(string tableName, Type dyparam)
		{
			StringBuilder sqlmem = new StringBuilder("select * from " + tableName);
			if (dyparam != null)
			{
				IEnumerable<PropertyInfo> fields = GetKeys(dyparam);
				sqlmem.Append(" where ").Append(fields.Join(" and ", m =>
				{
					if ((typeof(IEnumerable<object>).IsAssignableFrom(m.PropertyType)))
					{
						return m.Name + " in @" + m.Name;
					}
					else
					{
						return m.Name + "=@" + m.Name;
					}
				}));
			}
			return sqlmem.ToString();
		}


		public string ToDelete(string tableName, Type dyparam)
		{
			StringBuilder sqlmem = new StringBuilder("delete " + tableName);
			if (dyparam != null)
			{
				sqlmem.Append(" where ");
				IEnumerable<PropertyInfo> fields = GetKeys(dyparam, SqlOperate.Insert);
				sqlmem.Append($"{ fields.Join(" and ", m => m.Name + "=@" + m.Name)}");
			}
			return sqlmem.ToString();
		}


		public string ToInsert(string tableName, Type dyparam)
		{
			StringBuilder sqlmem = new StringBuilder("insert into " + tableName);
			IEnumerable<PropertyInfo> fields = GetKeys(dyparam, SqlOperate.Insert);
			sqlmem.Append($"({ fields.Join(",", m => m.Name)})values(@{ fields.Join(",@", m => m.Name)})");
			return sqlmem.ToString();
		}



		public string ToUpdate(string tableName, Type dyparam, params string[] id_clause)
		{
			if (dyparam == null)
				throw new ArgumentNullException("参数dyparam不能为null!");

			StringBuilder sqlmem = new StringBuilder();
			sqlmem.Append("update " + tableName);
			IEnumerable<PropertyInfo> fields = GetKeys(dyparam, SqlOperate.Update);
			var set_fi = fields.Where(m => !id_clause.Contains(m.Name));


			sqlmem.Append(" set ");
			sqlmem.Append(set_fi.Join(", ", m => m.Name + "=@" + m.Name));
			sqlmem.Append(" where ").Append(id_clause.Join(" and ", m => m + "=@" + m));
			return sqlmem.ToString();
		}

	}
}
