using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCollectionObjects
{
	public struct CollectionAccessConfiguration
	{

		public bool Enabled { get; set; }

		public bool IsReadOnly { get; set; }

		public bool IndexingEnabled { get; set; }

		public bool AddEnabled { get; set; }

		public bool RemoveEnabled { get; set; }

		public bool ReplaceEnabled { get; set; }

		public bool MoveEnabled { get; set; }

		public bool ShiftEnabled { get; set; }

		public bool CopyEnabled { get; set; }



	}
}
