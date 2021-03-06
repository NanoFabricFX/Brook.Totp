﻿using System;
using System.Net;
using System.Net.Http;
using Brook.Totp.Helper;
using Brook.Totp.Interface;
using Brook.Totp.Models;
using QRCoder;

namespace Brook.Totp
{
    public class TotpSetupGenerator : ITotpSetupGenerator
    {
        /// <summary>
        ///     生成一个bbbase64二维码图片 以便Authenticator 扫描添加
        /// </summary>
        /// <param name="issuer">应用程序或者公司名称 英文 中文可能无法显示.</param>
        /// <param name="accountIdentity">
        ///     用户名或者有效可以显示到Authenticator中
        /// </param>
        /// <param name="accountSecretKey">
        ///     密钥最好每个人都是不同的随机生成或者GUID，校验TOTP时需传入
        /// </param>
        /// <param name="pixelsPerModule">二维码每个点的像素大小默认5.</param>
        /// <returns>返回手动设置key和二维码base64.</returns>
        public TotpSetup GenerateBase64(string issuer, string accountIdentity, string accountSecretKey, int pixelsPerModule = 5)
        {
            return GenerateTopSetup(issuer, accountIdentity, accountSecretKey, pixelsPerModule, true);
        }


        /// <summary>
        ///     生成一个二维码内容，适合前端自己生成二维码 以便Authenticator 扫描添加
        /// </summary>
        /// <param name="issuer">应用程序或者公司名称 英文 中文可能无法显示.</param>
        /// <param name="accountIdentity">
        ///     用户名或者有效可以显示到Authenticator中
        /// </param>
        /// <param name="accountSecretKey">
        ///     密钥最好每个人都是不同的随机生成或者GUID，校验TOTP时需传入
        /// </param>
        /// <returns>返回手动设置key和二维码内容 适合前端自己生成二维码.</returns>
        public TotpSetup GenerateUrl(string issuer, string accountIdentity, string accountSecretKey)
        {
            return GenerateTopSetup(issuer, accountIdentity, accountSecretKey, isBa64: false);
        }

        /// <summary>
        ///     生成一个bbbase64二维码图片 以便Authenticator 扫描添加
        /// </summary>
        /// <param name="issuer">应用程序或者公司名称 英文 中文可能无法显示.</param>
        /// <param name="accountIdentity">
        ///     用户名或者有效可以显示到Authenticator中
        /// </param>
        /// <param name="accountSecretKey">
        ///     密钥最好每个人都是不同的随机生成或者GUID，校验TOTP时需传入
        /// </param>
        /// <param name="pixelsPerModule">二维码每个点的像素大小默认5.</param>
        /// <param name="isBa64">是否生成base64</param>
        /// <returns>返回手动设置key和二维码base64获取内容 依据isBa64.</returns>
        private TotpSetup GenerateTopSetup(string issuer, string accountIdentity, string accountSecretKey, int pixelsPerModule = 5, bool isBa64 = false)
        {
            Guard.NotNull(issuer);
            Guard.NotNull(accountIdentity);
            Guard.NotNull(accountSecretKey);

            accountIdentity = accountIdentity.Replace(" ", "");
            var encodedSecretKey = Base32.Encode(accountSecretKey);
            var provisionUrl = $"otpauth://totp/{accountIdentity}?secret={encodedSecretKey}&issuer={issuer}";

            var totpSetup = new TotpSetup
            {
                QrCodeImageBase64 = isBa64 ? GetQrBase64Imageg(provisionUrl, pixelsPerModule) : null,
                QrCodeImageContent = !isBa64 ? provisionUrl : null,
                ManualSetupKey = encodedSecretKey
            };

            return totpSetup;
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="provisionUrl"></param>
        /// <param name=""></param>
        /// <returns></returns>
        private string GetQrBase64Imageg(string provisionUrl,int pixelsPerModule)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(provisionUrl, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            string qrCodeImageAsBase64 = qrCode.GetGraphic(2);

            return  $"data:image/png;base64,{qrCodeImageAsBase64}";
        }
    }
}