﻿using HouseholdPlanner.Contracts.Common;
using HouseholdPlanner.Contracts.Notification;
using HouseholdPlanner.Models.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HouseholdPlanner.Services.Notification
{
    public class SendGridEmailService : IEmailService
    {
        private const string EmailTemplates = "EmailTemplates";
        private readonly HttpClient _httpClient;
        private readonly IFileService _fileService;
        private readonly SendGridOptions _sendGridOptions;
        private readonly EmailOptions _emailOptions;

        public SendGridEmailService(HttpClient httpClient, IFileService fileService, SendGridOptions sendGridOptions, EmailOptions emailOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _sendGridOptions = sendGridOptions ?? throw new ArgumentNullException(nameof(sendGridOptions));
            _emailOptions = emailOptions ?? throw new ArgumentNullException(nameof(emailOptions));

        }

        public async Task SendRegistrationEmail(string to, string registrationLink)
        {
            string emailContents = await GetRegistrationEmailContent(to, registrationLink);
            await SendEmail(emailContents);
        }

        public async Task SendWelcomeEmail(string to, string name)
        {
            string emailContents = await GetWelcomeEmailContent(to, name);
            await SendEmail(emailContents);
        }

		public async Task SendInvitation(string to, string name, string inviterName, string familyName, string registrationLink)
		{
			string emailContents = await GetInvitationEmailContent(to, name, inviterName, familyName, registrationLink);
			await SendEmail(emailContents);
		}

		private async Task<string> GetRegistrationEmailContent(string to, string registrationLink)
        {
            var emailContents = await _fileService.GetFileContentAsync(GetTemplatePath(_emailOptions.RegisterTemplateName));
            emailContents = emailContents.Replace("__toEmail__", to);
            emailContents = emailContents.Replace("__fromEmail__", _emailOptions.FromEmail);
			emailContents = emailContents.Replace("__confirmEmailLink__", System.Web.HttpUtility.UrlEncode(registrationLink));
            return emailContents;
        }

        private async Task<string> GetWelcomeEmailContent(string to, string name)
        {
            var emailContents = await _fileService.GetFileContentAsync(GetTemplatePath(_emailOptions.WelcomeTemplateName));
            emailContents = emailContents.Replace("__toEmail__", to);
            emailContents = emailContents.Replace("__name__", name);
            emailContents = emailContents.Replace("__fromEmail__", _emailOptions.FromEmail);
            return emailContents;
        }

		private async Task<string> GetInvitationEmailContent(string to, string name, string inviterName, string familyName, string registrationLink)
		{
			var emailContents = await _fileService.GetFileContentAsync(GetTemplatePath(_emailOptions.InvitationTempateName));
			emailContents = emailContents.Replace("__toEmail__", to);
			emailContents = emailContents.Replace("__inviterName__", inviterName);
			emailContents = emailContents.Replace("__familyName__", familyName);
			emailContents = emailContents.Replace("__name__", name);
			emailContents = emailContents.Replace("__confirmEmailLink__", System.Web.HttpUtility.UrlEncode(registrationLink));
			emailContents = emailContents.Replace("__fromEmail__", _emailOptions.FromEmail);
			return emailContents;
		}

		private void SetupHttpClient()
        {
            _httpClient.DefaultRequestHeaders.Clear();

			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_sendGridOptions.SendGridApiKey}");
        }

        private async Task SendEmail(string emailContent)
        {
            SetupHttpClient();
            var response = await _httpClient.PostAsync(_sendGridOptions.ApiUrl, new StringContent(emailContent, Encoding.UTF8, "application/json"));
        }

		private string GetTemplatePath(string templateName)
		{
			var currentDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			return Path.Combine(currentDir, EmailTemplates, templateName);
		}
	}
}
