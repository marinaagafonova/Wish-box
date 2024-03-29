using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wish_Box.Options;
using Microsoft.OpenApi.Models;
using Wish_Box.Models;
using Wish_Box.Repositories;

namespace Wish_Box
{
    public static class ConnectionString
    {
        public static string Value { get; set; }
    }
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private readonly IWebHostEnvironment _currentEnvironment;


        public Startup(IConfiguration configuration, IWebHostEnvironment currentEnvironment)
        {
            Configuration = configuration;
            _currentEnvironment = currentEnvironment;
        }

        public virtual void ConfigureDependencies(IServiceCollection services)
        {

        }

        //This method gets called by the runtime.Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;                    
                });

            services.AddControllersWithViews();
            services.AddSession();

            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            if (_currentEnvironment.IsEnvironment("Testing"))
            {
                services.AddDbContextPool<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestingDB"));
            }
            else
            {
                services.AddDbContext<AppDbContext>(builder =>
                    builder.UseSqlServer(connectionString));
            }
            ConnectionString.Value = Configuration.GetConnectionString("DefaultConnection");
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
            options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });
            services.AddControllersWithViews();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddRazorPages();
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo{ Title = "Wishbox API", Version = "v1"});
            });

            services.AddTransient<IRepository<Following>, FollowingsRepository>();
            services.AddTransient<IRepository<User>, UserRepository>();

            services.AddTransient<IRepository<TakenWish>, TakenWishRepository>();
            services.AddTransient<IRepository<Comment>, CommentRepository>();
            services.AddTransient<IRepository<Wish>, WishRepository>();
            services.AddTransient<IRepository<WishRating>, WishRatingRepository>();
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //app.UseExceptionHandler("/Home/Error");
                //// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

            app.UseSwagger(option =>
            {
                option.RouteTemplate = swaggerOptions.JsonRoute;
            });

            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description);
            });
            //         app.UseSwagger()
            //.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DOA.API V1");
            //});

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //    //endpoints.MapControllers();
            //    endpoints.MapRazorPages();
            //});
            //app.UseMvc(
            //routes =>
            //{
            //    routes.MapRoute("Add", "{controller=Following}/{action=Add}");
            //}
            //);



            app.UseMvc(routes =>
            {
                //routes.MapRoute("api", "api/post", new { controller = "Following", action = "PostFollowing" });
                //routes.MapRoute("following", "following/delete", new { controller = "Following", action = "Remove" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            //app.UseMvcWithDefaultRoute();
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Search}/{id?}");
            //});
        }
    }
}
