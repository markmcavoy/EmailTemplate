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
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace EmailTemplateProcessor
{
	/// <summary>
	/// EmailTemplateProcessor used to send an EmailTemplate object via SMTP
	/// </summary>
	public class EmailProcessor
    {
        #region vars

        /// <summary>
		/// string holds the name of the SMTP server
		/// </summary>
		protected string _stmpServerName;
		/// <summary>
		/// falg indicating if we are in debug mode
		/// </summary>
		protected bool _debugMode;
		/// <summary>
		/// string holding the email address that we use in debug mode
		/// </summary>
		protected string _debugEmailAddress;

        /// <summary>
        /// reference to log4net logger
        /// </summary>
		private readonly static ILog logger = LogManager.GetLogger(typeof(EmailProcessor));

        #endregion

        #region cstor

        /// <summary>
		/// construtor used to create a EmailProcessor object
		/// 
		/// </summary>
		/// <param name="SmtpServerName">name of SMTP server to use to send emails</param>
		public EmailProcessor(string SmtpServerName)
		{
			this._stmpServerName = SmtpServerName;
            string configFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase.Substring(8));
            FileInfo configFile = new FileInfo(configFilePath + @"\log4net.config");
			XmlConfigurator.Configure(configFile);
			logger.Debug("STMP server: " + SmtpServerName);
        }

        #endregion

        #region properties

        /// <summary>
		/// allow debuging of email messages
		/// 
		/// When DebugMode is true, all emails generated will go To the email 
		/// address supplied in DebugEmail property.
		/// </summary>
		public bool DebugMode
		{
			get
			{
				return _debugMode;
			}
			set
			{
				_debugMode = value;
			}
		}

		/// <summary>
		/// Get/Set the DebugEmail address, this is the email address where
		/// emails will be sent when we are in DebugMode
		/// </summary>
		public string DebugEmail
		{
			get
			{
				return _debugEmailAddress;
			}
			set
			{
				_debugEmailAddress = value;
			}
        }

        #endregion

        #region methods


        /// <summary>
        /// Sends an Email Template
        /// </summary>
        /// <remarks>Sending using the method means the SMTP does not require
        /// authentication</remarks>
        /// <param name="message"><see cref="EmailTemplate">EmailTemplate</see> to send</param>
        public void SendEmail(EmailTemplate message)
        {
            SendEmail(message, null);
        }

        /// <summary>
        /// Sends an Email Template
        /// </summary>
        /// <remarks>Sending using the methods allow a username and password to be
        /// defined which contains the account required to authenticate with
        /// the SMTP server</remarks>
        /// <param name="message"><see cref="EmailTemplate">EmailTemplate</see> to send</param>
        /// <param name="username">username to authenticate with the SMTP server</param>
        /// <param name="password">password to authenticate with the SMTP server</param>
        public void SendEmail(EmailTemplate message, string username, string password)
        {
            SendEmail(message, new NetworkCredential(username, password));
        }

        /// <summary>
		/// Sends an Email Template
		/// </summary>
        /// <remarks>Sending using the methods allow a network credentials object
        /// to be defined which contains the account required to authenticate with
        /// the SMTP server</remarks>
        /// <param name="message"><see cref="EmailTemplate">EmailTemplate</see> to send</param>
        /// <param name="credentials">Credentials required to access the SMTP server</param>
		public void SendEmail(EmailTemplate message, NetworkCredential credentials)
		{
			MailMessage mail;
			Attachment mailAttachment;
			string[] to = message.To;	
			SmtpClient smtpClient = new SmtpClient(_stmpServerName);

			for(int i = 0; i < to.Length; i++)
			{
				mail = new MailMessage();

				if(i==0)
				{
					//check if we cc or bcc as we do this when we send the first email
					if(message.Cc!=null){
                        MailAddress ccAddress = new MailAddress(message.Cc);
                        mail.CC.Add(ccAddress);
                    }

                    if (message.Bcc != null)
                    {
                        MailAddress bccAddress = new MailAddress(message.Bcc);
                        mail.Bcc.Add(bccAddress);
                    }
				}

				//if we are in debug mode we send the email to the debug address
				//we ignore the values in the message string[]
				if(_debugMode)
				{
                    MailAddress toAddress = new MailAddress(_debugEmailAddress);
					mail.To.Add(toAddress);
				}
				else
				{
                    MailAddress toAddress = new MailAddress(message.To[i]);
                    mail.To.Add(toAddress);
				}

                MailAddress fromAddress = new MailAddress(message.From);
				mail.From = fromAddress;

                if (!string.IsNullOrEmpty(message.ReplyTo))
                {
                    MailAddress replyToAddress = new MailAddress(message.ReplyTo);
                    mail.ReplyTo = replyToAddress;
                }

				switch(message.MailPriority)
				{
					case MailPriorityType.High:
						mail.Priority = MailPriority.High;
						break;

					case MailPriorityType.Normal:
						mail.Priority = MailPriority.Normal;
						break;

					case MailPriorityType.Low:
						mail.Priority = MailPriority.Low;
						break;
				}

				mail.Subject = message.PreviewSubjectLine();
				mail.Body = message.PreviewBody();

				switch(message.MailFormat)
				{
					case MailFormatType.PlainText:
						mail.IsBodyHtml = false;
						break;

					case MailFormatType.HtmlText:
						mail.IsBodyHtml = true;
						break;
				}	
			
				//attachments
				foreach(string attachment in message.Attachments)
				{
					mailAttachment = new Attachment(attachment);
					mail.Attachments.Add(mailAttachment);
				}
				
				logger.Info("Sending email \"" + message.TemplateName + "\" To:" + mail.To);

				//send the email
                if (credentials != null)
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = credentials;
                }
				smtpClient.Send(mail);

                mail.Dispose();
			}
        }

        #endregion
    }
}
