﻿using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFramework;
using TelegramBotFramework.Extensions;
using TelegramBotFramework.Handling;
using TelegramBotFramework.Pipeline;
using TelegramBotFramework.Settings;
using TelegramBotFramework.UsageExample.Services;
using Results = TelegramBotFramework.Results.Results;

// TODO: Group filtering.
// TODO: Generic typed filters.
// TODO: More Result templates.
// TODO: More efficient handler resolving.
// TODO: Different state models.
// TODO: Notifications.
// TODO: Bot info after startup.
// TODO: Different persistence and builder options in builders (Locale Service, State Machine, Notification Service).

var builder = BotApplication.CreateBuilder(new BotApplicationBuilderOptions
{
    Args = args,
    ReceiverOptions = new ReceiverOptions
    {
        AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery,],
        ThrowPendingUpdates = true,
    },
});

builder.HostBuilder.Services.AddScoped<WeatherService>();

builder.SetTokenFromConfiguration("BotToken");

var app = builder.Build();

app.UseCallbackAutoAnswering();

var group = app.HandleGroup(x => x.Update.Type == UpdateType.Message);

group.Handle(() => Results.MessageReply("I'm replied from group handler!"));

app.Handle(() =>
{
    var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Send photo!", "Photo"));
    return Results.MessageEdit("Button pressed", keyboard);
}).FilterCallbackData(x => x == "Hello");

app.Handle(() => Results.Photo("cat.jpeg", "Little cat")).FilterCallbackData(x => x == "Photo");

app.Handle(() => Results.MessageReply("I'm replied!"))
    .FilterText(x => x.Equals("reply", StringComparison.OrdinalIgnoreCase));

app.Handle(async (string messageText, WeatherService weatherService) =>
{
    var weather = await weatherService.GetWeather();
    var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Hello", "Hello"));
    return ($"Hello, {messageText}, weather is {weather}", keyboard);
}).FilterUpdateType(x => x == UpdateType.Message);

app.Run();