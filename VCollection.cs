using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCollectionObjects
{
	public class VCollection<TKey, TValue> : VArray<KeyValuePair<TKey?, TValue?>>
	{


		public new void Add(TKey? key, TValue? value) => Add(new KeyValuePair<TKey?, TValue?>(key, value));

		private void dev()
		{
			foreach(var sel in this)
		}

	}
}
