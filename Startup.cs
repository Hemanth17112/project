using CMDWebAPI.Services;
using CMDDALayer;
using CMDEFLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CMDWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddScoped<IAppointmentRepository<AppointmentDetail>, AppointmentDetailRepository>();
            services.AddScoped<IAppointmentRepository<Comment>, CommentRepository>();
            services.AddScoped<IAppointmentRepository<Doctor>, DoctorRepository>();
            services.AddScoped<IAppointmentRepository<Patient>, PatientRepository>();
            services.AddScoped<IAppointmentRepository<Prescription>, PrescriptionRepository>();
            services.AddScoped<IAppointmentRepository<Recommendation>, RecommendationRepository>();
            services.AddScoped<IAppointmentRepository<Test>, TestRepository>();
            services.AddScoped<IAppointmentRepository<Vitals>, VitalsRepository>();

            services.AddTransient<ISenderService, SenderService>();

            services.AddDbContext<CMDDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CMDDbConn"),
                providerOptions => providerOptions.EnableRetryOnFailure()));

            services.AddControllers().AddJsonOptions(o => {
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CMDProjectApp", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CMDProjectApp v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
