/*
 * Copyright (c) 2006, bitethebullet.co.uk
   All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, 
      this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, 
      this list of conditions and the following disclaimer in the documentation
      and/or other materials provided with the distribution.
    * Neither the name of the bitethebullet.co.uk nor the names of its contributors
      may be used to endorse or promote products derived from this software
      without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
POSSIBILITY OF SUCH DAMAGE.
 * */

using System;
using System.Collections;
using System.Data.Common;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;

using log4net;

using EmailTemplateProcessor.Entities;

namespace EmailTemplateProcessor
{
	/// <summary>
	/// enumerator of mail types
	/// </summary>
	public enum MailFormatType
	{
		/// <summary>
		/// Plain Text email format
		/// </summary>
		PlainText,
		/// <summary>
		/// Html Text email format
		/// </summary>
		HtmlText
	}

	/// <summary>
	/// Email priority enumerator
	/// </summary>
	public enum MailPriorityType
	{
		/// <summary>
		/// Normal
		/// </summary>
		Normal,
		/// <summary>
		/// High
		/// </summary>
		High,
		/// <summary>
		/// Low
		/// </summary>
		Low
	}
	/// <summary>
	/// Models an email message. Based on the template based to constructor
	/// 
	/// Allows template to be modified and loaded with data before using the EmailTemplateProcessor
	/// to send the email/emails
	/// 
	/// Able to send the same email message to multiple recipentants.
	/// </summary>
	public class EmailTemplate
    {

        #region vars

        /// <summary>
		/// string to hold the path to the mailtemplate folder
		/// </summary>
		protected string _path;
		/// <summary>
		/// string, filename of the mailtemplate
		/// </summary>
		protected string _fileName;
		/// <summary>
		/// string array to email adderss
		/// </summary>
		protected string[] _to;
		/// <summary>
		/// string cc email address
		/// </summary>
		protected string _cc;
		/// <summary>
		/// string bcc email address
		/// </summary>
		protected string _bcc;
		/// <summary>
		/// enumerator of the mail priority
		/// </summary>
		protected MailPriorityType _mailPriority;
		/// <summary>
		/// string from email address
		/// </summary>
		protected string _from;
		/// <summary>
		/// string from email address
		/// </summary>
        protected string _replyTo;
        /// <summary>
        /// string replyto email address
        /// </summary>
        protected string _subject;
		/// <summary>
		/// string email body, this will contain the raw XML
		/// </summary>
		protected string _body;
		/// <summary>
		/// string array of full filenames to attachments, this
		/// will contain the raw XML
		/// </summary>
		protected string[] _attachments;
		/// <summary>
		/// enum of mailformatype
		/// </summary>
		protected MailFormatType _mailFormatType;
		/// <summary>
		/// string template name
		/// </summary>
		protected string _templateName;
		/// <summary>
		/// string template description
		/// </summary>
		protected string _templateDescription;
		/// <summary>
		/// string template application value
		/// </summary>
		protected string _templateApplication;
		/// <summary>
		/// hashtable of internal data
		/// </summary>
		protected Hashtable _data = null;
		/// <summary>
		/// string containing body element with data bound
		/// </summary>
		protected string _bodyOutput = null;
		/// <summary>
		/// string containing subject element with data bound
		/// </summary>
		protected string _subjectOutput = null;

		private bool _useNameSpaces = false;

		/// <summary>
		/// Const the URI namespace of the template elements
		/// this is the value we use to mark the emailtemplate elements with
		/// </summary>
		protected const string TEMPLATE_NAMESPACE_URI = "http://bitethebullet.co.uk/EmailProcessor/Template";

		/// <summary>
		/// Const the URI namespace of the userdata elements that are provided in the
		/// email template file
		/// </summary>
		protected const string USERVALUE_NAMESPACE_URI = "http://www.bitethebullet.co.uk/EmailProcessor/UserData";


        /// <summary>
        /// Log4net logger used for logging
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(typeof(EmailTemplate));

        #endregion 

        #region enum

        /// <summary>
		/// Enum used to define which of the field
		/// we are using 
		/// </summary>
		private enum ElementType
		{
			Subject,
			Body
		}

		/// <summary>
		/// enum used to define the userData element type
		/// </summary>
		private enum UserDataType
		{
			Item,
			List,
			Dictionary
		}

		#endregion


        #region cstor

        /// <summary>
		/// constructor to create an Email Template
		/// </summary>
		/// <param name="path">path to the folder containing the template files</param>
		/// <param name="fileName">filename of template to use</param>
		public EmailTemplate(string path, string fileName)
		{
			this._path = path;
			this._fileName = fileName;
			InitTemplate(path, fileName);
		}

		/// <summary>
		/// constructor to create an Email Template
		/// </summary>
		/// <param name="fullyQuailifiedFileName">fully quailified path to the template to use</param>
		public EmailTemplate(string fullyQuailifiedFileName)
		{
			int pos = fullyQuailifiedFileName.LastIndexOf("\\");
			this._path = fullyQuailifiedFileName.Substring(0, pos);
			this._fileName = fullyQuailifiedFileName.Substring(pos+1);
			InitTemplate(this._path, this._fileName);
        }

        #endregion



        #region properties

        /// <summary>
        /// Get/set the string array of To addresses. This allows
        /// the email to be sent to more than one recipient. If you only
        /// require sending the email to a single recipient use the <see cref="ToSingleAddress">
        /// ToSingleAddress</see> property
        /// </summary>
        public string[] To
        {
            get
            {
                return this._to;
            }
            set
            {
                this._to = value;
            }
        }

        /// <summary>
        /// Get/set the address where the email should be sent. Using this property means
        /// the email will only be sent to a single recipient.
        /// </summary>
        public string ToSingleAddress
        {
            get { return this._to[0]; }
            set
            {
                string[] to = new string[1];
                to[0] = value;
                _to = to;
            }
        }

        /// <summary>
        /// get/set the From email address, string
        /// </summary>
        public string From
        {
            get
            {
                return this._from;
            }
            set
            {
                this._from = value;
            }
        }

        /// <summary>
        /// get/set the ReplyTo email address, string
        /// </summary>
        public string ReplyTo
        {
            get
            {
                return this._replyTo;
            }
            set
            {
                this._replyTo = value;
            }
        }

        /// <summary>
        /// get/set the Email Subject, string
        /// </summary>
        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                this._subject = value;

                //reload the data in case we have to swap out userData items
                LoadSubjectData();
            }
        }

        /// <summary>
        /// get/set the Email Body
        /// 
        /// this will contain any userData elements that are contained in the email template
        /// this propery will never have the data values in the email
        /// </summary>
        public string Body
        {
            get
            {
                return _body;
            }
            set
            {
                this._body = this.FormatWhitespace(value);

                LoadBodyData();
            }
        }

        /// <summary>
        /// get/set string array of attachments. Each string will
        /// be the absolute path the attachment. No relative paths are
        /// allowed using this property
        /// </summary>
        public string[] Attachments
        {
            get
            {
                return _attachments;
            }
            set
            {
                _attachments = value;
            }
        }

        /// <summary>
        /// get/set MailFormat, see the enum MailFormatType
        /// value can be HTML or plain text
        /// </summary>
        public MailFormatType MailFormat
        {
            get
            {
                return _mailFormatType;
            }
            set
            {
                _mailFormatType = value;
            }
        }

        /// <summary>
        /// get read-only template name as stored in the template file
        /// </summary>
        public string TemplateName
        {
            get
            {
                return _templateName;
            }
        }

        /// <summary>
        /// get read-only template description as stored in the template file
        /// </summary>
        public string TemplateDescription
        {
            get
            {
                return _templateDescription;
            }
        }

        /// <summary>
        /// get read-only application attribute value as stored in the template file
        /// </summary>
        public string Application
        {
            get
            {
                return _templateApplication;
            }
        }

        /// <summary>
        /// get read-only fileName of template
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
        }

        /// <summary>
        /// get read-only path to template directory
        /// </summary>
        public string Path
        {
            get
            {
                return _path;
            }
        }

        /// <summary>
        /// get/set CC email address
        /// </summary>
        public string Cc
        {
            get
            {
                return _cc;
            }
            set
            {
                _cc = value;
            }
        }

        /// <summary>
        /// get/set BCC email address
        /// </summary>
        public string Bcc
        {
            get
            {
                return _bcc;
            }
            set
            {
                _bcc = value;
            }
        }

        /// <summary>
        /// get/set the Mail Priority
        /// 
        /// if no value is given in the template then the 
        /// default value is "Normal"
        /// </summary>
        public MailPriorityType MailPriority
        {
            get
            {
                return _mailPriority;
            }
            set
            {
                _mailPriority = value;
            }
        }

        #endregion


        private void InitTemplate(string path, string fileName)
		{
            if(!File.Exists(path + "\\" + fileName))			    
				throw new EmailTemplateException("Template file does not exist: " + fileName);

            FileInfo templateFile = new FileInfo(path + "\\" + fileName);

			//default the mail priority to normal
			_mailPriority = MailPriorityType.Normal;
			
			//from version 1.2+ all the template elements are now in a namespace
			//we need to check for these but also allow older templates to work ok
			//
			//a new attribute is now set in the emailTemplate element called version
			//this defines the version that the template was created to work with
			//if this is missing we assume that the template is 1.1 and hence doesn't
			//have an namespaces

			double fileVersion = ReadTemplateFileVersion(path + "\\" + fileName);
			
			ArrayList attachments = new ArrayList();

            using (StreamReader stream = new StreamReader(path + "\\" + fileName)) 
            { 
                using(XmlTextReader xmlReader = new XmlTextReader(stream))
                {
                    //read the verison of files
                    if (fileVersion == 1.1)
                    {
                        ReadFileVersion1_1(xmlReader, attachments);
                    }
                    else if ((fileVersion == 1.2) || (fileVersion == 2.0))
                    {
                        ReadFileVerson1_2(xmlReader, attachments);
                    }
                    else
                        throw new EmailTemplateException("Template file version is invalid");
                }
            }

			//check each attachment resolves to a file that exists
            //this now supports absolute or relative
            for (int i = 0; i < attachments.Count; i++)
            {
                //attempt to resolve the attachment value
                string attachmentFileName = attachments[i].ToString();

                //determine if its a fully qualified filepath
                if (!File.Exists(attachmentFileName))
                {
                    Log.DebugFormat("File: {0} is not absolute, attempting to find relative to the template",
                                    attachmentFileName);

                    //check if we need to get this relative path combined with the 
                    //base path based on the template
                    attachmentFileName = System.IO.Path.Combine(_path,
                                                                attachmentFileName);
                    //resolve path
                    attachmentFileName = System.IO.Path.GetFullPath(attachmentFileName);

                    //determine if we can locate this relative file
                    if (File.Exists(attachmentFileName))
                    {
                        Log.DebugFormat("Found attachment: {0}", attachmentFileName);
                        attachments[i] = attachmentFileName;
                    }
                    else
                    {
                        Log.ErrorFormat("Unable to find attachment: {0}", attachmentFileName);
                        throw new EmailTemplateException("Attachment file not found: " + attachments[i].ToString());
                    }
                }                
            }

			this._attachments = (string[])attachments.ToArray(typeof(String));
			
		}

        //private void

		/// <summary>
		/// Method to read data stored in a 1.0 or 1.1 version email template
		/// file and load the data into properties in this object
		/// </summary>
		/// <param name="xmlReader">Opened XmlReader of the email template to load</param>
		/// <param name="attachments">Arraylist used to as temp store attachments</param>
		protected void ReadFileVersion1_1(XmlReader xmlReader, ArrayList attachments)
		{
			//read the xml file and load the data into the object
			while(xmlReader.Read())
			{
				if(xmlReader.NodeType==XmlNodeType.Element)
				{
					switch(xmlReader.LocalName)
					{
						case "emailTemplate":
							_templateApplication = xmlReader.GetAttribute("application");
							break;

						case "name":
							_templateName = xmlReader.ReadString();
							break;

						case "description":
							_templateDescription = xmlReader.ReadString();
							break;

						case "mailFormat":
							string mailTypeValue = xmlReader.ReadString();
							if(mailTypeValue=="PlainText")
								_mailFormatType = MailFormatType.PlainText;
							else
								_mailFormatType = MailFormatType.HtmlText;

							break;

						case "to":
							string[] to = new string[1];
							to[0] = xmlReader.ReadString();
							this._to = to;
							break;

						case "from":
							this._from =xmlReader.ReadString();
							break;

                        case "replyTo":
                            this._replyTo = xmlReader.ReadString();
                            break;

                        case "subject":
							this._subject = xmlReader.ReadInnerXml();
							break;

						case "body":
							_body = FormatWhitespace(xmlReader.ReadInnerXml());
							break;

						case "attachment":
							attachments.Add(xmlReader.ReadString());
							break;

						case "cc":
							this.Cc = xmlReader.ReadString();
							break;

						case "bcc":
							this.Bcc = xmlReader.ReadString();
							break;

						case "priority":
							string priority = xmlReader.ReadString().ToUpper();
							if(priority=="HIGH")
								this.MailPriority = MailPriorityType.High;
							else if(priority=="NORMAL")
								this.MailPriority = MailPriorityType.Normal;
							else if(priority=="LOW")
								this.MailPriority = MailPriorityType.Low;

							break;
					}
				}
			}
		}

		/// <summary>
		/// Method to read data stored in a 1.2 version email template
		/// file and load the data into properties in this object
		/// </summary>
		/// <param name="xmlReader">Opened XmlReader of the email template to load</param>
		/// <param name="attachments">Arraylist used to as temp store attachments</param>
		protected void ReadFileVerson1_2(XmlReader xmlReader, ArrayList attachments)
		{
			//read the xml file and load the data into the object
			while(xmlReader.Read())
			{
				if(xmlReader.NodeType==XmlNodeType.Element && xmlReader.NamespaceURI==TEMPLATE_NAMESPACE_URI)
				{
					switch(xmlReader.LocalName)
					{
						case "emailTemplate":
							_templateApplication = xmlReader.GetAttribute("application");
							break;

						case "name":
							_templateName = xmlReader.ReadString();
							break;

						case "description":
							_templateDescription = xmlReader.ReadString();
							break;

						case "mailFormat":
							string mailTypeValue = xmlReader.ReadString();
							if(mailTypeValue=="PlainText")
								_mailFormatType = MailFormatType.PlainText;
							else
								_mailFormatType = MailFormatType.HtmlText;

							break;

						case "to":
							string[] to = new string[1];
							to[0] = xmlReader.ReadString();
							this._to = to;
							break;

						case "from":
							this._from = xmlReader.ReadString();
							break;

                        case "replyTo":
                            this._replyTo = xmlReader.ReadString();
                            break;

                        case "subject":
							this._subject = xmlReader.ReadInnerXml();
							break;

						case "body":
							_body = FormatWhitespace(xmlReader.ReadInnerXml());
							break;

						case "attachment":
							attachments.Add(xmlReader.ReadString());
							break;

						case "cc":
							this.Cc = xmlReader.ReadString();
							break;

						case "bcc":
							this.Bcc = xmlReader.ReadString();
							break;

						case "priority":
							string priority = xmlReader.ReadString().ToUpper();
							if(priority=="HIGH")
								this.MailPriority = MailPriorityType.High;
							else if(priority=="NORMAL")
								this.MailPriority = MailPriorityType.Normal;
							else if(priority=="LOW")
								this.MailPriority = MailPriorityType.Low;

							break;
					}
				}
			}
		}

		/// <summary>
		/// Reads the version number of the email template. This will allow us to 
		/// load files that are from an older version of the application
		/// </summary>
		/// <param name="fullFilePath">full path and filename of the email template</param>
		/// <returns>Version number of the application that the email template was created for</returns>
		protected double ReadTemplateFileVersion(string fullFilePath)
		{
			StreamReader stream = new StreamReader(fullFilePath);
			XmlTextReader xmlReader = new XmlTextReader(stream);
			ArrayList attachments = new ArrayList();

			//read the xml file using localnames to we find the "emailTemplate" element
			while(xmlReader.Read())
			{
				if(xmlReader.NodeType == XmlNodeType.Element)
				{
					if(xmlReader.LocalName == "emailTemplate")
					{
						//read the attribute "version" value and convert to
						//double
						if(!xmlReader.HasAttributes)
						{
							_useNameSpaces = false;
							return 1.1;
						}

						xmlReader.MoveToAttribute("version");

						while(xmlReader.ReadAttributeValue())
						{
							_useNameSpaces = true;
							return Double.Parse(xmlReader.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);														
						}

						//if this attribute is missing we have a version 1.0 - 1.1
						//so return 1.1
						_useNameSpaces = false;
						return 1.1;
					}
				}
			}
			return 1.1;
		}


		/// <summary>
		/// Method to format whitespace characters such as \t \r and \n
		/// these can be in the email template and we need to replace them
		/// with the correct ascii values
		/// </summary>
		/// <param name="input">String value we are going to check for whitespace escape characters</param>
		/// <returns>String with whitespace formatting as defined by the markup</returns>
		protected string FormatWhitespace(string input)
		{
			if(input==null)
				return input;

			//do \r
			System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(@"\\r");
			input = rg.Replace(input, Convert.ToChar(13).ToString());

			//do \n
			rg = new System.Text.RegularExpressions.Regex(@"\\n");
			input = rg.Replace(input, Convert.ToChar(10).ToString());

			//do \t
			rg = new System.Text.RegularExpressions.Regex(@"\\t");
			input = rg.Replace(input, Convert.ToChar(9).ToString());

			return input;

        }

        /// <summary>
		/// method to return the body of the email with the data 
		/// added
		/// </summary>
		/// <returns>string contain a Preview of how the body of the email will look</returns>
		public virtual string PreviewBody()
		{
			/* check we have got a preview string
			*  this can happen if the template does not cotain
			* any userData users and the method LoadData does not
			* get called
			*/
			if(_bodyOutput==null)
			{
				this.LoadData(null);
			}

			return _bodyOutput;
		}

		/// <summary>
		/// method to return the subject line with the data added
		/// </summary>
		/// <returns>string containing the Preview of the subject line</returns>
		public virtual string PreviewSubjectLine()
		{
			if(_subjectOutput==null)
			{
				this.LoadData(null);
			}

			return _subjectOutput;
		}

		/// <summary>
		/// Loads a hashtable into the email template. The key from the hashtable will be used to
		/// map to any matching userData elements with a matching name. 
		/// </summary>
		/// <param name="data"></param>
		public void LoadData(Hashtable data)
		{
			if(_data == null ||data != null)
				_data = data;

			LoadBodyData();
			LoadSubjectData();
		}

		/// <summary>
		/// Load the subject line with any userdata values
		/// </summary>
		protected void LoadSubjectData()
		{
			//parse the subjectline and create the output
			string temp = "<subject>" + _subject + "</subject>";
			Hashtable values = ParseElement(temp);
			_subjectOutput = this.MergeData(values, ElementType.Subject);
		}

		/// <summary>
		/// Load the body with any userdata values
		/// </summary>
		protected void LoadBodyData()
		{
			if(_body==null)
				return;

			//parses the body and create the output
			string temp = "<body>" + _body + "</body>";
			//parse all the data for importing into the body field
			Hashtable values = ParseElement(temp);
			_bodyOutput = this.MergeData(values, ElementType.Body);
		}


		/// <summary>
		/// Parse the element snippet and load a Hashtable with the data for the user defined data
		/// elements.
		/// </summary>
		/// <param name="element">String element we are going to parse</param>
		/// <returns></returns>
		private Hashtable ParseElement(string element)
		{
			string name;
			string defaultValue;
			string mandatory;
			string listItemStart;
			string listItemEnd;
			string listItemSeparator;
			string listRowSeparator;
			object userItem;

			Queue queue = new Queue();
			Hashtable values = new Hashtable();
			XmlTextReader reader = new XmlTextReader(element, XmlNodeType.Element, null);

			if(_data==null)
				_data = new Hashtable();

			while(!reader.EOF)
			{
				reader.Read();

				if(reader.NodeType == XmlNodeType.Element)
				{
					if(_useNameSpaces && reader.NamespaceURI != USERVALUE_NAMESPACE_URI)
					{
						continue;
					}

					switch(reader.LocalName)
					{
						case "userData":
		
							this.ClearQueue(values, queue);
							name = reader.GetAttribute("name", "");
							defaultValue = reader.GetAttribute("default", "");
							mandatory = reader.GetAttribute("mandatory", "");

                            if(name == null)
                                throw new EmailTemplateException("Missing mandatory UserData name attribute");


							//add the item to list of userData fields to merge
							if(!values.ContainsKey(name))
							{
							    values.Add(name, this.CreateUserDataItem(name, defaultValue, mandatory, 
									EmailTemplate.UserDataType.Item));
                            }

							break;	

						case "userDataList":
							this.ClearQueue(values, queue);
							name = reader.GetAttribute("name", "");
							mandatory = reader.GetAttribute("mandatory", "");

                            if (name == null)
                                throw new EmailTemplateException("Missing mandatory UserDataList name attribute");

							UserDataList list = (UserDataList)this.CreateUserDataItem(name,
																						null,
																						mandatory,
																						EmailTemplate.UserDataType.List);

							if(reader.IsEmptyElement)
							{
								listItemStart = reader.GetAttribute("itemStart", "");
								listItemEnd = reader.GetAttribute("itemEnd", "");
								listItemSeparator = reader.GetAttribute("itemSeparator", "");

								//we have all the atributes we need create the list and
								//add it to the arraylist
								
								list.ItemStart = listItemStart;
								list.ItemEnd = listItemEnd;
								list.ItemSeparator = listItemSeparator;
								if(_data.ContainsKey(name))
								{
									if(_data[name] is IEnumerable)
									{
                                        if (!values.ContainsKey(name))
                                        {
                                            list.IEnumerable = (IEnumerable)_data[name];
                                            values.Add(name, list);
                                        }
									}
									else
									{
										throw new EmailTemplateException("IDictionary interface not supported on the data item from " + name);
									}
								}
								else if(list.Mandatory)
								{
									throw new EmailTemplateException("Mandatory userDataList missing data, name: " + name);
								}
							}
							else
							{
								//need to read the elements and add them to the object later
								queue.Enqueue(list);
							}
							break;

						case "userDataDictionary":
							this.ClearQueue(values, queue);
							name = reader.GetAttribute("name", "");
							mandatory = reader.GetAttribute("mandatory", "");

                            if (name == null)
                                throw new EmailTemplateException("Missing mandatory UserDataDictionary name attribute");

							UserDataDictionary dict = (UserDataDictionary)this.CreateUserDataItem(name, null, mandatory,
																									EmailTemplate.UserDataType.Dictionary);

							if(reader.IsEmptyElement)
							{
								listItemStart = reader.GetAttribute("itemStart", "");
								listItemEnd = reader.GetAttribute("itemEnd", "");
								listItemSeparator = reader.GetAttribute("itemSeparator", "");
								listRowSeparator = reader.GetAttribute("rowSeparator", "");

								dict.ItemStart = listItemStart;
								dict.ItemEnd = listItemEnd;
								dict.ItemSeparator = listItemSeparator;
								dict.RowSeparator = listRowSeparator;

								if(_data.ContainsKey(name))
								{
									if(_data[name] is IDictionary)
									{
                                        if (!values.ContainsKey(name))
                                        {
                                            dict.Dictionary = (IDictionary)_data[name];
                                            values.Add(name, dict);
                                        }
									}
									else
									{
										throw new EmailTemplateException("userDataDictionary data item does not support IDictionary");
									}

								}
								else if(dict.Mandatory)
								{
									throw new EmailTemplateException("Mandatory userDataDictionary missing data, name: " + name);
								}
							}
							else
							{
								//need to read the elements and add them to the object
								queue.Enqueue(dict);

							}
							break;

						case "itemStart":
							//get the first item from the queue and updates the correct property
							userItem = queue.Peek();
							if(userItem is UserDataDictionary)
							{
								UserDataDictionary userDataDictionary = (UserDataDictionary)userItem;
								userDataDictionary.ItemStart = reader.ReadInnerXml();
							}
							else
							{
								//list
								UserDataList userDataList = (UserDataList)userItem;
								userDataList.ItemStart = reader.ReadInnerXml();
							}
							break;

						case "itemEnd":
							//get the first item from the queue and updates the correct property
							userItem = queue.Peek();
							if(userItem is UserDataDictionary)
							{
								UserDataDictionary userDataDictionary = (UserDataDictionary)userItem;
								userDataDictionary.ItemEnd = reader.ReadInnerXml();
							}
							else
							{
								//list
								UserDataList userDataList = (UserDataList)userItem;
								userDataList.ItemEnd = reader.ReadInnerXml();
							}
							break;

						case "itemSeparator":
							//get the first item from the queue and updates the correct property
							userItem = queue.Peek();
							if(userItem is UserDataDictionary)
							{
								UserDataDictionary userDataDictionary = (UserDataDictionary)userItem;
								userDataDictionary.ItemSeparator = reader.ReadInnerXml();
							}
							else
							{
								//list
								UserDataList userDataList = (UserDataList)userItem;
								userDataList.ItemSeparator = reader.ReadInnerXml();
							}
							break;

						case "rowSeparator":
							//get the first item from the queue and updates the correct property
							userItem = queue.Peek();
							if(userItem is UserDataDictionary)
							{
								UserDataDictionary userDataDictionary = (UserDataDictionary)userItem;
								userDataDictionary.RowSeparator = reader.ReadInnerXml();
							}
							break;
					}
				}				
			}

			ClearQueue(values, queue);
			return values;
			#region old code

			/*string name;
			string mandatoryValue;
			bool mandatory;
			string defaultValue;
			string listItemStart;
			string listItemEnd;
			string listItemSeparator;
			string listRowSeparator;

			Hashtable values = new Hashtable();

			//set the value for the member var
			if(data==null)
				_data = new Hashtable();
			else
				_data = data;


			//set the xpath query objects
			XmlTextReader reader = new XmlTextReader(element, XmlNodeType.Element, null);
			XPathDocument xpDoc = new XPathDocument(reader);
			XPathNavigator nav = xpDoc.CreateNavigator();
			XPathExpression express;

			//setup the namespace
			XmlNamespaceManager context = new XmlNamespaceManager(nav.NameTable);
			context.AddNamespace("user", USERVALUE_NAMESPACE_URI);			

			#region Select userData node

			//select all the userData elements in the email template
						if(_useNameSpaces)
						{
							//newer files have namesapces on elements
							express = nav.Compile("//user:userData");
							express.SetContext(context);
						}
						else
						{
							//older files don't use namespaces
							express = nav.Compile("//userData");
						}
						

						XPathNodeIterator nodes = nav.Select(express);

			while(nodes.MoveNext())
			{
				//reset the vars
				name = null;
				mandatory = false;
				defaultValue = null;
				mandatoryValue = null;

				defaultValue = nodes.Current.GetAttribute("default", "");
				name = nodes.Current.GetAttribute("name", "");
				mandatoryValue = nodes.Current.GetAttribute("mandatory", "");

				if(mandatoryValue!=string.Empty)
					mandatory = Boolean.Parse(mandatoryValue.ToString());

				//all elements must have a name, so check we this
				if(name==string.Empty)
					throw new EmailTemplateException("userData element must have an attribute name");

				if(!_data.ContainsKey(name))
				{
					if(mandatory && defaultValue==string.Empty)
						throw new EmailTemplateException("userData is mandatory and no data or default value supplied: " + name);

					if(defaultValue!=string.Empty)
					{
						//insert default value into the text
						values.Add(name, defaultValue);
					}
					else
					{
						//simply remove element and replace with nothing
					}
				}
				else
				{
					values.Add(name, _data[name].ToString());
				}


			}
			#endregion

			#region Select userDataList node

			if(_useNameSpaces)
			{
				express = nav.Compile("//user:userDataList");
				express.SetContext(context);
			}
			else
			{
				express = nav.Compile("//userDataList");
			}

				nodes = nav.Select(express);

			while(nodes.MoveNext())
			{
				//reset local vars
				name = null;
				mandatoryValue = null;
				mandatory = false;
				defaultValue = null;

				name = nodes.Current.GetAttribute("name", "");
				mandatoryValue = nodes.Current.GetAttribute("mandatory", "");
				defaultValue = nodes.Current.GetAttribute("default", "");

				if(mandatoryValue!=string.Empty)
					mandatory = Boolean.Parse(mandatoryValue.ToString());

				//all elements must have a name, so check we this
				if(name==string.Empty)
					throw new EmailTemplateException("userData element must have an attribute name");

				if(data==null && mandatory)
				{
					throw new EmailTemplateException("userListData item is not supplied and marked as mandatory: " + name);
				}
				else if(!_data.ContainsKey(name))
				{
					if(mandatory && defaultValue==string.Empty)
						throw new EmailTemplateException("userData is mandatory and no data or default value supplied: " + name);
				}
				else
				{
					//read the other attributes or elements for this element
					listItemStart = string.Empty;
					listItemEnd = string.Empty;
					listItemSeparator = string.Empty;

					if(nodes.Current.HasChildren)
					{
						nodes.Current.MoveToFirstChild(); 					
						listItemStart = nodes.Current.Value;
						nodes.Current.MoveToNext();
						listItemEnd = nodes.Current.Value;
						nodes.Current.MoveToNext();
						listItemSeparator = nodes.Current.Value;
					}
					else
					{
						listItemStart = nodes.Current.GetAttribute("itemStart", "");
						listItemEnd = nodes.Current.GetAttribute("itemEnd", "");
						listItemSeparator = nodes.Current.GetAttribute("itemSeparator", "");
					}

					//test that the item in the hashtable does support the 
					//IEnumerable interface
					object list = data[name];

					if(list is IEnumerable)
					{
						EmailTemplate.UserList userDataList = new UserList((IEnumerable)data[name], listItemStart, listItemSeparator, listItemEnd);
						values.Add(name, userDataList);
					}
					else
						throw new EmailTemplateException("The UserDataList data object does not support the IEnumerable interface, Name: " + name);

				}
			

			}
			#endregion

			#region Select userDataDictionary

			if(_useNameSpaces)
			{
				express = nav.Compile("//user:userDataDictionary");
				express.SetContext(context);
			}
			else
			{
				express = nav.Compile("//userDataDictionary");
			}

			nodes = nav.Select(express);

			while(nodes.MoveNext())
			{
				//reset local vars
				name = null;
				mandatoryValue = null;
				mandatory = false;
				defaultValue = null;

				name = nodes.Current.GetAttribute("name", "");
				mandatoryValue = nodes.Current.GetAttribute("mandatory", "");
				defaultValue = nodes.Current.GetAttribute("default", "");

				if(mandatoryValue!=string.Empty)
					mandatory = Boolean.Parse(mandatoryValue.ToString());

				//all elements must have a name, so check we this
				if(name==string.Empty)
					throw new EmailTemplateException("userDataDictionary element must have an attribute name");

				if(data==null && mandatory)
				{
					throw new EmailTemplateException("userDataDictionary item is not supplied and marked as mandatory: " + name);
				}
				else if(!_data.ContainsKey(name))
				{
					if(mandatory && defaultValue==string.Empty)
						throw new EmailTemplateException("userDataDictionary is mandatory and no data or default value supplied: " + name);
				}
				else
				{
					//read the other attributes or elements for this element
					listItemStart = string.Empty;
					listItemEnd = string.Empty;
					listItemSeparator = string.Empty;
					listRowSeparator = string.Empty;

					if(nodes.Current.HasChildren)
					{
						nodes.Current.MoveToFirstChild(); 					
						listItemStart = nodes.Current.Value;
						nodes.Current.MoveToNext();
						listItemEnd = nodes.Current.Value;
						nodes.Current.MoveToNext();
						listItemSeparator = nodes.Current.Value;
						nodes.Current.MoveToNext();
						listRowSeparator = nodes.Current.Value; //nodes.Current.SelectChildren("rowSeparator", USERVALUE_NAMESPACE_URI).Current.Value;

						//debug output userDataList config settings
						Console.Out.WriteLine("ItemStart: {0}", listItemStart);
						Console.Out.WriteLine("ItemEnd: {0}", listItemEnd);
						Console.Out.WriteLine("ItemSeparator: {0}", listItemSeparator);
						Console.Out.WriteLine("RowSeparator: {0}", listRowSeparator);
					}
					else
					{
						listItemStart = nodes.Current.GetAttribute("itemStart", "");
						listItemEnd = nodes.Current.GetAttribute("itemEnd", "");
						listItemSeparator = nodes.Current.GetAttribute("itemSeparator", "");
						listRowSeparator = nodes.Current.GetAttribute("rowSeparator", "");
					}

					//test we have an object for this element that is an IDictionary
					object dict = data[name];

					if(dict is IDictionary)
					{
						UserDictionary userDict = new UserDictionary((IDictionary)data[name],
							listItemStart, listItemEnd, listItemSeparator, listRowSeparator);
						values.Add(name, userDict);
					}
					else
						throw new EmailTemplateException("UserDataDictionary item does not support an IDictionaryEnumerator interface name: "  + name);
							
				}
			}

			#endregion
			

			return values;*/
			#endregion
		}

		private void ClearQueue(Hashtable table, Queue runningItems)
		{
			if(runningItems.Count > 0)
			{
				//get the item from the queue work out the type and add 
				//the data item
				object queuedItem = runningItems.Dequeue();

				if(queuedItem is UserDataDictionary)
				{
					UserDataDictionary dict = (UserDataDictionary)queuedItem;

					//check if we have the data item
					if(_data.ContainsKey(dict.Name))
					{
						if(_data[dict.Name] is IDictionary)
						{
                            dict.Dictionary = (IDictionary)_data[dict.Name];

                            //only add a dictinary with the same id once, other
                            //wise we'll throw an error
                            if (!table.ContainsKey(dict.Name))
                            {                                
                                table.Add(dict.Name, dict);
                            }
						}
						else
						{
							throw new EmailTemplateException("userDataDictionary data item does not support IDictionary");
						}
					}
					else if(dict.Mandatory)
					{	
						throw new EmailTemplateException("Mandatory userDataDictionary missing data, name: " + dict.Name);
					}
				}
				else
				{
					UserDataList list = (UserDataList)queuedItem;

					//check if we have the data item
					if(_data.ContainsKey(list.Name))
					{
						if(_data[list.Name] is IEnumerable)
						{
                            list.IEnumerable = (IEnumerable)_data[list.Name];

                            //only add a list with the same id once, other
                            //wise we'll throw an error
                            if (!table.ContainsKey(list.Name))
                            {
                                table.Add(list.Name, list);
                            }
							
							
						}
						else
						{
							throw new EmailTemplateException("IEnumerable interface not supported on the data item from " + list.Name);
						}
					}
					else if(list.Mandatory)
					{	
						throw new EmailTemplateException("Mandatory UserDataList missing data, name: " + list.Name);
					}
				}
			}
		}

		private UserDataItem CreateUserDataItem(string name, string defaultValue, string mandatory, UserDataType type)
		{
			bool bMandatory = false;

			//check name ok
			if(name == string.Empty || name == null)
			{
				throw new EmailTemplateException("userData Item must supplier a name attribute");
			}

			if(mandatory != string.Empty && mandatory != null)
				bMandatory = bool.Parse(mandatory);

			if(!_data.ContainsKey(name))
			{
				//is this item marked mandatory
				if(bMandatory)
					throw new EmailTemplateException("Mandatory data item not supplied for item: " + name);
			}

			if(type == UserDataType.Dictionary)
			{
				//dictionary
				UserDataDictionary dictionary = new UserDataDictionary(name, bMandatory);
				return dictionary;
			}
			else if(type == UserDataType.Item)
			{
				//item
				UserDataItem item = new UserDataItem(name, defaultValue, bMandatory);
				return item;
			}
			else
			{
				//list
				UserDataList list = new UserDataList(name, bMandatory);
				return list;
			}
		}

		/// <summary>
		/// Merge the element with the userData field
		/// </summary>
		/// <param name="data">Hashtable of data that we are going to replace the userData elements with</param>
		/// <param name="type">ElementType of the element we are working on</param>
		/// <returns>string of the element with the userData loaded into the string</returns>
		private string MergeData(Hashtable data, ElementType type)
		{
			string temp=null;
			StringBuilder sb = new StringBuilder();
			int length = 0;
			int iStart = 0;
			int iFind = 0;
			int iTemp = 0;
			int elementEnd;
			string element;
			string name;
			string foundElement = null;
			bool blnElements = false;

			XmlDocument xmlDoc = new XmlDocument();

			if(type==ElementType.Subject)
			{
				temp = _subject;
			}
			else if(type==ElementType.Body)
			{
				temp = _body;
			}

			
			//build the output string using the data in values
			length = temp.Length;
			
			while(iStart < length)
			{
				blnElements = false;
				iFind = temp.IndexOf("<", iStart);
				if(iFind==-1)
				{
					//copy from the start to finish then break
					sb.Append(temp.Substring(iStart));
					break;
				}
				else
				{
					//copy all the data up to where we have found this starting element
					 sb.Append(temp.Substring(iStart, iFind-iStart));

					//read the element we have just found into an XMLDocument
					//this way we can easily get the attribute "Name" and hence
					//load that data from the hashtable
					elementEnd = temp.IndexOf(">", iFind)+1;
					element = temp.Substring(iFind, elementEnd - iFind);

					//check the element is a userData element and
					//not a html tag
					if(element.IndexOf("userData") == -1)
					{
						iStart = temp.IndexOf(">", iFind)+1;
						sb.Append(element);
						continue;
					}

					//check we have got the closing element since
					//in version 1.2 we have child elements on the
					//list and dictionary elements
					if(element.IndexOf(" ")!= -1)
						foundElement = element.Substring(1, element.IndexOf(" ") - 1);

					//test to see if the tag is self closing or if we need to find the end element
					if(element[element.Length-2]!='/')
					{
						//find the next </foundElement tag to get the complete fragment
						iTemp = temp.IndexOf("</"+foundElement, iFind);
						element = temp.Substring(iFind, (temp.IndexOf(">", iTemp) - iFind)+1);
						iStart = temp.IndexOf(">", iTemp)+1; 
						blnElements = true;
					}

					//if the element we have found is a closing element we need to skip it
					if(element[1]== '/' )
					{
						iStart = temp.IndexOf(">", iFind)+1; 
						continue;
					}

					xmlDoc.LoadXml(element);
					name = xmlDoc.DocumentElement.GetAttribute("name");

					 //insert value
					if(data.ContainsKey(name))
					{
						//test to see if we have a UserList object
						if(data[name] is UserDataList)
						{
							UserDataList userDataList = (UserDataList)data[name];

							//output the start string
							if(userDataList.ItemStart!=string.Empty)
							{
								//check if we are in plain or html mode
								//if in plain mode we need to format the whitespace
								//so we output correctly
								//html needs no special processing
								if(_mailFormatType==MailFormatType.PlainText)
								{
									sb.Append(FormatWhitespace(userDataList.ItemStart));
								}
								else
								{
									sb.Append(userDataList.ItemStart);
								}
							}

							//enumerate over the values
							IEnumerator iterator = userDataList.IEnumerable.GetEnumerator();
							while(iterator.MoveNext())
							{
								sb.Append(iterator.Current.ToString());

								//check if we are in plain or html mode
								//if in plain mode we need to format the whitespace
								//so we output correctly
								//html needs no special processing
								if(_mailFormatType==MailFormatType.PlainText)
								{
									sb.Append(FormatWhitespace(userDataList.ItemSeparator));
								}
								else
								{
									sb.Append(userDataList.ItemSeparator);
								}
							}
							//trim the last ItemSeparator of the stringBuilder
							if(userDataList.ItemSeparator.Length > 0)
							{
								sb.Remove(sb.Length - userDataList.ItemSeparator.Length, userDataList.ItemSeparator.Length);
							}

							//output the end string
							if(userDataList.ItemEnd!=string.Empty)
							{
								//check if we are in plain or html mode
								//if in plain mode we need to format the whitespace
								//so we output correctly
								//html needs no special processing
								if(_mailFormatType==MailFormatType.PlainText)
								{
									sb.Append(FormatWhitespace(userDataList.ItemEnd));
								}
								else
								{
									sb.Append(userDataList.ItemEnd);
								}
							}
						}
						else if(data[name] is UserDataDictionary)
						{
							UserDataDictionary userDict = (UserDataDictionary)data[name];

							//if we have a start string output it
							if(userDict.ItemStart!=string.Empty)
							{
								//check if we are in plain or html mode
								//if in plain mode we need to format the whitespace
								//so we output correctly
								//html needs no special processing
								if(_mailFormatType==MailFormatType.PlainText)
								{
									sb.Append(FormatWhitespace(userDict.ItemStart));
								}
								else
								{
									sb.Append(userDict.ItemStart);
								}
								
							}

							
							//iterator over the key/value pairs
							IDictionary dict = userDict.Dictionary;

							IDictionaryEnumerator dictEnum = dict.GetEnumerator();
							while(dictEnum.MoveNext())
							{
								sb.Append(dictEnum.Key.ToString());

								//check if we are in plain or html mode
								//if in plain mode we need to format the whitespace
								//so we output correctly
								//html needs no special processing
								if(_mailFormatType==MailFormatType.PlainText)
								{
									sb.Append(FormatWhitespace(userDict.ItemSeparator));
								}
								else
								{
									sb.Append(userDict.ItemSeparator);
								}
															
								sb.Append(dictEnum.Value.ToString());

								//check if we are in plain or html mode
								//if in plain mode we need to format the whitespace
								//so we output correctly
								//html needs no special processing
								if(_mailFormatType==MailFormatType.PlainText)
								{
									sb.Append(FormatWhitespace(userDict.RowSeparator));
								}
								else
								{
									sb.Append(userDict.RowSeparator);
								}
							}

							//remove the last remove separator from the output

							//todo we need to fix this for the plain text format when we have whitespace formatting?
							if(userDict.RowSeparator.Length > 0 )
							{
								sb.Remove(sb.Length - userDict.RowSeparator.Length, userDict.RowSeparator.Length);
							}

							//if we have an end string output it
							if(userDict.ItemEnd!=string.Empty)
							{
								//check if we are in plain or html mode
								//if in plain mode we need to format the whitespace
								//so we output correctly
								//html needs no special processing
								if(_mailFormatType==MailFormatType.PlainText)
								{
									sb.Append(FormatWhitespace(userDict.ItemEnd));
								}
								else
								{
									sb.Append(userDict.ItemEnd);
								}
							}
						}
						else if(data[name] is UserDataItem)
						{
							if(_data.ContainsKey(name))
							{
								sb.Append(_data[name].ToString());
							}
							else
							{
								//simple string data to output
								UserDataItem userItem = (UserDataItem)data[name];							
								sb.Append(userItem.DefaultValue);
							}
						}
					}
					else
					{
						//no data so skip over the field
					}

					if(!blnElements)
						iStart = temp.IndexOf(">", iFind)+1;
				 }
			}

			//escape any XML reserved characters in the plain text
			//email messages
			if(this.MailFormat == MailFormatType.PlainText)
			{
				sb.Replace("&lt;", "<");
				sb.Replace("&gt;", ">");
				sb.Replace("&amp;", "&");
				sb.Replace("&apos;", "'");
				sb.Replace("&quot;", "\"");
			}
			else
			{
				//we must be using HTML format in which case
				//remove \r\n\t whitespace that we don't need
				sb.Replace("\r", "");
				sb.Replace("\n", "");
				sb.Replace("\t", "");
			}

			//Console.Out.WriteLine(sb.ToString());

			return sb.ToString();
		}


		
	}

	
}
