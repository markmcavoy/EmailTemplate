using System;

namespace EmailTemplateProcessor.Entities
{
	/// <summary>
	/// Summary description for UserDataItem.
	/// </summary>
	public class UserDataItem
	{
		/// <summary>
		/// internal string, holds the name of the userDataItem
		/// </summary>
		protected string _name;
		/// <summary>
		/// internal string, holds the default value to use if no data is supplied
		/// for the userItem
		/// </summary>
		protected string _default;
		/// <summary>
		/// internal bool, used to indicate if the userdataitem is mandatory. mandatory objects
		/// must supply a value in the data or a default value
		/// </summary>
		protected bool _mandatory;

		#region Constructor
		/// <summary>
		/// default constructor
		/// </summary>
		public UserDataItem()
		{
		}

		/// <summary>
		/// constructor to init the all the standard attributes that a userDataItem will
		/// have
		/// </summary>
		/// <param name="name">string name of the userDataItem</param>
		/// <param name="defaultValue">string the default value to use if no data supplied</param>
		/// <param name="mandatory">bool is the userdataitem is mandatory</param>
		public UserDataItem(string name, string defaultValue, bool mandatory)
		{
			this._default = defaultValue;
			this._mandatory = mandatory;
			this._name = name;
		}
		#endregion

		/// <summary>
		/// Property Name, get/set the name of the userdata item
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Property DefaultValue, get/set the default value to use if no
		/// data is supplied
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return _default;
			}
			set
			{
				_default = value;
			}
		}

		/// <summary>
		/// Property Mandatory, get/set indicates if the userDataitem is mandatory
		/// </summary>
		public bool Mandatory
		{
			get
			{
				return _mandatory;
			}
			set
			{
				_mandatory = value;
			}
		}
	}
}
