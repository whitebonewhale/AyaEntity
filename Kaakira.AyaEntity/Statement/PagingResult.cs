using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyaEntity.SQLTools
{
	public class PagingResult<T>
	{
		public int Total { get; set; }
		public IEnumerable<T> Rows { get; set; }
	}
}
