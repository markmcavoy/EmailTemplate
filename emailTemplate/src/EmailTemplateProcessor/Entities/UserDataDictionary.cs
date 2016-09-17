using System;
using System.Collections;

namespace EmailTemplateProcessor.Entities
{
	/// <summary>
	/// Class used to hold the information we need to output an IDictionary
	/// object.
	/// 
	/// We used this when we are merging the data into the output text.
	/// </summary>
	public class UserDataDictionary : UserDataItem
	{
		/// <summary>
		/// holds the IDictionary, containing the data we are
		/// going to iterator over
		/// </summary>
		protected IDictionary _IDictionary;
		/// <summary>
		/// holds the ItemStart string
		/// </summary>
		protected string _itemStart;
		/// <summary>
		/// holds the ItemEnd string
		/// </summary>
		protected string _itemEnd;
		/// <summary>
		/// holds the ItemSeparator string
		/// </summary>
		protected string _itemSeparator;
		/// <summary>
		/// holda the RowSeparator string
		/// </summary>
		protected string _rowSeparator;


		#region Constructor

		/// <summary>
		/// Default constructor used to create a UserDataDictionary object
		/// </summary>
		public UserDataDictionary()
		{
		}

		/// <summary>
		/// Constructor used to create a UserDataDictionary object
		/// </summary>
		/// <param name="name">Name of the user data item</param>
		/// <param name="mandatory">Mandatory, boolean indicating if the item is mandatory in the data Hashtable</param>
		public UserDataDictionary(string name, bool mandatory) : base(name, null, mandatory)
		{
		}

		/// <summary>
		/// Constructor used to create a UserDataDictionary object
		/// </summary>
		/// <param name="name">Name of the user data item</param>
		/// <param name="mandatory">Mandatory, boolean indicating if the item is mandatory in the data Hashtable</param>
		/// <param name="dictionary"></param>
		/// <param name="itemStart">String value to output before the first item </param>
		/// <param name="itemEnd">String value to outputted after the last item is outpetted</param>
		/// <param name="itemSeparator">String value outputted between each kay value pair</param>
		/// <param name="rowSeparator">String value outputted after each value, but not the last item in 
		/// the IDictionar list</param>
		public UserDataDictionary(string name, bool mandatory, IDictionary dictionary, 
										string itemStart, string itemEnd,
										string itemSeparator, string rowSeparator): base(name, null, mandatory)
		{
			_IDictionary = dictionary;
			_itemStart = itemStart;
			_itemEnd = itemEnd;
			_itemSeparator = itemSeparator;
			_rowSeparator = rowSeparator;
		}

		#endregion

		

		/// <summary>
		/// get/set the ItemStart string which is outputted before we are
		/// iterating
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
		/// get/set the item separator, this string is output between a key
		/// and a value
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
		/// get/set ItemEnd, outputed after we have completed the iteration
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
		/// get/set string RowSeparator, this value is outputed after each iteration.
		/// 
		/// note the final iteration does not have this value after it, but will have the 
		/// ItemEnd value
		/// </summary>
		public string RowSeparator
		{
			get
			{
				return _rowSeparator;
			}
			set
			{
				_rowSeparator = value;
			}
		}

		/// <summary>
		/// get/set the IDictionaryEnumerator, this contains the key/value data
		/// that we are going to iterator over
		/// </summary>
		public IDictionary Dictionary
		{
			get
			{
				return _IDictionary;
			}
			set
			{
				_IDictionary = value;
			}
		}

	}
}
