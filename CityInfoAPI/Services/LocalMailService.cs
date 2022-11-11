﻿namespace CityInfoAPI.Services;

public class LocalMailService : IMailService
{
    private readonly string _mailTo = string.Empty;
    private readonly string _mailFrom = string.Empty;

    public LocalMailService(IConfiguration config)
    {
        _mailTo = config["mailSettings:mailToAddress"];
        _mailFrom = config["mailSettings:mailFromAddress"];
    }
    public void Send(string subject, string message)
    {
        //send mail - output to console window
        Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, " +
                          $"with {nameof(LocalMailService)}.");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");
    }
}