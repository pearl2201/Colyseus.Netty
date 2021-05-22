using Colyseus.Common;
using Colyseus.Common.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Colyseus.Common.Settings;

namespace Colyseus.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

       

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Colyseus.Server", Version = "v1" });
            });

            var rabbitMqSettings = Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>();
            services.AddMassTransit(x =>
            {
                //x.AddConsumer<UserUpdateConsumerService>();
                //x.AddConsumer<LearningBlockActivatedUpdateConsumerService>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.Port,
                        rabbitMqSettings.Virtual, h =>
                        {
                            h.Username(rabbitMqSettings.Username);
                            h.Password(rabbitMqSettings.Password);
                        });


                    cfg.ReceiveEndpoint(rabbitMqSettings.Endpoint, e =>
                    {
                        //e.ConfigureConsumer<UserUpdateConsumerService>(context);
                        //e.ConfigureConsumer<LearningBlockActivatedUpdateConsumerService>(context);
                    });
                });
            });
            services.AddMassTransitHostedService();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Colyseus.Server v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

  
       

      
    }
}
