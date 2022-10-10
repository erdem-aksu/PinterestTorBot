using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bogus;
using Bogus.DataSets;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PinterestTorBot.PinterestClient.Models;
using PinterestTorBot.PinterestClient.Models.Inputs;

namespace PinterestTorBot.PinterestClient.Api
{
    internal class AuthApi : IAuthApi
    {
        private PinterestApi Api { get; set; }

        private const string RegistrationCompleteExperienceId = "11:10105";

        private static readonly List<object> FirstStepActions = new List<object>()
        {
            new {name = "unauth_eu_cookie_banner_view"},
            new {name = "search_type.unknown.home"},
            new {name = "web_previously_logged_out.false"},
            new {name = "unauth_web.attempt_autologin"},
            new {name = "web.unauth.window_size"},
            new {name = "navigatorCredentials.retrieve.success.pure_react"},
            new {name = "autologin.create_google_autologin_button.number_3"},
            new {name = "unauth_web.facebook_autologin.get_login_status.attempt"},
            new {name = "autologin.google_start.pure_react"},
            new {name = "unauth.personalized_login.fetch_user_info.info.no_user_info.logged_out_cookie_false"},
            new {name = "unauth_web.facebook_autologin.get_login_status.response"},
            new {name = "autologin.facebook_attempt.pure_react"},
            new {name = "autologin.google_attempt.pure_react"},
            new {name = "autologin.google_fail"},
        };

        private static readonly List<object> SecondStepActions = new List<object>()
        {
            new {name = "homepage_button_exp.signup.click"},
            new {name = "web.unauth.modal_title.view"},
            new {name = "unauth_web_modal.home_page.undefined_tier.signup.shown"},
            new {name = "unauth.authentication_modal.shown.SMALL_TOGGLE.signup"},
            new {name = "unauth_web_container.google_one_tap_modal.open.other.shown"},
            new {name = "unauth.google_one_tap.gsi_script_loaded"},
            new {name = "unauth.google_one_tap.sdk_exists"},
            new {name = "unauth.google_one_tap.initialize"},
            new {name = "unauth.google_one_tap.display_moment.not_displayed.opt_out_or_no_session"},
            new {name = "unauth.google_one_tap.error.prompt_display_failed"},
        };

        private static readonly List<object> ThirdStepActions = new List<object>()
        {
            new {name = "unauth_email_validation_attempt"},
            new {name = "unauth.signup_step_1.completed"},
            new {name = "one_step_age_signup_complete"},
            new {name = "signup_home_page"},
            new {name = "signup.container.home_page"},
            new {name = "signup.source.homePage"},
            new {name = "signup_referrer.direct"},
            new {name = "signup_referrer_module.unauth_home_react_page"},
            new {name = "web_signup.email.success.home.home_page.direct.open"},
            new {name = "unauth.signup_one_step.completed"},
        };

        private static readonly List<object> BusinessFirstStepActions = new List<object>()
        {
            new {name = "traffic.desktop.direct.BusinessAccountCreatePage.unauth"},
            new {name = "traffic_subdomain.www.pinterest.com.direct.BusinessAccountCreatePage.unauth"},
            new {name = "traffic_subdomain.www.pinterest.com.desktop.direct.BusinessAccountCreatePage.unauth"},
            new {name = "traffic_subdomain.www.pinterest.com.desktop.unauth"},
            new {name = "unauth_eu_cookie_banner_view"},
            new {name = "create_business_account_singlestep.loaded"},
            new {name = "web_previously_logged_out.false"},
            new {name = "unauth_web.attempt_autologin"},
            new {name = "web.unauth.window_size"},
            new {name = "navigatorCredentials.retrieve.success.pure_react"},
            new {name = "autologin.create_google_autologin_button.number_3"},
            new {name = "unauth.personalized_login.fetch_user_info.info.no_user_info.logged_out_cookie_false"},
            new {name = "autologin.google_start.pure_react"},
            new {name = "unauth_web.facebook_autologin.get_login_status.attempt"},
            new {name = "unauth_web.facebook_autologin.get_login_status.response"},
            new {name = "autologin.facebook_attempt.pure_react"},
            new {name = "autologin.google_attempt.pure_react"},
            new {name = "autologin.google_fail"},
        };

        private static readonly List<object> BusinessSecondStepActions = new List<object>()
        {
            new {name = "create_business_account_singlestep.emailFieldFocused"},
            new {name = "create_business_account_singlestep.passwordFieldFocused"},
        };

        private static readonly List<object> BusinessThirdStepActions = new List<object>()
        {
            new {name = "create_business_account_singlestep.submit_clicked"},
            new {name = "signup_unknown_placement"},
            new {name = "signup.container.container_unknown"},
            new {name = "signup.source.signupSource_unknown"},
            new {name = "signup_referrer.direct"},
            new {name = "signup_referrer_module.business_account_create_page"},
            new {name = "web_signup.email.success.page_unknown.container_unknown.direct.tier_unknown"},
            new {name = "create_business_account_singlestep.signup_success"}
        };

        private static readonly List<object> BusinessFourthStepActions = new List<object>()
        {
            new {name = "web.auth.window_size"},
        };


        public AuthApi(PinterestApi api)
        {
            Api = api;
        }

        public async Task RegisterAsync(RegisterInput input)
        {
            if (string.IsNullOrEmpty(input.FirstName) || string.IsNullOrEmpty(input.Email) ||
                string.IsNullOrEmpty(input.Password))
            {
                throw new ArgumentNullException(nameof(input));
            }

            await SendRegisterActionRequest(FirstStepActions);
            await SendRegisterActionRequest(SecondStepActions);

            await Api.GetAsync(PinterestApiConstants.ResourceCheckEmail,
                new {email = input.Email});

            await Api.PostAsync(PinterestApiConstants.ResourceCreateRegister, input);

            await SendRegisterActionRequest(ThirdStepActions);

            await Api.PostAsync(PinterestApiConstants.ResourceUpdateUserSettings,
                new {full_name = input.FirstName, first_name = input.FirstName});

            await UpdateUserState("NUX_WELCOME_NAME_STEP_STATUS", 2);

            await Api.PostAsync(PinterestApiConstants.ResourceUpdateUserSettings,
                new {gender = input.Gender});
            
            await UpdateUserState("NUX_GENDER_STEP_STATUS", 2);
            
            await UpdateUserState("NUX_COUNTRY_LOCALE_STEP_STATUS", 1);

            await Api.PostAsync(PinterestApiConstants.ResourceUpdateUserSettings,
                new {country = input.Country, locale = input.Locale});
            
            await UpdateUserState("NUX_COUNTRY_LOCALE_STEP_STATUS", 2);
            
            await UpdateUserState("NUX_TOPIC_PICKER_STEP_STATUS", 2);
            
            await Api.PostAsync(PinterestApiConstants.ResourceRegistrationComplete,
                new {placed_experience_id = RegistrationCompleteExperienceId});
            
            await UpdateUserState("NUX_LOADING_STEP_STATUS", 2);
        }

        public async Task RegisterBusinessAsync(BusinessRegisterInput input, BusinessRegisterSecondInput secondInput)
        {
            if (string.IsNullOrEmpty(input.Email) || string.IsNullOrEmpty(input.Password))
            {
                throw new ArgumentNullException(nameof(input));
            }

            await SendBusinessRegisterActionRequest(BusinessFirstStepActions);
            await SendBusinessRegisterActionRequest(BusinessSecondStepActions);

            await Api.GetAsync(PinterestApiConstants.ResourceCheckEmail,
                new {email = input.Email});

            input.BusinessName = input.Email.Split('@').First();

            await Api.PostAsync(PinterestApiConstants.ResourceCreateRegister, input);


            await SendBusinessRegisterActionRequest(BusinessThirdStepActions);
            await SendBusinessRegisterActionRequest();

            await UpdateUserState("NUX_COUNTRY_LOCALE_STEP_STATUS", 1);

            await Api.PostAsync(PinterestApiConstants.ResourceUpdateUserSettings,
                new {country = secondInput.Country, locale = secondInput.Locale});

            await UpdateUserState("NUX_COUNTRY_LOCALE_STEP_STATUS", 2);

            await Api.PostAsync(PinterestApiConstants.ResourceUpdateUserSettings,
                new {account_type = "not_sure", business_name = secondInput.BusinessName});

            await Api.PostAsync(PinterestApiConstants.ResourceUpdateUserSettings,
                new {advertising_intent = 3});

            await Api.PostAsync(PinterestApiConstants.ResourceRegistrationComplete,
                new {placed_experience_id = RegistrationCompleteExperienceId});
        }

        public async Task<AutoRegisteredAccount> AutoRegisterAsync(string country = "US", string locale = "en-US")
        {
            var faker = new Faker();

            var gender = faker.Person.Gender;
            var name = faker.Name.FirstName(gender);
            var surname = faker.Name.LastName(gender);
            var username =
                (name.ToLowerInvariant() + "_" + surname.ToLowerInvariant() + faker.Random.Number(99)).Replace(" ",
                    string.Empty);
            var email = faker.Internet.Email(name.ToLowerInvariant(), surname.ToLowerInvariant());
            var password = faker.Internet.Password(16);
            var businessName = $"{name} {surname}";
            var avatarUrl = faker.Internet.Avatar();

            await RegisterBusinessAsync(
                new BusinessRegisterInput
                {
                    Email = email,
                    Password = password,
                    BusinessName = businessName
                },
                new BusinessRegisterSecondInput
                {
                    BusinessName = businessName,
                    Country = country,
                    Locale = locale
                });

            byte[] avatar;

            using (var webClient = new WebClient())
            {
                avatar = webClient.DownloadData(faker.Internet.Avatar());
            }


            await Api.PostAsync(PinterestApiConstants.ResourceUpdateUserSettings, new ProfileInput()
            {
                About = faker.Name.JobTitle(),
                Gender = gender == Name.Gender.Male ? "male" : "female",
                Username = username
            });

            return new AutoRegisteredAccount
            {
                Gender = gender == Name.Gender.Male ? "male" : "female",
                Name = name,
                Surname = surname,
                UserName = username,
                Email = email,
                Password = password,
                BusinessName = businessName
            };
        }

        public async Task<AutoRegisteredAccount> AutoRegisterPersonalAsync(string country = "US", string locale = "en-US")
        {
            var faker = new Faker();

            var gender = faker.Person.Gender;
            var name = faker.Name.FirstName(gender);
            var surname = faker.Name.LastName(gender);
            var username =
                (name.ToLowerInvariant() + "_" + surname.ToLowerInvariant() + faker.Random.Number(99)).Replace(" ",
                    string.Empty);
            var email = faker.Internet.Email(name.ToLowerInvariant(), surname.ToLowerInvariant());
            var password = faker.Internet.Password(16);
            var businessName = $"{name} {surname}";
            var avatarUrl = faker.Internet.Avatar();

            await RegisterAsync(new RegisterInput
            {
                Email = email,
                Password = password,
                FirstName = businessName,
                Country = country,
                Locale = locale,
                Gender = gender == Name.Gender.Male ? "male" : "female"
            });

            await Api.PostAsync(PinterestApiConstants.ResourceUpdateUserSettings, new ProfileInput()
            {
                About = faker.Name.JobTitle(),
                Username = username,
                FirstName = name,
                LastName = surname
            });

            return new AutoRegisteredAccount
            {
                Gender = gender == Name.Gender.Male ? "male" : "female",
                Name = name,
                Surname = surname,
                UserName = username,
                Email = email,
                Password = password,
                BusinessName = businessName
            };
        }

        public async Task ConvertToBusinessAsync(string businessName, string websiteUrl, bool login = true)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceConvertToBusiness,
                new
                {
                    business_name = businessName,
                    website_url = websiteUrl,
                    account_type = "other"
                }, login);
        }

        public async Task ConfirmEmailAsync(string link)
        {
            var uri = new Uri(link);

            if (uri.Host != "post.pinterest.com")
            {
                throw new ArgumentException("Url is invalid.", nameof(link));
            }

            await Api.GetAsync(uri.ToString());
        }

        public async Task Login()
        {
            await Api.Login();
        }

        public void Logout()
        {
            Api.Logout();
        }

        private async Task UpdateUserState(string state, int value)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceUserState, new {state, value});
        }

        private async Task GetUserState(string state)
        {
            await Api.GetAsync(PinterestApiConstants.ResourceUserStateGet, new {state});
        }

        private async Task SendRegisterActionRequest(List<object> actions = null)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceUpdateRegistrationTrack,
                new {actions = actions ?? new List<object>()},
                serializerSettings: new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver()
                });
        }

        private async Task SendBusinessRegisterActionRequest(List<object> actions = null)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceUpdateRegistrationTrack,
                new {actions = actions ?? new List<object>()},
                serializerSettings: new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver()
                });
        }
    }
}