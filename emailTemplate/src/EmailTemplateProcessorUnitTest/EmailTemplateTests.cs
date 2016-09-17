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
using NUnit.Framework;
using EmailTemplateProcessor;

namespace EmailTemplateProcessorUnitTest
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class EmailTemplateTests
	{
		protected string _fileName;
		protected string _path;
		protected string _fqFileName;
		protected string _simpleFileName;
		protected string _fqSimpleFileName;
		protected string _fsUserDataFileName;
		protected string _fsUserDataFileName2;
		protected string _fsUserDataFileName3;
		protected string _fsUserDataFileName4;
		protected string _fsUserDataFileName5;
		protected string _fsUserDataFileName6;
		protected string _fsUserDataFileName7;
		protected string _fsUSerDataFileName8;
		protected string _fsUserDataFileName9;
		protected string _fsUserDataFileName10;

		public EmailTemplateTests()
		{
		}

		/// <summary>
		/// helper readonly property used as the single place to set the working pathing for the
		/// unit tests
		/// 
		/// need this since work and home machine have different file structure
		/// </summary>
		public static string Path
		{
			get
			{
				return System.Environment.CurrentDirectory + @"\mailTemplates\plaintext";
			}
		}

		[TestFixtureSetUp]
		public void SetUpFilePaths()
		{
			_fileName = "notThere.xml";
			_path = Path;
			_fqFileName = @Path + "\notThere.xml";

			_simpleFileName = "simpleTemplate.xml";
			_fqSimpleFileName = _path + "\\" + _simpleFileName;


			_fsUserDataFileName = _path + "\\simpleUserDataTest.xml";
			_fsUserDataFileName2 = _path + "\\simpleUserDataTest2.xml";
			_fsUserDataFileName3 = _path + "\\simpleSequenceTemplate.xml";
			_fsUserDataFileName4 = _path + "\\attachmentTestTemplate.xml";
			_fsUserDataFileName5 = _path + "\\badCharactersTeamplate.xml";
			_fsUserDataFileName6 = _path + "\\escapedCharactersTemplate.xml";
			_fsUserDataFileName7 = _path + "\\plainTextTemplate.xml";
			_fsUSerDataFileName8 = _path + "\\simpleUserDataTest3.xml";
			_fsUserDataFileName9 = _path + "\\simpleUserDataTest4.xml";
			_fsUserDataFileName10 = _path + "\\whitespaceTest.xml";
		}

		/// <summary>
		/// load template that doesn't exist using path and filename name constructor
		/// </summary>
		[Test]
		[ExpectedException(typeof(EmailTemplateException))]
		public void LoadInvalidTemplate()
		{
			EmailTemplate template = new EmailTemplate(_path, _fileName);
		}

		/// <summary>
		/// load template that doesn't exist using path and fullyqulifiedfile
		/// </summary>
		[Test]
		[ExpectedException(typeof(EmailTemplateException))]
		public void LoadInvalidTemplate2()
		{
			EmailTemplate template = new EmailTemplate(_fqFileName);
		}

		/// <summary>
		/// load template using path and filename name constructor
		/// </summary>
		[Test]
		public void LoadTemplate()
		{
			EmailTemplate template = new EmailTemplate(_path, _simpleFileName);
			Assert.AreEqual(_path, template.Path);
			Assert.AreEqual(_simpleFileName, template.FileName);

			Assert.AreEqual("Simple Template", template.TemplateName);
			Assert.AreEqual("Template used to test", template.TemplateDescription);
			Assert.AreEqual("TestApplication", template.Application);
			Assert.AreEqual(MailFormatType.PlainText, template.MailFormat);

			Assert.AreEqual(1, template.To.Length);
            Assert.AreEqual("emailtemplate@mailinator.com", template.To[0]);
            Assert.AreEqual("emailtemplate@mailinator.com", template.From);
			Assert.AreEqual("TestEmail", template.Subject);
			Assert.AreEqual(0, template.Attachments.Length);
			Assert.AreEqual(MailPriorityType.Normal, template.MailPriority);
		}

		/// <summary>
		/// load template using fullyqulifiedfile name constructor
		/// </summary>
		[Test]
		public void LoadTemplate2()
		{
			EmailTemplate template = new EmailTemplate(_fqSimpleFileName);
			Assert.AreEqual(_path, template.Path);
			Assert.AreEqual(_simpleFileName, template.FileName);

			Assert.AreEqual("Simple Template", template.TemplateName);
			Assert.AreEqual("Template used to test", template.TemplateDescription);
			Assert.AreEqual("TestApplication", template.Application);
			Assert.AreEqual(MailFormatType.PlainText, template.MailFormat);

			Assert.AreEqual(1, template.To.Length);
            Assert.AreEqual("emailtemplate@mailinator.com", template.To[0]);
            Assert.AreEqual("emailtemplate@mailinator.com", template.From);
			Assert.AreEqual("TestEmail", template.Subject);
			Assert.AreEqual(0, template.Attachments.Length);
		}

		/// <summary>
		/// check we can override the default properties from those loaded from the
		/// template file
		/// </summary>
		[Test]
		public void LoadTemplateSetProperties()
		{
			EmailTemplate template = new EmailTemplate(_fqSimpleFileName);
			
			string[] to = new string[2]{"markmc@talkgas.net", "mark.mcavoy@commmercemedia.net"};
			template.To = to;
            template.From = "sample1@sampleemailaddressnotreal.info";
			template.Subject = "new SubjeCt";
			template.Body = null;
			template.MailPriority = MailPriorityType.High;
            template.Cc = "sample2@sampleemailaddressnotreal.info";
            template.Bcc = "sample3@sampleemailaddressnotreal.info";

			Assert.AreEqual(to, template.To);
            Assert.AreEqual("sample1@sampleemailaddressnotreal.info", template.From);
			Assert.AreEqual("new SubjeCt", template.Subject);
			Assert.AreEqual(null, template.Body);
            Assert.AreEqual("sample2@sampleemailaddressnotreal.info", template.Cc);
            Assert.AreEqual("sample3@sampleemailaddressnotreal.info", template.Bcc);
			
		}

		/// <summary>
		/// check that basic data copying is occuring
		/// </summary>
		[Test]
		public void LoadTemplateInsertData()
		{
			Hashtable data = new Hashtable();
			string now = DateTime.Now.ToString();
			data.Add("DataTime", now);

			EmailTemplate template = new EmailTemplate(_fqSimpleFileName);
			template.LoadData(data);

			string body = template.PreviewBody();
			Assert.IsNotNull(body);
			Assert.IsTrue(body.IndexOf("Mark") != -1);
			Assert.IsTrue(body.IndexOf(now) != - 1);
		}

		/// <summary>
		/// when the template is missing a name attribute on the userData element
		/// </summary>
		[Test]
		[ExpectedException(typeof(EmailTemplateException))]
		public void BrokenUserDataTemplate()
		{
			Hashtable data = new Hashtable();
			string now = DateTime.Now.ToString();
			data.Add("DataTime", now);

			EmailTemplate template = new EmailTemplate(_fsUserDataFileName);
			template.LoadData(data);
		}

		/// <summary>
		/// check we throw exception when mandatory userdata is missing data/default value
		/// </summary>
		[Test]
		[ExpectedException(typeof(EmailTemplateException))]
		public void MissingMandatoryData()
		{
			Hashtable data = new Hashtable();
			string now = DateTime.Now.ToString();
			data.Add("DataTime", now);

			EmailTemplate template = new EmailTemplate(_fsUserDataFileName2);
			template.LoadData(data);
		}

		/// <summary>
		/// check the format of the body is correc when we skip some userData element and insert default and data values
		/// </summary>
		[Test]
		public void BodySpacingFormatTest()
		{
			Hashtable data = new Hashtable();
			string now = DateTime.Now.ToString();
			data.Add("UserData3", "8");
			data.Add("UserData4", "10");

			EmailTemplate template = new EmailTemplate(_fsUserDataFileName3);
			template.LoadData(data);

			string body = template.PreviewBody();
			Assert.IsNotNull(body);
			Assert.AreEqual("1 2 3 4 5 6 7 8 9 10", body);
            Assert.AreEqual("emailtemplate@mailinator.com", template.Cc);
            Assert.AreEqual("emailtemplate@mailinator.com", template.Bcc);
			Assert.AreEqual(MailPriorityType.High, template.MailPriority);
		}

		/// <summary>
		/// test to check we load an attachment ok
		/// </summary>
		[Test]
		public void LoadTemplateWithAttachment()
		{
			EmailTemplate template = new EmailTemplate(_fsUserDataFileName4);

			Assert.AreEqual("Simple Template", template.TemplateName);
			Assert.AreEqual("Template used to test", template.TemplateDescription);
			Assert.AreEqual("TestApplication", template.Application);
			Assert.AreEqual(MailFormatType.PlainText, template.MailFormat);

			Assert.AreEqual(1, template.To.Length);
            Assert.AreEqual("emailtemplate@mailinator.com", template.To[0]);
            Assert.AreEqual("emailtemplate@mailinator.com", template.From);
			Assert.AreEqual("TestEmail", template.Subject);
			Assert.AreEqual(2, template.Attachments.Length);
			Assert.AreEqual(MailPriorityType.Normal, template.MailPriority);
		}

		/// <summary>
		/// loads a template with un-escaped reserved characters
		/// </summary>
		[Test]
		[ExpectedException(typeof(EmailTemplateException))]
		public void LoadBadCharactersTemplate()
		{
			EmailTemplate template = new EmailTemplate(_fsUserDataFileName5);

		}

		/// <summary>
		/// tests that preview is current when values escaped for XML
		/// also test that the preview works when o data is loaded. i.e. a template
		/// that is just text with no userData
		/// </summary>
		[Test]
		public void LoadEscapedCharactersTemplate()
		{
			EmailTemplate template = new EmailTemplate(_fsUserDataFileName6);
			Assert.AreEqual("1 < 2 & 'mark' > \"test\"", template.PreviewBody());
		}

		/// <summary>
		/// test we still get the body preview when there is no userdata to supply using 
		/// the LoadData method of the template
		/// </summary>
		[Test]
		public void LoadPlainTextTemplate()
		{
			EmailTemplate template = new EmailTemplate(_fsUserDataFileName7);
			Assert.AreEqual("mark",template.PreviewBody());
		}


		[Test]
		public void LoadSimpleDefaultDataTemplate()
		{
			EmailTemplate template = new EmailTemplate(_fsUSerDataFileName8);
			Assert.AreEqual("mark mark", template.PreviewBody());
		}

		[Test]
		public void LoadNoStringDataValues()
		{
			EmailTemplate template = new EmailTemplate(_fsUserDataFileName9);

			int name1 = 31;
			DateTime name2 = DateTime.Now;

			Hashtable data = new Hashtable();
			data.Add("name1", name1);
			data.Add("name2", name2);

			template.LoadData(data);

			string body = template.PreviewBody();
			string[] bodyData = body.Split(';');

			Assert.AreEqual(2, bodyData.Length);
			Assert.AreEqual(name2.ToString(), bodyData[1]);
			Assert.AreEqual(name1.ToString(), bodyData[0]);
		}

		/// <summary>
		/// whitespace text
		/// </summary>
		[Test]
		public void WhiteSpaceTest()
		{
			EmailTemplate template = new EmailTemplate(_fsUserDataFileName10);

			EmailProcessor processor = new EmailProcessor("localhost");
#if SENDMAIL
            processor.SendEmail(template);
#endif

		}


        /// <summary>
        /// Check the single ToSingleAddress property works as expected.
        /// </summary>
        [Test]
        public void TestToSingleAddressProperty()
        {
            EmailTemplate template = new EmailTemplate(_fqSimpleFileName);

            string to = "mail@somedomainiguess.com";

            template.ToSingleAddress = to;

            Assert.AreEqual(to, template.ToSingleAddress);
            Assert.AreEqual(1, template.To.Length);
            Assert.AreEqual(to, template.To[0]);

        }
	}
}
