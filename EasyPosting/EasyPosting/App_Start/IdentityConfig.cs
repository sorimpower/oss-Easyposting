using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using EasyPosting.Models;

namespace EasyPosting
{
    // 이 응용 프로그램에서 사용되는 응용 프로그램 사용자 관리자를 구성합니다. UserManager는 ASP.NET Identity에서 정의하며 응용 프로그램에서 사용됩니다.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
  
          public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // 사용자 이름에 대한 유효성 검사 논리 구성
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // 암호에 대한 유효성 검사 논리 구성
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // 2단계 인증 공급자를 등록합니다. 이 응용 프로그램은 사용자 확인 코드를 받는 단계에서 전화 및 전자 메일을 사용합니다.
            // 공급자 및 플러그 인을 여기에 쓸 수 있습니다.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "보안 코드",
                BodyFormat = "Your security code is: {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            //// 전자 메일을 보낼 전자 메일 서비스를 여기에 플러그 인으로 추가합니다.
            //// Plug in your email service here to send an email.
            //var credentialUserName = "info@ourdoamin.com";
            //var sentFrom = "noreply@ourdoamin.com";
            //var pwd = "ourpassword";

            //// Configure the client:
            //System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("mail.ourdoamin.com");

            //client.Port = 25;
            //client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;

            //// Creatte the credentials:
            //System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(credentialUserName, pwd);
            //client.EnableSsl = true;
            //client.Credentials = credentials;

            //// Create the message:
            //var mail = new System.Net.Mail.MailMessage(sentFrom, message.Destination);
            //mail.Subject = message.Subject;
            //mail.Body = message.Body;

            //return Task.FromResult(mail);
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // 텍스트 메시지를 보낼 SMS 서비스를 여기에 플러그 인으로 추가합니다.
            return Task.FromResult(0);
        }
    }
}
