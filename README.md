# EmailTemplate
It's a simple framework written in C# that allow email messages to be composed in a XML file that can then be merged with data at runtime and sent to a recipient.
Put another way, it allows you to create a web application that sends emails without having to hard code the email message in your assembly.

Sample
Here is a really simple example of an email template.

```xml

<?xml version="1.0" encoding="utf-8" ?> 
<template:emailTemplate application="example application" version="2.0"  
xmlns:user="http://www.bitethebullet.co.uk/EmailProcessor/UserData" 
 xmlns:template="http://bitethebullet.co.uk/EmailProcessor/Template"> 
<template:name>User Registered</template:name> 
<template:description>Email template used as an example,  
sends a welcome email to new registered  
users along with there details</template:description> 
<template:mailFormat>PlainText</template:mailFormat> 
<template:message> 
 <template:to></template:to> 
 <template:from>admin@dummysite.com</template:from> 
 <template:subject>Welcome to dummySite</template:subject> 
 <template:body><user:userData name="user_name" mandatory="true" /> 
  
Thank you for registering with www.dummysite.com, our account  
details are shown below 
<user:userDataDictionary mandatory="true" name="userAccountDetails"> 
  <user:itemStart></user:itemStart> 
  <user:itemEnd></user:itemEnd> 
  <user:itemSeparator>:- </user:itemSeparator> 
  <user:rowSeparator>/r/n</user:rowSeparator> 
 </user:userDataDictionary> 
Regards 
Site Administrator 
 </template:body> 
 <template:attachments> 
 </template:attachments> 
</template:message> 
</template:emailTemplate>

```
The template is created in XML and contains markup elements which are replaced with data prior to sending the email.
Sample C# code used to populate the template and send the email is shown below.
``` csharp
using System; 
using System.Collections; 
using System.Collections.Specialized; 
 
using EmailTemplateProcessor; 
 
namespace Email_Template_Example 
{ 
    /// <summary> 
    /// Example used to send a sample email using email 
    ///template framework 
    /// </summary> 
    class Example 
    { 
        /// <summary> 
        /// The main entry point for the application. 
        /// </summary> 
        [STAThread] 
        static void Main(string[] args) 
        { 
            //normally the data would come from a database,  
            //here we will load the  
            //data into the hashtable ourselves 
            Hashtable data = new Hashtable(); 
            data.Add("user_name", "John Smith"); 
 
            //create some sample user account data in a  
            //hashtable 
            OrderedDictionary userAccount = new OrderedDictionary(); 
            userAccount.Add("Username", "smithj"); 
            userAccount.Add("Password", "secret"); 
            userAccount.Add("Street", "1 New Town"); 
            userAccount.Add("City", "London"); 
            userAccount.Add("Postcode", "L1 3RG"); 
 
            //add the userdetails hashtable to data 
            data.Add("userAccountDetails", userAccount); 
 
 
            //merge the template with the data 
            EmailTemplate mailTemplate = new EmailTemplate( 
                        System.Environment.CurrentDirectory,  
                        @"\mailTemplate\simpleListTemplate.xml"); 
            mailTemplate.LoadData(data); 
             
            //since we are sending this email to one address we 
            //can use the new property 
            mailTemplate.ToSingleAddress = ""; //add your address  
                                         //here to get this to work 
 
            //other wise we could call this method with  
            //a string array 
            //holding all the receipt email address 
            //mailTemplate.To = new string[] {"some@emailaddresshere.info", 
            // "...", "..."};  
            //this would come the database in real world 
 
            EmailProcessor processor = new EmailProcessor("localhost"); 
            processor.SendEmail(mailTemplate); 
        } 
    } 
}
```
