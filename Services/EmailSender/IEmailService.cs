﻿namespace WebRegistry.Services.EmailSender
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string email, string subject, string message);
    }
}
