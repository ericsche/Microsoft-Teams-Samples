﻿using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.WebApi;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Teams.TemplateBotCSharp.Bot;
using Microsoft.Teams.TemplateBotCSharp.Bots;
using Microsoft.Teams.TemplateBotCSharp.Dialogs;
using System.Reflection;
using System.Web.Http;

namespace Microsoft.Teams.TemplateBotCSharp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(BotConfig.Register);
        }

        // Setting Bot data store policy to use last write win
        // Example if bot service got restarted, existing conversation would just overwrite data to store
        public static class BotConfig
        {
            public static void Register(HttpConfiguration config)
            {
                var builder = new ContainerBuilder();
                builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

                // The ConfigurationCredentialProvider will retrieve the MicrosoftAppId and
                // MicrosoftAppPassword from Web.config
                builder.RegisterType<ConfigurationCredentialProvider>().As<ICredentialProvider>().SingleInstance();

                // Create the Bot Framework Adapter with error handling enabled.
                builder.RegisterType<AdapterWithErrorHandler>().As<IBotFrameworkHttpAdapter>().SingleInstance();

                // The Memory Storage used here is for local bot debugging only. When the bot
                // is restarted, everything stored in memory will be gone.
                IStorage dataStore = new MemoryStorage();

                // Create Conversation State object.
                // The Conversation State object is where we persist anything at the conversation-scope.
                var conversationState = new ConversationState(dataStore);
                var userState = new UserState(dataStore);
                var privateConversationData = new PrivateConversationState(dataStore);
                builder.RegisterInstance(conversationState).As<ConversationState>().SingleInstance();
                builder.RegisterInstance(userState).As<UserState>().SingleInstance();
                builder.RegisterInstance(privateConversationData).As<PrivateConversationState>().SingleInstance();

                // Register the main dialog, which is injected into the DialogBot class
                builder.RegisterType<RootDialog>().SingleInstance();

                // Register the DialogBot with RootDialog as the IBot interface
                builder.RegisterType<DialogBot<RootDialog>>().As<IBot>();

                var container = builder.Build();
                var resolver = new AutofacWebApiDependencyResolver(container);
                config.DependencyResolver = resolver;
            }
        }
    }
}
