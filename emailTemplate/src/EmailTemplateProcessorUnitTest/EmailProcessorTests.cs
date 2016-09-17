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
	/// Summary description for EmailProcessorTests.
	/// </summary>
	[TestFixture]
	public class EmailProcessorTests
	{
		protected string _fqSimpleTemplate;
		protected string _fqSimpleSubjectTemplate;

		public EmailProcessorTests()
		{
		}

		[TestFixtureSetUp]
		public void SetUpVar()
		{
			string _path = EmailTemplateTests.Path; 
			_fqSimpleTemplate = _path + "\\simpleSequenceTemplate.xml";
			_fqSimpleSubjectTemplate = _path + @"\simpleSubjectTemplate.xml";
		}

		/// <summary>
		/// test all properties work correcly
		/// </summary>
		[Test]
		public void TestSettingProperties()
		{
			EmailProcessor processor = new EmailProcessor("localhost");
			Assert.AreEqual(false, processor.DebugMode);

			processor.DebugMode = true;
            processor.DebugEmail = "emailtemplate@mailinator.com";
			Assert.AreEqual(true, processor.DebugMode);
            Assert.AreEqual("emailtemplate@mailinator.com", processor.DebugEmail);
		}

		/// <summary>
		/// simple test email send
		/// </summary>
		[Test]
		public void TestSimpleSending()
		{
			EmailProcessor processor = new EmailProcessor("localhost");
			EmailTemplate template = new EmailTemplate(_fqSimpleTemplate);
            template.To = new string[1] { "emailtemplate@mailinator.com" };

			Hashtable data = new Hashtable();
			data.Add("UserData3", "8");
			data.Add("UserData4", "10");

			template.LoadData(data);

#if SENDMAIL
			processor.SendEmail(template);
#endif
		}

		[Test]
		public void TestSubjectLineSending()
		{
			EmailProcessor processor = new EmailProcessor("localhost");
			EmailTemplate template = new EmailTemplate(_fqSimpleSubjectTemplate);
            template.To = new string[1] {"emailtemplate@mailinator.com" };

			Hashtable data = new Hashtable();
			data.Add("UserData3", "8");
			data.Add("UserData4", "10");
			data.Add("SubjectData1", "FooBar");

			template.LoadData(data);

#if SENDMAIL
            processor.SendEmail(template);
#endif
		}


		/// <summary>
		/// test all properties work correcly
		/// </summary>
		[Test]
		public void TestOverridingProperties()
		{
			EmailProcessor processor = new EmailProcessor("localhost");
			EmailTemplate template = new EmailTemplate(_fqSimpleSubjectTemplate);
            template.To = new string[1] { "emailtemplate@mailinator.com" };

			Hashtable data = new Hashtable();
			data.Add("UserData3", "8");
			data.Add("UserData4", "10");
			data.Add("SubjectData1", "FooBar");

			template.LoadData(data);
			template.Subject = "subject and body overRide";
			template.Body = "\r\nMark\r\n";

#if SENDMAIL
            processor.SendEmail(template);
#endif
		}
	}
}
