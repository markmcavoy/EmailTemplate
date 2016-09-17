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
using NUnit.Framework;
using EmailTemplateProcessor;

namespace EmailTemplateProcessorUnitTest
{
	/// <summary>
	/// EmailTemplateDictionaryTests - unit test for the userDictionary email of the
	/// email templates
	/// </summary>
	[TestFixture]
	public class EmailTemplateListTests
	{
		protected string _path;
		protected string _simpleListTest;
		protected string _simpleListTest2;
		protected string _simpleListTest3;

		public EmailTemplateListTests()
		{
		}

		[TestFixtureSetUp]
		public void SetUpTest()
		{
			_path = EmailTemplateTests.Path;
			_simpleListTest = _path + @"\simpleListTemplate.xml";
			_simpleListTest2 = _path + @"\simpleListTemplate2.xml";
			_simpleListTest3 = _path + @"\simpleListTemplate3.xml";
		}

		/// <summary>
		/// test to make sure a mandatory userListData element will
		/// throw an error if not found within the user data
		/// </summary>
		[Test]
		[ExpectedException(typeof(EmailTemplateException))]
		public void MissingMandatoryList()
		{
			EmailTemplate email = new EmailTemplate(_simpleListTest);
			email.LoadData(null);
			email.PreviewBody();
		}


		/// <summary>
		/// Test simple userListData, make sure we have the start, end and separator items
		/// as well as the data values themselves
		/// </summary>
		[Test]
		public void SimpleListTest()
		{
			EmailTemplate email = new EmailTemplate(_simpleListTest);

			Hashtable data = new Hashtable();

			//create an arraylist
			//any IEnumerable object can be used here
			ArrayList numbers = new ArrayList();

			numbers.Add(1);
			numbers.Add(2);
			numbers.Add(3);
			numbers.Add(4);
			numbers.Add(5);

			data.Add("UserDataList1", numbers);

			email.LoadData(data);

			Assert.AreEqual(" start 1, 2, 3, 4, 5 end ", email.PreviewBody());

		}

        /// <summary>
        /// Test that the userdatalist element will work with generic list collections
        /// </summary>
        [Test]
        public void SimpleGenericListTest()
        {
            EmailTemplate email = new EmailTemplate(_simpleListTest);

            Hashtable data = new Hashtable();

            //create an arraylist
            //any IEnumerable object can be used here
            List<int> numbers = new List<int>();

            numbers.Add(1);
            numbers.Add(2);
            numbers.Add(3);
            numbers.Add(4);
            numbers.Add(5);

            data.Add("UserDataList1", numbers);

            email.LoadData(data);

            Assert.AreEqual(" start 1, 2, 3, 4, 5 end ", email.PreviewBody());

        }


		/// <summary>
		/// Test that output ok when itemstart, end, separator are empty
		/// </summary>
		[Test]
		public void SimpleListTest2()
		{
			EmailTemplate email = new EmailTemplate(_simpleListTest3);

			Hashtable data = new Hashtable();

			//create an arraylist
			//any IEnumerable object can be used here
			ArrayList numbers = new ArrayList();

			numbers.Add(1);
			numbers.Add(2);
			numbers.Add(3);
			numbers.Add(4);
			numbers.Add(5);

			data.Add("UserDataList1", numbers);

			email.LoadData(data);

			Assert.AreEqual("12345", email.PreviewBody());
			
		}


		/// <summary>
		/// test that a missing userListData element doesn't cause a problem
		/// when its not supplied, the element is not mandatory
		/// </summary>
		[Test]
		public void SimpleMissingList()
		{
			Hashtable data = new Hashtable();
			data.Add("UserData1", "Foo");
			data.Add("UserData2", "Bar");

			EmailTemplate email = new EmailTemplate(_simpleListTest2);
			email.LoadData(data);

			Assert.AreEqual("FooBar", email.PreviewBody());
		}

		/// <summary>
		/// test to make sure that if we try to bind an object that doesn't support
		/// IEnumerable we throw an errro
		/// </summary>
		[Test]
		[ExpectedException(typeof(EmailTemplateException))]
		public void NotIEnumerableObject()
		{
			Hashtable data = new Hashtable();
			data.Add("UserDataList1", DateTime.Now);

			EmailTemplate email = new EmailTemplate(_simpleListTest);
			email.LoadData(data);
		}
	}
}
