using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;


namespace NodeWebApi.Components
{
    public class Utils
    {
        private readonly IConfiguration configuration;
        private const int MAXAGE = 604800;
        public Utils(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        #region private fields
        private IDictionary<string, string> _directoryDic;
        private int? maxCacheAge;
        #endregion

        /// <summary>
        /// where the static file(s) directory is located
        /// </summary>
        /// <value></value>
        internal string StorageDirectory
        {
            get
            {
                _directoryDic ??= configuration.GetSection("FileDirectory").Get<IDictionary<string, string>>();

                return Environment.OSVersion.Platform == PlatformID.Unix ? _directoryDic["Linux"] : _directoryDic["Windows"];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected int CacheMaxAge
        {
            get
            {
                maxCacheAge ??= configuration.GetSection("CacheMaxAge").Get<int?>() ?? MAXAGE;

                return maxCacheAge.Value;
            }
        }
        public StaticFileOptions GetStaticFileOptions()
        {
            //更改默认文件夹 (StaticFileOptions方法)，默认文件夹为wwwroot
            var staticfile = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(this.StorageDirectory), //指定目录，这里指C盘，也可以是其他目录

                //手动设置MIME Type,或者设置一个默认值， 以解决某些文件MIME Type文件识别不到，出现404错误
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/json", //设置默认MIME Type
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={this.CacheMaxAge}");
                },
            };
            return staticfile;
        }
    }
}