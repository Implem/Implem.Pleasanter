using Fido2NetLib;
using Fido2NetLib.Objects;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    public class PasskeysController(IFido2 fido2) : Controller
    {
        private const string Fido2AttestationOptions = "fido2.attestationOptions";
        private const string Fido2AssertionOptions = "fido2.assertionOptions";

        [HttpPost]
        public string Passkeys()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = PasskeyUtilities.Passkeys(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string MakeCredentialOptions()
        {
            var context = new Context();
            if (!Parameters.Authentication.PasskeyParameters.Enabled)
            {
                return Messages.ResponseNotFound(context: context)
                    .ToJson();
            }
            var log = new SysLogModel(context: context);
            var ss = SiteSettingsUtilities.UsersSiteSettings(context: context);
            var passkeys = new PasskeyCollection(
                context: context,
                column: Rds.PasskeysDefaultColumns(),
                where: Rds.PasskeysWhere()
                        .UserId(context.UserId));
            var existingKeys = passkeys.Select(pk =>
            {
                var key = new PublicKeyCredentialDescriptor(
                    type: PublicKeyCredentialType.PublicKey,
                    id: pk.CredentialId.FromBase64UrlString(),
                    transports: pk.PasskeyData.Transports);
                return key;
            }).ToList();
            var attType = AttestationConveyancePreference.None;
            var residentKey = ResidentKeyRequirement.Required;
            var userVerification = UserVerificationRequirement.Preferred;
            var authenticatorSelection = new AuthenticatorSelection
            {
                ResidentKey = residentKey,
                UserVerification = userVerification
            };
            var exts = new AuthenticationExtensionsClientInputs()
            {
                Extensions = true,
                UserVerificationMethod = true,
                CredProps = true
            };
            var json = string.Empty;
            try
            {
                var fido2User = new Fido2User
                {
                    Id = System.Text.Encoding.UTF8.GetBytes(context.UserId.ToString()),
                    Name = context.LoginId,
                    DisplayName = context?.User?.Name ?? string.Empty
                };
                var options = fido2.RequestNewCredential(new RequestNewCredentialParams
                {
                    User = fido2User,
                    ExcludeCredentials = existingKeys,
                    AuthenticatorSelection = authenticatorSelection,
                    AttestationPreference = attType,
                    Extensions = exts
                });
                HttpContext.Session.SetString(Fido2AttestationOptions, options.ToJson());
                json = new ResponseCollection()
                    .Invoke(
                    methodName: "passkeyRegister",
                    args: options.ToJson())
                        .ToJson();
            }
            catch (System.Exception ex)
            {
                json = Messages.ResponseInternalServerError(context: context)
                    .ToJson();
                _ = new SysLogModel(
                    context: context,
                    e: ex);
            }
            finally
            {
                log.Finish(context: context, responseSize: json.Length);
            }
            return json;
        }

        [HttpPost]
        public async Task<string> MakeCredential()
        {
            var context = new Context();
            if (!Parameters.Authentication.PasskeyParameters.Enabled)
            {
                return Messages.ResponseNotFound(context: context)
                    .ToJson();
            }
            var log = new SysLogModel(context: context);
            var ss = SiteSettingsUtilities.UsersSiteSettings(context: context);
            var json = string.Empty;

            try
            {
                var bodyData = JsonSerializer.Deserialize<AuthenticatorAttestationRawResponse>(context.Forms.Data("data"));
                var optionsJson = HttpContext.Session.GetString(Fido2AttestationOptions);
                var options = CredentialCreateOptions.FromJson(optionsJson);
                var passkeys = new PasskeyCollection(
                    context: context,
                    column: Rds.PasskeysDefaultColumns(),
                    where: Rds.PasskeysWhere()
                        .UserId(context.UserId));
                async Task<bool> IsCredentialIdUniqueToUserCallback(IsCredentialIdUniqueToUserParams args, CancellationToken cancellationToken)
                {
                    return !passkeys.Any(p => p.CredentialId == args.CredentialId.ToBase64UrlString());
                }
                var credential = await fido2.MakeNewCredentialAsync(new MakeNewCredentialParams
                {
                    AttestationResponse = bodyData,
                    OriginalOptions = options,
                    IsCredentialIdUniqueToUserCallback = IsCredentialIdUniqueToUserCallback
                });
                var passkeyModel = new PasskeyModel(
                    context: context,
                    userId: context.UserId)
                {
                    CredentialId = credential.Id.ToBase64UrlString(),
                    Title = Displays.PasskeyDefaultTitle(context, context.LoginId, (passkeys.Count + 1).ToString()),
                    PasskeyData = new PasskeyData
                    {
                        PublicKey = credential.PublicKey,
                        SignCount = credential.SignCount,
                        AttestationFormat = credential.AttestationFormat,
                        AaGuid = credential.AaGuid,
                        Transports = credential.Transports,
                        IsBackupEligible = credential.IsBackupEligible,
                        IsBackedUp = credential.IsBackedUp,
                        AttestationObject = credential.AttestationObject,
                        AttestationClientDataJson = credential.AttestationClientDataJson
                    }
                };
                var errorData = passkeyModel.Create(context, ss);
                if (errorData.Type != Error.Types.None)
                {
                    return new ResponseCollection()
                        .Message(errorData.Message(context))
                        .ToJson();
                }
                json = new ResponseCollection()
                    .Html(target: "#EditPasskeyWrap",
                        value: new HtmlBuilder()
                            .EditPasskey(context: context))
                    .Invoke("loaded")
                    .Message(Messages.Created(context: context, [passkeyModel.Title]))
                    .ToJson();
                log.Finish(context: context, responseSize: json.Length);
            }
            catch (System.Exception ex)
            {
                json = Messages.ResponseInternalServerError(context: context)
                    .ToJson();
                _ = new SysLogModel(
                    context: context,
                    e: ex);
            }
            finally
            {
                log.Finish(context: context, responseSize: json.Length);
            }
            return json;
        }

        [AllowAnonymous]
        [HttpPost]
        public string GetAssertionOptions()
        {
            var context = new Context();
            if (!Parameters.Authentication.PasskeyParameters.Enabled)
            {
                return Messages.ResponseNotFound(context: context)
                    .ToJson();
            }
            var log = new SysLogModel(context: context);
            List<PublicKeyCredentialDescriptor> existingCredentials = [];
            var exts = new AuthenticationExtensionsClientInputs()
            {
                Extensions = true,
                UserVerificationMethod = true
            };
            var uv = UserVerificationRequirement.Discouraged;
            var json = string.Empty;
            try
            {
                var options = fido2.GetAssertionOptions(new GetAssertionOptionsParams()
                {
                    AllowedCredentials = existingCredentials,
                    UserVerification = uv,
                    Extensions = exts
                });
                HttpContext.Session.SetString(Fido2AssertionOptions, options.ToJson());
                json = new ResponseCollection()
                    .Invoke(
                    methodName: "passkeyLogin",
                    args: options.ToJson())
                        .ToJson();
            }
            catch (System.Exception ex)
            {
                json = Messages.ResponseInternalServerError(context: context)
                    .ToJson();
                _ = new SysLogModel(
                    context: context,
                    e: ex);
            }
            finally
            {
                log.Finish(context: context, responseSize: json.Length);
            }
            return json;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<string> MakeAssertion(string returnUrl = "")
        {
            var context = new Context();
            if (!Parameters.Authentication.PasskeyParameters.Enabled)
            {
                return Messages.ResponseNotFound(context: context)
                    .ToJson();
            }
            var log = new SysLogModel(context: context);
            var json = string.Empty;
            try
            {
                var ss = SiteSettingsUtilities.UsersSiteSettings(context: context);
                var clientResponse = JsonSerializer.Deserialize<AuthenticatorAssertionRawResponse>(context.Forms.Data("data"));
                var jsonOptions = HttpContext.Session.GetString(Fido2AssertionOptions);
                var options = AssertionOptions.FromJson(jsonOptions);
                var passkeys = new PasskeyCollection(
                    context: context,
                    column: Rds.PasskeysDefaultColumns(),
                    where: Rds.PasskeysWhere()
                        .CredentialId(clientResponse.Id));
                var passkey = passkeys.FirstOrDefault();
                if (passkey == null)
                {
                    return Messages.ResponseIncorrectUser(context: context, data: [Displays.NotFound(context)])
                        .ToJson();
                }
                var storedCounter = passkey?.PasskeyData?.SignCount ?? 0;
                async Task<bool> IsUserHandleOwnerOfCredentialIdCallback(IsUserHandleOwnerOfCredentialIdParams args, CancellationToken cancellationToken)
                {
                    var userId = System.Text.Encoding.UTF8.GetString(args.UserHandle);
                    var credentialId = args.CredentialId.ToBase64UrlString();
                    return passkey?.UserId.ToString() == userId && passkey?.CredentialId == credentialId;
                }
                var res = await fido2.MakeAssertionAsync(new MakeAssertionParams
                {
                    AssertionResponse = clientResponse,
                    OriginalOptions = options,
                    StoredPublicKey = passkey.PasskeyData.PublicKey,
                    StoredSignatureCounter = storedCounter,
                    IsUserHandleOwnerOfCredentialIdCallback = IsUserHandleOwnerOfCredentialIdCallback
                });
                passkey.PasskeyData.SignCount = res.SignCount;
                _ = new PasskeyModel(
                    context: context,
                    passkeyId: passkey.PasskeyId)
                {
                    PasskeyData = passkey.PasskeyData
                }.Update(context, ss);
                json = new UserModel(
                    context: context,
                    ss: ss,
                    userId: passkey.UserId)
                        .Get(
                            context: context,
                            ss: ss,
                            where: Rds.UsersWhere()
                                .UserId(passkey.UserId))
                        .Authenticate(
                            context: context,
                            returnUrl: returnUrl,
                            isAuthenticationByMail: false,
                            isAuthenticationByPasskey: true);
            }
            catch (System.Exception ex)
            {
                json = Messages.ResponseInternalServerError(context: context)
                    .ToJson();
                _ = new SysLogModel(
                    context: context,
                    e: ex);
            }
            finally
            {
                log.Finish(context: context, responseSize: json.Length);
            }
            return json;
        }

        [HttpPost]
        public string ChangeTitleDialog()
        {
            var context = new Context();
            if (!Parameters.Authentication.PasskeyParameters.Enabled)
            {
                return Messages.ResponseNotFound(context: context)
                    .ToJson();
            }
            var log = new SysLogModel(context: context);
            var json = string.Empty;
            try
            {
                var passkeyId = context.Forms.Data("passkeyId").ToInt();
                var passkey = new PasskeyModel(
                    context: context,
                    passkeyId: passkeyId);
                if (passkey?.UserId != context.UserId)
                {
                    return Messages.ResponseNotFound(context: context)
                         .ToJson();
                }
                json = new ResponseCollection()
                    .Html(
                        target: "#PasskeyChangeTitleDialog",
                        value: new HtmlBuilder()
                            .PasskeyChangeTitleForm(
                                context: context,
                                passkeyId: passkey.PasskeyId,
                                title: passkey.Title))
                    .ToJson();
            }
            catch (System.Exception ex)
            {
                json = Messages.ResponseInternalServerError(context: context)
                    .ToJson();
                _ = new SysLogModel(
                    context: context,
                    e: ex);
            }
            finally
            {
                log.Finish(context: context, responseSize: json.Length);
            }
            return json;
        }

        [HttpPost]
        public string ChangeTitle(int id)
        {
            var context = new Context();
            if (!Parameters.Authentication.PasskeyParameters.Enabled)
            {
                return Messages.ResponseNotFound(context: context)
                    .ToJson();
            }
            var log = new SysLogModel(context: context);
            var json = string.Empty;
            try
            {
                var title = context.Forms.Data(nameof(Displays.Passkeys_Title));
                var ss = SiteSettingsUtilities.UsersSiteSettings(context: context);
                var passkey = new PasskeyModel(
                    context: context,
                    passkeyId: id);
                if (passkey.UserId != context.UserId)
                {
                    return Messages.ResponseNotFound(context: context)
                        .ToJson();
                }
                passkey.Title = string.IsNullOrEmpty(title) ? passkey.Title : title;
                var errorData = passkey.Update(context, ss);
                if (errorData.Type != Error.Types.None)
                {
                    return new ResponseCollection()
                        .Message(errorData.Message(context))
                        .ToJson();
                }
                json = new ResponseCollection()
                    .Html(target: "#EditPasskeyWrap",
                        value: new HtmlBuilder()
                            .EditPasskey(context: context))
                    .Invoke("closePasskeyChangeTitleDialog")
                    .Message(Messages.Updated(context: context, [passkey.Title]))
                    .ToJson();
            }
            catch (System.Exception ex)
            {
                json = Messages.ResponseInternalServerError(context: context)
                    .ToJson();
                _ = new SysLogModel(
                    context: context,
                    e: ex);
            }
            finally
            {
                log.Finish(context: context, responseSize: json.Length);
            }
            return json;
        }

        public string Delete(int id)
        {
            var context = new Context();
            if (!Parameters.Authentication.PasskeyParameters.Enabled)
            {
                return Messages.ResponseNotFound(context: context)
                    .ToJson();
            }
            var log = new SysLogModel(context: context);
            var json = string.Empty;
            try
            {
                var passkey = new PasskeyModel(
                context: context,
                passkeyId: id);
                if (passkey.UserId != context.UserId)
                {
                    return Messages.ResponseNotFound(context: context)
                        .ToJson();
                }
                var errorData = passkey.PhysicalDelete(context);
                if (errorData.Type != Error.Types.None)
                {
                    return new ResponseCollection()
                        .Message(errorData.Message(context))
                        .ToJson();
                }
                json = new ResponseCollection()
                    .Html(target: "#EditPasskeyWrap",
                        value: new HtmlBuilder()
                            .EditPasskey(context: context))
                    .Invoke("closePasskeyChangeTitleDialog")
                    .Message(Messages.Deleted(context: context, [passkey.Title]))
                    .ToJson();
            }
            catch (System.Exception ex)
            {
                json = Messages.ResponseInternalServerError(context: context)
                    .ToJson();
                _ = new SysLogModel(
                    context: context,
                    e: ex);
            }
            finally
            {
                log.Finish(context: context, responseSize: json.Length);
            }
            return json;
        }

        public string DeleteBulk()
        {
            var context = new Context();
            if (!Parameters.Authentication.PasskeyParameters.Enabled)
            {
                return Messages.ResponseNotFound(context: context)
                    .ToJson();
            }
            var log = new SysLogModel(context: context);
            var json = string.Empty;
            try
            {
                var ids = context.Forms.IntList("EditPasskey");
                if (ids.Count <= 0)
                {
                    return Messages.ResponseSelectTargets(context: context).ToJson();
                }
                var count = Repository.ExecuteNonQuery(
                    context: context,
                    statements:
                    [
                        Rds.PhysicalDeletePasskeys(
                    tableType: Implem.Libraries.DataSources.SqlServer.Sqls.TableTypes.Normal,
                    where: Rds.PasskeysWhere()
                        .UserId(context.UserId)
                        .PasskeyId_In(ids))
                    ]);
                json = new ResponseCollection()
                    .Html(target: "#EditPasskeyWrap",
                        value: new HtmlBuilder()
                            .EditPasskey(context: context))
                    .Message(Messages.BulkDeleted(context: context, Displays.Passkey(context), count.ToString()))
                    .ToJson();
            }
            catch (System.Exception ex)
            {
                json = Messages.ResponseInternalServerError(context: context)
                    .ToJson();
                _ = new SysLogModel(
                    context: context,
                    e: ex);
            }
            finally
            {
                log.Finish(context: context, responseSize: json.Length);
            }
            return json;
        }
    }
}
