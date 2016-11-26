using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;

namespace OAuth.Tools
{
    /// <summary>
    /// ��Ȩ����
    /// </summary>
    public abstract class OAuth2Base
    {
        protected WebClient wc = new WebClient();
        public OAuth2Base()
        {
            wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            wc.Headers.Add("Pragma", "no-cache");
        }

        #region ��������
        /// <summary>
        /// ���صĿ���ID��
        /// </summary>
        public string openID = string.Empty;
        /// <summary>
        /// ���ʵ�Token
        /// </summary>
        public string token = string.Empty;
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime expiresTime;

        /// <summary>
        /// �������˺��ǳ�
        /// </summary>
        public string nickName = string.Empty;

        /// <summary>
        /// �������˺�ͷ���ַ
        /// </summary>
        public string headUrl = string.Empty;
        /// <summary>
        /// �״�����ʱ���ص�Code
        /// </summary>
        internal string code = string.Empty;
        internal abstract OAuthServer server
        {
            get;
        }

        #endregion

        #region �ǹ���������·����LogoͼƬ��ַ��

        internal abstract string OAuthUrl
        {
            get;
        }
        internal abstract string TokenUrl
        {
            get;
        }
        internal abstract string ImgUrl
        {
            get;
        }
        #endregion

        #region WebConfig��Ӧ�����á�AppKey��AppSercet��CallbackUrl��
        internal string AppKey
        {
            get
            {
                return Tool.GetConfig(server.ToString() + ".AppKey");
            }
        }
        internal string AppSercet
        {
            get
            {
                return Tool.GetConfig(server.ToString() + ".AppSercet");
            }
        }
        internal string CallbackUrl
        {
            get
            {
                return Tool.GetConfig(server.ToString() + ".CallbackUrl");
            }
        }
        #endregion

        #region ��������

        /// <summary>
        /// ���Token
        /// </summary>
        /// <returns></returns>
        protected string GetToken(string method)
        {
            string result = string.Empty;
            try
            {
                string para = "grant_type=authorization_code&client_id=" + AppKey + "&client_secret=" + AppSercet + "&code=" + code + "&state=" + server;
                para += "&redirect_uri=" + HttpUtility.UrlEncode(CallbackUrl) + "&rnd=" + DateTime.Now.Second;
                if (method == "POST")
                {
                    if (string.IsNullOrEmpty(wc.Headers["Content-Type"]))
                    {
                        wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    }
                    result = wc.UploadString(TokenUrl, method, para);
                }
                else
                {
                    result = wc.DownloadString(TokenUrl + "?" + para);
                }
            }
            catch (Exception err)
            {
                throw err;
            }
            return result;
        }

        /// <summary>
        /// ��ȡ�Ƿ�ͨ����Ȩ��
        /// </summary>
        public abstract bool Authorize();
        /// <param name="bindAccount">���ذ󶨵��˺ţ���δ�󶨷��ؿգ�</param>
        public bool Authorize(out OAuth2Account account)
        {
            account = null;
            if (Authorize())
            {
                account = GetBindAccount();
                return true;
            }
            return false;
        }

        #endregion

        #region �������˺�

        /// <summary>
        /// ��ȡ�Ѿ��󶨵��˺�
        /// </summary>
        /// <returns></returns>
        public OAuth2Account GetBindAccount()
        {
            OAuth2Account oa = new OAuth2Account();
            oa.Token = token;
            oa.ExpireTime = expiresTime;
            oa.NickName = nickName;
            oa.HeadUrl = headUrl;
            oa.BindAccount="";
            return oa;
        }

        /// <summary>
        /// ��Ӱ��˺�
        /// </summary>
        /// <param name="bindAccount"></param>
        /// <returns></returns>
        public bool SetBindAccount(string bindAccount)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(openID) && !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(bindAccount))
            {
                OAuth2Account oa = new OAuth2Account();
                oa.OAuthServer = server.ToString();
                oa.Token = token;
                oa.OpenID = openID;
                oa.ExpireTime = expiresTime;
                oa.BindAccount = bindAccount;
                oa.NickName = nickName;
                oa.HeadUrl = headUrl;
                result = true;//oa.Insert ִ����Ӳ���
            }
            return result;
        }
        #endregion
    }

    /// <summary>
    /// �ṩ��Ȩ�ķ�����
    /// </summary>
    public enum OAuthServer
    {
        /// <summary>
        /// ����΢��
        /// </summary>
        SinaWeiBo,
        /// <summary>
        /// ��ѶQQ
        /// </summary>
        QQ,
        /// <summary>
        /// �Ա���
        /// </summary>
        TaoBao,
        /// <summary>
        /// ��������δ֧�֣�
        /// </summary>
        RenRen,
        /// <summary>
        /// ��Ѷ΢����δ֧�֣�
        /// </summary>
        QQWeiBo,
        /// <summary>
        /// ��������δ֧�֣�
        /// </summary>
        KaiXin,
        /// <summary>
        /// ���ţ�δ֧�֣�
        /// </summary>
        FeiXin,
        /// <summary>
        /// 
        /// </summary>
        None,
    }
}
