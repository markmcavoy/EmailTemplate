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
	/// Summary description for EmailProcessorSubjectLineTests.
	/// </summary>
	[TestFixture]
	public class EmailTemplateSubjectLineTests
	{
		string _path;
		string _simpleSubjectLineTest;
		string _simpleSubjectLineTestNoDefaultValue;
		
		[TestFixtureSetUp]
		public void SetUpTests()
		{
			_path = EmailTemplateTests.Path;
			_simpleSubjectLineTest = _path + @"\simpleSubjectTemplate.xml";
			_simpleSubjectLineTestNoDefaultValue = @_path + @"\simpleSubjectTemplateException.xml";
		}

		[Test]
		public void TestDefaultSubjectLine()
		{
			EmailTemplate emailTemplate = new EmailTemplate(_simpleSubjectLineTest);
			Assert.AreEqual("TestEmail Mark", emailTemplate.PreviewSubjectLine());
		}

		[Test]
		public void TestSubjectLine()
		{
			Hashtable table = new Hashtable();
			table.Add("SubjectData1", "FooBar");

			EmailTemplate emailTemplate = new EmailTemplate(_simpleSubjectLineTest);
			emailTemplate.LoadData(table);

			Assert.AreEqual("TestEmail FooBar", emailTemplate.PreviewSubjectLine());
			
		}

		[Test]
		[ExpectedException(typeof(EmailTemplateException))]
		public void ExceptionSubjectLine()
		{
			Hashtable table = new Hashtable();

			EmailTemplate emailTemplate = new EmailTemplate(_simpleSubjectLineTestNoDefaultValue);
			emailTemplate.LoadData(table);
			
		}

		[Test]
		public void TestSubjectLineWithNoDefault()
		{
			Hashtable table = new Hashtable();
			table.Add("SubjectData1", "FooBar");

			EmailTemplate emailTemplate = new EmailTemplate(_simpleSubjectLineTestNoDefaultValue);
			emailTemplate.LoadData(table);

			Assert.AreEqual("TestEmail FooBar", emailTemplate.PreviewSubjectLine());
			
		}


		public EmailTemplateSubjectLineTests()
		{
		}
	}
}
