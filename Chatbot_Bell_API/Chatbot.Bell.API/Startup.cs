using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chatbot.Bell.API.DataAccess;
using Chatbot.Bell.API.Model;
using Chatbot.Bell.API.Processor;
using Chatbot.Bell.API.RabbitMQManager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Chatbot.Bell.API
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = AppConfig.KeyIssuer,
                    ValidAudience = AppConfig.Audience_DR,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.SecurityKey))
                };
            });

            services.AddMvc();
            services.AddScoped<ChatbotBellDataAccess>();
            services.AddScoped<ChatbotBellProcessor>();
            services.AddScoped<ShoppingBagProcessor>();
            services.AddScoped<ChatBotBusinessLogic>();
            services.AddScoped<ElasticsearchApiProcessor>();
            services.AddMemoryCache();

            //services.Configure<Sqlqueries>(Configuration);
            services.AddControllers();
            #region AddRabbitMQ
            services.AddRabbit(Configuration);
            #endregion
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Bell Chatbot Integration API",
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //IConfiguration appConfig = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //    //.AddJsonFile("dbSettings.json", optional: false, reloadOnChange: true)
            //    .AddXmlFile("SqlQueries.xml", optional: false, reloadOnChange: true)
            //    .Build();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            });
            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bell Chatbot Integration API"));
        }
    }
}