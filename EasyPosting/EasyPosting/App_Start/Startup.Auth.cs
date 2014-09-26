using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Google;
using Owin;
using System;
using EasyPosting.Models;

namespace EasyPosting
{
    public partial class Startup
    {
        // 인증 구성에 대한 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=301864를 참조하십시오.
        public void ConfigureAuth(IAppBuilder app)
        {
            // 요청당 단일 인스턴스를 사용하도록 db 컨텍스트와 사용자 관리자 구성합니다.
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // 응용 프로그램이 쿠키를 사용하여 로그인한 사용자에 대한 정보를 저장하도록 설정합니다.
            // 또한 쿠키를 사용하여 타사 로그인 공급자를 통한 사용자 로그인 관련 정보를 일시적으로 저장하도록 설정합니다.
            // 쿠키에 서명을 구성합니다.
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // 타사 로그인 공급자로 로그인할 수 있으려면 다음 줄의 주석 처리를 제거합니다.
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                //ClientId = "713812983827-ci9bvg070f6uprhsdqcdotc76uu0mvmb.apps.googleusercontent.com",
                ClientId = "969021499602-cbi2p9ou8o9905aipbithg0o8dp1ucpn.apps.googleusercontent.com",
                //ClientSecret = "bM8yQKGK76T2OFSxQTHEomdk"
                ClientSecret = "oGD4U9y-Ua72tOV_1AWtWXoT"
            });
        }
    }
}