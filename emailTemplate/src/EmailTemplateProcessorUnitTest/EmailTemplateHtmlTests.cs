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
using System.Collections.Specialized;

using NUnit.Framework;

using EmailTemplateProcessor;

namespace EmailTemplateProcessorUnitTest
{
	[TestFixture]
	public class EmailTemplateHtmlTests
	{
		protected string _path;
		protected string _fsUserDataFileName;
		protected string _fsUserDataFileName2;
		protected string _fsUserDataFileName3;
		protected string _fsUserDataFileName4;
		protected string _fsUserDataFileName5;
        protected string _fsUserDataFileName6;
        protected string _fsUserDataFileName7;

		public EmailTemplateHtmlTests()
		{
		}

		[TestFixtureSetUp]
		public void SetUpFilePaths()
		{
			_path = System.Environment.CurrentDirectory + @"\mailTemplates\html";

			_fsUserDataFileName = _path + "\\simpleUserData1.xml";
			_fsUserDataFileName2 = _path + "\\simpleUserData2.xml";
			_fsUserDataFileName3 = _path + "\\simpleUserData3.xml";
			_fsUserDataFileName4 = _path + "\\simpleUserDataSubject1.xml";
			_fsUserDataFileName5 = _path + "\\sampleHTMLEmail.xml";
            _fsUserDataFileName6 = _path + "\\simpleUserData4.xml";
            _fsUserDataFileName7 = _path + "\\simpleUserData5.xml";
		}

		[Test]
		public void SimpleHtmlTemplate()
		{
			Hashtable data = new Hashtable();
			string now = DateTime.Now.ToString();
			string expectedBody = @"<b>Sample email</b> from email template processor<br /><br /><b>" + now + "</b>";

			
			data.Add("UserData1", now);

			EmailTemplate template = new EmailTemplate(_fsUserDataFileName);
			template.LoadData(data);

			string body = template.PreviewBody();
			Assert.AreEqual(MailFormatType.HtmlText, template.MailFormat);
			Assert.IsNotNull(body);
			Assert.IsTrue(body.IndexOf(now) != - 1);
			Assert.AreEqual(expectedBody, body);

			EmailProcessor processor = new EmailProcessor("localhost");
			template.Subject = "HtmlTemplate";

			Assert.AreEqual("HtmlTemplate", template.PreviewSubjectLine());

#if SENDMAIL
            processor.SendEmail(template);
#endif
		}

		[Test]
		public void SimpleHtmlTemplateList()
		{
			Hashtable data = new Hashtable();
			string now = DateTime.Now.ToString();
			//string expectedBody = @"<b>Sample email</b> from email template processor<br /><br /><b>" + now + "</b>";

			ArrayList numbers = new ArrayList();
			numbers.Add(1);
			numbers.Add(2);
			numbers.Add(3);
			numbers.Add(4);
			numbers.Add(5);

			data.Add("UserData1", now);
			data.Add("UserDataList1", numbers);

			EmailTemplate template = new EmailTemplate(_fsUserDataFileName2);
			template.LoadData(data);

			string expectedBody = "<b>Sample email</b> from email template processor<br /><br /><b>" + now.ToString() + "</b>Mark<br /> start 1, 2, 3, 4, 5 end ";
			string body = template.PreviewBody();
			Assert.AreEqual(MailFormatType.HtmlText, template.MailFormat);
			Assert.IsNotNull(body);
			Assert.IsTrue(body.IndexOf(now) != - 1);
			Assert.IsTrue(body.IndexOf("Mark") != -1);
			Assert.IsTrue(body.IndexOf("start 1") != -1);
			Assert.AreEqual(expectedBody, body);

			EmailProcessor processor = new EmailProcessor("localhost");
			template.Subject = "HtmlTemplateList";

			Assert.AreEqual("HtmlTemplateList", template.PreviewSubjectLine());

#if SENDMAIL
            processor.SendEmail(template);
#endif
		}

		[Test]
		public void SimpleHtmlTemplateSubject()
		{
			Hashtable data = new Hashtable();
			string now = DateTime.Now.ToString();

			ArrayList numbers = new ArrayList();
			numbers.Add(1);
			numbers.Add(2);
			numbers.Add(3);
			numbers.Add(4);
			numbers.Add(5);

			data.Add("UserData1", now);
			data.Add("UserDataList1", numbers);
			data.Add("SubjectData1", "123");

			EmailTemplate template = new EmailTemplate(_fsUserDataFileName4);
			template.LoadData(data);

			string body = template.PreviewBody();
			Assert.AreEqual(MailFormatType.HtmlText, template.MailFormat);
			Assert.IsNotNull(body);
			Assert.IsTrue(body.IndexOf(now) != - 1);
			Assert.IsTrue(body.IndexOf("Mark") != -1);
			Assert.IsTrue(body.IndexOf("start 1") != -1);
			Assert.AreEqual("TestEmail 123", template.PreviewSubjectLine());
		}


		[Test]
		public void SimpleHtmlTemplateDictionary()
		{
			Hashtable data = new Hashtable();
			string now = DateTime.Now.ToString();

			ArrayList numbers = new ArrayList();
			numbers.Add(1);
			numbers.Add(2);
			numbers.Add(3);
			numbers.Add(4);
			numbers.Add(5);

			//create the dictionary that we want to output
            OrderedDictionary dictionary = new OrderedDictionary();
			for(int i = 1; i < 4; i++)
			{
				dictionary["key"+i] = "value"+i;
			}

			data.Add("UserData1", now);
			data.Add("UserDataList1", numbers);
			data.Add("UserDataDictionary1", dictionary);

			EmailTemplate template = new EmailTemplate(_fsUserDataFileName3);
			template.LoadData(data);

			

			string body = template.PreviewBody();
			Assert.AreEqual(MailFormatType.HtmlText, template.MailFormat, "Mail format is not HTML");
			Assert.IsNotNull(body, "Body of the email is null");
			Assert.IsTrue(body.IndexOf(now) != - 1, "date value not found in email");
			Assert.IsTrue(body.IndexOf("Mark") != -1, "We should have userdata item output the default value");
			Assert.IsTrue(body.IndexOf("start 1") != -1, "The list should output a start like this on the userDataList tag");

            Assert.IsTrue(body.IndexOf("key1--value1<br />") != -1, "The dictionary should output like this on the userDataDictionary");

            //check that the dictionary values are in the correct order
            int key1 = body.IndexOf("key1--value1");
            int key2 = body.IndexOf("key2--value2");
            int key3 = body.IndexOf("key3--value3");

            Assert.IsTrue(key1 < key2, "Dictionary values are in the incorrect order");
            Assert.IsTrue(key2 < key3, "Dictionary values are in the incorrect order");
            
			
			EmailProcessor processor = new EmailProcessor("localhost");
			template.Subject = "HtmlTemplateDictionary";

			Assert.AreEqual("HtmlTemplateDictionary", template.PreviewSubjectLine(), "subject line not as expected");

#if SENDMAIL
            processor.SendEmail(template);
#endif
		}


        /// <summary>
        /// TEst we can output the same userData item more than once 
        /// in the same template without any problems
        /// </summary>
        [Test]
        public void TestWritingDataElementTwice()
        {
            Hashtable data = new Hashtable();
            data.Add("UserData1", "Twice");

            EmailTemplate template = new EmailTemplate(_fsUserDataFileName6);
            template.LoadData(data);

            //check we have the expected value in the body
            string expectedBody = "<b>TwiceTwice</b>";

            Assert.AreEqual(expectedBody, template.PreviewBody(), "Body text is not as expected");
        }


        /// <summary>
        /// TEst we can output the same userDataList item more than once 
        /// in the same template without any problems
        /// </summary>
        [Test]
        public void TestWritingListElementTwice()
        {
            ///create an array of data to output
            ArrayList numbers = new ArrayList();
            numbers.Add(1);
            numbers.Add(2);
            numbers.Add(3);
            numbers.Add(4);
            numbers.Add(5);

            Hashtable data = new Hashtable();
            data.Add("UserDataList1", numbers);

            EmailTemplate template = new EmailTemplate(_fsUserDataFileName7);
            template.LoadData(data);

            //check we have the expected value in the body
            string expectedBody = "<b> start 1, 2, 3, 4, 5 end  start 1, 2, 3, 4, 5 end </b>";

            Assert.AreEqual(expectedBody, template.PreviewBody(), "Body text is not as expected");
        }


		[Test]
		public void SimpleBodyOverwriteTemplate()
		{
			Hashtable data = new Hashtable();
			string now = DateTime.Now.ToString();
			string expectedBody = @"<b>Sample email</b> from email template processor<br /><br /><b>" + now + "</b>";

			
			data.Add("UserData1", now);

			EmailTemplate template = new EmailTemplate(_fsUserDataFileName);
			template.LoadData(data);

			string body = template.PreviewBody();
			Assert.AreEqual(MailFormatType.HtmlText, template.MailFormat);
			Assert.IsNotNull(body);
			Assert.IsTrue(body.IndexOf(now) != - 1);
			Assert.AreEqual(expectedBody, body);

			EmailProcessor processor = new EmailProcessor("localhost");
			template.Subject = "HtmlTemplate";
			template.Body = "OverWrite";

			Assert.AreEqual("HtmlTemplate", template.PreviewSubjectLine());
			Assert.AreEqual("OverWrite", template.PreviewBody());

			template.Subject = "OverWrite!";

#if SENDMAIL
            processor.SendEmail(template);
#endif
		}

		[Test]
		public void SampleHtmlEmailTemplate()
		{
			Hashtable data = new Hashtable();
			string now = DateTime.Now.ToString();
						
			data.Add("UserData1", now);

			EmailTemplate template = new EmailTemplate(_fsUserDataFileName5);
			template.LoadData(data);

			string body = template.PreviewBody();
			Assert.AreEqual(MailFormatType.HtmlText, template.MailFormat);
			Assert.IsNotNull(body);
			Assert.IsTrue(body.IndexOf(now) != - 1);

			EmailProcessor processor = new EmailProcessor("localhost");
			template.Subject = "SampleHtmlTemplate";

			Assert.AreEqual("SampleHtmlTemplate", template.PreviewSubjectLine());

#if SENDMAIL
            processor.SendEmail(template);
#endif
		}
	}
}
