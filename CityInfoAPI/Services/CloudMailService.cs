﻿namespace CityInfoAPI.Services;

public class CloudMailService : IMailService
{
    private readonly string _mailTo = string.Empty;
    private readonly string _mailFrom = string.Empty;

    public CloudMailService(IConfiguration config)
    {
        _mailTo = config["mailSettings:mailToAddress"];
        _mailFrom = config["mailSettings:mailFromAddress"];
    }

    public void Send(string subject, string message)
    {
        //send mail - output to console window
        Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, " +
                          $"with {nameof(CloudMailService)}.");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");
    }
}