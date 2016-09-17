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
using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;
using EmailTemplateProcessor;

namespace EmailTemplateProcessorUnitTest
{
	/// <summary>
	/// Summary description for EmailTemplateDictionaryTests.
	/// </summary>
	[TestFixture]
	public class EmailTemplateDictionaryTests
	{
		protected string _path;
		protected string _fpSimpleTest;
		protected string _fpSimpleTest2;

		[TestFixtureSetUp]
		public void SetUpTest()
		{
			_path = EmailTemplateTests.Path;
			_fpSimpleTest = _path + @"\simpleDictionaryTemplate.xml";
			_fpSimpleTest2 = _path + @"\simpledictionaryTemplate2.xml";
		}

		/// <summary>
		/// test that we output the data correct when using a simple dictionary
		/// </summary>
		[Test]
		public void SimpleTest()
		{
			Hashtable data = new Hashtable();

			//create the dictionary that we want to output
			Hashtable dictionary = new Hashtable();
			for(int i = 1; i < 4; i++)
			{
				dictionary["key"+i] = "value"+i;
			}

			dictionary["username"] = "smithj";
			dictionary["password"] = "secret";
			dictionary["email"] = "email@domain.com";

			data.Add("UserDataDictionary1", dictionary);

			EmailTemplate email = new EmailTemplate(_fpSimpleTest);
			email.LoadData(data);
			email.Subject = "Dictionary plain text";

            string body = email.PreviewBody();

            Assert.IsTrue(body.IndexOf("key1\tvalue1") > -1);
            Assert.IsTrue(body.IndexOf("key2\tvalue2") > -1);
            Assert.IsTrue(body.IndexOf("key3\tvalue3") > -1);
            Assert.IsTrue(body.IndexOf("username\tsmithj") > -1);
            Assert.IsTrue(body.IndexOf("password\tsecret") > -1);
            Assert.IsTrue(body.IndexOf("email\temail@domain.com") > -1);

			EmailProcessor processor = new EmailProcessor("localhost");
#if SENDMAIL
            processor.SendEmail(email);
#endif

		}

		/// <summary>
		/// simple test to check that an object that doesn't support
		/// IDictionaryEnumerator interface throws an exception
		/// </summary>
		[Test]
		[ExpectedException(typeof(EmailTemplateException))]
		public void NotValidTypeDictionary()
		{
			Hashtable data = new Hashtable();
			data.Add("UserDataDictionary1", DateTime.Now);

			EmailTemplate email = new EmailTemplate(_fpSimpleTest);
			email.LoadData(data);
		}

		/// <summary>
		/// simple test to check we throw an exception when we
		/// run a template with a mandatory dictionary and do not
		/// supply it
		/// </summary>
		[Test]
		[ExpectedException(typeof(EmailTemplateException))]
		public void MissingMandatoryDictionary()
		{
			Hashtable data = new Hashtable();
			data.Add("UserData1", DateTime.Now);

			EmailTemplate email = new EmailTemplate(_fpSimpleTest);
			email.LoadData(data);
		}

		/// <summary>
		/// check that if we don't supply a dictionary object and the item
		/// is not mandatory the output is still ok
		/// </summary>
		[Test]
		public void NoDictionarySupplied()
		{
			Hashtable data = new Hashtable();
			data.Add("UserData1", "Foo");
			data.Add("UserData2", "Bar");

			EmailTemplate email = new EmailTemplate(_fpSimpleTest2);
			email.LoadData(data);

			Assert.AreEqual("FooBar", email.PreviewBody());

		}

        /// <summary>
        /// Test can process a Dictionary T, T object
        /// when passed in for userdatadictionary element
        /// </summary>
        [Test]
        public void TestGenericDictionary()
        {
            Dictionary<string, long> data = new Dictionary<string, long>();
            data.Add("one", 1000);
            data.Add("two", 2000);
            data.Add("three", 3000);

            Hashtable bindingData = new Hashtable();
            bindingData.Add("UserDataDictionary1", data);

            EmailTemplate template = new EmailTemplate(_fpSimpleTest);
            template.LoadData(bindingData);

            string body = template.PreviewBody();

            //validate the data is outputted as expected;
            int dataElement1 = body.IndexOf("one\t1000");
            int dataElement2 = body.IndexOf("two\t2000");
            int dataElement3 = body.IndexOf("three\t3000");

            //check we have the elements
            Assert.IsTrue(dataElement1 > -1, "Missing the first element of the hashtable");
            Assert.IsTrue(dataElement2 > -1, "Missing the second element of the hashtable");
            Assert.IsTrue(dataElement3 > -1, "Missing the third element of the hashtable");

            //check the order is correct
            Assert.IsTrue(dataElement2 > dataElement1);
            Assert.IsTrue(dataElement3 > dataElement2);
        }

        /// <summary>
        /// Test the elements of the hashtable used as the userdatadictionary data
        /// source are outputted in the correct order
        /// </summary>
        [Test]
        public void DictionaryOrderTest()
        {
            OrderedDictionary data = new OrderedDictionary();
            data.Add("one", "1000");
            data.Add("two", "2000");
            data.Add("three", "3000");

            Hashtable bindingData = new Hashtable();
            bindingData.Add("UserDataDictionary1", data);

            EmailTemplate template = new EmailTemplate(_fpSimpleTest);
            template.LoadData(bindingData);

            string body = template.PreviewBody();

            //validate the data is outputted as expected;
            int dataElement1 = body.IndexOf("one\t1000");
            int dataElement2 = body.IndexOf("two\t2000");
            int dataElement3 = body.IndexOf("three\t3000");

            //check we have the elements
            Assert.IsTrue(dataElement1 > -1);
            Assert.IsTrue(dataElement2 > -1);
            Assert.IsTrue(dataElement3 > -1);

            //check the order is correct
            Assert.IsTrue(dataElement2 > dataElement1, "first item is notin the correct order");
            Assert.IsTrue(dataElement3 > dataElement2, "second item is notin the correct order");
        }


        /// <summary>
        /// Test we can output an OrderedDictionary object correctly
        /// </summary>
        [Test]
        public void OrderedDictionaryTest()
        {
            OrderedDictionary data = new OrderedDictionary();
            data.Add("one", "1000");
            data.Add("two", "2000");
            data.Add("three", "3000");
            data.Add("four", "4000");
            data.Add("five", "5000");

            Hashtable bindingData = new Hashtable();
            bindingData.Add("UserDataDictionary1", data);

            EmailTemplate template = new EmailTemplate(_fpSimpleTest);
            template.LoadData(bindingData);

            string body = template.PreviewBody();

            //validate the data is outputted as expected;
            int dataElement1 = body.IndexOf("one\t1000");
            int dataElement2 = body.IndexOf("two\t2000");
            int dataElement3 = body.IndexOf("three\t3000");
            int dataElement4 = body.IndexOf("four\t4000");
            int dataElement5 = body.IndexOf("five\t5000");

            //check we have the elements
            Assert.IsTrue(dataElement1 > -1);
            Assert.IsTrue(dataElement2 > -1);
            Assert.IsTrue(dataElement3 > -1);
            Assert.IsTrue(dataElement4 > -1);
            Assert.IsTrue(dataElement5 > -1);

            //check the order is correct
            Assert.IsTrue(dataElement2 > dataElement1, "first item is not in the correct order");
            Assert.IsTrue(dataElement3 > dataElement2, "second item is not in the correct order");
            Assert.IsTrue(dataElement4 > dataElement3, "third item is not in the correct order");
            Assert.IsTrue(dataElement5 > dataElement4, "fourth item is not in the correct order");
        }

		public EmailTemplateDictionaryTests()
		{
		}
	}
}
