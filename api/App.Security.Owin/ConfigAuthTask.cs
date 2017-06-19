﻿namespace App.Security.Owin
{
    using App.Common;
    using App.Common.Configurations;
    using App.Common.Extensions;
    using App.Common.Validation;
    using App.Security.Owin.Token;
    using Common.Tasks;
    using global::Owin;
    using Microsoft.Owin.Security.OAuth;
    using Security.Owin.UserNamePwd;
    using System;

    public class ConfigAuthTask : BaseTask<TaskArgument<IAppBuilder>>, IConfigAppTask<TaskArgument<IAppBuilder>>
    {
        public ConfigAuthTask() : base(App.Common.ApplicationType.All)
        {
        }

        public override void Execute(TaskArgument<IAppBuilder> arg)
        {
            if (!this.IsValid(arg.Type) || !AuthType.Owin.IsIncludedIn(Configuration.Current.Authentication.AuthType)) { return; }
            AuthType authType = Configuration.Current.Authentication.AuthType;
            switch (authType)
            {
                case AuthType.OwinTokenBase:
                    ConfigOwinTokenBase(arg.Data);
                    break;
                case AuthType.OwinBasic:
                    ConfigBasicAuth(arg.Data);
                    break;
                default:
                    throw new ValidationException("common.authentication.invalidAuthType");
            }
        }
        private void ConfigOwinTokenBase(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new Microsoft.Owin.PathString("/auth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new OwinTokenAuthorizationServerProvider()
            };
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseOAuthAuthorizationServer(options);
        }
        private void ConfigBasicAuth(IAppBuilder app)
        {
            app.Use<UserNamePwdAuthMiddleware>(new UserNamePwdAuthOptions());
        }
    }
}