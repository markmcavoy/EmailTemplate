using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;

using log4net;

using EmailTemplateProcessor;

namespace Email_Template_Example
{
	/// <summary>
	/// Example used to send a sample email using email template framework
	/// </summary>
	class Example
	{
        protected static readonly ILog Log = LogManager.GetLogger(typeof(Example));

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
            Log.Debug("Starting to send email message");

			//normally the data would come from a database, here we will load the 
			//data into the hashtable ourselves
			Hashtable data = new Hashtable();
			data.Add("user_name", "John Smith");

			//create some sample user account data in a hashtable
            OrderedDictionary userAccount = new OrderedDictionary();
			userAccount.Add("Username", "smithj");
			userAccount.Add("Password", "secret");
			userAccount.Add("Street", "1 New Town");
			userAccount.Add("City", "London");
			userAccount.Add("Postcode", "L1 3RG");

			//add the userdetails hashtable to data
			data.Add("userAccountDetails", userAccount);


			//merge the template with the data
			EmailTemplate mailTemplate = new EmailTemplate(System.Environment.CurrentDirectory, @"\mailTemplate\simpleListTemplate.xml");
			mailTemplate.LoadData(data);
			
            //since we are sending this email to one address we
            //can use the new property
            mailTemplate.ToSingleAddress = ConfigurationManager.AppSettings["emailToAddress"];

            //other wise we could call this method with a string array
            //holding all the receipt email address
            //mailTemplate.To = new string[] {"some@emailaddresshere.info", "...", "..."}; //this would come the database in real world

			EmailProcessor processor = new EmailProcessor("localhost");
			processor.SendEmail(mailTemplate);

            Log.Debug("Email message sent to user");
		}
	}
}
