using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NodeWebApi.Components;

namespace NodeWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _util = new Utils(Configuration);
        }

        public IConfiguration Configuration { get; }
        private Utils _util;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //目录浏览，即可以访问目录
            if (env.IsDevelopment())
            {
                var dir = new DirectoryBrowserOptions
                {
                    FileProvider = new PhysicalFileProvider(_util.StorageDirectory)
                };
                app.UseDirectoryBrowser(dir);
            }
            // var provider = new FileExtensionContentTypeProvider();//使用一组默认映射创建新的提供程序
            // provider.Mappings.Add(".jpg", "image/jpeg");//手动设置对应MIME Type
            // staticfile.ContentTypeProvider = provider; //将文件映射到内容类型

            app.UseStaticFiles(_util.GetStaticFileOptions());

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync(string.Empty);
            });
        }
    }
}