using System;
using System.Collections;

namespace EmailTemplateProcessor.Entities
{
	/// <summary>
	/// class used to hold wrap the IEnumerable object that
	/// we are using for userDataList elements
	/// 
	/// Here we store the object and the itemStart, itemEnd and itemSeparator 
	/// values. During the merging of the data we can test for this object and
	/// output as required
	/// </summary>
	public class UserDataList : UserDataItem
	{
		
		
		/// <summary>
		/// IEnumerable value cotaining the data we are going to
		/// iterator over
		/// </summary>
		protected IEnumerable _IEnumerable;
		/// <summary>
		/// Item Start value, contains the string outputted at the start of the
		/// iteration
		/// </summary>
		protected string _itemStart;
		/// <summary>
		/// Item Separator, contains the string used to separate the values
		/// when looping thro' the iterator
		/// </summary>
		protected string _itemSeparator;
		/// <summary>
		/// Item End, contains the string outputed after the last item
		/// </summary>
		protected string _itemEnd;


		#region Constructors
		/// <summary>
		/// Default Constructor
		/// </summary>
		public UserDataList():base()
		{
			
		}

		/// <summary>
		/// Creates a userDatalist object
		/// </summary>
		/// <param name="name">string name of the userDataList</param>
		/// <param name="mandatory">bool mandatory id the userDataList is mandatory</param>
		public UserDataList(string name, bool mandatory) : base(name, null, mandatory)
		{
		}

		/// <summary>
		/// Creates a userDataList object
		/// </summary>
		/// <param name="name"></param>
		/// <param name="mandatory"></param>
		/// <param name="IEnumberable"></param>
		/// <param name="itemStart"></param>
		/// <param name="itemSeparator"></param>
		/// <param name="itemEnd"></param>
		public UserDataList(string name, bool mandatory, 
								IEnumerable IEnumberable, string itemStart, 
								string itemSeparator, string itemEnd) : base(name, null, mandatory)
		{
			this._IEnumerable = IEnumberable;
			this._itemStart = itemStart;
			this._itemSeparator = itemSeparator;
			this._itemEnd = itemEnd;
		}
		#endregion

			/// <summary>
			/// Property used to get/set the ItemStart value
			/// 
			/// This is the string that will be prefixed to the start of the enumerator output. 
			/// </summary>
			public string ItemStart
			{
				get
				{
					return _itemStart;
				}
				set
				{
					_itemStart = value;
				}
			}

			/// <summary>
			/// Property used to get/set the ItemSeparator value
			/// 
			/// This is the string that is inserted between each value when
			/// we output the enumerator.
			/// </summary>
			public string ItemSeparator
			{
				get
				{
					return _itemSeparator;
				}
				set
				{
					_itemSeparator = value;
				}
			}

			/// <summary>
			/// Property used to get/set the ItemEnd Value.
			/// 
			/// String value that is output after the final item in the 
			/// enumerator
			/// </summary>
			public string ItemEnd
			{
				get
				{
					return _itemEnd;
				}
				set
				{
					_itemEnd = value;
				}
			}

			/// <summary>
			/// Property to get/set the IEnumerable object that holds the values that
			/// we are going to use to enumerator over
			/// </summary>
			public IEnumerable IEnumerable
			{
				get
				{
					return _IEnumerable;
				}
				set
				{
					_IEnumerable = value;
				}
			}
		

	}
}
