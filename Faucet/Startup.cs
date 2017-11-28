using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentScheduler;
using stratfaucet.Lib;
using stratfaucet.Jobs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace stratfaucet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static IWalletQueueService WalletQueue;
        public static ConcurrentQueue<string> AddressesSeen;
        public static ConcurrentQueue<string> IPAddressesSeen;

        public static IWalletUtils WalletUtils;

        public void ConfigureServices(IServiceCollection services)
        {

            // TODO switch to scheduler that allows dependency injection in constructor
            WalletQueue = new WalletQueueService();
            services.AddMvc();

            AddressesSeen = new ConcurrentQueue<string>();
            IPAddressesSeen = new ConcurrentQueue<string>();


            WalletUtils = new WalletUtils(Configuration);

            InitJobScheduler();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true

                });
            }
            else
            {
                app.UseExceptionHandler("/Faucet/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });

            });
        }

        private void InitJobScheduler() {
            JobManager.AddJob(() => SendCoinJob.Execute(), s => s
                .ToRunEvery(3)
                .Seconds());
        }
    }
}
